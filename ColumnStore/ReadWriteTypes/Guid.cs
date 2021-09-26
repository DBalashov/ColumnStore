using System;
using System.IO;
using System.Runtime.InteropServices;

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
                               1 + range.Length() * (1 << (int)type);

            var buff = poolBytes.Rent(requireBytes);
            var span = buff.AsSpan();

            BitConverter.TryWriteBytes(span, (ushort)r.Values.Length);
            var spanValues = MemoryMarshal.Cast<Guid, byte>(r.Values);
            spanValues.CopyTo(span.Slice(2));

            span    = span.Slice(2 + spanValues.Length);
            span[0] = (byte)type;

            r.Indexes.CompactValues(span.Slice(1), type);

            targetStream.Write(buff, 0, requireBytes);
            poolBytes.Return(buff);
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var span                  = buff.AsSpan(offset);
            var dictionaryValuesCount = BitConverter.ToUInt16(span);

            var dictionaryValues = MemoryMarshal.Cast<byte, Guid>(span.Slice(2)).Slice(0, dictionaryValuesCount);
            span = span.Slice(2 + dictionaryValuesCount * 16);

            var values      = new Guid[count];
            var compactType = (CompactType)span[0];

            var indexes = span.Slice(1).UncompactValues(count, compactType);
            for (var i = 0; i < indexes.Length; i++)
                values[i] = dictionaryValues[indexes[i]];
            return values;
        }
    }
}