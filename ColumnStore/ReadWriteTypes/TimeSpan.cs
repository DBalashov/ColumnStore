using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerTimeSpan : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var ts = (TimeSpan[]) values;
        var v  = new int[range.Length()];
        for (var i = range.Start.Value; i < range.End.Value; i++)
            v[i - range.Start.Value] = (int) (ts[i].TotalSeconds * 1000);

        targetStream.Write(MemoryMarshal.Cast<int, byte>(v));
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var span = buff.Read<int>(count);
        var r    = new TimeSpan[count];
        for (var i = 0; i < count; i++)
            r[i] = TimeSpan.FromSeconds(span[i] / 1000.0);
        return r;
    }
}