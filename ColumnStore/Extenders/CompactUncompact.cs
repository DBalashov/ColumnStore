using System;

namespace ColumnStore;

static class CompactUncompactExtenders
{
    internal static void CompactValues(this int[] values, Span<byte> targetBuff, CompactType type)
    {
        switch (type)
        {
            case CompactType.Byte:
            {
                var idx = 0;
                foreach (var value in values)
                {
                    targetBuff[idx] = (byte) value;
                    idx++;
                }

                break;
            }

            case CompactType.Short:
            {
                var idx = 0;
                foreach (var value in values)
                {
                    targetBuff[idx]     =  (byte) (value        & 0xFF);
                    targetBuff[idx + 1] =  (byte) ((value >> 8) & 0xFF);
                    idx                 += 2;
                }

                break;
            }

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

    internal static int[] UncompactValues(this Span<byte> buff, int count, CompactType compactType)
    {
        switch (compactType)
        {
            case CompactType.Byte:
            {
                var r = new int[count];
                for (var i = 0; i < count; i++)
                    r[i] = buff[i];

                return r;
            }

            case CompactType.Short:
            {
                var offset = 0;
                var r      = new int[count];
                for (var i = 0; i < count; i++, offset += 2)
                    r[i] = ((int) buff[offset]) |
                           (((int) buff[offset + 1]) << 8);
                return r;
            }

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
    }
}