using System;
using System.Runtime.InteropServices;

namespace ColumnStore;

sealed class ReadWriteHandlerDouble : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        if (values.GetValue(range.Start.Value) is double)
        {
            var doubles = (double[]) values;
            var v       = poolFloats.Rent(range.Length());
            for (var i = range.Start.Value; i < range.End.Value; i++)
                v[i - range.Start.Value] = (float) doubles[i];

            targetStream.Write(MemoryMarshal.Cast<float, byte>(v));
            poolFloats.Return(v);
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
        var span = MemoryMarshal.Cast<byte, float>(buff.Slice(0, count * 4));
        var r    = new double[count];
        for (var i = 0; i < count; i++)
            r[i] = span[i];
        return r;
    }
}