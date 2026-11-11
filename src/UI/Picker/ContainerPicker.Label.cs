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
        TMP_Text Label(Transform parent, string text, float x, float y, float w, float h, float size)
        {
            var label = Rect(parent, text, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>(); label.font = font; label.fontSize = size;
            label.color = Paper; label.text = text; label.richText = false; label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.NoWrap; label.alignment = TextAlignmentOptions.MidlineLeft; return label;
        }
    }
}
