using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>Mysterious Treasures: Uncommons, Commons and Trainers (Rares and LV.X are in CardsMT.cs).</summary>
    public static partial class CardsMT
    {
        private static bool IsUnown(PokemonInPlay m, string letter) { return m.Name == "Unown [" + letter + "]" || m.Name == "Unown " + letter; }

        private static void RegisterUncommonsAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("MT-19", new AbomasnowGlacierSnow());
            r.RegisterByKey("MT-20", new AriadosSticky());
            r.RegisterByKey("MT-21", new BastiodonProtectiveWall());
            r.RegisterByKey("MT-22", new ComboMon(new CallInMon(), new CardsCG.IfDefendingConditionMon("Strange Bell", SpecialCondition.Confused, m => m.IsBasic)));
            r.RegisterByKey("MT-23", new CardsDS.EvolutionaryMon("Evolutionary Toxic", EvolutionaryToxic, c => c.Opp.Active != null, new StrikeAndFadeMon()));
            r.RegisterByKey("MT-24", new ComboMon(new StringBombMon(), new SearchAttachAnyWayMon("Nutritional Support", EnergyType.Grass, 2)));
            r.RegisterByKey("MT-25", new GlalieCraggyFace());
            r.RegisterByKey("MT-26", new GyaradosDragonDna());
            r.RegisterByKey("MT-27", new CardsASR1.PlusMon("Concerto", 10, c => c.MyPokemon(m => m.Name == "Kricketot" || m.Name == "Kricketune").Count));
            r.RegisterByKey("MT-28", new ComboMon(new CardsASR1.TimesMon("Lightning Twister", c => BasicEnergyCards(c.Pokemon)), new ChainLightningMon()));
            r.RegisterByKey("MT-29", new MantineJumboFin());
            r.RegisterByKey("MT-30", new MrMimeAiryWall());
            r.RegisterByKey("MT-31", new NidoqueenMotherPheromone());
            r.RegisterByKey("MT-32", new NinetalesColorShift());
            r.RegisterByKey("MT-33", new ComboMon(new AssuranceMon(), new HastyHeadbuttMon()));
            r.RegisterByKey("MT-34", new SlakingEnergeticImpulse());
            r.RegisterByKey("MT-35", new ComboMon(new SidestepMon(), new PersuadeMon()));
            r.RegisterByKey("MT-36", new ToxicroakPoisonSacs());
            r.RegisterByKey("MT-37", new UnownItem());
            r.RegisterByKey("MT-38", new ComboMon(new BadTemperMon(), new DefensiveClawMon()));
            r.RegisterByKey("MT-39", new CardsDS.EvolutionaryMon("Freeze-up", FreezeUp, c => c.Opp.Active != null, new IceBindMon()));
            r.RegisterByKey("MT-40", new ComboMon(new CardsASR1.TimesMon("Fury", CardsCG.SelfCounters), new MagnitudeMon()));
            r.RegisterByKey("MT-41", new ComboMon(new SearchAttackMon("Evolution Impulse", d => d.IsEvolution, 1), new CoinsMon("Leaf Boomerang", 2)));
            r.RegisterByKey("MT-42", new CardsSS.BabyEvolutionMon("Chimecho", new SearchAttackMon("Inviting Bell", d => d.IsSupporter, 1)));
            r.RegisterByKey("MT-44", new CardsDS.EvolutionaryMon("Evolutionary Vitality", EvolutionaryVitality, c => c.Me.Deck.Count > 0, new NoRetreatMon("Hover Over")));
            r.RegisterByKey("MT-45", new DewgongColdFat());
            r.RegisterByKey("MT-46", new ComboMon(new CoinsMon("Fury Attack", 3), new TriplePickMon()));
            r.RegisterByKey("MT-47", new ComboMon(new DrawAttackMon("Snake Hook", 1), new SwitchSelfMon("Bite and Run", false)));
            r.RegisterByKey("MT-48", new ComboMon(new GatherUpMon(), new MarvelousShineMon()));
            r.RegisterByKey("MT-49", new ComboMon(new SearchAttackMon("Crane Neck", DpTrainer, 1), new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("MT-50", new LookAtHandMon("Pulse Search"));
            r.RegisterByKey("MT-51", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Rock Cannon"), new CardsFST1.TwoBenchedMon("Rock Slide", 10)));
            r.RegisterByKey("MT-52", new CardsSS.BabyEvolutionMon("Chansey", new LivelyMon()));
            r.RegisterByKey("MT-53", new MendMon());
            r.RegisterByKey("MT-54", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("MT-55", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Centrifugal Force", 20, true), new SwirlingRippleMon()));
            r.RegisterByKey("MT-56", new SearchDiscardToHandMon("Rescue", d => d.IsPokemon, 2));
            r.RegisterByKey("MT-57", new ComboMon(new CrashBomberMon(), new CardsMA.BenchOnlyMon("Aqua Liner", 40)));
            r.RegisterByKey("MT-58", new ComboMon(new WildSporesMon(), new CardsE3B.HealSelfMon("Extend Fungus", 20)));
            r.RegisterByKey("MT-59", new ComboMon(new CardsSS.EachOpponentMon("Pressurized Gas", 10), new RocketTackleMon()));
            r.RegisterByKey("MT-60", new CardsDR.FlipDiscardSelfMon("Fireworks", EnergyType.Fire));
            r.RegisterByKey("MT-61", new ComboMon(new SpikeArmorMon(), new CardsCG.IfDefendingConditionMon("Poison Spike", SpecialCondition.Poisoned, m => m.DamageCounters > 0)));
            r.RegisterByKey("MT-62", new CardsSSB.AnyOpponentTextMon("Ice Rider", 30));
            r.RegisterByKey("MT-64", new ComboMon(new BonusLeafMon(), new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("MT-65", new UnownEquip());
            r.RegisterByKey("MT-66", new UnownMagnet());
            r.RegisterByKey("MT-67", new UnownThrow());
            r.RegisterByKey("MT-68", new ComboMon(new WakeUpPunchMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("MT-69", new ComboMon(new PlaySearchMon(), new UltraEvolutionMon()));
            r.RegisterByKey("MT-70", new ThrowOffMon());
            r.RegisterByKey("MT-71", new RecoilMon("Steel Tackle", 10));
            r.RegisterByKey("MT-72", new CardsDR.FlipEachOpponentMon("Mud Spit", 10));
            r.RegisterByKey("MT-73", new PlainMon());
            r.RegisterByKey("MT-74", new CardsPAR2.FlipConditionMon("Dull Light", SpecialCondition.Confused));
            r.RegisterByKey("MT-75", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("MT-76", new ComboMon(new CardsSP1.FlipNoDamageMon("Scrunch"), new RecoilMon("Double-edge", 60)));
            r.RegisterByKey("MT-77", new ComboMon(new CardsPAR2.FlipConditionMon("Sleep Scent", SpecialCondition.Asleep), new FlipBonusMon("Jump On", 10)));
            r.RegisterByKey("MT-78", new ComboMon(new GhastlySoundMon(), new CardsPAR2.FlipConditionMon("Finger Poke", SpecialCondition.Poisoned)));
            r.RegisterByKey("MT-79", new PlainMon());
            r.RegisterByKey("MT-80", new ComboMon(new CoinsMon("Double Stab", 2), new LockAfterAttackMon("Accelerating Stab", true)));
            r.RegisterByKey("MT-81", new ElectromagneticJamMon());
            r.RegisterByKey("MT-82", new DirectHitBombMon());
            r.RegisterByKey("MT-83", new DodgeMon("Elegant Swim", false));
            r.RegisterByKey("MT-84", new CardsG1.ChooseBenchedMon("Stone Throw", 2, 10, false));
            r.RegisterByKey("MT-85", new CardsDRI1.WhirlwindMon("Push Down"));
            r.RegisterByKey("MT-86", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("MT-87", new MountainBiteMon());
            r.RegisterByKey("MT-88", new CardsSS.BabyEvolutionMon("Magmar", new CardsPAR2.FlipConditionMon("Scorch", SpecialCondition.Burned)));
            r.RegisterByKey("MT-89", new ComboMon(new CardsASR1.TimesMon("Flail", CardsCG.SelfCounters), new CardsBWP.BothHeadsOrNothingMon("Dragon Rage")));
            r.RegisterByKey("MT-90", new ComboMon(new CardsROS1.BenchFamilyMon("Swarm", "Murkrow", 1), new SwitchSelfMon("Hide Crowd", false)));
            r.RegisterByKey("MT-91", new ComboMon(new CardsSS.BenchNamedMon("Call for Family", new[] { "Nidoran ♂", "Nidoran ♀" }, false, 1),
                new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("MT-92", new ParalyzeFlipMon("Mushroom Tackle"));
            r.RegisterByKey("MT-93", new CardsSS.BabyEvolutionMon("Pikachu", new TailsNothingMon("Chupi")));
            r.RegisterByKey("MT-95", new ComboMon(new CardsSSB.AnyOpponentTextMon("Sharpshooting", 10), new ParalyzeFlipMon("Numbing Water")));
            r.RegisterByKey("MT-96", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("MT-97", new ParalyzeFlipMon("Freezing Headbutt"));
            r.RegisterByKey("MT-98", new PlasmaMon());
            r.RegisterByKey("MT-99", new CardsDRX.BothConditionMon("Drowsy", SpecialCondition.Asleep));
            r.RegisterByKey("MT-100", new ComboMon(new DrawAttackMon("Collect", 1), new TailsNothingMon("Snowball")));
            r.RegisterByKey("MT-101", new ComboMon(new LookAtHandMon("Curiosity"), new CardsXYP1.FlipTailsRecoilMon("Snowball Fight", 10)));
            r.RegisterByKey("MT-102", new WillpowerMon());
            r.RegisterByKey("MT-103", new ComboMon(new NoRetreatMon("Impound"), new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("MT-104", new CardsE2.SleepInducerMon(false));
            r.RegisterByKey("MT-105", new ComboMon(new CardsDP.RestMon("Rest", 20), new SweetPalmMon()));
            r.RegisterByKey("MT-106", new CardsFST2.ConditionBonusMon("Shining Fang", 10, c => c.Defending != null && c.Defending.DamageCounters > 0));
            r.RegisterByKey("MT-107", new Cards30CC.BenchHitMon("Fire Soul", 10));
            r.RegisterByKey("MT-108", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));

            r.RegisterByKey("MT-109", new BebesSearch());
            r.RegisterByKey("MT-111", new FossilExcavator());
            r.RegisterByKey("MT-112", new LakeBoundary());
            r.RegisterByKey("MT-113", new NightMaintenance());
            r.RegisterByKey("MT-114", new QuickBall());
            r.RegisterByKey("MT-115", new TeamGalacticsWager());
            r.RegisterByKey("MT-116", new ArmorFossil());
            r.RegisterByKey("MT-117", new SkullFossil());
            r.RegisterByKey("MT-124", new TimeSpaceDistortion());
        }

        internal static int BasicEnergyCards(PokemonInPlay mon) { return mon == null ? 0 : mon.Energy.Count(e => e.Def.IsBasicEnergy); }

        // ------------------------------------------------------------------ Uncommons

        /// <summary>Glacier Snow (Poké-Body): while Active, when damaged by the opponent's attack (even if Knocked Out), the Attacking Pokémon is now Asleep. Heavy Blizzard: heads, 1 damage counter on each of the opponent's Benched Pokémon.</summary>
        private sealed class AbomasnowGlacierSnow : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                Announce(ctx, "Glacier Snow");
                ctx.ApplyCondition(attacker, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Blizzard") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    foreach (var m in ctx.OpponentBench()) if (ctx.CanAffect(m)) ctx.PlaceCounters(m, 1);
                return true;
            }
        }

        /// <summary>Sticky (Poké-Body): every Pokémon's Retreat Cost (both players') is 1 more, except any Ariados (Feb 2008 errata). Offensive Needle.</summary>
        private sealed class AriadosSticky : Mon
        {
            private static readonly Mon Needle = new CardsSI.HeadsTailsConditionMon("Offensive Needle", SpecialCondition.Poisoned, SpecialCondition.Paralyzed);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon == null || mon == null || mon.Name == "Ariados" ? cost : cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Needle.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Protective Wall (Poké-Body): prevent all damage done to your Benched Pokémon by the opponent's attacks. Anger Revenge: if
        /// it (as Bastiodon) was damaged by an attack during the opponent's last turn, 40 to 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class BastiodonProtectiveWall : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                return src.Pokemon != null && t != null && t.Owner == src.Player && !info.TargetIsActive && info.Attacker != null && info.Attacker.Owner != src.Player
                    && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Anger Revenge") return false;
                var me = ctx.Pokemon;
                bool hurt = me.AttackDamageTurn == ctx.OpponentsLastTurn && me.TurnEvolved != ctx.Turn;
                ctx.DamageDefending(atk.Damage);
                if (hurt && ctx.OpponentBench().Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Anger Revenge: do 40 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 40);
                }
                return true;
            }
        }

        /// <summary>Call In: draw a card; with Chingling under it, 2 more.</summary>
        private sealed class CallInMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call In") return false;
                ctx.Draw(EvolvedFrom(ctx, "Chingling") ? 3 : 1);
                return true;
            }
        }

        /// <summary>Evolutionary Toxic: the Defending Pokémon is now Poisoned, 2 damage counters between turns.</summary>
        private static void EvolutionaryToxic(EffectContext ctx)
        {
            var target = ctx.Opp.Active;
            if (target == null || !ctx.CanAffect(target)) return;
            if (ctx.ApplyCondition(target, SpecialCondition.Poisoned)) target.PoisonCounterBase = Math.Max(target.PoisonCounterBase, 2);
        }

        /// <summary>Strike and Fade: 50 to 1 of the opponent's Pokémon (no W/R on the Bench); tails, shuffle this Pokémon and its cards into your deck.</summary>
        private sealed class StrikeAndFadeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strike and Fade") return false;
                var me = ctx.Pokemon;
                ctx.DamageAnyOpponent(50);
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.Engine.ShufflePokemonIntoDeck(me);
                return true;
            }
        }

        /// <summary>String Bomb: a coin for each Energy from basic Energy cards on this Pokémon and the Defending Pokémon; 30 times the heads.</summary>
        private sealed class StringBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "String Bomb") return false;
                int coins = BasicEnergyCards(ctx.Pokemon) + BasicEnergyCards(ctx.Defending);
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(coins));
                return true;
            }
        }

        /// <summary>"Search your deck for up to N [type] Energy cards and attach them to your Pokémon in any way you like" (Nutritional Support, Shadow Charge).</summary>
        internal sealed class SearchAttachAnyWayMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int max;

            public SearchAttachAnyWayMon(string attack, EnergyType type, int max)
            {
                this.attack = attack;
                this.type = type;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var picks = ctx.SearchMyDeck(d => CardsRS.BasicOf(d, type), max, attack + ": choose up to " + max + " " + type + " Energy cards");
                foreach (var card in picks) ctx.Me.Deck.Remove(card);
                if (picks.Count > 0) ctx.AttachCardsAnyWay(picks, picks.Count, (c, m) => true, attack + ": attach this " + type + " Energy card to which Pokémon?", false);
                foreach (var card in picks) if (!ctx.Me.Hand.Contains(card) && !ctx.Me.Discard.Contains(card) && !ctx.MyPokemon().Any(m => m.Energy.Contains(card))) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Craggy Face (Poké-Body): while Glalie is Active, damage from attacks of the opponent's Stage 2 Pokémon is 20 less (before W/R; errata: evolved or not). Intimidation Pebble: heads, no Trainers or Supporters next turn.</summary>
        private sealed class GlalieCraggyFace : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || !src.IsActive(me) || a == null || a.Owner == src.Player || a.Def.Stage != Stage.Stage2 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Intimidation Pebble") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                {
                    string who = ctx.Engine.PlayerName(ctx.Opponent);
                    ctx.AddEffect(new DpTrainerLockEffect(who + " can't play Trainer cards during their next turn (Intimidation Pebble)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                    ctx.AddEffect(new CardsTWM2.SupporterLockEffect(who + " can't play Supporter cards during their next turn (Intimidation Pebble)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                }
                return true;
            }
        }

        /// <summary>
        /// Dragon DNA (Poké-Body): Gyarados can use the attacks of its Basic Pokémon (paying for them); those do 30 more to the
        /// Defending Pokémon (before W/R) if they do any damage (Dec 2007), and only while it's evolved (May 2008). Enrage.
        /// </summary>
        private sealed class GyaradosDragonDna : Mon
        {
            private static CardDef BasicOf(PokemonInPlay me) { return me != null && me.Stack.Count > 1 ? me.Stack[0].Def : null; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var basic = BasicOf(src.Pokemon);
                if (basic == null) yield break;
                for (int i = 0; i < basic.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(basic, i);
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var basic = BasicOf(me);
                var atk = src.Engine.ResolvingAttack;
                if (basic == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || atk == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return basic.Attacks.Contains(atk) ? damage + 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enrage") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                bool first = true;
                for (int i = 0; i < 40; i++)
                {
                    if (!ctx.Flip())
                    {
                        if (first && ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                        break;
                    }
                    first = false;
                    var card = CardsHP.RandomOpponentCard(ctx);
                    if (card != null) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                }
                return true;
            }
        }

        /// <summary>Chain Lightning: 20 to each of the opponent's Benched Pokémon that shares a type with the Defending Pokémon (no W/R).</summary>
        private sealed class ChainLightningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chain Lightning") return false;
                var defending = ctx.Defending;
                var types = defending == null ? new List<EnergyType>() : ctx.Engine.TypesOf(defending).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.OpponentBench().Where(b => ctx.Engine.TypesOf(b).Any(types.Contains)).ToList()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Jumbo Fin (Poké-Body): with Mantyke under it, your Water Pokémon's Retreat Cost is 2 less (stacks). Giant Wave.</summary>
        private sealed class MantineJumboFin : Mon
        {
            private static readonly Mon Wave = new LockAfterAttackMon("Giant Wave", true);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !EvolvedFrom(src, "Mantyke") || !src.Engine.IsType(mon, EnergyType.Water)) return cost;
                return Math.Max(0, cost - 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wave.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Airy Wall (Poké-Body): prevent all damage done to Mr. Mime by attacks from the opponent's Pokémon with 2 or fewer Energy
        /// cards; with Mime Jr. under it, all effects of those attacks too. Trick Play: you place a coin face; a wrong guess means 50
        /// to the Defending Pokémon (no W/R); a right one, 20 to Mr. Mime.
        /// </summary>
        private sealed class MrMimeAiryWall : Mon
        {
            private static bool Light(PokemonInPlay a) { return a != null && a.Energy.Count <= 2; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != src.Player && Light(info.Attacker)
                    && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var me = src.Pokemon;
                if (me == null || target != me || kind != EffectSourceKind.Attack || src.IsMyTurn || !EvolvedFrom(src, "Mime Jr.")) return false;
                return Light(src.Opp.Active);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick Play") return false;
                bool heads = ctx.AskYesNo("Trick Play: put the coin heads up? (No = tails)");
                bool guessHeads = ctx.Engine.AskYesNo(ctx.Opponent, "Trick Play: guess the coin is heads? (No = tails)", ctx.Source);
                if (guessHeads != heads)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed wrong (Trick Play).");
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, 50, false, false, false);
                }
                else
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed right (Trick Play).");
                    ctx.DamageSelf(20);
                }
                return true;
            }
        }

        /// <summary>Mother Pheromone (Poké-Body): your Nidoran ♀, Nidorina, Nidoran ♂, Nidorino and Nidoking's attacks cost 1 Colorless less. Give Aid: base 50 with no more Benched Pokémon than the opponent.</summary>
        private sealed class NidoqueenMotherPheromone : Mon
        {
            private static readonly string[] Family = { "Nidoran ♀", "Nidorina", "Nidoran ♂", "Nidorino", "Nidoking" };

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !Family.Contains(mon.Name)) return;
                cost.Remove(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Give Aid") return false;
                ctx.DamageDefending(ctx.Me.Bench.Count <= ctx.Opp.Bench.Count ? 50 : atk.Damage);
                return true;
            }
        }

        /// <summary>Color Shift (Poké-Power): once during your turn, Ninetales becomes the type(s) of 1 of the opponent's Pokémon until the end of your turn. Not with a Special Condition. Fire Blast.</summary>
        private sealed class NinetalesColorShift : Mon
        {
            private static readonly Mon Blast = new CardsASC.SelfDiscardTypeMon("Fire Blast", EnergyType.Fire, 1);

            protected override string Activated { get { return "Color Shift"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.OpponentPokemon().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Color Shift: Ninetales becomes the same type as which of your opponent's Pokémon?");
                if (target == null) return;
                var types = ctx.Engine.TypesOf(target).ToList();
                ctx.AddEffect(new TypeCopyEffect(ctx.Pokemon.Name + " is " + string.Join("/", types) + " this turn (Color Shift)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn, types));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        private sealed class TypeCopyEffect : ContinuousEffect
        {
            private readonly List<EnergyType> copied;

            public TypeCopyEffect(string name, int owner, int targetUid, int expiresAfterTurn, List<EnergyType> copied) : base(name, owner, targetUid, expiresAfterTurn, false) { this.copied = copied; }

            public override void ModifyTypes(GameEngine engine, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon.Uid != TargetUid || copied.Count == 0) return;
                types.Clear();
                types.AddRange(copied);
            }
        }

        /// <summary>Assurance: base 60 if the Defending Pokémon has 60 HP or less left.</summary>
        private sealed class AssuranceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assurance") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.RemainingHp(ctx.Defending) <= 60 ? 60 : atk.Damage);
                return true;
            }
        }

        /// <summary>Hasty Headbutt: 20 to itself; the damage ignores Weakness, Resistance and effects on the Defending Pokémon.</summary>
        private sealed class HastyHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hasty Headbutt") return false;
                ctx.DamageSelf(20);
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        /// <summary>
        /// Energetic Impulse (Poké-Power): once during your turn, if Slaking is Active, flip: heads, Lazy Blow's base damage is 130
        /// this turn; tails, it can't attack or retreat this turn.
        /// </summary>
        private sealed class SlakingEnergeticImpulse : Mon
        {
            private const string Marker = "Energetic Impulse";

            protected override string Activated { get { return Marker; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.Flip()) me.Markers[Marker] = ctx.Turn;
                else ctx.AddEffect(new StuckEffect(me.Name + " can't attack or retreat this turn (Energetic Impulse)", ctx.Player, me.Uid, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lazy Blow") return false;
                int turn;
                bool pumped = ctx.Pokemon.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn && ctx.Pokemon.MovedToActiveOnTurn != ctx.Turn;
                ctx.DamageDefending(pumped ? 130 : atk.Damage);
                return true;
            }
        }

        private sealed class StuckEffect : ContinuousEffect
        {
            public StuckEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Sidestep: with Bonsly under it, heads prevents all effects of attacks, damage included, next turn.</summary>
        private sealed class SidestepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sidestep") return false;
                ctx.DamageDefending(atk.Damage);
                if (EvolvedFrom(ctx, "Bonsly") && ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Persuade: the opponent switches the Defending Pokémon with a Benched Pokémon; the new one can't retreat next turn.</summary>
        private sealed class PersuadeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Persuade") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.OpponentSwitchesActive() && ctx.Opp.Active != null) ctx.DefendingCantRetreat(ctx.Opp.Active);
                return true;
            }
        }

        /// <summary>Poison Sacs (Poké-Body): the opponent's Poisoned Pokémon stay Poisoned when they evolve, devolve or level up. Knuckle Claws.</summary>
        private sealed class ToxicroakPoisonSacs : Mon
        {
            private static readonly Mon Claws = new ConditionAttackMon("Knuckle Claws", SpecialCondition.Poisoned);

            public override bool KeepsPoisonOnEvolve(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon != null && mon.Owner != src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        /// <summary>Hidden Power (Unown I): an Energy card on the Defending Pokémon provides only Colorless until the end of the opponent's next turn.</summary>
        private sealed class FaceDownEnergyEffect : ContinuousEffect
        {
            private readonly CardInstance card;

            public FaceDownEnergyEffect(string name, int owner, int expiresAfterTurn, CardInstance card) : base(name, owner, -1, expiresAfterTurn, false) { this.card = card; }

            public override void ModifyProvidedEnergy(GameEngine engine, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (energy != card || units.Count == 0) return;
                units.Clear();
                units.Add(EnergyMask.Of(EnergyType.Colorless));
            }
        }

        /// <summary>ITEM (Poké-Power): once during your turn, with Unown I, T, E and M on your Bench, search your deck for a Trainer card. Only 1 ITEM a turn.</summary>
        private sealed class UnownItem : Mon
        {
            protected override string Activated { get { return "ITEM"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var bench = ctx.Me.Bench;
                return ctx.Me.Deck.Count > 0 && new[] { "I", "T", "E", "M" }.All(l => bench.Any(m => IsUnown(m, l)));
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(DpTrainer, 1, "ITEM: choose a Trainer card");
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                var defending = ctx.Defending;
                if (defending == null || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Hidden Power: put which Energy card face down?")[0];
                ctx.AddEffect(new FaceDownEnergyEffect(card.Def.Name + " on " + defending.Name + " provides only Colorless Energy (Hidden Power)", ctx.Player, ctx.Turn + 1, card));
                return true;
            }
        }

        /// <summary>Bad Temper: the Defending Pokémon is now Confused; its attacks do 60 more to the Active Pokémon next turn (before W/R).</summary>
        private sealed class BadTemperMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bad Temper") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                ctx.AddEffect(new AngryEffect(defending.Name + "'s attacks do 60 more damage during its next turn (Bad Temper)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class AngryEffect : ContinuousEffect
        {
            public AngryEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target == null || target.Owner == attacker.Owner || damage <= 0) return damage;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack || engine.State.Players[target.Owner].Active != target) return damage;
                return damage + 60;
            }
        }

        /// <summary>Defensive Claw: with Teddiursa on your Bench, 20 more; if that Teddiursa is damaged, 40 more instead and it's fully healed.</summary>
        private sealed class DefensiveClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Defensive Claw") return false;
                var bears = ctx.MyBench().Where(m => m.Name == "Teddiursa").ToList();
                var hurt = bears.Where(m => m.DamageCounters > 0).ToList();
                PokemonInPlay healed = null;
                if (hurt.Count > 0) healed = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Defensive Claw: remove all damage counters from which Teddiursa?");
                ctx.DamageDefending(atk.Damage + (healed != null ? 40 : bears.Count > 0 ? 20 : 0));
                if (healed != null && ctx.InPlay(healed)) ctx.Heal(healed, healed.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Freeze-up: flip 2 coins; both heads, discard the Defending Pokémon and all cards attached (not a Knock Out).</summary>
        private static void FreezeUp(EffectContext ctx)
        {
            var target = ctx.Opp.Active;
            if (target == null) return;
            if (ctx.FlipCoins(2) == 2 && ctx.InPlay(target) && ctx.CanAffect(target)) ctx.Engine.DiscardPokemonFromPlay(target);
        }

        /// <summary>Ice Bind: unless the opponent discards a card from their hand, the Defending Pokémon is now Paralyzed.</summary>
        private sealed class IceBindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Bind") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (ctx.Opp.Hand.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Ice Bind: discard a card from your hand so " + defending.Name + " isn't Paralyzed?", ctx.Source))
                {
                    var card = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 1, 1, "Ice Bind: discard which card?", ctx.Card.Def)[0];
                    ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                }
                else ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Magnitude: 20 to each Benched Pokémon, yours and your opponent's (no W/R).</summary>
        private sealed class MagnitudeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnitude") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench().Concat(ctx.OpponentBench()).ToList()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Evolutionary Vitality: look at the top 5 cards; the Energy cards there go into your hand; shuffle.</summary>
        private static void EvolutionaryVitality(EffectContext ctx)
        {
            var top = ctx.LookAtTop(5);
            var energy = top.Where(c => c.Def.IsEnergy).ToList();
            if (energy.Count > 0) ctx.DeckToHand(energy);
            ctx.ShuffleMyDeck();
        }

        /// <summary>Cold Fat (Poké-Body): while Asleep, attack damage to Dewgong is 40 less (after W/R). Collapse: Dewgong is now Asleep.</summary>
        private sealed class DewgongColdFat : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || (me.Conditions & SpecialCondition.Asleep) == 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Collapse") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Triple Pick: 30 to each of 3 of the opponent's Evolved Pokémon (no W/R on the Bench).</summary>
        private sealed class TriplePickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Pick") return false;
                var evolved = ctx.OpponentPokemon(m => CardsE2.IsEvolved(m));
                var picks = evolved.Count <= 3 ? evolved : ctx.ChoosePokemonMany(evolved, 3, 3, "Triple Pick: choose 3 of your opponent's Evolved Pokémon");
                foreach (var m in picks) ctx.DamageTarget(m, 30);
                return true;
            }
        }

        /// <summary>Gather Up: up to 2 Energy cards from your discard pile into your hand.</summary>
        private sealed class GatherUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Up") return false;
                return new SearchDiscardToHandMon("Gather Up", d => d.IsEnergy, 2).HandleAttack(ctx, atk);
            }
        }

        /// <summary>"Search your discard pile for up to N [cards], show them and put them into your hand" (Rescue, Gather Up).</summary>
        internal sealed class SearchDiscardToHandMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly int max;

            public SearchDiscardToHandMon(string attack, Func<CardDef, bool> filter, int max)
            {
                this.attack = attack;
                this.filter = filter;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => filter(c.Def)).ToList();
                if (pile.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                var picks = ctx.ChooseCards(pile, 1, Math.Min(max, pile.Count), attack + ": put which cards into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Marvelous Shine: heads, 4 damage counters on 1 of the opponent's Pokémon; tails, remove 4 from 1 of yours.</summary>
        private sealed class MarvelousShineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Marvelous Shine") return false;
                if (ctx.Flip())
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Marvelous Shine: put 4 damage counters on which of your opponent's Pokémon?");
                    if (target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, 4);
                }
                else
                {
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    var target = hurt.Count == 0 ? null : ctx.ChoosePokemon(hurt, "Marvelous Shine: remove 4 damage counters from which of your Pokémon?");
                    if (target != null) ctx.Heal(target, 40);
                }
                return true;
            }
        }

        /// <summary>"Look at your opponent's hand" (Pulse Search, Curiosity), after the damage.</summary>
        private sealed class LookAtHandMon : Mon
        {
            private readonly string attack;

            public LookAtHandMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), attack, attack + ": your opponent's hand", ctx.Source);
                return true;
            }
        }

        /// <summary>Lively: remove 2 damage counters from 1 of your Pokémon.</summary>
        private sealed class LivelyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lively") return false;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var target = hurt.Count == 0 ? null : ctx.ChoosePokemon(hurt, "Lively: remove 2 damage counters from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Mend: attach a Metal Energy card from your discard pile to this Pokémon; if you do, remove 2 damage counters from it.</summary>
        private sealed class MendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mend") return false;
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && CardsRS.BasicOf(c.Def, EnergyType.Metal)).ToList();
                if (pile.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Mend: attach which Metal Energy card?")[0];
                if (ctx.Me.Discard.Remove(card))
                {
                    if (ctx.Engine.AttachEnergyCard(card, me)) ctx.Heal(me, 20);
                    else if (!ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                }
                return true;
            }
        }

        /// <summary>Swirling Ripple: with a Water Pokémon in the opponent's play, 30 more and Confused.</summary>
        private sealed class SwirlingRippleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swirling Ripple") return false;
                var defending = ctx.Defending;
                bool water = ctx.OpponentPokemon().Any(m => ctx.Engine.IsType(m, EnergyType.Water));
                ctx.DamageDefending(atk.Damage + (water ? 30 : 0));
                if (water && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Crash Bomber: with a Special Energy card on the Defending Pokémon, 30 more; then discard a Special Energy card from it.</summary>
        private sealed class CrashBomberMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crash Bomber") return false;
                var defending = ctx.Defending;
                bool special = defending != null && defending.Energy.Any(e => e.Def.IsSpecialEnergy);
                ctx.DamageDefending(atk.Damage + (special ? 30 : 0));
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => e.Def.IsSpecialEnergy))
                    ctx.DiscardEnergyFrom(defending, 1, "Crash Bomber: discard a Special Energy card from " + defending.Name, e => e.Def.IsSpecialEnergy);
                return true;
            }
        }

        /// <summary>Wild Spores: if this Pokémon evolved from Paras this turn, 40 damage instead and the Defending Pokémon is Asleep and Poisoned.</summary>
        private sealed class WildSporesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wild Spores") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool fresh = me.TurnEvolved == ctx.Turn && EvolvedFrom(ctx, "Paras");
                ctx.DamageDefending(fresh ? 40 : atk.Damage);
                if (fresh && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Rocket Tackle: 10 to itself; heads, prevent all attack damage to it next turn.</summary>
        private sealed class RocketTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rocket Tackle") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(10);
                if (ctx.Flip() && ctx.InPlay(me)) ctx.SelfTakesLessNextTurn(10000);
                return true;
            }
        }

        /// <summary>Spike Armor: during the opponent's next turn, if this Pokémon is damaged by their attack (even if Knocked Out), 4 damage counters on the Attacking Pokémon.</summary>
        private sealed class SpikeArmorMon : Mon
        {
            private const string Marker = "Spike Armor";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Marker) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Pokemon.Markers[Marker] = ctx.Turn + 1;
                return true;
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                int turn;
                var me = ctx.Pokemon;
                if (me == null || !me.Markers.TryGetValue(Marker, out turn) || turn != ctx.Turn || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                ctx.Log("Spike Armor: 4 damage counters on " + attacker.Name + ".");
                ctx.Engine.PlaceDamageCounters(attacker, 4, "Spike Armor");
            }

            public override bool ReactsToOwnDamageWithoutAbility { get { return true; } }
        }

        /// <summary>Bonus Leaf: remove 3 damage counters from each of your Benched Pokémon with any Grass Energy attached.</summary>
        private sealed class BonusLeafMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bonus Leaf") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench().Where(b => b.DamageCounters > 0 && ctx.Engine.EnergyCountOfType(b, EnergyType.Grass) > 0).ToList()) ctx.Heal(m, 30);
                return true;
            }
        }

        /// <summary>
        /// EQUIP (Poké-Power): once during your turn, if Unown E is on your Bench, discard its attached cards and attach it to 1 of your
        /// Pokémon as a Pokémon Tool (normal Tool rules: Mysterious Treasures FAQ); that Pokémon gets +10 HP. Hidden Power: during the
        /// opponent's next turn, their coin flips are all tails.
        /// </summary>
        private sealed class UnownEquip : Mon
        {
            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && ctx.Engine.CanAttachToolTo(ctx.Card, m)); }

            protected override string Activated { get { return "EQUIP"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Pokemon.Stack.Count == 1 && Holders(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var target = ctx.ChoosePokemon(Holders(ctx), "EQUIP: attach Unown E to which of your Pokémon as a Pokémon Tool?");
                if (target == null) return;
                var card = me.Stack[0];
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.Discard.Remove(card);
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachTool(card, target);
            }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                var holder = src.Pokemon;
                return holder != null && mon == holder && holder.HasTool(src.Card) ? hp + 10 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                ctx.AddEffect(new AllTailsEffect(ctx.Engine.PlayerName(ctx.Opponent) + "'s coin flips are tails during their next turn (Hidden Power)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class AllTailsEffect : ContinuousEffect
        {
            private readonly int affected;

            public AllTailsEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool ForcesTails(GameEngine engine, int player) { return player == affected; }
        }

        /// <summary>MAGNET (Poké-Power): once during your turn, if Unown M is Active, heads switches in 1 of the opponent's Benched Pokémon. Not with a Special Condition. Hidden Power: heads 30 to the Defending; tails 30 to 1 of yours (no W/R).</summary>
        private sealed class UnownMagnet : Mon
        {
            protected override string Activated { get { return "MAGNET"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsDS.PowerReady(ctx) && ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.GustOpponent("MAGNET: switch in which of your opponent's Benched Pokémon?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (ctx.Flip()) ctx.DamageDefending(30);
                else
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Hidden Power: do 30 damage to which of your Pokémon?");
                    if (target != null) ctx.DamageWith(target, 30, false, false, false);
                }
                return true;
            }
        }

        /// <summary>THROW (Poké-Power): once during your turn, if Unown T is Active, discard a card from your hand, then heads puts 2 damage counters on 1 of the opponent's Benched Pokémon. Not with a Special Condition. Hidden Power: hand disruption both ways.</summary>
        private sealed class UnownThrow : Mon
        {
            protected override string Activated { get { return "THROW"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "THROW: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (!ctx.Flip() || ctx.OpponentBench().Count == 0) return;
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "THROW: put 2 damage counters on which Benched Pokémon?");
                if (target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (ctx.Opp.Hand.Count > 0)
                {
                    var card = ctx.ChooseCards(ctx.Opp.Hand.ToList(), 1, 1, "Hidden Power: shuffle which card from your opponent's hand into their deck?")[0];
                    ctx.Opp.Hand.Remove(card);
                    ctx.Opp.Deck.Add(card);
                    ctx.Engine.ShuffleDeck(ctx.Opponent);
                }
                if (ctx.Me.Hand.Count > 0)
                {
                    var mine = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Me.Hand.ToList(), 1, 1, "Hidden Power: shuffle which card from your opponent's hand into their deck?", ctx.Card.Def)[0];
                    ctx.Me.Hand.Remove(mine);
                    ctx.Me.Deck.Add(mine);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Wake-up Punch: base 60 if this Pokémon evolved from an Asleep Slakoth this turn.</summary>
        private sealed class WakeUpPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wake-up Punch") return false;
                var me = ctx.Pokemon;
                bool woke = me.TurnEvolved == ctx.Turn && EvolvedFrom(ctx, "Slakoth") && (me.ConditionsBeforeEvolve & SpecialCondition.Asleep) != 0;
                ctx.DamageDefending(woke ? 60 : atk.Damage);
                return true;
            }
        }

        /// <summary>Play Search: look at the top 5 cards, put 1 into your hand, then shuffle.</summary>
        private sealed class PlaySearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Play Search") return false;
                var top = ctx.LookAtTop(5);
                if (top.Count == 0) return true;
                ctx.DeckToHand(ctx.ChooseCards(top, 1, 1, "Play Search: put which card into your hand?"));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Ultra Evolution: search your deck for Alakazam and put it on this Pokémon (evolving it).</summary>
        private sealed class UltraEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultra Evolution") return false;
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.Name == "Alakazam" && d.IsPokemon && !d.IsLevelUp, 1, "Ultra Evolution: choose Alakazam");
                if (found.Count > 0 && ctx.InPlay(me) && ctx.Me.Deck.Remove(found[0])) ctx.Engine.EvolvePokemon(me, found[0]);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Throw Off: discard the Pokémon Tools on 1 of the opponent's Pokémon, then 20 to it (no W/R on the Bench).</summary>
        private sealed class ThrowOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Throw Off") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Throw Off: choose 1 of your opponent's Pokémon");
                if (target == null) return true;
                if (ctx.CanAffect(target)) ctx.Engine.DiscardAllTools(target);
                ctx.DamageTarget(target, 20);
                return true;
            }
        }

        /// <summary>Ghastly Sound: heads, the opponent can't play Supporter cards during their next turn.</summary>
        private sealed class GhastlySoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ghastly Sound") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsTWM2.SupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards during their next turn (Ghastly Sound)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Electromagnetic Jam: heads, the opponent can't attach Energy cards from their hand to their Active Pokémon next turn (the effect is on the player).</summary>
        private sealed class ElectromagneticJamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electromagnetic Jam") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new ActiveJamEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't attach Energy from their hand to their Active Pokémon next turn (Electromagnetic Jam)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class ActiveJamEffect : ContinuousEffect
        {
            private readonly int affected;

            public ActiveJamEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target)
            {
                return target != null && target.Owner == affected && engine.State.Players[affected].Active == target;
            }
        }

        /// <summary>Direct-hit Bomb: a coin for each Energy on this Pokémon; 10 per heads to 1 of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class DirectHitBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Direct-hit Bomb") return false;
                int heads = ctx.FlipCoins(ctx.EnergyCount(ctx.Pokemon));
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Direct-hit Bomb: choose 1 of your opponent's Pokémon");
                if (target != null && heads > 0) ctx.DamageTarget(target, 10 * heads);
                return true;
            }
        }

        /// <summary>Mountain Bite: you may discard the top card of your deck; if you do, 10 more and this Pokémon is now Asleep.</summary>
        private sealed class MountainBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Bite") return false;
                var me = ctx.Pokemon;
                bool more = ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Mountain Bite: discard the top card of your deck for 10 more damage (and fall Asleep)?");
                if (more) ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                ctx.DamageDefending(atk.Damage + (more ? 10 : 0));
                if (more && ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Plasma: heads, attach a Lightning Energy card from your discard pile to this Pokémon.</summary>
        private sealed class PlasmaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plasma") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !ctx.InPlay(me)) return true;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && CardsRS.BasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (pile.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Plasma: attach which Lightning Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Willpower: heads, during your next turn Rollout's base damage is 40. Rollout.</summary>
        private sealed class WillpowerMon : Mon
        {
            private const string Marker = "Willpower";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Willpower")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Rollout") return false;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 40 : atk.Damage);
                return true;
            }
        }

        /// <summary>Sweet Palm: before the damage, remove 1 damage counter from the Defending Pokémon.</summary>
        private sealed class SweetPalmMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Palm") return false;
                if (ctx.Defending != null && ctx.Defending.DamageCounters > 0) ctx.Heal(ctx.Defending, 10);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ------------------------------------------------------------------ Trainers

        /// <summary>Bebe's Search (Supporter): put a card from your hand on top of your deck, then search your deck for a Pokémon for your hand; shuffle.</summary>
        private sealed class BebesSearch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var card = ctx.ChooseCards(others, 1, 1, "Bebe's Search: put which card from your hand on top of your deck?")[0];
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Insert(0, card);
                ctx.SearchToHand(d => d.IsPokemon, 1, "Bebe's Search: choose a Pokémon");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Fossil Excavator (Supporter): a Trainer with "Fossil" in its name or a Stage 1 / Stage 2 that evolves from one, from your
        /// deck or discard pile, into your hand (not Old Amber or what evolves from it: May 2008).
        /// </summary>
        private sealed class FossilExcavator : CardScript
        {
            private static bool Wanted(CardDef d)
            {
                if (d.IsTrainer && d.Name.Contains("Fossil")) return true;
                if (!d.IsPokemon || (d.Stage != Stage.Stage1 && d.Stage != Stage.Stage2)) return false;
                return !string.IsNullOrEmpty(d.EvolvesFrom) && (d.EvolvesFrom.Contains("Fossil") || FossilLine.Contains(d.EvolvesFrom));
            }

            // Stage 1 Pokémon that evolve from a Fossil (their Stage 2s count too).
            private static readonly string[] FossilLine = { "Omanyte", "Kabuto", "Lileep", "Anorith", "Cranidos", "Shieldon", "Tirtouga", "Archen", "Tyrunt", "Amaura" };

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Me.Discard.Any(c => Wanted(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool discard = ctx.Me.Discard.Any(c => Wanted(c.Def)) && !ctx.Engine.DiscardToHandBlocked(ctx.Player)
                    && (ctx.Me.Deck.Count == 0 || ctx.ChooseOption("Fossil Excavator: search where?", "Deck", "Discard pile") == 1);
                if (discard)
                {
                    var card = ctx.ChooseCards(ctx.Me.Discard.Where(c => Wanted(c.Def)).ToList(), 1, 1, "Fossil Excavator: put which card into your hand?")[0];
                    ctx.DiscardToHand(new[] { card });
                    return;
                }
                ctx.SearchToHand(Wanted, 1, "Fossil Excavator: choose a Fossil card or a Pokémon that evolves from a Fossil");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Lake Boundary (Stadium): Weakness for each Pokémon (both players') is ×2 (a Weakness removed or reduced to 0 stays that way).</summary>
        private sealed class LakeBoundary : CardScript
        {
            public override bool AppliesWeaknessAsDouble(EffectContext src) { return true; }
        }

        /// <summary>Night Maintenance: up to 3 Pokémon and basic Energy cards from your discard pile shuffled into your deck.</summary>
        private sealed class NightMaintenance : CardScript
        {
            private static bool Wanted(CardDef d) { return d.IsPokemon || d.IsBasicEnergy; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => Wanted(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => Wanted(c.Def)).ToList();
                if (pile.Count == 0) return;
                ctx.DiscardToDeck(ctx.ChooseCards(pile, 1, Math.Min(3, pile.Count), "Night Maintenance: shuffle up to 3 Pokémon and basic Energy cards into your deck"));
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Quick Ball: reveal cards from the top of your deck until a Pokémon; it goes into your hand and the others are shuffled back (no shuffle if the first card was it: Oct 2007).</summary>
        private sealed class QuickBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int i = ctx.Me.Deck.FindIndex(c => c.Def.IsPokemon);
                var revealed = ctx.Me.Deck.Take(i < 0 ? ctx.Me.Deck.Count : i + 1).ToList();
                ctx.Engine.ShowCards(ctx.Opponent, revealed, "Quick Ball", "Quick Ball: the revealed cards", ctx.Source);
                if (i >= 0) ctx.DeckToHand(new[] { revealed[revealed.Count - 1] });
                if (i != 0) ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Team Galactic's Wager (Supporter): each player shuffles their hand into their deck; Rock-Paper-Scissors; the winner draws up
        /// to 6 cards, the loser up to 3 (you draw first; each draws at least 1 if they can). Not if nobody could draw.
        /// </summary>
        private sealed class TeamGalacticsWager : CardScript
        {
            private static readonly string[] Throws = { "Rock", "Paper", "Scissors" };

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count + ctx.Me.Hand.Count(c => c != ctx.Card) > 0 || ctx.Opp.Deck.Count + ctx.Opp.Hand.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var card in ctx.Me.Hand.Where(c => c != ctx.Card).ToList())
                {
                    ctx.Me.Hand.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                foreach (var card in ctx.Opp.Hand.ToList())
                {
                    ctx.Opp.Hand.Remove(card);
                    ctx.Opp.Deck.Add(card);
                }
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                bool iWin = false;
                for (int round = 0; ; round++)
                {
                    if (round >= 5)
                    {
                        iWin = ctx.Flip();
                        break;
                    }
                    int mine = ctx.ChooseOption("Team Galactic's Wager: Rock, Paper or Scissors?", Throws);
                    var opts = new List<DecisionOption>();
                    for (int i = 0; i < Throws.Length; i++) opts.Add(new DecisionOption { Label = Throws[i], Index = i });
                    int theirs = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Team Galactic's Wager: Rock, Paper or Scissors?", opts, ctx.Source, false);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " threw " + Throws[mine] + "; " + ctx.Engine.PlayerName(ctx.Opponent) + " threw " + Throws[theirs] + ".");
                    if (mine == theirs) continue;
                    iWin = (mine + 3 - theirs) % 3 == 1;
                    break;
                }
                ctx.DrawUntil(iWin ? 6 : 3);
                DrawUpTo(ctx, ctx.Opponent, iWin ? 3 : 6);
            }

            private static void DrawUpTo(EffectContext ctx, int player, int size)
            {
                var p = ctx.State.Players[player];
                int n = Math.Max(0, size - p.Hand.Count);
                if (n > 0) ctx.Engine.DrawCards(player, n);
            }
        }

        /// <summary>Armor Stone (Poké-Body): when damaged by the opponent's attack, flip until tails; 10 less for each heads.</summary>
        private sealed class ArmorFossil : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || damage <= 0 || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!info.ApplyOnce("Armor Stone#" + me.Uid)) return damage;
                int heads = src.Engine.FlipUntilTails(src.Player, "Armor Stone");
                return damage - 10 * heads;
            }
        }

        /// <summary>Skull Stone (Poké-Body): during the opponent's turn, if this would be Knocked Out by their attack's damage, flip until tails; 1 damage counter on the Attacking Pokémon for each heads.</summary>
        private sealed class SkullFossil : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (me == null || !byOpponentsAttack || src.IsMyTurn) return;
                var attacker = src.OpponentPokemon().FirstOrDefault(m => m.Uid == me.HitByAttackerUid);
                if (attacker == null) return;
                int heads = src.FlipUntilTails();
                Announce(src, "Skull Stone");
                if (heads > 0) src.Engine.PlaceDamageCounters(attacker, heads, "Skull Stone");
            }
        }

        /// <summary>Time-Space Distortion: flip 3 coins; for each heads, a Pokémon from your discard pile into your hand.</summary>
        private sealed class TimeSpaceDistortion : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsPokemon) && !ctx.Engine.DiscardToHandBlocked(ctx.Player); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(3);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (heads == 0 || pile.Count == 0) return;
                int n = Math.Min(heads, pile.Count);
                ctx.DiscardToHand(ctx.ChooseCards(pile, n, n, "Time-Space Distortion: put which Pokémon into your hand?"));
            }
        }
    }
}
