"""Compile common attack text into effect functions.

The text is consumed clause by clause from the start; every clause regex
returns (phase, step). 'pre' steps run before the attack's damage (they can
change ctx.damage or cancel the attack), 'post' steps run after it, and a
'replace' step does the damage itself instead of the usual hit. If any part
of the text isn't recognised, compile_attack returns None and the attack is
reported as unimplemented (it still does its printed damage in play).
"""
import re

from ..carddb import TYPE, COLORLESS

N = r"(\d+)"
NUM = {"a": 1, "an": 1, "1": 1, "2": 2, "3": 3, "4": 4, "5": 5, "6": 6}
STATUS = r"(Paralyzed|Asleep|Confused|Poisoned|Burned)"
TYPES = r"(Grass|Fire|Water|Lightning|Psychic|Fighting|Darkness|Metal|Dragon|Colorless)"
NO_WR = r"\s*\(Don't apply Weakness and Resistance for Benched Pokémon\.\)"

CLAUSES = []


def clause(pattern):
    rx = re.compile(pattern)

    def deco(fn):
        CLAUSES.append((rx, fn))
        return fn
    return deco


def _status(mon, s):
    s = s.lower()
    if s in ("asleep", "confused", "paralyzed"):
        mon.set_rotation(s)
    else:
        setattr(mon, s, True)


def _is_ex(mon):
    return mon.card.ex or mon.card.mega


# ------------------------------------------------------------------ damage
def _set_or_add(m, count_fn, atk):
    """'does N damage for each X' replaces the damage; 'N more damage for each X' adds to it."""
    per = int(m.group(1))
    more = "more" in m.group(0).split("for each")[0]

    def step(g, ctx):
        k = count_fn(g, ctx)
        ctx.damage = ctx.damage + per * k if more else per * k
    return "pre", step


@clause(r"Flip (\d+) coins\. This attack does (\d+) (more )?damage for each heads\.")
def _(m, atk):
    n, per, more = int(m.group(1)), int(m.group(2)), m.group(3)

    def step(g, ctx):
        heads = sum(g.flip() for _ in range(n))
        ctx.damage = ctx.damage + per * heads if more else per * heads
    return "pre", step


@clause(r"Flip a coin until you get tails\. This attack does (\d+) (more )?damage for each heads\.")
def _(m, atk):
    per, more = int(m.group(1)), m.group(2)

    def step(g, ctx):
        heads = 0
        while g.flip():
            heads += 1
        ctx.damage = ctx.damage + per * heads if more else per * heads
    return "pre", step


@clause(r"Flip a coin for each Energy attached to this Pokémon\. This attack does (\d+) damage for each heads\.")
def _(m, atk):
    per = int(m.group(1))

    def step(g, ctx):
        ctx.damage = per * sum(g.flip() for _ in range(g.energy_count(ctx.attacker)))
    return "pre", step


