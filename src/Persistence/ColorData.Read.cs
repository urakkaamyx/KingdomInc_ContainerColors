using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Assets.Source.Flow;
using LightJson;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static partial class ColorData
    {
        internal static void Read(JsonValue json, FlowDeco deco)
        {
            if (!Supports(deco) || !json.IsJsonObject) return;
            var extension = json[Key];
            if (!extension.IsJsonObject || !extension["version"].IsInteger || extension["version"].AsInteger != 1) return;
            var rgba = extension["rgba"];
            if (!rgba.IsJsonArray || rgba.AsJsonArray.Count != 4) return;
            for (int i = 0; i < 4; i++) if (!rgba[i].IsInteger || rgba[i].AsInteger < 0 || rgba[i].AsInteger > 255) return;
            colors.GetValue(deco, _ => new Entry()).Color = new Color32((byte)rgba[0].AsInteger, (byte)rgba[1].AsInteger, (byte)rgba[2].AsInteger, (byte)rgba[3].AsInteger);
            deco.StyleIndex = Mathf.Clamp(deco.StyleIndex, 0, 4);
        }
    }
}
