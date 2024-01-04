using System;
using System.Collections.Generic;

namespace ACH_Transform.ACHFileProcessor.Models
{
    public class ACHRecordType6 : ACHBaseRecord
    {
        // Addenda record for the Detail 
        public ACHRecordType7 AddendaRecord = null;

        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode = "6";

        // Position 2-3: Transaction Code (numeric)
        public string TransactionCode { get; set; }

        // Position 4-11: Receiving DFI Identification Number (numeric)
        public string ReceivingDFINumber { get; set; }

        // Position 12-22: Check Digit (numeric)
        public string CheckDigit { get; set; }

        // Position 13-29: DFIAccount Number (alpha-numeric)
        public string DFIAccountNumber { get; set; }

        // Position 30-39: Amount (numeric)
        public decimal Amount { get; set; }

        // Position 40-54: Receiver Identification Number (alpha-numeric)
        public string ReceiverIdentificationNumber { get; set; }

        // Position 55-76: Receiver Name (alpha)
        public string ReceiverName { get; set; }

        // Position 77-78: Discretionary Data (alpha-numeric)
        public string DiscretionaryData { get; set; }

        // Position 79-79: Addenda Record Indicator (numeric)
        public string AddendaRecordIndicator { get; set; }

        // Position 80-94: Trace Number (alpha-numeric)
        public string TraceNumber { get; set; }

        public override string WriteAsText() =>
            $"{RecordTypeCode}" +
            $"{DataFormatHelper.FormatForAch(TransactionCode, 2, true)}" +
            $"{DataFormatHelper.FormatForAch(ReceivingDFINumber, 8, true)}" +
            $"{DataFormatHelper.FormatForAch(CheckDigit, 1)}" +
            $"{DataFormatHelper.FormatForAch(DFIAccountNumber, 17)}" +
            $"{DataFormatHelper.FormatForAch(Amount, 10)}" +
            $"{DataFormatHelper.FormatForAch(ReceiverIdentificationNumber, 15)}" +
            $"{DataFormatHelper.FormatForAch(ReceiverName, 22)}" +
            $"{DataFormatHelper.FormatForAch(DiscretionaryData, 2)}" +
            $"{DataFormatHelper.FormatForAch(AddendaRecordIndicator, 1)}" +
            $"{DataFormatHelper.FormatForAch(TraceNumber, 15)}";

        public override void WriteToConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RecordTypeCode);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(TransactionCode, 2));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(ReceivingDFINumber, 8));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(CheckDigit, 1));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DataFormatHelper.FormatForAch(DFIAccountNumber, 17));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DataFormatHelper.FormatForAch(Amount, 10));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(DataFormatHelper.FormatForAch(ReceiverIdentificationNumber, 15));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(DataFormatHelper.FormatForAch(ReceiverName, 22));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(DiscretionaryData, 2));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DataFormatHelper.FormatForAch(AddendaRecordIndicator, 1));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(TraceNumber, 15));
            Console.WriteLine(String.Empty);
        }
    }
}
