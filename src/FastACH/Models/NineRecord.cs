
namespace FastACH.Models
{
    public class NineRecord : IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "9";

        // Position 2-7: Batch Count (numeric)
        public int BatchCount { get; set; }

        // Position 8-13: Block Count (numeric)
        public int BlockCount { get; set; }

        // Position 14-21: Entry/Addenda Count (numeric)
        public int EntryAddendaCount { get; set; }

        // Position 22-31: Entry Hash (numeric)
        public long EntryHash { get; set; }

        // Position 32-43: Total Debit Entry Dollar Amount (numeric)
        public decimal TotalDebitEntryDollarAmount { get; set; }

        // Position 44-55: Total Credit Entry Dollar Amount (numeric)
        public decimal TotalCreditEntryDollarAmount { get; set; }

        // Position 56-94: Reserved (blank space)
        public string Reserved => new string(' ', 39);

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(DataFormatHelper.FormatForAch(BatchCount, 6));
            writer.Write(DataFormatHelper.FormatForAch(BlockCount, 6));
            writer.Write(DataFormatHelper.FormatForAch(EntryAddendaCount, 8));
            writer.Write(DataFormatHelper.FormatForAch(EntryHash % 10000000000, 10));
            writer.Write(DataFormatHelper.FormatForAch(TotalDebitEntryDollarAmount, 12));
            writer.Write(DataFormatHelper.FormatForAch(TotalCreditEntryDollarAmount, 12));
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid File Control Record Header (8 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            BatchCount = DataFormatHelper.ParseInt(data.Substring(1, 6).Trim());
            BlockCount = DataFormatHelper.ParseInt(data.Substring(7, 6).Trim());
            EntryAddendaCount = DataFormatHelper.ParseInt(data.Substring(13, 8).Trim());
            EntryHash = DataFormatHelper.ParseInt(data.Substring(21, 10).Trim());
            TotalDebitEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(31, 12).Trim());
            TotalCreditEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(43, 12).Trim());
        }
    }
}
