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
using FastACH.Builders; // For using AchFileBuilder
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

### Writing ACH file using AchFileBuilder (Recommended)

The `AchFileBuilder` provides a fluent API for building ACH files with less boilerplate code.

#### Simple Example - Credit and Debit Transactions
``` csharp
var achFile = new AchFileBuilder()
    .With(
        ImmediateDestination: "123456789",
        ImmediateOrigin: "987654321",
        ImmediateDestinationName: "Bank of America",
        ImmediateOriginName: "My Corporation")
    .WithBatch(batch => batch
        .With(
            CompanyId: "1234567890",
            OriginatingDFIID: "12345678",
            CompanyEntryDescription: "PAYROLL",
            CompanyName: "My Company")
        .WithCreditTransaction(
            amount: 1500.00m,
            routingNumber: "111111111",
            accountNumber: "987654321",
            receiverName: "John Doe")
        .WithDebitTransaction(
            amount: 500.00m,
            routingNumber: "222222222",
            accountNumber: "123456789",
            receiverName: "Jane Smith"))
    .Build();

await achFile.WriteToFile("ACH.txt");
```

#### Advanced Example - With All Optional Fields
``` csharp
var achFile = new AchFileBuilder()
    .With(
        ImmediateDestination: "123456789",
        ImmediateOrigin: "987654321",
        ImmediateDestinationName: "PNC Bank",
        ImmediateOriginName: "Microsoft Inc.",
        ReferenceCode: "REF12345",
        FileIdModifier: 'A')
    .WithBatch(batch => batch
        .With(
            CompanyId: "1234567890",
            OriginatingDFIID: "12345678",
            CompanyEntryDescription: "PAYROLL",
            CompanyName: "My Company",
            ServiceClassCode: 200,
            entryClassCode: "PPD",
            CompanyDiscretionaryData: "DISCRETIONARY",
            CompanyDescriptiveDate: new DateOnly(2024, 1, 15),
            EffectiveEntryDate: new DateOnly(2024, 1, 31),
            OriginatorsStatusCode: '1',
            BatchNumber: 1)
        .WithCreditTransaction(
            amount: 1500.00m,
            routingNumber: "111111111",
            accountNumber: "987654321",
            receiverName: "Employee One",
            receiverId: "EMP001",
            discretionaryData: "PAY")
        .WithAddenda(
            addendaTypeCode: 5,
            addendaInformation: "Salary payment for January 2024",
            addendaSequenceNumber: 1))
    .Build();

await achFile.WriteToFile("ACH.txt");
```

#### Multiple Batches Example
``` csharp
var achFile = new AchFileBuilder()
    .With("123456789", "987654321")
    .WithBatch(batch => batch
        .With("1234567890", "12345678", "PAYROLL", "Company A")
        .WithCreditTransaction(1000.00m, "111111111", "ACCT001", "Employee 1")
        .WithCreditTransaction(1200.00m, "222222222", "ACCT002", "Employee 2"))
    .WithBatch(batch => batch
        .With("0987654321", "87654321", "INVOICE", "Company B")
        .WithDebitTransaction(500.00m, "333333333", "ACCT003", "Customer 1")
        .WithDebitTransaction(750.00m, "444444444", "ACCT004", "Customer 2"))
    .Build();

await achFile.WriteToFile("ACH.txt");
```

### Writing ACH file (Manual Approach)
``` csharp
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
            TransactionRecords =
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
                    AddendaRecords = new List<AddendaRecord>
                    {
                        new AddendaRecord()
                        {
                            AddendaInformation = "Monthly bill"
                        }
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
                    }
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