using UnityEngine;

namespace NIX.Module.Popup
{
    [CreateAssetMenu(menuName = "NIX/Popup/Config")]
    public class PopupAnimConfig : ScriptableObject
    {
        public bool IgnoreTimeScale = false;
        [Tooltip("Pause game on activation.")]
        public bool PauseGame = false;
        [Tooltip("Block input on playing animation")]
        public bool BlockInput = true;
    }
}