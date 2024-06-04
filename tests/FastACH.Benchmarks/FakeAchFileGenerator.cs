using FastACH.Records;

namespace FastACH.Benchmarks
{
    internal class FakeAchFileGenerator
    {
        public static async Task CreateAchFile(string path, int numberOfEntries)
        {
            var achFile = CreateAchFile(numberOfEntries / 10, 10);
            var writer = new AchFileWriter();
            await writer.WriteToFile(achFile, path);
        }

        public static AchFile CreateAchFile(int numberOfBatches, int numberOfEntriesInEachBatch)
        {
            var achFile = new AchFile()
            {
                FileHeader = new FileHeaderRecord()
                {
                    ImmediateDestination = "123456789",
                    ImmediateOrigin = "123456789",
                    FileCreationDate = DateOnly.FromDateTime(DateTime.Now),
                    FileCreationTime = TimeOnly.FromDateTime(DateTime.Now),
                    FileIdModifier = 'A',
                    ImmediateDestinationName = "PNC Bank",
                    ImmediateOriginName = "Microsoft Inc.",
                    ReferenceCode = "00000000"
                },
                BatchRecordList =
                    Enumerable.Range(0, numberOfBatches)
                        .Select(_ => CreateBatchRecord(numberOfEntriesInEachBatch)).ToList()
            };

            return achFile;
        }


        private static BatchRecord CreateBatchRecord(int numberOfEntries)
        {
            return new BatchRecord()
            {
                BatchHeader = new BatchHeaderRecord()
                {
                    ServiceClassCode = 200,
                    CompanyName = "companyName",
                    CompanyDiscretionaryData = "companyDiscretionary",
                    CompanyId = "companyID",
                    CompanyEntryDescription = "EntryDescr",
                    CompanyDescriptiveDate = new DateOnly(2011, 02, 03),
                    EffectiveEntryDate = new DateOnly(2011, 01, 02),
                    OriginatingDFIID = "DFINumber"
                },
                TransactionRecords = Enumerable.Range(0, numberOfEntries).Select(_ => CreateTransactionDetails()).ToList(),
            };
        }

        private static TransactionRecord CreateTransactionDetails()
        {
            return new TransactionRecord
            {
                EntryDetail = new EntryDetailRecord()
                {
                    TransactionCode = 22,
                    ReceivingDFIID = 12345678,
                    CheckDigit = '9',
                    DFIAccountNumber = "1313131313",
                    Amount = 22M,
                    ReceiverIdentificationNumber = "ID Number",
                    ReceiverName = "ID Name",
                    DiscretionaryData = "Desc Data",
                    AddendaRecordIndicator = false,
                },
                Addenda = null
                //Addenda = new AddendaRecord()
                //{
                //    AddendaInformation = "Monthly bill"
                //}
            };
        }
    }
}
