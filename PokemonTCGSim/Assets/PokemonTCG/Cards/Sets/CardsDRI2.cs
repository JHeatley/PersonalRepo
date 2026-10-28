using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Destined Rivals (DRI) 71-181: the cards that weren't already in the pool. Pokémon whose attacks only do their
    /// printed damage need no script: Electrike, Nosepass, Team Rocket's Nidoran♂, Marnie's Morgrem, Arven's Maschiff,
    /// Taillow.
    /// </summary>
    public static class CardsDRI2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DRI-71", new DrawAttackMon("Zapping Draw", 1));
            r.RegisterByKey("DRI-72", new SearchAttackMon("Procurement", d => d.IsItem, 1));
            r.RegisterByKey("DRI-73", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("DRI-74", new TeamRocketsAmpharosDRI());
            r.RegisterByKey("DRI-76", new ManectricDRI());
            r.RegisterByKey("DRI-77", new CardsDRI1.RotomDRI());
            r.RegisterByKey("DRI-79", new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep));
            r.RegisterByKey("DRI-80", new TeamRocketsHypnoDRI());
            r.RegisterByKey("DRI-82", new TeamRocketsWobbuffetDRI());
            r.RegisterByKey("DRI-83", new StevensBaltoyDRI());
            r.RegisterByKey("DRI-84", new StevensClaydolDRI());
            r.RegisterByKey("DRI-85", new TeamRocketsChinglingDRI());
            r.RegisterByKey("DRI-86", new StevensCarbinkDRI());
            r.RegisterByKey("DRI-88", new TeamRocketsDottlerDRI());
            r.RegisterByKey("DRI-89", new TeamRocketsOrbeetleDRI());
            r.RegisterByKey("DRI-90", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("DRI-91", new CardsWHT.DragOffMon("Drag Off", 30));
            r.RegisterByKey("DRI-92", new AnnihilapeDRI());
            r.RegisterByKey("DRI-93", new EthansSudowoodoDRI());
            r.RegisterByKey("DRI-94", new CardsSSP2.StompOffMon("Mountain Munch", 1));
            r.RegisterByKey("DRI-95", new TeamRocketsPupitarDRI());
            r.RegisterByKey("DRI-96", new TeamRocketsTyranitarDRI());
            r.RegisterByKey("DRI-98", new ProbopassDRI());
            r.RegisterByKey("DRI-99", new FlipBonusMon("Steady Punch", 20));
            r.RegisterByKey("DRI-100", new MedichamDRI());
            r.RegisterByKey("DRI-101", new RegirockExDRI());
            r.RegisterByKey("DRI-105", new CardsDRI1.WhirlwindMon("Push Down"));
            r.RegisterByKey("DRI-106", new HippowdonDRI());
            r.RegisterByKey("DRI-107", new CardsPFL.FlipUntilTailsTimesMon("Running Charge"));
            r.RegisterByKey("DRI-108", new MudCoatMon(30));
            r.RegisterByKey("DRI-109", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("DRI-110", new ArvensToedscruelDRI());
            r.RegisterByKey("DRI-111", new CardsDRI1.OgerponDRI("Rock Kagura", EnergyType.Fighting));
            r.RegisterByKey("DRI-112", new ParalyzeFlipMon("Drag Down"));
            r.RegisterByKey("DRI-113", new TeamRocketsArbokDRI());
            r.RegisterByKey("DRI-114", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("DRI-115", new TeamRocketsNidorinaDRI());
            r.RegisterByKey("DRI-116", new TeamRocketsNidoqueenDRI());
            r.RegisterByKey("DRI-118", new CardsBLK.FinishingBlowMon("Horn Rend"));
            r.RegisterByKey("DRI-119", new CardsBS1.ToxicMon("Tainted Horn", 8));
            r.RegisterByKey("DRI-120", new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned));
            r.RegisterByKey("DRI-121", new TeamRocketsGolbatDRI());
            r.RegisterByKey("DRI-122", new TeamRocketsCrobatExDRI());
            r.RegisterByKey("DRI-123", new TeamRocketsGrimerDRI());
            r.RegisterByKey("DRI-124", new TeamRocketsMukDRI());
            r.RegisterByKey("DRI-125", new TeamRocketsKoffingDRI());
            r.RegisterByKey("DRI-126", new TeamRocketsWeezingDRI());
            r.RegisterByKey("DRI-130", new ExBonusMon("Pointy Nails", 40));
            r.RegisterByKey("DRI-131", new ExBonusMon("Pointy Claws", 70));
            r.RegisterByKey("DRI-132", new CrunchMon("Crunch"));
            r.RegisterByKey("DRI-133", new RecoilMon("Wild Tackle", 30));
            r.RegisterByKey("DRI-134", new DrawAttackMon("Filch", 1));
            r.RegisterByKey("DRI-136", new MarniesGrimmsnarlExDRI());
            r.RegisterByKey("DRI-137", new MarniesMorpekoDRI());
            r.RegisterByKey("DRI-139", new ArvensMabosstiffExDRI());
            r.RegisterByKey("DRI-140", new ForretressDRI());
            r.RegisterByKey("DRI-141", new SkarmoryDRI());
            r.RegisterByKey("DRI-142", new CardsPGO.SplitBombMon("Sonic Double"));
            r.RegisterByKey("DRI-146", new ZamazentaDRI());
            r.RegisterByKey("DRI-147", new ConditionAttackMon("Dangerous Incisors", SpecialCondition.Poisoned));
            r.RegisterByKey("DRI-148", new CardsEPO.FinalGambitMon("Reckless Abandon", 90));
            r.RegisterByKey("DRI-149", new TeamRocketsMeowthDRI());
            r.RegisterByKey("DRI-150", new TeamRocketsPersianExDRI());
            r.RegisterByKey("DRI-151", new CoinsMon("Dizzy Punch", 2));
            r.RegisterByKey("DRI-152", new TaurosDRI());
            r.RegisterByKey("DRI-155", new TeamRocketsPorygonZDRI());
            r.RegisterByKey("DRI-157", new DrawAttackMon("Add On", 3));
            r.RegisterByKey("DRI-158", new GnawThroughMon("Gnaw Through"));
            r.RegisterByKey("DRI-159", new ArvensGreedentDRI());
            r.RegisterByKey("DRI-160", new CardsDRI1.WhirlwindMon("Push Down", true));

            r.RegisterByName("Arven's Sandwich", new ArvensSandwich());
            r.RegisterByName("Emcee's Hype", new EmceesHype());
            r.RegisterByName("Ethan's Adventure", new EthansAdventure());
            r.RegisterByName("Granite Cave", new GraniteCave());
            r.RegisterByName("Spikemuth Gym", new SpikemuthGym());
            r.RegisterByName("Team Rocket's Bother-Bot", new TeamRocketsBotherBot());
            r.RegisterByName("Team Rocket's Great Ball", new TeamRocketsGreatBall());
            r.RegisterByName("Team Rocket's Venture Bomb", new TeamRocketsVentureBomb());
            r.RegisterByName("TM Machine", new TMMachine());
        }

        // ================================================================== Shared helpers

        /// <summary>"If your opponent's Active Pokémon is a Pokémon ex, this attack does N more damage."</summary>
        public sealed class ExBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public ExBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var d = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (d != null && (d.Def.IsEx || d.Def.IsMegaEx) ? bonus : 0));
                return true;
            }
        }

        /// <summary>Crunch: discard an Energy from the opponent's Active Pokémon after the damage.</summary>
        public sealed class CrunchMon : Mon
        {
            private readonly string attack;

            public CrunchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                DiscardDefendingEnergy(ctx, atk);
                return true;
            }

            public static void DiscardDefendingEnergy(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.DiscardEnergyFrom(defending, 1, atk.Name + ": discard an Energy from " + defending.Name);
            }
        }

        /// <summary>Corrosive Sludge: at the end of the opponent's next turn, the Defending Pokémon and its cards are discarded (not a Knock Out).</summary>
        private sealed class DiscardDefendingLaterEffect : ContinuousEffect
        {
            private readonly string attack;

            public DiscardDefendingLaterEffect(string name, int owner, int targetUid, int currentTurn, string attack)
                : base(name, owner, targetUid, currentTurn + 1, true)
            {
                this.attack = attack;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon == null || engine.CurrentEffectPrevented(mon, false)) return;
                engine.Log(Owner, attack + ": " + mon.Name + " is discarded.");
                engine.DiscardPokemonFromPlay(mon);
            }
        }

        /// <summary>Strong Bash: during the opponent's next turn, damage done to this Pokémon by an attack is put on the Attacking Pokémon as damage counters.</summary>
        private sealed class MirrorDamageEffect : ContinuousEffect
        {
            public MirrorDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage < 10) return;
                engine.PlaceDamageCounters(info.Attacker, damage / 10, Name);
            }
        }

        // ================================================================== Lightning

        /// <summary>Darkest Impulse: when the opponent evolves a Pokémon from their hand, put 4 damage counters on it (doesn't stack).</summary>
        private sealed class TeamRocketsAmpharosDRI : Mon
        {
            public override void OnOpponentEvolvedFromHand(EffectContext src, PokemonInPlay evolved)
            {
                // Only one Darkest Impulse applies: the first such Ampharos in play.
                var first = src.MyPokemon(m => src.Engine.HasAbilityNamed(m, "Darkest Impulse")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != src.Pokemon || !src.InPlay(evolved)) return;
                Announce(src, "Darkest Impulse");
                src.PlaceCounters(evolved, 4);
            }
        }

        /// <summary>Flash Impact: also 40 damage to 1 of your own Benched Pokémon.</summary>
        private sealed class ManectricDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Impact") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Flash Impact: choose 1 of your Benched Pokémon to take 40 damage");
                if (target != null) ctx.DamageTarget(target, 40);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Bench Manipulation: the opponent flips a coin for each of their Benched Pokémon; 80 per tails, no Weakness or Resistance.</summary>
        private sealed class TeamRocketsHypnoDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bench Manipulation") return false;
                int n = ctx.Opp.Bench.Count;
                int heads = n == 0 ? 0 : ctx.Engine.FlipCoins(ctx.Opponent, n, "Bench Manipulation");
                int tails = n - heads;
                if (tails > 0 && ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage * tails, false, false, false);
                return true;
            }
        }

        /// <summary>Rocket Mirror: move all damage counters from 1 of your Benched Team Rocket's Pokémon to the opponent's Active Pokémon.</summary>
        private sealed class TeamRocketsWobbuffetDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rocket Mirror") return false;
                var from = ctx.Me.Bench.Where(m => m.Def.IsTeamRockets && m.DamageCounters > 0).ToList();
                if (from.Count == 0 || ctx.Defending == null) return true;
                var source = ctx.ChoosePokemon(from, "Rocket Mirror: move all damage counters from which Benched Team Rocket's Pokémon?");
                if (source != null) ctx.Engine.MoveDamageCounters(source, ctx.Defending, source.DamageCounters, "Rocket Mirror");
                return true;
            }
        }

        /// <summary>Summoning Sign: put up to 2 Basic Steven's Pokémon from your deck onto your Bench.</summary>
        private sealed class StevensBaltoyDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Summoning Sign") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.IsOwnersPokemon("Steven"), 2, "Summoning Sign: put up to 2 Basic Steven's Pokémon onto your Bench");
                return true;
            }
        }

        /// <summary>Eerie Light: Confused. Clay Blast: discard all Energy from this Pokémon.</summary>
        private sealed class StevensClaydolDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Eerie Light")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Clay Blast")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Chiming Commotion: discard a random card from the opponent's hand.</summary>
        private sealed class TeamRocketsChinglingDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chiming Commotion") return false;
                TefPokemon.DiscardRandomFromOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Stone Palace: while this Pokémon is on your Bench, your Steven's Pokémon take 30 less damage from the opponent's attacks (doesn't stack).</summary>
        private sealed class StevensCarbinkDRI : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || src.IsActive(src.Pokemon) || !info.AgainstOpponent) return damage;
                if (info.Target.Owner != src.Player || !info.Target.Def.IsOwnersPokemon("Steven")) return damage;
                return info.ApplyOnce("Stone Palace") ? damage - 30 : damage;
            }
        }

        /// <summary>Disruptive Radar: look at the top 5 cards of the opponent's deck and put them back in any order.</summary>
        private sealed class TeamRocketsDottlerDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disruptive Radar") return false;
                ReorderOpponentTop(ctx, 5, atk.Name);
                return true;
            }
        }

        /// <summary>"Look at the top N cards of your opponent's deck and put them back in any order."</summary>
        public static void ReorderOpponentTop(EffectContext ctx, int count, string source)
        {
            var deck = ctx.Opp.Deck;
            var top = deck.Take(Math.Min(count, deck.Count)).ToList();
            if (top.Count == 0) return;
            ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of your opponent's deck", ctx.Player);
            foreach (var c in top) deck.Remove(c);
            var ordered = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = ctx.ChooseCards(left, 1, 1, source + ": choose the card to put " + (ordered.Count == 0 ? "on top of your opponent's deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                ordered.Add(pick);
                left.Remove(pick);
            }
            ordered.Add(left[0]);
            deck.InsertRange(0, ordered);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top " + ordered.Count + " cards of the other player's deck and put them back in any order.");
        }

        /// <summary>Rocket Brain: as often as you like during your turn, move 1 damage counter from 1 of your Team Rocket's Pokémon to another of your Pokémon. Psychic: +40 per Energy on the opponent's Active.</summary>
        private sealed class TeamRocketsOrbeetleDRI : Mon
        {
            private const string Brain = "Rocket Brain";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Brain || !ctx.IsMyTurn) return false;
                if (ctx.Engine.CounterMovementBlocked() || ctx.Me.InPlay().Count() < 2) return false;
                return ctx.Me.InPlay().Any(m => m.Def.IsTeamRockets && m.DamageCounters > 0);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing moves as many as they like, then presses Done; bots move one per use.
                RepeatableAbility.Repeat(ctx,
                    () => !ctx.Engine.CounterMovementBlocked() && ctx.Me.InPlay().Count() >= 2 && ctx.Me.InPlay().Any(m => m.Def.IsTeamRockets && m.DamageCounters > 0),
                    offerDone =>
                    {
                        var from = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m.Def.IsTeamRockets && m.DamageCounters > 0),
                            "Rocket Brain: move a damage counter from which Team Rocket's Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                        if (from == null) return false;
                        var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), "Rocket Brain: move it to which of your Pokémon?", offerDone);
                        if (to == null) return false;
                        ctx.Engine.MoveDamageCounters(from, to, 1, Brain);
                        return true;
                    });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending == null ? 0 : 40 * ctx.EnergyCount(ctx.Defending)));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Lose Cool: with 2 or more damage counters, its attacks do 120 more to the opponent's Active. Impact Blow: can't use it next turn.</summary>
        private sealed class AnnihilapeDRI : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || src.Pokemon.DamageCounters < 2 || !info.ToOpponentsActive) return damage;
                return damage + 120;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Impact Blow") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Impound: the Defending Pokémon can't retreat. Try to Imitate: flip; heads uses 1 of the opponent's Active Pokémon's attacks.</summary>
        private sealed class EthansSudowoodoDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Impound")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Try to Imitate")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    CardDef copied;
                    int index;
                    if (ChooseCopiedAttack(ctx, defending, "Try to Imitate: choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index))
                        UseCopiedAttack(ctx, copied, index);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Explosive Ascension: evolve this Pokémon with a card from your deck that evolves from it.</summary>
        private sealed class TeamRocketsPupitarDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explosive Ascension") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Deck.Count > 0)
                {
                    CardsSSP1.EvolveFromDeck(ctx, ctx.Pokemon, "Explosive Ascension: choose a card that evolves from " + ctx.Pokemon.Name);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Sand Stream: in Pokémon Checkup, while Active, 2 damage counters on each of the opponent's Basic Pokémon. Demolition Tackle: discard an Energy from the opponent's Active.</summary>
        private sealed class TeamRocketsTyranitarDRI : Mon
        {
            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? 1 - src.Player : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon)) return;
                var basics = src.OpponentPokemon(m => m.Def.IsBasicPokemon);
                if (basics.Count == 0) return;
                Announce(src, "Sand Stream");
                foreach (var m in basics) src.PlaceCounters(m, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Demolition Tackle") return false;
                CrunchMon.DiscardDefendingEnergy(ctx, atk);
                return true;
            }
        }

        /// <summary>Mountain Drop: +70 if a Stadium is in play.</summary>
        private sealed class ProbopassDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Drop") return false;
                ctx.DamageDefending(atk.Damage + (ctx.State.Stadium != null ? 70 : 0));
                return true;
            }
        }

        /// <summary>Harmonious Spirit Palm: +120 if this Pokémon and the opponent's Active have the same amount of Energy.</summary>
        private sealed class MedichamDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Harmonious Spirit Palm") return false;
                bool same = ctx.Defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(ctx.Defending);
                ctx.DamageDefending(atk.Damage + (same ? 120 : 0));
                return true;
            }
        }

        /// <summary>Regi Charge: attach up to 2 Basic Fighting Energy from your discard pile to it. Giant Rock: +140 against a Stage 2.</summary>
        private sealed class RegirockExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Regi Charge")
                {
                    var fighting = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                    if (fighting.Count == 0) return true;
                    var picks = ctx.ChooseCards(fighting, 0, Math.Min(2, fighting.Count), "Regi Charge: attach up to 2 Basic Fighting Energy from your discard pile");
                    foreach (var e in picks)
                        if (ctx.Me.Discard.Remove(e)) ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                    return true;
                }
                if (atk.Name == "Giant Rock")
                {
                    bool stage2 = ctx.Defending != null && ctx.Defending.Def.Stage == Stage.Stage2;
                    ctx.DamageDefending(atk.Damage + (stage2 ? 140 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Super Sandstorm: also 40 to each Benched Pokémon (both sides) that has damage counters.</summary>
        private sealed class HippowdonDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Sandstorm") return false;
                ctx.DamageDefending(atk.Damage);
                var hurt = ctx.OpponentBench().Concat(ctx.MyBench()).Where(m => m.DamageCounters > 0).ToList();
                foreach (var m in hurt) ctx.DamageTarget(m, 40);
                return true;
            }
        }

        /// <summary>Mud Coat, Solid Body: this Pokémon takes 30 less damage from attacks. The numbers are passed in.</summary>
        internal sealed class MudCoatMon : Mon
        {
            private readonly int amount;

            public MudCoatMon(int amount)
            {
                this.amount = amount;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - amount : damage;
            }
        }

        /// <summary>Pull: switch in 1 of the opponent's Benched Pokémon. Reckless Charge: 30 damage to itself.</summary>
        private sealed class ArvensToedscruelDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pull")
                {
                    ctx.GustOpponent("Pull: choose 1 of your opponent's Benched Pokémon to switch in");
                    return true;
                }
                if (atk.Name == "Reckless Charge")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(30);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Darkness

        /// <summary>Potent Glare: while Active, the opponent can't play Pokémon with an Ability from their hand (except Team Rocket's Pokémon). Spinning Tail: 30 to each of the opponent's Pokémon.</summary>
        private sealed class TeamRocketsArbokDRI : Mon
        {
            public override bool BlocksPlayingPokemon(EffectContext src, int player, CardDef card)
            {
                if (src.Pokemon == null || player == src.Player || !src.IsActive(src.Pokemon)) return false;
                return card.HasAbility && !card.IsTeamRockets; // Pokémon Powers / Poké-Powers / Poké-Bodies aren't Abilities
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spinning Tail") return false;
                foreach (var m in ctx.OpponentPokemon()) ctx.DamageTarget(m, 30);
                return true;
            }
        }

        /// <summary>Dark Awakening: choose up to 2 of your Darkness Pokémon and evolve each with a card from your deck.</summary>
        private sealed class TeamRocketsNidorinaDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Awakening") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var dark = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Darkness)
                                              && ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.EvolvesFrom == m.Name));
                if (dark.Count > 0)
                {
                    var picks = ctx.ChoosePokemonMany(dark, 0, Math.Min(2, dark.Count), "Dark Awakening: choose up to 2 of your Darkness Pokémon to evolve");
                    foreach (var m in picks)
                        CardsSSP1.EvolveFromDeck(ctx, m, "Dark Awakening: choose a card that evolves from " + m.Name);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Love Impact: +120 if a Pokémon with "Nidoking" in its name is on your Bench.</summary>
        private sealed class TeamRocketsNidoqueenDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Love Impact") return false;
                bool king = ctx.Me.Bench.Any(m => m.Name.Contains("Nidoking"));
                ctx.DamageDefending(atk.Damage + (king ? 120 : 0));
                return true;
            }
        }

        /// <summary>Sneaky Bite: when played to evolve, you may put 2 damage counters on 1 of the opponent's Pokémon. Confuse Ray: Confused.</summary>
        private sealed class TeamRocketsGolbatDRI : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Sneaky Bite", ctx.Opp.HasPokemonInPlay, "Sneaky Bite: put 2 damage counters on 1 of your opponent's Pokémon?")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Sneaky Bite: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Confuse Ray") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Biting Spree: when played to evolve, you may put 2 damage counters on each of 2 of the opponent's Pokémon. Assassin's Return: you may put this Pokémon into your hand.</summary>
        private sealed class TeamRocketsCrobatExDRI : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Biting Spree", ctx.Opp.HasPokemonInPlay, "Biting Spree: put 2 damage counters on each of 2 of your opponent's Pokémon?")) return;
                var all = ctx.OpponentPokemon();
                int n = Math.Min(2, all.Count);
                foreach (var t in ctx.ChoosePokemonMany(all, n, n, "Biting Spree: choose " + n + " of your opponent's Pokémon"))
                    ctx.PlaceCounters(t, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assassin's Return") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Engine.IsKnockedOut(me)) return true;
                if (!ctx.AskYesNo("Assassin's Return: put " + me.Name + " into your hand (discarding all attached cards)?")) return true;
                foreach (var e in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, e);
                ctx.Engine.DiscardAllTools(me);
                ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>Corrosive Sludge: at the end of the opponent's next turn, discard the Defending Pokémon and all attached cards.</summary>
        private sealed class TeamRocketsGrimerDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Corrosive Sludge") return false;
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new DiscardDefendingLaterEffect(defending.Name + " will be discarded at the end of " + ctx.Engine.Their(ctx.Opponent) + " next turn (Corrosive Sludge)",
                    ctx.Player, defending.Uid, ctx.Turn, atk.Name));
                return true;
            }
        }

        /// <summary>Gooped Up: Confused, and it can't retreat next turn. Hazardous Venom: 100 for each Special Condition on the opponent's Active.</summary>
        private sealed class TeamRocketsMukDRI : Mon
        {
            private static int Conditions(PokemonInPlay mon)
            {
                if (mon == null) return 0;
                int n = 0;
                foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                    if (mon.Has(c)) n++;
                return n;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Gooped Up")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending)) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Hazardous Venom")
                {
                    ctx.DamageDefending(atk.Damage * Conditions(defending));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Smog Signals: when it's damaged by an opponent's attack while Active (even if Knocked Out), put up to 2 Pokémon with "Koffing" in their name from your deck onto your Bench.</summary>
        private sealed class TeamRocketsKoffingDRI : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || ctx.Me.Deck.Count == 0 || ctx.BenchSpace <= 0) return;
                Announce(ctx, "Smog Signals");
                ctx.SearchToBench(d => d.Name.Contains("Koffing"), 2, "Smog Signals: put up to 2 Pokémon with \"Koffing\" in their name onto your Bench");
            }
        }

        /// <summary>Explode Together Now: 40 for each Pokémon in play with "Koffing" or "Weezing" in its name (both sides).</summary>
        private sealed class TeamRocketsWeezingDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explode Together Now") return false;
                int n = ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Count(m => m.Name.Contains("Koffing") || m.Name.Contains("Weezing"));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Punk Up: when played to evolve, you may attach up to 5 Basic Darkness Energy from your deck to your Marnie's Pokémon in any way. Shadow Bullet: also 30 to 1 Benched Pokémon.</summary>
        private sealed class MarniesGrimmsnarlExDRI : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Punk Up", ctx.Me.Deck.Count > 0, "Punk Up: search your deck for up to 5 Basic Darkness Energy and attach them to your Marnie's Pokémon?")) return;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Darkness), 5, "Punk Up: choose up to 5 Basic Darkness Energy cards");
                ctx.AttachCardsAnyWay(found, found.Count, (c, m) => m.Def.IsOwnersPokemon("Marnie"), "Punk Up", false);
                foreach (var c in found) // anything that couldn't be attached goes back
                    if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c))) ctx.Me.Deck.Add(c);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Bullet") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(30);
                return true;
            }
        }

        /// <summary>Spiky Wheel: +40 for each Darkness Energy attached to this Pokémon.</summary>
        private sealed class MarniesMorpekoDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiky Wheel") return false;
                ctx.DamageDefending(atk.Damage + 40 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Darkness));
                return true;
            }
        }

        /// <summary>Vigorous Tackle: +120 with no damage counters on it. Boss Headbutt: can't use it next turn.</summary>
        private sealed class ArvensMabosstiffExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Vigorous Tackle")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.DamageCounters == 0 ? 120 : 0));
                    return true;
                }
                if (atk.Name == "Boss Headbutt")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantUseAttackNextTurn(atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        /// <summary>Iron Shake-Up: you may move any amount of Metal Energy between your Pokémon. Hurricane of Needles: 4 coins, 80 per heads.</summary>
        private sealed class ForretressDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hurricane of Needles")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(4));
                    return true;
                }
                if (atk.Name != "Iron Shake-Up") return false;
                ctx.DamageDefending(atk.Damage);
                for (int guard = 0; guard < 30; guard++)
                {
                    var holders = ctx.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Metal)));
                    if (holders.Count == 0 || ctx.MyPokemon().Count < 2) break;
                    var labels = new List<string> { "Done moving Metal Energy" };
                    labels.AddRange(holders.Select(m => "Move a Metal Energy from " + ctx.Engine.DescribeFor(ctx.Player, m)));
                    int pick = ctx.ChooseOption("Iron Shake-Up: move Metal Energy between your Pokémon?", labels.ToArray());
                    if (pick == 0) break;
                    var from = holders[pick - 1];
                    var metal = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, EnergyType.Metal)).ToList();
                    var energy = metal.Count == 1 ? metal[0] : ctx.ChooseCards(metal, 1, 1, "Iron Shake-Up: move which Energy?")[0];
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Iron Shake-Up: move " + energy.Def.Name + " to which Pokémon?");
                    if (to == null || !ctx.Engine.MoveEnergy(from, to, energy)) break;
                }
                return true;
            }
        }

        /// <summary>Roost: heal 50, and it can't retreat during your next turn.</summary>
        private sealed class SkarmoryDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roost") return false;
                var me = ctx.Pokemon;
                ctx.Heal(me, 50);
                var effect = new CantRetreatEffect(me.Name + " can't retreat during its next turn (Roost)", ctx.Player, me.Uid, ctx.Turn + 2);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>Strong Bash: during the opponent's next turn, damage done to it by attacks comes back as damage counters on the Attacking Pokémon.</summary>
        private sealed class ZamazentaDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong Bash") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new MirrorDamageEffect(me.Name + " returns attack damage as damage counters (Strong Bash)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Paw-cket Pilfer: a random card from the opponent's hand is shuffled into their deck. Fury Swipes: 3 coins, 20 per heads.</summary>
        private sealed class TeamRocketsMeowthDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Paw-cket Pilfer")
                {
                    CardsTWM1.ShuffleRandomFromOpponentHand(ctx, 1);
                    return true;
                }
                if (atk.Name == "Fury Swipes")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Haughty Order: use an attack of a Pokémon among the top 10 cards of the opponent's deck, then they shuffle. Cruel Slash: Confused.</summary>
        private sealed class TeamRocketsPersianExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Cruel Slash")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name != "Haughty Order") return false;
                var top = ctx.Opp.Deck.Take(Math.Min(10, ctx.Opp.Deck.Count)).ToList();
                if (top.Count == 0) return true;
                ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of your opponent's deck", ctx.Player);
                var mons = top.Where(c => c.Def.IsPokemon && c.Def.Attacks.Count > 0).ToList();
                CardDef copied = null;
                int index = -1;
                if (mons.Count > 0)
                {
                    var pick = ctx.ChooseCards(mons, 0, 1, "Haughty Order: choose a Pokémon whose attack to use");
                    if (pick.Count > 0)
                    {
                        copied = pick[0].Def;
                        index = ChooseAttackOf(ctx, copied, "Haughty Order: choose 1 of " + copied.Name + "'s attacks to use");
                    }
                }
                else ctx.Log("There were no Pokémon with attacks among the top " + top.Count + " cards of the other player's deck.");
                if (copied != null && index >= 0) UseCopiedAttack(ctx, copied, index);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Clean Hit: +50 against an Evolution Pokémon.</summary>
        private sealed class TaurosDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clean Hit") return false;
                bool evolved = ctx.Defending != null && ctx.Defending.Def.IsEvolution;
                ctx.DamageDefending(atk.Damage + (evolved ? 50 : 0));
                return true;
            }
        }

        /// <summary>Reconstitute: once during your turn, discard 2 cards from your hand to draw a card. R Command: 20 for each Team Rocket Supporter in your discard pile.</summary>
        private sealed class TeamRocketsPorygonZDRI : Mon
        {
            protected override string Activated { get { return "Reconstitute"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count >= 2 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Reconstitute: discard 2 cards from your hand");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "R Command") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.Discard.Count(c => c.Def.IsSupporter && c.Def.Name.Contains("Team Rocket")));
                return true;
            }
        }

        /// <summary>Gnaw Through: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon. Used for Gnaw Through, Peck Off (the attack name is passed in).</summary>
        internal sealed class GnawThroughMon : Mon
        {
            private readonly string attack;

            public GnawThroughMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (defending != null && defending.Tool != null && ctx.CanAffect(defending)) ctx.Engine.DiscardTool(defending);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Greedy Order: when played to evolve, you may put up to 2 Arven's Sandwich from your discard pile into your hand.</summary>
        private sealed class ArvensGreedentDRI : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var sandwiches = ctx.Me.Discard.Where(c => c.Def.Name == "Arven's Sandwich").ToList();
                if (!TriggerOnce(ctx, "Greedy Order", sandwiches.Count > 0, "Greedy Order: put up to 2 Arven's Sandwich from your discard pile into your hand?")) return;
                var picks = ctx.ChooseCards(sandwiches, 1, Math.Min(2, sandwiches.Count), "Greedy Order: choose up to 2 Arven's Sandwich");
                ctx.DiscardToHand(picks);
            }
        }

        // ================================================================== Trainers

        /// <summary>Heal 30 from your Active Pokémon (100 if it's an Arven's Pokémon).</summary>
        private sealed class ArvensSandwich : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && a.DamageCounters > 0 && !ctx.Engine.HealingBlocked(a);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                if (a != null) ctx.Heal(a, a.Def.IsOwnersPokemon("Arven") ? 100 : 30);
            }
        }

        /// <summary>Draw 2 cards, 2 more if the opponent has 3 or fewer Prize cards left.</summary>
        private sealed class EmceesHype : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(ctx.Opp.Prizes.Count <= 3 ? 4 : 2);
            }
        }

        /// <summary>Search for up to 3 Ethan's Pokémon and Basic Fire Energy in any combination.</summary>
        private sealed class EthansAdventure : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsOwnersPokemon("Ethan") || CardsSSP1.IsBasicOf(d, EnergyType.Fire), 3,
                    "Ethan's Adventure: search for up to 3 Ethan's Pokémon and Basic Fire Energy cards");
            }
        }

        /// <summary>Steven's Pokémon (both players') take 30 less damage from the opponent's attacks.</summary>
        private sealed class GraniteCave : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon != null || !info.AgainstOpponent || !info.Target.Def.IsOwnersPokemon("Steven")) return damage;
                return damage - 30;
            }
        }

        /// <summary>Once during each player's turn, they may search their deck for a Marnie's Pokémon.</summary>
        private sealed class SpikemuthGym : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsOwnersPokemon("Marnie"), 1, "Spikemuth Gym: search your deck for a Marnie's Pokémon");
                return false;
            }
        }

        /// <summary>Turn 1 of the opponent's face-down Prize cards face up; a random card from their hand is revealed and you may have them switch the two.</summary>
        private sealed class TeamRocketsBotherBot : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Opp.Prizes.Any(c => !ctx.Opp.FaceUpPrizeIds.Contains(c.Id));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var opp = ctx.Opp;
                var faceDown = opp.Prizes.Where(c => !opp.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (faceDown.Count == 0) return;
                var labels = new string[faceDown.Count];
                for (int i = 0; i < faceDown.Count; i++) labels[i] = "Face-down Prize card " + (i + 1);
                var prize = faceDown[ctx.ChooseOption("Team Rocket's Bother-Bot: turn which of your opponent's Prize cards face up?", labels)];
                opp.FaceUpPrizeIds.Add(prize.Id);
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { prize }, "had a Prize card turned face up");
                if (opp.Hand.Count == 0) return;
                var card = opp.Hand[ctx.State.Rng.Next(opp.Hand.Count)];
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { card }, "revealed a random card from " + ctx.Engine.Their(ctx.Opponent) + " hand");
                if (!ctx.AskYesNo("Team Rocket's Bother-Bot: have your opponent switch " + card.Def.Name + " from their hand with the face-up " + prize.Def.Name + "?")) return;
                int slot = opp.Prizes.IndexOf(prize);
                opp.Hand.Remove(card);
                opp.Prizes[slot] = card;
                opp.Hand.Add(prize);
                opp.FaceUpPrizeIds.Remove(prize.Id);
                opp.FaceUpPrizeIds.Add(card.Id); // the Prize card stays face up
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " switched " + card.Def.Name + " with the face-up Prize card " + prize.Def.Name + ".");
            }
        }

        /// <summary>Flip: heads finds an Evolution Team Rocket's Pokémon, tails a Basic Team Rocket's Pokémon.</summary>
        private sealed class TeamRocketsGreatBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip())
                    ctx.SearchToHand(d => d.IsEvolution && d.IsTeamRockets, 1, "Team Rocket's Great Ball: search your deck for an Evolution Team Rocket's Pokémon");
                else
                    ctx.SearchToHand(d => d.IsBasicPokemon && d.IsTeamRockets, 1, "Team Rocket's Great Ball: search your deck for a Basic Team Rocket's Pokémon");
            }
        }

        /// <summary>Flip: heads puts 2 damage counters on 1 of the opponent's Pokémon, tails 2 on your Active Pokémon.</summary>
        private sealed class TeamRocketsVentureBomb : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Team Rocket's Venture Bomb: put 2 damage counters on which of your opponent's Pokémon?");
                    if (target != null) ctx.PlaceCounters(target, 2);
                }
                else if (ctx.Me.Active != null) ctx.PlaceCounters(ctx.Me.Active, 2);
            }
        }

        /// <summary>Search for up to 3 Pokémon Tools with "Technical Machine" in their name.</summary>
        private sealed class TMMachine : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTool && d.Name.Contains("Technical Machine"), 3, "TM Machine: search your deck for up to 3 Technical Machines");
            }
        }
    }
}
