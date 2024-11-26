using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace ColumnStore.Tests.Entity
{
    public class WriteEntity_ReadColumn : Base
    {
        CDT[]                         keys;
        Dictionary<CDT, SimpleEntity> entities;

        [SetUp]
        public void Setup()
        {
            keys     = GetKeys();
            entities = keys.ToDictionary(p => p, p => new SimpleEntity(p));
            TestContext.WriteLine($"Keys: {keys.Length}");
        }

        void checkRead<T>(PersistentColumnStore store, string columnName, Func<SimpleEntity, T> getProperty)
        {
            var values = entities.ToDictionary(p => p.Key, p => getProperty(p.Value));
            store.Typed.Write(columnName, values);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var sd     = keys.First();
            var ed     = keys.Last().Add(TimeSpan.FromSeconds(1));
            var result = store.Entity.Read<SimpleEntity>(sd, ed);
            Assert.That(result != null);
            Assert.That(result.Any());
            Assert.That(result.Count == values.Count);
            Assert.That(!result.Keys.Except(keys).Any(), "Keys not matched");

            foreach (var v in values)
            {
                if (!result.TryGetValue(v.Key, out var entity))
                    Assert.Fail($"Can't found key [{(DateTime) v.Key:s}]");

                var entityValue = getProperty(entity);
                Assert.That(entityValue.Equals(v.Value), $"Item not equals [{(DateTime) v.Key:s}]");
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadByte(bool compressed) => checkRead(GetStore(compressed), "ColumnByte", entity => entity.ColumnByte);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadSByte(bool compressed) => checkRead(GetStore(compressed), "ColumnSByte", entity => entity.ColumnSByte);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadBoolean(bool compressed) => checkRead(GetStore(compressed), "ColumnBool", entity => entity.ColumnBool);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadDouble(bool compressed) => checkRead(GetStore(compressed), "ColumnDouble", entity => entity.ColumnDouble);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadGuid(bool compressed) => checkRead(GetStore(compressed), "ColumnGuid", entity => entity.ColumnGuid);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadInt(bool compressed) => checkRead(GetStore(compressed), "ColumnInt", entity => entity.ColumnInt);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadInt16(bool compressed) => checkRead(GetStore(compressed), "ColumnInt16", entity => entity.ColumnInt16);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadInt64(bool compressed) => checkRead(GetStore(compressed), "ColumnInt64", entity => entity.ColumnInt64);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadUInt(bool compressed) => checkRead(GetStore(compressed), "ColumnUInt", entity => entity.ColumnUInt);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadUInt16(bool compressed) => checkRead(GetStore(compressed), "ColumnUInt16", entity => entity.ColumnUInt16);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadUInt64(bool compressed) => checkRead(GetStore(compressed), "ColumnUInt64", entity => entity.ColumnUInt64);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadString(bool compressed) => checkRead(GetStore(compressed), "ColumnString", entity => entity.ColumnString);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadDateTime(bool compressed) => checkRead(GetStore(compressed), "ColumnDateTime", entity => entity.ColumnDateTime);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteEntityReadTimeSpan(bool compressed) => checkRead(GetStore(compressed), "ColumnTimeSpan", entity => entity.ColumnTimeSpan);
    }
}