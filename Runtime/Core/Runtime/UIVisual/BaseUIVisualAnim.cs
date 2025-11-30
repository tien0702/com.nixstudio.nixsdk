using System;
using UnityEngine;

namespace NIX.Core.UI
{
    public abstract class BaseUIVisualAnim : MonoBehaviour
    {
        public abstract void Open(Action onOpened = null);

        public abstract void Close(Action onClosed = null);
    }
}