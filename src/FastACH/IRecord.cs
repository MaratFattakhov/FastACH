namespace FastACH
{
    public interface IRecord
    {
        public string RecordTypeCode { get; }

        public void Write(ILineWriter writer);

        public uint LineNumber { get; }
    }
}
