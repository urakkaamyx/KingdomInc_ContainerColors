using Assets.Behaviour.Flow;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomContainerColors
{
    public sealed partial class ColorBackground : MonoBehaviour
    {
        internal Image Image;
        internal Color NativeTint;
        internal bool Applied;
    }
}
