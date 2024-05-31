using FluentAssertions;

namespace FastACH.Tests
{
    public class AchFileWriterTests
    {
        [Fact]
        public async Task WriteToFile_Writes_The_Same_File()
        {
            // Arrange
            var reader = new AchFileReader();
            var achFile = await reader.Read("..\\..\\..\\ACH.txt");
            var target = new AchFileWriter();
            var expected = await File.ReadAllLinesAsync("..\\..\\..\\ACH.txt");

            // Act
            await target.WriteToFile(achFile, "..\\..\\..\\ACH_saved.txt");

            // Assert
            var actual = await File.ReadAllLinesAsync("..\\..\\..\\ACH_saved.txt");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
