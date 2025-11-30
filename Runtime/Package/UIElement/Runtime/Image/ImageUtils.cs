using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public class ImageUtils : MonoBehaviour
    {
        public static void PivotBySprite(Image image)
        {
            if (image == null || image.sprite == null)
            {
                Debug.Log("Image or sprite is null!");
                return;
            }

            Vector2 spritePivot = image.sprite.pivot;

            spritePivot.x /= image.sprite.rect.width;
            spritePivot.y /= image.sprite.rect.height;

            image.rectTransform.pivot = spritePivot;
        }
    }
}