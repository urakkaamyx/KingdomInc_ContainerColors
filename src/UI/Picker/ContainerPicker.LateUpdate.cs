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
        void LateUpdate()
        {
            if (!Window) return;
            // Fit within the native popup canvas even at low resolutions.
            var canvas = GetComponentInParent<Canvas>().rootCanvas;
            var bounds = (RectTransform)canvas.transform;
            float scale = Mathf.Min(1, (bounds.rect.height - 64) / Window.sizeDelta.y, (bounds.rect.width - 32) / Window.sizeDelta.x);
            Window.localScale = Vector3.one * Mathf.Max(.1f, scale);
        }
    }
}
