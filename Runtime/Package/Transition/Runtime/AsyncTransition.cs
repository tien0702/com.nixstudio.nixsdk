using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NIX.Packages
{
    public abstract class AsyncTransition : MonoBehaviour
    {
        [SerializeField] protected float _TimeTransition;

        public virtual async Task Transition(Func<Task> waitAction, Action onMiddle = null)
        {
            gameObject.SetActive(true);

            await TransitionIn();

            if (waitAction != null)
                await waitAction();

            onMiddle?.Invoke();

            await TransitionOut();

            gameObject.SetActive(false);
        }

        protected abstract Task TransitionIn();
        protected abstract Task TransitionOut();
    }
}