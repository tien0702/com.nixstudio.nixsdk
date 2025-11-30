using System;
using NIX.Module.UI;
using UnityEngine.UI;

namespace NIX.Module.Popup
{
    public class ClosePopupBtn : CloseUIVisualBtn
    {
        protected virtual void Reset()
        {
            if (_Btn == null) _Btn = GetComponent<Button>();
            if (_UIVisual == null)
            {
                _UIVisual = GetComponentInParent<BasePopup>();
            }
        }
    }
}