using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    class Range
    {
        /// <summary> included </summary>
        public readonly int From;

        /// <summary> not included </summary>
        public readonly int To;

        public readonly int Length;

        /// <param name="from">included</param>
        /// <param name="to">not included</param>
        internal Range(int from, int to)
        {
            From   = from;
            To     = to;
            Length = to - from;
        }
        
#if DEBUG
        public override string ToString() => $"{From} - {To}: {Length}";
#endif
    }
    
    sealed class RangeWithKey : Range
    {
        public readonly CDT Key;

        /// <param name="key"></param>
        /// <param name="from">included</param>
        /// <param name="to">not included</param>
        internal RangeWithKey(CDT key, int from, int to) : base(from, to) => Key = key;

#if DEBUG
        public override string ToString() => $"{Key}: {base.ToString()}";
#endif
    }

    static class CDTRangeIndexExtenders
    {
        [NotNull]
        internal static IEnumerable<RangeWithKey> GetRange([NotNull] this CDT[] values, CDTUnit unit)
        {
            if (!values.Any())
                yield break;

            var key        = values[0].Trunc(unit);
            var startIndex = 0;
            for (var i = 1; i < values.Length; i++)
            {
                var currentKey = values[i].Trunc(unit);
                if (currentKey != key)
                {
                    yield return new RangeWithKey(key, startIndex, i);
                    key        = currentKey;
                    startIndex = i;
                }
            }

            yield return new RangeWithKey(key, startIndex, values.Length);
        }
    }
}