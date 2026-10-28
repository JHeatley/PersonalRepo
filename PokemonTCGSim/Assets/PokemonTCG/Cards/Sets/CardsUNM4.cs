using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unified Minds (UNM) part 4: Trainers and Special Energy (UNM 186-213, and Viridian Forest, UNM 256, also in Team Up).
    /// Your rulings (v72): Channeler removes the effects of the opponent's attacks on you and your Pokémon but not Special
    /// Conditions. U-Turn Board and Recycle Energy go to the hand instead of the discard pile whenever they're discarded from play
    /// (GameEngine.DiscardFromPlay). Blaine's Quiz Show: the opponent guesses from the names of the Pokémon in the game that have
    /// the named attack.
    /// </summary>
    public static class CardsUNM4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Blaine's Quiz Show", new BlainesQuizShow());
            r.RegisterByName("Blizzard Town", new BlizzardTown());
            r.RegisterByName("Blue's Tactics", new BluesTactics());
            r.RegisterByName("Channeler", new Channeler());
            r.RegisterByName("Cherish Ball", new SearchItem(d => d.IsPokemon && d.IsGX, "Cherish Ball: search your deck for a Pokémon-GX"));
            r.RegisterByName("Coach Trainer", new CoachTrainer());
            r.RegisterByName("Dark City", new DarkCity());
            r.RegisterByName("Ear-Ringing Bell", new EarRingingBell());
            r.RegisterByName("Flyinium Z: Air Slash", new ZCrystal("Air Slash", SpeedingSkystrike));
            r.RegisterByName("Giant Bomb", new GiantBomb());
            r.RegisterByName("Great Potion", new GreatPotion());
            r.RegisterByName("Grimsley", new Grimsley());
            r.RegisterByName("Hapu", new Hapu());
            r.RegisterByName("Karate Belt", new KarateBelt());
            r.RegisterByName("Misty's Favor", new SearchUpTo(d => d.IsSupporter, 3, "Misty's Favor: search your deck for up to 3 Supporter cards"));
            r.RegisterByName("Normalium Z: Tackle", new ZCrystal("Tackle", BarrelingBlitz));
            r.RegisterByName("Poké Maniac", new SearchUpTo(d => d.IsPokemon && d.RetreatCost == 4, 3, "Poké Maniac: search your deck for up to 3 Pokémon with a Retreat Cost of exactly 4"));
            r.RegisterByName("Pokémon Research Lab", new PokemonResearchLab());
            r.RegisterByName("Reset Stamp", new ResetStamp());
            r.RegisterByName("Slumbering Forest", new SlumberingForest());
            r.RegisterByName("Stadium Nav", new StadiumNav());
            r.RegisterByName("Tag Switch", new TagSwitch());
            r.RegisterByName("U-Turn Board", new UTurnBoard());
            r.RegisterByName("Recycle Energy", new RecycleEnergy());
            r.RegisterByName("Weakness Guard Energy", new WeaknessGuardEnergy());
            r.RegisterByName("Viridian Forest", new ViridianForest());
        }

        // ================================================================== Shared helpers

        /// <summary>"Search your deck for up to N [cards], reveal them, and put them into your hand."</summary>
        internal sealed class SearchUpTo : CardScript
        {
            private readonly Func<CardDef, bool> filter;
            private readonly int count;
            private readonly string prompt;

            public SearchUpTo(Func<CardDef, bool> filter, int count, string prompt)
            {
                this.filter = filter;
                this.count = count;
                this.prompt = prompt;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(filter, count, prompt); }
        }

        /// <summary>
        /// Z-Crystals: a holder that has the named attack can use the GX attack printed on this Tool (it still needs the Energy).
        /// The GX attack counts as the player's GX attack for the game.
        /// </summary>
        internal sealed class ZCrystal : CardScript
        {
            private readonly string baseAttack;
            private readonly Action<EffectContext, AttackDef> resolve;

            public ZCrystal(string baseAttack, Action<EffectContext, AttackDef> resolve)
            {
                this.baseAttack = baseAttack;
                this.resolve = resolve;
            }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0 || !mon.Def.Attacks.Any(a => a.Name == baseAttack))
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex) { resolve(ctx, ctx.Attack(attackIndex)); }
        }

        /// <summary>Speeding Skystrike-GX: 180, then prevent all effects of attacks, damage included, done to this Pokémon during the opponent's next turn.</summary>
        private static void SpeedingSkystrike(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
        }

        /// <summary>Barreling Blitz-GX: flip a coin until you get tails, 40 more damage for each heads.</summary>
        private static void BarrelingBlitz(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + 40 * ctx.FlipUntilTails());
        }

        // ================================================================== Supporters

        /// <summary>
        /// Blaine's Quiz Show: put a Pokémon from your hand face down and name 1 of its attacks; the opponent guesses its name, then
        /// it's revealed. A right guess: the opponent draws 4 cards; a wrong one: you draw 4. The Pokémon goes back to your hand.
        /// </summary>
        private sealed class BlainesQuizShow : CardScript
        {
            private static List<CardInstance> Candidates(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(x => x != ctx.Card && x.Def.IsPokemon && x.Def.Attacks.Count > 0).ToList();
            }

            /// <summary>Every card in the game, so the opponent can be offered the names of the Pokémon that have the attack.</summary>
            private static IEnumerable<CardDef> CardsInGame(EffectContext ctx)
            {
                foreach (var p in ctx.State.Players)
                {
                    foreach (var c in p.Deck) yield return c.Def;
                    foreach (var c in p.Hand) yield return c.Def;
                    foreach (var c in p.Discard) yield return c.Def;
                    foreach (var c in p.Prizes) yield return c.Def;
                    foreach (var c in p.LostZone) yield return c.Def;
                    foreach (var mon in p.InPlay())
                        foreach (var c in mon.Stack) yield return c.Def;
                }
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Candidates(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var candidates = Candidates(ctx);
                if (candidates.Count == 0) return;
                var hidden = ctx.ChooseCards(candidates, 1, 1, "Blaine's Quiz Show: put which Pokémon from your hand face down?")[0];
                var attacks = hidden.Def.Attacks.Select(a => a.Name).Distinct().ToArray();
                string attack = attacks.Length == 1 ? attacks[0] : attacks[ctx.ChooseOption("Blaine's Quiz Show: tell your opponent the name of which attack?", attacks)];
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a Pokémon face down: it has the attack " + attack + ".");
                var names = CardsInGame(ctx).Where(d => d.IsPokemon && d.Attacks.Any(a => a.Name == attack)).Select(d => d.Name).Distinct().OrderBy(n => n).ToList();
                if (!names.Contains(hidden.Def.Name)) names.Add(hidden.Def.Name);
                int guess = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Blaine's Quiz Show: guess the face-down Pokémon (it has " + attack + ")",
                    names.Select((label, i) => new DecisionOption { Label = label, Index = i }).ToList(), ctx.Source);
                string guessed = names[Math.Max(0, Math.Min(names.Count - 1, guess))];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { hidden }, "revealed the face-down Pokémon");
                bool right = guessed == hidden.Def.Name;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed " + guessed + ": " + (right ? "right." : "wrong."));
                if (right) ctx.Engine.DrawCards(ctx.Opponent, 4);
                else ctx.Draw(4);
            }
        }

        /// <summary>Blue's Tactics: at the end of this turn, draw cards until you have 8 cards in your hand.</summary>
        private sealed class BluesTactics : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new DrawUntilAtEndOfTurnEffect(ctx.Engine.PlayerName(ctx.Player) + " draws until 8 cards at the end of the turn (Blue's Tactics)", ctx.Player, ctx.Turn, 8));
            }
        }

        private sealed class DrawUntilAtEndOfTurnEffect : ContinuousEffect
        {
            private readonly int size;

            public DrawUntilAtEndOfTurnEffect(string name, int owner, int turn, int size) : base(name, owner, -1, turn, false)
            {
                this.size = size;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn || engine.State.CurrentPlayer != Owner) return;
                int n = size - engine.State.Players[Owner].Hand.Count;
                if (n > 0) engine.DrawCards(Owner, n);
            }
        }

        /// <summary>
        /// Channeler: remove all effects of attacks on you and on each of your Pokémon, your own attacks' included (your ruling,
        /// v134, following the Compendium: it removes Steelix's Gigaton Shake; Special Conditions stay, v72): timed effects on your
        /// Pokémon, effects on you (your Pokémon can't attack, your Trainer cards are stopped, your coin flips are tails), Item lock
        /// and evolution lock.
        /// </summary>
        private sealed class Channeler : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var mine = new HashSet<int>(ctx.MyPokemon().Select(m => m.Uid));
                var trainers = ctx.Me.Hand.Where(x => x.Def.IsTrainer && x != ctx.Card).ToList();
                int me = ctx.Player;
                var myPokemon = ctx.MyPokemon();
                var engine = ctx.Engine;
                int removed = ctx.State.Effects.RemoveAll(e => e.FromAttack
                    && (mine.Contains(e.TargetUid)
                        || (e.TargetUid < 0 && (trainers.Any(c => e.BlocksTrainer(engine, me, c)) || myPokemon.Any(m => e.PreventsAttacks(engine, m))
                                                || myPokemon.Any(m => m.Def.Attacks.Any(a => e.PreventsAttackNamed(engine, m, a.Name)))
                                                || e.ForcesTails(engine, me) || e.ForcesTrainerFlip(engine, me) || e.IsOnPlayer(me)))));
                if (ctx.Me.ItemLockTurn >= ctx.Turn)
                {
                    ctx.Me.ItemLockTurn = -1;
                    removed++;
                }
                if (ctx.Me.EvolveLockTurn >= ctx.Turn)
                {
                    ctx.Me.EvolveLockTurn = -1;
                    removed++;
                }
                ctx.Log("Channeler: " + (removed == 0 ? "there were no effects of attacks to remove." : "the effects of attacks on " + ctx.Engine.PlayerName(ctx.Player) + " and their Pokémon were removed."));
            }
        }

        /// <summary>Coach Trainer: draw 2 cards, 2 more if your Active Pokémon is a TAG TEAM.</summary>
        private sealed class CoachTrainer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool tag = ctx.Me.Active != null && ctx.Me.Active.Def.IsTagTeam;
                ctx.Draw(tag ? 4 : 2);
            }
        }

        /// <summary>Grimsley: move up to 3 damage counters from 1 of the opponent's Pokémon to another of their Pokémon.</summary>
        private sealed class Grimsley : CardScript
        {
            private static List<PokemonInPlay> Givers(EffectContext ctx) { return ctx.OpponentPokemon(m => m.DamageCounters > 0); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.OpponentPokemon().Count > 1 && Givers(ctx).Count > 0 && !ctx.Engine.CounterMovementBlocked();
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(Givers(ctx), "Grimsley: move damage counters from which of your opponent's Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != from), "Grimsley: move them to which of your opponent's Pokémon?");
                if (to == null) return;
                int max = Math.Min(3, from.DamageCounters);
                int n = max == 1 ? 1 : ctx.AskNumber(1, max, "Grimsley: move how many damage counters (up to " + max + ")?");
                ctx.Engine.MoveDamageCounters(from, to, n, "Grimsley");
            }
        }

        /// <summary>Hapu: look at the top 6 cards of your deck, put 2 into your hand and discard the rest.</summary>
        private sealed class Hapu : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                if (top.Count == 0) return;
                int n = Math.Min(2, top.Count);
                var picks = ctx.ChooseCards(top, n, n, "Hapu: put 2 of these cards into your hand (the rest are discarded)");
                ctx.DeckToHand(picks);
                int rest = top.Count - picks.Count;
                if (rest > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, rest);
            }
        }

        // ================================================================== Items

        /// <summary>Great Potion: heal 50 damage from your Active Pokémon-GX.</summary>
        private sealed class GreatPotion : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && active.Def.IsGX && active.DamageCounters > 0 && !ctx.Engine.HealingBlocked(active);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 50);
            }
        }

        /// <summary>Reset Stamp: the opponent shuffles their hand into their deck and draws a card for each of their remaining Prize cards.</summary>
        private sealed class ResetStamp : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, ctx.Opp.Prizes.Count);
            }
        }

        /// <summary>Stadium Nav: flip 2 coins; for each heads, search your deck for a Stadium card and put it into your hand.</summary>
        private sealed class StadiumNav : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.SearchToHand(d => d.IsStadiumCard, heads, "Stadium Nav: search your deck for up to " + heads + " Stadium card" + (heads == 1 ? "" : "s"));
            }
        }

        /// <summary>Tag Switch: move up to 2 Energy from 1 of your TAG TEAM Pokémon to another of your Pokémon.</summary>
        private sealed class TagSwitch : CardScript
        {
            private static List<PokemonInPlay> Givers(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsTagTeam && m.Energy.Count > 0); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon().Count > 1 && Givers(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(Givers(ctx), "Tag Switch: move Energy from which of your TAG TEAM Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Tag Switch: move the Energy to which of your Pokémon?");
                if (to == null) return;
                var picks = ctx.ChooseCards(from.Energy.ToList(), 1, Math.Min(2, from.Energy.Count), "Tag Switch: move up to 2 Energy from " + from.Name + " to " + to.Name);
                foreach (var card in picks) ctx.Engine.MoveEnergy(from, to, card);
            }
        }

        // ================================================================== Tools

        /// <summary>Ear-Ringing Bell: if the holder is your Active Pokémon and is damaged by an opponent's attack, the Attacking Pokémon is now Confused.</summary>
        private sealed class EarRingingBell : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || !ctx.IsActive(attacker)) return;
                ctx.Log("Ear-Ringing Bell: " + attacker.Name + " is now Confused.");
                ctx.Engine.ApplyCondition(attacker, SpecialCondition.Confused);
            }
        }

        /// <summary>
        /// Giant Bomb: discarded at the end of the opponent's turn. If the holder is your Active Pokémon and takes 180 or more damage
        /// from an opponent's attack, 10 damage counters on the Attacking Pokémon.
        /// </summary>
        private sealed class GiantBomb : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || damage < 180 || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                ctx.Engine.PlaceDamageCounters(attacker, 10, "Giant Bomb");
            }

            /// <summary>Discarded at the end of the opponent's turn by the engine, even while Tools have no effect (like Bursting Balloon, v97).</summary>
            public override bool DiscardedAtEndOfOpponentsTurn { get { return true; } }
        }

        /// <summary>Karate Belt: while you have more Prize cards left than the opponent, the holder's attacks cost [F] less.</summary>
        private sealed class KarateBelt : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Me.Prizes.Count <= src.Opp.Prizes.Count) return;
                cost.Remove(EnergyType.Fighting);
            }
        }

        /// <summary>U-Turn Board: the holder's Retreat Cost is [C] less; discarded from play, it goes into your hand instead.</summary>
        private sealed class UTurnBoard : CardScript
        {
            public override bool GoesToHandWhenDiscardedFromPlay { get { return true; } }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(0, cost - 1) : cost;
            }
        }

        // ================================================================== Stadiums

        /// <summary>Blizzard Town: Pokémon with 40 HP or less left (both players') can't attack.</summary>
        private sealed class BlizzardTown : CardScript
        {
            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && src.Engine.RemainingHp(mon) <= 40;
            }
        }

        /// <summary>Dark City: Basic Darkness Pokémon in play (both players') have no Retreat Cost.</summary>
        private sealed class DarkCity : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.Def.Stage == Stage.Basic && src.Engine.IsType(mon, EnergyType.Darkness) ? 0 : cost;
            }
        }

        /// <summary>
        /// Pokémon Research Lab: once during each player's turn, they may search their deck for up to 2 Pokémon that evolve from
        /// Unidentified Fossil and put them onto their Bench; if they do, their turn ends.
        /// </summary>
        private sealed class PokemonResearchLab : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            protected override bool Use(EffectContext ctx)
            {
                int n = Math.Min(2, ctx.BenchSpace);
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == "Unidentified Fossil" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    "Pokémon Research Lab: put up to " + n + " Pokémon that evolve from Unidentified Fossil onto your Bench");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
                ctx.Log("Pokémon Research Lab: " + ctx.Engine.PlayerName(ctx.Player) + "'s turn ends.");
                ctx.Engine.RequestEndTurn();
                return false;
            }
        }

        /// <summary>Slumbering Forest: an Asleep Pokémon's owner flips 2 coins instead of 1 between turns; it wakes up only on 2 heads.</summary>
        private sealed class SlumberingForest : CardScript
        {
            public override int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins) { return Math.Max(coins, 2); }
        }

        /// <summary>Viridian Forest: once during each player's turn, they may discard a card from their hand to search their deck for a basic Energy card.</summary>
        private sealed class ViridianForest : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Viridian Forest: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (ctx.Me.Discard.Contains(card) || ctx.Me.LostZone.Contains(card)) ctx.SearchToHand(d => d.IsBasicEnergy, 1, "Viridian Forest: search your deck for a basic Energy card");
                return false;
            }
        }

        // ================================================================== Special Energy

        /// <summary>Recycle Energy: provides [C]; discarded from play, it goes into your hand instead.</summary>
        private sealed class RecycleEnergy : CardScript
        {
            public override bool GoesToHandWhenDiscardedFromPlay { get { return true; } }
        }

        /// <summary>Weakness Guard Energy: provides [C]; the Pokémon it's attached to has no Weakness.</summary>
        private sealed class WeaknessGuardEnergy : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }
        }
    }
}
