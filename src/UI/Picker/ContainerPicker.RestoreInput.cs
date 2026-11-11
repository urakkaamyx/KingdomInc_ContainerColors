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
        void RestoreInput()
        {
            if (restoreInput && Assets.Behaviour.UI.GameUI.Instance && !Assets.Behaviour.UI.GameUI.MenuVisible)
                Assets.Behaviour.Input.PlayerInput.Enable();
            restoreInput = false;
        }
    }
}
