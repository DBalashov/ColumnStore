using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerTimeSpan : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var ts = (TimeSpan[]) values;

        var buff = poolInts.Rent(range.Length());
        for (int i = range.Start.Value, k = 0; i < range.End.Value; i++, k++)
            buff[k] = (int) ts[i].TotalMilliseconds;

        targetStream.Write(MemoryMarshal.Cast<int, byte>(buff.AsSpan(0, range.Length())));
        poolInts.Return(buff);
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var span = buff.Read<int>(count);
        var r    = new TimeSpan[count];
        for (var i = 0; i < count; i++)
            r[i] = TimeSpan.FromMilliseconds(span[i]);
        return r;
    }
}