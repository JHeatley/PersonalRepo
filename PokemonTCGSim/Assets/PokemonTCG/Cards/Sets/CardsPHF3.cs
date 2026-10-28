using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Phantom Forces (PHF, v102), part 3: the Trainers and Special Energy (91-112). Enhanced Hammer, Professor Sycamore, Shauna,
    /// Tierno, VS Seeker and Double Colorless Energy were already in the pool; Lysandre's Trump Card is on the Expanded ban list.
    /// Head Ringer and Jamming Net (Team Flare Hyper Gear) go on 1 of the opponent's Pokémon-EX (your ruling, v102).
    /// </summary>
    public static class CardsPHF3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("AZ", new AZ());
            r.RegisterByName("Battle Compressor", new BattleCompressor());
            r.RegisterByName("Dimension Valley", new DimensionValley());
            r.RegisterByName("Gengar Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Hand Scope", new HandScope());
            r.RegisterByName("Head Ringer", new HeadRinger());
            r.RegisterByName("Jamming Net", new JammingNet());
            r.RegisterByName("Lysandre's Trump Card", new LysandresTrumpCard());
            r.RegisterByName("Manectric Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Robo Substitute", new RoboSubstitute());
            r.RegisterByName("Roller Skates", new RollerSkates());
            r.RegisterByName("Steel Shelter", new SteelShelter());
            r.RegisterByName("Target Whistle", new TargetWhistle());
            r.RegisterByName("Trick Coin", new TrickCoin());
            r.RegisterByName("Xerosic", new Xerosic());
            r.RegisterByName("Mystery Energy", new MysteryEnergy());
        }

        /// <summary>AZ: put 1 of your Pokémon into your hand; discard all cards attached to it (each to its owner's discard pile).</summary>
        private sealed class AZ : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.CanPutIntoHand(m)).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.MyPokemon(m => ctx.Engine.CanPutIntoHand(m));
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "AZ: put which of your Pokémon into your hand?");
                if (mon == null) return;
                var pokemonCards = mon.Stack.ToList();
                var attached = mon.Energy.Concat(mon.Tools).ToList();
                ctx.Engine.RemoveFromPlay(mon);
                ctx.Me.Hand.AddRange(pokemonCards);
                ctx.Engine.DiscardFromPlay(attached);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, mon.Name + " was put into " + ctx.Engine.PlayerName(ctx.Player) + "'s hand"
                    + (attached.Count > 0 ? " and its attached cards were discarded" : "") + " (AZ).", mon.Top.Id, mon.Uid);
            }
        }

        /// <summary>Battle Compressor: search your deck for up to 3 cards and discard them.</summary>
        private sealed class BattleCompressor : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => true, 3, "Battle Compressor: choose up to 3 cards to discard", false);
                ctx.Me.Discard.AddRange(found);
                if (found.Count > 0) ctx.Engine.Reveal(ctx.Player, found, "discarded these cards from " + ctx.Engine.Their(ctx.Player) + " deck (Battle Compressor)");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Dimension Valley: each Psychic Pokémon's attacks cost [C] less (both players').</summary>
        private sealed class DimensionValley : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || !src.Engine.IsType(mon, EnergyType.Psychic)) return;
                int i = cost.IndexOf(EnergyType.Colorless);
                if (i >= 0) cost.RemoveAt(i);
            }
        }

        /// <summary>Hand Scope: your opponent reveals their hand.</summary>
        private sealed class HandScope : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx) { CardsTWM1.RevealOpponentHand(ctx); }
        }

        /// <summary>Head Ringer (Pokémon Tool F): on 1 of the opponent's Pokémon-EX without a Tool; its attacks cost [C] more.</summary>
        private sealed class HeadRinger : CardScript
        {
            public override bool AttachesToOpponentsPokemon { get { return true; } }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Def.IsPokemonEX; }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != null && mon == src.Pokemon) cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Jamming Net (Pokémon Tool F): on 1 of the opponent's Pokémon-EX without a Tool; its attacks do 20 less damage to your Pokémon (before Weakness and Resistance).</summary>
        private sealed class JammingNet : CardScript
        {
            public override bool AttachesToOpponentsPokemon { get { return true; } }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Def.IsPokemonEX; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.AgainstOpponent || damage <= 0) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>Lysandre's Trump Card (errata): each player shuffles their discard pile into their deck, except any Lysandre's Trump Card (Compendium, 2014-11-13).</summary>
        private sealed class LysandresTrumpCard : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int side in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[side];
                    var pile = p.Discard.Where(c => c != ctx.Card && c.Def.Name != "Lysandre's Trump Card").ToList();
                    foreach (var card in pile) p.Discard.Remove(card);
                    p.Deck.AddRange(pile);
                    ctx.Engine.ShuffleDeck(side);
                    ctx.Log(ctx.Engine.PlayerName(side) + " shuffled " + pile.Count + " cards from " + ctx.Engine.Their(side) + " discard pile into " + ctx.Engine.Their(side) + " deck.");
                }
            }
        }

        /// <summary>Robo Substitute: played as a 30-HP Colorless Basic Pokémon (the engine's Antique Fossil rules: can't retreat, may be discarded, no Prize); Special Conditions affect it.</summary>
        private sealed class RoboSubstitute : CardScript
        {
        }

        /// <summary>Roller Skates: flip a coin; heads, draw 3 cards.</summary>
        private sealed class RollerSkates : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.Draw(3);
            }
        }

        /// <summary>Steel Shelter: Metal Pokémon (both players') can't be affected by Special Conditions, and lose any they have.</summary>
        private sealed class SteelShelter : CardScript
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && src.Engine.IsType(target, EnergyType.Metal);
            }

            public override bool CuresConditionsItPrevents { get { return true; } }
        }

        /// <summary>Target Whistle: put a Basic Pokémon from the opponent's discard pile onto their Bench.</summary>
        private sealed class TargetWhistle : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx)
            {
                return ctx.Opp.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Engine.BenchSpace(ctx.Opponent) > 0 && Basics(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Basics(ctx);
                if (pile.Count == 0 || ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return;
                var pick = ctx.ChooseCards(pile, 1, 1, "Target Whistle: put which Basic Pokémon onto your opponent's Bench?")[0];
                if (ctx.Opp.Discard.Remove(pick)) ctx.Engine.PutOntoBench(ctx.Opponent, pick, false);
            }
        }

        /// <summary>Trick Coin: once during your turn, the holder's attack coin flips may be ignored and flipped again (1 such effect per turn).</summary>
        private sealed class TrickCoin : CardScript
        {
            public override bool OffersAttackReflip(EffectContext src) { return src.Pokemon != null; }
        }

        /// <summary>Xerosic: discard a Pokémon Tool or Special Energy attached to any Pokémon in play (yours or your opponent's).</summary>
        private sealed class Xerosic : CardScript
        {
            private static bool Removable(CardInstance card) { return card.Def.IsTool || card.Def.IsSpecialEnergy; }

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Tools.Concat(m.Energy).Any(Removable))
                    .Concat(ctx.OpponentPokemon(m => m.Tools.Concat(m.Energy).Any(Removable) && ctx.CanAffect(m))).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Holders(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var holders = Holders(ctx);
                if (holders.Count == 0) return;
                var mon = ctx.ChoosePokemon(holders, "Xerosic: discard a Pokémon Tool or Special Energy from which Pokémon?");
                if (mon == null) return;
                var cards = mon.Tools.Concat(mon.Energy).Where(Removable).ToList();
                var card = ctx.ChooseCards(cards, 1, 1, "Xerosic: discard which card from " + mon.Name + "?")[0];
                if (card.Def.IsTool) ctx.Engine.DiscardTool(mon, card);
                else ctx.Engine.DiscardEnergy(mon, card);
            }
        }

        /// <summary>Mystery Energy: Psychic only, on a Psychic Pokémon, whose Retreat Cost is [C][C] less.</summary>
        private sealed class MysteryEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Psychic)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Psychic) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Psychic); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Psychic); }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon != src.Pokemon || !src.Engine.IsType(mon, EnergyType.Psychic)) return cost;
                return Math.Max(0, cost - 2);
            }
        }
    }
}
