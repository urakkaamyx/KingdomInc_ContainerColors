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
    public sealed class ContainerPicker : MonoBehaviour
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
        static Color WithAlpha(Color c, float a) { c.a = a; return c; }

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
        internal void RefreshFromModel()
        {
            var deco = Popup.Decoration.Decoration;
            Color color = ColorData.TryGet(deco, out var saved) ? (Color)saved : WithAlpha(ColorData.Palette(deco)[Mathf.Clamp(deco.StyleIndex, 0, 4)], deco.Label != null ? 1 : .6f);
            Color.RGBToHSV(color, out hue, out saturation, out brightness); alpha = color.a; Refresh();
        }
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
        void Commit()
        {
            if (!ColorResearch.Unlocked) return;
            ColorData.Set(Popup.Decoration.Decoration, Current); Popup.Decoration.UpdateStyle(); Refresh();
        }
        internal void EnterHex(string text)
        {
            if (!ColorData.ParseHex(text, ((Color32)Current).a, out var color))
            {
                validation.text = "Use six hex digits, e.g. #AF78E2."; validation.color = new Color(1, .53f, .43f); return;
            }
            Color.RGBToHSV(color, out hue, out saturation, out brightness); Commit();
        }
        internal void ResetColor()
        {
            if (!ColorResearch.Unlocked) return;
            ColorData.Clear(Popup.Decoration.Decoration); Popup.Decoration.UpdateStyle(); RefreshFromModel();
        }
        void OnDisable()
        {
            if (HexField) HexField.DeactivateInputField();
            if (EventSystem.current && EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
                EventSystem.current.SetSelectedGameObject(null);
            RestoreInput();
            if (Window) Window.localScale = Vector3.one;
        }
        void RestoreInput()
        {
            if (restoreInput && Assets.Behaviour.UI.GameUI.Instance && !Assets.Behaviour.UI.GameUI.MenuVisible)
                Assets.Behaviour.Input.PlayerInput.Enable();
            restoreInput = false;
        }
        void LateUpdate()
        {
            if (!Window) return;
            // Fit within the native popup canvas even at low resolutions.
            var canvas = GetComponentInParent<Canvas>().rootCanvas;
            var bounds = (RectTransform)canvas.transform;
            float scale = Mathf.Min(1, (bounds.rect.height - 64) / Window.sizeDelta.y, (bounds.rect.width - 32) / Window.sizeDelta.x);
            Window.localScale = Vector3.one * Mathf.Max(.1f, scale);
        }
        PickerSurface Surface(string name, float x, float y, float w, float h, PickerMode mode)
        {
            var result = Rect(transform, name, x, y, w, h).gameObject.AddComponent<PickerSurface>(); result.Mode = mode; return result;
        }
        static readonly Color Paper = new Color(1, .94f, .79f), Muted = new Color(.74f, .69f, .61f);
        internal static RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var rect = (RectTransform)new GameObject(name, typeof(RectTransform)).transform; rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1); rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(w, h); return rect;
        }
        TMP_Text Label(Transform parent, string text, float x, float y, float w, float h, float size)
        {
            var label = Rect(parent, text, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>(); label.font = font; label.fontSize = size;
            label.color = Paper; label.text = text; label.richText = false; label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.NoWrap; label.alignment = TextAlignmentOptions.MidlineLeft; return label;
        }
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
