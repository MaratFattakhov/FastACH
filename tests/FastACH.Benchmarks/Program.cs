using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using ChoETL.NACHA;
using FastACH;
using FastACH.Benchmarks;

var config = DefaultConfig.Instance.WithSummaryStyle(
    summaryStyle: BenchmarkDotNet.Reports.SummaryStyle.Default.WithTimeUnit(
        Perfolizer.Horology.TimeUnit.Millisecond));

var summary = BenchmarkRunner.Run<FastACHvsChoetlNacha>(config);

public class FastACHvsChoetlNacha
{
    const string achFileName = "test.ach";

    [Params(1000, 10000, 100000)]
    public int NumberOfEntries { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        FakeAchFileGenerator.CreateAchFile(achFileName, NumberOfEntries).Wait();
    }

    [Benchmark]
    public async Task FastACH()
    {
        var reader = new AchFileReader();
        var achFile = await reader.Read(achFileName);
        if (achFile.BatchRecordList.Count != (NumberOfEntries / 10))
        {
            throw new Exception("Incorrect number of batches : " + achFile.BatchRecordList.Count);
        }
    }

    [Benchmark]
    public void ChoetlNacha()
    {
        var batchCount = 0;
        foreach (var r in new ChoNACHAReader(achFileName))
        {
            switch (r)
            {
                case ChoNACHABatchControlRecord batchControlRecord:
                    batchCount++;
                    break;
            }
        }

        if (batchCount != (NumberOfEntries / 10))
        {
            throw new Exception("Incorrect number of batches : " + batchCount);
        }
    }
}