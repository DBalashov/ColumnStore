using System;
using System.IO;
using System.Text;

namespace ColumnStore
{
    // todo nullable bitmap
    class ReadWriteHandlerString : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            using var bw = new BinaryWriter(targetStream, Encoding.Default, true);

            var r = values.Dictionarize<string>(range);

            // write dictionary values
            bw.Write((ushort) r.Values.Length);
            foreach (var item in r.Values)
            {
                var buff = Encoding.UTF8.GetBytes(item); //!!! -> byte[]
                bw.Write((byte) buff.Length);
                bw.Write(buff, 0, buff.Length);
            }

            // write value indexes
            var compactType = r.Values.Length.GetCompactType();
            bw.Write((byte) compactType);

            var requireBytes = range.Length * (1 << (int) compactType);
            var buffIndexes  = poolBytes.Rent(requireBytes);
            CompactValues(r.Indexes, buffIndexes, 0, compactType);
            bw.Write(buffIndexes, 0, requireBytes);
            poolBytes.Return(buffIndexes);
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            // read dictionary values
            var dictionaryValuesCount = BitConverter.ToUInt16(buff, offset);
            offset += 2;

            var dictionaryValues = new string[dictionaryValuesCount];
            for (var i = 0; i < dictionaryValuesCount; i++)
            {
                var length = buff[offset];
                dictionaryValues[i] =  Encoding.UTF8.GetString(buff, offset + 1, length);
                offset              += length + 1;
            }

            // read value indexes
            var compactType = (CompactType) buff[offset++];
            var indexes     = UncompactValues(buff, offset, count, compactType);

            var values = new string[count];
            for (var i = 0; i < count; i++)
                values[i] = dictionaryValues[indexes[i]];

            return values;
        }
    }
}