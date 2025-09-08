using FluentAssertions;

namespace FastACH.Tests
{
    public class FileReadingLineMapTests
    {
        [Fact]
        public async Task Read_WithLineMap_Returns_LineMap()
        {
            // Arrange & Act
            var lineMap = new List<(IRecord record, uint line)>();
            var achFile = await AchFile.Read("..\\..\\..\\ACH.txt", lineMap);

            // Assert
            lineMap.Should().NotBeNull();
            lineMap.Should().NotBeEmpty();
            // First record should be a FileHeaderRecord with line 1
            var first = lineMap.First();
            first.line.Should().Be(1);
            achFile.FileHeader.Should().BeSameAs(first.record);
            // Last record should be FileControlRecord
            var last = lineMap.Last();
            last.record.Should().BeSameAs(achFile.FileControl);
        }

        [Fact]
        public async Task Read_WithLineMap_LineNumbers_Are_Strictly_Increasing()
        {
            var lineMap = new List<(IRecord record, uint line)>();
            var _ = await AchFile.Read("..\\..\\..\\ACH.txt", lineMap);
            lineMap.Select(x => x.line).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task Read_WithLineMap_Count_Matches_Number_Of_Records()
        {
            var lineNumbers = new List<(IRecord record, uint line)>();
            var achFile = await AchFile.Read("..\\..\\..\\ACH.txt", lineNumbers);

            var expectedCount = 2 // file header + file control
                + achFile.BatchRecordList.Count * 2 // each batch header + batch control
                + achFile.BatchRecordList.SelectMany(b => b.TransactionRecords).Count() // entry detail records
                + achFile.BatchRecordList.SelectMany(b => b.TransactionRecords).SelectMany(t => t.AddendaRecords).Count();

            lineNumbers.Count.Should().Be(expectedCount);
        }
    }
}
