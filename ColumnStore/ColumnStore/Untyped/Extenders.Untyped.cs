using System;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    static class ColumnUntypedExtenders
    {
        [NotNull]
        internal static byte[] Pack([NotNull] this UntypedColumn data, Range range = null, bool withCompression = false)
        {
            using var stm = new WriteStreamWrapper(withCompression);
            range ??= new Range(0, data.Keys.Length);

            stm.Write(BitConverter.GetBytes(range.Length), 0, 4);

            var buff = data.Keys.PackStructs(range.From * 4, range.Length);
            stm.Write(buff, 0, range.Length * 4);

            data.Values.PackData(stm, range);

            return stm.ToArray();
        }

        [NotNull]
        internal static UntypedColumn Unpack([NotNull] this byte[] buff, bool withDecompression)
        {
            if (withDecompression)
                buff = buff.GZipUnpack();

            var count  = BitConverter.ToInt32(buff, 0);
            var offset = 4;

            var keys = buff.UnpackStructs<CDT>(offset, count);
            offset += count * 4;

            var values = buff.UnpackData(offset);
            return new UntypedColumn(keys, values);
        }
        
        [NotNull]
        internal static UntypedColumn MergeWithReplace([NotNull] this UntypedColumn existing, [NotNull] Range range, [NotNull] UntypedColumn newData)
        {
            var sd = newData.Keys[range.From];
            var ed = newData.Keys[range.To - 1];

            var firstPartIndexTo = Array.BinarySearch(existing.Keys, sd);
            if (firstPartIndexTo < 0)
                firstPartIndexTo = ~firstPartIndexTo;

            var secondPartIndexFrom = Array.BinarySearch(existing.Keys, ed);
            if (secondPartIndexFrom < 0)
                secondPartIndexFrom = ~secondPartIndexFrom;

            var restLength = existing.Keys.Length - secondPartIndexFrom - 1;
            var newLength  = firstPartIndexTo + range.Length + restLength;
            var newKeys    = new CDT[newLength];
            var newValues  = Array.CreateInstance(newData.Values.GetValue(0).GetType(), newLength);

            var offset = 0;

            Array.Copy(existing.Keys, offset, newKeys, 0, firstPartIndexTo);
            Array.Copy(existing.Values, offset, newValues, 0, firstPartIndexTo);
            offset += firstPartIndexTo;

            Array.Copy(newData.Keys, range.From, newKeys, offset, range.Length);
            Array.Copy(newData.Values, range.From, newValues, offset, range.Length);
            offset += range.Length;

            Array.Copy(newData.Keys, secondPartIndexFrom, newKeys, offset, restLength);
            Array.Copy(newData.Values, secondPartIndexFrom, newValues, offset, restLength);

            return new UntypedColumn(newKeys, newValues);
        }
    }
}