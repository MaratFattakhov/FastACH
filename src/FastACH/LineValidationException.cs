namespace FastACH
{
    /// <summary>
    /// Exception thrown when validation errors occur while reading a line of an ACH record.
    /// </summary>
    public class LineValidationException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the LineValidationException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="position">The character position within the line where the validation error occurred.</param>
        public LineValidationException(string message, int position) : base(message)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the character position within the line where the validation error occurred.
        /// </summary>
        public int Position { get; }
    }
}
