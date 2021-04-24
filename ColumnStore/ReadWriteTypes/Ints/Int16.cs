using System;
using System.Buffers;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerInt16 : IntBase
    {
        public override void Pack(Array values, Stream targetStream, Range range) =>
            packIntXX(values, targetStream, range, values.Dictionarize<short>(range), 2);

        public override Array Unpack(byte[] buff, int count, int offset) =>
            unpackIntXX(buff, count, offset, poolShorts, 2);
    }
}