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
        internal static void Open(DecoOptionsPopup popup)
        {
            var existing = popup.GetComponentInChildren<ContainerPicker>(true);
            bool supported = ColorData.Supports(popup.Decoration?.Decoration) && ColorResearch.Unlocked;
            if (!supported) { if (existing) existing.gameObject.SetActive(false); return; }
            if (!existing)
            {
                var window = (RectTransform)AccessTools.Field(typeof(DecoOptionsPopup), "_window").GetValue(popup);
                var rect = Rect(window, "ContainerColorPicker", 16, 210, 468, 358);
                existing = rect.gameObject.AddComponent<ContainerPicker>(); existing.Popup = popup; existing.Window = window;
                existing.font = ((TMP_InputField)AccessTools.Field(typeof(DecoOptionsPopup), "_labelText").GetValue(popup)).textComponent.font;
                existing.Build();
            }
            float top = popup.Decoration.Decoration.Label != null ? 260 : 210;
            ((RectTransform)existing.transform).anchoredPosition = new Vector2(16, -top);
            existing.Window.sizeDelta = new Vector2(existing.Window.sizeDelta.x, top + 430);
            existing.gameObject.SetActive(true);
            ColorRendering.EnsurePalette(popup.Decoration);
            existing.RefreshFromModel();
        }
    }
}
