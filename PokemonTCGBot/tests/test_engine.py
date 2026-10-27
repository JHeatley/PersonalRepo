import random

import pytest

from tcg.carddb import BY_PRINT, BASIC_ENERGY, FIGHTING, GRASS, PSYCHIC, DARKNESS, COLORLESS, TYPE, card
from tcg.deckio import load_deck, deck_names, parse_deck
from tcg.engine import Game, AttackCtx, can_pay
from tcg import effects


def pr(code, num):
    return BY_PRINT[(code, str(num))]


def energy(t, n=1):
    return [BASIC_ENERGY[t]] * n


def fresh(seed=0, a="lucario", b="basic_box"):
    """A game paused at its first main-phase decision."""
    g = Game(load_deck(a), load_deck(b), seed=seed)
    while g.decision.kind != "main":
        g.step(0)
    return g


def place(g, idx, active, bench=(), active_energy=()):
    """Replace a player's board with fresh Pokemon (which count as in play since before this turn)."""
    p = g.players[idx]
    p.active = g.new_pokemon(active, idx)
    p.active.turn_entered = 0
    p.active.energy = list(active_energy)
    p.bench = []
    for c in bench:
        m = g.new_pokemon(c, idx)
        m.turn_entered = 0
        p.bench.append(m)
    return p


def drive(gen, answer=0):
    """Run an engine generator, answering every decision with option `answer`."""
    try:
        d = next(gen)
        while True:
            d = gen.send(min(answer, len(d.options) - 1))
    except StopIteration as e:
        return e.value


# ----------------------------------------------------------------- decks
@pytest.mark.parametrize("name", deck_names())
def test_meta_decks_are_legal_and_fully_implemented(name):
    deck = load_deck(name)
    assert len(deck) == 60
    missing = {c.name: effects.missing_parts(c) for c in deck if effects.missing_parts(c)}
    assert not missing


def test_parse_tcg_live_export():
    deck = parse_deck("""Pokémon: 2
    3 Dreepy TWM 128
    Trainer: 1
    2 Ultra Ball MEG 131
    Energy: 1
    5 Basic {P} Energy MEE 13
    1 Psychic Energy SVE 5""")
    assert [c.name for c in deck].count("Dreepy") == 3
    assert sum(c.basic_energy and c.energy_type == PSYCHIC for c in deck) == 6


# ----------------------------------------------------------------- energy
def test_can_pay():
    G, F, C = frozenset({GRASS}), frozenset({FIGHTING}), frozenset({COLORLESS})
    both = frozenset({PSYCHIC, DARKNESS})
    assert can_pay([F, F], (FIGHTING, FIGHTING))
    assert not can_pay([F, G], (FIGHTING, FIGHTING))
    assert can_pay([F, G, C], (FIGHTING, COLORLESS, COLORLESS))
    assert not can_pay([C, C], (FIGHTING,))
    assert can_pay([both, both], (PSYCHIC, DARKNESS))
    assert not can_pay([F], (FIGHTING, COLORLESS))


def test_wild_growth_doubles_basic_grass():
    g = fresh()
    p = place(g, g.current, card("Teal Mask Ogerpon ex"), bench=[pr("MEG", 10)], active_energy=energy(GRASS, 2))
    assert g.energy_count(p.active, GRASS) == 4  # Meganium on the bench


# ---------------------------------------------------------------- damage
def test_weakness_and_mega_ex_gives_three_prizes():
    g = fresh()
    me, opp = g.current, 1 - g.current
    p = place(g, me, pr("MEG", 77), active_energy=energy(FIGHTING, 2))           # Mega Lucario ex
    o = place(g, opp, pr("MEG", 104), bench=[pr("JTG", 56)])                     # Mega Kangaskhan ex
    atk = p.active.card.attacks[1]                                                # Mega Brave 270
    ctx = AttackCtx(g, p, p.active, atk)
    assert ctx.hit() == 540
    g.attacking = ctx
    prizes = len(p.prizes)
    drive(g.knockouts())
    assert len(p.prizes) == prizes - 3
    assert o.active.name == "Lillie's Clefairy ex"


def test_resistance():
    g = fresh()
    me, opp = g.current, 1 - g.current
    p = place(g, me, pr("MEG", 77), active_energy=energy(FIGHTING, 2))
    place(g, opp, pr("SSP", 76))                                                  # Latias ex resists Fighting
    assert AttackCtx(g, p, p.active, p.active.card.attacks[0]).hit() == 100


def test_tera_bench_protection_blocks_damage_but_not_counters():
    g = fresh()
    me, opp = g.current, 1 - g.current
    p = place(g, me, card("Fezandipiti ex"), active_energy=energy(DARKNESS, 3))
    o = place(g, opp, pr("MEG", 104), bench=[card("Teal Mask Ogerpon ex")])
    ctx = AttackCtx(g, p, p.active, p.active.card.attacks[0])
    assert ctx.snipe(o.bench[0], 100) == 0
    drive(g.place_counters(me, o.bench, 6))
    assert o.bench[0].damage == 60


