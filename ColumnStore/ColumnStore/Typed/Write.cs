using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore
{
    partial class ColumnStoreTyped
    {
        public void Write<V>(string columnName, Dictionary<CDT, V> values)
        {
            if (values == null)
                throw new ArgumentException("Can't be null", nameof(values));
            if (!values.Any())
                throw new ArgumentException("Can't be empty", nameof(values));
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Can't be empty", nameof(columnName));

            var entries = new Dictionary<string, byte[]>();
            foreach (var range in values.GroupToDictionary(p => p.Trunc(ps.Unit).Value))
            {
                var sectionName = ps.Path.BuildSectionName(columnName, range.Key);

                var data = ps.Container[sectionName];
                entries.Add(sectionName, data != null
                                ? data.Unpack<V>(ps.Compressed).MergeWithReplace(range.Value).Pack(ps.Compressed)
                                : range.Value.ToDictionary(p => p.Key, p => p.Value).Pack(ps.Compressed));
            }

            ps.Container.Put(entries);
        }
    }
}