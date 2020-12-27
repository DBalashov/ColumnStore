using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ColumnStore;
using FileContainer;

namespace Examples
{
    class Program
    {
        static readonly Random   r         = new Random(Guid.NewGuid().GetHashCode());
        static readonly int[]    pageSizes = new[] {2048, 4096, 8192, 16384};
        static readonly int      every     = 1;
        const           int      readDays  = 63;
        static readonly DateTime sd        = new DateTime(2020, 1, 1);
        static readonly DateTime ed        = new DateTime(2021, 1, 1);

        static readonly DateTime[] keys = Enumerable.Range(0, (int) (ed.Subtract(sd).TotalMinutes / every))
                                                    .Select(p => sd.AddMinutes(p * every))
                                                    .ToArray();

        static readonly Dictionary<CDT, SimpleEntity> entities = keys.ToDictionary(p => (CDT) p, p => new SimpleEntity(p));

        static void Main(string[] args)
        {
            // generate data

            Console.WriteLine("Keys: {0} (each {1} minutes), {2:u} - {3:u}", entities.Count, every, sd, ed);

            testTyped(false);
            testTyped(true);
            testEntities(false);
            testEntities(true);
        }

        #region testTyped

        static void testTyped(bool compressedContainer)
        {
            // generate some data
            var dataInts      = keys.ToDictionary(p => (CDT) p, p => p.Minute + p.Second + p.Day);
            var dataBytes     = keys.ToDictionary(p => (CDT) p, p => (byte) (p.Minute + p.Second + p.Day));
            var dataDoubles   = keys.ToDictionary(p => (CDT) p, p => p.TimeOfDay.TotalMilliseconds);
            var dataTimeSpans = keys.ToDictionary(p => (CDT) p, p => p.TimeOfDay);
            var dataStrings   = keys.ToDictionary(p => (CDT) p, p => "Item Address " + p.ToString("yyyyMMdd") + "/" + p.Month + "/" + p.Minute + "/" + p.Day);
            var dataGuids     = keys.ToDictionary(p => (CDT) p, p => new Guid((uint) p.Year, 0, 0, (byte) p.Year, (byte) p.Month, (byte) p.Day, (byte) p.Hour, 0, 0, 0, 0));
            var dataDateTimes = keys.ToDictionary(p => (CDT) p, p => p);

            Console.WriteLine("\nWriting typed arrays, compressed: {0}, page size: {1}", compressedContainer ? "YES" : "NO", string.Join(", ", pageSizes));

            // write & read
            foreach (var pageSize in pageSizes)
            {
                using (var pc = new InMemoryContainer(pageSize))
                using (var ps = new PersistentColumnStore(pc, CDTUnit.Month, compressedContainer))
                {
                    var sw = Stopwatch.StartNew();

                    Console.Write("[{0,5}] Writing", pageSize);

                    ps.Write("Ints", dataInts);
                    ps.Write("Bytes", dataBytes);
                    ps.Write("Doubles", dataDoubles);
                    ps.Write("TimeSpans", dataTimeSpans);
                    ps.Write("Strings", dataStrings);
                    ps.Write("Guids", dataGuids);
                    ps.Write("DateTimes", dataDateTimes);

                    Console.WriteLine(", duration: {0} ms, Length: {1:F2} MB, avg bytes/value: {2:F2}",
                                      sw.ElapsedMilliseconds, pc.Length / 1048576.0,
                                      pc.Length / (keys.Length * 7.0));


                    var readFrom = keys[r.Next(keys.Length / 2)];
                    var readTo   = readFrom.AddDays(readDays);

                    Console.Write("[{0,5}] Reading, random period: {1} days, {2:u} - {3:u}", pageSize, readDays, readFrom, readTo);
                    sw.Restart();

                    var valueInts      = ps.Read<int>(readFrom, readTo, "Ints");
                    var valueBytes     = ps.Read<byte>(readFrom, readTo, "Bytes");
                    var valueDoubles   = ps.Read<double>(readFrom, readTo, "Doubles");
                    var valueTimeSpans = ps.Read<TimeSpan>(readFrom, readTo, "TimeSpans");
                    var valueStrings   = ps.Read<string>(readFrom, readTo, "Strings");
                    var valueGuids     = ps.Read<Guid>(readFrom, readTo, "Guids");
                    var valueDateTimes = ps.Read<DateTime>(readFrom, readTo, "DateTimes");

                    Console.WriteLine(", duration: {0} ms, {1} item(s)", sw.ElapsedMilliseconds, valueInts.Count);
                }
            }
        }

        #endregion

        #region testEntities

        static void testEntities(bool compressedContainer)
        {
            Console.WriteLine("\nWriting entities, compressed: {0}, page size: {1}", compressedContainer ? "YES" : "NO", string.Join(", ", pageSizes));
            foreach (var pageSize in pageSizes)
            {
                using (var pc = new InMemoryContainer(pageSize))
                using (var ps = new PersistentColumnStore(pc, CDTUnit.Month, compressedContainer))
                {
                    var sw = Stopwatch.StartNew();

                    Console.Write("[{0,5}] Writing", pageSize);
                    ps.WriteEntity(entities);

                    Console.WriteLine(", duration: {0} ms, Length: {1:F2} MB, avg bytes/value: {2:F2}",
                                      sw.ElapsedMilliseconds, pc.Length / 1048576.0,
                                      pc.Length / (entities.Count * 35.0)); // 35 fields in SimpleEntity

                    var readFrom = keys[r.Next(keys.Length / 2)];
                    var readTo   = readFrom.AddDays(readDays);

                    Console.Write("[{0,5}] Reading, random period: {1} days, {2:u} - {3:u}", pageSize, readDays, readFrom, readTo);
                    sw.Restart();

                    var valueInts = ps.ReadEntity<SimpleEntity>(readFrom, readTo);
                    Console.WriteLine(", duration: {0} ms, {1} item(s)", sw.ElapsedMilliseconds, valueInts.Count);
                }
            }
        }

        #endregion
    }
}