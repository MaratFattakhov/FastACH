using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// File Control Record (9 record)
    /// </summary>
    public record class FileControlRecord : IRecord
    {
        /// <summary>
        /// Position 1-1: Record Type Code (numeric)
        /// </summary>
        public string RecordTypeCode => "9";

        /// <summary>
        /// Position 2-7: Batch Count (numeric)
        /// </summary>
        public required uint BatchCount { get; set; } = 0;

        /// <summary>
        /// Position 8-13: Block Count (numeric)
        /// </summary>
        public required uint BlockCount { get; set; } = 0;

        /// <summary>
        /// Position 14-21: Entry/Addenda Count (numeric)
        /// </summary>
        public required uint EntryAddendaCount { get; set; } = 0;

        /// <summary>
        /// Position 22-31: Entry Hash (numeric)
        /// </summary>
        public required ulong EntryHash { get; set; } = 0;

        /// <summary>
        /// Position 32-43: Total Debit Entry Dollar Amount (numeric)
        /// </summary>
        public required decimal TotalDebitEntryDollarAmount { get; set; } = 0;

        /// <summary>
        /// Position 44-55: Total Credit Entry Dollar Amount (numeric)
        /// </summary>
        public required decimal TotalCreditEntryDollarAmount { get; set; } = 0;

        /// <summary>
        /// Position 56-94: Reserved (blank space)
        /// </summary>
        public string Reserved => new string(' ', 39);

        public uint LineNumber { get; internal set; }

        internal FileControlRecord()
        {
        }

        public static FileControlRecord Empty
        {
            get
            {
                return new FileControlRecord() { BatchCount = 0, BlockCount = 0, EntryAddendaCount = 0, EntryHash = 0, TotalDebitEntryDollarAmount = 0, TotalCreditEntryDollarAmount = 0 };
            }
        }

        [SetsRequiredMembers]
        internal FileControlRecord(ReadOnlySpan<char> data)
        {
            var reader = new LineReader(data, 1);
            BatchCount = reader.ReadUInt(6);
            BlockCount = reader.ReadUInt(6);
            EntryAddendaCount = reader.ReadUInt(8);
            EntryHash = reader.ReadULong(10);
            TotalDebitEntryDollarAmount = reader.ReadDecimal(12) / 100;
            TotalCreditEntryDollarAmount = reader.ReadDecimal(12) / 100;
        }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode, 1);
            writer.Write(BatchCount, 6);
            writer.Write(BlockCount, 6);
            writer.Write(EntryAddendaCount, 8);
            writer.Write(EntryHash % 10000000000, 10);
            writer.Write((ulong)Math.Round(TotalDebitEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write((ulong)Math.Round(TotalCreditEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write(Reserved, 39);
        }
    }
}
