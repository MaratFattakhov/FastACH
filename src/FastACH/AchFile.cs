using FastACH.Records;
using System.Text;

namespace FastACH
{
    public class AchFile
    {
        public AchFile()
        {
        }

        public required FileHeaderRecord FileHeader { get; set; }
        public List<BatchRecord> BatchRecordList { get; set; } = new List<BatchRecord>();
        public FileControlRecord FileControl { get; set; } = FileControlRecord.Empty;

        /// <summary>
        /// Recalculates control records, usually used as you want to write the file somewhere.
        /// </summary>
        public void UpdateControlRecords(WritingOptions options)
        {
            foreach (var batchRecord in BatchRecordList)
            {
                UpdateControlRecords(batchRecord, options);
            }

            var itemsCount = BatchRecordList.SelectMany(x =>
                x.TransactionRecords.Select(p => p.AddendaRecords.Count + 1)).Sum()
                + BatchRecordList.Count * 2 // 2 for batch header and batch control
                + 2; // 2 for file header and file control

            FileControl.BatchCount = (uint)BatchRecordList.Count;
            FileControl.BlockCount = (uint)Math.Ceiling(itemsCount / 10.0);
            FileControl.EntryAddendaCount = (uint)BatchRecordList.Select(x => (int)x.BatchControl.EntryAddendaCount).Sum();
            FileControl.EntryHash = BatchRecordList
                .Select(p => p.BatchControl.EntryHash)
                .Aggregate((ulong)0, (a, b) => a + b);
            FileControl.TotalCreditEntryDollarAmount = BatchRecordList.Sum(p => p.BatchControl.TotalCreditEntryDollarAmount);
            FileControl.TotalDebitEntryDollarAmount = BatchRecordList.Sum(p => p.BatchControl.TotalDebitEntryDollarAmount);
        }

        public Task WriteToFile(string filePath, CancellationToken ct = default) => WriteToFile(filePath, _ => { }, ct);

