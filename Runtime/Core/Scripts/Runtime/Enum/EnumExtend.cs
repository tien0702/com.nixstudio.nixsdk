using System;

namespace NIX.Core
{
    public static class EnumExtend
    {
        public static T Next<T>(this T src) where T : System.Enum
        {
            T[] values = (T[])System.Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, src) + 1;
            return index == values.Length ? values[0] : values[index];
        }

        public static T Prev<T>(this T src) where T : System.Enum
        {
            T[] values = (T[])System.Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, src) - 1;
            return index < 0 ? values[^1] : values[index];
        }
    }
}