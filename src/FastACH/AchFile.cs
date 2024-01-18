using FastACH.Models;

namespace FastACH
{
    public class AchFile
    {
        public OneRecord OneRecord = new();
        public List<FiveRecord> BatchRecordList = new();
        public NineRecord NineRecord = new();

        /// <summary>
        /// Recalculates Nine record totals, usually used as you want to write the file somewhere.
        /// </summary>
        public void RecalculateTotals(
            Func<ulong> batchNumberGenerator,
            Func<string> traceNumberGenerator)
        {
            foreach (var batchRecord in BatchRecordList)
            {
                batchRecord.RecalculateTotals(batchNumberGenerator, traceNumberGenerator);
            }

            var itemsCount = BatchRecordList.SelectMany(x => 
                x.SixRecordList.Select(sixRecord => sixRecord.AddendaRecordIndicator ? 2 : 1)).Sum()
                + BatchRecordList.Count * 2;

            NineRecord.BatchCount = (uint)BatchRecordList.Count;
            NineRecord.BlockCount = (uint)Math.Ceiling(itemsCount / 10.0);
            NineRecord.EntryAddendaCount = (uint)BatchRecordList.Select(x => x.SixRecordList.Count()).Sum();
            NineRecord.EntryHash = BatchRecordList
                .Select(p => p.EightRecord.EntryHash)
                .Aggregate((ulong)0, (a, b) => a + b);
            NineRecord.TotalCreditEntryDollarAmount = BatchRecordList
                .SelectMany(x => x.SixRecordList.Where(y => TransactionCodes.IsCredit(y.TransactionCode)).Select(x => x.Amount)).Sum();
            NineRecord.TotalDebitEntryDollarAmount = BatchRecordList
                .SelectMany(x => x.SixRecordList.Where(y => TransactionCodes.IsDebit(y.TransactionCode)).Select(x => x.Amount)).Sum();
        }
    }
}
