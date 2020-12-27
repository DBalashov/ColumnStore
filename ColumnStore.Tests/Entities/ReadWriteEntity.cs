using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ColumnStore.Tests.Entity
{
    public class ReadWriteEntity : Base
    {
        CDT[]                         keys;
        Dictionary<CDT, SimpleEntity> entities;

        [SetUp]
        public void Setup()
        {
            keys     = GetKeys();
            entities = keys.ToDictionary(p => p, p => new SimpleEntity(p));
        }

        void checkRead(PersistentColumnStore store)
        {
            var items = store.ReadEntity<SimpleEntity>(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)));
            Assert.IsNotNull(items);
            Assert.IsNotEmpty(items);
            Assert.IsTrue(items.Count == entities.Count, $"Count mismatch: {entities.Count} expected, {items.Count} arrived");
            Assert.IsEmpty(items.Keys.Except(keys), "Keys not matched");

            foreach (var item in items)
            {
                if (!entities.TryGetValue(item.Key, out var entity))
                    Assert.Fail("Can't found key [{0:s}]", (DateTime) item.Key);

                Assert.IsNotNull(item, "Entity is null [{0:s}]", (DateTime) item.Key);
                Assert.IsTrue(item.Value.Equals(entity), "Item not equals [{0:s}]", (DateTime) item.Key);
            }
        }

        [Test]
        public void WriteSingle()
        {
            var store = GetStore();
            store.WriteEntity(entities);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
        }

        [Test]
        public void WriteMulti()
        {
            var store = GetStore();
            store.WriteEntity(entities);
            store.WriteEntity(entities);
            checkRead(store);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
        }

        [Test]
        public void WriteRead()
        {
            var store = GetStore();
            store.WriteEntity(entities);
            checkRead(store);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
        }
    }
}