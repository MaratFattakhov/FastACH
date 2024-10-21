using FastACH.Records;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class AddendaRecordTests
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

            // Act
            var record = new AddendaRecord(s, 1);

            // Assert
            record.Should().BeEquivalentTo(new AddendaRecord()
            {
                AddendaInformation = addendaInformation,
                AddendaSequenceNumber = addendaSequenceNumber,
                EntryDetailSequenceNumber = entryDetailSequenceNumber
            });
        }
    }
}