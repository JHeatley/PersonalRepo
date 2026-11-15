using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>Rising Rivals (RR; v173): Uncommons, Commons, Trainers and Energy.</summary>
    public static partial class CardsRR
    {
        private const string NeedleShockLimit = "Needle Shock";

        private static void RegisterCommons(CardScriptRegistry r)
        {
            r.RegisterByKey("RR-14", new Fx()
                .On("Return Blow", (c, a) => ReturnBlow(c))
                .After("Metal Fang", (c, a) =>
                {
                    if (c.Me.Deck.Count == 0 || !c.InPlay(c.Pokemon) || !c.AskYesNo("Metal Fang: discard the top card of your deck to remove 2 damage counters and all Special Conditions?")) return;
                    c.Engine.DiscardTopOfDeck(c.Player, 1);
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 20);
                    c.Engine.RemoveAllConditions(c.Pokemon);
                }));
            r.RegisterByKey("RR-15", new PowerFx("Flutter Wings", c => c.Me.Deck.Count > 0, c =>
                    c.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Grass), 1, "Flutter Wings: search your deck for a Grass Pokémon"),
                new Fx().After("Needle Shock", (c, a) =>
                {
                    int t;
                    bool usedLast = c.Me.NamedLimitUsedOnTurn.TryGetValue(NeedleShockLimit, out t) && t == c.MyLastTurn;
                    c.Me.NamedLimitUsedOnTurn[NeedleShockLimit] = c.Turn;
                    var d = c.Defending;
                    if (usedLast || d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                })));
            r.RegisterByKey("RR-16", new Fx()
                .Effect("Hand Refresh", (c, a) => EachShufflesAndDraws(c, 4, "Hand Refresh"))
                .On("Payback", (c, a) =>
                {
                    bool last = c.Opp.Prizes.Count == 1;
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (last ? 50 : 0));
                    if (last && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Confused);
                    return true;
                }));
            r.RegisterByKey("RR-17", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("RR-18", new Fx()
                .With(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep))
                .Damage("Psywave", (c, a) => a.Damage + 10 * (c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("RR-19", Undevelop(new Fx().On("Evolving Flare", (c, a) => EvolvingBonus(c, a, SpecialCondition.Burned))));
            r.RegisterByKey("RR-20", new Fx()
                .After("Chop Up", (c, a) => HitEachBenched(c, 10, m => m.DamageCounters > 0))
                .Effect("Feint", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); }));
            r.RegisterByKey("RR-21", new GastrodonStickyHold());
            r.RegisterByKey("RR-22", new Fx()
                .Effect("Muddy Bomb", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Muddy Bomb: choose 1 of your opponent's Pokémon (30 damage)");
                    if (t == null) return;
                    c.DamageTarget(t, 30);
                    foreach (var m in c.OpponentPokemon(x => x != t).ToList()) c.DamageTarget(m, 10);
                })
                .Damage("Raging Sea", (c, a) => a.Damage + 10 * c.MyBench().Count(m => m.DamageCounters > 0)));
            r.RegisterByKey("RR-23", new Fx()
                .Damage("Rage", (c, a) => a.Damage + 10 * c.Pokemon.DamageCounters)
                .With(new RecoilMon("Double-edge", 60)));
            r.RegisterByKey("RR-24", FocusEnergy("Megahorn", 100, true));
            r.RegisterByKey("RR-25", new HippowdonSandCover());
            r.RegisterByKey("RR-26", Undevelop(new Fx()
                .With(new FlipBonusMon("Quick Attack", 30))
                .After("Evolving Thunder", (c, a) => { if (CardsP9.JustEvolvedFrom(c, "Eevee")) HitEachBenched(c, 10); })));
            r.RegisterByKey("RR-27", new MamoswineIcyAura());
            r.RegisterByKey("RR-28", new Fx()
                .Effect("Magic Heal", (c, a) =>
                {
                    int heads = c.FlipCoins(3);
                    for (int i = 0; i < heads; i++)
                    {
                        var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                        if (hurt.Count == 0) break;
                        var t = c.ChoosePokemon(hurt, "Magic Heal: remove 1 damage counter from which Pokémon? (" + (i + 1) + " of " + heads + ")");
                        if (t != null) c.Heal(t, 10);
                    }
                })
                .With(new ShieldAttackMon("Barrier Attack", 10)));
            r.RegisterByKey("RR-29", new NidokingTerritoriality());
            r.RegisterByKey("RR-30", new NidoqueenMaternalComfort());
            r.RegisterByKey("RR-31", new Fx()
                .Effect("Thunder Throw", (c, a) => HitBenched(c, 2, 10, "Thunder Throw"))
                .Damage("Repeat Lightning", (c, a) => a.Damage + 10 * c.OpponentBench().Count(m => m.DamageCounters > 0)));
            r.RegisterByKey("RR-32", new Fx()
                .With(new CoinsMon("Double Lariat", 2))
                .Effect("Rock Tumble", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); }));
            r.RegisterByKey("RR-33", new Fx()
                .Effect("Pick and Collect", (c, a) =>
                {
                    var pile = c.BasicEnergyInDiscard();
                    if (pile.Count > 0) c.DiscardToHand(c.ChooseCards(pile, 1, Math.Min(4, pile.Count), "Pick and Collect: put up to 4 basic Energy cards into your hand"));
                })
                .After("Roll Over", (c, a) =>
                {
                    bool heads = c.Flip();
                    var d = c.Defending;
                    if (heads && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Asleep);
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                }));
            r.RegisterByKey("RR-34", Undevelop(new Fx()
                .After("Muddy Water", (c, a) => HitBenched(c, 1, 10, "Muddy Water"))
                .Effect("Evolving Aqua", (c, a) => HitAny(c, CardsP9.JustEvolvedFrom(c, "Eevee") ? 60 : 40, "Evolving Aqua", m => m.DamageCounters > 0))));
            r.RegisterByKey("RR-35", new VespiquenRoyalGain());
            r.RegisterByKey("RR-36", new CardsDS.EvolutionaryMon("Gather Ice", c =>
                {
                    var water = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Water)).ToList();
                    var picks = c.ChooseCards(water, 0, water.Count, "Gather Ice: attach as many Water Energy cards from your hand to " + c.Pokemon.Name + " as you like");
                    foreach (var x in picks) if (c.Me.Hand.Remove(x) && !c.Engine.AttachEnergyCard(x, c.Pokemon, true) && !c.Me.Discard.Contains(x)) c.Me.Hand.Add(x);
                }, c => c.Me.Hand.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Water)), new Fx()
                .After("Cold Crush", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.DiscardEnergyFrom(c.Pokemon, 1, "Cold Crush: discard an Energy card attached to " + c.Pokemon.Name);
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.DiscardEnergyFrom(d, 1, "Cold Crush: discard an Energy card attached to " + d.Name);
                })));
            r.RegisterByKey("RR-37", new Fx()
                .Effect("Skill Dive", (c, a) => HitAny(c, 10, "Skill Dive"))
                .With(new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("RR-38", new Fx()
                .Effect("Recover", (c, a) =>
                {
                    if (DiscardTypeFromSelf(c, EnergyType.Psychic, 1) > 0 && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                })
                .With(new CardsASC.FlipDiscardDefendingEnergyMon("Mysterious Beam")));
            r.RegisterByKey("RR-39", new Fx()
                .Effect("Reckless Bomb", (c, a) =>
                {
                    foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 20);
                    if (c.InPlay(c.Pokemon)) c.DamageSelf(100);
                })
                .After("Reflect Energy", (c, a) => EnergyToBench(c, "Reflect Energy")));
            r.RegisterByKey("RR-40", new Fx()
                .With(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused))
                .Effect("Attack and Hide", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Attack and Hide: put 3 damage counters on which of your opponent's Pokémon?");
                    if (t != null && c.CanAffect(t)) c.PlaceCounters(t, 3);
                    if (c.InPlay(c.Pokemon) && c.AskYesNo("Attack and Hide: shuffle " + c.Pokemon.Name + " and all cards attached to it into your deck?")) c.Engine.ShufflePokemonIntoDeck(c.Pokemon);
                }));
            r.RegisterByKey("RR-41", new GlaceonFrostWind());
            r.RegisterByKey("RR-42", new HippowdonSandArmor());
            r.RegisterByKey("RR-43", new Fx().Effect("Split Bomb", (c, a) =>
                {
                    var pool = c.OpponentPokemon();
                    int k = Math.Min(2, pool.Count);
                    foreach (var t in c.ChoosePokemonMany(pool, k, k, "Split Bomb: choose 2 of your opponent's Pokémon")) c.DamageTarget(t, 20);
                }));
            r.RegisterByKey("RR-44", new Fx()
                .Damage("Heavy Metal", (c, a) => a.Damage + 20 * c.FlipCoins(c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Metal)))
                .With(new ShieldAttackMon("Granite Head", 10)));
            r.RegisterByKey("RR-45", new LeafeonEnergyRefresh());
            r.RegisterByKey("RR-46", new Fx()
                .Damage("Cross-Cut", (c, a) => a.Damage + (c.Defending != null && CardsE2.IsEvolved(c.Defending) ? 20 : 0))
                .Effect("Brush Off", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.OpponentBench().Count == 0 || d == null) { DidNothing(c); return; }
                    if (c.CanAffect(d)) c.Engine.ShufflePokemonIntoDeck(d);
                }));
            r.RegisterByKey("RR-47", new Fx().Effect("Pickup Power", (c, a) =>
                {
                    int heads = c.FlipCoins(3);
                    var pile = c.BasicEnergyInDiscard();
                    if (heads > 0 && pile.Count > 0) c.DiscardToHand(c.ChooseCards(pile, Math.Min(heads, pile.Count), Math.Min(heads, pile.Count), "Pickup Power: put " + Math.Min(heads, pile.Count) + " basic Energy cards into your hand"));
                }));
            r.RegisterByKey("RR-48", new CardsASC.FlipDiscardDefendingEnergyMon("Crushing Blow"));
            r.RegisterByKey("RR-49", new SharpedoEnergyShark());
            r.RegisterByKey("RR-50", new PowerFx("Aqua Recycle", c => c.Me.Discard.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Water)), c =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Water)).ToList();
                    c.DiscardToHand(c.ChooseCards(pile, 1, 1, "Aqua Recycle: put which Water Energy card into your hand?"));
                }, new Fx()
                .After("Synchro Gain", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(c.Pokemon) && c.EnergyCount(c.Pokemon) == c.EnergyCount(d) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                })
                .With(new LockAfterAttackMon("Powerful Spin", false))));
            r.RegisterByKey("RR-51", new Fx()
                .Effect("Mend", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Metal)).ToList();
                    if (pile.Count == 0) return;
                    var card = c.ChooseCards(pile, 1, 1, "Mend: attach which Metal Energy card?")[0];
                    if (c.Me.Discard.Remove(card))
                    {
                        if (c.Engine.AttachEnergyCard(card, c.Pokemon, false)) { if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 10); }
                        else if (!c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                    }
                })
                .On("Squeeze", (c, a) =>
                {
                    bool heads = c.Flip();
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (heads ? 50 : 0));
                    if (heads && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    return true;
                }));
            r.RegisterByKey("RR-52", new Fx()
                .On("Fly", (c, a) =>
                {
                    if (!c.Flip()) return DidNothing(c);
                    c.DamageDefending(a.Damage);
                    if (c.InPlay(c.Pokemon)) c.ProtectSelfNextTurn();
                    return true;
                })
                .Effect("Blessed Fruit", (c, a) =>
                {
                    var pool = c.MyBench().Where(m => m.DamageCounters > 0 && c.Engine.IsType(m, EnergyType.Grass)).ToList();
                    if (pool.Count == 0) return;
                    var t = c.ChoosePokemon(pool, "Blessed Fruit: remove all damage counters from which Benched Grass Pokémon?");
                    if (t != null) c.Heal(t, t.DamageCounters * 10);
                }));
            r.RegisterByKey("RR-53", new Fx()
                .Effect("Energy Typhoon", (c, a) =>
                {
                    var energy = c.Opp.Discard.Where(x => x.Def.IsEnergy).ToList();
                    c.DamageDefending(20 * energy.Count);
                    foreach (var x in energy) if (c.Opp.Discard.Remove(x)) c.Opp.Deck.Insert(0, x);
                    if (energy.Count > 0) c.Engine.ShuffleDeck(c.Opponent);
                })
                .With(new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("RR-54", new Fx()
                .With(new ConditionAttackMon("Sleep Pulse", SpecialCondition.Asleep))
                .After("Earthquake", (c, a) => { foreach (var m in c.MyBench().ToList()) c.DamageTarget(m, 10); }));
            r.RegisterByKey("RR-55", new Fx()
                .With(new DrawAttackMon("Collect", 2))
                .After("Primal Breath", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new NoEvolveTargetEffect(d.Name + " can't evolve or level up from the hand (Primal Breath)", c.Player, d.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("RR-56", new Fx()
                .Effect("Tail Code", (c, a) => CardsPL.TailCode(c))
                .Damage("Snap Attack", (c, a) => c.Defending != null && c.Defending.Energy.Count > 0 ? 20 : a.Damage));
            r.RegisterByKey("RR-57", new Fx().Damage("Iron Head", (c, a) => 10 * c.FlipUntilTails()));
            r.RegisterByKey("RR-58", new Fx()
                .Effect("Scary Face", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.Flip() && c.CanAffect(d)) c.AddEffect(new NoAttackNoRetreatEffect(d.Name + " can't attack or retreat (Scary Face)", c.Player, d.Uid, c.Turn + 1));
                })
                .With(new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool")));
            r.RegisterByKey("RR-59", new Fx()
                .Effect("Signs of Evolution", (c, a) => c.SearchToHand(d => d.IsPokemon && d.EvolvesFrom == c.Pokemon.Name, 2, "Signs of Evolution: search your deck for up to 2 cards that evolve from " + c.Pokemon.Name))
                .With(new SwitchSelfMon("Bounce", false)));
            r.RegisterByKey("RR-60", new Fx().After("Fire Tail Slap", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); }));
            r.RegisterByKey("RR-61", new Fx()
                .Effect("Shell Scatter", (c, a) =>
                {
                    foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10);
                    if (c.InPlay(c.Pokemon)) c.SelfCantUseAttackNextTurn("Shell Scatter");
                })
                .After("Bomb Risk", (c, a) =>
                {
                    foreach (var m in c.MyBench().Concat(c.OpponentBench()).ToList()) if (c.InPlay(m)) c.DamageTarget(m, 10);
                    if (c.InPlay(c.Pokemon) && !c.Flip()) c.DamageSelf(80);
                }));
            r.RegisterByKey("RR-62", new Fx().On("Loaded Needle", (c, a) =>
                {
                    bool heads = c.Flip();
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (heads ? 30 : 0));
                    if (!heads && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                    return true;
                }));
            r.RegisterByKey("RR-63", new Fx()
                .Effect("Stoke", (c, a) => SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Fire), "Stoke", 1, m => m == c.Pokemon))
                .With(new RecoilMon("Take Down", 10)));
            r.RegisterByKey("RR-64", new Fx()
                .With(new CardsDRI1.WhirlwindMon("Push Down"))
                .Damage("Endeavor", (c, a) => a.Damage + 20 * c.FlipCoins(2)));
            r.RegisterByKey("RR-65", new Fx()
                .After("Corner", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); })
                .With(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned)));
            r.RegisterByKey("RR-66", new KakunaExoskeleton());
            r.RegisterByKey("RR-67", new KecleonColorfulBody());
            r.RegisterByKey("RR-68", new Fx()
                .Effect("Offensive Gas", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.Flip() || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Confused);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                })
                .After("Gas Bomb", (c, a) => { if (c.InPlay(c.Pokemon)) c.DamageSelf(30); }));
            r.RegisterByKey("RR-69", new CardsSS.BabyEvolutionMon("Snorlax", new Fx().Effect("Rest", (c, a) =>
                {
                    c.Engine.RemoveAllConditions(c.Pokemon);
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 60);
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("RR-70", new CardsSS.BabyEvolutionMon("Snorlax", new Fx()
                .Effect("Errand-Running", (c, a) => c.SearchToHand(CardsDPEra.DpTrainer, 1, "Errand-Running: search your deck for a Trainer card"))));
            r.RegisterByKey("RR-71", new SearchAttackMon("Offer Help", d => d.IsSupporter, 1));
            r.RegisterByKey("RR-72", new Fx()
                .Effect("Leer", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.Flip() && c.CanAffect(d)) c.AddEffect(new CantAttackEffect(d.Name + " can't attack (Leer)", c.Player, d.Uid, c.Turn + 1));
                })
                .With(new TailsNothingMon("Horn Hazard")));
            r.RegisterByKey("RR-73", new Fx()
                .After("Jump Tackle", (c, a) => { if (c.InPlay(c.Pokemon)) c.DamageSelf(10); })
                .On("Stress Poison", (c, a) =>
                {
                    bool hurt = c.Pokemon.DamageCounters > 0;
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (hurt ? 20 : 0));
                    if (hurt && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                    return true;
                }));
            r.RegisterByKey("RR-74", new Fx()
                .Effect("Toxic", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d) && c.ApplyCondition(d, SpecialCondition.Poisoned)) d.PoisonCounterBase = Math.Max(d.PoisonCounterBase, 2);
                })
                .Effect("Frustration", (c, a) => HitAny(c, 40, "Frustration", m => m.DamageCounters == 0)));
            r.RegisterByKey("RR-75", new Fx()
                .After("Blind", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Blind)", c.Player, d.Uid, c.Turn + 1));
                })
                .Effect("Feint Attack", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Feint Attack: choose 1 of your opponent's Pokémon");
                    if (t != null) c.DamageWith(t, 30, false, false, true);
                }));
            r.RegisterByKey("RR-76", new QuagsireSubmerge());
            r.RegisterByKey("RR-77", new Fx()
                .Effect("Find Ice", (c, a) => c.SearchToHand(d => CardsRS.BasicOf(d, EnergyType.Water), 2, "Find Ice: search your deck for up to 2 Water Energy cards"))
                .After("Sheer Cold", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.Flip() && c.CanAffect(d)) c.AddEffect(new CantAttackEffect(d.Name + " can't attack (Sheer Cold)", c.Player, d.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("RR-78", new ComboMon(new CardsDPEra.HardenMon("Harden", 40), new CardsDPEra.AstonishMon("Astonish")));
            r.RegisterByKey("RR-79", new Fx()
                .Effect("Ripple", (c, a) =>
                {
                    if (!c.Flip()) return;
                    foreach (var m in c.OpponentPokemon().ToList()) if (c.CanAffect(m)) c.PlaceCounters(m, 1);
                })
                .Effect("Brine", (c, a) => HitAny(c, 20, "Brine", m => m.DamageCounters > 0)));
            r.RegisterByKey("RR-80", new Fx().Effect("Healing Sea", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                    if (heads == 0 || hurt.Count == 0) return;
                    var t = c.ChoosePokemon(hurt, "Healing Sea: remove damage counters from which Pokémon?");
                    if (t != null) c.Heal(t, 10 * heads);
                }));
            r.RegisterByKey("RR-81", new SnorlaxBadSleepingHabits());
            r.RegisterByKey("RR-82", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("RR-83", new Fx()
                .Effect("Cosmic Draw", (c, a) => { if (c.OpponentPokemon(CardsE2.IsEvolved).Count > 0) c.Draw(3); })
                .Effect("Swift", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, true); }));
            r.RegisterByKey("RR-84", new PowerFx("Gather Sand", c => c.PokemonIsActive && c.Me.Discard.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Fighting)), c =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fighting)).ToList();
                    var card = c.ChooseCards(pile, 1, 1, "Gather Sand: attach which Fighting Energy card?")[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                }, new Fx().Damage("Grind", (c, a) => 10 * c.EnergyCount(c.Pokemon)), true));
            r.RegisterByKey("RR-85", new TurtwigOvergrow());
            r.RegisterByKey("RR-86", new Fx()
                .Effect("Rescue String", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsPokemon).ToList();
                    if (pile.Count == 0) return;
                    var picks = c.ChooseCards(pile, 1, Math.Min(5, pile.Count), "Rescue String: shuffle up to 5 Pokémon from your discard pile into your deck");
                    c.DiscardToDeck(picks);
                })
                .With(new FlipBonusMon("Needling Sting", 10)));
            r.RegisterByKey("RR-87", new WeezingCamouflageGas());
            r.RegisterByKey("RR-88", new AaronsCollection());
            r.RegisterByKey("RR-90", new BerthasWarmth());
            r.RegisterByKey("RR-91", new FlintsWillpower());
            r.RegisterByKey("RR-92", new LuciansAssignment());
            r.RegisterByKey("RR-93", new PokemonContestHall());
            r.RegisterByKey("RR-94", new SunyshoreCityGym());
            r.RegisterByKey("RR-95", new TechnicalMachineG());
            r.RegisterByKey("RR-96", new SpRadar());
            r.RegisterByKey("RR-98", new VolknersPhilosophy());
            r.RegisterByKey("RR-101", new SpEnergy());
            r.RegisterByKey("RR-102", new UpperEnergy());
        }

        /// <summary>"If this Pokémon evolved from Eevee during this turn, 20 more damage and the Defending Pokémon is now [condition]."</summary>
        private static bool EvolvingBonus(EffectContext c, AttackDef a, SpecialCondition condition)
        {
            bool now = CardsP9.JustEvolvedFrom(c, "Eevee");
            var d = c.Defending;
            c.DamageDefending(a.Damage + (now ? 20 : 0));
            if (now && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, condition);
            return true;
        }

        /// <summary>Sticky Hold (Poké-Body): when it's switched or retreats to the Bench, move any of its Energy cards to the new Active Pokémon. Calling Wave.</summary>
        private sealed class GastrodonStickyHold : BodyMon
        {
            public GastrodonStickyHold() : base(new Fx().Effect("Calling Wave", (c, a) =>
                {
                    int n = Math.Min(2, c.BenchSpace);
                    if (n <= 0) return;
                    var picks = c.SearchMyDeck(d => d.IsPokemon && d.Name.StartsWith("Gastrodon"), n, "Calling Wave: search your deck for up to 2 Gastrodon");
                    foreach (var card in picks)
                    {
                        var mon = c.Engine.PutOntoBench(c.Player, card, false);
                        if (mon != null) c.Engine.PlaceDamageCounters(mon, 2, "Calling Wave");
                    }
                    c.ShuffleMyDeck();
                })) { }

            public override void OnLeftActiveSpot(EffectContext ctx, PokemonInPlay newActive)
            {
                var me = ctx.Pokemon;
                if (me == null || newActive == null || me.Energy.Count == 0 || !Works(ctx, "Sticky Hold")) return;
                var picks = ctx.ChooseCards(me.Energy.ToList(), 0, me.Energy.Count, "Sticky Hold: move which Energy cards to " + newActive.Name + "?");
                foreach (var e in picks) ctx.Engine.MoveEnergy(me, newActive, e);
            }
        }

        /// <summary>Sand Cover (Poké-Body): while Hippowdon is your Active Pokémon, 1 damage counter on each of the opponent's Pokémon LV.X between turns. Save Sand. Groundquake.</summary>
        private sealed class HippowdonSandCover : BodyMon
        {
            public HippowdonSandCover() : base(new Fx()
                .On("Save Sand", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fighting)).ToList();
                    if (pile.Count > 0 && c.AskYesNo("Save Sand: attach a Fighting Energy card from your discard pile to " + c.Pokemon.Name + " first?"))
                    {
                        var card = c.ChooseCards(pile, 1, 1, "Save Sand: attach which Fighting Energy card?")[0];
                        if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                    }
                    c.DamageDefending(a.Damage + 10 * c.EnergyCount(c.Pokemon));
                    return true;
                })
                .After("Groundquake", (c, a) =>
                {
                    foreach (var m in c.MyBench().Concat(c.OpponentBench()).Where(m => !CardsE2.IsEvolved(m)).ToList()) if (c.InPlay(m)) c.DamageTarget(m, 10);
                })) { }

            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? src.Opponent : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (!src.PokemonIsActive || !Works(src, "Sand Cover")) return;
                foreach (var m in src.OpponentPokemon(x => x.Def.IsLevelUp).ToList()) src.Engine.PlaceDamageCounters(m, 1, "Sand Cover");
            }
        }

        /// <summary>Icy Aura (Poké-Body): while Mamoswine GL is your Active Pokémon, 1 damage counter on each Active Pokémon that isn't Water between turns. Avalanche.</summary>
        private sealed class MamoswineIcyAura : BodyMon
        {
            public MamoswineIcyAura() : base(new Fx().After("Avalanche", (c, a) => { if (c.Flip()) HitEachBenched(c, 10); })) { }

            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? -1 : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (!src.PokemonIsActive || !Works(src, "Icy Aura")) return;
                foreach (var m in new[] { src.Me.Active, src.Opp.Active })
                    if (m != null && !src.Engine.IsType(m, EnergyType.Water)) src.Engine.PlaceDamageCounters(m, 1, "Icy Aura");
            }
        }

        /// <summary>Territoriality (Poké-Body): when your Active Pokémon is damaged by an opponent's attack, 2 damage counters on the attacker (only 2 in all). Fling Away. Giga Horn.</summary>
        private sealed class NidokingTerritoriality : BodyMon
        {
            public NidokingTerritoriality() : base(new Fx()
                .On("Fling Away", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) { c.DamageDefending(a.Damage); return true; }
                    c.DamageDefending(30);
                    HitBenched(c, 1, 30, "Fling Away");
                    return true;
                })
                .On("Giga Horn", (c, a) =>
                {
                    if (c.FlipCoins(2) == 0) return DidNothing(c);
                    c.DamageDefending(a.Damage);
                    return true;
                })) { }

            public override void OnOwnPokemonDamagedByAttack(EffectContext src, PokemonInPlay target, PokemonInPlay attacker, bool targetWasActive, int damage)
            {
                if (!targetWasActive || attacker == null || attacker.Owner == src.Player || damage <= 0 || !src.InPlay(attacker)) return;
                if (!Works(src, "Territoriality") || !CardsCRZ1.IsLeadSource(src, "Territoriality")) return;
                Announce(src, "Territoriality");
                src.Engine.PlaceDamageCounters(attacker, 2, "Territoriality");
            }
        }

        /// <summary>Maternal Comfort (Poké-Body): between turns, remove 1 damage counter from each of your Pokémon (only 1 Maternal Comfort). Ruthless Tail.</summary>
        private sealed class NidoqueenMaternalComfort : BodyMon
        {
            public NidoqueenMaternalComfort() : base(new Fx().Damage("Ruthless Tail", (c, a) => a.Damage + 10 * c.OpponentBench().Count)) { }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (!Works(src, "Maternal Comfort") || !CardsCRZ1.IsLeadSource(src, "Maternal Comfort")) return;
                var hurt = src.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                Announce(src, "Maternal Comfort");
                foreach (var m in hurt) src.Heal(m, 10);
            }
        }

        /// <summary>Royal Gain (Poké-Body): when you attach a Grass Energy card from your hand to it, remove 1 damage counter from it. Leaf Needle.</summary>
        private sealed class VespiquenRoyalGain : BodyMon
        {
            public VespiquenRoyalGain() : base(new Fx()
                .Damage("Leaf Needle", (c, a) => a.Damage + 20 * c.FlipCoins(c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Grass)))) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || !CardsRS.BasicOf(energy.Def, EnergyType.Grass) || target.DamageCounters == 0 || !Works(src, "Royal Gain")) return;
                src.Heal(target, 10);
            }
        }

        /// <summary>Frost Wind (Poké-Body): while Glaceon is your Active Pokémon, attack damage to your Pokémon from the opponent is 10 less. Ice Blade. Ice Bind.</summary>
        private sealed class GlaceonFrostWind : BodyMon
        {
            public GlaceonFrostWind() : base(new Fx()
                .Effect("Ice Blade", (c, a) => HitAny(c, 30, "Ice Blade"))
                .After("Ice Bind", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d)) return;
                    if (c.Opp.Hand.Count > 0 && c.Engine.AskYesNo(c.Opponent, "Ice Bind: discard a card from your hand (or your Active Pokémon is Paralyzed)?", c.Source))
                    {
                        var card = c.Engine.ChooseCards(c.Opponent, c.Opp.Hand.ToList(), 1, 1, "Ice Bind: discard which card?", c.Source)[0];
                        c.Engine.DiscardFromHand(c.Opponent, card);
                        return;
                    }
                    if (c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                })) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Owner != src.Player || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !src.PokemonIsActive || !Works(src, "Frost Wind")) return damage;
                return Math.Max(0, damage - 10);
            }
        }

        /// <summary>Sand Armor (Poké-Body): with any Fighting Energy attached, attack damage to it is 10 less. Bite and Crush.</summary>
        private sealed class HippowdonSandArmor : BodyMon
        {
            public HippowdonSandArmor() : base(new Fx().Damage("Bite and Crush", (c, a) => a.Damage + 10 * c.FlipCoins(2))) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (info.Target != me || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!me.Energy.Any(e => CardsDRI1.ProvidesType(src, me, e, EnergyType.Fighting)) || !Works(src, "Sand Armor")) return damage;
                return Math.Max(0, damage - 10);
            }
        }

        /// <summary>Energy Refresh (Poké-Body): while Leafeon is your Active Pokémon, attaching an Energy card from your hand removes 2 damage counters from that Pokémon. Plus Energy.</summary>
        private sealed class LeafeonEnergyRefresh : BodyMon
        {
            public LeafeonEnergyRefresh() : base(new Fx()
                .After("Plus Energy", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsBasicEnergy).ToList();
                    if (energy.Count > 0) c.AttachCardsAnyWay(energy, 1, (x, m) => true, "Plus Energy: attach a basic Energy card from your hand", true);
                })
                .With(new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep))) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target.Owner != src.Player || target.DamageCounters == 0 || !src.PokemonIsActive || !Works(src, "Energy Refresh")) return;
                src.Heal(target, 20);
            }
        }

        /// <summary>Energy Shark (Poké-Body): when your Active Sharpedo is damaged by an opponent's attack, flip; heads, discard an Energy card from the attacker. Crunch.</summary>
        private sealed class SharpedoEnergyShark : BodyMon
        {
            public SharpedoEnergyShark() : base(new Fx()
                .After("Crunch", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new TakenChangeEffect(d.Name + " takes 40 more damage from attacks until the end of your next turn (Crunch)", c.Player, d.Uid, c.Turn + 2, 40));
                })) { }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || attacker.Energy.Count == 0) return;
                if (!ctx.Flip()) return;
                Announce(ctx, "Energy Shark");
                ctx.DiscardEnergyFrom(attacker, 1, "Energy Shark: discard an Energy card attached to " + attacker.Name);
            }
        }

        /// <summary>Exoskeleton (Poké-Body): attack damage to it is 20 less. Dangerous Evolution.</summary>
        private sealed class KakunaExoskeleton : BodyMon
        {
            public KakunaExoskeleton() : base(new Fx().Effect("Dangerous Evolution", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                    if (c.Flip()) EvolveFromDeck(c, c.Pokemon, x => !x.IsLevelUp, "Dangerous Evolution: search your deck for a card that evolves from " + c.Pokemon.Name);
                })) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !Works(src, "Exoskeleton")) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>Colorful Body (Poké-Body): Kecleon is every type. Triple Smash.</summary>
        private sealed class KecleonColorfulBody : BodyMon
        {
            private static readonly EnergyType[] All = { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Colorless };

            public KecleonColorfulBody() : base(new Fx().Damage("Triple Smash", (c, a) => a.Damage + 20 * c.FlipCoins(3))) { }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon != src.Pokemon || !Works(src, "Colorful Body")) return;
                foreach (var t in All) if (!types.Contains(t)) types.Add(t);
            }
        }

        /// <summary>Submerge (Poké-Body): on your Bench, prevent all damage done to it by attacks. Punch and Run.</summary>
        private sealed class QuagsireSubmerge : BodyMon
        {
            public QuagsireSubmerge() : base(new SwitchSelfMon("Punch and Run", true)) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && src.Me.Bench.Contains(src.Pokemon) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Submerge");
            }
        }

        /// <summary>Bad Sleeping Habits (Poké-Body): while Snorlax is Asleep, the opponent's Active Pokémon can't retreat. Toss and Turn (usable while Asleep). Heavy Press.</summary>
        private sealed class SnorlaxBadSleepingHabits : BodyMon
        {
            public SnorlaxBadSleepingHabits() : base(new Fx()
                .Damage("Toss and Turn", (c, a) => a.Damage + (c.Pokemon.Has(SpecialCondition.Asleep) ? 30 : 0))
                .On("Heavy Press", (c, a) =>
                {
                    bool heads = c.Flip();
                    c.DamageDefending(a.Damage + (heads ? 40 : 0));
                    if (!heads && c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                    return true;
                })) { }

            public override bool UsableWhileAsleep(AttackDef attack) { return attack != null && attack.Name == "Toss and Turn"; }

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner != src.Player && src.Opp.Active == mon && src.Pokemon != null && src.Pokemon.Has(SpecialCondition.Asleep) && Works(src, "Bad Sleeping Habits");
            }
        }

        /// <summary>Overgrow (Poké-Body): with 60 HP or less left, its attacks do 30 more damage to the Defending Pokémon. Giga Drain.</summary>
        private sealed class TurtwigOvergrow : BodyMon
        {
            public TurtwigOvergrow() : base(new Fx().Effect("Giga Drain", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    if (done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
                })) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (info.Target == null || info.Target != src.Opp.Active || src.RemainingHp(src.Pokemon) > 60 || !Works(src, "Overgrow")) return damage;
                return damage + 30;
            }
        }

        /// <summary>Camouflage Gas (Poké-Body): if Weezing is Confused and Knocked Out, the opponent takes no Prize card. Damage Breakdown. Smog.</summary>
        private sealed class WeezingCamouflageGas : BodyMon
        {
            public WeezingCamouflageGas() : base(new Fx()
                .Effect("Damage Breakdown", (c, a) =>
                {
                    var d = c.Defending;
                    int n = c.Pokemon.DamageCounters;
                    if (d != null && n > 0 && c.CanAffect(d)) c.PlaceCounters(d, n);
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                })
                .With(new ConditionAttackMon("Smog", SpecialCondition.Poisoned))) { }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return knockedOut == src.Pokemon && knockedOut.Has(SpecialCondition.Confused) && Works(src, "Camouflage Gas") ? 0 : prizes;
            }
        }

        // ------------------------------------------------------------------ Trainers and Energy

        /// <summary>Aaron's Collection (Supporter): up to 2 Pokémon SP and basic Energy cards from the discard pile into your hand.</summary>
        private sealed class AaronsCollection : CardScript
        {
            private static bool Fits(CardDef d) { return (d.IsPokemon && d.IsPokemonSP) || d.IsBasicEnergy; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => Fits(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => Fits(c.Def)).ToList();
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Aaron's Collection: put up to 2 Pokémon SP and basic Energy cards into your hand"));
            }
        }

        /// <summary>Bertha's Warmth (Supporter): remove 5 damage counters from 1 of your Pokémon SP.</summary>
        private sealed class BerthasWarmth : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => SP(m) && m.DamageCounters > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var t = ctx.ChoosePokemon(ctx.MyPokemon(m => SP(m) && m.DamageCounters > 0), "Bertha's Warmth: remove 5 damage counters from which Pokémon SP?");
                if (t != null) ctx.Heal(t, 50);
            }
        }

        /// <summary>Flint's Willpower (Supporter): attach a basic Energy card from your hand to 1 of your Pokémon SP.</summary>
        private sealed class FlintsWillpower : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(SP).Count > 0 && ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsBasicEnergy); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                ctx.AttachCardsAnyWay(energy, 1, (c, m) => SP(m), "Flint's Willpower: attach a basic Energy card to 1 of your Pokémon SP", false);
            }
        }

        /// <summary>Lucian's Assignment (Supporter): move as many Energy cards from 1 of your Pokémon as you like to another.</summary>
        private sealed class LuciansAssignment : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.InPlay().Count() > 1 && ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Energy.Count > 0), "Lucian's Assignment: move Energy cards from which Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Lucian's Assignment: move them to which Pokémon?");
                if (to == null) return;
                foreach (var e in ctx.ChooseCards(from.Energy.ToList(), 1, from.Energy.Count, "Lucian's Assignment: move which Energy cards?")) ctx.Engine.MoveEnergy(from, to, e);
            }
        }

        /// <summary>
        /// Pokémon Contest Hall (Stadium): once during each player's turn, if their Bench isn't full, they may flip; heads, a Basic
        /// Pokémon from their deck onto the Bench, and they may attach a Pokémon Tool from their deck to it.
        /// </summary>
        private sealed class PokemonContestHall : CardScript
        {
            private const string Limit = "Pokémon Contest Hall";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return true;
                var benched = ctx.SearchToBench(d => d.IsBasicPokemon, 1, "Pokémon Contest Hall: search your deck for a Basic Pokémon");
                if (benched.Count == 0) return true;
                var mon = ctx.MyPokemon(m => m.Top == benched[0]).FirstOrDefault();
                if (mon == null || !ctx.Me.Deck.Any(c => c.Def.IsTool) || !ctx.AskYesNo("Pokémon Contest Hall: search your deck for a Pokémon Tool to attach to " + mon.Name + "?")) return true;
                var tools = ctx.SearchMyDeck(d => d.IsTool, 1, "Pokémon Contest Hall: search your deck for a Pokémon Tool");
                if (tools.Count > 0)
                {
                    if (ctx.Engine.CanAttachToolTo(tools[0], mon)) ctx.Engine.AttachTool(tools[0], mon);
                    else ctx.Me.Deck.Add(tools[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Sunyshore City Gym (Stadium): Lightning Pokémon's attack damage ignores Resistance; Lightning Pokémon have no Weakness.</summary>
        private sealed class SunyshoreCityGym : CardScript
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                return info.Attacker != null && src.Engine.IsType(info.Attacker, EnergyType.Lightning) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && mon.Def.HasType(EnergyType.Lightning); }
        }

        /// <summary>G-107 Technical Machine G: attach to 1 of your Pokémon SP; it may use Damage Porter.</summary>
        private sealed class TechnicalMachineG : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !SP(mon) || src.Card.Def.Attacks.Count == 0) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override bool IsAttachedNonTool { get { return true; } }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(SP).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(SP), "Technical Machine G: attach it to which of your Pokémon SP?");
                if (target == null) return;
                ctx.CardPlacedElsewhere = true;
                ctx.Engine.AttachTool(ctx.Card, target);
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                int n = ctx.Pokemon.DamageCounters;
                HitAny(ctx, 10 * n, "Damage Porter");
            }
        }

        /// <summary>G-109 SP Radar: put a card from your hand on top of your deck, then search your deck for a Pokémon SP.</summary>
        private sealed class SpRadar : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.Where(c => c != ctx.Card).ToList(), 1, 1, "SP Radar: put which card from your hand on top of your deck?")[0];
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Insert(0, card);
                ctx.SearchToHand(d => d.IsPokemon && d.IsPokemonSP, 1, "SP Radar: search your deck for a Pokémon SP");
            }
        }

        /// <summary>Volkner's Philosophy (Supporter): you may discard a card from your hand, then draw until you have 6.</summary>
        private sealed class VolknersPhilosophy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                int hand = ctx.Me.Hand.Count(c => c != ctx.Card);
                return ctx.Me.Deck.Count > 0 && (hand < 6 || (hand == 6));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count > 0 && (ctx.Me.Hand.Count >= 6 || ctx.AskYesNo("Volkner's Philosophy: discard a card from your hand first?")))
                    ctx.Engine.DiscardFromHand(ctx.Player, ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Volkner's Philosophy: discard which card?")[0]);
                ctx.DrawUntil(6);
            }
        }

        /// <summary>SP Energy: [C]; on a Pokémon SP, every type but only 1 Energy at a time.</summary>
        private sealed class SpEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return new List<int> { SP(ctx.Pokemon) ? EnergyMask.AnyType : EnergyMask.Colorless };
            }
        }

        /// <summary>Upper Energy: [C]; [C][C] while you have more Prize cards left than the opponent, on a Pokémon that isn't LV.X.</summary>
        private sealed class UpperEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                bool more = mon != null && !mon.Def.IsLevelUp && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count;
                return more ? new List<int> { EnergyMask.Colorless, EnergyMask.Colorless } : new List<int> { EnergyMask.Colorless };
            }
        }
    }
}
