using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    static class DictionarizeExtenders
    {
        internal static DictionarizeResult<T> Dictionarize<T>([NotNull] this Array values, Range range)
        {
            var v       = (T[]) values;
            var r       = new Dictionary<T, int>(); // value : index
            var indexes = new int[range.Length];
            for (var i = range.From; i < range.To; i++)
            {
                var item = v[i];
                if (!r.TryGetValue(item, out var index))
                {
                    index = r.Count;
                    r.Add(item, index);
                }

                indexes[i - range.From] = index;
            }

            return new DictionarizeResult<T>(indexes, r.OrderBy(p => p.Value).Select(p => p.Key).ToArray());
        }
    }

    readonly struct DictionarizeResult<T>
    {
        [NotNull] internal readonly T[]   Values;
        [NotNull] internal readonly int[] Indexes;

        internal DictionarizeResult([NotNull] int[] indexes, [NotNull] T[] values)
        {
            Indexes = indexes;
            Values  = values;
        }
    }
}