"""Encode a pending Decision as numpy arrays for the network.

Everything is from the deciding player's point of view and only uses
information that player could know, the way a real player keeps track:
both boards, their own hand, both discard piles, sizes of hidden zones,
which of their own cards are still unseen (in deck or prizes, worked out
from their decklist), cards known to be in the opponent's hand (revealed
by searches, Eri...), and every card the opponent has shown this game.
Card identities are passed as ids (0 = empty, card.id + 1 otherwise).
"""
from collections import Counter

import numpy as np

from tcg.carddb import CARDS, NUM_CARDS, NUM_TYPES, COLORLESS
from tcg.core import MAX_BENCH, DECISION_INDEX, OPTION_INDEX, DECISION_KINDS

SLOTS = 1 + MAX_BENCH          # per side
NUM_SLOTS = 2 * SLOTS          # me then opponent
MAX_HAND = 40
MAX_DISCARD = 60
MAX_UNSEEN = 60
MAX_KNOWN_HAND = 20
MAX_SEEN = 60
NO_TARGET = NUM_SLOTS          # option target index meaning "none"
SLOT_FEATURES = 26
GLOBAL_FEATURES = 20 + len(DECISION_KINDS)
OPTION_FEATURES = 6


def _card_static(c):
    f = np.zeros(64, dtype=np.float32)
    f[0], f[1], f[2] = c.is_pokemon, c.is_trainer, c.is_energy
    if c.is_pokemon:
        f[3 + c.stage] = 1
        f[6] = c.hp / 300
        for t in c.types:
            f[7 + t] = 1
        for t in c.weakness:
            f[17 + t] = 1
        for t in c.resistance:
            f[27 + t] = 1
        f[37] = c.retreat / 4
        f[38], f[39], f[40], f[41] = c.ex, c.mega, c.tera, c.rule_box
        f[42] = len(c.attacks) / 2
        if c.attacks:
            f[43] = max(a.base for a in c.attacks) / 300
            f[44] = min(len(a.cost) for a in c.attacks) / 4
            f[45] = max(len(a.cost) for a in c.attacks) / 4
            f[46] = any(a.mod for a in c.attacks)
        f[47] = len(c.abilities)
    elif c.is_trainer:
        f[48 + ["Item", "Supporter", "Stadium", "Pokémon Tool"].index(c.trainer_kind)] = 1
        f[52] = c.ace_spec
    else:
        f[53] = c.basic_energy
        if c.basic_energy:
            f[54 + c.energy_type] = 1
    return f


# Row 0 is the "no card" entry
CARD_STATIC = np.zeros((NUM_CARDS + 1, 64), dtype=np.float32)
for _c in CARDS:
    CARD_STATIC[_c.id + 1] = _card_static(_c)


def _slot(g, mon, active, out):
    f = out
    f[0] = 1
    f[1] = active
    hp = g.max_hp(mon)
    f[2] = (hp - mon.damage) / 300
    f[3] = mon.damage / 300
    f[4] = hp / 300
    units = g.energy_units(mon)
    for u in units:
        if len(u) == 1:
            f[5 + next(iter(u))] += 0.25
        else:
            f[5 + COLORLESS] += 0.25
    f[15] = len(mon.energy) / 4
    f[16], f[17], f[18], f[19], f[20] = mon.poisoned, mon.burned, mon.asleep, mon.paralyzed, mon.confused
    f[21] = mon.turn_entered < g.turn
    f[22] = mon.moved_turn == g.turn
    f[23] = any(k.startswith("no_attack") and v >= g.turn for k, v in mon.markers.items())
    f[24] = any(k.startswith(("prevent", "take_less")) and v >= g.turn for k, v in mon.markers.items())
    f[25] = (len(mon.cards) - 1) / 2


def _ids(counts, size):
    out = np.zeros(size, dtype=np.int64)
    i = 0
    for cid, n in sorted(counts.items()):
        for _ in range(n):
            if i >= size:
                return out
            out[i] = cid + 1
            i += 1
    return out


def unseen_cards(g, p):
    """p's decklist minus every card of theirs they can see: what's in their deck and prizes."""
    left = Counter(c.id for c in p.decklist)
    visible = [c.id for c in p.hand + p.discard + p.lost]
    for m in p.in_play():
        visible += [c.id for c in m.cards + m.energy] + ([m.tool.id] if m.tool else [])
    if g.stadium is not None and g.stadium_owner == p.idx:
        visible.append(g.stadium.id)
    left.subtract(visible)
    return Counter({k: v for k, v in left.items() if v > 0})


