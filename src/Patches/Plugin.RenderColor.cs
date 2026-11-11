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
        [HarmonyPatch(typeof(WorldFlowDeco), nameof(WorldFlowDeco.UpdateStyle))]
        static partial class RenderColor
        {
 static void Postfix(WorldFlowDeco __instance) => ColorRendering.Apply(__instance);
        }
    }
}
