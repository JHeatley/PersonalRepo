using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Emerald Rare Pokémon and Pokémon-ex (EM 1-21, 90-100, 107; v166). Unlimited only. 1-on-1: "each Defending Pokémon" is the Defending Pokémon.</summary>
    public static class CardsEM
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("EM-1", new LowHpBonusMon(new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned),
                CardsSS.ConditionBonus("Damage Burn", 20, d => d.DamageCounters > 0))));
            r.RegisterByKey("EM-2", new CardsDX.FormChangeMon("Deoxys", new CardsMA.SwiftMon("Swift")));
            r.RegisterByKey("EM-3", new ComboMon(new DrawAttackMon("Collect", 3), new ParalyzeFlipMon("Body Slam"), CardsSS.ConditionBonus("Mega Throw", 40, d => d.Def.IsOldEx)));
            r.RegisterByKey("EM-4", new HealDanceMon(new CardsHL.NoWeaknessResistanceMon("Mind Shock")));
            r.RegisterByKey("EM-5", new CardsFLF1.NoResistanceMon("Rock Tumble"));
            r.RegisterByKey("EM-6", new CardsFLF1.NoResistanceMon("Aqua Sonic"));
            r.RegisterByKey("EM-7", new ComboMon(new CardsMA.SwiftMon("Swift"), new CardsSS.AlsoEachBenchMon("Tail Shock", 10)));
            r.RegisterByKey("EM-8", new CardsPAR2.FlipConditionMon("Super Hypno Wave", SpecialCondition.Asleep));
            r.RegisterByKey("EM-9", new CardsDX.DanceMon("Dragon Dance"));
            r.RegisterByKey("EM-10", new SceptileGreenEssence());
            r.RegisterByKey("EM-11", new SwampertWaterCyclone());
            r.RegisterByKey("EM-12", new ComboMon(new DrawAttackMon("Collect", 1), new EvolvedConditionMon("Warp Sounds", SpecialCondition.Confused)));
            r.RegisterByKey("EM-13", new ComboMon(new CardsSS.EachOpponentMon("Hailstone", 10), CardsSS.ConditionBonus("Super Slash", 20, CardsE2.IsEvolved)));
            r.RegisterByKey("EM-14", new EruptionMon(10));
            r.RegisterByKey("EM-15", new ComboMon(new ScaryFaceMon(), new CardsBS1.ExtraWaterMon("Hydro Pump")));
            r.RegisterByKey("EM-16", new CardsPAR2.FlipConditionMon("Dazzle Blast", SpecialCondition.Confused));
            r.RegisterByKey("EM-17", new ComboMon(new DrawAttackMon("Collect", 1), new CardsSS.EachOpponentMon("Wide Laser", 10)));
            r.RegisterByKey("EM-18", new ConditionAttackMon("Hypno Splash", SpecialCondition.Asleep));
            r.RegisterByKey("EM-19", new CardsFLF1.NoResistanceMon("Rock Tumble"));
            r.RegisterByKey("EM-20", CardsSS.ConditionBonus("Bite Off", 30, d => d.Def.IsOldEx));
            r.RegisterByKey("EM-21", new ComboMon(CardsSS.ConditionBonus("Extra Claws", 20, d => d.Def.IsOldEx), new FlipBonusMon("Quick Attack", 10)));

            r.RegisterByKey("EM-90", new AltariaExMist());
            r.RegisterByKey("EM-91", new CacturneExCursedGlare());
            r.RegisterByKey("EM-92", new CardsDX.ConditionImmuneMon(SpecialCondition.Asleep | SpecialCondition.Paralyzed,
                new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new EruptionMon(20))));
            r.RegisterByKey("EM-93", new CardsDX.FormChangeMon("Deoxys ex", new PiercingMon("Fastwave")));
            r.RegisterByKey("EM-94", new DusclopsExDarkHole());
            r.RegisterByKey("EM-95", new MedichamExWiseAura());
            r.RegisterByKey("EM-96", new MiloticExMysticScale());
            r.RegisterByKey("EM-97", new RaichuExRaiShield());
            r.RegisterByKey("EM-98", new ComboMon(new ParalyzeFlipMon("Ice Beam"), new IcebergCrushMon()));
            r.RegisterByKey("EM-99", new ComboMon(new MendMon(), new CardsFST2.ConditionBonusMon("Metal Crush", 20, c => CardsSS.NamedInPlay(c, "Registeel ex"))));
            r.RegisterByKey("EM-100", new ComboMon(new BlockSignalMon(), new CardsSS.EachOpponentMon("Wide Laser", 20)));
            r.RegisterByKey("EM-107", new ComboMon(new DrawAttackMon("Spearhead", 1), new RecoilMon("Nosedive", 10)));
        }

        // ================================================================== Shared

        /// <summary>Blaze / Hunch (Poké-Body): with 40 HP or less left, this Pokémon's attacks do 40 more to the Defending Pokémon (before W/R).</summary>
        internal sealed class LowHpBonusMon : Mon
        {
            private readonly Mon attacks;

            public LowHpBonusMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || src.Engine.RemainingHp(me) > 40) return damage;
                return damage + 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Not affected by Resistance or by effects on the Defending Pokémon" (Weakness still applies): Dragon Mist, Fastwave.</summary>
        internal sealed class PiercingMon : Mon
        {
            private readonly string attack;

            public PiercingMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon is an Evolved Pokémon, it is now [condition]" (Warp Sounds, Strange Scale).</summary>
        internal sealed class EvolvedConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public EvolvedConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool evolved = defending != null && CardsE2.IsEvolved(defending);
                ctx.DamageDefending(atk.Damage);
                if (evolved && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>Heal Dance (Poké-Power): once during your turn, 2 damage counters off 1 of your Pokémon; only 1 Heal Dance each turn; not with a Special Condition.</summary>
        internal sealed class HealDanceMon : Mon
        {
            private const string Power = "Heal Dance";
            private readonly Mon attacks;

            public HealDanceMon(Mon attacks) { this.attacks = attacks; }

            protected override string Activated { get { return Power; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, Power + ": remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Eruption: each player discards the top card of their deck; N more for each Energy card discarded.</summary>
        internal sealed class EruptionMon : Mon
        {
            private readonly int per;

            public EruptionMon(int per) { this.per = per; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eruption") return false;
                int energy = 0;
                foreach (var card in ctx.Engine.DiscardTopOfDeck(ctx.Player, 1)) if (card.Def.IsEnergy) energy++;
                foreach (var card in ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1)) if (card.Def.IsEnergy) energy++;
                ctx.DamageDefending(atk.Damage + per * energy);
                return true;
            }
        }

        // ================================================================== Rares

        /// <summary>Green Essence (Poké-Body): while Sceptile is in play, your Active Pokémon with Grass Energy attached can't be affected by Special Conditions. Swift.</summary>
        private sealed class SceptileGreenEssence : Mon
        {
            private static readonly Mon Swift = new CardsMA.SwiftMon("Swift");

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                if (src.Pokemon == null || target == null || target.Owner != src.Player || !src.IsActive(target)) return false;
                return target.Energy.Any(e => CardsHIF1.Provides(src, target, e, EnergyType.Grass));
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swift.HandleAttack(ctx, atk); }
        }

        /// <summary>Water Cyclone (Poké-Power): as often as you like, move a Water Energy from your Active Pokémon to 1 of your Benched Pokémon; not with a Special Condition. Spinning Tail. Aqua Sonic.</summary>
        private sealed class SwampertWaterCyclone : Mon
        {
            private const string Power = "Water Cyclone";
            private static readonly Mon Attacks = new ComboMon(new CardsSS.EachOpponentMon("Spinning Tail", 10), new CardsFLF1.NoResistanceMon("Aqua Sonic"));

            private static List<CardInstance> Water(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return new List<CardInstance>();
                return active.Energy.Where(e => CardsHIF1.Provides(ctx, active, e, EnergyType.Water)).ToList();
            }

            private static bool Ready(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && Water(ctx).Count > 0; }

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
                    var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.Me.Bench.ToList(), Power + ": move a Water Energy from your Active Pokémon to which Benched Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    var cards = Water(ctx);
                    if (to == null || cards.Count == 0) return false;
                    var card = ctx.ChooseCards(cards, 1, 1, Power + ": move which Energy?")[0];
                    return ctx.Engine.MoveEnergy(ctx.Me.Active, to, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Scary Face: heads, the Defending Pokémon can't attack or retreat during the opponent's next turn.</summary>
        private sealed class ScaryFaceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scary Face") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Scary Face)", ctx.Player, defending.Uid, ctx.Turn + 1));
                ctx.AddEffect(new NoRetreatEffect(defending.Name + " can't retreat during its next turn (Scary Face)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoRetreatEffect : ContinuousEffect
        {
            public NoRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        // ================================================================== Pokémon-ex

        /// <summary>Mist (Poké-Body): attack damage from Stage 2 Pokémon (both players') is 30 less (after W/R). Dive. Dragon Mist.</summary>
        private sealed class AltariaExMist : Mon
        {
            private static readonly Mon Mist = new PiercingMon("Dragon Mist");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.Def.Stage == Stage.Stage2 && a.Stack.Count > 1 ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Mist.HandleAttack(ctx, atk); }
        }

        /// <summary>Cursed Glare (Poké-Body): while Cacturne ex is Active, the opponent can't attach Special Energy (other than Darkness and Metal Energy) from their hand to their Active Pokémon. Psybeam. Spike Rend.</summary>
        private sealed class CacturneExCursedGlare : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused),
                CardsSS.ConditionBonus("Spike Rend", 20, d => d.DamageCounters > 0));

            public override bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || energy == null || target == null || target.Owner == src.Player || !src.IsActive(target)) return false;
                var d = energy.Def;
                // A Holon's Pokémon attached as Energy is a Special Energy card (Compendium: Cursed Glare, 2006-10-05; v169).
                return (d.IsSpecialEnergy || d.AttachesAsEnergy) && d.Name != "Darkness Energy" && d.Name != "Metal Energy";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Hole (Poké-Body): while Dusclops ex is on your Bench, your Pokémon have no Darkness Weakness. Shadow Beam: 2 damage counters per Energy on Dusclops ex.</summary>
        private sealed class DusclopsExDarkHole : Mon
        {
            public override EnergyType? ModifyWeakness(EffectContext src, PokemonInPlay mon, EnergyType? weakness)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner != src.Player || !src.Me.Bench.Contains(me)) return weakness;
                return weakness == EnergyType.Darkness ? (EnergyType?)null : weakness;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Beam") return false;
                int n = 2 * ctx.EnergyCount(ctx.Pokemon);
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Wise Aura (Poké-Body): while Medicham ex is your Active Pokémon, no Pokémon (both players') other than Pokémon-ex can use Poké-Powers. Pure Power. Sky Kick.</summary>
        private sealed class MedichamExWiseAura : Mon
        {
            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && mon != null && !mon.Def.IsOldEx;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pure Power")
                {
                    ctx.PlaceCountersAnyWay(3, () => ctx.OpponentPokemon());
                    return true;
                }
                if (atk.Name != "Sky Kick") return false;
                var d = ctx.Defending;
                bool resists = d != null && (d.Def.Resistance == EnergyType.Fighting || d.Def.SecondResistance == EnergyType.Fighting);
                ctx.DamageDefending(atk.Damage + (resists ? 40 : 0));
                return true;
            }
        }

        /// <summary>
        /// Mystic Scale (Poké-Body): while Milotic ex is in play, no one can play Technical Machine cards, and all Technical Machines
        /// in play are discarded. Gentle Wrap. Reflect Energy.
        /// </summary>
        private sealed class MiloticExMysticScale : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new NoRetreatMon("Gentle Wrap"), new CardsE1.ReflectEnergyMon());

            private static bool IsTM(CardDef d) { return d != null && d.IsTrainer && d.Name.Contains("Technical Machine"); }

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card) { return src.Pokemon != null && card != null && IsTM(card.Def); }

            public override void OnStateCheck(EffectContext src)
            {
                if (src.Pokemon == null) return;
                for (int side = 0; side < 2; side++)
                    foreach (var mon in src.State.Players[side].InPlay().ToList())
                    {
                        foreach (var tm in mon.Tools.Where(c => IsTM(c.Def)).ToList()) src.Engine.DiscardTool(mon, tm);
                        foreach (var tm in mon.Attached.Where(c => IsTM(c.Def)).ToList())
                            if (mon.Attached.Remove(tm)) src.State.Players[tm.Owner].Discard.Add(tm);
                    }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rai-shield (Poké-Body): attack damage from the opponent's Pokémon-ex to each of your Raichu ex is 30 less (after W/R); only 1
        /// Rai-shield applies. Power Short: 30 to 1 of the opponent's Pokémon, 20 more if it has a Poké-Power. Pika Bolt.
        /// </summary>
        private sealed class RaichuExRaiShield : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var t = info.Target;
                var a = info.Attacker;
                if (me == null || t == null || t.Owner != src.Player || t.Name != "Raichu ex" || a == null || a.Owner == src.Player || !a.Def.IsOldEx) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                var first = src.MyPokemon(m => src.Engine.HasAbilityNamed(m, "Rai-shield")).OrderBy(m => m.Uid).FirstOrDefault();
                return first == me ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Short") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Power Short: choose 1 of your opponent's Pokémon");
                if (target == null) return true;
                bool power = target.Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower);
                ctx.DamageTarget(target, 30 + (power ? 20 : 0));
                return true;
            }
        }

        /// <summary>Iceberg Crush: if Regirock ex is in play, flip; heads, discard an Energy card from the Defending Pokémon.</summary>
        private sealed class IcebergCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iceberg Crush") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!CardsSS.NamedInPlay(ctx, "Regirock ex") || !ctx.Flip()) return true;
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Iceberg Crush: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Mend: a Fighting Energy card from your discard pile onto Regirock ex; if you do, 1 damage counter comes off it.</summary>
        private sealed class MendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mend") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Mend: attach which Fighting Energy card?")[0];
                if (!ctx.Me.Discard.Remove(card)) return true;
                if (!ctx.Engine.AttachEnergyCard(card, me))
                {
                    ctx.Me.Discard.Add(card);
                    return true;
                }
                if (me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Block Signal: if Regice ex is in play, flip; heads, the Defending Pokémon is Confused.</summary>
        private sealed class BlockSignalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Block Signal") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (CardsSS.NamedInPlay(ctx, "Regice ex") && ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
