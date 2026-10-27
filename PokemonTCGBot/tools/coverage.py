"""Report which cards have every printed effect implemented.

    python tools/coverage.py            # whole pool summary
    python tools/coverage.py decks      # what's missing in each deck in decks/
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
from tcg.carddb import CARDS  # noqa: E402
from tcg import effects  # noqa: E402
from tcg.deckio import load_deck, deck_names  # noqa: E402


def summary():
    by_kind = {}
    for c in CARDS:
        k = c.supertype
        done, total = by_kind.get(k, (0, 0))
        by_kind[k] = (done + effects.card_implemented(c), total + 1)
    for k, (done, total) in by_kind.items():
        print(f"{k:8} {done:5}/{total} fully implemented ({100 * done / total:.0f}%)")
    atks = [(c, a) for c in CARDS for a in c.attacks if a.text]
    ok = sum(effects.attack_implemented(c, a) for c, a in atks)
    print(f"attacks with text: {ok}/{len(atks)} implemented")


def decks():
    for name in deck_names():
        deck = load_deck(name)
        missing = {}
        for c in deck:
            parts = effects.missing_parts(c)
            if parts:
                missing[c.name] = parts
        print(f"\n== {name}: {len(missing)} cards incomplete")
        for n, parts in missing.items():
            for p in parts:
                print(f"   {n}: {p}")


if __name__ == "__main__":
    decks() if sys.argv[1:] == ["decks"] else summary()
