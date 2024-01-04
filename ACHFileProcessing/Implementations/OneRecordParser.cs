using ACH_Transform.ACHFileProcessor.Interfaces;
using ACH_Transform.ACHFileProcessor.Models;
using System;

namespace ACH_Transform.ACHFileProcessor.Implementations
{
    internal class OneRecordParser : IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data)
        {
            if (String.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid File Header (1 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ACHRecordType1 oneRecord = new();

            oneRecord.ImmediateDestination = data.Substring(3, 10).Trim();
            oneRecord.ImmediateOrigin = data.Substring(13, 10).Trim();
            oneRecord.FileCreationDate = data.Substring(23, 6).Trim();
            oneRecord.FileCreationTime = data.Substring(29, 4).Trim();
            oneRecord.FileIdModifier = data.Substring(33, 1).Trim();                
            oneRecord.ImmediateDestinationName = data.Substring(40, 23).Trim();
            oneRecord.ImmediateOriginName = data.Substring(63, 23).Trim();
            oneRecord.ReferenceCode = data.Substring(86, 8).Trim();
                
            return oneRecord;
        }
    }
}
