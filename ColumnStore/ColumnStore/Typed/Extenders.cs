using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

static class ColumnTypedExtenders
{
    internal static byte[] Pack<V>(this Dictionary<int, V> values, bool withCompression)
    {
        using var stm = withCompression ? (IVirtualWriteStream) new StreamCompress(new MemoryStream()) : new StreamRaw(new MemoryStream());

        stm.Write(BitConverter.GetBytes(values.Count).AsSpan());

        var storedKeys   = ArrayPool<int>.Shared.Rent(values.Count);
        var storedValues = ArrayPool<V>.Shared.Rent(values.Count);

        var offset = 0;
        foreach (var v in values)
        {
            storedKeys[offset]   = v.Key;
            storedValues[offset] = v.Value;
            offset++;
        }

        stm.Write(MemoryMarshal.Cast<int, byte>(storedKeys.AsSpan(0, values.Count)));
        ArrayPool<int>.Shared.Return(storedKeys);

        storedValues.PackData(stm, new Range(0, values.Count));

        ArrayPool<V>.Shared.Return(storedValues);

        return stm.GetBytes();
    }

    internal static Dictionary<int, V> Unpack<V>(this byte[] buff, bool withDecompression)
    {
        var span = (withDecompression ? new StreamDecompress(buff).GetBytes() : buff).AsSpan();

        var count = span.Read<int>();
        var keys  = span.Read<int>(count);

        var values = (V[]) span.UnpackData();
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
            r[newValue.Key] = newValue.Value;

        return r;
    }
}