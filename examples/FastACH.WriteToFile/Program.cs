using FastACH;
using FastACH.Records;

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
    BatchRecordList = new List<BatchRecord>()
    {
        new BatchRecord()
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
            TransactionDetailsList =
            {
                new TransactionDetails
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
                        AddendaRecordIndicator = true,
                    },
                    Addenda = new AddendaRecord()
                    {
                        AddendaInformation = "Monthly bill"
                    }
                },
                new TransactionDetails()
                {
                    EntryDetail = new EntryDetailRecord()
                    {
                        TransactionCode = 27,
                        ReceivingDFIID = 12345678,
                        CheckDigit = '9',
                        DFIAccountNumber = "1313131313",
                        Amount = 27M,
                        ReceiverIdentificationNumber = "ID Number",
                        ReceiverName = "ID Name",
                        DiscretionaryData = "Desc Data",
                        AddendaRecordIndicator = false,
                    },
                    Addenda = null
                }
            }
        }
    }
};

var writer = new AchFileWriter();

await writer.WriteToFile(achFile, "..\\..\\..\\ACH.txt");