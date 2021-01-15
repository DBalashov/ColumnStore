using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ColumnStore
{
    static class EntityExtenders
    {
        static readonly ArrayPool<byte> poolBytes = ArrayPool<byte>.Shared;
        static readonly ArrayPool<int>  poolInts  = ArrayPool<int>.Shared;

        [NotNull]
        internal static byte[] Pack<E, V>([NotNull] this KeyValue<E>[] newData, [NotNull] PropertyInfo prop, bool withCompression)
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

            var buff = poolBytes.Rent(newData.Length * 4);
            Buffer.BlockCopy(storedKeys, 0, buff, 0, newData.Length * 4);
            stm.Write(buff, 0, newData.Length * 4);
            poolInts.Return(storedKeys);
            poolBytes.Return(buff);

            storedValues.PackData(stm, new Range(0, newData.Length));
            return stm.ToArray();
        }

        [NotNull]
        internal static Dictionary<int, V> MergeWithReplace<E, V>([NotNull] this Dictionary<int, V> existing,
                                                                  [NotNull]      KeyValue<E>[]      newData,
                                                                  [NotNull]      PropertyInfo       prop)
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
}