using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ColumnStore;

partial class ColumnStoreEntity
{
    public void Write<E>(Dictionary<CDT, E> entities) where E : class
    {
        ArgumentNullException.ThrowIfNull(entities);
        if (!entities.Any())
            throw new ArgumentException("Can't be empty", nameof(entities));

        var props        = ReflectionExtenders.GetProps<E>();
        var writeEntries = new Dictionary<string, byte[]>();

        foreach (var range in entities.GroupToDictionary(p => p.Trunc(ps.Unit).Value))
        {
            foreach (var prop in props)
            {
                var sectionName = ps.Path.BuildSectionName(prop.Key, range.Key);
                var data        = ps.Container[sectionName];

                var buff = prop.Value.DataType switch
                           {
                               StoredDataType.Byte  => pack<E, byte>(data, range.Value, prop.Value),
                               StoredDataType.SByte => pack<E, sbyte>(data, range.Value, prop.Value),
                               
                               StoredDataType.Int  => pack<E, int>(data, range.Value, prop.Value),
                               StoredDataType.UInt => pack<E, uint>(data, range.Value, prop.Value),

                               StoredDataType.Int16  => pack<E, short>(data, range.Value, prop.Value),
                               StoredDataType.UInt16 => pack<E, ushort>(data, range.Value, prop.Value),

                               StoredDataType.Int64  => pack<E, Int64>(data, range.Value, prop.Value),
                               StoredDataType.UInt64 => pack<E, UInt64>(data, range.Value, prop.Value),
                               
                               StoredDataType.Boolean  => pack<E, bool>(data, range.Value, prop.Value),
                               StoredDataType.Double   => pack<E, double>(data, range.Value, prop.Value),
                               StoredDataType.Float    => pack<E, float>(data, range.Value, prop.Value),
                               StoredDataType.String   => pack<E, string>(data, range.Value, prop.Value),
                               StoredDataType.Guid     => pack<E, Guid>(data, range.Value, prop.Value),
                               StoredDataType.TimeSpan => pack<E, TimeSpan>(data, range.Value, prop.Value),
                               StoredDataType.DateTime => pack<E, DateTime>(data, range.Value, prop.Value),
                               StoredDataType.Decimal  => pack<E, decimal>(data, range.Value, prop.Value),
                               StoredDataType.Half     => pack<E, Half>(data, range.Value, prop.Value),
                               StoredDataType.TimeOnly => pack<E, TimeOnly>(data, range.Value, prop.Value),
                               StoredDataType.DateOnly => pack<E, DateOnly>(data, range.Value, prop.Value),
                               _                       => throw new NotSupportedException(prop.Value + " not supported")
                           };


                writeEntries.Add(sectionName, buff);
            }
        }

        ps.Container.Put(writeEntries);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[] pack<E, V>(byte[]? data, KeyValue<E>[] newData, CSPropertyInfo prop) =>
        data != null
            ? data.Unpack<V>(ps.Compressed).MergeWithReplace(newData, prop).Pack(ps.Compressed)
            : newData.Pack<E, V>(prop, ps.Compressed);
}