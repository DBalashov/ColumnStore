using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ColumnStore.Tests.Typed
{
    public class Delete : Base
    {
        CDT[] keys;

        [SetUp]
        public void Setup()
        {
            keys = GetKeys();
            TestContext.WriteLine($"Keys: {keys.Length}");
        }

        void delete<T>(bool compressed, Func<Dictionary<CDT, T>> getData, Func<CDT, CDT, Dictionary<CDT, T>> getDataPart)
        {
            var columnName = typeof(T).Name;
            var store      = GetStore(compressed);
            var data       = getData();
            store.Typed.Write(columnName, data);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
            
            var part = getDataPart(keys.Skip(keys.Length / 2).First(),
                                   keys.Skip(keys.Length / 2 + keys.Length / 3).First());

            var range = new CDTRange(part.First().Key, part.Last().Key);
            store.Delete<T>(columnName, range);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
            
            var absentItems = store.Typed.Read<T>(range.From, range.To, columnName);
            Assert.IsNotNull(absentItems);

            var before = store.Typed.Read<T>(data.First().Key, range.From, columnName);
            Assert.IsNotNull(before);
            Assert.IsNotEmpty(before);
            Assert.IsTrue(before.Keys.All(c => c < range.From));

            var after = store.Typed.Read<T>(range.To, keys.Last(), columnName);
            Assert.IsNotNull(after);
            Assert.IsNotEmpty(after);
            Assert.IsTrue(after.Keys.All(c => c >= range.To));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteByte(bool compressed) => delete(compressed, () => GetBytes(keys), (sd, ed) => GetBytes(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteSByte(bool compressed) => delete(compressed, () => GetSBytes(keys), (sd, ed) => GetSBytes(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteInt16s(bool compressed) => delete(compressed, () => GetInt16s(keys), (sd, ed) => GetInt16s(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteUInt16s(bool compressed) => delete(compressed, () => GetUInt16s(keys), (sd, ed) => GetUInt16s(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteBoolean(bool compressed) => delete(compressed, () => GetBytes(keys), (sd, ed) => GetBytes(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteDouble(bool compressed) => delete(compressed, () => GetDoubles(keys), (sd, ed) => GetDoubles(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteGuid(bool compressed) => delete(compressed, () => GetGuids(keys), (sd, ed) => GetGuids(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteInt(bool compressed) => delete(compressed, () => GetInts(keys), (sd, ed) => GetInts(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteUInt(bool compressed) => delete(compressed, () => GetUInts(keys), (sd, ed) => GetUInts(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteInt64(bool compressed) => delete(compressed, () => GetInt64s(keys), (sd, ed) => GetInt64s(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteUInt64(bool compressed) => delete(compressed, () => GetUInt64s(keys), (sd, ed) => GetUInt64s(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteString(bool compressed) => delete(compressed, () => GetStrings(keys), (sd, ed) => GetStrings(keys, sd, ed));

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteDateTime(bool compressed) => delete(compressed, () => GetDateTimes(keys), (sd, ed) => GetDateTimes(keys, sd, ed));
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteTimeSpan(bool compressed) => delete(compressed, () => GetTimeSpans(keys), (sd, ed) => GetTimeSpans(keys, sd, ed));
    }
}