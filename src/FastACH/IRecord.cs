namespace FastACH
{
    /// <summary>
    /// Represents an ACH record that can be written to a line.
    /// </summary>
    public interface IRecord
    {
        /// <summary>
        /// Gets the record type code that identifies the type of ACH record.
        /// </summary>
        public string RecordTypeCode { get; }

        /// <summary>
        /// Writes the record to the specified line writer.
        /// </summary>
        /// <param name="writer">The line writer to write the record to.</param>
        public void Write(ILineWriter writer);

        /// <summary>
        /// Gets the line number where this record appears in the ACH file.
        /// </summary>
        public uint LineNumber { get; }
    }
}
