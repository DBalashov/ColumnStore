using System;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerDouble : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var requireBytes = range.Length * 4;
            var buff         = poolBytes.Rent(requireBytes);

            if (!(values.GetValue(range.From) is float))
            {
                var doubles = (double[]) values;
                var v       = poolFloats.Rent(range.Length);
                for (var i = range.From; i < range.To; i++)
                    v[i - range.From] = (float) doubles[i];

                Buffer.BlockCopy(v, 0, buff, 0, requireBytes);
                poolFloats.Return(v);
            }
            else Buffer.BlockCopy(values, range.From * 4, buff, 0, requireBytes);

            targetStream.Write(buff, 0, requireBytes);
            poolBytes.Return(buff);
        }

        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var values = poolFloats.Rent(count);
            Buffer.BlockCopy(buff, offset, values, 0, count * 4);

            var floats = new double[count];
            for (var i = 0; i < count; i++)
                floats[i] = values[i];

            poolFloats.Return(values);

            return floats;
        }
    }
}