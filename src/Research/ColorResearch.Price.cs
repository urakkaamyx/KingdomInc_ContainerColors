using System;
using Assets.Behaviour.UI;
using Assets.Source.Player;
using UnityEngine;
using KingdomFoundation.Api;

namespace KingdomContainerColors
{
    internal static partial class ColorResearch
    {
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
