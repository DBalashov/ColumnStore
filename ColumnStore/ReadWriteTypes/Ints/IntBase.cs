using System;
using System.Buffers;

namespace ColumnStore;

abstract class IntBase : ReadWriteBase
{
    protected void packIntXX<T>(Array values, IVirtualWriteStream targetStream, Range range, DictionarizeResult<T> r, int elementSize)
    {
        var compactType = r.Values.Length.GetCompactType();
        if (compactType <= CompactType.Short)
        {
            var requireBytes = 2 + r.Values.Length * elementSize +
                               1 + range.Length()  * (1 << (int) compactType);

            var buff   = poolBytes.Rent(requireBytes);
            var offset = 0;

            // write dictionary values
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) r.Values.Length), 0, buff, offset, 2);
            offset += 2;

            Buffer.BlockCopy(r.Values, 0, buff, offset, r.Values.Length * elementSize);
            offset += r.Values.Length * elementSize;

            // write value indexes
            buff[offset++] = (byte) compactType;
            r.Indexes.CompactValues(buff.AsSpan(offset, requireBytes - offset), compactType);
            targetStream.Write(buff.AsSpan(0,           requireBytes));
            poolBytes.Return(buff);
        }
        else
        {
            var requireBytes = 1 + elementSize * range.Length();
            var buff         = poolBytes.Rent(requireBytes);

            int offset = 0;
            buff[offset++] = (byte) compactType;

            // write values
            Buffer.BlockCopy(values, range.Start.Value * elementSize, buff, offset, range.Length() * elementSize);
            targetStream.Write(buff.AsSpan(0, requireBytes));
            poolBytes.Return(buff);
        }
    }

    protected Array unpackIntXX<T>(Span<byte> buff, int count, ArrayPool<T> pool, int elementSize)
    {
        var dictionaryValuesCount = BitConverter.ToUInt16(buff);
        var offset                = 2;

        var dictionaryValues = pool.Rent(dictionaryValuesCount);
        Buffer.BlockCopy(buff.Slice(offset, dictionaryValuesCount * elementSize).ToArray(), 0,
                         dictionaryValues, 0, dictionaryValuesCount * elementSize);
        offset += dictionaryValuesCount * elementSize;

        var compactType = (CompactType) buff[offset++];
        var values      = new T[count];

        if (compactType <= CompactType.Short)
        {
            var indexes = buff.Slice(offset).UncompactValues(count, compactType);
            for (var i = 0; i < indexes.Length; i++)
                values[i] = dictionaryValues[indexes[i]];
        }
        else
        {
            Buffer.BlockCopy(buff.Slice(offset, count * elementSize).ToArray(), 0,
                             values, 0, count * elementSize);
        }

        pool.Return(dictionaryValues);

        return values;
    }
}