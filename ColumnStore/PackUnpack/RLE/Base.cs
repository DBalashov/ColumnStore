using System;

namespace ColumnStore;

abstract class RLEPackUnpackHandler
{
    internal abstract byte[]? Pack(Array source);

    internal abstract Array Unpack(byte[] buff);
}