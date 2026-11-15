using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>Platinum (PL; v173): Uncommons, Commons and the SH cards.</summary>
    public static partial class CardsPL
    {
        private static void RegisterCommons(CardScriptRegistry r)
        {
            r.RegisterByKey("PL-18", new Fx()
                .On("Midnight Eyes", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null) return true;
                    int done = c.DamageDefending(a.Damage);
                    if (!c.InPlay(d) || !c.CanAffect(d)) return true;
                    // Perish Song checks that this same Pokémon (leveling up keeps it; evolving doesn't) was hit by Midnight Eyes.
                    if (c.ApplyCondition(d, SpecialCondition.Asleep) || done > 0) d.Markers["Midnight Eyes"] = c.Turn;
                    return true;
                })
                .Effect("Perish Song", (c, a) =>
                {
                    var d = c.Defending;
                    int t;
                    if (d == null || !d.Has(SpecialCondition.Asleep) || !c.CanAffect(d)) return;
                    if (d.Markers.TryGetValue("Midnight Eyes", out t) && t == c.MyLastTurn) c.Engine.KnockOut(d, "Perish Song");
                })
                .After("Healing Song", (c, a) => HealEach(c, 10)));
            r.RegisterByKey("PL-19", new PowerFx("Temper Tantrum", c => c.Me.Hand.Count > 0, c =>
                {
                    var picks = c.ChooseCards(c.Me.Hand.ToList(), 1, c.Me.Hand.Count, "Temper Tantrum: discard as many cards from your hand as you like");
                    foreach (var x in picks) c.Engine.DiscardFromHand(c.Player, x);
                    if (picks.Count > 0) c.Engine.PlaceDamageCounters(c.Pokemon, picks.Count, "Temper Tantrum");
                }, new Fx()
                .Effect("Darkness Switch", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.Pokemon.Energy.Count == 0 || d == null || !c.CanAffect(d)) { DidNothing(c); return; }
                    if (c.DiscardEnergyFrom(c.Pokemon, 1, "Darkness Switch: discard an Energy card attached to " + c.Pokemon.Name) == 0) return;
                    int mine = c.Pokemon.DamageCounters;
                    c.Pokemon.DamageCounters = d.DamageCounters;
                    d.DamageCounters = mine;
                    c.Log(c.Pokemon.Name + " and " + d.Name + " switched their damage counters (Darkness Switch).");
                })
                .Damage("Loneliness", (c, a) =>
                {
                    bool show = !c.Me.Hand.Any(x => x.Def.IsPokemon) && c.AskYesNo("Loneliness: show your hand to your opponent (no Pokémon in it: 30 more damage)?");
                    if (show) c.Engine.ShowCards(c.Opponent, c.Me.Hand.ToList(), "Loneliness", "Loneliness: your opponent's hand", c.Source);
                    return a.Damage + (show ? 30 : 0);
                })));
            r.RegisterByKey("PL-20", new BastiodonMetalTrait());
            r.RegisterByKey("PL-21", new Fx()
                .On("Colorful Powder", (c, a) =>
                {
                    int n = c.EnergyCount(c.Pokemon);
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (n >= 4 ? 40 : 0));
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return true;
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                    if (n >= 3) c.ApplyCondition(d, SpecialCondition.Burned);
                    if (n >= 4) c.ApplyCondition(d, SpecialCondition.Asleep);
                    return true;
                })
                .Damage("Silver Scale", (c, a) => c.Defending != null && c.Engine.ResistanceOf(c.Defending) != null ? 60 : a.Damage));
            r.RegisterByKey("PL-22", new PowerFx("Nurse Call", c => c.Me.Hand.Count > 0, c =>
                {
                    var card = c.ChooseCards(c.Me.Hand.ToList(), 1, 1, "Nurse Call: discard a card from your hand")[0];
                    c.Engine.DiscardFromHand(c.Player, card);
                    var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                    var t = hurt.Count == 0 ? null : c.ChoosePokemon(hurt, "Nurse Call: remove 2 damage counters from which Pokémon?");
                    if (t != null) c.Heal(t, 20);
                }, new Fx()
                .After("Return", (c, a) => c.DrawUntil(6))
                .With(new RecoilMon("Double-edge", 60)), false, true));
            r.RegisterByKey("PL-23", new Fx()
                .After("Energy Stream", (c, a) =>
                {
                    var pile = c.BasicEnergyInDiscard();
                    if (pile.Count == 0 || !c.InPlay(c.Pokemon) || !c.Flip()) return;
                    var card = c.ChooseCards(pile, 1, 1, "Energy Stream: attach which basic Energy card?")[0];
                    if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .With(new LockAfterAttackMon("Diamond Blow", false)));
            r.RegisterByKey("PL-24", new DugtrioSinkhole());
            r.RegisterByKey("PL-25", new DustoxCamouflagePattern());
            r.RegisterByKey("PL-26", new Fx()
                .With(new CardsHPB.KnockOffMon("Knock Off", false))
                .Effect("Jet Smash", (c, a) =>
                {
                    HitAny(c, 70, "Jet Smash");
                    if (c.InPlay(c.Pokemon)) c.SelfCantUseAttackNextTurn("Jet Smash");
                }));
            r.RegisterByKey("PL-27", new Fx()
                .With(new SwitchSelfMon("Strafe", true))
                .On("Broken-space Blow", (c, a) => { DamageLostOnKnockOut(c, a.Damage); return true; }));
            r.RegisterByKey("PL-28", new CardsGE.TailsNothingParalyzeMon("Dragonbreath"));
            r.RegisterByKey("PL-29", new Fx()
                .On("Swim", (c, a) =>
                {
                    bool water = c.OpponentPokemon().Any(m => m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Water)));
                    if (water && c.OpponentBench().Count > 0 && c.AskYesNo("Swim: do 30 damage to 1 of your opponent's Benched Pokémon instead?"))
                    {
                        var t = c.ChoosePokemon(c.OpponentBench(), "Swim: do 30 damage to which Benched Pokémon?");
                        if (t != null) c.DamageTarget(t, 30);
                        return true;
                    }
                    c.DamageDefending(a.Damage);
                    return true;
                })
                .On("Water Slide", (c, a) =>
                {
                    bool moved = false;
                    if (c.MyBench().Count > 0 && c.Pokemon.Energy.Count > 0 && c.AskYesNo("Water Slide: move all Energy attached to " + c.Pokemon.Name + " to 1 of your Benched Pokémon (20 more damage)?"))
                    {
                        var to = c.ChoosePokemon(c.MyBench(), "Water Slide: move the Energy to which Benched Pokémon?");
                        if (to != null) foreach (var e in c.Pokemon.Energy.ToList()) moved |= c.Engine.MoveEnergy(c.Pokemon, to, e);
                    }
                    c.DamageDefending(a.Damage + (moved ? 20 : 0));
                    return true;
                }));
            r.RegisterByKey("PL-30", new Fx()
                .Effect("Wriggle", (c, a) =>
                {
                    foreach (var m in c.OpponentPokemon().ToList())
                        if (c.Flip() && c.InPlay(m)) c.DamageTarget(m, 30);
                })
                .With(new CardsSS.MinusSelfCountersMon("Dwindling Wave")));
            r.RegisterByKey("PL-31", new Fx()
                .Effect("Rushing Flames", (c, a) =>
                {
                    var fire = new List<KeyValuePair<PokemonInPlay, CardInstance>>();
                    foreach (var m in c.MyPokemon()) foreach (var e in m.Energy) if (CardsRS.BasicOf(e.Def, EnergyType.Fire)) fire.Add(new KeyValuePair<PokemonInPlay, CardInstance>(m, e));
                    var cards = fire.Select(p => p.Value).ToList();
                    var picks = cards.Count == 0 ? new List<CardInstance>() : c.ChooseCards(cards, 0, cards.Count, "Rushing Flames: discard as many Fire Energy cards attached to your Pokémon as you like");
                    int n = 0;
                    foreach (var e in picks) { var holder = fire.First(p => p.Value == e).Key; if (c.Engine.DiscardEnergy(holder, e)) n++; }
                    int heads = n == 0 ? 0 : c.FlipCoins(n);
                    c.DamageDefending(80 * heads);
                })
                .Damage("Rage", (c, a) => a.Damage + 10 * c.Pokemon.DamageCounters));
            r.RegisterByKey("PL-32", new Fx()
                .Damage("Revenge Melody", (c, a) => 20 * c.Me.Discard.Count(x => x.Def.IsPokemon && (x.Def.Name == "Kricketot" || x.Def.Name == "Kricketune")))
                .On("Bug Buzz", (c, a) =>
                {
                    var d = c.Defending;
                    bool asleep = d != null && d.Has(SpecialCondition.Asleep);
                    c.DamageDefending(a.Damage + (asleep ? 30 : 0));
                    if (d != null && c.InPlay(d) && d.Has(SpecialCondition.Asleep)) c.Engine.RemoveCondition(d, SpecialCondition.Asleep);
                    return true;
                }));
            r.RegisterByKey("PL-33", new Fx()
                .Effect("Harrumph", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Harrumph: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    if (c.CanAffect(t)) foreach (var tool in t.Tools.ToList()) c.Engine.DiscardTool(t, tool);
                    c.DamageTarget(t, 40);
                })
                .After("Body Press", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.Flip() || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Paralyzed);
                    c.DiscardEnergyFrom(d, 1, "Body Press: discard an Energy card attached to " + d.Name);
                }));
            r.RegisterByKey("PL-34", new PowerFx("Cheerful Voice", null, c =>
                {
                    c.AddEffect(new AttackBonusEffect(c.Pokemon.Name + "'s attacks do 60 more damage during your next turn (Cheerful Voice)", c.Player, c.Pokemon.Uid, c.Turn + 2, 60));
                    c.Engine.RequestEndTurn();
                }, new Fx()
                .With(new ConditionAttackMon("Mad Dance", SpecialCondition.Confused))
                .Effect("Best Dance", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    if (c.InPlay(c.Pokemon))
                    {
                        if (done > 0) c.Heal(c.Pokemon, done);
                        c.SelfCantUseAttackNextTurn("Best Dance");
                    }
                })));
            r.RegisterByKey("PL-35", new Fx()
                .Effect("Love Call", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Love Call: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    var types = c.Engine.TypesOf(t);
                    c.SearchToHand(d => d.IsPokemon && types.Any(d.HasType), 1, "Love Call: search your deck for a Pokémon of the same type as " + t.Name);
                })
                .After("Sweet Kiss", (c, a) =>
                {
                    if (c.Opp.Deck.Count > 0 && c.Engine.AskYesNo(c.Opponent, "Sweet Kiss: draw a card?", c.Source)) c.Engine.DrawCards(c.Opponent, 1);
                }));
            r.RegisterByKey("PL-36", new Fx()
                .Effect("Flame Bash", (c, a) =>
                {
                    int heads = c.FlipUntilTails();
                    if (heads > 0) SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Fire), "Flame Bash", heads);
                })
                .After("Mysterious Flames", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    if (c.MyPokemon().Sum(m => c.EnergyCount(m)) <= c.OpponentPokemon().Sum(m => c.EnergyCount(m))) return;
                    c.ApplyCondition(d, SpecialCondition.Burned);
                    c.ApplyCondition(d, SpecialCondition.Confused);
                }));
            r.RegisterByKey("PL-37", new Fx()
                .After("Tsunami", (c, a) => HitEachBenched(c, 10))
                .With(new CardsPAR2.FlipConditionMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("PL-38", new Fx()
                .Damage("Energy Blow", (c, a) => a.Damage + 10 * c.EnergyCount(c.Pokemon))
                .After("Aromatherapy", (c, a) => HealEach(c, 20)));
            r.RegisterByKey("PL-39", new Fx()
                .Damage("Green Blast", (c, a) => a.Damage + 10 * c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Grass)))
                .With(new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep)));
            r.RegisterByKey("PL-40", new ToxicroakAnticipation());
            r.RegisterByKey("PL-41", new PowerFx("Galactic Switch", c => c.Me.InPlay().Count() > 1 && c.MyPokemon(m => SP(m) && m.Energy.Count > 0).Count > 0, c =>
                {
                    var from = c.ChoosePokemon(c.MyPokemon(m => SP(m) && m.Energy.Count > 0), "Galactic Switch: move an Energy card from which Pokémon SP?");
                    if (from == null) return;
                    var to = c.ChoosePokemon(c.MyPokemon(m => m != from), "Galactic Switch: move it to which Pokémon?");
                    if (to == null) return;
                    var e = from.Energy.Count == 1 ? from.Energy[0] : c.ChooseCards(from.Energy.ToList(), 1, 1, "Galactic Switch: move which Energy card?")[0];
                    c.Engine.MoveEnergy(from, to, e);
                    c.Engine.PlaceDamageCounters(c.Pokemon, 2, "Galactic Switch");
                }, new Fx()
                .After("Psychic Pulse", (c, a) => HitEachBenched(c, 10, m => m.DamageCounters > 0))));
            r.RegisterByKey("PL-42", new Fx()
                .Effect("Spike Wound", (c, a) => HitAny(c, 50, "Spike Wound", m => m.DamageCounters > 0))
                .After("Poison Experiment", (c, a) =>
                {
                    var d = c.Defending;
                    if (!c.InPlay(c.Pokemon)) return;
                    var opts = new List<string> { "Don't discard" };
                    bool g = c.Pokemon.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Grass));
                    bool dk = c.Pokemon.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Darkness));
                    if (g) opts.Add("Discard a Grass Energy (Poisoned)");
                    if (dk) opts.Add("Discard a Darkness Energy (Paralyzed)");
                    if (opts.Count == 1) return;
                    var pick = opts[c.ChooseOption("Poison Experiment: discard an Energy?", opts.ToArray())];
                    if (pick.StartsWith("Don't")) return;
                    var type = pick.Contains("Grass") ? EnergyType.Grass : EnergyType.Darkness;
                    if (DiscardTypeFromSelf(c, type, 1) == 0 || d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, type == EnergyType.Grass ? SpecialCondition.Poisoned : SpecialCondition.Paralyzed);
                }));
            r.RegisterByKey("PL-43", new Fx()
                .Effect("Stretch Vine", (c, a) => HitBenched(c, 2, 10, "Stretch Vine"))
                .With(new CardsTWM1.HealSelfMon("Absorb", 20))
                .On("Vine Extract", (c, a) =>
                {
                    var d = c.Defending;
                    bool already = d != null && d.DamageCounters > 0;
                    c.DamageDefending(a.Damage);
                    if (already && c.InPlay(d) && c.CanAffect(d))
                    {
                        c.ApplyCondition(d, SpecialCondition.Burned);
                        c.ApplyCondition(d, SpecialCondition.Poisoned);
                    }
                    return true;
                }));
            r.RegisterByKey("PL-44", new ComboMon(new CardsSIT1.AscensionMon(), new ConditionAttackMon("Poison Thread", SpecialCondition.Poisoned)));
            r.RegisterByKey("PL-45", new FlipBonusMon("Firebreathing", 20));
            r.RegisterByKey("PL-46", new ComboMon(new FlipBonusMon("Rock Smash", 20), new CardsMAB.KnockOverMon()));
            r.RegisterByKey("PL-48", new Fx()
                .After("Spark", (c, a) => HitBenched(c, 2, 10, "Spark"))
                .After("Tail Code", (c, a) => TailCode(c)));
            r.RegisterByKey("PL-49", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("PL-50", new Fx()
                .After("Black Cry", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new NoPowersEffect(d.Name + " can't retreat or use Poké-Powers (Black Cry)", c.Player, d.Uid, c.Turn + 1, true));
                })
                .Damage("Dark Slash", (c, a) =>
                {
                    bool has = c.Pokemon.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Darkness));
                    bool more = has && c.AskYesNo("Dark Slash: discard a Darkness Energy for 20 more damage?") && DiscardTypeFromSelf(c, EnergyType.Darkness, 1) > 0;
                    return a.Damage + (more ? 20 : 0);
                }));
            r.RegisterByKey("PL-51", new SwitchSelfMon("Teleportation Burst", true));
            r.RegisterByKey("PL-52", new Fx().After("Interrupt", (c, a) => { if (c.Flip()) PickFromHandToDeck(c, "Interrupt"); }));
            r.RegisterByKey("PL-53", new Fx()
                .After("Aura Sphere", (c, a) => HitBenched(c, 1, 10, "Aura Sphere"))
                .Damage("Do the Wave", (c, a) => a.Damage + 10 * c.MyBench().Count));
            r.RegisterByKey("PL-54", new MightyenaColdFeet());
            r.RegisterByKey("PL-55", new ComboMon(new CardsTEU1.AmnesiaMon("Upper Hand"), new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("PL-56", new Fx()
                .After("Fire Tail Slap", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); })
                .With(new ParalyzeFlipMon("Paralyzing Gaze")));
            r.RegisterByKey("PL-57", new MukSludgeCell());
            r.RegisterByKey("PL-58", new Fx()
                .Effect("Water Vein", (c, a) =>
                {
                    var top = c.Me.Deck.Take(5).ToList();
                    c.Engine.ShowCards(c.Opponent, top, "Water Vein", "Water Vein: the top cards of your opponent's deck", c.Source);
                    c.Log("Water Vein revealed: " + string.Join(", ", top.Select(x => x.Def.Name)) + ".");
                    int n = top.Count(x => x.Def.IsEnergy);
                    int heads = n == 0 ? 0 : c.FlipCoins(n);
                    c.ShuffleMyDeck();
                    c.DamageDefending(50 * heads);
                })
                .After("Octazooka", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Octazooka)", c.Player, d.Uid, int.MaxValue));
                }));
            r.RegisterByKey("PL-59", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("PL-60", new Fx()
                .Effect("Stealth Rock", (c, a) => HitAny(c, 30, "Stealth Rock"))
                .With(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("PL-61", new Fx()
                .With(new CardsTWM1.HealSelfMon("Shed Skin", 40))
                .With(new SwitchSelfMon("Bite and Escape", true))
                .After("Paralyze Poison", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                    if (c.Flip()) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                }));
            r.RegisterByKey("PL-62", new Fx()
                .Effect("Endure", (c, a) =>
                {
                    if (c.Flip() && c.InPlay(c.Pokemon)) c.AddEffect(new EndureEffect(c.Pokemon.Name + " can't be Knocked Out by damage from attacks (Endure)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                })
                .After("Rock Slide", (c, a) => HitBenched(c, 2, 10, "Rock Slide")));
            r.RegisterByKey("PL-63", new ComboMon(new CardsSIT1.AscensionMon(), new ParalyzeFlipMon("Sticky String")));
            r.RegisterByKey("PL-64", new Fx()
                .With(new CardsXYP1.FlipTailsRecoilMon("Reckless Charge", 10))
                .Damage("Fight Back", (c, a) => a.Damage + (c.Pokemon.DamageCounters > 0 ? 20 : 0)));
            r.RegisterByKey("PL-65", new Fx()
                .With(new CoinsMon("Double Slap", 2))
                .After("Rocket Tackle", (c, a) =>
                {
                    if (!c.InPlay(c.Pokemon)) return;
                    c.DamageSelf(10);
                    if (c.InPlay(c.Pokemon) && c.Flip()) c.AddEffect(new NoDamageEffect(c.Pokemon.Name + " is protected from attack damage (Rocket Tackle)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("PL-66", new CardsRG.ThickSkinMon(new Fx()
                .Effect("Invite and Strike", (c, a) =>
                {
                    if (c.OpponentBench().Count > 0) c.GustOpponent("Invite and Strike: switch in which of your opponent's Benched Pokémon?");
                    c.DamageDefending(20);
                })
                .After("Chop Up", (c, a) => HitEachBenched(c, 10, m => m.DamageCounters > 0))));
            r.RegisterByKey("PL-67", new Fx()
                .Damage("Sneaky Attack", (c, a) => a.Damage + (c.Pokemon.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Darkness)) ? 10 : 0))
                .Effect("Shoot Needle", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    var pool = c.OpponentPokemon();
                    int k = Math.Min(heads, pool.Count);
                    if (k == 0) return;
                    foreach (var t in c.ChoosePokemonMany(pool, k, k, "Shoot Needle: choose " + k + " of your opponent's Pokémon (10 damage each)")) c.DamageTarget(t, 10);
                }));
            r.RegisterByKey("PL-68", new Fx()
                .With(new CardsPAR2.FlipConditionMon("Poison Breath", SpecialCondition.Poisoned))
                .After("Sweet Saliva", (c, a) => HealEach(c, 10, true)));
            r.RegisterByKey("PL-69", new Fx()
                .Effect("Healing Trial", (c, a) =>
                {
                    if (c.Flip()) { if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 30); }
                    else if (c.Defending != null && c.Defending.DamageCounters > 0) c.Heal(c.Defending, 30);
                })
                .Damage("Pulled Punch", (c, a) => c.Defending != null && c.Defending.DamageCounters > 0 ? 10 : a.Damage));
            r.RegisterByKey("PL-71", new Fx()
                .With(new DrawAttackMon("Collect", 1))
                .After("Shoot Through", (c, a) => HitBenched(c, 1, 10, "Shoot Through")));
            r.RegisterByKey("PL-72", new Fx()
                .Effect("Dig Under", (c, a) =>
                {
                    var t = c.OpponentBench().Count == 0 ? null : c.ChoosePokemon(c.OpponentBench(), "Dig Under: choose 1 of your opponent's Benched Pokémon");
                    if (t != null) c.DamageWith(t, 10, false, false, false);
                })
                .With(new FlipBonusMon("Trip Over", 20)));
            r.RegisterByKey("PL-73", new Fx()
                .With(new BenchSearchAttackMon("Call for Family", 1))
                .Effect("Spring Out", (c, a) => HitBenched(c, 1, 10, "Spring Out")));
            r.RegisterByKey("PL-74", new ComboMon(new CardsDRI1.WhirlwindMon("Roar"), new ParalyzeFlipMon("Thunder Fang")));
            r.RegisterByKey("PL-75", new ComboMon(new CardsROS1.BenchFamilyMon("Division", "Grimer", 1), new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned)));
            r.RegisterByKey("PL-76", new CardsSS.BabyEvolutionMon("Chansey", new Fx()
                .Effect("Hospitality", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var pulled = c.GustOpponent("Hospitality: switch in which of your opponent's Benched Pokémon?");
                    if (pulled != null && c.InPlay(pulled) && pulled.DamageCounters > 0) c.Heal(pulled, 20);
                })));
            r.RegisterByKey("PL-77", new Fx()
                .Effect("Honcho's Command", (c, a) => c.SearchToHand(d => d.IsStadium || Invention(d), 2, "Honcho's Command: search your deck for up to 2 Stadium or Team Galactic's Invention cards"))
                .Effect("Target Attack", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Target Attack: choose 1 of your opponent's Pokémon");
                    if (t != null) c.DamageTarget(t, 20 + (t.DamageCounters > 0 ? 20 : 0));
                }));
            r.RegisterByKey("PL-78", new ConditionAttackMon("Lullaby", SpecialCondition.Asleep));
            r.RegisterByKey("PL-79", new Fx()
                .Damage("Grind", (c, a) => 10 * c.EnergyCount(c.Pokemon))
                .With(new ParalyzeFlipMon("Ice Beam")));
            r.RegisterByKey("PL-80", new Fx()
                .Effect("Stretch Tongue", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Stretch Tongue: choose 1 of your opponent's Pokémon");
                    if (t != null) c.DamageWith(t, 10, false, false, false);
                })
                .With(new CardsHPB.KnockOffMon("Knock Off", false)));
            r.RegisterByKey("PL-81", new Fx()
                .With(new CardsRS.ExtraEnergyMon("Energy Ball", 2))
                .Effect("Synthesis", (c, a) => { if (c.Flip()) SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Grass), "Synthesis"); }));
            r.RegisterByKey("PL-82", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 1), new ShieldAttackMon("Expand", 10)));
            r.RegisterByKey("PL-83", new Fx().Effect("Take Back", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsTrainer).ToList();
                    if (pile.Count == 0 || !c.Flip()) return;
                    c.DiscardToHand(c.ChooseCards(pile, 1, 1, "Take Back: put which Trainer card into your hand?"));
                }));
            r.RegisterByKey("PL-84", new CardsDAA1.GustAttackMon("Pull", false));
            r.RegisterByKey("PL-85", new Fx().After("Muddy Water", (c, a) => HitBenched(c, 1, 10, "Muddy Water")));
            r.RegisterByKey("PL-86", new ComboMon(new CardsROS1.BenchFamilyMon("Howl", "Poochyena", 1), new TailsNothingMon("Lunge")));
            r.RegisterByKey("PL-87", new Fx()
                .Effect("Headache", (c, a) =>
                {
                    if (c.Flip()) c.AddEffect(new CardsDPEra.DpTrainerLockEffect(c.Engine.PlayerName(c.Opponent) + " can't play Trainer, Supporter or Stadium cards during their next turn (Headache)", c.Player, c.Opponent, c.Turn + 1));
                })
                .On("Latent Power", (c, a) =>
                {
                    if (c.Flip()) { c.DamageDefending(a.Damage); return true; }
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                    return true;
                }));
            r.RegisterByKey("PL-88", new Fx()
                .After("Chip Off", (c, a) =>
                {
                    while (c.Opp.Hand.Count > 5)
                    {
                        var card = RandomFromHand(c, c.Opp);
                        c.Engine.DiscardFromHand(c.Opponent, card);
                    }
                })
                .On("Poor Sleep", (c, a) =>
                {
                    bool heads = c.Flip();
                    c.DamageDefending(a.Damage + (heads ? 30 : 0));
                    if (heads && c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                    return true;
                }));
            r.RegisterByKey("PL-89", new ComboMon(new CardsSW.FutureSightMon(), new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("PL-90", new CoinsMon("Trickle", 2));
            r.RegisterByKey("PL-91", new FlipBonusMon("Steady Punch", 10));
            r.RegisterByKey("PL-92", new Fx()
                .With(new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep))
                .On("Fade Out", (c, a) =>
                {
                    if (c.MyBench().Count == 0) return DidNothing(c);
                    c.DamageDefending(a.Damage);
                    if (c.InPlay(c.Pokemon)) c.Engine.ReturnPokemonToHand(c.Pokemon);
                    return true;
                }));
            r.RegisterByKey("PL-93", new Fx()
                .Effect("Heal Bell", (c, a) => HealEach(c, 10))
                .With(new RecoilMon("Take Down", 10)));
            r.RegisterByKey("PL-94", new PowerFx("Poison Structure", c => c.State.Stadium != null && c.State.StadiumOwner == c.Player, c =>
                {
                    foreach (var m in new[] { c.Me.Active, c.Opp.Active })
                        if (m != null && !SP(m) && (m.Owner == c.Player || c.CanAffect(m))) c.ApplyCondition(m, SpecialCondition.Poisoned);
                }, new Fx()
                .After("Smokescreen", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Smokescreen)", c.Player, d.Uid, c.Turn + 1));
                })));
            r.RegisterByKey("PL-95", new Fx()
                .With(new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters))
                .Effect("Slack Off", (c, a) =>
                {
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, c.Pokemon.DamageCounters * 10);
                    c.SelfCantAttackNextTurn();
                }));
            r.RegisterByKey("PL-97", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new SwitchSelfMon("Run Around", true)));
            r.RegisterByKey("PL-98", new ComboMon(new SearchAttackMon("Show Off", d => d.IsBasicEnergy, 1), new CardsXYP1.FlipTailsRecoilMon("Take Down", 10)));
            r.RegisterByKey("PL-99", new Fx()
                .Effect("Focus Energy", (c, a) => c.Pokemon.Markers["Focus Energy"] = c.Turn + 2)
                .On("Fire Shard", (c, a) =>
                {
                    int t;
                    int dmg = c.Pokemon.Markers.TryGetValue("Focus Energy", out t) && t == c.Turn ? 80 : a.Damage;
                    if (!c.Flip()) return DidNothing(c);
                    c.DamageDefending(dmg);
                    return true;
                }));
            r.RegisterByKey("PL-100", new Fx()
                .With(new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned))
                .After("Flame Ball", (c, a) => EnergyToBench(c, "Flame Ball")));
            r.RegisterByKey("PL-101", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("PL-102", new Fx()
                .Effect("Reheat", (c, a) =>
                {
                    var energy = c.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                    if (energy.Count == 0) return;
                    var picks = c.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Reheat: discard up to 2 Energy cards from your hand (draw 2 for each)");
                    foreach (var x in picks) c.Engine.DiscardFromHand(c.Player, x);
                    if (picks.Count > 0) c.Draw(2 * picks.Count);
                })
                .With(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("PL-103", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("PL-SH4", new LotadSwiftSwim());
            r.RegisterByKey("PL-SH5", new Fx()
                .Effect("Roost", (c, a) =>
                {
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 40);
                    c.AddEffect(new SelfNoRetreatEffect(c.Pokemon.Name + " can't retreat during your next turn (Roost)", c.Player, c.Pokemon.Uid, c.Turn + 2));
                })
                .Effect("Mirror Move", (c, a) =>
                {
                    var me = c.Pokemon;
                    if (me.AttackDamageTurn != c.OpponentsLastTurn || me.AttackDamageThatTurn <= 0) { DidNothing(c); return; }
                    c.DamageDefending(me.AttackDamageThatTurn);
                })
                .With(new CoinsMon("Fury Attack", 3)));
            r.RegisterByKey("PL-SH6", new Fx()
                .Effect("Find Wildfire", (c, a) => c.SearchToHand(d => CardsRS.BasicOf(d, EnergyType.Fire), 2, "Find Wildfire: search your deck for up to 2 Fire Energy cards"))
                .After("Brushfire", (c, a) => HitEachBenched(c, 10, m => c.Engine.IsType(m, EnergyType.Grass)))
                .On("Inflame", (c, a) =>
                {
                    var fire = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)).ToList();
                    if (fire.Count == 0) return DidNothing(c);
                    c.Engine.DiscardFromHand(c.Player, c.ChooseCards(fire, 1, 1, "Inflame: discard a Fire Energy card from your hand")[0]);
                    c.DamageDefending(a.Damage);
                    return true;
                }));
        }

        /// <summary>Tail Code: move an Energy card attached to the Defending Pokémon to another of the opponent's Pokémon.</summary>
        internal static void TailCode(EffectContext c)
        {
            var d = c.Defending;
            if (d == null || !c.InPlay(d) || d.Energy.Count == 0 || c.Opp.Bench.Count == 0 || !c.CanAffect(d)) return;
            var e = d.Energy.Count == 1 ? d.Energy[0] : c.ChooseCards(d.Energy.ToList(), 1, 1, "Tail Code: move which Energy card?")[0];
            var to = c.ChoosePokemon(c.OpponentBench(), "Tail Code: move it to which of your opponent's Pokémon?");
            if (to != null) c.Engine.MoveEnergy(d, to, e);
        }

        /// <summary>Look at the opponent's hand, choose 1 card, and they shuffle it into their deck.</summary>
        internal static void PickFromHandToDeck(EffectContext c, string attack)
        {
            if (c.Opp.Hand.Count == 0) return;
            var card = c.ChooseCards(c.Opp.Hand.ToList(), 1, 1, attack + ": choose 1 card from your opponent's hand (they shuffle it into their deck)")[0];
            c.Opp.Hand.Remove(card);
            c.Opp.Deck.Add(card);
            c.Engine.ShuffleDeck(c.Opponent);
            c.Log(c.Engine.PlayerName(c.Opponent) + " shuffled " + card.Def.Name + " into their deck (" + attack + ").");
        }

        /// <summary>Metal Trait (Poké-Body): with a Pokémon Tool attached, remove 1 damage counter between turns. Iron Defense. Iron Tackle.</summary>
        private sealed class BastiodonMetalTrait : BodyMon
        {
            public BastiodonMetalTrait() : base(new ComboMon(new DodgeMon("Iron Defense", false), new RecoilMon("Iron Tackle", 30))) { }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || me.Tools.Count == 0 || me.DamageCounters == 0 || !Works(src, "Metal Trait")) return;
                src.Heal(me, 10);
            }
        }

        /// <summary>Sinkhole (Poké-Body): when the opponent's Active Pokémon retreats, 2 damage counters on it. Push Down. Magnitude.</summary>
        private sealed class DugtrioSinkhole : BodyMon
        {
            public DugtrioSinkhole() : base(new Fx()
                .With(new CardsDRI1.WhirlwindMon("Push Down"))
                .After("Magnitude", (c, a) =>
                {
                    foreach (var m in c.MyBench().Concat(c.OpponentBench()).ToList()) if (c.InPlay(m)) c.DamageTarget(m, 10);
                })) { }

            public override void OnPokemonRetreated(EffectContext src, PokemonInPlay retreated)
            {
                if (retreated == null || retreated.Owner == src.Player || !src.InPlay(retreated) || !Works(src, "Sinkhole")) return;
                src.Engine.PlaceDamageCounters(retreated, 2, "Sinkhole");
            }
        }

        private static int ConditionCount(PokemonInPlay m)
        {
            int n = 0;
            foreach (SpecialCondition s in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                if (m.Has(s)) n++;
            return n;
        }

        /// <summary>Camouflage Pattern (Poké-Body): prevent all effects of attacks by the opponent's Pokémon with 2 or more Special Conditions. Smogscreen. Chemical Scale.</summary>
        private sealed class DustoxCamouflagePattern : BodyMon
        {
            public DustoxCamouflagePattern() : base(new Fx()
                .After("Smogscreen", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                    c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Smogscreen)", c.Player, d.Uid, c.Turn + 1));
                })
                .After("Chemical Scale", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !HasPowerOrBody(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Burned);
                    c.ApplyCondition(d, SpecialCondition.Confused);
                })) { }

            private static bool Shielded(EffectContext src, PokemonInPlay attacker)
            {
                return attacker != null && attacker.Owner != src.Player && ConditionCount(attacker) >= 2 && Works(src, "Camouflage Pattern");
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Shielded(src, info.Attacker);
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack && Shielded(src, src.Opp.Active);
            }
        }

        /// <summary>Anticipation (Poké-Body): prevent all effects of attacks, except damage, done to Toxicroak G. Deep Poison.</summary>
        private sealed class ToxicroakAnticipation : BodyMon
        {
            public ToxicroakAnticipation() : base(new Fx()
                .Damage("Deep Poison", (c, a) => a.Damage + (c.Defending != null && c.Defending.Has(SpecialCondition.Poisoned) ? 40 : 0))) { }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack && Works(src, "Anticipation");
            }
        }

        /// <summary>Cold Feet (Poké-Body): with a Special Condition, its attacks cost no Energy. Collude. Desperate Attack.</summary>
        private sealed class MightyenaColdFeet : BodyMon
        {
            public MightyenaColdFeet() : base(new Fx()
                .Damage("Collude", (c, a) => a.Damage + (c.Me.SupporterPlayedThisTurn ? 20 : 0))
                .Damage("Desperate Attack", (c, a) => a.Damage + (c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending) ? 30 : 0))) { }

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (Affected(ctx.Pokemon) && Works(ctx, "Cold Feet")) cost.Clear();
            }
        }

        /// <summary>Sludge Cell (Poké-Body): still affected by a Special Condition between turns, remove 2 damage counters. Strange Poison. Strange Sludge.</summary>
        private sealed class MukSludgeCell : BodyMon
        {
            public MukSludgeCell() : base(new Fx()
                .After("Strange Poison", (c, a) =>
                {
                    if (c.Flip()) { var d = c.Defending; if (d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned); }
                    else if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Poisoned);
                })
                .On("Strange Sludge", (c, a) =>
                {
                    bool poisoned = c.Pokemon.Has(SpecialCondition.Poisoned);
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (poisoned ? 20 : 0));
                    if (poisoned && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Confused);
                    return true;
                })) { }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !Affected(me) || me.DamageCounters == 0 || !Works(src, "Sludge Cell")) return;
                src.Heal(me, 20);
            }
        }

        /// <summary>Swift Swim (Poké-Body): with any Water Energy attached, no Retreat Cost. Blot. Jump On.</summary>
        private sealed class LotadSwiftSwim : BodyMon
        {
            public LotadSwiftSwim() : base(new ComboMon(new CardsTWM1.HealSelfMon("Blot", 10), new FlipBonusMon("Jump On", 20))) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon != src.Pokemon || !mon.Energy.Any(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Water)) || !Works(src, "Swift Swim")) return cost;
                return 0;
            }
        }
    }
}
