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
        void Build()
        {
            Label(transform, "Custom color", 0, 0, 360, 26, 21);
            SV = Surface("SaturationBrightness", 0, 38, 270, 180, PickerMode.SaturationValue);
            HueStrip = Surface("Hue", 284, 38, 24, 180, PickerMode.Hue);
            SV.Changed = p => { saturation = p.x; brightness = p.y; Commit(); };
            HueStrip.Changed = p => { hue = 1 - p.y; Commit(); };
            Rect(transform, "PreviewChecker", 324, 38, 144, 68).gameObject.AddComponent<CheckerGraphic>().raycastTarget = false;
            preview = Rect(transform, "ColorPreview", 324, 38, 144, 68).gameObject.AddComponent<Image>(); preview.raycastTarget = false;
            Label(transform, "HEX", 324, 118, 144, 20, 13).color = Muted;
            HexField = Input("HexColor", 324, 143, 144, 36);
            HexField.characterLimit = 7;
            HexField.onEndEdit.AddListener(EnterHex);
            HexField.onSelect.AddListener(_ => {
                restoreInput = Assets.Behaviour.Input.PlayerInput.Enabled;
                Assets.Behaviour.Input.PlayerInput.Disable();
            });
            HexField.onDeselect.AddListener(_ => RestoreInput());
            rgb = Label(transform, "", 324, 188, 144, 27, 11);
            Label(transform, "Opacity", 0, 242, 80, 24, 16);
            Rect(transform, "AlphaChecker", 92, 242, 294, 24).gameObject.AddComponent<CheckerGraphic>().raycastTarget = false;
            AlphaStrip = Surface("Opacity", 92, 242, 294, 24, PickerMode.Opacity);
            AlphaStrip.Changed = p => { alpha = p.x; Commit(); };
            opacity = Label(transform, "", 404, 242, 64, 24, 16);
            string[] presets = { "F1A43C", "E9D361", "F078A9", "AF78E2", "6798F2", "55CFE3", "54C1AD", "88BF59", "E8DED2", "737586" };
            for (int i = 0; i < presets.Length; i++)
            {
                ColorData.ParseHex(presets[i], 255, out var color); var selected = color;
                var image = Rect(transform, "Preset_" + presets[i], i * 47, 288, 42, 28).gameObject.AddComponent<Image>(); image.color = color;
                var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
                button.onClick.AddListener(() => { Color.RGBToHSV(selected, out hue, out saturation, out brightness); Commit(); });
            }
            validation = Label(transform, "Changes apply immediately.", 0, 332, 300, 24, 12); validation.color = Muted;
            var resetImage = Rect(transform, "ResetColor", 324, 328, 144, 32).gameObject.AddComponent<Image>(); resetImage.color = new Color(.23f, .19f, .16f);
            ResetButton = resetImage.gameObject.AddComponent<Button>(); ResetButton.targetGraphic = resetImage;
            Label(resetImage.transform, "Original color", 0, 0, 144, 32, 15).alignment = TextAlignmentOptions.Center;
            ResetButton.onClick.AddListener(ResetColor);
        }
    }
}
