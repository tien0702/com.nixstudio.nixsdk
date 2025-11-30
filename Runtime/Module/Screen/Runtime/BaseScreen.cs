using System;
using NIX.Core.UI;
using UnityEngine;

namespace NIX.Module.Screen
{
    public abstract class BaseScreen : BaseUIVisual
    {
        [SerializeField] protected BaseUIVisualAnim _VisualAnim;

        public override void OnComponentAdded()
        {
            base.OnComponentAdded();

            _VisualAnim = GetComponent<BaseUIVisualAnim>();
        }

        public override void Open(Action onOpened = null)
        {
            base.Open(onOpened);
            if (_VisualAnim != null) _VisualAnim.Open(OnOpened);
            else OnOpened();
        }

        public override void Close(Action onClosed = null)
        {
            base.Close(onClosed);
            if (_VisualAnim != null) _VisualAnim.Close(OnClosed);
            else OnClosed();
        }
    }
}