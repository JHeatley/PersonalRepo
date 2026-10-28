using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ancient Origins (AOR, v98), part 3: the Trainers and Special Energy (69-83, Steven 95, Trainers' Mail 100). Energy Recycler,
    /// Level Ball, Lucky Helmet and Energy Retrieval were already in the pool; Hex Maniac and Forest of Giant Plants are on the
    /// Expanded ban list (Unlimited only).
    /// </summary>
    public static class CardsAOR3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Ace Trainer", new AceTrainer());
            r.RegisterByName("Ampharos Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Eco Arm", new EcoArm());
            r.RegisterByName("Faded Town", new FadedTown());
            r.RegisterByName("Forest of Giant Plants", new ForestOfGiantPlants());
            r.RegisterByName("Hex Maniac", new HexManiac());
            r.RegisterByName("Lysandre", new BosssOrders());
            r.RegisterByName("Paint Roller", new PaintRoller());
            r.RegisterByName("Sceptile Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Tyranitar Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Dangerous Energy", new DangerousEnergy());
            r.RegisterByName("Flash Energy", new FlashEnergy());
            r.RegisterByName("Steven", new Steven());
            r.RegisterByName("Trainers' Mail", new TrainersMail());
        }

        /// <summary>Ace Trainer: only with more Prize cards left than the opponent; both shuffle their hands into their decks, you draw 6 and they draw 3 (2015-08-13).</summary>
        private sealed class AceTrainer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Draw(6);
                ctx.Engine.DrawCards(ctx.Opponent, 3);
            }
        }

        /// <summary>Eco Arm: shuffle up to 3 Pokémon Tool cards from your discard pile into your deck.</summary>
        private sealed class EcoArm : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsTool); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var tools = ctx.Me.Discard.Where(c => c.Def.IsTool).ToList();
                if (tools.Count == 0) return;
                var picks = tools.Count <= 3 ? tools : ctx.ChooseCards(tools, 3, 3, "Eco Arm: shuffle which 3 Pokémon Tool cards into your deck?");
                foreach (var tool in picks)
                    if (ctx.Me.Discard.Remove(tool)) ctx.Me.Deck.Add(tool);
                ctx.Engine.Reveal(ctx.Player, picks, "shuffled these Pokémon Tool cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Faded Town: between turns, 2 damage counters on each Mega Evolution Pokémon in play (Primal Pokémon too: 2015).</summary>
        private sealed class FadedTown : CardScript
        {
            public override int CheckupAffects(EffectContext src) { return -1; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                for (int i = 0; i < 2; i++)
                    foreach (var mon in src.State.Players[i].InPlay().ToList())
                        if (mon.Def.IsXYMegaEvolution) src.Engine.PlaceDamageCounters(mon, 2, "Faded Town");
            }
        }

        /// <summary>
        /// Forest of Giant Plants: each player's Grass Pokémon can evolve into Grass Pokémon during their first turn and the turn they
        /// were played (Basic to Stage 1 to Stage 2 in one turn: 2015-08-13).
        /// </summary>
        private sealed class ForestOfGiantPlants : CardScript
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                if (target == null || evolution == null || src.Engine.StadiumShielded(target)) return false;
                bool grassInto = evolution.PokemonType == EnergyType.Grass || evolution.SecondType == EnergyType.Grass;
                return grassInto && src.Engine.IsType(target, EnergyType.Grass);
            }
        }

        /// <summary>
        /// Hex Maniac: until the end of the opponent's next turn, every Pokémon in play, in the hands and in the discard piles has no
        /// Abilities, cards put into play that turn included. Like Garbotoxin, it takes away the Pokémon's own Abilities, not one a
        /// Tool gives it. It's a Trainer's effect, so θ Stop doesn't stop it.
        /// </summary>
        internal sealed class HexManiacEffect : ContinuousEffect
        {
            public HexManiacEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool BlocksAbility(GameEngine engine, PokemonInPlay mon, string abilityName, int depth)
            {
                return mon != null && CardsBKP2.GarbodorBKP.OwnAbility(mon, abilityName);
            }

            public override bool BlocksAbilitiesOutOfPlay(GameEngine engine, int player, CardDef card) { return true; }
        }

        private sealed class HexManiac : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new HexManiacEffect("No Pokémon has any Abilities until the end of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s next turn (Hex Maniac)",
                    ctx.Player, ctx.Turn + 1));
            }
        }

        /// <summary>Paint Roller: discard any Stadium card in play; if you do, draw a card.</summary>
        private sealed class PaintRoller : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.State.Stadium != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.State.Stadium == null) return;
                ctx.Engine.DiscardStadium();
                if (ctx.State.Stadium == null) ctx.Draw(1);
            }
        }

        /// <summary>Dangerous Energy: Darkness only, on a Darkness Pokémon. Damaged while Active by an opponent's Pokémon-EX's attack, 2 counters on the attacker.</summary>
        private sealed class DangerousEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Darkness)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Darkness) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Darkness); }

            /// <summary>A Pokémon that's Darkness only through an Ability (Golurk's Double Type, the Eeveelutions' Effects) keeps it (2015-08-13).</summary>
            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Darkness); }

            /// <summary>Discarded by that attack, it does nothing (2015-08-13): the reaction only runs while it's still attached.</summary>
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (!holderWasActive || holder == null || attacker == null || attacker.Owner == ctx.Player || !attacker.Def.IsPokemonEX || !ctx.InPlay(attacker)) return;
                if (!holder.Energy.Contains(ctx.Card) || !ctx.Engine.IsType(holder, EnergyType.Darkness)) return;
                ctx.Engine.PlaceDamageCounters(attacker, 2, "Dangerous Energy");
            }
        }

        /// <summary>Flash Energy: Lightning only, on a Lightning Pokémon, which has no Weakness.</summary>
        private sealed class FlashEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Lightning)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Lightning) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Lightning); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Lightning); }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Lightning);
            }
        }

        /// <summary>Steven: search your deck for a Supporter card and a basic Energy card and put them into your hand.</summary>
        private sealed class Steven : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsSupporter, 1, "Steven: choose a Supporter card");
                found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Steven: choose a basic Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Trainers' Mail: look at the top 4 cards of your deck; you may put a Trainer card (not Trainers' Mail) from them into your hand; shuffle the rest back.</summary>
        private sealed class TrainersMail : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(4);
                var trainers = top.Where(c => c.Def.IsTrainer && c.Def.Name != "Trainers' Mail").ToList();
                var picks = trainers.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(trainers, 0, 1, "Trainers' Mail: you may put a Trainer card into your hand");
                foreach (var card in picks)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Hand.Add(card);
                if (picks.Count > 0) ctx.Engine.Reveal(ctx.Player, picks, "put this card into " + ctx.Engine.Their(ctx.Player) + " hand (Trainers' Mail)");
                ctx.ShuffleMyDeck();
            }
        }
    }
}
