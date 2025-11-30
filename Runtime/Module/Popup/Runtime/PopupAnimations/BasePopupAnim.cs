using System;
using NIX.Core.UI;
using UnityEngine;

namespace NIX.Module.Popup
{
    public class BasePopupAnim : BaseUIVisualAnim
    {
        [SerializeField] protected PopupAnimConfig _Config;

        public override void Open(Action onOpened)
        {
        }

        public override void Close(Action onClosed)
        {
        }
    }
}