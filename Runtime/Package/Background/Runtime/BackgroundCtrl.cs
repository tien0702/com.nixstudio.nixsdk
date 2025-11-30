using System.Collections.Generic;
using UnityEngine;

namespace NIX.Module
{
    public class BackgroundCtrl : MonoBehaviour
    {
        protected virtual void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                this.AddBackground(transform.GetChild(i).gameObject);
            }
        }

        protected Dictionary<string, GameObject> _BackgroundObjects = new();

        public virtual void AddBackground(GameObject background)
        {
            if (!_BackgroundObjects.TryAdd(background.name, background))
            {
                Debug.LogError("Duplicate Background");
            }
        }

        public virtual void RemoveBackground(string id)
        {
            if (!_BackgroundObjects.Remove(id))
            {
                Debug.Log("Background Not Found");
            }
        }

        public virtual void ActiveBackground(string id)
        {
            if (id == null) return;

            // Disable all Backgrounds
            foreach (var bg in _BackgroundObjects)
            {
                bg.Value.SetActive(false);
            }

            // Active Backgrounds
            if (_BackgroundObjects.TryGetValue(id, out var target))
            {
                target.SetActive(true);
            }
        }

        public virtual void ActiveBackgrounds(List<string> ids)
        {
            if (ids == null) return;

            // Disable all Backgrounds
            foreach (var bg in _BackgroundObjects)
            {
                bg.Value.gameObject.SetActive(false);
            }

            // Active Backgrounds
            ids.ForEach(id =>
            {
                if (_BackgroundObjects.TryGetValue(id, out var bg))
                {
                    bg.gameObject.SetActive(true);
                }
            });
        }
    }
}