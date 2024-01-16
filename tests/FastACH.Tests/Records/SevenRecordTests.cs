using FastACH.Models;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class SevenRecordTests
    {
        [Theory]
        [InlineData("Monthly bill", 1, 1)]
        public void ParseRecord(
            string addendaInformation,
            uint addendaSequenceNumber,
            ulong entryDetailSequenceNumber)
        { 
            // Arrange
            var s = $"705{addendaInformation,-80}{addendaSequenceNumber:0000}{entryDetailSequenceNumber:0000000}";
            var record = new SevenRecord();

            // Act
            record.ParseRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new SevenRecord()
            {
                AddendaInformation = addendaInformation,
                AddendaSequenceNumber = addendaSequenceNumber,
                EntryDetailSequenceNumber = entryDetailSequenceNumber
            });
        }
    }
}