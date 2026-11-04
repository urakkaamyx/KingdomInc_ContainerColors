using System;
using Assets.Behaviour.UI;
using Assets.Source.Player;
using UnityEngine;

namespace KingdomContainerColors
{
    internal static class ColorResearch
    {
        internal const string TechId = "container_colors";
        internal const string PreviousId = "deco_sticker";
        internal const double PriceMultiplier = 1.25;
        internal static bool Unlocked => GamePlayer.Current != null && GamePlayer.Current.HasTech(TechNode.Get(TechId));

        internal static void EnsureRegistered()
        {
            if (TechNode.Get(TechId) != null) return;
            var stickers = TechNode.Get(PreviousId);
            if (stickers == null) throw new InvalidOperationException("Stickers research is unavailable.");
            TechNode.Add(new TechNode(TechId) {
                Name = "Container Colors",
                Description = "Unlock a color picker for containers and labels. Choose custom colors, enter hex RGB values, and adjust opacity.",
                NodeType = TechNodeType.Decoration, Previous = stickers,
                RelativePosition = new Vector2Int(-180, 0), IconName = ColorArtwork.IconId,
                CostTier = stickers.CostTier, CostCurrency = stickers.CostCurrency,
                CostMultiplier = PriceMultiplier,
                OnUnlock = player => {
                    var popup = GameUI.Instance ? GameUI.Instance.DecoPopup : null;
                    if (popup && popup.gameObject.activeInHierarchy) ContainerPicker.Open(popup);
                }
            });
        }
        internal static CurrencyCollection Price()
        {
            var price = new CurrencyCollection();
            // Derive from Stickers' actual price, including prestige reductions.
            // A separate seeded native price could otherwise end up cheaper.
            foreach (var entry in TechNode.Get(PreviousId).GetCost().GetCurrency())
                price.Add(entry.c, (long)Math.Ceiling(entry.amt * PriceMultiplier));
            return price;
        }
    }
}
