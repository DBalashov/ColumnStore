using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerDateTime : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var buff      = new CDT[range.Length()];
        var dateTimes = (DateTime[]) values;
        for (int i = range.Start.Value, k = 0; i < range.End.Value; i++, k++)
            buff[k] = dateTimes[i];

        targetStream.Write(MemoryMarshal.Cast<CDT, byte>(buff));
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var r    = new DateTime[count];
        var span = buff.Read<CDT>(count);
        for (var i = 0; i < r.Length; i++)
            r[i] = span[i];
        return r;
    }
}