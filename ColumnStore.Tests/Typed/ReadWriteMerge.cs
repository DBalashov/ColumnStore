using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ColumnStore.Tests.Typed
{
    public class ReadWriteMerge : Base
    {
        CDT[]                 keys;
        PersistentColumnStore storeUncompressed;
        PersistentColumnStore storeCompressed;

        [SetUp]
        public void Setup()
        {
            keys              = GetKeys();
            storeUncompressed = GetStore(false);
            storeCompressed   = GetStore(true);
            TestContext.WriteLine($"Keys: {keys.Length}");
        }

        void writeWithMerge<T>(Func<Dictionary<CDT, T>> getData)
        {
            var d          = getData();
            var columnName = typeof(T).Name;
            storeCompressed.Typed.Write(columnName, d);
            storeCompressed.Typed.Write(columnName, d);
            storeUncompressed.Typed.Write(columnName, d);
            storeUncompressed.Typed.Write(columnName, d);
            TestContext.WriteLine($"Pages(U): {storeUncompressed.Container.TotalPages}, Length(U)={storeUncompressed.Container.Length / 1024} KB");
            TestContext.WriteLine($"Pages(C): {storeCompressed.Container.TotalPages}, Length(C)={storeCompressed.Container.Length     / 1024} KB");
        }

        [Test]
        public void WriteByte() => writeWithMerge(() => GetBytes(keys));

        [Test]
        public void WriteSByte() => writeWithMerge(() => GetSBytes(keys));

        [Test]
        public void WriteBoolean() => writeWithMerge(() => GetBoolean(keys));

        [Test]
        public void WriteDouble() => writeWithMerge(() => GetDoubles(keys));

        [Test]
        public void WriteGuid() => writeWithMerge(() => GetGuids(keys));

        [Test]
        public void WriteInt() => writeWithMerge(() => GetInts(keys));

        [Test]
        public void WriteUInt() => writeWithMerge(() => GetUInts(keys));

        [Test]
        public void WriteInt16() => writeWithMerge(() => GetInt16s(keys));

        [Test]
        public void WriteUInt16() => writeWithMerge(() => GetUInt16s(keys));

        [Test]
        public void WriteInt64() => writeWithMerge(() => GetInt64s(keys));

        [Test]
        public void WriteUInt64() => writeWithMerge(() => GetUInt64s(keys));

        [Test]
        public void WriteString() => writeWithMerge(() => GetStrings(keys));

        [Test]
        public void WriteDateTime() => writeWithMerge(() => GetDateTimes(keys));

        [Test]
        public void WriteDateOnlys() => writeWithMerge(() => GetDateOnlys(keys));
        
        [Test]
        public void WriteTimeOnlys() => writeWithMerge(() => GetTimeOnlys(keys));
        
        [Test]
        public void WriteDecimals() => writeWithMerge(() => GetDecimals(keys));
        
        [Test]
        public void WriteTimeSpan() => writeWithMerge(() => GetTimeSpans(keys));
        
        [Test]
        public void WriteHalfs() => writeWithMerge(() => GetHalfs(keys));
    }
}