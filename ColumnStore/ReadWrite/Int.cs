using System;
using System.Buffers;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerInt : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var r           = values.Dictionarize<int>(range);
            var compactType = r.Values.Length.GetCompactType();

            if (compactType <= CompactType.Short)
            {
                var requireBytes = 2 + r.Values.Length * 4 +
                                   1 + range.Length * (1 << (int) compactType);

                var buff   = poolBytes.Rent(requireBytes);
                var offset = 0;

                // write dictionary values
                Buffer.BlockCopy(BitConverter.GetBytes((ushort) r.Values.Length), 0, buff, offset, 2);
                offset += 2;

                Buffer.BlockCopy(r.Values, 0, buff, offset, r.Values.Length * 4);
                offset += r.Values.Length * 4;

                // write value indexes
                buff[offset++] = (byte) compactType;
                CompactValues(r.Indexes, buff, offset, compactType);
                targetStream.Write(buff, 0, requireBytes);
                poolBytes.Return(buff);
            }
            else
            {
                var requireBytes = 1 + 4 * range.Length;
                var buff         = poolBytes.Rent(requireBytes);

                int offset = 0;
                buff[offset++] = (byte) compactType;

                // write values
                Buffer.BlockCopy(values, range.From * 4, buff, offset, range.Length * 4);
                targetStream.Write(buff, 0, requireBytes);
                poolBytes.Return(buff);
            }
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var dictionaryValuesCount = BitConverter.ToUInt16(buff, offset);
            offset += 2;

            var dictionaryValues = poolInts.Rent(dictionaryValuesCount);
            Buffer.BlockCopy(buff, offset, dictionaryValues, 0, dictionaryValuesCount * 4);
            offset += dictionaryValuesCount * 4;

            var compactType = (CompactType) buff[offset++];
            var values      = new int[count];

            if (compactType <= CompactType.Short)
            {
                var indexes = UncompactValues(buff, offset, count, compactType);
                for (var i = 0; i < indexes.Length; i++)
                    values[i] = dictionaryValues[indexes[i]];
            }
            else
            {
                Buffer.BlockCopy(buff, offset, values, 0, count * 4);
            }

            poolInts.Return(dictionaryValues);

            return values;
        }
    }
}