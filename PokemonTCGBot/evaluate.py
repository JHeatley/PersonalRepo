"""Pit two agents against each other over many games.

    python evaluate.py checkpoints/best.pt heuristic --games 1000
    python evaluate.py checkpoints/best.pt:greedy checkpoints/pool_000100.pt --games 2000
    python evaluate.py checkpoints/best.pt heuristic --deck-a dragapult --matchups
    python evaluate.py checkpoints/best.pt:greedy checkpoints/best.pt:greedy --games 2000 --deck-table
                                         # the bot against itself: how each deck fares in the meta

Agents are 'random', 'heuristic', or a checkpoint path (add ':greedy' to
always play the network's top choice instead of sampling).
"""
import argparse
import json
import os
import time

import torch

from tcg.deckio import load_deck, deck_names
from bot.agents import make_agent
from bot.arena import play_match


def merge_saved(path, matchups):
    """Add these results to a running total kept in a JSON file; returns the combined totals."""
    total = {}
    if os.path.exists(path):
        with open(path, encoding="utf-8") as f:
            total = {tuple(k.split("|")): v for k, v in json.load(f).items()}
    for key, rec in matchups.items():
        t = total.setdefault(key, [0, 0, 0])
        for i in range(3):
            t[i] += rec[i]
    with open(path, "w", encoding="utf-8") as f:
        json.dump({"|".join(k): v for k, v in total.items()}, f)
    return total


def deck_tables(matchups, mirror, min_games=20, top=8):
    """Each deck's record against the field, then the most lopsided deck-vs-deck pairings."""
    decks, pairs = {}, {}
    for (da, db), (w, d, l) in matchups.items():
        sides = [(da, (w, d, l))] + ([(db, (l, d, w))] if mirror and da != db else [])
        for deck, rec in sides:
            t = decks.setdefault(deck, [0, 0, 0])
            for i in range(3):
                t[i] += rec[i]
        if da != db:
            key, rec = ((da, db), (w, d, l)) if da < db else ((db, da), (l, d, w))
            t = pairs.setdefault(key, [0, 0, 0])
            for i in range(3):
                t[i] += rec[i]

    def pct(w, d, l):
        return 100 * (w + d / 2) / max(w + d + l, 1)

    print(f"\n{'Deck':24} {'W-D-L':>11} {'win%':>6}")
    for deck, rec in sorted(decks.items(), key=lambda kv: -pct(*kv[1])):
        print(f"{deck:24} {'-'.join(map(str, rec)):>11} {pct(*rec):6.1f}")
    big = [(k, v) for k, v in pairs.items() if sum(v) >= min_games]
    if big:
        print(f"\nMost lopsided matchups ({min_games}+ games):")
        for (da, db), rec in sorted(big, key=lambda kv: -abs(pct(*kv[1]) - 50))[:top]:
            w = pct(*rec)
            fav, dog, p = (da, db, w) if w >= 50 else (db, da, 100 - w)
            print(f"  {fav} beats {dog} {p:.0f}% ({sum(rec)} games)")


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("a")
    ap.add_argument("b")
    ap.add_argument("--games", type=int, default=500)
    ap.add_argument("--decks", default=None, help="comma-separated deck pool (default: all)")
    ap.add_argument("--deck-a", default=None, help="fix agent A's deck")
    ap.add_argument("--deck-b", default=None, help="fix agent B's deck")
    ap.add_argument("--matchups", action="store_true", help="print the per-deck matchup table")
    ap.add_argument("--deck-table", action="store_true",
                    help="print each deck's win rate and the most lopsided matchups")
    ap.add_argument("--save", default=None, help="add these results to a running total in this JSON file")
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
    matchups = merge_saved(args.save, r["matchups"]) if args.save else r["matchups"]
    if args.matchups:
        if args.save:
            print(f"\n(running totals over {sum(sum(v) for v in matchups.values())} games in {args.save})")
        rows = sorted(matchups.items(), key=lambda kv: -(kv[1][0] + kv[1][1] / 2) / max(sum(kv[1]), 1))
        print(f"\n{'A deck':24} {'B deck':24} {'W-D-L':>10} {'win%':>6}")
        for (da, db), (w, d, l) in rows:
            n = w + d + l
            print(f"{da:24} {db:24} {f'{w}-{d}-{l}':>10} {100 * (w + d / 2) / n:6.1f}")
    if args.deck_table:
        if args.save:
            print(f"\n(running totals over {sum(sum(v) for v in matchups.values())} games in {args.save})")
        deck_tables(matchups, mirror=args.a == args.b)


if __name__ == "__main__":
    main()
