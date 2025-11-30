using System.Collections;
using System.Collections.Generic;
using NIX.Core.DesignPatterns;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NIX.Packages
{
    [ExecuteAlways]
    [RequireComponent(typeof(ScrollRect))]
    public class HorizontalGallery : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [Header("Ref")]
        [SerializeField] protected ScrollRect _ScrollRect;
        [SerializeField] protected Pooler _ElementPooler;
        
        [Header("Layout")]
        [SerializeField] protected float _Spacing = 30f;
        [SerializeField] protected float _Distance = 600f;
        [SerializeField] protected Vector2 _ElementSize;

        [Header("Scale Settings")]
        [SerializeField, Min(0f)] protected float _MinScale = 0.2f;
        [SerializeField, Min(0f)] protected float _MaxScale = 1f;

        [Header("Focus Settings")]
        [SerializeField] protected float _SpeedThreshold = 30f;

        [Header("Snap Settings")]
        [SerializeField] protected float _SnapDuration = 0.35f;
        [SerializeField] protected Ease _SnapEase = Ease.OutBack;
        [SerializeField] protected float _SolveEpsilon = 0.5f;
        [SerializeField] protected int _SolveMaxIter = 8;
        protected Coroutine _WaitVelocityCoroutine;
        protected Tween _SnapTween;

        protected virtual void OnEnable()
        {
            if (_ScrollRect == null) _ScrollRect = GetComponent<ScrollRect>();
            _ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_ScrollRect.viewport);
            ReBuildLayout();
        }

        protected void OnDisable()
        {
            _ScrollRect?.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        private void OnScrollValueChanged(Vector2 _)
        {
            ReBuildLayout();
        }

        private void Update()
        {
            ReBuildLayout();
        }

        private IEnumerator WaitStopScroll()
        {
            yield return new WaitUntil(IsSpeedThresholdReached);
            SnapTo(GetNearestActiveElement());
        }

        private bool IsSpeedThresholdReached()
        {
            return _ScrollRect.velocity.magnitude < _SpeedThreshold;
        }

        /// <summary>
        /// Absolute layout & per-item scaling based on horizontal distance to viewport center.
        /// </summary>
        [ContextMenu("ReBuild Now")]
        public void ReBuildLayout()
        {
            var elements = GetActiveElements();

            float sidePadding = _ScrollRect.viewport.rect.width * 0.5f;
            float totalWidth = sidePadding * 2f + Mathf.Max(0f, _Spacing * (elements.Count - 1));
            float cursor = sidePadding;
            float posY = _ScrollRect.content.rect.height * 0.5f;

            float vpCenterX = GalleryUtils.GetViewportCenterXInContent(_ScrollRect);

            foreach (var element in elements)
            {
                // Distance on X axis in content space
                float elemCenterX = GalleryUtils.GetElementCenterXInContent(_ScrollRect, element.Rt);
                float distance = Mathf.Abs(elemCenterX - vpCenterX);

                // Ratio & size
                float ratio = Mathf.Lerp(_MinScale, _MaxScale, 1f - Mathf.Max(distance / _Distance, 0f));
                element.SetRatio(_ElementSize ,ratio);

                // Width after ratio update
                float w = element.Rt.rect.width;
                totalWidth += w;

                // Position element using current width
                float posX = cursor + element.Rt.pivot.x * w;
                element.SetAxisPosition(posX, axis: 0);
                element.SetAxisPosition(-posY, axis: 1);

                cursor += w + _Spacing;
            }

            GalleryUtils.SetSize(_ScrollRect.content, new Vector2(totalWidth, _ScrollRect.content.sizeDelta.y));
            _ScrollRect.content.ForceUpdateRectTransforms();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_SnapTween.isAlive) _SnapTween.Stop();
            if (_WaitVelocityCoroutine != null) StopCoroutine(_WaitVelocityCoroutine);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!Application.isPlaying) return;
            if (_WaitVelocityCoroutine != null) StopCoroutine(_WaitVelocityCoroutine);
            _WaitVelocityCoroutine = StartCoroutine(WaitStopScroll());
        }

        protected GalleryElement GetNearestActiveElement()
        {
            var elements = GetActiveElements();
            if (elements == null || elements.Count == 0) return null;
            return GalleryUtils.GetNearestByX(_ScrollRect, elements);
        }

        protected List<GalleryElement> GetActiveElements()
        {
            return GalleryUtils.GetActiveElements(_ScrollRect);
        }

        public void SnapTo(GalleryElement element, bool immediate = false)
        {
            if (element == null || element.Rt == null || _ScrollRect == null) return;

            // Stop inertia & ongoing tween
            _ScrollRect.StopMovement();
            if (_SnapTween.isAlive) _SnapTween.Stop();

            // Ensure fresh layout before solving
            Canvas.ForceUpdateCanvases();
            ReBuildLayout();

            float targetX = GalleryUtils.SolveSnapTargetX(
                _ScrollRect,
                element.Rt,
                ReBuildLayout,
                _SolveMaxIter,
                _SolveEpsilon);

#if UNITY_EDITOR
            if (!Application.isPlaying) immediate = true;
#endif

            var ct = _ScrollRect.content;

            if (immediate)
            {
                var p = ct.anchoredPosition; p.x = targetX; ct.anchoredPosition = p;
                ReBuildLayout();
                _ScrollRect.velocity = Vector2.zero;
                return;
            }

            float startX = ct.anchoredPosition.x;

            _SnapTween = Tween.Custom(
                startX,
                targetX,
                duration: _SnapDuration,
                onValueChange: newX =>
                {
                    var pos = ct.anchoredPosition;
                    pos.x = newX;
                    ct.anchoredPosition = pos;
                },
                ease: _SnapEase)
            .OnUpdate(this, (gallery, tween) => { ReBuildLayout(); })
            .OnComplete(() =>
            {
                ReBuildLayout();
                _ScrollRect.velocity = Vector2.zero;
            });
        }
    }
}
