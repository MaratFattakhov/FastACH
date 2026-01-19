using FastACH.Records;
using System.Globalization;

namespace FastACH.Builders
{
    public sealed class AchFileBuilder
    {
        private FileHeaderRecord? _header;
        private List<BatchRecord> _batchRecords = new List<BatchRecord>();
        private static DateOnly _fileCreationDate = DateOnly.FromDateTime(DateTime.Now.Date);
        private static TimeOnly _fileCreationTime = TimeOnly.FromDateTime(DateTime.Now.Date);

        public AchFileBuilder With(string ImmediateDestination = "",
            string ImmediateOrigin = "",
            string ImmediateDestinationName = "",
            string ImmediateOriginName = "",
            string ReferenceCode = "",
            char FileIdModifier = 'A')
        {
            _header = new FileHeaderRecord() { ImmediateDestination = ImmediateDestination, ImmediateOrigin = ImmediateOrigin, FileCreationDate = _fileCreationDate, FileCreationTime = _fileCreationTime, ImmediateDestinationName = ImmediateDestinationName, ImmediateOriginName = ImmediateOriginName, ReferenceCode = ReferenceCode, FileIdModifier = FileIdModifier };
            return this;
        }


        public AchFileBuilder WithBatch(Action<BatchRecordBuilder> configureBatch)
        {
            var batchBuilder = new BatchRecordBuilder();
            configureBatch(batchBuilder);
            _batchRecords.Add(batchBuilder.Build());
            return this;
        }

        public AchFile Build()
        {
            return new AchFile
            {
                FileHeader = _header!,
                BatchRecordList = _batchRecords
            };
        }

        public class BatchRecordBuilder
        {
            private BatchHeaderRecord? _header;
            private List<TransactionRecord> _transactions = new List<TransactionRecord>();

            public BatchRecordBuilder With(string CompanyId = "", string OriginatingDFIID = "", string CompanyEntryDescription = "", string CompanyName = "", uint ServiceClassCode = 1, string entryClassCode = "PPD", string CompanyDiscretionaryData = "",
                DateOnly? CompanyDescriptiveDate = null,
                DateOnly? EffectiveEntryDate = null,
                char OriginatorsStatusCode = '1',
                ulong BatchNumber = 0)
            {
                _header = new BatchHeaderRecord() { CompanyId = CompanyId, OriginatingDFIID = OriginatingDFIID, CompanyEntryDescription = CompanyEntryDescription, CompanyName = CompanyName, ServiceClassCode = ServiceClassCode, StandardEntryClassCode = entryClassCode, CompanyDiscretionaryData = CompanyDiscretionaryData, CompanyDescriptiveDate = CompanyDescriptiveDate, EffectiveEntryDate = EffectiveEntryDate, OriginatorsStatusCode = OriginatorsStatusCode, BatchNumber = BatchNumber };
                return this;
            }

            public BatchRecordBuilder WithDebitTransaction(decimal amount, string routingNumber, string accountNumber, string receiverName = "", string receiverId = "", string discretionaryData = "")
            {
                var entryDetail = new EntryDetailRecord()
                {
                    Amount = amount,
                    AddendaRecordIndicator = false,
                    CheckDigit = routingNumber.Last(),
                    DFIAccountNumber = accountNumber,
                    ReceiverIdentificationNumber = receiverId,
                    ReceiverName = receiverName,
                    ReceivingDFIID = ulong.Parse(routingNumber.PadLeft(9).AsSpan().Slice(0, 8), CultureInfo.InvariantCulture),
                    TransactionCode = 27,
                    DiscretionaryData = discretionaryData
                };

                _transactions.Add(new TransactionRecord() { EntryDetail = entryDetail });
                return this;
            }

            public BatchRecordBuilder WithCreditTransaction(decimal amount, string routingNumber, string accountNumber, string receiverName = "", string receiverId = "", string discretionaryData = "")
            {
                var entryDetail = new EntryDetailRecord()
                {
                    Amount = amount,
                    AddendaRecordIndicator = false,
                    CheckDigit = routingNumber.Last(),
                    DFIAccountNumber = accountNumber,
                    ReceiverIdentificationNumber = receiverId,
                    ReceiverName = receiverName,
                    ReceivingDFIID = ulong.Parse(routingNumber.PadLeft(9).AsSpan().Slice(0, 8), CultureInfo.InvariantCulture),
                    TransactionCode = 22,
                    DiscretionaryData = discretionaryData
                };

                _transactions.Add(new TransactionRecord() { EntryDetail = entryDetail });
                return this;
            }

            public BatchRecordBuilder WithAddenda(uint addendaTypeCode, string addendaInformation = "", uint addendaSequenceNumber = 1)
            {
                if (_transactions.Count == 0)
                    throw new InvalidOperationException("Cannot add addenda without a transaction. Add a transaction first.");

                var lastTransaction = _transactions[_transactions.Count - 1];

                if (lastTransaction.AddendaRecords == null)
                    lastTransaction.AddendaRecords = new List<AddendaRecord>();

                lastTransaction.EntryDetail.AddendaRecordIndicator = true;

                var addenda = new AddendaRecord()
                {
                    AddendaTypeCode = addendaTypeCode,
                    AddendaInformation = addendaInformation,
                    AddendaSequenceNumber = addendaSequenceNumber
                };
                lastTransaction.AddendaRecords.Add(addenda);

                return this;
            }

            public BatchRecord Build()
            {
                return new BatchRecord
                {
                    BatchHeader = _header ??
                        new BatchHeaderRecord()
                        {
                            CompanyId = "",
                            OriginatingDFIID = "",
                            CompanyEntryDescription = "",
                            CompanyName = "",
                            ServiceClassCode = 200,
                        },
                    TransactionRecords = _transactions
                };
            }
        }
    }
}
