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
        internal DecoOptionsPopup Popup;
        internal PickerSurface SV, HueStrip, AlphaStrip;
        internal TMP_InputField HexField;
        internal Button ResetButton;
        internal RectTransform Window;
        TMP_FontAsset font;
        TMP_Text rgb, opacity, validation;
        Image preview;
        float hue, saturation, brightness, alpha;
        bool restoreInput;
        internal Color Current => WithAlpha(Color.HSVToRGB(hue, saturation, brightness), alpha);
        static readonly Color Paper = new Color(1, .94f, .79f), Muted = new Color(.74f, .69f, .61f);

        static Color WithAlpha(Color c, float a) { c.a = a; return c; }
    }
}
