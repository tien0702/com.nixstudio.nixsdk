using NIX.Core.DesignPatterns;
using UnityEngine;

namespace NIX.Packages
{
    public class TouchFeedbackCtrl : MonoBehaviour
    {
        [SerializeField] protected AudioClip _TouchSfx;
        protected Pooler _Pooler;

        private bool _isMobilePlatform = false;

        protected virtual void Awake()
        {
            _Pooler = GetComponent<Pooler>();
        }

        protected void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            _isMobilePlatform = true;
#endif
            if (!_isMobilePlatform) return;
            Input.simulateMouseWithTouches = false;
            Input.multiTouchEnabled = false;
        }

        protected void Update()
        {
            if (_isMobilePlatform && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == UnityEngine.TouchPhase.Began)
                {
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = 0;
                    ShowEffect(touchPosition, touch.position);
                }
            }

            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                ShowEffect(mousePosition, Input.mousePosition);
            }
        }

        protected virtual void ShowEffect(Vector3 position, Vector3 touchPosition)
        {
            var ef = _Pooler.GetObject<ParticleSystem>();
            ef.gameObject.SetActive(true);
            ef.transform.position = position;

            if (_TouchSfx != null)
            {
                //AudioManager.Instance.PlaySfx(_TouchSfx);
            }
        }
    }
}