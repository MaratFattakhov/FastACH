using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FastACH;
using FastACH.Benchmarks;

var summary = BenchmarkRunner.Run<FastACHvsChoetlNacha>();

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
    public async Task<AchFile> FastACH()
    {
        var reader = new AchFileReader();
        var achFile = await reader.Read(achFileName);
        if (achFile.BatchRecordList.Count != (NumberOfEntries / 10))
        {
            throw new Exception("Incorrect number of batches : " + achFile.BatchRecordList.Count);
        }
        return achFile;
    }
}