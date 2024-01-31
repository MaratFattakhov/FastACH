using FastACH.Records;

namespace FastACH
{
    public class TransactionRecord
    {
        public required EntryDetailRecord EntryDetail { get; set; }
        public required AddendaRecord? Addenda { get; set; }
    }
}
