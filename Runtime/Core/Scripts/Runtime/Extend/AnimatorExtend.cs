using System;
using UnityEngine;

namespace NIX.Core.Extend
{
    public static class AnimatorExtend
    {
        public static async Awaitable WaitUntilAsync(Func<bool> condition)
        {
            while (!condition())
            {
                await Awaitable.NextFrameAsync();
            }
        }

        public static void Play(this Animator animator, string animName, Action onComplete)
        {
            _ = InternalPlay(animator, animName, onComplete);
        }

        private static async Awaitable InternalPlay(Animator animator, string animName, Action onComplete)
        {
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");
                return;
            }

            animator.Play(animName);

            await Awaitable.NextFrameAsync();

            await WaitUntilAsync(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName(animName)
            );

            float duration = animator.GetCurrentAnimatorStateInfo(0).length;

            await Awaitable.WaitForSecondsAsync(duration);

            onComplete?.Invoke();
        }
    }
}