using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore;

partial class ColumnStoreEntity : IColumnStoreEntity
{
    readonly PersistentColumnStore ps;

    internal ColumnStoreEntity(PersistentColumnStore ps) => this.ps = ps;

    public Dictionary<CDT, E> Read<E>(CDT from, CDT to) where E : class, new()
    {
        if (from >= to)
            throw new ArgumentException($"Invalid values: {from} >= {to}");

        var r = new Dictionary<CDT, E>();

        var props = ReflectionExtenders.GetProps<E>();
        foreach (var range in new CDTRange(from, to).GetRanges(ps.Unit))
        {
            foreach (var prop in props)
            {
                var sectionName = ps.Path.BuildSectionName(prop.Key, range.Key.Value);
                var data        = ps.Container[sectionName];
                if (data == null) continue;

                r = prop.Value.DataType switch
                    {
                        StoredDataType.Byte  => unpack<E, byte>(data, range, r, prop.Value),
                        StoredDataType.SByte => unpack<E, sbyte>(data, range, r, prop.Value),
                        
                        StoredDataType.Int      => unpack<E, int>(data, range, r, prop.Value),
                        StoredDataType.UInt     => unpack<E, uint>(data, range, r, prop.Value),
                        
                        StoredDataType.Int16    => unpack<E, short>(data, range, r, prop.Value),
                        StoredDataType.UInt16   => unpack<E, ushort>(data, range, r, prop.Value),
                        
                        StoredDataType.Int64    => unpack<E, Int64>(data, range, r, prop.Value),
                        StoredDataType.UInt64   => unpack<E, UInt64>(data, range, r, prop.Value),
                        
                        StoredDataType.Boolean  => unpack<E, bool>(data, range, r, prop.Value),
                        StoredDataType.Double   => unpack<E, double>(data, range, r, prop.Value),
                        StoredDataType.Float    => unpack<E, float>(data, range, r, prop.Value),
                        StoredDataType.String   => unpack<E, string>(data, range, r, prop.Value),
                        StoredDataType.Guid     => unpack<E, Guid>(data, range, r, prop.Value),
                        StoredDataType.DateTime => unpack<E, DateTime>(data, range, r, prop.Value),
                        StoredDataType.TimeSpan => unpack<E, TimeSpan>(data, range, r, prop.Value),
                        StoredDataType.Decimal  => unpack<E, decimal>(data, range, r, prop.Value),
                        StoredDataType.Half     => unpack<E, Half>(data, range, r, prop.Value),
                        StoredDataType.TimeOnly => unpack<E, TimeOnly>(data, range, r, prop.Value),
                        StoredDataType.DateOnly => unpack<E, DateOnly>(data, range, r, prop.Value),
                        _                       => throw new NotSupportedException(prop.Value.DataType + " not supported")
                    };
            }
        }

        return r;
    }

    Dictionary<CDT, T> unpack<T, V>(byte[] data, CDTKeyRange keyRange, Dictionary<CDT, T> target, CSPropertyInfo prop) where T : class, new()
    {
        var setValue      = (Action<T, V>) prop.Setter;
        var createFunctor = (Func<T>) prop.InstanceCreator;
        foreach (var item in data.Unpack<V>(ps.Compressed).Where(p => keyRange.Range.InRange(p.Key)))
        {
            var key = new CDT(item.Key);
            if (!target.TryGetValue(key, out var entity))
                target.Add(key, entity = createFunctor());

            setValue(entity, item.Value);
        }

        return target;
    }
}