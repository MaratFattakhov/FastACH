using FastACH.Records;

namespace FastACH
{
    /// <summary>
    /// Represents a Batch of ACH Transactions.
    /// </summary>
    public class BatchRecord
    {
        /// <summary>
        /// Gets or sets the Batch Header Record.
        /// </summary>
        public required BatchHeaderRecord BatchHeader { get; set; }

        /// <summary>
        /// Gets or sets the Batch Control Record.
        /// </summary>
        public BatchControlRecord BatchControl { get; set; } = BatchControlRecord.Empty;

        /// <summary>
        /// Gets or sets the list of transactions for this batch.
        /// </summary>
        public List<TransactionRecord> TransactionRecords { get; set; } = new();
    }
}