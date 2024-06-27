using FluentAssertions;

namespace FastACH.Tests
{
    public class FileWritingTests
    {
        [Fact]
        public async Task WriteToFile_Writes_The_Same_File()
        {
            // Arrange
            var achFile = await AchFile.Read("..\\..\\..\\ACH.txt");
            var expected = await File.ReadAllLinesAsync("..\\..\\..\\ACH.txt");

            // Act
            await achFile.WriteToFile("..\\..\\..\\ACH_saved.txt");

            // Assert
            var actual = await File.ReadAllLinesAsync("..\\..\\..\\ACH_saved.txt");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
