using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    [RequireComponent(typeof(Image))]
    public class RescaleByParent : MonoBehaviour
    {
        public enum Dimension
        {
            Width,
            Height,
            Auto
        }

        [SerializeField] protected Dimension _UseDimension = Dimension.Auto;
        [SerializeField] protected float _Offset;
        [SerializeField] protected bool _FitOnEnable = true;

        protected RectTransform _RectTransform;
        protected RectTransform _ParentRect;

        private void OnEnable()
        {
            if (_FitOnEnable) Fit();
        }

        [ContextMenu("Rescale")]
        public virtual void Fit()
        {
            if (_RectTransform == null) _RectTransform = GetComponent<RectTransform>();
            if (_ParentRect == null) _ParentRect = transform.parent.GetComponent<RectTransform>();
            Canvas.ForceUpdateCanvases();
            _ParentRect.ForceUpdateRectTransforms();
            _RectTransform.ForceUpdateRectTransforms();

            Vector2 size = _RectTransform.sizeDelta;
            float scale = 0f;
            if (_UseDimension == Dimension.Auto)
            {
                scale = size.x > size.y ? GetScaleByWidth() : GetScaleByHeight();
            }
            else
            {
                scale = _UseDimension == Dimension.Width ? GetScaleByWidth() : GetScaleByHeight();
            }

            _RectTransform.localScale = new Vector3(scale, scale, scale) * (1f - _Offset);
        }

        private float GetScaleByWidth()
        {
            Vector2 size = _RectTransform.sizeDelta;
            Vector2 parentSize = _ParentRect.sizeDelta;
            return parentSize.x / size.x;
        }

        private float GetScaleByHeight()
        {
            Vector2 size = _RectTransform.sizeDelta;
            Vector2 parentSize = _ParentRect.sizeDelta;
            return parentSize.y / size.y;
        }
    }
}