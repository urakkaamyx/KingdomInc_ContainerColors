using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class NativeColor
    {
        internal static void Prefix(UnityEngine.Component __instance)
        {
            var popup = FoundationApi.Decorations.PopupFor(__instance);
            if (ColorData.Supports(FoundationApi.Decorations.Current(popup)))
                ColorData.Clear(FoundationApi.Decorations.Current(popup));
        }

        internal static void Postfix(UnityEngine.Component __instance)
        {
            var popup = FoundationApi.Decorations.PopupFor(__instance);
            var picker = popup ? popup.GetComponentInChildren<ContainerPicker>(true) : null;
            if (picker && picker.gameObject.activeSelf)
                picker.RefreshFromModel();
        }
    }
}
