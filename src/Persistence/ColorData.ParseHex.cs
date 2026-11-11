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
        internal static bool ParseHex(string text, byte alpha, out Color32 color)
        {
            text = (text ?? "").Trim().TrimStart('#');
            if (text.Length == 6 && uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint number))
            {
                color = new Color32((byte)(number >> 16), (byte)(number >> 8), (byte)number, alpha); return true;
            }
            color = default; return false;
        }
    }
}
