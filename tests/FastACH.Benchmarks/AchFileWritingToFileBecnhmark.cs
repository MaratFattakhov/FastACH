using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ChoETL.NACHA;

namespace FastACH.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1,
        warmupCount: 1, iterationCount: 1, invocationCount: 1)]
    public class AchFileWritingToFileBecnhmark
    {
        const string achFileName = "test.ach";
        private AchFile? _achFile;

        [Params(1000, 10000, 100000)]
        public int NumberOfEntries { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _achFile = FakeAchFileGenerator.CreateAchFile(NumberOfEntries / 10, 10);
        }

        [Benchmark]
        public async Task FastACH()
        {
            await _achFile!.WriteToFile(achFileName);
        }

        [Benchmark]
        public void ChoetlNacha()
        {
            ChoNACHAConfiguration config = new ChoNACHAConfiguration();
            config.DestinationBankRoutingNumber = "123456789";
            config.OriginatingCompanyId = "123456789";
            config.DestinationBankName = "PNC Bank";
            config.OriginatingCompanyName = "Microsoft Inc.";
            config.ReferenceCode = "Internal Use Only.";
            using (var nachaWriter = new ChoNACHAWriter("ACH.txt", config))
            {
                foreach (var batch in _achFile!.BatchRecordList)
                {
                    using (var bw1 = nachaWriter.CreateBatch(200))
                    {
                        foreach (var entry in batch.TransactionRecords)
                        {
                            if (entry.EntryDetail is not null)
                            {
                                using (var entry1 = bw1.CreateDebitEntryDetail(20, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
                                {
                                    //entry1.CreateAddendaRecord("Monthly bill");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
