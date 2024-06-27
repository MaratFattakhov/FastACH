using FluentAssertions;

namespace FastACH.Tests
{
    public class FileReadingTests
    {
        [Fact]
        public async Task Read_Returns_AchFile()
        {
            // Arrange && Act
            var actual = await AchFile.Read("..\\..\\..\\ACH.txt");

            // Assert
            actual.Should().NotBeNull();
            actual.FileHeader.Should().NotBeNull();
            actual.BatchRecordList.Should().NotBeNull();
        }
    }
}
