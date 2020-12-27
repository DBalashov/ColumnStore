using System;
using JetBrains.Annotations;

namespace ColumnStore
{
    abstract class RLEPackUnpackHandler
    {
        [CanBeNull]
        internal abstract byte[] Pack([NotNull] Array source);

        [NotNull]
        internal abstract Array Unpack([NotNull] byte[] buff);
    }
}