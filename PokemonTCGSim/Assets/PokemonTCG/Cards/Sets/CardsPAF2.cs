using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Paldean Fates (PAF) 92-245: the shiny Pokémon and the ex Special Illustration Rares that weren't already in the pool
    /// (all G-mark, Expanded only). Pokémon whose attacks only do their printed damage need no script: Oddish, Snover,
    /// Luxio, Pawmi, Abra, Sneasel, Pawniard, Pidgeotto, Wingull.
    /// </summary>
    public static class CardsPAF2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PAF-93", new BloomingEnergyMon("Semi-Blooming Energy", 3));
            r.RegisterByKey("PAF-94", new BloomingEnergyMon("Fully Blooming Energy", 8));
            r.RegisterByKey("PAF-95", new DodgeMon("Agility", false));
            r.RegisterByKey("PAF-96", new DodgeMon("Splashing Dodge", false));
            r.RegisterByKey("PAF-97", new DriftingDodgeMon(new Cards30CC.BenchHitMon("Flowery Zephyr", 30)));
            r.RegisterByKey("PAF-98", new DriftingDodgeMon(new Cards30CC.BenchHitMon("Fluffy Breeze", 30)));
            r.RegisterByKey("PAF-101", new AbomasnowPAF());
            r.RegisterByKey("PAF-103", new CardsMEG.SandAttackMon("Apply Oil"));
            r.RegisterByKey("PAF-104", new ArbolivaPAF());
            r.RegisterByKey("PAF-106", new CardsMEG.ChargeSelfMon("Increasing Spice", EnergyType.Fire, 1));
            r.RegisterByKey("PAF-107", new ScovillainPAF());
            r.RegisterByKey("PAF-108", new CardsPFL.FlipUntilTailsTimesMon("Ball Roll"));
            r.RegisterByKey("PAF-111", new ComboMon(new Cards30C.SelfCounterBonusMon("Raging Horns", 10), new CardsTWM1.SelfDiscardMon("Blaze Dash", 1)));
            r.RegisterByKey("PAF-112", new EnteiPAF());
            r.RegisterByKey("PAF-113", new OricorioPAF());
            r.RegisterByKey("PAF-116", new SlowpokePAF());
            r.RegisterByKey("PAF-117", new SlowbroPAF());
            r.RegisterByKey("PAF-118", new CardsCRZ1.PiercingMon("Swift"));
            r.RegisterByKey("PAF-119", new StarmiePAF());
            r.RegisterByKey("PAF-120", new ComboMon(new Cards30C.SelfCounterBonusMon("Raging Horns", 10), new CardsMEG.SnipeMon("Aqua Dive", 60)));
            r.RegisterByKey("PAF-121", new DodgeMon("Twisting Strike", false));
            r.RegisterByKey("PAF-122", new WugtrioPAF());
            r.RegisterByKey("PAF-123", new FinizenPAF());
            r.RegisterByKey("PAF-125", new VeluzaPAF());
            r.RegisterByKey("PAF-126", new DondozoPAF());
            r.RegisterByKey("PAF-127", new TatsugiriPAF());
            r.RegisterByKey("PAF-133", new FlipBonusMon("Tumbling Attack", 20));
            r.RegisterByKey("PAF-134", new ElectrodePAF());
            r.RegisterByKey("PAF-135", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("PAF-137", new LuxrayPAF());
            r.RegisterByKey("PAF-138", new PachirisuPAF());
            r.RegisterByKey("PAF-139", new ThundurusPAF());
            r.RegisterByKey("PAF-141", new ToxtricityPAF());
            r.RegisterByKey("PAF-143", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("PAF-145", new CardsPAF1.UnitedWingsMon());
            r.RegisterByKey("PAF-147", new WigglytuffPAF());
            r.RegisterByKey("PAF-149", new SwitchSelfMon("Teleportation Attack", false));
            r.RegisterByKey("PAF-155", new CardsBLK.FlailMon("Balloon Blast"));
            r.RegisterByKey("PAF-156", new DrifblimPAF());
            r.RegisterByKey("PAF-158", new SpiritombPAF());
            r.RegisterByKey("PAF-159", new KlefkiPAF());
            r.RegisterByKey("PAF-163", new RabscaPAF());
            r.RegisterByKey("PAF-165", new TinkatinkPAF());
            r.RegisterByKey("PAF-166", new ComboMon(new FlipBonusMon("Play Rough", 30), new IgnoreDefenderEffectsMon("Pulverizing Press")));
            r.RegisterByKey("PAF-172", new ComboMon(new Cards30C.SelfCounterBonusMon("Raging Horns", 10), new RecoilMon("Combat Tackle", 30)));
            r.RegisterByKey("PAF-173", new RecoilMon("Reckless Charge", 20));
            r.RegisterByKey("PAF-174", new LucarioPAF());
            r.RegisterByKey("PAF-176", new CardsPOR.SweetScentMon("Salt Coating", 20));
            r.RegisterByKey("PAF-177", new CoinsMon("Salt Cannon", 3));
            r.RegisterByKey("PAF-178", new GarganaclPAF());
            r.RegisterByKey("PAF-179", new GlimmetPAF());
            r.RegisterByKey("PAF-183", new WeavilePAF());
            r.RegisterByKey("PAF-184", new SableyePAF());
            r.RegisterByKey("PAF-186", new RecoilMon("Double-Edged Slash", 30));
            r.RegisterByKey("PAF-187", new KingambitPAF());
            r.RegisterByKey("PAF-189", new CardsPAF1.BasicEnergyFromDiscardMon("Berry Search", 1));
            r.RegisterByKey("PAF-190", new GrafaiaiPAF());
            r.RegisterByKey("PAF-191", new ScizorPAF());
            r.RegisterByKey("PAF-196", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("PAF-198", new ComboMon(new SearchAttackMon("Lead", d => d.IsSupporter, 1), new CoinsMon("Stompy Stomp", 2)));
            r.RegisterByKey("PAF-199", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("PAF-200", new DodrioPAF());
            r.RegisterByKey("PAF-201", new DittoPAF());
            r.RegisterByKey("PAF-205", new SkwovetPAF());
            r.RegisterByKey("PAF-206", new CardsASC.ToolBonusMon("Enhanced Fang", 80));
            r.RegisterByKey("PAF-211", new FlamigoPAF());
            r.RegisterByKey("PAF-218", new GlimmoraExPAF());
            r.RegisterByKey("PAF-221", new PidgeotExPAF());
            r.RegisterByKey("PAF-222", new WigglytuffExPAF());
            r.RegisterByKey("PAF-240", new WoChienExPAF());
            r.RegisterByKey("PAF-241", new ChiYuExPAF());
            r.RegisterByKey("PAF-244", new TingLuExPAF());
        }

        // ================================================================== Shared helpers

        /// <summary>
        /// Semi-Blooming / Fully Blooming Energy: when you evolve into this Pokémon from your hand, you may look at the top N cards
        /// of your deck and attach any number of Basic Energy there to your Pokémon; shuffle the rest back.
        /// </summary>
        public sealed class BloomingEnergyMon : Mon
        {
            private readonly string ability;
            private readonly int count;

            public BloomingEnergyMon(string ability, int count)
            {
                this.ability = ability;
                this.count = count;
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, ability, ctx.Me.Deck.Count > 0,
                        ability + ": look at the top " + count + " cards of your deck and attach Basic Energy there to your Pokémon?")) return;
                var top = ctx.LookAtTop(count);
                var pile = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, pile.Count, (c, m) => true, ability, true);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Drifting Dodge: if any damage is done to this Pokémon by attacks, flip a coin; on heads, prevent that damage.</summary>
        public sealed class DriftingDodgeMon : Mon
        {
            private readonly Mon attacks;

            public DriftingDodgeMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && src.Engine.FlipCoin(src.Player, "Drifting Dodge (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"This attack's damage isn't affected by any effects on your opponent's Active Pokémon" (Pulverizing Press).</summary>
        public sealed class IgnoreDefenderEffectsMon : Mon
        {
            private readonly string attack;

            public IgnoreDefenderEffectsMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Freezing Disaster: no Pokémon (both players') can be healed.</summary>
        private sealed class AbomasnowPAF : Mon
        {
            public override bool BlocksHealing(EffectContext src, PokemonInPlay mon) { return true; }
        }

        /// <summary>Enriching Oil: when you evolve into this Pokémon from your hand, you may heal all damage from 1 of your Pokémon.</summary>
        private sealed class ArbolivaPAF : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Enriching Oil", CardsMEG.AnyHurt(ctx), "Enriching Oil: heal all damage from 1 of your Pokémon?")) return;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
                var target = ctx.ChoosePokemon(hurt, "Enriching Oil: heal all damage from which Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
            }
        }

        /// <summary>Double Type: while in play it is Grass and Fire. Spicy Headbutt: not affected by Resistance.</summary>
        private sealed class ScovillainPAF : Mon
        {
            private static readonly Mon Headbutt = new CardsFLF1.NoResistanceMon("Spicy Headbutt");

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Grass)) types.Add(EnergyType.Grass);
                if (!types.Contains(EnergyType.Fire)) types.Add(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Headbutt.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Pressure: while Active, the opponent's Active Pokémon's attacks do 20 less damage to your Pokémon (before Weakness and Resistance). Blaze Ball: 20 more per Fire Energy.</summary>
        private sealed class EnteiPAF : Mon
        {
            private static readonly Mon Ball = new CardsMEG.EnergyBonusMon("Blaze Ball", EnergyType.Fire, 20);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (info.Target == null || info.Target.Owner != src.Player || !src.State.IsActive(info.Attacker)) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ball.HandleAttack(ctx, atk); }
        }

        /// <summary>Ardent Dancing: once during your turn, heal 20 damage from your Active Evolution Pokémon.</summary>
        private sealed class OricorioPAF : Mon
        {
            protected override string Activated { get { return "Ardent Dancing"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && active.Def.IsEvolution && active.DamageCounters > 0 && !ctx.Engine.HealingBlocked(active);
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Me.Active, 20); }
        }

        // ================================================================== Water

        /// <summary>Rest: this Pokémon is now Asleep; heal 30 from it.</summary>
        private sealed class SlowpokePAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                ctx.Engine.ApplyCondition(me, SpecialCondition.Asleep);
                ctx.Heal(me, 30);
                return true;
            }
        }

        /// <summary>Strange Behavior: as often as you like, move 1 damage counter from 1 of your other Pokémon to this Pokémon. Bubble Drain: heal 30 from it.</summary>
        private sealed class SlowbroPAF : Mon
        {
            private static readonly Mon Drain = new CardsTWM1.HealSelfMon("Bubble Drain", 30);

            private static bool CanMove(EffectContext ctx)
            {
                return !ctx.Engine.CounterMovementBlocked() && ctx.MyPokemon(m => m != ctx.Pokemon && m.DamageCounters > 0).Count > 0;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Strange Behavior" && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx) && ctx.InPlay(me), offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != me && m.DamageCounters > 0),
                        "Strange Behavior: move 1 damage counter from which Pokémon to " + me.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    return ctx.Engine.MoveDamageCounters(from, me, 1, "Strange Behavior") > 0;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drain.HandleAttack(ctx, atk); }
        }

        /// <summary>Mysterious Comet: once during your turn, put 2 damage counters on 1 of the opponent's Pokémon; if you did, discard this Pokémon and all attached cards.</summary>
        private sealed class StarmiePAF : Mon
        {
            protected override string Activated { get { return "Mysterious Comet"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Mysterious Comet: put 2 damage counters on which of your opponent's Pokémon?");
                if (target == null) return;
                int before = target.DamageCounters;
                ctx.PlaceCounters(target, 2);
                if (target.DamageCounters > before && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardPokemonFromPlay(ctx.Pokemon);
            }
        }

        /// <summary>Undersea Tunnel: flip 3 coins; for each heads, discard the top 3 cards of the opponent's deck.</summary>
        private sealed class WugtrioPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Undersea Tunnel") return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3 * heads);
                return true;
            }
        }

        /// <summary>Valiant Evolution: switch this Pokémon with 1 of your Benched Pokémon; if you do, evolve it from your deck.</summary>
        private sealed class FinizenPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Valiant Evolution") return false;
                var me = ctx.Pokemon;
                if (!ctx.SwitchSelfWithBench()) return true;
                if (ctx.InPlay(me) && ctx.Me.Deck.Count > 0)
                {
                    CardsSSP1.EvolveFromDeck(ctx, me, "Valiant Evolution: choose a card that evolves from " + me.Name);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Slim Screw: with no cards in your hand, it costs just 1 Water Energy.</summary>
        private sealed class VeluzaPAF : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Slim Screw" || ctx.Me.Hand.Count > 0) return;
                cost.Clear();
                cost.Add(EnergyType.Water);
            }
        }

        /// <summary>Release Rage: 50 for each Tatsugiri in your discard pile.</summary>
        private sealed class DondozoPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Release Rage") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.Discard.Count(c => c.Def.Name == "Tatsugiri"));
                return true;
            }
        }

        /// <summary>Mise en Place: search for up to 2 Basic Water Energy and attach them to 1 of your Basic Pokémon. Curl Up: put this Pokémon and everything attached into your hand.</summary>
        private sealed class TatsugiriPAF : Mon
        {
            private static readonly Mon CurlUp = new CardsPAF1.ReturnSelfMon("Curl Up");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mise en Place") return CurlUp.HandleAttack(ctx, atk);
                if (ctx.Me.Deck.Count == 0) return true;
                var basics = ctx.MyPokemon(m => m.IsBasic);
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 2, "Mise en Place: choose up to 2 Basic Water Energy cards");
                var target = found.Count > 0 && basics.Count > 0 ? ctx.ChoosePokemon(basics, "Mise en Place: attach them to which of your Basic Pokémon?") : null;
                foreach (var c in found)
                {
                    if (target != null) ctx.Engine.AttachEnergyCard(c, target);
                    else ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Bang Boom Chain: before damage, discard any number of Pokémon Tools from your Pokémon; 40 more for each.</summary>
        private sealed class ElectrodePAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bang Boom Chain") return false;
                var holders = ctx.MyPokemon(m => m.Tool != null);
                int discarded = 0;
                if (holders.Count > 0)
                {
                    var tools = holders.SelectMany(m => m.Tools).ToList();
                    var picks = ctx.ChooseCards(tools, 0, tools.Count, "Bang Boom Chain: discard any number of Pokémon Tools from your Pokémon (40 more damage each)");
                    foreach (var tool in picks)
                    {
                        var holder = ctx.Engine.ToolHolder(tool);
                        if (holder != null && ctx.Engine.DiscardTool(holder, tool)) discarded++;
                    }
                }
                ctx.DamageDefending(atk.Damage + 40 * discarded);
                return true;
            }
        }

        /// <summary>Swelling Flash (from your hand): once during your turn, if you have more Prize cards left than the opponent, put this Pokémon onto your Bench. Wild Charge: 20 to itself.</summary>
        private sealed class LuxrayPAF : Mon
        {
            private static readonly Mon Charge = new RecoilMon("Wild Charge", 20);

            private static string Limit(EffectContext ctx) { return "Swelling Flash#" + ctx.Card.Id; }

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && !ctx.NamedLimitUsedThisTurn(Limit(ctx)) && ctx.BenchSpace > 0 && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count;
            }

            public override void UseFromHand(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit(ctx));
                if (!ctx.Me.Hand.Contains(ctx.Card) || ctx.BenchSpace <= 0) return;
                ctx.Me.Hand.Remove(ctx.Card);
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Charge.HandleAttack(ctx, atk); }
        }

        /// <summary>Electricity Pouches: can't be Paralyzed. Everyone Discharge: 20 more per Benched Lightning Pokémon; not affected by Weakness.</summary>
        private sealed class PachirisuPAF : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & SpecialCondition.Paralyzed) != 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Everyone Discharge") return false;
                int n = ctx.Me.Bench.Count(m => ctx.Engine.IsType(m, EnergyType.Lightning));
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage + 20 * n, false, true, false);
                return true;
            }
        }

        /// <summary>Adverse Weather: while Active, prevent all damage done to your Benched Pokémon by the opponent's attacks. Gigantic Bolt: 90 to itself.</summary>
        private sealed class ThundurusPAF : Mon
        {
            private static readonly Mon Bolt = new RecoilMon("Gigantic Bolt", 90);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && info.AgainstOpponent && info.Target.Owner == src.Player && !info.TargetIsActive;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bolt.HandleAttack(ctx, atk); }
        }

        /// <summary>Leer: on heads, Paralyzed. Loud Mix: 30 more for each different type of Pokémon on your Bench.</summary>
        private sealed class ToxtricityPAF : Mon
        {
            private static readonly Mon Leer = new ParalyzeFlipMon("Leer");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Loud Mix") return Leer.HandleAttack(ctx, atk);
                var types = new HashSet<EnergyType>();
                foreach (var m in ctx.Me.Bench)
                    foreach (var t in ctx.Engine.TypesOf(m)) types.Add(t);
                ctx.DamageDefending(atk.Damage + 30 * types.Count);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Balloon Therapy: once during your turn, attach a Therapeutic Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class WigglytuffPAF : Mon
        {
            protected override string Activated { get { return "Balloon Therapy"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(c => c.Def.Name == "Therapeutic Energy"); }

            protected override void Ability(EffectContext ctx)
            {
                CardsSSP2.AttachFromHandAnyWay(ctx, d => d.Name == "Therapeutic Energy", 1, m => true, "Balloon Therapy");
            }
        }

        /// <summary>Curse Spreading: put 8 damage counters on the opponent's Pokémon in any way you like.</summary>
        private sealed class DrifblimPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Curse Spreading") return false;
                ctx.PlaceCountersAnyWay(8, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Fettered in Misfortune: Basic Pokémon V in play have no Abilities. Fade Out: put this Pokémon and everything attached into your hand.</summary>
        private sealed class SpiritombPAF : Mon
        {
            private static readonly Mon Fade = new CardsPAF1.ReturnSelfMon("Fade Out");

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return mon != null && mon.Def.IsBasicPokemon && mon.Def.IsPokemonV;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fade.HandleAttack(ctx, atk); }
        }

        /// <summary>Mischievous Lock: while Active, Basic Pokémon in play have no Abilities except Mischievous Lock. Joust: discard the Defending Pokémon's Tools before damage.</summary>
        private sealed class KlefkiPAF : Mon
        {
            private static readonly Mon Joust = new CardsBLK.PeckOffMon("Joust");

            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && src.PokemonIsActive && mon != null && mon.IsBasic && abilityName != "Mischievous Lock";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Joust.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Revival Blessing: put a Pokémon from your discard pile onto your Bench. Any Pokémon, Evolution Pokémon too (your
        /// ruling, v38); it goes onto the Bench on its own, like Summoning Gate. Psybeam: Confused.
        /// </summary>
        private sealed class RabscaPAF : Mon
        {
            private static readonly Mon Psybeam = new ConditionAttackMon("Psybeam", SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Revival Blessing") return Psybeam.HandleAttack(ctx, atk);
                var mons = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count == 0 || ctx.BenchSpace <= 0) return true;
                var pick = ctx.ChooseCards(mons, 1, 1, "Revival Blessing: put which Pokémon from your discard pile onto your Bench?");
                if (pick.Count == 0 || !ctx.Me.Discard.Remove(pick[0])) return true;
                ctx.Engine.PutOntoBench(ctx.Player, pick[0], false);
                return true;
            }
        }

        /// <summary>Scrap Pickup: put an Item card from your discard pile into your hand.</summary>
        private sealed class TinkatinkPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scrap Pickup") return false;
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(items, 1, 1, "Scrap Pickup: put which Item card into your hand?"));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Avenging Knuckle: 120 more if any of your Fighting Pokémon were Knocked Out by an attack last turn. Accelerating Stab: can't use it next turn.</summary>
        private sealed class LucarioPAF : Mon
        {
            private static readonly Mon Stab = new LockAfterAttackMon("Accelerating Stab", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Avenging Knuckle") return Stab.HandleAttack(ctx, atk);
                bool revenge = ctx.WasKnockedOutLastOpponentTurn(d => d.HasType(EnergyType.Fighting), true);
                ctx.DamageDefending(atk.Damage + (revenge ? 120 : 0));
                return true;
            }
        }

        /// <summary>Blessed Salt: during Pokémon Checkup, heal 20 from each of your Pokémon. Knocking Hammer: discard the top card of the opponent's deck.</summary>
        private sealed class GarganaclPAF : Mon
        {
            private static readonly Mon Hammer = new CardsSSP2.StompOffMon("Knocking Hammer", 1);

            public override void OnPokemonCheckup(EffectContext src)
            {
                foreach (var m in src.MyPokemon(x => x.DamageCounters > 0)) src.Heal(m, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hammer.HandleAttack(ctx, atk); }
        }

        /// <summary>Ascension: evolve this Pokémon with a card from your deck.</summary>
        private sealed class GlimmetPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ascension") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                CardsSSP1.EvolveFromDeck(ctx, ctx.Pokemon, "Ascension: choose a card that evolves from " + ctx.Pokemon.Name);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>
        /// Assaulting Hunt: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may switch in
        /// 1 of the opponent's Benched Basic Pokémon. Not when it's promoted after a Knock Out (Compendium).
        /// </summary>
        private sealed class WeavilePAF : Mon
        {
            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                const string name = "Assaulting Hunt";
                if (!ctx.IsMyTurn || ctx.Engine.Promoting || ctx.State.InCheckup) return;
                if (ctx.Pokemon == null || ctx.AbilityUsedThisTurn(name) || ctx.Engine.AbilityBlocked(ctx.Pokemon, name)) return;
                var targets = ctx.GustTargets().Where(m => m.IsBasic).ToList();
                if (targets.Count == 0) return;
                if (!ctx.AskYesNo(name + ": switch in 1 of your opponent's Benched Basic Pokémon to the Active Spot?")) return;
                ctx.MarkAbilityUsed(name);
                Announce(ctx, name);
                var pick = ctx.ChoosePokemon(targets, name + ": switch in which of your opponent's Benched Basic Pokémon?");
                if (pick != null) ctx.Engine.SwitchActive(ctx.Opponent, pick);
            }
        }

        /// <summary>Night Eyes: Asleep. Unseen Claw: 70 more if the opponent's Active Pokémon has a Special Condition.</summary>
        private sealed class SableyePAF : Mon
        {
            private static readonly Mon NightEyes = new ConditionAttackMon("Night Eyes", SpecialCondition.Asleep);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Unseen Claw") return NightEyes.HandleAttack(ctx, atk);
                bool affected = ctx.Defending != null && ctx.Defending.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage + (affected ? 70 : 0));
                return true;
            }
        }

        /// <summary>Leadership: your Basic Pokémon's attacks do 30 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class KingambitPAF : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Attacker.Owner != src.Player || !info.Attacker.IsBasic) return damage;
                return info.ToOpponentsActive ? damage + 30 : damage;
            }
        }

        /// <summary>Spit Poison: Poisoned. Colorful Graffiti: discard any Basic Energy from your hand; 40 for each type discarded.</summary>
        private sealed class GrafaiaiPAF : Mon
        {
            private static readonly Mon Spit = new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Graffiti") return Spit.HandleAttack(ctx, atk);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(energy, 0, energy.Count, "Colorful Graffiti: discard any number of Basic Energy cards from your hand (40 damage for each type)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                int types = picks.Select(p => p.Def.EnergyProvides).Distinct().Count();
                ctx.DamageDefending(atk.Damage * types);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Punishing Scissors: 50 more for each of the opponent's Pokémon in play that has an Ability.</summary>
        private sealed class ScizorPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Punishing Scissors") return false;
                int n = ctx.Opp.InPlay().Count(m => ctx.Engine.HasAbility(m));
                ctx.DamageDefending(atk.Damage + 50 * n);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Zooming Draw: once during your turn, put 1 damage counter on this Pokémon; if you do, draw a card. Ballistic Beak: 30 more per damage counter on it.</summary>
        private sealed class DodrioPAF : Mon
        {
            private static readonly Mon Beak = new Cards30C.SelfCounterBonusMon("Ballistic Beak", 30);

            protected override string Activated { get { return "Zooming Draw"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Engine.PlaceDamageCounters(ctx.Pokemon, 1, "Zooming Draw") > 0) ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Beak.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Transformative Start: once during your first turn, if this Pokémon is Active, you may search your deck for a Basic
        /// Pokémon (not Ditto); discard this Pokémon and all attached cards and put the chosen Pokémon in its place.
        /// </summary>
        private sealed class DittoPAF : Mon
        {
            protected override string Activated { get { return "Transformative Start"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsMyFirstTurn && ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var found = ctx.SearchMyDeck(d => d.IsBasicPokemon && d.Name != "Ditto", 1, "Transformative Start: choose a Basic Pokémon (not Ditto) to put in Ditto's place", true);
                if (found.Count == 0 || !ctx.InPlay(me) || me.Stack.Count != 1)
                {
                    foreach (var c in found) ctx.Me.Deck.Add(c);
                    ctx.ShuffleMyDeck();
                    return;
                }
                foreach (var e in me.Energy.ToList())
                {
                    me.Energy.Remove(e);
                    ctx.Me.Discard.Add(e);
                }
                ctx.Me.Discard.AddRange(me.Tools);
                me.Tools.Clear();
                var ditto = me.Top;
                me.Stack[0] = found[0];
                ctx.Me.Discard.Add(ditto);
                me.DamageCounters = 0;
                ctx.Engine.RemoveAllConditions(me);
                ctx.Engine.ClearEffectsOn(me);
                me.TurnEnteredPlay = ctx.Turn;
                me.AbilityUsedOnTurn.Clear();
                ctx.Engine.Emit(GameEventType.Evolve, ctx.Player, "Ditto was discarded and " + found[0].Def.Name + " was put in its place (Transformative Start).", found[0].Id, me.Uid);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Nest Stash: once during your turn, shuffle your hand and put it on the bottom of your deck; if you did, draw a card.</summary>
        private sealed class SkwovetPAF : Mon
        {
            protected override string Activated { get { return "Nest Stash"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                ctx.State.Rng.Shuffle(hand);
                ctx.Me.Hand.Clear();
                ctx.Me.Deck.AddRange(hand);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + hand.Count + (hand.Count == 1 ? " card" : " cards") + " on the bottom of their deck (Nest Stash).");
                ctx.Draw(1);
            }
        }

        /// <summary>Insta-Flock: when you play this Pokémon from your hand onto your Bench, you may search your deck for up to 3 Flamigo. United Wings.</summary>
        private sealed class FlamigoPAF : Mon
        {
            private static readonly Mon Wings = new CardsPAF1.UnitedWingsMon();

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Insta-Flock", ctx.Me.Deck.Count > 0, "Insta-Flock: search your deck for up to 3 Flamigo?")) return;
                ctx.SearchToHand(d => d.Name == "Flamigo", 3, "Insta-Flock: choose up to 3 Flamigo");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wings.HandleAttack(ctx, atk); }
        }

        // ================================================================== ex

        /// <summary>Dust Field: while Active, the opponent can't have more than 3 Benched Pokémon. Poisonous Gem: Poisoned.</summary>
        private sealed class GlimmoraExPAF : Mon
        {
            private static readonly Mon Gem = new ConditionAttackMon("Poisonous Gem", SpecialCondition.Poisoned);

            public override int ModifyMaxBench(EffectContext src, int player, int max)
            {
                return src.Pokemon != null && src.PokemonIsActive && player != src.Player ? Math.Min(max, 3) : max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Gem.HandleAttack(ctx, atk); }
        }

        /// <summary>Quick Search: once during your turn (1 Quick Search per turn), search your deck for a card. Blustery Wind: you may discard a Stadium in play.</summary>
        private sealed class PidgeotExPAF : Mon
        {
            protected override string Activated { get { return "Quick Search"; } }
            protected override bool OnePerPlayer { get { return true; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => true, 1, "Quick Search: put any card from your deck into your hand", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blustery Wind") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo("Blustery Wind: discard the Stadium in play (" + ctx.State.Stadium.Def.Name + ")?"))
                    CardsTWM1.DiscardStadiumInPlay(ctx);
                return true;
            }
        }

        /// <summary>Expanding Body: +100 HP with any Special Energy attached. Friend Tackle: 90 more if you played a Supporter from your hand this turn.</summary>
        private sealed class WigglytuffExPAF : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) ? hp + 100 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Friend Tackle") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.SupporterPlayedThisTurn ? 90 : 0));
                return true;
            }
        }

        /// <summary>Covetous Ivy: 60 to 1 of the opponent's Benched Pokémon for each Prize card they have taken.</summary>
        private sealed class WoChienExPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Covetous Ivy") return false;
                int taken = ctx.OpponentPrizesTaken;
                if (taken > 0 && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(60 * taken);
                return true;
            }
        }

        /// <summary>Jealously Singe: discard the top 2 cards of the opponent's deck. Flame Surge: for up to 3 Benched Pokémon, attach a Basic Fire Energy from your deck to each.</summary>
        private sealed class ChiYuExPAF : Mon
        {
            private static readonly Mon Singe = new CardsSSP2.StompOffMon("Jealously Singe", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Surge") return Singe.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var targets = ctx.ChoosePokemonMany(ctx.MyBench(), 0, Math.Min(3, ctx.Me.Bench.Count), "Flame Surge: choose up to 3 of your Benched Pokémon");
                foreach (var t in targets)
                {
                    var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 1, "Flame Surge: choose a Basic Fire Energy for " + t.Name);
                    foreach (var c in found) ctx.Engine.AttachEnergyCard(c, t);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Cursed Land: while Active, the opponent's damaged Pokémon (other than Pokémon ex) have no Abilities. Land Scoop: 2 damage counters on 1 of their Benched Pokémon.</summary>
        private sealed class TingLuExPAF : Mon
        {
            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && src.PokemonIsActive && mon != null && mon.Owner != src.Player && mon.DamageCounters > 0
                       && !mon.Def.IsEx && !mon.Def.IsMegaEx;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Land Scoop") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Land Scoop: put 2 damage counters on which of your opponent's Benched Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
                return true;
            }
        }
    }
}
