using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    internal static partial class ColorRendering
    {
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
