using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// File Header Record (1 record)
    /// </summary>
    public record class FileHeaderRecord : BaseRecord, IRecord
    {
        /// <summary>
        /// Position 1-1: Record Type Code (numeric)
        /// </summary>
        public string RecordTypeCode => "1";

        /// <summary>
        /// Position 2-3: Priority Code (numeric)
        /// </summary>
        public string PriorityCode => "01";

        /// <summary>
        /// Position 4-13: Immediate Destination (numeric)
        /// </summary>
        public required string ImmediateDestination { get; set; }

        /// <summary>
        /// Position 14-23: Immediate Origin (numeric)
        /// </summary>
        public required string ImmediateOrigin { get; set; }

        /// <summary>
        /// Position 24-29: File Creation Date (numeric, yyMMdd)
        /// </summary>
        public DateOnly FileCreationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Position 30-33: File Creation Time (numeric, HHmm)
        /// </summary>
        public TimeOnly? FileCreationTime { get; set; }

        /// <summary>
        /// Position 34-34: File ID Modifier (alpha-numeric)
        /// </summary>
        public char FileIdModifier { get; set; } = 'A';

        /// <summary>
        /// Position 35-37: Record Size (numeric)
        /// </summary>
        public uint RecordSize => 94;

        /// <summary>
        /// Position 38-39: Blocking Factor (numeric)
        /// </summary>
        public uint BlockingFactor => 10;

        /// <summary>
        /// Position 40-40: Format Code (numeric)
        /// </summary>
        public uint FormatCode => 1;

        /// <summary>
        /// Position 41-63: Immediate Destination Name (alpha-numeric)
        /// </summary>
        public string ImmediateDestinationName { get; set; } = string.Empty;

        /// <summary>
        /// Position 64-86: Immediate Origin Name (alpha-numeric)
        /// </summary>
        public string ImmediateOriginName { get; set; } = string.Empty;

        /// <summary>
        /// Position 87-94: Reference Code (alpha-numeric)
        /// </summary>
        public string ReferenceCode { get; set; } = string.Empty;

        public FileHeaderRecord()
        {
        }

        [SetsRequiredMembers]
        internal FileHeaderRecord(ReadOnlySpan<char> data, uint lineNumber)
        {
            var reader = new LineReader(data, 3);
            ImmediateDestination = reader.ReadString(10);
            ImmediateOrigin = reader.ReadString(10);
            FileCreationDate = reader.ReadDate(false)!.Value;
            FileCreationTime = reader.ReadTime();
            FileIdModifier = reader.ReadChar();
            reader.Skip(6);
            ImmediateDestinationName = reader.ReadString(23);
            ImmediateOriginName = reader.ReadString(23);
            ReferenceCode = reader.ReadString(8);
            LineNumber = lineNumber;
        }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode, 1);
            writer.Write(PriorityCode, 2);
            writer.Write(ImmediateDestination.PadLeft(10, ' '), 10);
            writer.Write(ImmediateOrigin.PadLeft(10, ' '), 10);
            writer.Write(FileCreationDate);
            writer.Write(FileCreationTime);
            writer.Write(FileIdModifier.ToString(), 1);
            writer.Write(RecordSize, 3);
            writer.Write(BlockingFactor, 2);
            writer.Write(FormatCode, 1);
            writer.Write(ImmediateDestinationName, 23);
            writer.Write(ImmediateOriginName, 23);
            writer.Write(ReferenceCode, 8);
        }
    }
}
