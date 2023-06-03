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

        span.Write((ushort) r.Values.Length);
        MemoryMarshal.Cast<Guid, byte>(r.Values).CopyTo(span);
        span = span.Slice(16 * r.Values.Length);

        span.Write((byte) type);
        r.Indexes.CompactValues(span, type);

        targetStream.Write(buff.AsSpan(0, requireBytes));
        poolBytes.Return(buff);
    }

    public override Array Unpack(Span<byte> span, int count)
    {
        var dictionaryValuesCount = span.Read<ushort>();
        var dictionaryValues      = span.Read<Guid>(dictionaryValuesCount);

        var values      = new Guid[count];
        var compactType = (CompactType) span.Read<byte>();

        var indexes = span.UncompactValues(count, compactType);
        for (var i = 0; i < indexes.Length; i++)
            values[i] = dictionaryValues[indexes[i]];
        return values;
    }
}