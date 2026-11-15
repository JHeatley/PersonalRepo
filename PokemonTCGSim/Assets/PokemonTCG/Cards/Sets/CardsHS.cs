using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// HeartGold &amp; SoulSilver (HS, 2010; v173): the cards not already in the pool (the others are prints). Unlimited only.
    /// LEGEND cards register both halves with the same script.
    /// </summary>
    public static class CardsHS
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HS-2", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("HS-3", new Fx().Effect("Fairy Power", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.MyPokemon(), "Fairy Power: return which of your Pokémon to your hand?");
                    if (t != null) c.Engine.ReturnPokemonToHand(t);
                }));
            r.RegisterByKey("HS-4", new Fx().After("Hyper Beam", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.DiscardEnergyFrom(d, 1, "Hyper Beam: discard an Energy card attached to " + d.Name);
                }));
            r.RegisterByKey("HS-5", new Fx()
                .With(new CoinsMon("Triple Kick", 3))
                .After("Close Combat", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new TakenChangeEffect(c.Pokemon.Name + " takes 20 more damage next turn (Close Combat)", c.Player, c.Pokemon.Uid, c.Turn + 1, 20));
                }));
            r.RegisterByKey("HS-6", new Fx()
                .Damage("Mass Attack", (c, a) => 10 * (c.MyPokemon().Count + c.OpponentPokemon().Count))
                .With(new ShieldAttackMon("Leaf Guard", 30)));
            r.RegisterByKey("HS-7", new PowerFx("Roast Reveal", c => c.Me.Hand.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)), c =>
                {
                    var fire = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)).ToList();
                    c.Engine.DiscardFromHand(c.Player, c.ChooseCards(fire, 1, 1, "Roast Reveal: discard which Fire Energy card?")[0]);
                    c.Draw(3);
                }, null));
            r.RegisterByKey("HS-8", new PowerFx("Night Sight", c => c.Me.Deck.Count > 0, c => c.Draw(1), new Fx()
                .Damage("Extrasensory", (c, a) => a.Damage + (c.Me.Hand.Count == c.Opp.Hand.Count ? 40 : 0))));
            r.RegisterByKey("HS-9", new Fx().After("Muddy Water", (c, a) => HitBenched(c, 1, 20, "Muddy Water")));
            r.RegisterByKey("HS-10", new Fx()
                .Damage("Iron Tail", (c, a) => 30 * c.FlipUntilTails())
                .After("Thunderbolt", (c, a) => { if (c.InPlay(c.Pokemon)) foreach (var e in c.Pokemon.Energy.ToList()) c.Engine.DiscardEnergy(c.Pokemon, e); }));
            r.RegisterByKey("HS-11", new BenchGuardMon("Shell Barricade", new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("HS-12", new PowerFx("Second Sight", c => c.Me.Deck.Count + c.Opp.Deck.Count > 0, c => ReorderTop(c, 3, "Second Sight", true),
                new ParalyzeFlipMon("Psyshock")));
            r.RegisterByKey("HS-13", new Fx().Damage("Double Return", (c, a) => c.Flip() ? 20 * c.Pokemon.DamageCounters : 0));
            r.RegisterByKey("HS-14", new Fx()
                .After("Acceleration Bolt", (c, a) =>
                {
                    var picks = c.SearchMyDeck(d => d.IsBasicEnergy, 2, "Acceleration Bolt: search your deck for up to 2 basic Energy cards");
                    if (picks.Count > 0)
                    {
                        var t = c.ChoosePokemon(c.MyPokemon(), "Acceleration Bolt: attach them to which Pokémon?");
                        foreach (var e in picks) if (t == null || !c.Engine.AttachEnergyCard(e, t, false)) c.Me.Deck.Add(e);
                    }
                    c.ShuffleMyDeck();
                })
                .With(new CardsFST1.FlipRecoilMon("Thunder", 20)));
            r.RegisterByKey("HS-15", new Fx()
                .Effect("Leech Life", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    if (done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
                })
                .With(new ConditionAttackMon("Poisonous Saliva", SpecialCondition.Poisoned)));
            r.RegisterByKey("HS-16", new Fx()
                .Effect("Cure Powder", (c, a) => HealEach(c, 30))
                .With(new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("HS-18", new Fx()
                .Effect("Energy Absorption", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsEnergy).ToList();
                    if (pile.Count == 0) return;
                    foreach (var card in c.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Energy Absorption: attach up to 2 Energy cards to " + c.Pokemon.Name))
                        if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .Damage("Big Eggsplosion", (c, a) => 40 * c.FlipCoins(c.EnergyCount(c.Pokemon))));
            r.RegisterByKey("HS-19", new ComboMon(new DrawAttackMon("Collect", 2), new SwitchSelfMon("Spin Turn", false)));
            r.RegisterByKey("HS-20", new Fx().Effect("Spinning Tail", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 20); }));
            r.RegisterByKey("HS-21", new ComboMon(new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1), new FlipBonusMon("Quick Blow", 20)));
            r.RegisterByKey("HS-22", new Fx()
                .After("Timid Tackle", (c, a) =>
                {
                    if (!c.InPlay(c.Pokemon)) return;
                    c.DamageSelf(20);
                    if (c.InPlay(c.Pokemon) && c.MyBench().Count > 0) c.SwitchSelfWithBench();
                })
                .Damage("Chomp", (c, a) => a.Damage + 10 * c.Pokemon.DamageCounters));
            r.RegisterByKey("HS-23", new PowerFx("Sleep Pendulum", c => c.Opp.Active != null, c =>
                {
                    var t = c.Opp.Active;
                    if (c.Flip() && t != null && c.CanAffect(t)) c.ApplyCondition(t, SpecialCondition.Asleep);
                }, new Fx().After("Psychic Shot", (c, a) => HitBenched(c, 1, 10, "Psychic Shot"))));
            r.RegisterByKey("HS-24", new Fx()
                .With(new ParalyzeFlipMon("Ice Beam"))
                .Effect("Ice Blade", (c, a) => HitAny(c, 30, "Ice Blade")));
            r.RegisterByKey("HS-25", new Fx()
                .After("Quick Draw", (c, a) => c.Draw(3))
                .Effect("Swift", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, true); }));
            r.RegisterByKey("HS-26", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new CardsTWM1.HealSelfMon("Giant Bloom", 20)));
            r.RegisterByKey("HS-27", new Fx()
                .Effect("Sharpen Claws", (c, a) =>
                {
                    int heads = c.FlipCoins(3);
                    for (int i = 0; i < heads && c.Opp.Hand.Count > 0; i++) c.Engine.DiscardFromHand(c.Opponent, RandomFromHand(c, c.Opp));
                })
                .Damage("Sneaky Attack", (c, a) => a.Damage + (c.Pokemon.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Darkness)) ? 30 : 0)));
            r.RegisterByKey("HS-28", new SweetSleepingFaceMon(new Fx().Effect("Playground", (c, a) =>
                {
                    foreach (int p in new[] { c.Player, c.Opponent })
                    {
                        var ps = c.State.Players[p];
                        int space = 5 - ps.Bench.Count;
                        var basics = ps.Deck.Where(x => x.Def.IsBasicPokemon).ToList();
                        if (space > 0 && basics.Count > 0)
                        {
                            foreach (var card in c.Engine.ChooseCards(p, basics, 0, Math.Min(space, basics.Count), "Playground: put any Basic Pokémon from your deck onto your Bench", c.Source))
                                if (ps.Deck.Remove(card) && c.Engine.PutOntoBench(p, card, false) == null) ps.Deck.Add(card);
                        }
                        c.Engine.ShuffleDeck(p);
                    }
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("HS-29", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("HS-31", new PowerFx("Sunshine Grace", c => c.Me.Deck.Count > 0, c =>
                c.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Grass), 1, "Sunshine Grace: search your deck for a Grass Pokémon"), null));
            r.RegisterByKey("HS-32", new CardsTWM1.SelfDiscardMon("Fire Spin", 2));
            r.RegisterByKey("HS-33", new SweetSleepingFaceMon(new Fx().Effect("Mischievous Punch", (c, a) =>
                {
                    if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, false);
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("HS-34", new Fx()
                .With(new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned))
                .After("Super Explosion", (c, a) => { if (c.InPlay(c.Pokemon)) c.DamageSelf(90); }));
            r.RegisterByKey("HS-36", new Fx()
                .After("Energy Link", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsEnergy).ToList();
                    if (pile.Count == 0 || !c.InPlay(c.Pokemon)) return;
                    var card = c.ChooseCards(pile, 1, 1, "Energy Link: attach which Energy card?")[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .With(new RecoilMon("Double-edge", 40)));
            r.RegisterByKey("HS-37", new Fx()
                .Effect("Recover", (c, a) =>
                {
                    if (DiscardTypeFromSelf(c, EnergyType.Water, 1) > 0 && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, c.Pokemon.DamageCounters * 10);
                })
                .Damage("Hyper Cannon", (c, a) => a.Damage + (c.FlipCoins(2) == 2 ? 50 : 0)));
            r.RegisterByKey("HS-38", new Fx().After("Big Bite", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("HS-39", new Fx()
                .Effect("Snowy Present", (c, a) =>
                {
                    int n = c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Water));
                    if (n > 0) c.Draw(n);
                })
                .Effect("Hail", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10); }));
            r.RegisterByKey("HS-40", new Fx()
                .Effect("Rock Hurl", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); })
                .With(new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("HS-41", new Fx()
                .With(new ParalyzeFlipMon("Glare"))
                .After("Fade Out", (c, a) => { if (c.InPlay(c.Pokemon)) c.Engine.ReturnPokemonToHand(c.Pokemon); }));
            r.RegisterByKey("HS-42", new Fx()
                .Effect("Thunder Spear", (c, a) => HitAny(c, 20, "Thunder Spear"))
                .With(new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("HS-43", new Fx()
                .Effect("Green Draw", (c, a) =>
                {
                    int n = c.MyPokemon(m => c.Engine.IsType(m, EnergyType.Grass)).Count;
                    if (n > 0) c.Draw(n);
                })
                .Damage("Double Headbutt", (c, a) => a.Damage + 20 * c.FlipCoins(2)));
            r.RegisterByKey("HS-44", new SweetSleepingFaceMon(new Fx().Effect("Graffiti", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.AddEffect(new CostUpEffect(d.Name + "'s attacks cost [C] more (Graffiti)", c.Player, d.Uid, c.Turn + 1));
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("HS-45", new Fx()
                .Effect("Group Swim", (c, a) => c.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Water), 1, "Group Swim: search your deck for a Water Pokémon"))
                .With(new LockAfterAttackMon("Aqua Slash", false)));
            r.RegisterByKey("HS-46", new MetapodGreenShield());
            r.RegisterByKey("HS-47", new ComboMon(new SearchAttackMon("Moomoo Squeeze", d => d.Name == "Moomoo Milk", 1), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("HS-48", new Fx().Effect("Sleep Drain", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Asleep);
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                }));
            r.RegisterByKey("HS-49", new Fx().After("Flamethrower", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.DiscardEnergyFrom(c.Pokemon, 1, "Flamethrower: discard an Energy attached to " + c.Pokemon.Name);
                }));
            r.RegisterByKey("HS-50", new CardsSI.HeadsTailsConditionMon("Offensive Needle", SpecialCondition.Poisoned, SpecialCondition.Paralyzed));
            r.RegisterByKey("HS-51", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("HS-52", new Fx()
                .After("Big Yawn", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Asleep);
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })
                .Damage("Madkinesis", (c, a) => a.Damage + 20 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Psychic)));
            r.RegisterByKey("HS-53", new Fx().Effect("Cosmic Cyclone", (c, a) =>
                {
                    var holders = new List<KeyValuePair<PokemonInPlay, CardInstance>>();
                    foreach (var m in c.MyPokemon()) foreach (var e in m.Energy) if (CardsDRI1.ProvidesType(c, m, e, EnergyType.Water)) holders.Add(new KeyValuePair<PokemonInPlay, CardInstance>(m, e));
                    var cards = holders.Select(p => p.Value).ToList();
                    var picks = cards.Count == 0 ? new List<CardInstance>() : c.ChooseCards(cards, 0, cards.Count, "Cosmic Cyclone: choose Water Energy attached to your Pokémon (20 damage each)");
                    c.DamageDefending(20 * picks.Count);
                    foreach (var e in picks)
                    {
                        var holder = holders.First(p => p.Value == e).Key;
                        if (holder.Energy.Remove(e)) c.Me.Deck.Add(e);
                    }
                    if (picks.Count > 0) c.ShuffleMyDeck();
                }));
            r.RegisterByKey("HS-54", new BenchTriggerFx("RETURN", c => c.MyPokemon(m => m.Energy.Count > 0).Count > 0, c =>
                {
                    var t = c.ChoosePokemon(c.MyPokemon(m => m.Energy.Count > 0), "RETURN: return all Energy attached to which of your Pokémon to your hand?");
                    if (t != null) foreach (var e in t.Energy.ToList()) c.Engine.ReturnEnergyToHand(t, e);
                }, null));
            r.RegisterByKey("HS-55", new BenchTriggerFx("FLASH", c => c.Me.Deck.Count > 0, c => ReorderTop(c, 5, "FLASH", false), null));
            r.RegisterByKey("HS-56", new ComboMon(new CoinsMon("Double Slap", 2), new ShieldAttackMon("Expand", 10)));
            r.RegisterByKey("HS-58", new Fx().After("Happy Punch", (c, a) => { if (c.InPlay(c.Pokemon) && c.Flip() && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 30); }));
            r.RegisterByKey("HS-60", new ShieldAttackMon("Minimize", 20));
            r.RegisterByKey("HS-62", new Fx().Effect("Sleep Inducer", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var pulled = c.GustOpponent("Sleep Inducer: switch in which of your opponent's Benched Pokémon?");
                    if (pulled != null && c.InPlay(pulled) && c.CanAffect(pulled)) c.ApplyCondition(pulled, SpecialCondition.Asleep);
                }));
            r.RegisterByKey("HS-63", new Fx().Damage("Continuous Eggsplosion", (c, a) => 10 * c.FlipUntilTails()));
            r.RegisterByKey("HS-64", new SearchAttackMon("Show Off", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("HS-66", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("HS-67", new SwitchSelfMon("Bounce", true));
            r.RegisterByKey("HS-68", new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep));
            r.RegisterByKey("HS-69", new Fx()
                .Effect("Mimic", (c, a) => ShuffleHandDraw(c, c.Player, c.Opp.Hand.Count))
                .With(new ParalyzeFlipMon("Lick")));
            r.RegisterByKey("HS-70", new Fx().After("Smokescreen", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Smokescreen)", c.Player, d.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("HS-71", new CoinsMon("Comet Punch", 4));
            r.RegisterByKey("HS-73", new Fx().Effect("Static Electricity", (c, a) =>
                {
                    int n = c.MyPokemon(m => m.Name == "Mareep").Count + c.OpponentPokemon(m => m.Name == "Mareep").Count;
                    SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Lightning), "Static Electricity", n, m => m == c.Pokemon);
                }));
            r.RegisterByKey("HS-74", new FlipBonusMon("Water Splash", 10));
            r.RegisterByKey("HS-75", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("HS-76", new RecoilMon("Double-edge Claw", 10));
            r.RegisterByKey("HS-77", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("HS-78", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("HS-79", new Fx().Effect("Defense Curl", (c, a) =>
                {
                    if (c.Flip()) c.AddEffect(new NoDamageEffect(c.Pokemon.Name + " is protected from attack damage (Defense Curl)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("HS-80", new Fx().Effect("Scout", (c, a) => LookAtHand(c, c.Opp)));
            r.RegisterByKey("HS-81", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("HS-82", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("HS-83", new Fx().Effect("Spider Web", (c, a) => { if (c.Defending != null) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("HS-85", new Fx().Effect("Cure Kernels", (c, a) =>
                {
                    var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                    var t = hurt.Count == 0 ? null : c.ChoosePokemon(hurt, "Cure Kernels: remove 2 damage counters from which Pokémon?");
                    if (t != null) c.Heal(t, 20);
                }));
            r.RegisterByKey("HS-87", new Fx()
                .With(new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned))
                .After("Ember", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); }));
            r.RegisterByKey("HS-88", new Fx().Effect("Tail Whip", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.Flip() && c.CanAffect(d)) c.AddEffect(new CantAttackEffect(d.Name + " can't attack (Tail Whip)", c.Player, d.Uid, c.Turn + 1));
                }));

            r.RegisterByKey("HS-97", new PokemonCollector());
            r.RegisterByKey("HS-101", new ProfessorOaksNewTheory());

            r.RegisterByKey("HS-105", new AmpharosConductivity());
            r.RegisterByKey("HS-106", new CardsDS.EvolutionaryMon("Blissful Nurse", c =>
                {
                    foreach (var m in c.MyPokemon(x => x.DamageCounters > 0).ToList())
                    {
                        c.Heal(m, m.DamageCounters * 10);
                        foreach (var e in m.Energy.ToList()) c.Engine.DiscardEnergy(m, e);
                    }
                }, c => c.MyPokemon(x => x.DamageCounters > 0).Count > 0, null));
            var hoOh = new HoOhSacredRainbow();
            r.RegisterByKey("HS-111", hoOh);
            r.RegisterByKey("HS-112", hoOh);
            var lugia = new BenchTriggerFx("Ocean Grow", c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.LookAtTop(5);
                    foreach (var x in top.Where(x => x.Def.IsEnergy).ToList())
                        if (c.Me.Deck.Remove(x) && !c.Engine.AttachEnergyCard(x, c.Pokemon, false) && !c.Me.Discard.Contains(x)) c.Me.Discard.Add(x);
                    foreach (var x in top) if (c.Me.Deck.Remove(x)) c.Me.Discard.Add(x);
                }, new Fx().After("Elemental Blast", (c, a) =>
                {
                    DiscardTypeFromSelf(c, EnergyType.Fire, 1);
                    DiscardTypeFromSelf(c, EnergyType.Water, 1);
                    DiscardTypeFromSelf(c, EnergyType.Lightning, 1);
                }));
            r.RegisterByKey("HS-113", lugia);
            r.RegisterByKey("HS-114", lugia);
            r.RegisterByKey("HS-123", new Fx().On("Thrash", (c, a) =>
                {
                    bool heads = c.Flip();
                    c.DamageDefending(a.Damage + (heads ? 20 : 0));
                    if (!heads && c.InPlay(c.Pokemon)) c.DamageSelf(20);
                    return true;
                }));
            r.RegisterByKey("HS-ONE", new AlphLithograph(c => LookAtHand(c, c.Opp), c => c.Opp.Hand.Count > 0));
        }

        /// <summary>During the opponent's next turn, the attacks of the Pokémon cost [C] more (Graffiti).</summary>
        internal sealed class CostUpEffect : ContinuousEffect
        {
            private readonly int count;

            public CostUpEffect(string name, int owner, int targetUid, int expiresAfterTurn, int count = 1) : base(name, owner, targetUid, expiresAfterTurn, true) { this.count = count; }

            public override void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != null && mon.Uid == TargetUid) for (int i = 0; i < count; i++) cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Green Shield (Poké-Body): your Grass Pokémon have no Weakness.</summary>
        private sealed class MetapodGreenShield : BodyMon
        {
            public MetapodGreenShield() : base(null) { }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner == src.Player && src.Engine.IsType(mon, EnergyType.Grass) && Works(src, "Green Shield");
            }
        }

        /// <summary>Conductivity (Poké-Body): when the opponent attaches an Energy card from their hand, 1 damage counter on that Pokémon. Lightning Crush.</summary>
        private sealed class AmpharosConductivity : BodyMon
        {
            public AmpharosConductivity() : base(new Fx().On("Lightning Crush", (c, a) =>
                {
                    bool heads = c.Flip();
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (heads ? 40 : 0));
                    if (!heads && d != null && c.InPlay(d) && d.Energy.Count > 0 && c.CanAffect(d)) c.DiscardEnergyFrom(d, 1, "Lightning Crush: discard an Energy attached to " + d.Name);
                    return true;
                })) { }

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target.Owner == src.Player || !src.InPlay(target) || !Works(src, "Conductivity")) return;
                src.Engine.PlaceDamageCounters(target, 1, "Conductivity");
            }
        }

        /// <summary>Sacred Rainbow (Poké-Body): all Energy attached to Ho-Oh LEGEND is Fire. Bright Wing.</summary>
        private sealed class HoOhSacredRainbow : BodyMon
        {
            public HoOhSacredRainbow() : base(new Fx().After("Bright Wing", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.DiscardEnergyFrom(c.Pokemon, 1, "Bright Wing: discard an Energy attached to " + c.Pokemon.Name);
                })) { }

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon || !Works(src, "Sacred Rainbow")) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }
        }

        /// <summary>Pokémon Collector (Supporter): search your deck for up to 3 Basic Pokémon.</summary>
        private sealed class PokemonCollector : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(d => d.IsBasicPokemon, 3, "Pokémon Collector: search your deck for up to 3 Basic Pokémon"); }
        }

        /// <summary>Professor Oak's New Theory (Supporter): shuffle your hand into your deck, then draw 6 cards.</summary>
        private sealed class ProfessorOaksNewTheory : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count + ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ShuffleHandDraw(ctx, ctx.Player, 6); }
        }

        /// <summary>
        /// Alph Lithograph (Item): the card's own instruction (ONE: look at the opponent's hand; TWO: shuffle your deck; THREE: return
        /// a Stadium in play to its owner's hand; FOUR: look at your face-down Prize cards).
        /// </summary>
        internal sealed class AlphLithograph : CardScript
        {
            private readonly Action<EffectContext> effect;
            private readonly Func<EffectContext, bool> ready;

            public AlphLithograph(Action<EffectContext> effect, Func<EffectContext, bool> ready)
            {
                this.effect = effect;
                this.ready = ready;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ready == null || ready(ctx); }

            public override void ResolveTrainer(EffectContext ctx) { effect(ctx); }
        }
    }
}
