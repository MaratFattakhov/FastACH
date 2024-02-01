using FastACH.Records;

namespace FastACH
{
    public class AchFileReader
    {
        /// <summary>
        /// Breaks file into objects, based on the NACHA file specification.
        /// </summary>
        public async Task<AchFile> Read(string filePath, CancellationToken cancellationToken = default)
        {
            var batchRecordList = new List<BatchRecord>();
            FileHeaderRecord? fileHeaderRecord = null;
            BatchRecord? currentBatch = null;
            uint lineNumber = 0;
            try
            {
                using StreamReader streamReader = new(filePath);
                var content = await streamReader.ReadToEndAsync(cancellationToken);
                using var stringReader = new StringReader(content);
                string? line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    lineNumber++;
                    switch (line.AsSpan().Slice(0, 1))
                    {
                        case "1":
                            FileHeaderRecord oneRecord = new(line);
                            fileHeaderRecord = oneRecord;
                            break;

                        case "5":
                            BatchHeaderRecord fiveRecord = new(line);
                            currentBatch = new BatchRecord() { BatchHeader = fiveRecord };
                            break;

                        case "6":
                            EntryDetailRecord sixRecord = new(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            var transactionDetails = new TransactionRecord() { EntryDetail = sixRecord, Addenda = null };
                            currentBatch.TransactionRecords.Add(transactionDetails);
                            break;

                        case "7":
                            AddendaRecord sevenRecord = new(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.TransactionRecords.Last().Addenda = sevenRecord;
                            break;

                        case "8":
                            BatchControlRecord eightRecord = new(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.BatchControl = eightRecord;
                            batchRecordList.Add(currentBatch);
                            break;

                        case "9":
                            FileControlRecord nineRecord = new(line);
                            return new AchFile()
                            {
                                BatchRecordList = batchRecordList,
                                FileControl = nineRecord,
                                FileHeader = fileHeaderRecord ?? throw new InvalidOperationException("Missing File Header Record."),
                            };

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AchFileReadingException(lineNumber, ex);
            }

            throw new InvalidOperationException("ACH File doesn't contain termination file control (9) record.");
        }
    }
}
