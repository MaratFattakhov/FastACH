using FastACH.Records;

namespace FastACH
{
    public class TransactionDetails
    {
        public required EntryDetailRecord EntryDetail { get; set; }
        public required AddendaRecord? Addenda { get; set; }
    }
}
