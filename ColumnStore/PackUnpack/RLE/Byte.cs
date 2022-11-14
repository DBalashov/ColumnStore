using System;
using System.Diagnostics.CodeAnalysis;

namespace ColumnStore
{
    [ExcludeFromCodeCoverage]
    class RLEPackUnpackHandlerBytes : RLEPackUnpackHandler
    {
        internal override byte[]? Pack(Array source)
        {
            if (source.Length == 0)
                return null;

            var values = (byte[]) source;

            var outBuff  = new byte[4 + values.Length * 2];
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

                if (outIndex + 2 >= outBuff.Length) return null; // result size > original size -> break & return null (uncompressable) 

                outBuff[outIndex]     =  (byte) count;
                outBuff[outIndex + 1] =  (byte) value;
                outIndex              += 2;
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
            var r        = new byte[outElementsCount];
            while (offset < buff.Length)
            {
                var cnt   = buff[offset++];
                var value = buff[offset++];
                for (var k = 0; k < cnt; k++, outIndex++)
                    r[outIndex] = value;
            }

            return r;
        }
    }
}