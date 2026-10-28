using System.Collections.Generic;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Sanity checks run between actions in tests and simulations. A failure here means an effect
    /// lost, duplicated or misplaced a card, which is the most common kind of rules-engine bug.
    /// </summary>
    public static class Invariants
    {
        /// <summary>Returns null when everything is fine, otherwise a description of the first problem found.</summary>
        public static string Check(GameEngine engine, int cardsPerPlayer)
        {
            var s = engine.State;
            var seen = new HashSet<int>();
            // Team Flare Hyper Gear (Head Ringer, Jamming Net...) is attached to the opponent's Pokémon: it stays its owner's card.
            var lent = new int[2];
            for (int i = 0; i < 2; i++)
                foreach (var mon in s.Players[i].InPlay())
                    foreach (var t in mon.Tools)
                        if (t.Owner != i && t.Owner >= 0 && t.Owner < 2) lent[t.Owner]++;
            for (int i = 0; i < 2; i++)
            {
                var p = s.Players[i];
                int count = lent[i];
                string err;

                err = Count(p.Deck, i, "deck", seen, ref count); if (err != null) return err;
                err = Count(p.Hand, i, "hand", seen, ref count); if (err != null) return err;
                err = Count(p.Discard, i, "discard", seen, ref count); if (err != null) return err;
                err = Count(p.Prizes, i, "prizes", seen, ref count); if (err != null) return err;
                err = Count(p.LostZone, i, "Lost Zone", seen, ref count); if (err != null) return err;
                err = Count(p.SetAsideHand, i, "set-aside hand", seen, ref count); if (err != null) return err;

                if (p.Bench.Count > engine.MaxBench(i)) return "Player " + (i + 1) + " has " + p.Bench.Count + " Benched Pokémon.";
                if (!s.InSetup && s.TurnNumber > 0 && !s.IsOver && p.Active == null && p.Bench.Count > 0)
                    return "Player " + (i + 1) + " has Benched Pokémon but no Active Pokémon.";

                foreach (var mon in p.InPlay())
                {
                    if (mon.Owner != i) return mon.Name + " is on the wrong side of the board.";
                    if (mon.DamageCounters < 0) return mon.Name + " has negative damage counters.";
                    if (!s.IsOver && engine.IsKnockedOut(mon)) return mon.Name + " should have been Knocked Out.";
                    // The bottom card is normally a Basic, but an effect can put an Evolution card into play on its own (Summoning Gate).
                    if (mon.Stack.Count == 0 || !(mon.Stack[0].Def.IsPokemon || mon.Stack[0].Def.IsFossil)) return "A Pokémon in play has no Pokémon card at the bottom of its stack.";
                    if (!s.IsActive(mon) && mon.Conditions != SpecialCondition.None) return "Benched " + mon.Name + " has a Special Condition.";
                    if (engine.PokemonToolCount(mon) > 1 && engine.PokemonToolCount(mon) > engine.MaxTools(mon)) return mon.Name + " has " + engine.PokemonToolCount(mon) + " Pokémon Tools attached.";
                    var own = new List<CardInstance>();
                    foreach (var c in mon.AllCards())
                    {
                        if (c.Owner != i && mon.Tools.Contains(c))
                        {
                            if (!seen.Add(c.Id)) return c.Def.Name + " #" + c.Id + " exists twice (found again on " + mon.Name + ").";
                        }
                        else own.Add(c);
                    }
                    err = Count(own, i, mon.Name, seen, ref count); if (err != null) return err;
                }

                if (s.Stadium != null && s.StadiumOwner == i)
                {
                    err = Count(new List<CardInstance> { s.Stadium }, i, "stadium", seen, ref count);
                    if (err != null) return err;
                    // v154.1: the other half of a Legendary Stadium in play is one of its owner's cards too.
                    if (s.StadiumPartner != null)
                    {
                        err = Count(new List<CardInstance> { s.StadiumPartner }, i, "stadium", seen, ref count);
                        if (err != null) return err;
                    }
                }

                if (cardsPerPlayer > 0 && count != cardsPerPlayer)
                    return "Player " + (i + 1) + " has " + count + " cards in total instead of " + cardsPerPlayer + ".";
            }
            return null;
        }

        private static string Count(IList<CardInstance> cards, int owner, string zone, HashSet<int> seen, ref int count)
        {
            foreach (var c in cards)
            {
                if (c.Owner != owner) return c.Def.Name + " (owned by Player " + (c.Owner + 1) + ") is in Player " + (owner + 1) + "'s " + zone + ".";
                if (!seen.Add(c.Id)) return c.Def.Name + " #" + c.Id + " exists twice (found again in " + zone + ").";
                count++;
            }
            return null;
        }
    }
}
