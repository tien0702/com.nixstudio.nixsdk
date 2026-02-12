using System;
using System.Globalization;

namespace NIX.Core.Utils
{
    namespace NIX.Utils
    {
        public static class NumericUtils
        {
            // —— INT —— //
            public static int ParseInt(string s)
                => int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);

            public static bool TryParseInt(string s, out int result)
                => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

            // —— LONG —— //
            public static long ParseLong(string s)
                => long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);

            public static bool TryParseLong(string s, out long result)
                => long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

            // —— SHORT —— //
            public static short ParseShort(string s)
                => short.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);

            public static bool TryParseShort(string s, out short result)
                => short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

            // —— FLOAT —— //
            public static float ParseFloat(string s)
                => float.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

            public static bool TryParseFloat(string s, out float result)
                => float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                    out result);

            // —— DOUBLE —— //
            public static double ParseDouble(string s)
                => double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

            public static bool TryParseDouble(string s, out double result)
                => double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                    out result);

            // —— DECIMAL —— //
            public static decimal ParseDecimal(string s)
                => decimal.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

            public static bool TryParseDecimal(string s, out decimal result)
                => decimal.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                    out result);

            /// <summary>
            /// Split an integer 'a' into 'n' parts where the sum of all parts equals 'a'.
            /// The parts are as evenly distributed as possible.
            /// </summary>
            public static int[] SplitIntegerEvenly(int a, int n)
            {
                if (n <= 0) throw new ArgumentException("n must be greater than 0.");

                int[] result = new int[n];
                int baseValue = a / n;
                int remainder = a % n;

                for (int i = 0; i < n; i++)
                {
                    result[i] = baseValue + (i < remainder ? 1 : 0);
                }

                return result;
            }

            /// <summary>
            /// Split an integer 'a' into 'n' random parts where the sum of all parts equals 'a'.
            /// </summary>
            public static int[] SplitIntegerRandom(int a, int n, Random rng = null)
            {
                if (n <= 0) throw new ArgumentException("n must be greater than 0.");
                if (a < 0) throw new ArgumentException("a must be non-negative.");

                rng ??= new Random();
                int[] cuts = new int[n - 1];

                // Generate random cut points
                for (int i = 0; i < cuts.Length; i++)
                {
                    cuts[i] = rng.Next(0, a + 1);
                }

                // Sort cut points
                Array.Sort(cuts);

                int[] result = new int[n];
                int prev = 0;
                for (int i = 0; i < cuts.Length; i++)
                {
                    result[i] = cuts[i] - prev;
                    prev = cuts[i];
                }

                result[n - 1] = a - prev;

                return result;
            }
        }
    }
}