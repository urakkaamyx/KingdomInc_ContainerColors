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
    [BepInPlugin(Id, "Container Colors", Version)]
    [BepInDependency("urakka.kingdominc.foundation")]
    public sealed partial class Plugin : BaseUnityPlugin
    {
        public const string Id = "urakka.kingdominc.containercolors", Version = "1.3.0";

        FoundationModContext modContext;
        internal Harmony hooks;

        void Awake()
        {
            modContext = FoundationApi.ForMod(Id);
            ColorResearch.EnsureRegistered(modContext);
            hooks = new Harmony(Id);
            foreach (var patch in new[] { typeof(SaveColor), typeof(LoadColor), typeof(CopyColor), typeof(RenderColor), typeof(OpenPicker), typeof(NativeColor), typeof(Selection) })
                hooks.CreateClassProcessor(patch).Patch();

            Logger.LogInfo("Container Colors ready: color picker with native save fallback.");
        }
    }
}
