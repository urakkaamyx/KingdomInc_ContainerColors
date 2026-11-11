using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    internal static partial class ColorRendering
    {
        static Texture2D ReadSprite(Sprite sprite)
        {
            var source = sprite.texture;
            var temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            var previous = RenderTexture.active;
            try
            {
                Graphics.Blit(source, temporary); RenderTexture.active = temporary;
                var rect = sprite.rect;
                var result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
                result.ReadPixels(rect, 0, 0); result.Apply(); result.filterMode = FilterMode.Point; result.wrapMode = TextureWrapMode.Clamp;
                return result;
            }
            finally { RenderTexture.active = previous; RenderTexture.ReleaseTemporary(temporary); }
        }
    }
}
