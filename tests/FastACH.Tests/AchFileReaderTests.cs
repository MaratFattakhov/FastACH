using FluentAssertions;

namespace FastACH.Tests
{
    public class AchFileReaderTests
    {
        [Fact]
        public async Task Read_Returns_AchFile()
        {
            // Arrange
            var target = new AchFileReader();

            // Act
            var actual = await target.Read("..\\..\\..\\ACH.txt");

            // Assert
            actual.Should().NotBeNull();
            actual.OneRecord.Should().NotBeNull();
            actual.BatchRecordList.Should().NotBeNull();
        }
    }
}
