using System;
using System.Globalization;
using System.Linq;
using Assets.Behaviour.UI.Deco;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class ContainerPicker : MonoBehaviour
    {
        internal void ResetColor()
        {
            if (!ColorResearch.Unlocked) return;
            ColorData.Clear(Popup.Decoration.Decoration); Popup.Decoration.UpdateStyle(); RefreshFromModel();
        }
    }
}
