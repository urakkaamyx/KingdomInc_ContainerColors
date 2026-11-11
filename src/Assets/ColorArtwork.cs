using System;
using System.IO;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static partial class ColorArtwork
    {
        internal const string IconId = "ContainerColors_RainbowBrushPlus";
        internal const int IconSize = 32;
        static Sprite sprite;
        static Texture2D texture;
        internal static Sprite Sprite
        {
            get
            {
                if (sprite) return sprite;
                using (var stream = typeof(ColorArtwork).Assembly.GetManifestResourceStream("ContainerColors.Icon.png"))
                using (var buffer = new MemoryStream())
                {
                    if (stream == null) throw new InvalidOperationException("Container Colors icon resource is missing.");
                    stream.CopyTo(buffer);
                    var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    try
                    {
                        if (!ImageConversion.LoadImage(source, buffer.ToArray(), false)) throw new InvalidDataException("Container Colors icon could not be decoded.");
                        var original = source.GetPixels32(); var pixels = new Color32[IconSize * IconSize];
                        // Sample onto the game's native pixel grid once. The high-
                        // resolution source remains available for listing artwork.
                        for (int y = 0; y < IconSize; y++) for (int x = 0; x < IconSize; x++)
                        {
                            int sx = Mathf.Min(source.width - 1, (int)((x + .5f) * source.width / IconSize));
                            int sy = Mathf.Min(source.height - 1, (int)((y + .5f) * source.height / IconSize));
                            pixels[y * IconSize + x] = original[sy * source.width + sx];
                        }
                        texture = new Texture2D(IconSize, IconSize, TextureFormat.RGBA32, false) {
                            name = IconId, filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp
                        };
                        texture.SetPixels32(pixels); texture.Apply(false, false);
                        sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, IconSize, IconSize), new Vector2(.5f, .5f), IconSize);
                        sprite.name = IconId; return sprite;
                    }
                    finally { UnityEngine.Object.Destroy(source); }
                }
            }
        }

        internal static void Release()
        {
            if (sprite) UnityEngine.Object.Destroy(sprite); if (texture) UnityEngine.Object.Destroy(texture);
            sprite = null; texture = null;
        }
    }
}
