using System;
using System.Buffers;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

abstract class IntBase : ReadWriteBase
{
    protected void packIntXX<T>(Array values, IVirtualWriteStream targetStream, Range range, DictionarizeResult<T> r, int elementSize) where T : struct
    {
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

    protected Array unpackIntXX<T>(Span<byte> span, int count, ArrayPool<T> pool, int elementSize) where T : struct
    {
        var dictionaryValuesCount = span.Read<ushort>();

        var dictionaryValues = pool.Rent(dictionaryValuesCount);
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

        pool.Return(dictionaryValues);

        return values;
    }
}