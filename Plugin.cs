using System;
using System.Linq;
using Assets.Behaviour.Flow;
using Assets.Behaviour.UI.Deco;
using Assets.Source.Flow;
using Assets.Source.Player;
using BepInEx;
using HarmonyLib;
using LightJson;
using UnityEngine;

namespace KingdomContainerColors
{
    [BepInPlugin(Id, "Container Colors", Version)]
    [BepInDependency("urakka.kingdominc.foundation", "0.5.0")]
    public sealed partial class Plugin : BaseUnityPlugin
    {
        public const string Id = "urakka.kingdominc.containercolors", Version = "1.3.0";
        internal static bool Diagnostic => Environment.GetCommandLineArgs().Contains("-containercolors-smoke");
        internal Harmony hooks;
        void Awake()
        {
            ColorResearch.EnsureRegistered();
            hooks = new Harmony(Id);
            foreach (var patch in new[] { typeof(SaveColor), typeof(LoadColor), typeof(CopyColor), typeof(RenderColor), typeof(OpenPicker), typeof(NativeColor), typeof(Selection) })
                hooks.CreateClassProcessor(patch).Patch();
            if (Diagnostic) { InstallDiagnostics(); Application.runInBackground = true; StartCoroutine(RuntimeCheck()); }
            Logger.LogInfo("Container Colors ready: color picker with native save fallback.");
        }
        void Start()
        {
            hooks.Patch(AccessTools.Method(typeof(TechNode), nameof(TechNode.GetCost)),
                prefix: new HarmonyMethod(typeof(Plugin), nameof(ResearchPrice)));
        }
        static bool ResearchPrice(TechNode __instance, ref CurrencyCollection __result)
        {
            if (__instance.Identifier != ColorResearch.TechId) return true;
            __result = ColorResearch.Price(); return false;
        }
        void OnDestroy()
        {
            ColorResearch.Release();
            hooks?.UnpatchSelf();
            foreach (var picker in FindObjectsByType<ContainerPicker>(FindObjectsInactive.Include, FindObjectsSortMode.None)) Destroy(picker.gameObject);
            ColorRendering.Release();
            ColorArtwork.Release();
        }
        [HarmonyPatch(typeof(FlowDeco), nameof(FlowDeco.ToJson))]
        static class SaveColor { static void Postfix(FlowDeco __instance, ref JsonValue __result) => ColorData.Write(__instance, ref __result); }
        [HarmonyPatch(typeof(FlowDeco), nameof(FlowDeco.FromJson))]
        static class LoadColor { static void Postfix(JsonValue json, FlowDeco __result) => ColorData.Read(json, __result); }
        [HarmonyPatch(typeof(FlowDeco), nameof(FlowDeco.CreateCopy))]
        static class CopyColor { static void Postfix(FlowDeco __instance, FlowDeco __result) => ColorData.Copy(__instance, __result); }
        [HarmonyPatch(typeof(WorldFlowDeco), nameof(WorldFlowDeco.UpdateStyle))]
        static class RenderColor { static void Postfix(WorldFlowDeco __instance) => ColorRendering.Apply(__instance); }
        [HarmonyPatch(typeof(DecoOptionsPopup), "OnEnable")]
        static class OpenPicker { static void Postfix(DecoOptionsPopup __instance) => ContainerPicker.Open(__instance); }
        [HarmonyPatch(typeof(DecoOptionsStyle), nameof(DecoOptionsStyle.StyleSelected))]
        static class NativeColor
        {
            static void Prefix(DecoOptionsStyle __instance)
            {
                var popup = __instance.GetComponentInParent<DecoOptionsPopup>();
                if (ColorData.Supports(popup?.Decoration?.Decoration)) ColorData.Clear(popup.Decoration.Decoration);
            }
            static void Postfix(DecoOptionsStyle __instance)
            {
                var popup = __instance.GetComponentInParent<DecoOptionsPopup>();
                var picker = popup ? popup.GetComponentInChildren<ContainerPicker>(true) : null;
                if (picker && picker.gameObject.activeSelf) picker.RefreshFromModel();
            }
        }
        [HarmonyPatch(typeof(DecoOptionsStyle), "_updateSelection")]
        static class Selection
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
