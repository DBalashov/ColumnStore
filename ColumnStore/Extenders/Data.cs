using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ColumnStore
{
    public enum StoredDataType
    {
        Int      = 0,
        Int16    = 1,
        Int64    = 2,
        Double   = 3,
        DateTime = 4,
        Guid     = 5,
        String   = 6,
        TimeSpan = 7,
        Byte     = 8,
        Boolean  = 9
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

    public static class Extenders
    {
        public static StoredDataType DetectDataType(this Array a)
        {
            var v    = a.GetValue(0)!;
            var type = v.GetType();
            if (type == typeof(byte)) return StoredDataType.Byte;
            if (type == typeof(bool)) return StoredDataType.Boolean;
            if (type == typeof(int)) return StoredDataType.Int;
            if (type == typeof(short)) return StoredDataType.Int16;
            if (type == typeof(Int64)) return StoredDataType.Int64;
            if (type == typeof(string)) return StoredDataType.String;
            if (type == typeof(double)) return StoredDataType.Double;
            if (type == typeof(Guid)) return StoredDataType.Guid;
            if (type == typeof(DateTime)) return StoredDataType.DateTime;
            if (type == typeof(TimeSpan)) return StoredDataType.TimeSpan;
            throw new NotSupportedException(type.Name + " not supported");
        }

        static readonly Dictionary<StoredDataType, ReadWriteBase> readWriteHandlers = new()
        {
            [StoredDataType.Boolean]  = new ReadWriteHandlerBoolean(),
            [StoredDataType.Byte]     = new ReadWriteHandlerByte(),
            [StoredDataType.Double]   = new ReadWriteHandlerDouble(),
            [StoredDataType.DateTime] = new ReadWriteHandlerDateTime(),
            [StoredDataType.Guid]     = new ReadWriteHandlerGuid(),
            [StoredDataType.String]   = new ReadWriteHandlerString(),
            [StoredDataType.TimeSpan] = new ReadWriteHandlerTimeSpan(),
            [StoredDataType.Int]      = new ReadWriteHandlerInt(),
            [StoredDataType.Int16]    = new ReadWriteHandlerInt16(),
            [StoredDataType.Int64]    = new ReadWriteHandlerInt64()
        };

        internal static CompactType GetCompactType(this int count) =>
            count switch
            {
                <= 255 => CompactType.Byte,
                <= 65535 => CompactType.Short,
                _ => CompactType.Int
            };


        internal static string BuildSectionName(this string commonPath, string columnName, int rangeIndex) =>
            string.Join("/", commonPath, columnName, rangeIndex);


        internal static Array UnpackData(this Span<byte> data)
        {
            var count = BitConverter.ToInt32(data);
            var dataType = (StoredDataType)BitConverter.ToUInt16(data.Slice(4));
            return readWriteHandlers[dataType].Unpack(data.Slice(6), count);
        }

        internal static void PackData(this Array values, Stream targetStream, Range range)
        {
            var dataType = values.DetectDataType();

            targetStream.Write(BitConverter.GetBytes(range.Length()), 0, 4);
            targetStream.Write(BitConverter.GetBytes((ushort)dataType), 0, 2);

            readWriteHandlers[dataType].Pack(values, targetStream, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Array CreateSameType(this Array arr, int length) =>
            arr.Length == 0 ? throw new InvalidDataException("Array is empty") : Array.CreateInstance(arr.GetValue(0)!.GetType(), length);
    }
}