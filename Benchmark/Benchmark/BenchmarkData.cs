using System.Diagnostics.CodeAnalysis;

namespace Benchmark;

[ExcludeFromCodeCoverage]
public record BenchmarkData(int Length, double? BytePerValue);