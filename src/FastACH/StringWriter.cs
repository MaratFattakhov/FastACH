namespace FastACH
{
    public class StringWriter : ILineWriter
    {
        private readonly StreamWriter _streamWriter;

        public StringWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void Write(string value)
        {
            _streamWriter.Write(value);
        }
    }
}
