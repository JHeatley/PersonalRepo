"""Hand-written Items and Supporters."""
from .registry import trainer
from ..carddb import PSYCHIC, FIGHTING, GRASS, card as by_name

SOURCE = {}


def src(name):
    """The CardDef for a trainer, used as the 'source' of its decisions."""
    if name not in SOURCE:
        SOURCE[name] = by_name(name)
    return SOURCE[name]


# ------------------------------------------------------------------ helpers
def gust(g, p, source):
    o = g.opponent(p)
    t = yield from g.choose_pokemon(p.idx, o.bench, source, "bring to the Active Spot")
    if t is not None:
        g.switch_active(o, t)
        return True
    return False


def switch_own(g, p, source, optional=False):
    t = yield from g.choose_pokemon(p.idx, p.bench, source, "new Active Pokemon", optional=optional)
    if t is not None:
        g.switch_active(p, t)


def shuffle_hand_in(g, p):
    p.deck += p.hand
    p.hand = []
    g.shuffle(p)


def take_from_discard(g, p, pred, max_n, source, min_n=0):
    pool = [c for c in p.discard if pred(c)]
    picked = yield from g.choose_cards(p.idx, pool, min(min_n, len(pool)), max_n, source)
    for c in picked:
        p.discard.remove(c)
    return picked


def look_at_top(g, p, n, pred, max_n, source):
    """Look at the top n cards, take up to max_n matching ones, shuffle the rest back."""
    top = p.deck[-n:]
    del p.deck[-n:]
    picked = yield from g.choose_cards(p.idx, [c for c in top if pred(c)], 0, max_n, source)
    for c in picked:
        top.remove(c)
    p.deck += top
    g.shuffle(p)
    return picked


def discard_from_hand(g, p, n, source, pred=None):
    pool = [c for c in p.hand if pred is None or pred(c)]
    picked = yield from g.choose_cards(p.idx, pool, n, n, source, "discard")
    for c in picked:
        p.hand.remove(c)
        p.discard.append(c)
    return picked


def non_rule_box_pokemon(c):
    return c.is_pokemon and not c.rule_box


def ko_last_turn(g, p):
    return p.kod_turn == g.turn - 1


# --------------------------------------------------------------- Supporters
@trainer("Lillie's Determination")
def _(g, p):
    n = 8 if len(p.prizes) == 6 else 6
    shuffle_hand_in(g, p)
    g.draw(p, n)


@trainer("Judge")
def _(g, p):
    for q in g.players:
        shuffle_hand_in(g, q)
        g.draw(q, 4)


@trainer("Boss's Orders", can_play=lambda g, p: bool(g.opponent(p).bench))
def _(g, p):
    yield from gust(g, p, src("Boss's Orders"))


@trainer("Cyrano")
def _(g, p):
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_pokemon and c.ex, 3, src("Cyrano"))


@trainer("Ciphermaniac's Codebreaking")
def _(g, p):
    s = src("Ciphermaniac's Codebreaking")
    picked = yield from g.search_deck(p.idx, lambda c: True, 2, s, min_n=2)
    if len(picked) == 2:
        top = (yield from g.choose_cards(p.idx, picked, 1, 1, s, prompt="card to put on top"))[0]
        picked.remove(top)
        p.deck += picked + [top]
    else:
        p.deck += picked


@trainer("Crispin")
def _(g, p):
    s = src("Crispin")
    first = yield from g.search_deck(p.idx, lambda c: c.basic_energy, 1, s)
    if not first:
        return
    second = yield from g.search_deck(p.idx, lambda c: c.basic_energy and c.energy_type != first[0].energy_type, 1, s)
    picked = first + second
    if len(picked) == 1:
        p.hand += picked
        return
    to_hand = (yield from g.choose_cards(p.idx, picked, 1, 1, s, prompt="put into your hand"))[0]
    picked.remove(to_hand)
    p.hand.append(to_hand)
    target = yield from g.choose_pokemon(p.idx, p.in_play(), s, "attach energy to")
    target.energy.append(picked[0])


@trainer("Dawn")
def _(g, p):
    s = src("Dawn")
    for stage in (0, 1, 2):
        p.hand += yield from g.search_deck(p.idx, lambda c, st=stage: c.is_pokemon and c.stage == st, 1, s)


