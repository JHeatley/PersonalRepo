"""Pit two agents against each other over many games.

    python evaluate.py checkpoints/best.pt heuristic --games 1000
    python evaluate.py checkpoints/best.pt:greedy checkpoints/pool_000100.pt --games 2000
    python evaluate.py checkpoints/best.pt heuristic --deck-a dragapult --matchups

Agents are 'random', 'heuristic', or a checkpoint path (add ':greedy' to
always play the network's top choice instead of sampling).
"""
import argparse
import time

import torch

from tcg.deckio import load_deck, deck_names
from bot.agents import make_agent
from bot.arena import play_match


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("a")
    ap.add_argument("b")
    ap.add_argument("--games", type=int, default=500)
    ap.add_argument("--decks", default=None, help="comma-separated deck pool (default: all)")
    ap.add_argument("--deck-a", default=None, help="fix agent A's deck")
    ap.add_argument("--deck-b", default=None, help="fix agent B's deck")
    ap.add_argument("--matchups", action="store_true", help="print the per-deck matchup table")
    ap.add_argument("--seed", type=int, default=0)
    args = ap.parse_args()

    torch.set_num_threads(4)
    names = args.decks.split(",") if args.decks else deck_names()
    decks = {n: load_deck(n) for n in set(names) | {d for d in (args.deck_a, args.deck_b) if d}}
    a, b = make_agent(args.a), make_agent(args.b)
    t = time.time()
    r = play_match(a, b, decks, args.games, seed=args.seed, deck_a=args.deck_a, deck_b=args.deck_b)
    dt = time.time() - t
    print(f"{args.a} vs {args.b}: {r['wins']}-{r['draws']}-{r['losses']} (W-D-L), "
          f"win rate {r['win_rate']:.3f}, avg {r['avg_turns']:.1f} turns, {r['games'] / dt:.1f} games/s")
    if args.matchups:
        rows = sorted(r["matchups"].items(), key=lambda kv: -sum(kv[1]))
        print(f"\n{'A deck':24} {'B deck':24} {'W-D-L':>10} {'win%':>6}")
        for (da, db), (w, d, l) in rows:
            n = w + d + l
            print(f"{da:24} {db:24} {f'{w}-{d}-{l}':>10} {100 * (w + d / 2) / n:6.1f}")


if __name__ == "__main__":
    main()
