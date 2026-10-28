using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unbroken Bonds (UNB) part 2: Psychic, Fighting and Darkness (UNB 61-119). No regulation mark: Expanded. Hypno's Hypnotic
    /// Pendulum lets its owner choose the opponent's new Active Pokémon (GameEngine.PromoteActive); Gastly's Swelling Spite runs
    /// once the Knocked Out Gastly has left play (OnHolderKnockedOutRemoved). Your rulings (v73): Dark Union-GX can bench
    /// Evolution Pokémon too, and Ruler of the Night stops any attaching of a Special Energy card from the hand.
    /// </summary>
    public static class CardsUNB2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UNB-61", new MukAlolanMukGX());
            r.RegisterByKey("UNB-62", new ParalyzeFlipMon("Glare"));
            r.RegisterByKey("UNB-63", new ComboMon(new ParalyzeFlipMon("Wrap"), new CardsFST2.ConditionBonusMon("Heavy Choke", 120, UsedWrapLastTurn)));
            r.RegisterByKey("UNB-64", new CardsBLK.VenoshockMon(50));
            r.RegisterByKey("UNB-65", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("UNB-66", new CrobatUNB());
            r.RegisterByKey("UNB-67", new GastlyUNB());
            r.RegisterByKey("UNB-68", new ConditionAttackMon("Smog", SpecialCondition.Poisoned));
            r.RegisterByKey("UNB-69", new SinisterFogMon());
            r.RegisterByKey("UNB-70", new GengarUNB());
            r.RegisterByKey("UNB-71", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("UNB-72", new HypnoUNB());
            r.RegisterByKey("UNB-73", new FoulOdorMon());
            r.RegisterByKey("UNB-74", new WeezingUNB());
            r.RegisterByKey("UNB-75", new MewtwoUNB());
            r.RegisterByKey("UNB-76", new MewUNB());
            r.RegisterByKey("UNB-77", new CardsRCL1.CountersOnOneMon("Ominous Eyes", 1));
            r.RegisterByKey("UNB-78", new MismagiusUNB());
            r.RegisterByKey("UNB-79", new ComboMon(new CardsUNB1.CaturdayMon(1), new EarKinesisMon()));
            r.RegisterByKey("UNB-80", new ComboMon(new CardsUNB1.CaturdayMon(3), new PerplexingEyesMon()));
            r.RegisterByKey("UNB-81", new MarshadowUNB());
            r.RegisterByKey("UNB-82", new MarshadowMachampGX());
            r.RegisterByKey("UNB-83", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("UNB-84", new ComboMon(new CoinsMon("Continuous Scratch", 4), new NoRetreatMon("Sand Tomb")));
            r.RegisterByKey("UNB-85", new PlainMon()); // Underground Work: Giovanni's Exile (CardsHIF1) handles it
            r.RegisterByKey("UNB-86", new CardsPAL1.StadiumBonusMon("Home Ground", 60));
            r.RegisterByKey("UNB-87", new CardsSP1.FlipNoDamageMon("Defense Curl"));
            r.RegisterByKey("UNB-88", new RecoilMon("Double-Edge", 30));
            r.RegisterByKey("UNB-89", new ComboMon(new CardsFLF1.NoResistanceMon("Steamroller"), new LessPerRetreatMon("Heavy Slam", 20)));
            r.RegisterByKey("UNB-90", new CardsRCL1.SetupBoostMon("Focus Energy", "Beat", 70));
            r.RegisterByKey("UNB-91", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Bone Rush"), new CardsFST2.ConditionBonusMon("Assault Boom", 70, DefendingHasTool)));
            r.RegisterByKey("UNB-92", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("UNB-94", new DirtyWorkMon());
            r.RegisterByKey("UNB-95", new ComboMon(new CardsEVS1.DefenderCantAttackMon("Hefty Cannon", d => d.IsBasicPokemon), new CardsBUS1.OwnBenchQuakeMon("Break Ground", 20)));
            r.RegisterByKey("UNB-98", new GligarUNB());
            r.RegisterByKey("UNB-99", new ComboMon(new DrawAttackMon("Collect", 3), new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("UNB-100", new TyrogueUNB());
            r.RegisterByKey("UNB-101", new HitmontopUNB());
            r.RegisterByKey("UNB-102", new CardsFLF1.NoResistanceMon("Feint"));
            r.RegisterByKey("UNB-103", new ComboMon(new CardsMEG.SnipeMon("Linear Attack", 30), new CardsFST1.MoveEnergyToBenchMon("Power Cyclone")));
            r.RegisterByKey("UNB-104", new CardsPAL1.PrizeBehindBonusMon("Knuckle Blast", 60));
            r.RegisterByKey("UNB-105", new FightAloneMon(50));
            r.RegisterByKey("UNB-106", new StakatakaUNB());
            r.RegisterByKey("UNB-107", new GreninjaZoroarkGX());
            r.RegisterByKey("UNB-109", new HonchkrowGX());
            r.RegisterByKey("UNB-111", new SharpedoUNB());
            r.RegisterByKey("UNB-112", new SpiritombUNB());
            r.RegisterByKey("UNB-113", new SandileUNB());
            r.RegisterByKey("UNB-114", new GrandioseFangsMon());
            r.RegisterByKey("UNB-115", new NoRetreatMon("Corner"));
            r.RegisterByKey("UNB-116", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Chomp Chomp Panic", 50, true), new CardsDRI2.CrunchMon("Crunch")));
            r.RegisterByKey("UNB-117", new ComboMon(new BringDownMon(), new CardsCRZ1.PiercingMon("Mist Slash")));
            r.RegisterByKey("UNB-118", new ConditionAttackMon("Perplex", SpecialCondition.Confused));
            r.RegisterByKey("UNB-119", new ComboMon(new HypnoticReignMon(), new CardsSSP2.StompOffMon("Dark Pressure", 1)));
        }

        // ================================================================== Shared helpers

        private static bool UsedWrapLastTurn(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            return me != null && me.LastAttackName == "Wrap" && me.LastAttackTurn == ctx.MyLastTurn;
        }

        private static bool DefendingHasTool(EffectContext ctx) { return ctx.Defending != null && ctx.Defending.Tools.Count > 0; }

        /// <summary>Puts Poison on the opponent's Active Pokémon with N damage counters in Pokémon Checkup instead of 1 (Severe Poison, Nasty Goo Mix-GX).</summary>
        internal static void HeavyPoison(EffectContext ctx, PokemonInPlay defending, int counters)
        {
            if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = counters;
        }

        /// <summary>"This attack does N less damage for each Colorless in your opponent's Active Pokémon's Retreat Cost" (Heavy Slam).</summary>
        internal sealed class LessPerRetreatMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public LessPerRetreatMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                int damage = Math.Max(0, atk.Damage - per * cost);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log(attack + " did no damage.");
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Severe Poison: Poisoned with 8 damage counters in Pokémon Checkup. Poison Absorption: if the opponent's Active Pokémon is
        /// Poisoned, heal 100 from this Pokémon. Nasty Goo Mix-GX: Paralyzed and Poisoned; with 4 extra Energy, 15 counters of Poison.
        /// </summary>
        private sealed class MukAlolanMukGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Severe Poison")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    HeavyPoison(ctx, defending, 8);
                    return true;
                }
                if (atk.Name == "Poison Absorption")
                {
                    bool poisoned = defending != null && defending.Has(SpecialCondition.Poisoned);
                    ctx.DamageDefending(atk.Damage);
                    if (poisoned && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 100);
                    return true;
                }
                if (atk.Name == "Nasty Goo Mix-GX")
                {
                    bool extra = ExtraEnergy(ctx, atk, 4);
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending)) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    HeavyPoison(ctx, defending, extra ? 15 : 1);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Sound Veil: when you play it from your hand to evolve, you may prevent all effects of attacks, damage included, done to it
        /// during the opponent's next turn. Severe Poison: Poisoned with 4 damage counters in Pokémon Checkup.
        /// </summary>
        private sealed class CrobatUNB : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !TriggerOnce(ctx, "Sound Veil", true, "Sound Veil: prevent all effects of attacks done to " + me.Name + " during your opponent's next turn?")) return;
                ctx.AddEffect(new CardsPAR1.StanceEffect(me.Name + " is protected from attacks (Sound Veil)", ctx.Player, me.Uid, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Severe Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                HeavyPoison(ctx, defending, 4);
                return true;
            }
        }

        /// <summary>Swelling Spite: when this Pokémon is Knocked Out, search your deck for up to 2 Haunter and put them onto your Bench (once it has left play).</summary>
        private sealed class GastlyUNB : Mon
        {
            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                int room = Math.Min(2, src.BenchSpace);
                if (room <= 0 || src.Me.Deck.Count == 0) return;
                src.Engine.Emit(GameEventType.UseAbility, src.Player, "Swelling Spite: " + src.Engine.PlayerName(src.Player) + " searches for up to 2 Haunter.", src.Card.Id);
                var found = src.SearchMyDeck(d => d.Name == "Haunter" && !src.Engine.PutIntoPlayBlocked(src.Player, d), room, "Swelling Spite: put up to " + room + " Haunter onto your Bench");
                foreach (var card in found) src.Engine.PutOntoBench(src.Player, card, false);
                src.ShuffleMyDeck();
            }
        }

        /// <summary>Sinister Fog: the opponent's Active Pokémon is Poisoned; 1 damage counter on each of their Benched Pokémon.</summary>
        internal sealed class SinisterFogMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sinister Fog") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                foreach (var mon in ctx.OpponentBench()) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>Shadow Pain: when you play it from your hand to evolve, you may put 6 damage counters on the opponent's Pokémon-GX and Pokémon-EX in any way you like. Twilight Poison: Asleep and Poisoned.</summary>
        private sealed class GengarUNB : Mon
        {
            private static readonly Mon Twilight = new CardsCES1.ConditionsMon("Twilight Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Shadow Pain", ctx.OpponentPokemon(m => m.Def.IsGXorEX).Count > 0,
                    "Shadow Pain: put 6 damage counters on your opponent's Pokémon-GX and Pokémon-EX?")) return;
                ctx.PlaceCountersAnyWay(6, () => ctx.OpponentPokemon(m => m.Def.IsGXorEX));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Twilight.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Hypnotic Pendulum: when the opponent's Active Pokémon is Knocked Out, flip; on heads you choose their new Active Pokémon.
        /// Stir the Brain: 10 more for each card in the opponent's hand.
        /// </summary>
        private sealed class HypnoUNB : Mon
        {
            private static readonly Mon Stir = new CardsASR1.PlusMon("Stir the Brain", 10, c => c.Opp.Hand.Count);

            public override bool ChoosesOpponentsNewActive(EffectContext src, int player)
            {
                if (src.Pokemon == null || player == src.Player || src.State.Players[player].Bench.Count < 2) return false;
                src.Engine.Emit(GameEventType.UseAbility, src.Player, src.Engine.OwnedName(src.Pokemon) + " used its Ability Hypnotic Pendulum.", src.Card.Id, src.Pokemon.Uid);
                bool heads = src.Engine.FlipCoin(src.Player, "Hypnotic Pendulum");
                if (heads) src.Log(src.Engine.PlayerName(src.Player) + " chooses " + src.Engine.Their(player) + " new Active Pokémon (Hypnotic Pendulum).");
                return heads;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Stir.HandleAttack(ctx, atk); }
        }

        /// <summary>Foul Odor: both Active Pokémon are now Confused.</summary>
        private sealed class FoulOdorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foul Odor") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Detention Gas: while this Pokémon is your Active Pokémon, put 1 damage counter on each of the opponent's Basic Pokémon
        /// between turns. Splattering Sludge: also 20 to each of their Benched Pokémon that has damage counters on it.
        /// </summary>
        private sealed class WeezingUNB : Mon
        {
            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? 1 - src.Player : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null || !src.PokemonIsActive) return;
                foreach (var mon in src.OpponentPokemon(m => m.IsBasic)) src.Engine.PlaceDamageCounters(mon, 1, "Detention Gas");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splattering Sludge") return false;
                var hurt = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in hurt)
                    if (ctx.InPlay(mon) && !ctx.IsActive(mon)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Mind Report: when you play it from your hand onto your Bench, you may put a Supporter card from your discard pile on top of your deck. Psyshock: ignores effects on the Defending Pokémon.</summary>
        private sealed class MewtwoUNB : Mon
        {
            private static readonly Mon Psyshock = new CardsPAF2.IgnoreDefenderEffectsMon("Psyshock");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (!ctx.IsMyTurn || supporters.Count == 0 || !ctx.AskYesNo("Mind Report: put a Supporter card from your discard pile on top of your deck?")) return;
                Announce(ctx, "Mind Report");
                var pick = ctx.ChooseCards(supporters, 1, 1, "Mind Report: put which Supporter card on top of your deck?")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Me.Deck.Insert(0, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psyshock.HandleAttack(ctx, atk); }
        }

        /// <summary>Bench Barrier: prevent all damage done to your Benched Pokémon by the opponent's attacks. Psypower: 3 damage counters in any way you like.</summary>
        private sealed class MewUNB : Mon
        {
            private static readonly Mon Psypower = new CardsLOR1.CountersAnyWayMon("Psypower", 3);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target != null && info.Target.Owner == src.Player && !info.TargetIsActive && info.AgainstOpponent;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psypower.HandleAttack(ctx, atk); }
        }

        /// <summary>Mysterious Message: once during your turn, draw until you have 7 cards; if you drew any, this Pokémon is Knocked Out. Hypnoblast: Asleep.</summary>
        private sealed class MismagiusUNB : Mon
        {
            private static readonly Mon Hypnoblast = new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep);

            protected override string Activated { get { return "Mysterious Message"; } }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Mysterious Message"; }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 7 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.DrawUntil(7) > 0) ctx.Engine.KnockOut(ctx.Pokemon, "Mysterious Message");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hypnoblast.HandleAttack(ctx, atk); }
        }

        /// <summary>Ear Kinesis: 20 damage to 1 of the opponent's Benched Pokémon for each damage counter on it.</summary>
        private sealed class EarKinesisMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ear Kinesis") return false;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Ear Kinesis: choose 1 of your opponent's Benched Pokémon (20 damage for each damage counter on it)");
                if (target == null) return true;
                if (target.DamageCounters > 0) ctx.DamageTarget(target, 20 * target.DamageCounters);
                else ctx.Log("Ear Kinesis: " + target.Name + " has no damage counters, so it takes no damage.");
                return true;
            }
        }

        /// <summary>Perplexing Eyes: until the end of your next turn, the Defending Pokémon's Weakness is Psychic.</summary>
        internal sealed class PerplexingEyesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Perplexing Eyes") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new Cards30C.WeaknessChangeEffect(defending.Name + "'s Weakness is Psychic (Perplexing Eyes)", ctx.Player, defending.Uid, ctx.Turn + 2, EnergyType.Psychic));
                return true;
            }
        }

        /// <summary>Resetting Hole: once during your turn, from the Bench, discard the Stadium in play, then discard this Pokémon and its cards. Red Knuckles: 60 more against an Ultra Beast.</summary>
        private sealed class MarshadowUNB : Mon
        {
            private static readonly Mon Knuckles = new CardsFST2.ConditionBonusMon("Red Knuckles", 60, c => c.Defending != null && c.Defending.Def.IsUltraBeast);

            protected override string Activated { get { return "Resetting Hole"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.State.Stadium != null && ctx.Pokemon != null && !ctx.PokemonIsActive; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.State.Stadium == null) return;
                ctx.Engine.DiscardStadium();
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardPokemonFromPlay(ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Knuckles.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>
        /// Revenge: 90 more if any of your Pokémon were Knocked Out by damage from an attack during the opponent's last turn. Acme of
        /// Heroism-GX: with 1 extra Energy, if it would be Knocked Out by damage from an opponent's attack during their next turn, it
        /// isn't and its remaining HP becomes 10.
        /// </summary>
        private sealed class MarshadowMachampGX : Mon
        {
            private static readonly Mon Revenge = new Cards30C.RetaliateBonusMon("Revenge", 90);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Acme of Heroism-GX")
                {
                    bool extra = ExtraEnergy(ctx, atk, 1);
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (extra && ctx.InPlay(me))
                        ctx.AddEffect(new AcmeEffect(me.Name + " can't be Knocked Out by the opponent's attacks (Acme of Heroism-GX)", ctx.Player, me.Uid, ctx.Turn + 1));
                    return true;
                }
                return Revenge.HandleAttack(ctx, atk);
            }
        }

        private sealed class AcmeEffect : ContinuousEffect
        {
            public AcmeEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Dirty Work: discard the top card of the opponent's deck (5 cards if you played Giovanni's Exile from your hand this turn).</summary>
        private sealed class DirtyWorkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dirty Work") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int count = ctx.Me.PlayedSupporterThisTurn("Giovanni's Exile") ? 5 : 1;
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, count);
                return true;
            }
        }

        /// <summary>Free Flight: no Retreat Cost with no Energy attached. Shinobi Strike: 90 more if you played Janine from your hand this turn.</summary>
        private sealed class GligarUNB : Mon
        {
            private static readonly Mon Strike = new CardsBRS1.SupporterBonusMon("Shinobi Strike", "Janine", 90);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Strike.HandleAttack(ctx, atk); }
        }

        /// <summary>Bratty Kick: once during your turn, flip; on heads put 3 damage counters on 1 of the opponent's Pokémon. Your turn ends.</summary>
        private sealed class TyrogueUNB : Mon
        {
            protected override string Activated { get { return "Bratty Kick"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Bratty Kick: put 3 damage counters on which of your opponent's Pokémon?");
                    if (target != null) ctx.PlaceCounters(target, 3);
                }
                else ctx.Log("Bratty Kick: tails.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Finishing Combo: only if your Hitmonlee used Special Combo during your last turn; 60 damage to each of the opponent's Pokémon.</summary>
        private sealed class HitmontopUNB : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Finishing Combo") return true;
                var me = ctx.Me;
                return me.LastAttackName == "Special Combo" && me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackerDef != null && me.LastAttackerDef.Name == "Hitmonlee";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Finishing Combo") return false;
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, 60);
                return true;
            }
        }

        /// <summary>Fight Alone: 50 more for each Pokémon fewer you have in play than the opponent. The numbers are passed in.</summary>
        internal sealed class FightAloneMon : Mon
        {
            private readonly int each;

            public FightAloneMon(int each)
            {
                this.each = each;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fight Alone") return false;
                int fewer = ctx.OpponentPokemon().Count - ctx.MyPokemon().Count;
                ctx.DamageDefending(atk.Damage + (fewer > 0 ? each * fewer : 0));
                return true;
            }
        }

        /// <summary>Wall of Stone: max HP 200 while the opponent has 3 or fewer Prize cards left. Top Down: flip until tails; discard the top card of the opponent's deck for each heads.</summary>
        private sealed class StakatakaUNB : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Opp.Prizes.Count <= 3 ? 200 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Top Down") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>
        /// Dark Pulse: 30 more for each Darkness Energy on all your Pokémon. Dark Union-GX: put 2 Darkness Pokémon-GX / Pokémon-EX
        /// from your discard pile onto your Bench (Evolution Pokémon too: your ruling, v73); with 1 extra Energy, attach 2 Energy
        /// cards from your discard pile to each of them.
        /// </summary>
        private sealed class GreninjaZoroarkGX : Mon
        {
            private static readonly Mon Pulse = new CardsASR1.PlusMon("Dark Pulse", 30, DarknessEnergyInPlay);

            private static int DarknessEnergyInPlay(EffectContext ctx)
            {
                int n = 0;
                foreach (var mon in ctx.MyPokemon()) n += ctx.Engine.EnergyCountOfType(mon, EnergyType.Darkness);
                return n;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Union-GX") return Pulse.HandleAttack(ctx, atk);
                bool extra = ExtraEnergy(ctx, atk, 1);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.IsGXorEX && c.Def.PokemonType == EnergyType.Darkness
                                                      && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                int room = Math.Min(2, ctx.BenchSpace);
                if (pile.Count == 0 || room <= 0) return true;
                var picks = ctx.ChooseCards(pile, 0, Math.Min(room, pile.Count), "Dark Union-GX: put up to " + room + " Darkness Pokémon-GX or Pokémon-EX from your discard pile onto your Bench");
                var benched = new List<PokemonInPlay>();
                foreach (var card in picks)
                {
                    if (ctx.BenchSpace <= 0 || !ctx.Me.Discard.Remove(card)) continue;
                    var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    if (mon != null) benched.Add(mon);
                }
                if (!extra) return true;
                foreach (var mon in benched)
                {
                    var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                    if (energy.Count == 0) break;
                    var chosen = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Dark Union-GX: attach 2 Energy cards from your discard pile to " + mon.Name);
                    foreach (var card in chosen)
                        if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, mon);
                }
                return true;
            }
        }

        /// <summary>
        /// Ruler of the Night: while this Pokémon is your Active Pokémon, the opponent can't play Pokémon Tool, Special Energy or
        /// Stadium cards from their hand (by any means for Special Energy: your ruling, v73). Feather Storm: also 30 to 2 of their
        /// Benched Pokémon-GX / Pokémon-EX. Unfair-GX: the opponent reveals their hand and you discard 2 cards from it.
        /// </summary>
        private sealed class HonchkrowGX : Mon
        {
            private static bool RulerOn(EffectContext src, int player) { return src.Pokemon != null && player != src.Player && src.PokemonIsActive; }

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return RulerOn(src, player) && (card.Def.IsTool || card.Def.IsStadium);
            }

            public override bool BlocksEnergyCard(EffectContext src, int player, CardInstance card)
            {
                // A Pokémon card attached as an Energy (Charjabug's Battery) counts as a Special Energy card.
                return RulerOn(src, player) && (card.Def.IsSpecialEnergy || card.Def.IsPokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Feather Storm")
                {
                    ctx.DamageDefending(atk.Damage);
                    var targets = ctx.OpponentBench().Where(m => m.Def.IsGXorEX).ToList();
                    if (targets.Count == 0) return true;
                    var picks = targets.Count <= 2 ? targets : ctx.ChoosePokemonMany(targets, 2, 2, "Feather Storm: choose 2 of your opponent's Benched Pokémon-GX or Pokémon-EX (30 damage each)");
                    foreach (var mon in picks) ctx.DamageTarget(mon, 30);
                    return true;
                }
                if (atk.Name == "Unfair-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    TefPokemon.RevealOpponentHand(ctx);
                    var hand = ctx.Opp.Hand.ToList();
                    if (hand.Count == 0) return true;
                    var picks = hand.Count <= 2 ? hand : ctx.ChooseCards(hand, 2, 2, "Unfair-GX: discard which 2 cards from your opponent's hand?");
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Greedy Evolution: when you play it from your hand to evolve, you may look at the top 6 cards of your deck and attach any
        /// number of Darkness Energy cards there to it; shuffle the rest back. Bad Fangs: 20 more for each Darkness Energy on it.
        /// </summary>
        private sealed class SharpedoUNB : Mon
        {
            private static readonly Mon Fangs = new CardsMEG.EnergyBonusMon("Bad Fangs", EnergyType.Darkness, 20);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !TriggerOnce(ctx, "Greedy Evolution", ctx.Me.Deck.Count > 0, "Greedy Evolution: look at the top 6 cards of your deck for Darkness Energy cards?")) return;
                var top = ctx.LookAtTop(6);
                var dark = top.Where(c => CardsUNB1.IsBasicEnergyOf(c.Def, EnergyType.Darkness)).ToList();
                if (dark.Count > 0)
                {
                    var picks = ctx.ChooseCards(dark, 0, dark.Count, "Greedy Evolution: attach any number of these Darkness Energy cards to " + me.Name);
                    foreach (var card in picks)
                        if (ctx.Me.Deck.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fangs.HandleAttack(ctx, atk); }
        }

        /// <summary>Building Spite: once during your turn, put 1 damage counter on this Pokémon. Anguish Cry: 30 more for each damage counter on it.</summary>
        private sealed class SpiritombUNB : Mon
        {
            private static readonly Mon Anguish = new Cards30C.SelfCounterBonusMon("Anguish Cry", 30);

            protected override string Activated { get { return "Building Spite"; } }

            protected override void Ability(EffectContext ctx) { ctx.Engine.PlaceDamageCounters(ctx.Pokemon, 1, "Building Spite"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Anguish.HandleAttack(ctx, atk); }
        }

        /// <summary>Intimidating Fang: while this Pokémon is your Active Pokémon, the opponent's Active Pokémon's attacks do 20 less damage (before Weakness and Resistance).</summary>
        private sealed class SandileUNB : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || !src.PokemonIsActive || attacker == null || attacker.Owner == src.Player || attacker != src.Opp.Active || damage <= 0) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>Grandiose Fangs: if this attack's damage Knocks Out the opponent's Pokémon, this Pokémon's attacks do 120 more damage to their Active Pokémon during your next turn.</summary>
        private sealed class GrandioseFangsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grandiose Fangs") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (defending != null && ctx.Engine.IsKnockedOut(defending) && ctx.InPlay(me))
                    ctx.AddEffect(new CardsCRI.NextTurnPowerEffect(me.Name + "'s attacks do 120 more damage next turn (Grandiose Fangs)", ctx.Player, me.Uid, ctx.Turn + 2, 120));
                return true;
            }
        }

        /// <summary>Bring Down: the Pokémon in play, other than this one, with the lowest remaining HP is Knocked Out (you choose among ties).</summary>
        private sealed class BringDownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bring Down") return false;
                var self = ctx.Pokemon;
                var all = ctx.MyPokemon(m => m != self);
                all.AddRange(ctx.OpponentPokemon());
                if (all.Count == 0) return true;
                int low = all.Min(m => ctx.RemainingHp(m));
                var tied = all.Where(m => ctx.RemainingHp(m) == low).ToList();
                var target = ctx.ChoosePokemon(tied, "Bring Down: which of the Pokémon with the lowest remaining HP is Knocked Out?");
                if (target != null) ctx.Engine.KnockOut(target, "Bring Down");
                return true;
            }
        }

        /// <summary>Hypnotic Reign: the opponent reveals their hand; you may discard a Pokémon from it and use 1 of its attacks (not a GX attack) as this attack.</summary>
        private sealed class HypnoticReignMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypnotic Reign") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                var choices = ctx.Opp.Hand.Where(c => c.Def.IsPokemon && c.Def.Attacks.Any(a => !GameEngine.IsGxAttack(a))).ToList();
                if (choices.Count == 0) return true;
                var picks = ctx.ChooseCards(choices, 0, 1, "Hypnotic Reign: you may discard a Pokémon from your opponent's hand and use 1 of its attacks");
                if (picks.Count == 0) return true;
                var source = picks[0].Def;
                ctx.Engine.DiscardFromHand(ctx.Opponent, picks[0]);
                var opts = new List<DecisionOption>();
                for (int i = 0; i < source.Attacks.Count; i++)
                {
                    var move = source.Attacks[i];
                    if (GameEngine.IsGxAttack(move)) continue;
                    opts.Add(new DecisionOption { Label = source.Name + ": " + move.Name + (move.Damage > 0 ? " (" + move.Damage + move.DamageSuffix + ")" : ""), Card = source, Index = i });
                }
                if (opts.Count == 0) return true;
                int pick = opts.Count == 1 ? 0 : ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Hypnotic Reign: use which attack?", opts, ctx.Source);
                UseCopiedAttack(ctx, source, opts[pick].Index);
                return true;
            }
        }
    }
}
