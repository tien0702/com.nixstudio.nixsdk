using System;
using System.Collections.Generic;
using UnityEngine;

namespace NIX.Core.UI
{
    public enum UIVisualEvent
    {
        OnOpen,
        OnOpened,
        OnClose,
        OnClosed
    }

    public abstract class BaseUIVisual : BaseMono
    {
        protected Dictionary<UIVisualEvent, Action> _Events = new();

        public virtual void RegisterEvent(UIVisualEvent ev, Action callback)
        {
            if (!_Events.TryAdd(ev, callback))
            {
                _Events[ev] += callback;
            }
        }

        public virtual void UnregisterEvent(UIVisualEvent ev, Action callback)
        {
            if (!_Events.ContainsKey(ev))
                _Events[ev] -= callback;
        }

        protected virtual void PublishEvent(UIVisualEvent ev, bool clear = false)
        {
            if (!_Events.TryGetValue(ev, out var callbacks)) return;
            callbacks?.Invoke();
            if (clear) _Events[ev] = null;
        }

        public virtual void Open(Action onOpened = null)
        {
            gameObject.SetActive(true);
            PublishEvent(UIVisualEvent.OnOpen);
            RegisterEvent(UIVisualEvent.OnOpened, onOpened);
        }

        public virtual void Close(Action onClosed = null)
        {
            PublishEvent(UIVisualEvent.OnClose);
            RegisterEvent(UIVisualEvent.OnClosed, onClosed);
        }

        protected virtual void OnOpened()
        {
            PublishEvent(UIVisualEvent.OnOpened, true);
        }

        protected virtual void OnClosed()
        {
            gameObject.SetActive(false);
            PublishEvent(UIVisualEvent.OnClosed, true);
        }
        
        #if UNITY_EDITOR

        [ContextMenu("Open")]
        protected virtual void OpenTest()
        {
            this.Open();
        }

        [ContextMenu("Close")]
        protected virtual void CloseTest()
        {
            this.Open();
        }
        
        #endif
    }
}