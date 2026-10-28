using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unbroken Bonds (UNB) part 4: Trainers and Energy (UNB 164-190). Giovanni's Exile, Koga's Trap, Lt. Surge's Strategy,
    /// Martial Arts Dojo, Pokégear 3.0 and Power Plant were already in the pool. Dust Island is handled where Pokémon switch
    /// (GameEngine.SwitchActive asks the Stadium).
    /// </summary>
    public static class CardsUNB4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Beast Bringer", new BeastBringer());
            r.RegisterByName("Chip-Chip Ice Axe", new ChipChipIceAxe());
            r.RegisterByName("Devolution Spray Z", new DevolutionSprayZ());
            r.RegisterByName("Dusk Stone", new DuskStone());
            r.RegisterByName("Dust Island", new DustIsland());
            r.RegisterByName("Electromagnetic Radar", new DiscardTwoSearch(d => d.IsPokemon && d.IsGXorEX && d.PokemonType == EnergyType.Lightning, 2,
                "Electromagnetic Radar: search your deck for up to 2 Lightning Pokémon-GX or Pokémon-EX", true));
            r.RegisterByName("Energy Spinner", new EnergySpinner());
            r.RegisterByName("Fairy Charm Ability", new FairyCharm(m => m.Def.Abilities.Count > 0));
            r.RegisterByName("Fairy Charm Lightning", new FairyCharm(m => m.Def.PokemonType == EnergyType.Lightning));
            r.RegisterByName("Fire Crystal", new FireCrystal());
            r.RegisterByName("Green's Exploration", new GreensExploration());
            r.RegisterByName("Janine", new Janine());
            r.RegisterByName("Metal Core Barrier", new MetalCoreBarrier());
            r.RegisterByName("Molayne", new Molayne());
            r.RegisterByName("Red's Challenge", new DiscardTwoSearch(d => true, 1, "Red's Challenge: search your deck for a card", false));
            r.RegisterByName("Samson Oak", new SamsonOak());
            r.RegisterByName("Stealthy Hood", new StealthyHood());
            r.RegisterByName("Surprise Box", new SurpriseBox());
            r.RegisterByName("Ultra Forest Kartenvoy", new UltraForestKartenvoy());
            r.RegisterByName("Welder", new Welder());
            r.RegisterByName("Triple Acceleration Energy", new TripleAccelerationEnergy());
        }

        /// <summary>Beast Bringer: with exactly 6 Prize cards left, when the holder (an Ultra Beast) Knocks Out the opponent's Active Pokémon-GX or Pokémon-EX with an attack's damage, take 1 more Prize card.</summary>
        private sealed class BeastBringer : CardScript
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var holder = src.Pokemon;
                if (holder == null || taker != src.Player || knockedOut.Owner == src.Player || !holder.Def.IsUltraBeast) return prizes;
                if (src.Me.Prizes.Count != 6 || !knockedOut.Def.IsGXorEX || !knockedOut.HitByAttack || knockedOut.ForcedKnockOut) return prizes;
                if (knockedOut.HitByAttackerUid != holder.Uid || !knockedOut.HitWhileActive) return prizes;
                return prizes + 1;
            }
        }

        /// <summary>Chip-Chip Ice Axe: look at the top 3 cards of the opponent's deck and choose 1; they shuffle the others back, then the chosen card goes on top.</summary>
        private sealed class ChipChipIceAxe : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var opp = ctx.Opp;
                var top = opp.Deck.Take(3).ToList();
                if (top.Count == 0) return;
                ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of your opponent's deck", ctx.Player);
                var pick = ctx.ChooseCards(top, 1, 1, "Chip-Chip Ice Axe: choose the card to put on top of your opponent's deck")[0];
                foreach (var card in top) opp.Deck.Remove(card);
                foreach (var card in top) if (card != pick) opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                opp.Deck.Insert(0, pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 of the top cards of " + ctx.Engine.Their(ctx.Opponent) + " deck back on top (Chip-Chip Ice Axe).");
            }
        }

        /// <summary>Devolution Spray Z: devolve 1 of your evolved Pokémon by shuffling any number of its Evolution cards into your deck; it can't evolve this turn.</summary>
        private sealed class DevolutionSprayZ : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Stack.Count > 1 && m.Def.IsEvolution && !m.Def.IsMultiCard);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Devolution Spray Z: devolve which of your Pokémon?");
                if (target == null) return;
                int max = target.Stack.Count - 1;
                int n = max <= 1 ? 1 : ctx.AskNumber(1, max, "Devolution Spray Z: shuffle how many Evolution cards from " + target.Name + " into your deck?");
                ctx.Engine.Devolve(target, n, true);
                target.CantEvolveOnTurn = ctx.Turn;
            }
        }

        /// <summary>Dusk Stone: evolve 1 of your Pokémon into Mismagius, Honchkrow, Chandelure or Aegislash (Pokémon-GX included) from your deck, even on your first turn or the turn it came into play.</summary>
        private sealed class DuskStone : CardScript
        {
            private static readonly string[] Lines = { "Mismagius", "Honchkrow", "Chandelure", "Aegislash" };

            private static bool DuskCard(CardDef d)
            {
                if (!d.IsPokemon) return false;
                foreach (var n in Lines) if (d.Name == n || d.Name == n + "-GX") return true;
                return false;
            }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => !CardsEVS1.EvolveBlocked(ctx, m)
                                          && (m.Name == "Misdreavus" || m.Name == "Murkrow" || m.Name == "Lampent" || m.Name == "Doublade"));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0 || ctx.Me.Deck.Count == 0) return;
                var found = ctx.SearchMyDeck(d => DuskCard(d) && targets.Any(m => CardsEVS1.EvolvesOnto(ctx, m, d)), 1,
                    "Dusk Stone: search your deck for a Mismagius, Honchkrow, Chandelure or Aegislash that evolves from 1 of your Pokémon");
                if (found.Count > 0)
                {
                    var card = found[0];
                    var options = targets.Where(m => CardsEVS1.EvolvesOnto(ctx, m, card.Def)).ToList();
                    var target = ctx.ChoosePokemon(options, "Dusk Stone: evolve which Pokémon into " + card.Def.Name + "?");
                    if (target != null) ctx.Engine.EvolvePokemon(target, card);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Dust Island: a Poisoned Active Pokémon switched with a Benched Pokémon by a Trainer card's effect makes the new Active Pokémon Poisoned.</summary>
        private sealed class DustIsland : CardScript
        {
            public override bool PoisonFollowsTrainerSwitch(EffectContext src) { return true; }
        }

        /// <summary>"You can play this card only if you discard 2 other cards from your hand. Search your deck for ..." (Electromagnetic Radar, Red's Challenge).</summary>
        private sealed class DiscardTwoSearch : CardScript
        {
            private readonly Func<CardDef, bool> filter;
            private readonly int count;
            private readonly string prompt;
            private readonly bool reveal;

            public DiscardTwoSearch(Func<CardDef, bool> filter, int count, string prompt, bool reveal)
            {
                this.filter = filter;
                this.count = count;
                this.prompt = prompt;
                this.reveal = reveal;
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var discards = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, ctx.SourceName + ": discard 2 cards from your hand");
                foreach (var card in discards) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.SearchToHand(filter, count, prompt, reveal);
            }
        }

        /// <summary>Energy Spinner: search for a basic Energy card (up to 3 if you went second and it's your first turn).</summary>
        private sealed class EnergySpinner : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int max = CardsUNB1.SecondPlayersFirstTurn(ctx) ? 3 : 1;
                ctx.SearchToHand(d => d.IsBasicEnergy, max, "Energy Spinner: search your deck for " + (max == 1 ? "a basic Energy card" : "up to 3 basic Energy cards"));
            }
        }

        /// <summary>Fairy Charm: prevent all damage done to the holder (a Fairy Pokémon) by attacks from the opponent's Pokémon-GX and Pokémon-EX of a kind.</summary>
        private sealed class FairyCharm : CardScript
        {
            private readonly Func<PokemonInPlay, bool> kind;

            public FairyCharm(Func<PokemonInPlay, bool> kind) { this.kind = kind; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                var attacker = info.Attacker;
                if (holder == null || info.Target != holder || attacker == null || !info.AgainstOpponent) return false;
                if (!src.Engine.IsType(holder, EnergyType.Fairy) || !attacker.Def.IsGXorEX) return false;
                return kind(attacker);
            }
        }

        /// <summary>Fire Crystal: put 3 Fire Energy cards from your discard pile into your hand.</summary>
        private sealed class FireCrystal : CardScript
        {
            private static List<CardInstance> Fire(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Fire(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var fire = Fire(ctx);
                var picks = fire.Count <= 3 ? fire : ctx.ChooseCards(fire, 3, 3, "Fire Crystal: put which 3 Fire Energy cards into your hand?");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Green's Exploration: only with no Pokémon with Abilities in play (a Pokémon whose Abilities are turned off, by Power Plant say, has none: Compendium, 2019-05-02); search your deck for up to 2 Trainer cards.</summary>
        private sealed class GreensExploration : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && !ctx.MyPokemon().Any(m => ctx.Engine.HasAbility(m));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTrainer, 2, "Green's Exploration: search your deck for up to 2 Trainer cards");
            }
        }

        /// <summary>Janine: look at the top 4 cards of your deck, put 2 of them into your hand and shuffle the rest back.</summary>
        private sealed class Janine : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return;
                var picks = top.Count <= 2 ? top : ctx.ChooseCards(top, 2, 2, "Janine: put 2 of these cards into your hand");
                ctx.DeckToHand(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Metal Core Barrier: the holder (a Metal Pokémon) takes 70 less damage from the opponent's attacks; discarded at the end of the opponent's turn.</summary>
        private sealed class MetalCoreBarrier : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || damage <= 0 || !src.Engine.IsType(holder, EnergyType.Metal)) return damage;
                return Math.Max(0, damage - 70);
            }

            /// <summary>Discarded at the end of the opponent's turn by the engine, even while Tools have no effect (like Bursting Balloon, v97).</summary>
            public override bool DiscardedAtEndOfOpponentsTurn { get { return true; } }
        }

        /// <summary>Molayne: discard 2 Metal Energy cards from your hand to play it; shuffle a Trainer card from your discard pile into your deck.</summary>
        private sealed class Molayne : CardScript
        {
            private static List<CardInstance> Metal(EffectContext ctx) { return ctx.Me.Hand.Where(c => CardsUNB1.IsBasicEnergyOf(c.Def, EnergyType.Metal)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Metal(ctx).Count >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var metal = Metal(ctx);
                if (metal.Count < 2) return;
                var cost = metal.Count == 2 ? metal : ctx.ChooseCards(metal, 2, 2, "Molayne: discard 2 Metal Energy cards from your hand");
                foreach (var card in cost) ctx.Engine.DiscardFromHand(ctx.Player, card);
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer && c != ctx.Card).ToList();
                if (trainers.Count == 0) return;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Molayne: shuffle which Trainer card from your discard pile into your deck?");
                if (ctx.DiscardToDeck(pick).Count > 0) ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Samson Oak: draw 2 cards; 2 more if both Active Pokémon share a type.</summary>
        private sealed class SamsonOak : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                var mine = ctx.Me.Active;
                var theirs = ctx.Opp.Active;
                if (mine == null || theirs == null) return;
                var types = ctx.Engine.TypesOf(theirs);
                if (ctx.Engine.TypesOf(mine).Any(t => types.Contains(t))) ctx.Draw(2);
            }
        }

        /// <summary>Stealthy Hood: prevent all effects of the opponent's Abilities done to the holder.</summary>
        private sealed class StealthyHood : CardScript
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Ability;
            }
        }

        /// <summary>Surprise Box: put a card from the opponent's discard pile into their hand.</summary>
        private sealed class SurpriseBox : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Discard.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Opp.Discard.ToList();
                if (pile.Count == 0) return;
                var pick = ctx.ChooseCards(pile, 1, 1, "Surprise Box: put which card from your opponent's discard pile into their hand?")[0];
                if (ctx.Opp.Discard.Remove(pick)) ctx.Opp.Hand.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Def.Name + " from " + ctx.Engine.Their(ctx.Opponent) + " discard pile into " + ctx.Engine.Their(ctx.Opponent) + " hand (Surprise Box).");
            }
        }

        /// <summary>Ultra Forest Kartenvoy: this turn, damage from your Ultra Beasts' attacks isn't affected by any effects on the opponent's Active Pokémon.</summary>
        private sealed class UltraForestKartenvoy : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new KartenvoyEffect("Ultra Forest Kartenvoy: " + ctx.Engine.PlayerName(ctx.Player) + "'s Ultra Beasts ignore effects on the opponent's Active Pokémon this turn",
                    ctx.Player, ctx.Turn));
            }
        }

        private sealed class KartenvoyEffect : ContinuousEffect
        {
            public KartenvoyEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool IgnoresDefenderEffects(GameEngine engine, DamageInfo info)
            {
                return info.Attacker != null && info.Attacker.Owner == Owner && info.Attacker.Def.IsUltraBeast && info.AgainstOpponent && info.TargetIsActive;
            }
        }

        /// <summary>Welder: attach up to 2 Fire Energy cards from your hand to 1 of your Pokémon; if you do, draw 3 cards.</summary>
        private sealed class Welder : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => CardsUNB1.IsFireEnergyCard(c.Def)).ToList();
                var targets = ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m));
                if (fire.Count == 0 || targets.Count == 0) return;
                var picks = ctx.ChooseCards(fire, 0, Math.Min(2, fire.Count), "Welder: attach up to 2 Fire Energy cards from your hand to 1 of your Pokémon");
                if (picks.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Welder: attach the Fire Energy to which Pokémon?");
                if (target == null) return;
                int attached = 0;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card) && ctx.Engine.AttachEnergyCard(card, target, true)) attached++;
                if (attached > 0) ctx.Draw(3);
            }
        }

        /// <summary>
        /// Triple Acceleration Energy: attaches only to Evolution Pokémon and provides 3 Colorless Energy there; it's discarded at the
        /// end of the turn, or right away if it's on anything other than an Evolution Pokémon.
        /// </summary>
        private sealed class TripleAccelerationEnergy : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Def.IsEvolution; }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Pokemon.Def.IsEvolution; }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Pokemon.Def.IsEvolution) return new List<int>();
                return new List<int> { EnergyMask.Colorless, EnergyMask.Colorless, EnergyMask.Colorless };
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                if (src.Pokemon == null || !src.Pokemon.Energy.Contains(src.Card)) return;
                src.Log("Triple Acceleration Energy is discarded at the end of the turn.");
                src.Engine.DiscardEnergy(src.Pokemon, src.Card);
            }
        }
    }
}
