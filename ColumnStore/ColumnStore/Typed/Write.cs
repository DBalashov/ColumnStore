using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore
    {
        /// <summary> Write data for column to container</summary>
        /// <param name="columnName">case insensitive column name</param>
        /// <exception cref="ArgumentException"></exception>
        public void Write<V>(string columnName, [NotNull] Dictionary<CDT, V> values)
        {
            if (values==null)
                throw new ArgumentException("Can't be null", nameof(values));
            if (!values.Any())
                throw new ArgumentException("Can't be empty", nameof(values));
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Can't be empty", nameof(columnName));
            
            var entries = new Dictionary<string, byte[]>();
            foreach (var range in values.GroupToDictionary(p => p.Trunc(Unit).Value))
            {
                var sectionName = Path.BuildSectionName(columnName, range.Key);

                var data = Container[sectionName];
                entries.Add(sectionName, data != null
                                ? data.Unpack<V>(Compressed).MergeWithReplace(range.Value).Pack(Compressed)
                                : range.Value.ToDictionary(p => p.Key, p => p.Value).Pack(Compressed));
            }

            Container.Put(entries);
        }
    }
}