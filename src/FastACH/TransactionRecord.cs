using FastACH.Records;

namespace FastACH
{
    public class TransactionRecord
    {
        public required EntryDetailRecord EntryDetail { get; set; }
        public List<AddendaRecord> AddendaRecords { get; set; } = new List<AddendaRecord>();
    }
}
