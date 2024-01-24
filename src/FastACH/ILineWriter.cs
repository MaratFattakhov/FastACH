namespace FastACH
{
    public interface ILineWriter
    {
        void Write(string part, byte length);
        void Write(ulong value, byte length);
        void Write(DateOnly? date);
        void Write(TimeOnly? time);
    }
}
