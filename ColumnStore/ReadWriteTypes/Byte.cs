using System;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerByte : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range) => targetStream.Write(((byte[])values).AsSpan(range));

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var values = new byte[count];
            Buffer.BlockCopy(buff, offset, values, 0, count);
            return values;
        }
    }
}