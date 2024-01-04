using ACH_Transform.ACHFileProcessor.Implementations;
using ACH_Transform.ACHFileProcessor.Interfaces;
using ACH_Transform.ACHFileProcessor.Models;
using System;
using System.IO;
using System.Linq;

namespace ACH_Transform.ACHFileProcessor.Services
{
    public class AchFileService : IAchFileService
    {
        private Stream _fileStream;
        private ACHFile _achFile = new();

        /// <summary>
        /// Breaks file into objects, based on the NACHA file specification.
        /// </summary>
        private void ParseFile()
        {
            ACHRecordType5 currentBatch = new();

            using (StreamReader reader = new StreamReader(_fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    switch (line.Substring(0, 1))
                    {
                        case "1":
                            OneRecordParser oneRecordParser = new();
                            _achFile.OneRecord = (ACHRecordType1)oneRecordParser.ParseRecord(line);
                            break;

                        case "5":
                            FiveRecordParser fiveRecordParser = new();
                            currentBatch = (ACHRecordType5)fiveRecordParser.ParseRecord(line);
                            break;

                        case "6":
                            SixRecordParser sixRecordParser = new();
                            currentBatch.SixRecordList.Add((ACHRecordType6)sixRecordParser.ParseRecord(line));
                            break;

                        case "7":
                            SevenRecordParser sevenRecordParser = new();
                            currentBatch.SixRecordList.Last().AddendaRecord = (ACHRecordType7)sevenRecordParser.ParseRecord(line);
                            break;

                        case "8":
                            EightRecordParser eightRecordParser = new();
                            currentBatch.EightRecord = (ACHRecordType8)eightRecordParser.ParseRecord(line);
                            _achFile.BatchRecordList.Add(currentBatch);
                            currentBatch = new();
                            break;

                        case "9":
                            if (!line.StartsWith("999999999999999999999999"))
                            {
                                NineRecordParser nineRecordParser = new();
                                _achFile.NineRecord = (ACHRecordType9)nineRecordParser.ParseRecord(line);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public ACHFile GetACHFile()
        {
            return _achFile;
        }

        public void ProcessFile(string filePath)
        {
            ReadFile(filePath);
            ParseFile();
        }

        /// <summary>
        /// Reads the file from the file system and copies it into a MemoryStream.
        /// </summary>
        /// <param name="filePath">Path to the ach file.</param>
        private void ReadFile(string filePath)
        {
            try
            {
                // Open the file in FileMode.Open, FileAccess.Read, and FileShare.ReadWrite (optional).
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Create a MemoryStream and copy the content of the file into it.
                    MemoryStream memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);

                    // Reset the MemoryStream position to the beginning.
                    memoryStream.Position = 0;

                    _fileStream = memoryStream;
                    // Return the MemoryStream, which now contains the file data.
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file reading.
                Console.WriteLine($"Error reading the file: {ex.Message}");
                throw;
            }
        }

        public void WriteFile(string filePath)
        {
            _achFile.SaveFileToDisk(filePath);
        }

        public void WriteFileToS3(string key)
        {
            _achFile.SaveFileToS3(key);
        }

        public void WriteToConsole()
        {
            _achFile.OutputFileToConsole(); 
            Console.ResetColor();
        }
    }
}
