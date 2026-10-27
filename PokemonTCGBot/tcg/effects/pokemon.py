"""Hand-written Pokemon Abilities and attacks (the ones the text templates can't express)."""
from .registry import attack, ability, passive
from ..carddb import (GRASS, PSYCHIC, FIGHTING, DARKNESS, METAL, DRAGON, COLORLESS)


def _run(r):
    from ..engine import run
    return run(r)


def use_attack_of(g, ctx, card, atk):
    """Use another card's attack as this attack (Night Joker, Seek Inspiration)."""
    from . import attack_impl
    g.say(f"  using {card.name}'s {atk.name}")
    ctx.attack = atk
    ctx.damage = atk.base
    yield from _run(attack_impl(card, atk)(g, ctx))


def _ko_last_turn(g, p):
    return p.kod_turn == g.turn - 1


def _has_ability(mon, name):
    return any(a.name == name for a in mon.card.abilities)


# ============================================================ draw & search
@ability("Drakloak", "Recon Directive", can_use=lambda g, p, mon: bool(p.deck))
def _(g, p, mon):
    top = p.deck[-2:]
    del p.deck[-2:]
    keep = (yield from g.choose_cards(p.idx, top, 1, 1, mon.card, prompt="put into your hand"))[0]
    top.remove(keep)
    p.hand.append(keep)
    p.deck[:0] = top  # bottom of the deck


@ability("Fezandipiti ex", "Flip the Script", once="name", can_use=lambda g, p, mon: _ko_last_turn(g, p))
def _(g, p, mon):
    g.draw(p, 3)


def _last_ditch_ok(g, p, mon):
    return ("name", "Last-Ditch") not in p.used


@ability("Meowth ex", "Last-Ditch Catch", kind="on_bench", once=None, can_use=_last_ditch_ok)
def _(g, p, mon):
    p.used.add(("name", "Last-Ditch"))
    p.hand += yield from g.search_deck(p.idx, lambda c: c.is_supporter, 1, mon.card)


@ability("Mega Kangaskhan ex", "Run Errand", once="name", can_use=lambda g, p, mon: g.is_active(mon))
def _(g, p, mon):
    g.draw(p, 2)


for _name, _n in (("Kadabra", 2), ("Alakazam", 3)):
    @ability(_name, "Psychic Draw", kind="on_evolve", once=None)
    def _(g, p, mon, n=_n):
        g.draw(p, n)


@ability("N's Zoroark ex", "Trade", can_use=lambda g, p, mon: bool(p.hand))
def _(g, p, mon):
    c = (yield from g.choose_cards(p.idx, p.hand, 1, 1, mon.card, "discard"))[0]
    p.hand.remove(c)
    p.discard.append(c)
    g.draw(p, 2)


@ability("Thwackey", "Boom Boom Groove",
         can_use=lambda g, p, mon: _has_ability(p.active, "Festival Lead") and bool(p.deck))
def _(g, p, mon):
    p.hand += yield from g.search_deck(p.idx, lambda c: True, 1, mon.card, min_n=1)


@ability("Tatsugiri", "Attract Customers", can_use=lambda g, p, mon: g.is_active(mon) and bool(p.deck))
def _(g, p, mon):
    top = p.deck[-6:]
    del p.deck[-6:]
    picked = yield from g.choose_cards(p.idx, [c for c in top if c.is_supporter], 0, 1, mon.card)
    for c in picked:
        top.remove(c)
        p.hand.append(c)
    p.deck += top
    g.shuffle(p)


@ability("Lunatone", "Lunar Cycle", once="name",
         can_use=lambda g, p, mon: any(m.name == "Solrock" for m in p.in_play())
         and any(c.basic_energy and c.energy_type == FIGHTING for c in p.hand))
def _(g, p, mon):
    e = next(c for c in p.hand if c.basic_energy and c.energy_type == FIGHTING)
    p.hand.remove(e)
    p.discard.append(e)
    g.draw(p, 3)


