using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ColumnStore.Tests.Untyped
{
    public class ReadWrite : Base
    {
        CDT[]                     keys;
        Dictionary<string, Array> values;

        [SetUp]
        public void Setup()
        {
            keys   = GetKeys();
            values = new Dictionary<string, Array>();

            var vInts      = keys.Convert().ToDictionary(p => (CDT) p, p => p.Minute + p.Second + p.Day);
            var vBytes     = keys.Convert().ToDictionary(p => (CDT) p, p => (byte) (p.Minute + p.Second + p.Day));
            var vBooleans  = keys.Convert().ToDictionary(p => (CDT) p, p => (p.Minute + p.Second + p.Day) % p.Day > 0);
            var vDoubles   = keys.Convert().ToDictionary(p => (CDT) p, p => p.TimeOfDay.TotalMilliseconds);
            var vTimeSpans = keys.Convert().ToDictionary(p => (CDT) p, p => p.TimeOfDay);
            var vStrings   = keys.Convert().ToDictionary(p => (CDT) p, p => "Item Address " + p.ToString("yyyyMMdd") + "/" + p.Month + "/" + p.Minute + "/" + p.Day);
            var vGuids     = keys.Convert().ToDictionary(p => (CDT) p, p => new Guid((uint) p.Year, 0, 0, (byte) p.Year, (byte) p.Month, (byte) p.Day, (byte) p.Hour, 0, 0, 0, 0));
            var vDateTimes = keys.Convert().ToDictionary(p => (CDT) p, p => p);
            for (var i = 0; i < 2; i++)
            {
                values.Add("Ints_" + i, vInts.Values.ToArray());
                values.Add("Bytes_" + i, vBytes.Values.ToArray());
                values.Add("Doubles_" + i, vDoubles.Values.ToArray());
                values.Add("Guids_" + i, vGuids.Values.ToArray());
                values.Add("Strings_" + i, vStrings.Values.ToArray());
                values.Add("DateTimes_" + i, vDateTimes.Values.ToArray());
                values.Add("TimeSpans_" + i, vTimeSpans.Values.ToArray());
                values.Add("Boolean_" + i, vBooleans.Values.ToArray());
            }

            TestContext.WriteLine($"Keys: {keys.Length}");
        }

        void checkRead(UntypedColumn c, UntypedColumn orig)
        {
            Assert.IsNotNull(c);

            Assert.IsTrue(c.Keys.Length == orig.Keys.Length, $"Length mismatch: Expected Length={orig.Keys.Length}, returned Length={c.Keys.Length}");
            for (var i = 0; i < c.Keys.Length; i++)
            {
                Assert.IsTrue(c.Keys[i] == orig.Keys[i], $"Keys mismatch: Expected: {c.Keys[i]}, returned: {orig.Keys[i]}");

                var readValue = c.Values.GetValue(i);
                var origValue = orig.Values.GetValue(i);
                if (Equals(readValue, default) && Equals(origValue, default)) continue;
                Assert.IsTrue(Equals(readValue, origValue), "Element mismatch [{0:s}]: Expected: {1}, returned: {2}", (DateTime) c.Keys[i], origValue, readValue);
            }
        }

        [Test]
        public void WriteSingle()
        {
            var store = GetStore();

            var data = values.ToDictionary(p => p.Key, p => new UntypedColumn(keys, p.Value));
            store.WriteUntyped(data);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var result = store.ReadUntyped(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)), values.Keys.ToArray());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Keys.Except(values.Keys).Any());
            Assert.IsFalse(values.Keys.Except(result.Keys).Any());

            foreach (var item in result)
                checkRead(item.Value, data[item.Key]);
        }

        [Test]
        public void WriteMerge()
        {
            var store = GetStore();

            var data = values.ToDictionary(p => p.Key, p => new UntypedColumn(keys, p.Value));
            store.WriteUntyped(data);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var result = store.ReadUntyped(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)), values.Keys.ToArray());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Keys.Except(values.Keys).Any());
            Assert.IsFalse(values.Keys.Except(result.Keys).Any());

            foreach (var item in result)
                checkRead(item.Value, data[item.Key]);
        }

        // todo add read as typed column
    }
}