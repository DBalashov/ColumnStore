using System;
using System.Collections.Generic;
using System.Linq;
using FileContainer;
using NUnit.Framework;

namespace ColumnStore.Tests
{
    public abstract class Base
    {
        protected readonly Random r = new(Guid.Empty.GetHashCode());

        protected DateTime SD = new(2020, 1, 1);
        protected DateTime ED = new(2020, 12, 1);

        protected PersistentColumnStore GetStore() => new(new InMemoryContainer(1024), CDTUnit.Month);

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
            filter(keys, from, to)
                .ToDictionary(p => p,
                              p => ((DateTime) p).TimeOfDay.TotalMilliseconds);

        protected Dictionary<CDT, TimeSpan> GetTimeSpans(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
                .ToDictionary(p => p,
                              p => ((DateTime) p).TimeOfDay);

        protected Dictionary<CDT, DateTime> GetDateTimes(CDT[] keys, CDT? from = null, CDT? to = null) =>
            filter(keys, from, to)
                .ToDictionary(p => p,
                              p => (DateTime) p);


        protected void AssertIsEqual<T>(Dictionary<CDT, T> a1, Dictionary<CDT, T> a2)
        {
            Assert.IsTrue(a1.Count == a2.Count, "Length mismatch: {0} vs {1}", a1.Count, a2.Count);

            foreach (var item in a1)
            {
                if (!a2.TryGetValue(item.Key, out var second))
                    Assert.Fail("Key not found: {0}", item.Key);

                if (Equals(item.Value, default) && Equals(second, default)) continue;
                Assert.IsTrue(Equals(item.Value, second), "Elements is not equal [{0:s}]: {1} vs {2}", (DateTime) new CDT(item.Key), item.Value, second);
            }
        }
    }
}