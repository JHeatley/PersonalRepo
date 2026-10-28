using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Discovery (N2 1-75, 2001; v139). Unlimited only. The non-holo prints 21-31 and 33-36 are prints of 2-12 and 14-17.
    /// Pokémon Powers with the "Asleep, Confused or Paralyzed" line stop while their Pokémon has one of those; the ones without it
    /// (Unown [A], [F], [U], [O], [E], [I] and Igglybuff's Gaze) keep working (your ruling: follow the text). Igglybuff and Tyrogue
    /// are Baby Pokémon. The Unown: up to 4 Basic Pokémon with Unown in their names in a deck that has one (Decklist, your ruling).
    /// Your rulings: Shockwave and Hatch do their "then" part on heads only; Eevee's Energy Evolution reacts to any Energy card you
    /// attach, at any time; Counter's damage gets Weakness and Resistance; Frog Song's 40 comes after Weakness and Resistance.
    /// Pokémon whose attacks only do their printed damage: Hoppip 55, Larvitar 57.
    /// </summary>
    public static class CardsN2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N2-1", new PsychicPlusMon());
            r.RegisterByKey("N2-2", new SpikesMon());
            r.RegisterByKey("N2-3", new ComboMon(new DodgeMon("Detect", false), new CoinsMon("Triple Kick", 3)));
            r.RegisterByKey("N2-4", new ComboMon(new CardsCES2.ScreechMon("Crunch", 20), new DiscardOrNothingMon("Flamethrower", EnergyType.Fire, false)));
            r.RegisterByKey("N2-5", new ComboMon(new CardsPAR2.FlipConditionMon("Smog", SpecialCondition.Poisoned), new CardsOBF1.AttachFromDiscardToSelfMon("Collect Fire", EnergyType.Fire)));
            r.RegisterByKey("N2-6", new HydrocutterMon());
            r.RegisterByKey("N2-7", new LockOnMon());
            r.RegisterByKey("N2-8", new FrogSongMon());
            r.RegisterByKey("N2-9", new RecoilMon("Submission", 20));
            r.RegisterByKey("N2-10", new ComboMon(new FalseSwipeMon(), new CoinsMon("Double Claw", 2)));
            r.RegisterByKey("N2-11", new SketchMon());
            r.RegisterByKey("N2-12", new ComboMon(new CoinsMon("Slam", 2), new TrampleMon()));
            r.RegisterByKey("N2-13", new CardsCES1.FeintAttackMon("Feint Attack", 30));
            r.RegisterByKey("N2-14", new AngerUnown());
            r.RegisterByKey("N2-15", new ComboMon(new HeadpressMon(), new CoinsMon("Double Lariat", 2)));
            r.RegisterByKey("N2-16", new CardsG1.CounterMon("Counter", 1, true, true));
            r.RegisterByKey("N2-17", new ComboMon(new ShockwaveMon(), new CardsCRZ1.PiercingMon("Swift")));
            r.RegisterByKey("N2-18", new ComboMon(new TriplePoisonMon(), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("N2-19", new ComboMon(new MagicDustMon(), new HyperReverseMon()));
            r.RegisterByKey("N2-20", new ComboMon(new FlipBonusMon("Quick Attack", 20), new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("N2-32", new ComboMon(new FlipBonusMon("Quick Attack", 20), new PursuitMon()));
            r.RegisterByKey("N2-37", new ComboMon(new DiscardOrNothingMon("Recover", EnergyType.Water, true), new CoinsMon("Spike Cannon", 2)));
            r.RegisterByKey("N2-38", new EnergyEvolutionMon());
            r.RegisterByKey("N2-39", new CardsSUM4.DiscardDefendingToolsMon("Plunder"));
            r.RegisterByKey("N2-40", new GazeMon());
            r.RegisterByKey("N2-41", new SecretePoisonMon());
            r.RegisterByKey("N2-42", new ComboMon(new CardsLOR1.HardenMon("Harden", 20), new HatchMon()));
            r.RegisterByKey("N2-43", new ComboMon(new CardsPLS1.FlipBonusConditionMon("Squeeze", 20, SpecialCondition.Paralyzed), new SpikeBarrageMon()));
            r.RegisterByKey("N2-44", new ComboMon(new BellyDrumMon(), new CardsBS1.ExtraWaterMon("Water Gun")));
            r.RegisterByKey("N2-45", new DustDevilMon());
            r.RegisterByKey("N2-46", new FuryCutterMon());
            r.RegisterByKey("N2-47", new GuardUnown("[Darkness]", "Unown [D]", EnergyType.Darkness));
            r.RegisterByKey("N2-48", new FindUnown());
            r.RegisterByKey("N2-49", new GuardUnown("[Metal]", "Unown [M]", EnergyType.Metal));
            r.RegisterByKey("N2-50", new GuardUnown("[Normal]", "Unown [N]", EnergyType.Colorless));
            r.RegisterByKey("N2-51", new UndoUnown());
            r.RegisterByKey("N2-52", new EnergyCycleMon());
            r.RegisterByKey("N2-53", new RecoilMon("Spin Tackle", 10));
            r.RegisterByKey("N2-54", new ParalyzeFlipMon("Glare"));
            r.RegisterByKey("N2-55", new PlainMon());
            r.RegisterByKey("N2-56", new ReviveMon("Revive Friends", d => d.Name == "Kabuto", "a card named Kabuto"));
            r.RegisterByKey("N2-57", new PlainMon());
            r.RegisterByKey("N2-58", new CardsFST1.FlipRecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("N2-59", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("N2-60", new ReviveMon("Revive Fossil", EvolvesFromMysteriousFossil, "a card that evolves from Mysterious Fossil"));
            r.RegisterByKey("N2-61", new BurstMon());
            r.RegisterByKey("N2-62", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("N2-63", new ScoutMon());
            r.RegisterByKey("N2-64", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("N2-65", new NapTimeMon());
            r.RegisterByKey("N2-66", new TailsNothingMon("Smash Punch"));
            r.RegisterByKey("N2-67", new EngageUnown());
            r.RegisterByKey("N2-68", new IncreaseUnown());
            r.RegisterByKey("N2-69", new ObserveUnown());
            r.RegisterByKey("N2-70", new SpikeStabMon());
            r.RegisterByKey("N2-71", new SlimeMon());
        }

        // ================================================================== Shared

        private static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        /// <summary>A card that evolves from Mysterious Fossil (Omanyte, Kabuto, Aerodactyl).</summary>
        internal static bool EvolvesFromMysteriousFossil(CardDef d) { return d != null && d.IsPokemon && d.EvolvesFrom == "Mysterious Fossil"; }

        /// <summary>
        /// The letter of an Unown: "Unown [O]" (Neo Discovery) and "Unown O" (the Wizards promo's way) are both O, so Unown J's
        /// Join and the Unown [F]/[U] Powers count either kind. Null for anything else.
        /// </summary>
        internal static string UnownLetter(string name)
        {
            if (string.IsNullOrEmpty(name) || !name.StartsWith("Unown ", StringComparison.Ordinal)) return null;
            string rest = name.Substring(6).Trim().Trim('[', ']').Trim();
            return rest.Length == 1 ? rest : null;
        }

        /// <summary>The player has an Unown of each of these letters on their Bench.</summary>
        internal static bool UnownOnBench(EffectContext ctx, params string[] letters)
        {
            var have = new HashSet<string>(ctx.Me.Bench.Select(b => UnownLetter(b.Name)).Where(l => l != null));
            return letters.All(have.Contains);
        }

        /// <summary>"Discard 1 [type] Energy card attached to this Pokémon or this attack does nothing" (Flamethrower; Recover then heals it all).</summary>
        private sealed class DiscardOrNothingMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly bool healAll;

            public DiscardOrNothingMon(string attack, EnergyType type, bool healAll)
            {
                this.attack = attack;
                this.type = type;
                this.healAll = healAll;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var cards = me == null ? new List<CardInstance>() : me.Energy.Where(e => e.Def.IsEnergy && e.Def.EnergyProvides == type).ToList();
                    if (cards.Count == 0) return Nothing(ctx);
                    var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, attack + ": discard which " + type + " Energy card?")[0];
                    if (!ctx.Engine.DiscardEnergy(me, card)) return Nothing(ctx);
                }
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (healAll && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                return true;
            }
        }

        // ================================================================== Holo Pokémon

        /// <summary>Psychic: 30 + 10 for each Energy card attached to the Defending Pokémon.</summary>
        internal sealed class PsychicPlusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 10 * (defending == null ? 0 : defending.Energy.Count));
                return true;
            }
        }

        /// <summary>
        /// Spikes (Pokémon Power): during the opponent's turn, whenever 1 of their Benched Pokémon becomes their Active Pokémon (a
        /// switch, a retreat or a promotion), 10 damage to it, no Weakness or Resistance. Rapid Spin: 30, they switch, then you switch.
        /// </summary>
        private sealed class SpikesMon : Mon
        {
            private static readonly Mon RapidSpin = new CardsWP.HitmontopWP();

            public override void OnOpponentBenchedBecameActive(EffectContext src, int player, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (me == null || player == src.Player || mon == null || src.State.Players[player].Active != mon || !PowerWorks(me)) return;
                if (src.Engine.AbilityBlocked(me, "Spikes")) return;
                Announce(src, "Spikes");
                src.Engine.DealDamage(new DamageInfo { Target = mon, ApplyWeakness = false, ApplyResistance = false }, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return RapidSpin.HandleAttack(ctx, atk); }
        }

        /// <summary>Hydrocutter: a coin for each Energy card attached to this Pokémon (3 at most), 40 per heads.</summary>
        private sealed class HydrocutterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydrocutter") return false;
                int coins = Math.Min(3, ctx.Pokemon == null ? 0 : ctx.Pokemon.Energy.Count);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>
        /// Lock-on: during your next turn, tails flipped for this Pokémon's Electric Bolt count as heads (benching or evolving either
        /// Pokémon ends it). Electric Bolt: 2 coins; both heads, Paralyzed; any tails, nothing at all.
        /// </summary>
        private sealed class LockOnMon : Mon
        {
            private const string LockOn = "Lock-on";

            private static bool LockedOn(EffectContext ctx, PokemonInPlay me)
            {
                if (me == null) return false;
                foreach (var e in ctx.State.Effects)
                {
                    var marker = e as LockOnEffect;
                    if (marker == null || marker.TargetUid != me.Uid || marker.Owner != ctx.Player || marker.ActiveFromTurn > ctx.Turn) continue;
                    var target = ctx.State.FindPokemon(marker.DefendingUid);
                    if (target == null) continue;
                    foreach (var a in ctx.State.Effects)
                        if (a is LockOnAnchor && a.TargetUid == marker.DefendingUid && a.Name == marker.Name) return true;
                }
                return false;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (atk.Name == LockOn)
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me) || defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                    string name = me.Name + "'s Electric Bolt locks on " + defending.Name + " during " + ctx.Engine.Their(ctx.Player) + " next turn (Lock-on)";
                    if (!ctx.AddEffect(new LockOnAnchor(name, ctx.Player, defending.Uid, ctx.Turn + 2))) return true;
                    ctx.AddEffect(new LockOnEffect(name, ctx.Player, me.Uid, ctx.Turn + 2, defending.Uid) { ActiveFromTurn = ctx.Turn + 2 });
                    return true;
                }
                if (atk.Name != "Electric Bolt") return false;
                bool locked = LockedOn(ctx, me);
                int heads = ctx.FlipCoins(2);
                if (locked && heads < 2)
                {
                    ctx.Log("Lock-on: the tails count as heads.");
                    heads = 2;
                }
                if (heads < 2) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        private sealed class LockOnEffect : ContinuousEffect
        {
            public readonly int DefendingUid;

            public LockOnEffect(string name, int owner, int targetUid, int expiresAfterTurn, int defendingUid) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                DefendingUid = defendingUid;
            }
        }

        /// <summary>Lock-on's other half, on the Defending Pokémon: benching or evolving it ends the effect.</summary>
        private sealed class LockOnAnchor : ContinuousEffect
        {
            public LockOnAnchor(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }
        }

        /// <summary>
        /// Frog Song (Pokémon Power): whenever this Pokémon's attack damages the Defending Pokémon, if there are more than 3 Poliwag,
        /// Poliwhirl, Poliwrath and/or Politoed in play (both sides), that attack does 40 more damage, after Weakness and Resistance
        /// (your ruling). Doubleslap: 2 coins, 40 per heads.
        /// </summary>
        private sealed class FrogSongMon : Mon
        {
            private static readonly string[] Family = { "Poliwag", "Poliwhirl", "Poliwrath", "Politoed" };
            private static readonly Mon Doubleslap = new CoinsMon("Doubleslap", 2);

            internal static int FamilyInPlay(GameEngine engine)
            {
                int n = 0;
                for (int i = 0; i < 2; i++) n += engine.State.Players[i].InPlay().Count(m => Family.Contains(m.Name));
                return n;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || damage <= 0 || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !PowerWorks(me) || FamilyInPlay(src.Engine) <= 3 || !info.ApplyOnce("Frog Song")) return damage;
                src.Log("Frog Song: 40 more damage.");
                return damage + 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Doubleslap.HandleAttack(ctx, atk); }
        }

        /// <summary>False Swipe: damage equal to half the Defending Pokémon's remaining HP, rounded down to the nearest 10.</summary>
        private sealed class FalseSwipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "False Swipe") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int damage = Math.Max(0, ctx.RemainingHp(defending)) / 2 / 10 * 10;
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log("False Swipe does no damage.");
                return true;
            }
        }

        /// <summary>
        /// Sketch: if the Defending Pokémon attacked during the opponent's last turn while this Pokémon was in play, this Pokémon uses
        /// that attack without its Energy cost or anything else needed to use it (like Metronome).
        /// </summary>
        private sealed class SketchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sketch") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                var opp = ctx.Opp;
                var source = opp.LastAttackSourceDef;
                int index = opp.LastAttackIndex;
                bool attacked = defending != null && opp.LastAttackTurn == ctx.OpponentsLastTurn && opp.LastAttackerUid == defending.Uid
                                && source != null && index >= 0 && index < source.Attacks.Count;
                if (!attacked || me == null || me.TurnEnteredPlay >= opp.LastAttackTurn)
                {
                    ctx.Log("Sketch: the Defending Pokémon didn't attack last turn while " + (me == null ? "this Pokémon" : me.Name) + " was in play.");
                    return true;
                }
                if (source.Attacks[index].Name == "Sketch")
                {
                    ctx.Log("Sketch: copying Sketch does nothing.");
                    return true;
                }
                CardsBS1.CopyingWithMetronome = true;
                try { UseCopiedAttack(ctx, source, index); }
                finally { CardsBS1.CopyingWithMetronome = false; }
                return true;
            }
        }

        /// <summary>Trample: a coin for each Benched Pokémon (both sides); 30 to each one with heads (no Weakness or Resistance on the Bench).</summary>
        private sealed class TrampleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trample") return false;
                ctx.DamageDefending(atk.Damage);
                var hits = new List<PokemonInPlay>();
                foreach (var benched in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.Engine.FlipCoin(ctx.Player, "Trample (" + benched.Name + ")")) hits.Add(benched);
                foreach (var benched in hits)
                    if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 30);
                return true;
            }
        }

        /// <summary>Headpress: heads, the damage and Paralyzed; tails, nothing at all.</summary>
        private sealed class HeadpressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Headpress") return false;
                var defending = ctx.Defending;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Shockwave: heads, 10 damage to each of the opponent's Pokémon (no Weakness or Resistance), then they switch the Defending
        /// Pokémon with 1 of their Benched Pokémon (they choose). Tails, nothing (your ruling).
        /// </summary>
        private sealed class ShockwaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shockwave") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                foreach (var mon in ctx.OpponentPokemon())
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 10, false, false, false);
                ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Triple Poison: heads, Poisoned with 3 damage counters after each player's turn (even if it was already Poisoned).</summary>
        private sealed class TriplePoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (ctx.ApplyCondition(defending, SpecialCondition.Poisoned) || defending.Has(SpecialCondition.Poisoned)) defending.PoisonCounterBase = 3;
                return true;
            }
        }

        /// <summary>Magic Dust: heads, the Defending Pokémon is now Asleep, Confused, Paralyzed or Poisoned (your choice).</summary>
        private sealed class MagicDustMon : Mon
        {
            private static readonly SpecialCondition[] Choices = { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magic Dust") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                int pick = ctx.ChooseOption("Magic Dust: the Defending Pokémon is now...", "Asleep", "Confused", "Paralyzed", "Poisoned");
                if (pick < 0 || pick >= Choices.Length) pick = 0;
                ctx.ApplyCondition(defending, Choices[pick]);
                return true;
            }
        }

        /// <summary>Hyper Reverse: 10 for each Energy attached to the Defending Pokémon, then heal this Pokémon by the damage done.</summary>
        private sealed class HyperReverseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Reverse") return false;
                var defending = ctx.Defending;
                int energy = defending == null ? 0 : ctx.EnergyCount(defending);
                int done = energy > 0 ? ctx.DamageDefending(atk.Damage * energy) : 0;
                var me = ctx.Pokemon;
                if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, done);
                return true;
            }
        }

        // ================================================================== Unown

        /// <summary>
        /// Hidden Power (every Neo Discovery Unown) with Unown [A]'s [Anger]: 10 more for each damage counter on your Unown [A]. With
        /// more than 1 Unown [A] in play only 1 [Anger] counts (the one with the most counters). [Anger] has no Asleep, Confused or
        /// Paralyzed clause, so those don't stop it (your ruling).
        /// </summary>
        internal static int AngerBonus(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            if (me == null || me.Name.IndexOf("Unown", StringComparison.Ordinal) < 0) return 0;
            int best = 0;
            PokemonInPlay angry = null;
            foreach (var a in ctx.MyPokemon(m => m.Name == "Unown [A]"))
            {
                if (ctx.Engine.AbilityBlocked(a, "[Anger]") || a.DamageCounters <= best) continue;
                best = a.DamageCounters;
                angry = a;
            }
            if (angry == null) return 0;
            ctx.Log("[Anger] (" + angry.Name + "): " + 10 * best + " more damage.");
            return 10 * best;
        }

        /// <summary>
        /// v142: Unown [X]'s [XXXXX] (Neo Destiny): whenever 1 of your Pokémon with Unown in its name uses Hidden Power, flip a coin
        /// until you get tails; 10 more damage for each heads. With more than 1 Unown [X] in play only 1 [XXXXX] works. No Asleep,
        /// Confused or Paralyzed clause, so those don't stop it (your ruling for such Unown).
        /// </summary>
        internal static int XxxxxBonus(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            if (me == null || me.Name.IndexOf("Unown", StringComparison.Ordinal) < 0) return 0;
            PokemonInPlay x = null;
            foreach (var m in ctx.MyPokemon(u => u.Name == "Unown [X]"))
                if (!ctx.Engine.AbilityBlocked(m, "[XXXXX]")) { x = m; break; }
            if (x == null) return 0;
            int heads = ctx.FlipUntilTails();
            ctx.Log("[XXXXX] (" + x.Name + "): " + 10 * heads + " more damage.");
            return 10 * heads;
        }

        internal static bool HiddenPower(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk, 0); }

        /// <summary>Hidden Power with [Anger] and [XXXXX] added, plus extra (Unown [P]'s [Perform]).</summary>
        internal static bool HiddenPower(EffectContext ctx, AttackDef atk, int extra)
        {
            if (atk.Name != "Hidden Power") return false;
            int anger = AngerBonus(ctx);
            int x = XxxxxBonus(ctx);
            ctx.DamageDefending(atk.Damage + extra + anger + x);
            return true;
        }

        /// <summary>Unown [A]: [Anger] works through Hidden Power (AngerBonus).</summary>
        private sealed class AngerUnown : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        /// <summary>
        /// Unown [D] / [M] / [N]: whenever a Darkness / Metal / Colorless Pokémon damages 1 of your Pokémon, 30 less (after Weakness
        /// and Resistance). It stops if you have more than 1 of that Unown in play; Asleep, Confused or Paralyzed don't stop it.
        /// </summary>
        private sealed class GuardUnown : Mon
        {
            private readonly string power;
            private readonly string unown;
            private readonly EnergyType type;

            public GuardUnown(string power, string unown, EnergyType type)
            {
                this.power = power;
                this.unown = unown;
                this.type = type;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || damage <= 0 || info.Attacker == null || info.Target == null || info.Target.Owner != src.Player) return damage;
                if (!src.Engine.IsType(info.Attacker, type) || src.MyPokemon(m => m.Name == unown).Count != 1 || !info.ApplyOnce(power)) return damage;
                src.Log(power + " (" + src.Pokemon.Name + "): 30 less damage.");
                return Math.Max(0, damage - 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        /// <summary>[Find]: once during your turn, with Unown F, I, N and D on your Bench, put a Trainer card from your deck into your hand.</summary>
        private sealed class FindUnown : Mon
        {
            protected override string Activated { get { return "[Find]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && UnownOnBench(ctx, "F", "I", "N", "D"); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTrainer, 1, "[Find]: choose a Trainer card to put into your hand");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        /// <summary>[Undo]: once during your turn, with Unown U, N, D and O on your Bench, return your Active Pokémon and its cards to your hand.</summary>
        private sealed class UndoUnown : Mon
        {
            protected override string Activated { get { return "[Undo]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && UnownOnBench(ctx, "U", "N", "D", "O"); }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active != null) ctx.Engine.ReturnPokemonToHand(active);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        /// <summary>[Observe]: once during your turn, look at the top 5 cards of your opponent's deck (they stay in the same order).</summary>
        private sealed class ObserveUnown : Mon
        {
            protected override string Activated { get { return "[Observe]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.Opp.Deck.Take(5).ToList();
                ctx.Engine.ShowCards(ctx.Player, top, "Top of your opponent's deck", "[Observe]: the top " + top.Count + " cards of your opponent's deck", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top " + top.Count + " cards of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck ([Observe]).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        /// <summary>
        /// [Engage]: when you play this Pokémon from your hand, your opponent may shuffle their hand into their deck and draw 4 cards;
        /// either way, you may do the same.
        /// </summary>
        private sealed class EngageUnown : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Pokemon == null || ctx.Engine.AbilityBlocked(ctx.Pokemon, "[Engage]")) return;
                Announce(ctx, "[Engage]");
                if (ctx.Engine.AskYesNo(ctx.Opponent, "[Engage]: shuffle your hand into your deck and draw 4 cards?", ctx.Source))
                {
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                    ctx.Engine.DrawCards(ctx.Opponent, 4);
                }
                if (ctx.AskYesNo("[Engage]: shuffle your hand into your deck and draw 4 cards?"))
                {
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    ctx.Draw(4);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        /// <summary>[Increase]: when you play this Pokémon from your hand, you may put a Basic Pokémon with Unown in its name from your deck onto your Bench.</summary>
        private sealed class IncreaseUnown : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Pokemon == null || ctx.Engine.AbilityBlocked(ctx.Pokemon, "[Increase]") || ctx.BenchSpace <= 0 || ctx.Me.Deck.Count == 0) return;
                if (!ctx.AskYesNo("[Increase]: put an Unown from your deck onto your Bench?")) return;
                Announce(ctx, "[Increase]");
                ctx.SearchToBench(d => d.Name.IndexOf("Unown", StringComparison.Ordinal) >= 0, 1, "[Increase]: choose a card with Unown in its name");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HiddenPower(ctx, atk); }
        }

        // ================================================================== Other Pokémon

        /// <summary>Pursuit: during the opponent's next turn, if the Defending Pokémon tries to retreat, 10 damage to it (no Weakness or Resistance).</summary>
        private sealed class PursuitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pursuit") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.AddEffect(new PursuitEffect(defending.Name + " takes 10 damage if it tries to retreat during its next turn (Pursuit)", ctx.Player, defending.Uid, ctx.Turn + 1) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class PursuitEffect : ContinuousEffect
        {
            public PursuitEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnRetreatTried(GameEngine engine, PokemonInPlay mon)
            {
                if (mon == null || mon.Uid != TargetUid) return;
                engine.Emit(GameEventType.Info, Owner, "Pursuit: " + mon.Name + " tried to retreat.", mon.Top.Id, mon.Uid);
                engine.DealDamage(new DamageInfo { Target = mon, ApplyWeakness = false, ApplyResistance = false }, 10);
            }
        }

        /// <summary>
        /// Energy Evolution (Pokémon Power): whenever you attach an Energy card to this Pokémon (any way, any time: your ruling), flip
        /// a coin; heads, evolve it with a card from your deck that evolves from Eevee and is that Energy's type.
        /// </summary>
        private sealed class EnergyEvolutionMon : Mon
        {
            private const string EnergyEvolution = "Energy Evolution";

            public override void OnOwnerAttachedEnergyCard(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || me.Top != src.Card || !PowerWorks(me) || src.Engine.AbilityBlocked(me, EnergyEvolution) || src.Me.Deck.Count == 0) return;
                var type = energy.Def.EnergyProvides;
                Announce(src, EnergyEvolution);
                if (!src.Flip()) return;
                if (CardsEVS1.EvolveBlocked(src, me))
                {
                    src.Log(me.Name + " can't evolve right now (" + EnergyEvolution + ").");
                    return;
                }
                var found = src.SearchMyDeck(d => CardsEVS1.EvolvesOnto(src, me, d) && d.EvolvesFrom == "Eevee" && (d.PokemonType == type || d.SecondType == type), 1,
                    EnergyEvolution + ": choose a " + type + " card that evolves from Eevee", false);
                if (found.Count > 0 && src.InPlay(me)) src.Engine.EvolvePokemon(me, found[0]);
                else src.Me.Deck.AddRange(found);
                src.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Gaze (Pokémon Power): once during your turn, choose 1 of the opponent's Benched Pokémon with a Pokémon Power; that power
        /// stops until the end of the turn, or until that Pokémon leaves the Bench. No Asleep, Confused or Paralyzed clause.
        /// </summary>
        private sealed class GazeMon : Mon
        {
            private const string Gaze = "Gaze";

            protected override string Activated { get { return Gaze; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.Opp.Bench.Where(m => m.Def.Abilities.Any(a => a.Kind == AbilityDef.PokemonPower)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Gaze: stop the Pokémon Power of which of your opponent's Benched Pokémon?");
                if (target == null) return;
                ctx.AddEffect(new GazeEffect(target.Name + "'s Pokémon Power stops working this turn while it's on the Bench (Gaze)", ctx.Player, target.Uid, ctx.Turn, target.SpotSeq));
            }
        }

        private sealed class GazeEffect : ContinuousEffect
        {
            private readonly int spotSeq;

            public GazeEffect(string name, int owner, int targetUid, int expiresAfterTurn, int spotSeq) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.spotSeq = spotSeq;
            }

            public override bool StopsPokemonPower(GameEngine engine, PokemonInPlay mon, string powerName)
            {
                return mon != null && mon.Uid == TargetUid && mon.SpotSeq == spotSeq && engine.State.Players[mon.Owner].Bench.Contains(mon);
            }
        }

        /// <summary>
        /// Secrete Poison: during the opponent's next turn, whenever their attack damages this Pokémon (even if it's Knocked Out), their
        /// Active Pokémon is now Poisoned and this Pokémon does 10 damage to each of their Benched Pokémon.
        /// </summary>
        private sealed class SecretePoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Secrete Poison") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new SecretePoisonEffect(me.Name + " poisons the attacker when an attack damages it during the opponent's next turn (Secrete Poison)", ctx.Player, me.Uid, ctx.Turn + 1) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class SecretePoisonEffect : ContinuousEffect
        {
            public SecretePoisonEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                var me = info.Target;
                if (me == null || me.Uid != TargetUid || damage <= 0 || info.Attacker == null || info.Attacker.Owner == Owner) return;
                var opp = engine.State.Players[1 - Owner];
                engine.Emit(GameEventType.Info, Owner, "Secrete Poison (" + me.Name + ").", me.Top.Id, me.Uid);
                if (opp.Active != null) engine.ApplyCondition(opp.Active, SpecialCondition.Poisoned);
                foreach (var benched in opp.Bench.ToList())
                    engine.DealDamage(new DamageInfo { Target = benched, ApplyWeakness = false, ApplyResistance = false }, 10);
            }
        }

        /// <summary>Hatch: heads, remove all damage counters from this Pokémon, then evolve it with a card from your deck. Tails, nothing (your ruling).</summary>
        private sealed class HatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hatch") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                CardsEVS1.EvolveFromDeck(ctx, me, "Hatch");
                return true;
            }
        }

        /// <summary>Spike Barrage: a coin for each Water Energy attached to this Pokémon; 20 + 20 for each heads.</summary>
        private sealed class SpikeBarrageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spike Barrage") return false;
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 20 * heads);
                return true;
            }
        }

        /// <summary>Belly Drum: 3 damage counters on this Pokémon; if it isn't Knocked Out, attach 2 basic Energy cards from your deck to it.</summary>
        private sealed class BellyDrumMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Belly Drum") return false;
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.PlaceCounters(me, 3);
                if (!ctx.InPlay(me) || ctx.Engine.IsKnockedOut(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsEnergy && d.IsBasicEnergy, 2, "Belly Drum: choose up to 2 basic Energy cards to attach to " + me.Name);
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Dust Devil: 10 damage to each Pokémon in play that isn't a Fighting Pokémon (no Weakness or Resistance).</summary>
        private sealed class DustDevilMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dust Devil") return false;
                foreach (var mon in ctx.OpponentPokemon().Concat(ctx.MyPokemon()).ToList())
                    if (ctx.InPlay(mon) && !ctx.Engine.IsType(mon, EnergyType.Fighting)) ctx.DamageWith(mon, 10, false, false, false);
                return true;
            }
        }

        /// <summary>Fury Cutter: 4 coins; 10, plus 10 / 20 / 40 / 80 for exactly 1 / 2 / 3 / 4 heads.</summary>
        private sealed class FuryCutterMon : Mon
        {
            private static readonly int[] Bonus = { 0, 10, 20, 40, 80 };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(atk.Damage + Bonus[Math.Max(0, Math.Min(4, heads))]);
                return true;
            }
        }

        /// <summary>Energy Cycle: heads, move 1 Energy card from the Defending Pokémon to 1 of the opponent's Benched Pokémon (you choose both).</summary>
        private sealed class EnergyCycleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Cycle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var defending = ctx.Defending;
                var bench = ctx.OpponentBench();
                if (defending == null || defending.Energy.Count == 0 || bench.Count == 0) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Energy Cycle: move which Energy card?")[0];
                var to = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Energy Cycle: attach " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>
        /// Revive Friends / Revive Fossil (Pokémon Power): once during your turn, flip a coin; heads, put a card from your deck onto your
        /// Bench as a Basic Pokémon. Not while Asleep, Confused or Paralyzed, or with a full Bench.
        /// </summary>
        private sealed class ReviveMon : Mon
        {
            private static readonly Mon Kabuto = new WorkTogetherMon();
            private static readonly Mon Bind = new ParalyzeFlipMon("Bind");

            private readonly string power;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;

            public ReviveMon(string power, Func<CardDef, bool> filter, string what)
            {
                this.power = power;
                this.filter = filter;
                this.what = what;
            }

            protected override string Activated { get { return power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && filter(d) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, power + ": choose " + what + " to put onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Kabuto.HandleAttack(ctx, atk) || Bind.HandleAttack(ctx, atk); }
        }

        /// <summary>Work Together: heads, 10 + 10 for each Omanyte, Omastar, Kabuto and Kabutops on your Bench; tails, 10.</summary>
        private sealed class WorkTogetherMon : Mon
        {
            private static readonly string[] Names = { "Omanyte", "Omastar", "Kabuto", "Kabutops" };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Work Together") return false;
                int bonus = ctx.Flip() ? 10 * ctx.Me.Bench.Count(m => Names.Contains(m.Name)) : 0;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>
        /// Burst: flip a coin; heads, this Pokémon does 40 damage to itself and 10 to each Benched Pokémon (both sides). Flip a coin;
        /// tails, the attack does nothing at all.
        /// </summary>
        private sealed class BurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burst") return false;
                bool blast = ctx.Flip();
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (!blast) return true;
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(40);
                foreach (var benched in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>Scout: look at your opponent's hand.</summary>
        private sealed class ScoutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scout") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Scout: your opponent's hand", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand (Scout).");
                return true;
            }
        }

        /// <summary>Nap Time: remove all damage counters from this Pokémon; it's now Asleep.</summary>
        private sealed class NapTimeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nap Time") return false;
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Spike Stab: heads, the damage and Poisoned; tails, nothing at all.</summary>
        private sealed class SpikeStabMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spike Stab") return false;
                var defending = ctx.Defending;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Slime: during the opponent's next turn, an attack that would damage this Pokémon makes them flip a coin; tails, no damage.</summary>
        private sealed class SlimeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slime") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new SlimeEffect(me.Name + ": the opponent flips a coin when an attack would damage it during their next turn; tails, no damage (Slime)", ctx.Player, me.Uid, ctx.Turn + 1) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class SlimeEffect : ContinuousEffect
        {
            public SlimeEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var me = info.Target;
                if (me == null || me.Uid != TargetUid || info.Attacker == null || info.Attacker.Owner == Owner || damage <= 0) return damage;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (engine.FlipCoin(info.Attacker.Owner, "Slime")) return damage;
                engine.Emit(GameEventType.DamagePrevented, Owner, "Slime: the attack does no damage to " + me.Name + ".", me.Top.Id, me.Uid);
                return 0;
            }
        }
    }
}
