using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ColumnStore
{
    sealed class ReadWriteHandlerDouble : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            if (values.GetValue(range.Start.Value) is not float)
            {
                var doubles = (double[])values;
                var v       = poolFloats.Rent(range.Length());
                for (var i = range.Start.Value; i < range.End.Value; i++)
                    v[i - range.Start.Value] = (float)doubles[i];

                targetStream.Write(MemoryMarshal.Cast<float, byte>(v));
                poolFloats.Return(v);
            }
            else
            {
                targetStream.Write(MemoryMarshal.Cast<float, byte>(((float[])values).AsSpan(new Range(range.Start.Value * 4, range.End))));
            }
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
}