@trainer("Hilda")
def _(g, p):
    s = src("Hilda")
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_pokemon and c.stage > 0, 1, s)
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_energy, 1, s)


@trainer("Team Rocket's Petrel")
def _(g, p):
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_trainer, 1, src("Team Rocket's Petrel"))


@trainer("Eri")
def _(g, p):
    o = g.opponent(p)
    s = src("Eri")
    picked = yield from g.choose_cards(p.idx, [c for c in o.hand if c.is_item], 0, 2, s, "discard")
    for c in picked:
        o.hand.remove(c)
        o.discard.append(c)


@trainer("Xerosic's Machinations")
def _(g, p):
    o = g.opponent(p)
    n = len(o.hand) - 3
    if n > 0:
        yield from discard_from_hand(g, o, n, src("Xerosic's Machinations"))


@trainer("Pokémon Center Lady")
def _(g, p):
    t = yield from g.choose_pokemon(p.idx, p.in_play(), src("Pokémon Center Lady"), "heal")
    g.heal(t, 60)
    t.clear_conditions()


@trainer("Gladion's Final Battle", can_play=lambda g, p: len(p.hand) == 1)
def _(g, p):
    p.markers["bonus_nonrule:80"] = g.turn


@trainer("Black Belt's Training")
def _(g, p):
    p.markers["bonus_vs_ex:40"] = g.turn


@trainer("Briar", can_play=lambda g, p: len(g.opponent(p).prizes) == 2)
def _(g, p):
    p.markers["briar"] = g.turn


@trainer("Lana's Aid")
def _(g, p):
    p.hand += yield from take_from_discard(
        g, p, lambda c: non_rule_box_pokemon(c) or c.basic_energy, 3, src("Lana's Aid"))


@trainer("Gwynn", can_play=lambda g, p: any(non_rule_box_pokemon(c) for c in p.hand))
def _(g, p):
    picked = yield from g.choose_cards(p.idx, [c for c in p.hand if non_rule_box_pokemon(c)], 0, 2,
                                       src("Gwynn"), "discard")
    for c in picked:
        p.hand.remove(c)
        p.discard.append(c)
    g.draw(p, 3 * len(picked))


# -------------------------------------------------------------------- Items
@trainer("Ultra Ball", can_play=lambda g, p: len(p.hand) >= 3)
def _(g, p):
    s = src("Ultra Ball")
    yield from discard_from_hand(g, p, 2, s)
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_pokemon, 1, s)


@trainer("Poké Pad")
def _(g, p):
    p.hand += yield from g.search_deck(p.idx, non_rule_box_pokemon, 1, src("Poké Pad"))


@trainer("Buddy-Buddy Poffin", can_play=lambda g, p: len(p.bench) < g.bench_limit(p))
def _(g, p):
    room = g.bench_limit(p) - len(p.bench)
    for c in (yield from g.search_deck(p.idx, lambda c: c.is_basic and c.hp <= 70, min(2, room),
                                       src("Buddy-Buddy Poffin"))):
        g.put_on_bench(p, c)


@trainer("Night Stretcher", can_play=lambda g, p: any(c.is_pokemon or c.basic_energy for c in p.discard))
def _(g, p):
    p.hand += yield from take_from_discard(g, p, lambda c: c.is_pokemon or c.basic_energy, 1,
                                           src("Night Stretcher"), min_n=1)


def _candy_targets(g, p):
    if p.turns_taken == 0:
        return []
    out = []
    for m in p.in_play():
        if m.card.stage == 0 and m.turn_entered < g.turn:
            for c in p.hand:
                if c.is_pokemon and c.stage == 2:
                    mid = [x for x in _stage1_names(c)]
                    if m.name in mid:
                        out.append((m, c))
    return out


_STAGE1_CACHE = {}


def _stage1_names(stage2):
    """Names of Basics that this Stage 2 evolves from (via its Stage 1)."""
    if stage2.id not in _STAGE1_CACHE:
        from ..carddb import BY_NAME
        basics = set()
        for mid in BY_NAME.get(stage2.evolves_from, []):
            if mid.evolves_from:
                basics.add(mid.evolves_from)
        _STAGE1_CACHE[stage2.id] = basics
    return _STAGE1_CACHE[stage2.id]


