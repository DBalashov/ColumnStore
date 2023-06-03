using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ColumnStore;
using ColumnStore.Tests;
using FileContainer;

#pragma warning disable CS8618

namespace Benchmark;

public partial class MainTest
{
    readonly Dictionary<CDT, int>      dataInts      = Keys.ToDictionary(p => (CDT) p, p => p.Minute + p.Second + p.Day);
    readonly Dictionary<CDT, byte>     dataBytes     = Keys.ToDictionary(p => (CDT) p, p => (byte) (p.Minute + p.Second + p.Day));
    readonly Dictionary<CDT, bool>     dataBools     = Keys.ToDictionary(p => (CDT) p, p => (p.Minute + p.Second + p.Day) % p.Day > 0);
    readonly Dictionary<CDT, double>   dataDoubles   = Keys.ToDictionary(p => (CDT) p, p => p.TimeOfDay.TotalMilliseconds);
    readonly Dictionary<CDT, TimeSpan> dataTimeSpans = Keys.ToDictionary(p => (CDT) p, p => p.TimeOfDay);
    readonly Dictionary<CDT, string>   dataStrings   = Keys.ToDictionary(p => (CDT) p, p => "Item Address " + p.ToString("yyyyMMdd") + "/" + p.Month + "/" + p.Minute + "/" + p.Day);
    readonly Dictionary<CDT, Guid>     dataGuids     = Keys.ToDictionary(p => (CDT) p, p => new Guid((uint) p.Year, 0, 0, (byte) p.Year, (byte) p.Month, (byte) p.Day, (byte) p.Hour, 0, 0, 0, 0));
    readonly Dictionary<CDT, DateTime> dataDateTimes = Keys.ToDictionary(p => (CDT) p, p => p);
    
    #region TypedWrite

    [IterationSetup(Target = nameof(TypedWrite))]
    public void IterationStartup_TypedWrite()
    {
        store = new PersistentColumnStore(container = new InMemoryContainer(new PersistentContainerSettings(PageSize)), CDTUnit.Month, Compressed);
    }

    [Benchmark]
    public void TypedWrite()
    {
        store.Typed.Write("Ints",      dataInts);
        store.Typed.Write("Bytes",     dataBytes);
        store.Typed.Write("Bools",     dataBools);
        store.Typed.Write("Doubles",   dataDoubles);
        store.Typed.Write("TimeSpans", dataTimeSpans);
        store.Typed.Write("Strings",   dataStrings);
        store.Typed.Write("Guids",     dataGuids);
        store.Typed.Write("DateTimes", dataDateTimes);
        benchmarkData = new BenchmarkData((int) container.Length, container.Length / (Keys.Length * 8.0));
    }

    [IterationCleanup(Target = nameof(TypedWrite))]
    public void IterationCleanup_TypedWrite() => File.WriteAllText(GetFileName(nameof(TypedWrite), PageSize, Compressed), JsonSerializer.Serialize(benchmarkData));

    #endregion

    #region TypedMerge

    [IterationSetup(Targets = new[] {nameof(TypedMerge), nameof(TypedRead)})]
    public void IterationStartup_TypedMerge()
    {
        store = new PersistentColumnStore(container = new InMemoryContainer(new PersistentContainerSettings(PageSize)), CDTUnit.Month, Compressed);
        store.Typed.Write("Ints",      dataInts);
        store.Typed.Write("Bytes",     dataBytes);
        store.Typed.Write("Bools",     dataBools);
        store.Typed.Write("Doubles",   dataDoubles);
        store.Typed.Write("TimeSpans", dataTimeSpans);
        store.Typed.Write("Strings",   dataStrings);
        store.Typed.Write("Guids",     dataGuids);
        store.Typed.Write("DateTimes", dataDateTimes);
    }

    [Benchmark]
    public void TypedMerge()
    {
        store.Typed.Write("Ints",      dataInts);
        store.Typed.Write("Bytes",     dataBytes);
        store.Typed.Write("Bools",     dataBools);
        store.Typed.Write("Doubles",   dataDoubles);
        store.Typed.Write("TimeSpans", dataTimeSpans);
        store.Typed.Write("Strings",   dataStrings);
        store.Typed.Write("Guids",     dataGuids);
        store.Typed.Write("DateTimes", dataDateTimes);
        benchmarkData = new BenchmarkData((int) container.Length, container.Length / (Keys.Length * 8.0));
    }

    [IterationCleanup(Target = nameof(TypedMerge))]
    public void IterationCleanup_TypedMerge() => File.WriteAllText(GetFileName(nameof(TypedMerge), PageSize, Compressed), JsonSerializer.Serialize(benchmarkData));

    #endregion

    #region TypedRead

    [Benchmark]
    public void TypedRead()
    {
        var readFrom = Keys[Keys.Length / 4 * 3];
        var readTo   = readFrom.AddDays(ReadDays);

        var valueInts      = store.Typed.Read<int>(readFrom, readTo, "Ints");
        var valueBools     = store.Typed.Read<bool>(readFrom, readTo, "Bools");
        var valueBytes     = store.Typed.Read<byte>(readFrom, readTo, "Bytes");
        var valueDoubles   = store.Typed.Read<double>(readFrom, readTo, "Doubles");
        var valueTimeSpans = store.Typed.Read<TimeSpan>(readFrom, readTo, "TimeSpans");
        var valueStrings   = store.Typed.Read<string>(readFrom, readTo, "Strings");
        var valueGuids     = store.Typed.Read<Guid>(readFrom, readTo, "Guids");
        var valueDateTimes = store.Typed.Read<DateTime>(readFrom, readTo, "DateTimes");
        benchmarkData = new BenchmarkData((int) container.Length, container.Length / (Keys.Length * 8.0));
    }

    [IterationCleanup(Target = nameof(TypedRead))]
    public void IterationCleanup_TypedRead() => File.WriteAllText(GetFileName(nameof(TypedRead), PageSize, Compressed), JsonSerializer.Serialize(benchmarkData));

    #endregion
}