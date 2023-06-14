using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerTimeOnly : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var times = (TimeOnly[]) values;
        var buff  = poolInts.Rent(range.Length());
        for (int i = range.Start.Value, k = 0; i < range.End.Value; i++, k++)
            buff[k] = (int) times[i].ToTimeSpan().TotalMilliseconds;

        targetStream.Write(MemoryMarshal.Cast<int, byte>(buff.AsSpan(0, range.Length())));
        poolInts.Return(buff);
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var span = buff.Read<int>(count);
        var r    = new TimeOnly[count];
        for (var i = 0; i < count; i++)
            r[i] = TimeOnly.FromTimeSpan(TimeSpan.FromMilliseconds(span[i]));

        return r;
    }
}