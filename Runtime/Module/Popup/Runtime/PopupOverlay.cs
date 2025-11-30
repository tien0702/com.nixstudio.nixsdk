using System;
using NIX.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.Popup
{
    public class PopupOverlay : BaseUIVisual
    {
        [SerializeField] protected BasePopup _Popup;

        protected virtual void Reset()
        {
            _Popup = GetComponentInParent<BasePopup>();
        }

        protected virtual void Awake()
        {
            if (TryGetComponent(out Button btn))
            {
                btn.onClick.AddListener(OnPressBtn);
            }
        }

        protected virtual void OnPressBtn()
        {
            _Popup.Close();
        }
    }
}