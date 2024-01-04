using System;

namespace ACH_Transform.ACHFileProcessor.Models
{
    public class ACHRecordType9 : ACHBaseRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode = "9";

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

        public override string WriteAsText() =>
                $"{RecordTypeCode}" +                
                $"{DataFormatHelper.FormatForAch(BatchCount, 6)}" +
                $"{DataFormatHelper.FormatForAch(BlockCount, 6)}" +
                $"{DataFormatHelper.FormatForAch(EntryAddendaCount, 8)}" +
                $"{DataFormatHelper.FormatForAch(EntryHash % 10000000000, 10)}" +
                $"{DataFormatHelper.FormatForAch(TotalDebitEntryDollarAmount, 12)}" +
                $"{DataFormatHelper.FormatForAch(TotalCreditEntryDollarAmount, 12)}" +
                $"{Reserved}";

        public override void WriteToConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RecordTypeCode);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(BatchCount, 6));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(DataFormatHelper.FormatForAch(BlockCount, 6));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DataFormatHelper.FormatForAch(EntryAddendaCount, 8));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DataFormatHelper.FormatForAch(EntryHash, 10));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DataFormatHelper.FormatForAch(TotalDebitEntryDollarAmount, 12));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(TotalCreditEntryDollarAmount, 12));
            Console.WriteLine(String.Empty);
        }
    }
}
