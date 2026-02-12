using System.Collections.Generic;
using UnityEngine;

namespace NIX.Core.Extend
{
    public static class TransformExtension
    {
        public static List<T> GetChildren<T>(this Transform transform, bool includeInactive = false) where T : Component
        {
            var children = new List<T>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf && !includeInactive) continue;
                if (transform.TryGetComponent(out T component))
                    children.Add(component);
            }

            return children;
        }

        public static void RemoveChildren(this Transform transform)
        {
            while (transform.childCount > 0)
            {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        public static T FindChildByTag<T>(this Transform parent, string tag) where T : Component
        {
            if (parent == null)
                return null;

            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        return component;
                }

                T found = child.FindChildByTag<T>(tag);
                if (found != null)
                    return found;
            }

            return null;
        }
        
        public static List<T> FindChildrenByTag<T>(this Transform parent, string tag) where T : Component
        {
            List<T> results = new List<T>();
            if (parent == null) return results;

            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        results.Add(component);
                }

                results.AddRange(child.FindChildrenByTag<T>(tag));
            }

            return results;
        }

        public static Transform FindChildByPrefix(this Transform parent, string prefix, bool recursive = true)
        {
            foreach (Transform child in parent)
            {
                if (child.name.StartsWith(prefix))
                    return child;

                if (recursive)
                {
                    var match = child.FindChildByPrefix(prefix, true);
                    if (match != null) return match;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first child whose name ends with the specified suffix.
        /// </summary>
        /// <param name="parent">Parent transform to search in.</param>
        /// <param name="suffix">Name suffix to match.</param>
        /// <param name="recursive">Whether to search all descendants recursively.</param>
        /// <returns>First matching Transform or null if none found.</returns>
        public static Transform FindChildBySuffix(this Transform parent, string suffix, bool recursive = true)
        {
            foreach (Transform child in parent)
            {
                if (child.name.EndsWith(suffix))
                    return child;

                if (recursive)
                {
                    var match = child.FindChildBySuffix(suffix, true);
                    if (match != null) return match;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all children whose names start with the specified prefix.
        /// </summary>
        /// <param name="parent">Parent transform to search in.</param>
        /// <param name="prefix">Name prefix to match.</param>
        /// <param name="recursive">Whether to search all descendants recursively.</param>
        /// <returns>List of all matching Transforms (empty if none).</returns>
        public static List<Transform> FindChildrenByPrefix(this Transform parent, string prefix, bool recursive = true)
        {
            var results = new List<Transform>();
            foreach (Transform child in parent)
            {
                if (child.name.StartsWith(prefix))
                    results.Add(child);
                if (recursive)
                    results.AddRange(child.FindChildrenByPrefix(prefix, true));
            }

            return results;
        }

        /// <summary>
        /// Finds all children whose names end with the specified suffix.
        /// </summary>
        /// <param name="parent">Parent transform to search in.</param>
        /// <param name="suffix">Name suffix to match.</param>
        /// <param name="recursive">Whether to search all descendants recursively.</param>
        /// <returns>List of all matching Transforms (empty if none).</returns>
        public static List<Transform> FindChildrenBySuffix(this Transform parent, string suffix, bool recursive = true)
        {
            var results = new List<Transform>();
            foreach (Transform child in parent)
            {
                if (child.name.EndsWith(suffix))
                    results.Add(child);
                if (recursive)
                    results.AddRange(child.FindChildrenBySuffix(suffix, true));
            }

            return results;
        }
    }
}