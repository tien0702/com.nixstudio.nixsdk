using System;
using NIX.Core;
using UnityEngine;

namespace NIX.Module.CommonSO
{
    [CreateAssetMenu(menuName = "NIX/SO/IntSO")]
    public class ArrayIntSO : BaseSO
    {
        public int[] Arrays;

        public int GetIndexByValue(int value)
        {
            for (int i = Arrays.Length - 1; i >= 0; i--)
            {
                if(Arrays[i] <= value) return i;
            }

            return 0;
        }

        public int GetValueByIndex(int index)
        {
            return Arrays[Math.Clamp(index, 0, Arrays.Length - 1)];
        }
    }
}