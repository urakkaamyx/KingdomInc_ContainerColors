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
        internal static Color[] Palette(FlowDeco deco) => deco.Label != null ? LabelNative : Native;
    }
}
