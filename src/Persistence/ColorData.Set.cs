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
        internal static void Set(FlowDeco deco, Color32 color)
        {
            if (!Supports(deco)) return;
            colors.GetValue(deco, _ => new Entry()).Color = color;
            deco.StyleIndex = Fallback(deco, color);
        }
    }
}
