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
        void Start()
        {
            hooks.Patch(AccessTools.Method(typeof(TechNode), nameof(TechNode.GetCost)),
                prefix: new HarmonyMethod(typeof(Plugin), nameof(ResearchPrice)));
        }
    }
}
