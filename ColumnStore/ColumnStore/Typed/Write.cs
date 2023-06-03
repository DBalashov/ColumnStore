using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore;

partial class ColumnStoreTyped
{
    public void Write<V>(string columnName, Dictionary<CDT, V> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (!values.Any())
            throw new ArgumentException(nameof(values), "Can't be empty");
        if (string.IsNullOrEmpty(columnName))
            throw new ArgumentNullException(nameof(columnName));

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