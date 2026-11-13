# Custom Container Colors

**Current release: 1.3.1 — for Kingdom Inc game version 1.0.14.**

[GitHub releases](https://github.com/KingdomIncModding/ContainerColors/releases) · [Nexus Mods](https://www.nexusmods.com/kingdominc/mods/11) · Version 1.3.1

Choose any RGB color for containers and labels in Kingdom Inc with a live color picker, hex entry, opacity controls, and quick swatches. Labels keep their text editing and automatic sizing; containers keep their borders, resizing, locking, and anchoring.

## Features

- **Live Color Picker:** Choose hue, saturation, and brightness with real-time visual preview.
- **Hex Code Input:** Enter precise six-digit hex RGB codes (e.g. `#AF78E2`) with instant updates.
- **Opacity Slider:** Adjust transparency from fully opaque to subtle tinting.
- **Quick Swatches:** Instant access to palette presets plus the original five vanilla styles.
- **Label & Container Support:** Applies seamlessly across both workshop containers and standalone labels.
- **Persistent Storage:** Custom colors survive saving, reloading, and duplicating decorations.
- **Safe Fallback:** Built-in fallback colors keep saves intact and loadable if the mod is removed.

## Research

Find **Container Colors** under **Tech Tree > Mods** after unlocking **Stickers**. It costs 25% more than Stickers, rounded up, using the same currencies and prestige discounts. Existing research purchases remain unlocked after updating.

## Using the Picker

Open a container or label's edit/options menu. Choose a hue and then saturation and brightness, or enter six hex digits such as `#AF78E2` and press Enter. Drag **Opacity** to adjust transparency. Changes apply immediately.

The original five swatches restore native styles. **Original color** removes custom styling and uses the nearest built-in fallback. The label text field remains above the picker. Stickers keep their original behavior.

## Installation

1. Install [BepInEx 5 (Windows x64 Mono)](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.6).
2. Install [Foundation](https://www.nexusmods.com/kingdominc/mods/18) (0.14.0 or newer).
3. Extract the release ZIP into your Kingdom Inc game folder.
4. Confirm the file is located at `BepInEx/plugins/ContainerColors/ContainerColors.dll`.
5. Launch the game and research Container Colors in the Tech Tree!

## Compatibility

| Class.Method | Hook Type | Impact |
|---|---|---|
| FlowDeco.CreateCopy | Foundation hook (postfix) | Copies custom container color data when duplicating or copying decorations. |
| FlowDeco.FromJson | Foundation hook (postfix) | Restores saved custom container colors and opacity from kingdom save data. |
| DecoOptionsStyle.StyleSelected | Foundation hooks (prefix / postfix) | Handles selecting native style swatches and resetting custom colors. |
| DecoOptionsPopup.OnEnable | Foundation hook (postfix) | Injects the custom color picker and hex controls into the container decoration menu. |
| WorldFlowDeco.UpdateStyle | Foundation hook (postfix) | Renders custom colors and opacity onto container sprites and borders in the world. |
| FlowDeco.ToJson | Foundation hook (postfix) | Serializes custom container RGB and opacity values into kingdom save files. |
| DecoOptionsStyle._updateSelection | Foundation hook (postfix) | Highlights the active style swatch or clears selection when custom color is active. |

## Saves and Removal

Colors and opacity persist when saving and copying decorations. Close the game and delete `BepInEx/plugins/ContainerColors/` to remove the mod. Containers and labels load with built-in fallback colors and normal opacity.

Saving without the mod discards custom color data and its research unlock. Keep a save backup if you want those exact colors available after reinstalling.

## Credits and Source

Created by **Urakka** (rockyamyx). [Source code on GitHub](https://github.com/KingdomIncModding/ContainerColors). Research icon and promotional thumbnail use AI-generated artwork.