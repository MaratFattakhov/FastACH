namespace FastACH
{
    /// <summary>
    /// Provides configuration options for writing ACH files.
    /// </summary>
    public class WritingOptions
    {
        private readonly Func<string> _traceNumberGenerator;
        private readonly Func<ulong> _batchNumberGenerator;

        /// <summary>
        /// Initializes a new instance of the WritingOptions class with default generators for an ACH file.
        /// </summary>
        /// <param name="achFile">The ACH file to create writing options for.</param>
        public WritingOptions(AchFile achFile)
        {
            _traceNumberGenerator = GetDefaultTraceNumberGenerator(achFile);
            _batchNumberGenerator = GetDefaultBatchNumberGenerator();
        }

        /// <summary>
        /// Gets the next batch number for writing ACH batches.
        /// </summary>
        /// <returns>The next batch number.</returns>
        public virtual ulong GetNextBatchNumber()
        {
            return _batchNumberGenerator();
        }

        /// <summary>
        /// Gets the next trace number for writing ACH transactions.
        /// </summary>
        /// <returns>The next trace number as a string.</returns>
        public virtual string GetNextTraceNumber()
        {
            return _traceNumberGenerator();
        }

        /// <summary>
        /// Gets a function that generates sequential addenda sequence numbers.
        /// </summary>
        /// <returns>A function that returns incrementing addenda sequence numbers.</returns>
        public virtual Func<uint> GetAddendaSequenceNumberGenerator()
        {
            var addendaSequenceNumber = 0u;
            return new Func<uint>(() => ++addendaSequenceNumber);
        }

        /// <summary>
        /// Gets or sets the blocking factor (number of records per block). Default is 10.
        /// </summary>
        public uint BlockingFactor { get; set; } = 10;

        /// <summary>
        /// Gets or sets a value indicating whether control records should be automatically updated when writing. Default is true.
        /// </summary>
        public bool UpdateControlRecords { get; set; } = true;

        private Func<string> GetDefaultTraceNumberGenerator(AchFile achFile)
        {
            ulong traceNumber = 0;
            return new Func<string>(
                () => achFile.FileHeader.ImmediateDestination.PadLeft(8, ' ').Substring(0, 8) + (++traceNumber).ToString().PadLeft(7, '0'));
        }

        private Func<ulong> GetDefaultBatchNumberGenerator()
        {
            ulong batchNumber = 0;
            return new Func<ulong>(
                               () => ++batchNumber);
        }
    }
}
