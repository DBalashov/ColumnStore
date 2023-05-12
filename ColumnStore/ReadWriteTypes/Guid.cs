using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

// todo nullable bitmap
sealed class ReadWriteHandlerGuid : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var r = values.Dictionarize<Guid>(range);

        var type = r.Values.Length.GetCompactType();
        var requireBytes = 2 + 16             * r.Values.Length +
                           1 + range.Length() * (1 << (int) type);

        var buff = poolBytes.Rent(requireBytes);
        var span = buff.AsSpan(0, requireBytes);

        BitConverter.TryWriteBytes(span, (ushort) r.Values.Length);
        var spanValues = MemoryMarshal.Cast<Guid, byte>(r.Values);
        spanValues.CopyTo(span.Slice(2));

        span    = span.Slice(2 + spanValues.Length);
        span[0] = (byte) type;

        r.Indexes.CompactValues(span.Slice(1), type);

        targetStream.Write(buff.AsSpan(0, requireBytes));
        poolBytes.Return(buff);
    }

    public override Array Unpack(Span<byte> span, int count)
    {
        var dictionaryValuesCount = span.ReadUInt16();
        var dictionaryValues      = span.ReadGuids(dictionaryValuesCount);

        var values      = new Guid[count];
        var compactType = (CompactType) span.ReadByte();

        var indexes = span.UncompactValues(count, compactType);
        for (var i = 0; i < indexes.Length; i++)
            values[i] = dictionaryValues[indexes[i]];
        return values;
    }
}