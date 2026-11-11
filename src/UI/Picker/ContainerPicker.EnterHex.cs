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
        internal void EnterHex(string text)
        {
            if (!ColorData.ParseHex(text, ((Color32)Current).a, out var color))
            {
                validation.text = "Use six hex digits, e.g. #AF78E2."; validation.color = new Color(1, .53f, .43f); return;
            }
            Color.RGBToHSV(color, out hue, out saturation, out brightness); Commit();
        }
    }
}
