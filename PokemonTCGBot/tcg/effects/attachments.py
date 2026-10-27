"""Hand-written Pokemon Tools, Stadiums and special Energy."""
from .registry import tool, stadium, energy
from ..carddb import COLORLESS, PSYCHIC, GRASS, DARKNESS, card as by_name

C = frozenset((COLORLESS,))


def _on(src, mon):
    return src is mon


# -------------------------------------------------------------------- Tools
tool("Air Balloon", retreat_cost=lambda g, src, mon, cost: cost - 2 if _on(src, mon) else cost)
tool("Hero's Cape", max_hp=lambda g, src, mon, hp: hp + 100 if _on(src, mon) else hp)
tool("Brave Bangle", attack_damage=lambda g, src, ctx, target, dmg:
     dmg + 30 if src is ctx.attacker and not src.card.rule_box and g.is_active(target)
     and (target.card.ex or target.card.mega) else dmg)
tool("Binding Mochi", attack_damage=lambda g, src, ctx, target, dmg:
     dmg + 40 if src is ctx.attacker and src.poisoned and g.is_active(target) else dmg)


def _counter_gain(g, src, mon, attack, cost):
    p = g.players[mon.owner]
    if src is mon and len(p.prizes) > len(g.opponent(p).prizes) and COLORLESS in cost:
        cost = list(cost)
        cost.remove(COLORLESS)
    return cost


tool("Counter Gain", attack_cost=_counter_gain)


# ----------------------------------------------------------------- Stadiums
def _has_tera(p):
    return any(m.card.tera for m in p.in_play())


stadium("Area Zero Underdepths", bench_limit=lambda g, src, p, n: 8 if _has_tera(p) else n)
stadium("Festival Grounds", condition_immune=lambda g, src, mon: bool(mon.energy))
stadium("N's Castle", retreat_cost=lambda g, src, mon, cost: 0 if mon.card.owner_tag == "N" else cost)
stadium("Gravity Mountain", max_hp=lambda g, src, mon, hp: hp - 30 if mon.card.stage == 2 else hp)
stadium("Team Rocket's Watchtower", ability_blocked=lambda g, src, mon: COLORLESS in mon.card.types)
stadium("Forest of Vitality", evolve_same_turn=lambda g, src, mon, card:
        GRASS in mon.card.types and GRASS in card.types and g.players[mon.owner].turns_taken > 0)


def _academy(g, p):
    c = (yield from g.choose_cards(p.idx, p.hand, 1, 1, by_name("Academy at Night"), prompt="put on top of deck"))[0]
    p.hand.remove(c)
    p.deck.append(c)


stadium("Academy at Night", use=_academy, can_use=lambda g, p: bool(p.hand))


def _prism(g, p):
    picked = yield from g.choose_cards(p.idx, p.hand, 2, 2, by_name("Prism Tower"), "discard")
    for c in picked:
        p.hand.remove(c)
        p.discard.append(c)
    g.draw(p, 1)


stadium("Prism Tower", use=_prism, can_use=lambda g, p: len(p.hand) >= 2)


def _lumiose(g, p):
    found = yield from g.search_deck(p.idx, lambda c: c.is_basic, 1, by_name("Lumiose City"))
    for c in found:
        g.put_on_bench(p, c)
    g.force_end_turn = True


stadium("Lumiose City", use=_lumiose, can_use=lambda g, p: len(p.bench) < g.bench_limit(p))
stadium("Team Rocket's Factory", use=lambda g, p: g.draw(p, 2),
        can_use=lambda g, p: p.marker("tr_supporter", g.turn))


# ------------------------------------------------------------ Special Energy
def _telepathic_attach(g, p, mon):
    if PSYCHIC not in mon.card.types:
        return
    room = g.bench_limit(p) - len(p.bench)
    pred = lambda c: c.is_basic and PSYCHIC in c.types
    for c in (yield from g.search_deck(p.idx, pred, min(2, room), by_name("Telepathic Psychic Energy"))):
        g.put_on_bench(p, c)


energy("Telepathic Psychic Energy", lambda g, mon: [frozenset((PSYCHIC,))], on_attach=_telepathic_attach)
energy("Boomerang Energy", lambda g, mon: [C])  # re-attaching is handled by the engine
energy("Enriching Energy", lambda g, mon: [C], on_attach=lambda g, p, mon: g.draw(p, 4))


def _jet_attach(g, p, mon):
    if mon in p.bench:
        g.switch_active(p, mon)


energy("Jet Energy", lambda g, mon: [C], on_attach=_jet_attach)
energy("Mist Energy", lambda g, mon: [C], no_effects=lambda g, src, mon, kind: src is mon and kind == "attack")


def _spiky(g, src, ctx, target, dmg):
    if target is src and g.is_active(src) and ctx.attacker.owner != src.owner and g.in_play(ctx.attacker):
        ctx.attacker.damage += 20


energy("Spiky Energy", lambda g, mon: [C], after_damaged=_spiky)
energy("Growing Grass Energy", lambda g, mon: [frozenset((GRASS,))],
       max_hp=lambda g, src, mon, hp: hp + 20 if src is mon and GRASS in mon.card.types else hp)


def _tr(mon):
    return mon.card.owner_tag == "Team Rocket"


energy("Team Rocket's Energy", lambda g, mon: [frozenset((PSYCHIC, DARKNESS))] * 2, can_attach=lambda g, mon: _tr(mon))
energy("Ignition Energy", lambda g, mon: [C] * (3 if mon.card.stage > 0 else 1), discard_at_end_of_turn=True)


def _risky_ruins(g, src, mon):
    if mon.card.stage == 0 and DARKNESS not in mon.card.types and not g.in_setup:
        mon.damage += 20


stadium("Risky Ruins", on_benched=_risky_ruins)
