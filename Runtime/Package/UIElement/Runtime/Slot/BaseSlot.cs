using System;
using UnityEngine;

namespace NIX.Packages
{
    public class BaseSlot : MonoBehaviour
    {
        [SerializeField] protected RectTransform _Content;

        protected virtual void Reset()
        {
            _Content = transform.Find("Content") as RectTransform;
        }
    }
}