using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Steam Siege (STS, v93), part 2: the Lightning, Psychic, Fighting, Darkness and Metal Pokémon (38-75).</summary>
    public static class CardsSTS2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("STS-38", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("STS-39", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("STS-40", new AmpharosSTS());
            r.RegisterByKey("STS-42", new ComboMon(new DoubleThreadMon(), new NoRetreatMon("Electroweb")));
            r.RegisterByKey("STS-43", new ComeAlongMon());
            r.RegisterByKey("STS-44", new NidoPressMon());
            r.RegisterByKey("STS-45", new NidokingSTS());
            r.RegisterByKey("STS-46", new TransferPainMon());
            r.RegisterByKey("STS-47", new ComboMon(new ConditionAttackMon("Eerie Wave", SpecialCondition.Confused), new BurstCurseMon()));
            r.RegisterByKey("STS-48", new SlightlySimmerMon());
            r.RegisterByKey("STS-49", new ComboMon(new MirrorMirrorMon(), new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep)));
            r.RegisterByKey("STS-50", new ChandelureSTS());
            r.RegisterByKey("STS-51", new ComboMon(new CardsCES1.TwoTargetsMon("Hyperspace Punch", 20), new LockAfterAttackMon("Portal Strike", true)));
            r.RegisterByKey("STS-52", new MankeySTS());
            r.RegisterByKey("STS-53", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"), new SpiritedThrowMon("Spirited Throw", 60)));
            r.RegisterByKey("STS-54", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("STS-55", new ComboMon(new CardsDRI1.WhirlwindMon("Bounce Back"), new NoRetreatMon("Rock Tomb")));
            r.RegisterByKey("STS-56", new AnorithSTS());
            r.RegisterByKey("STS-57", new ComboMon(new CardsEPO.RushingWaterMon(), new ShieldAttackMon("Guard Claw", 30)));
            r.RegisterByKey("STS-58", new PoisonUpMon());
            r.RegisterByKey("STS-59", new ToxicroakSTS());
            r.RegisterByKey("STS-60", new DodgeMon("Nyan Roll", false));
            r.RegisterByKey("STS-61", new WeavileSTS());
            r.RegisterByKey("STS-62", new ComboMon(new ConditionAttackMon("Nightmare", SpecialCondition.Asleep), new DamagePlayMon()));
            r.RegisterByKey("STS-63", new CardsASR1.TimesMon("Charge Order", c => c.MyPokemon(m => m.Name == "Pawniard").Count));
            r.RegisterByKey("STS-64", new ComboMon(new RetaliateMon("Retaliate", 60), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("STS-65", new ComboMon(new CardsSLG1.DiscardEnergyToBenchMon("Oblivion Wing", EnergyType.Darkness), new CardsSVI1.FlipTailsLockMon("Darkness Blade")));
            r.RegisterByKey("STS-66", new BalefulNightMon());
            r.RegisterByKey("STS-67", new ComboMon(new CardsSUM5.OptionalBoostRecoilMon("Wild Edge", 50, 20), new CardsPFL.FlipUntilTailsTimesMon("Iron Tail")));
            r.RegisterByKey("STS-68", new CardsMEW1.SpreadBenchMon("Canyon Axe", 10));
            r.RegisterByKey("STS-69", new ShieldAttackMon("Rock Head", 20));
            r.RegisterByKey("STS-70", new ComboMon(new CounterHeadMon(), new FortressOfRageMon()));
            r.RegisterByKey("STS-71", new CoinsMon("Disorderly Flip", 4));
            r.RegisterByKey("STS-72", new CoinsMon("Disorderly Flip", 4));
            r.RegisterByKey("STS-73", new KlinklangSTS());
            r.RegisterByKey("STS-74", new ComboMon(new QuickGuardMon(), new CardsASR1.PlusMon("Revenge Blast", 30, c => c.OpponentPrizesTaken)));
            r.RegisterByKey("STS-75", new MagearnaEX());
        }

        // ================================================================== Lightning

        /// <summary>
        /// Shocking Light: once during your turn (before your attack), 3 damage counters on 1 of the opponent's Pokémon-EX (XY-era
        /// EX, not today's Pokémon ex). Gigavolt: heads, 40 more; tails, the opponent's Active Pokémon is now Paralyzed.
        /// </summary>
        private sealed class AmpharosSTS : Mon
        {
            protected override string Activated { get { return "Shocking Light"; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Def.IsPokemonEX); }

            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Shocking Light: put 3 damage counters on which of your opponent's Pokémon-EX?");
                ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigavolt") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 40 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Double Thread (errata, your ruling v134): 30 damage to 2 of the opponent's Benched Pokémon, applying Weakness and Resistance.</summary>
        private sealed class DoubleThreadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Thread") return false;
                var all = ctx.OpponentBench();
                if (all.Count == 0) return Nothing(ctx);
                int n = Math.Min(2, all.Count);
                var targets = n == all.Count ? all : ctx.ChoosePokemonMany(all, n, n, "Double Thread: choose 2 of your opponent's Benched Pokémon");
                foreach (var target in targets)
                {
                    if (!ctx.InPlay(target)) continue;
                    ctx.Engine.DealDamage(new DamageInfo
                    {
                        Attacker = ctx.Pokemon,
                        Target = target,
                        TargetIsActive = false,
                        ApplyWeakness = true,
                        ApplyResistance = true,
                        WeaknessAndResistanceOnBench = true
                    }, 30);
                }
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Come Along: search your deck for a Nidoran♀ and put it onto your Bench; shuffle.</summary>
        private sealed class ComeAlongMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Come Along") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.Name == "Nidoran♀", 1, "Come Along: choose a Nidoran♀ to put onto your Bench");
                return true;
            }
        }

        /// <summary>Nido Press: 40 more if Nidorina is on your Bench.</summary>
        private sealed class NidoPressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nido Press") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Bench.Any(m => m.Name == "Nidorina") ? 40 : 0));
                return true;
            }
        }

        /// <summary>King's Palace: your Nidoqueen's attacks do 20 more to the opponent's Active Pokémon. Power Lariat: 30 more for each Evolution Pokémon on your Bench.</summary>
        private sealed class NidokingSTS : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return attacker.Name == "Nidoqueen" ? damage + 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Lariat") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.Me.Bench.Count(m => m.Def.IsEvolution));
                return true;
            }
        }

        /// <summary>Transfer Pain: move 1 damage counter from 1 of your Pokémon to 1 of the opponent's Pokémon.</summary>
        private sealed class TransferPainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Transfer Pain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var theirs = ctx.OpponentPokemon();
                if (hurt.Count == 0 || theirs.Count == 0) return true;
                var source = ctx.ChoosePokemon(hurt, "Transfer Pain: move a damage counter from which of your Pokémon?");
                var dest = ctx.ChoosePokemon(theirs, "Transfer Pain: move it to which of your opponent's Pokémon?");
                if (source != null && dest != null) ctx.Engine.MoveDamageCounters(source, dest, 1, "Transfer Pain");
                return true;
            }
        }

        /// <summary>Burst Curse: discard 2 Energy from this Pokémon, then put 8 damage counters on the opponent's Pokémon in any way you like.</summary>
        private sealed class BurstCurseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burst Curse") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, "Burst Curse: discard 2 Energy from " + me.Name);
                ctx.PlaceCountersAnyWay(8, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Slightly Simmer: search your deck for up to 2 cards and discard them; shuffle.</summary>
        private sealed class SlightlySimmerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slightly Simmer") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => true, 2, "Slightly Simmer: choose up to 2 cards to discard", false);
                foreach (var card in found) ctx.Me.Discard.Add(card);
                if (found.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", found.Select(x => x.Def.Name).ToArray()) + " from " + ctx.Engine.Their(ctx.Player) + " deck (Slightly Simmer).");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Mirror Mirror: each player draws or discards until they have 4 cards in their hand; the opponent does it first.</summary>
        private sealed class MirrorMirrorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Mirror") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ToFour(ctx, ctx.Opponent);
                ToFour(ctx, ctx.Player);
                return true;
            }

            private static void ToFour(EffectContext ctx, int player)
            {
                var p = ctx.State.Players[player];
                if (p.Hand.Count < 4) ctx.Engine.DrawCards(player, 4 - p.Hand.Count);
                else if (p.Hand.Count > 4)
                {
                    int n = p.Hand.Count - 4;
                    var picks = ctx.Engine.ChooseCards(player, p.Hand.ToList(), n, n, "Mirror Mirror: discard " + n + " cards from your hand", ctx.Source);
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(player, card);
                }
            }
        }

        /// <summary>
        /// Sinister Selection: once during your turn (before your attack), look at the top 2 cards of your deck and put 1 of them into
        /// your hand (the other stays on top). Past Friends: 10 more for each Supporter card in your discard pile.
        /// </summary>
        private sealed class ChandelureSTS : Mon
        {
            protected override string Activated { get { return "Sinister Selection"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return;
                var pick = ctx.ChooseCards(top, 1, 1, "Sinister Selection: put 1 of the top 2 cards of your deck into your hand");
                ctx.DeckToHand(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Past Friends") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.Me.Discard.Count(c => c.Def.IsSupporter));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Focus Energy: during your next turn, this Pokémon's Flop has a base damage of 50 (it used Focus Energy during your last turn).</summary>
        private sealed class MankeySTS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus Energy")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name != "Flop") return false;
                bool focused = me.LastAttackName == "Focus Energy" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(focused ? 50 : atk.Damage);
                return true;
            }
        }

        /// <summary>Spirited Throw: 60 more if, before doing damage, the opponent's Active Pokémon has more remaining HP than this Pokémon. Used for Spirited Throw, Trap Bolt (the attack name and the numbers are passed in).</summary>
        internal sealed class SpiritedThrowMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SpiritedThrowMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool more = defending != null && ctx.RemainingHp(defending) > ctx.RemainingHp(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage + (more ? amount : 0));
                return true;
            }
        }

        /// <summary>Restored Barrier: your Restored Pokémon have no Weakness. X-Scissor: flip, 20 more.</summary>
        private sealed class AnorithSTS : Mon
        {
            private static readonly Mon Scissor = new FlipBonusMon("X-Scissor", 20);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner == src.Player && mon.Def.Stage == Stage.Restored;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Scissor.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Up: if the Defending Pokémon is Poisoned, 3 more damage counters on it between turns (it stacks: Compendium).</summary>
        private sealed class PoisonUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Up") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Has(SpecialCondition.Poisoned) && ctx.CanAffect(defending))
                {
                    defending.PoisonCounterBase += 3;
                    ctx.Log(defending.Name + " now takes " + defending.PoisonCounterBase + " damage counters from Poison between turns (Poison Up).");
                }
                return true;
            }
        }

        /// <summary>Poison Enzyme: prevent all damage done to this Pokémon by attacks from the opponent's Poisoned Pokémon. Poison Jab: flip, Poisoned.</summary>
        private sealed class ToxicroakSTS : Mon
        {
            private static readonly Mon Jab = new CardsPAR2.FlipConditionMon("Poison Jab", SpecialCondition.Poisoned);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                return info.Target == src.Pokemon && attacker != null && attacker.Owner != src.Player && src.Engine.CurrentSourceKind == EffectSourceKind.Attack
                       && attacker.Has(SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Jab.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Tear Away: as often as you like during your turn (before your attack), put a Pokémon Tool attached to 1 of your Pokémon into your hand.</summary>
        private sealed class WeavileSTS : Mon
        {
            private const string TearAway = "Tear Away";

            private static List<PokemonInPlay> WithTools(EffectContext ctx) { return ctx.MyPokemon(m => m.Tools.Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == TearAway && ctx.IsMyTurn && WithTools(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var holders = WithTools(ctx);
                if (holders.Count == 0) return;
                var holder = ctx.ChoosePokemon(holders, "Tear Away: take a Pokémon Tool from which of your Pokémon?");
                if (holder == null) return;
                var tool = ctx.ChooseCards(holder.Tools.ToList(), 1, 1, "Tear Away: which Pokémon Tool?")[0];
                holder.Tools.Remove(tool);
                ctx.Me.Hand.Add(tool);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + tool.Def.Name + " from " + holder.Name + " into " + ctx.Engine.Their(ctx.Player) + " hand (Tear Away).");
            }
        }

        /// <summary>Damage Play: move any number of damage counters on the opponent's Benched Pokémon to their other Pokémon in any way you like.</summary>
        private sealed class DamagePlayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Play") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int round = 0; round < 12; round++)
                {
                    var damaged = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                    if (damaged.Count == 0 || ctx.OpponentPokemon().Count < 2) break;
                    if (!ctx.AskYesNo(round == 0 ? "Damage Play: move damage counters from your opponent's Benched Pokémon?" : "Damage Play: move more damage counters?")) break;
                    var source = ctx.ChoosePokemon(damaged, "Damage Play: move damage counters from which of your opponent's Benched Pokémon?");
                    if (source == null) break;
                    var dest = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != source), "Damage Play: move them to which of your opponent's Pokémon?");
                    if (dest == null) break;
                    int n = ctx.AskNumber(1, source.DamageCounters, "Damage Play: move how many damage counters?");
                    ctx.Engine.MoveDamageCounters(source, dest, n, "Damage Play");
                }
                return true;
            }
        }

        /// <summary>Retaliate: 60 more if any of your Pokémon were Knocked Out by damage from an opponent's attack during their last turn. Used for Retaliate, Vengeful Fang (the attack name and the numbers are passed in).</summary>
        internal sealed class RetaliateMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public RetaliateMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutByAttackLastOpponentTurn() ? amount : 0));
                return true;
            }
        }

        /// <summary>Baleful Night: 30 damage to each of the opponent's Benched Pokémon that has any damage counters on it.</summary>
        private sealed class BalefulNightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Baleful Night") return false;
                var hurt = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var target in hurt)
                    if (ctx.InPlay(target)) ctx.DamageTarget(target, 30);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Counter Head: during the opponent's next turn, if this Pokémon is damaged by an attack (even if Knocked Out), as many damage counters on the Attacking Pokémon.</summary>
        private sealed class CounterHeadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Counter Head") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new CardsSVP.MirrorDamageEffect(ctx.Pokemon.Name + " returns the damage it takes as damage counters (Counter Head)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Fortress of Rage: 10 more for each of your Benched Pokémon that has any damage counters on it.</summary>
        private sealed class FortressOfRageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fortress of Rage") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.Me.Bench.Count(m => m.DamageCounters > 0));
                return true;
            }
        }

        /// <summary>Heavy Bumper: damage from the opponent's attacks is reduced by 10 for each [C] in their Active Pokémon's Retreat Cost (after W/R). Gear Spinner: 70 more next turn.</summary>
        private sealed class KlinklangSTS : Mon
        {
            private static readonly Mon Spinner = new CardsASC.NextTurnBoostMon("Gear Spinner", 70);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || damage <= 0) return damage;
                var theirs = src.Opp.Active;
                if (theirs == null) return damage;
                return Math.Max(0, damage - 10 * src.Engine.RetreatCostOf(theirs));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spinner.HandleAttack(ctx, atk); }
        }

        /// <summary>Quick Guard: prevent all damage from Basic Pokémon's attacks to this Pokémon next turn; it can't use Quick Guard during your next turn.</summary>
        private sealed class QuickGuardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Guard") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.AddEffect(new CardsMEG.BasicShieldEffect(me.Name + " takes no damage from Basic Pokémon's attacks during the opponent's next turn (Quick Guard)", ctx.Player, me.Uid, ctx.Turn + 1));
                ctx.SelfCantUseAttackNextTurn("Quick Guard");
                return true;
            }
        }

        /// <summary>
        /// Mystic Heart: prevent all effects of the opponent's attacks, except damage, done to each of your Pokémon that has any Metal
        /// Energy attached (damage counters an attack puts on are effects, so they're prevented). Soul Blaster: if it used Soul Blaster
        /// during your last turn, its base damage is 60.
        /// </summary>
        private sealed class MagearnaEX : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                // EffectsPrevented has already checked that the attack is the opponent's. Effects an attack left behind (Shell
                // Trap's counters during your turn) have no source Pokémon and are still prevented (Compendium, 2017-08-03).
                if (target == null || target.Owner != src.Player || kind != EffectSourceKind.Attack) return false;
                return target.Energy.Any(en => CardsHIF1.Provides(src, target, en, EnergyType.Metal));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Soul Blaster") return false;
                var me = ctx.Pokemon;
                bool tired = me.LastAttackName == "Soul Blaster" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(tired ? 60 : atk.Damage);
                return true;
            }
        }
    }
}
