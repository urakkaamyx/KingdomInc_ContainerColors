using System;
using System.Linq;
using BepInEx;
using KingdomFoundation.Json;
using KingdomFoundation.Api;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class OpenPicker
    {
        internal static void Postfix(UnityEngine.Component __instance) => ContainerPicker.Open(__instance);
    }
}
