using NIX.Module;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.Progress
{
    public class ProgressFillAmountCtrl : BaseProgress
    {
        [SerializeField] private Image _fill;

        private void Reset()
        {
            var fillObj = transform.Find("Fill");
            if (fillObj != null) _fill = fillObj.GetComponent<Image>();
        }

        protected override void Display()
        {
            _fill.fillAmount = CurrentValue / MaxValue;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_fill != null && MaxValue > 0f) Display();
        }
#endif
    }
}