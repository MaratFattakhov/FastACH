using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ChoETL.NACHA;

namespace FastACH.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1,
        warmupCount: 1, iterationCount: 1, invocationCount: 1)]
    public class AchFileReadingBecnhmark
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
            var achFile = await AchFile.Read(achFileName);
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
}
