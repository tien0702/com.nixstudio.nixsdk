using System;
using System.Collections.Generic;

namespace NIX.Core
{
    public class EnumEvent<TEnumKey> where TEnumKey : System.Enum
    {
        private Dictionary<TEnumKey, Action> _events = new();

        public void Subscribe(TEnumKey key, Action callback)
        {
            _events.TryAdd(key, null);
            _events[key] += callback;
        }

        public void Unsubscribe(TEnumKey key, Action callback)
        {
            if (_events.ContainsKey(key))
            {
                _events[key] -= callback;
            }
        }

        public void Publish(TEnumKey key)
        {
            if (_events.ContainsKey(key))
            {
                _events[key]?.Invoke();
            }
        }
    }
}