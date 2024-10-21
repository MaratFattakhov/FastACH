using System.Diagnostics.CodeAnalysis;

namespace FastACH.Records
{
    /// <summary>
    /// Addenda Record (7 record)
    /// </summary>
    public record class AddendaRecord : BaseRecord, IRecord
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
        public uint AddendaSequenceNumber { get; set; } = 1;

        // Position 88-94: Entry Detail Sequence Number (numeric)
        public ulong EntryDetailSequenceNumber { get; set; }

        public AddendaRecord()
        {
        }

        [SetsRequiredMembers]
        internal AddendaRecord(ReadOnlySpan<char> data, uint lineNumber)
        {
            var reader = new LineReader(data, 1);
            AddendaTypeCode = reader.ReadUInt(2);
            AddendaInformation = reader.ReadString(80);
            AddendaSequenceNumber = reader.ReadUInt(4);
            EntryDetailSequenceNumber = reader.ReadULong(7);
            LineNumber = lineNumber;
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
