"""Where card effects are registered.

Effects are plain functions or generators taking the Game. A generator can
pause for player choices with `x = yield from g.ask(...)` (or one of the
helpers built on it: g.choose_cards, g.choose_pokemon, g.search_deck...).

Hooks are passive effects. The engine calls every active hook with the
source Pokemon (or None for a Stadium) as `src`; each hook decides whether it
applies. Hook names and signatures:

    attack_damage(g, src, ctx, target, dmg) -> dmg   attacker side, before Weakness/Resistance
    damage_taken(g, src, ctx, target, dmg) -> dmg    defender side, after Weakness/Resistance
    prevent_damage(g, src, ctx, target) -> bool
    after_damaged(g, src, ctx, target, dmg)          generator allowed
    retreat_cost(g, src, mon, cost) -> cost
    max_hp(g, src, mon, hp) -> hp
    weakness(g, src, mon, types) -> types
    attack_cost(g, src, mon, attack, cost) -> cost
    prize_count(g, src, mon, n, by_attack) -> n
    on_knockout(g, src, mon, by_attack)              generator allowed; src is the KO'd Pokemon's own effects
    checkup(g, src)                                  generator allowed
    ability_blocked(g, src, mon) -> bool
    tools_blocked(g, src) -> bool
    bench_limit(g, src, player, n) -> n
    no_effects(g, src, mon, kind) -> bool             kind is 'attack' or 'ability'
    condition_immune(g, src, mon) -> bool
    evolve_same_turn(g, src, mon, card) -> bool
    attack_twice(g, src, mon) -> bool
    basic_energy_units(g, src, mon, energy_card, n) -> n
Special energy may also set can_attach(g, mon) -> bool and discard_at_end_of_turn=True.
"""

ATTACKS = {}      # (card name, attack name) -> fn(g, ctx)
ABILITIES = {}    # (card name, ability name) -> AbilityImpl
TRAINERS = {}     # card name -> TrainerImpl (Items, Supporters, Stadiums, Tools)
ENERGY = {}       # special energy name -> EnergyImpl


class AbilityImpl:
    """kind: 'activated' (once during your turn, you may...), 'on_evolve', 'on_bench', or 'passive'.
    once: 'pokemon' (each copy once per turn), 'name' (only 1 of that name per turn), or None."""

    def __init__(self, kind, use=None, can_use=None, hooks=None, once="pokemon", optional=True):
        self.kind = kind
        self.use = use
        self.can_use = can_use or (lambda g, p, mon: True)
        self.hooks = hooks or {}
        self.once = once
        self.optional = optional


class TrainerImpl:
    def __init__(self, play=None, can_play=None, hooks=None, use=None, can_use=None, can_attach=None,
                 first_turn_ok=False):
        self.play = play
        self.first_turn_ok = first_turn_ok
        self.can_play = can_play or (lambda g, p: True)
        self.hooks = hooks or {}
        self.use = use                    # Stadiums with "once during each player's turn"
        self.can_use = can_use or (lambda g, p: True)
        self.can_attach = can_attach or (lambda g, p, mon: True)  # Tools


class EnergyImpl:
    def __init__(self, provides, hooks=None, on_attach=None):
        self.provides = provides          # fn(g, mon) -> list of frozensets, one per unit of energy
        self.hooks = hooks or {}
        self.on_attach = on_attach        # fn(g, player, mon), generator allowed


def attack(card_name, attack_name):
    def deco(fn):
        ATTACKS[(card_name, attack_name)] = fn
        return fn
    return deco


def ability(card_name, ability_name, kind="activated", can_use=None, once="pokemon", optional=True):
    def deco(fn):
        ABILITIES[(card_name, ability_name)] = AbilityImpl(kind, fn, can_use, None, once, optional)
        return fn
    return deco


def passive(card_name, ability_name, **hooks):
    ABILITIES[(card_name, ability_name)] = AbilityImpl("passive", hooks=hooks, once=None)


def trainer(name, can_play=None, first_turn_ok=False):
    def deco(fn):
        TRAINERS[name] = TrainerImpl(play=fn, can_play=can_play, first_turn_ok=first_turn_ok)
        return fn
    return deco


def tool(name, can_attach=None, **hooks):
    TRAINERS[name] = TrainerImpl(hooks=hooks, can_attach=can_attach)


def stadium(name, use=None, can_use=None, **hooks):
    TRAINERS[name] = TrainerImpl(hooks=hooks, use=use, can_use=can_use)


def energy(name, provides, on_attach=None, **hooks):
    ENERGY[name] = EnergyImpl(provides, hooks, on_attach)
