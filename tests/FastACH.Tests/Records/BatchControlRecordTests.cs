using FastACH.Records;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class BatchControlRecordTests
    {
        [Theory]
        [InlineData(200, "Test", "", 4, 2, 3, 24691356, 123.51, 345.51)]
        public void ParseRecord(
            uint serviceClassCode,
            string companyId,
            string authCode,
            uint batchNumber,
            int dfiId,
            uint entryAddendaCount,
            ulong entryHash,
            decimal debit,
            decimal credit)
        {
            // Arrange
            var s = $"8{serviceClassCode}{entryAddendaCount,6}{entryHash,10}{(uint)(debit * 100),12}{(uint)(credit * 100),12}{companyId,10}{authCode,19}      {dfiId,8}{batchNumber,7}";

            // Act
            var record = new BatchControlRecord(s, 0);

            // Assert
            record.Should().BeEquivalentTo(new BatchControlRecord()
            {
                ServiceClassCode = serviceClassCode,
                CompanyIdentification = companyId,
                MessageAuthenticationCode = authCode,
                BatchNumber = batchNumber,
                OriginatingDFINumber = dfiId.ToString(),
                EntryAddendaCount = entryAddendaCount,
                EntryHash = entryHash,
                TotalCreditEntryDollarAmount = credit,
                TotalDebitEntryDollarAmount = debit,
            });
            record.Reserved.Should().Be("      ");
            record.RecordTypeCode.Should().Be("8");
        }
    }
}