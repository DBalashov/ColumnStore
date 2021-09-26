using System;
using System.Buffers;
using System.IO;

namespace ColumnStore
{
    static class ColumnUntypedExtenders
    {
        static readonly ArrayPool<byte> poolBytes = ArrayPool<byte>.Shared;

        internal static byte[] Pack( this UntypedColumn data, Range range = null, bool withCompression = false)
        {
            var stmTarget = new MemoryStream();
            range ??= new Range(0, data.Keys.Length);

            stmTarget.Write(BitConverter.GetBytes(range.Length), 0, 4);
            stmTarget.Write(BitConverter.GetBytes((int) data.Values.DetectDataType()), 0, 4);

            using var stm = new WriteStreamWrapper(stmTarget, withCompression);

            var buff = poolBytes.Rent(range.Length * 4);
            data.Keys.PackStructs(range.From*4, buff, 0, range.Length);
            stm.Write(buff, 0, range.Length * 4);
            poolBytes.Return(buff);

            data.Values.PackData(stm, range);

            return stm.ToArray();
        }

        internal static UntypedColumn Unpack( this byte[] buff, bool withDecompression)
        {
            var count  = BitConverter.ToInt32(buff, 0);
            var offset = 4;

            var dataType = BitConverter.ToInt32(buff, 4);
            offset += 4;

            if (withDecompression)
            {
                buff   = buff.GZipUnpack(offset);
                offset = 0;
            }

            var keys = buff.UnpackStructs<CDT>(offset, count);
            offset += count * 4;

            var values = buff.UnpackData(offset);
            return new UntypedColumn(keys, values);
        }

        internal static UntypedColumn MergeWithReplace( this UntypedColumn existing,  Range range,  UntypedColumn newData)
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
            var newValues  = newData.Values.CreateSameType(newLength);

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