using FastACH.Records;

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
                x.TransactionRecords.Select(p => p.Addenda is not null ? 2 : 1)).Sum()
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

        public Task WriteToFile(string filePath, CancellationToken ct = default)
        {
            return WriteToFile(filePath, _ => { }, ct);
        }

        public async Task WriteToFile(string filePath, Action<WritingOptions> configure, CancellationToken ct = default)
        {
            var options = new WritingOptions(this);

            configure(options);

            if (options.UpdateControlRecords)
            {
                UpdateControlRecords(options);
            }

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            var writer = new StringWriter(streamWriter);
            WriteToStream(streamWriter, this, options.BlockingFactor, _ => writer);

            await streamWriter.FlushAsync();
            memoryStream.Position = 0;
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await memoryStream.CopyToAsync(fileStream, ct);
            await fileStream.FlushAsync(ct);
        }

        public void WriteToConsole(Action<WritingOptions>? configure = null)
        {
            var options = new WritingOptions(this);

            configure?.Invoke(options);

            if (options.UpdateControlRecords)
            {
                UpdateControlRecords(options);
            }

            WriteToStream(Console.Out, this, options.BlockingFactor, ConsoleWriter.CreateForRecord);
        }

        public static void WriteToStream(
            TextWriter writer,
            AchFile achFile,
            uint blockingFactor,
            Func<IRecord, ILineWriter> getLineWriter)
        {
            var lineNumber = 0;
            WriteToStream(writer, achFile.FileHeader, getLineWriter, ref lineNumber);

            foreach (var batchRecord in achFile.BatchRecordList)
            {
                WriteToStream(writer, batchRecord.BatchHeader, getLineWriter, ref lineNumber);

                foreach (var transactionDetails in batchRecord.TransactionRecords)
                {
                    WriteToStream(writer, transactionDetails.EntryDetail, getLineWriter, ref lineNumber);

                    if (transactionDetails.Addenda != null)
                    {
                        WriteToStream(writer, transactionDetails.Addenda, getLineWriter, ref lineNumber);
                    }
                }

                WriteToStream(writer, batchRecord.BatchControl, getLineWriter, ref lineNumber);
            }

            WriteToStream(writer, achFile.FileControl, getLineWriter, ref lineNumber, false);

            // write extra fillers so block count is even at batch size, default=10
            for (long i = lineNumber; i < achFile.FileControl.BlockCount * blockingFactor; i++)
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
            using StreamReader streamReader = new(filePath);
            var content = await streamReader.ReadToEndAsync(cancellationToken);
            return ReadFromContent(content.AsSpan());
        }

        private static void CheckForInvalidCharOrTab(string line)
        {
            if (line.ToString().Contains("\t"))
                throw new InvalidOperationException("File contains tabs");

            foreach (var chtr in line)  //chtr stands for character
            {
                if (chtr >= 128)
                    throw new InvalidOperationException("File contains invalid characters");
            }
        }

        private static AchFile ReadFromContent(ReadOnlySpan<char> content)
        {
            var batchRecordList = new List<BatchRecord>();
            FileHeaderRecord? fileHeaderRecord = null;
            BatchRecord? currentBatch = null;
            uint lineNumber = 0;
            try
            {
                foreach (var line in content.EnumerateLines())
                {
                    CheckForInvalidCharOrTab(line.ToString());

                    lineNumber++;
                    switch (line.Slice(0, 1))
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
                EntryAddendaCount = (uint)transactionDetails.Count + (uint)transactionDetails.Where(x => x.Addenda != null).Count(),
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

                if (transactionDetails.EntryDetail.AddendaRecordIndicator && transactionDetails.Addenda != null)
                {
                    transactionDetails.Addenda.EntryDetailSequenceNumber = ulong.Parse(
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
            var lineWriter = getLineWriter(record);
            record.Write(lineWriter);

            if (newLine) writer.WriteLine();
        }
    }
}