@clause(r"Flip a coin\. If heads, this attack does (\d+) more damage\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        if g.flip():
            ctx.damage += n
    return "pre", step


@clause(r"Flip a coin\. If tails, this attack does nothing\.")
def _(m, atk):
    def step(g, ctx):
        if not g.flip():
            ctx.cancelled = True
    return "pre", step


@clause(r"This attack does (\d+) (more )?damage for each Energy attached to this Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: g.energy_count(ctx.attacker), atk)


@clause(r"This attack does (\d+) (more )?damage for each " + TYPES + r" Energy attached to this Pokémon\.")
def _(m, atk):
    t = TYPE[m.group(3)]
    return _set_or_add(m, lambda g, ctx: g.energy_count(ctx.attacker, t), atk)


@clause(r"This attack does (\d+) (more )?damage for each Energy attached to your opponent's Active Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: g.energy_count(ctx.defender), atk)


@clause(r"This attack does (\d+) (more )?damage for each Energy attached to both Active Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: g.energy_count(ctx.attacker) + g.energy_count(ctx.defender), atk)


@clause(r"This attack does (\d+) (more )?damage for each damage counter on this Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: ctx.attacker.damage // 10, atk)


@clause(r"This attack does (\d+) less damage for each damage counter on this Pokémon\.")
def _(m, atk):
    per = int(m.group(1))

    def step(g, ctx):
        ctx.damage = max(0, ctx.damage - per * (ctx.attacker.damage // 10))
    return "pre", step


@clause(r"This attack does (\d+) (more )?damage for each damage counter on your opponent's Active Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: ctx.defender.damage // 10, atk)


@clause(r"This attack does (\d+) (more )?damage for each of your opponent's Benched Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: len(ctx.opponent.bench), atk)


@clause(r"This attack does (\d+) (more )?damage for each of your Benched Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: len(ctx.player.bench), atk)


@clause(r"This attack does (\d+) (more )?damage for each Benched Pokémon \(both yours and your opponent's\)\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: len(ctx.player.bench) + len(ctx.opponent.bench), atk)


@clause(r"This attack does (\d+) (more )?damage for each of your Pokémon in play\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: len(ctx.player.in_play()), atk)


@clause(r"This attack does (\d+) (more )?damage for each Pokémon Tool attached to all of your Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: sum(x.tool is not None for x in ctx.player.in_play()), atk)


@clause(r"This attack does (\d+) (more )?damage for each Energy card in your discard pile\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: sum(c.is_energy for c in ctx.player.discard), atk)


@clause(r"This attack does (\d+) (more )?damage for each Prize card you have taken\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: 6 - len(ctx.player.prizes), atk)


@clause(r"This attack does (\d+) (more )?damage for each Prize card your opponent has taken\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: 6 - len(ctx.opponent.prizes), atk)


@clause(r"This attack does (\d+) (more )?damage for each card in your opponent's hand\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: len(ctx.opponent.hand), atk)


@clause(r"This attack does (\d+) (more )?damage for each card in your hand\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: len(ctx.player.hand), atk)


@clause(r"This attack does (\d+) (more )?damage for each Colorless in your opponent's Active Pokémon's Retreat Cost\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: g.retreat_cost(ctx.defender), atk)


def _bonus_if(pattern, cond):
    @clause(pattern)
    def _(m, atk):
        n = int(m.groups()[-1])

        def step(g, ctx):
            if cond(g, ctx, m):
                ctx.damage += n
        return "pre", step


_bonus_if(r"If your opponent's Active Pokémon is a Pokémon ex( or Pokémon V)?, this attack does (\d+) more damage\.",
          lambda g, ctx, m: _is_ex(ctx.defender))
_bonus_if(r"If your opponent's Active Pokémon already has any damage counters on it, this attack does (\d+) more damage\.",
          lambda g, ctx, m: ctx.defender.damage > 0)
_bonus_if(r"If any of your Pokémon were Knocked Out by damage from an attack during your opponent's last turn, "
          r"this attack does (\d+) more damage\.", lambda g, ctx, m: ctx.player.kod_turn == g.turn - 1)
_bonus_if(r"If your opponent's Active Pokémon is " + STATUS + r", this attack does (\d+) more damage\.",
          lambda g, ctx, m: getattr(ctx.defender, m.group(1).lower()))
_bonus_if(r"If your opponent's Active Pokémon is affected by a Special Condition, this attack does (\d+) more damage\.",
          lambda g, ctx, m: bool(ctx.defender.conditions))
_bonus_if(r"If your opponent's Active Pokémon is an Evolution Pokémon, this attack does (\d+) more damage\.",
          lambda g, ctx, m: ctx.defender.card.stage > 0)
_bonus_if(r"If your opponent's Active Pokémon is a Basic Pokémon, this attack does (\d+) more damage\.",
          lambda g, ctx, m: ctx.defender.card.stage == 0)
_bonus_if(r"If this Pokémon has a Pokémon Tool attached, this attack does (\d+) more damage\.",
          lambda g, ctx, m: ctx.attacker.tool is not None)
_bonus_if(r"If this Pokémon has any damage counters on it, this attack does (\d+) more damage\.",
          lambda g, ctx, m: ctx.attacker.damage > 0)
_bonus_if(r"If your Benched Pokémon have any damage counters on them, this attack does (\d+) more damage\.",
          lambda g, ctx, m: any(b.damage for b in ctx.player.bench))
_bonus_if(r"If your opponent has (\d+) or fewer Prize cards remaining, this attack does (\d+) more damage\.",
          lambda g, ctx, m: len(ctx.opponent.prizes) <= int(m.group(1)))
_bonus_if(r"If a Stadium is in play, this attack does (\d+) more damage\.",
          lambda g, ctx, m: g.stadium is not None)


_bonus_if(r"If this Pokémon moved from your Bench to the Active Spot this turn, this attack does (\d+) more damage\.",
          lambda g, ctx, m: ctx.attacker.moved_turn == g.turn)


@clause(r"This attack does (\d+) (more )?damage for each " + TYPES + r" Energy attached to all of your Pokémon\.")
def _(m, atk):
    t = TYPE[m.group(3)]
    return _set_or_add(m, lambda g, ctx: sum(g.energy_count(x, t) for x in ctx.player.in_play()), atk)


@clause(r"This attack does (\d+) (more )?damage for each Energy attached to all of your opponent's Pokémon\.")
def _(m, atk):
    return _set_or_add(m, lambda g, ctx: sum(g.energy_count(x) for x in ctx.opponent.in_play()), atk)


@clause(r"If this Pokémon has at least (\d+) extra Energy attached \(in addition to this attack's cost\), "
        r"this attack does (\d+) more damage\.")
def _(m, atk):
    extra, n = int(m.group(1)), int(m.group(2))

    def step(g, ctx):
        if g.energy_count(ctx.attacker) >= len(ctx.attack.cost) + extra:
            ctx.damage += n
    return "pre", step


@clause(r"This attack's damage isn't affected by Resistance\.")
def _(m, atk):
    def step(g, ctx):
        ctx.ignore_resistance = True
    return "pre", step


@clause(r"This attack's damage isn't affected by Weakness or Resistance\.")
def _(m, atk):
    def step(g, ctx):
        ctx.apply_wr = False
    return "pre", step


@clause(r"This attack's damage isn't affected by (Weakness or Resistance, or by )?any effects on your opponent's Active Pokémon\.")
def _(m, atk):
    no_wr = bool(m.group(1))

    def step(g, ctx):
        ctx.ignore_defender_effects = True
        if no_wr:
            ctx.apply_wr = False
    return "pre", step


@clause(r"Before doing damage, discard all Pokémon Tools from your opponent's Active Pokémon\.")
def _(m, atk):
    def step(g, ctx):
        if ctx.defender.tool:
            ctx.opponent.discard.append(ctx.defender.tool)
            ctx.defender.tool = None
    return "pre", step


# --------------------------------------------------------- targeted damage
@clause(r"This attack (also )?does (\d+) damage to (\d+|each) of your opponent's (Benched )?Pokémon\." + NO_WR)
def _(m, atk):
    also, n, count, bench_only = m.group(1), int(m.group(2)), m.group(3), m.group(4)

    def step(g, ctx):
        pool = ctx.opponent.bench if bench_only else ctx.opponent.in_play()
        if count == "each":
            targets = list(pool)
        else:
            targets = []
            for _ in range(int(count)):
                left = [t for t in pool if t not in targets]
                t = yield from g.choose_pokemon(ctx.player.idx, left, ctx.attacker.card, "damage target")
                if t is None:
                    break
                targets.append(t)
        for t in targets:
            ctx.snipe(t, n)
    return ("post" if also else "replace"), step


@clause(r"This attack (also )?does (\d+) damage to (\d+|each) of your Benched Pokémon\." + NO_WR)
def _(m, atk):
    n, count = int(m.group(2)), m.group(3)

    def step(g, ctx):
        if count == "each":
            targets = list(ctx.player.bench)
        else:
            t = yield from g.choose_pokemon(ctx.player.idx, ctx.player.bench, ctx.attacker.card, "damage your own")
            targets = [t] if t else []
        for t in targets:
            g.attack_damage(ctx, t, n, apply_wr=False)
    return "post", step


@clause(r"Put (\d+) damage counters on your opponent's Benched Pokémon in any way you like\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        yield from g.place_counters(ctx.player.idx, ctx.opponent.bench, n, ctx.attacker.card)
    return "post", step


@clause(r"Put (\d+) damage counters on your opponent's Pokémon in any way you like\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        yield from g.place_counters(ctx.player.idx, ctx.opponent.in_play(), n, ctx.attacker.card)
    return "post", step


@clause(r"This Pokémon also does (\d+) damage to itself\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        ctx.attacker.damage += n
    return "post", step


# ------------------------------------------------------------- conditions
@clause(r"(Flip a coin\. If heads, y|Y)our opponent's Active Pokémon is now " + STATUS +
        r"(?:,? and " + STATUS + r")?\.")
def _(m, atk):
    coin, s1, s2 = m.group(1).startswith("Flip"), m.group(2), m.group(3)

    def step(g, ctx):
        if ctx.defender is None or not g.in_play(ctx.defender) or (coin and not g.flip()):
            return
        g.apply_condition(ctx.defender, s1.lower(), ctx.player.idx)
        if s2:
            g.apply_condition(ctx.defender, s2.lower(), ctx.player.idx)
    return "post", step


@clause(r"This Pokémon is now " + STATUS + r"\.")
def _(m, atk):
    s = m.group(1)

    def step(g, ctx):
        _status(ctx.attacker, s)
    return "post", step


# ---------------------------------------------------------------- markers
def _marker_clause(pattern, who, key, turns, coin=False):
    @clause(pattern)
    def _(m, atk):
        k = key(m) if callable(key) else key

        def step(g, ctx):
            if coin and not g.flip():
                return
            mon = ctx.attacker if who == "self" else ctx.defender
            if mon is not None:
                mon.markers[k] = g.turn + turns
        return "post", step


_marker_clause(r"During your next turn, this Pokémon can't (attack|use attacks)\.", "self", "no_attack", 2)
_marker_clause(r"During your next turn, this Pokémon can't use ([^.]+?)\.", "self", lambda m: "no_attack:" + m.group(1), 2)
_marker_clause(r"During your opponent's next turn, (the Defending|that) Pokémon can't retreat\.", "opp", "no_retreat", 1)
_marker_clause(r"During your opponent's next turn, the Defending Pokémon can't (attack|use attacks)\.", "opp", "no_attack", 1)
_marker_clause(r"During your opponent's next turn, attacks used by the Defending Pokémon do (\d+) less damage "
               r"\(before applying Weakness and Resistance\)\.", "opp", lambda m: "deal_less:" + m.group(1), 1)
_marker_clause(r"During your opponent's next turn, this Pokémon takes (\d+) less damage from attacks "
               r"\(after applying Weakness and Resistance\)\.", "self", lambda m: "take_less:" + m.group(1), 1)
_marker_clause(r"During your next turn, attacks used by this Pokémon do (\d+) more damage to your opponent's Active "
               r"Pokémon \(before applying Weakness and Resistance\)\.", "self", lambda m: "deal_more:" + m.group(1), 2)
_marker_clause(r"Flip a coin\. If heads, during your opponent's next turn, prevent all damage from and effects of "
               r"attacks done to this Pokémon\.", "self", "prevent_all", 1, coin=True)
_marker_clause(r"Flip a coin\. If heads, during your opponent's next turn, prevent all damage done to this Pokémon "
               r"by attacks\.", "self", "prevent_all", 1, coin=True)
_marker_clause(r"During your opponent's next turn, prevent all damage done to this Pokémon by attacks from Basic "
               r"Pokémon\.", "self", "prevent_basic", 1)


@clause(r"During your opponent's next turn, they can't play any Item cards from their hand\.")
def _(m, atk):
    def step(g, ctx):
        ctx.opponent.markers["no_items"] = g.turn + 1
    return "post", step


# ------------------------------------------------------------ healing etc.
@clause(r"Heal (\d+) damage from this Pokémon\.")
def _(m, atk):
    n = int(m.group(1))
    return "post", lambda g, ctx: g.heal(ctx.attacker, n)


@clause(r"Heal (\d+) damage from each of your Pokémon\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        for x in ctx.player.in_play():
            g.heal(x, n)
    return "post", step


@clause(r"Heal (\d+) damage from 1 of your Pokémon\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        t = yield from g.choose_pokemon(ctx.player.idx, ctx.player.in_play(), ctx.attacker.card, "heal")
        g.heal(t, n)
    return "post", step


@clause(r"Heal from this Pokémon the same amount of damage you did to your opponent's Active Pokémon\.")
def _(m, atk):
    return "post", lambda g, ctx: g.heal(ctx.attacker, ctx.dealt)


# ---------------------------------------------------------- cards & energy
@clause(r"Draw (a card|(\d+) cards)\.")
def _(m, atk):
    n = int(m.group(2) or 1)
    return "post", lambda g, ctx: g.draw(ctx.player, n)


@clause(r"You may draw cards until you have (\d+) cards in your hand\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        if len(ctx.player.hand) < n and (yield from g.yes_no(ctx.player.idx, ctx.attacker.card, "draw?")):
            g.draw(ctx.player, n - len(ctx.player.hand))
    return "post", step


@clause(r"Shuffle your hand into your deck\. Then, draw (\d+) cards\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        p = ctx.player
        p.deck += p.hand
        p.hand = []
        g.shuffle(p)
        g.draw(p, n)
    return "post", step


@clause(r"Discard (an|\d+) Energy from this Pokémon\.")
def _(m, atk):
    n = NUM.get(m.group(1), None) or int(m.group(1))

    def step(g, ctx):
        picked = yield from g.choose_energy(ctx.player.idx, ctx.attacker, n, ctx.attacker.card, prompt="discard")
        g.discard_energy(ctx.attacker, picked)
    return "post", step


@clause(r"Discard (an|\d+) " + TYPES + r" Energy from this Pokémon\.")
def _(m, atk):
    n = NUM.get(m.group(1), None) or int(m.group(1))
    t = TYPE[m.group(2)]

    def step(g, ctx):
        pred = lambda e: t in _provides(g, ctx.attacker, e)
        picked = yield from g.choose_energy(ctx.player.idx, ctx.attacker, n, ctx.attacker.card, pred, "discard")
        g.discard_energy(ctx.attacker, picked)
    return "post", step


@clause(r"Discard all Energy from this Pokémon\.")
def _(m, atk):
    return "post", lambda g, ctx: g.discard_energy(ctx.attacker, list(ctx.attacker.energy))


@clause(r"Discard all " + TYPES + r" Energy from this Pokémon\.")
def _(m, atk):
    t = TYPE[m.group(1)]

    def step(g, ctx):
        g.discard_energy(ctx.attacker, [e for e in ctx.attacker.energy if t in _provides(g, ctx.attacker, e)])
    return "post", step


@clause(r"(Flip a coin\. If heads, d|D)iscard an Energy from your opponent's Active Pokémon\.")
def _(m, atk):
    coin = m.group(1).startswith("Flip")

    def step(g, ctx):
        d = ctx.defender
        if d is None or not g.in_play(d) or not d.energy or (coin and not g.flip()):
            return
        picked = yield from g.choose_energy(ctx.player.idx, d, 1, ctx.attacker.card, prompt="discard")
        g.discard_energy(d, picked)
    return "post", step


@clause(r"Flip (\d+) coins\. For each heads, discard an Energy from your opponent's Active Pokémon\.")
def _(m, atk):
    n = int(m.group(1))

    def step(g, ctx):
        heads = sum(g.flip() for _ in range(n))
        d = ctx.defender
        if heads and d is not None and g.in_play(d):
            picked = yield from g.choose_energy(ctx.player.idx, d, heads, ctx.attacker.card, prompt="discard")
            g.discard_energy(d, picked)
    return "post", step


@clause(r"Discard a Special Energy from your opponent's Active Pokémon\.")
def _(m, atk):
    def step(g, ctx):
        d = ctx.defender
        if d is not None and g.in_play(d):
            picked = yield from g.choose_energy(ctx.player.idx, d, 1, ctx.attacker.card,
                                                lambda e: not e.basic_energy, "discard")
            g.discard_energy(d, picked)
    return "post", step


@clause(r"Discard a random card from your opponent's hand\.")
def _(m, atk):
    def step(g, ctx):
        h = ctx.opponent.hand
        if h:
            ctx.opponent.discard.append(h.pop(g.rng.randrange(len(h))))
    return "post", step


@clause(r"Discard the top (card|(\d+) cards) of your opponent's deck\.")
def _(m, atk):
    n = int(m.group(2) or 1)

    def step(g, ctx):
        o = ctx.opponent
        for _ in range(min(n, len(o.deck))):
            o.discard.append(o.deck.pop())
    return "post", step


@clause(r"Discard the top (card|(\d+) cards) of your deck\.")
def _(m, atk):
    n = int(m.group(2) or 1)

    def step(g, ctx):
        p = ctx.player
        for _ in range(min(n, len(p.deck))):
            p.discard.append(p.deck.pop())
    return "post", step


@clause(r"Your opponent reveals their hand\.")
def _(m, atk):
    return "post", lambda g, ctx: None


@clause(r"Discard a Stadium in play\.")
def _(m, atk):
    def step(g, ctx):
        if g.stadium is not None:
            g.discard_stadium()
    return "post", step


@clause(r"Put an Energy attached to this Pokémon into your hand\.")
def _(m, atk):
    def step(g, ctx):
        picked = yield from g.choose_energy(ctx.player.idx, ctx.attacker, 1, ctx.attacker.card)
        for e in picked:
            ctx.attacker.energy.remove(e)
            ctx.player.hand.append(e)
    return "post", step


@clause(r"Put this Pokémon and all attached cards into your hand\.")
def _(m, atk):
    def step(g, ctx):
        mon = ctx.attacker
        if g.in_play(mon) and len(ctx.player.in_play()) > 1:
            g.discard_pokemon(mon)
            for c in mon.cards + mon.energy + ([mon.tool] if mon.tool else []):
                ctx.player.discard.remove(c)
                ctx.player.hand.append(c)
    return "post", step


@clause(r"Move an Energy from this Pokémon to 1 of your Benched Pokémon\.")
def _(m, atk):
    def step(g, ctx):
        if not ctx.player.bench or not ctx.attacker.energy:
            return
        e = (yield from g.choose_energy(ctx.player.idx, ctx.attacker, 1, ctx.attacker.card))[0]
        t = yield from g.choose_pokemon(ctx.player.idx, ctx.player.bench, ctx.attacker.card, "move energy to")
        ctx.attacker.energy.remove(e)
        t.energy.append(e)
    return "post", step


# --------------------------------------------------------------- searching
@clause(r"Search your deck for (a|up to (\d+)) Basic Pokémon and put (it|them) onto your Bench\. Then, shuffle your deck\.")
def _(m, atk):
    n = int(m.group(2) or 1)

    def step(g, ctx):
        p = ctx.player
        room = g.bench_limit(p) - len(p.bench)
        for c in (yield from g.search_deck(p.idx, lambda c: c.is_basic, min(n, room), ctx.attacker.card)):
            g.put_on_bench(p, c)
    return "post", step


@clause(r"Search your deck for (a|up to (\d+)) (Basic Energy cards?|Pokémon|Item cards?|Supporter cards?|cards?|"
        r"Basic " + TYPES + r" Energy cards?|Trainer cards?)(, reveal (it|them),)? and put (it|them) into your hand\. "
        r"Then, shuffle your deck\.")
def _(m, atk):
    n = int(m.group(2) or 1)
    what = m.group(3)
    pred = _card_filter(what)

    def step(g, ctx):
        ctx.player.hand += yield from g.search_deck(ctx.player.idx, pred, n, ctx.attacker.card)
    return "post", step


@clause(r"Search your deck for (a|up to (\d+)) Basic (" + TYPES[1:-1] + r" )?Energy cards? and attach (it|them) to "
        r"(this Pokémon|1 of your Pokémon|your Pokémon in any way you like|1 of your Benched Pokémon|"
        r"your Benched Pokémon in any way you like)\. Then, shuffle your deck\.")
def _(m, atk):
    n = int(m.group(2) or 1)
    t = TYPE[m.group(3).strip()] if m.group(3) else None
    where = m.group(5)

    def step(g, ctx):
        p = ctx.player
        pred = (lambda c: c.basic_energy) if t is None else (lambda c: c.basic_energy and c.energy_type == t)
        picked = yield from g.search_deck(p.idx, pred, n, ctx.attacker.card)
        targets = p.bench if "Bench" in where else p.in_play()
        fixed = ctx.attacker if where == "this Pokémon" else None
        one = None
        for e in picked:
            mon = fixed
            if mon is None and where.startswith("1 of"):
                if one is None:
                    one = yield from g.choose_pokemon(p.idx, targets, ctx.attacker.card, "attach to")
                mon = one
            if mon is None:
                mon = yield from g.choose_pokemon(p.idx, targets, ctx.attacker.card, "attach to")
            if mon is None:
                p.discard.append(e)
                continue
            mon.energy.append(e)
    return "post", step


@clause(r"Search your deck for a card that evolves from this Pokémon and put it onto this Pokémon to evolve it\. "
        r"Then, shuffle your deck\.")
def _(m, atk):
    def step(g, ctx):
        mon = ctx.attacker
        found = yield from g.search_deck(ctx.player.idx, lambda c: c.is_pokemon and c.evolves_from == mon.name,
                                         1, ctx.attacker.card)
        if found:
            g.evolve(mon, found[0])
    return "post", step


# ----------------------------------------------------------------- switching
@clause(r"(You may s|S)witch this Pokémon with 1 of your Benched Pokémon\.")
def _(m, atk):
    optional = m.group(1).startswith("You may")

    def step(g, ctx):
        p = ctx.player
        if not p.bench or not g.in_play(ctx.attacker) or not g.is_active(ctx.attacker):
            return
        t = yield from g.choose_pokemon(p.idx, p.bench, ctx.attacker.card, "switch to", optional=optional)
        if t is not None:
            g.switch_active(p, t)
    return "post", step


@clause(r"Switch out your opponent's Active Pokémon to the Bench\. \(Your opponent chooses the new Active Pokémon\.\)")
def _(m, atk):
    def step(g, ctx):
        o = ctx.opponent
        if o.bench and o.active is not None:
            t = yield from g.choose_pokemon(o.idx, o.bench, ctx.attacker.card, "new Active")
            g.switch_active(o, t)
    return "post", step


@clause(r"Switch in 1 of your opponent's Benched Pokémon to the Active Spot\.( This attack does (\d+) damage to the "
        r"new Active Pokémon\.)?")
def _(m, atk):
    dmg = int(m.group(2)) if m.group(2) else None

    def step(g, ctx):
        o = ctx.opponent
        if o.bench:
            t = yield from g.choose_pokemon(ctx.player.idx, o.bench, ctx.attacker.card, "gust")
            g.switch_active(o, t)
            ctx.defender = o.active
        if dmg is not None:
            ctx.damage = dmg
    return "pre", step


# ====================================================================== API
def _card_filter(what):
    what = what.rstrip("s") if what.endswith("cards") else what
    if what.startswith("Basic Energy"):
        return lambda c: c.basic_energy
    if what.startswith("Basic ") and "Energy" in what:
        t = TYPE[what.split()[1]]
        return lambda c: c.basic_energy and c.energy_type == t
    if what == "Pokémon":
        return lambda c: c.is_pokemon
    if what.startswith("Item"):
        return lambda c: c.is_item
    if what.startswith("Supporter"):
        return lambda c: c.is_supporter
    if what.startswith("Trainer"):
        return lambda c: c.is_trainer
    return lambda c: True


def _provides(g, mon, e):
    if e.basic_energy:
        return {e.energy_type}
    from . import ENERGY
    impl = ENERGY.get(e.name)
    units = impl.provides(g, mon) if impl else [frozenset((COLORLESS,))]
    return set().union(*units) if units else set()


def parse(text):
    """Split text into clause steps; None if any part isn't understood."""
    rest = text.strip()
    steps = []
    while rest:
        for rx, build in CLAUSES:
            m = rx.match(rest)
            if m:
                steps.append(build(m, None))
                rest = rest[m.end():].lstrip()
                break
        else:
            return None
    return steps


def compile_attack(card, atk):
    steps = parse(atk.text) if atk.text else []
    if steps is None:
        return None
    pre = [s for ph, s in steps if ph == "pre"]
    replace = [s for ph, s in steps if ph == "replace"]
    post = [s for ph, s in steps if ph == "post"]

    def fn(g, ctx):
        from ..engine import run
        for s in pre:
            yield from run(s(g, ctx))
            if ctx.cancelled:
                return
        if replace:
            for s in replace:
                yield from run(s(g, ctx))
        else:
            ctx.hit()
        for s in post:
            yield from run(s(g, ctx))
    fn.template = True
    return fn
