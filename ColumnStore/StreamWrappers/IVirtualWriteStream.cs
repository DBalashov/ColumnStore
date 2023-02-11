using System;

namespace ColumnStore;

interface IVirtualWriteStream : IDisposable
{
    void Write(Span<byte> buff);
    
    void Write(byte value);
    
    void Write(ushort value);
    
    void Write(short value);

    byte[] GetBytes();
}