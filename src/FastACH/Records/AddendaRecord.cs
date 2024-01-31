using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// Addenda Record (7 record)
    /// </summary>
    public class AddendaRecord : IRecord
    {
        // Position 1-1: Record Type Code (numeric)
        public string RecordTypeCode => "7";

        /// <summary>
        ///     Position 2-3: Addenda Type Code (numeric)
        ///     02 – Used for the POS, MTE and SHR standard entry classes.The addenda information is used for terminal location information.
        ///     05 – Used for CCD, CTX, and PPD standard entry classes.The Addenda information contains additional payment related information.
        ///     98 – Used for notification of Change entries. The addenda record contains the correct information.
        ///     99 – Used for Return Entries
        /// </summary>
        public uint AddendaTypeCode { get; set; } = 05;

        // Position 4-83: Addenda Information (alpha-numeric)
        public required string AddendaInformation { get; set; }

        // Position 84-87: Addenda Sequence Number (numeric)
        public uint AddendaSequenceNumber { get; set; }

        // Position 88-94: Entry Detail Sequence Number (numeric)
        public ulong EntryDetailSequenceNumber { get; set; }

        public AddendaRecord()
        {
        }

        [SetsRequiredMembers]
        internal AddendaRecord(ReadOnlySpan<char> data)
        {
            if (data.Length != 94)
            {
                throw new ArgumentException($"Invalid Addenda Record (7 record) length: Expected 94, Actual {data.Length}");
            }

            AddendaInformation = data.Slice(3, 80).Trim().ToString();
            AddendaSequenceNumber = uint.Parse(data.Slice(83, 4));
            EntryDetailSequenceNumber = ulong.Parse(data.Slice(87, 7));
        }

        public void Write(ILineWriter writer)
        {
            writer.Write(RecordTypeCode, 1);
            writer.Write(AddendaTypeCode, 2);
            writer.Write(AddendaInformation, 80);
            writer.Write(AddendaSequenceNumber, 4);
            writer.Write(EntryDetailSequenceNumber, 7);
        }
    }
}
