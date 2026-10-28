using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>XY Black Star Promos (XYP, v107), part 2: XY74-XY120 (the Mythical collection's 110-120 are printed again as 185-196).</summary>
    public static class CardsXYP2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("XYP-74", new CardsMEG.FlipUntilTailsBonusMon("Repeater Blaze", 20));
            r.RegisterByKey("XYP-75", new CardsEVS1.PrimevalBeakMon("Cross Slicer"));
            r.RegisterByKey("XYP-76", new CardsMEW1.SpreadBenchMon("Voltage Storm", 10));
            r.RegisterByKey("XYP-77", new TimeFreezeMon());
            r.RegisterByKey("XYP-78", new EonConnectionMon());
            r.RegisterByKey("XYP-79", new CardsFST2.ConditionBonusMon("Sky Blade", 50, c => c.Me.Bench.Any(m => m.Name == "Latias")));
            r.RegisterByKey("XYP-80", new ComboMon(new ParalyzeFlipMon("Thunder Nail"), new RecoilMon("Frozen Slice", 20)));
            r.RegisterByKey("XYP-81", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"), new LockAfterAttackMon("Flare Blizzard", true)));
            r.RegisterByKey("XYP-82", new RegigigasAwakening());
            r.RegisterByKey("XYP-83", new ComboMon(new GatherLightMon(), new JudgmentBlastMon("Judgment Blast")));
            r.RegisterByKey("XYP-84", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"), new CardsUNM2.OversparkMon()));
            r.RegisterByKey("XYP-85", new ComboMon(new SearchAttackMon("Hyperspace Ring", d => d.IsItem, 2), new CardsASC.PushDownMon("Wonder Trick")));
            r.RegisterByKey("XYP-86", new CardsASC.NextTurnBoostMon("Moonsault Blaze", 100));
            r.RegisterByKey("XYP-87", new StrongarmImpactMon());
            r.RegisterByKey("XYP-89", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new CardsFST2.ConditionBonusMon("Electric Ring", 30, c => c.Me.Bench.Any(m => m.Name == "Hoopa"))));
            r.RegisterByKey("XYP-90", new ComboMon(new SummoningDrawMon(), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("XYP-92", new EnergyHuntMon());
            r.RegisterByKey("XYP-93", new CelebiLeap());
            r.RegisterByKey("XYP-95", new ComboMon(new LetsEatTogetherMon(), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("XYP-96", new ComboMon(new CardsFLF1.NoResistanceMon("Mach Claw"), new LunaticSenseMon()));
            r.RegisterByKey("XYP-97", new FlipBonusMon("Rock Smash", 30));
            r.RegisterByKey("XYP-98", new CardsLTR2.GaiaHammerMon("Rock Drill Dive", 10, 10));
            r.RegisterByKey("XYP-100", new ComboMon(new ParalyzeFlipMon("Psy Bolt"), new LockAfterAttackMon("Zen Blade", true)));
            r.RegisterByKey("XYP-102", new AurorusCharm());
            r.RegisterByKey("XYP-103", new CardsASC.FlipDiscardDefendingEnergyMon("Wonder Bomb"));
            r.RegisterByKey("XYP-104", new TwinGrappleMon());
            r.RegisterByKey("XYP-110", new ComboMon(new Cards30C.PeerAtMon("Clairvoyance"), new CardsMEG.DefendingEnergyBonusMon("Psychic", 10)));
            r.RegisterByKey("XYP-111", new ComboMon(new TimeCallMon(), new CardsTWM1.HealSelfMon("Leech Seed", 10)));
            r.RegisterByKey("XYP-112", new ComboMon(new PrecognitiveDreamMon(), new CardsPFL.FlipUntilTailsTimesMon("Iron Head")));
            r.RegisterByKey("XYP-113", new ComboMon(new SearchAttackMon("Marine Guidance", d => d.IsPokemon && d.PokemonType == EnergyType.Water, 1), new SwitchSelfMon("Aqua Ring", false)));
            r.RegisterByKey("XYP-114", new CardsUPR3.AbyssalSleepMon("Abyssal Sleep"));
            r.RegisterByKey("XYP-115", new ComboMon(new CardsSHF1.HealEachMon("Aromatherapy", 30, false), new MagicalLeafMon(20, 20)));
            r.RegisterByKey("XYP-116", new ArceusTypeSwitch());
            r.RegisterByKey("XYP-117", new ComboMon(new FlipEnergyAssistMon(), new CardsSUM1.DiscardTypeFromSelfMon("Victory Ball", EnergyType.Fire, 1)));
            r.RegisterByKey("XYP-118", new KeldeoJustified());
            r.RegisterByKey("XYP-119", new ComboMon(new CardsMEG.SnipeMon("Linear Attack", 30), new CardsASC.FlipDiscardDefendingEnergyMon("Destructor Beam")));
            r.RegisterByKey("XYP-120", new ComboMon(new SopranoWaveMon(), new ConditionAttackMon("Entrancing Melody", SpecialCondition.Confused)));
        }

        // ================================================================== Shared helpers

        /// <summary>"Flip a coin. If heads, this attack does N more damage and heal M damage from this Pokémon." (Magical Leaf).</summary>
        internal sealed class MagicalLeafMon : Mon
        {
            private readonly int bonus;
            private readonly int heal;

            public MagicalLeafMon(int bonus, int heal)
            {
                this.bonus = bonus;
                this.heal = heal;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Leaf") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? bonus : 0));
                if (heads && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, heal);
                return true;
            }
        }

        /// <summary>Effect: the opponent can't evolve the Defending Pokémon from their hand (Time Freeze).</summary>
        private sealed class NoEvolveEffect : ContinuousEffect
        {
            public NoEvolveEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>Effect: this Pokémon is the chosen type until the end of your next turn (Type Switch).</summary>
        internal sealed class TypeSwitchEffect : ContinuousEffect
        {
            public readonly EnergyType Type;

            public TypeSwitchEffect(string name, int owner, int targetUid, int expiresAfterTurn, EnergyType type) : base(name, owner, targetUid, expiresAfterTurn, true) { Type = type; }

            public override void ModifyTypes(GameEngine engine, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon.Uid != TargetUid) return;
                types.Clear();
                types.Add(Type);
            }
        }

        // ================================================================== Attacks

        /// <summary>Time Freeze: during the opponent's next turn, they can't evolve the Defending Pokémon from their hand.</summary>
        private sealed class TimeFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Freeze") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new NoEvolveEffect(defending.Name + " can't evolve from the hand during the next turn (Time Freeze)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Eon Connection: draw a card; draw 1 more if Latios is on your Bench.</summary>
        private sealed class EonConnectionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eon Connection") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(ctx.Me.Bench.Any(m => m.Name == "Latios") ? 2 : 1);
                return true;
            }
        }

        /// <summary>Earthen Awakening: whenever you attach an Energy card from your hand to it, heal 20 from it. Gigas Punch: 2 tails, nothing.</summary>
        private sealed class RegigigasAwakening : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || me.DamageCounters == 0) return;
                Announce(src, "Earthen Awakening");
                src.Heal(me, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigas Punch") return false;
                if (ctx.FlipCoins(2) == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Gather Light: move as many Energy from your Benched Pokémon to this Pokémon as you like.</summary>
        internal sealed class GatherLightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Light") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => ctx.InPlay(me) && ctx.MyBench().Any(m => m != me && m.Energy.Count > 0), offerDone =>
                {
                    var givers = ctx.MyBench().Where(m => m != me && m.Energy.Count > 0).ToList();
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, givers, "Gather Light: move an Energy from which Benched Pokémon to " + me.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var card = giver.Energy.Count == 1 ? giver.Energy[0] : ctx.ChooseCards(giver.Energy.ToList(), 1, 1, "Gather Light: move which Energy?")[0];
                    return ctx.Engine.MoveEnergy(giver, me, card);
                });
                return true;
            }
        }

        /// <summary>Judgment Blast: 30 more for each type of basic Energy attached to this Pokémon. Used for Judgment Blast, Colorful Wind (the attack name is passed in).</summary>
        internal sealed class JudgmentBlastMon : Mon
        {
            private readonly string attack;

            public JudgmentBlastMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int types = ctx.Pokemon.Energy.Where(en => en.Def.IsBasicEnergy).Select(en => en.Def.EnergyProvides).Distinct().Count();
                ctx.DamageDefending(atk.Damage + 30 * types);
                return true;
            }
        }

        /// <summary>Strongarm Impact: you may do 30 more damage; if you do, discard the top 3 cards of each player's deck.</summary>
        private sealed class StrongarmImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strongarm Impact") return false;
                bool boost = ctx.AskYesNo("Strongarm Impact: do 30 more damage and discard the top 3 cards of each player's deck?");
                if (boost)
                {
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                }
                ctx.DamageDefending(atk.Damage + (boost ? 30 : 0));
                return true;
            }
        }

        /// <summary>Summoning Draw: if Pikachu is on your Bench, draw 3 cards.</summary>
        private sealed class SummoningDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Summoning Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Any(m => m.Name == "Pikachu")) ctx.Draw(3);
                return true;
            }
        }

        /// <summary>Energy Hunt: flip 3 coins; for each heads, attach a basic Energy card from your discard pile to your Benched Pokémon-EX in any way.</summary>
        private sealed class EnergyHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                var pile = ctx.BasicEnergyInDiscard();
                if (heads == 0 || pile.Count == 0 || !ctx.Me.Bench.Any(m => m.Def.IsPokemonEX)) return true;
                ctx.AttachCardsAnyWay(pile, heads, (card, mon) => ctx.Me.Bench.Contains(mon) && mon.Def.IsPokemonEX, "Energy Hunt", false);
                return true;
            }
        }

        /// <summary>
        /// Leap Through Time (θ Stop): if it would be Knocked Out for any reason, flip a coin; heads, it and its cards are shuffled into
        /// the deck instead and it isn't Knocked Out (your ruling). Sparkle Motion: 1 damage counter on each of the opponent's Pokémon.
        /// </summary>
        private sealed class CelebiLeap : Mon
        {
            public override bool EscapesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon) return false;
                Announce(src, "Leap Through Time");
                return src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sparkle Motion") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>Let's Eat Together: heal 30 damage from each Active Pokémon.</summary>
        private sealed class LetsEatTogetherMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Let's Eat Together") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0) ctx.Heal(ctx.Me.Active, 30);
                if (ctx.Opp.Active != null && ctx.Opp.Active.DamageCounters > 0) ctx.Heal(ctx.Opp.Active, 30);
                return true;
            }
        }

        /// <summary>Lunatic Sense: turn 1 of your face-down Prize cards face up (for the rest of the game); if it's a Pokémon, 60 more damage.</summary>
        private sealed class LunaticSenseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lunatic Sense") return false;
                var faceDown = ctx.Me.Prizes.Where(c => c != null && !ctx.Me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                bool pokemon = false;
                if (faceDown.Count > 0)
                {
                    var labels = new string[faceDown.Count];
                    for (int i = 0; i < labels.Length; i++) labels[i] = "Prize card " + (ctx.Me.Prizes.IndexOf(faceDown[i]) + 1);
                    int pick = labels.Length == 1 ? 0 : ctx.ChooseOption("Lunatic Sense: turn which face-down Prize card face up?", labels);
                    var card = faceDown[Math.Max(0, Math.Min(pick, faceDown.Count - 1))];
                    ctx.Me.FaceUpPrizeIds.Add(card.Id);
                    ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "turned a Prize card face up (Lunatic Sense)");
                    pokemon = card.Def.IsPokemon;
                }
                ctx.DamageDefending(atk.Damage + (pokemon ? 60 : 0));
                return true;
            }
        }

        /// <summary>Frozen Charm: your Pokémon with any Water Energy attached can't be Paralyzed. Crystal Breath: can't attack next turn.</summary>
        private sealed class AurorusCharm : Mon
        {
            private static readonly Mon Breath = new LockAfterAttackMon("Crystal Breath", false);

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && condition == SpecialCondition.Paralyzed
                    && target.Energy.Any(en => CardsDRI1.ProvidesType(src, target, en, EnergyType.Water));
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Breath.HandleAttack(ctx, atk); }
        }

        /// <summary>Twin Grapple: flip 2 coins; discard an Energy from the opponent's Active Pokémon for each heads.</summary>
        private sealed class TwinGrappleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twin Grapple") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, heads, "Twin Grapple: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Time Call: flip a coin; heads, search your deck for a card that evolves from 1 of your Pokémon and evolve it.</summary>
        private sealed class TimeCallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Call") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                CardsGEN2.EvolveOneFromDeck(ctx, ctx.MyPokemon(), null, "Time Call");
                return true;
            }
        }

        /// <summary>Precognitive Dream: draw 3 cards; this Pokémon is now Asleep.</summary>
        internal sealed class PrecognitiveDreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Precognitive Dream") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(3);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Type Switch: choose a type; this Pokémon is that type until the end of your next turn. Power Blast: discard an Energy.</summary>
        private sealed class ArceusTypeSwitch : Mon
        {
            private static readonly Mon Blast = new CardsTWM1.SelfDiscardMon("Power Blast", 1);
            private static readonly EnergyType[] Types = { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic,
                EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Fairy, EnergyType.Dragon };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Type Switch")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me)) return true;
                    var labels = Types.Select(t => t.ToString()).ToArray();
                    int pick = ctx.ChooseOption("Type Switch: which type does " + me.Name + " become until the end of your next turn?", labels);
                    var type = Types[Math.Max(0, Math.Min(pick, Types.Length - 1))];
                    ctx.AddEffect(new TypeSwitchEffect(me.Name + " is a " + type + " Pokémon until the end of its next turn (Type Switch)", ctx.Player, me.Uid, ctx.Turn + 2, type));
                    return true;
                }
                return Blast.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Energy Assist: flip a coin; heads, attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class FlipEnergyAssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (ctx.Me.Bench.Count == 0 || pile.Count == 0 || !ctx.Flip()) return true;
                ctx.AttachCardsAnyWay(pile, 1, (card, mon) => ctx.Me.Bench.Contains(mon), "Energy Assist", false);
                return true;
            }
        }

        /// <summary>Justified: this Pokémon's attacks do 50 more damage to the opponent's Darkness Pokémon (before Weakness and Resistance). Sacred Sword.</summary>
        private sealed class KeldeoJustified : Mon
        {
            private static readonly Mon Sword = new LockAfterAttackMon("Sacred Sword", true);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == me.Owner || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Darkness) ? damage + 50 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sword.HandleAttack(ctx, atk); }
        }

        /// <summary>Soprano Wave: flip 3 coins; 10 damage times the heads to each of the opponent's Pokémon.</summary>
        private sealed class SopranoWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Soprano Wave") return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) CardsCRZ1.DamageEachOpponent(ctx, 10 * heads);
                return true;
            }
        }
    }
}
