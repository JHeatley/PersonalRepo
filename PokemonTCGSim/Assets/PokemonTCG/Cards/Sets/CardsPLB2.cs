using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Plasma Blast (PLB, v109): Fighting to Colorless Pokémon (47-104) and the Trainers. Caitlin, Energy Retrieval, Pokémon Catcher,
    /// Rare Candy, Ultra Ball, Master Ball and Scoop Up Cyclone were already in the pool; Plasma Energy just provides Colorless Energy.
    /// </summary>
    public static class CardsPLB2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PLB-48", new CardsFST2.ConditionBonusMon("Last-Chance Chop", 70, c => c.RemainingHp(c.Pokemon) == 10));
            r.RegisterByKey("PLB-49", new MachampDiscipline());
            r.RegisterByKey("PLB-50", new ComboMon(new CardsASC.RandomDiscardMon("Knock Off"), new CardsFST2.ConditionBonusMon("Reinforced Lariat", 40, c => c.Pokemon.Tools.Count > 0)));
            r.RegisterByKey("PLB-51", new ComboMon(new CoinsMon("Freestyle Strike", 2), new ShoulderThrowMon()));
            r.RegisterByKey("PLB-52", new CardsFST2.ConditionBonusMon("Kick of Righteousness", 40, c => CardsPLB1.IsPlasma(c, c.Defending)));
            r.RegisterByKey("PLB-53", new CardsPLB1.PrehistoricCallMon(null));
            r.RegisterByKey("PLB-54", new ComboMon(new CardsWHT.FlipMoreMon("Acrobatics", 2, 20), new SwiftDiveMon()));
            r.RegisterByKey("PLB-55", new ComboMon(new RoarMon(), new FlipBonusMon("Ambush", 10)));
            r.RegisterByKey("PLB-56", new ComboMon(new NoRetreatMon("Dark Clamp"), new BlazingClawsMon()));
            r.RegisterByKey("PLB-57", new CardsASR1.TimesMon("Iron Head", c => c.FlipUntilTails()));
            r.RegisterByKey("PLB-58", new CardsASR1.TimesMon("Iron Head", c => c.FlipUntilTails()));
            r.RegisterByKey("PLB-59", new ComboMon(new KnockBackMon(), new CardsLTR2.GaiaHammerMon("Aura of the Land", 20, 20)));
            r.RegisterByKey("PLB-60", new JirachiStellarGuidance());
            r.RegisterByKey("PLB-61", new ComboMon(new CardsMEG.SnipeBenchMon("Steamroll", 20), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("PLB-63", new FlipBonusMon("Knock Away", 20));
            r.RegisterByKey("PLB-64", new SalamenceBreakwing());
            r.RegisterByKey("PLB-65", new ComboMon(new ReverseEdgeMon(), new FastForwardMon()));
            r.RegisterByKey("PLB-66", new ComboMon(new SwitchSelfMon("Strafe", true), new DimensionHealMon()));
            r.RegisterByKey("PLB-67", new StrongBondMon());
            r.RegisterByKey("PLB-68", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("PLB-69", new ComboMon(new CardsASR1.TimesMon("Dragonaxe", c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Metal)), new StrikeOfTheChampionMon()));
            r.RegisterByKey("PLB-70", new ComboMon(new BigSwingMon("Big Swing"), new CardsPAF2.IgnoreDefenderEffectsMon("Shred")));
            r.RegisterByKey("PLB-71", new ComboMon(new CallForFamilyMon(), new CoinsMon("Comet Punch", 4)));
            r.RegisterByKey("PLB-73", new CardsASC.FlipDiscardDefendingEnergyMon("Destructive Beam"));
            r.RegisterByKey("PLB-74", new PorygonZPlasmaTransfer());
            r.RegisterByKey("PLB-75", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("PLB-76", new AdrenalashMon());
            r.RegisterByKey("PLB-77", new ComboMon(new MisinformationMon(), new ConditionAttackMon("Tone-Deaf", SpecialCondition.Confused)));
            r.RegisterByKey("PLB-102", new PropagationMon(null));
            r.RegisterByKey("PLB-104", new DusknoirSinisterHand());

            r.RegisterByName("Cover Fossil", new CardsSTS3.FossilItem("Tirtouga"));
            r.RegisterByName("Plume Fossil", new CardsSTS3.FossilItem("Archen"));
            r.RegisterByName("Root Fossil Lileep", new CardsSTS3.FossilItem("Lileep"));
            r.RegisterByName("Iris", new Iris());
            r.RegisterByName("Professor Juniper", new ProfessorJuniper());
            r.RegisterByName("Reversal Trigger", new ReversalTrigger());
            r.RegisterByName("Silver Bangle", new SilverBangle());
            r.RegisterByName("Silver Mirror", new SilverMirror());
            r.RegisterByName("G Booster", new GBooster());
            r.RegisterByName("G Scope", new GScope());
        }

        // ================================================================== Fighting

        /// <summary>Badge of Discipline: your Fighting Pokémon's attack damage isn't affected by Resistance. Close Combat: tails, it takes 30 more next turn.</summary>
        private sealed class MachampDiscipline : Mon
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                var a = info.Attacker;
                return src.Pokemon != null && a != null && a.Owner == src.Player && src.Engine.IsType(a, EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Close Combat") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new CardsUNM3.TakeMoreEffect(me.Name + " takes 30 more damage from attacks (Close Combat)", ctx.Player, me.Uid, ctx.Turn + 1, 30));
                return true;
            }
        }

        /// <summary>Shoulder Throw: 80 minus 20 for each Colorless in the Defending Pokémon's Retreat Cost.</summary>
        private sealed class ShoulderThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shoulder Throw") return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                int dmg = Math.Max(0, atk.Damage - 20 * cost);
                if (dmg > 0) ctx.DamageDefending(dmg);
                return true;
            }
        }

        /// <summary>Swift Dive: the base damage is 50 if this Pokémon has 50 HP or less left.</summary>
        private sealed class SwiftDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swift Dive") return false;
                ctx.DamageDefending(ctx.RemainingHp(ctx.Pokemon) <= 50 ? 50 : atk.Damage);
                return true;
            }
        }

        // ================================================================== Darkness, Metal

        /// <summary>Roar: your opponent switches the Defending Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class RoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roar") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null && ctx.CanAffect(ctx.Defending)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Blazing Claws: against a Team Plasma Pokémon, 60 more damage and it is now Burned.</summary>
        private sealed class BlazingClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blazing Claws") return false;
                var defending = ctx.Defending;
                bool plasma = CardsPLB1.IsPlasma(ctx, defending);
                ctx.DamageDefending(atk.Damage + (plasma ? 60 : 0));
                if (plasma && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Knock Back: 50, then your opponent switches the Defending Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class KnockBackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Back") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Stellar Guidance: played from your hand onto your Bench, you may search your deck for a Supporter. Hypnostrike: both Active Pokémon fall Asleep.</summary>
        private sealed class JirachiStellarGuidance : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool ready = ctx.Me.Deck.Count > 0;
                if (!TriggerOnce(ctx, "Stellar Guidance", ready, "Stellar Guidance: search your deck for a Supporter card?")) return;
                ctx.SearchToHand(d => d.IsSupporter, 1, "Stellar Guidance: choose a Supporter card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypnostrike") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Breakwing: played from your hand to evolve, you may discard all Pokémon Tools from each of the opponent's Pokémon. Gaia Crush: discard the Stadium.</summary>
        private sealed class SalamenceBreakwing : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.OpponentPokemon(m => m.Tools.Count > 0).Count > 0;
                if (!TriggerOnce(ctx, "Breakwing", ready, "Breakwing: discard all Pokémon Tools attached to each of your opponent's Pokémon?")) return;
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0)) ctx.Engine.DiscardAllTools(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gaia Crush") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Reverse Edge: flip a coin; heads, put a card from your discard pile into your hand.</summary>
        private sealed class ReverseEdgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reverse Edge") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                var pick = pile.Count == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, "Reverse Edge: put which card from your discard pile into your hand?")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
                return true;
            }
        }

        /// <summary>Fast Forward: discard the top card of the opponent's deck for each Plasma Energy attached to this Pokémon.</summary>
        private sealed class FastForwardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fast Forward") return false;
                ctx.DamageDefending(atk.Damage);
                int n = CardsPLB1.PlasmaEnergy(ctx.Pokemon);
                if (n > 0 && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }

        /// <summary>Dimension Heal: heal 20 damage from this Pokémon for each Plasma Energy attached to it.</summary>
        private sealed class DimensionHealMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dimension Heal") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                int n = CardsPLB1.PlasmaEnergy(me);
                if (n > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20 * n);
                return true;
            }
        }

        /// <summary>Strong Bond: search your deck for a Supporter named Iris and put it into your hand.</summary>
        private sealed class StrongBondMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong Bond") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsSupporter && d.Name == "Iris", 1, "Strong Bond: choose an Iris");
                return true;
            }
        }

        /// <summary>Strike of the Champion: a Team Plasma Defending Pokémon is Knocked Out; otherwise the attack does nothing.</summary>
        private sealed class StrikeOfTheChampionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strike of the Champion") return false;
                var defending = ctx.Defending;
                if (!CardsPLB1.IsPlasma(ctx, defending))
                {
                    ctx.Log("Strike of the Champion: the Defending Pokémon isn't a Team Plasma Pokémon, so the attack does nothing.");
                    return true;
                }
                ctx.Engine.KnockOut(defending, "Strike of the Champion");
                return true;
            }
        }

        /// <summary>Big Swing: flip 2 coins; if either is tails, the attack does nothing. Used for Big Swing, Super Slice (the attack name is passed in).</summary>
        internal sealed class BigSwingMon : Mon
        {
            private readonly string attack;

            public BigSwingMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.FlipCoins(2) == 2) ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Plasma Transfer: as often as you like during your turn, move a Plasma Energy from 1 of your Pokémon to another. Tri Attack: 3 coins.</summary>
        private sealed class PorygonZPlasmaTransfer : Mon
        {
            private const string Transfer = "Plasma Transfer";
            private static readonly Mon Tri = new CoinsMon("Tri Attack", 3);

            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => CardsPLB1.PlasmaEnergy(m) > 0); }

            private static bool CanMove(EffectContext ctx) { return ctx.MyPokemon().Count > 1 && Holders(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Transfer && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone =>
                {
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Transfer + ": move a Plasma Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var card = giver.Energy.FirstOrDefault(e => e.Def.Name == "Plasma Energy");
                    if (card == null) return false;
                    var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != giver), Transfer + ": move it from " + giver.Name + " to which Pokémon?", offerDone);
                    return to != null && ctx.Engine.MoveEnergy(giver, to, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tri.HandleAttack(ctx, atk); }
        }

        /// <summary>Adrenalash: 50, and during your next turn each of this Pokémon's attacks does 50 more damage (before W/R).</summary>
        private sealed class AdrenalashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Adrenalash") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var boost = new CardsROS2.AllAttacksBoostEffect(me.Name + "'s attacks do 50 more damage during its next turn (Adrenalash)", ctx.Player, me.Uid, ctx.Turn + 2, 50);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        /// <summary>Misinformation: discard all Pokémon Tools attached to each of the opponent's Pokémon.</summary>
        private sealed class MisinformationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Misinformation") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m))) ctx.Engine.DiscardAllTools(mon);
                return true;
            }
        }

        /// <summary>Propagation: during your turn, if this Pokémon is in your discard pile, you may put it into your hand; usable again whenever it is back there (Compendium, 2013-05-16).</summary>
        internal sealed class PropagationMon : Mon
        {
            private readonly Mon attacks;

            public PropagationMon(Mon attacks) { this.attacks = attacks; }

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && !ctx.Engine.DiscardToHandBlocked(ctx.Player);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                ctx.Me.Hand.Add(ctx.Card);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s " + ctx.Card.Def.Name + " used its Ability Propagation: it went back into the hand.", ctx.Card.Id, -1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Sinister Hand: as often as you like during your turn, move 1 damage counter from 1 of the opponent's Pokémon to another of theirs. Shadow Punch: no Resistance.</summary>
        private sealed class DusknoirSinisterHand : Mon
        {
            private const string Hand = "Sinister Hand";
            private static readonly Mon Punch = new CardsFLF1.NoResistanceMon("Shadow Punch");

            private static bool CanMove(EffectContext ctx)
            {
                var opp = ctx.OpponentPokemon();
                return opp.Count >= 2 && opp.Any(m => m.DamageCounters > 0) && !ctx.Engine.CounterMovementBlocked();
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Hand && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone =>
                {
                    var origin = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.OpponentPokemon(m => m.DamageCounters > 0), Hand + ": move a damage counter from which of your opponent's Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (origin == null) return false;
                    var dest = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.OpponentPokemon(m => m != origin), Hand + ": move it to which of your opponent's Pokémon?", offerDone);
                    return dest != null && ctx.Engine.MoveDamageCounters(origin, dest, 1, Hand) > 0;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        // ================================================================== Trainers

        /// <summary>Iris: this turn, your Pokémon's attacks do 10 more damage to the Active Pokémon for each Prize card your opponent has taken (before W/R).</summary>
        private sealed class Iris : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int taken = ctx.OpponentPrizesTaken;
                if (taken <= 0)
                {
                    ctx.Log("Iris: your opponent hasn't taken any Prize cards, so your attacks do no more damage.");
                    return;
                }
                ctx.AddEffect(new IrisEffect("Your attacks do " + (10 * taken) + " more damage to the Active Pokémon this turn (Iris)", ctx.Player, ctx.Turn, 10 * taken));
            }
        }

        internal sealed class IrisEffect : ContinuousEffect
        {
            private readonly int bonus;

            public IrisEffect(string name, int owner, int expiresAfterTurn, int bonus) : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.bonus = bonus;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || target == null || attacker.Owner != Owner || target.Owner == Owner || damage <= 0 || !engine.State.IsActive(target)) return damage;
                return damage + bonus;
            }
        }

        /// <summary>Professor Juniper: discard your hand and draw 7 cards (not playable with an empty deck: Compendium, 2014-11-13).</summary>
        private sealed class ProfessorJuniper : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(7);
            }
        }

        /// <summary>Reversal Trigger: when the Team Plasma Pokémon it's attached to is Knocked Out by damage from an opponent's attack, search your deck for a card.</summary>
        private sealed class ReversalTrigger : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var mon = src.Pokemon;
                if (mon == null || !byOpponentsAttack || !src.Engine.IsTeamPlasma(mon) || src.Me.Deck.Count == 0) return;
                src.SearchToHand(d => true, 1, "Reversal Trigger: choose a card to put into your hand");
            }
        }

        /// <summary>Silver Bangle: the holder's attacks (not a Pokémon-EX's) do 30 more damage to the opponent's Active Pokémon-EX (before W/R).</summary>
        private sealed class SilverBangle : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || holder.Def.IsPokemonEX || !info.ToOpponentsActive || damage <= 0) return damage;
                return info.Target != null && info.Target.Def.IsPokemonEX ? damage + 30 : damage;
            }
        }

        /// <summary>Silver Mirror: prevents all effects of attacks, damage included, done to the holder (not a Pokémon-EX) by the opponent's Team Plasma Pokémon.</summary>
        private sealed class SilverMirror : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                var a = info.Attacker;
                if (holder == null || info.Target != holder || holder.Def.IsPokemonEX || a == null || a.Owner == holder.Owner || !src.Engine.IsTeamPlasma(a)) return damage;
                return 0;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var holder = src.Pokemon;
                if (holder == null || target != holder || holder.Def.IsPokemonEX || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != holder.Owner && src.Engine.IsTeamPlasma(attacker);
            }
        }

        /// <summary>G Booster / G Scope (ACE SPEC Pokémon Tools): the Genesect-EX they're attached to can also use the card's attack.</summary>
        private abstract class GenesectTool : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Name != "Genesect-EX" || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex) { Use(ctx, ctx.Attack(attackIndex)); }

            protected abstract void Use(EffectContext ctx, AttackDef atk);
        }

        /// <summary>G Booster: 200, not affected by effects on the Defending Pokémon; discard 2 Energy from the Genesect-EX.</summary>
        private sealed class GBooster : GenesectTool
        {
            protected override void Use(EffectContext ctx, AttackDef atk)
            {
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(me, Math.Min(2, me.Energy.Count), "G Booster: discard 2 Energy from " + me.Name);
            }
        }

        /// <summary>G Scope: 100 damage to 1 of the opponent's Benched Pokémon (no Weakness or Resistance).</summary>
        private sealed class GScope : GenesectTool
        {
            protected override void Use(EffectContext ctx, AttackDef atk)
            {
                ctx.DamageOneBenched(100);
            }
        }
    }
}
