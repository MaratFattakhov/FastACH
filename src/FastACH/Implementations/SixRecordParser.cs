using FastACH.Interfaces;
using FastACH.Models;

namespace FastACH.Implementations
{
    internal class SixRecordParser : IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Entry Detail Record (6 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ACHRecordType6 sixRecord = new();

            sixRecord.TransactionCode = data.Substring(1, 2).Trim();
            sixRecord.ReceivingDFINumber = data.Substring(3, 8).Trim();
            sixRecord.CheckDigit = data.Substring(11, 1).Trim();
            sixRecord.DFIAccountNumber = data.Substring(12, 17).Trim();
            sixRecord.Amount = DataFormatHelper.ParseDecimal(data.Substring(29, 10).Trim());
            sixRecord.ReceiverIdentificationNumber = data.Substring(39, 15).Trim();
            sixRecord.ReceiverName = data.Substring(54, 22).Trim();
            sixRecord.DiscretionaryData = data.Substring(76, 2).Trim();
            sixRecord.AddendaRecordIndicator = data.Substring(78, 1).Trim();
            sixRecord.TraceNumber = data.Substring(79, 15).Trim();

            return sixRecord;
        }
    }
}
