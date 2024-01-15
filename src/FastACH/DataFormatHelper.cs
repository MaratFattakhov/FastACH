
namespace FastACH
{
    public static class DataFormatHelper
    {
        public static List<string> CreditCodes = new List<string>() { "22", "32", "42" };
        public static List<string> DebitCodes = new List<string>() { "27", "37", "47" };

        /// <summary>
        /// Formats a string for ACH consumption (length, alignment, special char replacement)
        /// </summary>
        /// <param name="data">The string to be formatted</param>
        /// <param name="maxLength">max length of final value</param>
        /// <param name="padLeft">pads content left or right</param>
        /// <returns></returns>
        public static string FormatForAch(object data, int maxLength = 0, bool padLeft = false)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var stringValue = data.ToString() ?? string.Empty;

            if (data is decimal)
            {
                stringValue = stringValue
                    .Replace("$", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace(",", string.Empty);

                return maxLength > 0 ? GetDisplayValue(stringValue, true, maxLength, '0') : stringValue.Length > maxLength ? stringValue.Substring(0, maxLength) : stringValue;
            }
            else if (data is uint || data is int || data is long || data is ulong)
                return maxLength > 0 ? GetDisplayValue(data, true, maxLength, '0') : stringValue.Length > maxLength ? stringValue.Substring(0, maxLength) : stringValue;

            return maxLength > 0 ? GetDisplayValue(data, padLeft, maxLength, ' ') : stringValue.Length > maxLength ? stringValue.Substring(0, maxLength) : stringValue;
        }

        private static string GetDisplayValue(object value, bool padLeft, int maxLength, char filler) =>
            padLeft ? value.ToString().PadLeft(maxLength, filler) : value.ToString().PadRight(maxLength, filler);

        /// <summary>
        /// Parses decimal from ach file (format is xxxxxxxxxx, where decimal is xxxxxxxx.xx)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static decimal ParseDecimal(string data)
        {
            decimal value;
            string trimValue = data.TrimStart('0');
            string formattedValue = string.Empty;

            if (trimValue.Length < 2)
            {
                formattedValue = $"0.{trimValue.PadLeft(2, '0')}";
            }
            else
            {
                formattedValue = $"{trimValue.Substring(0, trimValue.Length - 2)}.{trimValue.Substring(trimValue.Length - 2)}";
            }

            if (decimal.TryParse(formattedValue, out value))
                return value;

            return 0;
        }

        public static int ParseInt(string data)
        {
            int value;
            string trimValue = data.TrimStart('0');

            if (int.TryParse(trimValue, out value))
                return value;

            return 0;
        }

        public static ulong ParseUlong(string data)
        {
            ulong value;
            string trimValue = data.TrimStart('0');

            if (ulong.TryParse(trimValue, out value))
                return value;

            return 0;
        }

        internal static string CleanupLastBaiDataElementOnLine(string value)
        {
            if (value.Contains("/"))
                value = value.Replace("/", string.Empty);

            return value;
        }
    }
}
