using FastACH.Records;

namespace FastACH
{
    /// <summary>
    /// Represents a Batch of ACH Transactions.
    /// </summary>
    public class BatchRecord
    {
        /// <summary>
        /// Represents the Batch Header Record.
        /// </summary>
        public required BatchHeaderRecord BatchHeader { get; set; }

        /// <summary>
        /// Represents the Batch Control Record.
        /// </summary>
        public BatchControlRecord BatchControl { get; set; } = BatchControlRecord.Empty;

        /// <summary>
        /// List of Tranasctions for this Batch.
        /// </summary>
        public List<TransactionRecord> TransactionRecords = new();
    }
}