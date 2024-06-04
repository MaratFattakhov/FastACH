namespace FastACH
{
    internal class StringWriter : ILineWriter
    {
        private readonly TextWriter _streamWriter;

        public StringWriter(TextWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        protected virtual void Write(string value)
        {
            _streamWriter.Write(value);
        }

        public void Write(string value, byte length)
        {
            if (value is null)
            {
                // support projects that use null strings
                value = string.Empty;
            }

            // todo: do we need this check?
            //if (value.Length > length)
            //    throw new InvalidOperationException($"Value {value} is too long for field length {length}");

            if (value.Length > length)
                value = value.Substring(0, length);

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
