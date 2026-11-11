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
        TMP_InputField Input(string name, float x, float y, float w, float h)
        {
            var image = Rect(transform, name, x, y, w, h).gameObject.AddComponent<Image>(); image.color = new Color(.1f, .085f, .085f);
            var viewport = Rect(image.transform, "Viewport", 8, 3, w - 16, h - 6); viewport.gameObject.AddComponent<RectMask2D>();
            var text = (TextMeshProUGUI)Label(viewport, "", 0, 0, w - 16, h - 6, 19); text.overflowMode = TextOverflowModes.Overflow;
            var input = image.gameObject.AddComponent<TMP_InputField>(); input.targetGraphic = image; input.textViewport = viewport; input.textComponent = text;
            input.lineType = TMP_InputField.LineType.SingleLine; input.richText = false; input.customCaretColor = true; input.caretColor = Paper;
            return input;
        }
    }
}
