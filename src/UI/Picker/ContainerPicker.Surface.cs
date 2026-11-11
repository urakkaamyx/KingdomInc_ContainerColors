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
        PickerSurface Surface(string name, float x, float y, float w, float h, PickerMode mode)
        {
            var result = Rect(transform, name, x, y, w, h).gameObject.AddComponent<PickerSurface>(); result.Mode = mode; return result;
        }
    }
}
