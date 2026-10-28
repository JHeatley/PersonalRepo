using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Paradox Rift (PAR) 1-88: the Grass, Fire, Water, Lightning and Psychic Pokémon that weren't already in the pool. Every
    /// card is G-mark (Expanded only). Reprints of cards already in the pool (Pansear, Armarouge ex, Chi-Yu, Horsea, Iron Bundle,
    /// Iron Hands ex, Natu, Xatu, Yamask, Scream Tail...) are alternate prints of those cards. Pokémon whose attacks only do
    /// their printed damage need no script: Dwebble, Nymble, Fuecoco, Remoraid, Vanillite, Vanillish, Wiglett (52).
    /// The rest of the set is in CardsPAR2.
    /// </summary>
    public static class CardsPAR1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PAR-1", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("PAR-2", new MasquerainPAR());
            r.RegisterByKey("PAR-3", new FroslassExPAR());
            r.RegisterByKey("PAR-4", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new CardsTWM1.HealSelfMon("Leech Seed", 10)));
            r.RegisterByKey("PAR-5", new MonkeyTrioMon(new ArmThrustNeedleMon()));
            r.RegisterByKey("PAR-7", new CardsASR1.FrostWallMon("Crag Bash", 100));
            r.RegisterByKey("PAR-8", new FlipBonusMon("Quick Blow", 20));
            r.RegisterByKey("PAR-9", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("PAR-10", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PAR-11", new DodgeMon("Protect", false));
            r.RegisterByKey("PAR-12", new OrbeetlePAR());
            r.RegisterByKey("PAR-15", new ToedscoolPAR());
            r.RegisterByKey("PAR-16", new CardsTWM1.HealSelfMon("Absorb", 20));
            r.RegisterByKey("PAR-17", new ToedscruelPAR());
            r.RegisterByKey("PAR-18", new WoChienPAR());
            r.RegisterByKey("PAR-19", new MagbyPAR());
            r.RegisterByKey("PAR-21", new MonkeyTrioMon(new RecoilMon("Heat Tackle", 30)));
            r.RegisterByKey("PAR-22", new VolcanionPAR());
            r.RegisterByKey("PAR-24", new CardsDRI1.EnergyLoopMon("Rolling Fireball"));
            r.RegisterByKey("PAR-25", new DodgeMon("Protect", false));
            r.RegisterByKey("PAR-26", new CardsMEG.ChargeSelfMon("Fiery Fighting Spirit", EnergyType.Fire, 1));
            r.RegisterByKey("PAR-28", new IronMothPAR());
            r.RegisterByKey("PAR-31", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("PAR-32", new KingdraPAR());
            r.RegisterByKey("PAR-34", new OctilleryPAR());
            r.RegisterByKey("PAR-35", new SearchAttackMon("Drawup Power", d => d.IsEnergy, 1));
            r.RegisterByKey("PAR-36", new MiloticPAR());
            r.RegisterByKey("PAR-37", new SnoruntPAR());
            r.RegisterByKey("PAR-38", new GarchompExPAR());
            r.RegisterByKey("PAR-39", new MantykePAR());
            r.RegisterByKey("PAR-40", new ComboMon(new SwitchSelfMon("Teleportation Burst", true), new PrizeCountMon(80)));
            r.RegisterByKey("PAR-41", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("PAR-42", new MonkeyTrioMon(new CardsBLW.ElectrisparkMon("Liquid Lashing", 30)));
            r.RegisterByKey("PAR-45", new VanilluxePAR());
            r.RegisterByKey("PAR-46", new TsareenaExPAR());
            r.RegisterByKey("PAR-47", new DodgeMon("Hide", false));
            r.RegisterByKey("PAR-48", new WimpodPAR());
            r.RegisterByKey("PAR-49", new GolisopodPAR());
            r.RegisterByKey("PAR-50", new GolisopodExPAR());
            r.RegisterByKey("PAR-51", new CoinsMon("Fury Headbutt", 3));
            r.RegisterByKey("PAR-53", new WugtrioPAR());
            r.RegisterByKey("PAR-54", new VeluzaPAR());
            r.RegisterByKey("PAR-55", new DondozoPAR());
            r.RegisterByKey("PAR-57", new ComboMon(new BringerMon("Snow Bringer", EnergyType.Water, 2), new CardsTWM1.SelfDiscardMon("Wrathful Blade", 2)));
            r.RegisterByKey("PAR-58", new MewtwoExPAR());
            r.RegisterByKey("PAR-59", new CardsMEG.SnipeMon("Crackling Shot", 30));
            r.RegisterByKey("PAR-60", new PlusMon("Plus"));
            r.RegisterByKey("PAR-61", new MinunPAR());
            r.RegisterByKey("PAR-62", new RecoilMon("Wild Charge", 20));
            r.RegisterByKey("PAR-63", new ZebstrikaPAR());
            r.RegisterByKey("PAR-64", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("PAR-65", new Cards30CC.BenchHitMon("Electrobullet", 30));
            r.RegisterByKey("PAR-66", new ZekromPAR());
            r.RegisterByKey("PAR-67", new OricorioPAR());
            r.RegisterByKey("PAR-68", new TapuKokoExPAR());
            r.RegisterByKey("PAR-69", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("PAR-73", new CardsTWM1.SelfDiscardMon("Luster Purge", 3));
            r.RegisterByKey("PAR-74", new DeoxysPAR());
            r.RegisterByKey("PAR-76", new CofagrigusExPAR());
            r.RegisterByKey("PAR-77", new RecoilMon("Reckless Charge", 20));
            r.RegisterByKey("PAR-78", new GourgeistPAR());
            r.RegisterByKey("PAR-79", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("PAR-80", new CardsMEG.DefendingEnergyBonusMon("Psychic", 10));
            r.RegisterByKey("PAR-81", new EspathraPAR());
            r.RegisterByKey("PAR-82", new TinkatinkPAR());
            r.RegisterByKey("PAR-83", new LockAfterAttackMon("Boundless Power", false));
            r.RegisterByKey("PAR-84", new Cards30C.TypeEnergyBonusMon("Alloy Aswing", EnergyType.Metal, 40));
            r.RegisterByKey("PAR-85", new ComboMon(new CardsDRI2.CrunchMon("Crushing Blow"), new Cards30C.TypeEnergyBonusMon("Alloyed Hammer", EnergyType.Metal, 120)));
            r.RegisterByKey("PAR-87", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("PAR-88", new CardsPFL.FlipUntilTailsTimesMon("Continuous Coin Toss"));
        }

        // ================================================================== Shared helpers

        /// <summary>
        /// Leaf / Snow / Sand Bringer: "Attach up to N Basic [T] Energy cards from your discard pile to 1 of your Pokémon."
        /// </summary>
        public sealed class BringerMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int max;

            public BringerMon(string attack, EnergyType type, int max)
            {
                this.attack = attack;
                this.type = type;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                if (pile.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), atk.Name + ": attach Basic " + type + " Energy from your discard pile to which of your Pokémon?");
                if (target == null) return true;
                var picks = ctx.ChooseCards(pile, 1, Math.Min(max, pile.Count), atk.Name + ": attach up to " + max + " Basic " + type + " Energy cards to " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>
        /// Monkey Trio (Simisage, Simisear, Simipour): if you have all three in play, ignore all Colorless Energy in the costs
        /// of this Pokémon's attacks.
        /// </summary>
        private sealed class MonkeyTrioMon : Mon
        {
            private readonly Mon attacks;

            public MonkeyTrioMon(Mon attacks) { this.attacks = attacks; }

            private static bool TrioInPlay(EffectContext ctx)
            {
                var names = ctx.Me.InPlay().Select(m => m.Name).ToList();
                return names.Contains("Simisage") && names.Contains("Simisear") && names.Contains("Simipour");
            }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !TrioInPlay(src)) return;
                cost.RemoveAll(t => t == EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Prevent all effects of attacks used by your opponent's Pokémon done to this Pokémon. (Damage is not an effect.)"</summary>
        public sealed class AttackEffectShieldEffect : ContinuousEffect
        {
            public AttackEffectShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return target.Uid == TargetUid && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>
        /// Espathra's Stance: prevent all damage from and effects of the opponent's attacks on this Pokémon. An Ability's effect,
        /// so unlike an attack's protection it isn't removed when the Pokémon moves to the Bench.
        /// </summary>
        public sealed class StanceEffect : ContinuousEffect
        {
            public StanceEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return target.Uid == TargetUid && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>"During your opponent's next turn, attacks used by the Defending Pokémon cost N Colorless more."</summary>
        public sealed class AttackCostUpEffect : ContinuousEffect
        {
            private readonly int amount;

            public AttackCostUpEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
            }

            public override void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon.Uid != TargetUid) return;
                for (int i = 0; i < amount; i++) cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Prize Count: 80 more if you have more Prize cards remaining than your opponent. The numbers are passed in.</summary>
        internal sealed class PrizeCountMon : Mon
        {
            private readonly int amount;

            public PrizeCountMon(int amount)
            {
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Prize Count") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count ? amount : 0));
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Daunting Eyes: flip until tails; for each heads, shuffle an Energy attached to the opponent's Active Pokémon into their deck.</summary>
        private sealed class MasquerainPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Daunting Eyes") return false;
                int heads = ctx.FlipUntilTails();
                var defending = ctx.Defending;
                if (heads <= 0 || defending == null || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var picks = defending.Energy.Count <= heads ? defending.Energy.ToList()
                    : ctx.ChooseCards(defending.Energy.ToList(), heads, heads, "Daunting Eyes: shuffle which " + heads + " Energy into your opponent's deck?");
                ctx.Engine.ShuffleEnergyIntoDeck(defending, picks);
                return true;
            }
        }

        /// <summary>
        /// Evanescent: if this Pokémon is in the Active Spot and is Knocked Out, flip a coin; on heads the opponent takes 1 fewer
        /// Prize card. Frost Bullet: also 20 to 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class FroslassExPAR : Mon
        {
            private static readonly Mon Bullet = new Cards30CC.BenchHitMon("Frost Bullet", 20);

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var me = src.Pokemon;
                if (me == null || knockedOut != me || taker == src.Player || prizes <= 0) return prizes;
                bool byAttack = knockedOut.HitByAttack && !knockedOut.ForcedKnockOut;
                bool active = byAttack ? knockedOut.HitWhileActive : src.State.IsActive(me);
                if (!active) return prizes;
                Announce(src, "Evanescent");
                if (!src.Engine.FlipCoin(src.Player, "Evanescent")) return prizes;
                src.Log("Evanescent: " + src.Engine.PlayerName(taker) + " takes 1 fewer Prize card.");
                return prizes - 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bullet.HandleAttack(ctx, atk); }
        }

        /// <summary>Arm Thrust Needle: next turn, prevent all damage from attacks by Pokémon that have an Ability, except Simisage.</summary>
        private sealed class ArmThrustNeedleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Arm Thrust Needle") return false;
                ctx.DamageDefending(atk.Damage);
                CardsPBL.ShieldSelfFrom(ctx, atk.Name, 9999, m => m.Name != "Simisage" && m.Def.Abilities.Any(a => a.IsAbility),
                    "Pokémon that have an Ability (except Simisage)");
                return true;
            }
        }

        /// <summary>Satellite Beam: 30 for each Energy card in the opponent's discard pile. Brain Shake: Confused.</summary>
        private sealed class OrbeetlePAR : Mon
        {
            private static readonly Mon Shake = new ConditionAttackMon("Brain Shake", SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Satellite Beam") return Shake.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage * ctx.Opp.Discard.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        /// <summary>Clinging Spore: attach a Basic Grass Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class ToedscoolPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clinging Spore") return false;
                CardsSSP2.AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 1, m => !ctx.IsActive(m), "Clinging Spore");
                return true;
            }
        }

        /// <summary>
        /// Slime Mold Colony: cards in the opponent's discard pile can't be put into their hand by the effects of their Abilities
        /// or Trainer cards. Mushroom Drain: heal 30 from this Pokémon.
        /// </summary>
        private sealed class ToedscruelPAR : Mon
        {
            private static readonly Mon Drain = new CardsTWM1.HealSelfMon("Mushroom Drain", 30);

            public override bool BlocksDiscardToHand(EffectContext src, int player)
            {
                return src.Pokemon != null && player != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drain.HandleAttack(ctx, atk); }
        }

        /// <summary>Leaf Bringer: attach up to 2 Basic Grass Energy from your discard pile to 1 of your Pokémon. Binding Greed: the Defending Pokémon's attacks cost 2 Colorless more next turn.</summary>
        private sealed class WoChienPAR : Mon
        {
            private static readonly Mon Bringer = new BringerMon("Leaf Bringer", EnergyType.Grass, 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Binding Greed") return Bringer.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new AttackCostUpEffect(defending.Name + "'s attacks cost 2 more Colorless Energy during its next turn (Binding Greed)",
                        ctx.Player, defending.Uid, ctx.Turn + 1, 2));
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Scorching Heater: next turn, if this Pokémon is damaged by an attack (even if Knocked Out), put 6 damage counters on the Attacking Pokémon.</summary>
        private sealed class MagbyPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Heater") return false;
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new RetaliateEffect(me.Name + " puts 6 damage counters on an attacker that damages it during the opponent's next turn (Scorching Heater)",
                        ctx.Player, me.Uid, ctx.Turn + 1, 6));
                return true;
            }
        }

        /// <summary>Dual Turbo: choose up to 2 of your Benched Pokémon and attach a Basic Fire Energy from your discard pile to each.</summary>
        private sealed class VolcanionPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Turbo") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0 || !ctx.Me.Discard.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire))) return true;
                var picks = ctx.ChoosePokemonMany(bench, 1, Math.Min(2, bench.Count), "Dual Turbo: attach a Basic Fire Energy from your discard pile to up to 2 of your Benched Pokémon");
                foreach (var mon in picks)
                {
                    var fire = ctx.Me.Discard.FirstOrDefault(x => CardsSSP1.IsBasicOf(x.Def, EnergyType.Fire));
                    if (fire == null) break;
                    ctx.Me.Discard.Remove(fire);
                    ctx.Engine.AttachEnergyCard(fire, mon);
                }
                return true;
            }
        }

        /// <summary>
        /// Thermal Reactor: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may move any
        /// amount of Fire Energy from your other Pokémon to it. Heat Ray: can't use it during your next turn.
        /// </summary>
        private sealed class IronMothPAR : Mon
        {
            private static readonly Mon HeatRay = new LockAfterAttackMon("Heat Ray", true);

            private static List<PokemonInPlay> OtherFireHolders(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fire)));
            }

            // Your ruling (v38): only during your turn, and not when Iron Moth is promoted after a Knock Out.
            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                if (ctx.Engine.Promoting || ctx.State.InCheckup) return;
                if (!TriggerOnce(ctx, "Thermal Reactor", OtherFireHolders(ctx).Count > 0, "Thermal Reactor: move Fire Energy from your other Pokémon to Iron Moth?")) return;
                var me = ctx.Pokemon;
                bool person = RepeatableAbility.ForPerson(ctx);
                for (int step = 0; step < 60; step++)
                {
                    var holders = OtherFireHolders(ctx);
                    if (holders.Count == 0 || !ctx.InPlay(me)) break;
                    var source = person
                        ? RepeatableAbility.ChoosePokemonOrDone(ctx, holders, "Thermal Reactor: move a Fire Energy from which Pokémon to " + me.Name + "? Press Done when finished.", true)
                        : holders[0];
                    if (source == null) break;
                    var fire = source.Energy.Where(x => CardsDRI1.ProvidesType(ctx, source, x, EnergyType.Fire)).ToList();
                    if (fire.Count == 0) break;
                    var card = fire.Select(x => x.Def.Name).Distinct().Count() == 1 ? fire[0]
                        : ctx.ChooseCards(fire, 1, 1, "Thermal Reactor: move which Energy from " + source.Name + "?")[0];
                    if (!ctx.Engine.MoveEnergy(source, me, card)) break;
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HeatRay.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>
        /// Whirltide: reveal the top 6 cards of your deck; 60 damage to 1 of the opponent's Pokémon for each Energy card there;
        /// discard those Energy cards and shuffle the rest back.
        /// </summary>
        private sealed class KingdraPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whirltide") return false;
                var top = ctx.LookAtTop(6);
                if (top.Count == 0) return true;
                ctx.Engine.Reveal(ctx.Player, top, "revealed the top " + top.Count + " cards of " + ctx.Engine.Their(ctx.Player) + " deck");
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0) ctx.DamageAnyOpponent(60 * energy.Count);
                foreach (var card in energy)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Discard.Add(card);
                if (energy.Count > 0)
                    ctx.Engine.EmitCards(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded the Energy cards revealed by Whirltide.", energy);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Suction Cup Draw: when you evolve into this Pokémon from your hand, you may draw 3 cards. Smokescreen Shot: the Defending Pokémon must flip to attack next turn.</summary>
        private sealed class OctilleryPAR : Mon
        {
            private static readonly Mon Smokescreen = new CardsMEG.SandAttackMon("Smokescreen Shot");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Suction Cup Draw", ctx.Me.Deck.Count > 0, "Suction Cup Draw: draw 3 cards?")) return;
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smokescreen.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Lifeboat: when you evolve into this Pokémon from your hand, you may have each player put a Basic Pokémon from their
        /// discard pile onto their Bench, the opponent first. Hypno Splash: Asleep.
        /// </summary>
        private sealed class MiloticPAR : Mon
        {
            private static readonly Mon Splash = new ConditionAttackMon("Hypno Splash", SpecialCondition.Asleep);

            private static bool CanRevive(EffectContext ctx, int player)
            {
                return ctx.State.Players[player].Discard.Any(c => c.Def.IsBasicPokemon) && ctx.Engine.BenchSpace(player) > 0;
            }

            private static void Revive(EffectContext ctx, int player)
            {
                if (!CanRevive(ctx, player)) return;
                var pile = ctx.State.Players[player].Discard.Where(c => c.Def.IsBasicPokemon).ToList();
                var pick = ctx.Engine.ChooseCards(player, pile, 1, 1, "Lifeboat: put a Basic Pokémon from your discard pile onto your Bench", ctx.Source);
                if (pick.Count == 0 || !ctx.State.Players[player].Discard.Remove(pick[0])) return;
                ctx.Engine.PutOntoBench(player, pick[0], false);
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool any = CanRevive(ctx, ctx.Opponent) || CanRevive(ctx, ctx.Player);
                if (!TriggerOnce(ctx, "Lifeboat", any, "Lifeboat: each player puts a Basic Pokémon from their discard pile onto their Bench (your opponent first). Use it?")) return;
                Revive(ctx, ctx.Opponent);
                Revive(ctx, ctx.Player);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Splash.HandleAttack(ctx, atk); }
        }

        /// <summary>Ice Shard: 30 more if the opponent's Active Pokémon is a Fighting Pokémon.</summary>
        private sealed class SnoruntPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Shard") return false;
                bool fighting = ctx.Defending != null && ctx.Engine.IsType(ctx.Defending, EnergyType.Fighting);
                ctx.DamageDefending(atk.Damage + (fighting ? 30 : 0));
                return true;
            }
        }

        /// <summary>
        /// Hydro Lander: attach up to 3 Basic Fighting Energy from your discard pile to your Benched Pokémon in any way you like.
        /// Sonic Dive: discard 2 Energy from this Pokémon; 120 damage to 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class GarchompExPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hydro Lander")
                {
                    ctx.DamageDefending(atk.Damage);
                    var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                    if (pile.Count > 0 && ctx.Me.Bench.Count > 0)
                        ctx.AttachCardsAnyWay(pile, 3, (c, m) => !ctx.IsActive(m), "Hydro Lander", true);
                    return true;
                }
                if (atk.Name == "Sonic Dive")
                {
                    var me = ctx.Pokemon;
                    ctx.DiscardEnergyFrom(me, 2, "Sonic Dive: discard which 2 Energy from " + me.Name + "?");
                    ctx.DamageAnyOpponent(120);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Buoyant Healing: heal 120 damage from 1 of your Benched Pokémon.</summary>
        private sealed class MantykePAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Buoyant Healing") return false;
                CardsMEG.HealOneOfMine(ctx, 120, "Buoyant Healing", m => !ctx.IsActive(m));
                return true;
            }
        }

        /// <summary>Frigid Room: the opponent's Pokémon that have 40 HP or less remaining can't attack.</summary>
        private sealed class VanilluxePAR : Mon
        {
            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && src.RemainingHp(mon) <= 40;
            }
        }

        /// <summary>
        /// Icicle Sole: put damage counters on 1 of the opponent's Pokémon until its remaining HP is 30. Trop Kick: heal 30 from
        /// this Pokémon; it recovers from all Special Conditions.
        /// </summary>
        private sealed class TsareenaExPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Icicle Sole")
                {
                    var targets = ctx.OpponentPokemon(m => ctx.RemainingHp(m) > 30);
                    if (targets.Count == 0) return true;
                    var target = ctx.ChoosePokemon(targets, "Icicle Sole: put damage counters on which of your opponent's Pokémon until it has 30 HP left?");
                    if (target == null) return true;
                    int counters = (ctx.RemainingHp(target) - 30) / 10;
                    if (counters > 0) ctx.PlaceCounters(target, counters);
                    return true;
                }
                if (atk.Name == "Trop Kick")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me)) return true;
                    ctx.Heal(me, 30);
                    ctx.Engine.RemoveAllConditions(me);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sneaky Snacking: on heads, discard a random card from the opponent's hand.</summary>
        private sealed class WimpodPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sneaky Snacking") return false;
                if (ctx.Flip()) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        /// <summary>Powerful Cross: 20 for each card in the opponent's hand.</summary>
        private sealed class GolisopodPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Powerful Cross") return false;
                ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Swing and Skedaddle: discard an Energy from this Pokémon; if you do, switch it with 1 of your Benched Pokémon.</summary>
        private sealed class GolisopodExPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swing and Skedaddle") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Swing and Skedaddle: discard which Energy from " + me.Name + "?") > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Suddenly Select: when you evolve into this Pokémon from your hand, you may search for up to 3 Pokémon Tool cards. Triple Whip: 3 coins, 70 each heads.</summary>
        private sealed class WugtrioPAR : Mon
        {
            private static readonly Mon Whip = new CoinsMon("Triple Whip", 3);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Suddenly Select", ctx.Me.Deck.Count > 0, "Suddenly Select: search your deck for up to 3 Pokémon Tool cards?")) return;
                ctx.SearchToHand(d => d.IsTool, 3, "Suddenly Select: choose up to 3 Pokémon Tool cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Whip.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Fillet Memento: if this Pokémon is in the Active Spot and is Knocked Out by damage from the opponent's attack, move up
        /// to 2 Water Energy cards from it to 1 of your Benched Pokémon. Hydro Pump: 20 more for each Water Energy attached.
        /// </summary>
        private sealed class VeluzaPAR : Mon
        {
            private static readonly Mon Pump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || !wasActive || me == null) return;
                var bench = src.MyBench().Where(m => m != me).ToList();
                var water = me.Energy.Where(e => CardsDRI1.ProvidesType(src, me, e, EnergyType.Water)).ToList();
                if (bench.Count == 0 || water.Count == 0) return;
                if (!src.AskYesNo("Fillet Memento: move up to 2 Water Energy from " + me.Name + " to 1 of your Benched Pokémon?")) return;
                Announce(src, "Fillet Memento");
                var target = src.ChoosePokemon(bench, "Fillet Memento: move the Water Energy to which Benched Pokémon?");
                if (target == null) return;
                var picks = src.ChooseCards(water, 1, Math.Min(2, water.Count), "Fillet Memento: move which Energy (up to 2)?");
                foreach (var card in picks) src.Engine.MoveEnergy(me, target, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Supplemental Swallow-Up: look at the top 5 cards; attach any number of Basic Energy there to this Pokémon; shuffle the rest back.</summary>
        private sealed class DondozoPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supplemental Swallow-Up") return false;
                var me = ctx.Pokemon;
                var top = ctx.LookAtTop(5);
                var pile = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (pile.Count > 0 && ctx.InPlay(me))
                    ctx.AttachCardsAnyWay(pile, pile.Count, (card, m) => m == me, "Supplemental Swallow-Up", true);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Lightning (and the Lightning Tera Mewtwo ex)

        /// <summary>
        /// Transfer Charge: attach up to 2 Basic Psychic Energy from your discard pile to your Pokémon in any way you like.
        /// Photon Kinesis: 30 more for each Psychic Energy attached to all of your Pokémon.
        /// </summary>
        private sealed class MewtwoExPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Transfer Charge")
                {
                    var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Psychic);
                    if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, 2, (c, m) => true, "Transfer Charge", true);
                    return true;
                }
                if (atk.Name == "Photon Kinesis")
                {
                    int n = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Psychic));
                    ctx.DamageDefending(atk.Damage + 30 * n);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Buddy Pulse: if you have Plusle in play, whenever the opponent attaches an Energy card from their hand to 1 of their
        /// Pokémon, put 2 damage counters on that Pokémon. "Doesn't stack": it works once per turn however many Minun you have
        /// (Compendium, 2023-11-02).
        /// </summary>
        private sealed class MinunPAR : Mon
        {
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target == null || energy == null || target.Owner == src.Player) return;
                if (!src.Me.InPlay().Any(m => m.Name == "Plusle")) return;
                const string limit = "Buddy Pulse";
                if (src.NamedLimitUsedThisTurn(limit)) return;
                src.MarkNamedLimit(limit);
                Announce(src, "Buddy Pulse");
                src.PlaceCounters(target, 2);
            }
        }

        /// <summary>Burst of Braying: attach Basic Lightning Energy from your discard pile, up to the number of Prize cards the opponent has taken, to your Pokémon in any way you like. Thunder: 50 to itself.</summary>
        private sealed class ZebstrikaPAR : Mon
        {
            private static readonly Mon Thunder = new RecoilMon("Thunder", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burst of Braying") return Thunder.HandleAttack(ctx, atk);
                int taken = ctx.OpponentPrizesTaken;
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning);
                if (taken > 0 && pile.Count > 0) ctx.AttachCardsAnyWay(pile, taken, (c, m) => true, "Burst of Braying", true);
                return true;
            }
        }

        /// <summary>Crushing Short: discard the Defending Pokémon's Tools before damage. Raging Thunder: also 40 to 1 of your Benched Pokémon.</summary>
        private sealed class ZekromPAR : Mon
        {
            private static readonly Mon Short = new CardsBLK.PeckOffMon("Crushing Short");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Thunder") return Short.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Raging Thunder: do 40 damage to which of your Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 40);
                return true;
            }
        }

        /// <summary>Pep Up: shuffle your hand into your deck, then draw a card for each Benched Pokémon (both players'). Volt Wave: on heads, Paralyzed.</summary>
        private sealed class OricorioPAR : Mon
        {
            private static readonly Mon Wave = new ParalyzeFlipMon("Volt Wave");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pep Up") return Wave.HandleAttack(ctx, atk);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                int n = ctx.Me.Bench.Count + ctx.Opp.Bench.Count;
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Vengeful Shock: if any of your Pokémon were Knocked Out by attack damage last turn, 90 more and Paralyzed. Extreme Current: discard an Energy.</summary>
        private sealed class TapuKokoExPAR : Mon
        {
            private static readonly Mon Current = new CardsTWM1.SelfDiscardMon("Extreme Current", 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vengeful Shock") return Current.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                bool revenge = ctx.WasKnockedOutByAttackLastOpponentTurn();
                ctx.DamageDefending(atk.Damage + (revenge ? 90 : 0));
                if (revenge && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Genome Spiral: move all Energy from this Pokémon to your Benched Pokémon in any way you like.</summary>
        private sealed class DeoxysPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Genome Spiral") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                foreach (var card in me.Energy.ToList())
                {
                    var bench = ctx.MyBench();
                    if (bench.Count == 0) break;
                    var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Genome Spiral: move " + card.Def.Name + " to which Benched Pokémon?");
                    if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                }
                return true;
            }
        }

        /// <summary>
        /// Gold Coffin: if this Pokémon is Knocked Out by damage from the opponent's attack, search your deck for a card and put
        /// it into your hand. Hollow Hands: put 5 damage counters on the opponent's Benched Pokémon in any way you like.
        /// </summary>
        private sealed class CofagrigusExPAR : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || src.Me.Deck.Count == 0) return;
                Announce(src, "Gold Coffin");
                src.SearchToHand(d => true, 1, "Gold Coffin: put a card from your deck into your hand", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hollow Hands") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.PlaceCountersAnyWay(5, () => ctx.OpponentBench());
                return true;
            }
        }

        /// <summary>Startling Pumpkin: if this Pokémon is Knocked Out by damage from the opponent's attack, discard 2 random cards from their hand. Shadow Bind: can't retreat.</summary>
        private sealed class GourgeistPAR : Mon
        {
            private static readonly Mon Bind = new NoRetreatMon("Shadow Bind");

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || src.Opp.Hand.Count == 0) return;
                Announce(src, "Startling Pumpkin");
                CardsASC.DiscardRandomFromOpponentHand(src, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bind.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Stance: when you evolve into this Pokémon from your hand, you may prevent all damage from and effects of the opponent's
        /// attacks on it until the end of their next turn. It's an Ability effect, so it stays if Espathra moves to the Bench, and
        /// it stops Magby's Scorching Heater counters (Compendium, 2023-11). Glittering Eyes: 70 more if Tulip is in your discard pile.
        /// </summary>
        private sealed class EspathraPAR : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!TriggerOnce(ctx, "Stance", true, "Stance: prevent all damage from and effects of your opponent's attacks on " + me.Name + " until the end of their next turn?")) return;
                ctx.AddEffect(new StanceEffect(me.Name + " is protected from the opponent's attacks until the end of their next turn (Stance)", ctx.Player, me.Uid, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Glittering Eyes") return false;
                bool tulip = ctx.Me.Discard.Any(c => c.Def.Name == "Tulip");
                ctx.DamageDefending(atk.Damage + (tulip ? 70 : 0));
                return true;
            }
        }

        /// <summary>Mountain Scrounging: look at the top card; put it into your hand, or discard it and draw a card.</summary>
        private sealed class TinkatinkPAR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Scrounging") return false;
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return true;
                if (ctx.AskYesNo("Mountain Scrounging: the top card of your deck is " + top[0].Def.Name + ". Put it into your hand? (If not, it's discarded and you draw a card.)"))
                {
                    ctx.DeckToHand(top);
                    return true;
                }
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                ctx.Draw(1);
                return true;
            }
        }
    }
}
