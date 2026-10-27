"""Read decklists in the Pokemon TCG Live / Limitless export format:

    Pokémon: 12
    4 Dreepy TWM 128
    ...
    Energy: 8
    3 Basic {P} Energy MEE 13

Cards are matched by set code + number when that printing is in the card
pool, otherwise by name (which must then be unambiguous).
"""
import os
import re

from .carddb import BY_NAME, BY_PRINT, BASIC_ENERGY, TYPE, CARDS

DECK_DIR = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "decks")
_LINE = re.compile(r"^(\d+)\s+(.+?)(?:\s+([A-Z0-9-]{2,5})\s+(\d+[a-z]?))?\s*$")
_ENERGY = re.compile(r"^(?:Basic\s+)?(?:\{([A-Z])\}|(\w+))\s+Energy$")
_SYMBOLS = {"G": "Grass", "R": "Fire", "W": "Water", "L": "Lightning", "P": "Psychic", "F": "Fighting",
            "D": "Darkness", "M": "Metal"}


def _basic_energy(name):
    m = _ENERGY.match(name)
    if not m:
        return None
    tname = _SYMBOLS.get(m.group(1)) if m.group(1) else m.group(2)
    return BASIC_ENERGY.get(TYPE.get(tname, -1))


def resolve(name, set_code=None, number=None):
    if set_code and (set_code, number) in BY_PRINT and BY_PRINT[(set_code, number)].name == name:
        return BY_PRINT[(set_code, number)]
    e = _basic_energy(name)
    if e is not None:
        return e
    found = BY_NAME.get(name) or BY_NAME.get(name.replace("Pokemon", "Pokémon"))
    if not found:
        raise ValueError(f"unknown card: {name} {set_code or ''} {number or ''}".strip())
    if len(found) > 1:
        options = ", ".join(f"{c.printings[0][0]} {c.printings[0][1]}" for c in found)
        raise ValueError(f"{name} is ambiguous ({options}); give a set code and number in the pool")
    return found[0]


def parse_deck(text):
    deck = []
    for raw in text.splitlines():
        line = raw.strip()
        if not line or line.endswith(":") or re.match(r"^(Pokémon|Pokemon|Trainer|Energy):?\s*\d*$", line) \
                or line.startswith(("#", "Total Cards")):
            continue
        m = _LINE.match(line)
        if not m:
            raise ValueError(f"can't read deck line: {raw!r}")
        n, name, code, num = int(m.group(1)), m.group(2), m.group(3), m.group(4)
        deck += [resolve(name, code, num)] * n
    return deck


def validate(deck):
    """Standard deck-building rules. Returns a list of problems (empty if legal)."""
    problems = []
    if len(deck) != 60:
        problems.append(f"{len(deck)} cards (need 60)")
    counts = {}
    for c in deck:
        if not c.basic_energy:
            counts[c.name] = counts.get(c.name, 0) + 1
    problems += [f"{n} copies of {name}" for name, n in counts.items() if n > 4]
    if sum(c.ace_spec for c in deck) > 1:
        problems.append("more than 1 ACE SPEC")
    if not any(c.is_basic for c in deck):
        problems.append("no Basic Pokemon")
    return problems


def load_deck(name_or_path):
    """Load decks/<name>.txt or any path."""
    path = name_or_path
    if not os.path.exists(path):
        path = os.path.join(DECK_DIR, name_or_path + ".txt")
    with open(path, encoding="utf-8") as f:
        deck = parse_deck(f.read())
    problems = validate(deck)
    if problems:
        raise ValueError(f"{path}: " + "; ".join(problems))
    return deck


def deck_names():
    return sorted(f[:-4] for f in os.listdir(DECK_DIR) if f.endswith(".txt"))
