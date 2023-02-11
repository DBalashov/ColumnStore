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

    internal static byte[] Pack<K, V>(this KeyValue<K>[] newData, PropertyInfo prop, bool withCompression)
    {
        using var stm = withCompression ? (IVirtualWriteStream) new StreamCompress(new MemoryStream()) : new StreamRaw(new MemoryStream());

        stm.Write(BitConverter.GetBytes(newData.Length).AsSpan());

        var getValue     = prop.getActionGet<K, V>();
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
        return stm.GetBytes();
    }

    internal static Dictionary<int, V> MergeWithReplace<K, V>(this Dictionary<int, V> existing,
                                                              KeyValue<K>[]           newData,
                                                              PropertyInfo            prop)
    {
        var range = new CDTRange(new CDT(newData.First().Key), new CDT(newData.Last().Key));

        var r = new Dictionary<int, V>();

        foreach (var value in existing)
            if (!range.InRange(value.Key))
                r.Add(value.Key, value.Value);

        var getValue = prop.getActionGet<K, V>();
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