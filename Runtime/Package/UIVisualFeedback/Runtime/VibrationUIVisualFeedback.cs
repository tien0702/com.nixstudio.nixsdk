using NIX.Core;
using NIX.Core.UI;
using UnityEngine;

namespace NIX.Packages
{
    public class VibrationUIVisualFeedback : BaseUIVisualFeedback
    {
        [SerializeField] private ImpactFeedbackStyle _style = ImpactFeedbackStyle.Light;
        protected override void OnEvent()
        {
            Vibration.Vibrate(_style);
        }
    }
}