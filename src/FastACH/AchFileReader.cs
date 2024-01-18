﻿using FastACH.Models;

namespace FastACH
{
    public class AchFileReader
    {
        public AchFileReader()
        {
        }

        /// <summary>
        /// Breaks file into objects, based on the NACHA file specification.
        /// </summary>
        public async Task<AchFile> Read(string filePath)
        {
            var achFile = new AchFile();
            FiveRecord currentBatch = new();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    switch (line.Substring(0, 1))
                    {
                        case "1":
                            OneRecord oneRecord= new();
                            oneRecord.ParseRecord(line);
                            achFile.OneRecord = oneRecord;
                            break;

                        case "5":
                            FiveRecord fiveRecord= new();
                            fiveRecord.ParseRecord(line);
                            currentBatch = fiveRecord;
                            break;

                        case "6":
                            SixRecord sixRecord= new();
                            sixRecord.ParseRecord(line);
                            currentBatch.SixRecordList.Add(sixRecord);
                            break;

                        case "7":
                            SevenRecord sevenRecord= new();
                            sevenRecord.ParseRecord(line);
                            currentBatch.SixRecordList.Last().AddendaRecord = sevenRecord;
                            break;

                        case "8":
                            EightRecord eightRecord= new();
                            eightRecord.ParseRecord(line);
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