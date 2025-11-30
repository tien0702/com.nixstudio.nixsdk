using UnityEngine;

namespace NIX.Packages
{
    public class ActiveOnControl : MonoBehaviour
    {
        private JoystickCtrl _joystickCtrl;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _joystickCtrl = GetComponent<JoystickCtrl>();
            _canvasGroup = _joystickCtrl.Joystick.GetComponent<CanvasGroup>();
            _joystickCtrl.Events.Subscribe(JoystickEvent.BeginDrag, OnBeginDrag);
            _joystickCtrl.Events.Subscribe(JoystickEvent.EndDrag, OnEndDrag);
            _canvasGroup.alpha = 0;
        }

        private void OnEndDrag()
        {
            _canvasGroup.alpha = 0;
        }

        private void OnBeginDrag()
        {
            _canvasGroup.alpha = 1;
        }
    }
}