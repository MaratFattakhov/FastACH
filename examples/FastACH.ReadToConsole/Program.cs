// See https://aka.ms/new-console-template for more information
using FastACH;

var achFileService = new AchFileReader();

var achFile = await achFileService.Read("ACH.txt");

achFile.OutputFileToConsole();

//var achFile = new AchFile()
//{
//    OneRecord = new ACHRecordType1()
//    {
//        ImmediateDestination = "123456789",
//        ImmediateOrigin = "987654321",
//        FileCreationDate = DateTime.Now.ToString("YYMMDD"),
//        FileCreationTime = DateTime.Now.ToString("HHmm"),
//        FileIdModifier = "A",
//        ImmediateDestinationName = "My Bank",
//        ImmediateOriginName = "Your Bank",
//        ReferenceCode = "REF"
//    },
//    BatchRecordList = new List<ACHRecordType5>
//    {
//        new ACHRecordType5()
//        {
//            BatchNumber = 1,
//            ServiceClassCode = "200",
//                CompanyName = "Company Name",
//                CompanyDiscretionaryData = "Discretionary Data",
//                CompanyIdentification = "Company ID",
//                StandardEntryClassCode = "PPD",
//                CompanyEntryDescription = "Entry Description",
//                CompanyDescriptiveDate = DateTime.Now.ToString("YYMMDD"),
//                EffectiveEntryDate = DateTime.Now.ToString("YYMMDD"),
//                SettlementDate = DateTime.Now.ToString("YYMMDD"),
//                OriginatorsStatusCode = "1",
//                OriginatorsDFINumber = "12345678",
//                SixRecordList = new List<ACHRecordType6>
//                {
//                    new ACHRecordType6()
//                    {
//                        TransactionCode = "22",
//                        ReceivingDFINumber = "12345678",
//                        CheckDigit = "1",
//                        DFIAccountNumber = "123456789",
//                        Amount = 1000,
//                        ReceiverIdentificationNumber = "123456789",
//                        ReceiverName = "John Doe",
//                        DiscretionaryData = "Discretionary Data"
//                    }
//                },
//        }
//    }
//};

Console.ReadLine();