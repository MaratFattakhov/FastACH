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
    OneRecord = new FileHeaderRecord()
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
                new TransactionRecord
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
                new TransactionRecord()
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

await writer.WriteToFile(achFile, "ACH.txt");
```

## Perfomance results

I created a benchmark to compare reading performance between FastACH and very popular [ChoETL.Nacha library](https://github.com/Cinchoo/ChoETL.NACHA).

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3007/22H2/2022Update/SunValley2)
13th Gen Intel Core i7-1370P, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 7.0.15 (7.0.1523.57226), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.15 (7.0.1523.57226), X64 RyuJIT AVX2


| Method       | NumberOfEntries | Mean          | Error      | StdDev     |
|------------- |---------------- |--------------:|-----------:|-----------:|
| FastACH      | 1000            |     0.8138 ms |  0.0123 ms |  0.0109 ms |
| ChoETL.Nacha | 1000            |   335.7584 ms |  6.6044 ms | 13.1896 ms |
| FastACH      | 10000           |    10.0477 ms |  0.1996 ms |  0.4505 ms |
| ChoETL.Nacha | 10000           | 3,240.3946 ms | 54.2025 ms | 50.7010 ms |