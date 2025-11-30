using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public static class GalleryUtils
    {
        /// <summary>
        /// Compute sizeDelta for a RectTransform given a desired absolute size in pixels.
        /// Works regardless of anchors.
        /// </summary>
        public static void SetSize(RectTransform rt, Vector2 newSize)
        {
            if (rt == null) return;

            Vector2 parentSize = Vector2.zero;
            if (rt.parent is RectTransform parent)
                parentSize = parent.rect.size;

            Vector2 anchorSpan = rt.anchorMax - rt.anchorMin;
            rt.sizeDelta = newSize - Vector2.Scale(parentSize, anchorSpan);
        }

        /// <summary>
        /// Viewport center X (in content local space).
        /// </summary>
        public static float GetViewportCenterXInContent(ScrollRect sr)
        {
            var vp = sr.viewport;
            var ct = sr.content;
            return ct.InverseTransformPoint(vp.TransformPoint(vp.rect.center)).x;
        }

        /// <summary>
        /// Element center X (in content local space). Uses relative bounds, robust to size/scale changes.
        /// </summary>
        public static float GetElementCenterXInContent(ScrollRect sr, RectTransform element)
        {
            var b = RectTransformUtility.CalculateRelativeRectTransformBounds(sr.content, element);
            return b.center.x;
        }

        /// <summary>
        /// Solve final content.anchoredPosition.x so that 'element' center == viewport center.
        /// Uses fixed-point iterations with layout rebuild in-between.
        /// </summary>
        public static float SolveSnapTargetX(
            ScrollRect sr,
            RectTransform element,
            System.Action rebuild,
            int maxIter,
            float epsilon)
        {
            var ct = sr.content;
            // Save & restore to avoid visible jumps during solving
            Vector2 saved = ct.anchoredPosition;

            for (int i = 0; i < maxIter; i++)
            {
                float elemX = GetElementCenterXInContent(sr, element);
                float vpX = GetViewportCenterXInContent(sr);
                float dx = elemX - vpX;
                if (Mathf.Abs(dx) <= epsilon) break;

                ct.anchoredPosition = new Vector2(ct.anchoredPosition.x - dx, ct.anchoredPosition.y);

                // Refresh sizes/positions for next iteration
                Canvas.ForceUpdateCanvases();
                rebuild?.Invoke();
            }

            float targetX = ct.anchoredPosition.x;

            // Restore before animating
            ct.anchoredPosition = saved;
            Canvas.ForceUpdateCanvases();
            rebuild?.Invoke();

            return targetX;
        }

        /// <summary>
        /// Collect active GalleryElement children of content.
        /// </summary>
        public static List<GalleryElement> GetActiveElements(ScrollRect sr)
        {
            var elements = new List<GalleryElement>();
            var ct = sr.content;
            for (int i = 0; i < ct.childCount; i++)
            {
                var child = ct.GetChild(i);
                if (child.gameObject.activeSelf && child.TryGetComponent(out GalleryElement e))
                    elements.Add(e);
            }
            return elements;
        }

        /// <summary>
        /// Nearest GalleryElement to viewport center on X axis (world-space check, robust).
        /// </summary>
        public static GalleryElement GetNearestByX(ScrollRect sr, List<GalleryElement> elements)
        {
            if (elements == null || elements.Count == 0) return null;

            var vp = sr.viewport;
            Vector3 vpCenterWorld = vp.TransformPoint(vp.rect.center);

            GalleryElement nearest = null;
            float bestD2 = float.PositiveInfinity;

            foreach (var e in elements)
            {
                if (e == null || !e.gameObject.activeInHierarchy) continue;
                var rt = e.Rt != null ? e.Rt : e.transform as RectTransform;
                if (rt == null) continue;

                Vector3 elemCenterWorld = rt.TransformPoint(rt.rect.center);
                float dx = elemCenterWorld.x - vpCenterWorld.x;
                float d2 = dx * dx;

                if (d2 < bestD2)
                {
                    bestD2 = d2;
                    nearest = e;
                }
            }
            return nearest;
        }
    }
}
