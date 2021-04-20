using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ColumnStore.Tests.Typed
{
    public class ReadWrite : Base
    {
        CDT[] keys;

        [SetUp]
        public void Setup()
        {
            keys = GetKeys();
            TestContext.WriteLine($"Keys: {keys.Length}");
        }

        #region writeRead / readSimple / readRange

        void writeRead<T>(Func<Dictionary<CDT, T>> getData, Func<CDT, CDT, Dictionary<CDT, T>> getDataPart)
        {
            var store      = GetStore();
            var data       = getData();
            var columnName = typeof(T).Name;
            store.Write(columnName, getData());
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            readSimple(columnName, store, data);
            readRange(columnName, store, getDataPart);
        }

        void readSimple<T>(string columnName, PersistentColumnStore store, Dictionary<CDT, T> originalValues)
        {
            var sd = keys.First();
            var ed = keys.Last().Add(TimeSpan.FromSeconds(1));

            var result = store.Read<T>(sd, ed, columnName);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.IsEmpty(result.Keys.Except(keys), "Keys not matched");
            AssertIsEqual(result, originalValues);
        }

        // /// <summary> read of random part of data </summary>
        void readRange<T>(string columnName, PersistentColumnStore store, Func<CDT, CDT, Dictionary<CDT, T>> getDataPart)
        {
            var part = getDataPart(keys.Skip(keys.Length / 2).First(),
                                   keys.Skip(keys.Length / 2 + keys.Length / 3).First());
            
            var result = store.Read<T>(part.First().Key, part.Last().Key.Add(TimeSpan.FromSeconds(1)), columnName);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Keys);
            Assert.IsNotEmpty(result.Values);

            Assert.IsEmpty(result.Keys.Except(keys), "Keys not matched");
            AssertIsEqual(result, part);
        }

        #endregion

        [Test]
        public void WriteByte()
        {
            writeRead(() => GetBytes(keys),
                      (sd, ed) => GetBytes(keys, sd, ed));
        }

        [Test]
        public void WriteDouble()
        {
            writeRead(() => GetDoubles(keys),
                      (sd, ed) => GetDoubles(keys, sd, ed));
        }

        [Test]
        public void WriteGuid()
        {
            writeRead(() => GetGuids(keys),
                      (sd, ed) => GetGuids(keys, sd, ed));
        }

        [Test]
        public void WriteInt()
        {
            writeRead(() => GetInts(keys),
                      (sd, ed) => GetInts(keys, sd, ed));
        }

        [Test]
        public void WriteString()
        {
            writeRead(() => GetStrings(keys),
                      (sd, ed) => GetStrings(keys, sd, ed));
        }

        [Test]
        public void WriteDateTime()
        {
            writeRead(() => GetDateTimes(keys),
                      (sd, ed) => GetDateTimes(keys, sd, ed));
        }

        [Test]
        public void WriteTimeSpan()
        {
            writeRead(() => GetTimeSpans(keys),
                      (sd, ed) => GetTimeSpans(keys, sd, ed));
        }
    }
}