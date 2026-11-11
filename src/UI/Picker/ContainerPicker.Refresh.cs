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
        void Refresh()
        {
            foreach (var surface in new[] { SV, HueStrip, AlphaStrip })
            {
                surface.Hue = hue; surface.Saturation = saturation; surface.Brightness = brightness; surface.Alpha = alpha; surface.SetVerticesDirty();
            }
            Color32 color = Current; preview.color = color;
            HexField.SetTextWithoutNotify(ColorData.Hex(color));
            rgb.text = "RGB " + color.r + " / " + color.g + " / " + color.b;
            opacity.text = Mathf.RoundToInt(alpha * 100) + "%";
            validation.text = "Changes apply immediately."; validation.color = Muted;
            // Popup.OnEnable runs before each child's OnEnable has assigned its
            // private parent. Set markers directly rather than calling that child
            // method during popup initialization.
            bool custom = ColorData.TryGet(Popup.Decoration.Decoration, out _);
            foreach (var choice in Popup.GetComponentsInChildren<DecoOptionsStyle>())
            {
                int index = (int)AccessTools.Field(typeof(DecoOptionsStyle), "_styleIndex").GetValue(choice);
                ((GameObject)AccessTools.Field(typeof(DecoOptionsStyle), "_selected").GetValue(choice)).SetActive(!custom && Popup.Decoration.Decoration.StyleIndex == index);
            }
        }
    }
}
