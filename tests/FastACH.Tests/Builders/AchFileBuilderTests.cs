using FastACH.Builders;
using FluentAssertions;

namespace FastACH.Tests.Builders
{
    public class AchFileBuilderTests
    {
        [Fact]
        public void Build_WithMinimalData_ShouldCreateValidAchFile()
        {
            // Arrange & Act
            var achFile = new AchFileBuilder()
                .With(
                    ImmediateDestination: "123456789",
                    ImmediateOrigin: "987654321")
                .Build();

            // Assert
            achFile.Should().NotBeNull();
            achFile.FileHeader.Should().NotBeNull();
            achFile.FileHeader.ImmediateDestination.Should().Be("123456789");
            achFile.FileHeader.ImmediateOrigin.Should().Be("987654321");
            achFile.BatchRecordList.Should().BeEmpty();
        }

        [Fact]
        public void With_AllFileHeaderFields_ShouldSetAllProperties()
        {
            // Arrange
            var expectedDestination = "123456789";
            var expectedOrigin = "987654321";
            var expectedDestinationName = "Destination Bank";
            var expectedOriginName = "Origin Company";
            var expectedReferenceCode = "REF12345";
            var expectedFileIdModifier = 'B';

            // Act
            var achFile = new AchFileBuilder()
                .With(
                    ImmediateDestination: expectedDestination,
                    ImmediateOrigin: expectedOrigin,
                    ImmediateDestinationName: expectedDestinationName,
                    ImmediateOriginName: expectedOriginName,
                    ReferenceCode: expectedReferenceCode,
                    FileIdModifier: expectedFileIdModifier)
                .Build();

            // Assert
            achFile.FileHeader.ImmediateDestination.Should().Be(expectedDestination);
            achFile.FileHeader.ImmediateOrigin.Should().Be(expectedOrigin);
            achFile.FileHeader.ImmediateDestinationName.Should().Be(expectedDestinationName);
            achFile.FileHeader.ImmediateOriginName.Should().Be(expectedOriginName);
            achFile.FileHeader.ReferenceCode.Should().Be(expectedReferenceCode);
            achFile.FileHeader.FileIdModifier.Should().Be(expectedFileIdModifier);
        }

        [Fact]
        public void With_DefaultFileIdModifier_ShouldBeA()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With(
                    ImmediateDestination: "123456789",
                    ImmediateOrigin: "987654321")
                .Build();

            // Assert
            achFile.FileHeader.FileIdModifier.Should().Be('A');
        }

