using ACH_Transform.ACHFileProcessor.Interfaces;
using ACH_Transform.ACHFileProcessor.Models;
using System;

namespace ACH_Transform.ACHFileProcessor.Implementations
{
    internal class FiveRecordParser : IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data)
        {
            if (String.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Batch Header Record Header (5 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ACHRecordType5 fiveRecord = new();

            fiveRecord.ServiceClassCode = data.Substring(1, 3).Trim();
            fiveRecord.CompanyName = data.Substring(4, 16).Trim();
            fiveRecord.CompanyDiscretionaryData = data.Substring(20, 20).Trim();
            fiveRecord.CompanyIdentification = data.Substring(40, 10).Trim();
            fiveRecord.StandardEntryClassCode = data.Substring(50, 3).Trim();
            fiveRecord.CompanyEntryDescription = data.Substring(53, 10).Trim();
            fiveRecord.CompanyDescriptiveDate = data.Substring(63, 6).Trim();
            fiveRecord.EffectiveEntryDate = data.Substring(69, 6).Trim();
            fiveRecord.SettlementDate = data.Substring(75, 3).Trim();
            fiveRecord.OriginatorsStatusCode = data.Substring(78, 1).Trim();
            fiveRecord.OriginatorsDFINumber = data.Substring(79, 8).Trim();
            fiveRecord.BatchNumber = DataFormatHelper.ParseInt(data.Substring(87, 7).Trim());

            return fiveRecord;
        }
    }
}
