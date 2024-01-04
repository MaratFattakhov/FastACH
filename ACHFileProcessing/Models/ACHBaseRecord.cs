using System.Collections.Generic;

namespace ACH_Transform.ACHFileProcessor.Models
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
