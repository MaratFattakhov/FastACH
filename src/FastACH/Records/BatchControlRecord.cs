using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// Batch Control Record (8 record)
    /// </summary>
    public record class BatchControlRecord : IRecord
    {
        /// <summary>
        /// Position 1-1: Record Type Code (numeric)
        /// </summary>
        public string RecordTypeCode => "8";

        /// <summary>
        /// Position 2-4: Service Class Code (numeric)
        /// </summary>
        public required uint ServiceClassCode { get; set; }

        /// <summary>
        /// Position 5-10: Entry/Addenda Count (numeric)
        /// </summary>
        public required ulong EntryAddendaCount { get; set; }

        /// <summary>
        /// Position 11-20: Entry Hash (numeric)
        /// </summary>
        public required ulong EntryHash { get; set; }

        /// <summary>
        /// Position 21-32: Total Debit Entry Dollar Amount (numeric)
        /// </summary>
        public required decimal TotalDebitEntryDollarAmount { get; set; }

        /// <summary>
        /// Position 33-44: Total Credit Entry Dollar Amount (numeric)
        /// </summary>
        public required decimal TotalCreditEntryDollarAmount { get; set; }

        /// <summary>
        /// Position 45-54: Company Identification (alpha-numeric)
        /// </summary>
        public required string CompanyIdentification { get; set; }

        /// <summary>
        /// Position 55-73: Message Authentication Code (alpha-numeric)
        /// </summary>
        public string MessageAuthenticationCode { get; set; } = string.Empty;

        /// <summary>
        /// Position 74-79: Reserved (blank space)
        /// </summary>
        public string Reserved => "      ";

        /// <summary>
        /// Position 80-87: Originating DFI Identification Number (numeric)
        /// </summary>
        public required string OriginatingDFINumber { get; set; }

        /// <summary>
        /// Position 88-94: Batch Number (numeric)
        /// </summary>
        public required ulong BatchNumber { get; set; }

        public uint LineNumber { get; internal set; }

        internal BatchControlRecord()
        {
        }

        public static BatchControlRecord Empty
        {
            get
            {
                return new BatchControlRecord() { ServiceClassCode = 0, EntryAddendaCount = 0, EntryHash = 0, TotalDebitEntryDollarAmount = 0, TotalCreditEntryDollarAmount = 0, CompanyIdentification = string.Empty, OriginatingDFINumber = string.Empty, BatchNumber = 0 };
            }
        }

        [SetsRequiredMembers]
        internal BatchControlRecord(ReadOnlySpan<char> data)
        {
            var reader = new LineReader(data, 1);
            ServiceClassCode = reader.ReadUInt(3);
            EntryAddendaCount = reader.ReadULong(6);
            EntryHash = reader.ReadULong(10);
            TotalDebitEntryDollarAmount = reader.ReadDecimal(12) / 100;
            TotalCreditEntryDollarAmount = reader.ReadDecimal(12) / 100;
            CompanyIdentification = reader.ReadString(10);
            MessageAuthenticationCode = reader.ReadString(19);
            reader.Skip(6);
            OriginatingDFINumber = reader.ReadString(8);
            BatchNumber = reader.ReadULong(7);
        }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode, 1);
            writer.Write(ServiceClassCode, 3);
            writer.Write(EntryAddendaCount, 6);
            writer.Write(EntryHash % 10000000000, 10);
            writer.Write((ulong)Math.Round(TotalDebitEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write((ulong)Math.Round(TotalCreditEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write(CompanyIdentification, 10);
            writer.Write(MessageAuthenticationCode, 19);
            writer.Write(Reserved, 6);
            writer.Write(OriginatingDFINumber, 8);
            writer.Write(BatchNumber, 7);
        }
    }
}
