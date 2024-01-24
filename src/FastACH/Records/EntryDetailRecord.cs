namespace FastACH.Records
{
    /// <summary>
    /// Represents an Entry Detail Record (6 record)
    /// </summary>
    public class EntryDetailRecord : IRecord
    {
        /// <summary>
        /// Position 1-1: Record Type Code (numeric)
        /// </summary>
        public string RecordTypeCode => "6";

        /// <summary>
        ///     Position 2-3: Transaction Code (numeric):
        ///     22 Live Checking Account Credit
        ///     23 Pre-note Checking Account Credit
        ///     27 Live Checking Account Debit
        ///     28 Pre-note Checking Account Debit
        ///     32 Live Savings Account Credit
        ///     33 Pre-note Savings Account Credit
        ///     37 Live Savings Account Debit
        ///     38 Pre-note Savings Account Debit
        /// </summary>
        public uint TransactionCode { get; set; }

        /// <summary>
        /// Position 4-11: Receiving DFI Identification (numeric)
        /// </summary>
        public ulong ReceivingDFIID { get; set; }

        /// <summary>
        /// Position 12-22: Check Digit (numeric)
        /// </summary>
        public char CheckDigit { get; set; }

        /// <summary>
        /// Position 13-29: DFIAccount Number (alpha-numeric)
        /// </summary>
        public string DFIAccountNumber { get; set; }

        /// <summary>
        /// Position 30-39: Amount (numeric)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Position 40-54: Receiver Identification Number (alpha-numeric)
        /// </summary>
        public string ReceiverIdentificationNumber { get; set; }

        /// <summary>
        /// Position 55-76: Receiver Name (alpha)
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Position 77-78: Discretionary Data (alpha-numeric)
        /// </summary>
        public string? DiscretionaryData { get; set; }

        /// <summary>
        /// Position 79-79: Addenda Record Indicator (numeric)
        /// </summary>
        public bool AddendaRecordIndicator { get; set; }

        /// <summary>
        /// Position 80-94: Trace Number (numeric)
        /// </summary>
        public string TraceNumber { get; set; }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode, 1);
            writer.Write(TransactionCode, 2);
            writer.Write(ReceivingDFIID, 8);
            writer.Write(CheckDigit.ToString(), 1);
            writer.Write(DFIAccountNumber, 17);
            writer.Write((ulong)Math.Round(Amount * 100, MidpointRounding.AwayFromZero), 10);
            writer.Write(ReceiverIdentificationNumber, 15);
            writer.Write(ReceiverName, 22);
            writer.Write(DiscretionaryData ?? string.Empty, 2);
            writer.Write(AddendaRecordIndicator ? "1" : "0", 1);
            writer.Write(TraceNumber, 15);
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Entry Detail Record (6 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            TransactionCode = uint.Parse(data.Substring(1, 2));
            ReceivingDFIID = ulong.Parse(data.Substring(3, 8));
            CheckDigit = data.Substring(11, 1)[0];
            DFIAccountNumber = data.Substring(12, 17).Trim();
            Amount = decimal.Parse(data.Substring(29, 10)) / 100;
            ReceiverIdentificationNumber = data.Substring(39, 15).Trim();
            ReceiverName = data.Substring(54, 22).Trim();
            DiscretionaryData = data.Substring(76, 2).Trim();
            AddendaRecordIndicator = data.Substring(78, 1) switch
            {
                "0" => false,
                "1" => true,
                _ => throw new ArgumentException($"Invalid Addenda Record Indicator (6 record) value: Expected 0 or 1, Actual {data.Substring(78, 1)}"),
            };
            TraceNumber = data.Substring(79, 15).Trim();
        }
    }
}
