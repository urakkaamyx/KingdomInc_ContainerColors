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
        [HarmonyPatch(typeof(DecoOptionsStyle), nameof(DecoOptionsStyle.StyleSelected))]
        static partial class NativeColor
        {
            static void Prefix(DecoOptionsStyle __instance)
            {
                var popup = __instance.GetComponentInParent<DecoOptionsPopup>();
                if (ColorData.Supports(popup?.Decoration?.Decoration)) ColorData.Clear(popup.Decoration.Decoration);
            }
        }
    }
}
