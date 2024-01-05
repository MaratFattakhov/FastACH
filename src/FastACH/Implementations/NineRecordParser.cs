using FastACH.Interfaces;
using FastACH.Models;

namespace FastACH.Implementations
{
    internal class NineRecordParser : IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid File Control Record Header (8 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ACHRecordType9 nineRecord = new();

            nineRecord.BatchCount = DataFormatHelper.ParseInt(data.Substring(1, 6).Trim());
            nineRecord.BlockCount = DataFormatHelper.ParseInt(data.Substring(7, 6).Trim());
            nineRecord.EntryAddendaCount = DataFormatHelper.ParseInt(data.Substring(13, 8).Trim());
            nineRecord.EntryHash = DataFormatHelper.ParseInt(data.Substring(21, 10).Trim());
            nineRecord.TotalDebitEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(31, 12).Trim());
            nineRecord.TotalCreditEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(43, 12).Trim());

            return nineRecord;
        }
    }
}
