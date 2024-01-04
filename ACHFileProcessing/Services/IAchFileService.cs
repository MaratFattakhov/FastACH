using System.IO;

namespace ACH_Transform.ACHFileProcessor.Services
{
    internal interface IAchFileService
    {
        public void ProcessFile(string filePath);
        public void WriteToConsole();
        public void WriteFile(string filePath);
        public void WriteFileToS3(string key);
    }
}
