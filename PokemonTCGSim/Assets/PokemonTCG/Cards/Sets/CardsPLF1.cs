using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Plasma Freeze (PLF, v110): Grass to Psychic Pokémon (1-57). Exeggcute is Plasma Blast's card. Team Plasma Pokémon are checked
    /// in play with GameEngine.IsTeamPlasma (a Team Plasma Badge makes its holder one).
    /// </summary>
    public static class CardsPLF1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PLF-1", new CoinsMon("Triple Stab", 3));
            r.RegisterByKey("PLF-2", new DodgeMon("Hide", false));
            r.RegisterByKey("PLF-3", new SwiftStingMon());
            r.RegisterByKey("PLF-5", new ComboMon(new BlockadeMon(), new FlipBonusMon("Stomp", 30)));
            r.RegisterByKey("PLF-6", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("PLF-8", new ComboMon(new FlipBonusMon("X-Scissor", 40), new CardsFFI2.SandSweepMon("Energy Bloom", 20)));
            r.RegisterByKey("PLF-9", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("PLF-10", new ComboMon(new RapidFireNeedlesMon(), new PaybackMon()));
            r.RegisterByKey("PLF-11", new ComboMon(new CardsASR1.TimesMon("Energy Crush", c => c.OpponentPokemon().Sum(m => m.Energy.Count)), new FlipBonusMon("Leaf Blade", 20)));
            r.RegisterByKey("PLF-12", new ComboMon(new CardsASR1.PlusMon("Vengeance", 10, c => c.Me.Discard.Count(x => x.Def.IsPokemon)), new RecoilMon("Heat Tackle", 10)));
            r.RegisterByKey("PLF-13", new HeatranExMon());
            r.RegisterByKey("PLF-14", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("PLF-15", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("PLF-16", new ChandelureFlareNavigate());
            r.RegisterByKey("PLF-17", new ComboMon(new PurifyingFlameMon(), new CardsFST2.ConditionBonusMon("Fusion Flare", 40, c => c.Me.Bench.Any(m => m.Name == "Zekrom"))));
            r.RegisterByKey("PLF-18", new CoinsMon("Fin Smack", 2));
            r.RegisterByKey("PLF-19", new CardsUPR3.SmokescreenMon("Smokescreen", 1));
            r.RegisterByKey("PLF-20", new ComboMon(new HealAllMineMon("Refreshing Rain", 30), new CardsFST2.ConditionBonusMon("Gold Breaker", 50, c => c.Defending != null && c.Defending.Def.IsPokemonEX)));
            r.RegisterByKey("PLF-21", new CoinsMon("Slam", 2));
            r.RegisterByKey("PLF-22", new CardsFST1.TakeLessMon(20, new CardsFST2.ConditionBonusMon("Mud Gun", 30, c => c.Engine.HasEnergyOfType(c.Pokemon, EnergyType.Fighting))));
            r.RegisterByKey("PLF-23", new GlaceonFreezeZone());
            r.RegisterByKey("PLF-24", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PLF-25", new CardsPAR2.FlipConditionMon("Suspicious Soundwave", SpecialCondition.Confused));
            r.RegisterByKey("PLF-26", new ComboMon(new CardsLTR2.GaiaHammerMon("Seismic Punch", 30, 30), new SwitchSelfMon("Splashing Turn", false)));
            r.RegisterByKey("PLF-27", new FlipBonusMon("Jump On", 10));
            r.RegisterByKey("PLF-28", new CardsFST2.ConditionBonusMon("Surefire Spin", 40, c => c.FlipCoins(2) == 2));
            r.RegisterByKey("PLF-29", new ComboMon(new CardsASR1.TimesMon("ChillMAX", c => c.Pokemon.Energy.Count == 0 ? 0 : c.FlipCoins(c.Pokemon.Energy.Count)), new ConditionAttackMon("Cold Breath", SpecialCondition.Asleep)));
            r.RegisterByKey("PLF-30", new ComboMon(new SearchAttackMon("Call Sign", d => d.IsPokemon && d.PokemonType == EnergyType.Water, 1), new CardsASR1.IcebergPressMon("Cryofreeze")));
            r.RegisterByKey("PLF-31", new ComboMon(new Cards30CC.BenchHitMon("Frost Spear", 30), new LockAfterAttackMon("Blizzard Burn", false)));
            r.RegisterByKey("PLF-33", new ElectrodeMagneticDraw());
            r.RegisterByKey("PLF-34", new ComboMon(new CoinsMon("Pin Missile", 4), new CardsEVS1.DefenderCantAttackMon("Electri-Defuse", d => d.IsPokemonEX)));
            r.RegisterByKey("PLF-36", new ComboMon(new CardsFST2.ConditionBonusMon("Special Tackle", 30, c => c.Pokemon.Energy.Any(e => e.Def.IsSpecialEnergy)), new CardsTWM1.SelfDiscardMon("Extreme Current", 1)));
            r.RegisterByKey("PLF-37", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2), new ParalyzeFlipMon("Electric Tail")));
            r.RegisterByKey("PLF-38", new ThundurusExMon());
            r.RegisterByKey("PLF-39", new ComboMon(new CardsFLF1.NoResistanceMon("Mach Claw"), new CardsFST2.ConditionBonusMon("Fusion Bolt", 40, c => c.Me.Bench.Any(m => m.Name == "Reshiram"))));
            r.RegisterByKey("PLF-40", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("PLF-41", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("PLF-42", new ComboMon(new ConditionAttackMon("Poison Horn", SpecialCondition.Poisoned), new CardsWHT.FlipMoreMon("Double Stomp", 2, 30)));
            r.RegisterByKey("PLF-43", new CardsCRZ1.AdrenaTackleMon("Hit Back"));
            r.RegisterByKey("PLF-44", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("PLF-45", new LurePoisonMon());
            r.RegisterByKey("PLF-46", new ComboMon(new PoisonSuctionMon(), new SludgeCrashMon()));
            r.RegisterByKey("PLF-47", new MrMimeBenchBarrier());
            r.RegisterByKey("PLF-48", new ComboMon(new CardsPLB1.DrawUntilAttackMon("Psy Alert", 6), new ShadowBallMon()));
            r.RegisterByKey("PLF-49", new NoRetreatMon("Shadow Cage"));
            r.RegisterByKey("PLF-50", new CardsLTR2.ReorderTopMon("Calculate", 4));
            r.RegisterByKey("PLF-51", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("PLF-52", new MetagrossPlasmaSearch());
            r.RegisterByKey("PLF-53", new DeoxysPowerConnect());
            r.RegisterByKey("PLF-54", new TransferPainMon());
            r.RegisterByKey("PLF-56", new CofagrigusSixFeetUnder());
            r.RegisterByKey("PLF-57", new ComboMon(new CardsMEG.SnipeMon("Elongating Arms", 30), new CoinsMon("Crazy Slap", 4)));
        }

        // ================================================================== Shared

        /// <summary>"Heal N damage from each of your Pokémon" (Vaporeon's Refreshing Rain).</summary>
        internal sealed class HealAllMineMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealAllMineMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon()) if (mon.DamageCounters > 0) ctx.Heal(mon, amount);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Swift Sting: with full HP, 40 more damage and the Defending Pokémon is now Confused and Poisoned.</summary>
        private sealed class SwiftStingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swift Sting") return false;
                var defending = ctx.Defending;
                bool full = ctx.Pokemon.DamageCounters == 0;
                ctx.DamageDefending(atk.Damage + (full ? 40 : 0));
                if (full && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Blockade: the opponent can't play Supporter cards from their hand during their next turn.</summary>
        private sealed class BlockadeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blockade") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsTWM2.SupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards during their next turn (Blockade)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Rapid-Fire Needles: 60, and 30 damage to 1 of your own Pokémon (no Weakness or Resistance on the Bench).</summary>
        private sealed class RapidFireNeedlesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid-Fire Needles") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Rapid-Fire Needles: do 30 damage to which of your Pokémon?");
                if (target == null) return true;
                if (target == ctx.Pokemon) ctx.DamageSelf(30);
                else ctx.DamageWith(target, 30, false, false, false);
                return true;
            }
        }

        /// <summary>Payback: if your opponent has only 1 Prize card left, 60 more damage and discard an Energy from the Defending Pokémon.</summary>
        private sealed class PaybackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Payback") return false;
                var defending = ctx.Defending;
                bool last = ctx.Opp.Prizes.Count(c => c != null) == 1;
                ctx.DamageDefending(atk.Damage + (last ? 60 : 0));
                if (last && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Payback: discard an Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Heat Boiler: 60 more with a Special Condition. Dynamite Press: with Plasma Energy, 10 more per damage counter on the Defending Pokémon.</summary>
        private sealed class HeatranExMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Heat Boiler")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Conditions != SpecialCondition.None ? 60 : 0));
                    return true;
                }
                if (atk.Name == "Dynamite Press")
                {
                    var defending = ctx.Defending;
                    int more = CardsPLB1.PlasmaEnergy(ctx.Pokemon) > 0 && defending != null ? 10 * defending.DamageCounters : 0;
                    ctx.DamageDefending(atk.Damage + more);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Flare Navigate: once during your turn, attach a Fire Energy card from your deck to 1 of your Pokémon and put 1 damage counter on it. Absorb Life: heal 30.</summary>
        private sealed class ChandelureFlareNavigate : Mon
        {
            private static readonly Mon Absorb = new CardsTWM1.HealSelfMon("Absorb Life", 30);

            protected override string Activated { get { return "Flare Navigate"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 1, "Flare Navigate: choose a Fire Energy card");
                foreach (var card in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Flare Navigate: attach " + card.Def.Name + " to which of your Pokémon?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) { ctx.Me.Deck.Add(card); continue; }
                    ctx.PlaceCounters(target, 1);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Absorb.HandleAttack(ctx, atk); }
        }

        /// <summary>Purifying Flame: 50, then remove all Special Conditions from this Pokémon.</summary>
        private sealed class PurifyingFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Purifying Flame") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Freeze Zone: each of your Team Plasma Pokémon's Retreat Cost is [C][C] less. Icy Wind: Asleep.</summary>
        private sealed class GlaceonFreezeZone : Mon
        {
            private static readonly Mon Wind = new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !src.Engine.IsTeamPlasma(mon)) return cost;
                return Math.Max(0, cost - 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wind.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Magnetic Draw: once during your turn, draw cards until you have 4 in your hand.</summary>
        private sealed class ElectrodeMagneticDraw : Mon
        {
            protected override string Activated { get { return "Magnetic Draw"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 4 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(4); }
        }

        /// <summary>
        /// Raiden Knuckle: attach an Energy card from your discard pile to 1 of your Benched Team Plasma Pokémon. Thunderous Noise: with
        /// Plasma Energy attached, discard an Energy from the Defending Pokémon.
        /// </summary>
        internal sealed class ThundurusExMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Raiden Knuckle")
                {
                    ctx.DamageDefending(atk.Damage);
                    var targets = ctx.MyBench().Where(m => ctx.Engine.IsTeamPlasma(m)).ToList();
                    var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                    if (targets.Count == 0 || pile.Count == 0) return true;
                    var card = pile.Select(c => c.Def.Name).Distinct().Count() == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, "Raiden Knuckle: attach which Energy card from your discard pile?")[0];
                    var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Raiden Knuckle: attach " + card.Def.Name + " to which Benched Team Plasma Pokémon?");
                    if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                    return true;
                }
                if (atk.Name == "Thunderous Noise")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (CardsPLB1.PlasmaEnergy(ctx.Pokemon) > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.DiscardEnergyFrom(defending, 1, "Thunderous Noise: discard an Energy from " + defending.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Psychic

        /// <summary>Lure Poison: flip a coin; heads, switch in 1 of the opponent's Benched Pokémon and it is now Poisoned.</summary>
        private sealed class LurePoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lure Poison") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count == 0 || !ctx.Flip()) return true;
                var pulled = ctx.GustOpponent("Lure Poison: choose 1 of your opponent's Benched Pokémon to switch in");
                if (pulled != null && TefPokemon.StillDefending(ctx, pulled)) ctx.ApplyCondition(pulled, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Poison Suction: if the Defending Pokémon is Poisoned, heal 60 damage from this Pokémon.</summary>
        private sealed class PoisonSuctionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Suction") return false;
                var defending = ctx.Defending;
                bool poisoned = defending != null && defending.Has(SpecialCondition.Poisoned);
                ctx.DamageDefending(atk.Damage);
                if (poisoned && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 60);
                return true;
            }
        }

        /// <summary>Sludge Crash: flip until tails; discard an Energy from the Defending Pokémon for each heads.</summary>
        private sealed class SludgeCrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sludge Crash") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, Math.Min(heads, defending.Energy.Count), "Sludge Crash: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Bench Barrier: prevent all damage done to your Benched Pokémon by attacks.</summary>
        private sealed class MrMimeBenchBarrier : Mon
        {
            private static readonly Mon Bolt = new ParalyzeFlipMon("Psy Bolt");

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                return src.Pokemon != null && t != null && info.Attacker != null && t.Owner == src.Player && !src.IsActive(t);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bolt.HandleAttack(ctx, atk); }
        }

        /// <summary>Shadow Ball: 40 damage to 1 of the opponent's Pokémon, with Weakness and Resistance for a Benched one too.</summary>
        private sealed class ShadowBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Ball") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Shadow Ball: do 40 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageWith(target, 40, true, true, false);
                return true;
            }
        }

        /// <summary>Plasma Search: once during your turn, search your deck for a Team Plasma card (1 Plasma Search per turn). Mind Bend: Confused.</summary>
        private sealed class MetagrossPlasmaSearch : Mon
        {
            private static readonly Mon Bend = new ConditionAttackMon("Mind Bend", SpecialCondition.Confused);

            protected override string Activated { get { return "Plasma Search"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(d => d.IsTeamPlasma, 1, "Plasma Search: choose a Team Plasma card"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bend.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Power Connect: your Team Plasma Pokémon's attacks (not Deoxys-EX's) do 10 more damage to the opponent's Active Pokémon (before
        /// W/R); each Deoxys-EX adds 10. Helix Force: with Plasma Energy, 30 more per Energy on the Defending Pokémon (the Energy it provides:
        /// Double Colorless Energy counts 2, Compendium 2013-06-20).
        /// </summary>
        internal sealed class DeoxysPowerConnect : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || a.Name == "Deoxys-EX" || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsTeamPlasma(a) ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Helix Force") return false;
                var defending = ctx.Defending;
                int more = CardsPLB1.PlasmaEnergy(ctx.Pokemon) > 0 && defending != null ? 30 * ctx.EnergyCount(defending) : 0;
                ctx.DamageDefending(atk.Damage + more);
                return true;
            }
        }

        /// <summary>Transfer Pain: move 1 damage counter from 1 of your Pokémon to 1 of your opponent's Pokémon.</summary>
        private sealed class TransferPainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Transfer Pain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var foes = ctx.OpponentPokemon(m => ctx.CanAffect(m));
                if (hurt.Count == 0 || foes.Count == 0) return true;
                var origin = ctx.ChoosePokemon(hurt, "Transfer Pain: move a damage counter from which of your Pokémon?");
                var dest = ctx.ChoosePokemon(foes, "Transfer Pain: move it to which of your opponent's Pokémon?");
                if (origin != null && dest != null) ctx.Engine.MoveDamageCounters(origin, dest, 1, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Six Feet Under: once during your turn, you may Knock Out this Pokémon; if you do, put 3 damage counters on the opponent's
        /// Pokémon in any way you like. Slap of Misfortune: the opponent's coin flips next turn are all tails.
        /// </summary>
        private sealed class CofagrigusSixFeetUnder : Mon
        {
            protected override string Activated { get { return "Six Feet Under"; } }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Six Feet Under"; }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Engine.KnockOut(ctx.Pokemon, "Six Feet Under")) return;
                ctx.PlaceCountersAnyWay(3, () => ctx.OpponentPokemon());
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slap of Misfortune") return false;
                ctx.DamageDefending(atk.Damage);
                var effect = new CardsFFI1.AllTailsEffect("Every coin flip " + ctx.Engine.PlayerName(ctx.Opponent) + " makes during their next turn is tails (Slap of Misfortune)", ctx.Player, ctx.Turn + 1);
                effect.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(effect);
                return true;
            }
        }
    }
}
