using System;
using System.Buffers;
using System.IO;

namespace ColumnStore;

abstract class ReadWriteBase
{
    protected static readonly ArrayPool<byte>   poolBytes   = ArrayPool<byte>.Shared;
    protected static readonly ArrayPool<float>  poolFloats  = ArrayPool<float>.Shared;
    protected static readonly ArrayPool<int>    poolInts    = ArrayPool<int>.Shared;
    protected static readonly ArrayPool<uint>   poolUInts   = ArrayPool<uint>.Shared;
    protected static readonly ArrayPool<short>  poolShorts  = ArrayPool<short>.Shared;
    protected static readonly ArrayPool<ushort> poolUShorts = ArrayPool<ushort>.Shared;
    protected static readonly ArrayPool<Int64>  poolInt64s  = ArrayPool<Int64>.Shared;
    protected static readonly ArrayPool<UInt64> poolUInt64s = ArrayPool<UInt64>.Shared;

    public abstract void Pack(Array values, IVirtualWriteStream targetStream, Range range);

    public abstract Array Unpack(Span<byte> buff, int count);
}