@ability("Genesect ex", "Metallic Signal")
def _(g, p, mon):
    pred = lambda c: c.is_pokemon and c.stage > 0 and METAL in c.types
    p.hand += yield from g.search_deck(p.idx, pred, 2, mon.card)


@ability("Abra", "Teleporter", can_use=lambda g, p, mon: g.is_active(mon) and bool(p.bench))
def _(g, p, mon):
    g.discard_pokemon(mon)
    for c in mon.cards + mon.energy + ([mon.tool] if mon.tool else []):
        p.discard.remove(c)
        p.deck.append(c)
    g.shuffle(p)


# ========================================================= energy & bench
def _has_basic(p, t):
    return any(c.basic_energy and c.energy_type == t for c in p.hand)


@ability("Teal Mask Ogerpon ex", "Teal Dance", can_use=lambda g, p, mon: _has_basic(p, GRASS))
def _(g, p, mon):
    e = next(c for c in p.hand if c.basic_energy and c.energy_type == GRASS)
    yield from g.attach_energy(p, e, mon, p.hand)
    g.draw(p, 1)


@ability("Hydrapple ex", "Ripening Charge", can_use=lambda g, p, mon: _has_basic(p, GRASS))
def _(g, p, mon):
    e = next(c for c in p.hand if c.basic_energy and c.energy_type == GRASS)
    t = yield from g.choose_pokemon(p.idx, p.in_play(), mon.card, "attach to")
    yield from g.attach_energy(p, e, t, p.hand)
    g.heal(t, 30)


@ability("Metang", "Metal Maker", can_use=lambda g, p, mon: bool(p.deck))
def _(g, p, mon):
    top = p.deck[-4:]
    del p.deck[-4:]
    metal = [c for c in top if c.basic_energy and c.energy_type == METAL]
    for e in (yield from g.choose_cards(p.idx, metal, 0, len(metal), mon.card, prompt="attach")):
        t = yield from g.choose_pokemon(p.idx, p.in_play(), mon.card, "attach to")
        top.remove(e)
        t.energy.append(e)
    g.rng.shuffle(top)
    p.deck[:0] = top


def _benched_darkness(p):
    return [m for m in p.bench if DARKNESS in m.card.types]


@ability("Toxtricity", "Sinister Surge", can_use=lambda g, p, mon: bool(_benched_darkness(p)))
def _(g, p, mon):
    found = yield from g.search_deck(p.idx, lambda c: c.basic_energy and c.energy_type == DARKNESS, 1, mon.card)
    if found:
        t = yield from g.choose_pokemon(p.idx, _benched_darkness(p), mon.card, "attach to")
        t.energy.append(found[0])
        t.damage += 20


@ability("Iron Leaves ex", "Rapid Vernier", kind="on_bench", once=None)
def _(g, p, mon):
    if p.active is mon or mon not in p.bench:
        return
    g.switch_active(p, mon)
    while True:
        donors = [m for m in p.in_play() if m is not mon and m.energy]
        if not donors:
            return
        frm = yield from g.choose_pokemon(p.idx, donors, mon.card, "move energy from", optional=True)
        if frm is None:
            return
        e = (yield from g.choose_energy(p.idx, frm, 1, mon.card))[0]
        frm.energy.remove(e)
        mon.energy.append(e)


@ability("Chien-Pao", "Snow Sink", kind="on_bench", once=None, can_use=lambda g, p, mon: g.stadium is not None)
def _(g, p, mon):
    g.discard_stadium()
    for q in g.players:
        yield from g.enforce_bench_limit(q)


@ability("Hariyama", "Heave-Ho Catcher", kind="on_evolve", can_use=lambda g, p, mon: bool(g.opponent(p).bench))
def _(g, p, mon):
    o = g.opponent(p)
    t = yield from g.choose_pokemon(p.idx, o.bench, mon.card, "bring to the Active Spot")
    g.switch_active(o, t)


@ability("Pecharunt ex", "Subjugating Chains", once="name",
         can_use=lambda g, p, mon: any(m.name != "Pecharunt ex" for m in _benched_darkness(p)))
