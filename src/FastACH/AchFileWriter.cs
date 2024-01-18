namespace FastACH
{
    public class AchFileWriter
    {
        private readonly Func<ulong>? _batchNumberGenerator;
        private readonly Func<string>? _traceNumberGenerator;
        private readonly uint _batchSize = 10;

        public AchFileWriter()
        {
            
        }

        public AchFileWriter(
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator,
            uint batchSize) : this()
        {
            _batchNumberGenerator = batchNumberGenerator;
            _traceNumberGenerator = traceNumberGenerator;
            _batchSize = batchSize;
        }

        public Task WriteToFile(AchFile achFile, string filePath)
        {
            RecalculateTotals(achFile);
            using var stream = new FileStream(filePath, FileMode.Create);
            using var streamWriter = new StreamWriter(stream);
            var writer = new StringWriter(streamWriter);
            WriteToStream(streamWriter, achFile, _ => writer);
            return Task.CompletedTask;
        }

        public void WriteToConsole(AchFile achFile)
        {
            RecalculateTotals(achFile);
            WriteToStream(Console.Out, achFile, ConsoleWriter.CreateForRecord);
        }

        public void WriteToStream(
            TextWriter writer, 
            AchFile achFile, 
            Func<IRecord, ILineWriter> getLineWriter)
        {
            var lineNumber = 0;
            WriteToStream(writer, achFile.OneRecord, getLineWriter, ref lineNumber);

            foreach (var batchRecord in achFile.BatchRecordList)
            {
                WriteToStream(writer, batchRecord, getLineWriter, ref lineNumber);

                foreach (var batchDetailRecord in batchRecord.SixRecordList)
                {
                    WriteToStream(writer, batchDetailRecord, getLineWriter, ref lineNumber);

                    if (batchDetailRecord.AddendaRecord != null)
                    {
                        WriteToStream(writer, batchDetailRecord.AddendaRecord, getLineWriter, ref lineNumber);
                    }
                }

                WriteToStream(writer, batchRecord.EightRecord, getLineWriter, ref lineNumber);
            }

            WriteToStream(writer, achFile.NineRecord, getLineWriter, ref lineNumber);

            // write extra fillers so block count is even at batch size, default=10
            for (long i = 0; i < achFile.NineRecord.BlockCount * _batchSize - lineNumber; i++)
            {
                writer.WriteLine(new string('9', 94));
            }
        }

        private void WriteToStream(TextWriter writer, IRecord record, Func<IRecord, ILineWriter> getLineWriter, ref int lineNumber)
        {
            lineNumber++;
            var lineWriter = getLineWriter(record);
            record.Write(lineWriter);
            writer.WriteLine();
        }

        private void RecalculateTotals(AchFile achFile)
        {
            achFile.RecalculateTotals(_batchNumberGenerator ?? GetDefaultBatchNumberGenerator(), _traceNumberGenerator ?? GetDefaultTraceNumberGenerator(achFile));
        }

        private Func<string> GetDefaultTraceNumberGenerator(AchFile achFile)
        {
            ulong traceNumber = 0;
            return new Func<string>(
                () => achFile.OneRecord.ImmediateDestination.PadLeft(8, ' ').Substring(0, 8) + (++traceNumber).ToString().PadLeft(7, '0'));
        }

        private Func<ulong> GetDefaultBatchNumberGenerator()
        {
            ulong batchNumber = 0;
            return new Func<ulong>(
                               () => ++batchNumber);
        }
    }
}
