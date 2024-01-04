using System;
using System.Transactions;

namespace ACH_Transform.ACHFileProcessor.Models
{
    public class ACHRecordType7 : ACHBaseRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode = "7";

        // Position 2-3: Addenda Type Code (numeric)
        public string AddendaTypeCode => "05";

        // Position 4-83: Addenda Information (alpha-numeric)
        public string AddendaInformation { get; set; }

        // Position 84-87: Addenda Sequence Number (numeric)
        public string AddendaSequenceNumber { get; set; }

        // Position 88-94: Entry Detail Sequence Number (numeric)
        public string EntryDetailSequenceNumber { get; set; }

        public override string WriteAsText() =>
            $"{RecordTypeCode}" +
            $"{AddendaTypeCode}" +
            $"{DataFormatHelper.FormatForAch(AddendaInformation, 80)}" +
            $"{DataFormatHelper.FormatForAch(AddendaSequenceNumber, 4, true)}" +
            $"{DataFormatHelper.FormatForAch(EntryDetailSequenceNumber, 7, true)}";

        public override void WriteToConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RecordTypeCode);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(AddendaTypeCode);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(AddendaInformation, 80));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(AddendaSequenceNumber, 4), true);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DataFormatHelper.FormatForAch(EntryDetailSequenceNumber, 7, true));
            Console.WriteLine(String.Empty);
        }
    }
}
