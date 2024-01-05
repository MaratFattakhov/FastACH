namespace FastACH.Models
{
    public class ACHRecordType1 : ACHBaseRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "1";

        // Position 2-3: Priority Code (numeric)
        public string PriorityCode => "01";

        // Position 4-13: Immediate Destination (numeric)
        public string ImmediateDestination { get; set; }

        // Position 14-23: Immediate Origin (numeric)
        public string ImmediateOrigin { get; set; }

        // Position 24-29: File Creation Date (numeric, YYMMDD)
        public string FileCreationDate { get; set; }

        // Position 30-33: File Creation Time (numeric, HHmm)
        public string FileCreationTime { get; set; }

        // Position 34-34: File ID Modifier (alpha-numeric)
        public string FileIdModifier { get; set; }

        // Position 35-37: Record Size (numeric)
        public string RecordSize => "094";

        // Position 38-39: Blocking Factor (numeric)
        public string BlockingFactor => "10";

        // Position 40-40: Format Code (numeric)
        public string FormatCode => "1";

        // Position 41-63: Immediate Destination Name (alpha-numeric)
        public string ImmediateDestinationName { get; set; }

        // Position 64-86: Immediate Origin Name (alpha-numeric)
        public string ImmediateOriginName { get; set; }

        // Position 87-94: Reference Code (alpha-numeric)
        public string ReferenceCode { get; set; }

        public override string WriteAsText()
        {
            return
                $"{RecordTypeCode}" +
                $"{PriorityCode}" +
                $"{DataFormatHelper.FormatForAch(ImmediateDestination, 10, true)}" +
                $"{DataFormatHelper.FormatForAch(ImmediateOrigin, 10, true)}" +
                $"{DataFormatHelper.FormatForAch(FileCreationDate, 6, true)}" +
                $"{DataFormatHelper.FormatForAch(FileCreationTime, 4, true)}" +
                $"{DataFormatHelper.FormatForAch(FileIdModifier, 1)}" +
                $"{RecordSize}" +
                $"{BlockingFactor}" +
                $"{FormatCode}" +
                $"{DataFormatHelper.FormatForAch(ImmediateDestinationName, 23)}" +
                $"{DataFormatHelper.FormatForAch(ImmediateOriginName, 23)}" +
                $"{DataFormatHelper.FormatForAch(ReferenceCode, 8)}";
        }

        public override void WriteToConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RecordTypeCode);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(PriorityCode);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(ImmediateDestination, 10, true));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(ImmediateOrigin, 10, true));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DataFormatHelper.FormatForAch(FileCreationDate, 6, true));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DataFormatHelper.FormatForAch(FileCreationTime, 4, true));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(DataFormatHelper.FormatForAch(FileIdModifier, 1));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(RecordSize);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(BlockingFactor);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(FormatCode);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DataFormatHelper.FormatForAch(ImmediateDestinationName, 23));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DataFormatHelper.FormatForAch(ImmediateOriginName, 23));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DataFormatHelper.FormatForAch(ReferenceCode, 8));
            Console.WriteLine(string.Empty);
        }
    }
}
