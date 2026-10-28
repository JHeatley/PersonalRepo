using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ruby &amp; Sapphire Trainers (RS 83, 85, 89; v149). Unlimited only. The set's other Trainers and its Energy cards are prints of
    /// cards already in the pool (Energy Removal 2, Energy Restore and Pokémon Reversal Expedition's; Energy Switch, Lum Berry, Poké
    /// Ball, PokéNav, Energy Search, Potion and Switch the newer cards Limitless files them with; Darkness and Metal Energy Neo
    /// Genesis's; Rainbow Energy Celestial Storm's). Oran Berry works like Healing Berry (your ruling there): at the end of each
    /// turn, before Pokémon Checkup.
    /// </summary>
    public static class CardsRST
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RS-83", new LadyOuting());
            r.RegisterByKey("RS-85", new OranBerry());
            r.RegisterByKey("RS-89", new ProfessorBirch());
        }

        /// <summary>Lady Outing (Supporter): up to 3 basic Energy cards of different types from your deck into your hand.</summary>
        private sealed class LadyOuting : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var taken = new List<CardInstance>();
                for (int i = 0; i < 3; i++)
                {
                    var types = new HashSet<EnergyType>(taken.Select(c => c.Def.EnergyProvides));
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && !types.Contains(d.EnergyProvides), 1,
                        "Lady Outing: choose a basic Energy card of a type you haven't taken (" + (i + 1) + " of up to 3)");
                    if (found.Count == 0) break;
                    taken.AddRange(found);
                }
                ctx.Me.Hand.AddRange(taken);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Oran Berry (Pokémon Tool): at the end of each turn (before Pokémon Checkup), with 2 or more damage counters, 2 come off and the berry is discarded.</summary>
        private sealed class OranBerry : CardScript
        {
            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || !holder.Tools.Contains(src.Card) || holder.DamageCounters < 2) return;
                src.Log("Oran Berry: " + holder.Name + " has " + holder.DamageCounters + " damage counters.");
                src.Heal(holder, 20);
                src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Professor Birch (Supporter): draw cards until you have 6 cards in your hand.</summary>
        private sealed class ProfessorBirch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) < 6;
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(6); }
        }
    }
}
