namespace FastACH
{
    public class WritingOptions
    {
        private readonly Func<string> _traceNumberGenerator;
        private readonly Func<ulong> _batchNumberGenerator;

        public WritingOptions(AchFile achFile)
        {
            _traceNumberGenerator = GetDefaultTraceNumberGenerator(achFile);
            _batchNumberGenerator = GetDefaultBatchNumberGenerator();
        }

        public virtual ulong GetNextBatchNumber()
        {
            return _batchNumberGenerator();
        }

        public virtual string GetNextTraceNumber()
        {
            return _traceNumberGenerator();
        }

        public uint BlockingFactor { get; set; } = 10;

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
