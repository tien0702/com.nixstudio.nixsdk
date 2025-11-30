using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public class AlphaCheckImage : Image
    {
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenPoint, eventCamera, out Vector2 localPoint);

            Rect rect = rectTransform.rect;
            Vector2 uv = new Vector2((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height);

            if (sprite == null || sprite.texture == null)
                return false;

            Texture2D texture = sprite.texture;
            int x = Mathf.RoundToInt(uv.x * texture.width);
            int y = Mathf.RoundToInt(uv.y * texture.height);

            Color color = texture.GetPixel(x, y);
            return color.a > 0.1f;
        }
    }
}