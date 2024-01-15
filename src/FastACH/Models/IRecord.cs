namespace FastACH.Models
{
    public interface IRecord
    {
        public string RecordTypeCode { get; }

        public void Write(ILineWriter writer);       
    }
}
