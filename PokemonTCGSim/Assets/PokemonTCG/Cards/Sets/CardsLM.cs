using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legend Maker Rare Pokémon, Pokémon-ex and Pokémon ★ (LM 1-28, 83-92; v170). Unlimited only. Many cards care about React
    /// Energy cards (LM 82, a Special Energy that provides [C]); a Holon's Pokémon attached as Energy isn't one. Mew ex 88 is the
    /// Celebrations' and Rainbow Energy 81 is Celestial Storm's (prints). Compendium rulings: Shadow Curse and Reactive Recharge
    /// need damage from the attack (not placed damage counters) and a working Poké-Power when the Knock Out happens; Banette ex's
    /// Shady Move moves 1 counter between any 2 Pokémon, once per Banette ex each turn; Evolutionary Fan may return Shiftry ex
    /// (only cards named Shiftry are excluded); Wobbuffet's Stages of Evolution adds up and affects only Active Pokémon; Shadow
    /// Tag's counters go away if the Defending Pokémon moves or evolves; Mew's Type Change stays on Mew if it's switched.
    /// 1-on-1: "1 of the Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsLM
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LM-1", new AerodactylReactiveProtection());
            r.RegisterByKey("LM-2", new AggronMon());
            r.RegisterByKey("LM-3", new ComboMon(new DevolutionWaveMon(), new CardsSSB.AnyOpponentTextMon("Linear Attack", 30)));
            r.RegisterByKey("LM-4", new DelcattyReactiveShift());
            r.RegisterByKey("LM-5", new GengarShadowCurse());
            r.RegisterByKey("LM-6", new ComboMon(new MendMon(), new EnragedLinearAttackMon(), new CardsFLF1.NoResistanceMon("Rock Tumble")));
            r.RegisterByKey("LM-7", new KabutopsAncientShell());
            r.RegisterByKey("LM-8", new LaprasSupportNavigation());
            r.RegisterByKey("LM-9", new ComboMon(new DerailMon(), new CardsASR1.PlusMon("Swift Blow", 20, c => React(c.Pokemon))));
            r.RegisterByKey("LM-10", new MewTypeChange());
            r.RegisterByKey("LM-11", new MukStench());
            r.RegisterByKey("LM-12", new ShiftryEvolutionaryFan());
            r.RegisterByKey("LM-13", new VictreebelNectarPod());
            r.RegisterByKey("LM-14", new WailordReactiveLift());
            r.RegisterByKey("LM-15", new ShiningHornMon(new ComboMon(new SearchAttackMon("Extra Call", d => d.IsPokemon && d.IsOldEx, 1),
                new CardsCES1.FeintAttackMon("Feint Attack", 20))));
            r.RegisterByKey("LM-16", new GirafarigRearSensor());
            r.RegisterByKey("LM-17", new GorebyssReactiveBooster());
            r.RegisterByKey("LM-18", new HuntailReactiveGenerator());
            r.RegisterByKey("LM-19", new CardsTRR.DualArmorMon(EnergyType.Water, new CardsSS.WaterExtraMon("Water Gun", 20)));
            r.RegisterByKey("LM-20", new ShadeMon("Sol Shade", "Solrock", EnergyType.Fire, new ComboMon(
                new SearchAttackMon("Moon Guidance", d => d.IsTrainer && !d.IsSupporter, 1), new ParalyzeFlipMon("Psyshock"))));
            r.RegisterByKey("LM-21", new MagmarStages());
            r.RegisterByKey("LM-22", new MagnetonReactiveRecharge());
            r.RegisterByKey("LM-23", new OmastarAncientFang());
            r.RegisterByKey("LM-24", new ShiningHornMon(new ComboMon(new SearchAttackMon("Cry for Help", d => d.IsPokemon && d.HasType(EnergyType.Grass) && !d.IsOldEx, 1),
                new CardsASR1.BenchRecoilMon("Overhead Toss", 20))));
            r.RegisterByKey("LM-25", new ShadeMon("Luna Shade", "Lunatone", EnergyType.Colorless, new ComboMon(
                new CardsSS.BenchNamedMon("Call for Family", new[] { "Lunatone" }, false, 1), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"))));
            r.RegisterByKey("LM-26", new SpindaPatternDistraction());
            r.RegisterByKey("LM-27", new ComboMon(new SearchAttackMon("Cry for Help", d => d.IsPokemon && d.HasType(EnergyType.Fire) && !d.IsOldEx, 1), new FireworksMon()));
            r.RegisterByKey("LM-28", new WobbuffetStages());

            r.RegisterByKey("LM-83", new ArcanineExFireRemedy());
            r.RegisterByKey("LM-84", new ArmaldoExDualArmor());
            r.RegisterByKey("LM-85", new BanetteExShadyMove());
            r.RegisterByKey("LM-86", new CardsSS.SafeguardExMon(new CardsHL.SilverWindMon()));
            r.RegisterByKey("LM-87", new FlygonExEmergeCharge());
            r.RegisterByKey("LM-89", new WalreinExIcyAura());
            r.RegisterByKey("LM-90", new RegiceStar());
            r.RegisterByKey("LM-91", new RegirockStar());
            r.RegisterByKey("LM-92", new RegisteelStar());
        }

        // ================================================================== Shared (React Energy)

        /// <summary>A React Energy card. A Holon's Pokémon attached as Energy isn't one.</summary>
        internal static bool IsReact(CardDef d) { return d != null && d.IsSpecialEnergy && d.Name == "React Energy"; }

        /// <summary>React Energy cards attached to the Pokémon.</summary>
        internal static int React(PokemonInPlay mon) { return mon == null ? 0 : mon.Energy.Count(e => IsReact(e.Def)); }

        internal static bool HasReact(PokemonInPlay mon) { return React(mon) > 0; }

        /// <summary>"Your opponent has only 1 Prize card left and this Pokémon is the only Pokémon you have in play" (the Regi ★ attacks).</summary>
        internal static bool LastStand(EffectContext ctx) { return ctx.Opp.Prizes.Count == 1 && ctx.Me.InPlay().Count() == 1; }

        /// <summary>
        /// "During your opponent's next turn, prevent all effects, including damage, done to this Pokémon by attacks from your
        /// opponent's Pokémon-ex" (Speed Stroke, Ice Barrier). It ends if the Pokémon moves to the Bench.
        /// </summary>
        internal sealed class ExGuardEffect : ContinuousEffect
        {
            public ExGuardEffect(string name, int owner, int targetUid, int currentTurn) : base(name, owner, targetUid, currentTurn + 1, true) { }

            private bool FromOpposingEx(PokemonInPlay attacker) { return attacker != null && attacker.Owner != Owner && attacker.Def.IsOldEx; }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return FromOpposingEx(info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return target != null && target.Uid == TargetUid && kind == EffectSourceKind.Attack && FromOpposingEx(engine.CurrentSourcePokemon);
            }
        }

        /// <summary>"Does N damage. [Guard effect] during your opponent's next turn" (Speed Stroke, Ice Barrier).</summary>
        internal sealed class ExGuardMon : Mon
        {
            private readonly string attack;

            public ExGuardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    ctx.AddEffect(new ExGuardEffect(me.Name + " is protected from attacks by Pokémon-ex during " + ctx.Engine.Possessive(ctx.Opponent) + " next turn (" + attack + ")",
                        ctx.Player, me.Uid, ctx.Turn));
                return true;
            }
        }

        /// <summary>Shining Horn (Poké-Body): as long as this Pokémon is the only Pokémon you have in play, your opponent's Basic Pokémon can't attack.</summary>
        internal sealed class ShiningHornMon : Mon
        {
            private readonly Mon attacks;

            public ShiningHornMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && mon.IsBasic && src.Me.InPlay().Count() == 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Sol Shade / Luna Shade (Poké-Body): as long as you have [partner] in play, each player's [type] Pokémon (excluding
        /// Pokémon-ex) can't use any Poké-Powers.
        /// </summary>
        internal sealed class ShadeMon : Mon
        {
            private readonly string body;
            private readonly string partner;
            private readonly EnergyType type;
            private readonly Mon attacks;

            public ShadeMon(string body, string partner, EnergyType type, Mon attacks)
            {
                this.body = body;
                this.partner = partner;
                this.type = type;
                this.attacks = attacks;
            }

            /// <summary>Working out a type asks which Abilities work, which asks this body again: inside that, it stops nothing.</summary>
            [ThreadStatic] private static bool checking;

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (checking || me == null || mon == null || mon.Def.IsOldEx || !src.Me.InPlay().Any(m => m.Name == partner)) return false;
                checking = true;
                try { return src.Engine.IsType(mon, type) && !src.Engine.AbilityBlocked(me, body); }
                finally { checking = false; }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Rares

        /// <summary>Reactive Protection (Poké-Body): damage from the opponent's attacks is 10 less per React Energy card on it (after W/R). Power Blow. Speed Stroke.</summary>
        private sealed class AerodactylReactiveProtection : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASR1.PlusMon("Power Blow", 10, c => c.EnergyCount(c.Pokemon)), new ExGuardMon("Speed Stroke"));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !info.AgainstOpponent) return damage;
                return damage - 10 * React(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Heavy Blow: 70 minus 10 per damage counter on Aggron, or 70 with React Energy attached. Bound Crush: 60 to any 1 of the opponent's Pokémon; not again next turn.</summary>
        private sealed class AggronMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Heavy Blow")
                {
                    ctx.DamageDefending(HasReact(me) ? atk.Damage : Math.Max(0, atk.Damage - 10 * me.DamageCounters));
                    return true;
                }
                if (atk.Name != "Bound Crush") return false;
                ctx.DamageAnyOpponent(60);
                if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn("Bound Crush");
                return true;
            }
        }

        /// <summary>
        /// Devolution Wave: choose up to 1 of the opponent's Evolved Pokémon per React Energy card on Cradily; each loses its highest
        /// Stage Evolution card, shuffled into the opponent's deck (choices first, so a Stage 2 is chosen only once: Compendium).
        /// </summary>
        private sealed class DevolutionWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devolution Wave") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = React(ctx.Pokemon);
                var targets = ctx.OpponentPokemon(m => CardsE2.IsEvolved(m) && ctx.CanAffect(m));
                if (n == 0 || targets.Count == 0) return true;
                var picks = ctx.ChoosePokemonMany(targets, 0, Math.Min(n, targets.Count), "Devolution Wave: choose up to " + n + " of your opponent's Evolved Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.Engine.Devolve(mon, 1, true);
                return true;
            }
        }

        /// <summary>Reactive Shift (Poké-Power): once during your turn, move a React Energy card from 1 of your Pokémon to another. Energy Link.</summary>
        private sealed class DelcattyReactiveShift : Mon
        {
            private const string Power = "Reactive Shift";
            private static readonly Mon Link = new CardsTRR.EnergyLinkMon("Energy Link");

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.MyPokemon().Count > 1 && ctx.MyPokemon(HasReact).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var e in mon.Energy.Where(c => IsReact(c.Def))) holders[e] = mon;
                if (holders.Count == 0) return;
                var card = ctx.ChooseCards(holders.Keys.ToList(), 1, 1, Power + ": move which React Energy card?")[0];
                var from = holders[card];
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), Power + ": move it to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Link.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Shadow Curse (Poké-Power): if Gengar is Knocked Out by damage from an opponent's attack, you may put 3 damage counters on 1
        /// of the opponent's Pokémon. Cursed Reaction: 2 counters any way you like (4 with React Energy on Gengar).
        /// </summary>
        private sealed class GengarShadowCurse : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null) return;
                var targets = src.OpponentPokemon();
                if (targets.Count == 0 || !src.AskYesNo("Shadow Curse: put 3 damage counters on 1 of your opponent's Pokémon?")) return;
                var target = src.ChoosePokemon(targets, "Shadow Curse: put 3 damage counters on which Pokémon?");
                if (target == null) return;
                Announce(src, "Shadow Curse");
                src.Engine.PlaceDamageCounters(target, 3, "Shadow Curse");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cursed Reaction") return false;
                ctx.PlaceCountersAnyWay(HasReact(ctx.Pokemon) ? 4 : 2, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Mend: a Fighting Energy card from your discard pile onto this Pokémon; if you do, 2 damage counters come off it.</summary>
        private sealed class MendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mend") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.BasicEnergyInDiscard(d => CardsDS.BasicOf(d, EnergyType.Fighting));
                if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Mend: attach which Fighting Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && ctx.Engine.AttachEnergyCard(card, me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                return true;
            }
        }

        /// <summary>Enraged Linear Attack: 10 damage per damage counter on this Pokémon to 1 of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class EnragedLinearAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enraged Linear Attack") return false;
                int dmg = 10 * ctx.Pokemon.DamageCounters;
                if (dmg > 0) ctx.DamageAnyOpponent(dmg);
                return true;
            }
        }

        /// <summary>Ancient Shell (Poké-Body): with your Omanyte or Omastar in play, attack damage to Kabutops is 20 less (after W/R). Energy Stream. Extra Claws.</summary>
        private sealed class KabutopsAncientShell : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsRGB.EnergyStreamMon(), CardsSS.ConditionBonus("Extra Claws", 30, d => d.Def.IsOldEx));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null) return damage;
                return src.Me.InPlay().Any(m => m.Name == "Omanyte" || m.Name == "Omastar") ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Support Navigation (Poké-Power): when you put Lapras onto your Bench from your hand, you may search your deck for a Supporter card.</summary>
        private sealed class LaprasSupportNavigation : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Support Navigation", ctx.Me.Deck.Count > 0, "Support Navigation: search your deck for a Supporter card?")) return;
                ctx.SearchToHand(d => d.IsSupporter, 1, "Support Navigation: choose a Supporter card");
            }
        }

        /// <summary>Derail: discard a Special Energy card, if any, from the Defending Pokémon.</summary>
        internal sealed class DerailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Derail") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => !e.Def.IsBasicEnergy))
                    ctx.DiscardEnergyFrom(defending, 1, "Derail: discard a Special Energy card from " + defending.Name, e => !e.Def.IsBasicEnergy);
                return true;
            }
        }

        /// <summary>Mew's Type Change: "Mew is the same type as that Pokémon until the end of your turn" (it stays with Mew if Mew is switched: Compendium).</summary>
        private sealed class TypeChangeEffect : ContinuousEffect
        {
            private readonly List<EnergyType> types;

            public TypeChangeEffect(string name, int owner, int targetUid, int currentTurn, List<EnergyType> types) : base(name, owner, targetUid, currentTurn, false)
            {
                this.types = types;
            }

            public override void ModifyTypes(GameEngine engine, PokemonInPlay mon, List<EnergyType> list)
            {
                if (mon == null || mon.Uid != TargetUid) return;
                list.Clear();
                list.AddRange(types);
            }
        }

        /// <summary>Type Change (Poké-Power): Mew becomes the Defending Pokémon's type(s) until the end of your turn. Link Blast: base 20 if the Energy amounts differ.</summary>
        private sealed class MewTypeChange : Mon
        {
            private static readonly Mon Blast = new CardsDS.BaseIfMon("Link Blast", 20, c => c.Defending != null && c.EnergyCount(c.Pokemon) != c.EnergyCount(c.Defending));

            protected override string Activated { get { return "Type Change"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Defending != null; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var types = ctx.Engine.TypesOf(ctx.Defending).ToList();
                ctx.AddEffect(new TypeChangeEffect(me.Name + " is " + string.Join(" and ", types) + " type until the end of the turn (Type Change)", ctx.Player, me.Uid, ctx.Turn, types));
                ctx.Log("Type Change: " + me.Name + " is now " + string.Join(" and ", types) + " type until the end of the turn.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Stench (Poké-Body): while Muk is your Active Pokémon, no Pokémon (both players') can use Poké-Powers. Poison Ring.</summary>
        private sealed class MukStench : Mon
        {
            private static readonly Mon Ring = new CardsPOR.PoisonRingMon("Poison Ring");

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                return me != null && mon != null && src.IsActive(me) && !src.Engine.AbilityBlocked(me, "Stench");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ring.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Evolutionary Fan (Poké-Power): when you evolve into Shiftry from your hand, you may return 1 of your Evolved Pokémon (not
        /// one named Shiftry) and its cards to your hand. Reactive Beating: Confused with React Energy on Shiftry. Cross-Cut.
        /// </summary>
        private sealed class ShiftryEvolutionaryFan : Mon
        {
            private static readonly Mon CrossCut = CardsSS.ConditionBonus("Cross-Cut", 30, CardsE2.IsEvolved);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var targets = ctx.MyPokemon(m => CardsE2.IsEvolved(m) && m.Name != "Shiftry");
                if (!TriggerOnce(ctx, "Evolutionary Fan", targets.Count > 0, "Evolutionary Fan: return 1 of your Evolved Pokémon and its cards to your hand?")) return;
                var target = ctx.ChoosePokemon(targets, "Evolutionary Fan: return which Pokémon to your hand?");
                if (target != null) ctx.Engine.ReturnPokemonToHand(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reactive Beating") return CrossCut.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (HasReact(ctx.Pokemon) && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Nectar Pod (Poké-Power): switch 1 of the opponent's Benched Stage 2 Evolved Pokémon with their Active Pokémon. Sleep Poison. Sharp Leaf.</summary>
        private sealed class VictreebelNectarPod : Mon
        {
            private const string Power = "Nectar Pod";
            private static readonly Mon Attacks = new ComboMon(new CardsCES1.ConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned),
                new FlipBonusMon("Sharp Leaf", 30));

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentBench().Where(m => m.Def.Stage == Stage.Stage2 && CardsE2.IsEvolved(m)).ToList(); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Active != null && Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Targets(ctx), Power + ": switch which of your opponent's Benched Stage 2 Pokémon into the Active Spot?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Reactive Lift (Poké-Body): with React Energy on Wailord, your Water Pokémon (not Pokémon-ex) have no Retreat Cost. Hypno Splash. Rely on Friends.</summary>
        private sealed class WailordReactiveLift : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Hypno Splash", SpecialCondition.Asleep),
                new CardsASR1.PlusMon("Rely on Friends", 10, c => c.Me.Bench.Count(m => m.Def.Stage == Stage.Stage1 && CardsE2.IsEvolved(m))));

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon.Owner != src.Player || mon.Def.IsOldEx || !HasReact(src.Pokemon)) return cost;
                return src.Engine.IsType(mon, EnergyType.Water) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rear Sensor (Poké-Body): each player's Active Basic Pokémon (not Pokémon-ex) can't use Poké-Powers. Foresight: reorder the
        /// top 5 cards of either deck. Disorder: Confused if the Defending Pokémon has Special Energy attached.
        /// </summary>
        private sealed class GirafarigRearSensor : Mon
        {
            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Def.IsOldEx || !mon.IsBasic || !src.State.IsActive(mon)) return false;
                return !src.Engine.AbilityBlocked(me, "Rear Sensor");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Foresight")
                {
                    int pick = ctx.ChooseOption("Foresight: look at the top 5 cards of which deck?", "Your deck", "Your opponent's deck");
                    CardsE2.ReorderTop(ctx, pick == 0 ? ctx.Player : ctx.Opponent, 5, "Foresight");
                    return true;
                }
                if (atk.Name != "Disorder") return false;
                var defending = ctx.Defending;
                bool special = defending != null && defending.Energy.Any(e => e.Def.IsSpecialEnergy);
                ctx.DamageDefending(atk.Damage);
                if (special && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Reactive Booster (Poké-Body): each React Energy card on your Huntail and this Gorebyss provides 2 Energy of every type (and
        /// does nothing else). Hydro Pump: 20 more per unused Water Energy, up to 40 more.
        /// </summary>
        private sealed class GorebyssReactiveBooster : Mon
        {
            private static readonly Mon Pump = new CardsSS.WaterExtraMon("Hydro Pump", 20);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (energy == null || holder == null || !IsReact(energy.Def) || holder.Owner != src.Player) return;
                if (holder != src.Pokemon && holder.Name != "Huntail") return;
                units.Clear();
                units.Add(EnergyMask.AnyType);
                units.Add(EnergyMask.AnyType);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Reactive Generator (Poké-Power): with no React Energy on Huntail, a React Energy card from your deck onto it. Bite Off.</summary>
        private sealed class HuntailReactiveGenerator : Mon
        {
            private const string Power = "Reactive Generator";
            private static readonly Mon BiteOff = CardsSS.ConditionBonus("Bite Off", 30, d => d.Def.IsOldEx);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && !HasReact(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                foreach (var card in ctx.SearchMyDeck(IsReact, 1, Power + ": choose a React Energy card")) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BiteOff.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Stages of Evolution (Poké-Body): while Magmar is an Evolved Pokémon, all its Energy is Fire Energy instead. Linear Attack.
        /// Burning Sensation: Burned if the Defending Pokémon already had damage counters.
        /// </summary>
        private sealed class MagmarStages : Mon
        {
            private static readonly Mon Linear = new CardsSSB.AnyOpponentTextMon("Linear Attack", 10);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon || !CardsUF.Evolved(holder)) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Sensation") return Linear.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                bool damaged = defending != null && defending.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage);
                if (damaged && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>
        /// Reactive Recharge (Poké-Power): if Magneton is Knocked Out by damage from an opponent's attack, you may move any number of
        /// React Energy cards from it to your other Pokémon. Multiple Force: with React Energy, 10 more per Magnemite and Magneton in play.
        /// </summary>
        private sealed class MagnetonReactiveRecharge : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null) return;
                var react = me.Energy.Where(e => IsReact(e.Def)).ToList();
                var others = src.MyPokemon(m => m != me);
                if (react.Count == 0 || others.Count == 0) return;
                var picks = src.ChooseCards(react, 0, react.Count, "Reactive Recharge: move which React Energy cards from " + me.Name + " to your other Pokémon?");
                if (picks.Count == 0) return;
                Announce(src, "Reactive Recharge");
                foreach (var card in picks)
                {
                    var to = src.ChoosePokemon(others, "Reactive Recharge: move this React Energy card to which Pokémon?");
                    if (to != null && me.Energy.Contains(card)) src.Engine.MoveEnergy(me, to, card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Multiple Force") return false;
                int bonus = 0;
                if (HasReact(ctx.Pokemon))
                    bonus = 10 * (ctx.MyPokemon(m => m.Name == "Magnemite" || m.Name == "Magneton").Count + ctx.OpponentPokemon(m => m.Name == "Magnemite" || m.Name == "Magneton").Count);
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Ancient Fang (Poké-Body): with your Kabuto, Kabutops or Kabutops ex in play, Omastar's attacks do 20 more to the Defending Pokémon (before W/R). Drag Off.</summary>
        private sealed class OmastarAncientFang : Mon
        {
            private static readonly Mon DragOff = new CardsTRR.DragOffMayMon();

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Me.InPlay().Any(m => m.Name == "Kabuto" || m.Name == "Kabutops" || m.Name == "Kabutops ex") ? damage + 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DragOff.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Pattern Distraction (Poké-Body): while Spinda is your Active Pokémon, the opponent flips a coin when their Basic Pokémon
        /// tries to attack; tails, the attack does nothing. Whimsy Draw: flip until tails, draw 2 per heads. Double-edge.
        /// </summary>
        private sealed class SpindaPatternDistraction : Mon
        {
            private static readonly Mon DoubleEdge = new CardsFST1.FlipRecoilMon("Double-edge", 10);

            public override int AttackFlipCoins(EffectContext src, PokemonInPlay attacker)
            {
                var me = src.Pokemon;
                return me != null && attacker != null && attacker.Owner != src.Player && attacker.IsBasic && src.IsActive(me) ? 1 : 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whimsy Draw") return DoubleEdge.HandleAttack(ctx, atk);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Draw(2 * heads);
                return true;
            }
        }

        /// <summary>Fireworks: flip a coin; tails, discard a Fire Energy attached to this Pokémon.</summary>
        private sealed class FireworksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fireworks") return false;
                var me = ctx.Pokemon;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && !CardsBS1.CopyingWithMetronome && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, "Fireworks: discard a Fire Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>
        /// Stages of Evolution (Poké-Body): while Wobbuffet is an Evolved Pokémon, the opponent's Active Pokémon's Retreat Cost is [C]
        /// more (each evolved Wobbuffet adds 1: Compendium). Grind. Shadow Tag: 7 damage counters on the Defending Pokémon at the end of
        /// the opponent's next turn (gone if it moves or evolves).
        /// </summary>
        private sealed class WobbuffetStages : Mon
        {
            private static readonly Mon Grind = new CardsASR1.TimesMon("Grind", c => c.EnergyCount(c.Pokemon));

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || !CardsUF.Evolved(me) || !src.State.IsActive(mon)) return cost;
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Tag") return Grind.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsTRR.DelayedCountersEffect(defending.Name + " gets 7 damage counters at the end of " + ctx.Engine.Possessive(ctx.Opponent) + " next turn (Shadow Tag)",
                        ctx.Player, defending.Uid, ctx.Turn, 7, false, "Shadow Tag"));
                return true;
            }
        }

        // ================================================================== Pokémon-ex

        /// <summary>
        /// Fire Remedy (Poké-Body): attaching a Fire Energy card from your hand to Arcanine ex removes 1 damage counter and all Special
        /// Conditions (1 counter even for a card that provides 2: Compendium). Overrun. Flame Swirl: discard 2 Fire Energy or 1 React Energy.
        /// </summary>
        private sealed class ArcanineExFireRemedy : Mon
        {
            private static readonly Mon Overrun = new Cards30CC.BenchHitMon("Overrun", 20);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !CardsHIF1.Provides(src, me, energy, EnergyType.Fire)) return;
                if (me.DamageCounters == 0 && me.Conditions == SpecialCondition.None) return;
                Announce(src, "Fire Remedy");
                if (me.DamageCounters > 0) src.Heal(me, 10);
                src.Engine.RemoveAllConditions(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Swirl") return Overrun.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (CardsBS1.CopyingWithMetronome || !ctx.InPlay(me)) return true;
                var fire = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire)).ToList();
                var react = me.Energy.Where(e => IsReact(e.Def)).ToList();
                bool useReact = react.Count > 0 && (fire.Count < 2 || ctx.ChooseOption("Flame Swirl: discard which?", "2 Fire Energy", "1 React Energy card") == 1);
                if (useReact) ctx.Engine.DiscardEnergy(me, react.Count == 1 ? react[0] : ctx.ChooseCards(react, 1, 1, "Flame Swirl: discard which React Energy card?")[0]);
                else ctx.DiscardEnergyFrom(me, Math.Min(2, fire.Count), "Flame Swirl: discard 2 Fire Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Dual Armor (Poké-Body): with React Energy attached, Armaldo ex is both Grass and Fighting. Spiral Drain. Vortex Chop: base 100 if the Defending Pokémon has Resistance.</summary>
        private sealed class ArmaldoExDualArmor : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsTWM1.HealSelfMon("Spiral Drain", 20),
                new CardsDS.BaseIfMon("Vortex Chop", 100, c => c.Defending != null && c.Engine.ResistanceOf(c.Defending).HasValue));

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon != src.Pokemon || !HasReact(mon)) return;
                if (!types.Contains(EnergyType.Grass)) types.Add(EnergyType.Grass);
                if (!types.Contains(EnergyType.Fighting)) types.Add(EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Shady Move (Poké-Power): once during your turn, if Banette ex is Active, move 1 damage counter from any Pokémon to another
        /// (either player's; once per Banette ex each turn: Compendium). Shadow Chant: 10 more per Supporter in your discard pile, up to 60 more.
        /// </summary>
        private sealed class BanetteExShadyMove : Mon
        {
            private const string Power = "Shady Move";

            private static List<PokemonInPlay> All(EffectContext ctx) { return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList(); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.PokemonIsActive && All(ctx).Count > 1 && All(ctx).Any(m => m.DamageCounters > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                var all = All(ctx);
                var from = ctx.ChoosePokemon(all.Where(m => m.DamageCounters > 0).ToList(), Power + ": move a damage counter from which Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(all.Where(m => m != from).ToList(), Power + ": move it to which Pokémon?");
                if (to != null) ctx.Engine.MoveDamageCounters(from, to, 1, Power);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Chant") return false;
                ctx.DamageDefending(atk.Damage + Math.Min(60, 10 * ctx.Me.Discard.Count(c => c.Def.IsSupporter)));
                return true;
            }
        }

        /// <summary>
        /// Emerge Charge (Poké-Power): when you evolve into Flygon ex from your hand, up to 2 Energy cards from your discard pile onto it.
        /// Reactive Blast: discard any number of React Energy cards from it for 30 more each.
        /// </summary>
        private sealed class FlygonExEmergeCharge : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (me == null || !TriggerOnce(ctx, "Emerge Charge", cards.Count > 0, "Emerge Charge: attach up to 2 Energy cards from your discard pile to Flygon ex?")) return;
                foreach (var card in ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Emerge Charge: attach which Energy cards to Flygon ex?"))
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reactive Blast") return false;
                var me = ctx.Pokemon;
                var react = me.Energy.Where(e => IsReact(e.Def)).ToList();
                int discarded = 0;
                if (react.Count > 0 && !CardsBS1.CopyingWithMetronome)
                    foreach (var card in ctx.ChooseCards(react, 0, react.Count, "Reactive Blast: discard any number of React Energy cards for 30 more damage each"))
                        if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                ctx.DamageDefending(atk.Damage + 30 * discarded);
                return true;
            }
        }

        /// <summary>Icy Aura (Poké-Body): while Walrein ex is your Active Pokémon, 1 damage counter on each Active Pokémon (both players') between turns, except Water Pokémon. Water Arrow. Ice Throw.</summary>
        private sealed class WalreinExIcyAura : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSSB.AnyOpponentTextMon("Water Arrow", 40),
                new CardsDS.BaseIfMon("Ice Throw", 120, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Fighting)));

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? -1 : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var targets = new[] { src.Me.Active, src.Opp.Active }.Where(m => m != null && !src.Engine.IsType(m, EnergyType.Water)).ToList();
                if (targets.Count == 0) return;
                Announce(src, "Icy Aura");
                foreach (var mon in targets)
                    if (src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Icy Aura");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Pokémon ★

        /// <summary>Ice Barrier: protected from Pokémon-ex attacks next turn. Final Blizzard: with 1 Prize card left for the opponent and Regice ★ alone in play, 30 to each of their Benched Pokémon.</summary>
        private sealed class RegiceStar : Mon
        {
            private static readonly Mon Barrier = new ExGuardMon("Ice Barrier");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Final Blizzard") return Barrier.HandleAttack(ctx, atk);
                bool last = LastStand(ctx);
                ctx.DamageDefending(atk.Damage);
                if (last)
                    foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Dig Drain: 1 damage counter off Regirock ★. Final Blast: base 100 with 1 Prize card left for the opponent and Regirock ★ alone in play.</summary>
        private sealed class RegirockStar : Mon
        {
            private static readonly Mon Blast = new CardsDS.BaseIfMon("Final Blast", 100, LastStand);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Drain") return Blast.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Barrier Attack: 10 less damage next turn. Final Laser: 3 damage counters any way you like (6 with 1 Prize card left for the opponent and Registeel ★ alone in play).</summary>
        private sealed class RegisteelStar : Mon
        {
            private static readonly Mon Barrier = new ShieldAttackMon("Barrier Attack", 10);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Final Laser") return Barrier.HandleAttack(ctx, atk);
                ctx.PlaceCountersAnyWay(LastStand(ctx) ? 6 : 3, () => ctx.OpponentPokemon());
                return true;
            }
        }
    }
}
