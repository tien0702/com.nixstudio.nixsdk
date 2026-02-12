using UnityEngine;

namespace NIX.Core.DesignPatterns
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var objs = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (objs is { Length: > 0 })
                    _instance = objs[0];
                if (objs is { Length: > 1 })
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_instance == null) _instance = this as T;
        }
    }
}