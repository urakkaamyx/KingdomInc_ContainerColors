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
        void OnDestroy()
        {
            modContext?.Dispose();
            ColorResearch.Release();
            hooks?.UnpatchSelf();
            foreach (var picker in FindObjectsByType<ContainerPicker>(FindObjectsInactive.Include, FindObjectsSortMode.None)) Destroy(picker.gameObject);
            ColorRendering.Release();
            ColorArtwork.Release();
        }
    }
}
