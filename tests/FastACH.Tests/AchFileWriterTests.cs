using FluentAssertions;

namespace FastACH.Tests
{
    public class AchFileWriterTests
    {
        [Fact]
        public async Task WriteToFile_Writes_10_lines()
        {
            // Arrange
            var reader = new AchFileReader();
            var achFile = await reader.Read("..\\..\\..\\ACH.txt");
            var target = new AchFileWriter();

            // Act
            await target.WriteToFile(achFile, "..\\..\\..\\ACH_saved.txt");

            // Assert
            (await File.ReadAllLinesAsync("..\\..\\..\\ACH_saved.txt")).Length.Should().Be(10);
        }
    }
}
