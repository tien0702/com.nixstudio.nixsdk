using System;
using System.Collections.Generic;
using UnityEngine;

namespace NIX.Packages
{
    public class IconSelector : MonoBehaviour
    {
        protected Dictionary<string, GameObject> _Icons = new();

        protected bool _Initialized = false;

        protected virtual void Awake()
        {
            if (!_Initialized) Initialize();
        }

        protected void Initialize()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                var go = transform.GetChild(i).gameObject;
                string key = go.name;
                _Icons.Add(key, go);
            }

            _Initialized = true;
        }

        public virtual void ActiveIcon(string iconName)
        {
            /*if(!_Initialized) Initialize();
            DisableAll();

            if (_Icons.TryGetValue(iconName, out var icon))
            {
                icon.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Icon Name {iconName} not found");
            }*/
            for (int i = 0; i < transform.childCount; ++i)
            {
                var go = transform.GetChild(i).gameObject;
                go.SetActive(go.name == iconName);
            }
        }

        public virtual void ActiveIcons(List<string> iconNames)
        {
            DisableAll();

            foreach (var iconName in iconNames)
            {
                if (_Icons.TryGetValue(iconName, out var icon))
                    icon.SetActive(true);
            }
        }

        public virtual void DisableAll()
        {
            foreach (var i in _Icons.Values)
            {
                i.SetActive(false);
            }
        }
    }
}