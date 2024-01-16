namespace FastACH
{
    internal static class TransactionCodes
    {
        public static List<uint> CreditCodes = new List<uint>() { 22, 32, 42 };
        public static List<uint> DebitCodes = new List<uint>() { 27, 37, 47 };

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
