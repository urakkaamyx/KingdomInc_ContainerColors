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
        internal static bool TryGet(FlowDeco deco, out Color32 color)
        {
            if (Supports(deco) && colors.TryGetValue(deco, out var entry)) { color = entry.Color; return true; }
            color = default; return false;
        }
    }
}
