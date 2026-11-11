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
        static partial class NativeColor
        {
            static void Postfix(DecoOptionsStyle __instance)
            {
                var popup = __instance.GetComponentInParent<DecoOptionsPopup>();
                var picker = popup ? popup.GetComponentInChildren<ContainerPicker>(true) : null;
                if (picker && picker.gameObject.activeSelf) picker.RefreshFromModel();
            }
        }
    }
}
