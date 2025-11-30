using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NIX.Packages
{
    public enum ButtonEvent
    {
        OnClick,
        OnPressDown,
        OnPressUp,
        OnEnter,
        OnExit
    }

    public class AdvancedButton : Button
    {
        private readonly Dictionary<ButtonEvent, Action> _events = new();

        public void Register(ButtonEvent ev, Action callback)
        {
            if (!_events.TryAdd(ev, callback))
            {
                _events[ev] += callback;
            }
        }

        public void Unregister(ButtonEvent ev, Action callback)
        {
            if (_events.ContainsKey(ev))
            {
                _events[ev] -= callback;
            }
        }

        protected void Publish(ButtonEvent ev, bool clear = false)
        {
            if (!_events.TryGetValue(ev, out var callbacks)) return;
            callbacks?.Invoke();
            if (clear) _events[ev] = null;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            Publish(ButtonEvent.OnClick);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Publish(ButtonEvent.OnPressDown);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Publish(ButtonEvent.OnPressUp);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            Publish(ButtonEvent.OnEnter);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            Publish(ButtonEvent.OnExit);
        }
    }
}