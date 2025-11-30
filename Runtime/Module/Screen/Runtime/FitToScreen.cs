using System.Collections;
using NIX.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.Progress
{
    [RequireComponent(typeof(Image))]
    public class FitToScreen : BaseMono
    {
        protected Canvas _Canvas;
        protected RectTransform _RectTransform;

        protected virtual IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Fit();
        }

        public virtual void Fit()
        {
            _Canvas = GetRootCanvas(transform);
            _RectTransform = this.GetComponent<RectTransform>();

            Vector2 targetSize = _RectTransform.sizeDelta;
            Vector2 screenSize = _Canvas.GetComponent<RectTransform>().sizeDelta;

            float scaleX = screenSize.x / targetSize.x;
            float scaleY = screenSize.y / targetSize.y;

            _RectTransform.localScale = Vector3.one * Mathf.Max(scaleX, scaleY);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_RectTransform);
        }

        public virtual void Fit(Canvas canvas)
        {
            _Canvas = canvas;
            _RectTransform = this.GetComponent<RectTransform>();

            Vector2 targetSize = _RectTransform.sizeDelta;
            Vector2 screenSize = _Canvas.GetComponent<RectTransform>().sizeDelta;


            float scaleX = screenSize.x / targetSize.x;
            float scaleY = screenSize.y / targetSize.y;

            _RectTransform.localScale = Vector3.one * Mathf.Max(scaleX, scaleY);
        }

        protected Canvas GetRootCanvas(Transform current)
        {
            Canvas result = null;
            while (current != null)
            {
                var canvas = current.GetComponent<Canvas>();
                if (canvas != null)
                {
                    result = canvas;
                }

                current = current.parent;
            }

            return result;
        }
    }
}