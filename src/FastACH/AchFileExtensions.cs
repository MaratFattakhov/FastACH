using FastACH.Records;

namespace FastACH
{
    public static class AchFileExtensions
    {
        public static bool IsBalanced(this AchFile achFile)
        {
            foreach (var batch in achFile.BatchRecordList)
            {
                if (GetTotalCreditEntryDollarAmount(batch) != GetTotalDebitEntryDollarAmount(batch))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetRoutingNumber(this EntryDetailRecord record)
        {
            var routingNumberString = record.ReceivingDFIID.ToString().PadLeft(8, '0');
            return routingNumberString + record.CheckDigit;
        }

        private static decimal GetTotalCreditEntryDollarAmount(BatchRecord batch) => batch.TransactionRecords.Where(x => TransactionCodes.IsCredit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero));
        private static decimal GetTotalDebitEntryDollarAmount(BatchRecord batch) => batch.TransactionRecords.Where(x => TransactionCodes.IsDebit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero));
    }
}
