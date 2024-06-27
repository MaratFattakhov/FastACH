using FastACH.Records;
using System.Globalization;

namespace FastACH.Tests.Buidler
{
    internal sealed class AchFileBuilder
    {
        private FileHeaderRecord? _header;
        private List<BatchRecord> _batchRecords = new List<BatchRecord>();
        private static DateOnly FileCreationDate = DateOnly.FromDateTime(DateTime.Now.Date);
        private static TimeOnly FileCreationTime = TimeOnly.FromDateTime(DateTime.Now.Date);

        public AchFileBuilder With(string ImmediateDestination = "",
            string ImmediateOrigin = "",
            string ImmediateDestinationName = "",
            string ImmediateOriginName = "",
            string ReferenceCode = "")
        {
            _header = new FileHeaderRecord() { ImmediateDestination = ImmediateDestination, ImmediateOrigin = ImmediateOrigin, FileCreationDate = FileCreationDate, FileCreationTime = FileCreationTime, ImmediateDestinationName = ImmediateDestinationName, ImmediateOriginName = ImmediateOriginName, ReferenceCode = ReferenceCode };
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

            public BatchRecordBuilder With(string CompanyId = "", string OriginatingDFIID = "", string CompanyEntryDescription = "", string CompanyName = "", uint ServiceClassCode = 1, string entryClassCode = "PPD")
            {
                _header = new BatchHeaderRecord() { CompanyId = CompanyId, OriginatingDFIID = OriginatingDFIID, CompanyEntryDescription = CompanyEntryDescription, CompanyName = CompanyName, ServiceClassCode = ServiceClassCode, StandardEntryClassCode = entryClassCode };
                return this;
            }

            public BatchRecordBuilder WithDebitTransaction(decimal amount, string routingNumber, string accountNumber, string receiverName = "", string receiverId = "")
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
                };

                _transactions.Add(new TransactionRecord() { EntryDetail = entryDetail });
                return this;
            }

            public BatchRecordBuilder WithCreditTransaction(decimal amount, string routingNumber, string accountNumber, string receiverName = "", string receiverId = "")
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
                    TransactionCode = 22
                };

                _transactions.Add(new TransactionRecord() { EntryDetail = entryDetail });
                return this;
            }

            public BatchRecord Build()
            {
                return new BatchRecord
                {
                    BatchHeader = _header!,
                    TransactionRecords = _transactions
                };
            }
        }
    }
}
