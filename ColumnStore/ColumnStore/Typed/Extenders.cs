using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColumnStore;

static class ColumnTypedExtenders
{
    static readonly ArrayPool<int> poolInts = ArrayPool<int>.Shared;

    internal static byte[] Pack<V>(this Dictionary<int, V> values, bool withCompression)
    {
        using var stm = new WriteStreamWrapper(new MemoryStream(), withCompression);

        stm.Write(BitConverter.GetBytes(values.Count), 0, 4);

        var storedKeys   = poolInts.Rent(values.Count);
        var storedValues = ArrayPool<V>.Shared.Rent(values.Count);

        var offset = 0;
        foreach (var v in values)
        {
            storedKeys[offset]   = v.Key;
            storedValues[offset] = v.Value;
            offset++;
        }

        stm.Write(MemoryMarshal.Cast<int, byte>(storedKeys.AsSpan(0, values.Count)));
        poolInts.Return(storedKeys);

        storedValues.PackData(stm, new Range(0, storedValues.Length));

        ArrayPool<V>.Shared.Return(storedValues);
        
        return stm.ToArray();
    }

    internal static Dictionary<int, V> Unpack<V>(this byte[] buff, bool withDecompression)
    {
        if (withDecompression)
            buff = buff.GZipUnpack();

        var count  = BitConverter.ToInt32(buff, 0);
        var offset = 4;

        var keys = MemoryMarshal.Cast<byte, int>(buff.AsSpan(offset, count * 4));
        offset += count * 4;

        var values = (V[]) buff.AsSpan(offset, buff.Length - offset).UnpackData();
        var r      = new Dictionary<int, V>(count);
        for (var i = 0; i < count; i++)
            r.Add(keys[i], values[i]);

        return r;
    }

    internal static Dictionary<int, V> MergeWithReplace<V>(this Dictionary<int, V> existing, KeyValue<V>[] newData)
    {
        var range = new CDTRange(new CDT(newData.First().Key), new CDT(newData.Last().Key));
        var r     = new Dictionary<int, V>();

        foreach (var value in existing.Where(p => !range.InRange(p.Key)))
            r.Add(value.Key, value.Value);

        foreach (var newValue in newData)
        {
            var key = newValue.Key;
            if (!r.ContainsKey(key))
                r.Add(key, newValue.Value);
            else r[key] = newValue.Value;
        }

        return r;
    }
}