using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerDouble : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var doubles = (double[]) values;
        var buff    = poolFloats.Rent(range.Length());
        for (int i = range.Start.Value, k = 0; i < range.End.Value; i++, k++)
            buff[k] = (float) doubles[i];

        targetStream.Write(MemoryMarshal.Cast<float, byte>(buff.AsSpan(0, range.Length())));
        poolFloats.Return(buff);
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var span = buff.Read<float>(count);
        var r    = new double[count];
        for (var i = 0; i < count; i++)
            r[i] = span[i];
        return r;
    }
}