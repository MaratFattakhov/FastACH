using FluentAssertions;
using System.Text;

namespace FastACH.Tests
{
    public class FileReadingTests : IDisposable
    {
        private readonly string _tempFilePath = Path.GetTempFileName();

        public void Dispose()
        {
            File.Delete(_tempFilePath);
        }

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
            Func<Task> act = async () => await AchFile.Read(_tempFilePath);
            await act.Should().ThrowAsync<AchFileReadingException>();
        }

        [Fact]
        public async Task Read_FileWithOnlyHeader_ThrowsException()
        {
            await File.WriteAllTextAsync(_tempFilePath, "101 123456789 9876543212001010100A094101Your Bank      Your Name           ");
            Func<Task> act = async () => await AchFile.Read(_tempFilePath);
            await act.Should().ThrowAsync<AchFileReadingException>();
        }

        [Fact]
        public async Task Read_FileWithNonAsciiCharacter_ThrowsAchFileReadingException()
        {
            var achContent =
                "101 123456789 1234567892401241434A094101PNC Bank               Microsoft Inc.         00000000\r\n" +
                "5200company Name    companyDiscretionarycompanyID PPDEntryDescr110203110102   1DFINumbe0000001\r\n" +
                "6221234567891313131313       0000002200ID Number      ID\u2019Name               De0123456780000001\r\n" +
                "82000000010012345678900000000000000000002200companyID                          DFINumbe0000001\r\n" +
                "9000001000001000000010012345678900000000000000002200000000000000000                           \r\n";

            await File.WriteAllTextAsync(_tempFilePath, achContent, Encoding.UTF8);

            Func<Task> act = async () => await AchFile.Read(_tempFilePath);

            await act.Should().ThrowAsync<AchFileReadingException>()
                .WithMessage("An Error happened on 3 line: Invalid character found at position 56: 6221234567891313131313       0000002200ID Number      ID\u2019Name               De0123456780000001");
        }

        [Fact]
        public async Task Read_AddendaRecordWithoutIndicator_ThrowsAchFileReadingException()
        {
            // Entry detail record has AddendaRecordIndicator = '0' (no addenda expected),
            // but an addenda (7) record follows.
            var achContent =
                "101 123456789 1234567892401241434A094101PNC Bank               Microsoft Inc.         00000000\r\n" +
                "5200companyName     companyDiscretionarycompanyID PPDEntryDescr110203110102   1DFINumbe0000001\r\n" +
                "6221234567891313131313       0000002200ID Number      ID Name               De0123456780000001\r\n" +
                "705Monthly bill                                                                    00010000001\r\n" +
                "82000000020024691356000000000000000000002200companyID                          DFINumbe0000001\r\n" +
                "9000001000001000000020024691356000000000000000000002200                                       \r\n";

            await File.WriteAllTextAsync(_tempFilePath, achContent, Encoding.UTF8);

            Func<Task> act = async () => await AchFile.Read(_tempFilePath);

            await act.Should().ThrowAsync<AchFileReadingException>()
                .WithMessage("*Addenda record found, but entry detail record indicates there are no addenda records.*");
        }

        [Fact]
        public async Task Read_BatchControlWithNoEntryRecords_ThrowsAchFileReadingException()
        {
            // Batch header (5) immediately followed by batch control (8) with no entry (6) records.
            var achContent =
                "101 123456789 1234567892401241434A094101PNC Bank               Microsoft Inc.         00000000\r\n" +
                "5200companyName     companyDiscretionarycompanyID PPDEntryDescr110203110102   1DFINumbe0000001\r\n" +
                "82000000000000000000000000000000000000000000companyID                          DFINumbe0000001\r\n" +
                "9000001000001000000000000000000000000000000000000000                                         \r\n";

            await File.WriteAllTextAsync(_tempFilePath, achContent, Encoding.UTF8);

            Func<Task> act = async () => await AchFile.Read(_tempFilePath);

            await act.Should().ThrowAsync<AchFileReadingException>()
                .WithMessage("*At least one entry (6) record is required*");
        }

        [Fact]
        public async Task Read_EntryDetailIndicatesAddendaButNoneFound_ThrowsAchFileReadingException()
        {
            // Entry detail record has AddendaRecordIndicator = '1' (addenda expected),
            // but batch control (8) follows immediately without any addenda (7) record.
            var achContent =
                "101 123456789 1234567892401241434A094101PNC Bank               Microsoft Inc.         00000000\r\n" +
                "5200companyName     companyDiscretionarycompanyID PPDEntryDescr110203110102   1DFINumbe0000001\r\n" +
                "6221234567891313131313       0000002200ID Number      ID Name               De1123456780000001\r\n" +
                "82000000010012345678900000000000000000002200companyID                          DFINumbe0000001\r\n" +
                "9000001000001000000010012345678900000000000000000002200                                       \r\n";

            await File.WriteAllTextAsync(_tempFilePath, achContent, Encoding.UTF8);

            Func<Task> act = async () => await AchFile.Read(_tempFilePath);

            await act.Should().ThrowAsync<AchFileReadingException>()
                .WithMessage("*Entry detail record indicates there are addenda records, but no addenda records found.*");
        }
    }
}
