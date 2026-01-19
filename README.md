# FastACH
.NET library for reading and writing ACH files

## Continuous integration

| Branch  | Build status |
|---------|--------------|
| master  | [![Build and Publish](https://github.com/MaratFattakhov/FastACH/actions/workflows/build.yml/badge.svg)](https://github.com/MaratFattakhov/FastACH/actions/workflows/build.yml) |

## Installation [![NuGet](https://img.shields.io/nuget/v/FastACH.svg)](https://www.nuget.org/packages/FastACH/)

### .NET 8.0, .NET 9.0, .NET 10.0
	PM> Install-Package FastACH

Add namespace to the program

``` csharp
using FastACH;
```

## Usage

### Reading ACH file
``` csharp
var achFile = await AchFile.Read("ACH.txt");

achFile.WriteToConsole(); // Output to console
```

``` csharp
// Line map can be used for the error reporting
var lineMap = new List<(IRecord record, uint line)>();
var achFile = await AchFile.Read("ACH.txt", lineMap);
```

### Reading ACH file to colorful console
``` csharp
var achFile = await AchFile.Read(name);
achFile.WriteToConsole();
```

![Console Output](doc/read_to_console.png)

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

await achFile.WriteToFile("ACH.txt");
```

## Perfomance results

I created a benchmark to compare reading performance between FastACH and very popular [ChoETL.Nacha library](https://github.com/Cinchoo/ChoETL.NACHA).

| Method      | NumberOfEntries | Mean         | Error | Gen0          | Gen1          | Gen2          | Allocated       |
|------------ |---------------- |-------------:|------:|--------------:|--------------:|--------------:|----------------:|
| FastACH     | 1000            |     11.62 ms |    NA |             - |             - |             - |       949.88 KB |
| ChoetlNacha | 1000            |    908.52 ms |    NA |   561000.0000 |   555000.0000 |   555000.0000 |   3770936.66 KB |
| FastACH     | 10000           |     31.50 ms |    NA |             - |             - |             - |      9297.73 KB |
| ChoetlNacha | 10000           |  5,316.70 ms |    NA |  2485000.0000 |  2427000.0000 |  2425000.0000 |  37579633.04 KB |
| FastACH     | 100000          |    214.57 ms |    NA |     7000.0000 |     6000.0000 |     2000.0000 |     92815.55 KB |
| ChoetlNacha | 100000          | 43,563.12 ms |    NA | 31105000.0000 | 30491000.0000 | 30491000.0000 | 375791391.55 KB |