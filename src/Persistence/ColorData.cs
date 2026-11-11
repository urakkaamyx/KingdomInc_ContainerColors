using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Assets.Source.Flow;
using LightJson;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static partial class ColorData
    {
        internal const string Key = "urakka.containercolors";
        static readonly ConditionalWeakTable<FlowDeco, Entry> colors = new ConditionalWeakTable<FlowDeco, Entry>();
        // Representative center colors of the native sprites, filled from the
        // installed palette by ColorRendering. These defaults also work before UI startup.
        internal static readonly Color[] Native = {
            new Color32(54, 45, 48, 255), new Color32(207, 169, 119, 255),
            new Color32(162, 69, 65, 255), new Color32(82, 126, 70, 255), new Color32(65, 100, 147, 255)
        };

        internal static readonly Color[] LabelNative = {
            new Color32(58, 44, 54, 255), new Color32(222, 189, 117, 255),
            new Color32(90, 27, 25, 255), new Color32(72, 89, 76, 255), new Color32(31, 57, 84, 255)
        };

        internal static bool Supports(FlowDeco deco) => deco != null && deco.Sticker == null;
    }
}
