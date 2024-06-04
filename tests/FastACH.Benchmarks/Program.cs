using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using FastACH.Benchmarks;

var config = DefaultConfig.Instance.WithSummaryStyle(
    summaryStyle: BenchmarkDotNet.Reports.SummaryStyle.Default.WithTimeUnit(
        Perfolizer.Horology.TimeUnit.Millisecond));

var summary = BenchmarkRunner.Run<AchFileReadingBecnhmark>(config);
//var summary = BenchmarkRunner.Run<AchFileWritingToFileBecnhmark>(config);