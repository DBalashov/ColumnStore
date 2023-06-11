using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        var props = typeof(E).GetProps();
        foreach (var range in new CDTRange(from, to).GetRanges(ps.Unit))
        {
            foreach (var prop in props)
            {
                var sectionName = ps.Path.BuildSectionName(prop.Key, range.Key.Value);
                var data        = ps.Container[sectionName];
                if (data == null) continue;

                if (prop.Value.PropertyType      == typeof(int)) unpack<E, int>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(uint)) unpack<E, uint>(data, range, r, prop.Value);

                else if (prop.Value.PropertyType == typeof(short)) unpack<E, short>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(ushort)) unpack<E, ushort>(data, range, r, prop.Value);

                else if (prop.Value.PropertyType == typeof(Int64)) unpack<E, Int64>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(UInt64)) unpack<E, UInt64>(data, range, r, prop.Value);

                else if (prop.Value.PropertyType == typeof(byte)) unpack<E, byte>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(sbyte)) unpack<E, sbyte>(data, range, r, prop.Value);

                else if (prop.Value.PropertyType == typeof(bool)) unpack<E, bool>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(double)) unpack<E, double>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(string)) unpack<E, string>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(Guid)) unpack<E, Guid>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(TimeSpan)) unpack<E, TimeSpan>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(DateTime)) unpack<E, DateTime>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(decimal)) unpack<E, decimal>(data, range, r, prop.Value);
                else if (prop.Value.PropertyType == typeof(Half)) unpack<E, Half>(data, range, r, prop.Value);

                else throw new NotSupportedException(prop.Value.PropertyType + " not supported");
            }
        }

        return r;
    }

    void unpack<T, V>(byte[] data, CDTKeyRange keyRange, Dictionary<CDT, T> target, PropertyInfo prop) where T : class, new()
    {
        var setValue = prop.getActionSet<T, V>();

        var ctor          = Expression.New(typeof(T));
        var memberInit    = Expression.MemberInit(ctor);
        var createFunctor = Expression.Lambda<Func<T>>(memberInit).Compile();

        foreach (var item in data.Unpack<V>(ps.Compressed).Where(p => keyRange.Range.InRange(p.Key)))
        {
            var key = new CDT(item.Key);
            if (!target.TryGetValue(key, out var entity))
                target.Add(key, entity = createFunctor());

            setValue(entity, item.Value);
        }
    }
}