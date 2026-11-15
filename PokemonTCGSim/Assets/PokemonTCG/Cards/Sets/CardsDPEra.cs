using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// v172: helpers the Diamond &amp; Pearl era sets share (DP, MT, SW, GE, MD, LA, SF, POP 6 and 8). Compendium meta-rulings: a
    /// Diamond &amp; Pearl or later card that says "Trainer" means only Trainer cards that aren't Supporters or Stadiums.
    /// </summary>
    public static class CardsDPEra
    {
        /// <summary>A "Trainer card" in Diamond &amp; Pearl wording: not a Supporter or a Stadium (Compendium, 2007-05-03).</summary>
        internal static bool DpTrainer(CardDef d) { return d != null && d.IsTrainer && !d.IsSupporter && !d.IsStadium; }

        /// <summary>"If [name] is anywhere under this Pokémon" / "if this Pokémon is evolved from [name]".</summary>
        internal static bool EvolvedFrom(EffectContext ctx, string name)
        {
            var me = ctx.Pokemon;
            if (me == null || me.Stack.Count < 2) return false;
            for (int i = 0; i < me.Stack.Count - 1; i++)
                if (me.Stack[i].Def.Name == name) return true;
            return false;
        }

        /// <summary>"Flip a coin. If heads, [the other script's effect]" after the printed damage; tails, only the damage.</summary>
        internal sealed class FlipGateMon : Mon
        {
            private readonly string attack;
            private readonly Mon onHeads;

            public FlipGateMon(string attack, Mon onHeads)
            {
                this.attack = attack;
                this.onHeads = onHeads;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Flip()) return onHeads.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>"Choose 1 card from your opponent's hand without looking. Look at it, then they shuffle it into their deck" (Astonish).</summary>
        internal sealed class AstonishMon : Mon
        {
            private readonly string attack;

            public AstonishMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { card }, attack, attack + ": the card you chose", ctx.Source);
                if (!hand.Remove(card)) return true;
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(attack + ": " + card.Def.Name + " was shuffled into " + ctx.Engine.Possessive(ctx.Opponent) + " deck.");
                return true;
            }
        }

        /// <summary>Harden: during the opponent's next turn, an attack's damage to this Pokémon of N or less (after W/R) is prevented.</summary>
        internal sealed class HardenMon : Mon
        {
            private readonly string attack;
            private readonly int max;

            public HardenMon(string attack, int max)
            {
                this.attack = attack;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    ctx.AddEffect(new HardenEffect(me.Name + " takes no attack damage of " + max + " or less during the opponent's next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1, max));
                return true;
            }
        }

        private sealed class HardenEffect : ContinuousEffect
        {
            private readonly int max;

            public HardenEffect(string name, int owner, int targetUid, int expiresAfterTurn, int max) : base(name, owner, targetUid, expiresAfterTurn, true) { this.max = max; }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || info.Attacker.Owner == Owner) return damage;
                return engine.CurrentSourceKind == EffectSourceKind.Attack && damage <= max ? 0 : damage;
            }
        }

        /// <summary>Bide: flip a coin; heads, during the opponent's next turn this Pokémon isn't Knocked Out by attack damage (it's left with 10 HP).</summary>
        internal sealed class BideMon : Mon
        {
            private readonly string attack;

            public BideMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new BideEffect(me.Name + " can't be Knocked Out by attack damage during the opponent's next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class BideEffect : ContinuousEffect
        {
            public BideEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>"Remove from this Pokémon as many damage counters as the damage you did to the Defending Pokémon" (Giga Drain); also "if this attack does any damage, remove N" (Leech Seed).</summary>
        internal sealed class DrainMon : Mon
        {
            private readonly string attack;
            private readonly int fixedHeal;

            /// <summary>fixedHeal 0: heal the damage done; otherwise heal that much if any damage was done.</summary>
            public DrainMon(string attack, int fixedHeal)
            {
                this.attack = attack;
                this.fixedHeal = fixedHeal;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                int done = ctx.DamageDefending(atk.Damage);
                if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, fixedHeal > 0 ? fixedHeal : done);
                return true;
            }
        }

        /// <summary>"If there is any Stadium card in play, this attack does N more damage. Discard that Stadium card" (Storm Up, Wreck).</summary>
        internal sealed class StadiumBonusDiscardMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public StadiumBonusDiscardMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? bonus : 0));
                if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>"Search your discard pile for a Trainer card and put it into your hand" (Scavenge; DP wording: not Supporters or Stadiums).</summary>
        internal sealed class TrainerFromDiscardMon : Mon
        {
            private readonly string attack;

            public TrainerFromDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => DpTrainer(c.Def)).ToList();
                if (pile.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                var pick = ctx.ChooseCards(pile, 1, 1, attack + ": put which Trainer card into your hand?");
                ctx.Engine.ShowCards(ctx.Opponent, pick, attack, attack + ": the card your opponent took", ctx.Source);
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        /// <summary>"Search your deck for [a card] and attach it to 1 of your Pokémon" (Synthesis).</summary>
        internal sealed class SearchAttachMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;

            public SearchAttachMon(string attack, Func<CardDef, bool> filter)
            {
                this.attack = attack;
                this.filter = filter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(filter, 1, attack + ": choose an Energy card to attach");
                foreach (var card in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), attack + ": attach " + card.Def.Name + " to which of your Pokémon?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target))
                        if (!ctx.Me.Deck.Contains(card) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Pokémon [that ...]. This attack does N damage to it" (no W/R on the Bench).</summary>
        internal sealed class ChooseOpponentMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly Func<PokemonInPlay, bool> filter;

            public ChooseOpponentMon(string attack, int amount, Func<PokemonInPlay, bool> filter)
            {
                this.attack = attack;
                this.amount = amount;
                this.filter = filter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(filter);
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, attack + ": " + amount + " damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, amount);
                return true;
            }
        }

        /// <summary>"The Defending Pokémon is now [condition] and can't retreat during your opponent's next turn" (on heads with flip).</summary>
        internal sealed class ConditionNoRetreatMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;
            private readonly bool flip;

            public ConditionNoRetreatMon(string attack, SpecialCondition condition, bool flip)
            {
                this.attack = attack;
                this.condition = condition;
                this.flip = flip;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if ((flip && !ctx.Flip()) || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, condition);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>The owner's opponent can't play Trainer cards (DP wording: not Supporters or Stadiums) from their hand until it ends.</summary>
        internal sealed class DpTrainerLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public DpTrainerLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected && card != null && DpTrainer(card.Def); }
        }

        /// <summary>Base for an Unown's Poké-Power usable only while it's on your Bench (ANALYZE, BOUNCE, CALL, DRAW...).</summary>
        internal abstract class BenchUnownPower : Mon
        {
            protected abstract string Power { get; }
            protected virtual bool Ready(EffectContext ctx) { return true; }
            protected abstract void Use(EffectContext ctx);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && Ready(ctx); }

            protected override void Ability(EffectContext ctx) { Use(ctx); }
        }
    }
}
