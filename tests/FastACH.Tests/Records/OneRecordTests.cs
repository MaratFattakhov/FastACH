using FastACH.Models;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class OneRecordTests
    {
        [Fact]
        public void ParseRecord()
        {
            // Arrange
            var s = "101123456789 123456789 2401101418A094101PNC Bank               Microsoft Inc.         00000000";
            var record = new OneRecord();

            // Act
            record.ParseRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new OneRecord
            {
                ImmediateDestination = "123456789",
                ImmediateOrigin = "123456789",
                FileCreationDate = new DateOnly(2024, 1, 10),
                FileCreationTime = new TimeOnly(14, 18, 0, 0, 0),
                FileIdModifier = 'A',
                ImmediateDestinationName = "PNC Bank",
                ImmediateOriginName = "Microsoft Inc.",
                ReferenceCode = "00000000"
            });
        }
    }
}