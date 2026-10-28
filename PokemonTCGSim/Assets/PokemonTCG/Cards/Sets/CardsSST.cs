using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sandstorm Trainer and Energy cards (SS 86-93; v158). Rare Candy 88 is Mega Evolution's and Mysterious Fossil 91 is
    /// Fossil's (prints). Claw Fossil and Root Fossil are played as 40-HP Basic Colorless Pokémon like the other fossils.
    /// </summary>
    public static class CardsSST
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SS-86", new DoubleFullHeal());
            r.RegisterByKey("SS-87", new LanettesNetSearch());
            r.RegisterByKey("SS-89", new WallysTraining());
            r.RegisterByKey("SS-90", new CardsASC.CounterattackMon("Jagged Stone", 1, null));
            r.RegisterByKey("SS-92", new CardsSS.RainDishMon("Spongy Stone", null));
            r.RegisterByKey("SS-93", new MultiEnergy());
        }

        /// <summary>Double Full Heal: remove all Special Conditions from your Active Pokémon (playable only if it has one, like Full Heal).</summary>
        private sealed class DoubleFullHeal : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.Conditions != SpecialCondition.None; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active != null) ctx.Engine.RemoveAllConditions(active);
            }
        }

        /// <summary>Lanette's Net Search (Supporter): up to 3 Basic Pokémon cards of different types (not Baby Pokémon) from your deck into your hand.</summary>
        private sealed class LanettesNetSearch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var taken = new List<CardInstance>();
                for (int i = 0; i < 3; i++)
                {
                    var types = new HashSet<EnergyType>(taken.Select(c => c.Def.PokemonType));
                    var found = ctx.SearchMyDeck(d => d.IsBasicPokemon && !d.IsBaby && !types.Contains(d.PokemonType), 1,
                        "Lanette's Net Search: choose a Basic Pokémon of a type you haven't taken (" + (i + 1) + " of up to 3)");
                    if (found.Count == 0) break;
                    taken.AddRange(found);
                }
                ctx.Me.Hand.AddRange(taken);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Wally's Training (Supporter): a card that evolves from your Active Pokémon, from your deck onto it. No evolving timing limits (your ruling).</summary>
        private sealed class WallysTraining : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                CardsEVS1.EvolveFromDeck(ctx, ctx.Me.Active, "Wally's Training");
            }
        }

        /// <summary>
        /// Multi Energy: every type, 1 Energy at a time; Colorless while the Pokémon has any other Special Energy card attached (your
        /// ruling: checked all the time, so 2 Multi Energy both give Colorless).
        /// </summary>
        private sealed class MultiEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var holder = ctx.Pokemon;
                bool otherSpecial = holder != null && holder.Energy.Any(e => e != ctx.Card && !e.Def.IsBasicEnergy);
                return RecifeEnergy.Units(otherSpecial ? EnergyMask.Colorless : EnergyMask.AnyType, 1);
            }
        }
    }
}
