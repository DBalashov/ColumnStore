using System;
using System.Runtime.InteropServices;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerGeneric<T> : ReadWriteBase where T : struct
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        var span = ((T[]) values).AsSpan(range);
        targetStream.Write(MemoryMarshal.Cast<T, byte>(span));
    }

    public override Array Unpack(Span<byte> buff, int count) =>
        buff.Read<T>(count).ToArray();
}