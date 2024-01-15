
namespace FastACH.Models
{
    public class SevenRecord : IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "7";

        // Position 2-3: Addenda Type Code (numeric)
        public string AddendaTypeCode => "05";

        // Position 4-83: Addenda Information (alpha-numeric)
        public string AddendaInformation { get; set; }

        // Position 84-87: Addenda Sequence Number (numeric)
        public string AddendaSequenceNumber { get; set; }

        // Position 88-94: Entry Detail Sequence Number (numeric)
        public string EntryDetailSequenceNumber { get; set; }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode);
            writer.Write(AddendaTypeCode);
            writer.Write(DataFormatHelper.FormatForAch(AddendaInformation, 80));
            writer.Write(DataFormatHelper.FormatForAch(AddendaSequenceNumber, 4, true));
            writer.Write(DataFormatHelper.FormatForAch(EntryDetailSequenceNumber, 7, true));
        }

        public void ParseRecord(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 94)
            {
                throw new ArgumentException($"Invalid Addenda Record (7 record) length: Expected 94, Actual {data?.Length ?? 0}");
            }

            AddendaInformation = data.Substring(3, 80).Trim();
            AddendaSequenceNumber = data.Substring(83, 4).Trim();
            EntryDetailSequenceNumber = data.Substring(87, 7).Trim();
        }
    }
}
