﻿namespace FastACH.Models
{
    public class FiveRecord : IRecord
    {
        // List of Tranasctions for this Batch
        public List<SixRecord> SixRecordList = new();

        public EightRecord EightRecord = new();

        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "5";

        /// <summary>
        ///     Position 2-4: Service Class Code (numeric)
        ///     200 – ACH Entries Mixed Debits and Credits
        ///     220 – ACH Credits Only
        ///     225 – ACH Debits Only
        /// </summary>
        public uint ServiceClassCode { get; set; }

        // Position 5-20: Company Name (alpha-numeric)
        public string CompanyName { get; set; }

        // Position 21-40: Company Discretionary Data (alpha-numeric)
        public string CompanyDiscretionaryData { get; set; }

        // Position 41-50: Company Identification (alpha-numeric)
        public string CompanyIdentification { get; set; }

        // Position 51-53: Standard Entry Class Code (alpha-numeric)
        public string StandardEntryClassCode { get; set; } = "PPD";

        // Position 54-63: Company Entry Description (alpha-numeric)
        public string CompanyEntryDescription { get; set; }

        // Position 64-69: Company Descriptive Date (numeric, yyMMdd)
        public string CompanyDescriptiveDate { get; set; }

        // Position 70-75: Effective Entry Date (numeric, yyMMdd)
        public DateOnly? EffectiveEntryDate { get; set; }

        // Position 76-78: Julian Settlement Date (numeric), Always blank
        public string JulianSettlementDate { get; set; } = "   ";

        // Position 79-79: Originator's Status Code (numeric)
        public char OriginatorsStatusCode { get; set; } = '1';

        // Position 80-87: Originator's DFI Identification Number (numeric)
        public string OriginatorsDFINumber { get; set; }

        // Position 88-94: Batch Number (numeric)
        public ulong BatchNumber { get; set; }

        public void RecalculateTotals(
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator)
        {
            UpdateBatchNumbers(batchNumberGenerator);
            UpdateTraceNumbers(traceNumberGenerator);
            EightRecord.EntryAddendaCount = (uint)SixRecordList.Count + (uint)SixRecordList.Where(x => x.AddendaRecord != null).Count();
            EightRecord.EntryHash = SixRecordList
                .Select(p => p.ReceivingDFINumber)
                .Aggregate((ulong)0, (a, b) => a + b);
            EightRecord.TotalCreditEntryDollarAmount = SixRecordList.Where(x => TransactionCodes.IsCredit(x.TransactionCode)).Sum(x => x.Amount);
            EightRecord.TotalDebitEntryDollarAmount = SixRecordList.Where(x => TransactionCodes.IsDebit(x.TransactionCode)).Sum(x => x.Amount);
        }

        public void UpdateBatchNumbers(Func<ulong> batchNumberGenerator)
        {
            BatchNumber = batchNumberGenerator();
            EightRecord.BatchNumber = BatchNumber;
        }

        public void UpdateTraceNumbers(Func<string> traceNumberGenerator)
        {
            foreach (var sixRecord in SixRecordList)
            {
                sixRecord.TraceNumber = traceNumberGenerator();
            }
        }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(ServiceClassCode, 3);
            writer.Write(CompanyName, 16);
            writer.Write(CompanyDiscretionaryData, 20);
            writer.Write(CompanyIdentification, 10);
            writer.Write(StandardEntryClassCode, 3);
            writer.Write(CompanyEntryDescription, 10);
            writer.Write(CompanyDescriptiveDate, 6);
            writer.Write(EffectiveEntryDate);
            writer.Write(JulianSettlementDate, 3);
            writer.Write(OriginatorsStatusCode.ToString(), 1);
            writer.Write(OriginatorsDFINumber, 8);
            writer.Write(BatchNumber, 7);
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Batch Header Record Header (5 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ServiceClassCode = uint.Parse(data.Substring(1, 3));
            CompanyName = data.Substring(4, 16).Trim();
            CompanyDiscretionaryData = data.Substring(20, 20).Trim();
            CompanyIdentification = data.Substring(40, 10).Trim();
            StandardEntryClassCode = data.Substring(50, 3).Trim();
            CompanyEntryDescription = data.Substring(53, 10).Trim();
            CompanyDescriptiveDate = data.Substring(63, 6).Trim();
            EffectiveEntryDate = DateOnly.TryParseExact(data.Substring(69, 6), "yyMMdd", out var date) ? date : null;
            JulianSettlementDate = data.Substring(75, 3).Trim();
            OriginatorsStatusCode = data.Substring(78, 1)[0];
            OriginatorsDFINumber = data.Substring(79, 8).Trim();
            BatchNumber = ulong.Parse(data.Substring(87, 7));
        }
    }
}
