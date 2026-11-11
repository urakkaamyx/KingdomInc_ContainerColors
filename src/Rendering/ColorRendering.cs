using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    internal static partial class ColorRendering
    {
        internal static readonly System.Reflection.FieldInfo Background = AccessTools.Field(typeof(WorldFlowDeco), "_background");
        static readonly System.Reflection.FieldInfo Palette = AccessTools.Field(typeof(WorldFlowDeco), "_backgroundColors");
        internal static Sprite Neutral { get; private set; }
        internal static Sprite LabelNeutral { get; private set; }
        static readonly Texture2D[] textures = new Texture2D[2];

        static Sprite NeutralFor(WorldFlowDeco world) => world.Decoration.Label != null ? LabelNeutral : Neutral;
    }
}
