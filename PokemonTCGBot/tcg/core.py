"""Game objects shared by the engine and the card effects."""
from collections import Counter

MAX_BENCH = 8  # hard ceiling (Area Zero Underdepths); the normal limit is 5
DEFAULT_BENCH = 5

# Every decision has one of these kinds; the network embeds them.
DECISION_KINDS = ["main", "go_first", "setup_active", "setup_bench", "promote", "pick_card",
                  "pick_pokemon", "pick_energy", "yes_no", "pick_option", "discard", "search", "counters"]
OPTION_KINDS = ["end", "attack", "ability", "retreat", "bench", "evolve", "attach", "tool", "play",
                "stadium", "card", "pokemon", "energy", "done", "yes", "no", "first", "second", "choice"]
DECISION_INDEX = {k: i for i, k in enumerate(DECISION_KINDS)}
OPTION_INDEX = {k: i for i, k in enumerate(OPTION_KINDS)}


class GameOver(Exception):
    def __init__(self, winner, reason):
        super().__init__(reason)
        self.winner = winner
        self.reason = reason


class Option:
    """One choice in a Decision.

    card   -- the card this option is about (card to play, card to take, attacking Pokemon...)
    target -- the Pokemon in play this option points at, if any
    data   -- anything else the code that asked needs (attack index, ability...)
    """
    __slots__ = ("kind", "card", "target", "data", "label")

    def __init__(self, kind, card=None, target=None, data=None, label=None):
        assert kind in OPTION_INDEX, kind
        self.kind = kind
        self.card = card
        self.target = target
        self.data = data
        self.label = label

    def __repr__(self):
        return f"Option({self.label or self.kind})"


class Decision:
    __slots__ = ("player", "kind", "options", "source", "prompt")

    def __init__(self, player, kind, options, source=None, prompt=""):
        assert kind in DECISION_INDEX, kind
        self.player = player
        self.kind = kind
        self.options = options
        self.source = source  # card whose effect is asking (None for game rules)
        self.prompt = prompt

    def __repr__(self):
        return f"Decision(P{self.player} {self.kind}: {self.options})"


class Pokemon:
    """A Pokemon in play: its evolution stack plus everything attached to it."""
    __slots__ = ("uid", "owner", "cards", "energy", "tool", "damage", "poisoned", "burned", "asleep",
                 "paralyzed", "confused", "turn_entered", "markers", "poison_damage", "moved_turn")

    def __init__(self, card, owner, turn, uid):
        self.uid = uid
        self.owner = owner
        self.cards = [card]
        self.energy = []
        self.tool = None
        self.damage = 0
        self.turn_entered = turn
        self.moved_turn = -1  # last turn it moved from the Bench to the Active Spot
        self.markers = {}
        self.clear_conditions()

    @property
    def card(self):
        return self.cards[-1]

    @property
    def name(self):
        return self.cards[-1].name

    def clear_conditions(self):
        self.poisoned = self.burned = self.asleep = self.paralyzed = self.confused = False
        self.poison_damage = 10

    @property
    def conditions(self):
        return [n for n in ("poisoned", "burned", "asleep", "paralyzed", "confused") if getattr(self, n)]

    def set_rotation(self, condition):
        """Asleep, Confused and Paralyzed replace each other."""
        self.asleep = self.confused = self.paralyzed = False
        setattr(self, condition, True)

    def leave_active(self):
        """Moving to the Bench (or evolving) clears conditions and attack effects."""
        self.clear_conditions()
        self.markers.clear()

    def marker(self, key, turn):
        until = self.markers.get(key)
        return until is not None and until >= turn

    def __repr__(self):
        return f"<{self.name} {self.damage}dmg {len(self.energy)}E>"


class Player:
    def __init__(self, idx, deck):
        self.idx = idx
        self.decklist = list(deck)  # the 60 cards this player registered (they know their own list)
        self.deck = list(deck)
        # What the opponent can know about this player, the way a real player keeps track:
        self.known_hand = Counter()  # cards revealed into this hand (searches that reveal, Eri...)
        self.seen = Counter()        # most copies of each card this player has shown at once this game
        self.hand = []
        self.discard = []
        self.prizes = []
        self.lost = []
        self.active = None
        self.bench = []
        self.markers = {}
        self.used = set()
        self.turns_taken = 0
        self.mulligans = 0
        self.kod_turn = -9  # last turn one of this player's Pokemon was Knocked Out
        self.kod_tr_turn = -9  # same, for Team Rocket's Pokemon
        self.energy_attached = self.supporter_played = self.retreated = self.stadium_played = False

    def in_play(self):
        return ([self.active] if self.active else []) + self.bench

    def has_pokemon(self):
        return self.active is not None or bool(self.bench)

    def marker(self, key, turn):
        until = self.markers.get(key)
        return until is not None and until >= turn
