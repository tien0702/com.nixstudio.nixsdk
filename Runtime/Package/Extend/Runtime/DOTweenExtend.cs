/*using System;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace NIX.Extension
{
    public static class DOTweenExtend
    {
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

        /// <summary>
        /// Move along an arc (curved path) in world space.
        /// </summary>
        public static Tween DOMoveArc(this Transform target, Vector3 endPos, float height, float duration)
        {
            Vector3 startPos = target.position;
            return DoArcPath(target, startPos, endPos, height, duration, false);
        }

        /// <summary>
        /// Move along an arc (curved path) in local space.
        /// </summary>
        public static Tween DOLocalMoveArc(this Transform target, Vector3 endLocalPos, float height, float duration)
        {
            Vector3 startLocalPos = target.localPosition;
            return DoArcPath(target, startLocalPos, endLocalPos, height, duration, true);
        }

        /// <summary>
        /// Shared logic for creating arc path.
        /// </summary>
        private static Tween DoArcPath(Transform target, Vector3 start, Vector3 end, float height, float duration,
            bool isLocal)
        {
            // Midpoint
            Vector3 midPoint = (start + end) * 0.5f;

            // Perpendicular direction (XY plane assumption, adjust if 3D full needed)
            Vector3 dir = (end - start).normalized;
            Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

            // Offset midpoint
            midPoint += perp * height;

            // Path points
            Vector3[] path = new Vector3[] { midPoint, end };

            if (isLocal)
                return target.DOLocalPath(path, duration, PathType.CatmullRom).SetEase(Ease.Linear);
            else
                return target.DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.Linear);
        }

        /// <summary>
        /// Tween an ulong value. If your DOTween build doesn't support ulong plugin,
        /// set useDoubleCompat = true to drive it via a double tween (rounded).
        /// </summary>
        public static Tween UInt(
            uint from, uint to, float duration,
            Action<uint> onUpdate,
            bool unscaledTime = false,
            Ease ease = Ease.OutQuad,
            UpdateType updateType = UpdateType.Normal,
            int delayMs = 0,
            TweenCallback onComplete = null,
            LinkBehaviour linkBehaviour = LinkBehaviour.KillOnDestroy,
            bool useDoubleCompat = false)
        {
            if (onUpdate == null) throw new ArgumentNullException(nameof(onUpdate));

            Tween t;

            if (!useDoubleCompat)
            {
                // Native uint tween (works if DOTween has UInt plugin)
                uint v = from;
                t = DOTween.To(() => v, x =>
                {
                    v = x;
                    onUpdate(x);
                }, to, duration);
            }
            else
            {
                // Compatibility via double tween. Round and clamp to [0, uint.MaxValue]
                double v = from;
                t = DOTween.To(() => v, x =>
                {
                    v = x;
                    if (x <= 0d)
                    {
                        onUpdate(0u);
                    }
                    else if (x >= uint.MaxValue)
                    {
                        onUpdate(uint.MaxValue);
                    }
                    else
                    {
                        onUpdate((uint)Math.Round(x));
                    }
                }, (double)to, duration);
            }

            t.SetEase(ease)
                .SetUpdate(updateType, unscaledTime);

            if (delayMs > 0) t.SetDelay(delayMs / 1000f);
            if (onComplete != null) t.OnComplete(onComplete);

            return t;
        }

        public static Tweener Double(double from, double to, float duration, Action<double> onVirtualUpdate)
        {
            double value = from;
            return DOTween.To(() => value, x =>
            {
                value = x;
                onVirtualUpdate?.Invoke(value);
            }, to, duration);
        }

        public static Sequence BounceOnce(this Transform target, float duration = 0.4f, float scaleFactor = 1.2f,
            float originFactor = 1f)
        {
            if (target == null) return null;

            Vector3 original = Vector3.one * originFactor;
            target.DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Append(target.DOScale(original * scaleFactor, duration * 0.5f).SetEase(Ease.OutQuad));
            seq.Append(target.DOScale(original, duration * 0.5f).SetEase(Ease.InQuad));

            return seq;
        }

        public static Sequence FlashOnce(
            this SpriteRenderer spriteRenderer,
            float duration = 0.4f)
        {
            if (spriteRenderer == null) return null;

            Material mat = spriteRenderer.material;
            mat.SetFloat(FlashAmount, 0f);

            Sequence seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => mat.GetFloat(FlashAmount),
                x => mat.SetFloat(FlashAmount, x),
                1f, duration * 0.5f));
            seq.Append(DOTween.To(() => mat.GetFloat(FlashAmount),
                x => mat.SetFloat(FlashAmount, x),
                0f, duration * 0.5f));

            return seq;
        }

        public static Sequence BounceWithFlashOnce(
            this SpriteRenderer target,
            float duration,
            float scaleFactor = 1.2f,
            float originFactor = 1f)
        {
            if (target == null) return null;

            Vector3 originalScale = Vector3.one * originFactor;
            target.DOKill();

            Material mat = target.material;
            mat.SetFloat(FlashAmount, 0f);
            Sequence seq = DOTween.Sequence();

            seq.Append(target.transform.DOScale(originalScale * scaleFactor, duration * 0.5f)
                .SetEase(Ease.OutQuad));
            seq.Join(DOTween.To(() => mat.GetFloat(FlashAmount),
                x => mat.SetFloat(FlashAmount, x),
                1f, duration * 0.5f));

            seq.Append(target.transform.DOScale(originalScale, duration * 0.5f)
                .SetEase(Ease.InQuad));
            seq.Join(DOTween.To(() => mat.GetFloat(FlashAmount),
                x => mat.SetFloat(FlashAmount, x),
                0f, duration * 0.5f));

            return seq;
        }
        
        public static Sequence DoRepeat(int times, float delay, Action<int> onTick)
        {
            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times), times, "Must be greater than zero.");
            }
            Sequence seq = DOTween.Sequence();

            for (int i = 0; i < times; i++)
            {
                if (i > 0)
                    seq.AppendInterval(delay);

                int index = i;
                seq.AppendCallback(() => onTick?.Invoke(index));
            }

            return seq;
        }
    }
}*/