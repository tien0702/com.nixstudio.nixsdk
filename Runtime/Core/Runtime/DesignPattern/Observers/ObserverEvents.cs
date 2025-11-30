using System;
using System.Collections.Generic;

namespace NIX.Core.DesignPatterns
{
    public class ObserverEvents<TEnumKey, TMessage> where TEnumKey : Enum
    {
        protected readonly Dictionary<TEnumKey, Action<TMessage>> _Events = new();

        public virtual void Clear()
        {
            _Events.Clear();
        }

        public virtual void ClearType(TEnumKey type)
        {
            if (_Events.TryGetValue(type, out var ev)) ev = null;
        }

        public virtual void Register(TEnumKey eventType, Action<TMessage> observer)
        {
            if (!_Events.TryAdd(eventType, observer))
            {
                _Events[eventType] += observer;
            }
        }

        public virtual void UnRegister(TEnumKey eventType, Action<TMessage> observer)
        {
            if (!_Events.TryGetValue(eventType, out var ev))
            {
                UnityEngine.Debug.LogWarning($"Event Type: {eventType} is not exists!");
                return;
            }

            ev -= observer;
        }

        public virtual void Publish(TEnumKey eventType, TMessage data = default)
        {
            if (!_Events.TryGetValue(eventType, out var observers)) return;

            observers?.Invoke(data);
        }
    }
}