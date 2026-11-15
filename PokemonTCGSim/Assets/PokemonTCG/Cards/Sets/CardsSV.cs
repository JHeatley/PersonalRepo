using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Supreme Victors (SV, 2009; v173): the cards not already in the pool (the others are prints). Unlimited only. Rares, LV.X,
    /// Trainers and the SH cards here; Uncommons and Commons in CardsSVB.cs.
    /// </summary>
    public static partial class CardsSV
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SV-1", new Fx()
                .Effect("Feint Attack", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Feint Attack: choose 1 of your opponent's Pokémon");
                    if (t != null) c.DamageWith(t, 20, false, false, true);
                })
                .Effect("Doom News", (c, a) =>
                {
                    var d = c.Defending;
                    foreach (var e in c.Pokemon.Energy.ToList()) c.Engine.ReturnEnergyToHand(c.Pokemon, e);
                    if (d != null && c.CanAffect(d)) c.AddEffect(new DelayedKnockOutEffect(d.Name + " is Knocked Out at the end of its next turn (Doom News)", c.Player, d.Uid, c.Turn + 1, "Doom News"));
                }));
            r.RegisterByKey("SV-2", new Fx()
                .Effect("Luring Flame", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var pulled = c.GustOpponent("Luring Flame: switch in which of your opponent's Benched Pokémon?");
                    if (pulled != null && c.InPlay(pulled) && c.CanAffect(pulled)) c.ApplyCondition(pulled, SpecialCondition.Burned);
                })
                .Damage("Vapor Kick", (c, a) => a.Damage + (c.OpponentPokemon(m => c.Engine.IsType(m, EnergyType.Water)).Count > 0 ? 30 : 0)));
            r.RegisterByKey("SV-3", new DrifblimPumpUp());
            r.RegisterByKey("SV-4", new Fx()
                .Effect("Dump and Draw", (c, a) => DumpAndDraw(c, "Dump and Draw"))
                .After("Electric Current", (c, a) => EnergyToBench(c, "Electric Current", e => CardsRS.BasicOf(e.Def, EnergyType.Lightning))));
            r.RegisterByKey("SV-5", new GarchompDragonIntimidation());
            r.RegisterByKey("SV-6", new CardsDS.EvolutionaryMon("Evolutionary Flame", c =>
                {
                    var t = c.Opp.Active;
                    if (t == null || !c.CanAffect(t)) return;
                    c.ApplyCondition(t, SpecialCondition.Burned);
                    c.ApplyCondition(t, SpecialCondition.Confused);
                }, c => c.Opp.Active != null, new Fx()
                .Effect("Fire Arrow", (c, a) => HitAny(c, 30, "Fire Arrow"))
                .After("Flame Ball", (c, a) =>
                {
                    if (c.MyBench().Count > 0 && c.Pokemon.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Fire)) && c.AskYesNo("Flame Ball: move a Fire Energy card to 1 of your Benched Pokémon?"))
                        EnergyToBench(c, "Flame Ball", e => CardsRS.BasicOf(e.Def, EnergyType.Fire));
                })));
            r.RegisterByKey("SV-7", new MetagrossGravitation());
            r.RegisterByKey("SV-8", new Fx()
                .On("Trash Burst", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                    var picks = energy.Count == 0 ? new List<CardInstance>() : c.ChooseCards(energy, 0, Math.Min(5, energy.Count), "Trash Burst: discard up to 5 Energy cards from your hand (10 more damage each)");
                    foreach (var x in picks) c.Engine.DiscardFromHand(c.Player, x);
                    c.DamageDefending(a.Damage + 10 * picks.Count);
                    return true;
                })
                .On("Twister", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads == 0) return DidNothing(c);
                    var d = c.Defending;
                    c.DamageDefending(a.Damage);
                    for (int i = 0; i < heads && d != null && c.InPlay(d) && d.Energy.Count > 0 && c.CanAffect(d); i++)
                        c.DiscardEnergyFrom(d, 1, "Twister: discard an Energy attached to " + d.Name);
                    return true;
                }));
            r.RegisterByKey("SV-9", new Fx()
                .After("Drain Punch", (c, a) =>
                {
                    int n = c.Defending == null ? 0 : c.EnergyCount(c.Defending);
                    if (n > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 10 * n);
                })
                .Damage("Rainbow Lariat", (c, a) => 20 * c.MyPokemon(SP).SelectMany(m => c.Engine.TypesOf(m)).Distinct().Count()));
            r.RegisterByKey("SV-10", new Fx()
                .Damage("Raging Drill", (c, a) => a.Damage + 10 * c.Pokemon.DamageCounters)
                .On("Deep Scrap", (c, a) =>
                {
                    var d = c.Defending;
                    c.DamageDefending(a.Damage);
                    if (d != null && (!c.InPlay(d) || c.Engine.IsKnockedOut(d))) c.Engine.DiscardTopOfDeck(c.Opponent, 3);
                    return true;
                }));
            r.RegisterByKey("SV-11", new ComboMon(new FlipBonusMon("Quick Attack", 10), new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("SV-12", new SwampertRootProtector());
            r.RegisterByKey("SV-13", new VenusaurGreenAroma());
            r.RegisterByKey("SV-14", new PowerFx("Speed Boost", c => c.PokemonIsActive && c.Me.Discard.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Grass)), c =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Grass)).ToList();
                    var card = c.ChooseCards(pile, 1, 1, "Speed Boost: attach which Grass Energy card?")[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                }, new Fx().On("Wind Return", (c, a) =>
                {
                    var grass = c.Pokemon.Energy.Where(e => CardsRS.BasicOf(e.Def, EnergyType.Grass)).ToList();
                    int n = 0;
                    if (grass.Count > 0 && c.AskYesNo("Wind Return: return all Grass Energy attached to " + c.Pokemon.Name + " to your hand (20 more damage each)?"))
                        foreach (var e in grass) if (c.Engine.ReturnEnergyToHand(c.Pokemon, e)) n++;
                    c.DamageDefending(a.Damage + 20 * n);
                    return true;
                })));
            r.RegisterByKey("SV-15", new ArcanineExtremeSpeed());
            r.RegisterByKey("SV-16", new ComboMon(new FlipBonusMon("Sharp Beak", 10), new ParalyzeFlipMon("Ice Beam")));
            r.RegisterByKey("SV-17", new ButterfreeCompoundEyes());
            r.RegisterByKey("SV-18", new Fx()
                .After("Moving Fire", (c, a) =>
                {
                    var donors = c.MyBench().Where(m => m.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Fire))).ToList();
                    if (donors.Count == 0 || !c.InPlay(c.Pokemon) || !c.AskYesNo("Moving Fire: move a Fire Energy card from 1 of your Benched Pokémon to " + c.Pokemon.Name + "?")) return;
                    var from = c.ChoosePokemon(donors, "Moving Fire: from which Benched Pokémon?");
                    if (from != null) c.Engine.MoveEnergy(from, c.Pokemon, from.Energy.First(e => CardsRS.BasicOf(e.Def, EnergyType.Fire)));
                })
                .After("Volcanic Crash", (c, a) =>
                {
                    if (c.OpponentPokemon(m => c.Engine.IsType(m, EnergyType.Water)).Count > 0) return;
                    int tails = 3 - c.FlipCoins(3);
                    if (tails > 0) c.Engine.DiscardTopOfDeck(c.Player, tails);
                }));
            r.RegisterByKey("SV-19", new Fx()
                .With(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned))
                .After("Earth Power", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads > 0) HitEachBenched(c, 10 * heads);
                }));
            r.RegisterByKey("SV-20", new Fx().Effect("Flame Jet", (c, a) => { if (c.Flip()) HitAny(c, 40, "Flame Jet"); }));
            r.RegisterByKey("SV-21", new Fx()
                .Effect("Heal Bell", (c, a) => HealEach(c, 30))
                .Effect("Super Psywave", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Super Psywave: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    int n = c.EnergyCount(t);
                    if (n > 0 && c.CanAffect(t)) c.PlaceCounters(t, n);
                }));
            r.RegisterByKey("SV-22", new Fx()
                .Effect("Antique Magic", (c, a) => c.PlaceCountersAnyWay(3, () => c.MyPokemon().Concat(c.OpponentPokemon()).ToList()))
                .Damage("Synchro Attack", (c, a) => c.Defending != null && c.RemainingHp(c.Defending) == c.RemainingHp(c.Pokemon) ? 90 : a.Damage));
            r.RegisterByKey("SV-23", new ComboMon(new SwitchSelfMon("Smash Turn", true), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("SV-24", new DewgongThickFat());
            r.RegisterByKey("SV-25", new PowerFx("Echo Draw", c => c.Me.Deck.Count > 0, c => c.Draw(1), null));
            r.RegisterByKey("SV-26", new Fx()
                .After("Ghost Hand", (c, a) =>
                {
                    if (c.MyBench().Count == 0) return;
                    var t = c.ChoosePokemon(c.MyBench(), "Ghost Hand: put 1 damage counter on which of your Benched Pokémon?");
                    if (t != null) c.PlaceCounters(t, 1);
                })
                .Damage("Cursed Wrath", (c, a) => 10 * c.Me.Discard.Count(x => x.Def.IsPokemon && x.Def.IsPokemonSP)));
            r.RegisterByKey("SV-27", new Fx()
                .After("Rushing Water", (c, a) => CardsPL.TailCode(c))
                .Damage("Escort", (c, a) => a.Damage + (c.Me.SupporterPlayedThisTurn ? 20 : 0)));
            r.RegisterByKey("SV-28", new ExploudErasingSound());
            r.RegisterByKey("SV-29", new PowerFx("Darkness Restore", c => c.PokemonIsActive && c.Opp.Bench.Count < 5 && c.Opp.Discard.Any(x => x.Def.IsBasicPokemon), c =>
                {
                    var pile = c.Opp.Discard.Where(x => x.Def.IsBasicPokemon).ToList();
                    var card = c.ChooseCards(pile, 1, 1, "Darkness Restore: put which Basic Pokémon onto your opponent's Bench?")[0];
                    if (c.Opp.Discard.Remove(card) && c.Engine.PutOntoBench(c.Opponent, card, false) == null) c.Opp.Discard.Add(card);
                }, new Fx()
                .Damage("Riot", (c, a) => a.Damage + 10 * c.MyPokemon().Concat(c.OpponentPokemon()).Count(m => !CardsE2.IsEvolved(m)))));
            r.RegisterByKey("SV-30", new Fx()
                .Effect("Licking-Licking Heal", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsBasicEnergy).ToList();
                    if (energy.Count == 0) return;
                    var card = c.ChooseCards(energy, 1, 1, "Licking-Licking Heal: attach which basic Energy card?")[0];
                    var t = c.ChoosePokemon(c.MyPokemon(), "Licking-Licking Heal: attach it to which Pokémon?");
                    if (t == null || !c.Me.Hand.Remove(card)) return;
                    if (!c.Engine.AttachEnergyCard(card, t, true)) { if (!c.Me.Discard.Contains(card)) c.Me.Hand.Add(card); return; }
                    if (t.DamageCounters > 0) c.Heal(t, 20);
                })
                .After("Return", (c, a) => c.DrawUntil(6)));
            r.RegisterByKey("SV-31", new FlipBonusMon("Mid-air Strike", 30));
            r.RegisterByKey("SV-32", new LunatoneMarvelEyes());
            r.RegisterByKey("SV-33", new Fx()
                .Effect("Turnaround Standby", (c, a) =>
                {
                    c.AddEffect(new AttackBonusEffect(c.Pokemon.Name + "'s Swallow does 40 more damage next turn (Turnaround Standby)", c.Player, c.Pokemon.Uid, c.Turn + 2, 40, "Swallow"));
                    c.AddEffect(new AttackBonusEffect(c.Pokemon.Name + "'s Bite does 40 more damage next turn (Turnaround Standby)", c.Player, c.Pokemon.Uid, c.Turn + 2, 40, "Bite"));
                })
                .Effect("Swallow", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    if (done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
                }));
            r.RegisterByKey("SV-34", new Fx()
                .After("Channeling", (c, a) => c.Engine.DrawCards(c.Opponent, 2))
                .Damage("Chakra Points", (c, a) => a.Damage + 10 * c.Opp.Hand.Count));
            r.RegisterByKey("SV-35", new Fx()
                .Damage("Aqua Tail", (c, a) => a.Damage + 20 * c.FlipCoins(c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water)))
                .With(new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("SV-36", new TailsNothingMon("Sky Attack"));
            r.RegisterByKey("SV-37", new MrMimeFocusWall());
            r.RegisterByKey("SV-38", new Fx()
                .Effect("Nutritional Support", (c, a) => SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Grass), "Nutritional Support", 2))
                .After("Hibernation Spore", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d) && c.ApplyCondition(d, SpecialCondition.Asleep)) d.SleepCoins = Math.Max(d.SleepCoins, 2);
                }));
            r.RegisterByKey("SV-39", new Fx()
                .Effect("Top Drop", (c, a) =>
                {
                    var top = c.Engine.DiscardTopOfDeck(c.Opponent, 1);
                    if (top.Count > 0 && top[0].Def.IsPokemon && top[0].Def.Hp > 0) c.DamageDefending(top[0].Def.Hp);
                })
                .Effect("Brick Break", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, true); }));
            r.RegisterByKey("SV-40", new RoseradeNaturalCure());
            r.RegisterByKey("SV-41", new CardsDPEra.AstonishMon("Astonish"));
            r.RegisterByKey("SV-42", new PowerFx("Dig Down", c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.LookAtTop(5);
                    var fighting = top.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fighting)).ToList();
                    if (fighting.Count > 0) c.DeckToHand(c.ChooseCards(fighting, 0, fighting.Count, "Dig Down: put any Fighting Energy cards into your hand"));
                    c.ShuffleMyDeck();
                }, new Fx().After("Needle", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.Flip() || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                })));
            r.RegisterByKey("SV-43", new Fx()
                .On("Horn Pierce", (c, a) => c.FlipCoins(2) == 2 ? c.DamageDefending(a.Damage) >= 0 : DidNothing(c))
                .After("Endure", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon) && c.Flip()) c.AddEffect(new EndureEffect(c.Pokemon.Name + " can't be Knocked Out by damage from attacks (Endure)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("SV-44", new ShedinjaMarvelShell());
            r.RegisterByKey("SV-45", new PowerFx("Sunshine Fate", c => c.MyPokemon(m => m.Name == "Lunatone").Count > 0 && c.Me.Deck.Count > 0, c =>
                {
                    var top = c.Me.Deck.Take(3).ToList();
                    foreach (var x in top) c.Me.Deck.Remove(x);
                    CardsCRZ1.PutOnTopInOrder(c, top, "Sunshine Fate");
                }, new Fx().On("Luna Turn", (c, a) =>
                {
                    var luna = c.Me.Discard.FirstOrDefault(x => x.Def.Name == "Lunatone");
                    c.DamageDefending(a.Damage + (luna != null ? 30 : 0));
                    if (luna != null) c.DiscardToDeck(new[] { luna });
                    return true;
                })));
            r.RegisterByKey("SV-46", new Fx()
                .Effect("Synchro Removal", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.CanAffect(d)) return;
                    var mine = c.Pokemon.Energy.SelectMany(e => c.Engine.EnergyUnitsOf(c.Pokemon, e)).ToList();
                    var match = d.Energy.Where(e => c.Engine.EnergyUnitsOf(d, e).Any(u => mine.Any(m => (m & u) != 0 && u != EnergyMask.Colorless))).ToList();
                    if (match.Count == 0) return;
                    var pick = match.Count == 1 ? match[0] : c.ChooseCards(match, 1, 1, "Synchro Removal: discard which Energy card?")[0];
                    c.Engine.DiscardEnergy(d, pick);
                })
                .Damage("Pulled Punch", (c, a) => c.Defending != null && c.Defending.DamageCounters > 0 ? 10 : a.Damage));
            r.RegisterByKey("SV-47", new Fx()
                .Effect("Rest", (c, a) =>
                {
                    c.Engine.RemoveAllConditions(c.Pokemon);
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })
                .With(new LockAfterAttackMon("Giant Wave", true)));
            r.RegisterByKey("SV-48", new FlipBonusMon("Lightning Wing", 20));

            r.RegisterByKey("SV-134", new BattleTower());
            r.RegisterByKey("SV-135", new ChampionsRoom());
            r.RegisterByKey("SV-136", new CynthiasGuidance());
            r.RegisterByKey("SV-137", new CyrussInitiative());
            r.RegisterByKey("SV-138", new NightTeleporter());
            r.RegisterByKey("SV-139", new PalmersContribution());

            r.RegisterByKey("SV-141", new CardsDS.EvolutionaryMon("Darkness Send", c =>
                {
                    int heads = c.FlipCoins(3);
                    for (int i = 0; i < heads && c.Opp.Deck.Count > 0; i++)
                    {
                        var top = c.Opp.Deck[0];
                        c.Opp.Deck.RemoveAt(0);
                        CardsCRZ1.PutInLostZone(c, top);
                    }
                }, c => c.PokemonIsActive && c.Opp.Deck.Count > 0, new Fx()
                .On("Darkness Slugger", (c, a) =>
                {
                    bool more = c.Me.Hand.Count > 0 && c.AskYesNo("Darkness Slugger: discard a card from your hand for 30 more damage?");
                    if (more) c.Engine.DiscardFromHand(c.Player, c.ChooseCards(c.Me.Hand.ToList(), 1, 1, "Darkness Slugger: discard which card?")[0]);
                    c.DamageDefending(a.Damage + (more ? 30 : 0));
                    return true;
                })));
            r.RegisterByKey("SV-142", new BlazikenBurningSpirit());
            r.RegisterByKey("SV-144", new PowerFx("Energy Recycle", c => c.Me.Discard.Any(x => x.Def.IsEnergy), c =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsEnergy).ToList();
                    var picks = c.ChooseCards(pile, 0, Math.Min(3, pile.Count), "Energy Recycle: choose up to 3 Energy cards to attach");
                    if (picks.Count > 0) c.AttachCardsAnyWay(picks, picks.Count, (x, m) => true, "Energy Recycle", false);
                    c.Engine.RequestEndTurn();
                }, new Fx()
                .Damage("Powerful Spark", (c, a) => a.Damage + 10 * c.MyPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("SV-147", new PowerFx("Fast Call", c => c.Me.Deck.Count > 0, c =>
                    c.SearchToHand(d => d.IsSupporter, 1, "Fast Call: search your deck for a Supporter card"),
                new Fx().On("Defog", (c, a) =>
                {
                    bool discard = c.State.Stadium != null && c.AskYesNo("Defog: discard the Stadium in play (base damage 70)?");
                    if (discard) c.Engine.DiscardStadium();
                    c.DamageDefending(discard ? 70 : a.Damage);
                    return true;
                })));

            r.RegisterByKey("SV-SH7", new MiloticAquaMirage());
            r.RegisterByKey("SV-SH8", new Fx()
                .After("Deep Sea Pressure", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new RetreatChangeEffect(d.Name + "'s Retreat Cost is 2 more (Deep Sea Pressure)", c.Player, d.Uid, c.Turn + 1, 2));
                })
                .Damage("Aqua Wave", (c, a) => a.Damage + 10 * c.FlipCoins(2)));
            r.RegisterByKey("SV-SH9", new Fx()
                .Effect("Sonicboom", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, false); })
                .With(new CardsSW.BatonPassMon("Baton Pass")));
            RegisterCommons(r);
        }

        /// <summary>"Discard up to 2 Energy cards from your hand. For each card you discarded, draw 2 cards."</summary>
        internal static void DumpAndDraw(EffectContext c, string attack)
        {
            var energy = c.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
            if (energy.Count == 0) return;
            var picks = c.ChooseCards(energy, 0, Math.Min(2, energy.Count), attack + ": discard up to 2 Energy cards from your hand (draw 2 for each)");
            foreach (var x in picks) c.Engine.DiscardFromHand(c.Player, x);
            if (picks.Count > 0) c.Draw(2 * picks.Count);
        }

        /// <summary>Pump Up (Poké-Body): with the opponent at 3 or fewer Prize cards, +40 HP. Shadow Ball (applies W/R on the Bench).</summary>
        private sealed class DrifblimPumpUp : BodyMon
        {
            public DrifblimPumpUp() : base(new Fx().Effect("Shadow Ball", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var t = c.ChoosePokemon(c.OpponentBench(), "Shadow Ball: choose 1 of your opponent's Benched Pokémon");
                    if (t != null) c.DamageWith(t, 40, true, true, false);
                })) { }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon == src.Pokemon && src.Opp.Prizes.Count <= 3 && Works(src, "Pump Up") ? hp + 40 : hp;
            }
        }

        /// <summary>Dragon Intimidation (Poké-Body): when your Active Garchomp is damaged by an opponent's attack, an Energy card on the attacker goes to its owner's hand. Guard Claw. Speed Impact.</summary>
        private sealed class GarchompDragonIntimidation : BodyMon
        {
            public GarchompDragonIntimidation() : base(new Fx()
                .With(new ShieldAttackMon("Guard Claw", 20))
                .Damage("Speed Impact", (c, a) => a.Damage - 20 * (c.Defending == null ? 0 : c.EnergyCount(c.Defending)))) { }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || attacker.Energy.Count == 0) return;
                Announce(ctx, "Dragon Intimidation");
                var e = attacker.Energy.Count == 1 ? attacker.Energy[0] : ctx.ChooseCards(attacker.Energy.ToList(), 1, 1, "Dragon Intimidation: put which Energy card into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(attacker, e);
            }
        }

        /// <summary>Gravitation (Poké-Body): each Pokémon in play gets -20 HP (only 20 from all Gravitation). Geo Impact.</summary>
        private sealed class MetagrossGravitation : BodyMon
        {
            public MetagrossGravitation() : base(new Fx().After("Geo Impact", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.State.Stadium == null || c.State.StadiumOwner != c.Player || d == null) return;
                    var types = c.Engine.TypesOf(d);
                    HitEachBenched(c, 20, m => c.Engine.TypesOf(m).Any(types.Contains));
                })) { }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || !Works(src, "Gravitation") || !CardsCRZ1.IsLeadSource(src, "Gravitation") || !LeadAcrossPlayers(src)) return hp;
                return hp - 20;
            }

            /// <summary>Only 1 Gravitation counts, even across both players (the first Metagross put into play).</summary>
            private static bool LeadAcrossPlayers(EffectContext src)
            {
                var all = src.MyPokemon().Concat(src.OpponentPokemon()).Where(m => m.Def.Abilities.Any(a => a.Name == "Gravitation")).OrderBy(m => m.PlaySeq).ToList();
                return all.Count == 0 || all[0] == src.Pokemon;
            }
        }

        /// <summary>Root Protector (Poké-Body): attack damage from the opponent's non-Evolved Pokémon is 20 less. Drag Off. Push Over.</summary>
        private sealed class SwampertRootProtector : BodyMon
        {
            public SwampertRootProtector() : base(new Fx()
                .Before("Drag Off", (c, a) =>
                {
                    if (c.OpponentBench().Count > 0 && c.AskYesNo("Drag Off: switch your opponent's Active Pokémon with 1 of their Benched Pokémon first?"))
                        c.GustOpponent("Drag Off: switch in which of your opponent's Benched Pokémon?");
                })
                .Damage("Push Over", (c, a) => a.Damage + 10 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fighting))) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || info.Attacker == null || info.Attacker.Owner == src.Player || CardsE2.IsEvolved(info.Attacker)) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !Works(src, "Root Protector")) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>Green Aroma (Poké-Body): your Grass Pokémon can't be affected by Special Conditions (and lose them). Desperate Pollen. Special Reaction.</summary>
        private sealed class VenusaurGreenAroma : BodyMon
        {
            public VenusaurGreenAroma() : base(new Fx()
                .After("Desperate Pollen", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.Pokemon.DamageCounters < 8 || d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Burned);
                    c.ApplyCondition(d, SpecialCondition.Confused);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                })
                .Damage("Special Reaction", (c, a) => a.Damage + 40 * (c.Defending == null ? 0 : ConditionCount(c.Defending)))) { }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target.Owner == src.Player && src.Engine.IsType(target, EnergyType.Grass) && Works(src, "Green Aroma");
            }

            public override bool CuresConditionsItPrevents { get { return true; } }
        }

        internal static int ConditionCount(PokemonInPlay m)
        {
            int n = 0;
            foreach (SpecialCondition s in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                if (m.Has(s)) n++;
            return n;
        }

        /// <summary>Extreme Speed (Poké-Body): Retreat Cost [C] less for each Fire Energy attached. Overrun.</summary>
        private sealed class ArcanineExtremeSpeed : BodyMon
        {
            public ArcanineExtremeSpeed() : base(new Fx().After("Overrun", (c, a) => HitBenched(c, 1, 20, "Overrun"))) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon != src.Pokemon || !Works(src, "Extreme Speed")) return cost;
                return Math.Max(0, cost - src.Engine.EnergyCountOfType(mon, EnergyType.Fire));
            }
        }

        /// <summary>Compound Eyes (Poké-Body): if the opponent's Active Pokémon has a Poké-Body, its attacks do 30 more to it. Select Powder.</summary>
        private sealed class ButterfreeCompoundEyes : BodyMon
        {
            public ButterfreeCompoundEyes() : base(new Fx().After("Select Powder", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    bool burn = c.ChooseOption("Select Powder: which Special Condition?", "Burned", "Poisoned") == 0;
                    c.ApplyCondition(d, burn ? SpecialCondition.Burned : SpecialCondition.Poisoned);
                })) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                if (info.Attacker != src.Pokemon || t == null || t != src.Opp.Active || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return t.Def.Abilities.Any(a => a.Kind == AbilityDef.PokeBody) && Works(src, "Compound Eyes") ? damage + 30 : damage;
            }
        }

        /// <summary>Thick Fat (Poké-Body): attack damage from Fire and Water Pokémon is 30 less. Ice Shard.</summary>
        private sealed class DewgongThickFat : BodyMon
        {
            public DewgongThickFat() : base(new Fx().Damage("Ice Shard", (c, a) => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Fighting) ? 80 : a.Damage)) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var at = info.Attacker;
                if (info.Target != src.Pokemon || at == null || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!src.Engine.IsType(at, EnergyType.Fire) && !src.Engine.IsType(at, EnergyType.Water)) return damage;
                return Works(src, "Thick Fat") ? Math.Max(0, damage - 30) : damage;
            }
        }

        /// <summary>Erasing Sound (Poké-Body): your Pokémon have no Weakness. Knock Back. Hyper Beam.</summary>
        private sealed class ExploudErasingSound : BodyMon
        {
            public ExploudErasingSound() : base(new ComboMon(new CardsDRI1.WhirlwindMon("Knock Back"), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"))) { }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && mon.Owner == src.Player && Works(src, "Erasing Sound"); }
        }

        /// <summary>Marvel Eyes (Poké-Body): with Solrock in play, prevent all effects of attacks by the opponent's Pokémon LV.X on Lunatone and Solrock. Gravity Wave.</summary>
        private sealed class LunatoneMarvelEyes : BodyMon
        {
            public LunatoneMarvelEyes() : base(new Fx().After("Gravity Wave", (c, a) => HitEachBenched(c, 30, m => c.Engine.RetreatCostOf(m) == 0))) { }

            private static bool Covered(EffectContext src, PokemonInPlay target)
            {
                return target != null && target.Owner == src.Player && (target == src.Pokemon || target.Name == "Solrock") && src.MyPokemon(m => m.Name == "Solrock").Count > 0 && Works(src, "Marvel Eyes");
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return Covered(src, info.Target) && info.Attacker != null && info.Attacker.Owner != src.Player && info.Attacker.Def.IsLevelUp && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var a = src.Opp.Active;
                return kind == EffectSourceKind.Attack && Covered(src, target) && a != null && a.Def.IsLevelUp;
            }
        }

        /// <summary>Focus Wall (Poké-Body): if a 70-or-more damage attack would Knock It Out, it stays at 10 HP. Desperate Slap.</summary>
        private sealed class MrMimeFocusWall : BodyMon
        {
            public MrMimeFocusWall() : base(new Fx().Damage("Desperate Slap", (c, a) => a.Damage + (c.Pokemon.DamageCounters >= 5 ? 40 : 0))) { }

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                return mon == src.Pokemon && mon.AttackDamageTurn == src.Turn && mon.AttackDamageThatTurn >= 70 && Works(src, "Focus Wall");
            }
        }

        /// <summary>Natural Cure (Poké-Body): attaching an Energy card from your hand to it removes its Special Conditions. Magical Leaf.</summary>
        private sealed class RoseradeNaturalCure : BodyMon
        {
            public RoseradeNaturalCure() : base(new Fx().On("Magical Leaf", (c, a) =>
                {
                    bool heads = c.Flip();
                    c.DamageDefending(a.Damage + (heads ? 20 : 0));
                    if (heads && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 20);
                    return true;
                })) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == src.Pokemon && Affected(target) && Works(src, "Natural Cure")) src.Engine.RemoveAllConditions(target);
            }
        }

        /// <summary>Marvel Shell (Poké-Body): prevent all effects of attacks by the opponent's Pokémon that have Poké-Powers or Poké-Bodies. Spike Wound.</summary>
        private sealed class ShedinjaMarvelShell : BodyMon
        {
            public ShedinjaMarvelShell() : base(new Fx().Effect("Spike Wound", (c, a) => HitAny(c, 30, "Spike Wound", m => m.DamageCounters > 0))) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && info.Attacker.Owner != src.Player && HasPowerOrBody(info.Attacker) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Marvel Shell");
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack && HasPowerOrBody(src.Opp.Active) && Works(src, "Marvel Shell");
            }
        }

        /// <summary>Burning Spirit (Poké-Body): attack damage to a Burned Pokémon is 40 more (only 40 from all). Jet Shoot.</summary>
        private sealed class BlazikenBurningSpirit : BodyMon
        {
            public BlazikenBurningSpirit() : base(new Fx().After("Jet Shoot", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new TakenChangeEffect(c.Pokemon.Name + " takes 40 more damage next turn (Jet Shoot)", c.Player, c.Pokemon.Uid, c.Turn + 1, 40));
                })) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target == null || !info.Target.Has(SpecialCondition.Burned) || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!Works(src, "Burning Spirit")) return damage;
                var all = src.MyPokemon().Concat(src.OpponentPokemon()).Where(m => m.Def.Abilities.Any(x => x.Name == "Burning Spirit")).OrderBy(m => m.PlaySeq).ToList();
                return all.Count > 0 && all[0] == src.Pokemon ? damage + 40 : damage;
            }
        }

        /// <summary>Aqua Mirage (Poké-Body): with no cards in your hand, prevent all attack damage from the opponent's Pokémon. Dwindling Wave.</summary>
        private sealed class MiloticAquaMirage : BodyMon
        {
            public MiloticAquaMirage() : base(new CardsSS.MinusSelfCountersMon("Dwindling Wave")) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && src.Me.Hand.Count == 0 && info.Attacker != null && info.Attacker.Owner != src.Player && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Aqua Mirage");
            }
        }

        // ------------------------------------------------------------------ Trainers

        /// <summary>Battle Tower (Stadium): leveling up a Pokémon from the hand removes 4 damage counters from it.</summary>
        private sealed class BattleTower : CardScript
        {
            public override void OnPokemonEvolvedFromHandUnderStadium(EffectContext src, PokemonInPlay evolved, CardDef before)
            {
                if (evolved != null && evolved.Def.IsLevelUp && evolved.DamageCounters > 0) src.Heal(evolved, 40);
            }
        }

        /// <summary>Champion's Room (Stadium): each Pokémon SP's Retreat Cost is [C] less.</summary>
        private sealed class ChampionsRoom : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return SP(mon) ? Math.Max(0, cost - 1) : cost; }
        }

        /// <summary>Cynthia's Guidance (Supporter): look at the top 7 cards, put 1 into your hand, shuffle.</summary>
        private sealed class CynthiasGuidance : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                ctx.DeckToHand(ctx.ChooseCards(top, 1, 1, "Cynthia's Guidance: put which card into your hand?"));
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Cyrus's Initiative (Supporter): flip 2; with any heads, look at the opponent's hand and put 1 card per heads on the bottom of their deck.</summary>
        private sealed class CyrussInitiative : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 0 || ctx.Opp.Hand.Count == 0) return;
                LookAtHand(ctx, ctx.Opp);
                int k = Math.Min(heads, ctx.Opp.Hand.Count);
                var picks = ctx.ChooseCards(ctx.Opp.Hand.ToList(), k, k, "Cyrus's Initiative: put " + k + " card(s) from your opponent's hand on the bottom of their deck");
                foreach (var c in picks) { ctx.Opp.Hand.Remove(c); ctx.Opp.Deck.Add(c); }
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + picks.Count + " card(s) from their hand on the bottom of their deck (Cyrus's Initiative).");
            }
        }

        /// <summary>Night Teleporter: flip; heads, put your hand on top of your deck, search it for any 1 card, shuffle.</summary>
        private sealed class NightTeleporter : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count + ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                foreach (var c in ctx.Me.Hand.Where(c => c != ctx.Card).ToList()) { ctx.Me.Hand.Remove(c); ctx.Me.Deck.Insert(0, c); }
                ctx.SearchToHand(d => true, 1, "Night Teleporter: search your deck for any card");
            }
        }

        /// <summary>Palmer's Contribution (Supporter): shuffle up to 5 Pokémon and basic Energy cards from your discard pile into your deck.</summary>
        private sealed class PalmersContribution : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsPokemon || d.IsBasicEnergy; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => Fits(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => Fits(c.Def)).ToList();
                ctx.DiscardToDeck(ctx.ChooseCards(pile, 1, Math.Min(5, pile.Count), "Palmer's Contribution: shuffle up to 5 Pokémon and basic Energy cards into your deck"));
            }
        }
    }
}
