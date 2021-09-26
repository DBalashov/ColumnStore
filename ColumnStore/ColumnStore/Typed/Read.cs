using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore
{
    partial class ColumnStoreTyped : IColumnStoreTyped
    {
        readonly PersistentColumnStore ps;

        internal ColumnStoreTyped(PersistentColumnStore ps) => this.ps = ps;

        public Dictionary<CDT, V> Read<V>(CDT from, CDT to, string columnName)
        {
            if (from >= to)
                throw new ArgumentException($"Invalid values: {from} >= {to}");
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Can't be empty", nameof(columnName));

            var r = new Dictionary<CDT, V>();

            var requireRange = new CDTRange(from, to);
            foreach (var range in new CDTRange(from, to).GetRanges(ps.Unit))
            {
                var sectionName = ps.Path.BuildSectionName(columnName, range.Key.Value);

                var data = ps.Container[sectionName];
                if (data == null) continue;

                var unpacked = data.Unpack<V>(ps.Compressed);
                foreach (var item in unpacked.Where(p => requireRange.InRange(p.Key)))
                    r.Add(new CDT(item.Key), item.Value);
            }

            return r;
        }
    }
}