using FastACH.Records;
using FluentAssertions;

namespace FastACH.Tests
{
    public class FileWritingLineNumberTests
    {
        [Fact]
        public async Task Write_Populates_LineNumbers()
        {
            // Arrange
            var achFile = new AchFile
            {
                FileHeader = new FileHeaderRecord
                {
                    ImmediateDestination = "091000015",
                    ImmediateOrigin = "123456789",
                }
            };
            
            // Just add one batch for testing
            var batch = new BatchRecord
            {
                BatchHeader = new BatchHeaderRecord
                {
                    ServiceClassCode = 200,
                    CompanyName = "YOUR COMPANY",
                    CompanyId = "1234567890",
                    CompanyEntryDescription = "PAYROLL",
                    EffectiveEntryDate = DateOnly.FromDateTime(DateTime.Now),
                    OriginatingDFIID = "12345678",
                }
            };
            
            // Add entry
            var entry = new EntryDetailRecord
            {
                TransactionCode = 22,
                ReceivingDFIID = 12345678,
                CheckDigit = '1',
                DFIAccountNumber = "1234567",
                Amount = 100.50m,
                ReceiverName = "John Doe",
                AddendaRecordIndicator = false
            };
            
            batch.TransactionRecords.Add(new TransactionRecord { EntryDetail = entry });
            achFile.BatchRecordList.Add(batch);

            // Act
            using var stream = new MemoryStream();
            await achFile.WriteToStream(stream);

            // Assert
            achFile.FileHeader.LineNumber.Should().Be(1);
            
            // Batch Header should be line 2
            batch.BatchHeader.LineNumber.Should().Be(2);
            
            // Entry should be line 3
            entry.LineNumber.Should().Be(3);
            
            // Batch Control should be line 4
            batch.BatchControl.LineNumber.Should().Be(4);
            
            // File Control should be line 5
            achFile.FileControl.LineNumber.Should().Be(5);
        }
    }
}
