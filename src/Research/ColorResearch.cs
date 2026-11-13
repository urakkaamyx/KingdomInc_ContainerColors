using System;
using UnityEngine;
using KingdomFoundation.Api;

namespace KingdomContainerColors
{
    internal static class ColorResearch
    {
        internal const string TechId = "container_colors";
        internal const string PreviousId = "deco_sticker";
        internal const double PriceMultiplier = 1.25;
        internal static bool Unlocked => FoundationApi.World.IsInKingdom && FoundationApi.Progression.HasTech(TechId);
        internal static ITechNode Registration { get; private set; }

        internal static void EnsureRegistered(FoundationModContext owner)
        {
            if (Registration?.IsRegistered == true)
                return;
            var stickers = FoundationApi.ResearchAccess.Find(PreviousId) ?? throw new InvalidOperationException("Stickers research is unavailable.");
            // Retain the saved ID; native prerequisites belong in Requirements,
            // while Previous/Root links are reserved for Foundation-owned nodes.
            Registration = owner.Register(new TechNodeDefinition(TechId, "Container Colors", Plugin.Id, "Container Colors", new Vector2(-800, 0), "Unlock a color picker for containers and labels. Choose custom colors, enter hex RGB values, and adjust opacity.", ColorArtwork.Sprite, TechNodeOptions.Create(nodeKind: "Decoration", costTier: stickers.CostTier, costMultiplier: PriceMultiplier, requirements: new[] { PreviousId })));
            var native = FoundationApi.ResearchAccess.Find(Registration.Id);
            native.SetCostCurrencies(stickers.CostCurrencies);
            native.ResetCost();
            Registration.Clicked += OnUnlock;
        }

        static void OnUnlock(ITechNode node)
        {
            var popup = FoundationApi.Decorations.Popup;
            if (popup && popup.gameObject.activeInHierarchy)
                ContainerPicker.Open(popup);
        }

        internal static GameMoney Price()
        {
            var price = new GameMoney();
            // Derive from Stickers' actual price, including prestige reductions.
            // A separate seeded native price could otherwise end up cheaper.
            foreach (var entry in FoundationApi.ResearchAccess.Find(PreviousId).GetCost().Entries)
                price.Add(entry.CurrencyId, (long)Math.Ceiling(entry.Amount * PriceMultiplier));
            return price;
        }

        internal static void Release()
        {
            Registration = null;
        }
    }
}
