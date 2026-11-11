using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class CheckerGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear(); var r = rectTransform.rect;
            for (float y = 0; y < r.height; y += 8) for (float x = 0; x < r.width; x += 8)
            {
                float shade = ((int)(x / 8) + (int)(y / 8)) % 2 == 0 ? .18f : .3f;
                PickerSurface.Quad(mesh, new Rect(r.xMin + x, r.yMin + y, Mathf.Min(8, r.width - x), Mathf.Min(8, r.height - y)), new Color(shade, shade, shade));
            }
        }
    }
}
