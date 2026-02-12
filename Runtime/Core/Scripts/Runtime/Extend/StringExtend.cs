using System.Text;
using System.Text.RegularExpressions;

namespace NIX.Core.Extend
{
    public static class StringExtend
    {
        public static string ConvertToName(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string s = Regex.Replace(input, @"[.,\-_]+", " ");

            s = Regex.Replace(s, @"(?<=[a-z])([A-Z])", " $1");
            s = Regex.Replace(s, @"\s+", " ").Trim();

            StringBuilder sb = new StringBuilder(s.Length);
            bool capitalizeNext = true;

            foreach (char c in s)
            {
                if (capitalizeNext && char.IsLetter(c))
                {
                    sb.Append(char.ToUpper(c));
                    capitalizeNext = false;
                }
                else
                {
                    sb.Append(char.ToLower(c));
                }

                if (c == ' ')
                    capitalizeNext = true;
            }

            return sb.ToString();
        }
    }
}