using FastACH.Models;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class NineRecordTests
    {
        [Theory]
        [InlineData(2, 1, 3, 24691356, 123.51, 345.51)]
        public void ParseRecord(
            int batchCount,
            int blockCount,
            int entryAddendaCount,
            int entryHash,
            decimal debit,
            decimal credit)
        {
            // Arrange
            var s = $"9{batchCount,6}{blockCount,6}{entryAddendaCount,8}{entryHash, 10}{(uint)(debit * 100), 12}{(uint)(credit * 100),12}                                       ";
            var record = new NineRecord();

            // Act
            record.ParseRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new NineRecord()
            {
                BatchCount = batchCount,
                BlockCount = blockCount,
                EntryAddendaCount = entryAddendaCount,
                EntryHash = entryHash,
                TotalCreditEntryDollarAmount = credit,
                TotalDebitEntryDollarAmount = debit,
            });
            record.Reserved.Should().Be(new string(' ', 39));
            record.RecordTypeCode.Should().Be("9");
        }
    }
}