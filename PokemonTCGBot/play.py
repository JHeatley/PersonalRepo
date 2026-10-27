"""Play against the bot in the terminal.

    python play.py                                   # random decks vs your best checkpoint, else models/bot_40k.pt
    python play.py --deck dragapult --bot-deck basic_box
    python play.py --bot checkpoints/latest.pt
    python play.py --bot heuristic
    python play.py --deck my_list.txt               # any TCG Live / Limitless export file
    python play.py --watch                          # bot vs bot, printing the whole game
"""
import argparse
import os
import random

from tcg.engine import Game
from tcg.deckio import load_deck, deck_names
from tcg.carddb import SYMBOL
from bot.agents import make_agent


def mon_line(g, m):
    energy = "".join(SYMBOL[next(iter(u))] if len(u) == 1 else "*" for u in g.energy_units(m)) or "-"
    extra = []
    if m.tool:
        extra.append(f"tool: {m.tool.name}")
    extra += m.conditions
    tag = " ex" if m.card.ex and not m.name.endswith("ex") else ""
    return f"{m.name}{tag}  HP {g.hp_left(m)}/{g.max_hp(m)}  [{energy}]" + (f"  ({', '.join(extra)})" if extra else "")


def show(g, me):
    p, o = g.players[me], g.players[1 - me]
    print("\n" + "=" * 72)
    print(f"Turn {g.turn}   {'YOUR' if g.current == me else 'OPPONENT'} turn")
    print(f"Opponent: {len(o.prizes)} prizes left | hand {len(o.hand)} | deck {len(o.deck)} | discard {len(o.discard)}")
    if o.active:
        print("   Active: " + mon_line(g, o.active))
    for i, m in enumerate(o.bench, 1):
        print(f"   Bench {i}: " + mon_line(g, m))
    if g.stadium is not None:
        print(f"Stadium: {g.stadium.name} ({'yours' if g.stadium_owner == me else 'theirs'})")
    print("-" * 72)
    if p.active:
        print("   Active: " + mon_line(g, p.active))
    for i, m in enumerate(p.bench, 1):
        print(f"   Bench {i}: " + mon_line(g, m))
    print(f"You: {len(p.prizes)} prizes left | deck {len(p.deck)} | discard {len(p.discard)}")
    print("Hand: " + ", ".join(c.name for c in p.hand))


def ask_human(g, me):
    d = g.decision
    show(g, me)
    title = d.kind.replace("_", " ")
    if d.source is not None:
        title += f" ({d.source.name})"
    if d.prompt:
        title += f": {d.prompt}"
    print(f"\n{title}")
    for i, opt in enumerate(d.options):
        print(f"  {i:2d}. {g.describe(opt)}")
    while True:
        raw = input("> ").strip().lower()
        if raw in ("q", "quit"):
            raise SystemExit
        if raw in ("?", "card", "cards"):
            for c in sorted({c.name for c in g.players[me].hand}):
                print("  " + c)
            continue
        if raw.isdigit() and int(raw) < len(d.options):
            return int(raw)
        print(f"enter 0-{len(d.options) - 1} (q to quit)")


def main():
    os.chdir(os.path.dirname(os.path.abspath(__file__)))  # find models/ and decks/ from anywhere
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--deck", help="your deck: a name from decks/ or a file path (default random)")
    ap.add_argument("--bot-deck", help="the bot's deck (default random)")
    default_bot = next((f"{p}:greedy" for p in ("checkpoints/best.pt", "models/bot_40k.pt") if os.path.exists(p)),
                       "heuristic")
    ap.add_argument("--bot", default=default_bot, help="checkpoint path, 'path:greedy', or 'heuristic'")
    ap.add_argument("--seed", type=int, default=None)
    ap.add_argument("--watch", action="store_true", help="watch the bot play itself instead of playing")
    args = ap.parse_args()

    rng = random.Random(args.seed)
    mine = args.deck or rng.choice(deck_names())
    theirs = args.bot_deck or rng.choice(deck_names())
    bot = make_agent(args.bot)
    print(f"You: {mine}   vs   Bot ({args.bot}): {theirs}")
    g = Game(load_deck(mine), load_deck(theirs), seed=args.seed, log=True)
    me, shown = 0, 0
    while not g.done:
        for line in g.log[shown:]:
            print("  " + line)
        shown = len(g.log)
        if g.actor() == me and not args.watch:
            g.step(ask_human(g, me))
        else:
            g.step(bot.act([g])[0])
    for line in g.log[shown:]:
        print("  " + line)
    if args.watch:
        print(f"\nResult: {'draw' if g.winner is None else f'P{g.winner} wins'} ({g.end_reason})")
    else:
        print("\n" + ("Draw." if g.winner is None else "You win!" if g.winner == me else "The bot wins.")
              + f" ({g.end_reason})")


if __name__ == "__main__":
    main()
