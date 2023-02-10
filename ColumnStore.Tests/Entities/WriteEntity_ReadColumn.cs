using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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
            
            var sd = keys.First();
            var ed = keys.Last().Add(TimeSpan.FromSeconds(1));
            var result  = store.Entity.Read<SimpleEntity>(sd, ed);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Count == values.Count);
            Assert.IsEmpty(result.Keys.Except(keys), "Keys not matched");

            foreach (var v in values)
            {
                if (!result.TryGetValue(v.Key, out var entity))
                    Assert.Fail("Can't found key [{0:s}]", (DateTime) v.Key);

                var entityValue = getProperty(entity);
                Assert.IsNotNull(entityValue, "Entity is null [{0:s}]", (DateTime) v.Key);
                Assert.IsTrue(entityValue.Equals(v.Value), "Item not equals [{0:s}]", (DateTime) v.Key);
            }
        }

        [Test]
        public void WriteEntityReadByte() => checkRead(GetStore(), "ColumnByte", entity => entity.ColumnByte);
        
        [Test]
        public void WriteEntityReadSByte() => checkRead(GetStore(), "ColumnSByte", entity => entity.ColumnSByte);
        
        [Test]
        public void WriteEntityReadBoolean() => checkRead(GetStore(), "ColumnBool", entity => entity.ColumnBool);

        [Test]
        public void WriteEntityReadDouble() => checkRead(GetStore(), "ColumnDouble", entity => entity.ColumnDouble);

        [Test]
        public void WriteEntityReadGuid() => checkRead(GetStore(), "ColumnGuid", entity => entity.ColumnGuid);

        [Test]
        public void WriteEntityReadInt() => checkRead(GetStore(), "ColumnInt", entity => entity.ColumnInt);
        
        [Test]
        public void WriteEntityReadInt16() => checkRead(GetStore(), "ColumnInt16", entity => entity.ColumnInt16);
        
        [Test]
        public void WriteEntityReadInt64() => checkRead(GetStore(), "ColumnInt64", entity => entity.ColumnInt64);
        
        [Test]
        public void WriteEntityReadUInt() => checkRead(GetStore(), "ColumnUInt", entity => entity.ColumnUInt);
        
        [Test]
        public void WriteEntityReadUInt16() => checkRead(GetStore(), "ColumnUInt16", entity => entity.ColumnUInt16);
        
        [Test]
        public void WriteEntityReadUInt64() => checkRead(GetStore(), "ColumnUInt64", entity => entity.ColumnUInt64);

        [Test]
        public void WriteEntityReadString() => checkRead(GetStore(), "ColumnString", entity => entity.ColumnString);

        [Test]
        public void WriteEntityReadDateTime() => checkRead(GetStore(), "ColumnDateTime", entity => entity.ColumnDateTime);

        [Test]
        public void WriteEntityReadTimeSpan() => checkRead(GetStore(), "ColumnTimeSpan", entity => entity.ColumnTimeSpan);
    }
}