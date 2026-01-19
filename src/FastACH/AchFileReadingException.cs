namespace FastACH
{
    /// <summary>
    /// Exception thrown when an error occurs while reading an ACH file.
    /// </summary>
    public class AchFileReadingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AchFileReadingException class with the line number where the error occurred.
        /// </summary>
        /// <param name="line">The line number where the error occurred.</param>
        /// <param name="innerException">The exception that caused the reading error.</param>
        public AchFileReadingException(uint line, Exception innerException) : base($"An Error happened on {line} line: {innerException.Message}", innerException)
        {
        }
    }
}
