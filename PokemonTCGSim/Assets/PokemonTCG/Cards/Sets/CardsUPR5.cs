using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ultra Prism (UPR, v85), part 5: Trainers and Special Energy (UPR 118-138), and Lana, Lusamine and Peeking Red Card, which
    /// are only secret rares in this set. Cynthia, Mt. Coronet, Pal Pad and Unidentified Fossil were already in the pool and keep
    /// their scripts. Rulings used (Compendium): Cyrus ◇ only shuffles Benched Pokémon and works against Omega Barrier (2018);
    /// Looker Whistle finds only cards named exactly "Looker"; Mars needs a card to draw; Missing Clover can take a Prize card
    /// again with 4 more copies; Super Boost Energy ◇ only works on a Stage 2; Peeking Red Card: the opponent draws.
    /// </summary>
    public static class CardsUPR5
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Ancient Crystal", new AncientCrystal());
            r.RegisterByName("Cyrus ◇", new CyrusPrismStar());
            r.RegisterByName("Electric Memory", new SilvallyMemory(EnergyType.Lightning));
            r.RegisterByName("Escape Board", new EscapeBoard());
            r.RegisterByName("Fire Memory", new SilvallyMemory(EnergyType.Fire));
            r.RegisterByName("Gardenia", new Gardenia());
            r.RegisterByName("Lillie", new Lillie());
            r.RegisterByName("Looker", new Looker());
            r.RegisterByName("Looker Whistle", new LookerWhistle());
            r.RegisterByName("Mars", new Mars());
            r.RegisterByName("Missing Clover", new MissingClover());
            r.RegisterByName("Order Pad", new OrderPad());
            r.RegisterByName("Pokémon Fan Club", new PokemonFanClub());
            r.RegisterByName("Volkner", new Volkner());
            r.RegisterByName("Super Boost Energy ◇", new SuperBoostEnergy());
            r.RegisterByName("Unit Energy GRW", new CardsFLI4.UnitEnergy(EnergyType.Grass, EnergyType.Fire, EnergyType.Water));
            r.RegisterByName("Unit Energy LPM", new CardsFLI4.UnitEnergy(EnergyType.Lightning, EnergyType.Psychic, EnergyType.Metal));
            // Secret rares only in this set
            r.RegisterByName("Lana", new Lana());
            r.RegisterByName("Lusamine", new Lusamine());
            r.RegisterByName("Peeking Red Card", new PeekingRedCard());
        }

        // ================================================================== Items

        /// <summary>Looker Whistle: search your deck for up to 2 cards named Looker.</summary>
        private sealed class LookerWhistle : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.Name == "Looker", 2, "Looker Whistle: choose up to 2 cards named Looker");
            }
        }

        /// <summary>
        /// Missing Clover: you may play 4 Missing Clover cards at once. With 1, look at the top card of your deck; with 4, take a Prize
        /// card (once for the 4 cards).
        /// </summary>
        private sealed class MissingClover : CardScript
        {
            private static List<CardInstance> Others(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.Name == "Missing Clover").ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || (Others(ctx).Count >= 3 && ctx.Me.Prizes.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = Others(ctx);
                if (others.Count >= 3 && ctx.Me.Prizes.Count > 0 && ctx.AskYesNo("Missing Clover: play 4 Missing Clover cards at once and take a Prize card?"))
                {
                    foreach (var card in others.Take(3))
                    {
                        ctx.Me.Hand.Remove(card);
                        ctx.Me.Discard.Add(card);
                    }
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " played 4 Missing Clover cards at once.");
                    ctx.Engine.TakePrizeCards(ctx.Player, 1);
                    return;
                }
                if (ctx.Me.Deck.Count == 0) return;
                var top = ctx.LookAtTop(1);
                ctx.Engine.ShowCards(ctx.Player, top, "Missing Clover", "The top card of your deck", ctx.Card.Def);
            }
        }

        /// <summary>Order Pad: flip a coin; on heads, search your deck for an Item card.</summary>
        private sealed class OrderPad : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.SearchToHand(CardsLOT4.IsItemCard, 1, "Order Pad: choose an Item card");
            }
        }

        /// <summary>Peeking Red Card: the opponent reveals their hand; you may have them shuffle it into their deck and draw that many cards.</summary>
        private sealed class PeekingRedCard : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed an empty hand.");
                    return;
                }
                ctx.Engine.Reveal(ctx.Opponent, hand, "revealed " + ctx.Engine.Their(ctx.Opponent) + " hand (Peeking Red Card)");
                if (!ctx.AskYesNo("Peeking Red Card: have your opponent shuffle their " + hand.Count + " cards into their deck and draw " + hand.Count + " cards?")) return;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, hand.Count);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Ancient Crystal: the Regirock, Regice, Registeel or Regigigas holding it takes 30 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class AncientCrystal : CardScript
        {
            private static bool IsRegi(PokemonInPlay mon)
            {
                var n = mon.Def.Name;
                return n == "Regirock" || n == "Regice" || n == "Registeel" || n == "Regigigas";
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || info.Attacker.Owner == holder.Owner || !IsRegi(holder)) return damage;
                return damage - 30;
            }
        }

        /// <summary>Electric / Fire Memory: the Silvally-GX holding it is that type (only that type).</summary>
        internal sealed class SilvallyMemory : CardScript
        {
            private readonly EnergyType type;

            public SilvallyMemory(EnergyType type) { this.type = type; }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Def.Name != "Silvally-GX") return;
                types.Clear();
                types.Add(type);
            }
        }

        /// <summary>Escape Board: the holder's Retreat Cost is [C] less, and it can retreat even if it's Asleep or Paralyzed.</summary>
        private sealed class EscapeBoard : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon ? cost - 1 : cost;
            }

            public override bool AllowsRetreatWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon;
            }
        }

        // ================================================================== Supporters

        /// <summary>
        /// Cyrus ◇: only if your Active Pokémon is a Water or Metal Pokémon (errata). The opponent chooses 2 of their Benched Pokémon
        /// and shuffles the others, with all attached cards, into their deck.
        /// </summary>
        private sealed class CyrusPrismStar : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null || !(ctx.Engine.IsType(active, EnergyType.Water) || ctx.Engine.IsType(active, EnergyType.Metal))) return false;
                return ctx.Opp.Bench.Count > 2;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var bench = ctx.Opp.Bench.ToList();
                if (bench.Count <= 2) return;
                var opts = bench.Select(m => new DecisionOption { Label = ctx.Engine.DescribeFor(ctx.Opponent, m), PokemonUid = m.Uid, CardId = m.Top.Id, Card = m.Def }).ToList();
                var keep = ctx.Engine.AskMany(ctx.Opponent, DecisionKind.ChoosePokemon, "Cyrus ◇: choose 2 of your Benched Pokémon to keep; the others are shuffled into your deck", opts, 2, 2, ctx.Source)
                    .Select(i => bench[i]).ToList();
                foreach (var mon in bench)
                    if (!keep.Contains(mon) && ctx.InPlay(mon)) ctx.Engine.ShufflePokemonIntoDeck(mon);
            }
        }

        /// <summary>Gardenia: heal 80 damage from 1 of your Pokémon that has any Grass Energy attached.</summary>
        private sealed class Gardenia : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.HasEnergyOfType(m, EnergyType.Grass));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Targets(ctx), "Gardenia: heal 80 damage from which Pokémon?");
                if (target != null) ctx.Heal(target, 80);
            }
        }

        /// <summary>Lana: heal 50 damage from each of your Pokémon that has any Water Energy attached.</summary>
        private sealed class Lana : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.HasEnergyOfType(m, EnergyType.Water));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in Targets(ctx)) ctx.Heal(mon, 50);
            }
        }

        /// <summary>Lillie: draw until you have 6 cards in your hand; on your first turn, until you have 8.</summary>
        private sealed class Lillie : CardScript
        {
            private static int Target(EffectContext ctx) { return ctx.IsMyFirstTurn ? 8 : 6; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                // Lillie is still in the hand while this is checked.
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count - 1 < Target(ctx);
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(Target(ctx)); }
        }

        /// <summary>Looker: draw 3 cards from the bottom of your deck.</summary>
        private sealed class Looker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                int n = 0;
                for (int i = 0; i < 3 && deck.Count > 0; i++)
                {
                    var card = deck[deck.Count - 1];
                    deck.RemoveAt(deck.Count - 1);
                    ctx.Me.Hand.Add(card);
                    n++;
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew " + n + (n == 1 ? " card" : " cards") + " from the bottom of " + ctx.Engine.Their(ctx.Player) + " deck (Looker).");
            }
        }

        /// <summary>Lusamine: put 2 in any combination of Supporter and Stadium cards from your discard pile into your hand (as many as there are, up to 2).</summary>
        private sealed class Lusamine : CardScript
        {
            private static List<CardInstance> Retrievable(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsSupporter || c.Def.IsStadiumCard).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Retrievable(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Retrievable(ctx);
                int n = Math.Min(2, pile.Count);
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Lusamine: put which 2 Supporter and Stadium cards into your hand?");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Mars: draw 2 cards; if you do, discard a random card from the opponent's hand.</summary>
        private sealed class Mars : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Draw(2) > 0 && ctx.Opp.Hand.Count > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
            }
        }

        /// <summary>Pokémon Fan Club: search your deck for up to 2 Basic Pokémon and put them into your hand.</summary>
        private sealed class PokemonFanClub : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicPokemon, 2, "Pokémon Fan Club: choose up to 2 Basic Pokémon");
            }
        }

        /// <summary>Volkner: search your deck for an Item card and a Lightning Energy card.</summary>
        private sealed class Volkner : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(CardsLOT4.IsItemCard, 1, "Volkner: choose an Item card");
                found.AddRange(ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 1, "Volkner: choose a Lightning Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        // ================================================================== Special Energy

        /// <summary>
        /// Super Boost Energy ◇: [C]. Attached to a Stage 2 Pokémon it provides every type, 1 Energy at a time; with 3 or more Stage 2
        /// Pokémon in play, every type, 4 Energy at a time. On anything else it's only [C] (Compendium, 2018-02-08).
        /// </summary>
        private sealed class SuperBoostEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var holder = ctx.Pokemon;
                if (holder == null || holder.Def.Stage != Stage.Stage2) return RecifeEnergy.Units(EnergyMask.Colorless, 1);
                int stage2 = ctx.State.Players[holder.Owner].InPlay().Count(m => m.Def.Stage == Stage.Stage2);
                return RecifeEnergy.Units(EnergyMask.AnyType, stage2 >= 3 ? 4 : 1);
            }
        }
    }
}
