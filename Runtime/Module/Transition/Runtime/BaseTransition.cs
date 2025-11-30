using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NIX.Core.UI;
using UnityEngine;

namespace NIX.Module.Transition
{
    public abstract class BaseTransition : BaseUIVisual
    {
        [SerializeField] protected float _TimeTransition;

        public virtual async UniTask Transition(Func<Task> waitAction, Action onMiddle = null)
        {
            await TransitionIn();
            if (waitAction != null)
                await waitAction();
            onMiddle?.Invoke();
            await TransitionOut();
        }

        public abstract UniTask TransitionIn();
        public abstract UniTask TransitionOut();
    }
}