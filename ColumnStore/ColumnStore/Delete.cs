using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 8604

namespace ColumnStore;

public partial class PersistentColumnStore
{
    /// <summary> Remove data for specified ranges. Ranges can be overlapped </summary>
    /// <param name="columnName">case insensitive column name</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException"></exception>
    public void Delete<T>(string columnName, params CDTRange[] ranges)
    {
        if (string.IsNullOrEmpty(columnName))
            throw new ArgumentException("Can't be null", nameof(columnName));

        var needForDelete = new List<string>();
        var needForWrite  = new Dictionary<string, Dictionary<int, T>?>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var range in ranges)
        {
            foreach (var portion in range.GetRanges(Unit))
            {
                var sectionName = Path.BuildSectionName(columnName, portion.Key.Value);

                if (!needForWrite.TryGetValue(sectionName, out var existingData) || existingData == null)
                {
                    var data = Container[sectionName];
                    if (data == null) continue;

                    existingData = data.Unpack<T>(Compressed);
                }

                var newData = new Dictionary<int, T>(existingData.Count);
                foreach (var item in existingData.Where(p => !portion.Range.InRange(p.Key)))
                    newData.Add(item.Key, item.Value);
                
                needForWrite[sectionName] = newData.Any() ? newData : null;
                if (!newData.Any())
                    needForDelete.Add(sectionName);
            }
        }

        if (needForDelete.Any())
            Container.Delete(needForDelete.ToArray());

        var forWrite = needForWrite.Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value.Pack(Compressed));
        if (forWrite.Any())
            Container.Put(forWrite);
    }
}