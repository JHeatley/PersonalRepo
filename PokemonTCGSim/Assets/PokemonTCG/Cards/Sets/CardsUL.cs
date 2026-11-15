using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>HS—Unleashed (UL, 2010; v173): the cards not already in the pool (the others are prints). Unlimited only.</summary>
    public static class CardsUL
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UL-1", new BenchTriggerFx("Stardust Song", c => c.Me.Discard.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Psychic)), c =>
                {
                    int heads = c.FlipCoins(3);
                    for (int i = 0; i < heads; i++)
                    {
                        var card = c.Me.Discard.FirstOrDefault(x => CardsRS.BasicOf(x.Def, EnergyType.Psychic));
                        if (card == null) break;
                        if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                    }
                }, new Fx().Effect("Time Hollow", (c, a) =>
                {
                    int n = c.EnergyCount(c.Pokemon);
                    var pool = c.OpponentPokemon(m => CardsE2.IsEvolved(m) && (m.Def.Stage == Stage.Stage1 || m.Def.Stage == Stage.Stage2));
                    if (n == 0 || pool.Count == 0) return;
                    foreach (var t in c.ChoosePokemonMany(pool, 0, Math.Min(n, pool.Count), "Time Hollow: choose up to " + n + " of your opponent's Evolved Pokémon"))
                        if (c.CanAffect(t)) c.Engine.Devolve(t, 1);
                })));
            r.RegisterByKey("UL-2", new Fx()
                .Effect("Hard Crush", (c, a) =>
                {
                    var top = c.Engine.DiscardTopOfDeck(c.Player, 3);
                    c.DamageDefending(50 * top.Count(x => x.Def.IsEnergy));
                })
                .After("Mantle Bazooka", (c, a) => DiscardTypeFromSelf(c, EnergyType.Fire, 2)));
            r.RegisterByKey("UL-3", new Fx().Effect("Deep Sea Swirl", (c, a) => ShuffleHandDraw(c, c.Player, 5)));
            r.RegisterByKey("UL-4", new MetagrossPsychicFloat());
            r.RegisterByKey("UL-5", new PowerFx("Magical Trans", c => c.Me.InPlay().Count() > 1 && PsychicHolders(c).Count > 0, c =>
                {
                    var from = c.ChoosePokemon(PsychicHolders(c), "Magical Trans: move a Psychic Energy from which Pokémon?");
                    if (from == null) return;
                    var to = c.ChoosePokemon(c.MyPokemon(m => m != from), "Magical Trans: move it to which Pokémon?");
                    if (to != null) c.Engine.MoveEnergy(from, to, from.Energy.First(e => CardsDRI1.ProvidesType(c, from, e, EnergyType.Psychic)));
                }, new Fx().After("Psychic Pulse", (c, a) => HitEachBenched(c, 10, m => m.DamageCounters > 0))));
            r.RegisterByKey("UL-6", new Fx()
                .Effect("Switch Cannon", (c, a) =>
                {
                    HitAny(c, 30, "Switch Cannon");
                    if (c.InPlay(c.Pokemon) && c.MyBench().Count > 0) c.SwitchSelfWithBench();
                })
                .After("Ink Bomb", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Ink Bomb)", c.Player, d.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("UL-7", new PowerFx("Leap Frog", c => c.Me.Active != null && c.MyBench().Any(m => c.Engine.IsType(m, EnergyType.Water)), c =>
                {
                    var t = c.ChoosePokemon(c.MyBench().Where(m => c.Engine.IsType(m, EnergyType.Water)).ToList(), "Leap Frog: switch which Benched Water Pokémon with your Active Pokémon?");
                    if (t != null) c.Engine.SwitchActive(c.Player, t);
                }, new Fx().Damage("Big Chorus", (c, a) => 30 * c.FlipCoins(c.MyPokemon(m => c.Engine.IsType(m, EnergyType.Water)).Count))));
            r.RegisterByKey("UL-8", new BenchTriggerFx("Celebration Wind", c => c.MyPokemon(m => m.Energy.Count > 0).Count > 0 && c.Me.InPlay().Count() > 1, c =>
                {
                    RepeatableAbility.Repeat(c, () => c.MyPokemon(m => m.Energy.Count > 0).Count > 0, done =>
                    {
                        var from = RepeatableAbility.ChoosePokemonOrDone(c, c.MyPokemon(m => m.Energy.Count > 0), "Celebration Wind: move an Energy card from which Pokémon? Press Done when finished.", true);
                        if (from == null) return false;
                        var to = c.ChoosePokemon(c.MyPokemon(m => m != from), "Celebration Wind: move it to which Pokémon?");
                        if (to == null) return false;
                        var e = from.Energy.Count == 1 ? from.Energy[0] : c.ChooseCards(from.Energy.ToList(), 1, 1, "Celebration Wind: move which Energy card?")[0];
                        return c.Engine.MoveEnergy(from, to, e);
                    });
                }, new Fx().After("Energy Bloom", (c, a) =>
                {
                    foreach (var m in c.MyPokemon(x => x.Energy.Count > 0 && x.DamageCounters > 0)) c.Heal(m, 30);
                })));
            r.RegisterByKey("UL-9", new Fx()
                .Damage("Push Over", (c, a) => 20 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fighting))
                .After("Rumble", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("UL-10", GigaDrain("Giga Drain"));
            r.RegisterByKey("UL-11", new Fx()
                .Damage("Psywave", (c, a) => 20 * (c.Defending == null ? 0 : c.EnergyCount(c.Defending)))
                .With(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("UL-12", new Fx()
                .With(new CoinsMon("Twineedle", 2))
                .After("Paralyze Poison", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                    if (c.Flip()) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                }));
            r.RegisterByKey("UL-13", new RepeatPowerFx("Wash Out", c => c.Me.Active != null && c.MyBench().Any(m => m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Water))), (c, done) =>
                {
                    var donors = c.MyBench().Where(m => m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Water))).ToList();
                    var from = RepeatableAbility.ChoosePokemonOrDone(c, donors, "Wash Out: move a Water Energy from which Benched Pokémon?" + (done ? " Press Done when finished." : ""), done);
                    if (from == null || c.Me.Active == null) return false;
                    return c.Engine.MoveEnergy(from, c.Me.Active, from.Energy.First(e => CardsDRI1.ProvidesType(c, from, e, EnergyType.Water)));
                }, new Fx().Effect("Hydro Launcher", (c, a) =>
                {
                    var water = c.Pokemon.Energy.Where(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Water)).ToList();
                    var picks = water.Count <= 2 ? water : c.ChooseCards(water, 2, 2, "Hydro Launcher: return which 2 Water Energy to your hand?");
                    foreach (var e in picks) c.Engine.ReturnEnergyToHand(c.Pokemon, e);
                    HitAny(c, 100, "Hydro Launcher");
                })));
            r.RegisterByKey("UL-14", new ComboMon(new ConditionAttackMon("Supersonic", SpecialCondition.Confused), new CoinsMon("Hurricane Wing", 4)));
            r.RegisterByKey("UL-15", new CoinsMon("Repeating Drill", 5));
            r.RegisterByKey("UL-16", new PowerFx("Water Acceleration", c => c.Me.Hand.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Water)), c =>
                {
                    var water = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Water)).ToList();
                    var card = c.ChooseCards(water, 1, 1, "Water Acceleration: attach which Water Energy card?")[0];
                    if (c.Me.Hand.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, true) && !c.Me.Discard.Contains(card)) c.Me.Hand.Add(card);
                }, null));
            r.RegisterByKey("UL-17", new Fx()
                .Effect("Water Arrow", (c, a) => HitAny(c, 30, "Water Arrow"))
                .On("Stream Pump", (c, a) =>
                {
                    bool more = c.Pokemon.Energy.Count > 0 && c.AskYesNo("Stream Pump: do 30 more damage and return an Energy card to your hand?");
                    c.DamageDefending(a.Damage + (more ? 30 : 0));
                    if (more && c.InPlay(c.Pokemon) && c.Pokemon.Energy.Count > 0)
                    {
                        var e = c.Pokemon.Energy.Count == 1 ? c.Pokemon.Energy[0] : c.ChooseCards(c.Pokemon.Energy.ToList(), 1, 1, "Stream Pump: return which Energy card?")[0];
                        c.Engine.ReturnEnergyToHand(c.Pokemon, e);
                    }
                    return true;
                }));
            r.RegisterByKey("UL-18", new Fx()
                .With(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused))
                .After("Reflect Energy", (c, a) => EnergyToBench(c, "Reflect Energy")));
            r.RegisterByKey("UL-19", new Fx().After("Bulk Up", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new AttackBonusEffect(c.Pokemon.Name + "'s attacks do 30 more damage next turn (Bulk Up)", c.Player, c.Pokemon.Uid, c.Turn + 2, 30));
                }));
            r.RegisterByKey("UL-20", new Fx()
                .Effect("Heat Acceleration", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)).ToList();
                    if (pile.Count == 0) return;
                    var picks = c.ChooseCards(pile, 1, Math.Min(3, pile.Count), "Heat Acceleration: choose up to 3 Fire Energy cards");
                    var t = c.ChoosePokemon(c.MyPokemon(), "Heat Acceleration: attach them to which Pokémon?");
                    if (t == null) return;
                    foreach (var card in picks) if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, t, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .With(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)));
            r.RegisterByKey("UL-21", new Fx()
                .After("Steamroll", (c, a) => HitBenched(c, 1, 20, "Steamroll"))
                .On("Dynamic Punch", (c, a) =>
                {
                    bool heads = c.Flip();
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (heads ? 40 : 0));
                    if (heads && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Confused);
                    return true;
                }));
            r.RegisterByKey("UL-22", new Fx().Effect("Bebop Punch", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Bebop Punch: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    int heads = c.FlipUntilTails();
                    if (heads > 0) c.DamageTarget(t, 50 * heads);
                }));
            r.RegisterByKey("UL-23", new RoseradeEnergySignal());
            r.RegisterByKey("UL-24", new ComboMon(new ShieldAttackMon("Guard Press", 20), new CoinsMon("Steel Swing", 2)));
            r.RegisterByKey("UL-25", new BenchTriggerFx("Hot Snort", c => c.Opp.Active != null, c =>
                {
                    var t = c.Opp.Active;
                    if (c.Flip() && t != null && c.CanAffect(t)) c.ApplyCondition(t, SpecialCondition.Burned);
                }, null));
            r.RegisterByKey("UL-26", new Fx()
                .With(new FlipBonusMon("Tail Crush", 20))
                .After("Hyper Beam", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.DiscardEnergyFrom(d, 1, "Hyper Beam: discard an Energy card attached to " + d.Name);
                }));
            r.RegisterByKey("UL-27", new FlipBonusMon("Cross Chop", 30));
            r.RegisterByKey("UL-28", new PowerFx("Sunny Heal", c => c.Me.Active != null && c.Me.Active.DamageCounters > 0, c => c.Heal(c.Me.Active, 10), null));
            r.RegisterByKey("UL-29", new Fx().After("Return", (c, a) => c.DrawUntil(6)));
            r.RegisterByKey("UL-30", new Fx().After("Mean Look", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("UL-31", new CardsPAR2.FlipConditionMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("UL-32", new Fx()
                .Effect("Speed Evolution", (c, a) => EvolveFromDeck(c, c.Pokemon, d => !d.IsLevelUp, "Speed Evolution: search your deck for a card that evolves from " + c.Pokemon.Name))
                .With(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("UL-33", new Fx().Effect("Energy Crane", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Psychic)).ToList();
                    if (pile.Count > 0) c.AttachCardsAnyWay(pile, 2, (x, m) => true, "Energy Crane: attach up to 2 Psychic Energy cards", true);
                }));
            r.RegisterByKey("UL-35", new Fx().Effect("Flare Bonus", (c, a) =>
                {
                    var fire = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)).ToList();
                    if (fire.Count == 0) { DidNothing(c); return; }
                    c.Engine.DiscardFromHand(c.Player, c.ChooseCards(fire, 1, 1, "Flare Bonus: discard which Fire Energy card?")[0]);
                    c.Draw(3);
                }));
            r.RegisterByKey("UL-37", new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep));
            r.RegisterByKey("UL-38", new PupitarBoostGas());
            r.RegisterByKey("UL-40", new Fx().After("Muddy Water", (c, a) => HitBenched(c, 1, 10, "Muddy Water")));
            r.RegisterByKey("UL-41", new Fx().On("Crimson Bull", (c, a) =>
                {
                    c.DamageDefending(30 * c.FlipCoins(3));
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                    return true;
                }));
            r.RegisterByKey("UL-42", new Fx().Effect("Water Arrow", (c, a) => HitAny(c, 20, "Water Arrow")));
            r.RegisterByKey("UL-43", new Fx()
                .Effect("Tail Code", (c, a) => CardsPL.TailCode(c))
                .With(new TailsNothingMon("Tail Smash")));
            r.RegisterByKey("UL-44", new SwitchSelfMon("Reaction", true));
            r.RegisterByKey("UL-45", new Fx().After("Muddy Water", (c, a) => HitBenched(c, 1, 10, "Muddy Water")));
            r.RegisterByKey("UL-46", new SearchAttackMon("Drawup Power", d => d.IsEnergy, 1));
            r.RegisterByKey("UL-49", new CoinsMon("Fin Smack", 2));
            r.RegisterByKey("UL-50", new Fx()
                .Effect("Mountain Eater", (c, a) =>
                {
                    // No card to discard, no healing (HS:Unleashed FAQ).
                    if (c.Engine.DiscardTopOfDeck(c.Opponent, 1).Count > 0 && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 20);
                })
                .With(new RecoilMon("Reckless Charge", 10)));
            r.RegisterByKey("UL-53", new Fx().Damage("Karate Chop", (c, a) => a.Damage - 10 * c.Pokemon.DamageCounters));
            r.RegisterByKey("UL-54", new Fx().Effect("Sharpshooting", (c, a) => HitAny(c, 10, "Sharpshooting")));
            r.RegisterByKey("UL-55", new SwitchSelfMon("Teleport", false));
            r.RegisterByKey("UL-56", new OnixEnergyHealer());
            r.RegisterByKey("UL-57", new Fx().Damage("Swing Around", (c, a) => a.Damage + 20 * c.FlipCoins(2)));
            r.RegisterByKey("UL-58", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("UL-59", new Fx().Effect("Water Arrow", (c, a) => HitAny(c, 20, "Water Arrow")));
            r.RegisterByKey("UL-60", new CoinsMon("Double Chop", 2));
            r.RegisterByKey("UL-61", new Fx().On("Petal Dance", (c, a) =>
                {
                    c.DamageDefending(20 * c.FlipCoins(3));
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                    return true;
                }));
            r.RegisterByKey("UL-62", new Fx().Effect("Roost", (c, a) =>
                {
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                    c.AddEffect(new SelfNoRetreatEffect(c.Pokemon.Name + " can't retreat during your next turn (Roost)", c.Player, c.Pokemon.Uid, c.Turn + 2));
                }));
            r.RegisterByKey("UL-64", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new ConditionAttackMon("Mystifying Horns", SpecialCondition.Confused)));
            r.RegisterByKey("UL-65", new RecoilMon("Take Down", 10));
            r.RegisterByKey("UL-66", new Fx().Effect("Green Call", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads > 0) c.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Grass), heads, "Green Call: search your deck for up to " + heads + " Grass Pokémon");
                }));
            r.RegisterByKey("UL-67", new Fx().Effect("Leech Seed", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    if (done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 10);
                }));
            r.RegisterByKey("UL-68", new Fx().After("Fireworks", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); }));
            r.RegisterByKey("UL-69", new Fx().Effect("Speed Evolution", (c, a) => EvolveFromDeck(c, c.Pokemon, d => !d.IsLevelUp, "Speed Evolution: search your deck for a card that evolves from " + c.Pokemon.Name)));
            r.RegisterByKey("UL-70", new Fx().Effect("Double Attack", (c, a) => HitBenched(c, 2, 10, "Double Attack")));

            r.RegisterByKey("UL-71", new CheerleadersCheer());
            r.RegisterByKey("UL-73", new SimpleTrainer(c => c.Me.Deck.Count > 0, c => c.Draw(c.Flip() ? 3 : 2)));
            r.RegisterByKey("UL-74", new SimpleTrainer(c => c.BasicEnergyInDiscard().Count > 0, c =>
                {
                    var pile = c.BasicEnergyInDiscard();
                    int n = Math.Min(4, pile.Count);
                    c.DiscardToDeck(c.ChooseCards(pile, n, n, "Energy Returner: shuffle " + n + " basic Energy cards into your deck"));
                }));
            r.RegisterByKey("UL-75", new SimpleTrainer(c => c.Me.Hand.Any(x => x != c.Card && x.Def.IsEnergy), c =>
                {
                    var energy = c.Me.Hand.Where(x => x != c.Card && x.Def.IsEnergy).ToList();
                    c.Engine.DiscardFromHand(c.Player, c.ChooseCards(energy, 1, 1, "Engineer's Adjustments: discard which Energy card?")[0]);
                    c.Draw(4);
                }));
            r.RegisterByKey("UL-76", new SimpleTrainer(c => c.Me.Discard.Any(x => x.Def.IsPokemon || x.Def.IsTrainer), c =>
                {
                    bool heads = c.Flip();
                    var pile = c.Me.Discard.Where(x => heads ? x.Def.IsPokemon : x.Def.IsTrainer).ToList();
                    if (pile.Count == 0) return;
                    var card = c.ChooseCards(pile, 1, 1, "Good Rod: put which card on top of your deck?")[0];
                    c.Me.Discard.Remove(card);
                    c.Me.Deck.Insert(0, card);
                    c.Engine.ShowCards(c.Opponent, new List<CardInstance> { card }, "Good Rod", "Good Rod: the card your opponent put on top of their deck", c.Source);
                }));
            r.RegisterByKey("UL-77", new SimpleTrainer(c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.LookAtTop(8);
                    var energy = top.Where(x => x.Def.IsEnergy).ToList();
                    if (energy.Count > 0) c.DeckToHand(c.ChooseCards(energy, 0, energy.Count, "Interviewer's Questions: put any Energy cards into your hand"));
                    c.ShuffleMyDeck();
                }));
            r.RegisterByKey("UL-81", new SimpleTrainer(c => c.Opp.Bench.Count > 0, c => c.OpponentSwitchesActive()));

            r.RegisterByKey("UL-84", new Fx()
                .Effect("Severe Poison", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d) && c.ApplyCondition(d, SpecialCondition.Poisoned)) d.PoisonCounterBase = Math.Max(d.PoisonCounterBase, 4);
                })
                .Effect("Skill Dive", (c, a) => HitAny(c, 30, "Skill Dive")));
            r.RegisterByKey("UL-85", new PowerFx("Spray Splash", c => c.OpponentPokemon().Count > 0, c =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Spray Splash: put 1 damage counter on which of your opponent's Pokémon?");
                    if (t != null && c.CanAffect(t)) c.PlaceCounters(t, 1);
                }, new Fx().Damage("Dragon Steam", (c, a) => c.OpponentPokemon(m => c.Engine.IsType(m, EnergyType.Fire)).Count > 0 ? 20 : a.Damage)));
            r.RegisterByKey("UL-86", new PowerFx("Underwater Dive", null, c =>
                    c.AddEffect(new CardsGE.TypeIsEffect(c.Pokemon.Name + " is Water this turn (Underwater Dive)", c.Player, c.Pokemon.Uid, c.Turn, EnergyType.Water)),
                new Fx().Damage("Powerful Spark", (c, a) => a.Damage + 10 * c.MyPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("UL-87", new CardsRG.ThickSkinMon(new Fx()
                .Effect("Energy Stream", (c, a) =>
                {
                    c.DamageDefending(a.Damage);
                    var pile = c.Me.Discard.Where(x => x.Def.IsEnergy).ToList();
                    if (pile.Count == 0 || !c.InPlay(c.Pokemon)) return;
                    var card = c.ChooseCards(pile, 1, 1, "Energy Stream: attach which Energy card?")[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .After("Gaia Crush", (c, a) =>
                {
                    if (c.State.Stadium != null && c.AskYesNo("Gaia Crush: discard the Stadium in play?")) c.Engine.DiscardStadium();
                })));
            r.RegisterByKey("UL-88", new Fx()
                .Effect("Darkness Howl", (c, a) =>
                {
                    foreach (var m in c.MyPokemon().Concat(c.OpponentPokemon()).Where(x => !c.Engine.IsType(x, EnergyType.Darkness)).ToList())
                        if (c.InPlay(m)) c.DamageTarget(m, 20);
                })
                .Effect("Power Claw", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, true, true); })
                .After("Megaton Tail", (c, a) => c.Engine.DiscardTopOfDeck(c.Player, 3)));
            r.RegisterByKey("UL-89", new UrsaringBerserk());

            var enteiRaikou = new Fx()
                .After("Detonation Spin", (c, a) => DiscardTypeFromSelf(c, EnergyType.Fire, 1))
                .Effect("Thunder Fall", (c, a) =>
                {
                    foreach (var e in c.Pokemon.Energy.ToList()) c.Engine.DiscardEnergy(c.Pokemon, e);
                    foreach (var m in c.MyPokemon().Concat(c.OpponentPokemon()).Where(HasPower).ToList())
                        if (c.InPlay(m)) c.DamageWith(m, 80, false, false, false);
                });
            r.RegisterByKey("UL-90", enteiRaikou);
            r.RegisterByKey("UL-91", enteiRaikou);
            var raikouSuicune = new Fx()
                .After("Thunderbolt Spear", (c, a) => { if (c.InPlay(c.Pokemon)) c.DamageSelf(50); })
                .After("Aurora Gain", (c, a) => { if (c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 50); });
            r.RegisterByKey("UL-92", raikouSuicune);
            r.RegisterByKey("UL-93", raikouSuicune);
            var suicuneEntei = new Fx()
                .Effect("Torrent Blade", (c, a) =>
                {
                    var water = c.Pokemon.Energy.Where(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Water)).ToList();
                    var picks = water.Count <= 2 ? water : c.ChooseCards(water, 2, 2, "Torrent Blade: return which 2 Water Energy to your hand?");
                    foreach (var e in picks) c.Engine.ReturnEnergyToHand(c.Pokemon, e);
                    HitBenched(c, 1, 100, "Torrent Blade");
                })
                .With(new ConditionAttackMon("Bursting Inferno", SpecialCondition.Burned));
            r.RegisterByKey("UL-94", suicuneEntei);
            r.RegisterByKey("UL-95", suicuneEntei);
            r.RegisterByKey("UL-TWO", new CardsHS.AlphLithograph(c => c.ShuffleMyDeck(), null));
        }

        private static List<PokemonInPlay> PsychicHolders(EffectContext c)
        {
            return c.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Psychic)));
        }

        /// <summary>"Remove from this Pokémon the number of damage counters equal to the damage you did to the Defending Pokémon."</summary>
        internal static Fx GigaDrain(string attack)
        {
            return new Fx().Effect(attack, (c, a) =>
            {
                int done = c.DamageDefending(a.Damage);
                if (done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
            });
        }

        /// <summary>A Trainer with a playability check and an effect.</summary>
        internal sealed class SimpleTrainer : CardScript
        {
            private readonly Func<EffectContext, bool> can;
            private readonly Action<EffectContext> effect;

            public SimpleTrainer(Func<EffectContext, bool> can, Action<EffectContext> effect)
            {
                this.can = can;
                this.effect = effect;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return can == null || can(ctx); }

            public override void ResolveTrainer(EffectContext ctx) { effect(ctx); }
        }

        /// <summary>Cheerleader's Cheer (Supporter): draw 3 cards; the opponent may draw a card.</summary>
        private sealed class CheerleadersCheer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Cheerleader's Cheer: draw a card?", ctx.Source)) ctx.Engine.DrawCards(ctx.Opponent, 1);
            }
        }

        /// <summary>Psychic Float (Poké-Body): your Active Pokémon with any Psychic Energy attached has no Retreat Cost. Double Leg Hammer.</summary>
        private sealed class MetagrossPsychicFloat : BodyMon
        {
            public MetagrossPsychicFloat() : base(new Fx().Effect("Double Leg Hammer", (c, a) => HitBenched(c, 2, 40, "Double Leg Hammer"))) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon != src.Me.Active || !mon.Energy.Any(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Psychic)) || !Works(src, "Psychic Float")) return cost;
                return 0;
            }
        }

        /// <summary>
        /// Energy Signal (Poké-Power): when you attach a Grass or Psychic Energy card from your hand to it during your turn, the
        /// Defending Pokémon is Confused (Grass) or Poisoned (Psychic). Not with a Special Condition. Power Blow.
        /// </summary>
        private sealed class RoseradeEnergySignal : BodyMon
        {
            public RoseradeEnergySignal() : base(new Fx().Damage("Power Blow", (c, a) => 20 * c.EnergyCount(c.Pokemon))) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                var d = src.Opp.Active;
                if (target != me || !src.IsMyTurn || !CardsE1.NoCondition(me) || d == null) return;
                bool grass = CardsDRI1.ProvidesType(src, me, energy, EnergyType.Grass);
                bool psychic = CardsDRI1.ProvidesType(src, me, energy, EnergyType.Psychic);
                if (!grass && !psychic) return;
                if (!src.AskYesNo("Energy Signal: use it?")) return;
                Announce(src, "Energy Signal");
                if (!src.CanAffect(d)) return;
                if (grass) src.ApplyCondition(d, SpecialCondition.Confused);
                if (psychic && src.InPlay(d)) src.ApplyCondition(d, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Boost Gas (Poké-Body): with any Energy attached, no Retreat Cost. Rage.</summary>
        private sealed class PupitarBoostGas : BodyMon
        {
            public PupitarBoostGas() : base(new Fx().Damage("Rage", (c, a) => a.Damage + 10 * c.Pokemon.DamageCounters)) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return mon == src.Pokemon && mon.Energy.Count > 0 && Works(src, "Boost Gas") ? 0 : cost; }
        }

        /// <summary>Energy Healer (Poké-Body): attaching an Energy card from your hand to it removes a damage counter. Boundless Power.</summary>
        private sealed class OnixEnergyHealer : BodyMon
        {
            public OnixEnergyHealer() : base(new LockAfterAttackMon("Boundless Power", false)) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == src.Pokemon && target.DamageCounters > 0 && Works(src, "Energy Healer")) src.Heal(target, 10);
            }
        }

        /// <summary>Berserk (Poké-Body): with any damage counters on it, its attacks do 60 more damage (before W/R). Hammer Arm.</summary>
        private sealed class UrsaringBerserk : BodyMon
        {
            public UrsaringBerserk() : base(new Fx().After("Hammer Arm", (c, a) => c.Engine.DiscardTopOfDeck(c.Opponent, 1))) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || src.Pokemon.DamageCounters == 0 || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return info.Target == src.Opp.Active && Works(src, "Berserk") ? damage + 60 : damage;
            }
        }
    }
}
