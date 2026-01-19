using FluentAssertions;

namespace FastACH.Tests
{
    public class FileReadingLineMapTests
    {
        [Fact]
        public async Task Read_Populates_LineNumbers()
        {
            // Arrange & Act
            var achFile = await AchFile.Read("ACH.txt");

            // Assert
            achFile.FileHeader.LineNumber.Should().Be(1);
            achFile.FileControl.LineNumber.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task Read_LineNumbers_Are_Strictly_Increasing()
        {
            var achFile = await AchFile.Read("ACH.txt");
            
            var records = new List<IRecord>();
            records.Add(achFile.FileHeader);
            foreach(var batch in achFile.BatchRecordList)
            {
                records.Add(batch.BatchHeader);
                foreach(var transaction in batch.TransactionRecords)
                {
                    records.Add(transaction.EntryDetail);
                    records.AddRange(transaction.AddendaRecords);
                }
                records.Add(batch.BatchControl);
            }
            records.Add(achFile.FileControl);

            records.Select(x => x.LineNumber).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task Read_Count_Matches_Number_Of_Records()
        {
            var achFile = await AchFile.Read("ACH.txt");

            var records = new List<IRecord>();
            records.Add(achFile.FileHeader);
            foreach(var batch in achFile.BatchRecordList)
            {
                records.Add(batch.BatchHeader);
                foreach(var transaction in batch.TransactionRecords)
                {
                    records.Add(transaction.EntryDetail);
                    records.AddRange(transaction.AddendaRecords);
                }
                records.Add(batch.BatchControl);
            }
            records.Add(achFile.FileControl);

            var expectedCount = 2 // file header + file control
                + achFile.BatchRecordList.Count * 2 // each batch header + batch control
                + achFile.BatchRecordList.SelectMany(b => b.TransactionRecords).Count() // entry detail records
                + achFile.BatchRecordList.SelectMany(b => b.TransactionRecords).SelectMany(t => t.AddendaRecords).Count();

            records.Count.Should().Be(expectedCount);
        }
    }
}
