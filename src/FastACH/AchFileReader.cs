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
            FiveRecord? currentBatch = null;

            using (StreamReader reader = new(filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    switch (line.Substring(0, 1))
                    {
                        case "1":
                            OneRecord oneRecord = new();
                            oneRecord.ParseRecord(line);
                            achFile.OneRecord = oneRecord;
                            break;

                        case "5":
                            FiveRecord fiveRecord = new();
                            fiveRecord.ParseRecord(line);
                            currentBatch = fiveRecord;
                            break;

                        case "6":
                            SixRecord sixRecord = new();
                            sixRecord.ParseRecord(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.SixRecordList.Add(sixRecord);
                            break;

                        case "7":
                            SevenRecord sevenRecord = new();
                            sevenRecord.ParseRecord(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.SixRecordList.Last().AddendaRecord = sevenRecord;
                            break;

                        case "8":
                            EightRecord eightRecord = new();
                            eightRecord.ParseRecord(line);
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch record found for entry record");
                            currentBatch.EightRecord = eightRecord;
                            achFile.BatchRecordList.Add(currentBatch);
                            currentBatch = new();
                            break;

                        case "9":
                            if (!line.StartsWith("999999999999999999999999"))
                            {
                                NineRecord nineRecord = new();
                                nineRecord.ParseRecord(line);
                                achFile.NineRecord = nineRecord;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return achFile;
        }
    }
}
