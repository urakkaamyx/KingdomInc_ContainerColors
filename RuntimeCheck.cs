using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Assets.Behaviour.Flow;
using Assets.Behaviour.UI;
using Assets.Behaviour.UI.Deco;
using Assets.Behaviour.UI.TechTree;
using Assets.Source.Flow;
using Assets.Source.Flow.Elements;
using Assets.Source.Player;
using Assets.Source.Util;
using BepInEx;
using HarmonyLib;
using LightJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class Plugin
    {
        readonly List<string> checks = new List<string>();
        readonly List<string> runtimeErrors = new List<string>();
        string stickersPrice, colorsPrice;
        void Check(bool condition, string text) { if (!condition) throw new InvalidOperationException(text); checks.Add(text); }
        static bool Equal(Color32 a, Color32 b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        static Image BackgroundOf(WorldFlowDeco world) => (Image)ColorRendering.Background.GetValue(world);
        internal static Vector2 ScreenPoint(RectTransform rect, float x = .5f, float y = .5f)
        {
            var local = new Vector3(Mathf.Lerp(rect.rect.xMin, rect.rect.xMax, x), Mathf.Lerp(rect.rect.yMin, rect.rect.yMax, y));
            return RectTransformUtility.WorldToScreenPoint(rect.GetComponentInParent<Canvas>().worldCamera, rect.TransformPoint(local));
        }
        void Drag(PickerSurface surface, float x, float y)
        {
            var point = ScreenPoint(surface.rectTransform, x, y);
            var hits = new List<RaycastResult>();
            var data = new PointerEventData(EventSystem.current) { position = point, button = PointerEventData.InputButton.Left };
            EventSystem.current.RaycastAll(data, hits);
            Check(hits.Count > 0 && hits[0].gameObject == surface.gameObject, "Picker receives topmost UI raycast: " + surface.name);
            data.pointerPressRaycast = hits[0]; surface.OnPointerDown(data); surface.OnDrag(data);
        }
        IEnumerator RuntimeCheck()
        {
            Application.logMessageReceived += WatchErrors;
            yield return new WaitForSecondsRealtime(5);
            string directory = Path.Combine(Paths.GameRootPath, "_Mods", "ContainerColors", "Verification"); Directory.CreateDirectory(directory);
            string error = null; FlowDeco custom = null, label = null; JsonObject saved = null; Color32 expected = default;
            try
            {
                Check(GamePlayer.Current == null && !GameUI.Instance, "No user kingdom or save loaded");
                ResearchChecks();
                ModelChecks();
                var player = GamePlayer.CreateNewPlayer();
                player.AddTech("tech_deco"); player.AddTech("deco_label");
                player.ActiveFlow.Elements.Clear(); player.ActiveFlow.Pipes.Clear(); player.ActiveFlow.Decorations.Clear();
                player.ActiveFlow.Label = "Container Colors verification";
                player.ActiveFlow.Elements.Add(new Manifold { Position = new Vector2Int(-520, 180) });
                custom = new FlowDeco { Position = new Vector2Int(-650, 280), Size = new Vector2Int(480, 400), IsParent = true };
                ColorData.Set(custom, new Color32(175, 120, 226, 153)); player.ActiveFlow.Decorations.Add(custom);
                for (int i = 0; i < 5; i++) player.ActiveFlow.Decorations.Add(new FlowDeco {
                    Position = new Vector2Int(-650 + i * 260, -220), Size = new Vector2Int(230, 200), StyleIndex = i });
                label = new FlowDeco { Label = "Custom label", Position = new Vector2Int(-200, 150), Size = new Vector2Int(200, 80), StyleIndex = 1 };
                player.ActiveFlow.Decorations.Add(label);
                player.Flows.Add(new FlowDiagram { Label = "Second color tab" });
                SceneManager.LoadScene("Game");
            }
            catch (Exception ex) { error = ex.ToString(); }
            yield return new WaitForSecondsRealtime(4);
            ContainerPicker picker = null;
            if (error == null) try
            {
                Check(custom.World, "Custom container built by native renderer");
                var image = BackgroundOf(custom.World);
                Check(image.sprite == ColorRendering.Neutral && image.type == Image.Type.Sliced, "Custom color retains sliced native border geometry");
                Check(Equal(image.color, new Color32(175, 120, 226, 153)), "Custom RGB and opacity render accurately");
                Check(ColorRendering.Neutral.border == new Vector4(8, 8, 8, 8), "Generated neutral sprite preserves all four native border widths");
                for (int i = 0; i < 5; i++)
                {
                    var native = GamePlayer.Current.ActiveFlow.Decorations[i + 1];
                    Check(BackgroundOf(native.World).sprite.name == new[] { "DecoRectDark", "DecoRectLight", "DecoRectRed", "DecoRectGreen", "DecoRectBlue" }[i], "Native style " + i + " retains its original sprite");
                    Check(!native.World.GetComponent<ColorBackground>(), "Native style " + i + " requires no custom renderer state");
                }
                WorldFlowCamera.Instance.SetZoomLevel(2);
                Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
                custom.World.OpenOptionsPopup();
                Check(!GameUI.Instance.DecoPopup.GetComponentInChildren<ContainerPicker>() && !ColorResearch.Unlocked,
                    "Container options retain only native controls before research");
                Check(((RectTransform)AccessTools.Field(typeof(DecoOptionsPopup), "_window").GetValue(GameUI.Instance.DecoPopup)).sizeDelta.y == 210,
                    "Locked picker does not leave empty space in the native popup");
                GameUI.Instance.TogglePopupMenu(GameUI.Instance.DecoPopup, false);
                label.World.OpenOptionsPopup();
                Check(!GameUI.Instance.DecoPopup.GetComponentInChildren<ContainerPicker>() && BackgroundOf(label.World).sprite.name == "DecoTitleYellow",
                    "Labels retain native controls and appearance before research");
                Check(((RectTransform)AccessTools.Field(typeof(DecoOptionsPopup), "_window").GetValue(GameUI.Instance.DecoPopup)).sizeDelta.y == 260,
                    "Locked label picker preserves native popup height");
                GameUI.Instance.TogglePopupMenu(GameUI.Instance.DecoPopup, false);
                VerifyResearchPurchase(directory);
                custom.World.OpenOptionsPopup();
                picker = GameUI.Instance.DecoPopup.GetComponentInChildren<ContainerPicker>();
                Check(picker && picker.gameObject.activeInHierarchy, "Opening native container options displays color picker");
                Canvas.ForceUpdateCanvases();
            }
            catch (Exception ex) { error = ex.ToString(); }
            yield return new WaitForSecondsRealtime(.25f);
            if (error == null) try
            {
                Capture(Path.Combine(directory, "container-color-picker.png"));
                picker.HexField.onSelect.Invoke("");
                Check(!Assets.Behaviour.Input.PlayerInput.Enabled, "Hex input disables gameplay keyboard shortcuts");
                picker.HexField.onDeselect.Invoke("");
                Check(Assets.Behaviour.Input.PlayerInput.Enabled, "Leaving hex input restores gameplay keyboard shortcuts");
                Drag(picker.HueStrip, .5f, .19f);
                Drag(picker.SV, .73f, .9f);
                Drag(picker.AlphaStrip, .82f, .5f);
                Check(ColorData.TryGet(custom, out var selected) && Equal(selected, picker.Current), "Dragging all three controls immediately updates the model");
                Check(Mathf.Abs(selected.a / 255f - .82f) < .01f, "Opacity control maps pointer position to saved alpha");
                picker.HexField.text = "#31C8D9"; picker.HexField.onEndEdit.Invoke(picker.HexField.text);
                Check(ColorData.TryGet(custom, out selected) && selected.r == 49 && selected.g == 200 && selected.b == 217, "Hex field accepts exact RGB values");
                var previous = selected; picker.HexField.text = "#ZZZZZZ"; picker.HexField.onEndEdit.Invoke(picker.HexField.text);
                Check(ColorData.TryGet(custom, out selected) && Equal(selected, previous), "Invalid hex leaves the previous color intact");
                picker.HexField.text = "#AF78E2"; picker.HexField.onEndEdit.Invoke(picker.HexField.text);
                expected = selected = (Color32)picker.Current;
                Check(custom.StyleIndex >= 0 && custom.StyleIndex < 5, "Custom choice always retains a built-in style index");
                custom.Locked = true; custom.World.UpdateStyle(); custom.IsParent = false; custom.World.UpdateStyle();
                Check(Equal(BackgroundOf(custom.World).color, expected), "Lock and anchor changes preserve custom color");
                custom.Locked = false; custom.IsParent = true; custom.World.UpdateStyle();
                var oldSize = custom.Size; custom.World.UpdateSize(custom.Position, oldSize + new Vector2Int(64, 64));
                Check(Equal(BackgroundOf(custom.World).color, expected), "Resizing preserves custom color");
                custom.World.UpdateSize(custom.Position, oldSize);
                custom.World.UpdateZoom(1); custom.World.UpdateZoom(2);
                Check(Equal(BackgroundOf(custom.World).color, expected), "Zoom changes preserve custom color");
                var copy = custom.CreateCopy(); copy.Position = new Vector2Int(250, 280);
                var copiedWorld = WorldFlowManager.Instance.CreateNewDeco(copy);
                Check(ColorData.TryGet(copy, out var copied) && Equal(copied, expected) && Equal(BackgroundOf(copiedWorld).color, expected), "Native clone copies custom metadata and appearance");
                WorldFlowManager.Instance.RemoveDecoration(copiedWorld);
                VerifyBlueprints(custom, directory);
                saved = SaveGame.SaveCurrentState();
                File.WriteAllText(Path.Combine(directory, "disposable-colored-save.json"), saved.ToString());
                Check(saved["Player"]["Flows"].IsJsonArray, "Full native save state is serialized");
                var choices = GameUI.Instance.DecoPopup.GetComponentsInChildren<DecoOptionsStyle>();
                var nativeChoice = choices.First(c => (int)AccessTools.Field(typeof(DecoOptionsStyle), "_styleIndex").GetValue(c) == 2);
                nativeChoice.StyleSelected();
                Check(!ColorData.TryGet(custom, out _) && custom.StyleIndex == 2 && BackgroundOf(custom.World).sprite.name == "DecoRectRed", "Original swatches clear custom color and restore their native sprite");
                Check(Mathf.Abs(BackgroundOf(custom.World).color.a - .6f) < .001f, "Restoring an original style restores native opacity");
                picker.HexField.onEndEdit.Invoke("#AF78E2"); picker.ResetButton.onClick.Invoke();
                Check(!ColorData.TryGet(custom, out _) && custom.ToJson()[ColorData.Key].IsNull, "Original color button removes custom save metadata");
                ColorData.Set(custom, expected); custom.World.UpdateStyle();
                GameUI.Instance.TogglePopupMenu(GameUI.Instance.DecoPopup, false);
                label.World.OpenOptionsPopup(); Canvas.ForceUpdateCanvases();
            }
            catch (Exception ex) { error = ex.ToString(); }
            yield return new WaitForSecondsRealtime(.25f);
            if (error == null) try
            {
                Check(picker.gameObject.activeInHierarchy && picker.Window.sizeDelta.y == 690 && ((RectTransform)picker.transform).anchoredPosition.y == -260,
                    "Label picker fits below the native text controls");
                Check(BackgroundOf(label.World).sprite.name == "DecoTitleYellow" && ((Color32)picker.Current).a == 255,
                    "Uncolored labels retain their native yellow sprite and full opacity");
                Drag(picker.HueStrip, .5f, .3f); Drag(picker.SV, .8f, .9f); Drag(picker.AlphaStrip, .74f, .5f);
                picker.HexField.onEndEdit.Invoke("#31C8D9"); var labelColor = (Color32)picker.Current;
                Check(ColorData.TryGet(label, out var labelSaved) && Equal(labelSaved, labelColor) && Equal(BackgroundOf(label.World).color, labelColor),
                    "Label hue, saturation, hex and opacity apply exact RGBA immediately");
                Check(BackgroundOf(label.World).sprite == ColorRendering.LabelNeutral && ColorRendering.LabelNeutral != ColorRendering.Neutral
                    && ColorRendering.LabelNeutral.rect.size == new Vector2(78, 18) && ColorRendering.LabelNeutral.border == new Vector4(8, 8, 8, 8),
                    "Custom labels preserve their own native sliced shape and borders");
                var labelInput = (TMPro.TMP_InputField)AccessTools.Field(typeof(DecoOptionsPopup), "_labelText").GetValue(picker.Popup);
                LayoutRebuilder.ForceRebuildLayoutImmediate(picker.Window); Canvas.ForceUpdateCanvases();
                var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = ScreenPoint((RectTransform)labelInput.transform) }, hits);
                Check(hits.Count > 0 && (hits[0].gameObject == labelInput.gameObject || hits[0].gameObject.transform.IsChildOf(labelInput.transform)),
                    "Native label text field remains reachable above the color picker: " + string.Join(",", hits.Select(h => h.gameObject.name)));
                labelInput.text = "Colored label"; picker.Popup.UpdateLabel();
                label.Locked = true; label.IsParent = true; label.World.UpdateStyle();
                Check(label.Label == "Colored label" && Equal(BackgroundOf(label.World).color, labelColor), "Editing label text, auto-sizing, locking and anchoring preserve custom color");
                var labelCopy = label.CreateCopy(); labelCopy.Position = new Vector2Int(250, 280);
                var labelCopyWorld = WorldFlowManager.Instance.CreateNewDeco(labelCopy);
                Check(ColorData.TryGet(labelCopy, out var copiedLabelColor) && Equal(copiedLabelColor, labelColor)
                    && Equal(BackgroundOf(labelCopyWorld).color, labelColor) && labelCopy.Label == label.Label, "Native label copy preserves text, color and opacity");
                WorldFlowManager.Instance.RemoveDecoration(labelCopyWorld);
                var labelChoice = picker.Popup.GetComponentsInChildren<DecoOptionsStyle>().First(c => (int)AccessTools.Field(typeof(DecoOptionsStyle), "_styleIndex").GetValue(c) == 2);
                labelChoice.StyleSelected();
                Check(!ColorData.TryGet(label, out _) && BackgroundOf(label.World).sprite.name == "DecoTitleRed" && BackgroundOf(label.World).color.a == 1,
                    "Native label swatches clear custom metadata and restore native opacity");
                picker.HexField.onEndEdit.Invoke("#31C8D9"); picker.ResetButton.onClick.Invoke();
                Check(!ColorData.TryGet(label, out _) && label.ToJson()[ColorData.Key].IsNull, "Label Original color removes custom save metadata");
                ColorData.Set(label, labelColor); label.World.UpdateStyle(); picker.RefreshFromModel();
                Capture(Path.Combine(directory, "label-color-picker.png"));
                VerifyBlueprints(custom, directory);
                saved = SaveGame.SaveCurrentState();
                File.WriteAllText(Path.Combine(directory, "disposable-colored-save.json"), saved.ToString());
                GameUI.Instance.TogglePopupMenu(GameUI.Instance.DecoPopup, false);
                WorldFlowManager.Instance.ShowFlow(1); WorldFlowManager.Instance.ShowFlow(0);
                Check(custom.World && Equal(BackgroundOf(custom.World).color, expected), "Switching workshop tabs preserves custom appearance");
                Check(label.World && ColorData.TryGet(label, out var tabColor) && Equal(BackgroundOf(label.World).color, tabColor), "Switching workshop tabs preserves label appearance");
                SaveGame.LoadState(JsonValue.Parse(saved.ToString()).AsJsonObject);
                Check(ColorResearch.Unlocked, "Purchased research survives full save reload");
                Check(ColorData.TryGet(GamePlayer.Current.Flows[0].Decorations[0], out var loaded) && Equal(loaded, expected), "Full save reload restores exact RGBA bytes");
                SceneManager.LoadScene("Game");
            }
            catch (Exception ex) { error = ex.ToString(); }
            yield return new WaitForSecondsRealtime(3);
            if (error == null) try
            {
                var reloaded = GamePlayer.Current.Flows[0].Decorations[0];
                Check(reloaded.World && Equal(BackgroundOf(reloaded.World).color, expected), "Save reload reconstructs the colored world container");
                reloaded.World.OpenOptionsPopup(); var reopened = GameUI.Instance.DecoPopup.GetComponentInChildren<ContainerPicker>();
                Check(reopened.HexField.text == ColorData.Hex(expected), "Reopened picker restores saved color");
                GameUI.Instance.TogglePopupMenu(GameUI.Instance.DecoPopup, false);
                var loadedLabel = GamePlayer.Current.Flows[0].Decorations.First(d => d.Label != null);
                ColorData.TryGet(loadedLabel, out var loadedLabelColor);
                Check(loadedLabel.Label == "Colored label" && ColorData.TryGet(loadedLabel, out _)
                    && Equal(BackgroundOf(loadedLabel.World).color, loadedLabelColor), "Full save reload restores label text and exact RGBA");
                loadedLabel.World.OpenOptionsPopup();
                Check(reopened.gameObject.activeInHierarchy && reopened.HexField.text == ColorData.Hex(loadedLabelColor), "Reopened label picker restores saved color");
                GameUI.Instance.TogglePopupMenu(GameUI.Instance.DecoPopup, false);
                // Remove every production hook while retaining only diagnostic save/
                // achievement guards. Newly loaded objects now run original game code.
                hooks.UnpatchSelf(); ColorRendering.Release();
                Check(!Harmony.HasAnyPatches(Id), "All Container Colors production patches removed");
                // Remove the custom lookup too, so native save loading must take
                // the same unknown-research path as a game without this plugin.
                ((Dictionary<string, TechNode>)AccessTools.Field(typeof(TechNode), "_allTech").GetValue(null)).Remove(ColorResearch.TechId);
                Check(TechNode.Get(ColorResearch.TechId) == null, "Removed plugin research identifier is unknown to the native loader");
                SaveGame.LoadState(JsonValue.Parse(saved.ToString()).AsJsonObject);
                Check(GamePlayer.Current.HasTech(ColorResearch.PreviousId), "Native save loader skips removed research while preserving Stickers");
                var fallback = GamePlayer.Current.Flows[0].Decorations[0];
                Check(!ColorData.TryGet(fallback, out _) && fallback.StyleIndex >= 0 && fallback.StyleIndex < 5, "Unmodified loader ignores color metadata and retains valid fallback");
                Check(fallback.ToJson()[ColorData.Key].IsNull, "Saving without mod discards custom metadata without losing container");
                SceneManager.LoadScene("Game");
            }
            catch (Exception ex) { error = ex.ToString(); }
            yield return new WaitForSecondsRealtime(3);
            if (error == null) try
            {
                var fallback = GamePlayer.Current.Flows[0].Decorations[0];
                Check(fallback.World && BackgroundOf(fallback.World).sprite.name.StartsWith("DecoRect"), "Unmodified world renderer loads custom-colored save with built-in fallback");
                Check(Mathf.Abs(BackgroundOf(fallback.World).color.a - .6f) < .001f, "Removal restores built-in opacity");
                var fallbackLabel = GamePlayer.Current.Flows[0].Decorations.First(d => d.Label != null);
                Check(fallbackLabel.World && BackgroundOf(fallbackLabel.World).sprite.name.StartsWith("DecoTitle") && BackgroundOf(fallbackLabel.World).color.a == 1
                    && !ColorData.TryGet(fallbackLabel, out _) && fallbackLabel.ToJson()[ColorData.Key].IsNull && fallbackLabel.Label == "Colored label",
                    "Unmodified world renderer loads colored labels with native fallback, full opacity and intact text");
                Capture(Path.Combine(directory, "container-colors-removed.png"));
                Check(runtimeErrors.Count == 0, "No Unity runtime errors or exceptions: " + string.Join(" | ", runtimeErrors));
            }
            catch (Exception ex) { error = ex.ToString(); }
            var report = new JsonObject {
                { "passed", error == null }, { "version", Version }, { "count", checks.Count }, { "error", error ?? "" },
                { "dllSha256", Hash(typeof(Plugin).Assembly.Location) }, { "gameVersion", Application.version }, { "unityVersion", Application.unityVersion },
                { "bepInExVersion", typeof(BaseUnityPlugin).Assembly.GetName().Version.ToString() },
                { "stickersPrice", stickersPrice }, { "containerColorsPrice", colorsPrice },
                { "checks", new JsonArray(checks.Select(s => new JsonValue(s)).ToArray()) }
            };
            File.WriteAllText(Path.Combine(directory, "runtime-tests.json"), report.ToString());
            if (error != null) Logger.LogError(error); else Logger.LogInfo("Container Colors diagnostic passed: " + checks.Count + " checks.");
            Application.Quit(error == null ? 0 : 1);
        }
        void WatchErrors(string condition, string trace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) runtimeErrors.Add(condition);
        }
        void ResearchChecks()
        {
            var fixture = new GamePlayer(); GamePlayer.Current = fixture;
            var research = TechNode.Get(ColorResearch.TechId); var stickers = TechNode.Get(ColorResearch.PreviousId);
            int count = TechNode.Count; ColorResearch.EnsureRegistered();
            Check(TechNode.Count == count && TechNode.Nodes.Count(t => t.Identifier == ColorResearch.TechId) == 1,
                "Research is registered once before player research arrays are allocated");
            Check(research != null && research.Previous == stickers && research.NodeType == TechNodeType.Decoration,
                "Container Colors research follows Stickers on the decoration branch");
            Check(research.Root == stickers.Root && research.Position == stickers.Position + new Vector2Int(-180, 0),
                "Research continues the native Stickers chain with matching spacing");
            Check(research.IconName == ColorArtwork.IconId && SpriteLibrary.Get(research.IconName) == ColorArtwork.Sprite,
                "Native research sprite lookup resolves the rainbow brush and medieval plus emblem");
            var icon = ColorArtwork.Sprite.texture; var pixels = icon.GetPixels32();
            Check(icon.width == 32 && icon.height == 32 && icon.filterMode == FilterMode.Point && pixels.Any(p => p.a == 0) && pixels.Any(p => p.a == 255),
                "Research icon uses a crisp 32-pixel grid and a genuinely transparent background");
            Check(pixels.Any(p => p.a > 200 && p.r > 190 && p.g < 70 && p.b < 100)
                && pixels.Any(p => p.a > 200 && p.g > 140 && p.r < 100 && p.b < 120)
                && pixels.Any(p => p.a > 200 && p.b > 140 && p.r < 100 && p.g < 140)
                && pixels.Any(p => p.a > 200 && p.r > 120 && p.b > 160 && p.g < 100),
                "Native-sized brush retains red, green, blue, and violet paint bands");
            Check(research.IsLocked && !ColorResearch.Unlocked, "New kingdoms require Stickers and Container Colors research");
            var nativeCost = stickers.GetCost().GetCurrency().ToArray(); var price = research.GetCost().GetCurrency().ToArray();
            Check(price.Select(c => c.c).SequenceEqual(nativeCost.Select(c => c.c)) && price.Zip(nativeCost, (a, b) =>
                a.amt == (long)Math.Ceiling(b.amt * 1.25) && a.amt > b.amt).All(v => v),
                "Research costs 25 percent more than the actual Stickers price in the same currencies");
            stickersPrice = string.Join(", ", nativeCost.Select(c => c.amt + " " + c.c));
            colorsPrice = string.Join(", ", price.Select(c => c.amt + " " + c.c));
            fixture.AddTech(stickers);
            Check(research.IsAvailable && !ColorResearch.Unlocked, "Stickers unlocks the research purchase without granting the picker");
            // Simulate upgrading a 1.0.0 save that predates the research identifier.
            fixture.Flows.Add(new FlowDiagram());
            var old = new FlowDeco { Size = new Vector2Int(320, 200) }; ColorData.Set(old, new Color32(33, 88, 177, 153));
            fixture.ActiveFlow.Decorations.Add(old);
            var reloaded = GamePlayer.FromJson(fixture.ToJson());
            Check(!ColorResearch.Unlocked && !reloaded.HasTech(research) && ColorData.TryGet(reloaded.ActiveFlow.Decorations[0], out _),
                "Earlier colored saves load without auto-granting research or discarding colors");
        }
        void VerifyResearchPurchase(string directory)
        {
            var research = TechNode.Get(ColorResearch.TechId); var player = GamePlayer.Current;
            GameUI.Instance.ToggleTechMenu(true); TechTreeMenu.Instance.FocusNode(research);
            var node = TechTreeMenu.Instance.Nodes.First(n => n.Tech == research);
            Check(node && node.GetTooltipTitle() == "Container Colors" && node.Tech.IsLocked,
                "Native tech tree displays Container Colors locked behind Stickers");
            var click = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
            node.OnPointerClick(click);
            Check(!ColorResearch.Unlocked, "Native purchase handler rejects research before Stickers");
            player.AddTech(ColorResearch.PreviousId); TechTreeMenu.Instance.UpdateTechTree();
            Check(research.IsAvailable, "Research becomes purchasable after Stickers is owned");
            player.SpendCurrency(player.GetCurrencies());
            node.OnPointerClick(click);
            Check(!ColorResearch.Unlocked, "Native purchase handler requires enough currency");
            var cost = research.GetCost(); player.AddCurrency(cost, false);
            Check(((Image)AccessTools.Field(typeof(UITechNode), "_icon").GetValue(node)).sprite == ColorArtwork.Sprite,
                "Rendered tech-tree node displays the custom brush icon");
            File.WriteAllBytes(Path.Combine(directory, "container-colors-icon-native.png"), ImageConversion.EncodeToPNG(ColorArtwork.Sprite.texture));
            Canvas.ForceUpdateCanvases(); Capture(Path.Combine(directory, "container-colors-research.png"));
            node.OnPointerClick(click);
            Check(ColorResearch.Unlocked && research.IsPurchased && player.GetCurrencies().IsEmpty,
                "Native research purchase spends the exact price and unlocks the picker");
            GameUI.Instance.ToggleTechMenu(false);
        }
        void ModelChecks()
        {
            foreach (var color in new[] { new Color32(255, 0, 0, 0), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 153), new Color32(175, 120, 226, 67), new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255) })
            {
                foreach (bool isLabel in new[] { false, true }) {
                var deco = new FlowDeco { Size = new Vector2Int(300, 200), Label = isLabel ? "Label round trip" : null }; ColorData.Set(deco, color);
                var json = deco.ToJson(); var restored = FlowDeco.FromJson(JsonValue.Parse(json.ToString()), new FlowDiagram());
                Check(json["StyleIndex"].AsInteger >= 0 && json["StyleIndex"].AsInteger < 5, "Valid native fallback for " + ColorData.Hex(color));
                Check(ColorData.TryGet(restored, out var read) && Equal(read, color), "Exact serialized RGBA round trip for " + ColorData.Hex(color));
                Check(ColorData.TryGet(restored.CreateCopy(), out read) && Equal(read, color), "Clone preserves RGBA for " + ColorData.Hex(color));
                }
            }
            var plain = new FlowDeco { StyleIndex = 3, Size = new Vector2Int(300, 200) };
            Check(plain.ToJson()[ColorData.Key].IsNull && !ColorData.TryGet(FlowDeco.FromJson(plain.ToJson(), new FlowDiagram()), out _), "Old saves require no custom color extension");
            var label = new FlowDeco { Label = "Label", StyleIndex = 1 }; ColorData.Set(label, new Color32(20, 40, 60, 80));
            Check(ColorData.TryGet(label, out _) && label.StyleIndex >= 0 && label.StyleIndex < 5, "Label color model retains a valid native fallback");
            ColorData.Set(label, ColorData.LabelNative[1]); Check(label.StyleIndex == 1, "Label fallback uses the label palette");
            var sticker = new FlowDeco { Sticker = DecoSticker.Create("IconSticker"), StyleIndex = 2 };
            ColorData.Set(sticker, new Color32(20, 40, 60, 80));
            Check(!ColorData.TryGet(sticker, out _) && sticker.StyleIndex == 2, "Sticker decorations remain unaffected");
            foreach (var malformed in new JsonValue[] { "bad", new JsonObject { { "version", 99 }, { "rgba", new JsonArray(1, 2, 3, 4) } },
                new JsonObject { { "version", 1 }, { "rgba", new JsonArray(-1, 2, 3, 4) } }, new JsonObject { { "version", 1 }, { "rgba", new JsonArray(1, 2, 256, 4) } },
                new JsonObject { { "version", 1 }, { "rgba", new JsonArray(1, 2, 3) } }, new JsonObject { { "version", 1 }, { "rgba", new JsonArray(1, 2, 3, "bad") } } })
            {
                var json = plain.ToJson(); json[ColorData.Key] = malformed;
                var loaded = FlowDeco.FromJson(json, new FlowDiagram()); Check(!ColorData.TryGet(loaded, out _) && loaded.StyleIndex == 3, "Malformed/unknown extension falls back safely: " + malformed.ToString());
            }
            Check(ColorData.ParseHex("  #aF78e2 ", 153, out var parsed) && ColorData.Hex(parsed) == "#AF78E2" && parsed.a == 153, "Hex parser accepts case and whitespace, retaining opacity");
            Check(!ColorData.ParseHex("#GG0000", 0, out _) && !ColorData.ParseHex("12345", 0, out _), "Hex parser rejects invalid/incomplete input");
        }
        void VerifyBlueprints(FlowDeco custom, string directory)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "SharableBlueprints");
            if (assembly == null) return;
            var box = Activator.CreateInstance(assembly.GetType("SharableBlueprints.SelectionBox"), -750, -150, 700, 350);
            var result = assembly.GetType("SharableBlueprints.BlueprintCapture").GetMethod("Capture").Invoke(null, new[] { GamePlayer.Current.ActiveFlow, box });
            var document = (JsonObject)result.GetType().GetField("Document").GetValue(result);
            var parsed = assembly.GetType("SharableBlueprints.PortableBlueprint").GetMethod("Parse").Invoke(null, new object[] { document.ToString() });
            var loadedDocument = (JsonObject)parsed.GetType().GetField("Document").GetValue(parsed);
            var flow = FlowDiagram.FromJson(loadedDocument["layout"]);
            var importedContainer = flow.Decorations.First(d => d.IsContainer);
            Check(ColorData.TryGet(importedContainer, out var color) && ColorData.TryGet(custom, out var expected) && Equal(color, expected), "Installed Sharable Blueprints capture/parse/import preserves exact RGBA");
            var sourceLabel = GamePlayer.Current.ActiveFlow.Decorations.FirstOrDefault(d => d.Label != null);
            if (ColorData.TryGet(sourceLabel, out var labelColor)) {
                var importedLabel = flow.Decorations.First(d => d.Label != null);
                Check(importedLabel.Label == sourceLabel.Label && ColorData.TryGet(importedLabel, out var actualLabelColor) && Equal(actualLabelColor, labelColor),
                    "Installed Sharable Blueprints capture/parse/import preserves label text and exact RGBA");
            }
            File.WriteAllText(Path.Combine(directory, "colored.kiblueprint.json"), document.ToString());
        }
        static string Hash(string path)
        {
            using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
        }
        static void Capture(string path)
        {
            // Overlay canvases do not appear in Camera.Render. Temporarily render
            // those existing canvases through the game camera for hidden-player QA.
            Canvas.ForceUpdateCanvases();
            var camera = TechTreeMenu.Visible ? (Camera)AccessTools.Field(typeof(TechTreeMenu), "_camera").GetValue(TechTreeMenu.Instance) : Camera.main;
            var overlays = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
                .Where(c => c.isRootCanvas && c.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
            var cameras = overlays.Select(c => c.worldCamera).ToArray();
            var distances = overlays.Select(c => c.planeDistance).ToArray();
            var orders = overlays.Select(c => c.sortingOrder).ToArray();
            var target = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            var previous = RenderTexture.active; var oldTarget = camera.targetTexture; int oldMask = camera.cullingMask;
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            try
            {
                foreach (var canvas in overlays) { canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10; canvas.sortingOrder = 30000; }
                camera.cullingMask = -1; camera.targetTexture = target;
                Canvas.ForceUpdateCanvases(); camera.Render();
                RenderTexture.active = target; texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0); texture.Apply();
                File.WriteAllBytes(path, ImageConversion.EncodeToPNG(texture));
            }
            finally
            {
                camera.targetTexture = oldTarget; camera.cullingMask = oldMask;
                for (int i = 0; i < overlays.Length; i++) { overlays[i].renderMode = RenderMode.ScreenSpaceOverlay; overlays[i].worldCamera = cameras[i]; overlays[i].planeDistance = distances[i]; overlays[i].sortingOrder = orders[i]; }
                Canvas.ForceUpdateCanvases();
                RenderTexture.active = previous; RenderTexture.ReleaseTemporary(target); Destroy(texture);
            }
        }
        void InstallDiagnostics()
        {
            var safety = new Harmony(Id + ".diagnostics");
            foreach (var type in new[] { typeof(NoSave), typeof(NoAutosave), typeof(NoLoad), typeof(NoAchievement), typeof(Offline) }) safety.CreateClassProcessor(type).Patch();
        }
        [HarmonyPatch(typeof(SaveGame), nameof(SaveGame.Store))] static class NoSave { static bool Prefix() => false; }
        [HarmonyPatch(typeof(SaveGame), nameof(SaveGame.StoreAutosaveState))] static class NoAutosave { static bool Prefix() => false; }
        [HarmonyPatch(typeof(SaveGame), nameof(SaveGame.LoadLatestSave))] static class NoLoad { static bool Prefix(ref bool __result) { __result = false; return false; } }
        [HarmonyPatch(typeof(SteamAchievement), nameof(SteamAchievement.Trigger))] static class NoAchievement { static bool Prefix() => false; }
        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Initialized), MethodType.Getter)] static class Offline { static bool Prefix(ref bool __result) { __result = false; return false; } }
    }
}
