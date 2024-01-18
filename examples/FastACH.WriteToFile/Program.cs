using FastACH;
using FastACH.Records;

var achFile = new AchFile()
{
    OneRecord = new OneRecord()
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
    {
        new FiveRecord()
        {
            ServiceClassCode = 200,
            CompanyName = "companyName",
            CompanyDiscretionaryData = "companyDiscretionary",
            CompanyId = "companyID",
            //StandardEntryClassCode = "PPD",
            CompanyEntryDescription = "EntryDescr",
            CompanyDescriptiveDate = new DateOnly(2011, 02, 03),
            EffectiveEntryDate = new DateOnly(2011, 01, 02),
            //JulianSettlementDate = DateTime.Now.ToString("YYMMDD"),
            //OriginatorsStatusCode = '1',
            OriginatingDFIID = "DFINumber",
            SixRecordList = new List<SixRecord>
                {
                    new SixRecord()
                    {
                        TransactionCode = 22,
                        ReceivingDFIID = 12345678,
                        CheckDigit = '9',
                        DFIAccountNumber = "1313131313",
                        Amount = 22M,
                        ReceiverIdentificationNumber = "ID Number",
                        ReceiverName = "ID Name",
                        DiscretionaryData = "Desc Data",
                        AddendaRecordIndicator = true,
                        AddendaRecord = new SevenRecord()
                        {
                            AddendaInformation = "Monthly bill"
                        }
                    },
                    new SixRecord()
                    {
                        TransactionCode = 27,
                        ReceivingDFIID = 12345678,
                        CheckDigit = '9',
                        DFIAccountNumber = "1313131313",
                        Amount = 27M,
                        ReceiverIdentificationNumber = "ID Number",
                        ReceiverName = "ID Name",
                        DiscretionaryData = "Desc Data"
                    }
                },
        }
    }
};

var writer = new AchFileWriter();

await writer.WriteToFile(achFile, "..\\..\\..\\ACH.txt");