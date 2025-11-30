using System.Collections.Generic;
using UnityEngine;
using System;

namespace NIX.Core.DesignPatterns
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<string, object> _topicServices = new();

        #region Type based services

        public void Register<T>(T service, bool replace = false) where T : class, IService
        {
            Type key = typeof(T);
            if (_services.ContainsKey(key) && replace)
            {
                _services[key] = service;
                Debug.Log($"{key} replaced");
            }
            else
            {
                _services.Add(key, service);
            }
        }

        public void Unregister<T>() where T : class, IService
        {
            Type key = typeof(T);

            if (!_services.ContainsKey(key))
            {
                Debug.Log($"{key} is not registered");
            }
            else
            {
                _services.Remove(key);
            }
        }

        public bool IsRegistered<T>() where T : class, IService
        {
            return _services.ContainsKey(typeof(T));
        }

        public T Get<T>() where T : class, IService
        {
            Type key = typeof(T);

            if (!_services.TryGetValue(key, out var service))
            {
                Debug.Log($"{key} not register");
                return default(T);
            }

            return (T)service;
        }

        #endregion

        #region Topic services

        public void RegisterTopic<T>(string topic, T service) where T : class, IService
        {
            if (!_topicServices.TryAdd(topic, service))
            {
                Debug.Log($"{topic} registered");
            }
        }

        public void UnregisterTopic(string topic)
        {
            if (!_topicServices.ContainsKey(topic))
            {
                Debug.Log($"{topic} is not registered");
            }
            else
            {
                _topicServices.Remove(topic);
            }
        }

        public bool IsTopicRegistered(string topic)
        {
            return _topicServices.ContainsKey(topic);
        }

        public T GetTopic<T>(string topic) where T : class, IService
        {
            if (!_topicServices.TryGetValue(topic, out var service))
            {
                Debug.Log($"{topic} not register");
                return default(T);
            }

            return (T)service;
        }

        #endregion
    }
}