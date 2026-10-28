using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Generations (GEN, v95), part 1: the numbered Pokémon (1-58). Generations reprinted many XY-era cards; each one gets its key
    /// here (GEN-n), and the XY sets still to come link their printings to it.
    /// </summary>
    public static class CardsGEN1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("GEN-1", new ComboMon(new FlipBonusMon("Frog Hop", 40), new CardsCES1.ConditionsMon("Poison Impact", SpecialCondition.Asleep, SpecialCondition.Poisoned)));
            r.RegisterByKey("GEN-2", new BloomBusterMon());
            r.RegisterByKey("GEN-3", new CardsSVI1.AdaptiveEvolutionMon());
            r.RegisterByKey("GEN-4", new MetapodGEN());
            r.RegisterByKey("GEN-5", new QuiverDanceMon());
            r.RegisterByKey("GEN-6", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("GEN-7", new ComboMon(new ColorfulSporesMon(), new FlipBonusMon("X-Scissor", 30)));
            r.RegisterByKey("GEN-8", new CardsTWM1.HealSelfMon("Absorb", 20));
            r.RegisterByKey("GEN-9", new ComboMon(new CardsASR1.BenchRecoilMon("Overhead Throw", 20), new HeavySuplexMon()));
            r.RegisterByKey("GEN-10", new ComboMon(new FlipBonusMon("Leaf Blade", 30), new NaturesBreathMon()));
            r.RegisterByKey("GEN-11", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire), new CardsWHT.FlipMoreMon("Burning Breath", 2, 40)));
            r.RegisterByKey("GEN-12", new FlipPerEnergyMoreMon("Heat Typhoon", EnergyType.Fire, 50));
            r.RegisterByKey("GEN-13", new ComboMon(new FlareBonusMon(), new FlipTailsDiscardTypeMon("Fire Blast", EnergyType.Fire, 1)));
            r.RegisterByKey("GEN-14", new DodgeMon("Agility", false));
            r.RegisterByKey("GEN-15", new ComboMon(new DodgeMon("Agility", false), new Cards30CC.BenchHitMon("Overrun", 20)));
            r.RegisterByKey("GEN-16", new CardsSUM1.DiscardTypeFromSelfMon("Ember", EnergyType.Fire, 1));
            r.RegisterByKey("GEN-17", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Whirlpool"), new Cards30CC.BenchHitMon("Hydro Press", 20)));
            r.RegisterByKey("GEN-18", new FlipTailsDiscardTypeMon("Dread Launcher", EnergyType.Water, 2));
            r.RegisterByKey("GEN-20", new ComboMon(new ClampCrushMon(), new CoinsMon("Spike Cannon", 5)));
            r.RegisterByKey("GEN-22", new CardsBWP.BothHeadsOrNothingMon("Epic Splash"));
            r.RegisterByKey("GEN-23", new ComboMon(new BerserkerSplashMon(), new FlipPerEnergyMoreMon("Aqua Tail", EnergyType.Water, 30)));
            r.RegisterByKey("GEN-24", new ComboMon(new CardsTWM1.HealSelfMon("Bubble Drain", 30), new CardsUPR1.LessPerSelfCounterMon("Deep Squall", 10)));
            r.RegisterByKey("GEN-25", new SearchAttackMon("Find Ice", d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Water, 3));
            r.RegisterByKey("GEN-26", new ComboMon(new ParalyzeFlipMon("Nuzzle"), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("GEN-27", new ComboMon(new CardsASR1.TimesMon("Circle Circuit", c => c.Me.Bench.Count), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("GEN-28", new ComboMon(new CardsCRZ1.PiercingMon("Swift"), new CardsCRI.BasicShieldAttackMon("Flash Ray")));
            r.RegisterByKey("GEN-29", new CardsASR1.BenchRecoilMon("Raging Thunder", 40));
            r.RegisterByKey("GEN-30", new CardsMEG.SnipeMon("Skill Dive", 10));
            r.RegisterByKey("GEN-31", new GolbatGEN());
            r.RegisterByKey("GEN-32", new ComboMon(new CardsPAL1.FlipHealSelfMon("Spacing Out", 10), new ScavengeMon()));
            r.RegisterByKey("GEN-33", new CardsCES1.FlipConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned));
            r.RegisterByKey("GEN-34", new HaunterGEN());
            r.RegisterByKey("GEN-35", new ComboMon(new CardsUNB2.SinisterFogMon(), new CreepShowMon()));
            r.RegisterByKey("GEN-36", new JynxGEN());
            r.RegisterByKey("GEN-37", new MeowsticEX());
            r.RegisterByKey("GEN-38", new MineMon());
            r.RegisterByKey("GEN-39", new ComboMon(new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10), new CardsFLF1.NoResistanceMon("Rock Tumble")));
            r.RegisterByKey("GEN-42", new MachampGEN());
            r.RegisterByKey("GEN-43", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("GEN-44", new RecoilMon("Double-Edge", 20));
            r.RegisterByKey("GEN-45", new ComboMon(new FlipBonusMon("Stone Edge", 30), new RecoilMon("Explosion", 100)));
            r.RegisterByKey("GEN-46", new RecoilMon("Megaton Fall", 60));
            r.RegisterByKey("GEN-47", new CardsMEG.SnipeBenchMon("Stretch Kick", 30));
            r.RegisterByKey("GEN-48", new CardsWHT.FlipMoreMon("Bullet Punch", 2, 20));
            r.RegisterByKey("GEN-49", new RecoilMon("Take Down", 10));
            r.RegisterByKey("GEN-50", new CardsTWM1.HealSelfMon("Moonlight", 30));
            r.RegisterByKey("GEN-51", new ComboMon(new CardsPOR.FollowMeMon("Follow Me"), new CardsMEG.WeakenDefendingMon("Moonblast", 30)));
            r.RegisterByKey("GEN-52", new MrMimeGEN());
            r.RegisterByKey("GEN-53", new ExhaustedTackleMon());
            r.RegisterByKey("GEN-54", new ComboMon(new ParalyzeFlipMon("Fake Out"), new FlipBonusMon("Ambush", 30)));
            r.RegisterByKey("GEN-55", new ComboMon(new CoinsMon("Double Stab", 2), new DrawAttackMon("Doduo Delivery", 2)));
            r.RegisterByKey("GEN-56", new DodrioGEN());
            r.RegisterByKey("GEN-57", new ComboMon(new RecoilMon("Take Down", 10), new SeethingAngerMon()));
            r.RegisterByKey("GEN-58", new SnorlaxGEN());
        }

        // ================================================================== Shared helpers

        /// <summary>"Flip a coin for each [T] Energy attached to this Pokémon. This attack does N more damage for each heads" (Heat Typhoon, Aqua Tail).</summary>
        internal sealed class FlipPerEnergyMoreMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int per;

            public FlipPerEnergyMoreMon(string attack, EnergyType type, int per)
            {
                this.attack = attack;
                this.type = type;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, type);
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                ctx.DamageDefending(atk.Damage + per * heads);
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, discard N [T] Energy attached to this Pokémon" (Fire Blast, Dread Launcher).</summary>
        internal sealed class FlipTailsDiscardTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;

            public FlipTailsDiscardTypeMon(string attack, EnergyType type, int count)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads || !ctx.InPlay(me)) return true;
                var typed = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, type)).ToList();
                int n = Math.Min(count, typed.Count);
                if (n == 0) return true;
                var picks = n == typed.Count ? typed : ctx.ChooseCards(typed, n, n, attack + ": discard " + n + " " + type + " Energy from " + me.Name);
                foreach (var card in picks) ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Bloom Buster: flip a coin; heads, also 30 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class BloomBusterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bloom Buster") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                    foreach (var mon in ctx.OpponentBench())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Adaptive Evolution: it can evolve during your first turn or the turn you play it. Harden: no damage of 60 or less next turn.</summary>
        private sealed class MetapodGEN : Mon
        {
            private static readonly Mon Harden = new CardsLOR1.HardenMon("Harden", 60);

            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                return src.Pokemon != null && target == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Harden.HandleAttack(ctx, atk); }
        }

        /// <summary>Quiver Dance: search your deck for a basic Energy card and attach it to this Pokémon; if you did, heal 40 damage from it.</summary>
        private sealed class QuiverDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quiver Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Quiver Dance: choose a basic Energy card to attach to " + me.Name);
                bool attached = false;
                foreach (var card in found) attached |= ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                if (attached && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 40);
                return true;
            }
        }

        /// <summary>
        /// Colorful Spores: choose 3 of your Pokémon (fewer if that's all you have); for each, search your deck for a basic Energy card
        /// of a type not chosen yet and attach it to that Pokémon.
        /// </summary>
        private sealed class ColorfulSporesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Spores") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                int n = Math.Min(3, mine.Count);
                var targets = n == mine.Count ? mine : ctx.ChoosePokemonMany(mine, n, n, "Colorful Spores: choose 3 of your Pokémon");
                var used = new HashSet<EnergyType>();
                foreach (var target in targets)
                {
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && !used.Contains(d.EnergyProvides), 1,
                        "Colorful Spores: choose a basic Energy card of a different type to attach to " + target.Name);
                    foreach (var card in found)
                    {
                        used.Add(card.Def.EnergyProvides);
                        if (ctx.InPlay(target)) ctx.Engine.AttachEnergyCard(card, target);
                        else ctx.Me.Deck.Add(card);
                    }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Heavy Suplex: 20 more for each [C] in the opponent's Active Pokémon's Retreat Cost.</summary>
        private sealed class HeavySuplexMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Suplex") return false;
                int retreat = ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending);
                ctx.DamageDefending(atk.Damage + 20 * retreat);
                return true;
            }
        }

        /// <summary>Nature's Breath: if any Stadium is in play, 30 more damage and heal 30 damage from this Pokémon.</summary>
        private sealed class NaturesBreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nature's Breath") return false;
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? 30 : 0));
                if (stadium && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        // ================================================================== Fire / Water

        /// <summary>Flare Bonus: discard a Fire Energy card from your hand; if you do, draw 3 cards.</summary>
        private sealed class FlareBonusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Bonus") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fire = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fire).ToList();
                if (fire.Count == 0) return true;
                var card = ctx.ChooseCards(fire, 1, 1, "Flare Bonus: discard a Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(3);
                return true;
            }
        }

        /// <summary>Clamp Crush: flip a coin; heads, the opponent's Active Pokémon is Paralyzed and an Energy attached to it is discarded.</summary>
        private sealed class ClampCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clamp Crush") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (defending.Energy.Count > 0 && ctx.CanAffect(defending))
                {
                    var energy = defending.Energy.Count == 1 ? defending.Energy[0]
                        : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Clamp Crush: discard which Energy from " + defending.Name + "?")[0];
                    ctx.Engine.DiscardEnergy(defending, energy);
                }
                return true;
            }
        }

        /// <summary>Berserker Splash: also 10 damage to each Benched Pokémon, both yours and the opponent's.</summary>
        private sealed class BerserkerSplashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Berserker Splash") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench().Concat(ctx.MyBench()).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Sneaky Bite: when played from your hand to evolve, you may put 2 damage counters on 1 of the opponent's Pokémon. Swoop Across: 10 to each of their Pokémon.</summary>
        private sealed class GolbatGEN : Mon
        {
            private static readonly Mon Swoop = new CardsCEC3.SpreadAmountMon("Swoop Across", 10);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var targets = ctx.OpponentPokemon();
                if (!TriggerOnce(ctx, "Sneaky Bite", targets.Count > 0, "Sneaky Bite: put 2 damage counters on 1 of your opponent's Pokémon?")) return;
                var target = ctx.ChoosePokemon(targets, "Sneaky Bite: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swoop.HandleAttack(ctx, atk); }
        }

        /// <summary>Scavenge: discard a Psychic Energy attached to this Pokémon; if you do, put an Item card from your discard pile into your hand.</summary>
        private sealed class ScavengeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scavenge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var psychic = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Psychic)).ToList();
                if (psychic.Count == 0) return true;
                var energy = ctx.ChooseCards(psychic, 1, 1, "Scavenge: discard which Psychic Energy?")[0];
                if (!ctx.Engine.DiscardEnergy(me, energy)) return true;
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                var picks = ctx.ChooseCards(items, 1, 1, "Scavenge: put which Item card from your discard pile into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Gothic Fear: when played from your hand to evolve, you may leave both Active Pokémon Confused (both or neither: Compendium, 2015-11-19). Poison Ring.</summary>
        private sealed class HaunterGEN : Mon
        {
            private static readonly Mon Ring = new CardsPOR.PoisonRingMon("Poison Ring");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool both = ctx.Me.Active != null && ctx.Opp.Active != null;
                if (!TriggerOnce(ctx, "Gothic Fear", both, "Gothic Fear: leave both Active Pokémon Confused?")) return;
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Confused);
                if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ring.HandleAttack(ctx, atk); }
        }

        /// <summary>Creep Show: if the opponent's Active Pokémon has 3 or more damage counters on it, it is Knocked Out.</summary>
        private sealed class CreepShowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Creep Show") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending != null && defending.DamageCounters >= 3 && ctx.CanAffect(defending)) ctx.Engine.KnockOut(defending, "Creep Show");
                return true;
            }
        }

        /// <summary>Victory Kiss: once during your turn, while this Pokémon is on your Bench, heal 10 damage from your Active Pokémon. Hug: no retreat.</summary>
        private sealed class JynxGEN : Mon
        {
            private static readonly Mon Hug = new NoRetreatMon("Hug");

            protected override string Activated { get { return "Victory Kiss"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hug.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Shadow Ear: once during your turn, while this Pokémon is your Active Pokémon, move 1 damage counter from 1 of your Pokémon to
        /// 1 of the opponent's Pokémon. Mind Shock: not affected by Weakness or Resistance.
        /// </summary>
        private sealed class MeowsticEX : Mon
        {
            private static readonly Mon MindShock = new CardsLOT1.PlainDamageMon("Mind Shock", false);

            protected override string Activated { get { return "Shadow Ear"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0 && ctx.OpponentPokemon().Count > 0
                       && !ctx.Engine.CounterMovementBlocked();
            }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var targets = ctx.OpponentPokemon();
                if (hurt.Count == 0 || targets.Count == 0) return;
                var source = ctx.ChoosePokemon(hurt, "Shadow Ear: move a damage counter from which of your Pokémon?");
                var target = ctx.ChoosePokemon(targets, "Shadow Ear: move it to which of your opponent's Pokémon?");
                if (source != null && target != null) ctx.Engine.MoveDamageCounters(source, target, 1, "Shadow Ear");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MindShock.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Mine: look at the top card of the opponent's deck; then you may have them shuffle their deck.</summary>
        private sealed class MineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mine") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count == 0) return true;
                var top = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.NoteLookedAt(top, "Top card of your opponent's deck", ctx.Player);
                if (ctx.AskYesNo("Mine: the top card of your opponent's deck is " + top[0].Def.Name + ". Have your opponent shuffle their deck?"))
                    ctx.Engine.ShuffleDeck(ctx.Opponent);
                else
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of the other player's deck and left it there.");
                return true;
            }
        }

        /// <summary>
        /// Fighting Fury: each of your Fighting Pokémon's attacks (Machamp's too) do 20 more damage to the opponent's Active Pokémon,
        /// before Weakness and Resistance; each Machamp adds its own. Machamp Crush: the Defending Pokémon's attacks do 40 less.
        /// </summary>
        private sealed class MachampGEN : Mon
        {
            private static readonly Mon Crush = new CardsMEG.WeakenDefendingMon("Machamp Crush", 40);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(attacker, EnergyType.Fighting) ? damage + 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crush.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fairy / Colorless

        /// <summary>Bench Barrier: prevent all damage done to your Benched Pokémon by attacks (anyone's). Juggling: 4 coins, 10 each.</summary>
        private sealed class MrMimeGEN : Mon
        {
            private static readonly Mon Juggling = new CoinsMon("Juggling", 4);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                if (src.Pokemon == null || t == null || t.Owner != src.Player || info.Attacker == null || !src.Me.Bench.Contains(t)) return damage;
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Juggling.HandleAttack(ctx, atk); }
        }

        /// <summary>Exhausted Tackle: flip a coin; heads, 30 damage to the opponent's Active Pokémon; tails, 30 damage to this Pokémon.</summary>
        private sealed class ExhaustedTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Exhausted Tackle") return false;
                if (ctx.Flip()) ctx.DamageDefending(30);
                else ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Retreat Aid: while this Pokémon is on your Bench, your Active Pokémon's Retreat Cost is [C][C] less. Fury Attack: 3 coins.</summary>
        private sealed class DodrioGEN : Mon
        {
            private static readonly Mon Fury = new CoinsMon("Fury Attack", 3);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon != src.Me.Active || !src.Me.Bench.Contains(src.Pokemon)) return cost;
                return Math.Max(0, cost - 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fury.HandleAttack(ctx, atk); }
        }

        /// <summary>Seething Anger: flip a coin for each damage counter on this Pokémon; 30 damage for each heads.</summary>
        private sealed class SeethingAngerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seething Anger") return false;
                int n = ctx.Pokemon.DamageCounters;
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Stir and Snooze: while Asleep it flips 2 coins between turns and wakes only on 2 heads. Sleepy Press: heal 20, then it's Asleep.</summary>
        private sealed class SnorlaxGEN : Mon
        {
            public override int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(coins, 2) : coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleepy Press") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 20);
                if (ctx.PokemonIsActive) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }
    }
}
