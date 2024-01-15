﻿using System.Globalization;

namespace FastACH.Models
{
    public class OneRecord : IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "1";

        // Position 2-3: Priority Code (numeric)
        public string PriorityCode => "01";

        // Position 4-13: Immediate Destination (numeric)
        public string ImmediateDestination { get; set; }

        // Position 14-23: Immediate Origin (numeric)
        public string ImmediateOrigin { get; set; }

        // Position 24-29: File Creation Date (numeric, yyMMdd)
        public DateOnly FileCreationDate { get; set; }

        // Position 30-33: File Creation Time (numeric, HHmm)
        public TimeOnly FileCreationTime { get; set; }

        // Position 34-34: File ID Modifier (alpha-numeric)
        public char FileIdModifier { get; set; }

        // Position 35-37: Record Size (numeric)
        public uint RecordSize => 94;

        // Position 38-39: Blocking Factor (numeric)
        public uint BlockingFactor => 10;

        // Position 40-40: Format Code (numeric)
        public uint FormatCode => 1;

        // Position 41-63: Immediate Destination Name (alpha-numeric)
        public string ImmediateDestinationName { get; set; }

        // Position 64-86: Immediate Origin Name (alpha-numeric)
        public string ImmediateOriginName { get; set; }

        // Position 87-94: Reference Code (alpha-numeric)
        public string ReferenceCode { get; set; }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(PriorityCode);
            writer.Write(DataFormatHelper.FormatForAch(ImmediateDestination, 10, true));
            writer.Write(DataFormatHelper.FormatForAch(ImmediateOrigin, 10, true));
            writer.Write($"{FileCreationDate:yyMMdd}");
            writer.Write($"{FileCreationTime:HHmm}");
            writer.Write(DataFormatHelper.FormatForAch(FileIdModifier, 1));
            writer.Write(DataFormatHelper.FormatForAch(RecordSize, 3, true));
            writer.Write($"{BlockingFactor}");
            writer.Write($"{FormatCode}");
            writer.Write(DataFormatHelper.FormatForAch(ImmediateDestinationName, 23));
            writer.Write(DataFormatHelper.FormatForAch(ImmediateOriginName, 23));
            writer.Write(DataFormatHelper.FormatForAch(ReferenceCode, 8));
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid File Header (1 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            ImmediateDestination = data.Substring(3, 10).Trim();
            ImmediateOrigin = data.Substring(13, 10).Trim();
            FileCreationDate = DateOnly.ParseExact(data.Substring(23, 6).Trim(), "yyMMdd", CultureInfo.InvariantCulture);
            FileCreationTime = TimeOnly.ParseExact(data.Substring(29, 4).Trim(), "HHmm", CultureInfo.InvariantCulture);
            FileIdModifier = data.Substring(33, 1).Trim()[0];
            ImmediateDestinationName = data.Substring(40, 23).Trim();
            ImmediateOriginName = data.Substring(63, 23).Trim();
            ReferenceCode = data.Substring(86, 8).Trim();
        }
    }
}
