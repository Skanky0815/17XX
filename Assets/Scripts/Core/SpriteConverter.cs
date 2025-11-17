using UnityEngine;

namespace Core
{
    public static class SpriteConverter
    {
        public static Texture2D ToTexture(Sprite sprite)
        {
            var texRect = sprite.textureRect;
            var x = Mathf.FloorToInt(texRect.x);
            var y = Mathf.FloorToInt(texRect.y);
            var width = Mathf.FloorToInt(texRect.width);
            var height = Mathf.FloorToInt(texRect.height);

            Texture2D cropped = new(width, height, TextureFormat.RGBA32, false);
            var pixels = sprite.texture.GetPixels(x, y, width, height);

            var pixels32 = new Color32[pixels.Length];
            for (var i = 0; i < pixels.Length; i++)
                pixels32[i] = pixels[i];

            cropped.SetPixels32(0, 0, width, height, pixels32);
            cropped.Apply();
            return cropped;
        }
    }
}