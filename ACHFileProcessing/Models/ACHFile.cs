using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ACH_Transform.ACHFileProcessor.Models
{
    public class ACHFile
    {
        private bool _shouldRecalculate = true;
        private int _fileLineCount = 0;

        public ACHFile()
        {
        }

        public ACHFile(bool shouldRecalculate)
        {
            _shouldRecalculate = shouldRecalculate;
        }

        public ACHRecordType1 OneRecord = new();
        public List<ACHRecordType5> BatchRecordList = new();
        public ACHRecordType9 NineRecord = new();

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
            WriteToStream(streamWriter, OneRecord.WriteAsText());

            for (int i = 0; i < BatchRecordList.Count; i++)
            {
                if (_shouldRecalculate)
                    BatchRecordList[i].RecalculateTotals(i + 1);

                WriteToStream(streamWriter, BatchRecordList[i].WriteAsText());

                foreach (var batchDetailRecord in BatchRecordList[i].SixRecordList)
                {
                    WriteToStream(streamWriter, batchDetailRecord.WriteAsText());

                    if (batchDetailRecord.AddendaRecord != null)
                    {
                        WriteToStream(streamWriter, batchDetailRecord.AddendaRecord.WriteAsText());
                    }
                }

                WriteToStream(streamWriter, BatchRecordList[i].EightRecord.WriteAsText());
            }

            // need to increment line count before the recalc to ensure the block count is correct
            _fileLineCount++;
            if (_shouldRecalculate)
                RecalculateFileTotals();

            WriteToStream(streamWriter, NineRecord.WriteAsText(), false);

            // write extra fillers so block count is even at 10
            for (int i = 0; i < ((NineRecord.BlockCount * 10) - _fileLineCount); i++)
            {
                WriteToStream(streamWriter, new string('9', 94), false);
            }
        }

        private void WriteToStream(StreamWriter streamWriter, string text, bool incrementLineCount = true)
        {
            // we track line nums in order to use the block fillers of 9999's at EOF
            if (incrementLineCount)
                _fileLineCount++;
            
            streamWriter.WriteLine(text);
        }

        /// <summary>
        /// Recalculates Nine record totals, usually used as you want to write the file somewhere.
        /// </summary>
        private void RecalculateFileTotals()
        {
            NineRecord.BatchCount = BatchRecordList.Count;
            NineRecord.BlockCount = (int)Math.Ceiling(_fileLineCount / 10.0);
            NineRecord.EntryAddendaCount = BatchRecordList.Select(x => x.SixRecordList.Count()).Sum();
            NineRecord.EntryHash = BatchRecordList.Select(x => x.EightRecord.EntryHash).Sum();
            NineRecord.TotalCreditEntryDollarAmount = BatchRecordList.Select(x => x.SixRecordList.Where(y => DataFormatHelper.CreditCodes.Contains(y.TransactionCode)).Select(x => x.Amount).Sum()).Sum();
            NineRecord.TotalDebitEntryDollarAmount = BatchRecordList.Select(x => x.SixRecordList.Where(y => DataFormatHelper.DebitCodes.Contains(y.TransactionCode)).Select(x => x.Amount).Sum()).Sum();
        }

        public void SaveFileToS3(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Outputs the data to the console for debugging (color coded outfor for easy reading)
        /// </summary>
        public void OutputFileToConsole()
        {
            _fileLineCount++;
            OneRecord.WriteToConsole();

            for (int i = 0; i < BatchRecordList.Count; i++)
            {
                if (_shouldRecalculate)
                    BatchRecordList[i].RecalculateTotals(i + 1);

                _fileLineCount++;
                BatchRecordList[i].WriteToConsole();

                foreach (var batchDetailRecord in BatchRecordList[i].SixRecordList)
                {
                    _fileLineCount++;
                    batchDetailRecord.WriteToConsole();

                    if (batchDetailRecord.AddendaRecord != null)
                    {
                        _fileLineCount++;
                        batchDetailRecord.AddendaRecord.WriteToConsole();
                    }
                }

                _fileLineCount++;
                BatchRecordList[i].EightRecord.WriteToConsole();
            }

            // need to increment line count before the recalc to ensure the block count is correct
            _fileLineCount++;
            if (_shouldRecalculate)
                RecalculateFileTotals();

            NineRecord.WriteToConsole();

            // write extra fillers so block count is even at 10
            for (int i = 0; i < ((NineRecord.BlockCount * 10) - _fileLineCount); i++)
            {
               new string('9', 94);
            }
        }
    }
}
