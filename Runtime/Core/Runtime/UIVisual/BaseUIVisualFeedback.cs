using System;
using UnityEngine;

namespace NIX.Core.UI
{
    public abstract class BaseUIVisualFeedback : BaseMono
    {
        [SerializeField] protected UIVisualEvent _EventType;
        protected BaseUIVisual _UIVisual;

        protected virtual void OnEnable()
        {
            _UIVisual ??= GetComponent<BaseUIVisual>();
            _UIVisual.RegisterEvent(_EventType, OnEvent);
        }

        protected virtual void OnDisable()
        {
            _UIVisual?.UnregisterEvent(_EventType, OnEvent);
        }

        protected abstract void OnEvent();
    }
}