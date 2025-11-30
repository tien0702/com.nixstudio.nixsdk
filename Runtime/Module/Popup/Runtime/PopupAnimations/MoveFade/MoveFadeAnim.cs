using System;
using NIX.Core.UI;
using PrimeTween;
using UnityEngine;

namespace NIX.Module.Popup
{
    public class MoveFadeAnim : BasePopupAnim
    {
        [SerializeField] protected PopupMoveFadeSO _AnimSo;
        [SerializeField] protected BaseAlphaUIVisual _Component;

        protected Sequence _Seq;

        protected virtual void Reset()
        {
            _Component = GetComponent<BaseAlphaUIVisual>();
        }

        public override void Open(Action onOpened)
        {
            base.Open(onOpened);
            _Seq.Stop();
            _Seq = Sequence.Create();
            transform.localPosition = _AnimSo.ClosedPos;
            _Seq.Group(Tween.LocalPosition(transform, _AnimSo.OpenedPos, _AnimSo.Duration, ease: _AnimSo.OpenEase,
                useUnscaledTime: _Config.IgnoreTimeScale));
            _Seq.Group(Tween.Custom(_Component.Alpha, _AnimSo.OpenedAlpha, _AnimSo.Duration, SetAlpha,
                ease: Ease.Linear,
                useUnscaledTime: _Config.IgnoreTimeScale));
            _Seq.OnComplete(onOpened);
        }

        public override void Close(Action onClosed)
        {
            base.Close(onClosed);
            _Seq.Stop();
            _Seq = Sequence.Create();
            _Seq.Group(Tween.LocalPosition(transform, _AnimSo.ClosedPos, _AnimSo.Duration, ease: _AnimSo.CloseEase,
                useUnscaledTime: _Config.IgnoreTimeScale));
            _Seq.Group(Tween.Custom(_Component.Alpha, _AnimSo.ClosedAlpha, _AnimSo.Duration, SetAlpha,
                ease: Ease.Linear,
                useUnscaledTime: _Config.IgnoreTimeScale));
            _Seq.OnComplete(onClosed);
        }

        protected virtual void SetAlpha(float alpha) => _Component.SetAlpha(alpha);
    }
}