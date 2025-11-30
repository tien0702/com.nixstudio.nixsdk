using System;
using NIX.Core.UI;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.Popup
{
    public abstract class BasePopup : BaseUIVisual
    {
        [SerializeField] protected PopupOverlay _Overlay;
        [SerializeField] protected BaseUIVisualAnim _PopupAnim;

        public override void OnComponentAdded()
        {
            base.OnComponentAdded();
            _Overlay = GetComponentInChildren<PopupOverlay>(true);
            _PopupAnim = GetComponentInChildren<BaseUIVisualAnim>(true);
        }

        public override void Open(Action onOpened = null)
        {
            base.Open(onOpened);
            _Overlay.Open();
            _PopupAnim.Open(OnOpened);
        }

        public override void Close(Action onClosed = null)
        {
            base.Close(onClosed);
            _Overlay.Close();
            _PopupAnim.Close(OnClosed);
        }
    }
}