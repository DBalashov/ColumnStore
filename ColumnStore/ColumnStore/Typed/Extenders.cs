using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    static class ColumnTypedExtenders
    {
        static readonly ArrayPool<byte> poolBytes = ArrayPool<byte>.Shared;
        static readonly ArrayPool<int>  poolInts  = ArrayPool<int>.Shared;

        [NotNull]
        internal static byte[] Pack<V>([NotNull] this Dictionary<int, V> values, bool withCompression)
        {
            using var stm = new WriteStreamWrapper(new MemoryStream(), withCompression);

            stm.Write(BitConverter.GetBytes(values.Count), 0, 4);

            var storedKeys   = poolInts.Rent(values.Count);
            var storedValues = new V[values.Count];

            var offset = 0;
            foreach (var v in values)
            {
                storedKeys[offset]   = v.Key;
                storedValues[offset] = v.Value;
                offset++;
            }

            var buff = poolBytes.Rent(values.Count * 4);
            Buffer.BlockCopy(storedKeys, 0, buff, 0, values.Count * 4);
            stm.Write(buff, 0, values.Count * 4);
            poolBytes.Return(buff);
            poolInts.Return(storedKeys);

            storedValues.PackData(stm, new Range(0, storedValues.Length));

            return stm.ToArray();
        }

        [NotNull]
        internal static Dictionary<int, V> Unpack<V>([NotNull] this byte[] buff, bool withDecompression)
        {
            if (withDecompression)
                buff = buff.GZipUnpack();

            var count  = BitConverter.ToInt32(buff, 0);
            var offset = 4;

            var keys = poolInts.Rent(count);
            Buffer.BlockCopy(buff, offset, keys, 0, count * 4);
            offset += count * 4;

            var dataBuff = poolBytes.Rent(buff.Length - offset);
            Buffer.BlockCopy(buff, offset, dataBuff, 0, buff.Length - offset);
            var values = (V[]) dataBuff.UnpackData();
            poolBytes.Return(dataBuff);

            var r = new Dictionary<int, V>(count);
            for (var i = 0; i < count; i++)
                r.Add(keys[i], values[i]);

            poolInts.Return(keys);

            return r;
        }

        [NotNull]
        internal static Dictionary<int, V> MergeWithReplace<V>([NotNull] this Dictionary<int, V> existing, [NotNull] KeyValue<V>[] newData)
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
}