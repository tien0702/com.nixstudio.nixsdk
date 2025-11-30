using UnityEngine;

namespace NIX.Packages
{
    public enum DisplayType
    {
        Non,
        OnBackground,
        Lowest,
        Highest
    }

    public class MarkObject : MonoBehaviour
    {
        [Header("Information")] [SerializeField]
        protected DisplayType _DisplayType = DisplayType.Highest;

        protected virtual void OnTransformParentChanged()
        {
            ChangeSibling(_DisplayType);
            UpdateTransform();
        }

        protected virtual void ChangeSibling(DisplayType displayType)
        {
            switch (displayType)
            {
                case DisplayType.Lowest:
                    transform.SetAsFirstSibling();
                    break;
                case DisplayType.Highest:
                    transform.SetAsLastSibling();
                    break;
                case DisplayType.OnBackground:
                    transform.SetSiblingIndex(1);
                    break;
            }
        }

        protected virtual void UpdateTransform()
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}