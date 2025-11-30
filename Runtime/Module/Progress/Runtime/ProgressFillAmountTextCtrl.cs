using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.Progress
{
    public class ProgressFillAmountTextCtrl : BaseProgress
    {
        [SerializeField] private Image _fill;
        [SerializeField] private TextMeshProUGUI _valueTxt;
        [SerializeField] private string _prefix, _suffix;

        private void Reset()
        {
            var fillObj = transform.Find("Fill");
            var valueObj = transform.Find("ValueTxt");
            if (fillObj != null) _fill = fillObj.GetComponent<Image>();
            if (valueObj != null) _valueTxt = fillObj.GetComponent<TextMeshProUGUI>();
        }

        protected override void Display()
        {
            _fill.fillAmount = CurrentValue / MaxValue;
            _valueTxt.text = $"{_prefix}{CurrentValue:F0}{_suffix}";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_fill != null && _valueTxt != null && MaxValue > 0f) Display();
        }
#endif
    }

}