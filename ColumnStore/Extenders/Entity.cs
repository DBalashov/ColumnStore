using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ColumnStore;

static class EntityExtenders
{
    static readonly ArrayPool<int> poolInts = ArrayPool<int>.Shared;

    internal static byte[] Pack<E, V>(this KeyValue<E>[] newData, PropertyInfo prop, bool withCompression)
    {
        using var stm = new WriteStreamWrapper(new MemoryStream(), withCompression);

        stm.Write(BitConverter.GetBytes(newData.Length), 0, 4);

        var getValue     = prop.getActionGet<E, V>();
        var storedKeys   = poolInts.Rent(newData.Length);
        var storedValues = new V[newData.Length];

        int index = 0;
        foreach (var item in newData)
        {
            storedKeys[index]     = item.Key;
            storedValues[index++] = getValue(item.Value);
        }

        stm.Write(MemoryMarshal.Cast<int, byte>(storedKeys.AsSpan(0, newData.Length)));
        poolInts.Return(storedKeys);

        storedValues.PackData(stm, new Range(0, newData.Length));
        return stm.ToArray();
    }

    internal static Dictionary<int, V> MergeWithReplace<E, V>(this Dictionary<int, V> existing,
                                                              KeyValue<E>[]           newData,
                                                              PropertyInfo            prop)
    {
        var range = new CDTRange(new CDT(newData.First().Key), new CDT(newData.Last().Key));

        var r = new Dictionary<int, V>();

        foreach (var value in existing)
            if (!range.InRange(value.Key))
                r.Add(value.Key, value.Value);

        var getValue = prop.getActionGet<E, V>();
        foreach (var newValue in newData)
        {
            var key = newValue.Key;

            var value = getValue(newValue.Value);
            if (!r.ContainsKey(key))
                r.Add(key, value);
            else r[key] = value;
        }

        return r;
    }
}