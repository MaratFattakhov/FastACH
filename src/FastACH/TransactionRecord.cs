using FastACH.Records;

namespace FastACH
{
    /// <summary>
    /// Represents an ACH transaction including the entry detail and any associated addenda records.
    /// </summary>
    public class TransactionRecord
    {
        /// <summary>
        /// Gets or sets the Entry Detail Record (6 record) containing the transaction details.
        /// </summary>
        public required EntryDetailRecord EntryDetail { get; set; }
        
        /// <summary>
        /// Gets or sets the list of Addenda Records (7 records) associated with this transaction.
        /// </summary>
        public List<AddendaRecord> AddendaRecords { get; set; } = new List<AddendaRecord>();
    }
}
