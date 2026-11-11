using System;
using Assets.Behaviour.UI;
using Assets.Source.Player;
using UnityEngine;
using KingdomFoundation.Api;

namespace KingdomContainerColors
{
    internal static partial class ColorResearch
    {
        static void OnUnlock(ITechNode node)
        {
            var popup = GameUI.Instance ? GameUI.Instance.DecoPopup : null;
            if (popup && popup.gameObject.activeInHierarchy) ContainerPicker.Open(popup);
        }
    }
}
