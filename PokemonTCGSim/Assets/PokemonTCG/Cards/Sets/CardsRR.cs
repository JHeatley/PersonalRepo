using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Rising Rivals (RR, 2009; v173): the cards not already in the pool (the others are prints). Unlimited only. Rares, LV.X and
    /// the Rotom cards here; Uncommons, Commons, Trainers and Energy in CardsRRB.cs.
    /// </summary>
    public static partial class CardsRR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RR-1", new ArcanineFlareCondition());
            r.RegisterByKey("RR-2", new ComboMon(new CardsDRI1.WhirlwindMon("Bounce Back"), new CardsTWM1.HealSelfMon("Smack Attack", 10)));
            r.RegisterByKey("RR-3", new DarkraiEerieAura());
            r.RegisterByKey("RR-4", new Fx()
                .Effect("Incite", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsSupporter).ToList();
                    if (pile.Count > 0) c.DiscardToHand(c.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Incite: put up to 2 Supporter cards into your hand"));
                })
                .With(new LockAfterAttackMon("Giant Wave", true)));
            r.RegisterByKey("RR-5", new FlygonRainbowFloat());
            r.RegisterByKey("RR-6", new Fx()
                .Effect("Sleep Inducer", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var pulled = c.GustOpponent("Sleep Inducer: switch in which of your opponent's Benched Pokémon?");
                    if (pulled != null && c.InPlay(pulled) && c.CanAffect(pulled)) c.ApplyCondition(pulled, SpecialCondition.Asleep);
                })
                .On("Wake-Up Slap", (c, a) =>
                {
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (Affected(d) ? 20 : 0));
                    if (d != null && c.InPlay(d)) c.Engine.RemoveAllConditions(d);
                    return true;
                }));
            r.RegisterByKey("RR-7", new JirachiFinalWish());
            r.RegisterByKey("RR-8", new LucarioBoundaryAura());
            r.RegisterByKey("RR-9", new Fx().On("Trash Bolt", (c, a) => DiscardEnergyFromHandThen(c, a, x => x.Def.IsEnergy)));
            r.RegisterByKey("RR-10", new Fx()
                .After("Psychic Removal", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.FlipCoins(2) < 2 || d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    foreach (var e in d.Energy.ToList()) c.Engine.DiscardEnergy(d, e);
                })
                .Damage("Grudge", (c, a) => a.Damage + 10 * c.OpponentPrizesTaken));
            r.RegisterByKey("RR-11", new Fx()
                .Effect("Trample", (c, a) =>
                {
                    foreach (var m in c.MyBench().Concat(c.OpponentBench()).ToList())
                        if (c.Flip() && c.InPlay(m)) c.DamageTarget(m, 30);
                })
                .Damage("Rend", (c, a) => a.Damage + (c.Defending != null && c.Defending.DamageCounters > 0 ? 20 : 0)));
            r.RegisterByKey("RR-12", new Fx()
                .After("Poison Bind", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                    c.DefendingCantRetreat(d);
                })
                .On("Long Whip", (c, a) => LongWhip(c, a, 30)));
            r.RegisterByKey("RR-13", new ShiftryUnluckyWind());

            r.RegisterByKey("RR-103", new RepeatPowerFx("Damage Switch", c => c.MyPokemon(m => SP(m) && m.DamageCounters > 0).Count > 0 && c.MyPokemon(SP).Count > 1, (c, done) =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(c, c.MyPokemon(m => SP(m) && m.DamageCounters > 0), "Damage Switch: move 1 damage counter from which Pokémon SP?" + (done ? " Press Done when finished." : ""), done);
                    if (from == null) return false;
                    var to = c.ChoosePokemon(c.MyPokemon(m => SP(m) && m != from), "Damage Switch: move it to which Pokémon SP?");
                    if (to == null) return false;
                    return c.Engine.MoveDamageCounters(from, to, 1, "Damage Switch") > 0;
                }, new Fx().Effect("Mind Shock", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, false); })));
            r.RegisterByKey("RR-104", new FloatzelWaterRescue());
            r.RegisterByKey("RR-105", new FlygonWindErosion());
            r.RegisterByKey("RR-106", new CardsDS.EvolutionaryMon("Blade Storm", c =>
                {
                    foreach (var m in c.OpponentPokemon().ToList()) if (c.CanAffect(m)) c.PlaceCounters(m, 1);
                }, c => c.PokemonIsActive && c.OpponentPokemon().Count > 0, new Fx()
                .Damage("Aimed Cut", (c, a) => a.Damage + 10 * (c.Defending == null ? 0 : c.Defending.DamageCounters))));
            r.RegisterByKey("RR-107", new PowerFx("Sand Reset", c => !c.Me.NamedLimitUsedOnTurn.ContainsKey(SandResetGame), SandReset, new Fx()
                .Effect("Double Shoot", (c, a) =>
                {
                    DiscardTypeFromSelf(c, EnergyType.Fighting, 2);
                    HitBenched(c, 2, 40, "Double Shoot");
                }), true));
            r.RegisterByKey("RR-108", new PowerFx("Intimidating Roar", c => c.Opp.Bench.Count > 0, c => c.OpponentSwitchesActive(),
                new CardsTWM1.SelfDiscardMon("Fire Spin", 2)));
            r.RegisterByKey("RR-110", new RepeatPowerFx("Magical Return", c => c.MyPokemon(m => m.Tools.Count > 0).Count > 0, (c, done) =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(c, c.MyPokemon(m => m.Tools.Count > 0), "Magical Return: return a Pokémon Tool or Technical Machine from which Pokémon?" + (done ? " Press Done when finished." : ""), done);
                    if (from == null) return false;
                    var tool = from.Tools.Count == 1 ? from.Tools[0] : c.ChooseCards(from.Tools.ToList(), 1, 1, "Magical Return: return which card?")[0];
                    from.Tools.Remove(tool);
                    c.Me.Hand.Add(tool);
                    c.Log(tool.Def.Name + " went back to " + c.Engine.PlayerName(c.Player) + "'s hand (Magical Return).");
                    return true;
                }, new Fx().Effect("Darkness Magic", (c, a) =>
                {
                    var d = c.Defending;
                    int n = Math.Min(8, c.Me.Hand.Count);
                    if (d != null && n > 0 && c.CanAffect(d)) c.PlaceCounters(d, n);
                })));
            r.RegisterByKey("RR-111", new PowerFx("Big Appetite", c => c.PokemonIsActive && c.Me.Hand.Count < 6 && c.Me.Deck.Count > 0, c =>
                {
                    c.DrawUntil(6);
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                }, new Fx().After("Exercise", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                    if (energy.Count == 0 || !c.InPlay(c.Pokemon)) return;
                    var picks = c.ChooseCards(energy, 0, energy.Count, "Exercise: discard as many Energy cards from your hand as you like (remove that many damage counters)");
                    foreach (var x in picks) c.Engine.DiscardFromHand(c.Player, x);
                    if (picks.Count > 0) c.Heal(c.Pokemon, 10 * picks.Count);
                })));

            r.RegisterByKey("RR-RT1", Shift("Fan Shift", EnergyType.Colorless, new Fx()
                .Effect("Spin Storm", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.Flip() && c.CanAffect(d)) c.Engine.ReturnPokemonToHand(d);
                })
                .With(new CardsFLF1.FlipTailsSelfDiscardMon("Air Slash"))));
            r.RegisterByKey("RR-RT2", Shift("Frost Shift", EnergyType.Water, new Fx()
                .Effect("Hail", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10); })
                .Damage("Crushing Ice", (c, a) => a.Damage + 10 * (c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending)))));
            r.RegisterByKey("RR-RT3", Shift("Heat Shift", EnergyType.Fire, new Fx()
                .Effect("Warm Up", (c, a) =>
                {
                    if (c.MyBench().Count == 0) return;
                    SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Fire), "Warm Up", 1, m => m != c.Me.Active);
                })
                .With(new ConditionAttackMon("Heat Burn", SpecialCondition.Burned))));
            r.RegisterByKey("RR-RT4", Shift("Mow Shift", EnergyType.Grass, new Fx()
                .After("Mow Down", (c, a) =>
                {
                    if (!c.Flip()) return;
                    foreach (var m in c.OpponentPokemon(m => m.Energy.Count > 0).ToList())
                        if (c.CanAffect(m)) c.DiscardEnergyFrom(m, 1, "Mow Down: discard an Energy card attached to " + m.Name);
                })));
            r.RegisterByKey("RR-RT5", Shift("Wash Shift", EnergyType.Water, new Fx()
                .Effect("Cleanse Away", (c, a) => HealEach(c, 30, true))
                .After("Drain Wash", (c, a) =>
                {
                    int heads = c.FlipUntilTails();
                    for (int i = 0; i < heads && c.Opp.Hand.Count > 0; i++) c.Engine.DiscardFromHand(c.Opponent, RandomFromHand(c, c.Opp));
                })));
            r.RegisterByKey("RR-RT6", new CharonsChoice());
            RegisterCommons(r);
        }

        /// <summary>Discard a matching card from your hand, then do the printed damage (nothing if you can't).</summary>
        internal static bool DiscardEnergyFromHandThen(EffectContext c, AttackDef a, Func<CardInstance, bool> filter)
        {
            var pool = c.Me.Hand.Where(filter).ToList();
            if (pool.Count == 0) return DidNothing(c);
            c.Engine.DiscardFromHand(c.Player, c.ChooseCards(pool, 1, 1, a.Name + ": discard which card from your hand?")[0]);
            c.DamageDefending(a.Damage);
            return true;
        }

        /// <summary>"If the Defending Pokémon is affected by a Special Condition, you may do N damage to any 1 Benched Pokémon instead."</summary>
        internal static bool LongWhip(EffectContext c, AttackDef a, int amount)
        {
            if (Affected(c.Defending) && c.OpponentBench().Count > 0 && c.AskYesNo(a.Name + ": do " + amount + " damage to 1 of your opponent's Benched Pokémon instead?"))
            {
                var t = c.ChoosePokemon(c.OpponentBench(), a.Name + ": choose 1 of your opponent's Benched Pokémon");
                if (t != null) c.DamageTarget(t, amount);
                return true;
            }
            c.DamageDefending(a.Damage);
            return true;
        }

        /// <summary>Flare Condition (Poké-Body): with any Fire Energy attached, no Weakness. Burn Out. Flames of Rage.</summary>
        private sealed class ArcanineFlareCondition : BodyMon
        {
            public ArcanineFlareCondition() : base(new Fx()
                .On("Burn Out", (c, a) =>
                {
                    bool more = c.AskYesNo("Burn Out: do 30 more damage (this Pokémon is then Burned)?");
                    c.DamageDefending(a.Damage + (more ? 30 : 0));
                    if (more && c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Burned);
                    return true;
                })
                .On("Flames of Rage", (c, a) =>
                {
                    DiscardTypeFromSelf(c, EnergyType.Fire, 1);
                    c.DamageDefending(a.Damage + 10 * c.Pokemon.DamageCounters);
                    return true;
                })) { }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon == src.Pokemon && mon.Energy.Any(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Fire)) && Works(src, "Flare Condition");
            }
        }

        /// <summary>Eerie Aura (Poké-Body): 1 damage counter on each of the opponent's Pokémon that remains Asleep between turns. Darkness Sleep.</summary>
        private sealed class DarkraiEerieAura : BodyMon
        {
            public DarkraiEerieAura() : base(new Fx()
                .On("Darkness Sleep", (c, a) =>
                {
                    var d = c.Defending;
                    bool already = d != null && d.DamageCounters > 0;
                    c.DamageDefending(a.Damage);
                    if (already && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Asleep);
                    return true;
                })) { }

            public override int CheckupAffects(EffectContext src) { return src.Opponent; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (!Works(src, "Eerie Aura")) return;
                foreach (var m in src.OpponentPokemon(x => x.Has(SpecialCondition.Asleep)).ToList()) src.Engine.PlaceDamageCounters(m, 1, "Eerie Aura");
            }
        }

        /// <summary>
        /// Rainbow Float (Poké-Body): your Pokémon that share a type with a basic Energy card attached to Flygon have no Retreat
        /// Cost. Sand Wall. Power Swing.
        /// </summary>
        private sealed class FlygonRainbowFloat : BodyMon
        {
            public FlygonRainbowFloat() : base(new Fx()
                .After("Sand Wall", (c, a) =>
                {
                    if (c.State.Stadium == null || c.State.StadiumOwner != c.Opponent) return;
                    c.Engine.DiscardStadium();
                    if (c.InPlay(c.Pokemon)) c.ProtectSelfNextTurn();
                })
                .Damage("Power Swing", (c, a) => a.Damage + 10 * c.MyBench().Count(CardsE2.IsEvolved))) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon.Owner != src.Player || src.Pokemon == null || !Works(src, "Rainbow Float")) return cost;
                var types = src.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).ToList();
                if (types.Count == 0) return cost;
                return src.Engine.TypesOf(mon).Any(types.Contains) ? 0 : cost;
            }
        }

        /// <summary>Final Wish (Poké-Power): when Knocked Out by damage from an opponent's attack, search your deck for any 1 card. Detour. Swift.</summary>
        private sealed class JirachiFinalWish : BodyMon
        {
            public JirachiFinalWish() : base(new Fx()
                .With(new CardsTRR.DetourMon())
                .Effect("Swift", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, true); })) { }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.IsMyTurn || src.Me.Deck.Count == 0 || !CardsE1.NoCondition(src.Pokemon)) return;
                if (!src.AskYesNo("Final Wish: search your deck for any 1 card?")) return;
                Announce(src, "Final Wish");
                src.SearchToHand(d => true, 1, "Final Wish: search your deck for any card");
            }
        }

        /// <summary>Boundary Aura (Poké-Body): Weakness is ×2 for every Pokémon. Combo Throw.</summary>
        private sealed class LucarioBoundaryAura : BodyMon
        {
            public LucarioBoundaryAura() : base(new Fx()
                .Damage("Combo Throw", (c, a) => a.Damage + 10 * (c.Defending == null ? 0 : c.EnergyCount(c.Defending)))) { }

            public override bool AppliesWeaknessAsDouble(EffectContext src) { return Works(src, "Boundary Aura"); }
        }

        /// <summary>Unlucky Wind (Poké-Body): while Shiftry is your Active Pokémon, the opponent's coin flips during their turn are tails. Conform. Seal Off.</summary>
        private sealed class ShiftryUnluckyWind : BodyMon
        {
            public ShiftryUnluckyWind() : base(new Fx()
                .After("Conform", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.Me.Hand.Count == c.Opp.Hand.Count && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Confused);
                })
                .After("Seal Off", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new SealOffEffect(d.Name + " can't use Poké-Powers or Poké-Bodies (Seal Off)", c.Player, d.Uid, c.Turn + 1));
                })) { }

            public override bool ForcesTailsFor(EffectContext src, int player)
            {
                // Not the Sleep flips between turns (Rising Rivals FAQ, 2009-05-21).
                return player != src.Player && src.State.CurrentPlayer == player && !src.State.InCheckup && src.PokemonIsActive && Works(src, "Unlucky Wind");
            }
        }

        /// <summary>
        /// Water Rescue (Poké-Body): when your other Water Pokémon (not Floatzel GL) is Knocked Out by damage from an opponent's
        /// attack, you may put it and its cards into your hand. Energy Cyclone.
        /// </summary>
        private sealed class FloatzelWaterRescue : BodyMon
        {
            public FloatzelWaterRescue() : base(new Fx()
                .Effect("Energy Cyclone", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                    var picks = energy.Count == 0 ? new List<CardInstance>() : c.ChooseCards(energy, 0, energy.Count, "Energy Cyclone: show as many Energy cards from your hand as you like (20 damage each)");
                    if (picks.Count > 0) c.Engine.ShowCards(c.Opponent, picks, "Energy Cyclone", "Energy Cyclone: the Energy cards your opponent chose", c.Source);
                    c.DamageDefending(20 * picks.Count);
                    foreach (var x in picks) if (c.Me.Hand.Remove(x)) c.Me.Deck.Insert(0, x);
                    c.ShuffleMyDeck();
                })) { }

            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                if (!byOpponentsAttack || mon == null || mon == src.Pokemon || mon.Name == "Floatzel GL" || mon.Name.StartsWith("Floatzel GL ")) return;
                if (!src.Engine.IsType(mon, EnergyType.Water) || !Works(src, "Water Rescue")) return;
                if (!src.AskYesNo("Water Rescue: put " + mon.Name + " and all cards attached to it into your hand?")) return;
                mon.Markers[GameEngine.AllToHandOnKnockOut] = 1;
            }
        }

        /// <summary>Wind Erosion (Poké-Body): while Flygon is your Active Pokémon, discard the top card of the opponent's deck between turns. Extreme Attack.</summary>
        private sealed class FlygonWindErosion : BodyMon
        {
            public FlygonWindErosion() : base(new Fx()
                .Effect("Extreme Attack", (c, a) =>
                {
                    var pool = c.OpponentPokemon(m => m.Def.IsLevelUp);
                    if (pool.Count == 0) return;
                    var t = c.ChoosePokemon(pool, "Extreme Attack: choose 1 of your opponent's Pokémon LV.X");
                    if (t != null) c.DamageTarget(t, 150);
                })) { }

            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? src.Opponent : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (!src.PokemonIsActive || src.Opp.Deck.Count == 0 || !Works(src, "Wind Erosion")) return;
                Announce(src, "Wind Erosion");
                src.Engine.DiscardTopOfDeck(src.Opponent, 1);
            }
        }

        private const string SandResetGame = "Sand Reset (once a game)";

        /// <summary>Sand Reset: once a game, each player shuffles every card in play except Pokémon and Supporters into their deck.</summary>
        private static void SandReset(EffectContext ctx)
        {
            ctx.Me.NamedLimitUsedOnTurn[SandResetGame] = ctx.Turn;
            foreach (int p in new[] { ctx.Player, ctx.Opponent })
            {
                var ps = ctx.State.Players[p];
                foreach (var m in ps.InPlay().ToList())
                {
                    foreach (var e in m.Energy.ToList()) { m.Energy.Remove(e); ps.Deck.Add(e); }
                    foreach (var t in m.Tools.ToList()) { m.Tools.Remove(t); ps.Deck.Add(t); }
                }
            }
            if (ctx.State.Stadium != null)
            {
                var st = ctx.State.Stadium;
                int owner = ctx.State.StadiumOwner;
                ctx.State.Stadium = null;
                ctx.State.Players[owner].Deck.Add(st);
            }
            ctx.Engine.ShuffleDeck(ctx.Player);
            ctx.Engine.ShuffleDeck(ctx.Opponent);
            ctx.Log("Sand Reset: each player shuffled the Energy, Pokémon Tools and Stadium cards in play into their deck.");
        }

        /// <summary>
        /// Charon's Choice (Supporter): switch a Rotom in play with any Rotom from your deck (cards, damage counters stay; Special
        /// Conditions and effects go); the old Rotom is shuffled into the deck. The card returns to your hand (approximation: at
        /// once, rather than at the end of the turn).
        /// </summary>
        private sealed class CharonsChoice : CardScript
        {
            private static bool Rotom(CardDef d) { return d.IsPokemon && d.Name.Contains("Rotom"); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => Rotom(m.Def) && m.Stack.Count == 1).Count > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(m => Rotom(m.Def) && m.Stack.Count == 1), "Charon's Choice: switch which Rotom?");
                var picks = ctx.SearchMyDeck(Rotom, 1, "Charon's Choice: search your deck for any Rotom");
                if (target != null && picks.Count > 0)
                {
                    var old = target.Stack[0];
                    target.Stack[0] = picks[0];
                    ctx.Engine.RemoveAllConditions(target);
                    ctx.Me.Deck.Insert(0, old);
                    ctx.Log("Charon's Choice: " + old.Def.Name + " was switched with " + picks[0].Def.Name + ".");
                }
                else foreach (var p in picks) ctx.Me.Deck.Add(p);
                ctx.ShuffleMyDeck();
                ctx.CardPlacedElsewhere = true;
                ctx.Me.Hand.Add(ctx.Card);
            }
        }
    }
}
