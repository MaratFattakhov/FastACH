namespace FastACH
{
    public class StringWriter : ILineWriter
    {
        private readonly TextWriter _streamWriter;

        public StringWriter(TextWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public virtual void Write(string value)
        {
            _streamWriter.Write(value);
        }

        public void Write(string value, byte length)
        {
            Write(value.PadRight(length, ' '));
        }

        public void Write(DateOnly? date)
        {
            Write($"{date:yyMMdd}", 6);
        }

        public void Write(ulong value, byte length)
        {
            var s = value.ToString();
            if (s.Length > length)
                throw new InvalidOperationException($"Value {s} is too long for field length {length}");

            Write(s.PadLeft(length, '0'));
        }

        public void Write(TimeOnly? time)
        {
            Write($"{time:HHmm}", 4);
        }
    }
}
