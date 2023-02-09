using System;
using System.IO;

namespace ColumnStore;

sealed class ReadWriteHandlerUInt16 : IntBase
{
    public override void Pack(Array values, Stream targetStream, Range range) =>
        packIntXX(values, targetStream, range, values.Dictionarize<ushort>(range), 2);

    public override Array Unpack(Span<byte> buff, int count) =>
        unpackIntXX(buff, count, poolUShorts, 2);
}