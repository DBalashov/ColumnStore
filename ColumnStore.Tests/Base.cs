using System;
using System.Collections.Generic;
using System.Linq;
using FileContainer;
using NUnit.Framework;

namespace ColumnStore.Tests
{
    public abstract class Base
    {
        protected DateTime SD = new(2020, 1, 1);
        protected DateTime ED = new(2020, 12, 1);

        protected PersistentColumnStore GetStore(bool compressed) => new(new InMemoryContainer(new PersistentContainerSettings(1024)), CDTUnit.Month, compressed);

        protected CDT[] GetKeys(int everyMinute = 10)
        {
            var keys = new List<CDT>();

            var dt = SD;
            while (dt < ED)
            {
                keys.Add(dt);
                dt = dt.AddMinutes(everyMinute);
            }

            return keys.ToArray();
        }

        CDT[] filter(CDT[] keys, CDT? from = null, CDT? to = null)
        {
            if (from == null && to == null) return keys;
            return keys.Where(p => (from == null || from <= p) && (to == null || p < to)).ToArray();
        }

        protected Dictionary<CDT, byte> GetBytes(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return (byte) (d.Minute + d.Second + d.Day);
                             });

        protected Dictionary<CDT, sbyte> GetSBytes(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return (sbyte) (d.Second + d.Day);
                             });

        protected Dictionary<CDT, bool> GetBoolean(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return (d.Minute + d.Second + d.Day) % d.Day > 0;
                             });

        protected Dictionary<CDT, int> GetInts(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return d.Minute + d.Second + d.Day;
                             });

        protected Dictionary<CDT, uint> GetUInts(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return (uint) (d.Minute + d.Second + d.Day);
                             });

        protected Dictionary<CDT, short> GetInt16s(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return (short) (d.Minute + d.Second + d.Day + 1);
                             });

        protected Dictionary<CDT, ushort> GetUInt16s(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return (ushort) (d.Minute + d.Second + d.Day + 1);
                             });

        protected Dictionary<CDT, Int64> GetInt64s(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var   d = (DateTime) p;
                                 Int64 x = d.Minute + d.Second + d.Day + 2;
                                 return x | ((x + 3) << 32);
                             });

        protected Dictionary<CDT, UInt64> GetUInt64s(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var   d = (DateTime) p;
                                 Int64 x = d.Minute + d.Second + d.Day + 2;
                                 return (UInt64) (x | ((x + 3) << 32));
                             });

        protected Dictionary<CDT, Guid> GetGuids(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return new Guid((uint) d.Year, 0, 0, (byte) d.Year, (byte) d.Month, (byte) d.Day, (byte) d.Hour, 0, 0, 0, 0);
                             });

        protected Dictionary<CDT, string> GetStrings(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var d = (DateTime) p;
                                 return "Item Address " + d.ToString("yyyyMMdd") + "/" + d.Month + "/" + d.Minute + "/" + d.Day;
                             });

        protected Dictionary<CDT, double> GetDoubles(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to).ToDictionary(p => p, p => ((DateTime) p).TimeOfDay.TotalMilliseconds);

        protected Dictionary<CDT, float> GetFloats(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to).ToDictionary(p => p, p => (float) ((DateTime) p).TimeOfDay.TotalMilliseconds);

        protected Dictionary<CDT, decimal> GetDecimals(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to).ToDictionary(p => p, p => (decimal) ((DateTime) p).TimeOfDay.TotalMilliseconds);

        protected Dictionary<CDT, TimeSpan> GetTimeSpans(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to).ToDictionary(p => p, p => ((DateTime) p).TimeOfDay);

        protected Dictionary<CDT, DateOnly> GetDateOnlys(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to).ToDictionary(p => p, p => DateOnly.FromDateTime(p));

        protected Dictionary<CDT, TimeOnly> GetTimeOnlys(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p =>
                             {
                                 var ts = ((DateTime) p).TimeOfDay;
                                 return new TimeOnly(ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                             });

        protected Dictionary<CDT, Half> GetHalfs(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
               .ToDictionary(p => p,
                             p => (Half) ((DateTime) p).TimeOfDay.TotalMilliseconds);

        protected Dictionary<CDT, DateTime> GetDateTimes(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to).ToDictionary(p => p, p => (DateTime) p);

        protected void AssertIsEqual<T>(Dictionary<CDT, T> result, Dictionary<CDT, T> originalValues)
        {
            Assert.That(result.Count == originalValues.Count, $"Length mismatch: result.Count={result.Count} vs originalValues.Count={originalValues.Count}");

            foreach (var item in result)
            {
                if (!originalValues.TryGetValue(item.Key, out var second))
                    Assert.Fail($"Key not found in result: {item.Key}");

                if (Equals(item.Value, default) && Equals(second, default)) continue;
                Assert.That(Equals(item.Value, second), $"Elements is not equal [{(DateTime) new CDT(item.Key):s}]: claimed={item.Value} vs original={second}");
            }
        }
    }
}