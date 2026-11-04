# Container Colors 1.2.0

Choose any RGB color for Kingdom Inc containers and labels with a hue strip, saturation/brightness picker, six-digit hex entry, opacity control, and quick swatches. Changes preview immediately on the decoration. Native borders, resizing, locking, and anchoring are preserved.

## Use

Research **Container Colors** directly after **Stickers** on the decoration branch. Its research price is **25% higher than Stickers**, rounded up, using the same currencies and native prestige discounts. Then open a container or label's existing options with its edit button. Use the **Custom color** picker below the original styles. Drag the hue strip, choose saturation and brightness in the square, or enter a color such as `#AF78E2` and press Enter. Drag **Opacity** to adjust transparency. The original five style buttons restore a normal game color. **Original color** removes the custom color and uses its automatically chosen built-in fallback.

Labels use the same picker below their native text field, with their own background shape and full native opacity when reset. Text editing and automatic sizing remain available. Stickers retain their native behavior. Older custom-colored saves retain their appearance, but editing custom colors now requires the new research. The original five styles remain available before research.

## Installation and removal

Requires the Unity Mono build of Kingdom Inc and BepInEx 5. The player package contains `BepInEx/plugins/ContainerColors/ContainerColors.dll`; extract it into the game directory, then restart the game. Manual installation is supported; Vortex deployment is untested.

To remove, close the game and delete `BepInEx/plugins/ContainerColors`. Containers and labels continue to load with the nearest built-in color and normal built-in opacity. No cleanup tool or save conversion is needed. Saving again without Container Colors discards custom color metadata and its research unlock; back up your save first if you want those custom colors available when reinstalling later.

Custom colors are saved inside each decoration, alongside a valid native `StyleIndex` from 0 through 4. No extra container types, custom sprite indices, or external sidecar files are used. Native cloning and Sharable Blueprints use the same copy and serialization hooks.

The local installer supports `-StageForNextLaunch` when the game is running. A first installation loads on the next launch. Updates preserve a running game's mapped DLL before staging the replacement. No running game is closed by the installer.

## Build and verification

Keep the project at `<game>/_Mods/ContainerColors` so local game and BepInEx reference assemblies resolve. Requires a .NET SDK supporting .NET Standard 2.1. Packaging uses Python 3's standard library.

```powershell
dotnet build _Mods/ContainerColors/ContainerColors.csproj -c Release
& ./_Mods/ContainerColors/verify-isolated.ps1
& ./_Mods/ContainerColors/verify-isolated.ps1 -WithExistingPlugins
& ./_Mods/ContainerColors/install.ps1 -StageForNextLaunch
python _Mods/ContainerColors/Tools/package_nexus.py
```

The diagnostic creates a private runtime with a disposable kingdom, blocks normal save loading/writes and achievements, and exercises tech-tree placement, locked/available research, native purchase charging and save persistence, native rendering, picker raycasts, color/alpha/hex changes, invalid metadata, original styles, cloning, tab switching, full save round trips, and removal of all production patches followed by native save loading and world rendering. Companion mode also exercises the installed Sharable Blueprints capture/parse/import path. Verification JSON and gameplay screenshots stay local under `Verification/`.

Verified against Kingdom Inc **Release v1.0.10**, Unity **6000.3.27f2**, and BepInEx **5.4.23.6**: **131 isolated checks** and **134 companion checks** passed, with no Unity runtime errors. Companion mode loads all seven installed plugins; it verifies the color feature and blueprint path rather than every companion feature.

The research icon is a rainbow paintbrush with a beveled gold medieval plus, displayed on a crisp 32-pixel grid. Its original artwork and the template-based Nexus thumbnail were created with the built-in image generation tool.

The neutral container and label sprites is derived from the installed game at runtime and cached once per decoration family. Picker gradients use UI meshes. No game textures, game assemblies, BepInEx assemblies, or user saves are redistributed.

Plugin ID: `urakka.kingdominc.containercolors`. Assembly: `ContainerColors.dll`. Author: Urakka. Repository naming target: `urakkaamyx/KingdomInc_ContainerColors` (not created by this task). Source licensing is not yet selected.

Source repository: https://github.com/urakkaamyx/KingdomInc_ContainerColors
