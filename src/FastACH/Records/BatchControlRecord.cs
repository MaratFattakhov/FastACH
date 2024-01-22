namespace FastACH.Records
{
    public class BatchControlRecord : IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "8";

        // Position 2-4: Service Class Code (numeric)
        public uint ServiceClassCode { get; set; }

        // Position 5-10: Entry/Addenda Count (numeric)
        public uint EntryAddendaCount { get; set; }

        // Position 11-20: Entry Hash (numeric)
        public ulong EntryHash { get; set; }

        // Position 21-32: Total Debit Entry Dollar Amount (numeric)
        public decimal TotalDebitEntryDollarAmount { get; set; }

        // Position 33-44: Total Credit Entry Dollar Amount (numeric)
        public decimal TotalCreditEntryDollarAmount { get; set; }

        // Position 45-54: Company Identification (alpha-numeric)
        public string CompanyIdentification { get; set; }

        // Position 55-73: Message Authentication Code (alpha-numeric)
        public string MessageAuthenticationCode { get; set; } = string.Empty;

        // Position 74-79: Reserved (blank space)
        public string Reserved => "      ";

        // Position 80-87: Originating DFI Identification Number (numeric)
        public string OriginatingDFINumber { get; set; }

        // Position 88-94: Batch Number (numeric)
        public ulong BatchNumber { get; set; }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(ServiceClassCode, 3);
            writer.Write(EntryAddendaCount, 6);
            writer.Write(EntryHash % 10000000000, 10);
            writer.Write((ulong)Math.Round(TotalDebitEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write((ulong)Math.Round(TotalCreditEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write(CompanyIdentification, 10);
            writer.Write(MessageAuthenticationCode, 19);
            writer.Write(Reserved);
            writer.Write(OriginatingDFINumber, 8);
            writer.Write(BatchNumber, 7);
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Batch Control Record Header (8 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ServiceClassCode = uint.Parse(data.Substring(1, 3));
            EntryAddendaCount = uint.Parse(data.Substring(4, 6));
            EntryHash = uint.Parse(data.Substring(10, 10));
            TotalDebitEntryDollarAmount = decimal.Parse(data.Substring(20, 12)) / 100;
            TotalCreditEntryDollarAmount = decimal.Parse(data.Substring(32, 12)) / 100;
            CompanyIdentification = data.Substring(44, 10).Trim();
            MessageAuthenticationCode = data.Substring(54, 19).Trim();
            OriginatingDFINumber = data.Substring(79, 8).Trim();
            BatchNumber = ulong.Parse(data.Substring(87, 7));
        }
    }
}
