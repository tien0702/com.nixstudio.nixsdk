using System.Collections.Generic;

namespace NIX.Core.Extend
{
    public static class ListExtend
    {
        public static T GetRandom<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}