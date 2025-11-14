using UnityEngine;

public class SpriteConverter
{
    public static Texture2D ToTexture(Sprite sprite)
    {
        Rect texRect = sprite.textureRect;
        int x = Mathf.FloorToInt(texRect.x);
        int y = Mathf.FloorToInt(texRect.y);
        int width = Mathf.FloorToInt(texRect.width);
        int height = Mathf.FloorToInt(texRect.height);

        Texture2D cropped = new(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = sprite.texture.GetPixels(x, y, width, height);

        Color32[] pixels32 = new Color32[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
            pixels32[i] = pixels[i];

        cropped.SetPixels32(0, 0, width, height, pixels32);
        cropped.Apply();
        return cropped;
    }
}