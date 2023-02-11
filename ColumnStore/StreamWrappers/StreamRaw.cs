using System;
using System.IO;

namespace ColumnStore;

sealed class StreamRaw : IVirtualWriteStream
{
    readonly MemoryStream stm;

    internal StreamRaw(MemoryStream stmTarget) => stm = stmTarget;

    public void Dispose() => stm.Dispose();

    public void Write(Span<byte> buff) => stm.Write(buff);

    public void Write(byte b) => stm.Write(new[] {b});

    public void Write(ushort value) => stm.Write(BitConverter.GetBytes(value));

    public void Write(short value) => stm.Write(BitConverter.GetBytes(value));

    public byte[] GetBytes() => stm.ToArray();
}