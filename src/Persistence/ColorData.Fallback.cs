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
        internal static int Fallback(FlowDeco deco, Color color)
        {
            var native = Palette(deco);
            int best = 0; float distance = float.MaxValue;
            for (int i = 0; i < native.Length; i++)
            {
                var d = new Vector3(color.r - native[i].r, color.g - native[i].g, color.b - native[i].b);
                float score = d.sqrMagnitude;
                if (score < distance) { best = i; distance = score; }
            }
            return best;
        }
    }
}
