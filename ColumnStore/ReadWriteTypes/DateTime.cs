using System;
using System.Runtime.InteropServices;

namespace ColumnStore;

sealed class ReadWriteHandlerDateTime : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var v  = new CDT[range.Length()];
        var dt = (DateTime[]) values;
        for (var i = range.Start.Value; i < range.End.Value; i++)
            v[i - range.Start.Value] = dt[i];

        targetStream.Write(MemoryMarshal.Cast<CDT, byte>(v));
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var r    = new DateTime[count];
        var span = MemoryMarshal.Cast<byte, CDT>(buff.Slice(0, count * 4));
        for (var i = 0; i < r.Length; i++)
            r[i] = span[i];
        return r;
    }
}