def test_fairy_zone_makes_dragons_weak_to_psychic():
    g = fresh()
    me, opp = g.current, 1 - g.current
    p = place(g, me, pr("JTG", 56), active_energy=energy(PSYCHIC, 2))             # Lillie's Clefairy ex
    place(g, opp, pr("TWM", 130))                                                 # Dragapult ex
    ctx = AttackCtx(g, p, p.active, p.active.card.attacks[0])
    assert ctx.hit(20) == 40


def test_crustle_blocks_damage_from_ex():
    g = fresh()
    me, opp = g.current, 1 - g.current
    p = place(g, me, pr("MEG", 104), active_energy=energy(GRASS, 3))
    place(g, opp, pr("DRI", 12))
    assert AttackCtx(g, p, p.active, p.active.card.attacks[0]).hit(200) == 0


# ------------------------------------------------------------ turn rules
def test_first_turn_restrictions():
    g = Game(load_deck("lucario"), load_deck("basic_box"), seed=3)
    while g.decision.kind != "main":
        g.step(0)
    assert g.turn == 1
    p = g.players[g.current]
    opts = g.main_options(p)
    assert not any(o.kind == "attack" for o in opts)
    assert not any(o.kind == "play" and o.card.is_supporter and o.card.name != "Team Rocket's Proton" for o in opts)
    assert not any(o.kind == "evolve" for o in opts)


def test_rare_candy_needs_a_stage_2_in_hand():
    g = fresh(a="dragapult", b="lucario")
    me = g.current
    g.turn = 3
    p = place(g, me, pr("TWM", 128))  # Dreepy
    p.turns_taken = 1
    p.hand = [card("Rare Candy")]
    assert not any(o.kind == "play" and o.card.name == "Rare Candy" for o in g.main_options(p))
    p.hand.append(pr("TWM", 130))
    assert any(o.kind == "play" and o.card.name == "Rare Candy" for o in g.main_options(p))


def test_no_evolving_a_pokemon_played_this_turn_unless_forest_of_vitality():
    g = fresh(a="hydrapple", b="lucario")
    me = g.current
    g.turn = 3
    p = place(g, me, pr("TWM", 17))  # Applin
    p.turns_taken = 1
    p.active.turn_entered = g.turn
    p.hand = [pr("TWM", 18)]  # Dipplin
    assert not any(o.kind == "evolve" for o in g.main_options(p))
    g.stadium, g.stadium_owner = card("Forest of Vitality"), me
    assert any(o.kind == "evolve" for o in g.main_options(p))


def test_retreat_cost_modifiers():
    g = fresh()
    me = g.current
    p = place(g, me, pr("MEG", 104))                    # Mega Kangaskhan ex: retreat 3
    assert g.retreat_cost(p.active) == 3
    p.active.tool = card("Air Balloon")
    assert g.retreat_cost(p.active) == 1
    p.bench.append(g.new_pokemon(pr("SSP", 76), me))    # Latias ex: Basics retreat for free
    assert g.retreat_cost(p.active) == 0


def test_attack_text_templates():
    from tcg.effects.templates import parse
    assert parse("Flip 2 coins. This attack does 30 damage for each heads.") is not None
    assert parse("Your opponent's Active Pokémon is now Burned and Confused.") is not None
    assert parse("Discard 2 Energy from this Pokémon.") is not None
    assert parse("Gibberish that no card says.") is None


# ------------------------------------------------------------------ fuzz
def test_random_games_conserve_cards():
    decks = {n: load_deck(n) for n in deck_names()}
    rng = random.Random(7)
    for i in range(30):
        g = Game(decks[rng.choice(list(decks))], decks[rng.choice(list(decks))], seed=i)
        while not g.done:
            g.step(rng.randrange(len(g.decision.options)))
        for p in g.players:
            total = len(p.deck) + len(p.hand) + len(p.discard) + len(p.prizes) + len(p.lost)
            total += sum(len(m.cards) + len(m.energy) + (m.tool is not None) for m in p.in_play())
            total += g.stadium is not None and g.stadium_owner == p.idx
            assert total == 60


def test_bugs_cannon_scales_with_grass_energy_and_hits_crustle():
    g = fresh()
    me, opp = g.current, 1 - g.current
    p = place(g, me, pr("PFL", 8), active_energy=energy(GRASS, 3))   # Genesect, not an ex
    o = place(g, opp, pr("DRI", 12))                                  # Crustle blocks damage from ex only
    atk = p.active.card.attacks[0]
    ctx = AttackCtx(g, p, p.active, atk)
    g.attacking = ctx
    drive(effects.attack_impl(p.active.card, atk)(g, ctx))
    assert o.active.damage == 60
