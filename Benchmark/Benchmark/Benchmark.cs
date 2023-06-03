using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ColumnStore;
using ColumnStore.Tests;
using FileContainer;

#pragma warning disable CS8618

namespace Benchmark;

[SimpleJob(RuntimeMoniker.Net70, baseline: true)]
[WarmupCount(3)]
[IterationCount(3)]
// [MemoryDiagnoser]
[Config(typeof(Config))]
public partial class MainTest
{
    const int ReadDays = 63;

    static readonly DateTime StartDate = new(2020, 1, 1);
    static readonly DateTime EndDate   = new(2021, 1, 1);
    static readonly int      Every     = 1;

    static readonly DateTime[] Keys = Enumerable.Range(0, (int) (EndDate.Subtract(StartDate).TotalMinutes / Every))
                                                .Select(p => StartDate.AddMinutes(p * Every))
                                                .ToArray();

    PagedContainerAbstract container;
    PersistentColumnStore  store;

    BenchmarkData benchmarkData;

    [Params(false, true)]
    public bool Compressed { get; set; }

    [Params(2048, 4096, 8192, 16384)]
    public int PageSize { get; set; }

    public static string GetFileName(string method, int pageSize, bool compressed) => Path.Combine(Path.GetTempPath(), method + "_" + string.Join("_", pageSize, compressed ? 1 : 0) + ".json");
}