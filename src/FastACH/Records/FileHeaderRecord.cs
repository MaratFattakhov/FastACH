using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// File Header Record (1 record)
    /// </summary>
    public record class FileHeaderRecord : IRecord
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
        internal FileHeaderRecord(ReadOnlySpan<char> data)
        {
            if (data.Length != 94)
            {
                throw new ArgumentException($"Invalid File Header (1 record) length: Expected 94, Actual {data.Length}");
            }

            ImmediateDestination = data.Slice(3, 10).Trim().ToString();
            ImmediateOrigin = data.Slice(13, 10).Trim().ToString();
            FileCreationDate = DateOnly.ParseExact(data.Slice(23, 6), "yyMMdd");
            FileCreationTime = TimeOnly.TryParseExact(data.Slice(29, 4), "HHmm", out var time) ? time : null;
            FileIdModifier = data.Slice(33, 1).Trim()[0];
            ImmediateDestinationName = data.Slice(40, 23).Trim().ToString();
            ImmediateOriginName = data.Slice(63, 23).Trim().ToString();
            ReferenceCode = data.Slice(86, 8).Trim().ToString();
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
