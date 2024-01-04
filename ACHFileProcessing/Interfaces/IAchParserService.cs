using ACH_Transform.ACHFileProcessor.Models;

namespace ACH_Transform.ACHFileProcessor.Interfaces
{
    public interface IAchParserService
    {
        public ACHBaseRecord ParseRecord(string data);
    }
}
