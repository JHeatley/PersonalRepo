using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>HS—Undaunted (UD, 2010; v173): the cards not already in the pool (the others are prints). Unlimited only.</summary>
    public static class CardsUD
    {
        private static bool FromEevee(PokemonInPlay m) { return m != null && m.Def.EvolvesFrom == "Eevee"; }

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UD-1", new PowerFx("Hustle Step", c => c.MyPokemon(m => m.DamageCounters > 0).Count > 0, c => HealEach(c, 10), new Fx()
                .On("Dance 'til Dawn", (c, a) =>
                {
                    c.DamageDefending(30 * c.FlipCoins(3));
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                    return true;
                })));
            r.RegisterByKey("UD-2", new Fx()
                .Effect("Solar Suggestion", (c, a) =>
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                        if (hurt.Count == 0) break;
                        var from = RepeatableAbility.ChoosePokemonOrDone(c, hurt, "Solar Suggestion: move a damage counter from which of your Pokémon? (" + (i + 1) + " of up to 4; Done to stop)", true);
                        if (from == null) break;
                        var to = c.ChoosePokemon(c.OpponentPokemon(), "Solar Suggestion: move it to which of your opponent's Pokémon?");
                        if (to == null || !c.CanAffect(to)) break;
                        c.Engine.MoveDamageCounters(from, to, 1, "Solar Suggestion");
                    }
                })
                .With(new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("UD-3", new Fx()
                .After("Mirror Shot", (c, a) => Smoke(c, "Mirror Shot"))
                .Effect("Everyone Explode Now", (c, a) =>
                {
                    var team = c.MyPokemon(m => m.Name == "Pineco" || m == c.Pokemon);
                    c.DamageDefending(30 * team.Count);
                    foreach (var m in team.ToList()) if (c.InPlay(m)) c.DamageWith(m, 30, false, false, false);
                }));
            r.RegisterByKey("UD-4", new Fx()
                .Effect("Ninja Fang", (c, a) =>
                {
                    var d = c.Defending;
                    bool clean = d != null && d.DamageCounters == 0;
                    int done = c.DamageDefending(a.Damage);
                    if (clean && done > 0 && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                })
                .With(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("UD-5", new Fx()
                .Damage("Fire Counterattack", (c, a) => a.Damage + (c.OpponentPokemon(m => c.Engine.IsType(m, EnergyType.Fighting)).Count > 0 ? 60 : 0))
                .After("Dark Roar", (c, a) => OpponentDiscards(c, "Dark Roar")));
            r.RegisterByKey("UD-6", new Fx()
                .With(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned))
                .On("Lava Flow", (c, a) =>
                {
                    var fire = c.Pokemon.Energy.Where(e => CardsRS.BasicOf(e.Def, EnergyType.Fire)).ToList();
                    var picks = fire.Count == 0 ? new List<CardInstance>() : c.ChooseCards(fire, 0, fire.Count, "Lava Flow: discard any number of Fire Energy cards (20 more damage each)");
                    int n = 0;
                    foreach (var e in picks) if (c.Engine.DiscardEnergy(c.Pokemon, e)) n++;
                    c.DamageDefending(a.Damage + 20 * n);
                    return true;
                }));
            r.RegisterByKey("UD-8", new PowerFx("Portrait", c => c.PokemonIsActive && c.Opp.Hand.Count > 0, c =>
                {
                    LookAtHand(c, c.Opp);
                    // Only a Supporter you could play now (HS:Undaunted FAQ, 2010-09-09).
                    var sups = c.Opp.Hand.Where(x => x.Def.IsSupporter && c.Engine.Scripts.For(x.Def).CanPlayTrainer(new EffectContext(c.Engine, c.Player, x, null))).ToList();
                    if (sups.Count == 0) return;
                    var card = c.ChooseCards(sups, 1, 1, "Portrait: use the effect of which Supporter card?")[0];
                    var tctx = new EffectContext(c.Engine, c.Player, card, null);
                    var script = c.Engine.Scripts.For(card.Def);
                    c.Log(c.Pokemon.Name + " uses the effect of " + card.Def.Name + " (Portrait).");
                    if (script.CanPlayTrainer(tctx)) script.ResolveTrainer(tctx);
                }, new CoinsMon("Tail Rap", 2)));
            r.RegisterByKey("UD-9", new Fx()
                .Effect("Blessed Wings", (c, a) =>
                {
                    foreach (var m in c.MyPokemon(x => x.DamageCounters > 0)) c.Heal(m, m.DamageCounters * 10);
                    if (c.InPlay(c.Pokemon)) c.Engine.ShufflePokemonIntoDeck(c.Pokemon);
                })
                .With(new TailsNothingMon("Air Cutter")));
            r.RegisterByKey("UD-10", new Fx()
                .After("Moonlight Fang", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new ShieldFromEffect(c.Pokemon.Name + " is protected from Pokémon with Poké-Powers or Poké-Bodies (Moonlight Fang)", c.Player, c.Pokemon.Uid, c.Turn + 1, (eng, m) => HasPowerOrBody(m)));
                })
                .With(new FlipBonusMon("Quick Blow", 30)));
            r.RegisterByKey("UD-11", new DodrioRetreatAid());
            r.RegisterByKey("UD-12", new Fx()
                .With(new RecoilMon("Balloon Tackle", 20))
                .Effect("Take Away", (c, a) =>
                {
                    var d = c.Defending;
                    c.Engine.ShufflePokemonIntoDeck(c.Pokemon);
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.Engine.ShufflePokemonIntoDeck(d);
                }));
            r.RegisterByKey("UD-13", new Fx()
                .Effect("Spike Spread", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10); })
                .After("Gyro Ball", (c, a) =>
                {
                    if (!c.InPlay(c.Pokemon) || c.MyBench().Count == 0 || !c.AskYesNo("Gyro Ball: switch " + c.Pokemon.Name + " with 1 of your Benched Pokémon?")) return;
                    if (c.SwitchSelfWithBench() && c.Opp.Bench.Count > 0) c.OpponentSwitchesActive();
                }));
            r.RegisterByKey("UD-14", new Fx().On("Backward Belt Throw", (c, a) =>
                {
                    bool more = c.AskYesNo("Backward Belt Throw: do 20 more damage (and 20 damage to " + c.Pokemon.Name + ")?");
                    c.DamageDefending(a.Damage + (more ? 20 : 0));
                    if (more && c.InPlay(c.Pokemon)) c.DamageSelf(20);
                    return true;
                }));
            r.RegisterByKey("UD-15", new Fx()
                .With(new CardsDRI1.WhirlwindMon("Whirlwind"))
                .Effect("Blindside", (c, a) => HitAny(c, 50, "Blindside", m => m.DamageCounters > 0)));
            r.RegisterByKey("UD-16", new Fx()
                .After("Shadow Bind", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); })
                .Damage("Vengeance", (c, a) => a.Damage + 10 * c.Me.Discard.Count(x => x.Def.IsPokemon && x.Def.HasType(EnergyType.Darkness))));
            r.RegisterByKey("UD-17", new Fx()
                .Damage("Miasma Wind", (c, a) => 50 * (c.Defending == null ? 0 : CardsSV.ConditionCount(c.Defending)))
                .With(new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep)));
            r.RegisterByKey("UD-18", new CoinsMon("Quad Smash", 4));
            r.RegisterByKey("UD-19", new Fx()
                .With(new CardsPAR2.FlipConditionMon("Sleeping Spell", SpecialCondition.Asleep))
                .Damage("Poltergeist", (c, a) =>
                {
                    LookAtHand(c, c.Opp);
                    return 30 * c.Opp.Hand.Count(x => x.Def.IsTrainer);
                }));
            r.RegisterByKey("UD-20", new PowerFx("Mischievous Trick", c => c.Me.Deck.Count > 0 && c.Me.Prizes.Any(p => !c.Me.FaceUpPrizeIds.Contains(p.Id)), c =>
                {
                    var faceDown = c.Me.Prizes.Where(p => !c.Me.FaceUpPrizeIds.Contains(p.Id)).ToList();
                    var prize = faceDown[c.State.Rng.Next(faceDown.Count)];
                    int i = c.Me.Prizes.IndexOf(prize);
                    var top = c.Me.Deck[0];
                    c.Me.Deck[0] = prize;
                    c.Me.Prizes[i] = top;
                    c.Log("Mischievous Trick: a face-down Prize card was switched with the top card of the deck.");
                }, new Fx().Effect("Plasma Arrow", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Plasma Arrow: choose 1 of your opponent's Pokémon");
                    if (t != null && c.EnergyCount(t) > 0) c.DamageWith(t, 20 * c.EnergyCount(t), false, false, false);
                })));
            r.RegisterByKey("UD-21", new Fx().Effect("Steel Coat", (c, a) => SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Metal), "Steel Coat")));
            r.RegisterByKey("UD-22", new Fx().Effect("Fresh-Picked Fruit", (c, a) =>
                {
                    var hurt = c.MyBench().Where(m => m.DamageCounters > 0).ToList();
                    var t = hurt.Count == 0 ? null : c.ChoosePokemon(hurt, "Fresh-Picked Fruit: remove 6 damage counters from which Benched Pokémon?");
                    if (t != null) c.Heal(t, 60);
                }));
            r.RegisterByKey("UD-23", new VespiquenDefenseSign());
            r.RegisterByKey("UD-24", new VileplumeAllergyFlower());
            r.RegisterByKey("UD-25", new CardsDS.EvolutionaryMon("Claw Snag", c =>
                {
                    LookAtHand(c, c.Opp);
                    if (c.Opp.Hand.Count == 0) return;
                    c.Engine.DiscardFromHand(c.Opponent, c.ChooseCards(c.Opp.Hand.ToList(), 1, 1, "Claw Snag: discard which card from your opponent's hand?")[0]);
                }, c => c.Opp.Hand.Count > 0, new Fx().Effect("Feint Attack", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Feint Attack: choose 1 of your opponent's Pokémon");
                    if (t != null) c.DamageWith(t, 30, false, false, true);
                })));
            r.RegisterByKey("UD-26", new Fx().After("Flamethrower", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.DiscardEnergyFrom(c.Pokemon, 1, "Flamethrower: discard an Energy attached to " + c.Pokemon.Name);
                }));
            r.RegisterByKey("UD-27", new Fx().After("Miracle Powder", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.Flip() || !c.CanAffect(d)) return;
                    var names = new[] { "Asleep", "Burned", "Confused", "Paralyzed", "Poisoned" };
                    var conds = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
                    c.ApplyCondition(d, conds[c.ChooseOption("Miracle Powder: which Special Condition?", names)]);
                }));
            r.RegisterByKey("UD-28", new DodgeMon("Agility", false));
            r.RegisterByKey("UD-29", new Fx().After("Scrap Attack", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Metal)).ToList();
                    if (pile.Count == 0 || !c.InPlay(c.Pokemon) || !c.Flip()) return;
                    var card = pile[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                }));
            r.RegisterByKey("UD-30", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("UD-31", new Fx()
                .Effect("Sludge Drag", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var pulled = c.GustOpponent("Sludge Drag: switch in which of your opponent's Benched Pokémon?");
                    if (pulled == null || !c.InPlay(pulled) || !c.CanAffect(pulled)) return;
                    c.ApplyCondition(pulled, SpecialCondition.Confused);
                    c.ApplyCondition(pulled, SpecialCondition.Poisoned);
                })
                .Damage("Pester", (c, a) => a.Damage + (Affected(c.Defending) ? 30 : 0)));
            r.RegisterByKey("UD-32", new FlipBonusMon("Charging Horn", 20));
            r.RegisterByKey("UD-33", new Fx().After("Spark", (c, a) => HitBenched(c, 1, 20, "Spark")));
            r.RegisterByKey("UD-34", new Fx()
                .Damage("Razor-Sharp Incisors", (c, a) => 10 * (c.Defending == null ? 0 : c.Defending.DamageCounters))
                .After("Gnaw Up", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d) && d.Energy.Any(e => e.Def.IsSpecialEnergy))
                        c.DiscardEnergyFrom(d, 1, "Gnaw Up: discard which Special Energy card?", e => e.Def.IsSpecialEnergy);
                }));
            r.RegisterByKey("UD-35", new Fx()
                .Effect("Pull Out", (c, a) =>
                {
                    if (c.Me.Discard.Count == 0) return;
                    var card = c.ChooseCards(c.Me.Discard.ToList(), 1, 1, "Pull Out: put which card on top of your deck?")[0];
                    c.Me.Discard.Remove(card);
                    c.Me.Deck.Insert(0, card);
                    c.Engine.ShowCards(c.Opponent, new List<CardInstance> { card }, "Pull Out", "Pull Out: the card your opponent put on top of their deck", c.Source);
                })
                .Damage("Dark Hand", (c, a) => a.Damage + (c.Me.Hand.Count > c.Opp.Hand.Count ? 30 : 0)));
            r.RegisterByKey("UD-36", new Fx().After("Afterimage Strike", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new FlipShieldEffect(c.Pokemon.Name + " may dodge attack damage (Afterimage Strike)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("UD-37", new ComboMon(new CardsSI.HeadsTailsConditionMon("Stun Gas", SpecialCondition.Poisoned, SpecialCondition.Paralyzed), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("UD-38", new Fx()
                .Effect("Full-Belly Refresh", (c, a) =>
                {
                    c.Engine.RemoveAllConditions(c.Pokemon);
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 30);
                })
                .After("Startling Trip", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.Flip()) { if (d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Confused); }
                    else if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                }));
            r.RegisterByKey("UD-39", new Fx()
                .Effect("Chase Up", (c, a) => c.SearchToHand(d => true, 1, "Chase Up: search your deck for any card"))
                .On("Fly", (c, a) =>
                {
                    if (!c.Flip()) return DidNothing(c);
                    c.DamageDefending(a.Damage);
                    if (c.InPlay(c.Pokemon)) c.ProtectSelfNextTurn();
                    return true;
                }));
            r.RegisterByKey("UD-40", new BenchTriggerFx("DARK", c => c.Me.Deck.Count > 0, c =>
                c.SearchToHand(d => CardsRS.BasicOf(d, EnergyType.Darkness), 1, "DARK: search your deck for a Darkness Energy card"), null));
            r.RegisterByKey("UD-41", new Fx()
                .With(new CardsTWM1.HealSelfMon("Spiral Drain", 20))
                .Effect("Dual Splash", (c, a) =>
                {
                    var pool = c.OpponentPokemon();
                    int k = Math.Min(2, pool.Count);
                    foreach (var t in c.ChoosePokemonMany(pool, k, k, "Dual Splash: choose 2 of your opponent's Pokémon")) c.DamageTarget(t, 30);
                }));
            r.RegisterByKey("UD-42", new CardsPAR2.FlipConditionMon("Metal Sound", SpecialCondition.Confused));
            r.RegisterByKey("UD-43", new TailsNothingMon("Single Smash"));
            r.RegisterByKey("UD-44", new Fx().On("Enraged Assault", (c, a) =>
                {
                    bool rage = c.MyBench().Any(m => m.Name == "Vespiquen" && m.DamageCounters > 0);
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (rage ? 60 : 0));
                    if (rage && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                    return true;
                }));
            r.RegisterByKey("UD-45", new Fx().On("Double Headstrike", (c, a) => c.FlipCoins(2) == 2 ? c.DamageDefending(a.Damage) >= 0 : DidNothing(c)));
            r.RegisterByKey("UD-46", new Fx()
                .With(new ShieldAttackMon("Minimize", 20))
                .Effect("Pull", (c, a) => { if (c.OpponentBench().Count > 0 && c.Flip()) c.GustOpponent("Pull: switch in which of your opponent's Benched Pokémon?"); }));
            r.RegisterByKey("UD-48", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new ParalyzeFlipMon("Tickle")));
            r.RegisterByKey("UD-49", new Fx().After("Stun Poison", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.Flip() || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                }));
            r.RegisterByKey("UD-50", new Fx().Effect("Sticky Liquid", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.AddEffect(new RetreatChangeEffect(d.Name + "'s Retreat Cost is [C] more (Sticky Liquid)", c.Player, d.Uid, c.Turn + 1, 1));
                }));
            r.RegisterByKey("UD-54", new FlipBonusMon("Jump On", 10));
            r.RegisterByKey("UD-55", new Fx().Damage("Slap Down", (c, a) => a.Damage + 10 * c.FlipCoins(2)));
            r.RegisterByKey("UD-56", new Fx()
                .Effect("Selfish Draw", (c, a) =>
                {
                    if (c.Me.Deck.Count == 0) return;
                    var top = c.LookAtTop(1);
                    if (c.AskYesNo("Selfish Draw: put " + top[0].Def.Name + " into your hand? (No: discard it and draw a card)")) c.DeckToHand(top);
                    else
                    {
                        c.Engine.DiscardTopOfDeck(c.Player, 1);
                        c.Draw(1);
                    }
                })
                .After("Destructive Jaw", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.Flip() || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    c.DiscardEnergyFrom(d, 1, "Destructive Jaw: discard an Energy card attached to " + d.Name);
                }));
            r.RegisterByKey("UD-57", new Fx().Effect("Dual Draw", (c, a) =>
                {
                    c.Draw(3);
                    c.Engine.DrawCards(c.Opponent, 3);
                }));
            r.RegisterByKey("UD-58", new Fx().After("Astonish", (c, a) =>
                {
                    if (!c.Flip()) return;
                    var card = RandomFromHand(c, c.Opp);
                    if (card == null) return;
                    c.Engine.ShowCards(c.Player, new List<CardInstance> { card }, "Astonish", "Astonish: the card you chose", c.Source);
                    c.Opp.Hand.Remove(card);
                    c.Opp.Deck.Add(card);
                    c.Engine.ShuffleDeck(c.Opponent);
                }));
            r.RegisterByKey("UD-60", new Fx().Effect("Find a Friend", (c, a) =>
                {
                    if (c.Flip()) c.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Grass), 1, "Find a Friend: search your deck for a Grass Pokémon");
                }));
            r.RegisterByKey("UD-61", new CoinsMon("Slam", 2));
            r.RegisterByKey("UD-63", FocusEnergy("Surprise Attack", 80, true));
            r.RegisterByKey("UD-65", new LockAfterAttackMon("Slashing Strike", true));
            r.RegisterByKey("UD-66", new Fx().Effect("Rambunctious Party", (c, a) =>
                {
                    var top = c.LookAtTop(5);
                    var basics = top.Where(x => x.Def.IsBasicPokemon).ToList();
                    int space = c.BenchSpace;
                    if (basics.Count > 0 && space > 0)
                        foreach (var card in c.ChooseCards(basics, 0, Math.Min(space, basics.Count), "Rambunctious Party: put any Basic Pokémon onto your Bench"))
                            if (c.Me.Deck.Remove(card) && c.Engine.PutOntoBench(c.Player, card, false) == null) c.Me.Deck.Add(card);
                    c.ShuffleMyDeck();
                }));
            r.RegisterByKey("UD-67", new PowerFx("Active Volcano", c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.Engine.DiscardTopOfDeck(c.Player, 1);
                    if (top.Count == 0 || !CardsRS.BasicOf(top[0].Def, EnergyType.Fire)) return;
                    if (c.Me.Discard.Remove(top[0]) && !c.Engine.AttachEnergyCard(top[0], c.Pokemon, false) && !c.Me.Discard.Contains(top[0])) c.Me.Discard.Add(top[0]);
                }, null));
            r.RegisterByKey("UD-69", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("UD-70", new Fx().Effect("Plead", (c, a) =>
                {
                    if (c.Engine.AskYesNo(c.Opponent, "Plead: may your opponent draw 2 cards? (No: their Togepi does 20 damage to your Active Pokémon)", c.Source)) c.Draw(2);
                    else c.DamageDefending(20);
                }));

            r.RegisterByKey("UD-71", new BurnedTower());
            r.RegisterByKey("UD-72", new Defender());
            r.RegisterByKey("UD-73", new CardsUL.SimpleTrainer(c => c.Me.Hand.Any(x => x != c.Card && x.Def.IsEnergy), c =>
                {
                    var energy = c.Me.Hand.Where(x => x != c.Card && x.Def.IsEnergy).ToList();
                    var card = c.ChooseCards(energy, 1, 1, "Energy Exchanger: put which Energy card on top of your deck?")[0];
                    c.Me.Hand.Remove(card);
                    c.Me.Deck.Insert(0, card);
                    c.SearchToHand(d => d.IsEnergy, 1, "Energy Exchanger: search your deck for an Energy card");
                }));
            r.RegisterByKey("UD-74", new CardsUL.SimpleTrainer(c => c.Me.Discard.Any(x => x.Def.IsPokemon || x.Def.IsBasicEnergy), c =>
                {
                    var mons = c.Me.Discard.Where(x => x.Def.IsPokemon).ToList();
                    var energy = c.BasicEnergyInDiscard();
                    var picks = new List<CardInstance>();
                    if (mons.Count > 0) picks.AddRange(c.ChooseCards(mons, Math.Min(3, mons.Count), Math.Min(3, mons.Count), "Flower Shop Lady: choose 3 Pokémon"));
                    if (energy.Count > 0) picks.AddRange(c.ChooseCards(energy, Math.Min(3, energy.Count), Math.Min(3, energy.Count), "Flower Shop Lady: choose 3 basic Energy cards"));
                    c.DiscardToDeck(picks);
                }));
            r.RegisterByKey("UD-75", new CardsUL.SimpleTrainer(c => c.Me.Deck.Count > 0, LegendBox));
            r.RegisterByKey("UD-76", new RuinsOfAlph());
            r.RegisterByKey("UD-77", new CardsUL.SimpleTrainer(c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.LookAtTop(5);
                    c.DeckToHand(c.ChooseCards(top, Math.Min(2, top.Count), Math.Min(2, top.Count), "Sage's Training: put 2 cards into your hand"));
                    foreach (var x in top) if (c.Me.Deck.Remove(x)) c.Me.Discard.Add(x);
                }));
            r.RegisterByKey("UD-78", new CardsUL.SimpleTrainer(c => c.Me.Deck.Count > 0, c =>
                {
                    c.Draw(2);
                    OpponentDiscards(c, "Team Rocket's Trickery");
                }));

            r.RegisterByKey("UD-81", new EspeonEvolutionMemories());
            r.RegisterByKey("UD-82", new PowerFx("Fire Breath", c => c.Opp.Active != null, c =>
                {
                    var t = c.Opp.Active;
                    if (c.Flip() && t != null && c.CanAffect(t)) c.ApplyCondition(t, SpecialCondition.Burned);
                }, new Fx().After("Dark Clamp", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); })));
            r.RegisterByKey("UD-83", new RepeatPowerFx("Voltage Increase", c => c.MyPokemon(m => m != c.Pokemon && m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Lightning))).Count > 0, (c, done) =>
                {
                    var donors = c.MyPokemon(m => m != c.Pokemon && m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Lightning)));
                    var from = RepeatableAbility.ChoosePokemonOrDone(c, donors, "Voltage Increase: move a Lightning Energy from which Pokémon?" + (done ? " Press Done when finished." : ""), done);
                    if (from == null) return false;
                    return c.Engine.MoveEnergy(from, c.Pokemon, from.Energy.First(e => CardsDRI1.ProvidesType(c, from, e, EnergyType.Lightning)));
                }, new Fx().After("Mega Thunderbolt", (c, a) => { if (c.InPlay(c.Pokemon)) foreach (var e in c.Pokemon.Energy.ToList()) c.Engine.DiscardEnergy(c.Pokemon, e); })));
            r.RegisterByKey("UD-84", new ScizorRedArmor());
            r.RegisterByKey("UD-85", new PowerFx("Opponent's Choice", c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.Me.Deck.Take(2).ToList();
                    c.Engine.ShowCards(c.Opponent, top, "Opponent's Choice", "Opponent's Choice: the top cards of your opponent's deck", c.Source);
                    var pick = top.Count == 1 ? top[0] : c.Engine.ChooseCards(c.Opponent, top, 1, 1, "Opponent's Choice: which card goes into your opponent's hand?", c.Source)[0];
                    c.DeckToHand(new[] { pick });
                    foreach (var x in top.Where(x => x != pick)) if (c.Me.Deck.Remove(x)) c.Me.Deck.Add(x);
                }, null));
            r.RegisterByKey("UD-86", new PowerFx("Cloud-Covered Moon", c => c.PokemonIsActive, c =>
                {
                    if (c.Flip()) c.Engine.ReturnPokemonToHand(c.Pokemon);
                }, new Fx().Damage("Evoblast", (c, a) => a.Damage + 10 * c.MyPokemon(FromEevee).Count)));

            var kyogreGroudon = new Fx()
                .Effect("Mega Tidal Wave", (c, a) =>
                {
                    var top = c.Engine.DiscardTopOfDeck(c.Opponent, 5);
                    int n = top.Count(x => x.Def.IsEnergy);
                    if (n > 0) HitEachBenched(c, 30 * n);
                })
                .Effect("Massive Eruption", (c, a) =>
                {
                    var top = c.Engine.DiscardTopOfDeck(c.Player, 5);
                    c.DamageDefending(100 * top.Count(x => x.Def.IsEnergy));
                });
            r.RegisterByKey("UD-87", kyogreGroudon);
            r.RegisterByKey("UD-88", kyogreGroudon);
            var rayquazaDeoxys = new RayquazaSpaceVirus();
            r.RegisterByKey("UD-89", rayquazaDeoxys);
            r.RegisterByKey("UD-90", rayquazaDeoxys);
            r.RegisterByKey("UD-THREE", new CardsHS.AlphLithograph(c =>
                {
                    var st = c.State.Stadium;
                    if (st == null) return;
                    int owner = c.State.StadiumOwner;
                    c.State.Stadium = null;
                    c.State.Players[owner].Hand.Add(st);
                    c.Log(st.Def.Name + " went back to " + c.Engine.PlayerName(owner) + "'s hand (Alph Lithograph).");
                }, c => c.State.Stadium != null));
        }

        private static void Smoke(EffectContext c, string attack)
        {
            var d = c.Defending;
            if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (" + attack + ")", c.Player, d.Uid, c.Turn + 1));
        }

        /// <summary>The opponent discards a card from their hand (they choose).</summary>
        internal static void OpponentDiscards(EffectContext c, string what)
        {
            if (c.Opp.Hand.Count == 0) return;
            var card = c.Engine.ChooseCards(c.Opponent, c.Opp.Hand.ToList(), 1, 1, what + ": discard a card from your hand", c.Source)[0];
            c.Engine.DiscardFromHand(c.Opponent, card);
        }

        /// <summary>Legend Box: reveal the top 10; with both halves of a LEGEND there, bench it and attach the revealed Energy to it.</summary>
        private static void LegendBox(EffectContext c)
        {
            var top = c.Me.Deck.Take(10).ToList();
            c.Engine.ShowCards(c.Opponent, top, "Legend Box", "Legend Box: the top cards of your opponent's deck", c.Source);
            c.Log("Legend Box revealed: " + string.Join(", ", top.Select(x => x.Def.Name)) + ".");
            var pairs = top.Where(x => x.Def.IsLegend && x.Def.LegendHalf == "top" && top.Any(y => y.Def.IsLegend && y.Def.LegendHalf == "bottom" && y.Def.Name == x.Def.Name)).ToList();
            if (pairs.Count > 0 && c.BenchSpace > 0)
            {
                var first = pairs.Count == 1 ? pairs[0] : c.ChooseCards(pairs, 1, 1, "Legend Box: bench which Pokémon LEGEND?")[0];
                var second = top.First(y => y.Def.IsLegend && y.Def.LegendHalf == "bottom" && y.Def.Name == first.Def.Name);
                c.Me.Deck.Remove(first);
                c.Me.Deck.Remove(second);
                var mon = c.Engine.PutOntoBench(c.Player, first, false);
                if (mon != null)
                {
                    mon.Stack.Insert(0, second);
                    foreach (var e in top.Where(x => x.Def.IsEnergy).ToList())
                        if (c.Me.Deck.Remove(e) && !c.Engine.AttachEnergyCard(e, mon, false) && !c.Me.Discard.Contains(e)) c.Me.Deck.Add(e);
                }
                else { c.Me.Deck.Add(first); c.Me.Deck.Add(second); }
            }
            c.ShuffleMyDeck();
        }

        /// <summary>During the opponent's next turn, when the Pokémon would be damaged by an attack, flip; heads, prevent it (Afterimage Strike).</summary>
        private sealed class FlipShieldEffect : ContinuousEffect
        {
            public FlipShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || damage <= 0 || engine.CurrentSourceKind != EffectSourceKind.Attack) return false;
                return engine.FlipCoin(Owner, "Afterimage Strike");
            }
        }

        /// <summary>Retreat Aid (Poké-Body): while Dodrio is on your Bench, your Active Pokémon's Retreat Cost is [C][C] less. Incessant Peck.</summary>
        private sealed class DodrioRetreatAid : BodyMon
        {
            public DodrioRetreatAid() : base(new Fx().Damage("Incessant Peck", (c, a) => a.Damage + 20 * c.FlipUntilTails())) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon != src.Me.Active || !src.Me.Bench.Contains(src.Pokemon) || !Works(src, "Retreat Aid")) return cost;
                return Math.Max(0, cost - 2);
            }
        }

        /// <summary>Defense Sign (Poké-Body): prevent all attack damage to your Benched Grass Pokémon. Mach Wind.</summary>
        private sealed class VespiquenDefenseSign : BodyMon
        {
            public VespiquenDefenseSign() : base(new Fx().After("Mach Wind", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new RetreatChangeEffect(c.Pokemon.Name + " has no Retreat Cost during your next turn (Mach Wind)", c.Player, c.Pokemon.Uid, c.Turn + 2, null));
                })) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                return t != null && t.Owner == src.Player && src.Me.Bench.Contains(t) && src.Engine.IsType(t, EnergyType.Grass) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Defense Sign");
            }
        }

        /// <summary>Allergy Flower (Poké-Body): neither player can play Trainer cards (Items). Dazzling Pollen.</summary>
        private sealed class VileplumeAllergyFlower : BodyMon
        {
            public VileplumeAllergyFlower() : base(new Fx().On("Dazzling Pollen", (c, a) =>
                {
                    bool heads = c.Flip();
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (heads ? 20 : 0));
                    if (!heads && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Confused);
                    return true;
                })) { }

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card) { return card != null && CardsDPEra.DpTrainer(card.Def) && Works(src, "Allergy Flower"); }
        }

        /// <summary>Evolution Memories (Poké-Body): it may use the attacks of your Pokémon that evolve from Eevee. Solar Ray.</summary>
        private sealed class EspeonEvolutionMemories : BodyMon
        {
            public EspeonEvolutionMemories() : base(new Fx().After("Solar Ray", (c, a) => HealEach(c, 10))) { }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (!Works(src, "Evolution Memories")) return list;
                var seen = new HashSet<string>();
                foreach (var m in src.Me.InPlay())
                {
                    if (m == src.Pokemon || !FromEevee(m) || !seen.Add(m.Def.BaseKey)) continue;
                    for (int i = 0; i < m.Def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(m.Def, i));
                }
                return list;
            }
        }

        /// <summary>Red Armor (Poké-Body): prevent all attack damage from the opponent's Pokémon with Special Energy attached. Metal Scissors.</summary>
        private sealed class ScizorRedArmor : BodyMon
        {
            public ScizorRedArmor() : base(new Fx().Damage("Metal Scissors", (c, a) => a.Damage + 20 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Metal))) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && info.Attacker.Owner != src.Player && info.Attacker.Energy.Any(e => e.Def.IsSpecialEnergy)
                       && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Red Armor");
            }
        }

        /// <summary>Space Virus (Poké-Body): an opponent's Pokémon Knocked Out by its attack gives 1 more Prize card. Ozone Buster.</summary>
        private sealed class RayquazaSpaceVirus : BodyMon
        {
            public RayquazaSpaceVirus() : base(new Fx().After("Ozone Buster", (c, a) => DiscardTypeFromSelf(c, EnergyType.Fire, 99))) { }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut == null || knockedOut.Owner == src.Player || taker != src.Player || src.Pokemon == null || knockedOut.HitByAttackerUid != src.Pokemon.Uid) return prizes;
                return Works(src, "Space Virus") ? prizes + 1 : prizes;
            }
        }

        /// <summary>Burned Tower (Stadium): once during each player's turn, they may flip; heads, a basic Energy card from their discard pile into their hand.</summary>
        private sealed class BurnedTower : CardScript
        {
            private const string Limit = "Burned Tower";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.BasicEnergyInDiscard().Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return true;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "Burned Tower: put which basic Energy card into your hand?"));
                return true;
            }
        }

        private const string DefenderMarker = "Defender played";

        /// <summary>Defender (Item, attached): attack damage to it is 20 less (errata: any attack); discarded at the end of the opponent's next turn.</summary>
        private sealed class Defender : CardScript
        {
            public override bool IsAttachedNonTool { get { return true; } }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.InPlay().Any(); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Defender: attach it to which of your Pokémon?");
                if (target == null) return;
                ctx.CardPlacedElsewhere = true;
                ctx.Engine.AttachTool(ctx.Card, target);
                target.Markers[DefenderMarker + ctx.Card.Id] = ctx.Turn;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                // Errata: damage by any attack, not only the opponent's.
                if (info.Target != src.Pokemon || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Math.Max(0, damage - 20);
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                int t;
                var holder = src.Pokemon;
                if (holder == null || src.IsMyTurn || !holder.HasTool(src.Card)) return;
                if (holder.Markers.TryGetValue(DefenderMarker + src.Card.Id, out t) && src.Turn <= t) return;
                src.Log("Defender is discarded.");
                src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Ruins of Alph (Stadium): each Pokémon in play has no Resistance.</summary>
        private sealed class RuinsOfAlph : CardScript
        {
            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount) { return 0; }
        }
    }
}
