using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Arceus (AR, 2009; v173): the cards not already in the pool (the others are prints). Unlimited only. Rares, LV.X, the
    /// Arceus cards and the SH cards here; Uncommons, Commons and Trainers in CardsARB.cs.
    /// </summary>
    public static partial class CardsAR
    {
        private static bool Fossil(CardDef d) { return d.Name == "Helix Fossil" || d.Name == "Dome Fossil" || d.Name == "Old Amber"; }

        private static bool HasTool(PokemonInPlay m) { return m != null && m.Tools.Any(t => t.Def.IsTool); }

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("AR-1", new CharizardFireFormation());
            r.RegisterByKey("AR-2", new CardsDS.EvolutionaryMon("Snow Gift", c => c.SearchToHand(d => true, 1, "Snow Gift: search your deck for any card"), c => c.Me.Deck.Count > 0, new Fx()
                .On("Ground Frost", (c, a) => c.State.Stadium != null ? DidNothing(c) : c.DamageDefending(a.Damage) >= 0)));
            r.RegisterByKey("AR-3", new Fx()
                .With(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned))
                .On("Magma Mantle", (c, a) =>
                {
                    var top = c.Engine.DiscardTopOfDeck(c.Player, 3);
                    int n = top.Count(x => CardsRS.BasicOf(x.Def, EnergyType.Fire) || CardsRS.BasicOf(x.Def, EnergyType.Metal));
                    c.DamageDefending(a.Damage + 20 * n);
                    return true;
                }));
            r.RegisterByKey("AR-4", new Fx()
                .On("Primal Scythe", (c, a) =>
                {
                    var fossils = c.Me.Hand.Where(x => Fossil(x.Def)).ToList();
                    bool more = fossils.Count > 0 && c.AskYesNo("Primal Scythe: discard Helix Fossil, Dome Fossil or Old Amber from your hand for 50 more damage?");
                    if (more) c.Engine.DiscardFromHand(c.Player, c.ChooseCards(fossils, 1, 1, "Primal Scythe: discard which card?")[0]);
                    c.DamageDefending(a.Damage + (more ? 50 : 0));
                    return true;
                })
                .After("Rock Slide", (c, a) => HitBenched(c, 2, 10, "Rock Slide")));
            r.RegisterByKey("AR-5", new Fx()
                .After("Flash", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Flash)", c.Player, d.Uid, c.Turn + 1));
                })
                .On("Gadget Bolt", (c, a) =>
                {
                    bool big = HasTool(c.Pokemon) && c.AskYesNo("Gadget Bolt: discard " + c.Pokemon.Name + "'s Pokémon Tool to do 100 damage?");
                    if (big) c.Engine.DiscardTool(c.Pokemon, c.Pokemon.Tools.First(t => t.Def.IsTool));
                    c.DamageDefending(big ? 100 : a.Damage);
                    return true;
                }));
            r.RegisterByKey("AR-6", new Fx()
                .Effect("Cloak Party", (c, a) => c.SearchToHand(d => d.IsPokemon && (d.Name.StartsWith("Burmy") || d.Name.StartsWith("Wormadam")), 3, "Cloak Party: search your deck for up to 3 Burmy and Wormadam"))
                .After("Energy Assist", (c, a) =>
                {
                    var pile = c.BasicEnergyInDiscard();
                    if (pile.Count > 0 && c.MyBench().Count > 0) c.AttachCardsAnyWay(pile, 1, (x, m) => m != c.Me.Active, "Energy Assist: attach a basic Energy card to 1 of your Benched Pokémon", false);
                }));
            r.RegisterByKey("AR-7", new ProbopassCompetitiveness());
            r.RegisterByKey("AR-8", new PowerFx("Top Accelerator", c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.Me.Deck[0];
                    c.Engine.ShowCards(c.Opponent, new List<CardInstance> { top }, "Top Accelerator", "Top Accelerator: the top card of your opponent's deck", c.Source);
                    c.Log("Top Accelerator revealed " + top.Def.Name + ".");
                    if (top.Def.IsBasicEnergy) c.AttachCardsAnyWay(new List<CardInstance> { top }, 1, (x, m) => true, "Top Accelerator: attach " + top.Def.Name + " to 1 of your Pokémon", false);
                    else c.Engine.DiscardTopOfDeck(c.Player, 1);
                }, new Fx().After("Shoot Through", (c, a) => HitBenched(c, 1, 20, "Shoot Through"))));
            r.RegisterByKey("AR-9", new Fx()
                .Effect("Damage Roller", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null) return;
                    int gap = c.RemainingHp(d) - c.RemainingHp(c.Pokemon);
                    if (gap <= 0 || !c.CanAffect(d)) { DidNothing(c); return; }
                    c.PlaceCounters(d, gap / 10);
                })
                .On("Entangle Tackle", (c, a) =>
                {
                    bool more = c.AskYesNo("Entangle Tackle: do 30 more damage (and 30 damage to 1 of your Pokémon)?");
                    c.DamageDefending(a.Damage + (more ? 30 : 0));
                    if (more)
                    {
                        var t = c.ChoosePokemon(c.MyPokemon(), "Entangle Tackle: do 30 damage to which of your Pokémon?");
                        if (t != null) c.DamageWith(t, 30, false, false, false);
                    }
                    return true;
                }));
            r.RegisterByKey("AR-10", new Fx()
                .With(new ShieldAttackMon("Leaf Guard", 20))
                .Damage("Swallow Up", (c, a) => c.Defending != null && c.RemainingHp(c.Defending) < c.RemainingHp(c.Pokemon) ? 120 : a.Damage));
            r.RegisterByKey("AR-11", new Fx()
                .After("Corner", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); })
                .On("Convert Blow", (c, a) =>
                {
                    var me = c.Pokemon;
                    bool psy = me.Energy.Any(e => CardsDRI1.ProvidesType(c, me, e, EnergyType.Psychic));
                    bool fight = me.Energy.Any(e => CardsDRI1.ProvidesType(c, me, e, EnergyType.Fighting));
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (fight ? 30 : 0));
                    if (psy && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                    return true;
                }));
            r.RegisterByKey("AR-12", new Fx()
                .After("Charge Beam", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsEnergy).ToList();
                    if (pile.Count == 0 || !c.InPlay(c.Pokemon) || !c.Flip()) return;
                    var card = c.ChooseCards(pile, 1, 1, "Charge Beam: attach which Energy card?")[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .On("Lightning Strike", (c, a) =>
                {
                    bool big = c.Pokemon.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Lightning)) && c.AskYesNo("Lightning Strike: discard all Lightning Energy for 80 damage?");
                    if (big) DiscardTypeFromSelf(c, EnergyType.Lightning, 99);
                    c.DamageDefending(big ? 80 : a.Damage);
                    return true;
                }));
            r.RegisterByKey("AR-13", new PowerFx("Unearth", c => c.Me.Deck.Count > 0, c => c.SearchToHand(Fossil, 1, "Unearth: search your deck for Helix Fossil, Dome Fossil or Old Amber"),
                new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("AR-14", new Fx()
                .Effect("Oracle Arrow", (c, a) => HitAny(c, 40, "Oracle Arrow", m => m.DamageCounters > 0))
                .After("Antigravity", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new RetreatChangeEffect(c.Pokemon.Name + " has no Retreat Cost until the end of your next turn (Antigravity)", c.Player, c.Pokemon.Uid, c.Turn + 2, null));
                }));
            r.RegisterByKey("AR-15", new CherrimCloudySky());
            r.RegisterByKey("AR-16", new PowerFx("Curse", c => c.OpponentPokemon(m => m.DamageCounters > 0).Count > 0 && c.OpponentPokemon().Count > 1, c =>
                {
                    var from = c.ChoosePokemon(c.OpponentPokemon(m => m.DamageCounters > 0), "Curse: move 1 damage counter from which of your opponent's Pokémon?");
                    if (from == null) return;
                    var to = c.ChoosePokemon(c.OpponentPokemon(m => m != from), "Curse: move it to which of your opponent's Pokémon?");
                    if (to != null) c.Engine.MoveDamageCounters(from, to, 1, "Curse");
                }, new Fx().After("Shadow Skip", (c, a) =>
                {
                    HitBenched(c, 1, 10, "Shadow Skip");
                    if (c.InPlay(c.Pokemon) && c.MyBench().Count > 0 && c.AskYesNo("Shadow Skip: switch " + c.Pokemon.Name + " with 1 of your Benched Pokémon?")) c.SwitchSelfWithBench();
                })));
            r.RegisterByKey("AR-17", new Fx()
                .Effect("Sharpshooting", (c, a) => HitAny(c, 40, "Sharpshooting"))
                .With(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("AR-18", new Fx()
                .On("Wreck", (c, a) =>
                {
                    bool stadium = c.State.Stadium != null;
                    c.DamageDefending(a.Damage + (stadium ? 50 : 0));
                    if (stadium && c.State.Stadium != null) c.Engine.DiscardStadium();
                    return true;
                })
                .After("Avalanche", (c, a) => { if (c.Flip()) HitEachBenched(c, 10); }));
            r.RegisterByKey("AR-19", new Fx()
                .Effect("Tumble Down", (c, a) =>
                {
                    var fighting = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fighting)).ToList();
                    var picks = fighting.Count == 0 ? new List<CardInstance>() : c.ChooseCards(fighting, 0, fighting.Count, "Tumble Down: discard as many Fighting Energy cards from your hand as you like (30 damage each)");
                    foreach (var x in picks) c.Engine.DiscardFromHand(c.Player, x);
                    c.DamageDefending(30 * picks.Count);
                })
                .Effect("Rock Tumble", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); }));
            r.RegisterByKey("AR-20", new Fx()
                .With(new CardsDRI1.WhirlwindMon("Push Out"))
                .Damage("Vortex Chop", (c, a) => c.Defending != null && c.Engine.ResistanceOf(c.Defending) != null ? 120 : a.Damage));
            r.RegisterByKey("AR-21", new PowerFx("Relaxing Shower", c => c.Me.Hand.Any(x => x.Def.IsEnergy), c =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                    c.Engine.DiscardFromHand(c.Player, c.ChooseCards(energy, 1, 1, "Relaxing Shower: discard which Energy card?")[0]);
                    HealEach(c, 10);
                }, new Fx().After("Hover Heal", (c, a) => { if (c.InPlay(c.Pokemon)) c.Engine.RemoveAllConditions(c.Pokemon); })));
            r.RegisterByKey("AR-22", new Fx()
                .Damage("Second Bite", (c, a) => a.Damage + 10 * (c.Defending == null ? 0 : c.Defending.DamageCounters))
                .Effect("Swift", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, true); }));
            r.RegisterByKey("AR-23", new Fx()
                .Effect("Time Spiral", (c, a) =>
                {
                    var pool = c.OpponentPokemon(CardsE2.IsEvolved);
                    if (pool.Count == 0) return;
                    var t = c.ChoosePokemon(pool, "Time Spiral: choose 1 of your opponent's Evolved Pokémon");
                    if (t != null && c.CanAffect(t)) c.Engine.Devolve(t, 1, true);
                })
                .Damage("Primal Tentacles", (c, a) => a.Damage + 10 * c.Me.Discard.Count(x => Fossil(x.Def))));
            r.RegisterByKey("AR-24", new Fx()
                .With(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep))
                .After("Continuous Crush", (c, a) =>
                {
                    int heads = c.FlipUntilTails();
                    var d = c.Defending;
                    for (int i = 0; i < heads && d != null && c.InPlay(d) && d.Energy.Count > 0 && c.CanAffect(d); i++)
                        c.DiscardEnergyFrom(d, 1, "Continuous Crush: discard an Energy card attached to " + d.Name);
                }));
            r.RegisterByKey("AR-25", new CardsSS.BabyEvolutionMon("Pikachu", new Fx().Effect("Baby Steps", (c, a) =>
                {
                    var top = c.LookAtTop(5);
                    if (top.Count == 0) return;
                    c.DeckToHand(c.ChooseCards(top, 1, 1, "Baby Steps: put which card into your hand?"));
                    c.ShuffleMyDeck();
                })));
            r.RegisterByKey("AR-26", new BenchTriggerFx("Dowsing Code", c => c.Me.Discard.Any(x => x.Def.IsTool), c =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsTool).ToList();
                    c.DiscardToDeck(c.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Dowsing Code: shuffle up to 2 Pokémon Tool cards into your deck"));
                }, new FlipBonusMon("Ambush", 40)));
            r.RegisterByKey("AR-27", new Fx()
                .After("Boosted Voltage", (c, a) => { if (HasTool(c.Pokemon)) HitEachBenched(c, 20, m => !CardsE2.IsEvolved(m)); })
                .After("Thunder Blast", (c, a) => DiscardTypeFromSelf(c, EnergyType.Lightning, 1)));
            r.RegisterByKey("AR-28", new RapidashWildGuard());
            r.RegisterByKey("AR-29", new Fx()
                .Effect("Recruit", (c, a) =>
                {
                    LookAtHand(c, c.Opp);
                    var sups = c.Opp.Hand.Where(x => x.Def.IsSupporter).ToList();
                    if (sups.Count == 0) return;
                    var card = c.ChooseCards(sups, 1, 1, "Recruit: discard which Supporter card (and use its effect)?")[0];
                    c.Engine.DiscardFromHand(c.Opponent, card);
                    var tctx = new EffectContext(c.Engine, c.Player, card, null);
                    var script = c.Engine.Scripts.For(card.Def);
                    if (script.CanPlayTrainer(tctx)) script.ResolveTrainer(tctx);
                })
                .Damage("Extend Fang", (c, a) => a.Damage + (HasTool(c.Pokemon) ? 40 : 0)));
            r.RegisterByKey("AR-30", new SceptileGreenBreath());
            r.RegisterByKey("AR-31", new Fx()
                .After("Leaf Supply", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsBasicEnergy).ToList();
                    if (energy.Count > 0) c.AttachCardsAnyWay(energy, 1, (x, m) => true, "Leaf Supply: you may attach a basic Energy card from your hand", true);
                })
                .With(new CoinsMon("Dual Cut", 2)));
            r.RegisterByKey("AR-32", new SpiritombKeystoneSeal());

            r.RegisterByKey("AR-96", new ArceusMultitype(new Fx().After("Psychic Bolt", (c, a) =>
                {
                    DiscardTypeFromSelf(c, EnergyType.Lightning, 1);
                    DiscardTypeFromSelf(c, EnergyType.Psychic, 1);
                })));
            r.RegisterByKey("AR-97", new PowerFx("Level-Down", c => c.OpponentPokemon(m => m.Def.IsLevelUp && m.Stack.Count >= 2).Count > 0, c =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(m => m.Def.IsLevelUp && m.Stack.Count >= 2), "Level-Down: choose 1 of your opponent's Pokémon LV.X");
                    if (t != null && c.CanAffect(t)) c.Engine.Devolve(t, 1, true);
                }, new Fx().Effect("Compound Pain", (c, a) =>
                {
                    foreach (var m in c.OpponentPokemon(x => x.DamageCounters > 0).ToList()) c.DamageTarget(m, 30);
                })));
            r.RegisterByKey("AR-98", new SalamenceDoubleFall());
            r.RegisterByKey("AR-99", new PowerFx("Healing Growth", c => c.MyPokemon(m => m.DamageCounters > 0).Count > 0, c =>
                {
                    if (!c.Flip()) return;
                    var t = c.ChoosePokemon(c.MyPokemon(m => m.DamageCounters > 0), "Healing Growth: remove 4 damage counters from which Pokémon?");
                    if (t != null) c.Heal(t, 40);
                }, new Fx().Effect("Big Growth", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Grass)).ToList();
                    if (pile.Count > 0) c.AttachCardsAnyWay(pile, pile.Count, (x, m) => true, "Big Growth: attach Grass Energy cards from your discard pile", true);
                })));

            r.RegisterByKey("AR-AR1", new Fx().Damage("Prize Count", (c, a) => a.Damage + (c.Me.Prizes.Count > c.Opp.Prizes.Count ? 60 : 0)));
            r.RegisterByKey("AR-AR2", new Fx().After("Leaf Refresh", (c, a) => HealEach(c, 30, true)));
            r.RegisterByKey("AR-AR3", new Fx().After("Bright Flame", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon) && !c.Flip()) c.DiscardEnergyFrom(c.Pokemon, 2, "Bright Flame: discard 2 Energy attached to " + c.Pokemon.Name);
                }));
            r.RegisterByKey("AR-AR4", new Fx().Effect("Fastwave", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, true); }));
            r.RegisterByKey("AR-AR5", new Fx()
                .Effect("Ripple Swell", (c, a) =>
                {
                    var arceus = c.MyPokemon(m => m.Name.StartsWith("Arceus"));
                    var types = arceus.Select(m => c.Engine.TypesOf(m).FirstOrDefault()).Distinct().Count();
                    if (arceus.Count < 6 || types < 6) { DidNothing(c); return; }
                    var picks = c.SearchMyDeck(d => d.IsBasicEnergy, 6, "Ripple Swell: search your deck for up to 6 basic Energy cards");
                    var used = new HashSet<int>();
                    foreach (var card in picks)
                    {
                        var free = c.MyPokemon(m => !used.Contains(m.Uid));
                        var t = free.Count == 0 ? null : c.ChoosePokemon(free, "Ripple Swell: attach " + card.Def.Name + " to which Pokémon?");
                        if (t == null || !c.Engine.AttachEnergyCard(card, t, false)) { c.Me.Deck.Add(card); continue; }
                        used.Add(t.Uid);
                    }
                    c.ShuffleMyDeck();
                })
                .Effect("Sky Spear", (c, a) =>
                {
                    HitAny(c, 80, "Sky Spear");
                    if (c.InPlay(c.Pokemon)) foreach (var e in c.Pokemon.Energy.ToList()) EnergyToLostZone(c, c.Pokemon, e);
                }));
            r.RegisterByKey("AR-AR6", new SwitchSelfMon("Lightning Turn", false));
            r.RegisterByKey("AR-AR7", new ConditionAttackMon("Mind Bend", SpecialCondition.Confused));
            r.RegisterByKey("AR-AR8", new Fx().After("Break Ground", (c, a) => { foreach (var m in c.MyBench().ToList()) c.DamageTarget(m, 10); }));
            r.RegisterByKey("AR-AR9", new Fx().After("Metal Barrier", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new ShieldFromEffect(c.Pokemon.Name + " is protected from Pokémon LV.X (Metal Barrier)", c.Player, c.Pokemon.Uid, c.Turn + 1, (eng, m) => m.Def.IsLevelUp));
                }));

            r.RegisterByKey("AR-SH10", new StarBarrier(new Fx().On("Dragon Rage", (c, a) => c.FlipCoins(2) == 2 ? c.DamageDefending(a.Damage) >= 0 : DidNothing(c))));
            r.RegisterByKey("AR-SH11", new StarBarrier(new Fx().After("Blue Fire", (c, a) => DiscardTypeFromSelf(c, EnergyType.Fire, 99))));
            r.RegisterByKey("AR-SH12", new StarBarrier(new Fx().Damage("Payback", (c, a) => a.Damage + (c.Opp.Prizes.Count == 1 ? 30 : 0))));
            RegisterCommons(r);
        }

        /// <summary>Fire Formation (Poké-Body): its attacks do 10 more to the opponent's Active Pokémon for each Fire Pokémon on your Bench. Burning Tail.</summary>
        private sealed class CharizardFireFormation : BodyMon
        {
            public CharizardFireFormation() : base(new Fx().After("Burning Tail", (c, a) => DiscardTypeFromSelf(c, EnergyType.Fire, 1))) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || info.Target == null || info.Target != src.Opp.Active || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!Works(src, "Fire Formation")) return damage;
                return damage + 10 * src.MyBench().Count(m => src.Engine.IsType(m, EnergyType.Fire));
            }
        }

        /// <summary>Competitiveness (Poké-Body): without a Supporter in play, its attacks do 30 more to the Active Pokémon. Top Chop. Tumbling Attack.</summary>
        private sealed class ProbopassCompetitiveness : BodyMon
        {
            public ProbopassCompetitiveness() : base(new Fx()
                .After("Top Chop", (c, a) => c.Draw(1))
                .With(new FlipBonusMon("Tumbling Attack", 30))) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || info.Target == null || info.Target != src.Opp.Active || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return !src.Me.SupporterPlayedThisTurn && Works(src, "Competitiveness") ? damage + 30 : damage;
            }
        }

        /// <summary>Cloudy Sky (Poké-Body): attack damage to your Grass and Fire Pokémon is 10 less. Worry Seed.</summary>
        private sealed class CherrimCloudySky : BodyMon
        {
            public CherrimCloudySky() : base(new CardsPAR2.FlipConditionMon("Worry Seed", SpecialCondition.Confused)) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                if (t == null || t.Owner != src.Player || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!src.Engine.IsType(t, EnergyType.Grass) && !src.Engine.IsType(t, EnergyType.Fire)) return damage;
                return Works(src, "Cloudy Sky") ? Math.Max(0, damage - 10) : damage;
            }
        }

        /// <summary>Wild Guard (Poké-Body): prevent all effects of attacks by the opponent's Pokémon SP. Rising Lunge.</summary>
        private sealed class RapidashWildGuard : BodyMon
        {
            public RapidashWildGuard() : base(new FlipBonusMon("Rising Lunge", 20)) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && info.Attacker.Owner != src.Player && SP(info.Attacker) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Wild Guard");
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack && SP(src.Opp.Active) && Works(src, "Wild Guard");
            }
        }

        /// <summary>Green Breath (Poké-Body): attaching a Grass Energy card from your hand to it removes 2 damage counters. Leaf Blast. Poison Claws.</summary>
        private sealed class SceptileGreenBreath : BodyMon
        {
            public SceptileGreenBreath() : base(new Fx()
                .Damage("Leaf Blast", (c, a) => 20 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Grass))
                .With(new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned))) { }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == src.Pokemon && CardsRS.BasicOf(energy.Def, EnergyType.Grass) && target.DamageCounters > 0 && Works(src, "Green Breath")) src.Heal(target, 20);
            }
        }

        /// <summary>Keystone Seal (Poké-Body): while Spiritomb is your Active Pokémon, neither player can play Trainer cards (Items). Darkness Grace.</summary>
        private sealed class SpiritombKeystoneSeal : BodyMon
        {
            public SpiritombKeystoneSeal() : base(new Fx().Effect("Darkness Grace", (c, a) =>
                {
                    var names = c.MyPokemon().Select(m => m.Name).ToList();
                    var found = c.SearchMyDeck(d => d.IsPokemon && d.IsEvolution && !d.IsLevelUp && names.Contains(d.EvolvesFrom), 1, "Darkness Grace: search your deck for a card that evolves from 1 of your Pokémon");
                    if (found.Count > 0)
                    {
                        var targets = c.MyPokemon(m => c.Engine.CanEvolveInto(m, found[0].Def, false));
                        var t = targets.Count == 0 ? null : c.ChoosePokemon(targets, "Darkness Grace: evolve which Pokémon?");
                        if (t != null)
                        {
                            c.Engine.EvolvePokemon(t, found[0]);
                            if (c.InPlay(c.Pokemon)) c.PlaceCounters(c.Pokemon, 1);
                        }
                        else c.Me.Deck.Add(found[0]);
                    }
                    c.ShuffleMyDeck();
                })) { }

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return card != null && CardsDPEra.DpTrainer(card.Def) && src.PokemonIsActive && Works(src, "Keystone Seal");
            }
        }

        /// <summary>Multitype (Poké-Body): this Arceus LV.X is the type of the Arceus under it.</summary>
        private sealed class ArceusMultitype : BodyMon
        {
            public ArceusMultitype(Mon attacks) : base(attacks) { }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon != src.Pokemon || mon.Stack.Count < 2) return;
                var below = mon.Stack[mon.Stack.Count - 2].Def;
                if (!below.IsPokemon) return;
                types.Clear();
                types.Add(below.PokemonType);
                if (below.SecondType.HasValue && below.SecondType.Value != below.PokemonType) types.Add(below.SecondType.Value);
            }
        }

        private const string DoubleFallMarker = "Double Fall";

        /// <summary>Double Fall (Poké-Power): leveled up from the hand, each Pokémon its attacks Knock Out this turn gives 1 more Prize card. Steam Blast.</summary>
        private sealed class SalamenceDoubleFall : BodyMon
        {
            public SalamenceDoubleFall() : base(new Fx().After("Steam Blast", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.DiscardEnergyFrom(c.Pokemon, 1, "Steam Blast: discard an Energy attached to " + c.Pokemon.Name);
                })) { }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!ctx.PokemonIsActive || !TriggerOnce(ctx, "Double Fall", true, "Double Fall: take 1 more Prize card for each Pokémon Salamence Knocks Out this turn?")) return;
                ctx.Pokemon.Markers[DoubleFallMarker] = ctx.Turn;
            }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                int t;
                var me = src.Pokemon;
                if (knockedOut == null || knockedOut.Owner == src.Player || taker != src.Player || me == null || !me.Markers.TryGetValue(DoubleFallMarker, out t) || t != src.Turn) return prizes;
                return knockedOut.HitByAttackerUid == me.Uid ? prizes + 1 : prizes;
            }
        }

        /// <summary>Star Barrier (Poké-Body): with any Energy attached, no Weakness.</summary>
        private sealed class StarBarrier : BodyMon
        {
            public StarBarrier(Mon attacks) : base(attacks) { }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon == src.Pokemon && mon.Energy.Count > 0 && Works(src, "Star Barrier"); }
        }
    }
}
