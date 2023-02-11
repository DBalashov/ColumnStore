using System;
using System.IO;
using System.IO.Compression;

namespace ColumnStore;

sealed class StreamCompress : IVirtualWriteStream
{
    readonly MemoryStream stm;
    readonly GZipStream   stmCompress;

    internal StreamCompress(MemoryStream stmTarget)
    {
        stm         = stmTarget;
        stmCompress = new GZipStream(stm, CompressionMode.Compress, true);
    }

    public void Dispose()
    {
        stmCompress.Dispose();
        stm.Dispose();
    }

    public void Write(Span<byte> buff) => stmCompress.Write(buff);

    public void Write(byte b) => stmCompress.Write(new[] {b});

    public void Write(ushort value) => stmCompress.Write(BitConverter.GetBytes(value));

    public void Write(short value) => stmCompress.Write(BitConverter.GetBytes(value));

    public byte[] GetBytes()
    {
        stmCompress.Flush();
        return stm.ToArray();
    }
}