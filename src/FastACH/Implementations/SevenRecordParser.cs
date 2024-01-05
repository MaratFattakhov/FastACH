using FastACH.Interfaces;
using FastACH.Models;

namespace FastACH.Implementations
{
    internal class SevenRecordParser : IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Addenda Record (7 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ACHRecordType7 sevenRecord = new();

            sevenRecord.AddendaInformation = data.Substring(3, 80).Trim();
            sevenRecord.AddendaSequenceNumber = data.Substring(83, 4).Trim();
            sevenRecord.EntryDetailSequenceNumber = data.Substring(87, 7).Trim();

            return sevenRecord;
        }
    }
}
