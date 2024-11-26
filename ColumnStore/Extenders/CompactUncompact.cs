using System;

namespace ColumnStore;

static class CompactUncompactExtenders
{
    internal static void CompactValues(this int[] values, Span<byte> targetBuff, CompactType compactType)
    {
        switch (compactType)
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

            default:
                throw new NotSupportedException(compactType + " not supported");
        }
    }

    internal static int[] UncompactValues(this Span<byte> buff, int count, CompactType compactType)
    {
        var r = new int[count];
        switch (compactType)
        {
            case CompactType.Byte:
            {
                for (var i = 0; i < count; i++)
                    r[i] = buff[i];

                return r;
            }

            case CompactType.Short:
            {
                var offset = 0;
                for (var i = 0; i < count; i++, offset += 2)
                    r[i] = ((int) buff[offset]) |
                           (((int) buff[offset + 1]) << 8);
                return r;
            }

            default:
                throw new NotSupportedException(compactType + " not supported");
        }
    }
}