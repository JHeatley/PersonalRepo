using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Platinum (PL, 2009; v173): the cards not already in the pool (the others are prints). Unlimited only. Rares, LV.X and
    /// Trainers here; Uncommons, Commons and the SH cards in CardsPLB.cs.
    /// </summary>
    public static partial class CardsPL
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PL-1", new AmpharosDamageBind());
            r.RegisterByKey("PL-2", new PowerFx("Dig Well", c => c.Me.Deck.Count > 0, DigWell, new Fx()
                .Damage("Aqua Press", (c, a) => a.Damage + 10 * c.MyPokemon().Concat(c.OpponentPokemon()).Sum(m => c.EnergyCount(m)))
                .Effect("Double Launcher", (c, a) =>
                {
                    c.DiscardEnergyFrom(c.Pokemon, 2, "Double Launcher: discard 2 Energy attached to " + c.Pokemon.Name);
                    HitBenched(c, 2, 60, "Double Launcher");
                    if (c.InPlay(c.Pokemon)) c.SelfCantUseAttackNextTurn("Double Launcher");
                })));
            r.RegisterByKey("PL-3", new PowerFx("Fire Breath", c => c.Opp.Active != null, c =>
                {
                    var t = c.Opp.Active;
                    if (t != null && c.CanAffect(t)) c.ApplyCondition(t, SpecialCondition.Burned);
                }, new Fx()
                .After("Clutch", (c, a) => c.DefendingCantRetreat(c.Defending))
                .With(new CardsTWM1.SelfDiscardMon("Fire Spin", 2))));
            r.RegisterByKey("PL-4", new PowerFx("Power Circulation", c => c.BasicEnergyInDiscard().Count > 0, PowerCirculation, new Fx()
                .On("Power Heal", (c, a) =>
                {
                    c.DamageDefending(a.Damage + 10 * c.Pokemon.DamageCounters);
                    if (c.InPlay(c.Pokemon)) c.Heal(c.Pokemon, 20);
                    return true;
                })));
            r.RegisterByKey("PL-5", new BenchTriggerFx("Reverse Time", c => c.Me.Discard.Any(x => ReverseTimeCard(x.Def)), ReverseTime, new Fx()
                .After("Time-Space Traveling", (c, a) => c.DrawUntil(7))));
            r.RegisterByKey("PL-6", new DialgaTimeAura());
            r.RegisterByKey("PL-7", new Fx()
                .After("Deafen", (c, a) => c.AddEffect(new CardsDPEra.DpTrainerLockEffect(c.Engine.PlayerName(c.Opponent) + " can't play Trainer or Stadium cards during their next turn (Deafen)", c.Player, c.Opponent, c.Turn + 1)))
                .Damage("Second Strike", (c, a) => a.Damage + (c.Defending != null && c.Defending.DamageCounters >= 2 ? 20 : 0)));
            r.RegisterByKey("PL-8", new RepeatPowerFx("Psychic Connect", c => c.Me.Active != null && PsychicOnBench(c).Count > 0, (c, done) =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(c, PsychicOnBench(c), "Psychic Connect: move a Psychic Energy from which Benched Pokémon?" + (done ? " Press Done when finished." : ""), done);
                    if (from == null || c.Me.Active == null) return false;
                    var e = from.Energy.First(x => CardsDRI1.ProvidesType(c, from, x, EnergyType.Psychic));
                    return c.Engine.MoveEnergy(from, c.Me.Active, e);
                }, new Fx()
                .Damage("Energy Burst", (c, a) => a.Damage + 10 * (c.EnergyCount(c.Pokemon) + (c.Defending == null ? 0 : c.EnergyCount(c.Defending))))
                .After("Psychic Protection", (c, a) => { if (c.InPlay(c.Pokemon)) c.AddEffect(new NoWeaknessEffect(c.Pokemon.Name + " has no Weakness (Psychic Protection)", c.Player, c.Pokemon.Uid, c.Turn + 1)); })));
            r.RegisterByKey("PL-9", new BenchTriggerFx("Let Loose", null, LetLoose, new Fx()
                .Effect("Earth Power", (c, a) =>
                {
                    int heads = c.FlipCoins(2);
                    if (heads > 0) HitEachBenched(c, 10 * heads);
                })));
            r.RegisterByKey("PL-10", new Fx()
                .Effect("Over Slash", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10); })
                .After("Dark Wing Flaps", (c, a) =>
                {
                    var card = RandomFromHand(c, c.Opp);
                    if (card == null) return;
                    c.Engine.ShowCards(c.Player, new List<CardInstance> { card }, "Dark Wing Flaps", "Dark Wing Flaps: the card you chose", c.Source);
                    c.Log(c.Engine.PlayerName(c.Opponent) + " shuffles " + card.Def.Name + " into their deck (Dark Wing Flaps).");
                    c.Opp.Hand.Remove(card);
                    c.Opp.Deck.Add(card);
                    c.Engine.ShuffleDeck(c.Opponent);
                }));
            r.RegisterByKey("PL-11", new ManectricElectricBarrier());
            r.RegisterByKey("PL-12", new Fx()
                .With(new SwitchSelfMon("Splashing Turn", true))
                .After("Pearl Breath", (c, a) => HitEachBenched(c, 10)));
            r.RegisterByKey("PL-13", new RampardosIronSkull());
            r.RegisterByKey("PL-14", new Fx()
                .After("Flower Aroma", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.Heal(c.Pokemon, 20);
                    if (c.Defending != null && c.CanAffect(c.Defending)) c.ApplyCondition(c.Defending, SpecialCondition.Asleep);
                })
                .On("Damage Aid", (c, a) =>
                {
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (Affected(d) ? 50 : 0));
                    if (d != null && c.InPlay(d)) c.Engine.RemoveAllConditions(d);
                    return true;
                }));
            r.RegisterByKey("PL-15", new ComboMon(new CardsCES1.AttachFromHandToSelfMon("Growth", EnergyType.Grass, 1), new CardsFLF1.FlipTailsSelfDiscardMon("Air Slash")));
            r.RegisterByKey("PL-16", new SlakingLazyPaunch());
            r.RegisterByKey("PL-17", new Fx()
                .Effect("Call for Family", (c, a) => c.SearchToBench(d => d.IsBasicPokemon && d.IsPokemonSP, 2, "Call for Family: search your deck for up to 2 Basic Pokémon SP"))
                .Damage("Team Attack", (c, a) => a.Damage + 10 * c.MyPokemon(m => m.Def.IsPokemonSP).Count));

            r.RegisterByKey("PL-104", new BrokenTimeSpace());
            r.RegisterByKey("PL-105", new CyrussConspiracy());
            r.RegisterByKey("PL-106", new GalacticHq());
            r.RegisterByKey("PL-107", new LevelMax());
            r.RegisterByKey("PL-109", new LookersInvestigation());
            r.RegisterByKey("PL-111", new MiasmaValley());
            r.RegisterByKey("PL-115", new PokemonRescue());
            r.RegisterByKey("PL-116", new EnergyGain());
            r.RegisterByKey("PL-117", new PowerSpray());
            r.RegisterByKey("PL-118", new PokeTurn());

            r.RegisterByKey("PL-122", new DialgaGTimeCrystal());
            r.RegisterByKey("PL-123", new PowerFx("Tri-Poison", c => c.Opp.Active != null, c =>
                {
                    if (!c.Flip()) return;
                    var t = c.Opp.Active;
                    if (t == null || !c.CanAffect(t)) return;
                    if (c.ApplyCondition(t, SpecialCondition.Poisoned)) t.PoisonCounterBase = Math.Max(t.PoisonCounterBase, 3);
                }, new Fx()
                .Effect("Sniping Tail", (c, a) =>
                {
                    var d = c.Defending;
                    HitBenched(c, 1, 40, "Sniping Tail");
                    if (d != null && c.InPlay(d)) c.DefendingCantRetreat(d);
                })));
            r.RegisterByKey("PL-125", new PowerFx("Lost Cyclone", c => c.Me.Bench.Count >= 4 || c.Opp.Bench.Count >= 4, LostCyclone, new Fx()
                .Effect("Hydro Shot", (c, a) =>
                {
                    c.DiscardEnergyFrom(c.Pokemon, 2, "Hydro Shot: discard 2 Energy attached to " + c.Pokemon.Name);
                    HitAny(c, 80, "Hydro Shot");
                })));
            r.RegisterByKey("PL-126", new ShayminThankfulness());
            RegisterCommons(r);
        }

        // ------------------------------------------------------------------ Rares

        /// <summary>Damage Bind (Poké-Body): each Pokémon with damage counters on it can't use Poké-Powers. Gigavolt. Reflect Energy.</summary>
        private sealed class AmpharosDamageBind : BodyMon
        {
            public AmpharosDamageBind() : base(new Fx()
                .On("Gigavolt", (c, a) =>
                {
                    bool heads = c.Flip();
                    c.DamageDefending(a.Damage + (heads ? 30 : 0));
                    if (!heads && c.Defending != null && c.CanAffect(c.Defending)) c.ApplyCondition(c.Defending, SpecialCondition.Paralyzed);
                    return true;
                })
                .After("Reflect Energy", (c, a) => EnergyToBench(c, "Reflect Energy"))) { }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return mon != null && mon.DamageCounters > 0 && Works(src, "Damage Bind"); }
        }

        /// <summary>Dig Well: look at the top 3 cards, attach any Water Energy cards there to your Pokémon, discard the rest.</summary>
        private static void DigWell(EffectContext ctx)
        {
            var top = ctx.LookAtTop(3);
            var water = top.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Water)).ToList();
            if (water.Count > 0) ctx.AttachCardsAnyWay(water, water.Count, (c, m) => true, "Dig Well: attach Water Energy cards", true);
            foreach (var c in top) if (ctx.Me.Deck.Remove(c)) { ctx.Me.Discard.Add(c); ctx.Log(c.Def.Name + " was discarded (Dig Well)."); }
        }

        /// <summary>Power Circulation: up to 2 basic Energy cards from the discard pile on top of the deck (any order), then 2 damage counters on Delcatty.</summary>
        private static void PowerCirculation(EffectContext ctx)
        {
            var pile = ctx.BasicEnergyInDiscard();
            var picks = ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Power Circulation: put up to 2 basic Energy cards on top of your deck");
            foreach (var c in picks) ctx.Me.Discard.Remove(c);
            CardsCRZ1.PutOnTopInOrder(ctx, picks, "Power Circulation");
            ctx.Engine.PlaceDamageCounters(ctx.Pokemon, 2, "Power Circulation");
        }

        private static bool ReverseTimeCard(CardDef d) { return (d.IsPokemon && !d.IsLevelUp) || d.IsBasicEnergy; }

        /// <summary>Reverse Time: up to 3 Pokémon (not LV.X) and basic Energy cards from the discard pile on top of the deck in any order.</summary>
        private static void ReverseTime(EffectContext ctx)
        {
            var pile = ctx.Me.Discard.Where(c => ReverseTimeCard(c.Def)).ToList();
            var picks = ctx.ChooseCards(pile, 0, Math.Min(3, pile.Count), "Reverse Time: put up to 3 Pokémon and basic Energy cards on top of your deck");
            foreach (var c in picks) ctx.Me.Discard.Remove(c);
            if (picks.Count > 0) CardsCRZ1.PutOnTopInOrder(ctx, picks, "Reverse Time");
        }

        /// <summary>Time Aura (Poké-Body): while Dialga is your Active Pokémon, the opponent can't evolve their Active Pokémon from their hand. Metal Burn.</summary>
        private sealed class DialgaTimeAura : BodyMon
        {
            public DialgaTimeAura() : base(new Fx().After("Metal Burn", (c, a) => DiscardTypeFromSelf(c, EnergyType.Metal, 99))) { }

            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target)
            {
                return target != null && target.Owner != src.Player && src.Opp.Active == target && src.PokemonIsActive && Works(src, "Time Aura");
            }
        }

        private static List<PokemonInPlay> PsychicOnBench(EffectContext ctx)
        {
            return ctx.MyBench().Where(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Psychic))).ToList();
        }

        /// <summary>Let Loose: each player shuffles their hand into their deck and draws up to 4 cards (you first).</summary>
        private static void LetLoose(EffectContext ctx)
        {
            foreach (int p in new[] { ctx.Player, ctx.Opponent })
            {
                var ps = ctx.State.Players[p];
                foreach (var c in ps.Hand.ToList()) { ps.Hand.Remove(c); ps.Deck.Add(c); }
                ctx.Engine.ShuffleDeck(p);
                int n = ctx.Engine.AskNumber(p, 0, Math.Min(4, ps.Deck.Count), "Let Loose: draw how many cards (up to 4)?", ctx.Source);
                ctx.Engine.DrawCards(p, n);
            }
        }

        /// <summary>Electric Barrier (Poké-Body): prevent all damage done to your other Benched Pokémon by attacks. Power Wave. Attract Current.</summary>
        private sealed class ManectricElectricBarrier : BodyMon
        {
            public ManectricElectricBarrier() : base(new Fx()
                .Effect("Power Wave", (c, a) =>
                {
                    foreach (var m in c.MyPokemon().Concat(c.OpponentPokemon()).Where(HasPower).ToList())
                        if (c.InPlay(m)) c.DamageTarget(m, 30);
                })
                .After("Attract Current", (c, a) => SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Lightning), "Attract Current"))) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                return t != null && t.Owner == src.Player && t != src.Pokemon && src.Me.Bench.Contains(t) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Electric Barrier");
            }
        }

        /// <summary>Iron Skull (Poké-Body): its attacks ignore Resistance and effects on the Defending Pokémon. Head Smash. Mold Breaker.</summary>
        private sealed class RampardosIronSkull : BodyMon
        {
            public RampardosIronSkull() : base(new Fx()
                .On("Head Smash", (c, a) =>
                {
                    var d = c.Defending;
                    c.DamageDefending(a.Damage);
                    if (d != null && (!c.InPlay(d) || c.Engine.IsKnockedOut(d)) && c.InPlay(c.Pokemon)) c.DamageSelf(40);
                    return true;
                })
                .After("Mold Breaker", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new TakenChangeEffect(c.Pokemon.Name + " takes 20 less damage until the end of your next turn (Mold Breaker)", c.Player, c.Pokemon.Uid, c.Turn + 2, -20));
                })) { }

            public override bool IgnoresResistance(EffectContext src, DamageInfo info) { return info.Attacker == src.Pokemon && Works(src, "Iron Skull"); }

            public override bool AttacksIgnoreDefenderEffects(EffectContext src, DamageInfo info) { return info.Attacker == src.Pokemon && Works(src, "Iron Skull"); }
        }

        /// <summary>Lazy Paunch (Poké-Body): Slaking can't attack if it attacked during your last turn. Best Dash: it takes 50 more damage next turn.</summary>
        private sealed class SlakingLazyPaunch : BodyMon
        {
            public SlakingLazyPaunch() : base(new Fx()
                .After("Best Dash", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new TakenChangeEffect(c.Pokemon.Name + " takes 50 more damage next turn (Best Dash)", c.Player, c.Pokemon.Uid, c.Turn + 1, 50));
                })) { }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return !(ctx.Pokemon.LastAttackTurn == ctx.MyLastTurn && Works(ctx, "Lazy Paunch"));
            }
        }

        /// <summary>Lost Cyclone: each player with 4 or more Benched Pokémon keeps 3; the others go to the Lost Zone with their cards (you choose first).</summary>
        private static void LostCyclone(EffectContext ctx)
        {
            foreach (int p in new[] { ctx.Player, ctx.Opponent })
            {
                var bench = ctx.State.Players[p].Bench.ToList();
                if (bench.Count < 4) continue;
                var keep = ChooseMonsFor(ctx, p, bench, 3, 3, "Lost Cyclone: choose 3 Benched Pokémon to keep (the others go to the Lost Zone)");
                foreach (var m in bench.Where(m => !keep.Contains(m)).ToList())
                {
                    if (p != ctx.Player && !ctx.CanAffect(m)) continue;
                    CardsLOR1.PokemonToLostZone(ctx, m, true);
                }
            }
        }

        /// <summary>Thankfulness (Poké-Body): your Grass Pokémon other than Shaymin get +40 HP (only 1 Thankfulness). Seed Flare.</summary>
        private sealed class ShayminThankfulness : BodyMon
        {
            public ShayminThankfulness() : base(new Fx()
                .On("Seed Flare", (c, a) =>
                {
                    var grass = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Grass)).ToList();
                    int n = grass.Count == 0 ? 0 : c.AttachCardsAnyWay(grass, grass.Count, (x, m) => true, "Seed Flare: attach Grass Energy cards from your hand", true);
                    c.DamageDefending(a.Damage + 20 * n);
                    return true;
                })) { }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon.Owner != src.Player || mon.Name.StartsWith("Shaymin")) return hp;
                if (!mon.Def.HasType(EnergyType.Grass) || !Works(src, "Thankfulness") || !CardsCRZ1.IsLeadSource(src, "Thankfulness")) return hp;
                return hp + 40;
            }
        }

        /// <summary>Time Crystal (Poké-Body): Pokémon that aren't Pokémon SP can't use Poké-Bodies. Remove Lost.</summary>
        private sealed class DialgaGTimeCrystal : BodyMon
        {
            public DialgaGTimeCrystal() : base(new Fx()
                .On("Remove Lost", (c, a) =>
                {
                    var d = c.Defending;
                    int heads = c.FlipUntilTails();
                    c.DamageDefending(a.Damage);
                    for (int i = 0; i < heads && d != null && c.InPlay(d) && d.Energy.Count > 0 && c.CanAffect(d); i++)
                    {
                        var e = d.Energy.Count == 1 ? d.Energy[0] : c.ChooseCards(d.Energy.ToList(), 1, 1, "Remove Lost: put which Energy card in the Lost Zone?")[0];
                        EnergyToLostZone(c, d, e);
                    }
                    return true;
                })) { }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName)
            {
                return mon != null && !SP(mon) && Works(src, "Time Crystal");
            }
        }

        // ------------------------------------------------------------------ Trainers

        /// <summary>Broken Time-Space (Stadium): each player may evolve a Pokémon they played or evolved that turn.</summary>
        private sealed class BrokenTimeSpace : CardScript
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution) { return true; }
        }

        private static bool Invention(CardDef d) { return d.IsTrainer && d.Name.Contains("Team Galactic's Invention"); }

        /// <summary>Cyrus's Conspiracy (Supporter): search for a Supporter, a basic Energy card and a Team Galactic's Invention card.</summary>
        private sealed class CyrussConspiracy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(d => d.IsSupporter, 1, "Cyrus's Conspiracy: search your deck for a Supporter card"));
                found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Cyrus's Conspiracy: search your deck for a basic Energy card"));
                found.AddRange(ctx.SearchMyDeck(Invention, 1, "Cyrus's Conspiracy: search your deck for a Team Galactic's Invention card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Galactic HQ (Stadium): whenever a player evolves a Pokémon from their hand, put 2 damage counters on it.</summary>
        private sealed class GalacticHq : CardScript
        {
            public override void OnPokemonEvolvedFromHandUnderStadium(EffectContext src, PokemonInPlay evolved, CardDef before)
            {
                if (evolved != null && src.InPlay(evolved)) src.Engine.PlaceDamageCounters(evolved, 2, "Galactic HQ");
            }
        }

        /// <summary>Level Max: flip; heads, search your deck for a Pokémon LV.X that levels up from 1 of your Pokémon and put it on it.</summary>
        private sealed class LevelMax : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var targets = ctx.MyPokemon(m => ctx.Me.Deck.Any(c => c.Def.IsLevelUp && ctx.Engine.CanEvolveInto(m, c.Def, false)));
                if (targets.Count == 0)
                {
                    ctx.SearchMyDeck(d => false, 0, "Level Max: no Pokémon LV.X levels up from your Pokémon");
                    ctx.ShuffleMyDeck();
                    return;
                }
                var target = ctx.ChoosePokemon(targets, "Level Max: level up which Pokémon?");
                EvolveFromDeck(ctx, target, d => d.IsLevelUp, "Level Max: search your deck for a Pokémon LV.X");
            }
        }

        /// <summary>Looker's Investigation (Supporter): look at the opponent's hand; you or they shuffle their hand into the deck and draw up to 5.</summary>
        private sealed class LookersInvestigation : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                LookAtHand(ctx, ctx.Opp);
                int who = ctx.ChooseOption("Looker's Investigation: who shuffles their hand into their deck and draws up to 5 cards?", "You", "Your opponent") == 0 ? ctx.Player : ctx.Opponent;
                var p = ctx.State.Players[who];
                foreach (var c in p.Hand.ToList()) { p.Hand.Remove(c); p.Deck.Add(c); }
                ctx.Engine.ShuffleDeck(who);
                // At least 1 card if they can (PUI Rules Team, 2009-02-19).
                int n = ctx.Engine.AskNumber(who, Math.Min(1, p.Deck.Count), Math.Min(5, p.Deck.Count), "Looker's Investigation: draw how many cards (up to 5)?", ctx.Source);
                ctx.Engine.DrawCards(who, n);
            }
        }

        /// <summary>Miasma Valley (Stadium): a Basic Pokémon (not Grass or Psychic) benched from a player's hand gets 2 damage counters.</summary>
        private sealed class MiasmaValley : CardScript
        {
            public override void OnPokemonBenchedFromHand(EffectContext src, PokemonInPlay benched)
            {
                // Its type right now: Kecleon's Colorful Body makes it Grass and Psychic too (PUI Rules Team, 2009-07-23).
                if (benched == null || !benched.Def.IsBasicPokemon || src.Engine.IsType(benched, EnergyType.Grass) || src.Engine.IsType(benched, EnergyType.Psychic)) return;
                src.Engine.PlaceDamageCounters(benched, 2, "Miasma Valley");
            }
        }

        /// <summary>Pokémon Rescue: a Pokémon from your discard pile into your hand.</summary>
        private sealed class PokemonRescue : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "Pokémon Rescue: put which Pokémon into your hand?"));
            }
        }

        /// <summary>G-101 Energy Gain (Pokémon Tool): attach only to a Pokémon SP; its attacks cost [C] less.</summary>
        private sealed class EnergyGain : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return SP(target); }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon != src.Pokemon || !SP(mon)) return;
                cost.Remove(EnergyType.Colorless);
            }
        }

        /// <summary>
        /// G-103 Power Spray: during the opponent's turn, when their Pokémon uses a Poké-Power, play it to prevent all effects of
        /// that Poké-Power (it still counts as used). Only with 3 or more Pokémon SP in play.
        /// </summary>
        private sealed class PowerSpray : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return false; }

            public override bool CancelsPokePowerFromHand { get { return true; } }

            public override bool CancelPokePowerFromHand(EffectContext ctx, PokemonInPlay user, string power)
            {
                if (user == null || user.Owner == ctx.Player || ctx.MyPokemon(m => m.Def.IsPokemonSP).Count < 3) return false;
                if (!ctx.AskYesNo("Play G-103 Power Spray to stop " + user.Name + "'s " + power + "?")) return false;
                ctx.Me.Hand.Remove(ctx.Card);
                ctx.Me.Discard.Add(ctx.Card);
                ctx.Engine.Emit(GameEventType.PlayTrainer, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " played Team Galactic's Invention G-103 Power Spray: " + user.Name + "'s " + power + " was stopped.", ctx.Card.Id);
                return true;
            }
        }

        /// <summary>G-105 Poké Turn: return 1 of your Pokémon SP and all cards attached to it to your hand.</summary>
        private sealed class PokeTurn : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(SP).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(SP), "Poké Turn: return which Pokémon SP to your hand?");
                if (target != null) ctx.Engine.ReturnPokemonToHand(target);
            }
        }
    }
}
