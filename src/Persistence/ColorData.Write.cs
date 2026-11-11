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
        internal static void Write(FlowDeco deco, ref JsonValue json)
        {
            if (!TryGet(deco, out var color) || !json.IsJsonObject) return;
            // Never persist a custom sprite index. The unmodified game can ignore
            // this extension and still render a valid native style.
            json["StyleIndex"] = Mathf.Clamp(deco.StyleIndex, 0, 4);
            json[Key] = new JsonObject { { "version", 1 }, { "rgba", new JsonArray((int)color.r, (int)color.g, (int)color.b, (int)color.a) } };
        }
    }
}
