using System;
using System.Buffers;

namespace ColumnStore;

abstract class ReadWriteBase
{
    protected static readonly ArrayPool<byte>  poolBytes  = ArrayPool<byte>.Shared;
    protected static readonly ArrayPool<float> poolFloats = ArrayPool<float>.Shared;
    protected static readonly ArrayPool<int>   poolInts   = ArrayPool<int>.Shared;

    public abstract void Pack(Array values, IVirtualWriteStream targetStream, Range range);

    public abstract Array Unpack(Span<byte> buff, int count);
}