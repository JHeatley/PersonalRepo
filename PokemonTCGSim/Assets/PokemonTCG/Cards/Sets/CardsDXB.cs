using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Deoxys Uncommon and Common Pokémon (DX 31-83; v165). Unlimited only.</summary>
    public static class CardsDXB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DX-31", new CardsDX.ConditionImmuneMon(SpecialCondition.Paralyzed, new CardsTWM1.HealSelfMon("Spiral Drain", 10)));
            r.RegisterByKey("DX-32", new CardsDX.ConditionImmuneMon(SpecialCondition.Confused, new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep),
                CardsSS.ConditionBonus("Extra Ball", 30, d => d.Def.IsOldEx))));
            r.RegisterByKey("DX-33", new CardsDX.TypeLiftMon("Aqua Lift", EnergyType.Water, new FlipBonusMon("Ambush", 20)));
            r.RegisterByKey("DX-34", new CardsRS.NaturalCureMon(EnergyType.Water, new CardsTWM1.HealSelfMon("Blot", 20)));
            r.RegisterByKey("DX-35", new CardsSS.SurpriseMon());
            r.RegisterByKey("DX-36", new LunatoneMoonglow());
            r.RegisterByKey("DX-37", new BurningBallMon());
            r.RegisterByKey("DX-38", new ComboMon(new CardsSSB.AnyOpponentTextMon("Thunder Spear", 20), new MagneticTackleMon()));
            r.RegisterByKey("DX-39", new MasquerainIntimidatingPattern());
            r.RegisterByKey("DX-40", new ComboMon(PsychicBoom(), new FlipBonusMon("Quick Blow", 20)));
            r.RegisterByKey("DX-41", new ComboMon(new SniffOutAnyMon(), new SparkMon("Negative Spark", AbilityDef.PokeBody)));
            r.RegisterByKey("DX-42", new NosepassMagneticReversal());
            r.RegisterByKey("DX-43", new ComboMon(new ConditionAttackMon("Eerie Light", SpecialCondition.Confused), new CardsRSB.CoinsPlusMon("One-Two Strike", 2, 20)));
            r.RegisterByKey("DX-44", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new SparkMon("Positive Spark", AbilityDef.PokePower)));
            r.RegisterByKey("DX-45", new ShelgonHardProtection());
            r.RegisterByKey("DX-46", new CardsDX.ShieldFromMon("Advanced Armor", CardsE2.IsEvolved, "Evolved Pokémon"));
            r.RegisterByKey("DX-47", new SolrockSunbeam());
            r.RegisterByKey("DX-48", new StarmieCoreGuard());
            r.RegisterByKey("DX-49", new ComboMon(new MidAirCrushMon(), new NosediveMon()));
            r.RegisterByKey("DX-50", new VigorothVigorousAura());
            r.RegisterByKey("DX-51", new ComboMon(new LiabilityMon(), new SmogscreenMon()));
            r.RegisterByKey("DX-52", new ComboMon(new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned), new CoinsMon("Doubleslap", 2)));
            r.RegisterByKey("DX-53", new CardsBS1.SelfdestructMon("Selfdestruct", 10, 50));
            r.RegisterByKey("DX-54", new CardsTEU1.AmnesiaMon("Amnesia"));
            r.RegisterByKey("DX-55", new FlipOverMon());
            r.RegisterByKey("DX-56", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("DX-57", new CardsFST1.TakeLessMon(10, new CoinsMon("Double Chop", 2)));
            r.RegisterByKey("DX-58", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("DX-59", new HighVoltageMon());
            r.RegisterByKey("DX-60", new ComboMon(new CardsRG.TypeFromDeckToSelfMon("Recharge", EnergyType.Lightning, 1), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("DX-61", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("DX-62", new ConditionAttackMon("Smog", SpecialCondition.Poisoned));
            r.RegisterByKey("DX-63", new FlipBonusMon("Aqua Wave", 10));
            r.RegisterByKey("DX-64", new ComboMon(new SwitchSelfMon("Leap Out", false), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("DX-65", new ComboMon(new ThrustMon(), new FlipBonusMon("Rising Lunge", 10)));
            r.RegisterByKey("DX-66", new CardsDX.MirrorCoatMon(new TailsNothingMon("Razor Wind")));
            r.RegisterByKey("DX-67", new CardsRGB.DigUnderMon());
            r.RegisterByKey("DX-68", new NumelDense());
            r.RegisterByKey("DX-69", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("DX-70", new CardsSS.SurpriseMon());
            r.RegisterByKey("DX-71", new ComboMon(new DrawAttackMon("Collect", 1), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("DX-72", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new FlipBonusMon("Trip Over", 10)));
            r.RegisterByKey("DX-73", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("DX-74", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("DX-75", new NoRetreatMon("Magma Ring"));
            r.RegisterByKey("DX-76", new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep));
            r.RegisterByKey("DX-77", new RapidSpinMon());
            r.RegisterByKey("DX-78", new DodgeMon("Agility", false));
            r.RegisterByKey("DX-79", new SwabluFluff());
            r.RegisterByKey("DX-80", new DodgeMon("Fly", true));
            r.RegisterByKey("DX-81", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("DX-82", new ComboMon(new CardsSIT1.AscensionMon(), new CardsSVI1.MiraclePowderMon("Miracle Essence")));
            r.RegisterByKey("DX-83", new CardsDX.ConditionImmuneMon(SpecialCondition.Paralyzed, null));
        }

        private static Mon PsychicBoom()
        {
            return new CardsASR1.PlusMon("Psychic Boom", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending));
        }

        /// <summary>Moonglow (Poké-Body): your Solrock have no Retreat Cost. Foresight: reorder the top 5 of either deck. Target Beam: 10 more per Solrock.</summary>
        private sealed class LunatoneMoonglow : Mon
        {
            private static readonly Mon Beam = new CardsASR1.PlusMon("Target Beam", 10, c => c.MyPokemon(m => m.Name == "Solrock").Count);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && mon.Name == "Solrock" ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foresight") return Beam.HandleAttack(ctx, atk);
                int pick = ctx.ChooseOption("Foresight: look at the top 5 cards of which deck?", "Your deck", "Your opponent's deck");
                CardsE2.ReorderTop(ctx, pick == 0 ? ctx.Player : ctx.Opponent, 5, "Foresight");
                return true;
            }
        }

        /// <summary>Burning Ball: with 2 or more Fire Energy on Magcargo, the Defending Pokémon is Burned.</summary>
        private sealed class BurningBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Ball") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool hot = me.Energy.Count(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire)) >= 2;
                ctx.DamageDefending(atk.Damage);
                if (hot && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Magnetic Tackle: you may do 10 more per Lightning Energy on Manectric; if you do, it does 10 damage to itself.</summary>
        private sealed class MagneticTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Tackle") return false;
                var me = ctx.Pokemon;
                int lightning = me.Energy.Count(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning));
                bool more = lightning > 0 && ctx.AskYesNo("Magnetic Tackle: do " + (10 * lightning) + " more damage and 10 damage to Manectric?");
                ctx.DamageDefending(atk.Damage + (more ? 10 * lightning : 0));
                if (more && ctx.InPlay(me)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Intimidating Pattern (Poké-Body): while Masquerain is your Active Pokémon, the opponent's attacks do 20 less (before W/R); only 1 applies. Stun Spore.</summary>
        private sealed class MasquerainIntimidatingPattern : Mon
        {
            private static readonly Mon Spore = new ParalyzeFlipMon("Stun Spore");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || !src.IsActive(me) || a == null || a.Owner == src.Player || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                var first = src.MyPokemon(m => src.IsActive(m) && src.Engine.HasAbilityNamed(m, "Intimidating Pattern")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != me) return damage;
                return Math.Max(0, damage - 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spore.HandleAttack(ctx, atk); }
        }

        /// <summary>Sniff Out: any 1 card from your discard pile into your hand.</summary>
        private sealed class SniffOutAnyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sniff Out") return false;
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Sniff Out: put which card into your hand?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Hand.Add(card);
                return true;
            }
        }

        /// <summary>Negative / Positive Spark: 20 to each of the opponent's Pokémon with a Poké-Body (Poké-Power); no W/R.</summary>
        private sealed class SparkMon : Mon
        {
            private readonly string attack;
            private readonly string kind;

            public SparkMon(string attack, string kind)
            {
                this.attack = attack;
                this.kind = kind;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                string wanted = kind;
                foreach (var mon in ctx.OpponentPokemon(m => m.Def.Abilities.Any(a => a.Kind == wanted)))
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Magnetic Reversal (Poké-Power): once during your turn, if Nosepass is Active, flip; heads, switch in 1 of the opponent's Benched Pokémon. Sharpen.</summary>
        private sealed class NosepassMagneticReversal : Mon
        {
            protected override string Activated { get { return "Magnetic Reversal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.GustOpponent("Magnetic Reversal: choose the new Defending Pokémon");
            }
        }

        /// <summary>Hard Protection (Poké-Body): no damage to Shelgon from your own Pokémon's attacks. Rock Smash.</summary>
        private sealed class ShelgonHardProtection : Mon
        {
            private static readonly Mon Smash = new FlipBonusMon("Rock Smash", 10);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner != src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smash.HandleAttack(ctx, atk); }
        }

        /// <summary>Sunbeam (Poké-Body): your Lunatone's maximum HP is 80. Scorching Light. Reflected Beam: 10 per Lunatone to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class SolrockSunbeam : Mon
        {
            private static readonly Mon Light = new CardsSI.HeadsTailsConditionMon("Scorching Light", SpecialCondition.Paralyzed, SpecialCondition.Burned);

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && mon.Name == "Lunatone" ? 80 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflected Beam") return Light.HandleAttack(ctx, atk);
                int n = ctx.MyPokemon(m => m.Name == "Lunatone").Count;
                if (n > 0 && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10 * n);
                return true;
            }
        }

        /// <summary>Core Guard (Poké-Body): with any Psychic Energy attached, attack damage to Starmie is 10 less (after W/R). Energy Flip. Psychic Boom.</summary>
        private sealed class StarmieCoreGuard : Mon
        {
            private static readonly Mon Boom = PsychicBoom();

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return me.Energy.Any(e => CardsHIF1.Provides(src, me, e, EnergyType.Psychic)) ? damage - 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Flip") return Boom.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var hit = ctx.DamageOneBenched(10);
                if (hit == null || !ctx.InPlay(hit) || hit.Energy.Count == 0 || !ctx.CanAffect(hit)) return true;
                var others = ctx.OpponentPokemon(m => m != hit);
                if (others.Count == 0 || !ctx.AskYesNo("Energy Flip: move an Energy from " + hit.Name + " to another of your opponent's Pokémon?")) return true;
                var card = hit.Energy.Count == 1 ? hit.Energy[0] : ctx.ChooseCards(hit.Energy.ToList(), 1, 1, "Energy Flip: move which Energy?")[0];
                var to = ctx.ChoosePokemon(others, "Energy Flip: move it to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(hit, to, card);
                return true;
            }
        }

        /// <summary>Mid-air Crush: 20 to 1 of the opponent's Pokémon; heads, the opponent discards an Energy card from it.</summary>
        private sealed class MidAirCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mid-air Crush") return false;
                var hit = ctx.DamageAnyOpponent(20);
                if (!ctx.Flip() || hit == null || !ctx.InPlay(hit) || hit.Energy.Count == 0 || !ctx.CanAffect(hit)) return true;
                var pick = ctx.Engine.ChooseCards(ctx.Opponent, hit.Energy.ToList(), 1, 1, "Mid-air Crush: discard an Energy card from " + hit.Name, ctx.Source);
                if (pick.Count > 0) ctx.Engine.DiscardEnergy(hit, pick[0]);
                return true;
            }
        }

        /// <summary>Nosedive: tails, Swellow does 10 damage to itself.</summary>
        private sealed class NosediveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nosedive") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Vigorous Aura (Poké-Body): while Vigoroth is your Active Pokémon, each player's Active Pokémon's attacks do 10 more to the Active Pokémon (before W/R). Scratch.</summary>
        private sealed class VigorothVigorousAura : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || !src.IsActive(me) || a == null || !src.IsActive(a) || info.Target == null || info.Target.Owner == a.Owner || !info.TargetIsActive) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 10;
            }
        }

        /// <summary>Liability: damage counters on the Defending Pokémon until it has 10 HP left; Weezing does 70 damage to itself.</summary>
        private sealed class LiabilityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Liability") return false;
                var defending = ctx.Defending;
                if (defending != null)
                {
                    int n = (ctx.RemainingHp(defending) - 10) / 10;
                    if (n > 0) ctx.PlaceCounters(defending, n);
                }
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(70);
                return true;
            }
        }

        /// <summary>Smogscreen: the Defending Pokémon is Poisoned and must flip to attack during the opponent's next turn.</summary>
        private sealed class SmogscreenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smogscreen") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsMEG.SandAttackEffect(defending.Name + " must flip a coin to attack during its next turn (Smogscreen)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Flip Over: Beldum does 10 damage to itself (no W/R).</summary>
        private sealed class FlipOverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flip Over") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>High Voltage: heads, the opponent can't play Trainer cards during their next turn. Gnaw.</summary>
        private sealed class HighVoltageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Voltage") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsCES3.TrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (High Voltage)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Thrust: tails, nothing; heads, the damage and an Energy card discarded from the Defending Pokémon.</summary>
        private sealed class ThrustMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thrust") return false;
                var defending = ctx.Defending;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Thrust: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Dense (Poké-Body): attack damage from any Evolved Pokémon (both players') is 20 less (after W/R). Ram.</summary>
        private sealed class NumelDense : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return CardsE2.IsEvolved(a) ? damage - 20 : damage;
            }
        }

        /// <summary>Rapid Spin: the opponent switches the Defending Pokémon with a Benched Pokémon, then you switch Staryu with 1 of your Benched Pokémon.</summary>
        private sealed class RapidSpinMon : Mon
        {
            private static readonly Mon Theirs = new CardsE3B.OpponentSwitchesMon("Rapid Spin");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!Theirs.HandleAttack(ctx, atk)) return false;
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Fluff (Poké-Body): with a damage counter on it, the first time an opponent's attack would affect Swablu, flip; heads, nothing happens to it. Peck.</summary>
        private sealed class SwabluFluff : Mon
        {
            public override string FlipsToDodgeAttack(EffectContext src, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || src.IsMyTurn || target.DamageCounters < 1) return null;
                return src.Engine.CurrentSourceOwner == src.Player ? null : "Fluff";
            }
        }
    }
}
