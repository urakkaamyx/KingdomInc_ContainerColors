using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class Selection
    {
        internal static void Postfix(UnityEngine.Component __instance)
        {
            var popup = FoundationApi.Decorations.PopupFor(__instance);
            if (popup && ColorData.TryGet(FoundationApi.Decorations.Current(popup), out _))
                FoundationApi.Decorations.SetChoiceSelected(__instance, false);
        }
    }
}
