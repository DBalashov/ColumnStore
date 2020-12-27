using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore
    {
        /// <summary> Write data to column data </summary>
        /// <param name="columnName">case insensitive</param>
        /// <exception cref="ArgumentException"></exception>
        public void WriteUntyped([NotNull] string columnName, [NotNull] UntypedColumn newData)
        {
            if (newData == null)
                throw new ArgumentException("Can't be null", nameof(newData));

            var entries = new Dictionary<string, byte[]>();

            foreach (var range in newData.Keys.GetRange(Unit))
            {
                var sectionName = Path.BuildSectionName(columnName, range.Key.Value);

                var data = Container[sectionName];
                entries.Add(sectionName, data != null
                                ? data.Unpack(Compressed).MergeWithReplace(range, newData).Pack(null, Compressed)
                                : newData.Pack(range, Compressed));
            }

            Container.Put(entries);
        }

        /// <summary> Write data to multiple columns data </summary>
        /// <param name="newData">keys must be case insensitive</param>
        public void WriteUntyped([NotNull] Dictionary<string, UntypedColumn> newData)
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

                foreach (var range in newDataItem.Value.Keys.GetRange(Unit))
                {
                    var sectionName = Path.BuildSectionName(newDataItem.Key, range.Key.Value);

                    var data = Container[sectionName];
                    entries.Add(sectionName, data != null
                                    ? data.Unpack(Compressed).MergeWithReplace(range, newDataItem.Value).Pack(null, Compressed)
                                    : newDataItem.Value.Pack(range, Compressed));
                }

                Container.Put(entries);
            }
        }
    }
}