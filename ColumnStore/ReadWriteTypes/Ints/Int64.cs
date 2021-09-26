using System;
using System.IO;

namespace ColumnStore
{
    sealed class ReadWriteHandlerInt64 : IntBase
    {
        public override void Pack(Array values, Stream targetStream, Range range) =>
            packIntXX(values, targetStream, range, values.Dictionarize<Int64>(range, 0), 8);

        public override Array Unpack(Span<byte> buff, int count) =>
            unpackIntXX(buff, count, poolInt64s, 8);
    }
}