using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore;

static class DictionarizeExtenders
{
    internal static DictionarizeResult<T> Dictionarize<T>(this Array values, Range range, T nullReplaceTo = default(T)!) where T : notnull
    {
        var v       = (T[]) values;
        var r       = new Dictionary<T, int>(); // value : index
        var indexes = new int[range.Length()];
        for (var i = range.Start.Value; i < range.End.Value; i++)
        {
            var item = v[i] ?? nullReplaceTo;
            if (!r.TryGetValue(item, out var index))
            {
                index = r.Count;
                r.Add(item, index);
            }

            indexes[i - range.Start.Value] = index;
        }

        return new DictionarizeResult<T>(indexes, r.OrderBy(p => p.Value).Select(p => p.Key).ToArray());
    }
}

readonly struct DictionarizeResult<T>
{
    internal readonly T[]   Values;
    internal readonly int[] Indexes;

    internal DictionarizeResult(int[] indexes, T[] values)
    {
        Indexes = indexes;
        Values  = values;
    }
}