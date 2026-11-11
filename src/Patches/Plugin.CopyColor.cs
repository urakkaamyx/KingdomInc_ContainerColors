using System;
using System.Linq;
using Assets.Behaviour.Flow;
using Assets.Behaviour.UI.Deco;
using Assets.Source.Flow;
using Assets.Source.Player;
using BepInEx;
using HarmonyLib;
using LightJson;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    public sealed partial class Plugin : BaseUnityPlugin
    {
        [HarmonyPatch(typeof(FlowDeco), nameof(FlowDeco.CreateCopy))]
        static partial class CopyColor
        {
 static void Postfix(FlowDeco __instance, FlowDeco __result) => ColorData.Copy(__instance, __result);
        }
    }
}
