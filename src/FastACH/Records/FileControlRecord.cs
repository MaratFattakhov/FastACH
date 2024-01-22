namespace FastACH.Records
{
    public class FileControlRecord : IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "9";

        // Position 2-7: Batch Count (numeric)
        public uint BatchCount { get; set; }

        // Position 8-13: Block Count (numeric)
        public uint BlockCount { get; set; }

        // Position 14-21: Entry/Addenda Count (numeric)
        public uint EntryAddendaCount { get; set; }

        // Position 22-31: Entry Hash (numeric)
        public ulong EntryHash { get; set; }

        // Position 32-43: Total Debit Entry Dollar Amount (numeric)
        public decimal TotalDebitEntryDollarAmount { get; set; }

        // Position 44-55: Total Credit Entry Dollar Amount (numeric)
        public decimal TotalCreditEntryDollarAmount { get; set; }

        // Position 56-94: Reserved (blank space)
        public string Reserved => new string(' ', 39);

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(BatchCount, 6);
            writer.Write(BlockCount, 6);
            writer.Write(EntryAddendaCount, 8);
            writer.Write(EntryHash % 10000000000, 10);
            writer.Write((ulong)Math.Round(TotalDebitEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write((ulong)Math.Round(TotalCreditEntryDollarAmount * 100, MidpointRounding.AwayFromZero), 12);
            writer.Write(Reserved);
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid File Control Record Header (8 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            BatchCount = uint.Parse(data.Substring(1, 6));
            BlockCount = uint.Parse(data.Substring(7, 6));
            EntryAddendaCount = uint.Parse(data.Substring(13, 8));
            EntryHash = ulong.Parse(data.Substring(21, 10));
            TotalDebitEntryDollarAmount = decimal.Parse(data.Substring(31, 12)) / 100;
            TotalCreditEntryDollarAmount = decimal.Parse(data.Substring(43, 12)) / 100;
        }
    }
}
