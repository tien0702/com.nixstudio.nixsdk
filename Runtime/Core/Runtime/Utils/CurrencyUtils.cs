using System;
using System.Text;

namespace NIX.Core.Utils
{
    public static class CurrencyUtils
    {
        private static readonly string[] SuffixesStandard = { "", "K", "M", "B", "T", "Q" };

        /// <summary>
        /// Format currency with K/M/B... suffixes. Only positive numbers allowed.
        /// </summary>
        public static string FormatStandard(double value, int decimals = 2)
        {
            if (value < 0) return null;
            if (value < 1000d) return value.ToString("N0");

            int idx = 0;
            while (value >= 1000d && idx < SuffixesStandard.Length - 1)
            {
                value /= 1000d;
                idx++;
            }

            string format = "F" + decimals;
            string result = value.ToString(format);

            if (result.Contains('.'))
            {
                result = result.TrimEnd('0').TrimEnd('.');
            }

            return result + SuffixesStandard[idx];
        }

        /// <summary>
        /// Format currency with alphabet suffix: a..z, aa..zz, aaa...
        /// </summary>
        public static string FormatAlphabet(double value, int decimals = 2, bool upperCase = false)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Only positive numbers are allowed.");
            if (value < 1000d) return value.ToString("0." + new string('0', decimals));

            int alphabetIndex = 0;
            while (value >= 1000d)
            {
                value /= 1000d;
                alphabetIndex++;
            }

            return value.ToString("0." + new string('0', decimals)) +
                   GetAlphabetSuffixExcelStyle(alphabetIndex, upperCase);
        }

        private static string GetAlphabetSuffixExcelStyle(int index, bool upperCase)
        {
            if (index <= 0) return "";

            StringBuilder sb = new StringBuilder();
            while (index > 0)
            {
                index--;
                int rem = index % 26;
                char letter = (char)('a' + rem);
                if (upperCase) letter = char.ToUpperInvariant(letter);
                sb.Insert(0, letter);
                index /= 26;
            }

            return sb.ToString();
        }
    }
}