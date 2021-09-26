using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColumnStore
{
    sealed class ReadWriteHandlerTimeSpan : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var ts = (TimeSpan[])values;
            var v  = new int[range.Length()];
            for (var i = range.Start.Value; i < range.End.Value; i++)
                v[i - range.Start.Value] = (int)ts[i].TotalSeconds;

            targetStream.Write(MemoryMarshal.Cast<int, byte>(v));
        }

        public override Array Unpack(Span<byte> buff, int count)
        {
            var span = MemoryMarshal.Cast<byte, int>(buff.Slice(0, count * 4));
            var r    = new TimeSpan[count];
            for (var i = 0; i < count; i++)
                r[i] = TimeSpan.FromSeconds(span[i]);
            return r;
        }
    }
}