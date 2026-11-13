using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class CopyColor
    {
        internal static void Postfix(WorkspaceObject __instance, WorkspaceObject __result) => ColorData.Copy(__instance, __result);
    }
}