        [Fact]
        public void WithBatch_MinimalBatchData_ShouldCreateValidBatch()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With(
                        CompanyId: "1234567890",
                        OriginatingDFIID: "12345678",
                        CompanyEntryDescription: "PAYROLL",
                        CompanyName: "My Company"))
                .Build();

            // Assert
            achFile.BatchRecordList.Should().HaveCount(1);
            var batchHeader = achFile.BatchRecordList[0].BatchHeader;
            batchHeader.CompanyId.Should().Be("1234567890");
            batchHeader.OriginatingDFIID.Should().Be("12345678");
            batchHeader.CompanyEntryDescription.Should().Be("PAYROLL");
            batchHeader.CompanyName.Should().Be("My Company");
        }

        [Fact]
        public void WithBatch_AllBatchHeaderFields_ShouldSetAllProperties()
        {
            // Arrange
            var expectedCompanyId = "1234567890";
            var expectedOriginatingDFIID = "12345678";
            var expectedCompanyEntryDescription = "PAYROLL";
            var expectedCompanyName = "My Company";
            var expectedServiceClassCode = 220u;
            var expectedEntryClassCode = "CCD";
            var expectedCompanyDiscretionaryData = "DISC DATA";
            var expectedCompanyDescriptiveDate = new DateOnly(2024, 1, 15);
            var expectedEffectiveEntryDate = new DateOnly(2024, 1, 20);
            var expectedOriginatorsStatusCode = '2';
            var expectedBatchNumber = 5ul;

            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With(
                        CompanyId: expectedCompanyId,
                        OriginatingDFIID: expectedOriginatingDFIID,
                        CompanyEntryDescription: expectedCompanyEntryDescription,
                        CompanyName: expectedCompanyName,
                        ServiceClassCode: expectedServiceClassCode,
                        entryClassCode: expectedEntryClassCode,
                        CompanyDiscretionaryData: expectedCompanyDiscretionaryData,
                        CompanyDescriptiveDate: expectedCompanyDescriptiveDate,
                        EffectiveEntryDate: expectedEffectiveEntryDate,
                        OriginatorsStatusCode: expectedOriginatorsStatusCode,
                        BatchNumber: expectedBatchNumber))
                .Build();

            // Assert
            var batchHeader = achFile.BatchRecordList[0].BatchHeader;
            batchHeader.CompanyId.Should().Be(expectedCompanyId);
            batchHeader.OriginatingDFIID.Should().Be(expectedOriginatingDFIID);
            batchHeader.CompanyEntryDescription.Should().Be(expectedCompanyEntryDescription);
            batchHeader.CompanyName.Should().Be(expectedCompanyName);
            batchHeader.ServiceClassCode.Should().Be(expectedServiceClassCode);
            batchHeader.StandardEntryClassCode.Should().Be(expectedEntryClassCode);
            batchHeader.CompanyDiscretionaryData.Should().Be(expectedCompanyDiscretionaryData);
            batchHeader.CompanyDescriptiveDate.Should().Be(expectedCompanyDescriptiveDate);
            batchHeader.EffectiveEntryDate.Should().Be(expectedEffectiveEntryDate);
            batchHeader.OriginatorsStatusCode.Should().Be(expectedOriginatorsStatusCode);
            batchHeader.BatchNumber.Should().Be(expectedBatchNumber);
        }

        [Fact]
        public void WithBatch_DefaultValues_ShouldUseExpectedDefaults()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With(
                        CompanyId: "1234567890",
                        OriginatingDFIID: "12345678",
                        CompanyEntryDescription: "PAYROLL",
                        CompanyName: "My Company"))
                .Build();

            // Assert
            var batchHeader = achFile.BatchRecordList[0].BatchHeader;
            batchHeader.ServiceClassCode.Should().Be(1u);
            batchHeader.StandardEntryClassCode.Should().Be("PPD");
            batchHeader.CompanyDiscretionaryData.Should().Be("");
            batchHeader.CompanyDescriptiveDate.Should().BeNull();
            batchHeader.EffectiveEntryDate.Should().BeNull();
            batchHeader.OriginatorsStatusCode.Should().Be('1');
            batchHeader.BatchNumber.Should().Be(0ul);
        }

        [Fact]
        public void WithCreditTransaction_AllFields_ShouldSetAllProperties()
        {
            // Arrange
            var expectedAmount = 100.50m;
            var expectedRoutingNumber = "123456789";
            var expectedAccountNumber = "987654321";
            var expectedReceiverName = "John Doe";
            var expectedReceiverId = "ID123";
            var expectedDiscretionaryData = "DD";

            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "My Company")
                    .WithCreditTransaction(
                        amount: expectedAmount,
                        routingNumber: expectedRoutingNumber,
                        accountNumber: expectedAccountNumber,
                        receiverName: expectedReceiverName,
                        receiverId: expectedReceiverId,
                        discretionaryData: expectedDiscretionaryData))
                .Build();

            // Assert
            var transaction = achFile.BatchRecordList[0].TransactionRecords[0];
            transaction.EntryDetail.Amount.Should().Be(expectedAmount);
            transaction.EntryDetail.DFIAccountNumber.Should().Be(expectedAccountNumber);
            transaction.EntryDetail.ReceiverName.Should().Be(expectedReceiverName);
            transaction.EntryDetail.ReceiverIdentificationNumber.Should().Be(expectedReceiverId);
            transaction.EntryDetail.DiscretionaryData.Should().Be(expectedDiscretionaryData);
            transaction.EntryDetail.TransactionCode.Should().Be(22u); // Credit transaction code
            transaction.EntryDetail.AddendaRecordIndicator.Should().BeFalse();
        }

        [Fact]
        public void WithDebitTransaction_AllFields_ShouldSetAllProperties()
        {
            // Arrange
            var expectedAmount = 75.25m;
            var expectedRoutingNumber = "123456789";
            var expectedAccountNumber = "987654321";
            var expectedReceiverName = "Jane Smith";
            var expectedReceiverId = "ID456";
            var expectedDiscretionaryData = "XY";

            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "My Company")
                    .WithDebitTransaction(
                        amount: expectedAmount,
                        routingNumber: expectedRoutingNumber,
                        accountNumber: expectedAccountNumber,
                        receiverName: expectedReceiverName,
                        receiverId: expectedReceiverId,
                        discretionaryData: expectedDiscretionaryData))
                .Build();

            // Assert
            var transaction = achFile.BatchRecordList[0].TransactionRecords[0];
            transaction.EntryDetail.Amount.Should().Be(expectedAmount);
            transaction.EntryDetail.DFIAccountNumber.Should().Be(expectedAccountNumber);
            transaction.EntryDetail.ReceiverName.Should().Be(expectedReceiverName);
            transaction.EntryDetail.ReceiverIdentificationNumber.Should().Be(expectedReceiverId);
            transaction.EntryDetail.DiscretionaryData.Should().Be(expectedDiscretionaryData);
            transaction.EntryDetail.TransactionCode.Should().Be(27u); // Debit transaction code
            transaction.EntryDetail.AddendaRecordIndicator.Should().BeFalse();
        }

        [Fact]
        public void WithCreditTransaction_DefaultOptionalFields_ShouldUseEmptyStrings()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "My Company")
                    .WithCreditTransaction(100.00m, "123456789", "987654321"))
                .Build();

            // Assert
            var transaction = achFile.BatchRecordList[0].TransactionRecords[0];
            transaction.EntryDetail.ReceiverName.Should().Be("");
            transaction.EntryDetail.ReceiverIdentificationNumber.Should().Be("");
            transaction.EntryDetail.DiscretionaryData.Should().Be("");
        }

        [Fact]
        public void WithAddenda_AllFields_ShouldSetAllProperties()
        {
            // Arrange
            var expectedAddendaTypeCode = 5u;
            var expectedAddendaInformation = "Payment for invoice #12345";
            var expectedAddendaSequenceNumber = 2u;

            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "My Company")
                    .WithCreditTransaction(100.00m, "123456789", "987654321")
                    .WithAddenda(
                        addendaTypeCode: expectedAddendaTypeCode,
                        addendaInformation: expectedAddendaInformation,
                        addendaSequenceNumber: expectedAddendaSequenceNumber))
                .Build();

            // Assert
            var transaction = achFile.BatchRecordList[0].TransactionRecords[0];
            transaction.EntryDetail.AddendaRecordIndicator.Should().BeTrue();
            transaction.AddendaRecords.Should().HaveCount(1);
            transaction.AddendaRecords![0].AddendaTypeCode.Should().Be(expectedAddendaTypeCode);
            transaction.AddendaRecords[0].AddendaInformation.Should().Be(expectedAddendaInformation);
            transaction.AddendaRecords[0].AddendaSequenceNumber.Should().Be(expectedAddendaSequenceNumber);
        }

        [Fact]
        public void WithAddenda_DefaultSequenceNumber_ShouldBeOne()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "My Company")
                    .WithCreditTransaction(100.00m, "123456789", "987654321")
                    .WithAddenda(5u, "Payment info"))
                .Build();

            // Assert
            var transaction = achFile.BatchRecordList[0].TransactionRecords[0];
            transaction.AddendaRecords![0].AddendaSequenceNumber.Should().Be(1u);
        }

        [Fact]
        public void WithAddenda_WithoutTransaction_ShouldThrowException()
        {
            // Arrange
            var builder = new AchFileBuilder()
                .With("123456789", "987654321");

            // Act & Assert
            Action act = () => builder.WithBatch(batch => batch
                .With("1234567890", "12345678", "PAYROLL", "My Company")
                .WithAddenda(5u, "Payment info"));

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot add addenda without a transaction. Add a transaction first.");
        }

        [Fact]
        public void WithBatch_MultipleTransactions_ShouldAddAllTransactions()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "My Company")
                    .WithCreditTransaction(100.00m, "123456789", "111111111")
                    .WithDebitTransaction(50.00m, "123456789", "222222222")
                    .WithCreditTransaction(75.00m, "123456789", "333333333"))
                .Build();

            // Assert
            achFile.BatchRecordList[0].TransactionRecords.Should().HaveCount(3);
            achFile.BatchRecordList[0].TransactionRecords[0].EntryDetail.TransactionCode.Should().Be(22u); // Credit
            achFile.BatchRecordList[0].TransactionRecords[1].EntryDetail.TransactionCode.Should().Be(27u); // Debit
            achFile.BatchRecordList[0].TransactionRecords[2].EntryDetail.TransactionCode.Should().Be(22u); // Credit
        }

        [Fact]
        public void WithBatch_MultipleBatches_ShouldAddAllBatches()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With("123456789", "987654321")
                .WithBatch(batch => batch
                    .With("1234567890", "12345678", "PAYROLL", "Company A")
                    .WithCreditTransaction(100.00m, "123456789", "111111111"))
                .WithBatch(batch => batch
                    .With("0987654321", "87654321", "INVOICE", "Company B")
                    .WithDebitTransaction(50.00m, "123456789", "222222222"))
                .Build();

            // Assert
            achFile.BatchRecordList.Should().HaveCount(2);
            achFile.BatchRecordList[0].BatchHeader.CompanyName.Should().Be("Company A");
            achFile.BatchRecordList[1].BatchHeader.CompanyName.Should().Be("Company B");
        }

        [Fact]
        public void FluentAPI_ComplexScenario_ShouldBuildCorrectly()
        {
            // Act
            var achFile = new AchFileBuilder()
                .With(
                    ImmediateDestination: "123456789",
                    ImmediateOrigin: "987654321",
                    ImmediateDestinationName: "Bank of America",
                    ImmediateOriginName: "My Corporation",
                    ReferenceCode: "REF001",
                    FileIdModifier: 'C')
                .WithBatch(batch => batch
                    .With(
                        CompanyId: "1234567890",
                        OriginatingDFIID: "12345678",
                        CompanyEntryDescription: "PAYROLL",
                        CompanyName: "My Company",
                        ServiceClassCode: 200u,
                        entryClassCode: "PPD",
                        CompanyDiscretionaryData: "DISC",
                        EffectiveEntryDate: new DateOnly(2024, 1, 31))
                    .WithCreditTransaction(
                        amount: 1500.00m,
                        routingNumber: "111111111",
                        accountNumber: "ACCT001",
                        receiverName: "Employee One",
                        receiverId: "EMP001",
                        discretionaryData: "D1")
                    .WithAddenda(
                        addendaTypeCode: 5u,
                        addendaInformation: "Salary payment",
                        addendaSequenceNumber: 1u)
                    .WithCreditTransaction(
                        amount: 2000.00m,
                        routingNumber: "222222222",
                        accountNumber: "ACCT002",
                        receiverName: "Employee Two",
                        receiverId: "EMP002",
                        discretionaryData: "D2"))
                .Build();

            // Assert
            achFile.FileHeader.ImmediateDestination.Should().Be("123456789");
            achFile.FileHeader.FileIdModifier.Should().Be('C');
            achFile.BatchRecordList.Should().HaveCount(1);
            achFile.BatchRecordList[0].BatchHeader.EffectiveEntryDate.Should().Be(new DateOnly(2024, 1, 31));
            achFile.BatchRecordList[0].TransactionRecords.Should().HaveCount(2);
            achFile.BatchRecordList[0].TransactionRecords[0].EntryDetail.DiscretionaryData.Should().Be("D1");
            achFile.BatchRecordList[0].TransactionRecords[0].AddendaRecords.Should().HaveCount(1);
            achFile.BatchRecordList[0].TransactionRecords[1].EntryDetail.DiscretionaryData.Should().Be("D2");
            achFile.BatchRecordList[0].TransactionRecords[1].AddendaRecords.Should().BeEmpty();
        }
    }
}
