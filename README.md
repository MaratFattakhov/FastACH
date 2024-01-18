[![CircleCI](https://dl.circleci.com/status-badge/img/circleci/WCT5H9fXuyQcpJQ1Sfb13U/BoxnGAvKKRXYvq1VrtDyaF/tree/master.svg?style=shield&circle-token=253f4e8ac4b2923770cf508b09a77ce98e8ff63e)](https://dl.circleci.com/status-badge/redirect/circleci/WCT5H9fXuyQcpJQ1Sfb13U/BoxnGAvKKRXYvq1VrtDyaF/tree/master)

# FastACH
.NET library for reading and writing ACH files

## Installation [![NuGet](https://img.shields.io/nuget/v/FastACH.svg)](https://www.nuget.org/packages/FastACH/)

### .NET 7.0
	PM> Install-Package FastACH

Add namespace to the program

``` csharp
using FastACH;
```

## Usage

### Reading ACH file
``` csharp
var reader = new AchFileReader();

var achFile = await reader.Read("ACH.txt");

var achFileWriter = new AchFileWriter();

achFileWriter.WriteToConsole(achFile); // Output to console
```

### Writing ACH file
``` csharp
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
            CompanyIdentification = "companyID",
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
                        ReceivingDFINumber = 12345678,
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
                        ReceivingDFINumber = 12345678,
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

await writer.WriteToFile(achFile, "ACH.txt");
```

## Perfomance results
todo