@trainer("Rare Candy", can_play=lambda g, p: bool(_candy_targets(g, p)))
def _(g, p):
    s = src("Rare Candy")
    pairs = _candy_targets(g, p)
    mon = yield from g.choose_pokemon(p.idx, list({id(m): m for m, _ in pairs}.values()), s, "evolve")
    stage2s = [c for m, c in pairs if m is mon]
    c = (yield from g.choose_cards(p.idx, stage2s, 1, 1, s))[0]
    p.hand.remove(c)
    g.say(f"  {mon.name} evolves into {c.name}")
    g.evolve(mon, c)
    yield from g._trigger(p, mon, "on_evolve")


@trainer("Switch", can_play=lambda g, p: bool(p.bench))
def _(g, p):
    yield from switch_own(g, p, src("Switch"))


@trainer("Prime Catcher", can_play=lambda g, p: bool(g.opponent(p).bench))
def _(g, p):
    s = src("Prime Catcher")
    if (yield from gust(g, p, s)) and p.bench:
        yield from switch_own(g, p, s)


@trainer("Energy Switch", can_play=lambda g, p: len(p.in_play()) > 1 and
         any(e.basic_energy for m in p.in_play() for e in m.energy))
def _(g, p):
    s = src("Energy Switch")
    frm = yield from g.choose_pokemon(p.idx, [m for m in p.in_play() if any(e.basic_energy for e in m.energy)],
                                      s, "move energy from")
    e = (yield from g.choose_energy(p.idx, frm, 1, s, lambda e: e.basic_energy))[0]
    to = yield from g.choose_pokemon(p.idx, [m for m in p.in_play() if m is not frm], s, "move energy to")
    frm.energy.remove(e)
    to.energy.append(e)


@trainer("Unfair Stamp", can_play=ko_last_turn)
def _(g, p):
    o = g.opponent(p)
    shuffle_hand_in(g, p)
    shuffle_hand_in(g, o)
    g.draw(p, 5)
    g.draw(o, 2)


@trainer("Special Red Card", can_play=lambda g, p: len(g.opponent(p).prizes) <= 3)
def _(g, p):
    o = g.opponent(p)
    if o.hand:
        g.rng.shuffle(o.hand)
        o.deck[:0] = o.hand
        o.hand = []
        g.draw(o, 3)


@trainer("Secret Box", can_play=lambda g, p: len(p.hand) >= 4)
def _(g, p):
    s = src("Secret Box")
    yield from discard_from_hand(g, p, 3, s)
    for pred in (lambda c: c.is_item, lambda c: c.is_tool, lambda c: c.is_supporter, lambda c: c.is_stadium):
        p.hand += yield from g.search_deck(p.idx, pred, 1, s)


@trainer("Pokégear 3.0", can_play=lambda g, p: bool(p.deck))
def _(g, p):
    p.hand += yield from look_at_top(g, p, 7, lambda c: c.is_supporter, 1, src("Pokégear 3.0"))


@trainer("Bug Catching Set", can_play=lambda g, p: bool(p.deck))
def _(g, p):
    pred = lambda c: (c.is_pokemon and GRASS in c.types) or (c.basic_energy and c.energy_type == GRASS)
    p.hand += yield from look_at_top(g, p, 7, pred, 2, src("Bug Catching Set"))


@trainer("Sacred Ash", can_play=lambda g, p: any(c.is_pokemon for c in p.discard))
def _(g, p):
    picked = yield from take_from_discard(g, p, lambda c: c.is_pokemon, 5, src("Sacred Ash"))
    p.deck += picked
    g.shuffle(p)


def _benched(p, pred):
    return [m for m in p.bench if pred(m)]


@trainer("Wondrous Patch", can_play=lambda g, p: any(c.basic_energy and c.energy_type == PSYCHIC for c in p.discard)
         and bool(_benched(p, lambda m: PSYCHIC in m.card.types)))
def _(g, p):
    s = src("Wondrous Patch")
    e = next(c for c in p.discard if c.basic_energy and c.energy_type == PSYCHIC)
    t = yield from g.choose_pokemon(p.idx, _benched(p, lambda m: PSYCHIC in m.card.types), s, "attach to")
    p.discard.remove(e)
    t.energy.append(e)


