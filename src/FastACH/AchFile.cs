using FastACH.Records;

namespace FastACH
{
    public class AchFile
    {
        public AchFile()
        {
        }

        public required FileHeaderRecord FileHeader { get; set; }
        public List<BatchRecord> BatchRecordList { get; set; } = new List<BatchRecord>();
        public FileControlRecord FileControl { get; set; } = FileControlRecord.Empty;

        /// <summary>
        /// Recalculates Nine record totals, usually used as you want to write the file somewhere.
        /// </summary>
        public void RecalculateTotals(
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator)
        {
            foreach (var batchRecord in BatchRecordList)
            {
                RecalculateTotals(batchRecord, batchNumberGenerator, traceNumberGenerator);
            }

            var itemsCount = BatchRecordList.SelectMany(x =>
                x.TransactionRecords.Select(p => p.Addenda is not null ? 2 : 1)).Sum()
                + BatchRecordList.Count * 2 // 2 for batch header and batch control
                + 2; // 2 for file header and file control

            FileControl.BatchCount = (uint)BatchRecordList.Count;
            FileControl.BlockCount = (uint)Math.Ceiling(itemsCount / 10.0);
            FileControl.EntryAddendaCount = (uint)BatchRecordList.Select(x => (int)x.BatchControl.EntryAddendaCount).Sum();
            FileControl.EntryHash = BatchRecordList
                .Select(p => p.BatchControl.EntryHash)
                .Aggregate((ulong)0, (a, b) => a + b);
            FileControl.TotalCreditEntryDollarAmount = BatchRecordList.Sum(p => p.BatchControl.TotalCreditEntryDollarAmount);
            FileControl.TotalDebitEntryDollarAmount = BatchRecordList.Sum(p => p.BatchControl.TotalDebitEntryDollarAmount);
        }

        private void RecalculateTotals(
            BatchRecord batch,
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator)
        {
            UpdateBatchNumbers(batch, batchNumberGenerator);
            UpdateTraceNumbers(batch, traceNumberGenerator);
            batch.BatchControl = Create(batch.BatchHeader, batch.TransactionRecords);
        }

        private BatchControlRecord Create(BatchHeaderRecord batchHeader, List<TransactionRecord> transactionDetails)
        {
            return new BatchControlRecord()
            {
                BatchNumber = batchHeader.BatchNumber,
                ServiceClassCode = batchHeader.ServiceClassCode,
                EntryAddendaCount = (uint)transactionDetails.Count + (uint)transactionDetails.Where(x => x.Addenda != null).Count(),
                EntryHash = transactionDetails
                    .Select(p => p.EntryDetail.ReceivingDFIID)
                    .Aggregate((ulong)0, (a, b) => a + b),
                TotalCreditEntryDollarAmount = transactionDetails.Where(x => TransactionCodes.IsCredit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero)),
                TotalDebitEntryDollarAmount = transactionDetails.Where(x => TransactionCodes.IsDebit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero)),
                CompanyIdentification = batchHeader.CompanyId,
                OriginatingDFINumber = batchHeader.OriginatingDFIID,
            };
        }

        private void UpdateBatchNumbers(BatchRecord batch, Func<ulong> batchNumberGenerator)
        {
            var batchNumber = batchNumberGenerator();
            batch.BatchHeader.BatchNumber = batchNumber;
        }

        private void UpdateTraceNumbers(BatchRecord batch, Func<string> traceNumberGenerator)
        {
            foreach (var transactionDetails in batch.TransactionRecords)
            {
                var traceNumber = traceNumberGenerator();
                transactionDetails.EntryDetail.TraceNumber = traceNumber;

                if (transactionDetails.EntryDetail.AddendaRecordIndicator && transactionDetails.Addenda != null)
                {
                    transactionDetails.Addenda.EntryDetailSequenceNumber = ulong.Parse(
                        traceNumber.Substring(traceNumber.Length - 7, 7));
                }
            }
        }
    }
}
