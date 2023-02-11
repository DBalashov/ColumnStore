using System;
using System.Runtime.InteropServices;

namespace ColumnStore;

sealed class ReadWriteHandlerSByte : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var span = ((sbyte[]) values).AsSpan(range);
        targetStream.Write(MemoryMarshal.Cast<sbyte, byte>(span));
    }

    public override Array Unpack(Span<byte> buff, int count)
    {
        var values     = new sbyte[count];
        var valuesSpan = MemoryMarshal.Cast<sbyte, byte>(values);
        buff.Slice(0, count).CopyTo(valuesSpan);
        return values;
    }
}