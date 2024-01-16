namespace FastACH
{
    public class ConsoleWriter : StringWriter
    {
        private readonly ConsoleColor[] _colors;
        private int _colorIndex = 0;

        protected ConsoleWriter(params ConsoleColor[] colors): base(Console.Out)
        {
           _colors = colors;
        }

        public static ConsoleWriter CreateForRecord(IRecord record)
        {
            switch (record.RecordTypeCode)
            {
                case "1":
                    return CreateForOneRecord();
                case "5":
                    return CreateForFiveRecord();
                case "6":
                    return CreateForSixRecord();
                case "7":
                    return CreateForSevenRecord();
                case "8":
                    return CreateForEightRecord();
                case "9":
                    return CreateForNineRecord();
                default:
                    throw new NotImplementedException();
            }

            // obe record
        }

        private static ConsoleWriter CreateForNineRecord()
        {
            return new ConsoleWriter(
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.Magenta,
                ConsoleColor.Gray,
                ConsoleColor.Yellow,
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.White
            );
        }

        private static ConsoleWriter CreateForEightRecord()
        {
            return new ConsoleWriter(
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.Magenta,
                ConsoleColor.Gray,
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.White,
                ConsoleColor.White,
                ConsoleColor.Red,
                ConsoleColor.Green
            );
        }

        private static ConsoleWriter CreateForSevenRecord()
        {
            return new ConsoleWriter(
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.White,
                ConsoleColor.Green,
                ConsoleColor.Gray
            );
        }

        private static ConsoleWriter CreateForSixRecord()
        {
            return new ConsoleWriter(
                ConsoleColor.Red,
                ConsoleColor.White,
                ConsoleColor.Blue,
                ConsoleColor.Blue,
                ConsoleColor.Gray,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.White,
                ConsoleColor.Red,
                ConsoleColor.Blue
            );
        }

        private static ConsoleWriter CreateForOneRecord()
        {
            return new ConsoleWriter(
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.White,
                ConsoleColor.Green,
                ConsoleColor.Gray,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.White,
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.White,
                ConsoleColor.Green
            );
        }

        private static ConsoleWriter CreateForFiveRecord()
        {
            return new ConsoleWriter(
                ConsoleColor.Red,
                ConsoleColor.White,
                ConsoleColor.Green,
                ConsoleColor.Gray,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.White,
                ConsoleColor.Red,
                ConsoleColor.Blue,
                ConsoleColor.White,
                ConsoleColor.Green,
                ConsoleColor.Yellow
            );
        }

        public override void Write(string part)
        {
            Console.ForegroundColor = _colors[_colorIndex++];
            base.Write(part);
            Console.ResetColor();
        }
    }
}
