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

            var vInts    = keys.Convert().ToDictionary(p => (CDT) p, p => p.Minute + p.Second + p.Day);
            var vUInts   = keys.Convert().ToDictionary(p => (CDT) p, p => (uint) (p.Minute   + p.Second + p.Day));
            var vInt16s  = keys.Convert().ToDictionary(p => (CDT) p, p => (short) (p.Minute  + p.Second + p.Day));
            var vUInt16s = keys.Convert().ToDictionary(p => (CDT) p, p => (ushort) (p.Minute + p.Second + p.Day));
            var vInt64s = keys.Convert().ToDictionary(p => (CDT) p, p =>
                                                                    {
                                                                        Int64 x = p.Minute + p.Second + p.Day + 1;
                                                                        return x | ((x + 2) << 32);
                                                                    });

            var vUInt64s = keys.Convert().ToDictionary(p => (CDT) p, p =>
                                                                     {
                                                                         Int64 x = p.Minute + p.Second + p.Day + 1;
                                                                         return (UInt64) (x | ((x + 2) << 32));
                                                                     });

            var vBytes     = keys.Convert().ToDictionary(p => (CDT) p, p => (byte) (p.Minute  + p.Second + p.Day));
            var vSBytes    = keys.Convert().ToDictionary(p => (CDT) p, p => (sbyte) (p.Second + p.Day));
            var vBooleans  = keys.Convert().ToDictionary(p => (CDT) p, p => (p.Minute + p.Second + p.Day) % p.Day > 0);
            var vDoubles   = keys.Convert().ToDictionary(p => (CDT) p, p => p.TimeOfDay.TotalMilliseconds);
            var vFloats    = keys.Convert().ToDictionary(p => (CDT) p, p => (float) (p.TimeOfDay.TotalMilliseconds));
            var vDecimals  = keys.Convert().ToDictionary(p => (CDT) p, p => (decimal) p.TimeOfDay.TotalMilliseconds);
            var vTimeSpans = keys.Convert().ToDictionary(p => (CDT) p, p => p.TimeOfDay);
            var vStrings   = keys.Convert().ToDictionary(p => (CDT) p, p => "Item Address " + p.ToString("yyyyMMdd") + "/" + p.Month + "/" + p.Minute + "/" + p.Day);
            var vGuids     = keys.Convert().ToDictionary(p => (CDT) p, p => new Guid((uint) p.Year, 0, 0, (byte) p.Year, (byte) p.Month, (byte) p.Day, (byte) p.Hour, 0, 0, 0, 0));
            var vDateTimes = keys.Convert().ToDictionary(p => (CDT) p, p => p);
            var vHalfs     = keys.Convert().ToDictionary(p => (CDT) p, p => (Half) (p.TimeOfDay.TotalMilliseconds / 2.123));
            var vDateOnlys = keys.Convert().ToDictionary(p => (CDT) p, DateOnly.FromDateTime);
            var vTimeOnlys = keys.Convert().ToDictionary(p => (CDT) p, TimeOnly.FromDateTime);
            for (var i = 0; i < 2; i++)
            {
                values.Add("Ints_"      + i, vInts.Values.ToArray());
                values.Add("UInts_"     + i, vUInts.Values.ToArray());
                values.Add("Int16s_"    + i, vInt16s.Values.ToArray());
                values.Add("UInt16s_"   + i, vUInt16s.Values.ToArray());
                values.Add("Int64s_"    + i, vInt64s.Values.ToArray());
                values.Add("UInt64s_"   + i, vUInt64s.Values.ToArray());
                values.Add("Bytes_"     + i, vBytes.Values.ToArray());
                values.Add("SBytes_"    + i, vSBytes.Values.ToArray());
                values.Add("Doubles_"   + i, vDoubles.Values.ToArray());
                values.Add("Floats_"    + i, vFloats.Values.ToArray());
                values.Add("Guids_"     + i, vGuids.Values.ToArray());
                values.Add("Strings_"   + i, vStrings.Values.ToArray());
                values.Add("DateTimes_" + i, vDateTimes.Values.ToArray());
                values.Add("TimeSpans_" + i, vTimeSpans.Values.ToArray());
                values.Add("Boolean_"   + i, vBooleans.Values.ToArray());
                values.Add("Half_"      + i, vHalfs.Values.ToArray());
                values.Add("DateOnlys_" + i, vDateOnlys.Values.ToArray());
                values.Add("TimeOnlys_" + i, vTimeOnlys.Values.ToArray());
                values.Add("Decimals_"  + i, vDecimals.Values.ToArray());
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
        [TestCase(false)]
        [TestCase(true)]
        public void WriteSingle(bool compressed)
        {
            var store = GetStore(compressed);

            var data = values.ToDictionary(p => p.Key, p => new UntypedColumn(keys, p.Value));
            store.Untyped.Write(data);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var result = store.Untyped.Read(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)), values.Keys.ToArray());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Keys.Except(values.Keys).Any());
            Assert.IsFalse(values.Keys.Except(result.Keys).Any());

            foreach (var item in result)
                checkRead(item.Value, data[item.Key]);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteMerge(bool compressed)
        {
            var store = GetStore(compressed);

            var data = values.ToDictionary(p => p.Key, p => new UntypedColumn(keys, p.Value));
            store.Untyped.Write(data);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var result = store.Untyped.Read(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)), values.Keys.ToArray());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Keys.Except(values.Keys).Any());
            Assert.IsFalse(values.Keys.Except(result.Keys).Any());

            foreach (var item in result)
                checkRead(item.Value, data[item.Key]);
        }

        // todo add read as typed column
    }
}