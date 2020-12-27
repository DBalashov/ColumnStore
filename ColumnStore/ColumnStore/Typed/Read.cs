using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore
    {
        /// <summary> Read column data with type V from container. Return empty dictionary if column or no data found. </summary>
        /// <param name="columnName">case insensitive column name</param>
        /// <exception cref="ArgumentException"></exception>
        [NotNull]
        public Dictionary<CDT, V> Read<V>(CDT from, CDT to, string columnName)
        {
            if (from >= to)
                throw new ArgumentException($"Invalid values: {from} >= {to}");
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Can't be empty", nameof(columnName));

            var r = new Dictionary<CDT, V>();

            var requireRange = new CDTRange(from, to);
            foreach (var range in new CDTRange(from, to).GetRanges(Unit))
            {
                var sectionName = Path.BuildSectionName(columnName, range.Key.Value);

                var data = Container[sectionName];
                if (data == null) continue;

                var unpacked = data.Unpack<V>(Compressed);
                foreach (var item in unpacked.Where(p => requireRange.InRange(p.Key)))
                    r.Add(new CDT(item.Key), item.Value);
            }

            return r;
        }
    }
}