namespace FastACH.Models
{
    public class ACHRecordType5 : ACHBaseRecord
    {
        public ACHRecordType5()
        {
        }

        // List of Tranasctions for this Batch
        public List<ACHRecordType6> SixRecordList = new();
        public ACHRecordType8 EightRecord = new();

        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode = "5";

        // Position 2-4: Service Class Code (numeric)
        public string ServiceClassCode { get; set; }

        // Position 5-20: Company Name (alpha-numeric)
        public string CompanyName { get; set; }

        // Position 21-40: Company Discretionary Data (alpha-numeric)
        public string CompanyDiscretionaryData { get; set; }

        // Position 41-50: Company Identification (alpha-numeric)
        public string CompanyIdentification { get; set; }

        // Position 51-53: Standard Entry Class Code (alpha-numeric)
        public string StandardEntryClassCode { get; set; }

        // Position 54-63: Company Entry Description (alpha-numeric)
        public string CompanyEntryDescription { get; set; }

        // Position 64-69: Company Descriptive Date (numeric, YYMMDD)
        public string CompanyDescriptiveDate { get; set; }

        // Position 70-75: Effective Entry Date (numeric, YYMMDD)
        public string EffectiveEntryDate { get; set; }

        // Position 76-78: Settlement Date (numeric, YYMMDD)
        public string SettlementDate { get; set; }

        // Position 79-79: Originator's Status Code (numeric)
        public string OriginatorsStatusCode { get; set; }

        // Position 80-87: Originator's DFI Identification Number (numeric)
        public string OriginatorsDFINumber { get; set; }

        // Position 88-94: Batch Number (numeric)
        public int BatchNumber { get; set; }

        public void RecalculateTotals(int counter)
        {
            BatchNumber = counter;
            EightRecord.BatchNumber = counter;
            EightRecord.EntryAddendaCount = SixRecordList.Count + SixRecordList.Where(x => x.AddendaRecord != null).Count();
            EightRecord.EntryHash = SixRecordList.Sum(x => long.Parse(x.ReceivingDFINumber));
            EightRecord.TotalCreditEntryDollarAmount = SixRecordList.Where(x => DataFormatHelper.CreditCodes.Contains(x.TransactionCode)).Sum(x => x.Amount);
            EightRecord.TotalDebitEntryDollarAmount = SixRecordList.Where(x => DataFormatHelper.DebitCodes.Contains(x.TransactionCode)).Sum(x => x.Amount);
        }

        public override string WriteAsText() =>
            $"{RecordTypeCode}" +
            $"{DataFormatHelper.FormatForAch(ServiceClassCode, 3, true)}" +
            $"{DataFormatHelper.FormatForAch(CompanyName, 16)}" +
            $"{DataFormatHelper.FormatForAch(CompanyDiscretionaryData, 20)}" +
            $"{DataFormatHelper.FormatForAch(CompanyIdentification, 10, true)}" +
            $"{DataFormatHelper.FormatForAch(StandardEntryClassCode, 3, true)}" +
            $"{DataFormatHelper.FormatForAch(CompanyEntryDescription, 10)}" +
            $"{DataFormatHelper.FormatForAch(CompanyDescriptiveDate, 6)}" +
            $"{DataFormatHelper.FormatForAch(EffectiveEntryDate, 6)}" +
            $"{DataFormatHelper.FormatForAch(SettlementDate, 3)}" +
            $"{DataFormatHelper.FormatForAch(OriginatorsStatusCode, 1)}" +
            $"{DataFormatHelper.FormatForAch(OriginatorsDFINumber, 8)}" +
            $"{DataFormatHelper.FormatForAch(BatchNumber, 7)}";

        public override void WriteToConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RecordTypeCode);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(ServiceClassCode, 3, true));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(CompanyName, 16));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DataFormatHelper.FormatForAch(CompanyDiscretionaryData, 20));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DataFormatHelper.FormatForAch(CompanyIdentification, 10, true));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(DataFormatHelper.FormatForAch(StandardEntryClassCode, 3, true));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(DataFormatHelper.FormatForAch(CompanyEntryDescription, 10));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(CompanyDescriptiveDate, 6));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DataFormatHelper.FormatForAch(EffectiveEntryDate, 6));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(SettlementDate, 3));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(OriginatorsStatusCode, 1));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(OriginatorsDFINumber, 8));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DataFormatHelper.FormatForAch(BatchNumber, 7));
            Console.WriteLine(string.Empty);
        }
    }
}
