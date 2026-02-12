namespace NIX.Core.Extend
{
    public static class ArrayExtend
    {
        public static T GetSafe<T>(this T[] array, int index)
        {
            if (array == null || index < 0 || index >= array.Length)
                return default;
            return array[index];
        }
    }
}