using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Pokémon 151 (MEW) 1-99: the Pokémon that weren't already in the pool (all G-mark, Expanded only). Reprints already in
    /// the pool (Venusaur ex, Charizard ex, Blastoise ex, Alakazam ex, Mew ex, Zapdos ex, Kangaskhan ex, Mewtwo...) are
    /// alternate prints. Pokémon whose attacks only do their printed damage need no script: Weedle, Nidoran♂, Nidorino,
    /// Venonat, Diglett, Dugtrio, Weepinbell, Tentacool, Seel, Gastly, Drowzee.
    /// </summary>
    public static class CardsMEW1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MEW-1", new CardsTWM1.HealSelfMon("Leech Seed", 20));
            r.RegisterByKey("MEW-2", new CardsTWM1.HealSelfMon("Leech Seed", 20));
            r.RegisterByKey("MEW-5", new CardsTWM1.SelfDiscardMon("Fire Blast", 1));
            r.RegisterByKey("MEW-7", new WithdrawMon("Withdraw"));
            r.RegisterByKey("MEW-8", new WartortleMEW());
            r.RegisterByKey("MEW-10", new DefendingTypeBonusMon("Leaf Munch", EnergyType.Grass, 30));
            r.RegisterByKey("MEW-11", new WithdrawMon("Defensive Posture"));
            r.RegisterByKey("MEW-12", new ComboMon(new CardsDRI1.WhirlwindMon("Whirlwind"), new ByeByeFlightMon()));
            r.RegisterByKey("MEW-14", new CardsBRS1.LivelyFruitMon());
            r.RegisterByKey("MEW-15", new BeedrillMEW());
            r.RegisterByKey("MEW-18", new DodgeMon("Fly", true));
            r.RegisterByKey("MEW-19", new CounterBonusMon("Gnaw the Wound", 10));
            r.RegisterByKey("MEW-20", new CounterBonusMon("Second Bite", 30));
            r.RegisterByKey("MEW-21", new CardsTEU3.EvolutionaryAdvantageMon());
            r.RegisterByKey("MEW-22", new FearowMEW());
            r.RegisterByKey("MEW-23", new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray"));
            r.RegisterByKey("MEW-24", new ComboMon(new NoRetreatMon("Bind Down"), new MenacingFangsMon()));
            r.RegisterByKey("MEW-25", new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1));
            r.RegisterByKey("MEW-27", new SandshrewMEW());
            r.RegisterByKey("MEW-28", new ComboMon(new NoRetreatMon("Rumble"), new CardsPFL.DamagedBonusMon("Spike Rend", 100)));
            r.RegisterByKey("MEW-29", new ConditionAttackMon("Poison Horn", SpecialCondition.Poisoned));
            r.RegisterByKey("MEW-30", new SearchAttackMon("Fetch Family", d => d.IsPokemon, 3));
            r.RegisterByKey("MEW-31", new CardsCRI.BasicShieldAttackMon("Queen Press"));
            r.RegisterByKey("MEW-34", new NidokingMEW());
            r.RegisterByKey("MEW-35", new ClefairyMEW());
            r.RegisterByKey("MEW-36", new ComboMon(new CardsPOR.FollowMeMon("Follow Me"), new MoreMoonMon("More Moon")));
            r.RegisterByKey("MEW-37", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("MEW-38", new ComboMon(new ConditionAttackMon("Heat Wave", SpecialCondition.Burned), new MirroredFlamesMon()));
            r.RegisterByKey("MEW-41", new ZubatMEW());
            r.RegisterByKey("MEW-42", new CardsMEG.SnipeMon("Skill Dive", 40));
            r.RegisterByKey("MEW-46", new ConditionAttackMon("Spore Ball", SpecialCondition.Asleep));
            r.RegisterByKey("MEW-47", new ParasectMEW());
            r.RegisterByKey("MEW-49", new VenomothMEW());
            r.RegisterByKey("MEW-52", new MeowthMEW());
            r.RegisterByKey("MEW-53", new PersianMEW());
            r.RegisterByKey("MEW-54", new PsyduckMEW());
            r.RegisterByKey("MEW-55", new GolduckMEW());
            r.RegisterByKey("MEW-56", new CardsASR1.ThrashMon(20, 20));
            r.RegisterByKey("MEW-57", new PrimeapeMEW());
            r.RegisterByKey("MEW-58", new GrowlitheMEW());
            r.RegisterByKey("MEW-59", new ComboMon(new TorridTorrentMon(), new CardsASC.SelfDiscardTypeMon("Dynamite Fang", EnergyType.Fire, 2)));
            r.RegisterByKey("MEW-60", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("MEW-61", new FlipBonusMon("Frog Hop", 60));
            r.RegisterByKey("MEW-62", new ComboMon(new ParalyzeFlipMon("Bubble Beam"), new FlipBonusMon("Heroic Punch", 150)));
            r.RegisterByKey("MEW-66", new CardsSSP2.StompOffMon("Mountain Mashing", 1));
            r.RegisterByKey("MEW-67", new CardsSSP2.StompOffMon("Mountain Ramming", 1));
            r.RegisterByKey("MEW-68", new MachampMEW());
            r.RegisterByKey("MEW-69", new NoRetreatMon("Bind Down"));
            r.RegisterByKey("MEW-71", new VictreebelMEW());
            r.RegisterByKey("MEW-73", new ComboMon(new ConditionAttackMon("Poisonous Whip", SpecialCondition.Poisoned), new TentacularPanicMon()));
            r.RegisterByKey("MEW-74", new CardsASC.SelfGuardMon("Stiffen", 30));
            r.RegisterByKey("MEW-75", new CardsPFL.FlipUntilTailsTimesMon("Rock Cannon"));
            r.RegisterByKey("MEW-76", new ComboMon(new DynamicRollMon(), new CardsFLF1.NoResistanceMon("Rock Blaster")));
            r.RegisterByKey("MEW-77", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("MEW-78", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new SwitchSelfMon("Mach Turn", false)));
            r.RegisterByKey("MEW-79", new SlowpokeMEW());
            r.RegisterByKey("MEW-80", new SlowbroMEW());
            r.RegisterByKey("MEW-81", new RecoilMon("Big Explosion", 60));
            r.RegisterByKey("MEW-82", new MagnetonMEW());
            r.RegisterByKey("MEW-83", new DrawAttackMon("Package Deal", 2));
            r.RegisterByKey("MEW-87", new CardsPGO.SplitBombMon("Dual Splash"));
            r.RegisterByKey("MEW-88", new GrimerMEW());
            r.RegisterByKey("MEW-89", new MukMEW());
            r.RegisterByKey("MEW-90", new CardsASC.SelfGuardMon("Shell Press", 30));
            r.RegisterByKey("MEW-91", new CardsASC.SelfGuardMon("Protect Charge", 80));
            r.RegisterByKey("MEW-93", new HaunterMEW());
            r.RegisterByKey("MEW-94", new GengarMEW());
            r.RegisterByKey("MEW-95", new OnixMEW());
            r.RegisterByKey("MEW-97", new HypnoMEW());
            r.RegisterByKey("MEW-98", new KrabbyMEW());
            r.RegisterByKey("MEW-99", new CardsSSP2.StompOffMon("Hammer Arm", 1));
        }

        // ================================================================== Shared helpers (also used by CardsMEW2)

        /// <summary>Withdraw / Defensive Posture: "Flip a coin. If heads, during your opponent's next turn, prevent all damage done to this Pokémon by attacks."</summary>
        public sealed class WithdrawMon : Mon
        {
            private readonly string attack;

            public WithdrawMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage from attacks during the opponent's next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>"If your opponent's Active Pokémon is a [T] Pokémon, this attack does N more damage."</summary>
        public sealed class DefendingTypeBonusMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int bonus;

            public DefendingTypeBonusMon(string attack, EnergyType type, int bonus)
            {
                this.attack = attack;
                this.type = type;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool match = ctx.Defending != null && ctx.Engine.IsType(ctx.Defending, type);
                ctx.DamageDefending(atk.Damage + (match ? bonus : 0));
                return true;
            }
        }

        /// <summary>"This attack does N more damage for each damage counter on your opponent's Active Pokémon."</summary>
        public sealed class CounterBonusMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public CounterBonusMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int counters = ctx.Defending == null ? 0 : ctx.Defending.DamageCounters;
                ctx.DamageDefending(atk.Damage + per * counters);
                return true;
            }
        }

        /// <summary>"This attack also does N damage to each of your opponent's Benched Pokémon" (Spinning Fumes, Blizzard).</summary>
        public sealed class SpreadBenchMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SpreadBenchMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var b in ctx.OpponentBench()) ctx.DamageTarget(b, amount);
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, the Defending Pokémon's Retreat Cost is [C] more" (Gummy Press, Sticky Jail).</summary>
        public sealed class RetreatUpEffect : ContinuousEffect
        {
            public RetreatUpEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost)
            {
                return mon.Uid == TargetUid ? cost + 1 : cost;
            }
        }

        /// <summary>Psyduck's Overthink: during the opponent's next turn, every coin they flip counts as tails.</summary>
        public sealed class OverthinkEffect : ContinuousEffect
        {
            public OverthinkEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool ForcesTails(GameEngine engine, int player)
            {
                return player != Owner && engine.State.CurrentPlayer == player;
            }
        }

        // ================================================================== Grass

        /// <summary>
        /// Bye-Bye Flight (and Noctowl's Carry Off, v66): shuffle 1 of the opponent's Benched Pokémon and all attached cards into
        /// their deck, then this Pokémon and all attached cards into yours. With no Benched Pokémon the attack does nothing; if the
        /// chosen Pokémon is protected, this Pokémon is still shuffled into your deck (your ruling, v38).
        /// </summary>
        internal sealed class ByeByeFlightMon : Mon
        {
            private readonly string attack;

            public ByeByeFlightMon() : this("Bye-Bye Flight") { }

            public ByeByeFlightMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return Nothing(ctx);
                var target = ctx.ChoosePokemon(bench, attack + ": shuffle which of your opponent's Benched Pokémon into their deck?");
                if (target != null)
                {
                    if (ctx.CanAffect(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                    else ctx.Log("The effect of " + attack + " on " + target.Name + " was prevented.");
                }
                // Your ruling (v38): this Pokémon is shuffled into your deck even if their Pokémon was protected.
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Nadir Needle: with no cards in your hand, 120 more damage, and the opponent's Active Pokémon is Paralyzed and Poisoned.</summary>
        private sealed class BeedrillMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nadir Needle") return false;
                bool empty = ctx.Me.Hand.Count == 0;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (empty ? 120 : 0));
                if (empty && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Beak Catch: put up to 3 cards from your deck into your hand (at least 1 if you can, as with every "any card" search).</summary>
        private sealed class FearowMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Beak Catch") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 3, "Beak Catch: put up to 3 cards from your deck into your hand", false, 1);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Menacing Fangs: your opponent discards 2 cards from their hand (they choose).</summary>
        private sealed class MenacingFangsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Menacing Fangs") return false;
                CardsBLK.TightenUpMon.TightenUp(ctx, atk, 2);
                return true;
            }
        }

        /// <summary>Enthusiastic King: with Nidoqueen in play, ignore all Energy in the costs of this Pokémon's attacks. Venomous Impact: Poisoned.</summary>
        private sealed class NidokingMEW : Mon
        {
            private static readonly Mon Venom = new ConditionAttackMon("Venomous Impact", SpecialCondition.Poisoned);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (src.MyPokemon(m => m.Name == "Nidoqueen").Count == 0) return;
                cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Venom.HandleAttack(ctx, atk); }
        }

        /// <summary>Sand Screen: Trainer cards in the opponent's discard pile can't be put into their deck by their Item or Supporter cards.</summary>
        private sealed class SandshrewMEW : Mon
        {
            public override bool BlocksTrainerDiscardToDeck(EffectContext src, int player)
            {
                return src.Pokemon != null && player != src.Player;
            }
        }

        /// <summary>Revealing Echo: once during your turn, if this Pokémon is in the Active Spot, your opponent reveals their hand.</summary>
        private sealed class ZubatMEW : Mon
        {
            protected override string Activated { get { return "Revealing Echo"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive; }
            protected override void Ability(EffectContext ctx) { TefPokemon.RevealOpponentHand(ctx); }
        }

        // ================================================================== Psychic

        /// <summary>Moon-Viewing Invitation: put up to 3 Clefairy from your deck onto your Bench.</summary>
        private sealed class ClefairyMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moon-Viewing Invitation") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.Name == "Clefairy", 3, "Moon-Viewing Invitation: put up to 3 Clefairy onto your Bench");
                return true;
            }
        }

        /// <summary>More Moon: take 1 more Prize card if this attack's damage Knocks Out the opponent's Pokémon. Used for More Moon, Amp You Very Much (the attack name is passed in).</summary>
        internal sealed class MoreMoonMon : Mon
        {
            private readonly string attack;

            public MoreMoonMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int owner = ctx.Player;
                ctx.AddEffect(new ExtraPrizeEffect(attack + ": take 1 more Prize card for a Knock Out by this attack", owner, ctx.Turn,
                    (e, ko) => ko.HitByAttack && ko.HitByPlayer == owner && !ko.ForcedKnockOut));
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Mirrored Flames: 140 more damage if you and your opponent have the same number of cards in your hands.</summary>
        private sealed class MirroredFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirrored Flames") return false;
                bool same = ctx.Me.Hand.Count == ctx.Opp.Hand.Count;
                ctx.DamageDefending(atk.Damage + (same ? 140 : 0));
                return true;
            }
        }

        // ================================================================== Grass (Paras line, Venomoth)

        /// <summary>
        /// Spread Filaments: flip 2 coins and put up to that many Grass Pokémon from your deck onto your Bench. Any Grass Pokémon,
        /// Evolution Pokémon too: they go onto the Bench on their own (your ruling, v38).
        /// </summary>
        private sealed class ParasectMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spread Filaments") return false;
                int heads = ctx.FlipCoins(2);
                int n = Math.Min(heads, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.PokemonType == EnergyType.Grass, n,
                    "Spread Filaments: put up to " + n + " Grass Pokémon onto your Bench");
                foreach (var c in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    else ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Perplexing Powder: Confused, and the opponent can't play Item cards from their hand during their next turn.</summary>
        private sealed class VenomothMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Perplexing Powder") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during their next turn.");
                return true;
            }
        }

        // ================================================================== Colorless (Meowth line)

        /// <summary>Come Here Right Meow: flip a coin; on heads, switch in 1 of the opponent's Benched Pokémon.</summary>
        private sealed class MeowthMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Come Here Right Meow") return false;
                if (ctx.Flip() && ctx.GustTargets().Count > 0) ctx.GustOpponent("Come Here Right Meow: switch in which of your opponent's Benched Pokémon?");
                return true;
            }
        }

        /// <summary>Rocket Call: once during your turn, search your deck for a Giovanni's Charisma and put it into your hand.</summary>
        private sealed class PersianMEW : Mon
        {
            protected override string Activated { get { return "Rocket Call"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.Name == "Giovanni's Charisma", 1, "Rocket Call: choose a Giovanni's Charisma card");
            }
        }

        // ================================================================== Water (Psyduck line)

        /// <summary>Overthink: during your opponent's next turn, whenever they flip a coin, treat it as tails (Will doesn't change that).</summary>
        private sealed class PsyduckMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overthink") return false;
                ctx.AddEffect(new OverthinkEffect(ctx.Engine.PlayerName(ctx.Opponent) + "'s coin flips are all tails during their next turn (Overthink)",
                    ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Aquatic Rescue: put up to 4 Pokémon from your discard pile into your hand.</summary>
        private sealed class GolduckMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aquatic Rescue") return false;
                var mons = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(mons, 1, Math.Min(4, mons.Count), "Aquatic Rescue: put up to 4 Pokémon from your discard pile into your hand"));
                return true;
            }
        }

        // ================================================================== Fighting (Mankey line)

        /// <summary>Rant and Rave: this Pokémon is now Confused. Raging Smash: does nothing unless this Pokémon is Confused.</summary>
        private sealed class PrimeapeMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Rant and Rave")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Raging Smash")
                {
                    if (!ctx.Pokemon.Has(SpecialCondition.Confused)) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire (Growlithe line)

        /// <summary>Vaporize: discard a Water Energy from the opponent's Active Pokémon.</summary>
        private sealed class GrowlitheMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vaporize") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Vaporize: discard a Water Energy from " + defending.Name,
                        e => CardsDRI1.ProvidesType(ctx, defending, e, EnergyType.Water));
                return true;
            }
        }

        /// <summary>Torrid Torrent: attach up to 2 Basic Fire Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class TorridTorrentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Torrid Torrent") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var fire = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return true;
                var picks = ctx.ChooseCards(fire, 1, Math.Min(2, fire.Count), "Torrid Torrent: attach up to 2 Basic Fire Energy cards to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        // ================================================================== Fighting (Machop line)

        /// <summary>Guts: if this Pokémon would be Knocked Out by damage from an attack, flip a coin; heads, it's left with 10 HP. Mountain Chopping: discard the top 2 cards of the opponent's deck.</summary>
        private sealed class MachampMEW : Mon
        {
            private static readonly Mon Chopping = new CardsSSP2.StompOffMon("Mountain Chopping", 2);

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                return mon == src.Pokemon && src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Chopping.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass (Bellsprout line)

        /// <summary>
        /// Slow-Acting Acid: at the end of the opponent's next turn, 12 damage counters on the Defending Pokémon. It ends if that
        /// Pokémon leaves the Active Spot, evolves or leaves play first (your ruling, v38).
        /// </summary>
        private sealed class VictreebelMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slow-Acting Acid") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " gets 12 damage counters at the end of its next turn (Slow-Acting Acid)",
                        ctx.Player, defending.Uid, ctx.Turn, "Slow-Acting Acid", 12));
                return true;
            }
        }

        // ================================================================== Water (Tentacool line)

        /// <summary>Tentacular Panic: flip until tails, 90 damage for each heads; if the first flip is tails, the opponent's Active Pokémon is Confused.</summary>
        private sealed class TentacularPanicMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tentacular Panic") return false;
                int heads = ctx.FlipUntilTails();
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage * heads);
                if (heads == 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Fighting (Geodude line)

        /// <summary>Dynamic Roll: during your next turn, this Pokémon's attacks do 120 more damage to the opponent's Active Pokémon.</summary>
        private sealed class DynamicRollMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dynamic Roll") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                {
                    var boost = new CardsCRI.NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do 120 more damage during its next turn (Dynamic Roll)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, 120);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                }
                return true;
            }
        }

        // ================================================================== Psychic (Slowpoke line)

        /// <summary>Sea Bathing: heal 30 damage from this Pokémon and it recovers from all Special Conditions.</summary>
        private sealed class SlowpokeMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sea Bathing") return false;
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Heal(me, 30);
                ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }

        /// <summary>Big Yawn: both Active Pokémon are now Asleep. Laid-Back Tackle: does nothing if this Pokémon evolved this turn.</summary>
        private sealed class SlowbroMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Big Yawn")
                {
                    var defending = ctx.Defending;
                    if (defending != null) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name == "Laid-Back Tackle")
                {
                    if (ctx.Pokemon.TurnEvolved == ctx.Turn) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Lightning (Magnemite line)

        /// <summary>Junk Magnet: put up to 2 Item cards from your discard pile into your hand.</summary>
        private sealed class MagnetonMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Junk Magnet") return false;
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(items, 1, Math.Min(2, items.Count), "Junk Magnet: put up to 2 Item cards from your discard pile into your hand"));
                return true;
            }
        }

        // ================================================================== Water (Seel line)

        // ================================================================== Darkness (Grimer line)

        /// <summary>Gummy Press: during the opponent's next turn, the Defending Pokémon's Retreat Cost is [C] more.</summary>
        private sealed class GrimerMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gummy Press") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new RetreatUpEffect(defending.Name + "'s Retreat Cost is 1 more during its next turn (Gummy Press)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Sticky Jail: during the opponent's next turn, the Defending Pokémon's attacks cost [C] more and its Retreat Cost is [C] more.</summary>
        private sealed class MukMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sticky Jail") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsPAR1.AttackCostUpEffect(defending.Name + "'s attacks cost 1 more during its next turn (Sticky Jail)", ctx.Player, defending.Uid, ctx.Turn + 1, 1));
                ctx.AddEffect(new RetreatUpEffect(defending.Name + "'s Retreat Cost is 1 more during its next turn (Sticky Jail)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Psychic (Gastly line)

        /// <summary>Spirit Return: when you play this Pokémon from your hand to evolve, you may put a Supporter from your opponent's discard pile into their hand.</summary>
        private sealed class HaunterMEW : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var supporters = ctx.Opp.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (!TriggerOnce(ctx, "Spirit Return", supporters.Count > 0,
                    "Spirit Return: put a Supporter card from your opponent's discard pile into their hand?")) return;
                var pick = ctx.ChooseCards(supporters, 1, 1, "Spirit Return: choose a Supporter card from your opponent's discard pile")[0];
                if (!ctx.Opp.Discard.Remove(pick)) return;
                ctx.Opp.Hand.Add(pick);
                ctx.Log(pick.Def.Name + " was put from " + ctx.Engine.Their(ctx.Opponent) + " discard pile into " + ctx.Engine.Their(ctx.Opponent) + " hand (Spirit Return).");
            }
        }

        /// <summary>Poltergeist: the opponent reveals their hand; 50 damage for each Trainer card there. Hollow Dive: 3 damage counters on the opponent's Bench in any way you like.</summary>
        private sealed class GengarMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Poltergeist")
                {
                    TefPokemon.RevealOpponentHand(ctx);
                    int trainers = ctx.Opp.Hand.Count(c => c.Def.IsTrainer);
                    ctx.DamageDefending(atk.Damage * trainers);
                    return true;
                }
                if (atk.Name == "Hollow Dive")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.PlaceCountersAnyWay(3, () => ctx.OpponentBench());
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fighting (Onix)

        /// <summary>Thumpalanche: discard the top 5 cards of your deck; 80 damage for each Pokémon there with a printed Retreat Cost of exactly 4.</summary>
        private sealed class OnixMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thumpalanche") return false;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                int n = discarded.Count(c => c.Def.IsPokemon && c.Def.RetreatCost == 4);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        // ================================================================== Psychic (Drowzee line)

        /// <summary>Here for Hypnosis: when you play this Pokémon from your hand to evolve, you may make the opponent's Active Pokémon Asleep.</summary>
        private sealed class HypnoMEW : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var opp = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Here for Hypnosis", opp != null, "Here for Hypnosis: make your opponent's Active Pokémon Asleep?")) return;
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Asleep);
            }
        }

        // ================================================================== Water (Krabby line, Wartortle)

        /// <summary>Salt Water: flip a coin; on heads, search your deck for up to 2 Basic Water Energy and attach them to this Pokémon.</summary>
        private sealed class KrabbyMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Salt Water") return false;
                if (ctx.Flip()) CardsMEG.ChargeSelfMon.ChargeSelf(ctx, EnergyType.Water, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Free Diving: put up to 3 Basic Water Energy cards from your discard pile into your hand.</summary>
        private sealed class WartortleMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Free Diving") return false;
                var water = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(water, 1, Math.Min(3, water.Count), "Free Diving: put up to 3 Basic Water Energy cards into your hand"));
                return true;
            }
        }
    }
}
