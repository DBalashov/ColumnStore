using System;
using System.Buffers;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerInt64 : IntBase
    {
        public override void Pack(Array values, Stream targetStream, Range range) =>
            packIntXX(values, targetStream, range, values.Dictionarize<Int64>(range, 0), 8);

        public override Array Unpack(byte[] buff, int count, int offset) =>
            unpackIntXX(buff, count, offset, poolInt64s, 8);
    }
}