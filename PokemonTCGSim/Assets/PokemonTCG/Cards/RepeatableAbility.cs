using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// "As often as you like during your turn" Abilities (Solar Transfer, Electric Streamer, Inferno Fandango...).
    /// For the person playing, one use of the Ability keeps going: every prompt has a Done button, so you attach or move
    /// as many as you want and press Done, instead of opening the Pokémon and using the Ability again each time.
    /// Bots (and tests with scripted players) still do one per use, exactly as before, so the AI's learning isn't affected.
    /// </summary>
    public static class RepeatableAbility
    {
        /// <summary>A safety stop; nobody moves 60 Energy in one go.</summary>
        private const int MaxSteps = 60;

        public const string DoneLabel = "Done";

        /// <summary>The player is the person on screen (not a bot).</summary>
        public static bool ForPerson(EffectContext ctx)
        {
            return ctx.Engine.IsInteractive(ctx.Player);
        }

        /// <summary>Choose one of your Pokémon, or Done (null). Without offerDone it's the plain choice.</summary>
        public static PokemonInPlay ChoosePokemonOrDone(EffectContext ctx, IList<PokemonInPlay> candidates, string prompt, bool offerDone)
        {
            if (candidates == null || candidates.Count == 0) return null;
            if (!offerDone) return ctx.ChoosePokemon(candidates, prompt);
            var opts = new List<DecisionOption>();
            foreach (var m in candidates)
                opts.Add(new DecisionOption { Label = ctx.Engine.DescribeFor(ctx.Player, m), PokemonUid = m.Uid, CardId = m.Top.Id, Card = m.Def });
            opts.Add(new DecisionOption { Label = DoneLabel, Action = ActionType.EndTurn });
            int k = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChoosePokemon, prompt, opts, ctx.Source, false);
            return k >= 0 && k < candidates.Count ? candidates[k] : null;
        }

        /// <summary>
        /// Runs step until the person presses Done (step returns false) or nothing is left to do. Bots: one step.
        /// step(offerDone) does one attachment or move and returns whether it happened.
        /// </summary>
        public static int Repeat(EffectContext ctx, Func<bool> canStep, Func<bool, bool> step)
        {
            bool person = ForPerson(ctx);
            int done = 0;
            for (int i = 0; i < MaxSteps && canStep(); i++)
            {
                if (!step(person)) break;
                done++;
                if (!person) break;
            }
            return done;
        }

        /// <summary>
        /// Attach Energy from your hand, one target at a time: the person clicks a Pokémon for each Energy and presses Done.
        /// When the matching cards are all the same card (Basic Energy of one type), it doesn't ask which one.
        /// </summary>
        public static int AttachFromHand(EffectContext ctx, Func<CardDef, bool> cardFilter, Func<List<PokemonInPlay>> targets, string ability, string energyName)
        {
            return Repeat(ctx,
                () => ctx.Me.Hand.Any(c => cardFilter(c.Def)) && Attachable(ctx, targets()).Count > 0,
                offerDone =>
                {
                    var cards = ctx.Me.Hand.Where(c => cardFilter(c.Def)).ToList();
                    var spots = Attachable(ctx, targets());
                    if (cards.Count == 0 || spots.Count == 0) return false;
                    string left = offerDone ? " (" + cards.Count + " in your hand; press Done when finished)" : "";
                    var target = ChoosePokemonOrDone(ctx, spots, ability + ": attach " + energyName + " to which Pokémon?" + left, offerDone);
                    if (target == null) return false;
                    var card = cards.Select(c => c.Def.Name).Distinct().Count() == 1 ? cards[0]
                        : ctx.ChooseCards(cards, 1, 1, ability + ": attach which card to " + target.Name + "?")[0];
                    if (!ctx.Me.Hand.Remove(card)) return false;
                    ctx.Engine.AttachEnergyCard(card, target, true);
                    return true;
                });
        }

        private static List<PokemonInPlay> Attachable(EffectContext ctx, List<PokemonInPlay> targets)
        {
            return targets.Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
        }
    }
}
