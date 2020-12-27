using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace ColumnStore
{
    public enum StoredDataType
    {
        Int      = 0,
        Double   = 1,
        DateTime = 2,
        Guid     = 3,
        String   = 4,
        TimeSpan = 5,
        Byte     = 6
    }

    public enum StoredPackType
    {
        None       = 0,
        Dictionary = 1,
        RLE        = 2
    }

    internal enum CompactType
    {
        Byte  = 0,
        Short = 1,
        Int   = 2
    }

    static class ExtendersSectionData
    {
        static readonly Dictionary<StoredDataType, ReadWriteBase> readWriteHandlers = new Dictionary<StoredDataType, ReadWriteBase>()
        {
            [StoredDataType.Byte]     = new ReadWriteHandlerByte(),
            [StoredDataType.Double]   = new ReadWriteHandlerDouble(),
            [StoredDataType.DateTime] = new ReadWriteHandlerDateTime(),
            [StoredDataType.Guid]     = new ReadWriteHandlerGuid(),
            [StoredDataType.String]   = new ReadWriteHandlerString(),
            [StoredDataType.TimeSpan] = new ReadWriteHandlerTimeSpan(),
            [StoredDataType.Int]      = new ReadWriteHandlerInt()
        };

        internal static CompactType GetCompactType(this int count) =>
            count switch
            {
                <= 255 => CompactType.Byte,
                <= 65535 => CompactType.Short,
                _ => CompactType.Int
            };

        [NotNull]
        internal static string BuildSectionName([NotNull] this string commonPath, [NotNull] string columnName, int rangeIndex) =>
            string.Join("/", commonPath, columnName, rangeIndex);

        internal static StoredDataType DetectDataType([NotNull] this Array a)
        {
            var v    = a.GetValue(0);
            var type = v.GetType();
            if (type == typeof(byte)) return StoredDataType.Byte;
            if (type == typeof(int)) return StoredDataType.Int;
            if (type == typeof(string)) return StoredDataType.String;
            if (type == typeof(double)) return StoredDataType.Double;
            if (type == typeof(Guid)) return StoredDataType.Guid;
            if (type == typeof(DateTime)) return StoredDataType.DateTime;
            if (type == typeof(TimeSpan)) return StoredDataType.TimeSpan;
            throw new NotSupportedException(type.Name + " not supported");
        }

        [NotNull]
        internal static Array UnpackData([NotNull] this byte[] data, int offset = 0)
        {
            var count    = BitConverter.ToInt32(data, offset);
            offset += 4;
            
            var dataType = (StoredDataType) BitConverter.ToUInt16(data, offset);
            offset += 2;
            
            return readWriteHandlers[dataType].Unpack(data, count, offset);
        }

        internal static void PackData([NotNull] this Array values, [NotNull] Stream targetStream, Range range)
        {
            var dataType = values.DetectDataType();

            targetStream.Write(BitConverter.GetBytes(range.Length), 0, 4);
            targetStream.Write(BitConverter.GetBytes((ushort) dataType), 0, 2);
            readWriteHandlers[dataType].Pack(values, targetStream, range);
        }
    }
}