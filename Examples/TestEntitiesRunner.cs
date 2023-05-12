using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ColumnStore;
using FileContainer;

namespace Examples
{
    [ExcludeFromCodeCoverage]
    sealed class TestEntitiesRunner : TestAbstractRunner
    {
        readonly Dictionary<CDT, SimpleEntity> entities = Keys.ToDictionary(p => (CDT) p, p => new SimpleEntity(p));
        
        public void Run(bool compressedContainer)
        {
            Console.WriteLine("\nWriting entities, compressed: {0}, page size: {1}", compressedContainer ? "YES" : "NO", string.Join(", ", PageSizes));
            foreach (var pageSize in PageSizes)
            {
                using (var pc = new InMemoryContainer(new PersistentContainerSettings(pageSize)))
                using (var ps = new PersistentColumnStore(pc, CDTUnit.Month, compressedContainer))
                {
                    var sw = Stopwatch.StartNew();

                    Console.Write("[{0,5}] Writing", pageSize);
                    ps.Entity.Write(entities);

                    Console.WriteLine(", duration: {0} ms, Length: {1:F2} MB, avg bytes/value: {2:F2}",
                                      sw.ElapsedMilliseconds, pc.Length / 1048576.0,
                                      pc.Length                         / (entities.Count * 35.0)); // 35 fields in SimpleEntity

                    var readFrom = Keys[r.Next(Keys.Length / 2)];
                    var readTo   = readFrom.AddDays(ReadDays);

                    Console.Write("[{0,5}] Reading, random period: {1} days, {2:u} - {3:u}", pageSize, ReadDays, readFrom, readTo);
                    sw.Restart();

                    var valueInts = ps.Entity.Read<SimpleEntity>(readFrom, readTo);
                    Console.WriteLine(", duration: {0} ms, {1} item(s)", sw.ElapsedMilliseconds, valueInts.Count);
                }
            }
        }
    }
}