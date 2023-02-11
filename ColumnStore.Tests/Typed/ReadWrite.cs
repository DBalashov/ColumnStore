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

        void writeRead<T>(bool compressed, Func<Dictionary<CDT, T>> getData, Func<CDT, CDT, Dictionary<CDT, T>> getDataPart)
        {
            var store      = GetStore(compressed);
            var data       = getData();
            var columnName = typeof(T).Name;
            store.Typed.Write(columnName, getData());
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            readSimple(columnName, store, data);
            readRange(columnName, store, getDataPart);
        }

        void readSimple<T>(string columnName, PersistentColumnStore store, Dictionary<CDT, T> originalValues)
        {
            var sd = keys.First();
            var ed = keys.Last().Add(TimeSpan.FromSeconds(1));

            var result = store.Typed.Read<T>(sd, ed, columnName);
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

            var result = store.Typed.Read<T>(part.First().Key, part.Last().Key.Add(TimeSpan.FromSeconds(1)), columnName);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Keys);
            Assert.IsNotEmpty(result.Values);

            Assert.IsEmpty(result.Keys.Except(keys), "Keys not matched");
            AssertIsEqual(result, part);
        }

        #endregion

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteByte(bool compressed) => writeRead(compressed, () => GetBytes(keys), (sd, ed) => GetBytes(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteSByte(bool compressed) => writeRead(compressed, () => GetSBytes(keys), (sd, ed) => GetSBytes(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteBoolean(bool compressed) => writeRead(compressed, () => GetBoolean(keys), (sd, ed) => GetBoolean(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteDouble(bool compressed) => writeRead(compressed, () => GetDoubles(keys), (sd, ed) => GetDoubles(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteGuid(bool compressed) => writeRead(compressed, () => GetGuids(keys), (sd, ed) => GetGuids(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteInt(bool compressed) => writeRead(compressed, () => GetInts(keys), (sd, ed) => GetInts(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteUInt(bool compressed) => writeRead(compressed, () => GetUInts(keys), (sd, ed) => GetUInts(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteInt16(bool compressed) => writeRead(compressed, () => GetInt16s(keys), (sd, ed) => GetInt16s(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteUInt16(bool compressed) => writeRead(compressed, () => GetUInt16s(keys), (sd, ed) => GetUInt16s(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteInt64(bool compressed) => writeRead(compressed, () => GetInt64s(keys), (sd, ed) => GetInt64s(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteUInt64(bool compressed) => writeRead(compressed, () => GetUInt64s(keys), (sd, ed) => GetUInt64s(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteString(bool compressed) => writeRead(compressed, () => GetStrings(keys), (sd, ed) => GetStrings(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteDateTime(bool compressed) => writeRead(compressed, () => GetDateTimes(keys), (sd, ed) => GetDateTimes(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteTimeSpan(bool compressed) => writeRead(compressed, () => GetTimeSpans(keys), (sd, ed) => GetTimeSpans(keys, sd, ed));
    }
}