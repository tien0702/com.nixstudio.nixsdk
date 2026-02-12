using System;
using System.Collections.Generic;

namespace NIX.Core.DesignPatterns
{
    public sealed class TopicMessageBroker
    {
        private readonly Dictionary<string, List<Delegate>> _handlers = new();

        public void Subscribe<T>(string topic, Action<T> handler) where T : ITopicMessage
        {
            if (!_handlers.TryGetValue(topic, out var list))
            {
                list = new List<Delegate>();
                _handlers[topic] = list;
            }

            if (!list.Contains(handler)) list.Add(handler);
        }

        public void Unsubscribe<T>(string topic, Action<T> handler) where T : ITopicMessage
        {
            if (!_handlers.TryGetValue(topic, out var list)) return;
            list.Remove(handler);
            if (list.Count == 0) _handlers.Remove(topic);
        }

        public void Publish<T>(T message) where T : ITopicMessage
        {
            if (!_handlers.TryGetValue(message.Topic.ToString(), out var list)) return;
            foreach (var element in list)
            {
                element.DynamicInvoke(message);
            }
        }
    }
}