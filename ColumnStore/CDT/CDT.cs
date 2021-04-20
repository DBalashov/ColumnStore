using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace ColumnStore
{
    public enum CDTUnit
    {
        Year,
        Month,
        Day,
        Hour,
        Minute
    }

    /// <summary>
    /// CDT = CompactDateTime - 4-bytes (int32) date-time (used as key)
    ///
    /// 1-second precision
    /// 
    /// min value: 2010.1.1
    /// max value: 2068.1.1
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CDT : IComparable<CDT>, IComparable
    {
        const int MIN_YEAR = 2000;
        const int MAX_YEAR = 2068;

        static DateTime startDT = new(MIN_YEAR, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public readonly int Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CDT(DateTime dt) =>
            Value = dt.Year < MIN_YEAR || dt.Year > MAX_YEAR ? 0 : (int) dt.Subtract(startDT).TotalSeconds;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CDT(int value) =>
            Value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CDT(int year, int month, int day) =>
            Value = (int) new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Utc).Subtract(startDT).TotalSeconds;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CDT Trunc(CDTUnit to)
        {
            if (Value == 0)
                return new CDT(0);

            var dt = (DateTime) this;
            return to switch
            {
                CDTUnit.Year => new CDT(dt.Year, 1, 1),
                CDTUnit.Month => new CDT(dt.Year, dt.Month, 1),
                CDTUnit.Day => new CDT(dt.Year, dt.Month, dt.Day),
                CDTUnit.Hour => new CDT(new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc)),
                CDTUnit.Minute => new CDT(new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, DateTimeKind.Utc)),
                _ => throw new NotSupportedException(to.ToString())
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CDT Add(TimeSpan ts) => new (Value + (int) ts.TotalSeconds);

        /// <summary> return next time unit from current (next minute, hour, month, ...) </summary>
        public CDT NextNearest(CDTUnit to)
        {
            var dt = (DateTime) Trunc(to);
            return to switch
            {
                CDTUnit.Minute => dt.AddMinutes(1),
                CDTUnit.Hour => dt.AddHours(1),
                CDTUnit.Day => dt.AddDays(1),
                CDTUnit.Month => dt.AddMonths(1),
                CDTUnit.Year => dt.AddYears(1),
                _ => throw new NotSupportedException(to.ToString())
            };
        }

        #region DateTime <-> CDT, TimeSpan <- CDT

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator CDT(DateTime dt) => new(dt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DateTime(CDT dt) => dt.Value == 0
            ? DateTime.MinValue
            : startDT.AddSeconds(dt.Value);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static explicit operator TimeSpan(CDT dt) => dt.Value == 0
        //     ? TimeSpan.Zero
        //     : ((DateTime) dt).TimeOfDay;

        #endregion

        #region +  -  ==  !=

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static CDT operator +(CDT to, int seconds) => new CDT(to.Value + seconds);
        //
        // public static CDT operator -(CDT to, int seconds) => new CDT(to.Value - seconds);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CDT x, CDT y) => x.Value == y.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CDT x, CDT y) => x.Value != y.Value;

        #endregion

        #region >  <  >=  <=

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(CDT x, CDT y) => x.Value < y.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(CDT x, CDT y) => x.Value > y.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(CDT x, CDT y) => x.Value <= y.Value;

        public static bool operator >=(CDT x, CDT y) => x.Value >= y.Value;

        #endregion

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj) => obj is CDT cdt && cdt.Value == Value;

        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => Value;

        public int CompareTo(object obj)   => obj is CDT cdt ? Value.CompareTo(cdt.Value) : -1;
        public int CompareTo(CDT    other) => Value.CompareTo(other.Value);

#if DEBUG
        public override string ToString() => ((DateTime) this).ToString("yyyy-MM-dd HH:mm:ss");
#endif
    }
}