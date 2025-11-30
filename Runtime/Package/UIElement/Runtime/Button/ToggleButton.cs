using System;
using NIX.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    [RequireComponent(typeof(AdvancedButton))]
    public class ToggleButton : BaseMono
    {
        public event Action<bool> onValueChanged;
        [SerializeField] private Image _targetImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Sprite _onSprite, _offSprite, _onIcon, _offIcon;
        [SerializeField] private bool _isOn = true;
        private Button _btn;
        
        public bool IsOn => _isOn;

        protected void Awake()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(Toggle);
        }

        public void Toggle()
        {
            _isOn = !_isOn;
            onValueChanged?.Invoke(_isOn);
        }
        
        public void SetState(bool isOn)
        {
            _isOn = isOn;
            Refresh();
            onValueChanged?.Invoke(_isOn);
        }
        
        public void SetStateWithoutNotify(bool isOn)
        {
            _isOn = isOn;
            Refresh();
        }

        private void Refresh()
        {
            if (_targetImage != null)
            {
                _targetImage.sprite = _isOn ? _onSprite : _offSprite;
                _targetImage.SetNativeSize();
            }
            if (_iconImage != null)
            {
                _iconImage.sprite = _isOn ? _onIcon : _offIcon;
                _iconImage.SetNativeSize();
            }
        }

#if UNITY_EDITOR

        protected void OnValidate()
        {
            if (_targetImage == null)
                _targetImage = GetComponent<Image>();
            if (_iconImage == null)
            {
                transform.Find("Icon")?.TryGetComponent(out _iconImage);
            }
                
            Refresh();
        }
#endif
    }
}