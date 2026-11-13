using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class LoadColor
    {
        internal static void Postfix(JsonValue json, WorkspaceObject __result) => ColorData.Read(json, __result);
    }
}
