using UnityEngine;

namespace NIX.Packages
{
    public class GalleryElement : MonoBehaviour
    {
        [SerializeField] protected Transform _Content;
        [SerializeField] protected CanvasGroup _CanvasGroup;
        [SerializeField] protected RectTransform _RectTransform;
        
        public RectTransform Rt => _RectTransform;

        public void SetRatio(Vector2 originSize, float ratio)
        {
            if (_CanvasGroup != null)
                _CanvasGroup.alpha = ratio;
            
            _Content.localScale = Vector3.one * ratio;
            
            Vector2 parentSize = Vector2.zero;
            if (Rt.parent is RectTransform parent)
                parentSize = parent.rect.size;

            Vector2 anchorSpan = Rt.anchorMax - Rt.anchorMin;
            Vector2 sizeDelta = originSize * ratio - Vector2.Scale(parentSize, anchorSpan);

            Rt.sizeDelta = sizeDelta;
        }

        public void SetAxisPosition(float value, int axis)
        {
            Vector2 position = _RectTransform.anchoredPosition;
            if(axis == 0) position.x = value;
            else position.y = value;
            _RectTransform.anchoredPosition = position;
        }
    }
}