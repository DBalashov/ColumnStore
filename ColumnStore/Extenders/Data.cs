using System;
using System.IO;
using System.Runtime.CompilerServices;
using SpanByteExtenders;

namespace ColumnStore;

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
    Boolean  = 9,

    SByte    = 10,
    UInt     = 11,
    UInt16   = 12,
    UInt64   = 13,
    Half     = 14,
    Decimal  = 15,
    DateOnly = 16,
    TimeOnly = 17,
    Float    = 18
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
    public static StoredDataType DetectDataType(this Array arr)
    {
        if (arr.Length == 0) throw new InvalidDataException("Array is empty");

        var type = arr.GetValue(0)!.GetType();
        return ReflectionExtenders.TryDetectDataType(type, out var dataType)
                   ? dataType
                   : throw new NotSupportedException(type.Name + " not supported");
    }

    internal static CompactType GetCompactType(this int count) =>
        count switch
        {
            <= 255   => CompactType.Byte,
            <= 65535 => CompactType.Short,
            _        => CompactType.Int
        };


    internal static string BuildSectionName(this string commonPath, string columnName, int rangeIndex) =>
        string.Join("/", commonPath, columnName, rangeIndex);


    internal static Array UnpackData(this Span<byte> data)
    {
        var count    = data.Read<int>();
        var dataType = (StoredDataType) data.Read<ushort>();
        return dataType.GetHandler().Unpack(data, count);
    }

    internal static void PackData(this Array values, IVirtualWriteStream targetStream, Range range)
    {
        var dataType = values.DetectDataType();

        targetStream.Write(BitConverter.GetBytes(range.Length()));
        targetStream.Write(BitConverter.GetBytes((ushort) dataType));

        dataType.GetHandler().Pack(values, targetStream, range);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Array CreateSameType(this Array arr, int length) =>
        arr.Length == 0 ? throw new InvalidDataException("Array is empty") : Array.CreateInstance(arr.GetValue(0)!.GetType(), length);
}