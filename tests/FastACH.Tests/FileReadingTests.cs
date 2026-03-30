using FluentAssertions;
using System.Text;

namespace FastACH.Tests
{
    public class FileReadingTests
    {
        [Fact]
        public async Task Read_Returns_AchFile()
        {
            // Arrange && Act
            var actual = await AchFile.Read("ACH.txt");

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

        [Fact]
        public async Task Read_FileWithNonAsciiCharacter_ThrowsAchFileReadingException()
        {
            // Arrange
            // The curly apostrophe (U+2019) in the company name is a non-ASCII character.
            // AchFile.Read rejects characters >= 128 and throws AchFileReadingException.
            var achContent =
                "101123456789 123456789 2401101418A094101PNC Bank               Microsoft Inc.         00000000\r\n" +
                "5200companyName     companyDiscretionarycompanyID PPDEntryDescr110203110102   1DFINumbe0000001\r\n" +
                "6221234567891313131313       0000002251ID Number      ID’Name               De1123456780000001\r\n" +
                "82000000010012345678900000000000000000002251companyID                          DFINumbe0000001\r\n" +
                "9000001000001000000010012345678900000000000000002251000000000000000                             \r\n";

            var path = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(path, achContent, Encoding.UTF8);

                // Act
                Func<Task> act = async () => await AchFile.Read(path);

                // Assert
                var ex = await act.Should().ThrowAsync<AchFileReadingException>();
                ex.WithInnerException<ArgumentException>()
                  .WithMessage("*Invalid character found at position*");
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
