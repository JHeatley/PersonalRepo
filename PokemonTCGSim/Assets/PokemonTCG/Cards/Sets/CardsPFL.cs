using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Phantasmal Flames (PFL): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Oddish, Lotad, Charmeleon, Swinub, Pawmo, Misdreavus, Snubbull, Vibrava, Gastly, Haunter,
    /// Sandile, Krokorok.
    /// </summary>
    public static class CardsPFL
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PFL-2", new GloomPFL());
            r.RegisterByKey("PFL-3", new VileplumePFL());
            r.RegisterByKey("PFL-4", new MegaHeracrossExPFL());
            r.RegisterByKey("PFL-6", new CardsTWM1.HealSelfMon("Mega Drain", 30));
            r.RegisterByKey("PFL-7", new LudicoloPFL());
            r.RegisterByKey("PFL-8", new GenesectPFL());
            r.RegisterByKey("PFL-9", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("PFL-10", new CardsPLS1.StingMissileMon("Jumping Shot"));
            r.RegisterByKey("PFL-11", new CharmanderPFL());
            r.RegisterByKey("PFL-13", new MegaCharizardXExPFL());
            r.RegisterByKey("PFL-15", new CardsMEG.EnergyBonusMon("Blaze Ball", EnergyType.Fire, 20));
            r.RegisterByKey("PFL-16", new CardsMEG.EnergyBonusMon("Blaze Ball", EnergyType.Fire, 40));
            r.RegisterByKey("PFL-17", new RecoilMon("Burning Flare", 60));
            r.RegisterByKey("PFL-18", new OricorioExPFL());
            r.RegisterByKey("PFL-19", new SearchAttackMon("Gather Strength", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("PFL-20", new CeruledgePFL());
            r.RegisterByKey("PFL-21", new CardsTWM1.HealSelfMon("Bubble Drain", 20));
            r.RegisterByKey("PFL-22", new DewgongPFL());
            r.RegisterByKey("PFL-24", new FlipBonusMon("Rising Lunge", 30));
            r.RegisterByKey("PFL-25", new MamoswinePFL());
            r.RegisterByKey("PFL-26", new CardsMEG.EnergyInPlayBonusMon("Crystal Fall", EnergyType.Water, 4, 90));
            r.RegisterByKey("PFL-28", new CardsMEG.SnipeBenchMon("Targeted Dive", 70));
            r.RegisterByKey("PFL-29", new RotomExPFL());
            r.RegisterByKey("PFL-30", new FlipBonusMon("Play Rough", 20));
            r.RegisterByKey("PFL-31", new FlipBonusMon("Electric Run", 70));
            r.RegisterByKey("PFL-32", new CardsMEG.WeakenDefendingMon("Growl", 30));
            r.RegisterByKey("PFL-36", new MismagiusExPFL());
            r.RegisterByKey("PFL-38", new DamagedBonusMon("Finishing Blow", 90));
            r.RegisterByKey("PFL-39", new CardsMEG.ChargeSelfMon("Swelling Light", EnergyType.Psychic, 2));
            r.RegisterByKey("PFL-40", new MeloettaPFL());
            r.RegisterByKey("PFL-41", new MegaDiancieExPFL());
            r.RegisterByKey("PFL-42", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("PFL-43", new CardsTWM1.HealSelfMon("Draining Kiss", 10));
            r.RegisterByKey("PFL-44", new PerPokemonInPlayMon("Sweet Circle"));
            r.RegisterByKey("PFL-45", new CardsCRZ1.LimitBreakMon("Limit Break", 3, 90));
            r.RegisterByKey("PFL-46", new BramblinPFL());
            r.RegisterByKey("PFL-47", new BrambleghastPFL());
            r.RegisterByKey("PFL-48", new PaldeanTaurosPFL());
            r.RegisterByKey("PFL-49", new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned));
            r.RegisterByKey("PFL-50", new GliscorPFL());
            r.RegisterByKey("PFL-51", new CoinsMon("Double Headbutt", 2));
            r.RegisterByKey("PFL-53", new FlygonPFL());
            r.RegisterByKey("PFL-56", new MegaGengarExPFL());
            r.RegisterByKey("PFL-57", new FlipBonusMon("Ambush", 20));
            r.RegisterByKey("PFL-58", new CardsMEP.ExplosionYMon("Sniping Feathers", 2, 120));
            r.RegisterByKey("PFL-59", new SableyePFL());
            r.RegisterByKey("PFL-62", new SeviperPFL());
            r.RegisterByKey("PFL-63", new DrawAttackMon("Allure", 2));
            r.RegisterByKey("PFL-66", new CardsSTS2.RetaliateMon("Vengeful Fang", 160));
            r.RegisterByKey("PFL-69", new EternatusPFL());
            r.RegisterByKey("PFL-71", new BronzorPFL());
            r.RegisterByKey("PFL-72", new BronzongPFL());
            r.RegisterByKey("PFL-73", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("PFL-74", new DuraludonPFL());
            r.RegisterByKey("PFL-75", new ArchaludonPFL());
            r.RegisterByKey("PFL-76", new FlipUntilTailsTimesMon("Ball Roll"));
            r.RegisterByKey("PFL-77", new CardsBLK.RoundMon());
            r.RegisterByKey("PFL-78", new CardsEVS1.AstonishMon("Astonish"));
            r.RegisterByKey("PFL-79", new AmbipomPFL());
            r.RegisterByKey("PFL-80", new SmearglePFL());
            r.RegisterByKey("PFL-81", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PFL-82", new LinoonePFL());

            r.RegisterByName("Blowtorch", new Blowtorch());
            r.RegisterByName("Dizzying Valley", new DizzyingValley());
            r.RegisterByName("Firebreather", new Firebreather());
            r.RegisterByName("Grimsley's Move", new GrimsleysMove());
            r.RegisterByName("Sacred Charm", new SacredCharm());
        }

        // ================================================================== Shared helpers (also used by MEP)

        /// <summary>"Flip a coin until you get tails. This attack does N damage for each heads."</summary>
        public sealed class FlipUntilTailsTimesMon : Mon
        {
            private readonly string attack;

            public FlipUntilTailsTimesMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>"If your opponent's Active Pokémon already has any damage counters on it, this attack does N more damage."</summary>
        public sealed class DamagedBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public DamagedBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool hurt = ctx.Defending != null && ctx.Defending.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage + (hurt ? bonus : 0));
                return true;
            }
        }

        /// <summary>"This attack does N damage for each of your Pokémon in play."</summary>
        public sealed class PerPokemonInPlayMon : Mon
        {
            private readonly string attack;

            public PerPokemonInPlayMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon().Count);
                return true;
            }
        }

        /// <summary>"If you have any [T] Mega Evolution Pokémon ex in play" (any type when type is null).</summary>
        public static bool HasMegaInPlay(EffectContext ctx, EnergyType? type)
        {
            return ctx.MyPokemon(m => m.Def.IsMegaEx && (type == null || ctx.Engine.IsType(m, type.Value))).Count > 0;
        }

        /// <summary>Discard energy cards chosen from a list of (holder, energy) pairs on your own Pokémon. Returns how many were discarded.</summary>
        public static int DiscardChosenEnergy(EffectContext ctx, List<CardInstance> picks)
        {
            int n = 0;
            foreach (var e in picks)
            {
                var holder = ctx.MyPokemon(m => m.Energy.Contains(e)).FirstOrDefault();
                if (holder != null && ctx.Engine.DiscardEnergy(holder, e)) n++;
            }
            return n;
        }

        /// <summary>Attach a Basic Energy card of a type from your hand to 1 of your Pokémon (Stone Arms, Excited Turbo).</summary>
        public static bool AttachTypeFromHand(EffectContext ctx, EnergyType type, List<PokemonInPlay> targets, string source)
        {
            var energy = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, type)).ToList();
            targets = targets.Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
            if (energy.Count == 0 || targets.Count == 0) return false;
            var card = energy.Select(c => c.Def.Name).Distinct().Count() == 1 ? energy[0]
                : ctx.ChooseCards(energy, 1, 1, source + ": attach which Basic " + type + " Energy from your hand?")[0];
            var target = ctx.ChoosePokemon(targets, source + ": attach " + card.Def.Name + " to which Pokémon?");
            if (target == null || !ctx.Me.Hand.Remove(card)) return false;
            ctx.Engine.AttachEnergyCard(card, target, true);
            return true;
        }

        // ================================================================== Grass

        /// <summary>Disperse Drool: also 20 to each Benched Pokémon, both yours and your opponent's.</summary>
        private sealed class GloomPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disperse Drool") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.Opp.Bench.ToList()) ctx.DamageTarget(m, 20);
                foreach (var m in ctx.Me.Bench.ToList()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Pollen Bomb: Asleep and Poisoned. Lively Flower: +120 if it was healed this turn.</summary>
        private sealed class VileplumePFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pollen Bomb")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                    {
                        ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                        ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    }
                    return true;
                }
                if (atk.Name == "Lively Flower")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.HealedOnTurn == ctx.Turn ? 120 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Juggernaut Horn: + the attack damage it took during the opponent's last turn. Mountain Ramming: discard the top 2 cards of the opponent's deck.</summary>
        private sealed class MegaHeracrossExPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Juggernaut Horn")
                {
                    int taken = me.AttackDamageTurn == ctx.OpponentsLastTurn ? me.AttackDamageThatTurn : 0;
                    ctx.DamageDefending(atk.Damage + taken);
                    return true;
                }
                if (atk.Name == "Mountain Ramming")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Excited Heal: once during your turn, with a Grass Mega Evolution Pokémon ex in play, heal 60 from 1 of your Pokémon.</summary>
        private sealed class LudicoloPFL : Mon
        {
            protected override string Activated { get { return "Excited Heal"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return HasMegaInPlay(ctx, EnergyType.Grass) && CardsMEG.AnyHurt(ctx);
            }

            protected override void Ability(EffectContext ctx) { CardsMEG.HealOneOfMine(ctx, 60, "Excited Heal"); }
        }

        /// <summary>Bug's Cannon: 20 for each Grass Energy on it, to 1 of the opponent's Pokémon.</summary>
        private sealed class GenesectPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bug's Cannon") return false;
                int grass = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                if (grass > 0) ctx.DamageAnyOpponent(20 * grass);
                else Nothing(ctx);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Agile: no Retreat Cost while it has no Energy attached.</summary>
        private sealed class CharmanderPFL : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost;
            }
        }

        /// <summary>Inferno X: discard any amount of Fire Energy from your Pokémon, 90 damage for each.</summary>
        private sealed class MegaCharizardXExPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inferno X") return false;
                var fire = new List<CardInstance>();
                foreach (var m in ctx.MyPokemon())
                    foreach (var e in m.Energy)
                        if (CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fire)) fire.Add(e);
                var picks = fire.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(fire, 0, fire.Count, "Inferno X: discard any amount of Fire Energy from your Pokémon (" + atk.Damage + " damage each)");
                int n = DiscardChosenEnergy(ctx, picks);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Excited Turbo: as often as you like, with a Fire Mega Evolution Pokémon ex in play, attach a Basic Fire Energy from your hand to 1 of your Benched Fire Pokémon.</summary>
        private sealed class OricorioExPFL : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Fire) && ctx.Engine.CanAttachFromHand(m)).ToList();
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Excited Turbo" && ctx.IsMyTurn && HasMegaInPlay(ctx, EnergyType.Fire)
                       && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)) && Targets(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing attaches as many as they like, then presses Done; bots attach one per use.
                if (RepeatableAbility.ForPerson(ctx))
                {
                    RepeatableAbility.AttachFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Fire),
                        () => HasMegaInPlay(ctx, EnergyType.Fire) ? Targets(ctx) : new List<PokemonInPlay>(), "Excited Turbo", "a Basic Fire Energy");
                    return;
                }
                AttachTypeFromHand(ctx, EnergyType.Fire, Targets(ctx), "Excited Turbo");
            }
        }

        /// <summary>Infernal Slash: discard 4 Basic Fire Energy from your hand, or the attack does nothing.</summary>
        private sealed class CeruledgePFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Infernal Slash") return false;
                if (CardsDRI1.DiscardEnergyFromHand(ctx, EnergyType.Fire, 4, atk.Name)) ctx.DamageDefending(atk.Damage);
                else Nothing(ctx);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Thick Fat: takes 30 less from the opponent's Fire and Water Pokémon. Slam: 70 per heads on 2 coins.</summary>
        private sealed class DewgongPFL : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || !info.AgainstOpponent) return damage;
                bool hot = src.Engine.IsType(info.Attacker, EnergyType.Fire) || src.Engine.IsType(info.Attacker, EnergyType.Water);
                return hot ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slam") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(2));
                return true;
            }
        }

        /// <summary>Wreck: +120 if a Stadium is in play, then discard it. Blizzard Edge: discard 2 Energy.</summary>
        private sealed class MamoswinePFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Wreck")
                {
                    bool stadium = ctx.State.Stadium != null;
                    ctx.DamageDefending(atk.Damage + (stadium ? 120 : 0));
                    if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                    return true;
                }
                if (atk.Name == "Blizzard Edge")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Multi Adapter: each of your Pokémon with "Rotom" in its name (Rotom ex too) can have up to 2 Pokémon Tools. Two
        /// copies don't make it 3. When it stops working, the engine has the owner discard down to 1 (EnforceToolLimits).
        /// Thunderbolt: discard all Energy from this Pokémon.
        /// </summary>
        public sealed class RotomExPFL : Mon
        {
            public override int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max)
            {
                if (mon == null || mon.Owner != src.Player || mon.Name.IndexOf("Rotom", StringComparison.Ordinal) < 0) return max;
                return Math.Max(max, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderbolt") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Swirling Prose: while Active, the opponent's new Active Pokémon is Confused whenever their Active moves to the Bench during their turn. Hexa-Magic: may draw until 6.</summary>
        private sealed class MismagiusExPFL : Mon
        {
            public override void OnOpponentActiveMovedToBench(EffectContext src, int player, PokemonInPlay newActive)
            {
                if (player == src.Player || newActive == null || src.Opp.Active != newActive || !src.IsActive(src.Pokemon)) return;
                if (src.ApplyCondition(newActive, SpecialCondition.Confused))
                    src.Log("Swirling Prose (" + src.Pokemon.Name + ") confused " + newActive.Name + ".");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hexa-Magic") return false;
                ctx.DamageDefending(atk.Damage);
                CardsMEG.DrawUntilSixMon.DrawUntilSix(ctx, atk.Name);
                return true;
            }
        }

        /// <summary>Soothing Melody: heal 120 from 1 of your Benched Psychic Pokémon.</summary>
        private sealed class MeloettaPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Soothing Melody") return false;
                Func<PokemonInPlay, bool> benchedPsychic = m => ctx.Me.Bench.Contains(m) && ctx.Engine.IsType(m, EnergyType.Psychic);
                if (!CardsMEG.HealOneOfMine(ctx, 120, atk.Name, benchedPsychic)) Nothing(ctx);
                return true;
            }
        }

        /// <summary>Diamond Coat: takes 30 less damage from attacks. Garland Ray: discard up to 2 Energy from it, 120 each.</summary>
        private sealed class MegaDiancieExPFL : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Garland Ray") return false;
                var me = ctx.Pokemon;
                var energy = me.Energy.ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Garland Ray: discard up to 2 Energy from " + me.Name + " (" + atk.Damage + " damage each)");
                int n = 0;
                foreach (var e in picks) if (ctx.Engine.DiscardEnergy(me, e)) n++;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Sneaky Placement: 1 damage counter on 1 of the opponent's Pokémon.</summary>
        private sealed class BramblinPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sneaky Placement") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Sneaky Placement: put a damage counter on which Pokémon?");
                if (target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, 1);
                return true;
            }
        }

        /// <summary>Prison Panic: when it evolves from the hand, the opponent's Active Pokémon becomes Confused.</summary>
        private sealed class BrambleghastPFL : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var d = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Prison Panic", d != null, "Prison Panic: make your opponent's Active Pokémon Confused?")) return;
                if (ctx.CanAffect(d)) ctx.ApplyCondition(d, SpecialCondition.Confused);
            }
        }

        // ================================================================== Fighting

        /// <summary>Raging Charge: 40 for each of your damaged Tauros. Double-Edge: 20 to itself.</summary>
        private sealed class PaldeanTaurosPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Raging Charge")
                {
                    int n = ctx.MyPokemon(m => m.Name.Contains("Tauros") && m.DamageCounters > 0).Count;
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                if (atk.Name == "Double-Edge")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(20);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Poison Ring: Poisoned, and it can't retreat next turn.</summary>
        private sealed class GliscorPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Ring") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Sandy Flapping: when it evolves from the hand, or when it's Knocked Out in the Active Spot by an opponent's attack, discard the top 2 cards of the opponent's deck.</summary>
        private sealed class FlygonPFL : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Sandy Flapping", ctx.Opp.Deck.Count > 0, "Sandy Flapping: discard the top 2 cards of your opponent's deck?")) return;
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
            }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Opp.Deck.Count == 0) return;
                if (!src.AskYesNo("Sandy Flapping: discard the top 2 cards of your opponent's deck?")) return;
                Announce(src, "Sandy Flapping");
                src.Engine.DiscardTopOfDeck(src.Opponent, 2);
            }
        }

        // ================================================================== Darkness

        /// <summary>Shadowy Concealment: your Darkness Pokémon Knocked Out by an opponent's Pokémon ex attack give up 1 fewer Prize (doesn't stack). Void Gale: move an Energy to the Bench.</summary>
        private sealed class MegaGengarExPFL : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut.Owner != src.Player || taker == src.Player || prizes <= 0) return prizes;
                if (!src.Engine.IsType(knockedOut, EnergyType.Darkness)) return prizes;
                bool byEx = knockedOut.HitByAttack && !knockedOut.ForcedKnockOut && knockedOut.HitByAttackerDef != null
                            && (knockedOut.HitByAttackerDef.IsEx || knockedOut.HitByAttackerDef.IsMegaEx);
                if (!byEx) return prizes;
                // Doesn't stack: only the first Pokémon with a working Shadowy Concealment applies it.
                var first = src.MyPokemon(m => src.Engine.HasAbilityNamed(m, "Shadowy Concealment")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != null && first != src.Pokemon) return prizes;
                return prizes - 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Void Gale") return false;
                ctx.DamageDefending(atk.Damage);
                CardsDRI1.MoveEnergyToBench(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>Cocky Claw: +70 with a Stage 2 Darkness Pokémon on your Bench.</summary>
        private sealed class SableyePFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cocky Claw") return false;
                bool stage2 = ctx.Me.Bench.Any(m => m.Def.Stage == Stage.Stage2 && ctx.Engine.IsType(m, EnergyType.Darkness));
                ctx.DamageDefending(atk.Damage + (stage2 ? 70 : 0));
                return true;
            }
        }

        /// <summary>Excited Power: with a Darkness Mega Evolution Pokémon ex in play, its attacks do 120 more to the opponent's Active Pokémon.</summary>
        private sealed class SeviperPFL : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive) return damage;
                return HasMegaInPlay(src, EnergyType.Darkness) ? damage + 120 : damage;
            }
        }

        /// <summary>Shatter: discard a Stadium in play. Power Rush: flip; tails, it can't attack next turn.</summary>
        private sealed class EternatusPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Shatter")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                    return true;
                }
                if (atk.Name == "Power Rush")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        /// <summary>Iron Defense: flip; heads, prevent all damage done to it by attacks next turn.</summary>
        private sealed class BronzorPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Defense") return false;
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (Iron Defense)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Triple Draw: draw 3. Tool Drop: 40 for each Pokémon Tool attached to all Pokémon.</summary>
        private sealed class BronzongPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Triple Draw")
                {
                    ctx.Draw(3);
                    return true;
                }
                if (atk.Name == "Tool Drop")
                {
                    int tools = ctx.MyPokemon().Sum(m => m.Tools.Count) + ctx.OpponentPokemon().Sum(m => m.Tools.Count);
                    ctx.DamageDefending(atk.Damage * tools);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hyper Beam: discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class DuraludonPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Hyper Beam: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Coated Attack: no damage from Basic Pokémon's attacks next turn.</summary>
        private sealed class ArchaludonPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coated Attack") return false;
                ctx.DamageDefending(atk.Damage);
                CardsMEG.ShieldFromBasics(ctx, atk.Name);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Dual Tail: discard 2 Energy from it, then 60 to each of 2 of the opponent's Pokémon.</summary>
        private sealed class AmbipomPFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Tail") return false;
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                CardsDRI1.DamageTwoOpponents(ctx, 60, atk.Name);
                return true;
            }
        }

        /// <summary>Energizing Sketch: flip 3; attach that many Basic Energy from your discard pile to your Benched Pokémon.</summary>
        private sealed class SmearglePFL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energizing Sketch") return false;
                int heads = ctx.FlipCoins(3);
                var pile = ctx.BasicEnergyInDiscard();
                if (heads == 0 || pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                ctx.AttachCardsAnyWay(pile, heads, (c, m) => ctx.Me.Bench.Contains(m), "Energizing Sketch: attach Basic Energy to your Benched Pokémon");
                return true;
            }
        }

        /// <summary>Excited Dash: once during your turn, from the Bench with a Mega Evolution Pokémon ex in play, switch with your Active Pokémon.</summary>
        private sealed class LinoonePFL : Mon
        {
            protected override string Activated { get { return "Excited Dash"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Active != null && HasMegaInPlay(ctx, null);
            }

            protected override void Ability(EffectContext ctx) { ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon); }
        }

        // ================================================================== Trainers

        /// <summary>Discard a Basic Fire Energy from your hand to discard a Tool or Special Energy from an opponent's Pokémon, or a Stadium in play.</summary>
        private sealed class Blowtorch : CardScript
        {
            private static List<PokemonInPlay> WithTool(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Tool != null && ctx.CanAffect(m)); }
            private static List<PokemonInPlay> WithSpecial(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m)); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                if (!ctx.Me.Hand.Any(c => c != ctx.Card && CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire))) return false;
                return ctx.State.Stadium != null || WithTool(ctx).Count > 0 || WithSpecial(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => c != ctx.Card && CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return;
                var cost = ctx.ChooseCards(fire, 1, 1, "Blowtorch: discard a Basic Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, cost);

                var labels = new List<string>();
                var tools = WithTool(ctx);
                var specials = WithSpecial(ctx);
                if (tools.Count > 0) labels.Add("Discard a Pokémon Tool");
                if (specials.Count > 0) labels.Add("Discard a Special Energy");
                if (ctx.State.Stadium != null) labels.Add("Discard " + ctx.State.Stadium.Def.Name);
                if (labels.Count == 0) return;
                int pick = labels.Count == 1 ? 0 : ctx.ChooseOption("Blowtorch: choose what to discard", labels.ToArray());
                string choice = labels[pick];
                if (choice == "Discard a Pokémon Tool")
                {
                    var target = ctx.ChoosePokemon(tools, "Blowtorch: discard the Tool from which Pokémon?");
                    if (target != null) ctx.Engine.DiscardTool(target);
                }
                else if (choice == "Discard a Special Energy")
                {
                    var target = ctx.ChoosePokemon(specials, "Blowtorch: discard a Special Energy from which Pokémon?");
                    if (target == null) return;
                    var energy = ctx.ChooseCards(target.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList(), 1, 1, "Blowtorch: choose a Special Energy to discard")[0];
                    ctx.Engine.DiscardEnergy(target, energy);
                }
                else ctx.Engine.DiscardStadium();
            }
        }

        /// <summary>Confused Pokémon stay Confused when they evolve or devolve.</summary>
        private sealed class DizzyingValley : CardScript
        {
            public override bool KeepsConfusionOnEvolve(EffectContext src) { return true; }
        }

        /// <summary>Search your deck for up to 7 Basic Fire Energy cards and put them into your hand.</summary>
        private sealed class Firebreather : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 7, "Firebreather: choose up to 7 Basic Fire Energy cards");
            }
        }

        /// <summary>Not on your first turn. Look at the top 7 cards: bench a Darkness Pokémon from them (Evolution Pokémon too, your ruling), the rest go to the bottom in random order.</summary>
        private sealed class GrimsleysMove : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return !ctx.IsMyFirstTurn && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var dark = top.Where(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Darkness)).ToList();
                if (dark.Count > 0 && ctx.BenchSpace > 0)
                {
                    var picks = ctx.ChooseCards(dark, 0, 1, "Grimsley's Move: put a Darkness Pokémon onto your Bench");
                    foreach (var c in picks)
                    {
                        ctx.Me.Deck.Remove(c);
                        top.Remove(c);
                        ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    }
                }
                foreach (var c in top) ctx.Me.Deck.Remove(c);
                ctx.State.Rng.Shuffle(top);
                ctx.Me.Deck.AddRange(top);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + top.Count + (top.Count == 1 ? " card" : " cards") + " on the bottom of their deck.");
            }
        }

        /// <summary>Tool: the holder takes 30 less damage from attacks by the opponent's Pokémon that have an Ability.</summary>
        private sealed class SacredCharm : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent) return damage;
                return src.Engine.HasAbility(info.Attacker) ? damage - 30 : damage;
            }
        }
    }
}
