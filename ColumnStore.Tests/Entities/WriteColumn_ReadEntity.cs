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
            store.Entity.Write(entities);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");

            var items = store.Typed.Read<T>(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)), columnName);
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
        [TestCase(false)]
        [TestCase(true)]
        public void WriteByteReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnByte", entity => entity.ColumnByte);
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteSByteReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnSByte", entity => entity.ColumnSByte);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteBooleanReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnBool", entity => entity.ColumnBool);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteDoubleReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnDouble", entity => entity.ColumnDouble);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteGuidReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnGuid", entity => entity.ColumnGuid);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteIntReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnInt", entity => entity.ColumnInt);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteInt16ReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnInt16", entity => entity.ColumnInt16);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteInt64ReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnInt64", entity => entity.ColumnInt64);
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteUIntReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnUInt", entity => entity.ColumnUInt);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteUInt16ReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnUInt16", entity => entity.ColumnUInt16);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteUInt64ReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnUInt64", entity => entity.ColumnUInt64);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteStringReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnString", entity => entity.ColumnString);

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteDateTimeReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnDateTime", entity => entity.ColumnDateTime);
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteTimeSpanReadEntity(bool compressed) => checkRead(GetStore(compressed), "ColumnTimeSpan", entity => entity.ColumnTimeSpan);
    }
}