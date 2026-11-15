using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Power Keepers (PK, 2007; v171): the cards not already in the pool (the others are prints). Unlimited only. Pokémon here
    /// (1-70); Trainers, Pokémon-ex and Pokémon ★ in CardsPKEx.cs. Machamp 11, Chinchou 49 and Steven's Advice 83 are their own cards
    /// (they differ from the older printings). The Elite Four Stadiums (Drake's, Glacia's, Phoebe's, Sidney's) switch on the
    /// Synergy Effect Poké-Bodies and some attacks. Compendium rulings used: Cursed Eyes moves 3 counters (all of them if fewer);
    /// Psychic Protector applies after Weakness and Resistance and can discard more cards than needed; Skill Hack must copy an
    /// attack if it can; Chilling Breath's lock stays on the opponent; Sidney's Stadium removes conditions already there;
    /// Volcanic Flame still does its damage with fewer than 5 cards in the deck; Primal Stare stops Rare Candy too.
    /// </summary>
    public static partial class CardsPK
    {
        public static void Register(CardScriptRegistry r)
        {
            // Rares (1-18)
            r.RegisterByKey("PK-1", new AggronTerraforming());
            r.RegisterByKey("PK-2", new SynergyHealMon("Drake's Stadium", new CardsSS.SurpriseMon()));
            r.RegisterByKey("PK-3", new ComboMon(new RockBlastMon(), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("PK-4", new ComboMon(new CardsASR1.PlusMon("Grudge", 10, c => c.Engine.PrizesTaken(c.Opponent)), new BenchManipulationMon()));
            r.RegisterByKey("PK-6", new ConditionAttackMon("Bursting Inferno", SpecialCondition.Burned));
            r.RegisterByKey("PK-7", new CradilyEvolutionaryCall());
            r.RegisterByKey("PK-10", new KabutopsPrimalStare());
            r.RegisterByKey("PK-11", new CardsCG.OverzealousMon(new ComboMon(new CardsEM.PiercingMon("Brick Smash"), new FlipBonusMon("Cross Chop", 30))));
            r.RegisterByKey("PK-12", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Plasma", EnergyType.Lightning), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("PK-14", new ComboMon(new Cards30CC.BenchHitMon("Dark Mind", 20), new StadiumConditionMon("Mysterious Light", "Phoebe's Stadium", SpecialCondition.Confused)));
            r.RegisterByKey("PK-17", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new PullAwayMon()));
            r.RegisterByKey("PK-18", new ComboMon(new CardsUF.PlunderMon(), new CardsFST2.ConditionBonusMon("Dark Burst", 20, c => StadiumIs(c, "Sidney's Stadium"))));

            // Uncommons (21-41)
            r.RegisterByKey("PK-21", new CardsSS.BabyEvolutionMon("Pikachu",
                new SearchAttackMon("Cry for Help", d => d.IsPokemon && d.HasType(EnergyType.Lightning) && !d.IsOldEx, 1)));
            r.RegisterByKey("PK-22", new SableyeSynergyEffect());
            r.RegisterByKey("PK-23", new PoisonCountersMon("Toxic", 2));
            r.RegisterByKey("PK-25", new NoConditionsMon(new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new FlipBonusMon("Sharp Claws", 20))));
            r.RegisterByKey("PK-26", new CardsFST1.TakeLessMon(10, new FuryCutterMon(10, 30, 50)));
            r.RegisterByKey("PK-27", new CacturnePoisonStructure());
            r.RegisterByKey("PK-28", new CardsEM.ScaryFaceMon());
            r.RegisterByKey("PK-30", new GlalieSynergyEffect());
            r.RegisterByKey("PK-31", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new DreamEaterMon()));
            r.RegisterByKey("PK-32", new ComboMon(new ShieldAttackMon("Granite Head", 10), new FlipBonusMon("Stomp", 20)));
            r.RegisterByKey("PK-33", new FlipBonusMon("Steady Punch", 20));
            r.RegisterByKey("PK-34", new MedichamVigorousAura());
            r.RegisterByKey("PK-35", new NoConditionsMon(new CardsASC.NextTurnBoostMon("Extra Comet Punch", 30)));
            r.RegisterByKey("PK-36", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("PK-37", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("PK-38", new ComboMon(new ParalyzeFlipMon("Tight Jaw"), new CardsFST2.SelfCountersMon("Darkness Charge", 1)));
            r.RegisterByKey("PK-39", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("PK-40", new DragonbreathMon());
            r.RegisterByKey("PK-41", new CardsASC.CounterattackMon("Strikes Back", 1, new FlipBonusMon("Ambush", 20)));

            // Commons (42-70)
            r.RegisterByKey("PK-42", new CardsDF.DiscardEnergyToHandMon("Dig Deep"));
            r.RegisterByKey("PK-43", new CardsASR1.TimesMon("Retaliate", CardsCG.SelfCounters));
            r.RegisterByKey("PK-44", new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("PK-45", new MagneticLinesMon());
            r.RegisterByKey("PK-46", new CardsCES1.FeintAttackMon("Feint Attack", 10));
            r.RegisterByKey("PK-47", new CardsASC.CounterattackMon("Rough Skin", 1, null));
            r.RegisterByKey("PK-48", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("PK-49", new ComboMon(new FlipBonusMon("Positive Ion", 10), new CardsMEG.WeakenDefendingMon("Negative Ion", 10)));
            r.RegisterByKey("PK-50", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PK-51", new ComboMon(new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 1), new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("PK-52", new ComboMon(new CardsSS.SurpriseMon(), new NoRetreatMon("Acid")));
            r.RegisterByKey("PK-53", new ComboMon(new TailsNothingMon("Smash Punch"), new RecoilMon("Submission", 10)));
            r.RegisterByKey("PK-54", new Cards30CC.BenchHitMon("Spark", 10));
            r.RegisterByKey("PK-55", new ComboMon(new ParalyzeFlipMon("Psyshock"), new FlipBonusMon("Cross Chop", 10)));
            r.RegisterByKey("PK-56", new InfluenceMon());
            r.RegisterByKey("PK-57", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Charge", EnergyType.Lightning), new CoinsMon("Slam", 2)));
            r.RegisterByKey("PK-58", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("PK-59", new SwitchSelfMon("Teleportation Burst", false));
            r.RegisterByKey("PK-60", new ComboMon(new CardsE3B.HealSelfMon("Nap", 20), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("PK-61", new CardsSI.HeadsTailsConditionMon("Bad Dream", SpecialCondition.Asleep, SpecialCondition.Confused));
            r.RegisterByKey("PK-62", new CardsLOR1.EntanglingStringMon("Tail Whip"));
            r.RegisterByKey("PK-63", new CardsTEU1.AmnesiaMon("Amnesia"));
            r.RegisterByKey("PK-64", new ComboMon(new CardsMEG.SnipeBenchMon("Pebble Throw", 10), new CoinsMon("Double Headbutt", 2)));
            r.RegisterByKey("PK-65", new DefenseCurlMon());
            r.RegisterByKey("PK-66", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("PK-67", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("PK-68", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PK-69", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new CardsASC.SelfDiscardTypeMon("Ember", EnergyType.Fire, 1)));
            r.RegisterByKey("PK-70", new CardsSS.BabyEvolutionMon("Wobbuffet", new CardsASR1.TimesMon("Flail", CardsCG.SelfCounters)));

            RegisterExAndTrainers(r);
        }

        // ================================================================== Shared

        internal static bool StadiumIs(EffectContext ctx, string name) { return ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == name; }

        /// <summary>"If [Stadium] is in play, the Defending Pokémon is now [condition]" after the damage (Mysterious Light).</summary>
        internal sealed class StadiumConditionMon : Mon
        {
            private readonly string attack;
            private readonly string stadium;
            private readonly SpecialCondition condition;

            public StadiumConditionMon(string attack, string stadium, SpecialCondition condition)
            {
                this.attack = attack;
                this.stadium = stadium;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool on = StadiumIs(ctx, stadium);
                ctx.DamageDefending(atk.Damage);
                if (on && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"The Defending Pokémon is now Poisoned. Put N damage counters instead of 1 on it between turns" (Toxic).</summary>
        internal sealed class PoisonCountersMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public PoisonCountersMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = counters;
                return true;
            }
        }

        /// <summary>Thick Skin / Clear Body (Poké-Body): this Pokémon can't be affected by any Special Conditions.</summary>
        internal sealed class NoConditionsMon : Mon
        {
            private readonly Mon attacks;

            public NoConditionsMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target) { return src.Pokemon != null && target == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Synergy Effect (Poké-Body): with [Stadium] in play, remove 1 damage counter from this Pokémon between turns (Altaria).</summary>
        internal sealed class SynergyHealMon : Mon
        {
            private readonly string stadium;
            private readonly Mon attacks;

            public SynergyHealMon(string stadium, Mon attacks)
            {
                this.stadium = stadium;
                this.attacks = attacks;
            }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || me.DamageCounters == 0 || !StadiumIs(src, stadium)) return;
                Announce(src, "Synergy Effect");
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Fury Cutter: flip 3 coins; 1 heads, A more; 2 heads, B more; 3 heads, C more.</summary>
        internal sealed class FuryCutterMon : Mon
        {
            private readonly int one, two, three;

            public FuryCutterMon(int one, int two, int three)
            {
                this.one = one;
                this.two = two;
                this.three = three;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage + (heads == 1 ? one : heads == 2 ? two : heads == 3 ? three : 0));
                return true;
            }
        }

        // ================================================================== Rares

        /// <summary>Terraforming (Poké-Power): once during your turn, look at the top 5 cards of your deck and put them back in any order. Not with a Special Condition. Mix-Up.</summary>
        private sealed class AggronTerraforming : Mon
        {
            private static readonly Mon MixUp = new CardsSMP4.MillOpponentMon("Mix-Up", 1);

            protected override string Activated { get { return "Terraforming"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(5);
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                CardsCRZ1.PutOnTopInOrder(ctx, top, "Terraforming");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MixUp.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rock Blast: discard up to 5 Fighting Energy cards from Armaldo; for each, 20 to an opponent's Pokémon of your choice (the
        /// same one more than once if you like), not affected by Weakness or Resistance.
        /// </summary>
        private sealed class RockBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Blast") return false;
                var me = ctx.Pokemon;
                var fighting = me.Energy.Where(e => CardsRS.BasicOf(e.Def, EnergyType.Fighting)).ToList();
                if (fighting.Count == 0) return true;
                var picks = ctx.ChooseCards(fighting, 0, Math.Min(5, fighting.Count), "Rock Blast: discard up to 5 Fighting Energy cards (20 damage for each)");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Engine.DiscardEnergy(me, card)) n++;
                for (int i = 0; i < n; i++)
                {
                    var all = ctx.OpponentPokemon();
                    if (all.Count == 0) break;
                    var target = ctx.ChoosePokemon(all, "Rock Blast: 20 damage to which of your opponent's Pokémon? (" + (i + 1) + " of " + n + ")");
                    if (target != null) ctx.DamageWith(target, 20, false, false, false);
                }
                return true;
            }
        }

        /// <summary>Bench Manipulation: your opponent flips a coin for each of their Benched Pokémon; 40 for each tails, not affected by Weakness or Resistance.</summary>
        private sealed class BenchManipulationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bench Manipulation") return false;
                int coins = ctx.Opp.Bench.Count;
                int tails = coins == 0 ? 0 : coins - ctx.Engine.FlipCoins(ctx.Opponent, coins, "Bench Manipulation");
                var defending = ctx.Defending;
                if (defending != null && tails > 0) ctx.DamageWith(defending, atk.Damage * tails, false, false, false);
                return true;
            }
        }

        /// <summary>Evolutionary Call (Poké-Power): when you play Cradily from your hand to evolve, search your deck for up to 3 Pokémon cards. Poison Ring: Poisoned and can't retreat.</summary>
        private sealed class CradilyEvolutionaryCall : Mon
        {
            private const string Power = "Evolutionary Call";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, ctx.Me.Deck.Count > 0, "Use " + Power + "? Search your deck for up to 3 Basic Pokémon and Evolution cards.")) return;
                ctx.SearchToHand(d => d.IsPokemon, 3, Power + ": choose up to 3 Basic Pokémon and Evolution cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Ring") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>
        /// Primal Stare (Poké-Body): while Kabutops is your Active Pokémon, your opponent can't evolve their Active Pokémon from their
        /// hand (Rare Candy included: Compendium). Luring Antenna: before the damage, you may switch in 1 of their Benched Pokémon.
        /// </summary>
        private sealed class KabutopsPrimalStare : Mon
        {
            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && target != null && target.Owner != src.Player && src.IsActive(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Luring Antenna") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Luring Antenna: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent("Luring Antenna: choose 1 of your opponent's Benched Pokémon to switch in");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Pull Away: if your opponent has 5 or more cards in hand, they discard cards until they have 4.</summary>
        private sealed class PullAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull Away") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count < 5) return true;
                int extra = hand.Count - 4;
                var picks = ctx.Engine.ChooseCards(ctx.Opponent, hand.ToList(), extra, extra, "Pull Away: discard " + extra + " cards from your hand", ctx.Card.Def);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        // ================================================================== Uncommons

        /// <summary>
        /// Synergy Effect (Poké-Body): with Phoebe's Stadium in play, no damage to Sableye from attacks by the opponent's Pokémon-ex.
        /// Down Draw: draw 2 cards from the bottom of your deck. Feint Attack: 20.
        /// </summary>
        private sealed class SableyeSynergyEffect : Mon
        {
            private static readonly Mon Feint = new CardsCES1.FeintAttackMon("Feint Attack", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !a.Def.IsOldEx || !StadiumIs(src, "Phoebe's Stadium")) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Down Draw") return Feint.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var deck = ctx.Me.Deck;
                for (int i = 0; i < 2 && deck.Count > 0; i++)
                {
                    var card = deck[deck.Count - 1];
                    deck.RemoveAt(deck.Count - 1);
                    ctx.Me.Hand.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew from the bottom of " + ctx.Engine.Their(ctx.Player) + " deck (Down Draw).");
                return true;
            }
        }

        /// <summary>
        /// Poison Structure (Poké-Power): once during your turn, if Sidney's Stadium is in play, the Defending Pokémon is now Poisoned.
        /// Not with a Special Condition. Pin Missile: 4 coins, 20 each.
        /// </summary>
        private sealed class CacturnePoisonStructure : Mon
        {
            private static readonly Mon Missile = new CoinsMon("Pin Missile", 4);

            protected override string Activated { get { return "Poison Structure"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && StadiumIs(ctx, "Sidney's Stadium") && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (defending != null) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Missile.HandleAttack(ctx, atk); }
        }

        /// <summary>Synergy Effect (Poké-Body): with Glacia's Stadium in play, the opponent's attacks do 30 less to Glalie (after W/R). Powder Snow. Double-edge.</summary>
        private sealed class GlalieSynergyEffect : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep), new RecoilMon("Double-edge", 10));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !StadiumIs(src, "Glacia's Stadium")) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dream Eater: does nothing unless the Defending Pokémon is Asleep.</summary>
        internal sealed class DreamEaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream Eater") return false;
                if (ctx.Defending == null || !ctx.Defending.Has(SpecialCondition.Asleep)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Vigorous Aura (Poké-Body): while Medicham is your Active Pokémon, each player's Active Pokémon's attacks do 10 more to the Active Pokémon (before W/R). Rolling Kick.</summary>
        private sealed class MedichamVigorousAura : Mon
        {
            private static readonly Mon Kick = new ParalyzeFlipMon("Rolling Kick");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || !src.IsActive(me) || a == null || info.Target == null || a.Owner == info.Target.Owner || damage <= 0) return damage;
                if (!src.IsActive(a) || !info.TargetIsActive || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Kick.HandleAttack(ctx, atk); }
        }

        /// <summary>Dragonbreath: flip a coin; tails, the attack does nothing; heads, the Defending Pokémon is now Paralyzed.</summary>
        private sealed class DragonbreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragonbreath") return false;
                var defending = ctx.Defending;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Commons

        /// <summary>Magnetic Lines: flip a coin; heads, move a basic Energy card from the Defending Pokémon to another of the opponent's Pokémon.</summary>
        private sealed class MagneticLinesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Lines") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var basics = defending.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var others = ctx.OpponentPokemon(m => m != defending);
                if (basics.Count == 0 || others.Count == 0) return true;
                var card = ctx.ChooseCards(basics, 1, 1, "Magnetic Lines: move which basic Energy card from " + defending.Name + "?")[0];
                var to = ctx.ChoosePokemon(others, "Magnetic Lines: move " + card.Def.Name + " to which of your opponent's Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>
        /// Influence: search your deck for up to 2 in any combination of Omanyte, Kabuto, Aerodactyl, Lileep and Anorith and put them
        /// onto your Bench (as Basic Pokémon).
        /// </summary>
        private sealed class InfluenceMon : Mon
        {
            private static readonly string[] Names = { "Omanyte", "Kabuto", "Aerodactyl", "Lileep", "Anorith" };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Influence") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(2, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && Names.Contains(d.Name), n, "Influence: choose up to " + n + " of Omanyte, Kabuto, Aerodactyl, Lileep and Anorith");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Defense Curl: flip a coin; heads, no damage to this Pokémon during the opponent's next turn.</summary>
        internal sealed class DefenseCurlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Defense Curl") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage during the opponent's next turn (Defense Curl)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }
    }
}
