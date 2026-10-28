using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Obsidian Flames (OBF) 124-197: the Pokémon and Trainers that weren't already in the pool (all G-mark, Expanded only).
    /// Reprints already in the pool (Houndoom ex, Mawile, Kingambit, Melmetal ex...) are alternate prints. Pokémon whose
    /// attacks only do their printed damage need no script: Paldean Wooper (127), Houndour (132), Salandit, Salazzle,
    /// Excadrill, Meltan, Varoom (155), Dratini, Pidgey, Pidgeotto, Zigzagoon, Lillipup, Herdier, Lechonk (180, 182).
    /// </summary>
    public static class CardsOBF2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("OBF-124", new ComboMon(new SplittingBeamMon(), new RecoilMon("Gaia Press", 30)));
            r.RegisterByKey("OBF-126", new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned));
            r.RegisterByKey("OBF-128", new ComboMon(new CardsPOR.PoisonRingMon("Poison Ring"), new CardsSSP2.StompOffMon("Muddy Hammer", 1)));
            r.RegisterByKey("OBF-129", new ComboMon(new SplatteringPoisonMon(), new CardsBLK.VenoshockMon(120)));
            r.RegisterByKey("OBF-131", new ComboMon(new CoordinatedPackMon(), new TailsNothingMon("Focus Fangs")));
            r.RegisterByKey("OBF-133", new ComboMon(new DaringStrikeMon("Daring Strike", 70), new CardsTWM1.SelfDiscardMon("Shadow Slash", 1)));
            r.RegisterByKey("OBF-135", new AbsolExOBF());
            r.RegisterByKey("OBF-136", new ComboMon(new ConditionAttackMon("Dark Slumber", SpecialCondition.Asleep), new NightCycloneMon()));
            r.RegisterByKey("OBF-137", new NoRetreatMon("Hug"));
            r.RegisterByKey("OBF-138", new MalamarOBF());
            r.RegisterByKey("OBF-142", new LockAfterAttackMon("Slashing Steel", true));
            r.RegisterByKey("OBF-144", new BronzorOBF());
            r.RegisterByKey("OBF-145", new BronzongOBF());
            r.RegisterByKey("OBF-146", new ComboMon(new CoinsMon("Triple Nose", 3), new LockAfterAttackMon("Iron Buster", false)));
            r.RegisterByKey("OBF-148", new CoinsMon("Triple Cutter", 3));
            r.RegisterByKey("OBF-149", new CardsDEX.FuryCutterMon(120, 60, 20));
            r.RegisterByKey("OBF-151", new CardsMEW1.WithdrawMon("Defense Curl"));
            r.RegisterByKey("OBF-154", new DrawAttackMon("Spinning Draw", 1));
            r.RegisterByKey("OBF-156", new RevavroomExOBF());
            r.RegisterByKey("OBF-158", new CoinsMon("Dragon Tail", 2));
            r.RegisterByKey("OBF-159", new DragoniteExOBF());
            r.RegisterByKey("OBF-160", new CardsUPR3.AbyssalSleepMon("Soothing Lullaby"));
            r.RegisterByKey("OBF-161", new Cards30C.SelfCounterBonusMon("Outrage", 10));
            r.RegisterByKey("OBF-165", new DrawAttackMon("Spike Draw", 2));
            r.RegisterByKey("OBF-166", new EeveeOBF());
            r.RegisterByKey("OBF-168", new RecoilMon("Reckless Charge", 30));
            r.RegisterByKey("OBF-169", new CardsASC.SelfGuardMon("Fluffy Guard", 20));
            r.RegisterByKey("OBF-172", new CardsOBF1.RetreatCostDamageMon("Chomp Chomp Panic", 50, true));
            r.RegisterByKey("OBF-173", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("OBF-174", new BouffalantOBF());
            r.RegisterByKey("OBF-175", new CoinsMon("Stompy Stomp", 2));
            r.RegisterByKey("OBF-176", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("OBF-177", new CardsDRI2.CrunchMon("Crunch"));
            r.RegisterByKey("OBF-178", new CardsSSP2.StompOffMon("Nicked Nibble", 1));
            r.RegisterByKey("OBF-179", new GreedentExOBF());
            r.RegisterByKey("OBF-181", new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("OBF-183", new OinkologneOBF());
            r.RegisterByKey("OBF-185", new FlamigoOBF());

            r.RegisterByName("Brassius", new Brassius());
            r.RegisterByName("Geeta", new Geeta());
            r.RegisterByName("Letter of Encouragement", new LetterOfEncouragement());
            r.RegisterByName("Ortega", new Ortega());
            r.RegisterByName("Patrol Cap", new PatrolCap());
            r.RegisterByName("Pokémon League Headquarters", new PokemonLeagueHeadquarters());
            r.RegisterByName("Poppy", new Poppy());
            r.RegisterByName("Ryme", new Ryme());
            r.RegisterByName("Team Star Grunt", new TeamStarGrunt());
            r.RegisterByName("Town Store", new TownStore());
            r.RegisterByName("Vengeful Punch", new VengefulPunch());
        }

        // ================================================================== Fighting / Darkness

        /// <summary>Splitting Beam: also 20 damage to 2 of the opponent's Benched Pokémon (no Weakness or Resistance for the Bench).</summary>
        private sealed class SplittingBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splitting Beam") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                int n = Math.Min(2, bench.Count);
                var picks = ctx.ChoosePokemonMany(bench, n, n, "Splitting Beam: choose " + n + " of your opponent's Benched Pokémon to damage");
                foreach (var mon in picks) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Splattering Poison: both Active Pokémon are now Poisoned.</summary>
        private sealed class SplatteringPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splattering Poison") return false;
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Poisoned);
                if (ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Coordinated Pack: for each of your Benched Houndour, attach a Basic [D] Energy card from your deck to it.</summary>
        private sealed class CoordinatedPackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coordinated Pack") return false;
                var pack = ctx.Me.Bench.Where(m => m.Def.Name == "Houndour").ToList();
                if (pack.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Darkness), pack.Count,
                    "Coordinated Pack: choose up to " + pack.Count + " Basic Darkness Energy (1 for each of your Benched Houndour)");
                var waiting = pack.ToList();
                foreach (var card in found)
                {
                    if (waiting.Count == 0) { ctx.Me.Deck.Add(card); continue; }
                    var target = ctx.ChoosePokemon(waiting, "Coordinated Pack: attach " + card.Def.Name + " to which Houndour?");
                    waiting.Remove(target);
                    ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Daring Strike: 70 more damage if the opponent's Active Pokémon is an Evolution Pokémon. Used for Daring Strike, Clean Hit (the attack name and the numbers are passed in).</summary>
        internal sealed class DaringStrikeMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public DaringStrikeMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Def.IsEvolution ? amount : 0));
                return true;
            }
        }

        /// <summary>Future Sight: look at the top 3 cards of either player's deck and put them back in any order. Cursed Slug: +120 if the opponent has 3 or fewer cards in hand.</summary>
        private sealed class AbsolExOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Future Sight")
                {
                    bool mine = ctx.Me.Deck.Count > 0;
                    bool theirs = ctx.Opp.Deck.Count > 0;
                    if (!mine && !theirs) return true;
                    bool chooseOwn = mine && (!theirs || ctx.ChooseOption("Future Sight: look at the top 3 cards of which deck?", "Your deck", "Your opponent's deck") == 0);
                    if (chooseOwn) ReorderOwnTop(ctx, 3, "Future Sight");
                    else CardsDRI2.ReorderOpponentTop(ctx, 3, "Future Sight");
                    return true;
                }
                if (atk.Name == "Cursed Slug")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Opp.Hand.Count <= 3 ? 120 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>"Look at the top N cards of your deck and put them back in any order."</summary>
        public static void ReorderOwnTop(EffectContext ctx, int count, string source)
        {
            var top = ctx.LookAtTop(count);
            if (top.Count < 2)
            {
                if (top.Count == 1) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of their deck.");
                return;
            }
            foreach (var c in top) ctx.Me.Deck.Remove(c);
            var ordered = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = ctx.ChooseCards(left, 1, 1, source + ": choose the card to put " + (ordered.Count == 0 ? "on top of your deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                ordered.Add(pick);
                left.Remove(pick);
            }
            ordered.Add(left[0]);
            ctx.Me.Deck.InsertRange(0, ordered);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + ordered.Count + " cards back on top of their deck in any order.");
        }

        /// <summary>
        /// Night Cyclone: move all Energy from this Pokémon to your Benched Pokémon in any way you like. With no Benched Pokémon
        /// the Energy stays (Compendium, Tropius's Tropic Breeze).
        /// </summary>
        private sealed class NightCycloneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                foreach (var card in me.Energy.ToList())
                {
                    var bench = ctx.Me.Bench.Where(m => m != me).ToList();
                    if (bench.Count == 0) break;
                    var target = ctx.ChoosePokemon(bench, "Night Cyclone: move " + card.Def.Name + " to which Benched Pokémon?");
                    ctx.Engine.MoveEnergy(me, target, card);
                }
                return true;
            }
        }

        /// <summary>Psychic Insight: once during your turn, look at the top card of the opponent's deck, then the top card of yours. Hypnotic Ray: Asleep.</summary>
        private sealed class MalamarOBF : Mon
        {
            private static readonly Mon HypnoticRay = new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep);

            protected override string Activated { get { return "Psychic Insight"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var theirs = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.NoteLookedAt(theirs, "Top card of your opponent's deck", ctx.Player);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of the other player's deck.");
                if (ctx.Me.Deck.Count == 0) return;
                ctx.LookAtTop(1);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of their deck.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HypnoticRay.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Mirror Draw: draw cards until you have as many cards in your hand as the opponent.</summary>
        private sealed class BronzorOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Draw") return false;
                ctx.DrawUntil(ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Oracle Press: during the opponent's next turn, prevent all effects of their attacks done to it. Extrasensory: +90 with hands of equal size.</summary>
        private sealed class BronzongOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Oracle Press")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (ctx.InPlay(me))
                        ctx.AddEffect(new CardsPAR1.AttackEffectShieldEffect(me.Name + " is protected from the effects of attacks during the opponent's next turn (Oracle Press)",
                            ctx.Player, me.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Extrasensory")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count == ctx.Opp.Hand.Count ? 90 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Tune-Up: this Pokémon can have up to 4 Pokémon Tools (the engine discards down to 1 if it loses the Ability). Wild Drift: takes 30 less next turn.</summary>
        private sealed class RevavroomExOBF : Mon
        {
            private static readonly Mon WildDrift = new CardsASC.SelfGuardMon("Wild Drift", 30);

            public override int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(max, 4) : max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WildDrift.HandleAttack(ctx, atk); }
        }

        // ================================================================== Dragon

        /// <summary>Mighty Meteor (Tera): flip a coin; heads +140, tails this Pokémon can't attack during your next turn.</summary>
        private sealed class DragoniteExOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mighty Meteor") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 140 : 0));
                if (!heads) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Cheer Up: attach an Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class EeveeOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cheer Up") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "Cheer Up: attach which Energy card from your hand?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Cheer Up: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target == null) return true;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
                return true;
            }
        }

        /// <summary>Bouffer: this Pokémon takes 20 less damage from attacks. Damage Rush: flip until tails, +30 per heads.</summary>
        private sealed class BouffalantOBF : Mon
        {
            private static readonly Mon DamageRush = new CardsMEG.FlipUntilTailsBonusMon("Damage Rush", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DamageRush.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Never Ever Enough (Tera): look at the top 3 cards of your deck; put them all into your hand, or discard them all and draw
        /// 3 cards. Slip 'n' Roll: can't be used during your next turn.
        /// </summary>
        private sealed class GreedentExOBF : Mon
        {
            private static readonly Mon SlipNRoll = new LockAfterAttackMon("Slip 'n' Roll", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Never Ever Enough") return SlipNRoll.HandleAttack(ctx, atk);
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return true;
                string names = string.Join(", ", top.Select(c => c.Def.Name).ToArray());
                if (ctx.AskYesNo("Never Ever Enough: put " + names + " into your hand? (No: discard them and draw 3 cards)"))
                {
                    ctx.DeckToHand(top);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the top " + top.Count + " cards of their deck into their hand.");
                    return true;
                }
                ctx.Engine.DiscardTopOfDeck(ctx.Player, top.Count);
                ctx.Draw(3);
                return true;
            }
        }

        /// <summary>High-Impact Kick: flip a coin; on tails this Pokémon also does 60 damage to itself. Confounding Cologne: Confused.</summary>
        private sealed class OinkologneOBF : Mon
        {
            private static readonly Mon Cologne = new ConditionAttackMon("Confounding Cologne", SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High-Impact Kick") return Cologne.HandleAttack(ctx, atk);
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads) ctx.DamageSelf(60);
                return true;
            }
        }

        /// <summary>Synchronized Feathers: if a Flamigo is on your Bench, also 60 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class FlamigoOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchronized Feathers") return false;
                var me = ctx.Pokemon;
                bool partner = ctx.Me.Bench.Any(m => m != me && m.Def.Name == "Flamigo");
                ctx.DamageDefending(atk.Damage);
                if (partner) ctx.DamageOneBenched(60);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Brassius: shuffle your hand into your deck, then draw that many cards plus 1 (it doesn't count itself: Compendium, 2023-08-10).</summary>
        private sealed class Brassius : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || ctx.Me.Hand.Count(c => c != ctx.Card) > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = ctx.Me.Hand.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(n + 1);
            }
        }

        /// <summary>Geeta: attach up to 2 Basic Energy cards from your deck to 1 of your Pokémon; your Pokémon can't attack this turn (even ones played later).</summary>
        private sealed class Geeta : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Geeta: choose up to 2 Basic Energy cards to attach to 1 of your Pokémon");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Geeta: attach the Energy to which of your Pokémon?");
                    foreach (var card in found)
                    {
                        if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                        else ctx.Me.Deck.Add(card);
                    }
                }
                ctx.ShuffleMyDeck();
                ctx.AddEffect(new PlayerCantAttackEffect(ctx.Engine.Possessive(ctx.Player) + " Pokémon can't attack this turn (Geeta)", ctx.Player, ctx.Turn));
            }
        }

        /// <summary>"During this turn, your Pokémon can't attack": every Pokémon of that player, including ones that come into play later.</summary>
        public sealed class PlayerCantAttackEffect : ContinuousEffect
        {
            public PlayerCantAttackEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon) { return mon.Owner == Owner; }
        }

        /// <summary>
        /// Letter of Encouragement: only if any of your Pokémon were Knocked Out during the opponent's last turn (by an attack or an
        /// Ability; a Pokémon Checkup Knock Out doesn't count: Compendium, 2023-08-10). Search for up to 3 Basic Energy cards.
        /// </summary>
        private sealed class LetterOfEncouragement : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.WasKnockedOutLastOpponentTurn();
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicEnergy, 3, "Letter of Encouragement: search your deck for up to 3 Basic Energy cards");
            }
        }

        /// <summary>Ortega: the opponent reveals their hand; you put a card from it on the bottom of their deck, then they may draw a card.</summary>
        private sealed class Ortega : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (TefPokemon.OpponentShielded(ctx)) return;
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "Ortega: put which card on the bottom of your opponent's deck?")[0];
                ctx.Opp.Hand.Remove(pick);
                ctx.Opp.Deck.Add(pick);
                ctx.Log(pick.Def.Name + " was put on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck (Ortega).");
                if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Ortega: draw a card?", ctx.Source))
                    ctx.Engine.DrawCards(ctx.Opponent, 1);
            }
        }

        /// <summary>
        /// Patrol Cap: while the holder is in the Active Spot, cards in your deck can't be discarded by effects of the opponent's
        /// attacks, Abilities, Item cards, Pokémon Tool cards or Supporter cards (GameEngine.DeckDiscardBlocked).
        /// </summary>
        private sealed class PatrolCap : CardScript
        {
            public override bool ProtectsDeckFromDiscard(EffectContext src, int player)
            {
                return src.Pokemon != null && player == src.Player && src.PokemonIsActive;
            }
        }

        /// <summary>
        /// Pokémon League Headquarters: attacks of every Basic Pokémon in play cost [C] more. A cost an effect sets isn't raised, and
        /// a free attack then needs 1 Energy (Compendium, 2024). Lunatone's New Moon keeps it off its owner's Basics (2023-08-10).
        /// </summary>
        private sealed class PokemonLeagueHeadquarters : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon.IsBasic) cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Poppy: move up to 2 Energy from 1 of your Pokémon to another of your Pokémon.</summary>
        private sealed class Poppy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon().Count >= 2 && ctx.MyPokemon().Any(m => m.Energy.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mine = ctx.MyPokemon();
                var givers = mine.Where(m => m.Energy.Count > 0).ToList();
                if (givers.Count == 0 || mine.Count < 2) return;
                var from = ctx.ChoosePokemon(givers, "Poppy: move Energy from which of your Pokémon?");
                if (from == null) return;
                var energy = from.Energy.ToList();
                var picks = energy.Count == 1 ? energy : ctx.ChooseCards(energy, 1, Math.Min(2, energy.Count), "Poppy: choose up to 2 Energy to move");
                var to = ctx.ChoosePokemon(mine.Where(m => m != from).ToList(), "Poppy: move the Energy to which Pokémon?");
                if (to == null) return;
                foreach (var card in picks) ctx.Engine.MoveEnergy(from, to, card);
            }
        }

        /// <summary>Ryme: draw 3 cards, then switch out the opponent's Active Pokémon (they choose the new one).</summary>
        private sealed class Ryme : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                ctx.OpponentSwitchesActive();
            }
        }

        /// <summary>Team Star Grunt: put an Energy attached to the opponent's Active Pokémon on top of their deck.</summary>
        private sealed class TeamStarGrunt : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (defending == null || defending.Energy.Count == 0) return;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Team Star Grunt on " + defending.Name + " was prevented.");
                    return;
                }
                var pick = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Team Star Grunt: put which Energy on top of your opponent's deck?")[0];
                defending.Energy.Remove(pick);
                ctx.Opp.Deck.Insert(0, pick);
                ctx.Engine.Emit(GameEventType.Info, ctx.Opponent, pick.Def.Name + " was put on top of " + ctx.Engine.Their(ctx.Opponent) + " deck (Team Star Grunt).", pick.Id, defending.Uid);
            }
        }

        /// <summary>Town Store: once during each player's turn, they may search their deck for a Pokémon Tool card and put it into their hand.</summary>
        private sealed class TownStore : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTool, 1, "Town Store: search your deck for a Pokémon Tool card");
                return false;
            }
        }

        /// <summary>
        /// Vengeful Punch: if the holder is Knocked Out by damage from the opponent's attack, 4 damage counters on the Attacking
        /// Pokémon. A devolved Pokémon isn't the one that took the damage (Compendium, Golurk V, 2023-11-09).
        /// </summary>
        private sealed class VengefulPunch : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || src.Pokemon.DevolvedAfterHit) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player) return;
                src.Engine.PlaceDamageCounters(attacker, 4, "Vengeful Punch");
            }
        }
    }
}
