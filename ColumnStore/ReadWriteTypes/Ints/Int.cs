using System;
using System.IO;

namespace ColumnStore
{
    sealed class ReadWriteHandlerInt : IntBase
    {
        public override void Pack(Array values, Stream targetStream, Range range) =>
            packIntXX(values, targetStream, range, values.Dictionarize<int>(range), 4);

        public override Array Unpack(Span<byte> buff, int count) =>
            unpackIntXX(buff, count, poolInts, 4);
    }
}