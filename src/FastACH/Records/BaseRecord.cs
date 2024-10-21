namespace FastACH.Records
{
    public record class BaseRecord
    {
        public uint LineNumber { get; set; }

        public BaseRecord()
        {
        }
    }
}
