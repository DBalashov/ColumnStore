using System;
using System.IO;
using SpanByteExtenders;

namespace ColumnStore;

sealed class ReadWriteHandlerByte : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range) => targetStream.Write(((byte[]) values).AsSpan(range));

    public override Array Unpack(Span<byte> buff, int count) => 
        buff.Read<byte>(count).ToArray();
}