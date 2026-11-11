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
        void Commit()
        {
            if (!ColorResearch.Unlocked) return;
            ColorData.Set(Popup.Decoration.Decoration, Current); Popup.Decoration.UpdateStyle(); Refresh();
        }
    }
}
