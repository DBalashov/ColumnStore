// using System;
// using System.Linq;
// using NUnit.Framework;
// 
// namespace ColumnStore.Tests
// {
//     public class WriteIncompatibleTypes : Base
//     {
//         CDT[] keys;
//
//         [SetUp]
//         public void Setup()
//         {
//             keys = GetKeys();
//         }
//
//         void writeWithDifferentTypes(StoredDataType type, string columnName = null)
//         {
//             foreach (var otherType in Enum.GetValues(typeof(StoredDataType)).OfType<StoredDataType>().Where(p => p != type))
//                 Assert.Throws<IncompatibleTypesException>(() =>
//                 {
//                     var store = GetStore();
//                     store.Write(new KeyValueDataColumn(columnName ?? type.ToString(), keys, GetData(type, keys)));
//                     store.Write(new KeyValueDataColumn(columnName ?? type.ToString(), keys, GetData(otherType, keys)));
//                 });
//         }
//
//         [Test]
//         public void WriteByte()
//         {
//             writeWithDifferentTypes(StoredDataType.Byte);
//         }
//
//         [Test]
//         public void WriteDouble()
//         {
//             writeWithDifferentTypes(StoredDataType.Double);
//         }
//
//         [Test]
//         public void WriteGuid()
//         {
//             writeWithDifferentTypes(StoredDataType.Guid);
//         }
//
//         [Test]
//         public void WriteInt()
//         {
//             writeWithDifferentTypes(StoredDataType.Int);
//         }
//
//         [Test]
//         public void WriteString()
//         {
//             writeWithDifferentTypes(StoredDataType.String);
//         }
//
//         [Test]
//         public void WriteDateTime()
//         {
//             writeWithDifferentTypes(StoredDataType.DateTime);
//         }
//
//         [Test]
//         public void WriteTimeSpan()
//         {
//             writeWithDifferentTypes(StoredDataType.TimeSpan);
//         }
//     }
// }