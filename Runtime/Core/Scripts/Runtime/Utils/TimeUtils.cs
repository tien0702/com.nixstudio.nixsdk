using System;

namespace NIX.Core.Utils
{
    public enum TimeFormat
    {
        // Seconds only
        SS,
        ss,

        // Minutes:Seconds
        MM_SS,
        mm_ss,

        // Hours:Minutes:Seconds
        HH_MM_SS,
        hh_mm_ss,

        // Days:Hours:Minutes:Seconds
        DD_HH_MM_SS,
        dd_hh_mm_ss
    }

    public static class TimeUtils
    {
        public static string GetTimeUtcNowString()
        {
            DateTime nowUtc = DateTime.UtcNow;
            string isoUtc = nowUtc.ToString("o");
            return isoUtc;
        }

        public static string FormatTime(long totalSeconds, TimeFormat format)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

            switch (format)
            {
                case TimeFormat.SS:
                    return $"{time.Seconds:D2}";
                case TimeFormat.ss:
                    return $"{time.Seconds:d2}";

                case TimeFormat.MM_SS:
                    return $"{time.Minutes:D2}:{time.Seconds:D2}";
                case TimeFormat.mm_ss:
                    return $"{time.Minutes:d2}:{time.Seconds:d2}";

                case TimeFormat.HH_MM_SS:
                    return string.Format("{0:D2}:{1:D2}:{2:D2}",
                        (int)time.TotalHours, time.Minutes, time.Seconds);
                case TimeFormat.hh_mm_ss:
                    return string.Format("{0:d2}:{1:d2}:{2:d2}",
                        (int)time.TotalHours, time.Minutes, time.Seconds);
                case TimeFormat.DD_HH_MM_SS:
                    return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                        time.Days, time.Hours, time.Minutes, time.Seconds);
                case TimeFormat.dd_hh_mm_ss:
                    return string.Format("{0:d2}:{1:d2}:{2:d2}:{3:d2}",
                        time.Days, time.Hours, time.Minutes, time.Seconds);
                default:
                    return totalSeconds.ToString();
            }
        }

        public static DateTime TryParseIso8601DateTime(string isoUtc)
        {
            // TryParseExact with RoundtripKind để giữ DateTimeKind = Utc
            if (DateTime.TryParseExact(isoUtc, "o",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out DateTime parsedUtc))
            {
                return parsedUtc;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public static long GetElapsedSeconds(string dateTimeStr)
        {
            DateTime nowUtc = DateTime.UtcNow;
            DateTime utcParsed = TryParseIso8601DateTime(dateTimeStr);
            if (utcParsed == DateTime.MinValue)
            {
                throw new Exception("Invalid date time format");
            }

            return (long)(nowUtc - utcParsed).TotalSeconds;
        }
    }
}