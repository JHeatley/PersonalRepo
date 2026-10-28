using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sandstorm Uncommon and Common Pokémon (SS 27-85; v158). Unlimited only. Dunsparce 60 is Celestial Storm's (a print).
    /// Team Assembly and Influence put the cards onto the Bench as Basic Pokémon (like Revive Fossil; they evolve normally).
    /// </summary>
    public static class CardsSSB
    {
        private static readonly string[] OmanyteKabuto = { "Omanyte", "Kabuto" };
        private static readonly string[] Fossils = { "Omanyte", "Kabuto", "Aerodactyl", "Lileep", "Anorith" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SS-27", new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 1));
            r.RegisterByKey("SS-28", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("SS-29", new ArbokIntimidatingFang());
            r.RegisterByKey("SS-30", new ComboMon(new DrizzleMon(), new MaxBubblesMon()));
            r.RegisterByKey("SS-31", new CardsSS.BabyEvolutionMon("Marill", new SearchAttackMon("Jump Catch", d => d.IsTrainer, 1)));
            r.RegisterByKey("SS-32", new RapidSpinSS());
            r.RegisterByKey("SS-33", new ComboMon(new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned), new CardsFLF1.NoResistanceMon("Sky Uppercut")));
            r.RegisterByKey("SS-34", new CardsASR1.TimesMon("Ultra Energy Source", c => BasicCards(c.Me.Active) + BasicCards(c.Opp.Active)));
            r.RegisterByKey("SS-35", new ComboMon(new CardsE1.PlasmaMon(), new AnyOpponentTextMon("Thunder Spear", 40)));
            r.RegisterByKey("SS-36", new CardsSS.BabyEvolutionMon("Electabuzz", new CardsXY2.GatherEnergyMon()));
            r.RegisterByKey("SS-37", new ComboMon(new CardsRS.TextDamageMon("Double Wing Attack", 20),
                new CardsFST2.ConditionBonusMon("Rend", 20, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("SS-38", new IllumiseGlowingScreen());
            r.RegisterByKey("SS-39", new CardsFST1.TakeLessMon(20, new CardsSS.BenchNamedMon("Team Assembly", OmanyteKabuto, true, 99)));
            r.RegisterByKey("SS-40", new ComboMon(new ParalyzeFlipMon("Psyshock"), new LinkBlastSS()));
            r.RegisterByKey("SS-41", new CardsFCO2.FlipProtectMon("Iron Defense"));
            r.RegisterByKey("SS-42", new ComboMon(new CardsSS.BenchNamedMon("Influence", Fossils, false, 2), new TimeSpiralFlipMon()));
            r.RegisterByKey("SS-43", new CardsTEU1.AmnesiaMon("Amnesia"));
            r.RegisterByKey("SS-44", new ComboMon(new SniffOutMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("SS-45", new CardsSS.RainDishMon(new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("SS-46", new ComboMon(new CardsSS.SurpriseMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("SS-47", new ComboMon(new CardsSS.SurpriseMon(), new DarkMindMon()));
            r.RegisterByKey("SS-48", new FlipBonusMon("Steady Punch", 10));
            r.RegisterByKey("SS-49", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new TailsNothingMon("Razor Wind")));
            r.RegisterByKey("SS-50", new CardsBS1.ExtraWaterMon("Water Gun"));
            r.RegisterByKey("SS-51", new CardsSS.DiscardForConditionMon("Burning Claw", EnergyType.Fire, SpecialCondition.Burned));
            r.RegisterByKey("SS-52", new VigorothFocusEnergy());
            r.RegisterByKey("SS-53", new VolbeatUpliftingGlow());
            r.RegisterByKey("SS-54", new CardsSS.BabyEvolutionMon("Wobbuffet", new AlluringSmileMon()));
            r.RegisterByKey("SS-55", new XatuHealingWind());
            r.RegisterByKey("SS-56", new FlipBonusMon("Steel Headbutt", 20));
            r.RegisterByKey("SS-57", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, null));
            r.RegisterByKey("SS-58", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("SS-59", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("SS-61", new ComboMon(new CardsSS.SurpriseMon(), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("SS-62", new ComboMon(new HauntSS(), new DarkMindMon()));
            r.RegisterByKey("SS-63", new ComboMon(new SearchAttackMon("Signs of Evolution", d => d.IsPokemon && d.EvolvesFrom == "Eevee", 3), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("SS-64", new ParalyzeFlipMon("Bind"));
            r.RegisterByKey("SS-65", new CardsCRI.CounterConditionMon("Fire Veil", SpecialCondition.Burned, null));
            r.RegisterByKey("SS-66", new CardsSS.RainDishMon(null));
            r.RegisterByKey("SS-67", new CardsE2B.BlotMon());
            r.RegisterByKey("SS-68", new DoubleBubbleMon());
            r.RegisterByKey("SS-69", new ConditionAttackMon("Soothing Wave", SpecialCondition.Asleep));
            r.RegisterByKey("SS-70", new ComboMon(new CardsSS.BenchNamedMon("Team Assembly", OmanyteKabuto, true, 99), new ParalyzeFlipMon("Bind")));
            r.RegisterByKey("SS-71", new ComboMon(new ParalyzeFlipMon("Bind"), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("SS-72", new PlainMon());
            r.RegisterByKey("SS-73", new CardsCEC1.ConfusionWaveMon("Confusion Wave"));
            r.RegisterByKey("SS-74", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep),
                new CardsASR1.TimesMon("Psychic Boom", c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("SS-75", new CardsPAR2.FlipConditionMon("Poison Needle", SpecialCondition.Poisoned));
            r.RegisterByKey("SS-76", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("SS-77", new PlainMon());
            r.RegisterByKey("SS-78", new ComboMon(new GrowthSpurtMon(), new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("SS-79", new ComboMon(new EnergyCatchMon(), new RecoilMon("Double-edge", 10)));
            r.RegisterByKey("SS-80", new LockAfterAttackMon("Lazy Punch", false));
            r.RegisterByKey("SS-81", new CardsFCO2.FlipProtectMon("Super Speed"));
            r.RegisterByKey("SS-82", new NoRetreatMon("Sand Pit"));
            r.RegisterByKey("SS-83", new ConditionAttackMon("Super Hypno Wave", SpecialCondition.Asleep));
            r.RegisterByKey("SS-84", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("SS-85", new DrawAttackMon("Collect", 1));
        }

        private static int BasicCards(PokemonInPlay mon) { return mon == null ? 0 : mon.Energy.Count(e => e.Def.IsBasicEnergy); }

        /// <summary>
        /// Intimidating Fang (Poké-Body): while Arbok is your Active Pokémon, damage the opponent's attacks do to your Pokémon is 10
        /// less, before Weakness and Resistance. Toxic: Poisoned, 2 damage counters between turns.
        /// </summary>
        internal sealed class ArbokIntimidatingFang : Mon
        {
            private static readonly Mon Toxic = new CardsBS1.ToxicMon("Toxic", 2);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target == null || info.Target.Owner != src.Player) return damage;
                if (info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (info.IgnoreDefenderEffects && info.TargetIsActive) return damage;
                return Math.Max(0, damage - 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Toxic.HandleAttack(ctx, atk); }
        }

        /// <summary>Drizzle: attach as many Water Energy cards from your hand as you like to your Active Pokémon.</summary>
        private sealed class DrizzleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drizzle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var active = ctx.Me.Active;
                var water = ctx.Me.Hand.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Water)).ToList();
                if (active == null || water.Count == 0) return true;
                var picks = ctx.ChooseCards(water, 0, water.Count, "Drizzle: attach any number of Water Energy cards to " + active.Name);
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, active);
                return true;
            }
        }

        /// <summary>Max Bubbles: a coin for each Energy attached to your Active Pokémon; 30 for each heads.</summary>
        private sealed class MaxBubblesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Bubbles") return false;
                int coins = ctx.Me.Active == null ? 0 : ctx.EnergyCount(ctx.Me.Active);
                ctx.DamageDefending(coins <= 0 ? 0 : atk.Damage * ctx.FlipCoins(coins));
                return true;
            }
        }

        /// <summary>Rapid Spin: 10; the opponent switches their Defending Pokémon with a Benched Pokémon, then you switch Baltoy with 1 of yours.</summary>
        private sealed class RapidSpinSS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null && !ctx.Engine.IsKnockedOut(ctx.Opp.Active)) ctx.OpponentSwitchesActive();
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Pokémon. This attack does N damage to that Pokémon" (Weakness and Resistance for the Active one only): Thunder Spear.</summary>
        internal sealed class AnyOpponentTextMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public AnyOpponentTextMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, amount));
                return true;
            }
        }

        /// <summary>
        /// Glowing Screen (Poké-Body): while a Volbeat is in play, damage done to Illumise by attacks from Fighting and Darkness
        /// Pokémon is 30 less (no more than 30). Chaotic Noise: heads Confused, tails Asleep. Pester.
        /// </summary>
        private sealed class IllumiseGlowingScreen : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSI.HeadsTailsConditionMon("Chaotic Noise", SpecialCondition.Confused, SpecialCondition.Asleep), CardsSS.Pester(20));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!src.Engine.IsType(info.Attacker, EnergyType.Fighting) && !src.Engine.IsType(info.Attacker, EnergyType.Darkness)) return damage;
                return CardsSS.NamedInPlay(src, "Volbeat") ? Math.Max(0, damage - 30) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Link Blast: base damage 30 instead of 60 if Kirlia and the Defending Pokémon have a different amount of Energy attached.</summary>
        private sealed class LinkBlastSS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Link Blast") return false;
                var defending = ctx.Defending;
                bool same = defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(defending);
                ctx.DamageDefending(same ? atk.Damage : 30);
                return true;
            }
        }

        /// <summary>
        /// Time Spiral: if the opponent has an Evolved Pokémon, choose 1 and flip a coin; heads, its highest Stage Evolution card is
        /// shuffled into their deck.
        /// </summary>
        private sealed class TimeSpiralFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Spiral") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(CardsE2.IsEvolved);
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Time Spiral: choose 1 of your opponent's Evolved Pokémon");
                if (target != null && ctx.Flip() && ctx.InPlay(target) && ctx.CanAffect(target)) ctx.Engine.Devolve(target, 1, true);
                return true;
            }
        }

        /// <summary>Sniff Out: any 1 card from your discard pile into your hand.</summary>
        private sealed class SniffOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sniff Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "Sniff Out: put which card into your hand?"));
                return true;
            }
        }

        /// <summary>Dark Mind: the printed damage, and 10 to 1 of the opponent's Benched Pokémon (no Weakness or Resistance).</summary>
        internal sealed class DarkMindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Mind") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>Haunt: 1 damage counter on the Defending Pokémon.</summary>
        private sealed class HauntSS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Haunt") return false;
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 1);
                return true;
            }
        }

        /// <summary>Focus Energy: during your next turn, Vigoroth's Slash does 90 instead of 40.</summary>
        private sealed class VigorothFocusEnergy : Mon
        {
            private const string Marker = "Focus Energy";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == Marker)
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn;
                    return true;
                }
                if (atk.Name != "Slash") return false;
                int turn;
                bool focused = me.Markers.TryGetValue(Marker, out turn) && turn == ctx.MyLastTurn;
                ctx.DamageDefending(focused ? 90 : atk.Damage);
                return true;
            }
        }

        /// <summary>Uplifting Glow (Poké-Body): while an Illumise is in play, Volbeat's Retreat Cost is 0. Toxic Vibration: heads Poisoned, tails Asleep. Pester.</summary>
        private sealed class VolbeatUpliftingGlow : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSI.HeadsTailsConditionMon("Toxic Vibration", SpecialCondition.Poisoned, SpecialCondition.Asleep), CardsSS.Pester(20));

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && CardsSS.NamedInPlay(src, "Illumise") ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Alluring Smile: a Basic Pokémon card or Evolution card from your deck for each Energy attached to Wynaut, into your hand.</summary>
        private sealed class AlluringSmileMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Alluring Smile") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                if (n > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsPokemon, n, "Alluring Smile: choose up to " + n + " Pokémon cards");
                return true;
            }
        }

        /// <summary>
        /// Healing Wind (Poké-Power): once during your turn, remove 1 damage counter from your Active Pokémon (1-on-1: "each of your
        /// Active Pokémon"); not while Xatu has a Special Condition. Psyimpact: 1 damage counter on each of the opponent's Pokémon.
        /// </summary>
        private sealed class XatuHealingWind : Mon
        {
            private const string Power = "Healing Wind";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active != null && active.DamageCounters > 0) ctx.Heal(active, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyimpact") return false;
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>Double Bubble: 2 coins, 10 for each heads; if either is heads, the Defending Pokémon is Paralyzed.</summary>
        internal sealed class DoubleBubbleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Bubble") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Growth Spurt: attach a Grass Energy card from your hand to Shroomish.</summary>
        private sealed class GrowthSpurtMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Growth Spurt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var grass = ctx.Me.Hand.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Grass)).ToList();
                if (grass.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(grass, 1, 1, "Growth Spurt: attach which Grass Energy card?")[0];
                if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Energy Catch: a basic Energy card from your discard pile into your hand.</summary>
        private sealed class EnergyCatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Catch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0) return true;
                ctx.DiscardToHand(basics.Count == 1 ? basics : ctx.ChooseCards(basics, 1, 1, "Energy Catch: put which basic Energy card into your hand?"));
                return true;
            }
        }
    }
}
