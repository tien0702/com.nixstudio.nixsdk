using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NIX.Packages
{
    public enum LockType
    {
        GrayOverlay,
        SwapSprite
    };

    [RequireComponent(typeof(Image))]
    public class ImageUnlockable : Unlockable
    {
        [SerializeField] protected LockType _LockType = LockType.SwapSprite;
        [SerializeField] protected Sprite _LockSprite;
        [SerializeField] protected Sprite _UnlockSprite;
        [SerializeField] protected bool _ResetNativeSize = true;

        protected GrayImageToggle _Gray;
        protected Image _TargetImg;

        public Image TargetImg
        {
            get
            {
                if (_TargetImg == null)
                {
                    _TargetImg = GetComponent<Image>();
                }

                return _TargetImg;
            }
        }

        public GrayImageToggle Gray
        {
            get
            {
                if (_Gray == null)
                {
                    _Gray = GetComponent<GrayImageToggle>();
                }

                return _Gray;
            }
        }

        public override void SetState(LockState state)
        {
            base.SetState(state);
            switch (State)
            {
                case LockState.Lock:
                    ChangeToLock();
                    break;
                case LockState.Unlock:
                    ChangeToUnLock();
                    break;
            }
        }

        protected virtual void ChangeToUnLock()
        {
            switch (_LockType)
            {
                case LockType.GrayOverlay:
                    Gray.SetState(true);
                    break;
                case LockType.SwapSprite:
                    TargetImg.sprite = _UnlockSprite;
                    if (_ResetNativeSize) TargetImg.SetNativeSize();
                    break;
            }
        }

        protected virtual void ChangeToLock()
        {
            switch (_LockType)
            {
                case LockType.GrayOverlay:
                    Gray.SetState(false);
                    break;
                case LockType.SwapSprite:
                    TargetImg.sprite = _LockSprite;
                    if (_ResetNativeSize) TargetImg.SetNativeSize();
                    break;
            }
        }

#if UNITY_EDITOR
        private static bool _Editor_IsLock = false;
        [ContextMenu("Toggle")]
        private void Editor_Toggle()
        {
            _Editor_IsLock = !_Editor_IsLock;
            SetState(_Editor_IsLock ? LockState.Lock : LockState.Unlock);
        }
#endif
    }
}