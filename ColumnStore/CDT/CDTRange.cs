using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace ColumnStore;

public readonly struct CDTRange(DateTime sd, DateTime ed)
{
    public readonly CDT From = sd;
    public readonly CDT To   = ed;

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
    [ExcludeFromCodeCoverage]
    public override string ToString() => From + " - " + To;
#endif
}

public readonly struct CDTKeyRange(CDT key, CDT sd, CDT ed)
{
    public readonly CDT      Key   = key;
    public readonly CDTRange Range = new(sd, ed);

#if DEBUG
    [ExcludeFromCodeCoverage]
    public override string ToString() => Key + ":  " + base.ToString();
#endif
}