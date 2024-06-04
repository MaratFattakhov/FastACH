namespace FastACH
{
    public class AchFileReadingException : Exception
    {
        public AchFileReadingException(uint line, Exception innerException) : base($"An Error happened on {line}: {innerException.Message}", innerException)
        {
        }
    }
}
