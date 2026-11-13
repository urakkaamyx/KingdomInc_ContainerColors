using KingdomFoundation.Api;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using KingdomFoundation.Json;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class ColorData
    {
        internal static void Clear(WorkspaceObject deco)
        {
            if (deco != null)
                colors.Remove(deco);
        }

        internal static void Copy(WorkspaceObject source, WorkspaceObject copy)
        {
            if (TryGet(source, out var color))
                colors.GetValue(copy, _ => new global::KingdomContainerColors.ColorDataEntry()).Color = color;
        }

        internal const string Key = "urakka.containercolors";
        static readonly ConditionalWeakTable<WorkspaceObject, global::KingdomContainerColors.ColorDataEntry> colors = new ConditionalWeakTable<WorkspaceObject, global::KingdomContainerColors.ColorDataEntry>();
        // Representative center colors of the native sprites, filled from the
        // installed palette by ColorRendering. These defaults also work before UI startup.
        internal static readonly Color[] Native =
        {
            new Color32(54, 45, 48, 255),
            new Color32(207, 169, 119, 255),
            new Color32(162, 69, 65, 255),
            new Color32(82, 126, 70, 255),
            new Color32(65, 100, 147, 255)
        };
        internal static readonly Color[] LabelNative =
        {
            new Color32(58, 44, 54, 255),
            new Color32(222, 189, 117, 255),
            new Color32(90, 27, 25, 255),
            new Color32(72, 89, 76, 255),
            new Color32(31, 57, 84, 255)
        };
        internal static bool Supports(WorkspaceObject deco) => deco != null && (deco.Kind == WorkspaceObjectKind.Container || deco.Kind == WorkspaceObjectKind.Label);
        internal static int Fallback(WorkspaceObject deco, Color color)
        {
            var native = Palette(deco);
            int best = 0;
            float distance = float.MaxValue;
            for (int i = 0; i < native.Length; i++)
            {
                var d = new Vector3(color.r - native[i].r, color.g - native[i].g, color.b - native[i].b);
                float score = d.sqrMagnitude;
                if (score < distance)
                {
                    best = i;
                    distance = score;
                }
            }

            return best;
        }

        internal static string Hex(Color32 color) => "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        internal static Color[] Palette(WorkspaceObject deco) => deco.Label != null ? LabelNative : Native;
        internal static bool ParseHex(string text, byte alpha, out Color32 color)
        {
            text = (text ?? "").Trim().TrimStart('#');
            if (text.Length == 6 && uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint number))
            {
                color = new Color32((byte)(number >> 16), (byte)(number >> 8), (byte)number, alpha);
                return true;
            }

            color = default;
            return false;
        }

        internal static void Read(JsonValue json, WorkspaceObject deco)
        {
            if (!Supports(deco) || !json.IsJsonObject)
                return;
            var extension = json[Key];
            if (!extension.IsJsonObject || !extension["version"].IsInteger || extension["version"].AsInteger != 1)
                return;
            var rgba = extension["rgba"];
            if (!rgba.IsJsonArray || rgba.AsJsonArray.Count != 4)
                return;
            for (int i = 0; i < 4; i++)
                if (!rgba[i].IsInteger || rgba[i].AsInteger < 0 || rgba[i].AsInteger > 255)
                    return;
            colors.GetValue(deco, _ => new global::KingdomContainerColors.ColorDataEntry()).Color = new Color32((byte)rgba[0].AsInteger, (byte)rgba[1].AsInteger, (byte)rgba[2].AsInteger, (byte)rgba[3].AsInteger);
            deco.StyleIndex = Mathf.Clamp(deco.StyleIndex, 0, 4);
        }

        internal static void Set(WorkspaceObject deco, Color32 color)
        {
            if (!Supports(deco))
                return;
            colors.GetValue(deco, _ => new global::KingdomContainerColors.ColorDataEntry()).Color = color;
            deco.StyleIndex = Fallback(deco, color);
        }

        internal static bool TryGet(WorkspaceObject deco, out Color32 color)
        {
            if (Supports(deco) && colors.TryGetValue(deco, out global::KingdomContainerColors.ColorDataEntry entry))
            {
                color = entry.Color;
                return true;
            }

            color = default;
            return false;
        }

        internal static void Write(WorkspaceObject deco, ref JsonValue json)
        {
            if (!TryGet(deco, out var color) || !json.IsJsonObject)
                return;
            // Never persist a custom sprite index. The unmodified game can ignore
            // this extension and still render a valid native style.
            json["StyleIndex"] = Mathf.Clamp(deco.StyleIndex, 0, 4);
            json[Key] = new JsonObject
            {
                {
                    "version",
                    1
                },
                {
                    "rgba",
                    new JsonArray((int)color.r, (int)color.g, (int)color.b, (int)color.a)
                }
            };
        }
    }
}
