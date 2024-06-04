namespace FastACH
{
    internal static class TransactionCodes
    {
        public static List<uint> CreditCodes = new List<uint>() { 22, 32, 42, 52, 23, 33, 43, 53, 48 };
        public static List<uint> DebitCodes = new List<uint>() { 27, 37, 47, 55, 28, 38 };

        public static bool IsCredit(uint code)
        {
            return CreditCodes.Contains(code);
        }

        public static bool IsDebit(uint code)
        {
            return DebitCodes.Contains(code);
        }
    }
}
