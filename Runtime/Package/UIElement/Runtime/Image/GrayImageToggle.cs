using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    [RequireComponent(typeof(Image))]
    public class GrayImageToggle : MonoBehaviour
    {
        private Image _image; // The UI Image component
        private Texture2D _originalTexture; // The original colored texture
        private Texture2D _grayscaleTexture; // The grayscale texture
        private bool _init = false;

        private void Start()
        {
            if (!_init) Init();
        }

        private bool Init()
        {
            _image = GetComponent<Image>();
            // Get the original texture from the Image's sprite
            _originalTexture = SpriteToTexture2D(_image.sprite);

            // Create the grayscale texture
            _grayscaleTexture = new Texture2D(_originalTexture.width, _originalTexture.height);
            ConvertToGrayscale();
            _init = true;

            return true;
        }

        public bool Init(Image targetImage)
        {
            _image = targetImage;
            // Get the original texture from the Image's sprite
            _originalTexture = SpriteToTexture2D(_image.sprite);

            // Create the grayscale texture
            _grayscaleTexture = new Texture2D(_originalTexture.width, _originalTexture.height);
            ConvertToGrayscale();
            _init = true;

            return true;
        }

        public bool IsInit() => _init;

        public bool LoadOriginTexture()
        {
            // Get the original texture from the Image's sprite
            _originalTexture = SpriteToTexture2D(_image.sprite);

            // Create the grayscale texture
            _grayscaleTexture = new Texture2D(_originalTexture.width, _originalTexture.height);
            ConvertToGrayscale();
            return true;
        }

        // Toggle between grayscale and original color
        public void SetState(bool val)
        {
            if (!_init) Init();
            _image.sprite = TextureToSprite(val
                ?
                // Set the grayscale texture
                _grayscaleTexture
                :
                // Set the original colored texture
                _originalTexture);
        }

        // Convert original texture to grayscale
        private void ConvertToGrayscale()
        {
            Color[] pixels = _originalTexture.GetPixels();

            // Loop through each pixel and convert to grayscale
            for (int i = 0; i < pixels.Length; i++)
            {
                float grayscaleValue = pixels[i].grayscale;
                pixels[i] = new Color(grayscaleValue, grayscaleValue, grayscaleValue, pixels[i].a);
            }

            // Apply the grayscale pixels to the grayscale texture
            _grayscaleTexture.SetPixels(pixels);
            _grayscaleTexture.Apply();
        }

        // Helper function to convert a sprite to a Texture2D
        Texture2D SpriteToTexture2D(Sprite sprite)
        {
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y,
                (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        // Helper function to convert a Texture2D back to a sprite
        Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

#if UNITY_EDITOR
        private bool _stateEditor = false;
        [ContextMenu("Editor Toggle")]
        public void Editor_Toggle()
        {
            _stateEditor = !_stateEditor;
            SetState(_stateEditor);
        }
#endif
    }
}