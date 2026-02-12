using UnityEngine;

namespace NIX.Core.UI
{
    public abstract class BaseAlphaUIVisual : MonoBehaviour
    {
        public virtual float Alpha { get; }

        public abstract void SetAlpha(float alpha);
    }
}