using PrimeTween;
using UnityEngine;

namespace NIX.Module.Popup
{
    [CreateAssetMenu(menuName = "NIX/Popup/PopupMoveFade")]
    public class PopupMoveFadeSO : ScriptableObject
    {
        public float Duration;
        public Vector2 OpenedPos;
        public Vector2 ClosedPos;
        public Ease OpenEase;
        public Ease CloseEase;
        public float OpenedAlpha;
        public float ClosedAlpha;
    }
}