using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore
    {
        /// <summary> Write entities as column-based to container. Property of entity used as column name</summary>
        /// <exception cref="ArgumentException"></exception>
        public void WriteEntity<E>([NotNull] Dictionary<CDT, E> entities) where E : class
        {
            if (entities == null)
                throw new ArgumentException("Can't be null", nameof(entities));
            if (!entities.Any())
                throw new ArgumentException("Can't be empty", nameof(entities));
            
            var props        = typeof(E).GetProps();
            var writeEntries = new Dictionary<string, byte[]>();

            foreach (var range in entities.GroupToDictionary(p => p.Trunc(Unit).Value))
            {
                foreach (var prop in props)
                {
                    var sectionName = Path.BuildSectionName(prop.Key, range.Key);
                    var data        = Container[sectionName];

                    byte[] buff;
                    if (prop.Value.PropertyType == typeof(int))
                        buff = pack<E, int>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(byte))
                        buff = pack<E, byte>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(bool))
                        buff = pack<E, bool>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(double))
                        buff = pack<E, double>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(string))
                        buff = pack<E, string>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(Guid))
                        buff = pack<E, Guid>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(TimeSpan))
                        buff = pack<E, TimeSpan>(data, range.Value, prop.Value);
                    else if (prop.Value.PropertyType == typeof(DateTime))
                        buff = pack<E, DateTime>(data, range.Value, prop.Value);
                    else throw new NotSupportedException(prop.Value.PropertyType + " not supported");

                    writeEntries.Add(sectionName, buff);
                }
            }

            Container.Put(writeEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        byte[] pack<E, V>([CanBeNull] byte[] data, [NotNull] KeyValue<E>[] newData, [NotNull] PropertyInfo prop) =>
            data != null
                ? data.Unpack<V>(Compressed).MergeWithReplace(newData, prop).Pack(Compressed)
                : newData.Pack<E, V>(prop, Compressed);
    }
}