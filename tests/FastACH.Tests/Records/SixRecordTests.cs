using FastACH.Records;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class SixRecordTests
    {
        [Theory]
        [InlineData(22, 21234567, '3', "123456789", 22.55, "ID Number", "Serrano", "13", false, 923456780000001)]
        public void ParseRecord(
            uint transactionCode,
            ulong receivingDFINumber,
            char checkDigit,
            string DFIAccountNumber,
            decimal amount,
            string receiverIdentificationNumber,
            string receiverName,
            string discretionaryData,
            bool addendaRecordIndicator,
            ulong traceNumber)
        {
            // Arrange
            var s = $"6{transactionCode, 2}{receivingDFINumber, 8}{checkDigit}{DFIAccountNumber, -17}{(uint)(amount * 100):0000000000}{receiverIdentificationNumber, -15}{receiverName, -22}{discretionaryData, 2}{(addendaRecordIndicator ? "1" : "0"), 1}{traceNumber:000000000000000}";
            var record = new SixRecord();

            // Act
            record.ParseRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new SixRecord()
            {
                TransactionCode = transactionCode,
                AddendaRecordIndicator = addendaRecordIndicator,
                Amount = amount,
                CheckDigit = checkDigit,
                DFIAccountNumber = DFIAccountNumber,
                DiscretionaryData = discretionaryData,
                ReceiverIdentificationNumber = receiverIdentificationNumber,
                ReceiverName = receiverName,
                ReceivingDFINumber = receivingDFINumber,
                TraceNumber = traceNumber.ToString(),
            });
        }
    }
}