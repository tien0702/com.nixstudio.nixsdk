using NIX.Core;
using UnityEngine;

namespace NIX.Packages
{
    public class VibrationFeedback : BaseButtonFeedback
    {
        [SerializeField] private ImpactFeedbackStyle _impact = ImpactFeedbackStyle.Soft;

        protected override void OnEvent()
        {
            Vibration.Vibrate(_impact);
        }
    }
}