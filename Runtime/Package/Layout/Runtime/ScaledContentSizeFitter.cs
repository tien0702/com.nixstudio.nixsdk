using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NIX.Packages
{
    [AddComponentMenu("Layout/Content Size Fitter (With Scale)", 142)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitterWithScale : UIBehaviour, ILayoutSelfController
    {
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }

        [SerializeField] protected FitMode _HorizontalFit = FitMode.Unconstrained;
        [SerializeField] protected FitMode _VerticalFit = FitMode.Unconstrained;

        [Tooltip("If true, final size is multiplied by RectTransform.localScale (per axis).")] 
        [SerializeField] protected bool _MultiplyByScale = false;
        [System.NonSerialized] private RectTransform _Rect;

        private RectTransform rectTransform
        {
            get
            {
                if (_Rect == null) _Rect = GetComponent<RectTransform>();
                return _Rect;
            }
        }

        protected ContentSizeFitterWithScale()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        /// <summary>
        /// Calculate and apply the horizontal size.
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        /// Calculate and apply the vertical size.
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            float size;

            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform);
            size = (axis == 0 ? bounds.size.x : bounds.size.y);

            if (size <= 0f)
            {
                var r = rectTransform.rect;
                size = (axis == 0 ? r.width : r.height);
            }

            if (_MultiplyByScale)
            {
                float s = (axis == 0 ? rectTransform.localScale.x : rectTransform.localScale.y);
                if (!Mathf.Approximately(s, 0f)) size *= s;
            }

            SetSizeDeltaOnAxis(rectTransform, axis, size);
        }

        private static void SetSizeDeltaOnAxis(RectTransform rt, int axis, float targetSize)
        {
            Vector2 parentSize = Vector2.zero;
            if (rt.parent is RectTransform parent)
                parentSize = parent.rect.size;

            Vector2 anchorSpan = rt.anchorMax - rt.anchorMin;
            Vector2 sd = rt.sizeDelta;

            if (axis == 0)
                sd.x = targetSize - parentSize.x * anchorSpan.x;
            else
                sd.y = targetSize - parentSize.y * anchorSpan.y;

            rt.sizeDelta = sd;
        }

        protected void SetDirty()
        {
            if (!IsActive()) return;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }

    /// <summary>
    /// Minimal SetPropertyUtility copy for standalone usage.
    /// </summary>
    internal static class SetPropertyUtility
    {
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                currentValue = newValue;
                return true;
            }

            return false;
        }
    }
}