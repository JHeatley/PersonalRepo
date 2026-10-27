"""Card effects: attack text templates plus hand-written cards.

Lookup order for an attack: a hand-written implementation registered for
(card name, attack name), then the text template compiler, then plain damage.
"""
from .registry import ATTACKS, ABILITIES, TRAINERS, ENERGY
from . import templates
from . import trainers, pokemon, attachments  # noqa: F401  (registers effects)

_attack_cache = {}


def _plain(g, ctx):
    ctx.hit()


def attack_impl(card, atk):
    key = (card.id, atk.index)
    fn = _attack_cache.get(key)
    if fn is None:
        fn = ATTACKS.get((card.name, atk.name))
        if fn is None:
            fn = templates.compile_attack(card, atk) if atk.text else _plain
        if fn is None:
            fn = _plain
            fn_missing.add(key)
        _attack_cache[key] = fn
    return fn


fn_missing = set()


def trainer_impl(card):
    return TRAINERS.get(card.name)


_ability_hooks, _trainer_hooks, _energy_hooks = {}, {}, {}


def ability_hooks(card):
    """Hook tables of this card's passive Abilities (cached)."""
    h = _ability_hooks.get(card.id)
    if h is None:
        h = _ability_hooks[card.id] = [impl.hooks for a in card.abilities
                                       for impl in [ABILITIES.get((card.name, a.name))]
                                       if impl is not None and impl.kind == "passive"]
    return h


def trainer_hooks(card):
    h = _trainer_hooks.get(card.id)
    if h is None:
        impl = TRAINERS.get(card.name)
        h = _trainer_hooks[card.id] = impl.hooks if impl else {}
    return h


def energy_hooks(card):
    h = _energy_hooks.get(card.id)
    if h is None:
        impl = ENERGY.get(card.name)
        h = _energy_hooks[card.id] = impl.hooks if impl else {}
    return h


def attack_implemented(card, atk):
    if not atk.text or (card.name, atk.name) in ATTACKS:
        return True
    return templates.compile_attack(card, atk) is not None


def ability_implemented(card, ab):
    return (card.name, ab.name) in ABILITIES


def card_implemented(card):
    """True if every effect printed on this card has an implementation."""
    if card.is_pokemon:
        return all(attack_implemented(card, a) for a in card.attacks) and \
            all(ability_implemented(card, a) for a in card.abilities)
    if card.is_trainer:
        return card.name in TRAINERS
    return card.basic_energy or card.name in ENERGY


def missing_parts(card):
    out = []
    if card.is_pokemon:
        out += [f"attack {a.name}: {a.text}" for a in card.attacks if not attack_implemented(card, a)]
        out += [f"ability {a.name}: {a.text}" for a in card.abilities if not ability_implemented(card, a)]
    elif not card_implemented(card):
        out.append(card.text)
    return out
