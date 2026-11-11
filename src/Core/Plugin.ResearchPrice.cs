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
        static bool ResearchPrice(TechNode __instance, ref CurrencyCollection __result)
        {
            if (__instance.Identifier != ColorResearch.TechId) return true;
            __result = ColorResearch.Price(); return false;
        }
    }
}
