using System;
using System.Globalization;
using System.Linq;
using Assets.Behaviour.UI.Deco;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class ContainerPicker : MonoBehaviour
    {
        internal void RefreshFromModel()
        {
            var deco = Popup.Decoration.Decoration;
            Color color = ColorData.TryGet(deco, out var saved) ? (Color)saved : WithAlpha(ColorData.Palette(deco)[Mathf.Clamp(deco.StyleIndex, 0, 4)], deco.Label != null ? 1 : .6f);
            Color.RGBToHSV(color, out hue, out saturation, out brightness); alpha = color.a; Refresh();
        }
    }
}
