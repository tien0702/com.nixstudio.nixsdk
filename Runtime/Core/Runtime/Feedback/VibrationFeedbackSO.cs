using UnityEngine;

namespace NIX.Core.Feedback
{
    [CreateAssetMenu(menuName = "SO/Feedbacks/Vibration Feedback")]
    public class VibrationFeedbackSO : BaseFeedbackSO
    {
        public ImpactFeedbackStyle ImpactType = ImpactFeedbackStyle.Soft;

        public override void PlayFeedback()
        {
            Vibration.Vibrate(ImpactType);
        }
    }
}