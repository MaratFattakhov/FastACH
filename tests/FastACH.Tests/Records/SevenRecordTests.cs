using FastACH.Models;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class SevenRecordTests
    {
        [Theory]
        [InlineData("Monthly bill", "0001", "0000001")]
        public void ParseRecord(
            string addendaInformation,
            string addendaSequenceNumber,
            string entryDetailSequenceNumber)
        { 
            // Arrange
            var s = $"705{addendaInformation,-80}{addendaSequenceNumber}{entryDetailSequenceNumber}";
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