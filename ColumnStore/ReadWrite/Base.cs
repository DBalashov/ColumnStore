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

        public abstract void Pack([NotNull] Array values, [NotNull] Stream targetStream, Range range);

        [NotNull]
        public abstract Array Unpack([NotNull] byte[] buff, int count, int offset);

        #region CompactValues / UncompactValues

        // todo write to stream
        protected void CompactValues([NotNull] int[] values, [NotNull] byte[] targetBuff, int fromOffset, CompactType type)
        {
            switch (type)
            {
                case CompactType.Byte:
                    foreach (var value in values)
                    {
                        targetBuff[fromOffset] = (byte) value;
                        fromOffset++;
                    }

                    break;

                case CompactType.Short:
                    foreach (var value in values)
                    {
                        targetBuff[fromOffset]     =  (byte) (value & 0xFF);
                        targetBuff[fromOffset + 1] =  (byte) ((value >> 8) & 0xFF);
                        fromOffset                 += 2;
                    }

                    break;

                // case CompactType.Int:
                //     foreach (var value in values)
                //     {
                //         targetBuff[fromOffset]     =  (byte) (value & 0xFF);
                //         targetBuff[fromOffset + 1] =  (byte) ((value >> 8) & 0xFF);
                //         targetBuff[fromOffset + 2] =  (byte) ((value >> 16) & 0xFF);
                //         targetBuff[fromOffset + 3] =  (byte) ((value >> 24) & 0xFF);
                //         fromOffset                 += 4;
                //     }
                //
                //     break;

                default:
                    throw new NotSupportedException(type + " not supported");
            }
        }

        [NotNull]
        protected internal int[] UncompactValues([NotNull] byte[] buff, int offset, int count, CompactType compactType)
        {
            var r = new int[count];
            switch (compactType)
            {
                case CompactType.Byte:
                    for (var i = 0; i < count; i++, offset++)
                        r[i] = buff[offset];
                    break;

                case CompactType.Short:
                    for (var i = 0; i < count; i++, offset += 2)
                        r[i] = ((int) buff[offset]) |
                               (((int) buff[offset + 1]) << 8);

                    break;

                // case CompactType.Int:
                //     for (var i = 0; i < count; i++, offset += 4)
                //         r[i] = ((int) buff[offset]) |
                //                (((int) buff[offset + 1]) << 8) |
                //                (((int) buff[offset + 2]) << 16) |
                //                (((int) buff[offset + 3]) << 24);
                //
                //     break;

                default:
                    throw new NotSupportedException(compactType + " not supported");
            }

            return r;
        }

        #endregion
    }
}