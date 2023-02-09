using System;
using System.Diagnostics.CodeAnalysis;

namespace ColumnStore;

[ExcludeFromCodeCoverage]
class RLEPackUnpackHandlerUInt16s : RLEPackUnpackHandler
{
    internal override byte[]? Pack(Array source)
    {
        if (source.Length == 0)
            return null;

        var values = (ushort[]) source;

        var outBuff  = new byte[4 + (values.Length * 2) * 2];
        var outIndex = 4;

        var inIndex = 0;
        while (inIndex < values.Length)
        {
            var value = values[inIndex];
            var count = 0;
            while (inIndex < values.Length && values[inIndex] == value && count < 255)
            {
                inIndex++;
                count++;
            }

            if (outIndex * 2 + 2 >= outBuff.Length) return null; // result size > original size -> break & return null (uncompressable) 

            outBuff[outIndex] = (byte) count;

            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, outBuff, outIndex + 1, 2);

            outIndex += 1 + 2;
        }

        Buffer.BlockCopy(BitConverter.GetBytes(values.Length), 0, outBuff, 0, 4);
        Array.Resize(ref outBuff, outIndex);
        return outBuff;
    }

    internal override Array Unpack(byte[] buff)
    {
        var offset = 0;

        var outElementsCount = BitConverter.ToInt32(buff, offset);
        offset += 4;

        var outIndex = 0;
        var r        = new ushort[outElementsCount];
        while (offset < buff.Length)
        {
            var cnt   = buff[offset++];
            var value = BitConverter.ToUInt16(buff, offset);
            offset += 2;
            for (var k = 0; k < cnt; k++, outIndex++)
                r[outIndex] = value;
        }

        return r;
    }
}