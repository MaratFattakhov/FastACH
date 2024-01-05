namespace FastACH.Models
{
    public abstract class ACHBaseRecord
    {
        public ACHBaseRecord()
        {
        }

        public abstract string WriteAsText();
        public abstract void WriteToConsole();
    }
}
