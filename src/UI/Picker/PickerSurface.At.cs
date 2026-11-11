using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class PickerSurface : MaskableGraphic, IPointerDownHandler, IDragHandler
    {
        Color At(float x, float y)
        {
            if (Mode == PickerMode.SaturationValue) return Color.HSVToRGB(Hue, x, y);
            if (Mode == PickerMode.Hue) return Color.HSVToRGB(1 - y, 1, 1);
            var c = Color.HSVToRGB(Hue, Saturation, Brightness); c.a = x; return c;
        }
    }
}
