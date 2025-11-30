using UnityEngine;

namespace NIX.Packages
{
    public abstract class BaseButtonFeedback : MonoBehaviour
    {
        [SerializeField] protected ButtonEvent _Event = ButtonEvent.OnClick;
        protected AdvancedButton _Button;

        protected virtual void Awake()
        {
            _Button = GetComponent<AdvancedButton>();
        }

        protected virtual void OnEnable()
        {
            _Button.Register(_Event, OnEvent);
        }

        protected virtual void OnDisable()
        {
            _Button?.Unregister(_Event, OnEvent);
        }

        protected virtual void OnDestroy()
        {
            _Button?.Unregister(_Event, OnEvent);
        }

        protected abstract void OnEvent();
    }
}