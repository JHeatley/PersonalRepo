using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Surging Sparks (SSP) 62-124. Pokémon whose attacks only do their printed damage need no script:
    /// Zebstrika, Togepi, Marill, Yamask, Sandygast, Phanpy, Shroodle.
    /// </summary>
    public static class CardsSSP2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SSP-62", new DrawAttackMon("Add On", 1));
            r.RegisterByKey("SSP-64", new StunfiskSSP());
            r.RegisterByKey("SSP-65", new TapuKokoSSP());
            r.RegisterByKey("SSP-66", new FlipBonusMon("Aerial Ace", 20));
            r.RegisterByKey("SSP-67", new KilowattrelSSP());
            r.RegisterByKey("SSP-68", new KilowattrelExSSP());
            r.RegisterByKey("SSP-69", new MiraidonSSP());
            r.RegisterByKey("SSP-71", new CardsTWM1.HealSelfMon("Draining Kiss", 30));
            r.RegisterByKey("SSP-72", new TogekissSSP());
            r.RegisterByKey("SSP-74", new AzumarillSSP());
            r.RegisterByKey("SSP-77", new CardsMEG.SnipeMon("Skill Dive", 50));
            r.RegisterByKey("SSP-78", new UxieSSP());
            r.RegisterByKey("SSP-79", new MespritSSP());
            r.RegisterByKey("SSP-80", new AzelfSSP());
            r.RegisterByKey("SSP-81", new SigilyphSSP());
            r.RegisterByKey("SSP-84", new CardsBLK.ScoutMon("See Through"));
            r.RegisterByKey("SSP-85", new MeowsticSSP());
            r.RegisterByKey("SSP-86", new SylveonExSSP());
            r.RegisterByKey("SSP-88", new XerneasSSP());
            r.RegisterByKey("SSP-89", new CardsSSP1.OricorioSSP("Dazzle Dance"));
            r.RegisterByKey("SSP-91", new PalossandExSSP());
            r.RegisterByKey("SSP-92", new TapuLeleSSP());
            r.RegisterByKey("SSP-93", new IndeedeeSSP());
            r.RegisterByKey("SSP-94", new DodgeMon("Splashing Dodge", false));
            r.RegisterByKey("SSP-95", new EspathraSSP());
            r.RegisterByKey("SSP-96", new FlutterManeSSP());
            r.RegisterByKey("SSP-97", new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("SSP-98", new CoinsMon("Dual Chop", 2));
            r.RegisterByKey("SSP-99", new PrimeapeSSP());
            r.RegisterByKey("SSP-101", new PaldeanTaurosFightingSSP());
            r.RegisterByKey("SSP-103", new DonphanSSP());
            r.RegisterByKey("SSP-104", new CallForFamilyMon());
            r.RegisterByKey("SSP-105", new VibravaSSP());
            r.RegisterByKey("SSP-106", new FlygonExSSP());
            r.RegisterByKey("SSP-107", new GastrodonSSP());
            r.RegisterByKey("SSP-108", new StompOffMon("Burrow", 1));
            r.RegisterByKey("SSP-109", new ExcadrillSSP());
            r.RegisterByKey("SSP-110", new LandorusSSP());
            r.RegisterByKey("SSP-112", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("SSP-113", new GrapploctSSP());
            r.RegisterByKey("SSP-114", new CardsDAA2.RockHurlMon());
            r.RegisterByKey("SSP-115", new GlimmoraSSP());
            r.RegisterByKey("SSP-116", new KoraidonSSP());
            r.RegisterByKey("SSP-117", new StompOffMon("Stomp Off", 1));
            r.RegisterByKey("SSP-118", new StompOffMon("Stomp Off", 2));
            r.RegisterByKey("SSP-119", new HydreigonExSSP());
            r.RegisterByKey("SSP-121", new GrafaiaiSSP());
            r.RegisterByKey("SSP-122", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("SSP-123", new CardsPHF2.LuckyPunchMon("Trio-Cheehoo", 3));
            r.RegisterByKey("SSP-124", new ShieldAttackMon("Steel Wing", 30));
        }

        // ================================================================== Shared helpers (also used by CardsSSP3)

        /// <summary>Attach up to max Energy cards from your hand to your Pokémon in any way you like (each one counts as attached from the hand).</summary>
        public static int AttachFromHandAnyWay(EffectContext ctx, Func<CardDef, bool> cardFilter, int max, Func<PokemonInPlay, bool> targetFilter, string prompt)
        {
            int attached = 0;
            for (int i = 0; i < max; i++)
            {
                var cards = ctx.Me.Hand.Where(c => cardFilter(c.Def)).ToList();
                var targets = ctx.MyPokemon(m => targetFilter(m) && ctx.Engine.CanAttachFromHand(m));
                if (cards.Count == 0 || targets.Count == 0) break;
                var pick = ctx.ChooseCards(cards, 0, 1, prompt + " (" + (i + 1) + " of up to " + max + "; choose none to stop)");
                if (pick.Count == 0) break;
                var card = pick[0];
                var target = ctx.ChoosePokemon(targets, prompt + ": attach " + card.Def.Name + " to which Pokémon?");
                if (target == null) break;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
                attached++;
            }
            return attached;
        }

        /// <summary>"Discard the top N cards of your opponent's deck" after the damage.</summary>
        public sealed class StompOffMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public StompOffMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, count);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Paralyzing Crackle: flip; heads Paralyzed and discard an Energy from it.</summary>
        private sealed class StunfiskSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paralyzing Crackle") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (defending.Energy.Count > 0) ctx.DiscardEnergyFrom(defending, 1, "Paralyzing Crackle: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Summon Lightning: up to 2 Lightning Pokémon from your deck. Prize Count: +90 with more Prize cards left than the opponent.</summary>
        private sealed class TapuKokoSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Summon Lightning")
                {
                    if (ctx.Me.Deck.Count > 0)
                        ctx.SearchToHand(d => d.IsPokemon && d.PokemonType == EnergyType.Lightning, 2, "Summon Lightning: choose up to 2 Lightning Pokémon");
                    return true;
                }
                if (atk.Name == "Prize Count")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count ? 90 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Storm Bolt: move all Energy from this Pokémon to your Benched Pokémon in any way you like.</summary>
        private sealed class KilowattrelSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Storm Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                foreach (var e in me.Energy.ToList())
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Storm Bolt: move " + e.Def.Name + " to which Benched Pokémon?");
                    if (target != null) ctx.Engine.MoveEnergy(me, target, e);
                }
                return true;
            }
        }

        /// <summary>Return Charge: switch this Pokémon to the Bench; if you did, attach up to 2 Basic Lightning Energy from your hand to it. Thunder Lance: +40 per Lightning Energy.</summary>
        private sealed class KilowattrelExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Return Charge")
                {
                    var me = ctx.Pokemon;
                    if (!ctx.SwitchSelfWithBench()) return true;
                    AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 2, m => m == me,
                        "Return Charge: attach a Basic Lightning Energy to " + me.Name);
                    return true;
                }
                if (atk.Name == "Thunder Lance")
                {
                    ctx.DamageDefending(atk.Damage + 40 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Lightning));
                    return true;
                }
                return false;
            }
        }

        /// <summary>C.O.D.E.: Protect: your Future Pokémon take no damage from Pokémon ex's attacks next turn, while Miraidon stays Active.</summary>
        private sealed class CodeProtectEffect : ContinuousEffect
        {
            public CodeProtectEffect(string name, int owner, int miraidonUid, int expiresAfterTurn)
                : base(name, owner, miraidonUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var t = info.Target;
                if (t.Owner != Owner || !info.AgainstOpponent || !t.Def.IsFuture) return damage;
                if (!(info.Attacker.Def.IsEx || info.Attacker.Def.IsMegaEx)) return damage;
                var active = engine.State.Players[Owner].Active;
                return active != null && active.Uid == TargetUid ? 0 : damage;
            }
        }

        private sealed class MiraidonSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "C.O.D.E.: Protect") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive)
                    ctx.AddEffect(new CodeProtectEffect("Your Future Pokémon take no damage from Pokémon ex during the opponent's next turn while " + ctx.Pokemon.Name + " is Active (C.O.D.E.: Protect)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Wonder Kiss: when the opponent's Active Pokémon is Knocked Out, flip; heads take 1 more Prize card (doesn't stack).</summary>
        private sealed class TogekissSSP : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || taker != src.Player || knockedOut.Owner == src.Player || prizes <= 0) return prizes;
                if (!src.State.IsActive(knockedOut)) return prizes;
                var first = src.Me.InPlay().FirstOrDefault(m => m.Name == "Togekiss" && src.Engine.HasAbilityNamed(m, "Wonder Kiss"));
                if (first != src.Pokemon) return prizes;
                return src.Engine.FlipCoin(src.Player, "Wonder Kiss") ? prizes + 1 : prizes;
            }
        }

        /// <summary>Glistening Bubbles: with a Tera Pokémon in play, Double-Edge costs only 1 Psychic. Double-Edge: 50 to itself.</summary>
        private sealed class AzumarillSSP : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || attack.Name != "Double-Edge") return;
                if (!src.Me.InPlay().Any(m => m.Def.IsTera)) return;
                cost.Clear();
                cost.Add(EnergyType.Psychic);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double-Edge") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(50);
                return true;
            }
        }

        /// <summary>Painful Memories: 2 damage counters on each of the opponent's Pokémon.</summary>
        private sealed class UxieSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Painful Memories") return false;
                foreach (var m in ctx.OpponentPokemon()) ctx.PlaceCounters(m, 2);
                return true;
            }
        }

        /// <summary>Full Heart: attach up to 2 Basic Psychic Energy from your hand to your Pokémon. Guardian Burst: does nothing without Uxie and Azelf on your Bench.</summary>
        private sealed class MespritSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Full Heart")
                {
                    AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 2, m => true, "Full Heart: attach a Basic Psychic Energy");
                    return true;
                }
                if (atk.Name == "Guardian Burst")
                {
                    bool trio = ctx.Me.Bench.Any(b => b.Name == "Uxie") && ctx.Me.Bench.Any(b => b.Name == "Azelf");
                    if (!trio) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Neurokinesis: +10 per damage counter on all of the opponent's Pokémon.</summary>
        private sealed class AzelfSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Neurokinesis") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.OpponentPokemon().Sum(m => m.DamageCounters));
                return true;
            }
        }

        private sealed class SigilyphSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Double Draw")
                {
                    ctx.Draw(2);
                    return true;
                }
                if (atk.Name == "Psy Bolt")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Beckoning Tail: discard a Chill Teaser Toy from your hand to switch in 1 of the opponent's Benched Pokémon.</summary>
        private sealed class MeowsticSSP : Mon
        {
            protected override string Activated { get { return "Beckoning Tail"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => c.Def.Name == "Chill Teaser Toy") && ctx.GustTargets().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var toy = ctx.Me.Hand.FirstOrDefault(c => c.Def.Name == "Chill Teaser Toy");
                if (toy == null) return;
                ctx.Engine.DiscardFromHand(ctx.Player, toy);
                ctx.GustOpponent("Beckoning Tail: choose 1 of your opponent's Benched Pokémon to switch in");
            }
        }

        /// <summary>Magical Charm: the Defending Pokémon does 100 less next turn. Angelite: shuffle 2 opposing Benched Pokémon into their deck (not twice in a row).</summary>
        private sealed class SylveonExSSP : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Angelite") return true;
                int last = ctx.MyLastTurn;
                return !ctx.MyPokemon().Any(m => m.LastAttackName == "Angelite" && m.LastAttackTurn == last);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Magical Charm")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                        ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 100 less damage during its next turn (Magical Charm)",
                            ctx.Player, defending.Uid, ctx.Turn + 1, 100));
                    return true;
                }
                if (atk.Name == "Angelite")
                {
                    var targets = ctx.GustTargets();
                    if (targets.Count == 0) return true;
                    var picks = ctx.ChoosePokemonMany(targets, Math.Min(2, targets.Count), 2, "Angelite: choose 2 of your opponent's Benched Pokémon to shuffle into their deck");
                    foreach (var m in picks) ctx.Engine.ShufflePokemonIntoDeck(m);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Aurora Gain: heal 30. Giga Impact: can't attack next turn.</summary>
        private sealed class XerneasSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Aurora Gain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                    return true;
                }
                if (atk.Name == "Giga Impact")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sand Tomb: the Defending Pokémon can't retreat. Barite Jail: damage counters on each opposing Benched Pokémon until it has 100 HP left.</summary>
        private sealed class PalossandExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Sand Tomb")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Barite Jail")
                {
                    foreach (var b in ctx.OpponentBench())
                    {
                        int counters = (ctx.RemainingHp(b) - 100) / 10;
                        if (counters > 0) ctx.PlaceCounters(b, counters);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>Perplex: Confused. Mental Crush: +90 if the opponent's Active Pokémon is Confused.</summary>
        private sealed class TapuLeleSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Perplex")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Mental Crush")
                {
                    bool confused = defending != null && defending.Has(SpecialCondition.Confused);
                    ctx.DamageDefending(atk.Damage + (confused ? 90 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Obliging Heal: played from the hand to the Bench, you may heal 30 from your Active Pokémon and remove a Special Condition from it.</summary>
        private sealed class IndeedeeSSP : Mon
        {
            private static readonly SpecialCondition[] All =
                { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                bool ready = active != null && (active.DamageCounters > 0 || active.Conditions != SpecialCondition.None);
                if (!TriggerOnce(ctx, "Obliging Heal", ready, "Obliging Heal: heal 30 damage from your Active Pokémon and remove a Special Condition from it?")) return;
                ctx.Heal(active, 30);
                var has = All.Where(c => active.Has(c)).ToList();
                if (has.Count == 0) return;
                int pick = ctx.ChooseOneOption("Obliging Heal: which Special Condition should " + active.Name + " recover from?", has.Select(c => c.ToString()).ToArray());
                ctx.Engine.RemoveCondition(active, has[pick]);
            }
        }

        /// <summary>Mystical Eyes: devolve 1 of the opponent's evolved Pokémon (the top Evolution card goes to their hand). Spiral Drain: heal 30.</summary>
        private sealed class EspathraSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mystical Eyes")
                {
                    var evolved = ctx.OpponentPokemon(m => m.Stack.Count > 1 && !m.Def.IsFossil && !m.Def.IsMultiCard);
                    if (evolved.Count == 0) return true;
                    var target = ctx.ChoosePokemon(evolved, "Mystical Eyes: devolve which of your opponent's Pokémon?");
                    if (target != null) ctx.Engine.Devolve(target, 1);
                    return true;
                }
                if (atk.Name == "Spiral Drain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Perplexing Transfer: move all damage counters from 1 of your Benched Ancient Pokémon to the opponent's Active Pokémon. Moonblast: the Defending Pokémon does 30 less next turn.</summary>
        private sealed class FlutterManeSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Perplexing Transfer")
                {
                    var defending = ctx.Defending;
                    var sources = ctx.MyBench().Where(b => b.Def.IsAncient && b.DamageCounters > 0).ToList();
                    if (defending == null || sources.Count == 0) return true;
                    var from = ctx.ChoosePokemon(sources, "Perplexing Transfer: move all damage counters from which Benched Ancient Pokémon?");
                    if (from != null) ctx.Engine.MoveDamageCounters(from, defending, from.DamageCounters, "Perplexing Transfer");
                    return true;
                }
                if (atk.Name == "Moonblast")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                        ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 30 less damage during its next turn (Moonblast)",
                            ctx.Player, defending.Uid, ctx.Turn + 1, 30));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fighting

        private sealed class PrimeapeSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweep the Leg") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Sweep the Leg: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Blocking Stomp: a Basic Defending Pokémon can't attack during the opponent's next turn.</summary>
        private sealed class PaldeanTaurosFightingSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blocking Stomp") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Def.IsBasicPokemon)
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Blocking Stomp)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Knock Flat: ignores effects on the opponent's Active Pokémon. Guarded Rolling: discard 2 Energy; takes 100 less next turn.</summary>
        private sealed class DonphanSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Knock Flat")
                {
                    ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                if (atk.Name == "Guarded Rolling")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    ctx.SelfTakesLessNextTurn(100);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Screech: during your next turn, the Defending Pokémon takes 50 more damage from attacks.</summary>
        private sealed class VibravaSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Screech") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                var effect = new DamageTakenEffect(defending.Name + " takes 50 more damage from attacks during your next turn (Screech)",
                    ctx.Player, defending.Uid, ctx.Turn + 2, 50);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>Reversing Storm: you may switch this Pokémon with a Benched one. Sonic Peridot: 100 to each opposing Pokémon ex and V, no Weakness or Resistance.</summary>
        private sealed class FlygonExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Reversing Storm")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Bench.Count > 0 && ctx.PokemonIsActive && ctx.AskYesNo("Reversing Storm: switch " + ctx.Pokemon.Name + " with 1 of your Benched Pokémon?"))
                        ctx.SwitchSelfWithBench();
                    return true;
                }
                if (atk.Name == "Sonic Peridot")
                {
                    foreach (var m in ctx.OpponentPokemon(x => TefPokemon.IsExOrV(x.Def))) ctx.DamageWith(m, 100, false, false, false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sticky Bind: while this Pokémon is on your Bench, Benched Stage 2 Pokémon (both sides) have no Abilities.</summary>
        private sealed class GastrodonSSP : Mon
        {
            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                if (src.Pokemon == null || !src.Me.Bench.Contains(src.Pokemon)) return false;
                return mon.Def.Stage == Stage.Stage2 && src.State.Players[mon.Owner].Bench.Contains(mon);
            }
        }

        /// <summary>Digging Claw: discard the top card of the opponent's deck. Drill Smash: +80 with a Metal Pokémon on your Bench.</summary>
        private sealed class ExcadrillSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Digging Claw")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                    return true;
                }
                if (atk.Name == "Drill Smash")
                {
                    bool metal = ctx.Me.Bench.Any(b => ctx.Engine.IsType(b, EnergyType.Metal));
                    ctx.DamageDefending(atk.Damage + (metal ? 80 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Fist of Focus: attach an Energy card from your discard pile to this Pokémon. Buster Swing: Resistance doesn't apply.</summary>
        private sealed class LandorusSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fist of Focus")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                    if (energy.Count == 0 || !ctx.InPlay(me)) return true;
                    var card = ctx.ChooseCards(energy, 1, 1, "Fist of Focus: choose an Energy card to attach to " + me.Name)[0];
                    ctx.Me.Discard.Remove(card);
                    ctx.Engine.AttachEnergyCard(card, me);
                    return true;
                }
                if (atk.Name == "Buster Swing")
                {
                    ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Raging Tentacles: costs only 1 Fighting while this Pokémon has damage counters.</summary>
        private sealed class GrapploctSSP : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Raging Tentacles" || ctx.Pokemon == null || ctx.Pokemon.DamageCounters == 0) return;
                cost.Clear();
                cost.Add(EnergyType.Fighting);
            }
        }

        /// <summary>No Energy cards from the hand can be attached to one Pokémon (Corrosive Shards).</summary>
        private sealed class CorrosiveShardsEffect : ContinuousEffect
        {
            public CorrosiveShardsEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target) { return target.Uid == TargetUid; }
        }

        /// <summary>Corrosive Shards: Poisoned; Energy cards can't be attached from the opponent's hand to it next turn.</summary>
        private sealed class GlimmoraSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Corrosive Shards") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.AddEffect(new CorrosiveShardsEffect("Energy cards can't be attached from the hand to " + defending.Name + " during its next turn (Corrosive Shards)",
                    ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Unrelenting Onslaught: +150 if another of your Ancient Pokémon attacked during your last turn.</summary>
        private sealed class KoraidonSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Unrelenting Onslaught") return false;
                var me = ctx.Pokemon;
                int last = ctx.MyLastTurn;
                bool onslaught = ctx.MyPokemon().Any(m => m != me && m.Def.IsAncient && m.LastAttackTurn == last);
                ctx.DamageDefending(atk.Damage + (onslaught ? 150 : 0));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Crashing Headbutt: discard the top 3 cards of the opponent's deck. Obsidian: also 130 to 2 opposing Benched Pokémon.</summary>
        private sealed class HydreigonExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Crashing Headbutt")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                    return true;
                }
                if (atk.Name == "Obsidian")
                {
                    ctx.DamageDefending(atk.Damage);
                    var bench = ctx.OpponentBench();
                    if (bench.Count == 0) return true;
                    var targets = ctx.ChoosePokemonMany(bench, Math.Min(2, bench.Count), 2, "Obsidian: choose 2 of your opponent's Benched Pokémon (130 damage each)");
                    foreach (var t in targets) ctx.DamageTarget(t, 130);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Mischievous Painting: attach up to 3 Energy cards from the opponent's discard pile to their Pokémon. Energized Graffiti: 40 per Energy on all their Pokémon.</summary>
        private sealed class GrafaiaiSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mischievous Painting")
                {
                    var opp = ctx.Opp;
                    for (int i = 0; i < 3; i++)
                    {
                        var energy = opp.Discard.Where(c => c.Def.IsEnergy).ToList();
                        if (energy.Count == 0 || !opp.HasPokemonInPlay) break;
                        var pick = ctx.ChooseCards(energy, 0, 1, "Mischievous Painting: choose an Energy card from your opponent's discard pile (" + (i + 1) + " of up to 3; none to stop)");
                        if (pick.Count == 0) break;
                        var card = pick[0];
                        var script = ctx.Engine.Scripts.For(card.Def);
                        var targets = ctx.OpponentPokemon(m => script.CanAttachTo(new EffectContext(ctx.Engine, ctx.Opponent, card, m), m));
                        if (targets.Count == 0) break;
                        var target = ctx.ChoosePokemon(targets, "Mischievous Painting: attach " + card.Def.Name + " to which of your opponent's Pokémon?");
                        if (target == null) break;
                        opp.Discard.Remove(card);
                        ctx.Engine.AttachEnergyCard(card, target);
                    }
                    return true;
                }
                if (atk.Name == "Energized Graffiti")
                {
                    ctx.DamageDefending(atk.Damage * ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m)));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

    }
}
