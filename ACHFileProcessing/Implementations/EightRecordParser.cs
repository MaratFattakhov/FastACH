using ACH_Transform.ACHFileProcessor.Interfaces;
using ACH_Transform.ACHFileProcessor.Models;
using System;

namespace ACH_Transform.ACHFileProcessor.Implementations
{
    internal class EightRecordParser : IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data)
        {
            if (String.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Batch Control Record Header (8 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ACHRecordType8 eightRecord = new();

            eightRecord.ServiceClassCode = data.Substring(1, 3).Trim();
            eightRecord.EntryAddendaCount = DataFormatHelper.ParseInt(data.Substring(4, 6).Trim());
            eightRecord.EntryHash = DataFormatHelper.ParseInt(data.Substring(10, 10).Trim());
            eightRecord.TotalDebitEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(20, 12).Trim());
            eightRecord.TotalCreditEntryDollarAmount = DataFormatHelper.ParseDecimal(data.Substring(32, 12).Trim());
            eightRecord.CompanyIdentification = data.Substring(44, 10).Trim();
            eightRecord.MessageAuthenticationCode = data.Substring(54, 19).Trim();
            eightRecord.OriginatingDFINumber = data.Substring(79, 8).Trim();
            eightRecord.BatchNumber = DataFormatHelper.ParseInt(data.Substring(87, 7).Trim());

            return eightRecord;
        }
    }
}
