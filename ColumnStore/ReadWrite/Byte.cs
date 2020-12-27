using System;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerByte : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var buff   = poolBytes.Rent(range.Length);
            Buffer.BlockCopy(values, range.From, buff, 0, range.Length);
            targetStream.Write(buff, 0, range.Length);
            poolBytes.Return(buff);
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var values = new byte[count];
            Buffer.BlockCopy(buff, offset, values, 0, count);
            return values;
        }
    }
}