using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>Supreme Victors (SV; v173): Uncommons and Commons.</summary>
    public static partial class CardsSV
    {
        private static void RegisterCommons(CardScriptRegistry r)
        {
            r.RegisterByKey("SV-49", new Fx()
                .With(new ConditionAttackMon("Sing", SpecialCondition.Asleep))
                .Effect("Sonic Wing", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); }));
            r.RegisterByKey("SV-50", new Fx().After("Overrun", (c, a) => HitBenched(c, 1, 10, "Overrun")));
            r.RegisterByKey("SV-51", new CoinsMon("Tail Rap", 2));
            r.RegisterByKey("SV-52", new Fx()
                .After("Hover Hit", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d)) c.AddEffect(new RetreatChangeEffect(d.Name + "'s Retreat Cost is 0 (Hover Hit)", c.Player, d.Uid, c.Turn + 2, null));
                })
                .On("Crash Bomber", (c, a) =>
                {
                    var d = c.Defending;
                    bool special = d != null && d.Energy.Any(e => e.Def.IsSpecialEnergy);
                    c.DamageDefending(a.Damage + (special ? 40 : 0));
                    if (special && c.InPlay(d) && c.CanAffect(d)) c.DiscardEnergyFrom(d, 1, "Crash Bomber: discard which Special Energy card?", e => e.Def.IsSpecialEnergy);
                    return true;
                }));
            r.RegisterByKey("SV-53", new Fx().Effect("Swallow", (c, a) =>
                {
                    bool heads = c.Flip();
                    int done = c.DamageDefending(a.Damage);
                    if (heads && done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
                }));
            r.RegisterByKey("SV-54", new BenchTriggerFx("Disrupting Spy", c => c.Opp.Deck.Count > 0, c =>
                {
                    var top = c.Opp.Deck.Take(4).ToList();
                    foreach (var x in top) c.Opp.Deck.Remove(x);
                    var order = new List<CardInstance>();
                    var left = top.ToList();
                    while (left.Count > 1)
                    {
                        var pick = c.ChooseCards(left, 1, 1, "Disrupting Spy: which card goes next from the top of your opponent's deck?")[0];
                        order.Add(pick);
                        left.Remove(pick);
                    }
                    order.AddRange(left);
                    c.Opp.Deck.InsertRange(0, order);
                }, new Fx().Effect("Search and Escape", (c, a) =>
                {
                    c.SearchToHand(CardsDPEra.DpTrainer, 1, "Search and Escape: search your deck for a Trainer card");
                    if (c.InPlay(c.Pokemon)) c.Engine.ShufflePokemonIntoDeck(c.Pokemon);
                })));
            r.RegisterByKey("SV-55", new Fx().On("Petal Dance", (c, a) =>
                {
                    c.DamageDefending(30 * c.FlipCoins(3));
                    if (c.InPlay(c.Pokemon)) c.ApplyCondition(c.Pokemon, SpecialCondition.Confused);
                    return true;
                }));
            r.RegisterByKey("SV-56", new Fx()
                .Damage("Mach Blow", (c, a) => SP(c.Defending) ? 80 : a.Damage)
                .With(new TailsNothingMon("Giant Tail")));
            r.RegisterByKey("SV-58", new FlipBonusMon("Jet Screw", 10));
            r.RegisterByKey("SV-59", new Fx()
                .Effect("Healing Scale", (c, a) => HealEach(c, 10))
                .After("Sand Tomb", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("SV-60", new Fx().After("Earthquake", (c, a) => { foreach (var m in c.MyBench().ToList()) c.DamageTarget(m, 10); }));
            r.RegisterByKey("SV-61", new CoinsMon("Double Headbutt", 2));
            r.RegisterByKey("SV-62", new CardsDS.EvolutionaryMon("Evolutionary Pollen", c =>
                {
                    var t = c.Opp.Active;
                    if (t != null && c.CanAffect(t)) c.ApplyCondition(t, SpecialCondition.Asleep);
                }, c => c.Opp.Active != null, null));
            r.RegisterByKey("SV-63", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("SV-64", new Fx().After("Stomp Off", (c, a) => c.Engine.DiscardTopOfDeck(c.Opponent, 1)));
            r.RegisterByKey("SV-65", new Fx()
                .Effect("Smokescreen", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Smokescreen)", c.Player, d.Uid, c.Turn + 1));
                })
                .After("Fireworks", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); }));
            r.RegisterByKey("SV-66", new Fx()
                .Effect("Energy Absorption", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => x.Def.IsEnergy).ToList();
                    if (pile.Count == 0) return;
                    foreach (var card in c.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Energy Absorption: attach up to 2 Energy cards to " + c.Pokemon.Name))
                        if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                })
                .With(new ParalyzeFlipMon("Thunder Fang")));
            r.RegisterByKey("SV-67", new PowerFx("Plunge", c => c.Me.Bench.Contains(c.Pokemon) && c.Me.Active != null, c =>
                {
                    if (!c.Flip()) return;
                    var active = c.Me.Active;
                    foreach (var e in active.Energy.ToList()) c.Engine.MoveEnergy(active, c.Pokemon, e);
                    c.Engine.SwitchActive(c.Player, c.Pokemon);
                }, null));
            r.RegisterByKey("SV-68", new MasquerainIntimidatingPattern());
            r.RegisterByKey("SV-69", new MetangMetallicLift());
            r.RegisterByKey("SV-70", new Fx()
                .After("Cleansing Ring", (c, a) =>
                {
                    var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                    if (c.Me.Hand.Count < 2 || hurt.Count == 0 || !c.AskYesNo("Cleansing Ring: discard 2 cards from your hand to remove 4 damage counters from 1 of your Pokémon?")) return;
                    foreach (var x in c.ChooseCards(c.Me.Hand.ToList(), 2, 2, "Cleansing Ring: discard 2 cards")) c.Engine.DiscardFromHand(c.Player, x);
                    var t = c.ChoosePokemon(c.MyPokemon(m => m.DamageCounters > 0), "Cleansing Ring: remove 4 damage counters from which Pokémon?");
                    if (t != null) c.Heal(t, 40);
                })
                .Damage("Scale Blow", (c, a) => a.Damage - 10 * c.Me.Hand.Count));
            r.RegisterByKey("SV-71", new Fx()
                .Effect("Call for Family", (c, a) => c.SearchToBench(d => d.IsBasicPokemon && d.HasType(EnergyType.Lightning), 2, "Call for Family: search your deck for up to 2 Lightning Basic Pokémon"))
                .After("Trash Charge", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Lightning)).ToList();
                    if (pile.Count > 0) c.AttachCardsAnyWay(pile, 1, (x, m) => true, "Trash Charge: attach a Lightning Energy card", false);
                }));
            r.RegisterByKey("SV-72", new Fx().Effect("Switcheroo", (c, a) =>
                {
                    var holders = c.OpponentPokemon(m => m.Tools.Count > 0 && c.CanAffect(m));
                    var free = c.OpponentPokemon(m => m.Tools.Count == 0);
                    if (holders.Count == 0 || free.Count == 0) { DidNothing(c); return; }
                    var from = c.ChoosePokemon(holders, "Switcheroo: move a Pokémon Tool from which of your opponent's Pokémon?");
                    if (from == null) return;
                    var tool = from.Tools.Count == 1 ? from.Tools[0] : c.ChooseCards(from.Tools.ToList(), 1, 1, "Switcheroo: move which card?")[0];
                    var to = c.ChoosePokemon(c.OpponentPokemon(m => m.Tools.Count == 0 && m != from), "Switcheroo: move it to which of your opponent's Pokémon?");
                    if (to == null) return;
                    from.Tools.Remove(tool);
                    to.Tools.Add(tool);
                    c.Log(tool.Def.Name + " moved from " + from.Name + " to " + to.Name + " (Switcheroo).");
                }));
            r.RegisterByKey("SV-73", new Fx()
                .Effect("Circling Dive", (c, a) =>
                {
                    foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10);
                    if (c.InPlay(c.Pokemon) && c.MyBench().Count > 0) c.SwitchSelfWithBench();
                })
                .Effect("Parallel Drain", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    var hurt = c.MyPokemon(m => m.DamageCounters > 0);
                    if (done <= 0 || hurt.Count == 0) return;
                    var t = c.ChoosePokemon(hurt, "Parallel Drain: remove damage counters from which of your Pokémon?");
                    if (t != null) c.Heal(t, done);
                }));
            r.RegisterByKey("SV-74", new Fx().Effect("Draw In", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)).ToList();
                    if (pile.Count == 0) return;
                    foreach (var card in c.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Draw In: attach up to 2 Fire Energy cards to " + c.Pokemon.Name))
                        if (c.Me.Discard.Remove(card) && !c.Engine.AttachEnergyCard(card, c.Pokemon, false) && !c.Me.Discard.Contains(card)) c.Me.Discard.Add(card);
                }));
            r.RegisterByKey("SV-75", new Fx()
                .Effect("Gripthrow", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.Flip() && c.CanAffect(d)) c.Engine.ReturnPokemonToHand(d);
                })
                .Damage("Sever", (c, a) => a.Damage + (c.Defending != null && c.Defending.Def.Stage == Stage.Stage2 ? 30 : 0)));
            r.RegisterByKey("SV-76", new Fx()
                .Effect("Greedy Draw", (c, a) =>
                {
                    if (c.Me.Hand.Count > c.Opp.Hand.Count) { DidNothing(c); return; }
                    c.DrawUntil(c.Opp.Hand.Count + 1);
                })
                .After("Hand Charge", (c, a) =>
                {
                    var hand = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Lightning)).ToList();
                    if (hand.Count > 0) c.AttachCardsAnyWay(hand, 1, (x, m) => true, "Hand Charge: attach a Lightning Energy card from your hand", false);
                }));
            r.RegisterByKey("SV-77", new Fx()
                .With(new FlipBonusMon("Quick Attack", 30))
                .After("Gigashock", (c, a) => HitBenched(c, 2, 10, "Gigashock")));
            r.RegisterByKey("SV-78", new Fx()
                .Effect("Find", (c, a) =>
                {
                    var pile = c.Me.Discard.Where(x => CardsDPEra.DpTrainer(x.Def) || x.Def.IsSupporter).ToList();
                    if (pile.Count > 0) c.DiscardToHand(c.ChooseCards(pile, 1, 1, "Find: put which Trainer or Supporter card into your hand?"));
                })
                .With(new FlipBonusMon("Biting Fang", 20)));
            r.RegisterByKey("SV-79", new Fx()
                .Effect("Grand Swell", (c, a) =>
                {
                    int n = c.OpponentPokemon().Sum(m => m.Tools.Count) + (c.State.Stadium != null && c.State.StadiumOwner == c.Opponent ? 1 : 0);
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Grand Swell: choose 1 of your opponent's Pokémon");
                    if (t != null && n > 0) c.DamageTarget(t, 30 * n);
                })
                .With(new CardsTEU1.AmnesiaMon("Amnesia")));
            r.RegisterByKey("SV-80", new Fx()
                .With(new CoinsMon("Fury Attack", 3))
                .Effect("Rock Tumble", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); }));
            r.RegisterByKey("SV-81", new ConditionAttackMon("Poison Seed", SpecialCondition.Poisoned));
            r.RegisterByKey("SV-82", new Fx()
                .Effect("Discharge", (c, a) =>
                {
                    int n = DiscardTypeFromSelf(c, EnergyType.Lightning, 99);
                    c.DamageDefending(40 * (n == 0 ? 0 : c.FlipCoins(n)));
                })
                .Effect("Uproar", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10); }));
            r.RegisterByKey("SV-83", new Fx()
                .After("Silver Feather", (c, a) => c.AddEffect(new BenchTrapEffect(c.Engine.PlayerName(c.Opponent) + "'s Basic Pokémon benched from the hand get 2 damage counters next turn (Silver Feather)", c.Player, c.Opponent, c.Turn + 1, "Silver Feather")))
                .Effect("Metal Max", (c, a) =>
                {
                    int n = DiscardTypeFromSelf(c, EnergyType.Metal, 99);
                    c.DamageDefending(a.Damage + 40 * (n == 0 ? 0 : c.FlipCoins(n)));
                }));
            r.RegisterByKey("SV-84", new Fx()
                .Effect("Sharpshooting", (c, a) => HitAny(c, 10, "Sharpshooting"))
                .After("Lock Up", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("SV-85", new ComboMon(new CoinsMon("Double Peck", 2), new CardsASC.FlipDiscardDefendingEnergyMon("Air Crash")));
            r.RegisterByKey("SV-86", new ComboMon(new DrawAttackMon("Collect", 1), new RecoilMon("Reckless Charge", 10)));
            r.RegisterByKey("SV-87", new ComboMon(new CardsRS.ExtraEnergyMon("Hydro Pump", 2), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("SV-88", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsFLF1.FlipTailsSelfDiscardMon("Air Slash")));
            r.RegisterByKey("SV-89", new CoinsMon("Quick Turn", 2));
            r.RegisterByKey("SV-90", new SearchAttackMon("Pit Search", d => d.IsStadium, 1));
            r.RegisterByKey("SV-91", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("SV-94", new ComboMon(new SwitchSelfMon("Bounce", false), new CardsTWM1.HealSelfMon("Mini Drain", 10)));
            r.RegisterByKey("SV-95", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new CardsXYP1.FlipTailsRecoilMon("Nosedive", 10)));
            r.RegisterByKey("SV-96", new CardsCES1.AttachFromHandToSelfMon("Growth", EnergyType.Grass, 1));
            r.RegisterByKey("SV-97", new FlipBonusMon("Jump On", 20));
            r.RegisterByKey("SV-98", new CardsSS.BabyEvolutionMon("Chimecho", new Fx().Effect("Chime", (c, a) =>
                {
                    var pile = c.Opp.Discard.Where(x => x.Def.IsSupporter).ToList();
                    if (pile.Count == 0) { DidNothing(c); return; }
                    var card = c.ChooseCards(pile, 1, 1, "Chime: use the effect of which Supporter card in your opponent's discard pile?")[0];
                    c.Log(c.Pokemon.Name + " uses the effect of " + card.Def.Name + " (Chime).");
                    var tctx = new EffectContext(c.Engine, c.Player, card, null);
                    var script = c.Engine.Scripts.For(card.Def);
                    if (script.CanPlayTrainer(tctx)) script.ResolveTrainer(tctx);
                })));
            r.RegisterByKey("SV-99", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("SV-100", new FlipBonusMon("Sharp Pincers", 10));
            r.RegisterByKey("SV-102", new RecoilMon("Take Down", 10));
            r.RegisterByKey("SV-103", new ComboMon(new RecoilMon("Reckless Charge", 10), new DrawAttackMon("Collect", 1)));
            r.RegisterByKey("SV-104", new Fx()
                .With(new TailsNothingMon("Surprise Attack"))
                .Effect("Count and Draw", (c, a) =>
                {
                    int n = c.OpponentPokemon(m => !CardsE2.IsEvolved(m)).Count;
                    if (n > 0) c.Draw(n);
                }));
            r.RegisterByKey("SV-105", new TailsNothingMon("Focus Fist"));
            r.RegisterByKey("SV-106", new Fx().Effect("Sand Attack", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Sand Attack)", c.Player, d.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("SV-107", new ComboMon(new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters), new CoinsMon("Fury Attack", 3)));
            r.RegisterByKey("SV-108", new Fx().After("Fireworks", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); }));
            r.RegisterByKey("SV-110", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("SV-111", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("SV-112", new Fx().Damage("Extra Punch", (c, a) => a.Damage + (c.Defending != null && c.Defending.Def.IsLevelUp ? 50 : 0)));
            r.RegisterByKey("SV-113", new Fx().Damage("Karate Chop", (c, a) => a.Damage - 10 * c.Pokemon.DamageCounters));
            r.RegisterByKey("SV-114", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("SV-115", new CardsSS.BabyEvolutionMon("Mr. Mime", new Fx().Effect("Encore", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || d.Def.Attacks.Count == 0 || !c.CanAffect(d)) return;
                    int i = ChooseAttackOfDefending(c, d);
                    if (i >= 0) c.AddEffect(new OnlyAttackEffect(d.Name + " can only use " + d.Def.Attacks[i].Name + " (Encore)", c.Player, d.Uid, c.Turn + 1, d.Def.Attacks[i].Name));
                })));
            r.RegisterByKey("SV-116", new Fx().Damage("Mud Sport", (c, a) => a.Damage + (c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending) ? 10 : 0)));
            r.RegisterByKey("SV-117", new Fx()
                .With(new BenchSearchAttackMon("Call for Family", 1))
                .Effect("Dash Attack", (c, a) => HitBenched(c, 1, 10, "Dash Attack")));
            r.RegisterByKey("SV-118", new FlipBonusMon("Pachi", 10));
            r.RegisterByKey("SV-119", new ComboMon(new ConditionAttackMon("Spore", SpecialCondition.Asleep), new FlipBonusMon("Stomp", 10)));
            r.RegisterByKey("SV-120", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("SV-122", new FlipBonusMon("Stomp", 20));
            r.RegisterByKey("SV-123", new CardsPAR2.FlipConditionMon("Spit Poison", SpecialCondition.Poisoned));
            r.RegisterByKey("SV-125", new CardsPAR2.FlipConditionMon("Icy Wind", SpecialCondition.Asleep));
            r.RegisterByKey("SV-127", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("SV-128", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned), new TailsNothingMon("Tail Smash")));
            r.RegisterByKey("SV-131", new FlipBonusMon("Stomp", 10));
            r.RegisterByKey("SV-132", new CardsPAR2.FlipConditionMon("Tone-Deaf", SpecialCondition.Confused));
            r.RegisterByKey("SV-133", new CoinsMon("Quick Turn", 2));
        }

        /// <summary>Choose 1 of the Defending Pokémon's attacks; returns its index.</summary>
        internal static int ChooseAttackOfDefending(EffectContext c, PokemonInPlay d)
        {
            var names = d.Def.Attacks.Select(x => x.Name).ToArray();
            return names.Length == 1 ? 0 : c.ChooseOption("Choose 1 of " + d.Name + "'s attacks", names);
        }

        /// <summary>Intimidating Pattern (Poké-Body): while it's your Active Pokémon, attack damage by the opponent is 20 less (before W/R). Skim Attack.</summary>
        private sealed class MasquerainIntimidatingPattern : BodyMon
        {
            public MasquerainIntimidatingPattern() : base(new Fx().After("Skim Attack", (c, a) => c.Draw(2))) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Attacker.Owner == src.Player || info.Target == null || info.Target.Owner != src.Player) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !src.PokemonIsActive || !Works(src, "Intimidating Pattern")) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>Metallic Lift (Poké-Body): with any Metal Energy attached, no Retreat Cost. Psychic.</summary>
        private sealed class MetangMetallicLift : BodyMon
        {
            public MetangMetallicLift() : base(new Fx().Damage("Psychic", (c, a) => a.Damage + 10 * (c.Defending == null ? 0 : c.EnergyCount(c.Defending)))) { }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon != src.Pokemon || !mon.Energy.Any(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Metal)) || !Works(src, "Metallic Lift")) return cost;
                return 0;
            }
        }
    }
}
