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
        internal static void Copy(FlowDeco source, FlowDeco copy)
        {
            if (TryGet(source, out var color)) colors.GetValue(copy, _ => new Entry()).Color = color;
        }
    }
}
