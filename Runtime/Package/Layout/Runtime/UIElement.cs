using System;
using UnityEngine;

namespace NIX.Packages
{
    public class UIElement : MonoBehaviour
    {
        [SerializeField] protected RectTransform _Content;
        
        public RectTransform Content => _Content;

        private void Reset()
        {
            _Content = transform.Find("Content").GetComponent<RectTransform>();
        }
    }
}