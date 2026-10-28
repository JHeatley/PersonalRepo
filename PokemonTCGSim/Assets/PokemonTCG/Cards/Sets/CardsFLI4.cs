using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Forbidden Light (FLI, v84), part 4: Trainers and Special Energy (FLI 102-118, 143). Judge (POR), Lady (LOR), Unidentified
    /// Fossil (SIT) and Energy Recycler (DRI) were already in the pool. Pokémon Tools are not Item cards (your ruling, v90).
    /// </summary>
    public static class CardsFLI4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Beast Ring", new BeastRing());
            r.RegisterByName("Bonnie", new Bonnie());
            r.RegisterByName("Crasher Wake", new CrasherWake());
            r.RegisterByName("Diantha", new Diantha());
            r.RegisterByName("Eneporter", new Eneporter());
            r.RegisterByName("Fossil Excavation Map", new FossilExcavationMap());
            r.RegisterByName("Lysandre ◇", new LysandrePrismStar());
            r.RegisterByName("Lysandre Labs", new LysandreLabs());
            r.RegisterByName("Metal Frying Pan", new MetalFryingPan());
            r.RegisterByName("Mysterious Treasure", new MysteriousTreasure());
            r.RegisterByName("Ultra Recon Squad", new UltraReconSquad());
            r.RegisterByName("Ultra Space", new UltraSpace());
            r.RegisterByName("Beast Energy ◇", new BeastEnergy());
            r.RegisterByName("Unit Energy FDY", new UnitEnergy(EnergyType.Fighting, EnergyType.Darkness, EnergyType.Fairy));
        }

        // ================================================================== Items

        /// <summary>Beast Ring: only if the opponent has exactly 3 or 4 Prize cards left; search your deck for up to 2 basic Energy cards and attach them to 1 of your Ultra Beasts.</summary>
        private sealed class BeastRing : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                int left = ctx.Opp.Prizes.Count;
                return (left == 3 || left == 4) && ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => m.Def.IsUltraBeast).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var beasts = ctx.MyPokemon(m => m.Def.IsUltraBeast);
                if (beasts.Count == 0) return;
                var target = ctx.ChoosePokemon(beasts, "Beast Ring: attach basic Energy to which Ultra Beast?");
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Beast Ring: search your deck for up to 2 basic Energy cards");
                foreach (var c in found)
                    if (target == null || !ctx.Engine.AttachEnergyCard(c, target)) ctx.Me.Deck.Add(c);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Eneporter: move a Special Energy from 1 of the opponent's Pokémon to another of their Pokémon (Unnerve still blocks the effect on its Pokémon: Compendium, 2018-05-03).</summary>
        private sealed class Eneporter : CardScript
        {
            private static List<PokemonInPlay> Sources(EffectContext ctx)
            {
                return ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Sources(ctx).Count > 0 && ctx.OpponentPokemon().Count > 1; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var sources = Sources(ctx).Where(m => ctx.CanAffect(m)).ToList();
                if (sources.Count == 0) { ctx.Log("Eneporter had no effect."); return; }
                var origin = ctx.ChoosePokemon(sources, "Eneporter: move a Special Energy from which of your opponent's Pokémon?");
                if (origin == null) return;
                var special = origin.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                var energy = ctx.ChooseCards(special, 1, 1, "Eneporter: move which Special Energy?");
                var targets = ctx.OpponentPokemon(m => m != origin && ctx.CanAffect(m));
                if (energy.Count == 0 || targets.Count == 0) return;
                var dest = ctx.ChoosePokemon(targets, "Eneporter: move it to which of your opponent's Pokémon?");
                if (dest != null) ctx.Engine.MoveEnergy(origin, dest, energy[0]);
            }
        }

        /// <summary>Fossil Excavation Map: choose 1 — search your deck for an Unidentified Fossil, or put one from your discard pile into your hand.</summary>
        private sealed class FossilExcavationMap : CardScript
        {
            private static bool IsFossil(CardDef d) { return d.Name == "Unidentified Fossil"; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || ctx.Me.Discard.Any(c => IsFossil(c.Def));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var inDiscard = ctx.Me.Discard.Where(c => IsFossil(c.Def)).ToList();
                bool canSearch = ctx.Me.Deck.Count > 0;
                int choice = 0;
                if (canSearch && inDiscard.Count > 0)
                    choice = ctx.ChooseOption("Fossil Excavation Map: choose 1", "Search your deck for an Unidentified Fossil", "Put an Unidentified Fossil from your discard pile into your hand");
                else if (!canSearch) choice = 1;
                if (choice == 0)
                {
                    ctx.SearchToHand(IsFossil, 1, "Fossil Excavation Map: search your deck for an Unidentified Fossil");
                    return;
                }
                if (inDiscard.Count > 0) ctx.DiscardToHand(new List<CardInstance> { inDiscard[0] });
            }
        }

        /// <summary>Mysterious Treasure: discard a card from your hand (a cost: not playable with an empty deck, Compendium 2019-03-21); search your deck for a Psychic or Dragon Pokémon.</summary>
        private sealed class MysteriousTreasure : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "Mysterious Treasure: discard a card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.SearchToHand(d => d.IsPokemon && (d.PokemonType == EnergyType.Psychic || d.PokemonType == EnergyType.Dragon
                    || d.SecondType == EnergyType.Psychic || d.SecondType == EnergyType.Dragon), 1, "Mysterious Treasure: choose a Psychic or Dragon Pokémon");
            }
        }

        // ================================================================== Supporters

        /// <summary>Bonnie: only with a Stadium in play; discard it, and this turn your Zygarde-GX can use its GX attack even if you've used your GX attack.</summary>
        private sealed class Bonnie : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.State.Stadium != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                ctx.Me.ZygardeGxThisTurn = true;
                ctx.Log("Bonnie: this turn, " + ctx.Engine.Possessive(ctx.Player) + " Zygarde-GX can use its GX attack even if a GX attack was used.");
            }
        }

        /// <summary>Crasher Wake: discard 2 Water Energy cards from your hand (a cost); if you do, search your deck for up to 2 cards.</summary>
        private sealed class CrasherWake : CardScript
        {
            private static List<CardInstance> Water(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Water(ctx).Count >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var water = Water(ctx);
                if (water.Count < 2) return;
                var picks = water.Count == 2 ? water : ctx.ChooseCards(water, 2, 2, "Crasher Wake: discard which 2 Water Energy cards?");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.SearchToHand(d => true, 2, "Crasher Wake: choose up to 2 cards", false);
            }
        }

        /// <summary>Diantha: only if 1 of your Fairy Pokémon was Knocked Out during the opponent's last turn; put 2 cards from your discard pile into your hand.</summary>
        private sealed class Diantha : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Discard.Count(c => c != ctx.Card) > 0
                    && ctx.WasKnockedOutLastOpponentTurn(d => d.PokemonType == EnergyType.Fairy || d.SecondType == EnergyType.Fairy, false);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => c != ctx.Card).ToList();
                if (pile.Count == 0) return;
                int n = Math.Min(2, pile.Count);
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Diantha: put which 2 cards from your discard pile into your hand?");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Lysandre ◇: for each of your Fire Pokémon in play, put a card from the opponent's discard pile in the Lost Zone.</summary>
        private sealed class LysandrePrismStar : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Opp.Discard.Count > 0 && ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fire)).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fire)).Count, ctx.Opp.Discard.Count);
                if (n <= 0) return;
                var pile = ctx.Opp.Discard.ToList();
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Lysandre ◇: put which " + n + (n == 1 ? " card" : " cards") + " from your opponent's discard pile in the Lost Zone?");
                foreach (var c in picks) CardsCRZ1.DiscardToLostZone(ctx, c);
            }
        }

        /// <summary>Ultra Recon Squad: discard up to 2 Ultra Beast cards from your hand; draw 3 cards for each.</summary>
        private sealed class UltraReconSquad : CardScript
        {
            private static List<CardInstance> Beasts(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsUltraBeast).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Beasts(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var beasts = Beasts(ctx);
                if (beasts.Count == 0) return;
                var picks = ctx.ChooseCards(beasts, 1, Math.Min(2, beasts.Count), "Ultra Recon Squad: discard up to 2 Ultra Beast cards (draw 3 for each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                if (picks.Count > 0) ctx.Draw(3 * picks.Count);
            }
        }

        // ================================================================== Stadiums

        /// <summary>Lysandre Labs: Pokémon Tools in play have no effect ("has a Tool attached" still counts: Compendium, 2018-05).</summary>
        private sealed class LysandreLabs : CardScript
        {
            public override bool DisablesTools(EffectContext src) { return true; }
        }

        /// <summary>Ultra Space: once during each player's turn, they may search their deck for an Ultra Beast card.</summary>
        private sealed class UltraSpace : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsUltraBeast, 1, "Ultra Space: choose an Ultra Beast card");
                return false;
            }
        }

        // ================================================================== Tool

        /// <summary>Metal Frying Pan: the Metal Pokémon it's attached to takes 30 less damage from the opponent's attacks and has no Weakness.</summary>
        private sealed class MetalFryingPan : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || info.Attacker.Owner == holder.Owner) return damage;
                return src.Engine.IsType(holder, EnergyType.Metal) ? damage - 30 : damage;
            }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Metal);
            }
        }

        // ================================================================== Special Energy

        /// <summary>Beast Energy ◇: [C]; on an Ultra Beast every type, 1 at a time, and its attacks do 30 more damage to the opponent's Active Pokémon.</summary>
        private sealed class BeastEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                bool beast = ctx.Pokemon != null && ctx.Pokemon.Def.IsUltraBeast;
                return RecifeEnergy.Units(beast ? EnergyMask.AnyType : EnergyMask.Colorless, 1);
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !holder.Def.IsUltraBeast || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 30;
            }
        }

        /// <summary>Unit Energy: [C] in the hand; attached, it provides each of its 3 types, 1 Energy at a time.</summary>
        internal sealed class UnitEnergy : CardScript
        {
            private readonly int mask;

            public UnitEnergy(params EnergyType[] types)
            {
                int m = 0;
                foreach (var t in types) m |= EnergyMask.Of(t);
                mask = m;
            }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return RecifeEnergy.Units(ctx.Pokemon != null ? mask : EnergyMask.Colorless, 1);
            }
        }
    }
}
