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
        [HarmonyPatch(typeof(DecoOptionsStyle), "_updateSelection")]
        static partial class Selection
        {
            static void Postfix(DecoOptionsStyle __instance)
            {
                var popup = __instance.GetComponentInParent<DecoOptionsPopup>();
                if (popup && ColorData.TryGet(popup.Decoration?.Decoration, out _))
                    ((GameObject)AccessTools.Field(typeof(DecoOptionsStyle), "_selected").GetValue(__instance)).SetActive(false);
            }
        }
    }
}
