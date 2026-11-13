using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class SaveColor
    {
        internal static void Postfix(WorkspaceObject __instance, ref JsonValue __result) => ColorData.Write(__instance, ref __result);
    }
}
