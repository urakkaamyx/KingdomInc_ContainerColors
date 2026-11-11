using System;
using Assets.Behaviour.UI;
using Assets.Source.Player;
using UnityEngine;
using KingdomFoundation.Api;

namespace KingdomContainerColors
{
    internal static partial class ColorResearch
    {
        internal const string TechId = "container_colors";
        internal const string PreviousId = "deco_sticker";
        internal const double PriceMultiplier = 1.25;
        internal static bool Unlocked => GamePlayer.Current != null && GamePlayer.Current.HasTech(TechNode.Get(TechId));

        internal static ITechNode Registration { get; private set; }

        internal static void EnsureRegistered(FoundationModContext owner)
        {
            if (Registration?.IsRegistered == true) return;
            var stickers = TechNode.Get(PreviousId) ?? throw new InvalidOperationException("Stickers research is unavailable.");
            // Retain the saved ID; native prerequisites belong in Requirements,
            // while Previous/Root links are reserved for Foundation-owned nodes.
            Registration = owner.Register(new TechNodeDefinition(TechId, "Container Colors", Plugin.Id, "Container Colors", new Vector2(-800, 0),
                "Unlock a color picker for containers and labels. Choose custom colors, enter hex RGB values, and adjust opacity.",
                ColorArtwork.Sprite, new TechNodeOptions(nodeType: TechNodeType.Decoration, costTier: stickers.CostTier,
                    costMultiplier: PriceMultiplier, requirements: new[] { PreviousId })));
            var native = ((INativeTechNode)Registration).NativeNode;
            native.CostCurrency = stickers.CostCurrency;
            native.ResetCost();
            Registration.Clicked += OnUnlock;
        }
    }
}
