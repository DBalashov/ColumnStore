using System;
using System.Buffers;
using System.IO;
using JetBrains.Annotations;

namespace ColumnStore
{
    abstract class ReadWriteBase
    {
        protected static          ArrayPool<byte>  poolBytes  = ArrayPool<byte>.Shared;
        protected static readonly ArrayPool<float> poolFloats = ArrayPool<float>.Shared;
        protected static readonly ArrayPool<int>   poolInts   = ArrayPool<int>.Shared;
        protected static readonly ArrayPool<short> poolShorts = ArrayPool<short>.Shared;
        protected static readonly ArrayPool<Int64> poolInt64s = ArrayPool<Int64>.Shared;

        public abstract void Pack([NotNull] Array values, [NotNull] Stream targetStream, Range range);

        [NotNull]
        public abstract Array Unpack([NotNull] byte[] buff, int count, int offset);
    }
}