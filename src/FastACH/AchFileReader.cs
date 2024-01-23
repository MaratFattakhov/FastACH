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
            var achFile = new AchFile();
            BatchRecord? currentBatch = null;
            uint lineNumber = 0;
            try
            {
                using StreamReader reader = new(filePath);
                string? line;
                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    lineNumber++;
                    switch (line.Substring(0, 1))
                    {
                        case "1":
                            FileHeaderRecord oneRecord = new();
                            oneRecord.ParseRecord(line);
                            achFile.OneRecord = oneRecord;
                            break;

                        case "5":
                            BatchHeaderRecord fiveRecord = new();
                            fiveRecord.ParseRecord(line);
                            currentBatch = new BatchRecord() { BatchHeader = fiveRecord };
                            break;

                        case "6":
                            EntryDetailRecord sixRecord = new();
                            sixRecord.ParseRecord(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            var transactionDetails = new TransactionDetails() { EntryDetail = sixRecord, Addenda = null };
                            currentBatch.TransactionDetailsList.Add(transactionDetails);
                            break;

                        case "7":
                            AddendaRecord sevenRecord = new();
                            sevenRecord.ParseRecord(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.TransactionDetailsList.Last().Addenda = sevenRecord;
                            break;

                        case "8":
                            BatchControlRecord eightRecord = new();
                            eightRecord.ParseRecord(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.BatchControl = eightRecord;
                            achFile.BatchRecordList.Add(currentBatch);
                            break;

                        case "9":
                            if (!line.StartsWith("999999999999999999999999"))
                            {
                                FileControlRecord nineRecord = new();
                                nineRecord.ParseRecord(line);
                                achFile.NineRecord = nineRecord;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AchFileReadingException(lineNumber, ex);
            }

            return achFile;
        }
    }
}
