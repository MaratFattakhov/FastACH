using FastACH.Models;

namespace FastACH.Interfaces
{
    public interface IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data);
    }
}
