using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

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
            TestContext.WriteLine($"Keys: {keys.Length}");
        }

        void checkRead(PersistentColumnStore store)
        {
            var items = store.Entity.Read<SimpleEntity>(keys.First(), keys.Last().Add(TimeSpan.FromSeconds(1)));
            Assert.That(items != null);
            Assert.That(items.Any());
            Assert.That(items.Count == entities.Count,  $"Count mismatch: {entities.Count} expected, {items.Count} arrived");
            Assert.That(!items.Keys.Except(keys).Any(), "Keys not matched");

            foreach (var item in items)
            {
                if (!entities.TryGetValue(item.Key, out var entity))
                    Assert.Fail($"Can't found key [{(DateTime) item.Key:s}]");

                Assert.That(item.Value.Equals(entity), $"Item not equals [{(DateTime) item.Key:s}]");
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteSingle(bool compressed)
        {
            var store = GetStore(compressed);
            store.Entity.Write(entities);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteMulti(bool compressed)
        {
            var store = GetStore(compressed);
            store.Entity.Write(entities);
            store.Entity.Write(entities);
            checkRead(store);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void WriteRead(bool compressed)
        {
            var store = GetStore(compressed);
            store.Entity.Write(entities);
            checkRead(store);
            TestContext.WriteLine($"Pages: {store.Container.TotalPages}, Length={store.Container.Length / 1024} KB");
        }
    }
}