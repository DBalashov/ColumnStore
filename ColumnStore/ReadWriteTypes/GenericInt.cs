using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

class ReadWriteHandlerGenericInt<T> : ReadWriteBase where T : struct
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var elementSize = Unsafe.SizeOf<T>();
        var r           = values.Dictionarize<T>(range);

        var compactType = r.Values.Length.GetCompactType();
        if (compactType <= CompactType.Short)
        {
            var requireBytes = 2 + r.Values.Length * elementSize +
                               1 + range.Length()  * (1 << (int) compactType);

            var buff = poolBytes.Rent(requireBytes);
            var span = buff.AsSpan();

            span.Write((ushort) r.Values.Length);
            span.Write<T>(r.Values);
            span.Write((byte) compactType);

            r.Indexes.CompactValues(span, compactType);
            targetStream.Write(buff.AsSpan(0, requireBytes));
            poolBytes.Return(buff);
        }
        else
        {
            var requireBytes = 1 + elementSize * range.Length();
            var buff         = poolBytes.Rent(requireBytes);
            var span         = buff.AsSpan();

            span.Write((byte) compactType);

            Buffer.BlockCopy(values, range.Start.Value * elementSize, buff, 1, range.Length() * elementSize);
            targetStream.Write(buff.AsSpan(0, requireBytes));

            poolBytes.Return(buff);
        }
    }

    public override Array Unpack(Span<byte> span, int count)
    {
        var elementSize = Unsafe.SizeOf<T>();

        var dictionaryValuesCount = span.Read<ushort>();

        var dictionaryValues = ArrayPool<T>.Shared.Rent(dictionaryValuesCount);
        var length           = dictionaryValuesCount * elementSize;
        span.Slice(0, length).CopyTo(MemoryMarshal.Cast<T, byte>(dictionaryValues.AsSpan(0, dictionaryValuesCount)));

        span = span.Slice(length);

        var compactType = (CompactType) span.Read<byte>();
        var values      = new T[count];

        if (compactType <= CompactType.Short)
        {
            var indexes = span.UncompactValues(count, compactType);
            for (var i = 0; i < indexes.Length; i++)
                values[i] = dictionaryValues[indexes[i]];
        }
        else
        {
            span.Slice(0, count * elementSize).CopyTo(MemoryMarshal.Cast<T, byte>(values.AsSpan(0, count)));
        }

        ArrayPool<T>.Shared.Return(dictionaryValues);

        return values;
    }
}