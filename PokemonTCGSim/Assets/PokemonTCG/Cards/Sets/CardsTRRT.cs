using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Rocket Returns Trainer and Energy cards (TRR 84, 85, 87-95; v164). Copycat, Rocket's Admin. and Here Comes Team Rocket!
    /// are prints. Rocket's Hideout 87 is its own card (your ruling: Dark or Rocket's Pokémon). Rocket's Secret Machines play as
    /// Items.
    /// </summary>
    public static class CardsTRRT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TRR-84", new PokemonRetriever());
            r.RegisterByKey("TRR-85", new PowHandExtension());
            r.RegisterByKey("TRR-87", new RocketsHideoutTRR());
            r.RegisterByKey("TRR-88", new RocketsMission());
            r.RegisterByKey("TRR-89", new RocketsPokeBall());
            r.RegisterByKey("TRR-90", new RocketsTrickyGym());
            r.RegisterByKey("TRR-91", new SurpriseTimeMachine());
            r.RegisterByKey("TRR-92", new SwoopTeleporter());
            r.RegisterByKey("TRR-93", new VentureBomb());
            r.RegisterByKey("TRR-94", new DarkMetalEnergy());
            r.RegisterByKey("TRR-95", new REnergy());
        }

        /// <summary>Pokémon Retriever: 1 Basic Pokémon or Evolution card from your discard pile into your hand, or 3 of them into your deck.</summary>
        private sealed class PokemonRetriever : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                CardsTRR.RetrieveOneOrThree(ctx, d => d.IsPokemon, "Basic Pokémon or Evolution card", "Pokémon Retriever");
            }
        }

        /// <summary>
        /// Pow! Hand Extension: only with more Prize cards left than the opponent. Move an Energy card from the Defending Pokémon to
        /// another of the opponent's Pokémon, or switch in 1 of their Benched Pokémon.
        /// </summary>
        private sealed class PowHandExtension : CardScript
        {
            private static bool CanMove(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0 && ctx.Opp.Bench.Count > 0; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Opp.Active;
                int mode = CanMove(ctx) ? ctx.ChooseOption("Pow! Hand Extension: what do you want to do?", "Move an Energy card from the Defending Pokémon", "Switch in 1 of your opponent's Benched Pokémon") : 1;
                if (mode == 1)
                {
                    ctx.GustOpponent("Pow! Hand Extension: choose the new Defending Pokémon");
                    return;
                }
                if (!ctx.CanAffect(active)) return;
                var cards = active.Energy.ToList();
                var card = ctx.ChooseCards(cards, 1, 1, "Pow! Hand Extension: move which Energy card from " + active.Name + "?")[0];
                var bench = ctx.OpponentBench();
                var to = ctx.ChoosePokemon(bench, "Pow! Hand Extension: move " + card.Def.Name + " to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(active, to, card);
            }
        }

        /// <summary>Rocket's Hideout (Stadium, Team Rocket Returns): each Pokémon with Dark or Rocket's in its name (both players') gets +20 HP.</summary>
        private sealed class RocketsHideoutTRR : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon != null && CardsTRR.DarkOrRocket(mon.Name) ? hp + 20 : hp;
            }
        }

        /// <summary>Rocket's Mission (Supporter): discard a card from your hand, then draw 3 (4 if it was a Pokémon with Dark or Rocket's in its name).</summary>
        private sealed class RocketsMission : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count == 0) return;
                var card = ctx.ChooseCards(hand, 1, 1, "Rocket's Mission: discard a card from your hand")[0];
                bool rocket = card.Def.IsPokemon && CardsTRR.DarkOrRocket(card.Def.Name);
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(rocket ? 4 : 3);
            }
        }

        /// <summary>Rocket's Poké Ball: a Pokémon with Dark in its name from your deck into your hand.</summary>
        private sealed class RocketsPokeBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && CardsTRR.Dark(d.Name), 1, "Rocket's Poké Ball: choose a Pokémon with Dark in its name");
            }
        }

        /// <summary>Rocket's Tricky Gym (Stadium): each Pokémon with Dark or Rocket's in its name can use this card's attack, Feint Attack ([C]: 20 to 1 of the opponent's Pokémon, no W/R or effects).</summary>
        private sealed class RocketsTrickyGym : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                var gym = src.Card;
                if (gym == null || mon == null || gym.Def.Attacks.Count == 0 || !CardsTRR.DarkOrRocket(mon.Name)) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(gym.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                ctx.DamageAnyOpponent(20, true, true);
            }
        }

        /// <summary>
        /// Surprise! Time Machine: the highest Stage Evolution card of 1 of your Evolved Pokémon is shuffled into your deck (devolving
        /// it); if it's still in play, a card that evolves from it comes from your deck onto it (evolving it).
        /// </summary>
        private sealed class SurpriseTimeMachine : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(CardsE2.IsEvolved).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var evolved = ctx.MyPokemon(CardsE2.IsEvolved);
                var mon = ctx.ChoosePokemon(evolved, "Surprise! Time Machine: devolve which of your Pokémon?");
                if (mon == null) return;
                ctx.Engine.Devolve(mon, 1, true);
                if (ctx.InPlay(mon) && ctx.Me.Deck.Count > 0)
                {
                    string name = mon.Name;
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name, 1, "Surprise! Time Machine: choose a card that evolves from " + name, false);
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(mon, found[0]);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Swoop! Teleporter: a Basic Pokémon (not a Pokémon-ex) from your deck takes the place of 1 of your Basic Pokémon in play
        /// (not a Pokémon-ex) with everything on it; the first one goes to the discard pile.
        /// </summary>
        private sealed class SwoopTeleporter : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsBasicPokemon && !d.IsOldEx; }

            private static List<PokemonInPlay> Spots(EffectContext ctx) { return ctx.MyPokemon(m => m.Stack.Count == 1 && Fits(m.Def)); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Spots(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(Fits, 1, "Swoop! Teleporter: choose a Basic Pokémon (not a Pokémon-ex)");
                if (found.Count > 0)
                {
                    var spots = Spots(ctx);
                    var mon = spots.Count == 0 ? null : ctx.ChoosePokemon(spots, "Swoop! Teleporter: switch " + found[0].Def.Name + " with which of your Pokémon?");
                    if (mon == null) ctx.Me.Deck.Add(found[0]);
                    else
                    {
                        var old = mon.Top;
                        mon.Stack[mon.Stack.Count - 1] = found[0];
                        ctx.Me.Discard.Add(old);
                        ctx.Log("Swoop! Teleporter: " + old.Def.Name + " was switched with " + found[0].Def.Name + ".");
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Venture Bomb: heads, 1 damage counter on 1 of the opponent's Pokémon; tails, on 1 of yours.</summary>
        private sealed class VentureBomb : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool heads = ctx.Flip();
                var all = heads ? ctx.OpponentPokemon() : ctx.MyPokemon();
                if (all.Count == 0) return;
                var target = ctx.ChoosePokemon(all, "Venture Bomb: put 1 damage counter on which Pokémon?");
                if (target != null && (!heads || ctx.CanAffect(target))) ctx.PlaceCounters(target, 1);
            }
        }

        /// <summary>Dark Metal Energy: Darkness or Metal, 1 Energy at a time.</summary>
        private sealed class DarkMetalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Darkness) | EnergyMask.Of(EnergyType.Metal), 1);
            }
        }

        /// <summary>
        /// R Energy: only on a Pokémon with Dark or Rocket's in its name; 2 Darkness Energy; its Pokémon's attacks do 10 more to the
        /// opponent's Active Pokémon (before W/R); discarded at the end of your turn.
        /// </summary>
        private sealed class REnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Darkness), 2); }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && CardsTRR.DarkOrRocket(target.Name); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 10;
            }

            public override void OnEndOfTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder != null && holder.Energy.Contains(src.Card)) src.Engine.DiscardEnergy(holder, src.Card);
            }
        }
    }
}
