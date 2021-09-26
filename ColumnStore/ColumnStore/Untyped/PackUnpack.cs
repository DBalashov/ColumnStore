using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ColumnStore
{
    static class ColumnUntypedExtenders
    {
        internal static byte[] Pack(this UntypedColumn data, Range? range = null, bool withCompression = false)
        {
            var stmTarget = new MemoryStream();
            var r         = range ?? new Range(0, data.Keys.Length);

            stmTarget.Write(BitConverter.GetBytes(r.Length()), 0, 4);
            stmTarget.Write(BitConverter.GetBytes((int)data.Values.DetectDataType()), 0, 4);

            using var stm = new WriteStreamWrapper(stmTarget, withCompression);
            stm.Write(MemoryMarshal.Cast<CDT, byte>(data.Keys.AsSpan(r)));
            data.Values.PackData(stm, r);

            return stm.ToArray();
        }

        internal static UntypedColumn Unpack(this byte[] buff, bool withDecompression)
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

            var span = buff.AsSpan();
            var keys = MemoryMarshal.Cast<byte, CDT>(span.Slice(offset, count * 4)).ToArray();
            offset += count * 4;

            var values = span.Slice(offset).UnpackData();
            return new UntypedColumn(keys, values);
        }

        internal static UntypedColumn MergeWithReplace(this UntypedColumn existing, Range range, UntypedColumn newData)
        {
            var sd = newData.Keys[range.Start.Value];
            var ed = newData.Keys[range.End.Value - 1];

            var firstPartIndexTo = Array.BinarySearch(existing.Keys, sd);
            if (firstPartIndexTo < 0)
                firstPartIndexTo = ~firstPartIndexTo;

            var secondPartIndexFrom = Array.BinarySearch(existing.Keys, ed);
            if (secondPartIndexFrom < 0)
                secondPartIndexFrom = ~secondPartIndexFrom;

            var restLength = existing.Keys.Length - secondPartIndexFrom - 1;
            var newLength  = firstPartIndexTo + range.Length() + restLength;
            var newKeys    = new CDT[newLength];
            var newValues  = newData.Values.CreateSameType(newLength);

            var offset = 0;

            Array.Copy(existing.Keys, offset, newKeys, 0, firstPartIndexTo);
            Array.Copy(existing.Values, offset, newValues, 0, firstPartIndexTo);
            offset += firstPartIndexTo;

            Array.Copy(newData.Keys, range.Start.Value, newKeys, offset, range.Length());
            Array.Copy(newData.Values, range.Start.Value, newValues, offset, range.Length());
            offset += range.Length();

            Array.Copy(newData.Keys, secondPartIndexFrom, newKeys, offset, restLength);
            Array.Copy(newData.Values, secondPartIndexFrom, newValues, offset, restLength);

            return new UntypedColumn(newKeys, newValues);
        }
    }
}