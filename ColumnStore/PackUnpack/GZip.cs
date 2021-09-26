using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

namespace ColumnStore
{
    [ExcludeFromCodeCoverage]
    static class GZipExtenders
    {
        static readonly ArrayPool<byte> pool = ArrayPool<byte>.Shared;

        [DebuggerStepThrough]
        public static byte[] GZipUnpack(this byte[] data, int offset = 0)
        {
            var buff = pool.Rent(8192);

            using var stm = new MemoryStream();
            using (var stmSource = new MemoryStream(data))
            {
                stmSource.Position = offset;
                using (var gz = new GZipStream(stmSource, CompressionMode.Decompress, true))
                {
                    int read = 0;
                    while ((read = gz.Read(buff, 0, buff.Length)) > 0)
                        stm.Write(buff, 0, read);
                    gz.Close();
                }

                stmSource.Close();
            }

            pool.Return(buff);

            var r = stm.ToArray();
            stm.Close();

            return r;
        }
    }
}