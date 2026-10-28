using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Primal Clash (PRC, v101), part 3: the Trainers and Special Energy (122-144, Enhanced Hammer 162, Switch 163). Acro Bike, Energy
    /// Retrieval, Escape Rope, Exp. Share, Fresh Water Set, Gardevoir Spirit Link, Rare Candy, Scorched Earth, Weakness Policy,
    /// Enhanced Hammer and Switch were already in the pool.
    /// </summary>
    public static class CardsPRC3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Aggron Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Groudon Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Kyogre Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Archie's Ace in the Hole", new LastCardRevival(EnergyType.Water));
            r.RegisterByName("Maxie's Hidden Ball Trick", new LastCardRevival(EnergyType.Fighting));
            r.RegisterByName("Dive Ball", new DiveBall());
            r.RegisterByName("Professor Birch's Observations", new ProfessorBirchsObservations());
            r.RegisterByName("Repeat Ball", new RepeatBall());
            r.RegisterByName("Rough Seas", new RoughSeas());
            r.RegisterByName("Shrine of Memories", new ShrineOfMemories());
            r.RegisterByName("Silent Lab", new SilentLab());
            r.RegisterByName("Teammates", new Teammates());
            r.RegisterByName("Shield Energy", new ShieldEnergy());
            r.RegisterByName("Wonder Energy", new WonderEnergy());
        }

        private static bool IsType(CardDef d, EnergyType type) { return d.IsPokemon && (d.PokemonType == type || d.SecondType == type); }

        /// <summary>
        /// Archie's Ace in the Hole / Maxie's Hidden Ball Trick: only as the last card in your hand. Put a Water (Fighting) Pokémon
        /// of any Stage from your discard pile onto your Bench, then draw 5 cards. With nothing to put down (or a full Bench) you
        /// still draw.
        /// </summary>
        private sealed class LastCardRevival : CardScript
        {
            private readonly EnergyType type;

            public LastCardRevival(EnergyType type) { this.type = type; }

            public override bool CanPlayTrainer(EffectContext ctx) { return !ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => IsType(c.Def, type) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                if (pile.Count > 0 && ctx.BenchSpace > 0)
                {
                    var pick = ctx.ChooseCards(pile, 1, 1, ctx.SourceName + ": put which " + type + " Pokémon onto your Bench?")[0];
                    if (ctx.Me.Discard.Remove(pick)) ctx.Engine.PutOntoBench(ctx.Player, pick, false);
                }
                ctx.Draw(5);
            }
        }

        /// <summary>Dive Ball: search your deck for a Water Pokémon, reveal it, put it into your hand.</summary>
        private sealed class DiveBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => IsType(d, EnergyType.Water), 1, "Dive Ball: choose a Water Pokémon");
            }
        }

        /// <summary>Professor Birch's Observations: shuffle your hand into your deck; flip a coin: heads, draw 7; tails, draw 4.</summary>
        private sealed class ProfessorBirchsObservations : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Flip() ? 7 : 4);
            }
        }

        /// <summary>Repeat Ball: search your deck for a Pokémon with exactly the name of 1 of your Pokémon in play (Kyogre isn't Kyogre-EX: 2015-02-05).</summary>
        private sealed class RepeatBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.MyPokemon().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var names = new HashSet<string>(ctx.MyPokemon().Select(m => m.Name));
                ctx.SearchToHand(d => d.IsPokemon && names.Contains(d.Name), 1, "Repeat Ball: choose a Pokémon with the same name as 1 of yours in play");
            }
        }

        /// <summary>Rough Seas: once during each player's turn, that player may heal 30 damage from each of their Water and Lightning Pokémon.</summary>
        private sealed class RoughSeas : StadiumAction
        {
            private static bool Eligible(EffectContext ctx, PokemonInPlay mon)
            {
                return mon.DamageCounters > 0 && (ctx.Engine.IsType(mon, EnergyType.Water) || ctx.Engine.IsType(mon, EnergyType.Lightning));
            }

            protected override bool Ready(EffectContext ctx) { return ctx.MyPokemon().Any(m => Eligible(ctx, m)); }

            protected override bool Use(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon().Where(m => Eligible(ctx, m)).ToList()) ctx.Heal(mon, 30);
                return false;
            }
        }

        /// <summary>Shrine of Memories: each evolved Pokémon in play can use the attacks of its previous Evolutions (with the Energy they need).</summary>
        private sealed class ShrineOfMemories : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (mon == null || mon.Stack.Count < 2) return list;
                foreach (var card in mon.Stack)
                {
                    if (card == mon.Top || !card.Def.IsPokemon) continue;
                    for (int i = 0; i < card.Def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(card.Def, i));
                }
                return list;
            }
        }

        /// <summary>
        /// Silent Lab: every Basic Pokémon in play has no Abilities (a Pokémon whose top card isn't a Basic, BREAK included, keeps
        /// them: 2016-06-23). Ancient Traits aren't Abilities (2015-02-05).
        /// </summary>
        private sealed class SilentLab : CardScript
        {
            public override bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon)
            {
                return mon != null && mon.Def.IsBasicPokemon && !ctx.Engine.StadiumShielded(mon);
            }
        }

        /// <summary>Teammates: only if 1 of your Pokémon was Knocked Out during your opponent's last turn (not by Poison between turns: 2015-02-05); search for 2 cards.</summary>
        private sealed class Teammates : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.LastKnockedOutTurn == ctx.OpponentsLastTurn && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => true, 2, "Teammates: choose 2 cards", false);
            }
        }

        /// <summary>Shield Energy: Metal only, on a Metal Pokémon, which takes 10 less damage from the opponent's attacks (after Weakness and Resistance: 2015-05-07).</summary>
        private sealed class ShieldEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Metal)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Metal) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Metal); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Metal); }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || info.Attacker.Owner == holder.Owner) return damage;
                return src.Engine.IsType(holder, EnergyType.Metal) ? damage - 10 : damage;
            }
        }

        /// <summary>Wonder Energy: Fairy only, on a Fairy Pokémon; the opponent's attacks' effects (not damage) don't reach it.</summary>
        private sealed class WonderEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fairy)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Fairy) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Fairy); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fairy); }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target != null && target == src.Pokemon && kind == EffectSourceKind.Attack && src.Engine.IsType(target, EnergyType.Fairy);
            }
        }
    }
}
