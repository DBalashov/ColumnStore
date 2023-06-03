using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerSByte : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var span = ((sbyte[]) values).AsSpan(range);
        targetStream.Write(MemoryMarshal.Cast<sbyte, byte>(span));
    }

    public override Array Unpack(Span<byte> buff, int count) =>
        buff.Read<sbyte>(count).ToArray();
}