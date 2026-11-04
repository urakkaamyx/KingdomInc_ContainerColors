using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed class ColorBackground : MonoBehaviour
    {
        internal Image Image;
        internal Color NativeTint;
        internal bool Applied;
    }
    internal static class ColorRendering
    {
        internal static readonly System.Reflection.FieldInfo Background = AccessTools.Field(typeof(WorldFlowDeco), "_background");
        static readonly System.Reflection.FieldInfo Palette = AccessTools.Field(typeof(WorldFlowDeco), "_backgroundColors");
        internal static Sprite Neutral { get; private set; }
        internal static Sprite LabelNeutral { get; private set; }
        static readonly Texture2D[] textures = new Texture2D[2];
        static Sprite NeutralFor(WorldFlowDeco world) => world.Decoration.Label != null ? LabelNeutral : Neutral;

        internal static void Apply(WorldFlowDeco world)
        {
            if (!ColorData.Supports(world.Decoration)) return;
            var tracker = world.GetComponent<ColorBackground>();
            if (!ColorData.TryGet(world.Decoration, out var color))
            {
                if (tracker && tracker.Applied) { tracker.Image.color = tracker.NativeTint; tracker.Applied = false; }
                return;
            }
            var image = (Image)Background.GetValue(world);
            if (!image) return;
            EnsurePalette(world);
            var neutral = NeutralFor(world);
            if (!neutral) return;
            if (!tracker)
            {
                tracker = world.gameObject.AddComponent<ColorBackground>(); tracker.Image = image; tracker.NativeTint = image.color;
            }
            image.sprite = neutral; image.color = color; tracker.Applied = true;
        }
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
        static Texture2D ReadSprite(Sprite sprite)
        {
            var source = sprite.texture;
            var temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            var previous = RenderTexture.active;
            try
            {
                Graphics.Blit(source, temporary); RenderTexture.active = temporary;
                var rect = sprite.rect;
                var result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
                result.ReadPixels(rect, 0, 0); result.Apply(); result.filterMode = FilterMode.Point; result.wrapMode = TextureWrapMode.Clamp;
                return result;
            }
            finally { RenderTexture.active = previous; RenderTexture.ReleaseTemporary(temporary); }
        }
        internal static void Release()
        {
            foreach (var tracker in Object.FindObjectsByType<ColorBackground>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (tracker.Image) tracker.Image.color = tracker.NativeTint;
                tracker.Applied = false;
                var world = tracker.GetComponent<WorldFlowDeco>(); if (world) world.UpdateStyle();
            }
            if (Neutral) Object.Destroy(Neutral); if (LabelNeutral) Object.Destroy(LabelNeutral);
            Neutral = null; LabelNeutral = null;
            for (int i = 0; i < textures.Length; i++) { if (textures[i]) Object.Destroy(textures[i]); textures[i] = null; }
        }
    }
}
