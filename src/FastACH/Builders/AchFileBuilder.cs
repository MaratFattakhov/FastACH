using FastACH.Records;
using System.Globalization;

namespace FastACH.Builders
{
    /// <summary>
    /// Provides a fluent builder pattern for constructing ACH files.
    /// </summary>
    public sealed class AchFileBuilder
    {
        private FileHeaderRecord? _header;
        private List<BatchRecord> _batchRecords = new List<BatchRecord>();
        private static DateOnly _fileCreationDate = DateOnly.FromDateTime(DateTime.Now.Date);
        private static TimeOnly _fileCreationTime = TimeOnly.FromDateTime(DateTime.Now.Date);

        /// <summary>
        /// Configures the file header record with the specified values.
        /// </summary>
        /// <param name="ImmediateDestination">The immediate destination (routing number).</param>
        /// <param name="ImmediateOrigin">The immediate origin (company ID).</param>
        /// <param name="ImmediateDestinationName">The name of the immediate destination.</param>
        /// <param name="ImmediateOriginName">The name of the immediate origin.</param>
        /// <param name="ReferenceCode">Optional reference code.</param>
        /// <param name="FileIdModifier">File ID modifier character (default 'A').</param>
        /// <returns>The current AchFileBuilder instance for method chaining.</returns>
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


        /// <summary>
        /// Adds a batch to the ACH file by configuring it through the provided action.
        /// </summary>
        /// <param name="configureBatch">Action to configure the batch using a BatchRecordBuilder.</param>
        /// <returns>The current AchFileBuilder instance for method chaining.</returns>
        public AchFileBuilder WithBatch(Action<BatchRecordBuilder> configureBatch)
        {
            var batchBuilder = new BatchRecordBuilder();
            configureBatch(batchBuilder);
            _batchRecords.Add(batchBuilder.Build());
            return this;
        }

        /// <summary>
        /// Builds the ACH file with all configured headers and batches.
        /// </summary>
        /// <returns>A new AchFile instance.</returns>
        public AchFile Build()
        {
            return new AchFile
            {
                FileHeader = _header!,
                BatchRecordList = _batchRecords
            };
        }

        /// <summary>
        /// Provides a fluent builder pattern for constructing batch records.
        /// </summary>
        public class BatchRecordBuilder
        {
            private BatchHeaderRecord? _header;
            private List<TransactionRecord> _transactions = new List<TransactionRecord>();

            /// <summary>
            /// Configures the batch header record with the specified values.
            /// </summary>
            /// <param name="CompanyId">Company identification number.</param>
            /// <param name="OriginatingDFIID">Originating DFI identification number.</param>
            /// <param name="CompanyEntryDescription">Description of the batch entries.</param>
            /// <param name="CompanyName">Name of the company.</param>
            /// <param name="ServiceClassCode">Service class code (200=mixed, 220=credits, 225=debits).</param>
            /// <param name="entryClassCode">Standard entry class code (e.g., "PPD", "CCD").</param>
            /// <param name="CompanyDiscretionaryData">Optional company discretionary data.</param>
            /// <param name="CompanyDescriptiveDate">Optional company descriptive date.</param>
            /// <param name="EffectiveEntryDate">Optional effective entry date.</param>
            /// <param name="OriginatorsStatusCode">Originator's status code (default '1').</param>
            /// <param name="BatchNumber">Batch number (default 0).</param>
            /// <returns>The current BatchRecordBuilder instance for method chaining.</returns>
            public BatchRecordBuilder With(string CompanyId = "", string OriginatingDFIID = "", string CompanyEntryDescription = "", string CompanyName = "", uint ServiceClassCode = 1, string entryClassCode = "PPD", string CompanyDiscretionaryData = "",
                DateOnly? CompanyDescriptiveDate = null,
                DateOnly? EffectiveEntryDate = null,
                char OriginatorsStatusCode = '1',
                ulong BatchNumber = 0)
            {
                _header = new BatchHeaderRecord() { CompanyId = CompanyId, OriginatingDFIID = OriginatingDFIID, CompanyEntryDescription = CompanyEntryDescription, CompanyName = CompanyName, ServiceClassCode = ServiceClassCode, StandardEntryClassCode = entryClassCode, CompanyDiscretionaryData = CompanyDiscretionaryData, CompanyDescriptiveDate = CompanyDescriptiveDate, EffectiveEntryDate = EffectiveEntryDate, OriginatorsStatusCode = OriginatorsStatusCode, BatchNumber = BatchNumber };
                return this;
            }

            /// <summary>
            /// Adds a debit transaction to the batch.
            /// </summary>
            /// <param name="amount">The transaction amount.</param>
            /// <param name="routingNumber">The receiving bank's routing number.</param>
            /// <param name="accountNumber">The receiving account number.</param>
            /// <param name="receiverName">The name of the receiver.</param>
            /// <param name="receiverId">The receiver's identification number.</param>
            /// <param name="discretionaryData">Optional discretionary data.</param>
            /// <returns>The current BatchRecordBuilder instance for method chaining.</returns>
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

            /// <summary>
            /// Adds a credit transaction to the batch.
            /// </summary>
            /// <param name="amount">The transaction amount.</param>
            /// <param name="routingNumber">The receiving bank's routing number.</param>
            /// <param name="accountNumber">The receiving account number.</param>
            /// <param name="receiverName">The name of the receiver.</param>
            /// <param name="receiverId">The receiver's identification number.</param>
            /// <param name="discretionaryData">Optional discretionary data.</param>
            /// <returns>The current BatchRecordBuilder instance for method chaining.</returns>
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

            /// <summary>
            /// Adds an addenda record to the most recently added transaction.
            /// </summary>
            /// <param name="addendaTypeCode">The addenda type code.</param>
            /// <param name="addendaInformation">The addenda information content.</param>
            /// <param name="addendaSequenceNumber">The addenda sequence number.</param>
            /// <returns>The current BatchRecordBuilder instance for method chaining.</returns>
            /// <exception cref="InvalidOperationException">Thrown when no transaction exists to add the addenda to.</exception>
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

            /// <summary>
            /// Builds the batch record with all configured headers and transactions.
            /// </summary>
            /// <returns>A new BatchRecord instance.</returns>
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
