using FastACH.Records;

namespace FastACH
{
    /// <summary>
    /// Provides extension methods for ACH file operations.
    /// </summary>
    public static class AchFileExtensions
    {
        /// <summary>
        /// Determines whether the ACH file is balanced (total credits equal total debits for all batches).
        /// </summary>
        /// <param name="achFile">The ACH file to check for balance.</param>
        /// <returns>True if all batches are balanced; otherwise, false.</returns>
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

        /// <summary>
        /// Gets the complete 9-digit routing number (8-digit DFI ID + check digit) for an entry detail record.
        /// </summary>
        /// <param name="record">The entry detail record.</param>
        /// <returns>The complete 9-digit routing number as a string.</returns>
        public static string GetRoutingNumber(this EntryDetailRecord record)
        {
            var routingNumberString = record.ReceivingDFIID.ToString().PadLeft(8, '0');
            return routingNumberString + record.CheckDigit;
        }

        private static decimal GetTotalCreditEntryDollarAmount(BatchRecord batch) => batch.TransactionRecords.Where(x => TransactionCodes.IsCredit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero));
        private static decimal GetTotalDebitEntryDollarAmount(BatchRecord batch) => batch.TransactionRecords.Where(x => TransactionCodes.IsDebit(x.EntryDetail.TransactionCode)).Sum(x => Math.Round(x.EntryDetail.Amount, 2, MidpointRounding.AwayFromZero));
    }
}
