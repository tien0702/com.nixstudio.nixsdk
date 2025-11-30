using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NIX.Core.DesignPatterns
{
    public class BrokerInfo
    {
        public List<Delegate> Subscribers = new();
        public bool IsPublishing = false;
        public List<Delegate> UnSubscribers = new();
    }

    public sealed class MessageBroker
    {
        private readonly Dictionary<Type, BrokerInfo> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IMessage
        {
            if (!_handlers.TryGetValue(typeof(T), out var info))
            {
                info = new BrokerInfo();
                _handlers[typeof(T)] = info;
            }

            if (!info.Subscribers.Contains(handler)) info.Subscribers.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IMessage
        {
            if (!_handlers.TryGetValue(typeof(T), out var info)) return;
            if (info.IsPublishing)
            {
                info.UnSubscribers.Add(handler);
            }
            else
            {
                info.Subscribers.Remove(handler);
            }
        }

        public void Publish<T>(T message) where T : IMessage
        {
            if (!_handlers.TryGetValue(typeof(T), out var info)) return;
            info.IsPublishing = true;
            foreach (var element in info.Subscribers)
            {
                element.DynamicInvoke(message);
            }

            info.IsPublishing = false;
            // optimize this
            if (info.UnSubscribers.Count > 0)
            {
                info.Subscribers.RemoveAll(p => info.UnSubscribers.Contains(p));
                info.UnSubscribers.Clear();
            }
        }
    }
}