using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public class FollowScrollRect : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        public float YOffset = 0f;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _scrollRect.onValueChanged.AddListener(OnValueChanged);

            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnValueChanged(Vector2 val)
        {
            float offset = _scrollRect.content.rect.height - _scrollRect.viewport.rect.height;
            Vector2 newPos = new Vector2(_rectTransform.anchoredPosition.x,
                (offset - _scrollRect.content.anchoredPosition.y) * (-1f) + YOffset);

            _rectTransform.anchoredPosition = newPos;
        }
    }
}