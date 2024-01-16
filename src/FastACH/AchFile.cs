using FastACH.Models;

namespace FastACH
{
    public class AchFile
    {
        private bool _shouldRecalculate = true;
        private ulong _fileLineCount = 0;

        public AchFile()
        {
        }

        public AchFile(bool shouldRecalculate)
        {
            _shouldRecalculate = shouldRecalculate;
        }

        public OneRecord OneRecord = new();
        public List<FiveRecord> BatchRecordList = new();
        public NineRecord NineRecord = new();

        public void SaveFileToDisk(string filePath)
        {
            // Create a memory stream to store the lines
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create a StreamWriter to write to the memory stream
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    BuildFileContents(streamWriter);

                    // Flush the writer to ensure content is in the memory stream
                    streamWriter.Flush();

                    // Create a FileStream to write the memory stream content to a file
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        // Get the content of the memory stream as a byte array
                        byte[] memoryStreamContent = memoryStream.ToArray();

                        // Write the content to the file stream
                        fileStream.Write(memoryStreamContent, 0, memoryStreamContent.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Build ACH file, calculating 8 and 9 records accordingly (totals, amounts, hash). 
        /// Also indexes each record in the file, based on current counts, etc.
        /// </summary>
        /// <param name="streamWriter"></param>
        private void BuildFileContents(StreamWriter streamWriter)
        {
            var writer = new StringWriter(streamWriter);

            WriteToStream(streamWriter, OneRecord);

            for (int i = 0; i < BatchRecordList.Count; i++)
            {
                if (_shouldRecalculate)
                    BatchRecordList[i].RecalculateTotals((ulong)i + 1);

                WriteToStream(streamWriter, BatchRecordList[i]);

                foreach (var batchDetailRecord in BatchRecordList[i].SixRecordList)
                {
                    WriteToStream(streamWriter, batchDetailRecord);

                    if (batchDetailRecord.AddendaRecord != null)
                    {
                        WriteToStream(streamWriter, batchDetailRecord.AddendaRecord);
                    }
                }

                WriteToStream(streamWriter, BatchRecordList[i].EightRecord);
            }

            // need to increment line count before the recalc to ensure the block count is correct
            _fileLineCount++;
            if (_shouldRecalculate)
                RecalculateFileTotals();

            WriteToStream(streamWriter, NineRecord, false);

            // write extra fillers so block count is even at 10
            for (ulong i = 0; i < NineRecord.BlockCount * 10 - _fileLineCount; i++)
            {
                streamWriter.WriteLine(new string('9', 94));
            }
        }

        private void WriteToStream(StreamWriter streamWriter, IRecord record, bool incrementLineCount = true)
        {
            // we track line nums in order to use the block fillers of 9999's at EOF
            if (incrementLineCount)
                _fileLineCount++;

            var stringWriter = new StringWriter(streamWriter);
            record.Write(stringWriter);
            streamWriter.WriteLine();
        }

        /// <summary>
        /// Recalculates Nine record totals, usually used as you want to write the file somewhere.
        /// </summary>
        private void RecalculateFileTotals()
        {
            NineRecord.BatchCount = (uint)BatchRecordList.Count;
            NineRecord.BlockCount = (uint)Math.Ceiling(_fileLineCount / 10.0);
            NineRecord.EntryAddendaCount = (uint)BatchRecordList.Select(x => x.SixRecordList.Count()).Sum();
            NineRecord.EntryHash = BatchRecordList
                .Select(p => p.EightRecord.EntryHash)
                .Aggregate((ulong)0, (a, b) => a + b);
            NineRecord.TotalCreditEntryDollarAmount = BatchRecordList
                .SelectMany(x => x.SixRecordList.Where(y => TransactionCodes.IsCredit(y.TransactionCode)).Select(x => x.Amount)).Sum();
            NineRecord.TotalDebitEntryDollarAmount = BatchRecordList
                .SelectMany(x => x.SixRecordList.Where(y => TransactionCodes.IsDebit(y.TransactionCode)).Select(x => x.Amount)).Sum();
        }

        /// <summary>
        /// Outputs the data to the console for debugging (color coded outfor for easy reading)
        /// </summary>
        public void OutputFileToConsole()
        {
            _fileLineCount++;

            OneRecord.Write(ConsoleWriter.CreateForRecord(OneRecord));
            Console.WriteLine();

            for (int i = 0; i < BatchRecordList.Count; i++)
            {
                if (_shouldRecalculate)
                    BatchRecordList[i].RecalculateTotals((ulong)i + 1);

                _fileLineCount++;
                BatchRecordList[i].Write(ConsoleWriter.CreateForRecord(BatchRecordList[i]));
                Console.WriteLine();

                foreach (var batchDetailRecord in BatchRecordList[i].SixRecordList)
                {
                    _fileLineCount++;
                    batchDetailRecord.Write(ConsoleWriter.CreateForRecord(batchDetailRecord));
                    Console.WriteLine();

                    if (batchDetailRecord.AddendaRecord != null)
                    {
                        _fileLineCount++;
                        batchDetailRecord.AddendaRecord.Write(ConsoleWriter.CreateForRecord(batchDetailRecord.AddendaRecord));
                        Console.WriteLine();
                    }
                }

                _fileLineCount++;
                BatchRecordList[i].EightRecord.Write(ConsoleWriter.CreateForRecord(BatchRecordList[i].EightRecord));
                Console.WriteLine();
            }

            // need to increment line count before the recalc to ensure the block count is correct
            _fileLineCount++;
            if (_shouldRecalculate)
                RecalculateFileTotals();

            NineRecord.Write(ConsoleWriter.CreateForRecord(NineRecord));

            Console.WriteLine();
            // write extra fillers so block count is even at 10
            for (uint i = 0; i < NineRecord.BlockCount * 10 - _fileLineCount; i++)
            {
                Console.WriteLine(new string('9', 94));
            }
        }
    }
}
