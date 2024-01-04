using System;

namespace ACH_Transform.ACHFileProcessor.Models
{
    public class ACHRecordType8 : ACHBaseRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode = "8";

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
        public int BatchNumber { get; set; }

        public override string WriteAsText() =>
            $"{RecordTypeCode}" +
            $"{(ServiceClassCode.Length > 3 ? ServiceClassCode.Substring(0, 3) : ServiceClassCode.PadLeft(3, '0'))}" +
            $"{DataFormatHelper.FormatForAch(EntryAddendaCount, 6)}" +
            $"{DataFormatHelper.FormatForAch(EntryHash % 10000000000, 10, true)}" +
            $"{DataFormatHelper.FormatForAch(TotalDebitEntryDollarAmount, 12)}" +
            $"{DataFormatHelper.FormatForAch(TotalCreditEntryDollarAmount, 12)}" +
            $"{DataFormatHelper.FormatForAch(CompanyIdentification, 10)}" +
            $"{DataFormatHelper.FormatForAch(MessageAuthenticationCode, 19)}" +
            $"{Reserved}" +
            $"{DataFormatHelper.FormatForAch(OriginatingDFINumber, 8)}" +
            $"{DataFormatHelper.FormatForAch(BatchNumber, 7)}";

        public override void WriteToConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RecordTypeCode);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(ServiceClassCode, 3));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(DataFormatHelper.FormatForAch(EntryAddendaCount, 6));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DataFormatHelper.FormatForAch(EntryHash, 10));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DataFormatHelper.FormatForAch(TotalDebitEntryDollarAmount, 12));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(TotalCreditEntryDollarAmount, 12));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DataFormatHelper.FormatForAch(CompanyIdentification, 10));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(MessageAuthenticationCode, 19));
            Console.Write(Reserved);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DataFormatHelper.FormatForAch(OriginatingDFINumber, 8));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(BatchNumber, 7));
            Console.WriteLine(String.Empty);
        }
    }
}
