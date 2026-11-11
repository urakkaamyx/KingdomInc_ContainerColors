using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class PickerSurface : MaskableGraphic, IPointerDownHandler, IDragHandler
    {
        void Select(PointerEventData data)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out var local))
            {
                var rect = rectTransform.rect;
                Changed?.Invoke(new Vector2(Mathf.Clamp01((local.x - rect.xMin) / rect.width), Mathf.Clamp01((local.y - rect.yMin) / rect.height)));
            }
        }
    }
}
