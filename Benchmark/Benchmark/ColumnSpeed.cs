﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmark;

[ExcludeFromCodeCoverage]
public class ColumnSpeed : IColumn
{
    public string Id         { get; }
    public string ColumnName { get; }

    public ColumnSpeed(string columnName)
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
    public          string         Legend                       => "Speed in MB/sec";
    public override string         ToString()                   => ColumnName;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        var compressed = (bool) benchmarkCase.Parameters["Compressed"];
        var pageSize   = (int) benchmarkCase.Parameters["PageSize"];

        var fileName      = MainTest.GetFileName(benchmarkCase.Descriptor.WorkloadMethod.Name, pageSize, compressed);
        var benchmarkData = JsonSerializer.Deserialize<BenchmarkData>(File.ReadAllText(fileName))!;

        var bytes   = benchmarkData.Length                          / 1024.0 / 1024.0;
        var seconds = summary[benchmarkCase].ResultStatistics!.Mean / 1000000000;
        return (bytes / seconds).ToString("F2", CultureInfo.InvariantCulture);
    }
}