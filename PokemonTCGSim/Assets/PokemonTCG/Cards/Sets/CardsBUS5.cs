using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Burning Shadows (BUS, v88), Trainers (BUS 112-128) and the secret rare Items and Tool with no regular print here (Choice Band
    /// 162, Multi Switch 164, Rescue Stretcher 165). Escape Rope, Guzma, Lana, Olivia, Sophocles, Super Scoop Up and Wishful Baton
    /// were already in the pool.
    /// </summary>
    public static class CardsBUS5
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Acerola", new Acerola());
            r.RegisterByName("Bodybuilding Dumbbells", new BodybuildingDumbbells());
            r.RegisterByName("Kiawe", new Kiawe());
            r.RegisterByName("Mount Lanakila", new MountLanakila());
            r.RegisterByName("Plumeria", new Plumeria());
            r.RegisterByName("Po Town", new PoTown());
            r.RegisterByName("Rotom Dex Poké Finder Mode", new RotomDexPokeFinder());
            r.RegisterByName("Tormenting Spray", new TormentingSpray());
            r.RegisterByName("Weakness Policy", new WeaknessPolicy());
            r.RegisterByName("Wicke", new Wicke());
            r.RegisterByName("Choice Band", new ChoiceBand());
            r.RegisterByName("Multi Switch", new MultiSwitch());
            r.RegisterByName("Rescue Stretcher", new RescueStretcher());
        }

        // ================================================================== Supporters

        /// <summary>Acerola: put 1 of your Pokémon that has any damage counters, and all attached cards, into your hand. Heal Block doesn't stop it (it isn't healing).</summary>
        private sealed class Acerola : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon().Any(m => m.DamageCounters > 0); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var mon = ctx.ChoosePokemon(hurt, "Acerola: put which of your damaged Pokémon and all attached cards into your hand?");
                if (mon != null) ctx.Engine.ReturnPokemonToHand(mon);
            }
        }

        /// <summary>Kiawe: search your deck for up to 4 Fire Energy cards and attach them to 1 of your Pokémon. Your turn ends.</summary>
        private sealed class Kiawe : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Kiawe: attach the Fire Energy to which of your Pokémon?");
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 4, "Kiawe: search your deck for up to 4 Fire Energy cards");
                foreach (var card in found)
                    if (target == null || !ctx.InPlay(target) || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Plumeria: discard 2 cards from your hand; if you do, discard an Energy from 1 of the opponent's Pokémon. The discard is a cost (Compendium, 2019-03-21).</summary>
        private sealed class Plumeria : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.OpponentPokemon().Any(m => m.Energy.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count < 2) return;
                foreach (var card in ctx.ChooseCards(hand, 2, 2, "Plumeria: discard 2 cards from your hand"))
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                var withEnergy = ctx.OpponentPokemon(m => m.Energy.Count > 0);
                if (withEnergy.Count == 0) return;
                var target = ctx.ChoosePokemon(withEnergy, "Plumeria: discard an Energy from which of your opponent's Pokémon?");
                if (target == null) return;
                var energy = ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Plumeria: discard which Energy?")[0];
                ctx.Engine.DiscardEnergy(target, energy);
            }
        }

        /// <summary>Wicke: each player counts the cards in their hand, shuffles them into their deck, then draws that many cards.</summary>
        private sealed class Wicke : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int mine = ctx.Me.Hand.Count(c => c != ctx.Card);
                int theirs = ctx.Opp.Hand.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Player, mine);
                ctx.Engine.DrawCards(ctx.Opponent, theirs);
            }
        }

        // ================================================================== Items

        /// <summary>Rotom Dex Poké Finder Mode: look at the top 4 cards of your deck; put them back in any order, or shuffle them into your deck.</summary>
        private sealed class RotomDexPokeFinder : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return;
                int pick = ctx.ChooseOption("Rotom Dex Poké Finder Mode: the top cards are " + string.Join(", ", top.Select(c => c.Def.Name).ToArray()) + ".",
                    "Put them back in any order", "Shuffle them into your deck");
                if (pick == 1)
                {
                    ctx.ShuffleMyDeck();
                    return;
                }
                CardsOBF2.ReorderOwnTop(ctx, top.Count, "Rotom Dex Poké Finder Mode");
            }
        }

        /// <summary>Tormenting Spray: choose a random card from the opponent's hand; they reveal it, and if it's a Supporter, discard it.</summary>
        private sealed class TormentingSpray : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Opp.Hand.Count == 0) return;
                var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { card }, "revealed a random card from " + ctx.Engine.Their(ctx.Opponent) + " hand (Tormenting Spray)");
                if (card.Def.IsSupporter) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            }
        }

        /// <summary>Multi Switch: move an Energy from 1 of your Benched Pokémon to your Active Pokémon.</summary>
        private sealed class MultiSwitch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Any(m => m.Energy.Count > 0); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var holders = ctx.Me.Bench.Where(m => m.Energy.Count > 0).ToList();
                if (active == null || holders.Count == 0) return;
                var donor = ctx.ChoosePokemon(holders, "Multi Switch: move an Energy from which Benched Pokémon?");
                if (donor == null) return;
                var energy = ctx.ChooseCards(donor.Energy.ToList(), 1, 1, "Multi Switch: move which Energy to " + active.Name + "?")[0];
                ctx.Engine.MoveEnergy(donor, active, energy);
            }
        }

        /// <summary>Rescue Stretcher: put a Pokémon from your discard pile into your hand, or shuffle 3 Pokémon from your discard pile into your deck.</summary>
        private sealed class RescueStretcher : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pokemon = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return;
                int mode = ctx.ChooseOption("Rescue Stretcher: choose 1", "Put a Pokémon from your discard pile into your hand", "Shuffle 3 Pokémon from your discard pile into your deck");
                if (mode == 0)
                {
                    ctx.DiscardToHand(ctx.ChooseCards(pokemon, 1, 1, "Rescue Stretcher: put which Pokémon into your hand?"));
                    return;
                }
                int n = Math.Min(3, pokemon.Count);
                ctx.DiscardToDeck(ctx.ChooseCards(pokemon, n, n, "Rescue Stretcher: shuffle which " + n + " Pokémon into your deck?"));
                ctx.ShuffleMyDeck();
            }
        }

        // ================================================================== Tools

        /// <summary>Bodybuilding Dumbbells: the Stage 1 Pokémon it's attached to gets +40 HP.</summary>
        private sealed class BodybuildingDumbbells : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.Stage == Stage.Stage1 ? hp + 40 : hp;
            }
        }

        /// <summary>Weakness Policy: the Pokémon it's attached to has no Weakness.</summary>
        private sealed class WeaknessPolicy : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }
        }

        /// <summary>Choice Band: the holder's attacks do 30 more damage to the opponent's Active Pokémon-GX or Pokémon-EX (before Weakness and Resistance).</summary>
        private sealed class ChoiceBand : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0) return damage;
                return info.Target.Def.IsGXorEX ? damage + 30 : damage;
            }
        }

        // ================================================================== Stadiums

        /// <summary>Mount Lanakila: the Retreat Cost of each Basic Pokémon in play (both players') is [C] more.</summary>
        private sealed class MountLanakila : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.IsBasic ? cost + 1 : cost;
            }
        }

        /// <summary>Po Town: whenever any player plays a Pokémon from their hand to evolve 1 of their Pokémon, put 3 damage counters on that Pokémon.</summary>
        private sealed class PoTown : CardScript
        {
            public override void OnPokemonEvolvedFromHandUnderStadium(EffectContext src, PokemonInPlay evolved, CardDef before)
            {
                if (evolved == null) return;
                src.Engine.PlaceDamageCounters(evolved, 3, "Po Town");
            }
        }
    }
}
