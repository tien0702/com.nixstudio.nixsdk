using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    /// <summary>
    /// Base for advanced layout groups that:
    /// - Use RectTransform.rect size by default (can switch to PreferredSize).
    /// - Optionally consider child localScale for spacing/placement.
    /// - Support Auto/Manual rebuild control.
    /// - Can force children to Top-Left anchor preset (and pivot).
    /// - Expose "ResizeChildren" to also set width/height if desired (default: off).
    /// </summary>
    [ExecuteAlways]
    public abstract class AdvancedLayoutGroupBase : LayoutGroup
    {
        public enum UpdateMode
        {
            Auto,
            Manual
        }

        // ----- Config -----
        [Header("Layout")] [SerializeField] protected float _Spacing = 0f;
        [SerializeField] protected bool _ConsiderScale = true;
        [SerializeField] protected UpdateMode _Update = UpdateMode.Auto;

        [Header("Size Source & Resize")]
        [Tooltip(
            "If true, use LayoutUtility.GetPreferredSize (fallback to Min/Rect). If false, use RectTransform.rect size.")]
        [SerializeField]
        protected bool _UsePreferredSize = false;

        [Tooltip("If true, also set child size to the chosen base size. If false, position only.")] [SerializeField]
        protected bool _ResizeChildren = false;

        [Header("Child Anchor Preset")]
        [Tooltip("Force child anchors to Top-Left (anchorMin=(0,1), anchorMax=(0,1)).")]
        [SerializeField]
        protected bool _ForceAnchorsTopLeft = true;

        [Tooltip("Also set child pivot to Top-Left (0,1).")] [SerializeField]
        protected bool _AlsoSetPivotTopLeft = false;

        // Deferred rebuild flag for Manual mode
        protected bool _PendingRebuild;

        // Derived classes specify the primary axis (0 = Horizontal, 1 = Vertical).
        protected abstract int PrimaryAxis { get; }

        #region Public properties (optional)

        public float Spacing
        {
            get => _Spacing;
            set
            {
                if (!Mathf.Approximately(_Spacing, value))
                {
                    _Spacing = value;
                    ConditionalSetDirty();
                }
            }
        }

        public bool ConsiderScale
        {
            get => _ConsiderScale;
            set
            {
                if (_ConsiderScale != value)
                {
                    _ConsiderScale = value;
                    ConditionalSetDirty();
                }
            }
        }

        public UpdateMode Mode
        {
            get => _Update;
            set
            {
                if (_Update != value)
                {
                    _Update = value;
                    ConditionalSetDirty();
                }
            }
        }

        public bool UsePreferredSize
        {
            get => _UsePreferredSize;
            set
            {
                if (_UsePreferredSize != value)
                {
                    _UsePreferredSize = value;
                    ConditionalSetDirty();
                }
            }
        }

        public bool ResizeChildren
        {
            get => _ResizeChildren;
            set
            {
                if (_ResizeChildren != value)
                {
                    _ResizeChildren = value;
                    ConditionalSetDirty();
                }
            }
        }

        public bool ForceAnchorsTopLeft
        {
            get => _ForceAnchorsTopLeft;
            set
            {
                if (_ForceAnchorsTopLeft != value)
                {
                    _ForceAnchorsTopLeft = value;
                    ConditionalSetDirty();
                }
            }
        }

        public bool AlsoSetPivotTopLeft
        {
            get => _AlsoSetPivotTopLeft;
            set
            {
                if (_AlsoSetPivotTopLeft != value)
                {
                    _AlsoSetPivotTopLeft = value;
                    ConditionalSetDirty();
                }
            }
        }

        #endregion

        // ----- External control -----
        public void RebuildNow()
        {
            if (!isActiveAndEnabled) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        public void ScheduleRebuild()
        {
            if (!isActiveAndEnabled) return;
            _PendingRebuild = true;
        }

        // ----- Unity lifecycle / dirty control -----
        protected override void OnEnable()
        {
            base.OnEnable();
            ConditionalSetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ConditionalSetDirty();
        }
#endif

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
            ConditionalSetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            ConditionalSetDirty();
        }

        protected virtual void LateUpdate()
        {
            if (_PendingRebuild)
            {
                _PendingRebuild = false;
                RebuildNow();
            }
        }

        protected void ConditionalSetDirty()
        {
            if (!IsActive()) return;
            if (_Update == UpdateMode.Auto)
                LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            // Manual mode: caller must use RebuildNow() or ScheduleRebuild()
        }

        // ----- LayoutGroup overrides (call generic axis methods) -----
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal(); // fills rectChildren
            CalculateLayoutInputAxis(0);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalculateLayoutInputAxis(1);
        }

        public override void SetLayoutHorizontal()
        {
            SetLayoutAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetLayoutAxis(1);
        }

        // ===== Generic passes =====

        /// <summary>
        /// Calculate min/pref for the given axis.
        /// For primary axis: sum of scaled sizes + spacing.
        /// For cross axis: max of scaled sizes.
        /// </summary>
        protected void CalculateLayoutInputAxis(int axis)
        {
            float required = 0f;
            // Sum along primary + spacing
            foreach (var c in rectChildren)
            {
                if (!c.gameObject.activeSelf) continue;
                EnsureTopLeftPreset(c);
                float s = axis == 1 ? c.rect.width : c.rect.height;
                required += s;
            }

            if (rectChildren.Count > 1) required += _Spacing * (rectChildren.Count - 1);

            // Add padding on that axis
            float pad = axis == 0 ? padding.horizontal : padding.vertical;
            float min = pad + required;
            SetLayoutInputForAxis(min, min, 0f, axis);
            SetLayoutAxis(axis);
        }

        protected void SetLayoutAxis(int axis)
        {
            if (rectChildren.Count == 0) return;

            bool isPrimary = axis == PrimaryAxis;

            if (isPrimary)
            {
                // Measure base & scaled sizes on primary axis
                int n = rectChildren.Count;
                var baseSize = new float[n];
                var scaledSize = new float[n];
                float totalScaled = 0f;

                for (int i = 0; i < n; i++)
                {
                    var c = rectChildren[i];
                    EnsureTopLeftPreset(c);

                    float b = GetBaseSize(c, axis);
                    baseSize[i] = b;

                    float s = b * (_ConsiderScale ? GetAxisScale(c, axis) : 1f);
                    scaledSize[i] = s;
                    totalScaled += s;
                }

                if (n > 1) totalScaled += _Spacing * (n - 1);

                float start = GetStartOffset(axis, totalScaled);

                // Center-based progression: keeps spacing balanced under scale
                float prevEnd = start;
                for (int i = 0; i < n; i++)
                {
                    var c = rectChildren[i];
                    float b = baseSize[i];
                    float s = scaledSize[i];

                    float center = (i == 0) ? (start + 0.5f * s) : (prevEnd + _Spacing + 0.5f * s);
                    float startScaled = center - 0.5f * s;

                    // anchoredDesired = startScaled + pivot * s
                    float anchoredDesired = startScaled + GetAxisPivot(c, axis) * s;

                    // Convert to "inset" param for SetChildAlongAxis (without resizing unless enabled)
                    float posParam = anchoredDesired - GetAxisPivot(c, axis) * b;

                    if (_ResizeChildren) SetChildAlongAxis(c, axis, posParam, b);
                    else SetChildAlongAxis(c, axis, posParam);

                    prevEnd = center + 0.5f * s;
                }
            }
            else
            {
                // Cross axis: center inside single "line" box (max scaled size on this axis)
                int n = rectChildren.Count;
                var baseSize = new float[n];
                var scaledSize = new float[n];

                float maxScaled = 0f;
                for (int i = 0; i < n; i++)
                {
                    var c = rectChildren[i];
                    EnsureTopLeftPreset(c);

                    float b = GetBaseSize(c, axis);
                    baseSize[i] = b;

                    float s = b * (_ConsiderScale ? GetAxisScale(c, axis) : 1f);
                    scaledSize[i] = s;

                    if (s > maxScaled) maxScaled = s;
                }

                float lineStart = GetStartOffset(axis, maxScaled);

                for (int i = 0; i < n; i++)
                {
                    var c = rectChildren[i];
                    float b = baseSize[i];
                    float s = scaledSize[i];

                    float startForChild = lineStart + (maxScaled - s) * 0.5f; // center inside line
                    float anchoredDesired = startForChild + GetAxisPivot(c, axis) * s;

                    float posParam = anchoredDesired - GetAxisPivot(c, axis) * b;

                    if (_ResizeChildren) SetChildAlongAxis(c, axis, posParam, b);
                    else SetChildAlongAxis(c, axis, posParam);
                }
            }

            FitContentSize();
        }

        public void FitContentSize()
        {
            float axisSize = GetTotalSize(0, padding);
            Debug.Log(axisSize);
            rectTransform.sizeDelta = new Vector2(axisSize, rectTransform.sizeDelta.y);
        }

        protected float GetTotalSize(int axis, RectOffset paddingOffset)
        {
            int childCount = rectChildren.Count;
            if (childCount == 0)
                return (axis == 0)
                    ? paddingOffset.left + paddingOffset.right
                    : paddingOffset.top + paddingOffset.bottom;

            float total = 0f;

            if (axis == PrimaryAxis)
            {
                int countSpace = 0;
                // Sum of all children sizes (scaled)
                for (int i = 0; i < childCount; i++)
                {
                    var c = rectChildren[i];
                    if (!c.gameObject.activeSelf) continue;
                    float size = axis == 1 ? c.sizeDelta.y : c.sizeDelta.x;
                    total += size;
                    ++countSpace;
                }

                // Add spacing between items
                if (countSpace > 1) total += _Spacing * (countSpace - 1);
            }
            else
            {
                // Take max size along cross axis
                for (int i = 0; i < childCount; i++)
                {
                    var c = rectChildren[i];
                    if (!c.gameObject.activeSelf) continue;
                    float b = axis == 1 ? c.sizeDelta.y : c.sizeDelta.x;
                    float v = b;
                    if (v > total) total = v;
                }
            }

            // Add padding on this axis
            if (axis == 0) total += paddingOffset.left + paddingOffset.right;
            else total += paddingOffset.top + paddingOffset.bottom;

            return total;
        }

        // ===== Helpers =====
        protected float GetBaseSize(RectTransform rt, int axis)
        {
            if (_UsePreferredSize)
            {
                float s = LayoutUtility.GetPreferredSize(rt, axis);
                if (s <= 0f) s = LayoutUtility.GetMinSize(rt, axis);
                if (s <= 0f) s = axis == 0 ? rt.rect.width : rt.rect.height;
                return s;
            }
            else
            {
                return axis == 0 ? rt.rect.width : rt.rect.height;
            }
        }

        protected float GetAxisScale(RectTransform rt, int axis)
            => axis == 0 ? Mathf.Abs(rt.localScale.x) : Mathf.Abs(rt.localScale.y);

        protected float GetAxisPivot(RectTransform rt, int axis)
            => axis == 0 ? rt.pivot.x : rt.pivot.y;

        protected void EnsureTopLeftPreset(RectTransform child)
        {
            if (!_ForceAnchorsTopLeft || child == null) return;

            Vector2 tl = new Vector2(0f, 1f);
            if (child.anchorMin != tl) child.anchorMin = tl;
            if (child.anchorMax != tl) child.anchorMax = tl;
            if (_AlsoSetPivotTopLeft && child.pivot != tl) child.pivot = tl;
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

            if (_ConsiderScale)
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
    }
}