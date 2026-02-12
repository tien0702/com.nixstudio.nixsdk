using System;
using NIX.Core.UI;

namespace NIX.Module.Panel
{
    public class BasePanel : BaseUIVisual
    {
        protected BaseUIVisualAnim _VisualAnim;

        public override void Open(Action onOpened = null)
        {
            base.Open(onOpened);
            _VisualAnim.Open(OnOpened);
        }

        public override void Close(Action onClosed = null)
        {
            base.Close(onClosed);
            _VisualAnim?.Close(OnClosed);
        }
    }
}