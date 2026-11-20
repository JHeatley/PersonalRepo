using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>HS—Triumphant (TM, 2010; v173): the cards not already in the pool (the others are prints). Unlimited only.</summary>
    public static partial class CardsTM
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TM-1", new Fx()
                .Damage("Second Strike", (c, a) => a.Damage + (c.Defending != null && c.Defending.DamageCounters > 0 ? 40 : 0))
                .With(new ShieldAttackMon("Guard Claw", 20)));
            r.RegisterByKey("TM-2", new Fx()
                .With(new ConditionAttackMon("Midnight Eyes", SpecialCondition.Asleep))
                .Damage("Stadium Power", (c, a) => a.Damage + (c.State.Stadium != null ? 30 : 0)));
            r.RegisterByKey("TM-3", new ComboMon(new CardsSW.FutureSightMon(), new ParalyzeFlipMon("Leaf Bind")));
            r.RegisterByKey("TM-4", new Fx().After("Toxic Fang", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.Flip() && c.CanAffect(d) && c.ApplyCondition(d, SpecialCondition.Poisoned)) d.PoisonCounterBase = Math.Max(d.PoisonCounterBase, 2);
                }));
            r.RegisterByKey("TM-5", new Fx()
                .With(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep))
                .After("Snowstorm", (c, a) => HitEachBenched(c, 20, m => m.DamageCounters > 0)));
            r.RegisterByKey("TM-6", new NidokingPheromoneStamina());
            r.RegisterByKey("TM-7", new PowerFx("Dimension Transfer", c => c.Me.Discard.Any(x => x.Def.IsTrainer), c =>
                {
                    if (!c.Flip()) return;
                    var pile = c.Me.Discard.Where(x => x.Def.IsTrainer).ToList();
                    var card = c.ChooseCards(pile, 1, 1, "Dimension Transfer: put which Trainer card on top of your deck?")[0];
                    c.Me.Discard.Remove(card);
                    c.Me.Deck.Insert(0, card);
                    c.Engine.ShowCards(c.Opponent, new List<CardInstance> { card }, "Dimension Transfer", "Dimension Transfer: the card your opponent put on top of their deck", c.Source);
                }, new Fx().After("Suspicious Beam β", (c, a) =>
                {
                    if (!c.InPlay(c.Pokemon) || c.Pokemon.Energy.Any(e => e.Def.Name == "Rainbow Energy")) return;
                    c.DamageSelf(20);
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                })));
            r.RegisterByKey("TM-8", new RapidashFierySpirit());
            r.RegisterByKey("TM-9", new SolrockHealBlock());
            r.RegisterByKey("TM-10", new BenchTriggerFx("Spooky Whirlpool", null, c => ShuffleHandDraw(c, c.Opponent, 6), new Fx()
                .Effect("Color Tag", (c, a) =>
                {
                    var types = new[] { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Colorless };
                    var t = types[c.ChooseOption("Color Tag: choose a type", types.Select(x => x.ToString()).ToArray())];
                    foreach (var m in c.OpponentPokemon(x => c.Engine.IsType(x, t)).ToList()) if (c.CanAffect(m)) c.PlaceCounters(m, 1);
                })));
            r.RegisterByKey("TM-11", new PowerFx("Poison Moth Wind", c => c.Opp.Active != null && c.Me.Active != null, c =>
                {
                    var t = c.Flip() ? c.Opp.Active : c.Me.Active;
                    if (t != null && (t.Owner == c.Player || c.CanAffect(t))) c.ApplyCondition(t, SpecialCondition.Poisoned);
                }, new ParalyzeFlipMon("Stun Spore")));
            r.RegisterByKey("TM-12", new VictreebelTanglingTendrils());
            r.RegisterByKey("TM-13", new Fx()
                .After("Astonish", (c, a) =>
                {
                    var picks = new List<CardInstance>();
                    for (int i = 0; i < 2 && c.Opp.Hand.Count > picks.Count; i++)
                    {
                        var rest = c.Opp.Hand.Where(x => !picks.Contains(x)).ToList();
                        picks.Add(rest[c.State.Rng.Next(rest.Count)]);
                    }
                    if (picks.Count == 0) return;
                    c.Engine.ShowCards(c.Player, picks, "Astonish", "Astonish: the cards you chose", c.Source);
                    foreach (var x in picks) { c.Opp.Hand.Remove(x); c.Opp.Deck.Add(x); }
                    c.Engine.ShuffleDeck(c.Opponent);
                })
                .On("Tail Spank", (c, a) =>
                {
                    if (c.Me.Hand.Count < 2) return DidNothing(c);
                    foreach (var x in c.ChooseCards(c.Me.Hand.ToList(), 2, 2, "Tail Spank: discard 2 cards from your hand")) c.Engine.DiscardFromHand(c.Player, x);
                    c.DamageDefending(a.Damage);
                    return true;
                }));
            r.RegisterByKey("TM-14", new Fx()
                .Effect("Lost Crush", (c, a) =>
                {
                    var pool = c.OpponentPokemon(m => m.Energy.Count > 0 && c.CanAffect(m));
                    if (pool.Count == 0 || !c.Flip()) return;
                    var t = c.ChoosePokemon(pool, "Lost Crush: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    var e = t.Energy.Count == 1 ? t.Energy[0] : c.ChooseCards(t.Energy.ToList(), 1, 1, "Lost Crush: put which Energy card in the Lost Zone?")[0];
                    EnergyToLostZone(c, t, e);
                })
                .Effect("Breakdown", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.Opp.Hand.Count > 0 && c.CanAffect(d)) c.PlaceCounters(d, c.Opp.Hand.Count);
                }));
            r.RegisterByKey("TM-15", new Fx()
                .Effect("Legend Ceremony", (c, a) =>
                {
                    var tops = c.Me.Deck.Where(x => x.Def.IsLegend && x.Def.LegendHalf == "top" && c.Me.Deck.Any(y => y.Def.IsLegend && y.Def.LegendHalf == "bottom" && y.Def.Name == x.Def.Name)).ToList();
                    var first = c.SearchMyDeck(d => d.IsLegend && d.LegendHalf == "top" && tops.Any(t => t.Def == d), 1, "Legend Ceremony: search your deck for both halves of a Pokémon LEGEND");
                    if (first.Count > 0)
                    {
                        var second = c.Me.Deck.FirstOrDefault(y => y.Def.IsLegend && y.Def.LegendHalf == "bottom" && y.Def.Name == first[0].Def.Name);
                        c.Me.Hand.Add(first[0]);
                        if (second != null) { c.Me.Deck.Remove(second); c.Me.Hand.Add(second); }
                    }
                    c.ShuffleMyDeck();
                })
                .After("Reflect Energy", (c, a) => EnergyToBench(c, "Reflect Energy")));
            r.RegisterByKey("TM-16", new Fx()
                .Effect("Saliva Lure", (c, a) => { if (c.OpponentBench().Count > 0) c.GustOpponent("Saliva Lure: switch in which of your opponent's Benched Pokémon?"); })
                .After("Stick and Absorb", (c, a) => StickAndAbsorb(c, 30)));
            r.RegisterByKey("TM-17", new DittoDittobolic());
            r.RegisterByKey("TM-18", new Fx()
                .After("Calming Wind", (c, a) => { if (c.InPlay(c.Pokemon)) c.Engine.RemoveAllConditions(c.Pokemon); })
                .On("Dragon Stamp", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads == 0) return DidNothing(c);
                    var d = c.Defending;
                    c.DamageDefending(a.Damage);
                    if (heads == 2 && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    return true;
                }));
            r.RegisterByKey("TM-19", new Fx().Damage("Sand Impact", (c, a) => a.Damage + 20 * c.FlipCoins(c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fighting))));
            r.RegisterByKey("TM-20", new Fx()
                .After("Plasma", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Lightning)).ToList();
                    if (pile.Count == 0 || !c.InPlay(c.Pokemon)) return;
                    var card = pile[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .Effect("Thunder Shot", (c, a) => { foreach (var m in c.OpponentPokemon(x => x.Energy.Count > 0).ToList()) c.DamageTarget(m, 50); }));
            r.RegisterByKey("TM-21", new SweetSleepingFaceMon(new Fx().Effect("Sparking Ball", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Sparking Ball: choose 1 of your opponent's Pokémon");
                    if (t != null) c.DamageWith(t, 20, false, false, false);
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("TM-22", new GolduckNaturalRemedy());
            r.RegisterByKey("TM-23", new Fx()
                .After("Psychic Lock", (c, a) => c.AddEffect(new CardsSW.PokePowerLockEffect(c.Engine.PlayerName(c.Opponent) + " can't use Poké-Powers during their next turn (Psychic Lock)", c.Player, c.Opponent, c.Turn + 1)))
                .Effect("Bench Manipulation", (c, a) =>
                {
                    int tails = 0;
                    for (int i = 0; i < c.Opp.Bench.Count; i++) if (!c.Engine.FlipCoin(c.Opponent, "Bench Manipulation")) tails++;
                    if (tails > 0 && c.Defending != null) c.DamageWith(c.Defending, 40 * tails, false, false, false);
                }));
            r.RegisterByKey("TM-24", new Fx()
                .With(new CardsPAR2.FlipConditionMon("Entrancing Melody", SpecialCondition.Confused))
                .Damage("Fury Cutter", (c, a) =>
                {
                    int heads = c.FlipCoins(3);
                    return a.Damage + (heads == 1 ? 20 : heads == 2 ? 40 : heads == 3 ? 100 : 0);
                }));
            r.RegisterByKey("TM-25", new Fx().After("Selfdestruct", (c, a) =>
                {
                    foreach (var m in c.MyBench().Concat(c.OpponentBench()).ToList()) if (c.InPlay(m)) c.DamageTarget(m, 10);
                    if (c.InPlay(c.Pokemon)) c.DamageSelf(60);
                }));
            r.RegisterByKey("TM-26", new Fx()
                .On("Vital Throw", (c, a) => MayRecoil(c, a, 20, 20))
                .Damage("Hundred Furious Punches", (c, a) => a.Damage + 10 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fighting)));
            r.RegisterByKey("TM-27", new Fx()
                .Effect("Top Burner", (c, a) =>
                {
                    int n = c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fire);
                    if (n > 0) c.Engine.DiscardTopOfDeck(c.Opponent, n);
                    if (!c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 99);
                })
                .With(new ConditionAttackMon("Burst Punch", SpecialCondition.Burned)));
            r.RegisterByKey("TM-28", new Fx()
                .After("Return", (c, a) => c.DrawUntil(6))
                .Damage("Prize Count", (c, a) => a.Damage + (c.Me.Prizes.Count > c.Opp.Prizes.Count ? 30 : 0)));
            r.RegisterByKey("TM-29", new Fx()
                .After("Headwind", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new CardsHS.CostUpEffect(d.Name + "'s attacks cost [C][C] more (Headwind)", c.Player, d.Uid, c.Turn + 1, 2));
                })
                .With(new FlipBonusMon("Quick Attack", 30)));
            r.RegisterByKey("TM-30", new Fx()
                .After("Strip Bare", (c, a) =>
                {
                    if (c.FlipCoins(2) < 2) return;
                    foreach (var x in c.Opp.Hand.ToList()) c.Engine.DiscardFromHand(c.Opponent, x);
                })
                .Damage("Rage", (c, a) => a.Damage + 10 * c.Pokemon.DamageCounters));
            r.RegisterByKey("TM-31", new Fx()
                .Effect("Underwater Dive", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads > 0 && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 30 * heads);
                })
                .Damage("Swallow Up", (c, a) => a.Damage + (c.Defending != null && c.RemainingHp(c.Defending) < c.RemainingHp(c.Pokemon) ? 50 : 0)));
            r.RegisterByKey("TM-32", new ComboMon(new SearchAttackMon("Search and Invite", d => d.IsPokemon, 2), new CoinsMon("Slam", 2)));
            r.RegisterByKey("TM-33", new Fx().After("Shock Bolt", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Lightning, 99); }));
            r.RegisterByKey("TM-34", new Fx().On("Lightning Strike", (c, a) =>
                {
                    bool more = c.AskYesNo("Lightning Strike: do 60 more damage and discard all Lightning Energy?");
                    c.DamageDefending(a.Damage + (more ? 60 : 0));
                    if (more && c.InPlay(c.Pokemon)) DiscardTypeFromSelf(c, EnergyType.Lightning, 99);
                    return true;
                }));
            r.RegisterByKey("TM-35", new Fx()
                .Effect("Sneaky Placement", (c, a) => SneakyPlacement(c, 2))
                .Effect("Sleep Poison", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Asleep);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                }));
            r.RegisterByKey("TM-36", new ComboMon(new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused), new CoinsMon("Dizzy Punch", 2)));
            r.RegisterByKey("TM-37", new RecoilMon("Take Down", 20));
            r.RegisterByKey("TM-38", new Fx()
                .Effect("Licking Shot", (c, a) =>
                {
                    int n = c.EnergyCount(c.Pokemon);
                    if (n > 0) HitAny(c, 10 * n, "Licking Shot");
                })
                .After("Stick and Absorb", (c, a) => StickAndAbsorb(c, 20)));
            r.RegisterByKey("TM-39", new Fx()
                .Effect("Rendezvous Draw", (c, a) =>
                {
                    foreach (int p in new[] { c.Player, c.Opponent })
                    {
                        var deck = c.State.Players[p].Deck;
                        if (deck.Count == 0) continue;
                        c.Log(c.Engine.PlayerName(p) + " drew and revealed " + deck[0].Def.Name + " (Rendezvous Draw).");
                        c.Engine.DrawCards(p, 1);
                    }
                })
                .Before("Distorted Wave", (c, a) => { if (c.Defending != null && c.Defending.DamageCounters > 0) c.Heal(c.Defending, 10); }));
            r.RegisterByKey("TM-40", new Fx().Effect("Knuckle Down", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, true, true); }));
            r.RegisterByKey("TM-41", new SweetSleepingFaceMon(new Fx().Effect("Play with Fire", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Burned);
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("TM-42", new Fx().Effect("Eruption", (c, a) =>
                {
                    int n = c.Engine.DiscardTopOfDeck(c.Player, 1).Count(x => x.Def.IsEnergy) + c.Engine.DiscardTopOfDeck(c.Opponent, 1).Count(x => x.Def.IsEnergy);
                    c.DamageDefending(20 * n);
                }));
            r.RegisterByKey("TM-43", new CoinsMon("Tri Attack", 3));
            r.RegisterByKey("TM-44", new Fx()
                .With(new CoinsMon("Bonemerang", 2))
                .On("Bone Impact", (c, a) =>
                {
                    bool stadium = c.State.Stadium != null;
                    c.DamageDefending(a.Damage + (stadium ? 60 : 0));
                    if (stadium && c.State.Stadium != null) c.Engine.DiscardStadium();
                    return true;
                }));
            r.RegisterByKey("TM-45", new FlipBonusMon("Quick Blow", 10));
            r.RegisterByKey("TM-46", new TailsNothingMon("Lunge"));
            r.RegisterByKey("TM-47", new Fx().On("Twister", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads == 0) return DidNothing(c);
                    var d = c.Defending;
                    c.DamageDefending(a.Damage);
                    for (int i = 0; i < heads && d != null && c.InPlay(d) && d.Energy.Count > 0 && c.CanAffect(d); i++)
                        c.DiscardEnergyFrom(d, 1, "Twister: discard an Energy attached to " + d.Name);
                    return true;
                }));
            r.RegisterByKey("TM-48", new Fx().After("Blizzard", (c, a) =>
                {
                    if (c.Flip()) HitEachBenched(c, 10);
                    else foreach (var m in c.MyBench().ToList()) c.DamageTarget(m, 10);
                }));
            r.RegisterByKey("TM-49", new CardsDS.EvolutionaryMon("Mapping", c => c.SearchToHand(d => d.IsStadium, 1, "Mapping: search your deck for a Stadium card"), c => c.Me.Deck.Count > 0,
                new CoinsMon("3-D Attack", 3)));
            r.RegisterByKey("TM-50", new Fx()
                .After("Tentavolve", (c, a) =>
                {
                    var d = c.Defending;
                    if (!CardsP9.JustEvolvedFrom(c, "Tentacool") || d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                })
                .With(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("TM-51", new BenchTriggerFx("CURE", c => Affected(c.Me.Active), c => c.Engine.RemoveAllConditions(c.Me.Active), null));
            r.RegisterByKey("TM-52", new Fx().Effect("Double Attack", (c, a) =>
                {
                    var pool = c.OpponentPokemon();
                    int k = Math.Min(2, pool.Count);
                    foreach (var t in c.ChoosePokemonMany(pool, k, k, "Double Attack: choose 2 of your opponent's Pokémon")) c.DamageTarget(t, 20);
                }));
            r.RegisterByKey("TM-53", new Fx()
                .With(new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned))
                .After("Corrode Target", (c, a) =>
                {
                    if (c.Opp.Hand.Count == 0 || !c.Flip()) return;
                    LookAtHand(c, c.Opp);
                    c.Engine.DiscardFromHand(c.Opponent, c.ChooseCards(c.Opp.Hand.ToList(), 1, 1, "Corrode Target: discard which card from your opponent's hand?")[0]);
                }));
            r.RegisterByKey("TM-54", new Fx()
                .After("Shoot Through", (c, a) => HitBenched(c, 1, 10, "Shoot Through"))
                .With(new SwitchSelfMon("U-turn", false)));
            r.RegisterByKey("TM-55", new Fx().Effect("Imitate", (c, a) => { if (c.Opp.Hand.Count > c.Me.Hand.Count) c.DrawUntil(c.Opp.Hand.Count); }));
            r.RegisterByKey("TM-56", new Fx().Effect("Mountain Eater", (c, a) =>
                {
                    if (c.Engine.DiscardTopOfDeck(c.Player, 1).Count > 0 && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 20);
                }));
            r.RegisterByKey("TM-57", new Fx()
                .Effect("Inviting Scent", (c, a) => { if (c.OpponentBench().Count > 0) c.GustOpponent("Inviting Scent: switch in which of your opponent's Benched Pokémon?"); })
                .With(new RecoilMon("Careless Tackle", 10)));
            r.RegisterByKey("TM-58", new DodgeMon("Iron Defense", false));
            r.RegisterByKey("TM-59", FocusEnergy("Bite", 40, false));
            r.RegisterByKey("TM-60", new CuboneLonelyBone());
            r.RegisterByKey("TM-61", new Fx()
                .With(new DodgeMon("Sand Veil", false))
                .After("Mini Earthquake", (c, a) => { foreach (var m in c.MyBench().ToList()) c.DamageTarget(m, 10); }));
            r.RegisterByKey("TM-62", new Fx().After("Gentle Wrap", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("TM-63", new Fx().Effect("Sneaky Placement", (c, a) => SneakyPlacement(c, 1)));
            r.RegisterByKey("TM-64", new Fx()
                .Effect("Sweet Scent", (c, a) =>
                {
                    var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                    var t = hurt.Count == 0 ? null : c.ChoosePokemon(hurt, "Sweet Scent: remove 3 damage counters from which Pokémon?");
                    if (t != null) c.Heal(t, 30);
                })
                .Damage("Vulcan Beat", (c, a) => 30 * c.FlipCoins(c.MyPokemon(m => m.Name == "Volbeat").Count)));
            r.RegisterByKey("TM-66", new Fx().Effect("Tongue Whip", (c, a) => HitBenched(c, 1, 30, "Tongue Whip")));
            r.RegisterByKey("TM-67", new FlipBonusMon("Steady Punch", 10));
            r.RegisterByKey("TM-68", new ComboMon(new SwitchSelfMon("Magnetic Switch", false), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("TM-69", new Fx()
                .Effect("Friend Search", (c, a) =>
                {
                    var top = c.LookAtTop(5);
                    var mons = top.Where(x => x.Def.IsPokemon).ToList();
                    if (mons.Count > 0) c.DeckToHand(c.ChooseCards(mons, 1, 1, "Friend Search: put which Pokémon into your hand?"));
                    c.ShuffleMyDeck();
                })
                .With(new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("TM-70", new Fx().After("Pheromone Poison", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.MyBench().Any(m => m.Name == "Nidoran ♀") && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                }));
            r.RegisterByKey("TM-71", new Fx().Effect("Messenger", (c, a) =>
                {
                    c.SearchToHand(d => d.IsPokemon, 1, "Messenger: search your deck for a Pokémon");
                    if (c.InPlay(c.Pokemon)) c.Engine.ShufflePokemonIntoDeck(c.Pokemon);
                }));
            r.RegisterByKey("TM-73", new Fx().Effect("Recover", (c, a) =>
                {
                    if (c.DiscardEnergyFrom(c.Pokemon, 1, "Recover: discard an Energy attached to " + c.Pokemon.Name) > 0 && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                }));
            r.RegisterByKey("TM-74", new Fx().Effect("Tripping Headbutt", (c, a) =>
                {
                    if (c.Flip()) HitAny(c, 30, "Tripping Headbutt");
                    else
                    {
                        var t = c.ChoosePokemon(c.MyPokemon(), "Tripping Headbutt: do 30 damage to which of your Pokémon?");
                        if (t != null) c.DamageTarget(t, 30);
                    }
                }));
            r.RegisterByKey("TM-75", new Fx()
                .With(new CardsDPEra.FlipGateMon("Disable", new CardsTEU1.AmnesiaMon("Disable")))
                .Effect("Haunt", (c, a) => { if (c.Defending != null && c.CanAffect(c.Defending)) c.PlaceCounters(c.Defending, 1); }));
            r.RegisterByKey("TM-76", new CardsGE.TailsNothingParalyzeMon("Paralyzing Clamp"));
            r.RegisterByKey("TM-77", new Fx().Effect("Sleep Pearl", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Asleep);
                    if (c.MyBench().Count > 0) c.SwitchSelfWithBench();
                }));
            r.RegisterByKey("TM-78", new CardsDRI1.WhirlwindMon("Wing Flick"));
            r.RegisterByKey("TM-80", new Fx().After("Gentle Wrap", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("TM-81", CardsUL.GigaDrain("Leech Life"));
            r.RegisterByKey("TM-82", new Fx()
                .Effect("Illumisile", (c, a) =>
                {
                    if (c.MyPokemon(m => m.Name == "Illumise").Count == 0) { DidNothing(c); return; }
                    HitBenched(c, 1, 30, "Illumisile");
                })
                .After("Firefly Light", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Burned);
                    c.ApplyCondition(d, SpecialCondition.Confused);
                }));
            r.RegisterByKey("TM-83", new Fx().On("Magnetic Bomb", (c, a) =>
                {
                    bool heads = c.Flip();
                    c.DamageDefending(a.Damage + (heads ? 10 : 0));
                    if (!heads && c.InPlay(c.Pokemon)) c.DamageSelf(10);
                    return true;
                }));
            r.RegisterByKey("TM-84", new YanmaFreeFlight());
            RegisterTrainersAndPrimes(r);
        }

        private static bool MayRecoil(EffectContext c, AttackDef a, int more, int recoil)
        {
            bool yes = c.AskYesNo(a.Name + ": do " + more + " more damage (and " + recoil + " damage to " + c.Pokemon.Name + ")?");
            c.DamageDefending(a.Damage + (yes ? more : 0));
            if (yes && c.InPlay(c.Pokemon)) c.DamageSelf(recoil);
            return true;
        }

        private static void StickAndAbsorb(EffectContext c, int heal)
        {
            if (c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, heal);
            if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending);
        }

        private static void SneakyPlacement(EffectContext c, int n)
        {
            var t = c.ChoosePokemon(c.OpponentPokemon(), "Sneaky Placement: put damage counters on which of your opponent's Pokémon?");
            if (t != null && c.CanAffect(t)) c.PlaceCounters(t, n);
        }

        /// <summary>Pheromone Stamina (Poké-Body): +20 HP for each of your Nidoqueen in play. Venomous Horn.</summary>
        private sealed class NidokingPheromoneStamina : BodyMon
        {
            public NidokingPheromoneStamina() : base(new ConditionAttackMon("Venomous Horn", SpecialCondition.Poisoned)) { }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon == src.Pokemon && Works(src, "Pheromone Stamina") ? hp + 20 * src.MyPokemon(m => m.Name == "Nidoqueen").Count : hp;
            }
        }

        /// <summary>Fiery Spirit (Poké-Body): it can't be Confused. Ring of Fire.</summary>
        private sealed class RapidashFierySpirit : BodyMon
        {
            public RapidashFierySpirit() : base(new Fx().After("Ring of Fire", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Burned);
                    c.DefendingCantRetreat(d);
                })) { }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target == src.Pokemon && condition == SpecialCondition.Confused && Works(src, "Fiery Spirit");
            }

            public override bool CuresConditionsItPrevents { get { return true; } }
        }

        /// <summary>Heal Block (Poké-Body): with Lunatone in play, no damage counters can be removed from any Pokémon. Sun Flash.</summary>
        private sealed class SolrockHealBlock : BodyMon
        {
            public SolrockHealBlock() : base(new Fx().After("Sun Flash", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Sun Flash)", c.Player, d.Uid, c.Turn + 1));
                })) { }

            public override bool BlocksHealing(EffectContext src, PokemonInPlay mon) { return src.MyPokemon(m => m.Name == "Lunatone").Count > 0 && Works(src, "Heal Block"); }
        }

        /// <summary>Tangling Tendrils (Poké-Body): while it's your Active Pokémon, the opponent's Active Pokémon's Retreat Cost is [C][C] more. Acidic Drain.</summary>
        private sealed class VictreebelTanglingTendrils : BodyMon
        {
            public VictreebelTanglingTendrils() : base(new Fx().After("Acidic Drain", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d))
                    {
                        c.ApplyCondition(d, SpecialCondition.Burned);
                        c.ApplyCondition(d, SpecialCondition.Poisoned);
                    }
                    if (c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 30);
                })) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Opp.Active && src.PokemonIsActive && Works(src, "Tangling Tendrils") ? cost + 2 : cost;
            }
        }

        /// <summary>Dittobolic (Poké-Body): the opponent can have only 4 Benched Pokémon.</summary>
        private sealed class DittoDittobolic : BodyMon
        {
            public DittoDittobolic() : base(null) { }

            public override int CapMaxBench(EffectContext src, int player) { return player != src.Player && Works(src, "Dittobolic") ? 4 : int.MaxValue; }
        }

        /// <summary>Natural Remedy (Poké-Body): attaching a Water Energy card from your hand to it removes 2 damage counters. Powerful Splash.</summary>
        private sealed class GolduckNaturalRemedy : BodyMon
        {
            public GolduckNaturalRemedy() : base(new Fx().Damage("Powerful Splash", (c, a) => a.Damage + 10 * c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Water)))) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == src.Pokemon && CardsRS.BasicOf(energy.Def, EnergyType.Water) && target.DamageCounters > 0 && Works(src, "Natural Remedy")) src.Heal(target, 20);
            }
        }

        /// <summary>Lonely Bone (Poké-Body): attack damage from the opponent is 20 less for each Marowak in your discard pile. Bone Rush.</summary>
        private sealed class CuboneLonelyBone : BodyMon
        {
            public CuboneLonelyBone() : base(new Fx().Damage("Bone Rush", (c, a) => 20 * c.FlipUntilTails())) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || info.Attacker == null || info.Attacker.Owner == src.Player || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                int n = src.Me.Discard.Count(x => x.Def.Name == "Marowak");
                return n > 0 && Works(src, "Lonely Bone") ? Math.Max(0, damage - 20 * n) : damage;
            }
        }

        /// <summary>Free Flight (Poké-Body): with no Energy attached, no Retreat Cost.</summary>
        private sealed class YanmaFreeFlight : BodyMon
        {
            public YanmaFreeFlight() : base(null) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return mon == src.Pokemon && mon.Energy.Count == 0 && Works(src, "Free Flight") ? 0 : cost; }
        }
    }
}
