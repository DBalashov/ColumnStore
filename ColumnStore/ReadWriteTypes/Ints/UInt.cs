using System;
using System.IO;

namespace ColumnStore;

sealed class ReadWriteHandlerUInt : IntBase
{
    public override void Pack(Array values, Stream targetStream, Range range) =>
        packIntXX(values, targetStream, range, values.Dictionarize<uint>(range), 4);

    public override Array Unpack(Span<byte> buff, int count) =>
        unpackIntXX(buff, count, poolUInts, 4);
}