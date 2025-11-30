using UnityEngine;
using UnityEngine.UI;

namespace NIX.Core
{
    public class DetectIpadDevice : MonoBehaviour
    {
        public static bool IS_TABLET;
        private CanvasScaler _scaler;

        private void Awake()
        {
            IS_TABLET = IsTablet();
            _scaler = GetComponent<CanvasScaler>();
            _scaler.matchWidthOrHeight = IS_TABLET ? 1 : 0;
        }

        public bool IsTablet()
        {
            var mainCamera = Camera.main;
            return mainCamera != null && mainCamera.aspect > 0.59f;
        }
    }
}