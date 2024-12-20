using System;

namespace ColumnStore;

sealed class ReadWriteHandlerBoolean : ReadWriteBase
{
    static readonly byte[] byteMasks =
    {
        0b1111_1110,
        0b1111_1101,
        0b1111_1011,
        0b1111_0111,
        0b1110_1111,
        0b1101_1111,
        0b1011_1111,
        0b0111_1111,
    };

    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var byteCount = range.Length() / 8 + (range.Length() % 8 > 0 ? 1 : 0);
        var buff      = poolBytes.Rent(byteCount);
        var bools     = (bool[]) values;

        for (int i = range.Start.Value, index = 0; i < range.End.Value; i++, index++)
        {
            var byteIndex = index >> 3; // /8
            var bitIndex  = index % 8;

            var byteValue = (byte) (buff[byteIndex] & byteMasks[bitIndex]);

            if (bools[i])
                byteValue |= (byte) (1 << bitIndex);

            buff[byteIndex] = byteValue;
        }

        targetStream.Write(buff.AsSpan(0, byteCount));

        poolBytes.Return(buff);
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var values = new bool[count];

        for (var i = 0; i < count; i++)
            values[i] = (buff[(i >> 3)] & (1 << (i % 8))) > 0;

        return values;
    }
}