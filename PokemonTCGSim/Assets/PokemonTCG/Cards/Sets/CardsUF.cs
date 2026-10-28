using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unseen Forces Rare Pokémon, Pokémon-ex and Pokémon ★ (UF 1-33, 101-117; v168). Unlimited only. Typhlosion 17 is the Nintendo
    /// Black Star Promo 34 and Scizor ex 108 the Classic Collection's (prints). "Stages of Evolution" works while the Pokémon is an
    /// Evolved Pokémon (evolved from its Baby Pokémon). 1-on-1: "each Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsUF
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UF-1", new AmpharosEnergyConnect());
            r.RegisterByKey("UF-2", new ComboMon(new CardsASR1.PlusMon("Reactive Poison", 30, c => ConditionCount(c.Defending)), new SpiderTrapMon()));
            r.RegisterByKey("UF-3", new ComboMon(new GreenDanceMon(), new CardsFST2.ConditionBonusMon("Full Bloom", 50, c => c.MyPokemon(m => m.Name == "Bellossom").Count >= 3)));
            r.RegisterByKey("UF-4", new CardsDR.IntimidatingFangMon(new ComboMon(new CardsDX.PullAwayMon(), new CardsTRR.TonnageMon(30))));
            r.RegisterByKey("UF-5", new ComboMon(new CardsTRR.FireDanceMon(), new MultiMon("Multi Burn", SpecialCondition.Burned)));
            r.RegisterByKey("UF-6", new ComboMon(new SpikyShellMon(), new PopMon()));
            r.RegisterByKey("UF-7", new HoundoomLonesome());
            r.RegisterByKey("UF-8", new ComboMon(new AttractCurrentMon(), new MultiMon("Multi Pulse", SpecialCondition.Confused)));
            r.RegisterByKey("UF-9", new MeganiumHealingAroma());
            r.RegisterByKey("UF-10", new OctillerySuperSuctionCups());
            r.RegisterByKey("UF-11", new PoliwrathSpiralSwirl());
            r.RegisterByKey("UF-12", new Porygon2Reset());
            r.RegisterByKey("UF-13", new CardsTRR.DualArmorMon(EnergyType.Psychic, new ParallelGainMon()));
            r.RegisterByKey("UF-14", new SlowkingItemSearch());
            r.RegisterByKey("UF-15", new ComboMon(new CardsE2.CopyMon(), new CardsDX.MinusCountersMon("Karate Chop")));
            r.RegisterByKey("UF-16", new ComboMon(new ConditionAttackMon("Dazzle Dance", SpecialCondition.Confused),
                new CardsASR1.PlusMon("Green Blast", 10, c => c.MyPokemon().Sum(m => m.Energy.Count(e => CardsHIF1.Provides(c, m, e, EnergyType.Grass))))));
            r.RegisterByKey("UF-18", new UrsaringIntimidatingRing());
            r.RegisterByKey("UF-19", new ComboMon(new AquaSupplyMon(), new MultiMon("Multi Bubble", SpecialCondition.Asleep)));
            r.RegisterByKey("UF-20", new ComboMon(new HyperHealingMon(), new CardsRS.TextDamageMon("Double Tack", 20)));
            r.RegisterByKey("UF-21", new BabyEvolutionAnyMon(new[] { "Clefairy" }, new EeeeeeekMon()));
            r.RegisterByKey("UF-22", new ElectabuzzStages());
            r.RegisterByKey("UF-23", new BabyEvolutionAnyMon(new[] { "Electabuzz" }, new CardsHL.StadiumConditionMon("Magnetic Trip", "Low Pressure System", SpecialCondition.Confused)));
            r.RegisterByKey("UF-24", new HitmonchanStages());
            r.RegisterByKey("UF-25", new HitmonleeStages());
            r.RegisterByKey("UF-26", new HitmontopStages());
            r.RegisterByKey("UF-27", new SacredFireMon());
            r.RegisterByKey("UF-28", new JynxStages());
            r.RegisterByKey("UF-29", new CardsRSB.CoinsPlusMon("Aeroblast", 2, 20));
            r.RegisterByKey("UF-30", new ComboMon(new NightSongMon(), new PlunderMon()));
            r.RegisterByKey("UF-31", new BabyEvolutionAnyMon(new[] { "Jynx" }, new CountersAnyOpponentMon("Blown Kiss", 1)));
            r.RegisterByKey("UF-32", new ComboMon(new CardsEM.EvolvedConditionMon("Screechy Voice", SpecialCondition.Confused), new PushAwayMon()));
            r.RegisterByKey("UF-33", new BabyEvolutionAnyMon(new[] { "Hitmonlee", "Hitmonchan", "Hitmontop" },
                new CardsASR1.TimesMon("Desperate Punch", c => Math.Max(0, c.OpponentPokemon().Count - c.MyPokemon().Count))));

            r.RegisterByKey("UF-101", new BlisseyExBlissfulSupport());
            r.RegisterByKey("UF-102", new EspeonExDevoFlash());
            r.RegisterByKey("UF-103", new FeraligatrExOverpoweringFang());
            r.RegisterByKey("UF-104", new HoOhExGoldenWing());
            r.RegisterByKey("UF-105", new LugiaExSilverSparkle());
            r.RegisterByKey("UF-106", new MeganiumExNurture());
            r.RegisterByKey("UF-107", new ComboMon(new UpwardLickMon(), new SwitchSelfMon("Punch and Run", false), new BigSwallowUpMon()));
            r.RegisterByKey("UF-109", new CardsDX.ConditionImmuneMon(SpecialCondition.Poisoned, new ComboMon(new MetalChargeMon(), new MudslideMon())));
            r.RegisterByKey("UF-110", new TyphlosionExBurstingUp());
            r.RegisterByKey("UF-111", new TyranitarExMon());
            r.RegisterByKey("UF-112", new UmbreonExDarkerRing());
            r.RegisterByKey("UF-113", new ComboMon(new SwitchSelfMon("Burning Turn", false), new BehindStarMon("Detonation")));
            r.RegisterByKey("UF-114", new ComboMon(new SwitchSelfMon("Lightning Turn", false), new BehindStarMon("Meta Voltage")));
            r.RegisterByKey("UF-115", new ComboMon(new SwitchSelfMon("Splashing Turn", false), new BehindStarMon("Cross Wind")));
            r.RegisterByKey("UF-116", new PersianExNightCry());
            r.RegisterByKey("UF-117", new ComboMon(new SpiralLeafMon(), new TimeTrapMon()));
        }

        // ================================================================== Shared

        private static readonly SpecialCondition[] AllConditions =
            { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

        internal static int ConditionCount(PokemonInPlay mon) { return mon == null ? 0 : AllConditions.Count(mon.Has); }

        /// <summary>"Stages of Evolution" bodies: while this Pokémon is an Evolved Pokémon.</summary>
        internal static bool Evolved(PokemonInPlay mon) { return mon != null && mon.Stack.Count > 1; }

        /// <summary>Baby Evolution (Poké-Power) with one or more possible evolutions (Tyrogue: Hitmonlee, Hitmonchan or Hitmontop).</summary>
        internal sealed class BabyEvolutionAnyMon : Mon
        {
            private readonly string[] evolutions;
            private readonly Mon attacks;

            public BabyEvolutionAnyMon(string[] evolutions, Mon attacks)
            {
                this.evolutions = evolutions;
                this.attacks = attacks;
            }

            private List<CardInstance> Cards(EffectContext ctx)
            {
                var names = evolutions;
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && names.Contains(c.Def.Name)).ToList();
            }

            protected override string Activated { get { return "Baby Evolution"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Cards(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var cards = Cards(ctx);
                if (cards.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, "Baby Evolution: put which card onto " + me.Name + "?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.EvolvePokemon(me, card, true);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Multi Burn / Pulse / Bubble: with 3 or more types of basic Energy cards attached, 20 more and a Special Condition.</summary>
        internal sealed class MultiMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public MultiMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool multi = ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count() >= 3;
                ctx.DamageDefending(atk.Damage + (multi ? 20 : 0));
                if (multi && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"Put N damage counters on 1 of your opponent's Pokémon" (Blown Kiss).</summary>
        internal sealed class CountersAnyOpponentMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public CountersAnyOpponentMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, attack + ": put " + counters + " damage counter" + (counters == 1 ? "" : "s") + " on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, counters);
                return true;
            }
        }

        /// <summary>Black Cry: the Defending Pokémon can't use Poké-Powers during the opponent's next turn.</summary>
        internal sealed class NoPokePowerEffect : ContinuousEffect
        {
            public NoPokePowerEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }
        }

        internal sealed class NoRetreatEffect : ContinuousEffect
        {
            public NoRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        // ================================================================== Rares

        /// <summary>Energy Connect (Poké-Power): as often as you like, move a basic Energy card from 1 of your Benched Pokémon to your Active Pokémon. Miraculous Thunder.</summary>
        private sealed class AmpharosEnergyConnect : Mon
        {
            private const string Power = "Energy Connect";

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.Me.Bench.Where(m => m.Energy.Any(e => e.Def.IsBasicEnergy)).ToList(); }

            private static bool Ready(EffectContext ctx) { return ctx.Me.Active != null && Donors(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && Ready(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => Ready(ctx), offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a basic Energy card from which Benched Pokémon to your Active Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var cards = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                    var card = ctx.ChooseCards(cards, 1, 1, Power + ": move which Energy card?")[0];
                    return ctx.Engine.MoveEnergy(from, ctx.Me.Active, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Thunder") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                var lightning = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList();
                bool discarded = false;
                if (!CardsBS1.CopyingWithMetronome && lightning.Count > 0 && ctx.AskYesNo("Miraculous Thunder: discard all Lightning Energy to make the Defending Pokémon Burned and Confused?"))
                {
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                    discarded = true;
                }
                ctx.DamageDefending(atk.Damage);
                if (discarded && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                }
                return true;
            }
        }

        /// <summary>Spider Trap: you may first switch in 1 of the opponent's Benched Pokémon; the Defending Pokémon is now Asleep and Poisoned.</summary>
        private sealed class SpiderTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spider Trap") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Spider Trap: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent("Spider Trap: choose the new Defending Pokémon");
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Green Dance: up to 2 Grass Pokémon from your deck into your hand; if any, you may switch Bellossom with a Benched Pokémon.</summary>
        private sealed class GreenDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Green Dance") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Grass), 2, "Green Dance: choose up to 2 Grass Pokémon");
                if (found.Count > 0 && ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Green Dance: switch Bellossom with 1 of your Benched Pokémon?"))
                    ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Spiky Shell: 3 damage counters on the Defending Pokémon at the end of the opponent's next turn.</summary>
        private sealed class SpikyShellMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiky Shell") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsTRR.DelayedCountersEffect(defending.Name + " gets 3 damage counters at the end of its next turn (Spiky Shell)", ctx.Player, defending.Uid, ctx.Turn, 3, false, "Spiky Shell"));
                return true;
            }
        }

        /// <summary>Pop: 7 damage counters on Forretress; then its Energy cards go to your Benched Pokémon in any way you like.</summary>
        private sealed class PopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pop") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                ctx.PlaceCounters(me, 7);
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                foreach (var card in me.Energy.ToList())
                {
                    var bench = ctx.Me.Bench.ToList();
                    var to = ctx.ChoosePokemon(bench, "Pop: move " + card.Def.Name + " to which Benched Pokémon?");
                    if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                }
                return true;
            }
        }

        /// <summary>Lonesome (Poké-Body): while you have fewer Pokémon in play than the opponent, they can't play Trainer cards other than Supporters. Tight Jaw. Flamethrower.</summary>
        private sealed class HoundoomLonesome : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ParalyzeFlipMon("Tight Jaw"), new CardsDR.DiscardTypesMon("Flamethrower", EnergyType.Fire));

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                if (src.Pokemon == null || player == src.Player || card == null || !card.Def.IsTrainer || card.Def.IsSupporter) return false;
                return src.MyPokemon().Count < src.OpponentPokemon().Count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Attract Current: heads, a Lightning Energy card from your deck onto 1 of your Pokémon.</summary>
        private sealed class AttractCurrentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Attract Current") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                foreach (var card in ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Lightning), 1, "Attract Current: choose a Lightning Energy card"))
                {
                    var mons = ctx.MyPokemon();
                    var to = ctx.ChoosePokemon(mons, "Attract Current: attach it to which Pokémon?");
                    if (to == null || !ctx.Engine.AttachEnergyCard(card, to)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Healing Aroma (Poké-Body): while Meganium is your Active Pokémon, 1 damage counter off each non-ex Pokémon (both players') between turns. Bouncy Move.</summary>
        private sealed class MeganiumHealingAroma : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                for (int side = 0; side < 2; side++)
                    foreach (var mon in src.State.Players[side].InPlay().ToList())
                        if (!mon.Def.IsOldEx && mon.DamageCounters > 0) src.Heal(mon, 10);
            }

            public override int CheckupAffects(EffectContext src) { return -1; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bouncy Move") return false;
                var me = ctx.Pokemon;
                int put = ctx.AskNumber(0, 5, "Bouncy Move: put how many damage counters on Meganium (10 more damage each)?");
                if (put > 0) ctx.PlaceCounters(me, put);
                ctx.DamageDefending(atk.Damage + 10 * put);
                return true;
            }
        }

        /// <summary>Super Suction Cups (Poké-Body): while Octillery is your Active Pokémon, the opponent's Pokémon can't retreat. Standing By: next turn Pulse Blast does 120.</summary>
        private sealed class OctillerySuperSuctionCups : Mon
        {
            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && mon.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Standing By")
                {
                    if (!CardsBS1.CopyingWithMetronome && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Standing By: discard an Energy card");
                    return true;
                }
                if (atk.Name != "Pulse Blast") return false;
                bool ready = me.LastAttackName == "Standing By" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(ready ? 120 : atk.Damage);
                return true;
            }
        }

        /// <summary>Spiral Swirl (Poké-Body): if Poliwrath is Active and Knocked Out by an opponent's attack, the Attacking Pokémon is Confused. Beatdown. Hyper Splash.</summary>
        private sealed class PoliwrathSpiralSwirl : Mon
        {
            private static readonly Mon Attacks = new ComboMon(
                new CardsFST2.ConditionBonusMon("Beatdown", 30, c => c.Defending != null && (c.Engine.IsType(c.Defending, EnergyType.Darkness) || CardsTRR.Dark(c.Defending.Name))),
                CardsSS.ConditionBonus("Hyper Splash", 30, d => d.Def.Stage == Stage.Stage2));

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Pokemon == null) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                Announce(src, "Spiral Swirl");
                src.ApplyCondition(attacker, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>3-D Reset (Poké-Power): as often as you like, a Pokémon Tool attached to 1 of your Pokémon goes back to your hand. Data Retrieval: draw up to 8. Scramble Trip.</summary>
        private sealed class Porygon2Reset : Mon
        {
            private const string Power = "3-D Reset";

            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => m.Tools.Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && Holders(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => Holders(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Power + ": return a Pokémon Tool from which Pokémon to your hand?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var tool = from.Tools.Count == 1 ? from.Tools[0] : ctx.ChooseCards(from.Tools.ToList(), 1, 1, Power + ": return which Tool?")[0];
                    if (!from.Tools.Remove(tool)) return false;
                    ctx.State.Players[tool.Owner].Hand.Add(tool);
                    ctx.Log(Power + ": " + tool.Def.Name + " went back to " + ctx.Engine.Possessive(tool.Owner) + " hand.");
                    return true;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Data Retrieval")
                {
                    int n = 8 - ctx.Me.Hand.Count;
                    if (n > 0 && ctx.Me.Deck.Count > 0) ctx.Draw(n);
                    return true;
                }
                if (atk.Name != "Scramble Trip") return false;
                var defending = ctx.Defending;
                bool scramble = ctx.Pokemon.Energy.Any(e => e.Def.Name == "Scramble Energy");
                ctx.DamageDefending(atk.Damage + (scramble ? 20 : 0));
                if (scramble && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Parallel Gain: 1 damage counter off each of your Pokémon.</summary>
        private sealed class ParallelGainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parallel Gain") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
                return true;
            }
        }

        /// <summary>Item Search (Poké-Power): once during your turn, a Pokémon Tool from your deck into your hand. Aftermath: 10 more per Tool in your discard pile (up to 60).</summary>
        private sealed class SlowkingItemSearch : Mon
        {
            private static readonly Mon After = new CardsASR1.PlusMon("Aftermath", 10, c => Math.Min(6, c.Me.Discard.Count(x => x.Def.IsTool)));

            protected override string Activated { get { return "Item Search"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(d => d.IsTool, 1, "Item Search: choose a Pokémon Tool card"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return After.HandleAttack(ctx, atk); }
        }

        /// <summary>Intimidating Ring (Poké-Body): while Ursaring is your Active Pokémon, the opponent's Basic Pokémon can't attack or use Poké-Powers. Drag Off. Rock Smash.</summary>
        private sealed class UrsaringIntimidatingRing : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsTRR.DragOffMayMon(), new FlipBonusMon("Rock Smash", 20));

            private static bool Covers(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && mon.Owner != src.Player && mon.Stack.Count == 1 && mon.Def.IsBasicPokemon;
            }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Aqua Supply: you may attach a basic Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class AquaSupplyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Supply") return false;
                ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (cards.Count == 0) return true;
                var pick = ctx.ChooseCards(cards, 0, 1, "Aqua Supply: attach a basic Energy card from your hand?");
                if (pick.Count == 0) return true;
                var mons = ctx.MyPokemon();
                var to = ctx.ChoosePokemon(mons, "Aqua Supply: attach it to which Pokémon?");
                if (to != null && ctx.Me.Hand.Remove(pick[0]) && !ctx.Engine.AttachEnergyCard(pick[0], to)) ctx.Me.Hand.Add(pick[0]);
                return true;
            }
        }

        /// <summary>Hyper Healing: 1 damage counter and all Special Conditions off your Active Pokémon.</summary>
        private sealed class HyperHealingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Healing") return false;
                var active = ctx.Me.Active;
                if (active == null) return true;
                if (active.DamageCounters > 0) ctx.Heal(active, 10);
                ctx.Engine.RemoveAllConditions(active);
                return true;
            }
        }

        /// <summary>Eeeeeeek: shuffle your hand into your deck, then draw 6 cards.</summary>
        private sealed class EeeeeeekMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eeeeeeek") return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(6);
                return true;
            }
        }

        /// <summary>Stages of Evolution (Electabuzz): while Evolved, attack damage from the opponent's Pokémon with Special Energy attached is 40 less (after W/R). Double Shock. Luster Blast.</summary>
        private sealed class ElectabuzzStages : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || !Evolved(me) || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.Energy.Any(e => !e.Def.IsBasicEnergy) ? damage - 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Double Shock")
                {
                    var defending = ctx.Defending;
                    int heads = ctx.FlipCoins(2);
                    ctx.DamageDefending(atk.Damage * heads);
                    if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name != "Luster Blast") return false;
                ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 3, "Luster Blast: discard 3 Energy");
                return true;
            }
        }

        /// <summary>Stages of Evolution (Hitmonchan): while Evolved, +30 HP. Heavy Punch. Speedy Uppercut.</summary>
        private sealed class HitmonchanStages : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASR1.TimesMon("Heavy Punch", c => c.Opp.Bench.Count), new CardsMA.SwiftMon("Speedy Uppercut"));

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return src.Pokemon != null && mon == src.Pokemon && Evolved(mon) ? hp + 30 : hp; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Stages of Evolution (Hitmonlee): while Evolved, its attacks do 20 more to the opponent's Pokémon (before W/R). Stretch Kick. Mega Kick.</summary>
        private sealed class HitmonleeStages : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !Evolved(me) || info.Target == null || info.Target.Owner == src.Player) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stretch Kick") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>Stages of Evolution (Hitmontop): while Evolved and Active, damage from an opponent's attack puts 2 damage counters on the Attacking Pokémon. Upward Kick. Spiral Kick.</summary>
        private sealed class HitmontopStages : Mon
        {
            private static readonly Mon Kick = CardsSS.ConditionBonus("Upward Kick", 30, d => d.DamageCounters >= 2);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || !Evolved(ctx.Pokemon) || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Stages of Evolution");
                ctx.Engine.PlaceDamageCounters(attacker, 2, "Stages of Evolution");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Kick.HandleAttack(ctx, atk); }
        }

        /// <summary>Sacred Fire: heads, 60 to 1 of the opponent's Pokémon, not affected by Weakness or Resistance. Gust.</summary>
        private sealed class SacredFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sacred Fire") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageAnyOpponent(60, true);
                return true;
            }
        }

        /// <summary>Stages of Evolution (Jynx): while Evolved, no effects (other than damage) of the opponent's attacks, and no Weakness. Freeze Light. Pure Power: 4 counters any way.</summary>
        private sealed class JynxStages : Mon
        {
            private static readonly Mon Freeze = new CardsSI.HeadsTailsConditionMon("Freeze Light", SpecialCondition.Paralyzed, SpecialCondition.Burned);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !Evolved(me) || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon && Evolved(mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pure Power") return Freeze.HandleAttack(ctx, atk);
                ctx.PlaceCountersAnyWay(4, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Night Song: switch in 1 of the opponent's Benched Pokémon; the new Defending Pokémon is now Asleep.</summary>
        private sealed class NightSongMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Song") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.GustTargets().Count == 0) return true;
                var target = ctx.GustOpponent("Night Song: choose 1 of your opponent's Benched Pokémon to switch in");
                if (target != null && ctx.Opp.Active == target) ctx.ApplyCondition(target, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Plunder: before the damage, discard all Trainer cards attached to the Defending Pokémon.</summary>
        private sealed class PlunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plunder") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending))
                {
                    ctx.Engine.DiscardAllTools(defending);
                    foreach (var card in defending.Attached.Where(c => c.Def.IsTrainer).ToList())
                        if (defending.Attached.Remove(card)) ctx.State.Players[card.Owner].Discard.Add(card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Push Away: look at the opponent's hand and discard a Trainer card from it.</summary>
        private sealed class PushAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Push Away") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Push Away: your opponent's hand", ctx.Source);
                var trainers = hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return true;
                var card = ctx.ChooseCards(trainers, 1, 1, "Push Away: discard which Trainer card?")[0];
                if (ctx.Opp.Hand.Contains(card)) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        // ================================================================== Pokémon-ex and Pokémon ★

        /// <summary>
        /// Blissful Support (Poké-Power): when you evolve into Blissey ex from your hand, you may discard all Energy from any number of
        /// your Pokémon and remove all their damage counters. Energy Absorption: up to 3 Energy cards from your discard pile onto Blissey ex.
        /// </summary>
        private sealed class BlisseyExBlissfulSupport : Mon
        {
            private const string Power = "Blissful Support";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, ctx.MyPokemon().Count > 0, Power + ": discard Energy from your Pokémon to heal them?")) return;
                var picks = ctx.ChoosePokemonMany(ctx.MyPokemon(), 0, ctx.MyPokemon().Count, Power + ": choose any number of your Pokémon (all their Energy is discarded and all damage counters removed)");
                foreach (var mon in picks)
                {
                    if (!ctx.InPlay(mon)) continue;
                    ctx.Engine.DiscardAllEnergy(mon);
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 10 * mon.DamageCounters);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Absorption") return false;
                var me = ctx.Pokemon;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                foreach (var card in ctx.ChooseCards(cards, 0, Math.Min(3, cards.Count), "Energy Absorption: attach up to 3 Energy cards to Blissey ex"))
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Devo Flash (Poké-Power): when you evolve into Espeon ex from your hand, the highest Stage card of 1 of the opponent's Benched Evolved Pokémon goes back to their hand. Snap Tail. Psyloop.</summary>
        private sealed class EspeonExDevoFlash : Mon
        {
            private const string Power = "Devo Flash";
            private static readonly Mon Attacks = new ComboMon(new CardsSSB.AnyOpponentTextMon("Snap Tail", 30), new CardsDX.PsyloopMon());

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var targets = ctx.Opp.Bench.Where(CardsE2.IsEvolved).ToList();
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, targets.Count > 0, Power + ": devolve 1 of your opponent's Benched Evolved Pokémon?")) return;
                var target = ctx.ChoosePokemon(targets, Power + ": devolve which Benched Pokémon?");
                if (target != null && ctx.CanAffect(target)) ctx.Engine.Devolve(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Overpowering Fang (Poké-Body): while Feraligatr ex is your Active Pokémon, no Pokémon other than Pokémon-ex (both players') can use Poké-Powers or Poké-Bodies. Tsunami. Sore Spot.</summary>
        private sealed class FeraligatrExOverpoweringFang : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSS.AlsoEachBenchMon("Tsunami", 10),
                new CardsASR1.PlusMon("Sore Spot", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters));

            private static bool Covers(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && !mon.Def.IsOldEx; }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName) { return bodyName != "Overpowering Fang" && Covers(src, mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Golden Wing (Poké-Power): if Ho-Oh ex would be Knocked Out by an opponent's attack, you may move up to 2 Energy from it to your other Pokémon. Rainbow Burn.</summary>
        private sealed class HoOhExGoldenWing : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null || me.Energy.Count == 0) return;
                var others = src.MyPokemon(m => m != me);
                if (others.Count == 0 || !src.AskYesNo("Golden Wing: move up to 2 Energy from Ho-Oh ex to your other Pokémon?")) return;
                Announce(src, "Golden Wing");
                foreach (var card in src.ChooseCards(me.Energy.ToList(), 0, Math.Min(2, me.Energy.Count), "Golden Wing: move which Energy?"))
                {
                    var to = others.Count == 1 ? others[0] : src.ChoosePokemon(others, "Golden Wing: move " + card.Def.Name + " to which Pokémon?");
                    if (to != null) src.Engine.MoveEnergy(me, to, card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Burn") return false;
                int types = ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count();
                ctx.DamageDefending(atk.Damage + 20 * types);
                return true;
            }
        }

        /// <summary>Silver Sparkle (Poké-Body): if Lugia ex is Active and damaged by an opponent's attack, flip; heads, an Energy card on the Attacking Pokémon goes back to the opponent's hand. Elemental Blast.</summary>
        private sealed class LugiaExSilverSparkle : Mon
        {
            private static readonly Mon Blast = new CardsDR.DiscardTypesMon("Elemental Blast", EnergyType.Fire, EnergyType.Water, EnergyType.Lightning);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || attacker.Energy.Count == 0) return;
                Announce(ctx, "Silver Sparkle");
                if (!ctx.Flip()) return;
                var card = attacker.Energy.Count == 1 ? attacker.Energy[0] : ctx.ChooseCards(attacker.Energy.ToList(), 1, 1, "Silver Sparkle: return which Energy card to your opponent's hand?")[0];
                if (!attacker.Energy.Remove(card)) return;
                ctx.State.Players[card.Owner].Hand.Add(card);
                ctx.Log("Silver Sparkle: " + card.Def.Name + " went back to " + ctx.Engine.Possessive(card.Owner) + " hand.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Nurture and Heal (Poké-Power): once during your turn, attach a Grass Energy card from your hand to 1 of your Pokémon and remove 1 damage counter from it. Power Poison.</summary>
        private sealed class MeganiumExNurture : Mon
        {
            private const string Power = "Nurture and Heal";

            private static List<CardInstance> Grass(EffectContext ctx) { return ctx.Me.Hand.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Grass)).ToList(); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && Grass(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = Grass(ctx).First();
                var mons = ctx.MyPokemon();
                var to = ctx.ChoosePokemon(mons, Power + ": attach a Grass Energy card to which Pokémon?");
                if (to == null || !ctx.Me.Hand.Remove(card)) return;
                if (!ctx.Engine.AttachEnergyCard(card, to, true))
                {
                    ctx.Me.Hand.Add(card);
                    return;
                }
                if (ctx.InPlay(to) && to.DamageCounters > 0) ctx.Heal(to, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Poison") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Power Poison: discard an Energy");
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Upward Lick: 30 to 1 of the opponent's Pokémon (50 to a Stage 2 Pokémon).</summary>
        private sealed class UpwardLickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Upward Lick") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Upward Lick: choose 1 of your opponent's Pokémon");
                if (target != null) ctx.DamageTarget(target, target.Def.Stage == Stage.Stage2 && target.Stack.Count > 1 ? 50 : 30);
                return true;
            }
        }

        /// <summary>Swallow Up (Politoed ex): 120 instead if the Defending Pokémon has less HP left than Politoed ex.</summary>
        private sealed class BigSwallowUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swallow Up") return false;
                var d = ctx.Defending;
                bool smaller = d != null && ctx.RemainingHp(d) < ctx.RemainingHp(ctx.Pokemon);
                ctx.DamageDefending(smaller ? 120 : atk.Damage);
                return true;
            }
        }

        /// <summary>Metal Charge: 1 damage counter on Steelix ex.</summary>
        private sealed class MetalChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Charge") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, 1);
                return true;
            }
        }

        /// <summary>Mudslide: discard 2 Fighting Energy from Steelix ex, then 100 to 1 of the opponent's Pokémon.</summary>
        private sealed class MudslideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mudslide") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome) ctx.DiscardEnergyFrom(me, 2, "Mudslide: discard 2 Fighting Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fighting));
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 100));
                return true;
            }
        }

        /// <summary>Bursting Up (Poké-Power): when you evolve into Typhlosion ex from your hand, up to 1 Fire Energy per opponent's Benched Pokémon from your deck onto 1 of your Fire Pokémon. Kindle.</summary>
        private sealed class TyphlosionExBurstingUp : Mon
        {
            private const string Power = "Bursting Up";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                int n = ctx.Opp.Bench.Count;
                var fire = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fire));
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, n > 0 && fire.Count > 0 && ctx.Me.Deck.Count > 0, Power + ": attach Fire Energy from your deck to 1 of your Fire Pokémon?")) return;
                var to = ctx.ChoosePokemon(fire, Power + ": attach Fire Energy to which Fire Pokémon?");
                if (to == null) return;
                foreach (var card in ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Fire), n, Power + ": choose up to " + n + " Fire Energy cards"))
                    if (!ctx.Engine.AttachEnergyCard(card, to)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kindle") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Kindle: discard an Energy card from " + me.Name);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Kindle: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Tyranitar ex: Shatter (discard any Stadium), Derail (a Special Energy from the Defending Pokémon), Mix-Up (the opponent's top card), Losing Control (your top 3 cards).</summary>
        private sealed class TyranitarExMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                switch (atk.Name)
                {
                    case "Shatter":
                        ctx.DamageDefending(atk.Damage);
                        if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                        return true;
                    case "Derail":
                        ctx.DamageDefending(atk.Damage);
                        if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => !e.Def.IsBasicEnergy))
                            ctx.DiscardEnergyFrom(defending, 1, "Derail: discard a Special Energy card from " + defending.Name, e => !e.Def.IsBasicEnergy);
                        return true;
                    case "Mix-Up":
                        ctx.DamageDefending(atk.Damage);
                        ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                        return true;
                    case "Losing Control":
                        ctx.DamageDefending(atk.Damage);
                        ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                        return true;
                }
                return false;
            }
        }

        /// <summary>Darker Ring (Poké-Power): when you evolve into Umbreon ex from your hand, switch in 1 of the opponent's Benched Pokémon. Black Cry: no retreating or Poké-Powers for it next turn. Darkness Fang.</summary>
        private sealed class UmbreonExDarkerRing : Mon
        {
            private const string Power = "Darker Ring";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, ctx.GustTargets().Count > 0, Power + ": switch 1 of your opponent's Benched Pokémon with the Defending Pokémon?")) return;
                ctx.GustOpponent(Power + ": choose the new Defending Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Black Cry") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new NoRetreatEffect(defending.Name + " can't retreat during its next turn (Black Cry)", ctx.Player, defending.Uid, ctx.Turn + 1));
                ctx.AddEffect(new NoPokePowerEffect(defending.Name + " can't use Poké-Powers during its next turn (Black Cry)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Detonation / Meta Voltage / Cross Wind (the Pokémon ★): the damage, then a drawback if you have fewer Prize cards left than the opponent.</summary>
        private sealed class BehindStarMon : Mon
        {
            private readonly string attack;

            public BehindStarMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Prizes.Count >= ctx.Opp.Prizes.Count) return true;
                switch (attack)
                {
                    case "Detonation":
                        ctx.Engine.DiscardTopOfDeck(ctx.Player, 10);
                        break;
                    case "Meta Voltage":
                        foreach (var mon in ctx.MyPokemon())
                            if (ctx.InPlay(mon)) ctx.DamageWith(mon, 40, false, false, false);
                        break;
                    case "Cross Wind":
                        foreach (var mon in ctx.MyPokemon()) ctx.Engine.DiscardAllEnergy(mon);
                        break;
                }
                return true;
            }
        }

        /// <summary>Night Cry (Poké-Power): once during your turn, if Rocket's Persian ex is on your Bench, a Pokémon with Dark or Rocket's in its name from your deck into your hand. Toxic Claws.</summary>
        private sealed class PersianExNightCry : Mon
        {
            private static readonly Mon Claws = new CardsBS1.ToxicMon("Toxic Claws", 2);

            protected override string Activated { get { return "Night Cry"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && CardsTRR.DarkOrRocket(d.Name), 1, "Night Cry: choose a Pokémon with Dark or Rocket's in its name");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiral Leaf: heads, 1 damage counter on each of the opponent's Pokémon; tails, 1 damage counter off each of yours.</summary>
        private sealed class SpiralLeafMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Leaf") return false;
                if (ctx.Flip())
                {
                    foreach (var mon in ctx.OpponentPokemon())
                        if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 1);
                }
                else
                {
                    foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
                }
                return true;
            }
        }

        /// <summary>Time Trap: heads, reorder the top 4 cards of the opponent's deck; tails, of yours.</summary>
        private sealed class TimeTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Trap") return false;
                ctx.DamageDefending(atk.Damage);
                CardsE2.ReorderTop(ctx, ctx.Flip() ? ctx.Opponent : ctx.Player, 4, "Time Trap");
                return true;
            }
        }
    }
}
