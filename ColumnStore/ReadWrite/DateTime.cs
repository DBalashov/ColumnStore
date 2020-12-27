using System;
using System.IO;

namespace ColumnStore
{
    class ReadWriteHandlerDateTime : ReadWriteBase
    {
        public override void Pack(Array values, Stream targetStream, Range range)
        {
            var v      = new CDT[range.Length];
            var dt     = (DateTime[]) values;
            for (var i = range.From; i < range.To; i++)
                v[i - range.From] = dt[i];

            var buff = v.PackStructs();
            targetStream.Write(buff, 0, buff.Length);
        }
        
        public override Array Unpack(byte[] buff, int count, int offset)
        {
            var r     = new DateTime[count];
            var items = buff.UnpackStructs<CDT>(offset, count);
            for (var i = 0; i < r.Length; i++)
                r[i] = items[i];
            return r;
        }
    }
}