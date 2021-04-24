using System;
using System.IO;

namespace ColumnStore
{
    // todo nullable bitmap
    class ReadWriteHandlerGuid : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var r = values.Dictionarize<Guid>(range);

            var type = r.Values.Length.GetCompactType();
            var requireBytes = 2 + 16 * r.Values.Length +
                               1 + range.Length * (1 << (int) type);
            var buff = poolBytes.Rent(requireBytes);

            // write dictionary values
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) r.Values.Length), 0, buff, 0, 2);
            var offset = 2;

            r.Values.PackStructs(0, buff, offset);
            offset += r.Values.Length * 16;

            // write value indexes
            buff[offset++] = (byte) type;
            r.Indexes.CompactValues(buff, offset, type);

            targetStream.Write(buff, 0, requireBytes);
            poolBytes.Return(buff);
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var dictionaryValuesCount = BitConverter.ToUInt16(buff, offset);
            offset += 2;

            var dictionaryValues = buff.UnpackStructs<Guid>(offset, dictionaryValuesCount);
            offset += dictionaryValuesCount * 16;

            var values      = new Guid[count];
            var compactType = (CompactType) buff[offset++];

            var indexes = buff.UncompactValues(offset, count, compactType);
            for (var i = 0; i < indexes.Length; i++)
                values[i] = dictionaryValues[indexes[i]];
            return values;
        }
    }
}