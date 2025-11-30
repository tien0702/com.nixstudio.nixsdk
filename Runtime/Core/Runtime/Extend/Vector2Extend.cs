using UnityEngine;

namespace NIX.Core.Extend
{
    public static class Vector2Extend
    {
        public static float RandValue(this Vector2 vector)
        {
            return UnityEngine.Random.Range(vector.x, vector.y);
        }
    }
}