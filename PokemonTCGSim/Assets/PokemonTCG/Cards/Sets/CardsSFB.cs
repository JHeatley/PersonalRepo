using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>Stormfront: Uncommons, Commons, Trainers and the SH cards (Rares and LV.X are in CardsSF.cs).</summary>
    public static partial class CardsSF
    {
        private static bool AnyTrainerKind(CardDef d) { return d.IsTrainer; }

        private static void RegisterUncommonsAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("SF-12", new AbomasnowSnowVeil());
            r.RegisterByKey("SF-13", new BronzongCycler());
            r.RegisterByKey("SF-14", new CherrimSunnyDay());
            r.RegisterByKey("SF-15", new ComboMon(new ScorpionGrappleMon(), new CrossPoisonMon(), new DerailMon()));
            r.RegisterByKey("SF-16", new ComboMon(new DeliveryAllMon(), new LiftingMon(), new ConditionNoRetreatMon("Ominous Wind", SpecialCondition.Confused, true)));
            r.RegisterByKey("SF-17", new DusknoirSpiritPulse());
            r.RegisterByKey("SF-18", new GengarFaintingSpell());
            r.RegisterByKey("SF-19", new ComboMon(new CardsASR1.TimesMon("Tail Revenge", c => c.Me.Discard.Count(x => x.Def.IsPokemon && x.Def.Name == "Magikarp")), new WreakHavocMon(), new DragonBeatMon()));
            r.RegisterByKey("SF-20", new ComboMon(new TakeOutMon(), new CoinsMon("Hurricane Punch", 4), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("SF-21", new ComboMon(new RammingStrikeMon(), new ParadeMon()));
            r.RegisterByKey("SF-22", new RapidashBurningMane());
            r.RegisterByKey("SF-23", new RoseradeHiddenPoison());
            r.RegisterByKey("SF-24", new SalamenceBattleRush());
            r.RegisterByKey("SF-25", new ScizorHoneycombDefender());
            r.RegisterByKey("SF-26", new CardsDS.EvolutionaryMon("Evolutionary Gas", EvolutionaryGas, c => c.PokemonIsActive && c.Opp.Active != null,
                new ComboMon(new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned), new PlunderMon())));
            r.RegisterByKey("SF-27", new StaraptorProtectWing());
            r.RegisterByKey("SF-28", new ComboMon(new OutbreakPowerMon(), new CardsPFL.FlipUntilTailsTimesMon("Iron Tail")));
            r.RegisterByKey("SF-29", new CardsSS.RainDishMon("Green Renewal", new ComboMon(new GreenAcidMon(), new CardsFST1.TwoBenchedMon("Reaching Vine", 20))));
            r.RegisterByKey("SF-30", new TyranitarDarknessDrive());
            r.RegisterByKey("SF-31", new VespiquenGreenDignity());
            r.RegisterByKey("SF-32", new BibarelUnaware());
            r.RegisterByKey("SF-33", new BudewPoisonEnzyme());
            r.RegisterByKey("SF-34", new ComboMon(new DarkOneEyeMon(), new FlipBonusMon("Ambush", 20)));
            r.RegisterByKey("SF-35", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new TrickRoomMon()));
            r.RegisterByKey("SF-36", new ElectrodeRadiance());
            r.RegisterByKey("SF-37", new ComboMon(new CardsMEG.SandAttackMon("Flash"), new ElectroDiffusionMon()));
            r.RegisterByKey("SF-38", new ComboMon(new SearchAttackMon("Go and Collect", AnyTrainerKind, 1), new CardsDEX.FuryCutterMon(40, 20, 10)));
            r.RegisterByKey("SF-39", new ComboMon(new ConditionAttackMon("Tail Shake", SpecialCondition.Asleep), new CoinsMon("Slam", 2)));
            r.RegisterByKey("SF-40", new ComboMon(new ConditionAttackMon("Smog", SpecialCondition.Poisoned), new HoodwinkMon()));
            r.RegisterByKey("SF-41", new ComboMon(new FlipBonusMon("Steady Punch", 20), new BrickBreakMon()));
            r.RegisterByKey("SF-42", new ComboMon(new MagneticResonanceMon(), new CardsASR1.PlusMon("Magnetic Release", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("SF-43", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new CardsASC.FlipDiscardDefendingEnergyMon("Removal Pulse")));
            r.RegisterByKey("SF-44", new ComboMon(new DrawAttackMon("Collect", 1), new EnergyMilkMon(), new FlipBonusMon("Stomp", 20)));
            r.RegisterByKey("SF-45", new CardsSS.BabyEvolutionMon("Pikachu", new ElectricCircuitMon()));
            r.RegisterByKey("SF-46", new ComboMon(new RouseMon(), new Cards30CC.BenchHitMon("Overrun", 20)));
            r.RegisterByKey("SF-47", new PupitarEnergyProtection());
            r.RegisterByKey("SF-48", new ComboMon(new ImpersonateMon(), new OverconfidentMon()));
            r.RegisterByKey("SF-49", new SwordsDanceMon("Slashing Strike", 60));
            r.RegisterByKey("SF-50", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("SF-51", new ComboMon(new FlipBonusMon("Quick Attack", 20), new FlipGateMon("Mach Blade", new MachBladeMon())));
            r.RegisterByKey("SF-52", new Cards30CC.BenchHitMon("Shot Air", 30));
            r.RegisterByKey("SF-53", new CardsEM.ScaryFaceMon());
            r.RegisterByKey("SF-54", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new SelfAbandonmentMon()));
            r.RegisterByKey("SF-55", new ComboMon(new GyroSwapMon(), new ParalyzeFlipMon("Psyshock")));
            r.RegisterByKey("SF-56", new ComboMon(new CardsE3B.HealSelfMon("Nap", 20), new CoinsMon("Bullet Seed", 4)));
            r.RegisterByKey("SF-57", new AlertMon());
            r.RegisterByKey("SF-58", new ComboMon(new ParalyzeFlipMon("Constrict"), new CardsSSB.AnyOpponentTextMon("Linear Attack", 10)));
            r.RegisterByKey("SF-59", new SilhouetteMon());
            r.RegisterByKey("SF-60", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("SF-61", new ComboMon(new CardsMA.BenchOnlyMon("Aqua Liner", 10), new FlipBonusMon("Mouth Pump", 10)));
            r.RegisterByKey("SF-62", new ComboMon(new PitchDarkMon(), new SwitchSelfMon("Trick Gas", true)));
            r.RegisterByKey("SF-63", new CardsFST1.TwoBenchedMon("Rock Slide", 10));
            r.RegisterByKey("SF-64", new CardsDRI1.WhirlwindMon("Knock Back"));
            r.RegisterByKey("SF-65", new CardsSP1.FlipUntilTailsDrawMon("Sea Spray"));
            r.RegisterByKey("SF-66", new CardsGRI2.FlipBonusOrRecoilMon("Magnetic Bomb", 10, 10));
            r.RegisterByKey("SF-67", new CardsSSB.AnyOpponentTextMon("Random Spark", 20));
            r.RegisterByKey("SF-68", new ComboMon(new ConditionAttackMon("Lullaby", SpecialCondition.Asleep), new NightmareFeastMon()));
            r.RegisterByKey("SF-69", new ComboMon(new HardenMon("Harden", 40), new ParalyzeFlipMon("Bind")));
            r.RegisterByKey("SF-70", new SpeedBoltMon());
            r.RegisterByKey("SF-71", new DodgeMon("Agility", false));
            r.RegisterByKey("SF-72", new PetalSpikesMon());
            r.RegisterByKey("SF-73", new ComboMon(new NoRetreatMon("Stalk"), new CardsSI.HeadsTailsConditionMon("Dangerous Claw", SpecialCondition.Paralyzed, SpecialCondition.Poisoned)));
            r.RegisterByKey("SF-74", new ComboMon(new DodgeMon("Hide", false), new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep)));
            r.RegisterByKey("SF-75", new ComboMon(new CardsMEG.SandAttackMon("Sand Attack"), new CoinsMon("Double Stab", 2)));
            r.RegisterByKey("SF-76", new ComboMon(new SwitchSelfMon("Gnaw and Run", false), new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("SF-77", new ComboMon(new CardsSI.HeadsTailsConditionMon("Freezing Breath", SpecialCondition.Paralyzed, SpecialCondition.Asleep), new CardsXYP1.FlipTailsRecoilMon("Take Down", 10)));
            r.RegisterByKey("SF-78", new ComboMon(new IngrainMon(), new ParalyzeFlipMon("Tickle")));
            r.RegisterByKey("SF-79", new ComboMon(new SearchAttackMon("Call for Friends", d => d.IsBasicPokemon && !d.IsLevelUp, 1), new CardsPAR2.FlipConditionMon("Poison Breath", SpecialCondition.Poisoned)));
            r.RegisterByKey("SF-80", new ComboMon(new CardsCES2.ScreechMon("Screech", 20), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("SF-81", new ComboMon(new SearchAttackMon("Outlet", d => CardsRS.BasicOf(d, EnergyType.Lightning), 1), new CardsXYP1.FlipTailsRecoilMon("Bouncing Ball", 10)));

            r.RegisterByKey("SF-82", new ConductiveQuarry());
            r.RegisterByKey("SF-83", new EnergyLink());
            r.RegisterByKey("SF-86", new LuxuryBall());
            r.RegisterByKey("SF-87", new MarleysRequest());
            r.RegisterByKey("SF-88", new PlusItem(BlowerOne, BlowerTwo));
            r.RegisterByKey("SF-89", new PlusItem(DrawerOne, DrawerTwo));
            r.RegisterByKey("SF-90", new PlusItem(HealerOne, HealerTwo));

            r.RegisterByKey("SF-SH1", new DrifloonUnburden());
            r.RegisterByKey("SF-SH2", new ComboMon(new CountingSongMon(), new NightBindMon()));
            r.RegisterByKey("SF-SH3", new ComboMon(new FastballMon(), new CardsOBF1.AttachFromDiscardToSelfMon("Charge Beam", EnergyType.Lightning), new ParalyzeFlipMon("Thundershock")));
        }

        // ------------------------------------------------------------------ Uncommons

        /// <summary>Snow Veil (Poké-Body): while Abomasnow is Active, attack damage to your Pokémon is 20 less (after W/R). Snow Play, Below Zero.</summary>
        private sealed class AbomasnowSnowVeil : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target == null || info.Target.Owner != src.Player || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Snow Play")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var m in ctx.OpponentBench().Where(b => !ctx.Engine.IsType(b, EnergyType.Grass) && !ctx.Engine.IsType(b, EnergyType.Water)).ToList()) ctx.DamageTarget(m, 20);
                    return true;
                }
                if (atk.Name != "Below Zero") return false;
                var defending = ctx.Defending;
                bool fresh = ctx.Pokemon.TurnEvolved == ctx.Turn && EvolvedFrom(ctx, "Snover");
                ctx.DamageDefending(atk.Damage);
                if (fresh && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Cycler (Poké-Power): once during your turn, put a card from your hand on top of your deck, then up to 2 basic Energy cards from your deck into your hand. Not with a Special Condition. Strange Spin, Heavy Potential.</summary>
        private sealed class BronzongCycler : Mon
        {
            protected override string Activated { get { return "Cycler"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Cycler: put which card from your hand on top of your deck?")[0];
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Insert(0, card);
                ctx.SearchToHand(d => d.IsBasicEnergy, 2, "Cycler: choose up to 2 basic Energy cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Strange Spin")
                {
                    var defending = ctx.Defending;
                    bool same = ctx.Me.Hand.Count == ctx.Opp.Hand.Count;
                    ctx.DamageDefending(atk.Damage + (same ? 40 : 0));
                    if (same && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name != "Heavy Potential") return false;
                foreach (var m in ctx.OpponentPokemon())
                {
                    int n = ctx.Engine.RetreatCostOf(m);
                    if (n > 0 && ctx.CanAffect(m)) ctx.PlaceCounters(m, n);
                }
                return true;
            }
        }

        /// <summary>Sunny Day (Poké-Body): your Grass and Fire Pokémon's attacks do 10 more to the Defending Pokémon (before W/R). Salty-sweet Pollen.</summary>
        private sealed class CherrimSunnyDay : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(a, EnergyType.Grass) || src.Engine.IsType(a, EnergyType.Fire) ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Salty-sweet Pollen") return false;
                ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var target = hurt.Count == 0 ? null : ctx.ChoosePokemon(hurt, "Salty-sweet Pollen: remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Scorpion Grapple: heads, Paralyzed; tails, the Defending Pokémon can't retreat next turn.</summary>
        private sealed class ScorpionGrappleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorpion Grapple") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (ctx.Flip()) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                else ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Cross Poison: 4 coins, 20 times the heads; 2 or more heads, Poisoned.</summary>
        private sealed class CrossPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross Poison") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Derail: discard all Special Energy cards from the Defending Pokémon.</summary>
        private sealed class DerailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Derail") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    foreach (var e in defending.Energy.Where(x => x.Def.IsSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(defending, e);
                return true;
            }
        }

        /// <summary>Delivery: any 1 card from your discard pile into your hand.</summary>
        private sealed class DeliveryAllMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delivery") return false;
                if (ctx.Me.Discard.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Delivery: put which card into your hand?"));
                return true;
            }
        }

        /// <summary>Lifting: up to 2 Basic Pokémon from your deck onto your Bench; each may get a basic Energy card from your deck.</summary>
        private sealed class LiftingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lifting") return false;
                var before = ctx.Me.Bench.ToList();
                ctx.SearchToBench(d => !d.IsLevelUp, 2, "Lifting: choose up to 2 Basic Pokémon");
                foreach (var mon in ctx.Me.Bench.Where(m => !before.Contains(m)).ToList())
                {
                    if (ctx.Me.Deck.Count == 0 || !ctx.AskYesNo("Lifting: attach a basic Energy card from your deck to " + mon.Name + "?")) continue;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Lifting: choose a basic Energy card");
                    if (found.Count > 0 && !ctx.Engine.AttachEnergyCard(found[0], mon)) ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Spirit Pulse (Poké-Body): while Dusknoir is Active, 1 damage counter between turns on each of the opponent's Pokémon with Energy attached. Darkness Mist.</summary>
        private sealed class DusknoirSpiritPulse : Mon
        {
            private static readonly Mon Mist = new CardsFST2.ConditionBonusMon("Darkness Mist", 20, c => c.Defending != null && c.Defending.DamageCounters >= 2);

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? src.Opponent : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon)) return;
                var targets = src.OpponentPokemon(m => m.Energy.Count > 0);
                if (targets.Count == 0) return;
                Announce(src, "Spirit Pulse");
                foreach (var m in targets) src.Engine.PlaceDamageCounters(m, 1, "Spirit Pulse");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Mist.HandleAttack(ctx, atk); }
        }

        /// <summary>Fainting Spell (Poké-Power): once during the opponent's turn, if Gengar is Knocked Out by attack damage, flip: heads, the Attacking Pokémon is Knocked Out. Shadow Room, Poltergeist.</summary>
        private sealed class GengarFaintingSpell : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (me == null || !byOpponentsAttack || src.IsMyTurn || !CardsDS.PowerReady(src)) return;
                var attacker = src.Engine.AttackingPokemonOf(me);
                if (attacker == null || attacker.Owner == src.Player || !src.AskYesNo("Fainting Spell: flip a coin to Knock Out " + attacker.Name + "?")) return;
                Announce(src, "Fainting Spell");
                if (src.Flip()) src.Engine.KnockOut(attacker, "Fainting Spell");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Shadow Room")
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Shadow Room: put damage counters on which Pokémon?");
                    if (target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, target.Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower) ? 6 : 3);
                    return true;
                }
                if (atk.Name != "Poltergeist") return false;
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Poltergeist", "Poltergeist: your opponent's hand", ctx.Source);
                ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count(c => c.Def.IsTrainer));
                return true;
            }
        }

        /// <summary>Wreak Havoc: flip until tails; for each heads, discard the top card of the opponent's deck.</summary>
        private sealed class WreakHavocMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wreak Havoc") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        /// <summary>Dragon Beat: heads, discard an Energy card from each of the opponent's Pokémon.</summary>
        private sealed class DragonBeatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Beat") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                foreach (var m in ctx.OpponentPokemon(p => p.Energy.Count > 0 && ctx.CanAffect(p))) ctx.DiscardEnergyFrom(m, 1, "Dragon Beat: discard an Energy card from " + m.Name);
                return true;
            }
        }

        /// <summary>Take Out: a non-Evolved Defending Pokémon is Knocked Out instead of damaged.</summary>
        private sealed class TakeOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Take Out") return false;
                var defending = ctx.Defending;
                if (defending != null && !CardsE2.IsEvolved(defending) && ctx.CanAffect(defending)) ctx.Engine.KnockOut(defending, "Take Out");
                else ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Ramming Strike: flip until tails, 30 times the heads; you may start again, putting 2 damage counters on this Pokémon each time (not if that would Knock it Out).</summary>
        private sealed class RammingStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ramming Strike") return false;
                var me = ctx.Pokemon;
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < 10 && ctx.RemainingHp(me) > 20; i++)
                {
                    if (!ctx.AskYesNo("Ramming Strike: " + heads + " heads. Start again (2 damage counters on " + me.Name + ")?")) break;
                    ctx.Engine.PlaceDamageCounters(me, 2, "Ramming Strike");
                    heads = ctx.FlipUntilTails();
                }
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Parade: 10 more per Swinub, 20 per Piloswine and 40 per Mamoswine on your Bench.</summary>
        private sealed class ParadeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parade") return false;
                var b = ctx.Me.Bench;
                ctx.DamageDefending(atk.Damage + 10 * b.Count(m => m.Name == "Swinub") + 20 * b.Count(m => m.Name == "Piloswine") + 40 * b.Count(m => m.Name == "Mamoswine"));
                return true;
            }
        }

        /// <summary>Burning Mane (Poké-Body): while Active, when damaged by the opponent's attack (even if Knocked Out), the Attacking Pokémon is Burned. Shooting Fire.</summary>
        private sealed class RapidashBurningMane : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                Announce(ctx, "Burning Mane");
                ctx.ApplyCondition(attacker, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shooting Fire") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Shooting Fire: discard 2 Fire Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                ctx.DamageAnyOpponent(60);
                return true;
            }
        }

        /// <summary>Hidden Poison (Poké-Body): while Active, when damaged by the opponent's attack (even if Knocked Out), that Pokémon is Poisoned. Bowed Whip, Deep Poison.</summary>
        private sealed class RoseradeHiddenPoison : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                Announce(ctx, "Hidden Poison");
                ctx.ApplyCondition(attacker, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Bowed Whip")
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Bowed Whip: do 30 damage to which Pokémon?");
                    if (target == null) return true;
                    ctx.DamageTarget(target, 30);
                    if (ctx.Flip() && ctx.InPlay(target) && target.Energy.Count > 0 && ctx.CanAffect(target)) ctx.DiscardEnergyFrom(target, 1, "Bowed Whip: discard an Energy card from " + target.Name);
                    return true;
                }
                if (atk.Name != "Deep Poison") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && (ctx.Defending.Conditions & SpecialCondition.Poisoned) != 0 ? 30 : 0));
                return true;
            }
        }

        /// <summary>Battle Rush (Poké-Body): if the opponent has a Pokémon in play with 120 or more maximum HP, Salamence's attacks need no Colorless Energy. Steam Twister.</summary>
        private sealed class SalamenceBattleRush : Mon
        {
            private static readonly Mon Twister = new CardsDR.DiscardTypesMon("Steam Twister", EnergyType.Fire, EnergyType.Water);

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (ctx.OpponentPokemon().Any(m => ctx.Engine.MaxHp(m) >= 120)) cost.RemoveAll(t => t == EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Twister.HandleAttack(ctx, atk); }
        }

        /// <summary>Honeycomb Defender (Poké-Body): with 6 or more damage counters, attack damage to Scizor is 40 less (after W/R). Accelerate, Pound Down.</summary>
        private sealed class ScizorHoneycombDefender : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || me.DamageCounters < 6 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Accelerate")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (defending != null && (!ctx.InPlay(defending) || ctx.Engine.IsKnockedOut(defending)) && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                    return true;
                }
                if (atk.Name != "Pound Down") return false;
                bool none = !ctx.MyPokemon().Any(m => m.Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower));
                ctx.DamageDefending(atk.Damage + (none ? 30 : 0));
                return true;
            }
        }

        /// <summary>Evolutionary Gas: if the Defending Pokémon tries to attack during the opponent's next turn, that attack does nothing.</summary>
        private static void EvolutionaryGas(EffectContext ctx)
        {
            var target = ctx.Opp.Active;
            if (target != null && ctx.CanAffect(target)) ctx.AddEffect(new CantAttackEffect(target.Name + "'s attacks do nothing during its next turn (Evolutionary Gas)", ctx.Player, target.Uid, ctx.Turn + 1));
        }

        /// <summary>Plunder: before the damage, discard all Trainer cards attached to the Defending Pokémon.</summary>
        private sealed class PlunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plunder") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending)) ctx.Engine.DiscardAllTools(defending);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Protect Wing (Poké-Body): while Staraptor is Active, damage from the opponent's Stage 2 Pokémon's attacks is 20 less (before W/R: Mar 2009). Strong Breeze, Clutch.</summary>
        private sealed class StaraptorProtectWing : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new StrongBreezeMon(), new NoRetreatMon("Clutch"));

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || !src.IsActive(me) || a == null || a.Owner == src.Player || a.Def.Stage != Stage.Stage2 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Strong Breeze: heads, 1 of the opponent's Benched Pokémon and its cards go into their deck, shuffled.</summary>
        private sealed class StrongBreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong Breeze") return false;
                if (!ctx.Flip() || ctx.Opp.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Strong Breeze: put which Benched Pokémon into your opponent's deck?");
                if (target != null && ctx.CanAffect(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Outbreak Power: 20 to each of up to as many of the opponent's Pokémon as the Energy on Steelix (no W/R on the Bench).</summary>
        private sealed class OutbreakPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Outbreak Power") return false;
                int max = Math.Min(ctx.EnergyCount(ctx.Pokemon), ctx.OpponentPokemon().Count);
                if (max == 0) return true;
                foreach (var m in ctx.ChoosePokemonMany(ctx.OpponentPokemon(), 1, max, "Outbreak Power: do 20 damage to which Pokémon?")) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Green Acid: 2 coins; 1st heads, Confused; 2nd heads, Poisoned.</summary>
        private sealed class GreenAcidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Green Acid") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool first = ctx.Flip(), second = ctx.Flip();
                if (first && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (second && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Darkness Drive (Poké-Body): after an opponent's Pokémon uses a Poké-Power, you may attach a basic Darkness Energy card from your discard pile to Tyranitar. Grind, Spinning Tail.</summary>
        private sealed class TyranitarDarknessDrive : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASR1.TimesMon("Grind", c => c.EnergyCount(c.Pokemon)), new CardsSS.EachOpponentMon("Spinning Tail", 30));

            public override void OnPokePowerUsed(EffectContext src, PokemonInPlay user, string power)
            {
                var me = src.Pokemon;
                if (me == null || user == null || user.Owner == src.Player) return;
                var pile = src.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Darkness);
                if (pile.Count == 0 || !src.AskYesNo("Darkness Drive: attach a basic Darkness Energy card from your discard pile to " + me.Name + "?")) return;
                Announce(src, "Darkness Drive");
                if (src.Me.Discard.Remove(pile[0]) && !src.Engine.AttachEnergyCard(pile[0], me) && !src.Me.Discard.Contains(pile[0])) src.Me.Discard.Add(pile[0]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Green Dignity (Poké-Body): with more Prize cards left than the opponent, Vespiquen's attacks do 10 more to the Active Pokémon per Grass Pokémon on your Bench (before W/R). Bee Drain, Bee Powder.</summary>
        private sealed class VespiquenGreenDignity : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || src.Me.Prizes.Count <= src.Opp.Prizes.Count || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 10 * src.MyBench().Count(m => src.Engine.IsType(m, EnergyType.Grass));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Bee Drain")
                {
                    int done = ctx.DamageDefending(atk.Damage);
                    if (done > 0 && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, done);
                    return true;
                }
                if (atk.Name != "Bee Powder") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) < 2 || !TefPokemon.StillDefending(ctx, defending)) return true;
                foreach (var c in new[] { SpecialCondition.Burned, SpecialCondition.Paralyzed, SpecialCondition.Poisoned }) if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, c);
                return true;
            }
        }

        /// <summary>Unaware (Poké-Body): prevent all effects of attacks, except damage, done to Bibarel. Amnesia.</summary>
        private sealed class BibarelUnaware : Mon
        {
            private static readonly Mon Amnesia = new CardsTEU1.AmnesiaMon("Amnesia");

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters) { return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Amnesia.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Enzyme (Poké-Body): prevent all effects of attacks, damage included, from the opponent's Poisoned Pokémon. Baby Evolution (Roselia). Buddy-buddy.</summary>
        private sealed class BudewPoisonEnzyme : Mon
        {
            private static readonly Mon Baby = new CardsSS.BabyEvolutionMon("Roselia", new SearchAttackMon("Buddy-buddy", d => d.IsPokemon, 1));

            private static bool Poisoned(PokemonInPlay a) { return a != null && (a.Conditions & SpecialCondition.Poisoned) != 0; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != src.Player && Poisoned(info.Attacker) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && !src.IsMyTurn && Poisoned(src.Opp.Active);
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex) { return Baby.CanUseAbility(ctx, abilityIndex); }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { Baby.UseAbility(ctx, abilityIndex); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Baby.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark One-eye: you may discard a card from your hand; if you do, the opponent discards a card from theirs.</summary>
        private sealed class DarkOneEyeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark One-eye") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count == 0 || !ctx.AskYesNo("Dark One-eye: discard a card from your hand so your opponent discards one?")) return true;
                ctx.Engine.DiscardFromHand(ctx.Player, ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Dark One-eye: discard which card?")[0]);
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.DiscardFromHand(ctx.Opponent, ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 1, 1, "Dark One-eye: discard a card from your hand", ctx.Card.Def)[0]);
                return true;
            }
        }

        /// <summary>Trick Room: 20 more with your Stadium in play; with the opponent's, remove 2 damage counters from this Pokémon.</summary>
        private sealed class TrickRoomMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick Room") return false;
                bool mine = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
                bool theirs = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent;
                ctx.DamageDefending(atk.Damage + (mine ? 20 : 0));
                if (theirs && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 20);
                return true;
            }
        }

        /// <summary>Radiance (Poké-Body): while Active, when damaged by the opponent's attack (even if Knocked Out), 1 damage counter on each of the opponent's Pokémon. Low Current, Swift.</summary>
        private sealed class ElectrodeRadiance : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSW_LowCurrent(), new CardsMA.SwiftMon("Swift"));

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || damage <= 0) return;
                Announce(ctx, "Radiance");
                foreach (var m in ctx.OpponentPokemon()) ctx.Engine.PlaceDamageCounters(m, 1, "Radiance");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Low Current: if this Pokémon was damaged by an attack during the opponent's last turn, the Defending Pokémon is Paralyzed.</summary>
        private sealed class CardsSW_LowCurrent : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Low Current") return false;
                var defending = ctx.Defending;
                bool hurt = ctx.Pokemon.AttackDamageTurn == ctx.OpponentsLastTurn;
                ctx.DamageDefending(atk.Damage);
                if (hurt && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Electro Diffusion: 10 more per Lightning Energy; tails, discard all Lightning Energy from it.</summary>
        private sealed class ElectroDiffusionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electro Diffusion") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * ctx.Engine.EnergyCountOfType(me, EnergyType.Lightning));
                if (!ctx.Flip() && ctx.InPlay(me))
                    foreach (var e in me.Energy.Where(x => CardsDRI1.ProvidesType(ctx, me, x, EnergyType.Lightning)).ToList()) ctx.Engine.DiscardEnergy(me, e);
                return true;
            }
        }

        /// <summary>Hoodwink: you may put up to 3 Trainer, Supporter or Stadium cards from the opponent's discard pile into their hand.</summary>
        private sealed class HoodwinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hoodwink") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Opp.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (pile.Count == 0) return true;
                foreach (var c in ctx.ChooseCards(pile, 0, Math.Min(3, pile.Count), "Hoodwink: put which cards into your opponent's hand?"))
                    if (ctx.Opp.Discard.Remove(c)) ctx.Opp.Hand.Add(c);
                return true;
            }
        }

        /// <summary>Brick Break: the damage ignores Resistance and effects on the Defending Pokémon.</summary>
        private sealed class BrickBreakMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brick Break") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
                return true;
            }
        }

        /// <summary>Magnetic Resonance: with your Stadium in play, also 20 to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class MagneticResonanceMon : Mon
        {
            private static readonly Mon Two = new CardsFST1.TwoBenchedMon("Magnetic Resonance", 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Resonance") return false;
                if (ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player) return Two.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Energy Milk: flip until tails; for each heads, remove 2 damage counters from 1 of your Pokémon.</summary>
        private sealed class EnergyMilkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Milk") return false;
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < heads; i++)
                {
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count == 0) break;
                    var target = ctx.ChoosePokemon(hurt, "Energy Milk: remove 2 damage counters from which Pokémon?");
                    if (target != null) ctx.Heal(target, 20);
                }
                return true;
            }
        }

        /// <summary>Electric Circuit: up to 4 Lightning Energy cards from your discard pile into your hand.</summary>
        private sealed class ElectricCircuitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electric Circuit") return false;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList();
                if (pile.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, Math.Min(4, pile.Count), "Electric Circuit: put which Lightning Energy cards into your hand?"));
                return true;
            }
        }

        /// <summary>Rouse: 10 more per damage counter on this Pokémon; then remove 4 damage counters from it.</summary>
        private sealed class RouseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rouse") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 40);
                return true;
            }
        }

        /// <summary>Energy Protection (Poké-Body): attack damage to Pupitar is 10 less per Energy on it, no more than 30 (after W/R). Rock Smash.</summary>
        private sealed class PupitarEnergyProtection : Mon
        {
            private static readonly Mon Smash = new FlipBonusMon("Rock Smash", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - Math.Min(30, 10 * src.EnergyCount(me));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smash.HandleAttack(ctx, atk); }
        }

        /// <summary>Impersonate: discard a Supporter card from your deck and use its effect. (Overeager, going first when Sableye starts Active, isn't simulated.)</summary>
        private sealed class ImpersonateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Impersonate") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsSupporter, 1, "Impersonate: choose a Supporter card to discard and use");
                ctx.ShuffleMyDeck();
                if (found.Count == 0) return true;
                var card = found[0];
                ctx.Me.Discard.Add(card);
                if (!MrMimeTEF.WouldDoSomething(ctx, card)) return true;
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + card.Def.Name + " (Impersonate).");
                ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Remove(card);
                if (!ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Overconfident: base 40 if the Defending Pokémon has fewer HP left than this Pokémon.</summary>
        private sealed class OverconfidentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overconfident") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.RemainingHp(ctx.Defending) < ctx.RemainingHp(ctx.Pokemon) ? 40 : atk.Damage);
                return true;
            }
        }

        /// <summary>Swords Dance: during your next turn, [attack]'s base damage is N. [Attack]: it can't be used during your next turn.</summary>
        private sealed class SwordsDanceMon : Mon
        {
            private const string Marker = "Swords Dance";
            private readonly string attack;
            private readonly int boosted;

            public SwordsDanceMon(string attack, int boosted)
            {
                this.attack = attack;
                this.boosted = boosted;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == Marker)
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != attack) return false;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? boosted : atk.Damage);
                if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn(attack);
                return true;
            }
        }

        /// <summary>Mach Blade (on heads): 50 to 1 of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class MachBladeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Blade") return false;
                ctx.DamageAnyOpponent(50);
                return true;
            }
        }

        /// <summary>Self-abandonment: heads, 30 to the Defending Pokémon; tails, 10 to itself.</summary>
        private sealed class SelfAbandonmentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Self-abandonment") return false;
                if (ctx.Flip()) ctx.DamageDefending(30);
                else ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Gyro Swap: damage counters on the Defending Pokémon equal to this Pokémon's Retreat Cost.</summary>
        private sealed class GyroSwapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gyro Swap") return false;
                int n = ctx.Engine.RetreatCostOf(ctx.Pokemon);
                if (n > 0 && ctx.Defending != null && ctx.CanAffect(ctx.Defending)) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Alert: draw a card; then you may switch this Pokémon with a Benched one.</summary>
        private sealed class AlertMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Alert") return false;
                ctx.Draw(1);
                if (ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Alert: switch with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Silhouette: 1 damage counter on the Defending Pokémon (2 if it's already damaged).</summary>
        private sealed class SilhouetteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silhouette") return false;
                var d = ctx.Defending;
                if (d != null && ctx.CanAffect(d)) ctx.PlaceCounters(d, d.DamageCounters > 0 ? 2 : 1);
                return true;
            }
        }

        /// <summary>Pitch-Dark: the opponent can't play Trainer cards during their next turn.</summary>
        private sealed class PitchDarkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pitch-Dark") return false;
                ctx.AddEffect(new DpTrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (Pitch-Dark)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Nightmare Feast: if the Defending Pokémon is Asleep, 50 damage and remove 5 damage counters from this Pokémon; otherwise nothing.</summary>
        private sealed class NightmareFeastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nightmare Feast") return false;
                if (ctx.Defending == null || (ctx.Defending.Conditions & SpecialCondition.Asleep) == 0) return Nothing(ctx);
                ctx.DamageDefending(50);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 50);
                return true;
            }
        }

        /// <summary>Speed Bolt: if this Pokémon evolved from Pichu this turn, prevent all effects of attacks on it next turn.</summary>
        private sealed class SpeedBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Speed Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Pokemon.TurnEvolved == ctx.Turn && EvolvedFrom(ctx, "Pichu") && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Petal Spikes: heads (or with Budew under it, no flip), the Defending Pokémon is Asleep and Poisoned.</summary>
        private sealed class PetalSpikesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Petal Spikes") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!(EvolvedFrom(ctx, "Budew") || ctx.Flip()) || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Ingrain: tails, nothing; heads, attach a Grass Energy card from your deck to this Pokémon and prevent all effects of attacks on it next turn.</summary>
        private sealed class IngrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ingrain") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Grass), 1, "Ingrain: choose a Grass Energy card");
                bool attached = found.Count > 0 && ctx.Engine.AttachEnergyCard(found[0], me);
                if (found.Count > 0 && !attached) ctx.Me.Deck.Add(found[0]);
                ctx.ShuffleMyDeck();
                if (attached) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        // ------------------------------------------------------------------ Trainers

        /// <summary>Conductive Quarry (Stadium): once during each player's turn, flip: heads, a Lightning or Metal Energy card from their discard pile into their hand.</summary>
        private sealed class ConductiveQuarry : CardScript
        {
            private const string Limit = "Conductive Quarry";

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsEnergy && (c.Def.EnergyProvides == EnergyType.Lightning || c.Def.EnergyProvides == EnergyType.Metal)).ToList(); }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && Pile(ctx).Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player); }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return true;
                ctx.DiscardToHand(ctx.ChooseCards(Pile(ctx), 1, 1, "Conductive Quarry: put which Energy card into your hand?"));
                return true;
            }
        }

        /// <summary>Energy Link (Pokémon Tool): as often as you like during your turn, move an Energy card from its Pokémon to another of your Pokémon with Energy Link attached.</summary>
        private sealed class EnergyLink : CardScript
        {
            private static List<PokemonInPlay> Others(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.Tools.Any(t => t.Def.Name == "Energy Link")); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.IsMyTurn && ctx.Pokemon != null && ctx.Pokemon.Energy.Count > 0 && Others(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                var to = ctx.ChoosePokemon(Others(ctx), "Energy Link: move an Energy card from " + me.Name + " to which Pokémon?");
                if (to == null) return;
                var card = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Energy Link: move which Energy card?")[0];
                ctx.Engine.MoveEnergy(me, to, card);
            }
        }

        /// <summary>Luxury Ball: a non-LV.X Pokémon from your deck into your hand. Not while a Luxury Ball is in your discard pile.</summary>
        private sealed class LuxuryBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && !ctx.Me.Discard.Any(c => c.Def.Name == "Luxury Ball"); }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(d => d.IsPokemon && !d.IsLevelUp, 1, "Luxury Ball: choose a Pokémon (not LV.X)"); }
        }

        /// <summary>Marley's Request (Supporter): show 2 different Trainer, Supporter or Stadium cards from your discard pile; the opponent picks 1 for your hand (the other stays discarded). If they all share a name, take 1.</summary>
        private sealed class MarleysRequest : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsTrainer && c != ctx.Card).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                var names = pile.Select(c => c.Def.Name).Distinct().ToList();
                if (names.Count == 1)
                {
                    ctx.DiscardToHand(new[] { pile[0] });
                    return;
                }
                var first = ctx.ChooseCards(pile, 1, 1, "Marley's Request: choose the first card")[0];
                var second = ctx.ChooseCards(pile.Where(c => c.Def.Name != first.Def.Name).ToList(), 1, 1, "Marley's Request: choose a second, different card")[0];
                var pick = ctx.Engine.ChooseCards(ctx.Opponent, new List<CardInstance> { first, second }, 1, 1, "Marley's Request: which card goes into your opponent's hand?", ctx.Card.Def)[0];
                ctx.DiscardToHand(new[] { pick });
            }
        }

        /// <summary>
        /// The Poké "+" Items: you may play 2 at the same time for the stronger effect. Played with a second copy in your hand, you're
        /// asked whether to play both (the second copy is discarded with it).
        /// </summary>
        private sealed class PlusItem : CardScript
        {
            private readonly Action<EffectContext> one;
            private readonly Action<EffectContext> two;

            public PlusItem(Action<EffectContext> one, Action<EffectContext> two)
            {
                this.one = one;
                this.two = two;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var other = ctx.Me.Hand.FirstOrDefault(c => c != ctx.Card && c.Def.Name == ctx.Card.Def.Name);
                if (other != null && ctx.AskYesNo(ctx.Card.Def.Name + ": play 2 at the same time?"))
                {
                    ctx.Me.Hand.Remove(other);
                    ctx.Me.Discard.Add(other);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " played 2 " + ctx.Card.Def.Name + " at the same time.");
                    two(ctx);
                    return;
                }
                one(ctx);
            }
        }

        private static void BlowerOne(EffectContext ctx)
        {
            if (!ctx.Flip()) return;
            var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Poké Blower +: put 1 damage counter on which Pokémon?");
            if (target != null) ctx.Engine.PlaceDamageCounters(target, 1, "Poké Blower +");
        }

        private static void BlowerTwo(EffectContext ctx)
        {
            if (ctx.GustTargets().Count > 0) ctx.GustOpponent("Poké Blower +: switch in which of your opponent's Benched Pokémon?");
        }

        private static void DrawerOne(EffectContext ctx) { ctx.Draw(1); }

        private static void DrawerTwo(EffectContext ctx)
        {
            if (ctx.Me.Deck.Count == 0) return;
            ctx.DeckToHand(ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, Math.Min(2, ctx.Me.Deck.Count), "Poké Drawer +: put up to 2 cards into your hand"));
            ctx.ShuffleMyDeck();
        }

        private static void HealerOne(EffectContext ctx)
        {
            var active = ctx.Me.Active;
            if (active == null) return;
            if (active.DamageCounters > 0) ctx.Heal(active, 10);
            var has = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned }.Where(c => (active.Conditions & c) != 0).ToList();
            if (has.Count == 0) return;
            int pick = has.Count == 1 ? 0 : ctx.ChooseOneOption("Poké Healer +: remove which Special Condition?", has.Select(c => c.ToString()).ToArray());
            active.Conditions &= ~has[Math.Max(0, Math.Min(has.Count - 1, pick))];
        }

        private static void HealerTwo(EffectContext ctx)
        {
            var active = ctx.Me.Active;
            if (active == null) return;
            if (active.DamageCounters > 0) ctx.Heal(active, 80);
            ctx.Engine.RemoveAllConditions(active);
        }

        // ------------------------------------------------------------------ SH

        /// <summary>Unburden (Poké-Body): with a Pokémon Tool attached, Drifloon's Retreat Cost is 2 more. Big Explosion.</summary>
        private sealed class DrifloonUnburden : Mon
        {
            private static readonly Mon Boom = new RecoilMon("Big Explosion", 50);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return mon != null && mon == src.Pokemon && mon.Tools.Count > 0 ? cost + 2 : cost; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Boom.HandleAttack(ctx, atk); }
        }

        /// <summary>Counting Song: up to 3 damage counters on this Pokémon, then as many on the Defending Pokémon.</summary>
        private sealed class CountingSongMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Counting Song") return false;
                var me = ctx.Pokemon;
                int max = Math.Min(3, Math.Max(0, ctx.RemainingHp(me) / 10));
                if (max == 0 || ctx.Defending == null) return true;
                int n = ctx.AskNumber(0, max, "Counting Song: put how many damage counters on " + me.Name + "?");
                if (n == 0) return true;
                ctx.Engine.PlaceDamageCounters(me, n, "Counting Song");
                if (ctx.InPlay(ctx.Defending) && ctx.CanAffect(ctx.Defending)) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Night Bind: heads, the opponent can't attach Energy from their hand to their Active Pokémon next turn.</summary>
        private sealed class NightBindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Bind") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new NoAttachEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't attach Energy from their hand to their Active Pokémon next turn (Night Bind)", ctx.Player, ctx.Opponent, ctx.Turn + 1, true));
                return true;
            }
        }

        /// <summary>Fastball: choose 1 of the opponent's Pokémon; heads, 20 to it (no W/R on the Bench).</summary>
        private sealed class FastballMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fastball") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Fastball: choose 1 of your opponent's Pokémon");
                if (target != null && ctx.Flip()) ctx.DamageTarget(target, 20);
                return true;
            }
        }
    }
}
