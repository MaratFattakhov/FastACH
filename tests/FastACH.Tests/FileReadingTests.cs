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

        [Fact]
        public async Task Read_EmptyFile_ThrowsException()
        {
            var path = Path.GetTempFileName();
            try
            {
                Func<Task> act = async () => await AchFile.Read(path);
                await act.Should().ThrowAsync<AchFileReadingException>();
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public async Task Read_FileWithOnlyHeader_ThrowsException()
        {
            var path = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(path, "101 123456789 9876543212001010100A094101Your Bank      Your Name           ");
                Func<Task> act = async () => await AchFile.Read(path);
                await act.Should().ThrowAsync<AchFileReadingException>();
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
