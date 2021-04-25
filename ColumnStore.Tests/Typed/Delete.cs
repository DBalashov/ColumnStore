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

        void delete<T>(Func<Dictionary<CDT, T>> getData, Func<CDT, CDT, Dictionary<CDT, T>> getDataPart)
        {
            var columnName = typeof(T).Name;
            var store      = GetStore();
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
        public void DeleteByte() =>
            delete(() => GetBytes(keys),
                   (sd, ed) => GetBytes(keys, sd, ed));

        [Test]
        public void DeleteBoolean() =>
            delete(() => GetBytes(keys),
                   (sd, ed) => GetBytes(keys, sd, ed));

        [Test]
        public void DeleteDouble() =>
            delete(() => GetDoubles(keys),
                   (sd, ed) => GetDoubles(keys, sd, ed));

        [Test]
        public void DeleteGuid() =>
            delete(() => GetGuids(keys),
                   (sd, ed) => GetGuids(keys, sd, ed));

        [Test]
        public void DeleteInt() =>
            delete(() => GetInts(keys),
                   (sd, ed) => GetInts(keys, sd, ed));

        [Test]
        public void DeleteString() =>
            delete(() => GetStrings(keys),
                   (sd, ed) => GetStrings(keys, sd, ed));

        [Test]
        public void DeleteDateTime() =>
            delete(() => GetDateTimes(keys),
                   (sd, ed) => GetDateTimes(keys, sd, ed));

        [Test]
        public void DeleteTimeSpan() =>
            delete(() => GetTimeSpans(keys),
                   (sd, ed) => GetTimeSpans(keys, sd, ed));
    }
}