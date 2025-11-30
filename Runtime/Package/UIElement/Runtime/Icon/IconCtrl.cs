using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public class IconCtrl : MonoBehaviour
    {
        private Image _iconImage;
        private RescaleByParent _iconScaler;
        private bool _initialized = false;

        private void Initialize()
        {
            _iconImage = GetComponent<Image>();
            _iconScaler = GetComponent<RescaleByParent>();
            _initialized = true;
        }

        public void SetSprite(Sprite sprite, bool setNativeSize = true)
        {
            if (!_initialized) Initialize();
            _iconImage.sprite = sprite;
            if (setNativeSize) _iconImage.SetNativeSize();
            _iconScaler.Fit();
        }
    }
}