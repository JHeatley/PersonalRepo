using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Genesis Trainers and Special Energy (N1 19, 83-104; v138). Old Trainers play as Items (no Supporter limit), the
    /// Berries and Focus Band as Pokémon Tools. Super Scoop Up, Super Rod and Recycle Energy are the cards already in the pool.
    /// Your rulings: Darkness Energy and Metal Energy use Limitless's (later) text; Ecogym returns only basic Energy cards.
    /// "You can't play any more Trainer cards this turn" (Professor Elm, PokéGear, Time Capsule) is PlayerState.TrainersLockedTurn.
    /// </summary>
    public static class CardsN1T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N1-19", new MetalEnergyN1());
            r.RegisterByKey("N1-83", new ArcadeGame());
            r.RegisterByKey("N1-84", new Ecogym());
            r.RegisterByKey("N1-85", new EnergyCharge());
            r.RegisterByKey("N1-86", new FocusBand());
            r.RegisterByKey("N1-87", new Mary());
            r.RegisterByKey("N1-88", new PokeGear());
            r.RegisterByKey("N1-89", new SuperEnergyRetrieval());
            r.RegisterByKey("N1-90", new TimeCapsule());
            r.RegisterByKey("N1-91", new BillsTeleporter());
            r.RegisterByKey("N1-92", new CardFlipGame());
            r.RegisterByKey("N1-93", new HealBerry("Gold Berry", 4));
            r.RegisterByKey("N1-94", new MiracleBerry());
            r.RegisterByKey("N1-95", new NewPokedex());
            r.RegisterByKey("N1-96", new ProfessorElm());
            r.RegisterByKey("N1-97", new SproutTower());
            r.RegisterByKey("N1-99", new HealBerry("Berry", 2));
            r.RegisterByKey("N1-100", new DoubleGust());
            r.RegisterByKey("N1-101", new MooMooMilk());
            r.RegisterByKey("N1-102", new PokemonMarch());
            r.RegisterByKey("N1-104", new DarknessEnergyN1());
        }

        /// <summary>"You can't play any more Trainer cards this turn."</summary>
        private static void LockTrainers(EffectContext ctx)
        {
            ctx.Me.TrainersLockedTurn = ctx.Turn;
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " can't play any more Trainer cards this turn.");
        }

        // ================================================================== Energy

        /// <summary>Metal Energy (Limitless's text, your ruling): attack damage to a Metal Pokémon it's attached to is 10 less, after Weakness and Resistance.</summary>
        private sealed class MetalEnergyN1 : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(holder, EnergyType.Metal) ? Math.Max(0, damage - 10) : damage;
            }
        }

        /// <summary>Darkness Energy (Limitless's text, your ruling): a Darkness Pokémon's attacks do 10 more damage to the opponent's Active Pokémon, before Weakness and Resistance.</summary>
        private sealed class DarknessEnergyN1 : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(holder, EnergyType.Darkness) ? damage + 10 : damage;
            }
        }

        // ================================================================== Items

        /// <summary>Arcade Game: shuffle your deck and reveal the top 3; if 2 or more share a name, those go into your hand; the rest are shuffled back.</summary>
        private sealed class ArcadeGame : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.ShuffleMyDeck();
                var top = ctx.Me.Deck.Take(Math.Min(3, ctx.Me.Deck.Count)).ToList();
                ctx.Engine.Reveal(ctx.Player, top, "the top cards of the deck (Arcade Game)");
                var pair = top.GroupBy(c => c.Def.Name).FirstOrDefault(g => g.Count() >= 2);
                if (pair != null) ctx.DeckToHand(pair.ToList());
                else ctx.Log("Arcade Game: no 2 of those cards share a name.");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Energy Charge: flip a coin; heads, shuffle up to 2 Energy cards from your discard pile into your deck.</summary>
        private sealed class EnergyCharge : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsEnergy); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Energy Charge: shuffle up to 2 Energy cards from your discard pile into your deck");
                if (picks.Count == 0) return;
                ctx.Engine.Reveal(ctx.Player, picks, "shuffled into the deck (Energy Charge)");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Mary: draw 2 cards, then shuffle 2 cards from your hand into your deck.</summary>
        private sealed class Mary : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Me.Hand.Count(c => c != ctx.Card) >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                var hand = ctx.Me.Hand.ToList();
                int n = Math.Min(2, hand.Count);
                if (n == 0) return;
                var picks = n == hand.Count ? hand : ctx.ChooseCards(hand, n, n, "Mary: shuffle " + n + " cards from your hand into your deck");
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + picks.Count + " cards from " + ctx.Engine.Their(ctx.Player) + " hand into " + ctx.Engine.Their(ctx.Player) + " deck (Mary).");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>PokéGear: look at the top 7 cards of your deck; you may put a Trainer card from them into your hand (shown). Shuffle. No more Trainer cards this turn.</summary>
        private sealed class PokeGear : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var trainers = top.Where(c => c.Def.IsTrainer).ToList();
                var pick = ctx.ChooseCards(trainers, 0, 1, "PokéGear: put a Trainer card from the top 7 cards of your deck into your hand");
                if (pick.Count > 0)
                {
                    ctx.Engine.Reveal(ctx.Player, pick, "put into the hand (PokéGear)");
                    ctx.DeckToHand(pick);
                }
                ctx.ShuffleMyDeck();
                LockTrainers(ctx);
            }
        }

        /// <summary>Super Energy Retrieval: discard 2 other cards from your hand, then put up to 4 basic Energy cards from your discard pile into your hand.</summary>
        private sealed class SuperEnergyRetrieval : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.IsBasicEnergy).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && Basics(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count < 2) return;
                var discard = ctx.ChooseCards(hand, 2, 2, "Super Energy Retrieval: discard 2 cards from your hand");
                foreach (var card in discard) ctx.Engine.DiscardFromHand(ctx.Player, card);
                var basics = Basics(ctx).Where(c => !discard.Contains(c)).ToList();
                if (basics.Count == 0) return;
                var take = basics.Count <= 4 ? basics : ctx.ChooseCards(basics, 4, 4, "Super Energy Retrieval: put 4 basic Energy cards from your discard pile into your hand");
                ctx.Engine.Reveal(ctx.Player, take, "put into the hand (Super Energy Retrieval)");
                ctx.DiscardToHand(take);
            }
        }

        /// <summary>
        /// Time Capsule: your opponent may shuffle 5 Basic Pokémon, Evolution and/or basic Energy cards from their discard pile into
        /// their deck (all or none if they have fewer); then you may do the same. No more Trainer cards this turn.
        /// </summary>
        private sealed class TimeCapsule : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsPokemon || (d.IsEnergy && d.IsBasicEnergy); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                Recover(ctx, ctx.Opponent);
                Recover(ctx, ctx.Player);
                LockTrainers(ctx);
            }

            private static void Recover(EffectContext ctx, int player)
            {
                var p = ctx.State.Players[player];
                var cards = p.Discard.Where(c => Fits(c.Def)).ToList();
                if (cards.Count == 0) return;
                int n = Math.Min(5, cards.Count);
                if (!ctx.Engine.AskYesNo(player, "Time Capsule: shuffle " + n + " Pokémon and/or basic Energy cards from your discard pile into your deck?", ctx.Source)) return;
                var picks = cards.Count <= 5 ? cards : ctx.Engine.ChooseCards(player, cards, 5, 5, "Time Capsule: choose 5 cards to shuffle into your deck", ctx.Source);
                foreach (var card in picks)
                    if (p.Discard.Remove(card)) p.Deck.Add(card);
                ctx.Engine.Reveal(player, picks, "shuffled into the deck (Time Capsule)");
                ctx.Engine.ShuffleDeck(player);
            }
        }

        /// <summary>Bill's Teleporter: flip a coin; heads, draw 4 cards.</summary>
        private sealed class BillsTeleporter : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.Draw(4);
            }
        }

        /// <summary>
        /// Card-Flip Game: choose 1 of your opponent's face-down Prize cards and guess its kind (Energy, Trainer or Pokémon). It's turned
        /// face up and stays face up; a right guess draws 2 cards.
        /// </summary>
        private sealed class CardFlipGame : CardScript
        {
            private static List<CardInstance> FaceDown(EffectContext ctx) { return ctx.Opp.Prizes.Where(c => !ctx.Opp.FaceUpPrizeIds.Contains(c.Id)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return FaceDown(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var faceDown = FaceDown(ctx);
                if (faceDown.Count == 0) return;
                var labels = faceDown.Select((c, i) => "Face-down Prize card " + (i + 1)).ToArray();
                var prize = faceDown[labels.Length == 1 ? 0 : ctx.ChooseOption("Card-Flip Game: which of your opponent's face-down Prize cards?", labels)];
                int guess = ctx.ChooseOption("Card-Flip Game: guess what that Prize card is", "Energy card", "Trainer card", "Pokémon card");
                ctx.Opp.FaceUpPrizeIds.Add(prize.Id);
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { prize }, "Prize card turned face up (Card-Flip Game)");
                var d = prize.Def;
                bool right = guess == 0 ? d.IsEnergy : guess == 1 ? d.IsTrainer : d.IsPokemon;
                ctx.Log("Card-Flip Game: " + ctx.Engine.PlayerName(ctx.Player) + " guessed " + (guess == 0 ? "Energy" : guess == 1 ? "Trainer" : "Pokémon") + "; it's " + d.Name + (right ? " (right)." : " (wrong)."));
                if (right) ctx.Draw(2);
            }
        }

        /// <summary>New Pokédex: shuffle your deck, then look at up to 5 cards from the top and rearrange them as you like.</summary>
        private sealed class NewPokedex : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.ShuffleMyDeck();
                CardsOBF2.ReorderOwnTop(ctx, 5, "New Pokédex");
            }
        }

        /// <summary>Professor Elm: shuffle your hand into your deck, draw 7 cards. No more Trainer cards this turn.</summary>
        private sealed class ProfessorElm : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count + ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(7);
                LockTrainers(ctx);
            }
        }

        /// <summary>Double Gust: your opponent switches 1 of your Benched Pokémon (their choice) with your Active Pokémon; then you switch 1 of theirs with their Active Pokémon.</summary>
        private sealed class DoubleGust : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Bench.Count > 0 || ctx.Opp.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Bench.Count > 0 && ctx.Me.Active != null)
                {
                    var mine = ctx.Engine.ChoosePokemon(ctx.Opponent, ctx.Me.Bench.ToList(), "Double Gust: switch which of your opponent's Benched Pokémon with their Active Pokémon?", ctx.Source);
                    if (mine != null) ctx.Engine.SwitchActive(ctx.Player, mine);
                }
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null)
                {
                    var theirs = ctx.ChoosePokemon(ctx.Opp.Bench.ToList(), "Double Gust: switch which of your opponent's Benched Pokémon with their Active Pokémon?");
                    if (theirs != null) ctx.Engine.SwitchActive(ctx.Opponent, theirs);
                }
            }
        }

        /// <summary>Moo-Moo Milk: choose 1 of your Pokémon; flip 2 coins and remove 2 damage counters from it for each heads.</summary>
        private sealed class MooMooMilk : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Moo-Moo Milk: choose 1 of your Pokémon");
                int heads = ctx.FlipCoins(2);
                if (target != null && heads > 0) ctx.Heal(target, 20 * heads);
            }
        }

        /// <summary>
        /// Pokémon March: your opponent may put a Basic Pokémon from their deck onto their Bench; then you may do the same; each player
        /// shuffles (nothing for a player with a full Bench).
        /// </summary>
        private sealed class PokemonMarch : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                March(new EffectContext(ctx.Engine, ctx.Opponent, ctx.Card, null));
                March(ctx);
            }

            private static void March(EffectContext who)
            {
                if (who.BenchSpace <= 0 || who.Me.Deck.Count == 0) return;
                who.SearchToBench(d => true, 1, "Pokémon March: you may put a Basic Pokémon from your deck onto your Bench");
            }
        }

        // ================================================================== Stadiums

        /// <summary>Ecogym: an Energy card another player's attack, Pokémon Power or Trainer card discards from a Pokémon goes back to its owner's hand (basic Energy only: your ruling).</summary>
        private sealed class Ecogym : CardScript
        {
            public override bool ReturnsDiscardedEnergy(EffectContext src, PokemonInPlay from, CardInstance energy)
            {
                return energy != null && energy.Def.IsEnergy && energy.Def.IsBasicEnergy;
            }
        }

        /// <summary>Sprout Tower: damage done by Colorless Pokémon's attacks is 30 less (after Weakness and Resistance).</summary>
        private sealed class SproutTower : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !src.Engine.IsType(info.Attacker, EnergyType.Colorless)) return damage;
                return Math.Max(0, damage - 30);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Focus Band: if the holder would be Knocked Out by the opponent's attack, flip a coin; heads, it isn't and has 10 HP left. Then discard this card.</summary>
        private sealed class FocusBand : CardScript
        {
            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || src.Engine.CurrentSourceOwner == mon.Owner) return false;
                return src.Engine.FlipCoin(mon.Owner, "Focus Band");
            }

            public override void OnSavedFromKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon != null && mon.Tools.Contains(src.Card)) src.Engine.DiscardTool(mon, src.Card);
            }
        }

        /// <summary>
        /// Berry / Gold Berry: between turns, with at least N damage counters on the holder, you may remove N and discard this card;
        /// at the start of each turn it happens if it can.
        /// </summary>
        private sealed class HealBerry : CardScript
        {
            private readonly string name;
            private readonly int counters;

            public HealBerry(string name, int counters)
            {
                this.name = name;
                this.counters = counters;
            }

            public override void OnBetweenTurns(EffectContext src, bool startOfTurn)
            {
                var holder = src.Pokemon;
                if (holder == null || holder.DamageCounters < counters || !holder.Tools.Contains(src.Card)) return;
                if (!startOfTurn && !src.Engine.AskYesNo(src.Player, name + ": remove " + counters + " damage counters from " + holder.Name + " and discard " + name + "?", src.Source)) return;
                src.Log(name + " removes " + counters + " damage counters from " + holder.Name + ".");
                src.Heal(holder, counters * 10);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Miracle Berry: between turns (you may) or at the start of each turn, an Asleep, Confused, Paralyzed or Poisoned holder recovers from them; discard this card.</summary>
        private sealed class MiracleBerry : CardScript
        {
            private const SpecialCondition Cured = SpecialCondition.Asleep | SpecialCondition.Confused | SpecialCondition.Paralyzed | SpecialCondition.Poisoned;

            public override void OnBetweenTurns(EffectContext src, bool startOfTurn)
            {
                var holder = src.Pokemon;
                if (holder == null || (holder.Conditions & Cured) == 0 || !holder.Tools.Contains(src.Card)) return;
                if (!startOfTurn && !src.Engine.AskYesNo(src.Player, "Miracle Berry: cure " + holder.Name + " and discard Miracle Berry?", src.Source)) return;
                src.Log("Miracle Berry cures " + holder.Name + ".");
                src.Engine.RemoveCondition(holder, holder.Conditions & Cured);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }
    }
}
