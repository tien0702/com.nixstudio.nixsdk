using System;
using System.Collections.Generic;
using UnityEngine;

public static class StorageService
{
    private static readonly Dictionary<Type, BaseDataSO> Services = new();

    public static T Get<T>() where T : BaseDataSO
    {
        Type key = typeof(T);

        if (!Services.TryGetValue(key, out var service))
        {
            Debug.Log($"{key} not register");
            return default(T);
        }

        return (T)service;
    }

    public static void Register<T>(T service, bool replace = false) where T : BaseDataSO
    {
        Type key = service.GetType();
        if (Services.ContainsKey(key) && replace)
        {
            Services[key] = service;
            Debug.Log($"{key} replaced");
        }
        else
        {
            Services.Add(key, service);
        }
    }

    public static void Unregister<T>() where T : BaseDataSO
    {
        Type key = typeof(T);

        if (!Services.ContainsKey(key))
        {
            Debug.Log($"{key} is not registered");
        }
        else
        {
            Services.Remove(key);
        }
    }

    public static bool IsRegistered<T>() where T : BaseDataSO
    {
        return Services.ContainsKey(typeof(T));
    }
}