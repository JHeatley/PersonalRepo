using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Paradox Rift (PAR) 90-266: the Fighting, Darkness, Metal, Dragon and Colorless Pokémon, the Trainers and the Special
    /// Energy that weren't already in the pool (all G-mark, Expanded only). Reprints already in the pool (Groudon, Sandy Shocks
    /// ex, Roaring Moon ex, Aegislash, Porygon2, Ancient and Future Booster Energy Capsule, Earthen Vessel, Professor Sada's
    /// Vitality, Professor Turo's Scenario, Techno Radar...) are alternate prints. Pokémon whose attacks only do their printed
    /// damage need no script: Gible, Mienfoo, Purrloin, Ferroseed, Honedge (130).
    /// </summary>
    public static class CardsPAR2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PAR-90", new DodgeMon("Hard Headbutt", false));
            r.RegisterByKey("PAR-91", new ToxicMon("Toxic"));
            r.RegisterByKey("PAR-92", new ToxicMon("Toxic Sting"));
            r.RegisterByKey("PAR-95", new CardsTWM1.SelfDiscardMon("Power Blast", 1));
            r.RegisterByKey("PAR-97", new MienshaoPAR());
            r.RegisterByKey("PAR-98", new HoopaExPAR());
            r.RegisterByKey("PAR-99", new MiniorPAR());
            r.RegisterByKey("PAR-100", new ToxtricityExPAR());
            r.RegisterByKey("PAR-101", new FlipBonusMon("Stone Edge", 20));
            r.RegisterByKey("PAR-102", new NoRetreatMon("Corner"));
            r.RegisterByKey("PAR-103", new RecoilMon("Rocky Tackle", 30));
            r.RegisterByKey("PAR-104", new GarganaclPAR());
            r.RegisterByKey("PAR-105", new KlawfPAR());
            r.RegisterByKey("PAR-106", new CardsSVI2.MaddeningScentMon("Combat Beak", 20));
            r.RegisterByKey("PAR-107", new SlitherWingPAR());
            r.RegisterByKey("PAR-109", new TingLuPAR());
            r.RegisterByKey("PAR-110", new FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("PAR-111", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("PAR-112", new CrobatPAR());
            r.RegisterByKey("PAR-113", new AbsolPAR());
            r.RegisterByKey("PAR-115", new LiepardPAR());
            r.RegisterByKey("PAR-116", new ConditionAttackMon("Venomous Hit", SpecialCondition.Poisoned));
            r.RegisterByKey("PAR-117", new GarbodorPAR());
            r.RegisterByKey("PAR-118", new YveltalPAR());
            r.RegisterByKey("PAR-119", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PAR-120", new ThievulPAR());
            r.RegisterByKey("PAR-121", new MorpekoPAR());
            r.RegisterByKey("PAR-122", new LokixPAR());
            r.RegisterByKey("PAR-123", new BruteBonnetPAR());
            r.RegisterByKey("PAR-125", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 30));
            r.RegisterByKey("PAR-126", new JirachiPAR());
            r.RegisterByKey("PAR-128", new FerrothornPAR());
            r.RegisterByKey("PAR-129", new DurantPAR());
            r.RegisterByKey("PAR-131", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("PAR-132", new LockAfterAttackMon("Slashing Strike", true));
            r.RegisterByKey("PAR-133", new DoubladeSwordsDancePAR());
            r.RegisterByKey("PAR-135", new AegislashExPAR());
            r.RegisterByKey("PAR-136", new ZacianPAR());
            r.RegisterByKey("PAR-137", new SkeledirgeExPAR());
            r.RegisterByKey("PAR-138", new OrthwormPAR());
            r.RegisterByKey("PAR-139", new GholdengoExPAR());
            r.RegisterByKey("PAR-140", new AltariaExPAR());
            r.RegisterByKey("PAR-141", new TatsugiriPAR());
            r.RegisterByKey("PAR-142", new PorygonPAR());
            r.RegisterByKey("PAR-144", new PorygonZPAR());
            r.RegisterByKey("PAR-145", new DrawAttackMon("Filch", 1));
            r.RegisterByKey("PAR-146", new AmbipomPAR());
            r.RegisterByKey("PAR-147", new CardsBLK.LivelyNeedlesMon("Lively Tackle", 90));
            r.RegisterByKey("PAR-148", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("PAR-149", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("PAR-150", new ExploudPAR());
            r.RegisterByKey("PAR-151", new SpindaPAR());
            r.RegisterByKey("PAR-152", new SwabluPAR());
            r.RegisterByKey("PAR-153", new CardsPFL.FlipUntilTailsTimesMon("Damage Rush"));
            r.RegisterByKey("PAR-154", new LockAfterAttackMon("Tumble Over", false));
            r.RegisterByKey("PAR-155", new MausholdExPAR());
            r.RegisterByKey("PAR-156", new BombirdierExPAR());
            r.RegisterByKey("PAR-157", new CardsPGO.BuddyBoltMon("Driving Buddy", 70));
            r.RegisterByKey("PAR-158", new IronJugulisPAR());

            r.RegisterByName("Counter Catcher", new CounterCatcher());
            r.RegisterByName("Cursed Duster", new CursedDuster());
            r.RegisterByName("Defiance Vest", new DefianceVest());
            r.RegisterByName("Larry", new Larry());
            r.RegisterByName("Luxurious Cape", new LuxuriousCape());
            r.RegisterByName("Mela", new Mela());
            r.RegisterByName("Norman", new Norman());
            r.RegisterByName("Parasol Lady", new ParasolLady());
            r.RegisterByName("Rika", new Rika());
            r.RegisterByName("Roark", new Roark());
            r.RegisterByName("Shauntal", new Shauntal());
            r.RegisterByName("Snorlax Doll", new SnorlaxDoll());
            r.RegisterByName("Technical Machine: Blindside", new TechnicalMachineBlindside());
            r.RegisterByName("Technical Machine: Devolution", new TechnicalMachineDevolution());
            r.RegisterByName("Technical Machine: Evolution", new TechnicalMachineEvolution());
            r.RegisterByName("Technical Machine: Turbo Energize", new TechnicalMachineTurboEnergize());
            r.RegisterByName("Tulip", new Tulip());
            r.RegisterByName("Beach Court", new BeachCourt());
            r.RegisterByName("Medical Energy", new MedicalEnergy());
            r.RegisterByName("Reversal Energy", new ReversalEnergy());
        }

        // ================================================================== Shared helpers

        /// <summary>
        /// Toxic / Toxic Sting: "Flip a coin. If heads, your opponent's Active Pokémon is now Poisoned. During Pokémon Checkup,
        /// put 2 damage counters on that Pokémon instead of 1." (after any damage).
        /// </summary>
        public sealed class ToxicMon : Mon
        {
            private readonly string attack;

            public ToxicMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned))
                    defending.PoisonCounterBase = 2;
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, your opponent's Active Pokémon is now [condition]" (after any damage).</summary>
        public sealed class FlipConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public FlipConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>The player can't play Item cards (or Supporter cards) from their hand (Crobat's Echoing Madness).</summary>
        public sealed class TrainerKindLockEffect : ContinuousEffect
        {
            private readonly int affected;
            private readonly bool supporters;

            public TrainerKindLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn, bool supporters)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
                this.supporters = supporters;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                if (player != affected) return false;
                return supporters ? card.Def.IsSupporter : card.Def.IsItem;
            }
        }

        private static bool HasToolNamed(PokemonInPlay mon, string name)
        {
            return mon != null && mon.HasToolNamed(name);
        }

        // ================================================================== Fighting

        /// <summary>Three-Step Strike: 3 coins, 20 each heads. Whip Expert: 70 more if you attached a Pokémon Tool from your hand to this Pokémon this turn.</summary>
        private sealed class MienshaoPAR : Mon
        {
            private static readonly Mon Strike = new CoinsMon("Three-Step Strike", 3);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whip Expert") return Strike.HandleAttack(ctx, atk);
                bool tooled = ctx.Pokemon.ToolFromHandOnTurn == ctx.Turn;
                ctx.DamageDefending(atk.Damage + (tooled ? 70 : 0));
                return true;
            }
        }

        /// <summary>Energy Crush: 50 for each Energy attached to all of the opponent's Pokémon. Bandit's Fist: can't use it next turn.</summary>
        private sealed class HoopaExPAR : Mon
        {
            private static readonly Mon Fist = new LockAfterAttackMon("Bandit's Fist", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Crush") return Fist.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage * ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m)));
                return true;
            }
        }

        /// <summary>
        /// Far-Flying Meteor (errata, Compendium 2024-11-07: not once per turn): during your turn, whenever you attach an Energy
        /// card from your hand to this Pokémon on your Bench, you may switch it with your Active Pokémon. Gravitational Tackle:
        /// 20 for each Colorless in the Defending Pokémon's Retreat Cost.
        /// </summary>
        private sealed class MiniorPAR : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !src.IsMyTurn || src.IsActive(me) || src.Me.Active == null || !src.Me.Bench.Contains(me)) return;
                if (!src.AskYesNo("Far-Flying Meteor: switch " + me.Name + " with your Active Pokémon?")) return;
                Announce(src, "Far-Flying Meteor");
                src.Engine.SwitchActive(src.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gravitational Tackle") return false;
                int retreat = ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending);
                ctx.DamageDefending(atk.Damage * retreat);
                return true;
            }
        }

        /// <summary>Knocking Hammer: discard the top card of the opponent's deck. Gaia Punk: discard 3 Lightning Energy from your Pokémon.</summary>
        private sealed class ToxtricityExPAR : Mon
        {
            private static readonly Mon Hammer = new CardsSSP2.StompOffMon("Knocking Hammer", 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gaia Punk") return Hammer.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 3; i++)
                {
                    var holders = ctx.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Lightning)));
                    if (holders.Count == 0) break;
                    var holder = holders.Count == 1 ? holders[0]
                        : ctx.ChoosePokemon(holders, "Gaia Punk: discard a Lightning Energy from which of your Pokémon? (" + (i + 1) + " of 3)");
                    if (holder == null) break;
                    var lightning = holder.Energy.Where(x => CardsDRI1.ProvidesType(ctx, holder, x, EnergyType.Lightning)).ToList();
                    var pick = ctx.ChooseCards(lightning, 1, 1, "Gaia Punk: discard which Energy from " + holder.Name + "?")[0];
                    if (!ctx.Engine.DiscardEnergy(holder, pick)) break;
                }
                return true;
            }
        }

        /// <summary>Energizing Rock Salt: once during your turn, attach a Basic Fighting Energy from your discard pile to 1 of your Pokémon and heal 30 from it.</summary>
        private sealed class GarganaclPAR : Mon
        {
            protected override string Activated { get { return "Energizing Rock Salt"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting));
            }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.Me.Discard.FirstOrDefault(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting));
                if (card == null) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Energizing Rock Salt: attach a Basic Fighting Energy to which Pokémon (and heal 30 from it)?");
                if (target == null || !ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.AttachEnergyCard(card, target);
                ctx.Heal(target, 30);
            }
        }

        /// <summary>Unhinged Scissors: 160 more if this Pokémon is affected by a Special Condition. Boiled Press: this Pokémon is now Burned.</summary>
        private sealed class KlawfPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Unhinged Scissors")
                {
                    ctx.DamageDefending(atk.Damage + (me.Conditions != SpecialCondition.None ? 160 : 0));
                    return true;
                }
                if (atk.Name == "Boiled Press")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.Engine.ApplyCondition(me, SpecialCondition.Burned);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Stomp Off: discard the top card of the opponent's deck. Burning Turbulence: 90 to itself; the opponent's Active Pokémon is Burned.</summary>
        private sealed class SlitherWingPAR : Mon
        {
            private static readonly Mon Stomp = new CardsSSP2.StompOffMon("Stomp Off", 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Turbulence") return Stomp.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(90);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Sand Bringer: attach up to 2 Basic Fighting Energy from your discard pile to 1 of your Pokémon. Arrogant Impact: nothing if this Pokémon has 4 or more damage counters.</summary>
        private sealed class TingLuPAR : Mon
        {
            private static readonly Mon Bringer = new CardsPAR1.BringerMon("Sand Bringer", EnergyType.Fighting, 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Arrogant Impact") return Bringer.HandleAttack(ctx, atk);
                if (ctx.Pokemon.DamageCounters >= 4) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Echoing Madness: choose Item cards or Supporter cards; the opponent can't play the chosen kind from their hand next turn.</summary>
        private sealed class CrobatPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Echoing Madness") return false;
                ctx.DamageDefending(atk.Damage);
                int pick = ctx.ChooseOption("Echoing Madness: during your opponent's next turn, they can't play which cards from their hand?", "Item cards", "Supporter cards");
                bool supporters = pick == 1;
                ctx.AddEffect(new TrainerKindLockEffect("Echoing Madness: no " + (supporters ? "Supporter" : "Item") + " cards from the hand",
                    ctx.Player, ctx.Opponent, ctx.Turn + 1, supporters));
                return true;
            }
        }

        /// <summary>
        /// Drawareness: you may discard cards until you have 4 or fewer, then draw until you have 5. With more than 4 cards the
        /// discard, if you make it, goes down to 4 or fewer; with 4 or fewer you can draw without discarding (Compendium,
        /// 2023-11-02). Enhanced Blade: 60 more with a Tool attached.
        /// </summary>
        private sealed class AbsolPAR : Mon
        {
            private static readonly Mon Blade = new CardsASC.ToolBonusMon("Enhanced Blade", 60);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drawareness") return Blade.HandleAttack(ctx, atk);
                var hand = ctx.Me.Hand.ToList();
                var picks = new List<CardInstance>();
                if (hand.Count > 4)
                {
                    if (ctx.AskYesNo("Drawareness: discard cards from your hand until you have 4 or fewer? (Then you draw until you have 5.)"))
                        picks = ctx.ChooseCards(hand, hand.Count - 4, hand.Count, "Drawareness: discard cards until you have 4 or fewer in your hand");
                }
                else if (hand.Count > 0)
                    picks = ctx.ChooseCards(hand, 0, hand.Count, "Drawareness: you may discard any number of cards from your hand (you then draw until you have 5)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DrawUntil(5);
                return true;
            }
        }

        /// <summary>Dishonest Swap: move all damage counters from 1 of your Benched Pokémon to the opponent's Active Pokémon.</summary>
        private sealed class LiepardPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dishonest Swap") return false;
                var defending = ctx.Defending;
                var donors = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (donors.Count == 0 || defending == null) return true;
                var donor = ctx.ChoosePokemon(donors, "Dishonest Swap: move all damage counters from which Benched Pokémon to " + defending.Name + "?");
                if (donor != null) ctx.Engine.MoveDamageCounters(donor, defending, donor.DamageCounters, "Dishonest Swap");
                return true;
            }
        }

        /// <summary>Chuck: discard any number of Pokémon Tool cards from your hand, 50 for each. Venomous Hit: Poisoned.</summary>
        private sealed class GarbodorPAR : Mon
        {
            private static readonly Mon Venom = new ConditionAttackMon("Venomous Hit", SpecialCondition.Poisoned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chuck") return Venom.HandleAttack(ctx, atk);
                var tools = ctx.Me.Hand.Where(c => c.Def.IsTool).ToList();
                var picks = tools.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(tools, 0, tools.Count, "Chuck: discard any number of Pokémon Tool cards from your hand (50 damage for each)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        /// <summary>Cross-Cut: 60 more against an Evolution Pokémon. Dark Edge: discard an Energy from this Pokémon.</summary>
        private sealed class YveltalPAR : Mon
        {
            private static readonly Mon Edge = new CardsTWM1.SelfDiscardMon("Dark Edge", 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross-Cut") return Edge.HandleAttack(ctx, atk);
                bool evolved = ctx.Defending != null && ctx.Defending.Def.IsEvolution;
                ctx.DamageDefending(atk.Damage + (evolved ? 60 : 0));
                return true;
            }
        }

        /// <summary>Rob-'n'-Run: when you evolve into this Pokémon from your hand, you may have the opponent reveal their hand and shuffle 2 Energy cards from it into their deck.</summary>
        private sealed class ThievulPAR : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Rob-'n'-Run", ctx.Opp.Hand.Count > 0,
                        "Rob-'n'-Run: have your opponent reveal their hand, then shuffle 2 Energy cards from it into their deck?")) return;
                TefPokemon.RevealOpponentHand(ctx);
                var energy = ctx.Opp.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return;
                var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Rob-'n'-Run: shuffle which 2 Energy cards into your opponent's deck?");
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Engine.EmitCards(GameEventType.Shuffle, ctx.Opponent, "Rob-'n'-Run shuffled " + picks.Count + " Energy " + (picks.Count == 1 ? "card" : "cards")
                    + " from " + ctx.Engine.Possessive(ctx.Opponent) + " hand into " + ctx.Engine.Their(ctx.Opponent) + " deck.", picks);
            }
        }

        /// <summary>In a Hungry Hurry: no Retreat Cost with no Energy attached. Energizer Wheel: move 2 Darkness Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class MorpekoPAR : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energizer Wheel") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var dark = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Darkness)).ToList();
                if (dark.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Energizer Wheel: move 2 Darkness Energy to which Benched Pokémon?");
                if (target == null) return true;
                var picks = dark.Count <= 2 ? dark : ctx.ChooseCards(dark, 2, 2, "Energizer Wheel: move which 2 Energy?");
                foreach (var card in picks) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Bounce: switch this Pokémon with 1 of your Benched Pokémon. Punishing Kick: 40 more for each damage counter on the opponent's Active Pokémon.</summary>
        private sealed class LokixPAR : Mon
        {
            private static readonly Mon Bounce = new SwitchSelfMon("Bounce", false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Punishing Kick") return Bounce.HandleAttack(ctx, atk);
                int counters = ctx.Defending == null ? 0 : ctx.Defending.DamageCounters;
                ctx.DamageDefending(atk.Damage + 40 * counters);
                return true;
            }
        }

        /// <summary>Toxic Powder: once during your turn, if this Pokémon has an Ancient Booster Energy Capsule attached, both Active Pokémon are Poisoned. Rampaging Hammer: can't attack next turn.</summary>
        private sealed class BruteBonnetPAR : Mon
        {
            private static readonly Mon Hammer = new LockAfterAttackMon("Rampaging Hammer", false);

            protected override string Activated { get { return "Toxic Powder"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return HasToolNamed(ctx.Pokemon, "Ancient Booster Energy Capsule") && (ctx.Me.Active != null || ctx.Opp.Active != null);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Poisoned);
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hammer.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>
        /// Stellar Veil: prevent all damage counters from being put on your Benched Pokémon by the effects of attacks used by the
        /// opponent's Basic Pokémon. Charge Energy: search for up to 2 Basic Energy cards.
        /// </summary>
        private sealed class JirachiPAR : Mon
        {
            private static readonly Mon Charge = new SearchAttackMon("Charge Energy", d => d.IsBasicEnergy, 2);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || !damageCounters || kind != EffectSourceKind.Attack || target == null) return false;
                if (target.Owner != src.Player || src.State.IsActive(target)) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.IsBasic;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Charge.HandleAttack(ctx, atk); }
        }

        /// <summary>Exoskeleton: this Pokémon takes 30 less damage from attacks (after Weakness and Resistance). Spinning Needles: 100 more next turn.</summary>
        private sealed class FerrothornPAR : Mon
        {
            private static readonly Mon Needles = new CardsASC.NextTurnBoostMon("Spinning Needles", 100);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Needles.HandleAttack(ctx, atk); }
        }

        /// <summary>Swarming Rage: 20 for each damage counter on all of your Durant. Hard Scissors: takes 20 less next turn.</summary>
        private sealed class DurantPAR : Mon
        {
            private static readonly Mon Scissors = new CardsASC.SelfGuardMon("Hard Scissors", 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swarming Rage") return Scissors.HandleAttack(ctx, atk);
                int counters = ctx.MyPokemon(m => m.Name == "Durant").Sum(m => m.DamageCounters);
                ctx.DamageDefending(atk.Damage * counters);
                return true;
            }
        }

        /// <summary>Swords Dance: during your next turn, this Pokémon's Slicing Blade does 80 more damage (before Weakness and Resistance).</summary>
        private sealed class DoubladeSwordsDancePAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Swords Dance")
                {
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s Slicing Blade does 80 more damage during its next turn (Swords Dance)", ctx.Player, me.Uid, ctx.Turn + 2, "Slicing Blade", 80);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name == "Slicing Blade")
                {
                    int uid = me.Uid;
                    int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == "Slicing Blade").Sum(b => b.Bonus);
                    ctx.DamageDefending(atk.Damage + extra);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Peerless Edge: 70 for each Prize card you have taken. Double-Edged Slash: 30 to itself.</summary>
        private sealed class AegislashExPAR : Mon
        {
            private static readonly Mon Slash = new RecoilMon("Double-Edged Slash", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peerless Edge") return Slash.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage * ctx.MyPrizesTaken);
                return true;
            }
        }

        /// <summary>Iron Roar: attach a Basic Metal Energy from your discard pile to 1 of your Benched Pokémon. Brave Blade: can't attack next turn.</summary>
        private sealed class ZacianPAR : Mon
        {
            private static readonly Mon Blade = new LockAfterAttackMon("Brave Blade", false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Roar") return Blade.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var metal = ctx.Me.Discard.FirstOrDefault(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Metal));
                if (metal == null || ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Iron Roar: attach a Basic Metal Energy from your discard pile to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(metal)) ctx.Engine.AttachEnergyCard(metal, target);
                return true;
            }
        }

        /// <summary>
        /// Incendiary Song: once during your turn, discard a Basic Fire Energy from your hand; this turn your Pokémon's attacks do
        /// 60 more damage to the opponent's Active Pokémon. Luster Burn: not affected by effects on the Defending Pokémon.
        /// </summary>
        private sealed class SkeledirgeExPAR : Mon
        {
            private static readonly Mon Burn = new CardsPAF2.IgnoreDefenderEffectsMon("Luster Burn");

            protected override string Activated { get { return "Incendiary Song"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
            }

            protected override void Ability(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.FirstOrDefault(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
                if (fire == null) return;
                ctx.Engine.DiscardFromHand(ctx.Player, fire);
                ctx.AddEffect(new DamageBonusEffect("Incendiary Song: your attacks do 60 more damage to the opponent's Active Pokémon this turn", ctx.Player, ctx.Turn, 60, false));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burn.HandleAttack(ctx, atk); }
        }

        /// <summary>Punch and Draw: draw 2 cards. Crunch-Time Rush: 150 more with 3 or fewer cards in your deck.</summary>
        private sealed class OrthwormPAR : Mon
        {
            private static readonly Mon Punch = new DrawAttackMon("Punch and Draw", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crunch-Time Rush") return Punch.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage + (ctx.Me.Deck.Count <= 3 ? 150 : 0));
                return true;
            }
        }

        /// <summary>Coin Bonus: once during your turn, draw a card (2 if this Pokémon is Active). Make It Rain: discard any number of Basic Energy from your hand, 50 each.</summary>
        private sealed class GholdengoExPAR : Mon
        {
            protected override string Activated { get { return "Coin Bonus"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.Draw(ctx.PokemonIsActive ? 2 : 1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Make It Rain") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(energy, 0, energy.Count, "Make It Rain: discard any number of Basic Energy cards from your hand (50 damage for each)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Humming Heal: once during your turn, heal 20 from each of your Pokémon. Light Pulse: prevent all effects of the opponent's attacks on this Pokémon next turn.</summary>
        private sealed class AltariaExPAR : Mon
        {
            protected override string Activated { get { return "Humming Heal"; } }
            protected override bool AbilityReady(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Light Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new CardsPAR1.AttackEffectShieldEffect(me.Name + " is protected from the effects of attacks during the opponent's next turn (Light Pulse)",
                        ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Survival Strategy: search your deck for up to 2 cards; you may switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class TatsugiriPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Survival Strategy") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 2, "Survival Strategy: put up to 2 cards from your deck into your hand", false);
                if (ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Survival Strategy: switch this Pokémon with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Data Displacement: on heads, move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        private sealed class PorygonPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Data Displacement") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Data Displacement: move which Energy from " + defending.Name + "?")[0];
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Data Displacement: move " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(defending, target, card);
                return true;
            }
        }

        /// <summary>
        /// Buggy Turbo: once during your turn, flip a coin; on heads attach up to 4 Basic Energy from your discard pile to this
        /// Pokémon, on tails discard an Energy from it. Energized Attack: 40 for each Energy attached to this Pokémon.
        /// </summary>
        private sealed class PorygonZPAR : Mon
        {
            protected override string Activated { get { return "Buggy Turbo"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy) || ctx.Pokemon.Energy.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.Engine.FlipCoin(ctx.Player, "Buggy Turbo"))
                {
                    var pile = ctx.BasicEnergyInDiscard();
                    if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, 4, (card, m) => m == me, "Buggy Turbo", true);
                }
                else if (me.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(me, 1, "Buggy Turbo: discard which Energy from " + me.Name + "?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energized Attack") return false;
                ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
                return true;
            }
        }

        /// <summary>Collect: draw 2 cards. Hand Fling: 20 for each card in your hand.</summary>
        private sealed class AmbipomPAR : Mon
        {
            private static readonly Mon Collect = new DrawAttackMon("Collect", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hand Fling") return Collect.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage * ctx.Me.Hand.Count);
                return true;
            }
        }

        /// <summary>Sudden Shout: only if this Pokémon evolved from Loudred this turn, discard the opponent's Active Pokémon and all attached cards.</summary>
        private sealed class ExploudPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sudden Shout") return false;
                var me = ctx.Pokemon;
                bool justEvolved = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Loudred";
                if (!justEvolved) return Nothing(ctx);
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                ctx.Log("Sudden Shout discards " + defending.Name + " and all attached cards.");
                ctx.Engine.DiscardPokemonFromPlay(defending);
                return true;
            }
        }

        /// <summary>Tangled Feet: while this Pokémon is Confused, flip a coin when it's damaged by an attack; on heads prevent that damage. Teetering Steps: this Pokémon is now Confused.</summary>
        private sealed class SpindaPAR : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && me.Has(SpecialCondition.Confused)
                       && src.Engine.FlipCoin(src.Player, "Tangled Feet (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teetering Steps") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Bind Wound: heal 30 damage from 1 of your Pokémon.</summary>
        private sealed class SwabluPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bind Wound") return false;
                CardsMEG.HealOneOfMine(ctx, 30, "Bind Wound");
                return true;
            }
        }

        /// <summary>
        /// Solidarity: if this Pokémon is Active and damaged by the opponent's attack (even if Knocked Out), put 3 damage counters
        /// on the Attacking Pokémon for each of your Tandemaus, Maushold and Maushold ex in play. Nom-Nom-Nom Incisors: draw 2.
        /// </summary>
        private sealed class MausholdExPAR : Mon
        {
            private static readonly Mon Incisors = new DrawAttackMon("Nom-Nom-Nom Incisors", 2);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                int family = ctx.MyPokemon(m => m.Name == "Tandemaus" || m.Name == "Maushold" || m.Name == "Maushold ex").Count;
                if (family == 0) return;
                Announce(ctx, "Solidarity");
                ctx.Engine.PlaceDamageCounters(attacker, 3 * family, "Solidarity");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Incisors.HandleAttack(ctx, atk); }
        }

        /// <summary>Fast Carrier: usable on your first turn if you go first; put up to 3 Basic Pokémon from your deck onto your Bench. Shadowy Wind: you may put this Pokémon and everything attached into your hand.</summary>
        private sealed class BombirdierExPAR : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Fast Carrier"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fast Carrier")
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => true, 3, "Fast Carrier: put up to 3 Basic Pokémon onto your Bench");
                    return true;
                }
                if (atk.Name == "Shadowy Wind")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (ctx.InPlay(me) && ctx.Engine.CanPutIntoHand(me) && ctx.AskYesNo("Shadowy Wind: put " + me.Name + " and all attached cards into your hand?"))
                        ctx.Engine.ReturnPokemonToHand(me);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Homing Headbutt: 50 damage to 3 of the opponent's Pokémon that have damage counters. Baryon Beam: costs 3 Colorless
        /// with a Future Booster Energy Capsule attached.
        /// </summary>
        private sealed class IronJugulisPAR : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Baryon Beam" || !HasToolNamed(ctx.Pokemon, "Future Booster Energy Capsule")) return;
                cost.Clear();
                for (int i = 0; i < 3; i++) cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Homing Headbutt") return false;
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0) return true;
                int n = Math.Min(3, targets.Count);
                var picks = ctx.ChoosePokemonMany(targets, n, n, "Homing Headbutt: choose " + n + " of your opponent's Pokémon with damage counters (50 damage each)");
                foreach (var mon in picks) ctx.DamageTarget(mon, 50);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Counter Catcher: only if you have more Prize cards remaining than the opponent; switch in 1 of their Benched Pokémon.</summary>
        private sealed class CounterCatcher : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count && ctx.Opp.Bench.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.GustOpponent("Counter Catcher: switch in which of your opponent's Benched Pokémon?");
            }
        }

        /// <summary>Cursed Duster: if the holder is Knocked Out by damage from the opponent's attack, discard a random card from their hand.</summary>
        private sealed class CursedDuster : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Opp.Hand.Count == 0) return;
                src.Log("Cursed Duster: a random card from the other player's hand is discarded.");
                CardsASC.DiscardRandomFromOpponentHand(src, 1);
            }
        }

        /// <summary>Defiance Vest: with more Prize cards remaining than the opponent, the holder takes 40 less damage from their attacks.</summary>
        private sealed class DefianceVest : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.Me.Prizes.Count > src.Opp.Prizes.Count ? damage - 40 : damage;
            }
        }

        /// <summary>Larry: flip a coin; heads, search for up to 2 Pokémon; tails, search for a Basic Pokémon.</summary>
        private sealed class Larry : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.SearchToHand(d => d.IsPokemon, 2, "Larry: search your deck for up to 2 Pokémon");
                else ctx.SearchToHand(d => d.IsBasicPokemon, 1, "Larry: search your deck for a Basic Pokémon");
            }
        }

        /// <summary>Luxurious Cape: a holder without a Rule Box gets +100 HP, and gives 1 more Prize card when Knocked Out by damage from the opponent's attack.</summary>
        private sealed class LuxuriousCape : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && !mon.Def.HasRuleBox ? hp + 100 : hp;
            }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var holder = src.Pokemon;
                if (holder == null || knockedOut != holder || holder.Def.HasRuleBox || taker == src.Player) return prizes;
                bool byTheirAttack = knockedOut.HitByAttack && !knockedOut.ForcedKnockOut && knockedOut.HitByPlayer == taker;
                return byTheirAttack ? prizes + 1 : prizes;
            }
        }

        /// <summary>Mela: only if any of your Pokémon were Knocked Out last turn; attach a Basic Fire Energy from your discard pile to 1 of your Pokémon, then draw until you have 6.</summary>
        private sealed class Mela : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.WasKnockedOutLastOpponentTurn() && ctx.Me.Discard.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)) && ctx.Me.HasPokemonInPlay;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var fire = ctx.Me.Discard.FirstOrDefault(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
                if (fire == null) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Mela: attach a Basic Fire Energy from your discard pile to which Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(fire)) return;
                ctx.Engine.AttachEnergyCard(fire, target);
                ctx.DrawUntil(6);
            }
        }

        /// <summary>Norman: draw 2 cards, 2 more if the opponent's Active Pokémon is a Pokémon ex.</summary>
        private sealed class Norman : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Opp.Active;
                bool ex = active != null && (active.Def.IsEx || active.Def.IsMegaEx);
                ctx.Draw(ex ? 4 : 2);
            }
        }

        /// <summary>Parasol Lady: shuffle your hand into your deck and draw 4 (8 on your first turn if you went second).</summary>
        private sealed class ParasolLady : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                bool secondsFirstTurn = ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn;
                ctx.Draw(secondsFirstTurn ? 8 : 4);
            }
        }

        /// <summary>Rika: look at the top 4 cards, put 2 into your hand, shuffle the others and put them on the bottom of your deck.</summary>
        private sealed class Rika : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return;
                var picks = top.Count <= 2 ? top.ToList() : ctx.ChooseCards(top, 2, 2, "Rika: put which 2 cards into your hand?");
                ctx.DeckToHand(picks);
                var rest = top.Where(c => !picks.Contains(c)).ToList();
                if (rest.Count == 0) return;
                foreach (var card in rest) ctx.Me.Deck.Remove(card);
                ctx.State.Rng.Shuffle(rest);
                ctx.Me.Deck.AddRange(rest);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + rest.Count + (rest.Count == 1 ? " card" : " cards") + " on the bottom of " + ctx.Engine.Their(ctx.Player) + " deck (Rika).");
            }
        }

        /// <summary>Roark: draw 2 cards, then put a Basic Energy card from your discard pile into your hand.</summary>
        private sealed class Roark : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(energy, 1, 1, "Roark: put which Basic Energy card from your discard pile into your hand?"));
            }
        }

        /// <summary>Shauntal: flip a coin; heads, switch in 1 of the opponent's Benched Pokémon; tails, switch your Active Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class Shauntal : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Count > 0 || ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.GustOpponent("Shauntal: switch in which of your opponent's Benched Pokémon?");
                else ctx.SwitchMyActive("Shauntal: switch your Active Pokémon with which Benched Pokémon?");
            }
        }

        /// <summary>
        /// Snorlax Doll: its rules are in the engine (CardDef.IsDoll with the Antique Fossil rules): put face down as a 120-HP
        /// Basic Colorless Pokémon only when setting up, can be discarded during your turn, can't retreat or be affected by Special
        /// Conditions, and gives no Prize cards.
        /// </summary>
        private sealed class SnorlaxDoll : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return false; }
        }

        /// <summary>Technical Machines: the holder can use the attack on the card; discarded at the end of your turn.</summary>
        private abstract class TechnicalMachinePAR : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            /// <summary>Discarded at the end of your turn by the engine, even while Tools have no effect (your ruling, v97).</summary>
            public override bool DiscardedAtEndOfOwnersTurn { get { return true; } }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                Use(ctx, ctx.Attack(attackIndex));
            }

            protected abstract void Use(EffectContext ctx, AttackDef atk);
        }

        /// <summary>Blindside: 100 damage to 1 of the opponent's Pokémon that has damage counters on it.</summary>
        private sealed class TechnicalMachineBlindside : TechnicalMachinePAR
        {
            protected override void Use(EffectContext ctx, AttackDef atk)
            {
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Blindside: do 100 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, 100);
            }
        }

        /// <summary>Devolution: devolve each of the opponent's evolved Pokémon, putting the highest Stage card into their hand.</summary>
        private sealed class TechnicalMachineDevolution : TechnicalMachinePAR
        {
            protected override void Use(EffectContext ctx, AttackDef atk)
            {
                foreach (var mon in ctx.OpponentPokemon(m => m.Stack.Count > 1 && m.Def.IsEvolution))
                    ctx.Engine.Devolve(mon, 1);
            }
        }

        /// <summary>Evolution: choose up to 2 of your Benched Pokémon and evolve each with a card from your deck that evolves from it.</summary>
        private sealed class TechnicalMachineEvolution : TechnicalMachinePAR
        {
            protected override void Use(EffectContext ctx, AttackDef atk)
            {
                var bench = ctx.MyBench();
                if (bench.Count == 0 || ctx.Me.Deck.Count == 0) return;
                var picks = ctx.ChoosePokemonMany(bench, 1, Math.Min(2, bench.Count), "Evolution: choose up to 2 of your Benched Pokémon to evolve from your deck");
                foreach (var mon in picks) CardsSSP1.EvolveFromDeck(ctx, mon, "Evolution: choose a card that evolves from " + mon.Name);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Turbo Energize: search for up to 2 Basic Energy cards and attach them to your Benched Pokémon in any way you like.</summary>
        private sealed class TechnicalMachineTurboEnergize : TechnicalMachinePAR
        {
            protected override void Use(EffectContext ctx, AttackDef atk)
            {
                if (ctx.Me.Deck.Count == 0 || ctx.Me.Bench.Count == 0) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Turbo Energize: choose up to 2 Basic Energy cards");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, m) => !ctx.IsActive(m), "Turbo Energize");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Tulip: put up to 4 in any combination of Psychic Pokémon and Basic Psychic Energy from your discard pile into your hand.</summary>
        private sealed class Tulip : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => (c.Def.IsPokemon && c.Def.HasType(EnergyType.Psychic)) || CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, Math.Min(4, pile.Count), "Tulip: put up to 4 Psychic Pokémon and Basic Psychic Energy cards into your hand"));
            }
        }

        /// <summary>Beach Court: the Retreat Cost of each Basic Pokémon in play is 1 Colorless less.</summary>
        private sealed class BeachCourt : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.IsBasic ? cost - 1 : cost;
            }
        }

        // ================================================================== Special Energy

        /// <summary>Medical Energy: provides Colorless; when attached from your hand, heal 30 from that Pokémon.</summary>
        private sealed class MedicalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon != null && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 30);
            }
        }

        /// <summary>
        /// Reversal Energy: Colorless; with more Prize cards remaining than the opponent, on an Evolution Pokémon without a Rule
        /// Box it provides every type of Energy, 3 Energy at a time.
        /// </summary>
        private sealed class ReversalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var holder = ctx.Pokemon;
                bool on = holder != null && holder.Def.IsEvolution && !holder.Def.HasRuleBox && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count;
                return on ? RecifeEnergy.Units(EnergyMask.AnyType, 3) : RecifeEnergy.Units(EnergyMask.Colorless, 1);
            }
        }
    }
}
