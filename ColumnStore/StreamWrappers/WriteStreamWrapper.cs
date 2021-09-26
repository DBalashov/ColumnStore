using System.IO;
using System.IO.Compression;

namespace ColumnStore
{
    class WriteStreamWrapper : Stream
    {
        public override void Flush() => (zstm as Stream ?? stm).Flush();

        public override int Read(byte[] buffer, int offset, int count) => throw new System.NotImplementedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new System.NotImplementedException();

        public override void SetLength(long value) => throw new System.NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => (zstm as Stream ?? stm).Write(buffer, offset, count);

        public override bool CanRead  => false;
        public override bool CanSeek  => false;
        public override bool CanWrite => true;
        public override long Length   => (zstm as Stream ?? stm).Length;

        public override long Position
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        readonly MemoryStream stm;
        readonly GZipStream?  zstm;

        public WriteStreamWrapper(MemoryStream stm, bool withCompression)
        {
            this.stm = stm;
            if (withCompression)
                zstm = new GZipStream(stm, CompressionMode.Compress, true);
        }

        protected override void Dispose(bool disposing)
        {
            zstm?.Dispose();
            stm.Dispose();
            base.Dispose(disposing);
        }

        internal byte[] ToArray()
        {
            zstm?.Flush();
            return stm.ToArray();
        }
    }
}