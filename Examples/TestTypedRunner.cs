using System;
using System.Diagnostics;
using System.Linq;
using ColumnStore;
using FileContainer;

namespace Examples
{
    sealed class TestTypedRunner : TestAbstractRunner
    {
        public void Run(bool compressedContainer)
        {
            // generate some data
            var dataInts      = Keys.ToDictionary(p => (CDT) p, p => p.Minute + p.Second + p.Day);
            var dataBytes     = Keys.ToDictionary(p => (CDT) p, p => (byte) (p.Minute + p.Second + p.Day));
            var dataBools     = Keys.ToDictionary(p => (CDT) p, p => (p.Minute + p.Second + p.Day) % p.Day > 0);
            var dataDoubles   = Keys.ToDictionary(p => (CDT) p, p => p.TimeOfDay.TotalMilliseconds);
            var dataTimeSpans = Keys.ToDictionary(p => (CDT) p, p => p.TimeOfDay);
            var dataStrings   = Keys.ToDictionary(p => (CDT) p, p => "Item Address " + p.ToString("yyyyMMdd") + "/" + p.Month + "/" + p.Minute + "/" + p.Day);
            var dataGuids     = Keys.ToDictionary(p => (CDT) p, p => new Guid((uint) p.Year, 0, 0, (byte) p.Year, (byte) p.Month, (byte) p.Day, (byte) p.Hour, 0, 0, 0, 0));
            var dataDateTimes = Keys.ToDictionary(p => (CDT) p, p => p);

            Console.WriteLine("\nWriting typed arrays, compressed: {0}, page size: {1}", compressedContainer ? "YES" : "NO", string.Join(", ", PageSizes));

            // write & read
            foreach (var pageSize in PageSizes)
            {
                using (var pc = new InMemoryContainer(new PersistentContainerSettings(pageSize)))
                using (var ps = new PersistentColumnStore(pc, CDTUnit.Month, compressedContainer))
                {
                    var sw = Stopwatch.StartNew();

                    Console.Write("[{0,5}] Writing", pageSize);

                    ps.Typed.Write("Ints", dataInts);
                    ps.Typed.Write("Bytes", dataBytes);
                    ps.Typed.Write("Bools", dataBools);
                    ps.Typed.Write("Doubles", dataDoubles);
                    ps.Typed.Write("TimeSpans", dataTimeSpans);
                    ps.Typed.Write("Strings", dataStrings);
                    ps.Typed.Write("Guids", dataGuids);
                    ps.Typed.Write("DateTimes", dataDateTimes);

                    Console.WriteLine(", duration: {0} ms, Length: {1:F2} MB, avg bytes/value: {2:F2}",
                                      sw.ElapsedMilliseconds, pc.Length / 1048576.0,
                                      pc.Length / (Keys.Length * 7.0));

                    var readFrom = Keys[r.Next(Keys.Length / 2)];
                    var readTo   = readFrom.AddDays(ReadDays);

                    Console.Write("[{0,5}] Reading, random period: {1} days, {2:u} - {3:u}", pageSize, ReadDays, readFrom, readTo);
                    sw.Restart();

                    var valueInts      = ps.Typed.Read<int>(readFrom, readTo, "Ints");
                    var valueBools     = ps.Typed.Read<bool>(readFrom, readTo, "Bools");
                    var valueBytes     = ps.Typed.Read<byte>(readFrom, readTo, "Bytes");
                    var valueDoubles   = ps.Typed.Read<double>(readFrom, readTo, "Doubles");
                    var valueTimeSpans = ps.Typed.Read<TimeSpan>(readFrom, readTo, "TimeSpans");
                    var valueStrings   = ps.Typed.Read<string>(readFrom, readTo, "Strings");
                    var valueGuids     = ps.Typed.Read<Guid>(readFrom, readTo, "Guids");
                    var valueDateTimes = ps.Typed.Read<DateTime>(readFrom, readTo, "DateTimes");

                    Console.WriteLine(", duration: {0} ms, {1} item(s)", sw.ElapsedMilliseconds,
                                      (valueInts.Count + valueBytes.Count + valueBools.Count + valueDoubles.Count + valueTimeSpans.Count + valueStrings.Count + valueGuids.Count + valueDateTimes.Count) / 8);
                }
            }
        }
    }
}