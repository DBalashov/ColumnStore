using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace ColumnStore
{
    public class CDTRange
    {
        public readonly CDT From;
        public readonly CDT To;

        public CDTRange(DateTime sd, DateTime ed)
        {
            From = sd;
            To   = ed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InRange(int value) => From.Value <= value && value < To.Value;

        public IEnumerable<CDTRangeWithKey> GetRanges(CDTUnit unit)
        {
            var from = From.NextNearest(unit);
            yield return new CDTRangeWithKey(From.Trunc(unit), From, new CDT(Math.Min(from.Value, To.Value)));

            var to = To.Trunc(unit);

            while (from < to)
            {
                var next = from.NextNearest(unit);
                yield return new CDTRangeWithKey(from, from, next);

                from = next;
            }

            if (from < To)
                yield return new CDTRangeWithKey(from, from, To);
        }

#if DEBUG
        public override string ToString() => From + " - " + To;
#endif
    }
    
    public class CDTRangeWithKey : CDTRange
    {
        public readonly CDT Key;

        public CDTRangeWithKey(CDT key, CDT sd, CDT ed) : base(sd, ed) => Key = key;

#if DEBUG
        public override string ToString() => Key + ":  " + base.ToString();
#endif
    }
}