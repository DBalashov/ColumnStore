using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;

namespace ColumnStore;

sealed class StreamDecompress : IVirtualReadStream
{
    readonly MemoryStream stm;

    internal StreamDecompress(byte[] data, int offset = 0)
    {
        stm          = new MemoryStream(data);
        stm.Position = offset;
    }

    public byte[] GetBytes()
    {
        var buff = ArrayPool<byte>.Shared.Rent(8192);

        using var stmTarget = new MemoryStream();
        using (var gz = new GZipStream(stm, CompressionMode.Decompress, true))
        {
            var read = 0;
            while ((read = gz.Read(buff, 0, buff.Length)) > 0)
                stmTarget.Write(buff, 0, read);
            gz.Close();
        }

        ArrayPool<byte>.Shared.Return(buff);
        
        return stmTarget.ToArray();
    }

    public void Dispose() => stm.Dispose();
}