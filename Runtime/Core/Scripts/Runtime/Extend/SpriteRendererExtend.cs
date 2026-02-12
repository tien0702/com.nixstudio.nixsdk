using UnityEngine;

namespace NIX.Core.Extend
{
    public static class SpriteRendererExtend
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int RendererColorId = Shader.PropertyToID("_RendererColor");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private static MaterialPropertyBlock _mpb;

        public static void SetAlpha(this SpriteRenderer sr, float alpha)
        {
            if (sr == null) return;
            if (_mpb == null) _mpb = new MaterialPropertyBlock();

            sr.GetPropertyBlock(_mpb);

            // Try _RendererColor first (URP 2D Sprite)
            if (sr.sharedMaterial.HasProperty(RendererColorId))
            {
                Color c = sr.color;
                c.a = alpha;
                _mpb.SetColor(RendererColorId, c);
            }
            // Fallback to _Color (Built-in)
            else if (sr.sharedMaterial.HasProperty(ColorId))
            {
                Color c = sr.color;
                c.a = alpha;
                _mpb.SetColor(ColorId, c);
            }
            // Fallback to _BaseColor
            else if (sr.sharedMaterial.HasProperty(BaseColorId))
            {
                Color c = sr.color;
                c.a = alpha;
                _mpb.SetColor(BaseColorId, c);
            }

            sr.SetPropertyBlock(_mpb);
        }
    }
}