using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class PickerSurface : MaskableGraphic, IPointerDownHandler, IDragHandler
    {
        public void OnDrag(PointerEventData data) { if (data.button == PointerEventData.InputButton.Left) Select(data); }
    }
}
