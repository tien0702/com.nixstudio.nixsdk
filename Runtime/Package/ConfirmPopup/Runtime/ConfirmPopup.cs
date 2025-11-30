using System;
using System.Linq;
using NIX.Module.Popup;
using UnityEngine.UI;
using UnityEngine;

namespace NIX.Packages
{
    public class ConfirmPopup : BasePopup
    {
        protected Action<bool> _OnResultEvent;

        [SerializeField] protected Button _YesBtn, _NoBtn;
        protected bool _Result = false;

        public override void OnComponentAdded()
        {
            base.OnComponentAdded();
            _YesBtn = transform.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name.Equals("YesBtn"));
            _NoBtn = transform.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name.Equals("NoBtn"));
        }

        protected virtual void Awake()
        {
            _YesBtn.onClick.AddListener(OnPressYesBtn);
            _NoBtn.onClick.AddListener(OnPressNoBtn);
        }

        public virtual void OpenPopup(Action<bool> onResult, Action onOpened = null)
        {
            Open(onOpened);
            _OnResultEvent = onResult;
        }

        public override void Open(Action onOpened = null)
        {
            base.Open(onOpened);
            _Result = false;
        }

        protected virtual void OnPressNoBtn()
        {
            _Result = false;
            this.Close();
        }

        protected virtual void OnPressYesBtn()
        {
            _Result = true;
            this.Close();
        }

        public override void Close(Action onClosed = null)
        {
            base.Close(() =>
            {
                onClosed?.Invoke();
                _OnResultEvent?.Invoke(_Result);
                _OnResultEvent = null;
            });
        }
    }
}