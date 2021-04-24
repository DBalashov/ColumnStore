using System;
using System.Buffers;
using System.IO;
using JetBrains.Annotations;

namespace ColumnStore
{
    class ReadWriteHandlerInt : IntBase
    {
        public override void Pack(Array values, Stream targetStream, Range range) =>
            packIntXX(values, targetStream, range, values.Dictionarize<int>(range), 4);

        public override Array Unpack(byte[] buff, int count, int offset) =>
            unpackIntXX(buff, count, offset, poolInts, 4);
    }
}