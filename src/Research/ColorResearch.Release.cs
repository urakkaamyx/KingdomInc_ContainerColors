using System;
using Assets.Behaviour.UI;
using Assets.Source.Player;
using UnityEngine;
using KingdomFoundation.Api;

namespace KingdomContainerColors
{
    internal static partial class ColorResearch
    {
        internal static void Release()
        {
            Registration = null;
        }
    }
}