@trainer("N's PP Up", can_play=lambda g, p: any(c.basic_energy for c in p.discard)
         and bool(_benched(p, lambda m: m.card.owner_tag == "N")))
def _(g, p):
    s = src("N's PP Up")
    e = (yield from g.choose_cards(p.idx, [c for c in p.discard if c.basic_energy], 1, 1, s))[0]
    t = yield from g.choose_pokemon(p.idx, _benched(p, lambda m: m.card.owner_tag == "N"), s, "attach to")
    p.discard.remove(e)
    t.energy.append(e)


@trainer("Fighting Gong")
def _(g, p):
    pred = lambda c: (c.basic_energy and c.energy_type == FIGHTING) or (c.is_basic and FIGHTING in c.types)
    p.hand += yield from g.search_deck(p.idx, pred, 1, src("Fighting Gong"))


@trainer("Premium Power Pro")
def _(g, p):
    p.markers[f"bonus_type_{FIGHTING}:30"] = g.turn


@trainer("Jumbo Ice Cream", can_play=lambda g, p: p.active.damage > 0 and g.energy_count(p.active) >= 3)
def _(g, p):
    g.heal(p.active, 80)


# -------------------------------------------------------------- Team Rocket
def _tr(c):
    return c.name.startswith("Team Rocket's")


@trainer("Team Rocket's Ariana")
def _(g, p):
    target = 8 if all(m.card.owner_tag == "Team Rocket" for m in p.in_play()) else 5
    g.draw(p, target - len(p.hand))


@trainer("Team Rocket's Archer", can_play=lambda g, p: p.kod_tr_turn == g.turn - 1)
def _(g, p):
    o = g.opponent(p)
    shuffle_hand_in(g, p)
    shuffle_hand_in(g, o)
    g.draw(p, 5)
    g.draw(o, 3)


@trainer("Team Rocket's Proton", first_turn_ok=True)
def _(g, p):
    pred = lambda c: c.is_basic and c.owner_tag == "Team Rocket"
    p.hand += yield from g.search_deck(p.idx, pred, 3, src("Team Rocket's Proton"))


def _giovanni_ok(g, p):
    return p.active.card.owner_tag == "Team Rocket" and any(m.card.owner_tag == "Team Rocket" for m in p.bench)


@trainer("Team Rocket's Giovanni", can_play=_giovanni_ok)
def _(g, p):
    s = src("Team Rocket's Giovanni")
    t = yield from g.choose_pokemon(p.idx, [m for m in p.bench if m.card.owner_tag == "Team Rocket"], s, "switch to")
    g.switch_active(p, t)
    if g.opponent(p).bench:
        yield from gust(g, p, s)


@trainer("Team Rocket's Transceiver")
def _(g, p):
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_supporter and _tr(c), 1, src("Team Rocket's Transceiver"))


@trainer("Roto-Stick", can_play=lambda g, p: bool(p.deck))
def _(g, p):
    p.hand += yield from look_at_top(g, p, 4, lambda c: c.is_supporter, 4, src("Roto-Stick"))


@trainer("Miracle Headset", can_play=lambda g, p: any(c.is_supporter for c in p.discard))
def _(g, p):
    p.hand += yield from take_from_discard(g, p, lambda c: c.is_supporter, 2, src("Miracle Headset"))


def _rosa_ok(g, p):
    return len(p.prizes) > len(g.opponent(p).prizes) and any(m.card.stage == 2 for m in p.in_play()) \
        and any(c.basic_energy for c in p.discard)


@trainer("Rosa's Encouragement", can_play=_rosa_ok)
def _(g, p):
    s = src("Rosa's Encouragement")
    t = yield from g.choose_pokemon(p.idx, [m for m in p.in_play() if m.card.stage == 2], s, "attach to")
    for e in (yield from take_from_discard(g, p, lambda c: c.basic_energy, 2, s)):
        t.energy.append(e)


@trainer("Scoop Up Cyclone", can_play=lambda g, p: len(p.in_play()) > 1)
def _(g, p):
    mon = yield from g.choose_pokemon(p.idx, p.in_play(), src("Scoop Up Cyclone"), "put into your hand")
    g.discard_pokemon(mon)
    for c in mon.cards + mon.energy + ([mon.tool] if mon.tool else []):
        p.discard.remove(c)
        p.hand.append(c)
