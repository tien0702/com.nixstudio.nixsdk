using System.Collections;
using UnityEngine;

namespace NIX.Packages
{
    public class FitToTarget : MonoBehaviour
    {
        [SerializeField] protected bool _fitW, _fitH;

        [SerializeField] protected RectTransform _target;

        protected RectTransform _rectTransform;

        protected virtual IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            if (_fitH) _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _target.rect.height);
            if (_fitW) _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _target.rect.width);
        }
    }
}