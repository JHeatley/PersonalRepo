"""Measure how well a bot sequences its turns.

    python tools/sequencing.py models/bot_53k.pt --games 200
    python tools/sequencing.py checkpoints/latest.pt --deck venusaur

The bot plays itself (top-rated move each time) and these common
sequencing mistakes are counted. Lower is better, and they should fall as
training goes on:

  shuffled hand away with plays left: played a hand-shuffling Supporter
      (Lillie's Determination, Judge, Unfair Stamp...) while Items, Basics to
      bench or an Energy attachment were still available. Those cards go back
      into the deck unused.
  attacked with free resources left: attacked (which ends the turn) while an
      Energy attachment, an Ability or a Basic to bench was still available.
  drew, then shuffled it away: used a draw/search Ability (Recon Directive,
      Run Errand...) and then shuffled the hand away in the same turn.
"""
import argparse
import os
import random
import sys

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
import torch  # noqa: E402

from tcg.engine import Game  # noqa: E402
from tcg.deckio import load_deck, deck_names  # noqa: E402
from bot.agents import make_agent  # noqa: E402

HAND_SHUFFLERS = {"Lillie's Determination", "Judge", "Unfair Stamp", "Team Rocket's Archer"}
DRAW_ABILITIES = {"Recon Directive", "Run Errand", "Flip the Script", "Teal Dance", "Trade", "Lunar Cycle",
                  "Boom Boom Groove", "Attract Customers", "Run Away Draw", "Last-Ditch Catch"}
USE_FIRST = ("play", "bench", "attach", "evolve", "tool")


def audit(agent, decks, n_games, focus=None, seed=0):
    rng = random.Random(seed)
    names = list(decks)
    c = dict.fromkeys(["shuffles", "shuffled with plays left", "attacks", "attacked with resources left",
                       "draw abilities", "drew then shuffled"], 0)
    for i in range(n_games):
        a = focus or rng.choice(names)
        g = Game(decks[a], decks[rng.choice(names)], seed=seed * 7919 + i)
        turn, drew = None, False
        while not g.done:
            d = g.decision
            choice = agent.act([g])[0]
            o = d.options[choice]
            if d.kind == "main" and d.player == g.current and (focus is None or d.player == 0):
                if g.turn != turn:
                    turn, drew = g.turn, False
                others = [x for x in d.options if x is not o]
                if o.kind == "play" and o.card.name in HAND_SHUFFLERS:
                    c["shuffles"] += 1
                    c["shuffled with plays left"] += any(x.kind in USE_FIRST and x.card is not o.card for x in others)
                    c["drew then shuffled"] += drew
                elif o.kind == "attack":
                    c["attacks"] += 1
                    c["attacked with resources left"] += any(x.kind in ("attach", "ability", "bench") for x in others)
                elif o.kind == "ability" and o.data.name in DRAW_ABILITIES:
                    c["draw abilities"] += 1
                    drew = True
            g.step(choice)
    return c


def rates(c):
    return {
        "shuffled hand away with plays left": c["shuffled with plays left"] / max(c["shuffles"], 1),
        "attacked with free resources left": c["attacked with resources left"] / max(c["attacks"], 1),
        "drew, then shuffled it away": c["drew then shuffled"] / max(c["shuffles"], 1),
    }


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("bot", help="checkpoint path (or 'heuristic')")
    ap.add_argument("--games", type=int, default=200)
    ap.add_argument("--deck", default=None, help="only measure this deck's turns")
    ap.add_argument("--seed", type=int, default=0)
    args = ap.parse_args()
    torch.set_num_threads(2)
    decks = {n: load_deck(n) for n in deck_names()}
    spec = args.bot if args.bot in ("heuristic", "random") or ":" in args.bot else args.bot + ":greedy"
    c = audit(make_agent(spec), decks, args.games, args.deck, args.seed)
    print(f"{args.bot}: {args.games} games, {c['attacks']} attacks, {c['shuffles']} hand shuffles, "
          f"{c['draw abilities']} draw Abilities")
    for name, r in rates(c).items():
        print(f"  {name:38} {100 * r:5.1f}%")


if __name__ == "__main__":
    main()
