
namespace FastACH.Models
{
    public class SixRecord : IRecord
    {
        // Addenda record for the Detail 
        public SevenRecord AddendaRecord = null;

        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "6";

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

        // Position 80-94: Trace Number (numeric)
        public string TraceNumber { get; set; }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(DataFormatHelper.FormatForAch(TransactionCode, 2));
            writer.Write(DataFormatHelper.FormatForAch(ReceivingDFINumber, 8));
            writer.Write(DataFormatHelper.FormatForAch(CheckDigit, 1));
            writer.Write(DataFormatHelper.FormatForAch(DFIAccountNumber, 17));
            writer.Write(DataFormatHelper.FormatForAch(Amount, 10));
            writer.Write(DataFormatHelper.FormatForAch(ReceiverIdentificationNumber, 15));
            writer.Write(DataFormatHelper.FormatForAch(ReceiverName, 22));
            writer.Write(DataFormatHelper.FormatForAch(DiscretionaryData, 2));
            writer.Write(DataFormatHelper.FormatForAch(AddendaRecordIndicator, 1));
            writer.Write(DataFormatHelper.FormatForAch(TraceNumber, 15));
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Entry Detail Record (6 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            TransactionCode = data.Substring(1, 2).Trim();
            ReceivingDFINumber = data.Substring(3, 8).Trim();
            CheckDigit = data.Substring(11, 1).Trim();
            DFIAccountNumber = data.Substring(12, 17).Trim();
            Amount = DataFormatHelper.ParseDecimal(data.Substring(29, 10).Trim());
            ReceiverIdentificationNumber = data.Substring(39, 15).Trim();
            ReceiverName = data.Substring(54, 22).Trim();
            DiscretionaryData = data.Substring(76, 2).Trim();
            AddendaRecordIndicator = data.Substring(78, 1).Trim();
            TraceNumber = data.Substring(79, 15).Trim();
        }
    }
}