def _(g, p, mon):
    pool = [m for m in _benched_darkness(p) if m.name != "Pecharunt ex"]
    t = yield from g.choose_pokemon(p.idx, pool, mon.card, "switch in")
    g.switch_active(p, t)
    t.poisoned = not g.any_hook("condition_immune", t)


# ========================================================== damage counters
def _cursed_blast(n):
    def use(g, p, mon):
        o = g.opponent(p)
        targets = [m for m in o.in_play() if not g.effect_immune(m, p.idx, "ability")]
        if targets:
            t = yield from g.choose_pokemon(p.idx, targets, mon.card, "put damage counters on")
            t.damage += 10 * n
        mon.damage = g.max_hp(mon)  # Knocked Out; the engine gives the opponent its prizes
    return use


ability("Dusclops", "Cursed Blast")(_cursed_blast(5))
ability("Dusknoir", "Cursed Blast")(_cursed_blast(13))


def _adrena_ok(g, p, mon):
    return g.energy_count(mon, DARKNESS) > 0 and any(m.damage for m in p.in_play())


@ability("Munkidori", "Adrena-Brain", can_use=_adrena_ok)
def _(g, p, mon):
    o = g.opponent(p)
    frm = yield from g.choose_pokemon(p.idx, [m for m in p.in_play() if m.damage], mon.card, "move counters from")
    targets = [m for m in o.in_play() if not g.effect_immune(m, p.idx, "ability")]
    if not targets:
        return
    to = yield from g.choose_pokemon(p.idx, targets, mon.card, "move counters to")
    most = min(3, frm.damage // 10)
    n = 1 + (yield from g.choose_option(p.idx, [f"move {k}" for k in range(1, most + 1)], mon.card))
    frm.damage -= 10 * n
    to.damage += 10 * n


# ================================================================ passives
def _no_effects(g, src, mon, kind):
    return src is mon


for _name in ("Shuppet", "Banette", "Poltchageist", "Sinistcha"):
    passive(_name, "Hide 'n' Sneak", no_effects=_no_effects)

passive("Team Rocket's Articuno", "Repelling Veil",
        no_effects=lambda g, src, mon, kind: kind == "attack" and mon.owner == src.owner
        and mon.card.stage == 0 and mon.card.owner_tag == "Team Rocket")
passive("Latias ex", "Skyliner",
        retreat_cost=lambda g, src, mon, cost: 0 if mon.owner == src.owner and mon.card.stage == 0 else cost)
passive("Lillie's Clefairy ex", "Fairy Zone",
        weakness=lambda g, src, mon, types: (PSYCHIC,) if mon.owner != src.owner and DRAGON in mon.card.types
        else types)
passive("Meganium", "Wild Growth",
        basic_energy_units=lambda g, src, mon, e, n: max(n, 2) if mon.owner == src.owner
        and e.energy_type == GRASS else n)
passive("Crustle", "Mysterious Rock Inn",
        prevent_damage=lambda g, src, ctx, target: target is src and ctx.attacker.owner != src.owner
        and (ctx.attacker.card.ex or ctx.attacker.card.mega))


def _plasma_bane(g, src, mon, attack, cost):
    o = g.opponent(g.players[mon.owner])
    if mon is src and attack.name == "Trifrost" and any("Colress" in c.name for c in o.discard):
        return [COLORLESS]
    return cost


passive("Kyurem", "Plasma Bane", attack_cost=_plasma_bane)

for _name in ("Goldeen", "Seaking", "Dipplin"):
    passive(_name, "Festival Lead", attack_twice=lambda g, src, mon: src is mon and g.stadium is not None
            and g.stadium.name == "Festival Grounds")


# ================================================================= attacks
@attack("Alakazam", "Powerful Hand")
def _(g, ctx):
    d = ctx.defender
    if d is not None and not g.effect_immune(d, ctx.player.idx):
        d.damage += 20 * len(ctx.player.hand)


@attack("Wellspring Mask Ogerpon ex", "Torrential Pump")
def _(g, ctx):
    ctx.hit()
    a, p = ctx.attacker, ctx.player
    if len(a.energy) >= 3 and ctx.opponent.bench and (yield from g.yes_no(p.idx, a.card, "shuffle 3 Energy?")):
        picked = yield from g.choose_energy(p.idx, a, 3, a.card)
        for e in picked:
            a.energy.remove(e)
            p.deck.append(e)
        g.shuffle(p)
        t = yield from g.choose_pokemon(p.idx, ctx.opponent.bench, a.card, "120 damage to")
        ctx.snipe(t, 120)


@attack("Duskull", "Come and Get You")
def _(g, ctx):
    p = ctx.player
    room = g.bench_limit(p) - len(p.bench)
    pool = [c for c in p.discard if c.name == "Duskull"]
    for c in (yield from g.choose_cards(p.idx, pool, 0, min(3, room), ctx.attacker.card)):
        p.discard.remove(c)
        g.put_on_bench(p, c)


@attack("Banette", "Puppet Pull")
def _(g, ctx):
    ctx.hit()
    p = ctx.player
    p.hand += yield from g.search_deck(p.idx, lambda c: True, 1, ctx.attacker.card)


def _hide_n_sneak_in_discard(p):
    return sum(1 for c in p.discard if c.is_pokemon and any(a.name == "Hide 'n' Sneak" for a in c.abilities))


@attack("Dhelmise", "Vengeful Anchor")
def _(g, ctx):
    if _hide_n_sneak_in_discard(ctx.player) >= 4:
        ctx.damage += 140
    ctx.hit()


@attack("Poltchageist", "Furtive Drop")
def _(g, ctx):
    if not g.effect_immune(ctx.defender, ctx.player.idx):
        ctx.defender.damage += 10


@attack("Sinistcha", "Matcha Spin")
def _(g, ctx):
    if _hide_n_sneak_in_discard(ctx.player) >= 6:
        for m in ctx.opponent.in_play():
            if not g.effect_immune(m, ctx.player.idx):
                m.damage += 40


@attack("Celebi", "Traverse Time")
def _(g, ctx):
    p = ctx.player
    pred = lambda c: (c.is_pokemon and GRASS in c.types) or c.is_stadium
    p.hand += yield from g.search_deck(p.idx, pred, 3, ctx.attacker.card)


@attack("Slowpoke", "Dangle Tail")
def _(g, ctx):
    p = ctx.player
    picked = yield from g.choose_cards(p.idx, [c for c in p.discard if c.is_pokemon], 0, 1, ctx.attacker.card)
    for c in picked:
        p.discard.remove(c)
        p.hand.append(c)


@attack("Slowking", "Seek Inspiration")
def _(g, ctx):
    p = ctx.player
    if not p.deck:
        return
    top = p.deck.pop()
    p.discard.append(top)
    g.say(f"  discarded {top.name}")
    if not top.is_pokemon or top.rule_box or not top.attacks:
        return
    if len(top.attacks) == 1:
        atk = top.attacks[0]
    else:
        atk = top.attacks[(yield from g.choose_option(p.idx, [a.name for a in top.attacks], ctx.attacker.card))]
    yield from use_attack_of(g, ctx, top, atk)


@attack("Metagross", "Metallic Hammer")
def _(g, ctx):
    a, p = ctx.attacker, ctx.player
    metal = [e for e in a.energy if e.basic_energy and e.energy_type == METAL]
    if len(metal) >= 3 and (yield from g.yes_no(p.idx, a.card, "discard 3 Metal Energy for +150?")):
        g.discard_energy(a, metal[:3])
        ctx.damage += 150
    ctx.hit()


@attack("N's Zoroark ex", "Night Joker")
def _(g, ctx):
    p = ctx.player
    choices = [(m.card, atk) for m in p.bench if m.card.owner_tag == "N" for atk in m.card.attacks
               if atk.name != "Night Joker"]
    if not choices:
        return
    k = yield from g.choose_option(p.idx, [f"{c.name}: {a.name}" for c, a in choices], ctx.attacker.card)
    yield from use_attack_of(g, ctx, *choices[k])


@attack("Mega Lucario ex", "Aura Jab")
def _(g, ctx):
    ctx.hit()
    p = ctx.player
    pool = [c for c in p.discard if c.basic_energy and c.energy_type == FIGHTING]
    if not p.bench or not pool:
        return
    for e in (yield from g.choose_cards(p.idx, pool, 0, 3, ctx.attacker.card, prompt="attach")):
        t = yield from g.choose_pokemon(p.idx, p.bench, ctx.attacker.card, "attach to")
        p.discard.remove(e)
        t.energy.append(e)


@attack("Solrock", "Cosmic Beam")
def _(g, ctx):
    if any(m.name == "Lunatone" for m in ctx.player.bench):
        ctx.apply_wr = False
        ctx.hit()


@attack("Team Rocket's Murkrow", "Torment")
def _(g, ctx):
    ctx.hit()
    d = ctx.defender
    if d is not None and g.in_play(d) and d.card.attacks and not g.effect_immune(d, ctx.player.idx):
        k = yield from g.choose_option(ctx.player.idx, [a.name for a in d.card.attacks], ctx.attacker.card)
        d.markers["no_attack:" + d.card.attacks[k].name] = g.turn + 1


def _tr_supporter(c):
    return c.is_supporter and c.name.startswith("Team Rocket's")


@attack("Team Rocket's Honchkrow", "Rocket Feathers")
def _(g, ctx):
    p = ctx.player
    pool = [c for c in p.hand if _tr_supporter(c)]
    picked = yield from g.choose_cards(p.idx, pool, 0, len(pool), ctx.attacker.card, "discard")
    for c in picked:
        p.hand.remove(c)
        p.discard.append(c)
    ctx.hit(60 * len(picked))


@attack("Team Rocket's Porygon", "Hacking")
def _(g, ctx):
    p, o = ctx.player, ctx.opponent
    if not p.hand:
        return
    c = (yield from g.choose_cards(p.idx, p.hand, 1, 1, ctx.attacker.card, "discard"))[0]
    p.hand.remove(c)
    p.discard.append(c)
    if o.hand:
        c = (yield from g.choose_cards(o.idx, o.hand, 1, 1, ctx.attacker.card, "discard"))[0]
        o.hand.remove(c)
        o.discard.append(c)


for _name in ("Team Rocket's Porygon2", "Team Rocket's Porygon-Z"):
    @attack(_name, "R Command")
    def _(g, ctx):
        ctx.hit(20 * sum(_tr_supporter(c) for c in ctx.player.discard))


@attack("Team Rocket's Articuno", "Dark Frost")
def _(g, ctx):
    if any(e.name == "Team Rocket's Energy" for e in ctx.attacker.energy):
        ctx.damage += 60
    ctx.hit()


@ability("Dudunsparce", "Run Away Draw", can_use=lambda g, p, mon: bool(p.deck))
def _(g, p, mon):
    if g.draw(p, 3):
        g.discard_pokemon(mon)
        for c in mon.cards + mon.energy + ([mon.tool] if mon.tool else []):
            p.discard.remove(c)
            p.deck.append(c)
        g.shuffle(p)


def _solar_transfer_ok(g, p, mon):
    mons = p.in_play()
    return len(mons) > 1 and any(e.basic_energy and e.energy_type == GRASS for m in mons for e in m.energy)


@ability("Mega Venusaur ex", "Solar Transfer", once=None, can_use=_solar_transfer_ok)
def _(g, p, mon):
    grass = lambda e: e.basic_energy and e.energy_type == GRASS
    frm = yield from g.choose_pokemon(p.idx, [m for m in p.in_play() if any(grass(e) for e in m.energy)],
                                      mon.card, "move Grass Energy from")
    e = (yield from g.choose_energy(p.idx, frm, 1, mon.card, grass))[0]
    to = yield from g.choose_pokemon(p.idx, [m for m in p.in_play() if m is not frm], mon.card, "move it to")
    frm.energy.remove(e)
    to.energy.append(e)