def encode(g):
    """Encode g.decision for the player who must answer it."""
    d = g.decision
    me = g.players[d.player]
    opp = g.players[1 - d.player]
    slot_card = np.zeros(NUM_SLOTS, dtype=np.int64)
    slot_tool = np.zeros(NUM_SLOTS, dtype=np.int64)
    slot_feat = np.zeros((NUM_SLOTS, SLOT_FEATURES), dtype=np.float32)
    index = {}
    for side, pl in enumerate((me, opp)):
        if side == 1 and g.in_setup:
            break  # setup choices are made face down
        for k, mon in enumerate(pl.in_play()[:SLOTS]):
            s = side * SLOTS + k
            index[id(mon)] = s
            slot_card[s] = mon.card.id + 1
            slot_tool[s] = mon.tool.id + 1 if mon.tool else 0
            _slot(g, mon, k == 0, slot_feat[s])

    hand = np.zeros(MAX_HAND, dtype=np.int64)
    for i, c in enumerate(me.hand[:MAX_HAND]):
        hand[i] = c.id + 1
    discards = np.zeros((2, MAX_DISCARD), dtype=np.int64)
    for side, pl in enumerate((me, opp)):
        for i, c in enumerate(pl.discard[-MAX_DISCARD:]):
            discards[side, i] = c.id + 1

    gl = np.zeros(GLOBAL_FEATURES, dtype=np.float32)
    gl[:20] = (
        len(me.deck) / 60, len(opp.deck) / 60, len(me.prizes) / 6, len(opp.prizes) / 6,
        len(me.hand) / 10, len(opp.hand) / 10, len(me.discard) / 30, len(opp.discard) / 30,
        me.energy_attached, me.supporter_played, me.retreated, me.stadium_played,
        g.current == d.player, g.turn == 1, min(g.turn, 60) / 60, g.in_setup,
        g.stadium is not None and g.stadium_owner == d.player,
        me.kod_turn == g.turn - 1, opp.kod_turn == g.turn - 1, len(me.bench) / 8,
    )
    gl[20 + DECISION_INDEX[d.kind]] = 1
    stadium = g.stadium.id + 1 if g.stadium is not None else 0
    source = d.source.id + 1 if d.source is not None else 0

    k = len(d.options)
    opt_kind = np.zeros(k, dtype=np.int64)
    opt_card = np.zeros(k, dtype=np.int64)
    opt_target = np.full(k, NO_TARGET, dtype=np.int64)
    opt_feat = np.zeros((k, OPTION_FEATURES), dtype=np.float32)
    for i, o in enumerate(d.options):
        opt_kind[i] = OPTION_INDEX[o.kind]
        if o.card is not None:
            opt_card[i] = o.card.id + 1
        if o.target is not None:
            opt_target[i] = index.get(id(o.target), NO_TARGET)
        if o.kind == "attack":
            opt_feat[i, 0 + min(o.data, 1)] = 1
            atk = o.card.attacks[o.data]
            opt_feat[i, 2] = atk.base / 300
        elif o.kind == "choice":
            opt_feat[i, 3] = o.data / 5
        if o.card is not None:
            opt_feat[i, 4] = sum(c is o.card for c in me.hand) / 4
        opt_feat[i, 5] = i / 20
    return {
        "slot_card": slot_card, "slot_tool": slot_tool, "slot_feat": slot_feat,
        "hand": hand, "discards": discards, "globals": gl,
        "stadium": np.int64(stadium), "source": np.int64(source),
        "unseen": _ids(unseen_cards(g, me), MAX_UNSEEN),
        "opp_known_hand": _ids(opp.known_hand, MAX_KNOWN_HAND),
        "opp_seen": _ids(opp.seen, MAX_SEEN),
        "opt_kind": opt_kind, "opt_card": opt_card, "opt_target": opt_target, "opt_feat": opt_feat,
    }


FIXED_KEYS = ["slot_card", "slot_tool", "slot_feat", "hand", "discards", "globals", "stadium", "source",
              "unseen", "opp_known_hand", "opp_seen"]
MEMORY_KEYS = ["unseen", "opp_known_hand", "opp_seen"]  # the "what a real player keeps track of" inputs
OPTION_KEYS = ["opt_kind", "opt_card", "opt_target", "opt_feat"]


def collate(encs):
    """Stack encodings into batch arrays; options are padded and masked."""
    out = {k: np.stack([e[k] for e in encs]) for k in FIXED_KEYS}
    k = max(len(e["opt_kind"]) for e in encs)
    n = len(encs)
    mask = np.zeros((n, k), dtype=bool)
    for key in OPTION_KEYS:
        shape = (n, k) + encs[0][key].shape[1:]
        fill = NO_TARGET if key == "opt_target" else 0
        arr = np.full(shape, fill, dtype=encs[0][key].dtype)
        for i, e in enumerate(encs):
            arr[i, :len(e[key])] = e[key]
        out[key] = arr
    for i, e in enumerate(encs):
        mask[i, :len(e["opt_kind"])] = True
    out["opt_mask"] = mask
    return out
