using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ColumnStore
{
    partial class ColumnStoreEntity
    {
        public void Write<E>(Dictionary<CDT, E> entities) where E : class
        {
            if (entities == null)
                throw new ArgumentException("Can't be null", nameof(entities));
            if (!entities.Any())
                throw new ArgumentException("Can't be empty", nameof(entities));

            var props        = typeof(E).GetProps();
            var writeEntries = new Dictionary<string, byte[]>();

            foreach (var range in entities.GroupToDictionary(p => p.Trunc(ps.Unit).Value))
            {
                foreach (var prop in props)
                {
                    var sectionName = ps.Path.BuildSectionName(prop.Key, range.Key);
                    var data        = ps.Container[sectionName];

                    byte[] buff;
                    if (prop.Value.PropertyType == typeof(int)) buff           = pack<E, int>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(short)) buff    = pack<E, short>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(Int64)) buff    = pack<E, Int64>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(byte)) buff     = pack<E, byte>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(bool)) buff     = pack<E, bool>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(double)) buff   = pack<E, double>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(string)) buff   = pack<E, string>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(Guid)) buff     = pack<E, Guid>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(TimeSpan)) buff = pack<E, TimeSpan>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(DateTime)) buff = pack<E, DateTime>(data, range.Value, prop.Value);
                    else throw new NotSupportedException(prop.Value.PropertyType + " not supported");

                    writeEntries.Add(sectionName, buff);
                }
            }

            ps.Container.Put(writeEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte[] pack<E, V>(byte[]? data, KeyValue<E>[] newData, PropertyInfo prop) =>
            data != null
                ? data.Unpack<V>(ps.Compressed).MergeWithReplace(newData, prop).Pack(ps.Compressed)
                : newData.Pack<E, V>(prop, ps.Compressed);
    }
}