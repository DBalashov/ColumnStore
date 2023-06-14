using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerDouble : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        if (values.GetValue(range.Start.Value) is double)
        {
            var doubles = (double[]) values;
            var buff    = poolFloats.Rent(range.Length());
            for (int i = range.Start.Value, k = 0; i < range.End.Value; i++, k++)
                buff[k] = (float) doubles[i];

            targetStream.Write(MemoryMarshal.Cast<float, byte>(buff));
            poolFloats.Return(buff);
        }
        else if (values.GetValue(range.Start.Value) is float)
        {
            var span = ((float[]) values).AsSpan(new Range(range.Start.Value * 4, range.End));
            targetStream.Write(MemoryMarshal.Cast<float, byte>(span));
        }
        else throw new NotSupportedException($"Type {values.GetValue(range.Start.Value)?.GetType()} not supported");
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