using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace ColumnStore;

public readonly struct CDTRange
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

    public IEnumerable<CDTKeyRange> GetRanges(CDTUnit unit)
    {
        var from = From.NextNearest(unit);
        yield return new CDTKeyRange(From.Trunc(unit), From, new CDT(Math.Min(from.Value, To.Value)));

        var to = To.Trunc(unit);

        while (from < to)
        {
            var next = from.NextNearest(unit);
            yield return new CDTKeyRange(from, from, next);

            from = next;
        }

        if (from < To)
            yield return new CDTKeyRange(from, from, To);
    }

#if DEBUG
    public override string ToString() => From + " - " + To;
#endif
}

public readonly struct CDTKeyRange
{
    public readonly CDT      Key;
    public readonly CDTRange Range;

    public CDTKeyRange(CDT key, CDT sd, CDT ed)
    {
        Key   = key;
        Range = new CDTRange(sd, ed);
    }

#if DEBUG
    public override string ToString() => Key + ":  " + base.ToString();
#endif
}