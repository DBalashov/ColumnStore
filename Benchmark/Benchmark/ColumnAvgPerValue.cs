using System.Globalization;
using System.Text.Json;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmark;

public class ColumnAvgPerValue : IColumn
{
    public string Id         { get; }
    public string ColumnName { get; }

    public ColumnAvgPerValue(string columnName)
    {
        ColumnName = columnName;
        Id         = nameof(TagColumn) + "." + ColumnName;
    }

    public bool   IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    public string GetValue(Summary  summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, SummaryStyle.Default);

    public          bool           IsAvailable(Summary summary) => true;
    public          bool           AlwaysShow                   => true;
    public          ColumnCategory Category                     => ColumnCategory.Custom;
    public          int            PriorityInCategory           => 0;
    public          bool           IsNumeric                    => false;
    public          UnitType       UnitType                     => UnitType.Dimensionless;
    public          string         Legend                       => "Bytes/value (average for byte/bool/short/int/long/timespan/datetime/string columns)";
    public override string         ToString()                   => ColumnName;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        var compressed = (bool) benchmarkCase.Parameters["Compressed"];
        var pageSize   = (int) benchmarkCase.Parameters["PageSize"];

        var fileName      = MainTest.GetFileName(benchmarkCase.Descriptor.WorkloadMethod.Name, pageSize, compressed);
        var benchmarkData = JsonSerializer.Deserialize<BenchmarkData>(File.ReadAllText(fileName))!;

        return benchmarkData.BytePerValue.HasValue ? benchmarkData.BytePerValue.Value.ToString("F1", CultureInfo.InvariantCulture) : "-";
    }
}