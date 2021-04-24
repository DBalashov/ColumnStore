using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ColumnStore.Tests.Entity
{
    public class WriteColumn_ReadEntity : Base
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
            store.WriteEntity(entities);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var items = store.Read<T>(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)), columnName);
            Assert.IsNotNull(items);
            Assert.IsNotEmpty(items);
            Assert.IsTrue(items.Count == entities.Count);
            Assert.IsEmpty(items.Keys.Except(keys), "Keys not matched");

            foreach (var entity in entities)
            {
                if (!items.TryGetValue(entity.Key, out var item))
                    Assert.Fail("Can't found key [{0:s}]", (DateTime) entity.Key);

                var value = getProperty(entity.Value);
                Assert.IsNotNull(value, "Entity is null [{0:s}]", (DateTime) entity.Key);
                Assert.IsTrue(item.Equals(value), "Item not equals [{0:s}]", (DateTime) entity.Key);
            }
        }

        [Test]
        public void WriteByteReadEntity() => checkRead(GetStore(), "ColumnByte", entity => entity.ColumnByte);

        [Test]
        public void WriteBooleanReadEntity() => checkRead(GetStore(), "ColumnBool", entity => entity.ColumnBool);

        [Test]
        public void WriteDoubleReadEntity() => checkRead(GetStore(), "ColumnDouble", entity => entity.ColumnDouble);

        [Test]
        public void WriteGuidReadEntity() => checkRead(GetStore(), "ColumnGuid", entity => entity.ColumnGuid);

        [Test]
        public void WriteIntReadEntity() => checkRead(GetStore(), "ColumnInt", entity => entity.ColumnInt);

        [Test]
        public void WriteInt16ReadEntity() => checkRead(GetStore(), "ColumnInt16", entity => entity.ColumnInt16);

        [Test]
        public void WriteInt64ReadEntity() => checkRead(GetStore(), "ColumnInt64", entity => entity.ColumnInt64);

        [Test]
        public void WriteStringReadEntity() => checkRead(GetStore(), "ColumnString", entity => entity.ColumnString);

        [Test]
        public void WriteDateTimeReadEntity() => checkRead(GetStore(), "ColumnDateTime", entity => entity.ColumnDateTime);

        [Test]
        public void WriteTimeSpanReadEntity() => checkRead(GetStore(), "ColumnTimeSpan", entity => entity.ColumnTimeSpan);
    }
}