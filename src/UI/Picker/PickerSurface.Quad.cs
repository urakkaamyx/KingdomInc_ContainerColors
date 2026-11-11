using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class PickerSurface : MaskableGraphic, IPointerDownHandler, IDragHandler
    {
        internal static void Quad(VertexHelper mesh, Rect rect, Color color) => Quad(mesh, rect, color, color, color, color);
    }
}
