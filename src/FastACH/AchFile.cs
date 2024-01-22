using FastACH.Records;

namespace FastACH
{
    public class AchFile
    {
        public FileHeaderRecord OneRecord = new();
        public List<BatchRecord> BatchRecordList = new();
        public FileControlRecord NineRecord = new();

        /// <summary>
        /// Recalculates Nine record totals, usually used as you want to write the file somewhere.
        /// </summary>
        public void RecalculateTotals(
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator)
        {
            var adendaSequenceCounter = 0;
            var adendaSequenceNumberGenerator = new Func<uint>(() => (uint)++adendaSequenceCounter);

            foreach (var batchRecord in BatchRecordList)
            {
                RecalculateTotals(batchRecord, batchNumberGenerator, traceNumberGenerator, adendaSequenceNumberGenerator);
            }

            var itemsCount = BatchRecordList.SelectMany(x =>
                x.TransactionDetailsList.Select(p => p.EntryDetail.AddendaRecordIndicator ? 2 : 1)).Sum()
                + BatchRecordList.Count * 2;

            NineRecord.BatchCount = (uint)BatchRecordList.Count;
            NineRecord.BlockCount = (uint)Math.Ceiling(itemsCount / 10.0);
            NineRecord.EntryAddendaCount = (uint)BatchRecordList.Select(x => x.TransactionDetailsList.Count()).Sum();
            NineRecord.EntryHash = BatchRecordList
                .Select(p => p.BatchControl.EntryHash)
                .Aggregate((ulong)0, (a, b) => a + b);
            NineRecord.TotalCreditEntryDollarAmount = BatchRecordList.Sum(p => p.BatchControl.TotalCreditEntryDollarAmount);
            NineRecord.TotalDebitEntryDollarAmount = BatchRecordList.Sum(p => p.BatchControl.TotalDebitEntryDollarAmount);
        }

        public void RecalculateTotals(
            BatchRecord batch,
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator,
            Func<uint> adendaSequenceNumberGenerator)
        {
            UpdateBatchNumbers(batch, batchNumberGenerator);
            UpdateTraceNumbers(batch, traceNumberGenerator);
            UpdateAdendaSequenceCounters(batch, adendaSequenceNumberGenerator);
            batch.BatchControl.EntryAddendaCount = (uint)batch.TransactionDetailsList.Count + (uint)batch.TransactionDetailsList.Where(x => x.Addenda != null).Count();
            batch.BatchControl.EntryHash = batch.TransactionDetailsList
                .Select(p => p.EntryDetail.ReceivingDFIID)
                .Aggregate((ulong)0, (a, b) => a + b);
            batch.BatchControl.TotalCreditEntryDollarAmount = batch.TransactionDetailsList.Where(x => TransactionCodes.IsCredit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero));
            batch.BatchControl.TotalDebitEntryDollarAmount = batch.TransactionDetailsList.Where(x => TransactionCodes.IsDebit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero));
            batch.BatchControl.CompanyIdentification = batch.BatchHeader.CompanyId ?? string.Empty;
            batch.BatchControl.OriginatingDFINumber = batch.BatchHeader.OriginatingDFIID ?? string.Empty;
        }

        public void UpdateAdendaSequenceCounters(BatchRecord batch, Func<uint> adendaSequenceNumberGenerator)
        {
            foreach (var transactionDetails in batch.TransactionDetailsList)
            {

                if (transactionDetails.EntryDetail.AddendaRecordIndicator && transactionDetails.Addenda != null)
                {
                    var counter = adendaSequenceNumberGenerator();
                    transactionDetails.Addenda.AddendaSequenceNumber = counter;
                }
            }
        }

        public void UpdateBatchNumbers(BatchRecord batch, Func<ulong> batchNumberGenerator)
        {
            var batchNumber = batchNumberGenerator();
            batch.BatchHeader.BatchNumber = batchNumber;
            batch.BatchControl.BatchNumber = batchNumber;
        }

        public void UpdateTraceNumbers(BatchRecord batch, Func<string> traceNumberGenerator)
        {
            foreach (var transactionDetails in batch.TransactionDetailsList)
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
