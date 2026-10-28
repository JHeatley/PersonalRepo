using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Expedition Base Set Trainers (E1 139-142, 144-150; v144). Bill's Maintenance, Copycat, Master Ball, Super Scoop Up, Warp
    /// Point, Energy Search, Full Heal, Moo-Moo Milk, Switch and the Energy cards are prints of cards already in the pool; Potion is
    /// Base Set's (like Legendary Collection's, your ruling there). Compendium: Pokémon Reversal's and Strength Charm's errata,
    /// Professor Elm's Training Method can't be played with an empty deck, and LEGEND halves aren't Basic Pokémon for Dual Ball.
    /// Multi Technical Machine 01 is attached like a Pokémon Tool but isn't one: it doesn't take the Tool's place (Compendium).
    /// </summary>
    public static class CardsE1T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E1-139", new DualBall());
            r.RegisterByKey("E1-140", new EnergyRemoval2());
            r.RegisterByKey("E1-141", new EnergyRestore());
            r.RegisterByKey("E1-142", new MarysImpulse());
            r.RegisterByKey("E1-144", new MultiTechnicalMachine01());
            r.RegisterByKey("E1-145", new PokemonNurse());
            r.RegisterByKey("E1-146", new PokemonReversal());
            r.RegisterByKey("E1-147", new PowerCharge());
            r.RegisterByKey("E1-148", new ProfessorElmsTrainingMethod());
            r.RegisterByKey("E1-149", new ProfessorOaksResearch());
            r.RegisterByKey("E1-150", new StrengthCharm());
        }

        /// <summary>Dual Ball: 2 coins; for each heads, a Basic Pokémon (not a Baby Pokémon or a LEGEND half) from your deck into your hand.</summary>
        private sealed class DualBall : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 0 || ctx.Me.Deck.Count == 0) return;
                ctx.SearchToHand(d => d.IsBasicPokemon && !d.IsBaby && !d.IsLegend, heads, "Dual Ball: choose up to " + heads + " Basic Pokémon");
            }
        }

        /// <summary>Energy Removal 2: heads, discard an Energy card attached to 1 of the opponent's Pokémon.</summary>
        private sealed class EnergyRemoval2 : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var theirs = ctx.OpponentPokemon(m => m.Energy.Count > 0);
                if (theirs.Count == 0) return;
                var target = theirs.Count == 1 ? theirs[0] : ctx.ChoosePokemon(theirs, "Energy Removal 2: choose 1 of your opponent's Pokémon");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Energy Removal 2: discard which Energy card from " + target.Name + "?");
            }
        }

        /// <summary>Energy Restore: 3 coins; a basic Energy card from your discard pile into your hand for each heads.</summary>
        private sealed class EnergyRestore : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(3);
                var energy = ctx.BasicEnergyInDiscard();
                if (heads == 0 || energy.Count == 0) return;
                int n = Math.Min(heads, energy.Count);
                ctx.DiscardToHand(energy.Count == n ? energy : ctx.ChooseCards(energy, n, n, "Energy Restore: put " + n + " basic Energy card" + (n == 1 ? "" : "s") + " into your hand"));
            }
        }

        /// <summary>Mary's Impulse (Supporter): flip until tails; draw 2 cards for each heads.</summary>
        private sealed class MarysImpulse : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Draw(2 * heads);
            }
        }

        /// <summary>
        /// Multi Technical Machine 01: the Pokémon it's attached to may use its attack, Paralyzing Gaze ([C]: the Defending Pokémon is
        /// now Paralyzed); discarded at the end of your turn.
        /// </summary>
        private sealed class MultiTechnicalMachine01 : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override bool DiscardedAtEndOfOwnersTurn { get { return true; } }

            /// <summary>A Technical Machine, not a Pokémon Tool: it can share a Pokémon with a Tool (and another TM).</summary>
            public override bool IsAttachedNonTool { get { return true; } }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
            }
        }

        /// <summary>Pokémon Nurse (Supporter): remove all damage counters from 1 of your Pokémon, then discard all Energy cards attached to it.</summary>
        private sealed class PokemonNurse : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Pokémon Nurse: remove all damage counters from which Pokémon?");
                if (target == null) return;
                ctx.Heal(target, target.DamageCounters * 10);
                ctx.Engine.DiscardAllEnergy(target);
            }
        }

        /// <summary>Pokémon Reversal (Compendium erratum): choose 1 of the opponent's Benched Pokémon, then flip; heads, it switches with their Active Pokémon.</summary>
        private sealed class PokemonReversal : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.GustTargets();
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Pokémon Reversal: choose 1 of your opponent's Benched Pokémon");
                if (target == null || !ctx.Flip()) return;
                if (ctx.Opp.Bench.Contains(target)) ctx.Engine.SwitchActive(ctx.Opponent, target);
            }
        }

        /// <summary>Power Charge: heads, shuffle 2 Energy cards from your discard pile into your deck (1 if only 1).</summary>
        private sealed class PowerCharge : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return;
                int n = Math.Min(2, energy.Count);
                var picks = energy.Count == n ? energy : ctx.ChooseCards(energy, n, n, "Power Charge: shuffle which 2 Energy cards into your deck?");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Professor Elm's Training Method (Supporter): an Evolution card from your deck into your hand (not with an empty deck: Compendium).</summary>
        private sealed class ProfessorElmsTrainingMethod : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsEvolution, 1, "Professor Elm's Training Method: choose an Evolution card");
            }
        }

        /// <summary>Professor Oak's Research (Supporter): shuffle your hand into your deck, then draw 5 cards.</summary>
        private sealed class ProfessorOaksResearch : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
            }
        }

        /// <summary>
        /// Strength Charm (Pokémon Tool, Compendium erratum): its holder's attacks do 10 more damage to the Active Pokémon (before
        /// Weakness and Resistance); it's discarded at the end of the turn in which its holder attacked.
        /// </summary>
        private sealed class StrengthCharm : CardScript
        {
            private const string Marker = "Strength Charm attacked";

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !info.ApplyOnce("Strength Charm")) return damage;
                return damage + 10;
            }

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                if (src.Pokemon != null && attacker == src.Pokemon) src.Pokemon.Markers[Marker] = src.Turn;
            }

            public override void OnEndOfTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                int turn;
                if (holder == null || !holder.Markers.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                holder.Markers.Remove(Marker);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }
    }
}
