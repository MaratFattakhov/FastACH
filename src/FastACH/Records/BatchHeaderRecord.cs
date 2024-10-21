using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// Batch Header Record (5 record)
    /// </summary>
    public record class BatchHeaderRecord : BaseRecord, IRecord
    {
        /// <summary>
        /// Position 1-1: Record Type Code (numeric)
        /// </summary>
        public string RecordTypeCode => "5";

        /// <summary>
        ///     Position 2-4: Service Class Code (numeric)
        ///     200 – ACH Entries Mixed Debits and Credits
        ///     220 – ACH Credits Only
        ///     225 – ACH Debits Only
        /// </summary>
        public required uint ServiceClassCode { get; set; }

        /// <summary>
        /// Position 5-20: Company Name (alpha-numeric)
        /// </summary>
        public required string CompanyName { get; set; }

        /// <summary>
        /// Position 21-40: Company Discretionary Data (alpha-numeric)
        /// </summary>
        public string CompanyDiscretionaryData { get; set; } = string.Empty;

        /// <summary>
        /// Position 41-50: Company Identification (alpha-numeric)
        /// </summary>
        public required string CompanyId { get; set; }

        /// <summary>
        /// Position 51-53: Standard Entry Class Code (alpha-numeric)
        /// </summary>
        public string StandardEntryClassCode { get; set; } = "PPD";

        /// <summary>
        /// Position 54-63: Company Entry Description (alpha-numeric)
        /// </summary>
        public required string CompanyEntryDescription { get; set; }

        /// <summary>
        /// Position 64-69: Company Descriptive Date (numeric, yyMMdd)
        /// </summary>
        public DateOnly? CompanyDescriptiveDate { get; set; }

        /// <summary>
        /// Position 70-75: Effective Entry Date (numeric, yyMMdd)
        /// </summary>
        public DateOnly? EffectiveEntryDate { get; set; }

        /// <summary>
        /// Position 76-78: Julian Settlement Date (numeric), Always blank
        /// </summary>
        public string JulianSettlementDate { get; set; } = "   ";

        /// <summary>
        /// Position 79-79: Originator's Status Code (numeric)
        /// </summary>
        public char OriginatorsStatusCode { get; set; } = '1';

        /// <summary>
        /// Position 80-87: Originator's DFI Identification Number (numeric)
        /// </summary>
        public required string OriginatingDFIID { get; set; }

        /// <summary>
        /// Position 88-94: Batch Number (numeric)
        /// </summary>
        public ulong BatchNumber { get; set; } = 0;

        public BatchHeaderRecord()
        {
        }

        [SetsRequiredMembers]
        internal BatchHeaderRecord(ReadOnlySpan<char> data, uint lineNumber)
        {
            var reader = new LineReader(data, 1);
            ServiceClassCode = reader.ReadUInt(3);
            CompanyName = reader.ReadString(16);
            CompanyDiscretionaryData = reader.ReadString(20);
            CompanyId = reader.ReadString(10);
            StandardEntryClassCode = reader.ReadString(3);
            CompanyEntryDescription = reader.ReadString(10);
            CompanyDescriptiveDate = reader.ReadDate(true);
            EffectiveEntryDate = reader.ReadDate(true);
            JulianSettlementDate = reader.ReadString(3);
            OriginatorsStatusCode = reader.ReadChar();
            OriginatingDFIID = reader.ReadString(8);
            BatchNumber = reader.ReadULong(7);
            LineNumber = lineNumber;
        }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode, 1);
            writer.Write(ServiceClassCode, 3);
            writer.Write(CompanyName, 16);
            writer.Write(CompanyDiscretionaryData, 20);
            writer.Write(CompanyId, 10);
            writer.Write(StandardEntryClassCode, 3);
            writer.Write(CompanyEntryDescription, 10);
            writer.Write(CompanyDescriptiveDate);
            writer.Write(EffectiveEntryDate);
            writer.Write(JulianSettlementDate, 3);
            writer.Write(OriginatorsStatusCode.ToString(), 1);
            writer.Write(OriginatingDFIID, 8);
            writer.Write(BatchNumber, 7);
        }
    }
}
