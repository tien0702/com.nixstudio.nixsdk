using Cysharp.Threading.Tasks;
using NIX.Module.Transition;
using UnityEngine;

namespace NIX.Packages
{
    public class FadeTransition : BaseTransition
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public override void OnComponentAdded()
        {
            base.OnComponentAdded();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public override async UniTask TransitionIn()
        {
            gameObject.SetActive(true);
            float t = 0f;
            float time = _TimeTransition / 2f;
            while (t < time)
            {
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / time);
                await UniTask.Yield();
            }
        }

        public override async UniTask TransitionOut()
        {
            gameObject.SetActive(true);
            float t = 0f;
            float time = _TimeTransition / 2f;
            while (t < time)
            {
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / time);
                await UniTask.Yield();
            }

            gameObject.SetActive(false);
        }
    }
}