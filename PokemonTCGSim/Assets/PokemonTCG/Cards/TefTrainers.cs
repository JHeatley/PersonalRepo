using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Temporal Forces (TEF) Trainers added in v8. Registered by name, so reprints share them.</summary>
    public static class TefTrainers
    {
        public static void Register(CardScriptRegistry r)
        {
            // Items
            r.RegisterByName("Awakening Drum", new AwakeningDrum());
            r.RegisterByName("Boxed Order", new BoxedOrder());
            r.RegisterByName("Hand Trimmer", new HandTrimmer());
            r.RegisterByName("Master Ball", new MasterBall());
            r.RegisterByName("Reboot Pod", new RebootPod());

            // Pokémon Tools
            r.RegisterByName("Ancient Booster Energy Capsule", new AncientBoosterEnergyCapsule());
            r.RegisterByName("Future Booster Energy Capsule", new FutureBoosterEnergyCapsule());
            r.RegisterByName("Heavy Baton", new HeavyBaton());
            r.RegisterByName("Rescue Board", new RescueBoard());

            // Supporters
            r.RegisterByName("Morty's Conviction", new MortysConviction());

            // Stadiums
            r.RegisterByName("Full Metal Lab", new FullMetalLab());
            r.RegisterByName("Perilous Jungle", new PerilousJungle());
        }
    }

    // ================================================================== Items

    /// <summary>ACE SPEC: draw a card for each of your Ancient Pokémon in play.</summary>
    public sealed class AwakeningDrum : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Draw(ctx.MyPokemon(m => m.Def.IsAncient).Count);
        }
    }

    /// <summary>Search your deck for up to 2 Item cards. Your turn ends.</summary>
    public sealed class BoxedOrder : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsItem, 2, "Boxed Order: choose up to 2 Item cards (your turn then ends)");
            ctx.Engine.RequestEndTurn();
        }
    }

    /// <summary>Each player discards down to 5 cards in hand, the opponent first.</summary>
    public sealed class HandTrimmer : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            TrimTo5(ctx, ctx.Opponent);
            TrimTo5(ctx, ctx.Player);
        }

        private static void TrimTo5(EffectContext ctx, int player)
        {
            var hand = ctx.State.Players[player].Hand;
            int extra = hand.Count - 5;
            if (extra <= 0) return;
            var picks = ctx.Engine.ChooseCards(player, hand.ToList(), extra, extra, "Hand Trimmer: discard " + extra + (extra == 1 ? " card" : " cards") + " from your hand", ctx.Source);
            foreach (var c in picks) ctx.Engine.DiscardFromHand(player, c);
        }
    }

    /// <summary>ACE SPEC: search your deck for any Pokémon.</summary>
    public sealed class MasterBall : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToHand(d => d.IsPokemon, 1, "Master Ball: choose a Pokémon");
        }
    }

    /// <summary>ACE SPEC: attach a Basic Energy from your discard pile to each of your Future Pokémon.</summary>
    public sealed class RebootPod : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.MyPokemon(m => m.Def.IsFuture).Count > 0 && ctx.BasicEnergyInDiscard().Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            foreach (var mon in ctx.MyPokemon(m => m.Def.IsFuture))
            {
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) break;
                var pick = ctx.ChooseCards(energy, 1, 1, "Reboot Pod: choose a Basic Energy to attach to " + ctx.Engine.Describe(mon))[0];
                ctx.Me.Discard.Remove(pick);
                ctx.Engine.AttachEnergyCard(pick, mon);
            }
        }
    }

    // ================================================================== Pokémon Tools

    /// <summary>Ancient Pokémon only: +60 HP, recovers from and can't be affected by Special Conditions.</summary>
    public sealed class AncientBoosterEnergyCapsule : CardScript
    {
        public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
        {
            return mon == src.Pokemon && mon.Def.IsAncient ? hp + 60 : hp;
        }

        // The engine removes Special Conditions a Pokémon can't have right after each action ("recovers").
        public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
        {
            return target == src.Pokemon && target.Def.IsAncient;
        }
    }

    /// <summary>Future Pokémon only: no Retreat Cost, and +20 damage to the opponent's Active Pokémon.</summary>
    public sealed class FutureBoosterEnergyCapsule : CardScript
    {
        public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
        {
            return mon == src.Pokemon && mon.Def.IsFuture ? cost - 100 : cost;
        }

        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            return info.Attacker == src.Pokemon && src.Pokemon.Def.IsFuture && info.ToOpponentsActive ? damage + 20 : damage;
        }
    }

    /// <summary>
    /// If the holder (Retreat Cost exactly 4) is Knocked Out in the Active Spot by damage from an opponent's attack,
    /// move up to 3 Basic Energy from it to your Benched Pokémon.
    /// </summary>
    public sealed class HeavyBaton : CardScript
    {
        public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
        {
            var mon = src.Pokemon;
            if (mon == null || !byOpponentsAttack || !wasActive) return;
            if (src.Engine.RetreatCostOf(mon) != 4) return;
            for (int i = 0; i < 3; i++)
            {
                var bench = src.Me.Bench.Where(m => m != mon && !src.Engine.IsKnockedOut(m)).ToList();
                var energy = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (bench.Count == 0 || energy.Count == 0) return;
                var pick = src.ChooseCards(energy, 0, 1, "Heavy Baton: move a Basic Energy from " + mon.Name + " to a Benched Pokémon (" + (i + 1) + " of up to 3)");
                if (pick.Count == 0) return;
                var to = src.ChoosePokemon(bench, "Heavy Baton: move " + pick[0].Def.Name + " to which Benched Pokémon?");
                src.Engine.MoveEnergy(mon, to, pick[0]);
            }
        }
    }

    /// <summary>Retreat Cost [C] less; no Retreat Cost at 30 HP or less remaining.</summary>
    public sealed class RescueBoard : CardScript
    {
        public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
        {
            if (mon != src.Pokemon) return cost;
            return src.Engine.RemainingHp(mon) <= 30 ? cost - 100 : cost - 1;
        }
    }

    // ================================================================== Supporters

    /// <summary>Discard another card from your hand, then draw a card for each of your opponent's Benched Pokémon.</summary>
    public sealed class MortysConviction : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Hand.Count(c => c != ctx.Card) >= 1;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
            if (others.Count == 0) return;
            var discard = ctx.ChooseCards(others, 1, 1, "Morty's Conviction: discard a card from your hand")[0];
            ctx.Engine.DiscardFromHand(ctx.Player, discard);
            ctx.Draw(ctx.Opp.Bench.Count);
        }
    }

    // ================================================================== Stadiums

    /// <summary>Metal Pokémon (both players') take 30 less damage from the opponent's attacks.</summary>
    public sealed class FullMetalLab : CardScript
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            return info.AgainstOpponent && src.Engine.IsType(info.Target, EnergyType.Metal) ? damage - 30 : damage;
        }
    }

    /// <summary>Pokémon Checkup: 2 more damage counters on each Poisoned non-Darkness Pokémon.</summary>
    public sealed class PerilousJungle : CardScript
    {
        public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters)
        {
            return src.Engine.IsType(mon, EnergyType.Darkness) ? counters : counters + 2;
        }
    }
}
