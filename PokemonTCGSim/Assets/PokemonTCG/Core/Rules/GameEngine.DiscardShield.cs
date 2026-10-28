using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// v141: Parasect's Allergic Pollen (Neo Revelation): "cards in any player's discard piles are not affected by attacks or Pokémon
    /// Powers" (your ruling: only old-style Pokémon Powers, not Poké-Powers, Poké-Bodies or Abilities). Card scripts take cards out
    /// of discard piles in many ways, so the engine notes both discard piles when an attack or a Pokémon Power starts and, once it's
    /// over, puts back every card that was there and has left: from a hand, a deck, the Prize cards, the Lost Zone, a Pokémon's
    /// attached cards or Evolution stack (a Pokémon put into play from a discard pile leaves play again, and anything attached to it
    /// meanwhile goes to its owner's discard pile). Cards that only reached a discard pile during that attack or power aren't covered.
    /// </summary>
    public sealed partial class GameEngine
    {
        /// <summary>Something in play keeps discard piles out of attacks and Pokémon Powers right now (Allergic Pollen).</summary>
        public bool DiscardPilesShielded()
        {
            foreach (PassiveSource passive in Sources())
                if (passive.Script.ShieldsDiscardPiles(passive.Ctx)) return true;
            return false;
        }

        /// <summary>The card has an old-style Pokémon Power (Base Set to Neo), the kind Allergic Pollen covers.</summary>
        public static bool HasOldPokemonPower(CardDef def)
        {
            return def != null && def.Abilities.Any(a => a.Kind == AbilityDef.PokemonPower);
        }

        /// <summary>Notes both discard piles if they're shielded now; returns null otherwise. Pass the result to EndDiscardShield.</summary>
        public List<CardInstance>[] BeginDiscardShield()
        {
            if (!DiscardPilesShielded()) return null;
            return new[] { State.Players[0].Discard.ToList(), State.Players[1].Discard.ToList() };
        }

        /// <summary>Puts back every noted card that left its discard pile (see the class notes).</summary>
        public void EndDiscardShield(List<CardInstance>[] piles)
        {
            if (piles == null) return;
            for (int i = 0; i < 2 && i < piles.Length; i++)
                foreach (var card in piles[i])
                {
                    var discard = State.Players[i].Discard;
                    if (discard.Contains(card) || !TakeBackForShield(card)) continue;
                    discard.Add(card);
                    Emit(GameEventType.Info, i, "Allergic Pollen: " + card.Def.Name + " stays in " + Their(i) + " discard pile.", card.Id);
                }
        }

        private bool TakeBackForShield(CardInstance card)
        {
            foreach (var pl in State.Players)
            {
                if (pl.Hand.Remove(card) || pl.Deck.Remove(card) || pl.Prizes.Remove(card) || pl.LostZone.Remove(card)) return true;
                foreach (var mon in pl.InPlay().ToList())
                {
                    if (mon.Energy.Remove(card) || mon.Tools.Remove(card) || mon.Attached.Remove(card)) return true;
                    int at = mon.Stack.IndexOf(card);
                    if (at < 0) continue;
                    if (mon.Stack.Count > 1)
                    {
                        mon.Stack.RemoveAt(at);
                        return true;
                    }
                    var others = mon.AllCards().Where(c => c != card).ToList();
                    RemoveFromPlay(mon);
                    foreach (var other in others) State.Players[other.Owner].Discard.Add(other);
                    return true;
                }
            }
            return false;
        }
    }
}
