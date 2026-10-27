"""Load data/cards.json into CardDef objects.

Card ids are indices into CARDS and are what the neural net embeds, so the
importer keeps them stable (see tools/import_cards.py).
"""
import json
import os
import re

TYPE_NAMES = ["Grass", "Fire", "Water", "Lightning", "Psychic", "Fighting", "Darkness", "Metal",
              "Dragon", "Colorless"]
GRASS, FIRE, WATER, LIGHTNING, PSYCHIC, FIGHTING, DARKNESS, METAL, DRAGON, COLORLESS = range(10)
NUM_TYPES = len(TYPE_NAMES)
TYPE = {n: i for i, n in enumerate(TYPE_NAMES)}
SYMBOL = "GRWLPFDMNC"

RULE_BOX = {"ex", "MEGA", "EX", "GX", "V", "VMAX", "VSTAR", "TAG TEAM", "LEGEND", "BREAK", "LEVEL-UP"}
TRAINER_KINDS = ("Item", "Supporter", "Stadium", "Pokémon Tool")
_OWNER = re.compile(r"^((?:Team Rocket|[A-Z][\w.&' ]*?))'s ")

DATA = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "data", "cards.json")


class AttackDef:
    __slots__ = ("index", "name", "cost", "base", "mod", "text")

    def __init__(self, index, d):
        self.index = index
        self.name = d["name"]
        self.cost = tuple(TYPE[t] for t in d["cost"])
        m = re.match(r"(\d*)\s*([+×x\-]?)", d["damage"])
        self.base = int(m.group(1)) if m.group(1) else 0
        self.mod = m.group(2).replace("x", "×")
        self.text = d["text"]

    def __repr__(self):
        return f"<{self.name} {''.join(SYMBOL[t] for t in self.cost)} {self.base}{self.mod}>"


class AbilityDef:
    __slots__ = ("index", "name", "text")

    def __init__(self, index, d):
        self.index = index
        self.name = d["name"]
        self.text = d["text"]


class CardDef:
    def __init__(self, d):
        self.id = d["id"]
        self.name = d["name"]
        self.supertype = d["supertype"]
        self.subtypes = frozenset(d["subtypes"])
        self.rules = d.get("rules", [])
        self.printings = [tuple(p) for p in d["printings"]]
        self.image = d.get("image")
        self.is_pokemon = self.supertype == "Pokemon"
        self.is_trainer = self.supertype == "Trainer"
        self.is_energy = self.supertype == "Energy"
        self.ace_spec = "ACE SPEC" in self.subtypes
        # Pokemon
        self.stage = None
        self.hp = 0
        self.types = ()
        self.evolves_from = None
        self.weakness = ()
        self.resistance = ()
        self.retreat = 0
        self.attacks = ()
        self.abilities = ()
        self.ex = self.mega = self.tera = self.rule_box = False
        self.prizes = 1
        self.owner_tag = None
        self.ancient = "Ancient" in self.subtypes
        self.future = "Future" in self.subtypes
        if self.is_pokemon:
            self.stage = 2 if "Stage 2" in self.subtypes else 1 if "Stage 1" in self.subtypes else 0
            self.hp = d["hp"]
            self.types = tuple(TYPE[t] for t in d["types"])
            self.evolves_from = d.get("evolvesFrom")
            self.weakness = tuple(TYPE[t] for t in d["weaknesses"])
            self.resistance = tuple(TYPE[t] for t in d["resistances"])
            self.retreat = d["retreat"]
            self.attacks = tuple(AttackDef(i, a) for i, a in enumerate(d["attacks"]))
            self.abilities = tuple(AbilityDef(i, a) for i, a in enumerate(d["abilities"]))
            self.ex = "ex" in self.subtypes or "EX" in self.subtypes
            self.mega = "MEGA" in self.subtypes
            self.tera = "Tera" in self.subtypes
            self.rule_box = bool(self.subtypes & RULE_BOX)
            self.prizes = 3 if self.mega else 2 if self.rule_box else 1
            m = _OWNER.match(self.name)
            self.owner_tag = m.group(1) if m else None
        # Trainer
        self.trainer_kind = next((k for k in TRAINER_KINDS if k in self.subtypes), None) if self.is_trainer else None
        self.is_item = self.trainer_kind == "Item"
        self.is_supporter = self.trainer_kind == "Supporter"
        self.is_stadium = self.trainer_kind == "Stadium"
        self.is_tool = self.trainer_kind == "Pokémon Tool"
        # Energy
        self.basic_energy = self.is_energy and "Basic" in self.subtypes
        self.energy_type = None
        if self.basic_energy:
            self.energy_type = TYPE[self.name.split()[1]]
        self.text = " ".join(self.rules)

    @property
    def is_basic(self):
        return self.is_pokemon and self.stage == 0

    def has_type(self, t):
        return t in self.types

    def __repr__(self):
        return f"<{self.name} #{self.id}>"


def _load():
    with open(DATA) as f:
        return [CardDef(d) for d in json.load(f)]


CARDS = _load()
NUM_CARDS = len(CARDS)
BY_NAME = {}
for _c in CARDS:
    BY_NAME.setdefault(_c.name, []).append(_c)
BY_PRINT = {p: c for c in CARDS for p in c.printings}
BASIC_ENERGY = {c.energy_type: c for c in CARDS if c.basic_energy}


def card(name):
    """The card with this name; error if the name is ambiguous."""
    found = BY_NAME.get(name)
    if not found:
        raise KeyError(f"no card named {name!r}")
    if len(found) > 1:
        raise KeyError(f"{name!r} matches {len(found)} cards; use a set code and number")
    return found[0]
