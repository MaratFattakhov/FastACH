using FastACH.Models;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class OneRecordTests
    {
        [Theory]
        [InlineData("1234567890", "1234567890", "240110", "1454", 'A', "MyBank", "My Company", "00000000")]
        public void ParseRecord(
            string immediateDestination,
            string immediateOrigin,
            string fileCreationDate,
            string fileCreationTime,
            char fileIdModifier,
            string immediateDestinationName,
            string immediateOriginName,
            string referenceCode)
        {
            // Arrange
            var s = $"101{immediateDestination, 10}{immediateOrigin, 10}{fileCreationDate}{fileCreationTime}{fileIdModifier}094101{immediateDestinationName, -23}{immediateOriginName, -23}{referenceCode, -8}";
            var record = new OneRecord();

            // Act
            record.ParseRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new OneRecord
            {
                ImmediateDestination = immediateDestination,
                ImmediateOrigin = immediateOrigin,
                FileCreationDate = DateOnly.ParseExact(fileCreationDate, "yyMMdd"),
                FileCreationTime = TimeOnly.ParseExact(fileCreationTime, "HHmm"),
                FileIdModifier = fileIdModifier,
                ImmediateDestinationName = immediateDestinationName,
                ImmediateOriginName = immediateOriginName,
                ReferenceCode = referenceCode
            });
        }
    }
}