        public async Task WriteToFile(string filePath, Action<WritingOptions> configure, CancellationToken ct = default)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.ASCII);
            var writer = new StringWriter(streamWriter);
            Write(streamWriter, configure, _ => writer);
            await streamWriter.FlushAsync();
            memoryStream.Position = 0;

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await memoryStream.CopyToAsync(fileStream, ct);
            await fileStream.FlushAsync(ct);
        }

        public Task WriteToStream(Stream stream, CancellationToken ct = default) => WriteToStream(stream, _ => { }, ct);

        public Task WriteToStream(Stream stream, Action<WritingOptions> configure, CancellationToken ct = default)
        {
            using var streamWriter = new StreamWriter(stream, Encoding.ASCII);
            var writer = new StringWriter(streamWriter);
            Write(streamWriter, configure, _ => writer);
            return Task.CompletedTask;
        }

        public void WriteToConsole(Action<WritingOptions>? configure = null)
        {
            Write(Console.Out, configure, ConsoleWriter.CreateForRecord);
        }

        private void Write(
            TextWriter writer,
            Action<WritingOptions>? configure,
            Func<IRecord, ILineWriter> getLineWriter)
        {
            var options = new WritingOptions(this);

            configure?.Invoke(options);

            if (options.UpdateControlRecords)
            {
                UpdateControlRecords(options);
            }

            var lineNumber = 0;
            WriteToStream(writer, FileHeader, getLineWriter, ref lineNumber);

            foreach (var batchRecord in BatchRecordList)
            {
                WriteToStream(writer, batchRecord.BatchHeader, getLineWriter, ref lineNumber);

                foreach (var transactionDetails in batchRecord.TransactionRecords)
                {
                    WriteToStream(writer, transactionDetails.EntryDetail, getLineWriter, ref lineNumber);

                    foreach (var addendaRecord in transactionDetails.AddendaRecords)
                    {
                        WriteToStream(writer, addendaRecord, getLineWriter, ref lineNumber);
                    }
                }

                WriteToStream(writer, batchRecord.BatchControl, getLineWriter, ref lineNumber);
            }

            WriteToStream(writer, FileControl, getLineWriter, ref lineNumber, false);

            // write extra fillers so block count is even at batch size, default=10
            for (long i = lineNumber; i < FileControl.BlockCount * options.BlockingFactor; i++)
            {
                writer.WriteLine();
                writer.Write(new string('9', 94));
            }
        }

        /// <summary>
        /// Breaks file into objects, based on the NACHA file specification.
        /// </summary>
        /// <exception cref="AchFileReadingException">Thrown when the file is not in the correct format.</exception></exception>
        public static async Task<AchFile> Read(string filePath, CancellationToken cancellationToken = default)
        {
            using StreamReader streamReader = new(filePath, Encoding.ASCII);
            return await ReadFromStreamAsync(streamReader, cancellationToken);
        }

        private static async Task<AchFile> ReadFromStreamAsync(StreamReader streamReader, CancellationToken cancellationToken)
        {
            var batchRecordList = new List<BatchRecord>();
            FileHeaderRecord? fileHeaderRecord = null;
            BatchRecord? currentBatch = null;
            uint lineNumber = 0;
            try
            {
                string? line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    lineNumber++;
                    switch (line[0])
                    {
                        case '1':
                            FileHeaderRecord oneRecord = new(line) { LineNumber = lineNumber };
                            fileHeaderRecord = oneRecord;
                            break;
                        case '5':
                            if (currentBatch?.BatchControl == BatchControlRecord.Empty)
                                throw new InvalidOperationException("Batch control (8) record is missing for batch header record.");
                            BatchHeaderRecord fiveRecord = new(line) { LineNumber = lineNumber };
                            currentBatch = new BatchRecord() { BatchHeader = fiveRecord };
                            break;
                        case '6':
                            EntryDetailRecord sixRecord = new(line) { LineNumber = lineNumber };
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch (5) record found for entry record");
                            var transactionDetails = new TransactionRecord() { EntryDetail = sixRecord };
                            currentBatch.TransactionRecords.Add(transactionDetails);
                            break;
                        case '7':
                            AddendaRecord sevenRecord = new(line) { LineNumber = lineNumber };
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch (5) record found for entry record");
                            currentBatch.TransactionRecords.Last().AddendaRecords.Add(sevenRecord);
                            break;
                        case '8':
                            BatchControlRecord eightRecord = new(line) { LineNumber = lineNumber };
                            if (currentBatch is null)
                                throw new InvalidOperationException("No batch (5) record found for entry record");
                            currentBatch.BatchControl = eightRecord;
                            batchRecordList.Add(currentBatch);
                            break;
                        case '9':
                            if (currentBatch?.BatchControl == BatchControlRecord.Empty)
                                throw new InvalidOperationException("Batch control (8) record is missing for batch header record.");
                            FileControlRecord nineRecord = new(line) { LineNumber = lineNumber };
                            var result = new AchFile()
                            {
                                BatchRecordList = batchRecordList,
                                FileControl = nineRecord,
                                FileHeader = fileHeaderRecord ?? throw new InvalidOperationException("Missing File Header Record."),
                            };
                            return result;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AchFileReadingException(lineNumber, ex);
            }
            // If we reached EOF without encountering a file control (9) record, wrap the error
            // in AchFileReadingException for consistency with earlier error handling.
            throw new AchFileReadingException(lineNumber, new InvalidOperationException("ACH File doesn't contain termination file control (9) record."));
        }

        private void UpdateControlRecords(
            BatchRecord batch,
            WritingOptions options)
        {
            UpdateBatchNumbers(batch, options);
            UpdateTraceNumbers(batch, options);
            batch.BatchControl = Create(batch.BatchHeader, batch.TransactionRecords);
        }

        private BatchControlRecord Create(BatchHeaderRecord batchHeader, List<TransactionRecord> transactionDetails)
        {
            return new BatchControlRecord()
            {
                BatchNumber = batchHeader.BatchNumber,
                ServiceClassCode = batchHeader.ServiceClassCode,
                EntryAddendaCount = (uint)transactionDetails.Count + (uint)transactionDetails.Sum(x => x.AddendaRecords.Count),
                EntryHash = transactionDetails
                    .Select(p => p.EntryDetail.ReceivingDFIID)
                    .Aggregate((ulong)0, (a, b) => a + b),
                TotalCreditEntryDollarAmount = transactionDetails.Where(x => TransactionCodes.IsCredit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero)),
                TotalDebitEntryDollarAmount = transactionDetails.Where(x => TransactionCodes.IsDebit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero)),
                CompanyIdentification = batchHeader.CompanyId,
                OriginatingDFINumber = batchHeader.OriginatingDFIID,
            };
        }

        private void UpdateBatchNumbers(BatchRecord batch, WritingOptions options)
        {
            var batchNumber = options.GetNextBatchNumber();
            batch.BatchHeader.BatchNumber = batchNumber;
        }

        private void UpdateTraceNumbers(BatchRecord batch, WritingOptions options)
        {
            foreach (var transactionDetails in batch.TransactionRecords)
            {
                var traceNumber = options.GetNextTraceNumber();
                transactionDetails.EntryDetail.TraceNumber = traceNumber;

                if (!transactionDetails.EntryDetail.AddendaRecordIndicator) continue;

                var getNextAddendaSequenceNumber = options.GetAddendaSequenceNumberGenerator();

                foreach (var addendaRecord in transactionDetails.AddendaRecords)
                {
                    addendaRecord.AddendaSequenceNumber = getNextAddendaSequenceNumber();
                    addendaRecord.EntryDetailSequenceNumber = ulong.Parse(
                        traceNumber.Substring(traceNumber.Length - 7, 7));
                }
            }
        }

        private static void WriteToStream(
            TextWriter writer,
            IRecord record,
            Func<IRecord, ILineWriter> getLineWriter,
            ref int lineNumber,
            bool newLine = true)
        {
            lineNumber++;
            switch (record)
            {
                case FileHeaderRecord r: r.LineNumber = (uint)lineNumber; break;
                case BatchHeaderRecord r: r.LineNumber = (uint)lineNumber; break;
                case EntryDetailRecord r: r.LineNumber = (uint)lineNumber; break;
                case AddendaRecord r: r.LineNumber = (uint)lineNumber; break;
                case BatchControlRecord r: r.LineNumber = (uint)lineNumber; break;
                case FileControlRecord r: r.LineNumber = (uint)lineNumber; break;
            }

            var lineWriter = getLineWriter(record);
            record.Write(lineWriter);

            if (newLine) writer.WriteLine();
        }
    }
}
