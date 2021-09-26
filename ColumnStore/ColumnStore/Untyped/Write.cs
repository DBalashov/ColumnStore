using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore
{
    partial class ColumnStoreUntyped
    {
        public void Write(string columnName, UntypedColumn newData)
        {
            if (newData == null)
                throw new ArgumentException("Can't be null", nameof(newData));

            var entries = new Dictionary<string, byte[]>();

            foreach (var range in newData.Keys.GetRange(ps.Unit))
            {
                var sectionName = ps.Path.BuildSectionName(columnName, range.Key.Value);

                var data = ps.Container[sectionName];
                entries.Add(sectionName, data != null
                                ? data.Unpack(ps.Compressed).MergeWithReplace(range.Range, newData).Pack(null, ps.Compressed)
                                : newData.Pack(range.Range, ps.Compressed));
            }

            ps.Container.Put(entries);
        }

        public void Write(Dictionary<string, UntypedColumn> newData)
        {
            if (newData == null)
                throw new ArgumentException("Can't be null", nameof(newData));
            if (!newData.Any())
                throw new ArgumentException("Can't be empty", nameof(newData));
            if (newData.Any(c => c.Value == null))
                throw new ArgumentException("Value can't be null", nameof(newData));

            foreach (var newDataItem in newData)
            {
                var entries = new Dictionary<string, byte[]>(); // todo ^entries

                foreach (var range in newDataItem.Value.Keys.GetRange(ps.Unit))
                {
                    var sectionName = ps.Path.BuildSectionName(newDataItem.Key, range.Key.Value);

                    var data = ps.Container[sectionName];
                    entries.Add(sectionName, data != null
                                    ? data.Unpack(ps.Compressed).MergeWithReplace(range.Range, newDataItem.Value).Pack(null, ps.Compressed)
                                    : newDataItem.Value.Pack(range.Range, ps.Compressed));
                }

                ps.Container.Put(entries);
            }
        }
    }
}