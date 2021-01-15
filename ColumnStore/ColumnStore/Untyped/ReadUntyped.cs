using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore
    {
        /// <summary> Read column(s) data from container. Return only founded data for existing column. </summary>
        /// <param name="columnNames">case insensitive</param>
        /// <exception cref="ArgumentException"></exception>
        [NotNull]
        public Dictionary<string, UntypedColumn> ReadUntyped(CDT from, CDT to, params string[] columnNames)
        {
            if (from >= to)
                throw new ArgumentException($"Invalid values: {from} >= {to}");
            if (columnNames.Any(string.IsNullOrEmpty))
                throw new ArgumentException("Column names contain invalid values", nameof(columnNames));

            var r      = new Dictionary<string, UntypedColumn>(StringComparer.InvariantCultureIgnoreCase);
            var ranges = new CDTRange(from, to).GetRanges(Unit).ToArray();
            foreach (var columnName in columnNames.Distinct())
            {
                var acc = new List<UntypedColumn>();
                foreach (var range in ranges)
                {
                    var sectionName = Path.BuildSectionName(columnName, range.Key.Value);

                    var data = Container[sectionName];
                    if (data == null) continue;

                    var unpacked = filterByRange(data.Unpack(Compressed), range);
                    if (unpacked != null)
                        acc.Add(unpacked);
                }

                var item = combine(acc);
                if (item != null)
                    r.Add(columnName, item);
            }

            return r;
        }

        [CanBeNull]
        UntypedColumn filterByRange([NotNull] UntypedColumn unpacked, [NotNull] CDTRange range)
        {
            if (range.To < unpacked.Keys.First() ||
                range.From > unpacked.Keys.Last()) return null;
            
            var indexFrom = Array.BinarySearch(unpacked.Keys, range.From);
            if (indexFrom < 0)
                indexFrom = ~indexFrom;

            var indexTo = Array.BinarySearch(unpacked.Keys, range.To);
            if (indexTo < 0)
                indexTo = ~indexTo;

            if (indexFrom >= 0 || indexTo < unpacked.Keys.Length)
            {
                var newKeys   = new CDT[indexTo - indexFrom];
                var newValues = unpacked.Values.CreateSameType(newKeys.Length);
                Array.Copy(unpacked.Keys, indexFrom, newKeys, 0, indexTo - indexFrom);
                Array.Copy(unpacked.Values, indexFrom, newValues, 0, indexTo - indexFrom);

                return new UntypedColumn(newKeys, newValues);
            }

            return null;
        }

        [CanBeNull]
        UntypedColumn combine([NotNull] List<UntypedColumn> items)
        {
            if (!items.Any())
                return null;

            var totalLength = items.Sum(p => p.Keys.Length);
            var keys        = new CDT[totalLength];
            var values      = items[0].Values.CreateSameType(totalLength);

            var offset = 0;
            foreach (var item in items)
            {
                var len = item.Keys.Length;

                Array.Copy(item.Keys, 0, keys, offset, len);
                Array.Copy(item.Values, 0, values, offset, len);

                offset += len;
            }

            return new UntypedColumn(keys, values);
        }
    }
}