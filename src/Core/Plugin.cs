using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    [BepInPlugin(Id, "Container Colors", Version)]
    [BepInDependency("urakka.kingdominc.foundation", "0.14.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "urakka.kingdominc.containercolors", Version = "1.3.1";
        FoundationModContext modContext;
        readonly System.Collections.Generic.List<IDisposable> hooks = new System.Collections.Generic.List<IDisposable>();
        void Awake()
        {
            modContext = FoundationApi.ForMod(Id);
            ColorResearch.EnsureRegistered(modContext);
            RegisterHooks();
            Logger.LogInfo("Container Colors ready: color picker with native save fallback.");
        }

        private void RegisterHooks()
        {
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationSerialize, after: call =>
            {
                var result = call.Result<JsonValue>();
                global::KingdomContainerColors.SaveColor.Postfix((WorkspaceObject)call.Subject, ref result);
                call.SetResult(result);
            }));
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationDeserialize, after: call => global::KingdomContainerColors.LoadColor.Postfix(call.Argument<JsonValue>(0), call.Result<WorkspaceObject>())));
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationCopy, after: call => global::KingdomContainerColors.CopyColor.Postfix((WorkspaceObject)call.Subject, call.Result<WorkspaceObject>())));
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationStyleUpdate, after: call => global::KingdomContainerColors.RenderColor.Postfix(call.View)));
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationPopupOpen, after: call => global::KingdomContainerColors.OpenPicker.Postfix(call.View)));
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationStyleSelected, before: call => global::KingdomContainerColors.NativeColor.Prefix(call.View), after: call => global::KingdomContainerColors.NativeColor.Postfix(call.View)));
            hooks.Add(FoundationApi.GameHooks.Subscribe(Id, GameHookPoints.DecorationSelectionUpdate, after: call => global::KingdomContainerColors.Selection.Postfix(call.View)));
        }

        private void DisposeHooks()
        {
            foreach (var hook in hooks)
                hook.Dispose();
            hooks.Clear();
        }

        void OnDestroy()
        {
            modContext?.Dispose();
            ColorResearch.Release();
            DisposeHooks();
            foreach (var picker in FindObjectsByType<ContainerPicker>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                Destroy(picker.gameObject);
            ColorRendering.Release();
            ColorArtwork.Release();
        }

        static bool ResearchPrice(GameResearchNode __instance, ref GameMoney __result)
        {
            if (__instance.Identifier != ColorResearch.TechId)
                return true;
            __result = ColorResearch.Price();
            return false;
        }

        void Start()
        {
            hooks.Add(FoundationApi.GameHooks.SubscribeFiltered(Id, GameHookPoints.ResearchCost, new GameHookFilter(subjectId: ColorResearch.TechId), before: call =>
            {
                GameMoney result = null;
                if (!ResearchPrice((GameResearchNode)call.Subject, ref result))
                {
                    call.SetResult(result);
                    call.Cancel();
                }
            }));
        }
    }
}
