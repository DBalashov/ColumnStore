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
    Dictionary<CDT, SimpleEntity> entities;

    #region EntitiesWrite

    [IterationSetup(Target = nameof(EntitiesWrite))]
    public void IterationStartup_EntitesWrite()
    {
        store    = new PersistentColumnStore(container = new InMemoryContainer(new PersistentContainerSettings(PageSize)), CDTUnit.Month, Compressed);
        entities = Keys.ToDictionary(p => (CDT) p, p => new SimpleEntity(p));
    }

    [Benchmark]
    public void EntitiesWrite()
    {
        store.Entity.Write(entities);
        benchmarkData = new BenchmarkData((int) container.Length, null);
    }

    [IterationCleanup(Target = nameof(EntitiesWrite))]
    public void IterationCleanup_EntitiesWrite() => File.WriteAllText(GetFileName(nameof(EntitiesWrite), PageSize, Compressed), JsonSerializer.Serialize(benchmarkData));

    #endregion

    #region EntitiesMerge

    [IterationSetup(Targets = new[] {nameof(EntitiesMerge)})]
    public void IterationStartup_EntitiesMerge()
    {
        store    = new PersistentColumnStore(container = new InMemoryContainer(new PersistentContainerSettings(PageSize)), CDTUnit.Month, Compressed);
        entities = Keys.ToDictionary(p => (CDT) p, p => new SimpleEntity(p));
        store.Entity.Write(entities);
    }

    [Benchmark]
    public void EntitiesMerge()
    {
        store.Entity.Write(entities);
        benchmarkData = new BenchmarkData((int) container.Length, null);
    }

    [IterationCleanup(Target = nameof(EntitiesMerge))]
    public void IterationCleanup_EntitiesMerge() => File.WriteAllText(GetFileName(nameof(EntitiesMerge), PageSize, Compressed), JsonSerializer.Serialize(benchmarkData));

    #endregion

    #region EntitiesRead

    [IterationSetup(Targets = new[] {nameof(EntitiesRead)})]
    public void IterationStartup_EntitiesRead()
    {
        store    = new PersistentColumnStore(container = new InMemoryContainer(new PersistentContainerSettings(PageSize)), CDTUnit.Month, Compressed);
        entities = Keys.ToDictionary(p => (CDT) p, p => new SimpleEntity(p));
        store.Entity.Write(entities);
    }

    [Benchmark]
    public void EntitiesRead()
    {
        var readFrom = Keys[Keys.Length / 4 * 3];
        var readTo   = readFrom.AddDays(ReadDays);
        var values   = store.Entity.Read<SimpleEntity>(readFrom, readTo);
        benchmarkData = new BenchmarkData((int) container.Length, null);
    }

    [IterationCleanup(Target = nameof(TypedRead))]
    public void IterationCleanup_EntitiesRead() => File.WriteAllText(GetFileName(nameof(EntitiesRead), PageSize, Compressed), JsonSerializer.Serialize(benchmarkData));

    #endregion
}