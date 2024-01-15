
namespace FastACH.Models
{
    public class EightRecord: IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "8";

        // Position 2-4: Service Class Code (numeric)
        public string ServiceClassCode { get; set; }

        // Position 5-10: Entry/Addenda Count (numeric)
        public int EntryAddendaCount { get; set; }

        // Position 11-20: Entry Hash (numeric)
        public long EntryHash { get; set; }

        // Position 21-32: Total Debit Entry Dollar Amount (numeric)
        public decimal TotalDebitEntryDollarAmount { get; set; }

        // Position 33-44: Total Credit Entry Dollar Amount (numeric)
        public decimal TotalCreditEntryDollarAmount { get; set; }

        // Position 45-54: Company Identification (alpha-numeric)
        public string CompanyIdentification { get; set; }

        // Position 55-73: Message Authentication Code (alpha-numeric)
        public string MessageAuthenticationCode { get; set; }

        // Position 74-79: Reserved (blank space)
        public string Reserved => new string(' ', 6);

        // Position 80-87: Originating DFI Identification Number (numeric)
        public string OriginatingDFINumber { get; set; }

        // Position 88-94: Batch Number (alpha-numeric)
        public ulong BatchNumber { get; set; }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(ServiceClassCode.Length > 3 ? ServiceClassCode.Substring(0, 3) : ServiceClassCode.PadLeft(3, '0'));
            writer.Write(DataFormatHelper.FormatForAch(EntryAddendaCount, 6));
            writer.Write(DataFormatHelper.FormatForAch(EntryHash % 10000000000, 10, true));
            writer.Write(DataFormatHelper.FormatForAch(TotalDebitEntryDollarAmount, 12));
            writer.Write(DataFormatHelper.FormatForAch(TotalCreditEntryDollarAmount, 12));
            writer.Write(DataFormatHelper.FormatForAch(CompanyIdentification, 10));
            writer.Write(DataFormatHelper.FormatForAch(MessageAuthenticationCode, 19));
            writer.Write(Reserved);
            writer.Write(DataFormatHelper.FormatForAch(OriginatingDFINumber, 8));
            writer.Write(DataFormatHelper.FormatForAch(BatchNumber, 7));
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Batch Control Record Header (8 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ServiceClassCode = data.Substring(1, 3).Trim();
            EntryAddendaCount = DataFormatHelper.ParseInt(data.Substring(4, 6).Trim());
            EntryHash = DataFormatHelper.ParseInt(data.Substring(10, 10).Trim());
            TotalDebitEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(20, 12).Trim());
            TotalCreditEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(32, 12).Trim());
            CompanyIdentification = data.Substring(44, 10).Trim();
            MessageAuthenticationCode = data.Substring(54, 19).Trim();
            OriginatingDFINumber = data.Substring(79, 8).Trim();
            BatchNumber = DataFormatHelper.ParseUlong(data.Substring(87, 7).Trim());
        }
    }
}
