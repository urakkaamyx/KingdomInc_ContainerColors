using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    internal static partial class ColorRendering
    {
        internal static void EnsurePalette(WorldFlowDeco world)
        {
            if (!ColorData.Supports(world.Decoration) || NeutralFor(world)) return;
            int family = world.Decoration.Label != null ? 1 : 0;
            var native = ColorData.Palette(world.Decoration);
            var palette = Palette.GetValue(world) as Sprite[];
            if (palette == null || palette.Length < 5) return;
            for (int i = 0; i < 5; i++)
            {
                var copy = ReadSprite(palette[i]);
                native[i] = copy.GetPixel(copy.width / 2, copy.height / 2);
                if (i == 1)
                {
                    var pixels = copy.GetPixels32();
                    var center = copy.GetPixel(copy.width / 2, copy.height / 2);
                    float level = Mathf.Max(.01f, center.grayscale);
                    for (int j = 0; j < pixels.Length; j++)
                    {
                        byte gray = (byte)Mathf.RoundToInt(Mathf.Clamp01(((Color)pixels[j]).grayscale / level) * 255);
                        pixels[j] = new Color32(gray, gray, gray, pixels[j].a);
                    }
                    copy.name = "ContainerColorsNeutralTexture"; copy.SetPixels32(pixels); copy.Apply(false, true); textures[family] = copy;
                    var neutral = Sprite.Create(copy, new Rect(0, 0, copy.width, copy.height), new Vector2(.5f, .5f),
                        palette[i].pixelsPerUnit, 0, SpriteMeshType.FullRect, palette[i].border);
                    neutral.name = family == 1 ? "ContainerColorsLabelNeutral" : "ContainerColorsNeutral";
                    if (family == 1) LabelNeutral = neutral; else Neutral = neutral;
                }
                else Object.Destroy(copy);
            }
        }
    }
}
