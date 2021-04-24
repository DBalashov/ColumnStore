using System;
using System.IO;
using System.Linq;

namespace ColumnStore
{
    class ReadWriteHandlerTimeSpan : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var ts = (TimeSpan[]) values;
            var v  = new int[range.Length];
            for (var i = range.From; i < range.To; i++)
                v[i - range.From] = (int) ts[i].TotalSeconds;

            var requireBytes = range.Length * 4;
            var buff         = poolBytes.Rent(requireBytes);
            Buffer.BlockCopy(v, 0, buff, 0, requireBytes);
            targetStream.Write(buff, 0, requireBytes);
            poolBytes.Return(buff);
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var values = new int[count];
            Buffer.BlockCopy(buff, offset, values, 0, count * 4);
            return values.Select(p => TimeSpan.FromSeconds(p)).ToArray();
        }
    }
}