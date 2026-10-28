using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Hidden Legends Uncommon and Common Pokémon (HL 28-83; v161). Unlimited only. Beldum 29 is the Nintendo Black Star Promo 22 (a print).</summary>
    public static class CardsHLB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HL-28", new LevitateAnyMon(true));
            r.RegisterByKey("HL-30", new CardsHL.CastformMon("Castform", new ComboMon(new ForecastMon(), CardsSS.ConditionBonus("Extra Ball", 20, d => d.Def.IsOldEx))));
            r.RegisterByKey("HL-31", new HyperBeamFlipMon());
            r.RegisterByKey("HL-32", new CoralGlowMon());
            r.RegisterByKey("HL-33", new DodrioFastFeet());
            r.RegisterByKey("HL-34", new GlalieIceWall());
            r.RegisterByKey("HL-35", new ComboMon(new CardsDAA1.DrainMon("Cling"), new CoinsMon("Double Razor Leaf", 2)));
            r.RegisterByKey("HL-36", new ConditionAttackMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("HL-37", new CardsSS.BabyEvolutionMon("Jigglypuff", new SetSongMon()));
            r.RegisterByKey("HL-38", new LanturnEnergyGrounding());
            r.RegisterByKey("HL-39", new CardsPAR2.FlipConditionMon("Cracking Voice", SpecialCondition.Confused));
            r.RegisterByKey("HL-40", new ComboMon(new LovingDrawMon(), new SweetTemptationMon()));
            r.RegisterByKey("HL-41", new CardsASC.CounterattackMon("Strikes Back", 1, null));
            r.RegisterByKey("HL-42", new ComboMon(new FlipBonusMon("Steady Punch", 10), new PowerHealMon()));
            r.RegisterByKey("HL-43", new ComboMon(new ParalyzeFlipMon("Psyshock"), new CountersMon("Metal Ball", 5)));
            r.RegisterByKey("HL-44", new LevitateAnyMon(true));
            r.RegisterByKey("HL-45", new ComboMon(new CardsTEU1.AmnesiaMon("Upper Hand"), new CardsCES1.FeintAttackMon("Feint Attack", 40)));
            r.RegisterByKey("HL-46", new RhydonPowerDiffusion());
            r.RegisterByKey("HL-47", new SuperIcyWindMon());
            r.RegisterByKey("HL-48", new ComboMon(new HulaLaMon(), new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("HL-49", new ComboMon(new CardsSSB.AnyOpponentTextMon("Water Arrow", 20),
                new CardsASR1.PlusMon("Rainbow Star", 10, c => c.Defending == null ? 0 : c.Defending.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count())));
            r.RegisterByKey("HL-50", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new SwallowUpMon()));
            r.RegisterByKey("HL-51", new ComboMon(new LongTentacleMon(), new CardsBS1.ToxicMon("Toxic", 2)));
            r.RegisterByKey("HL-52", new ParalyzeFlipMon("Psyshock"));
            r.RegisterByKey("HL-53", new PlainMon());
            r.RegisterByKey("HL-54", new ComboMon(new CardsMA.CallForFamilyOneMon(d => true), new CountersMon("Metal Ball", 1)));
            r.RegisterByKey("HL-55", new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("HL-56", new CardsFST1.TwoBenchedMon("Spark", 10));
            r.RegisterByKey("HL-57", new CardsSSB.AnyOpponentTextMon("Random Spark", 10));
            r.RegisterByKey("HL-58", new CardsFST1.TakeLessMon(10, new ParalyzeFlipMon("Bubble")));
            r.RegisterByKey("HL-59", new ComboMon(new CardsMA.SwiftMon("Swift"), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("HL-60", new PlainMon());
            r.RegisterByKey("HL-61", new AscensionMon());
            r.RegisterByKey("HL-62", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("HL-63", new ConditionAttackMon("Sleepy Song", SpecialCondition.Asleep));
            r.RegisterByKey("HL-64", new CardsBS1.KarateChopMon());
            r.RegisterByKey("HL-65", new ComboMon(new CoinsMon("Double Chop", 2), new CardsSS.SpiralDrainMon("Calm Mind")));
            r.RegisterByKey("HL-66", new FlipBonusMon("Steady Punch", 10));
            r.RegisterByKey("HL-67", new ComboMon(new MinusEnergyMon(), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("HL-68", new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("HL-69", new ComboMon(new PlusEnergyFlipMon(), new CardsPAR2.FlipConditionMon("Dazzle Blast", SpecialCondition.Confused)));
            r.RegisterByKey("HL-70", new ComboMon(new CoinsMon("Double Stab", 2), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("HL-71", new ComboMon(new CardsMAB.AttachToSelfFromHandMon("Growth Spurt", d => CardsRS.BasicOf(d, EnergyType.Grass)), new FlipBonusMon("Trip Over", 10)));
            r.RegisterByKey("HL-72", new ShadowCrushMon());
            r.RegisterByKey("HL-73", new ParalyzeFlipMon("Ice Breath"));
            r.RegisterByKey("HL-74", new CardsSI.HeadsTailsConditionMon("Freezing Breath", SpecialCondition.Paralyzed, SpecialCondition.Asleep));
            r.RegisterByKey("HL-75", new CoreGuardMon());
            r.RegisterByKey("HL-76", new CardsFST2.ConditionBonusMon("Splash About", 10, c => c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending)));
            r.RegisterByKey("HL-77", new FreefloatingMon(new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("HL-78", new ConditionAttackMon("Sleepy Song", SpecialCondition.Asleep));
            r.RegisterByKey("HL-79", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("HL-80", new RechargeMon());
            r.RegisterByKey("HL-81", new AscensionMon());
            r.RegisterByKey("HL-82", new CardsE3B.OpponentSwitchesMon("Wail"));
            r.RegisterByKey("HL-83", new ComboMon(new CoinsMon("Quick Turn", 2), new CardsSS.SurpriseMon()));
        }

        /// <summary>"Put N damage counters on the Defending Pokémon" (Metal Ball).</summary>
        private sealed class CountersMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public CountersMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, counters);
                return true;
            }
        }

        /// <summary>Levitate (Poké-Body): with any Energy attached, this Pokémon's Retreat Cost is 0.</summary>
        private sealed class LevitateAnyMon : Mon
        {
            private readonly bool anyEnergy;

            public LevitateAnyMon(bool anyEnergy) { this.anyEnergy = anyEnergy; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return mon.Energy.Any(e => anyEnergy || e.Def.IsBasicEnergy) ? 0 : cost;
            }
        }

        /// <summary>Freefloating (Poké-Body): with no Energy attached, Tentacool's Retreat Cost is 0.</summary>
        private sealed class FreefloatingMon : Mon
        {
            private readonly Mon attacks;

            public FreefloatingMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Forecast: search your deck for a Stadium card and play it.</summary>
        private sealed class ForecastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Forecast") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                string inPlay = ctx.State.Stadium == null ? null : ctx.State.Stadium.Def.Name;
                var found = ctx.SearchMyDeck(d => d.IsStadiumAlone && d.Name != inPlay, 1, "Forecast: choose a Stadium card to play");
                ctx.ShuffleMyDeck();
                foreach (var card in found)
                {
                    ctx.Me.Deck.Remove(card);
                    ctx.Engine.PutStadiumIntoPlay(ctx.Player, card);
                }
                return true;
            }
        }

        /// <summary>Hyper Beam: flip a coin; heads, discard an Energy attached to the Defending Pokémon.</summary>
        private sealed class HyperBeamFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Hyper Beam: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Coral Glow: draw up to as many cards as the opponent has Basic Pokémon in play, no more than 10 in your hand.</summary>
        private sealed class CoralGlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coral Glow") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(ctx.OpponentPokemon(m => m.Def.IsBasicPokemon).Count, Math.Min(ctx.Me.Deck.Count, 10 - ctx.Me.Hand.Count));
                if (n > 0) ctx.Draw(n == 1 ? 1 : ctx.AskNumber(0, n, "Coral Glow: draw how many cards (up to " + n + ")?"));
                return true;
            }
        }

        /// <summary>Fast Feet (Poké-Body): Dodrio can retreat while Asleep or Paralyzed. Retaliate: 10 per damage counter on it. Slashing Strike: tails, not next turn.</summary>
        private sealed class DodrioFastFeet : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASR1.TimesMon("Retaliate", c => c.Pokemon.DamageCounters), new SlashingStrikeFlipMon());

            public override bool AllowsRetreatWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Slashing Strike: flip a coin; tails, Dodrio can't use Slashing Strike during your next turn.</summary>
        private sealed class SlashingStrikeFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slashing Strike") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Ice Wall (Poké-Body): the opponent's Pokémon with Special Energy attached do 40 less damage to Glalie with attacks (after
        /// Weakness and Resistance). Heavy Blizzard: heads, 1 damage counter on each of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class GlalieIceWall : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.Energy.Any(e => !e.Def.IsBasicEnergy) ? Math.Max(0, damage - 40) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Blizzard") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) foreach (var mon in ctx.OpponentBench()) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>Set Song: a Basic Pokémon and a basic Energy card from your deck into your hand.</summary>
        private sealed class SetSongMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Set Song") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicPokemon, 1, "Set Song: choose a Basic Pokémon");
                found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Set Song: choose a basic Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Energy Grounding (Poké-Power): once during the opponent's turn, when 1 of your Pokémon is Knocked Out by their attack, you
        /// may attach a basic Energy card discarded from it to Lanturn (only 1 Energy Grounding each turn). Lightning Strike: you may
        /// discard all Lightning Energy for 90 instead of 50.
        /// </summary>
        private sealed class LanturnEnergyGrounding : Mon
        {
            private const string Power = "Energy Grounding";

            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon == me || !byOpponentsAttack || src.IsMyTurn || src.NamedLimitUsedThisTurn(Power)) return;
                var basics = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0 || !src.InPlay(me)) return;
                if (!src.AskYesNo(Power + ": attach a basic Energy card from " + mon.Name + " to " + me.Name + "?")) return;
                src.MarkNamedLimit(Power);
                Announce(src, Power);
                var card = basics.Select(c => c.Def.Key).Distinct().Count() == 1 ? basics[0] : src.ChooseCards(basics, 1, 1, Power + ": attach which basic Energy card?")[0];
                src.Engine.MoveEnergy(mon, me, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Strike") return false;
                var me = ctx.Pokemon;
                int damage = atk.Damage;
                var cards = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList();
                if (!CardsBS1.CopyingWithMetronome && cards.Count > 0 && ctx.AskYesNo("Lightning Strike: discard all Lightning Energy from " + me.Name + " for 90 damage?"))
                {
                    int gone = 0;
                    foreach (var card in cards) if (ctx.Engine.DiscardEnergy(me, card)) gone++;
                    if (gone > 0) damage = 90;
                }
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Loving Draw: draw until you have as many cards in your hand as the opponent.</summary>
        private sealed class LovingDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Loving Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.DrawUntil(ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Sweet Temptation: switch in 1 of the opponent's Benched Pokémon and do 10 to the new Defending Pokémon (nothing without a Bench, as Luring Flame).</summary>
        private sealed class SweetTemptationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Temptation") return false;
                if (ctx.GustTargets().Count == 0) return Nothing(ctx);
                ctx.GustOpponent("Sweet Temptation: choose the new Defending Pokémon");
                ctx.DamageDefending(Math.Max(atk.Damage, 10));
                return true;
            }
        }

        /// <summary>Power Heal: 10 more per damage counter on Medicham, then 1 damage counter comes off it.</summary>
        private sealed class PowerHealMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Heal") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Power Diffusion (Poké-Body): while Rhydon is your Active Pokémon, prevent all damage done by attacks to your Benched Pokémon. Hyper Tail: 20 more against a Pokémon with a Poké-Power or Poké-Body.</summary>
        private sealed class RhydonPowerDiffusion : Mon
        {
            private static readonly Mon Tail = CardsSS.ConditionBonus("Hyper Tail", 20, d => d.Def.Abilities.Any(a => a.IsPokePowerOrBody));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var t = info.Target;
                if (me == null || t == null || t.Owner != src.Player || t == me || !src.IsActive(me) || src.IsActive(t)) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && info.Attacker != null ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tail.HandleAttack(ctx, atk); }
        }

        /// <summary>Super Icy Wind: 10 damage to each of the opponent's Pokémon, not affected by Weakness or Resistance.</summary>
        private sealed class SuperIcyWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Icy Wind") return false;
                foreach (var mon in ctx.OpponentPokemon()) if (ctx.InPlay(mon)) ctx.DamageWith(mon, 10, false, false, false);
                return true;
            }
        }

        /// <summary>The Hula-la: heads, the Defending Pokémon is Confused; tails, both it and Spinda are Confused.</summary>
        private sealed class HulaLaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "The Hula-la") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Swallow Up: 30 more if the Defending Pokémon has less HP left than Swalot (counted before the damage).</summary>
        private sealed class SwallowUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swallow Up") return false;
                var defending = ctx.Defending;
                bool fewer = defending != null && ctx.RemainingHp(defending) < ctx.RemainingHp(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage + (fewer ? 30 : 0));
                return true;
            }
        }

        /// <summary>Long Tentacle: heads, Confused; tails, discard an Energy card attached to the Defending Pokémon.</summary>
        private sealed class LongTentacleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Long Tentacle") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                else if (defending.Energy.Count > 0 && ctx.CanAffect(defending)) ctx.DiscardEnergyFrom(defending, 1, "Long Tentacle: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Ascension: a card that evolves from this Pokémon, from your deck onto it (no timing limits, like Wally's Training).</summary>
        private sealed class AscensionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ascension") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsEVS1.EvolveFromDeck(ctx, ctx.Pokemon, "Ascension");
                return true;
            }
        }

        /// <summary>Minus Energy: flip a coin; heads, the opponent returns an Energy card you choose from the Defending Pokémon to their hand.</summary>
        private sealed class MinusEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Minus Energy") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Minus Energy: return which Energy card to your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, card);
                return true;
            }
        }

        /// <summary>Plus Energy: flip a coin; heads, attach a Lightning Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class PlusEnergyFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plus Energy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Hand.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (cards.Count > 0 && ctx.Flip()) ctx.AttachCardsAnyWay(cards, 1, (c, m) => true, "Plus Energy: attach a Lightning Energy card", false);
                return true;
            }
        }

        /// <summary>Shadow Crush: you may discard a Psychic Energy card from Shuppet; if you do, the opponent discards an Energy card from the Defending Pokémon.</summary>
        private sealed class ShadowCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Crush") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                var psychic = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Psychic)).ToList();
                if (psychic.Count == 0 || !ctx.AskYesNo("Shadow Crush: discard a Psychic Energy card so your opponent discards an Energy card from " + defending.Name + "?")) return true;
                var card = psychic.Count == 1 ? psychic[0] : ctx.ChooseCards(psychic, 1, 1, "Shadow Crush: discard which Psychic Energy card?")[0];
                if (!ctx.Engine.DiscardEnergy(me, card) || !ctx.CanAffect(defending)) return true;
                var pick = ctx.Engine.ChooseCards(ctx.Opponent, defending.Energy.ToList(), 1, 1, "Shadow Crush: discard an Energy card from " + defending.Name, ctx.Source);
                if (pick.Count > 0) ctx.Engine.DiscardEnergy(defending, pick[0]);
                return true;
            }
        }

        /// <summary>Core Guard (Poké-Body): with any Psychic Energy attached, attacks do 10 less damage to Staryu (after Weakness and Resistance).</summary>
        private sealed class CoreGuardMon : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return me.Energy.Any(e => CardsHIF1.Provides(src, me, e, EnergyType.Psychic)) ? Math.Max(0, damage - 10) : damage;
            }
        }

        /// <summary>Recharge: a Lightning Energy card from your deck onto Voltorb.</summary>
        private sealed class RechargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recharge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                foreach (var card in ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Lightning), 1, "Recharge: choose a Lightning Energy card"))
                    if (!ctx.InPlay(me) || !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }
    }
}
