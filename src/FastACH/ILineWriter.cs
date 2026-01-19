namespace FastACH
{
    /// <summary>
    /// Provides methods for writing ACH record fields to a line.
    /// </summary>
    public interface ILineWriter
    {
        /// <summary>
        /// Writes a string value with the specified length, padding as necessary.
        /// </summary>
        /// <param name="part">The string value to write.</param>
        /// <param name="length">The total length of the field.</param>
        void Write(string part, byte length);
        
        /// <summary>
        /// Writes a numeric value with the specified length, padding with zeros as necessary.
        /// </summary>
        /// <param name="value">The numeric value to write.</param>
        /// <param name="length">The total length of the field.</param>
        void Write(ulong value, byte length);
        
        /// <summary>
        /// Writes a date value in the NACHA date format (yyMMdd).
        /// </summary>
        /// <param name="date">The date value to write.</param>
        void Write(DateOnly? date);
        
        /// <summary>
        /// Writes a time value in the NACHA time format (HHmm).
        /// </summary>
        /// <param name="time">The time value to write.</param>
        void Write(TimeOnly? time);
    }
}
