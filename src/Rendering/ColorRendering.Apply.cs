using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    internal static partial class ColorRendering
    {
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
    }
}
