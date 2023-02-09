using System;
using System.IO;

namespace ColumnStore;

sealed class ReadWriteHandlerByte : ReadWriteBase
{
    public override void Pack(Array values, Stream targetStream, Range range) => targetStream.Write(((byte[]) values).AsSpan(range));

    public override Array Unpack(Span<byte> buff, int count)
    {
        var values = new byte[count];
        buff.Slice(0, count).CopyTo(values.AsSpan());
        return values;
    }
}