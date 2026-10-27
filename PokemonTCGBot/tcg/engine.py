"""Pokemon TCG rules engine for the real card pool.

The game runs as a generator. Whenever a player has to choose something,
it yields a Decision; `Game.step(i)` answers with the index of an option and
the game runs until the next Decision. Card effects are written the same way
(see effects/registry.py), so a search, a discard or a damage-counter
placement in the middle of an attack is an ordinary decision.

    g = Game(deck_a, deck_b, seed=1)
    while not g.done:
        g.step(pick(g.decision))

Rules covered: mulligans, the go-first choice, setup, prizes (1/2/3 for
regular/ex/Mega ex), one energy attachment / Supporter / Stadium / retreat
per turn, no attack or Supporter for the first player on turn 1, no evolving
on your first turn or a Pokemon's first turn in play, Weakness and
Resistance, Tera bench protection, Poisoned/Burned/Asleep/Paralyzed/Confused
with the checkup, Pokemon Tools, Stadiums, special energy, Abilities.
Not covered: any effect whose card isn't implemented (see tools/coverage.py).
"""
import inspect
import random

from .carddb import COLORLESS, NUM_TYPES, CARDS
from .core import (Option, Decision, GameOver, Pokemon, Player, DEFAULT_BENCH, MAX_BENCH)
from . import effects as fx

HAND_SIZE = 7
PRIZE_COUNT = 6
MAX_TURNS = 120
ANY_TYPE = frozenset(range(NUM_TYPES))


def run(result):
    """Run an effect that may or may not be a generator."""
    if inspect.isgenerator(result):
        return (yield from result)
    return result


def can_pay(units, cost):
    """units: one frozenset of payable types per energy unit. Colorless takes any unit."""
    if len(units) < len(cost):
        return False
    specific = [t for t in cost if t != COLORLESS]
    specific.sort(key=lambda t: sum(t in u for u in units))

    def match(i, used):
        if i == len(specific):
            return True
        for j, u in enumerate(units):
            if not used >> j & 1 and specific[i] in u and match(i + 1, used | 1 << j):
                return True
        return False
    return match(0, 0)


def _marker_total(mon, prefix, turn):
    return sum(int(k[len(prefix):]) for k, until in mon.markers.items() if k.startswith(prefix) and until >= turn)


class AttackCtx:
    def __init__(self, game, player, attacker, attack):
        self.g = game
        self.player = player
        self.opponent = game.players[1 - player.idx]
        self.attacker = attacker
        self.defender = self.opponent.active
        self.attack = attack
        self.damage = attack.base
        self.apply_wr = True
        self.ignore_resistance = False
        self.ignore_defender_effects = False  # "isn't affected by any effects on your opponent's Active"
        self.cancelled = False
        self.dealt = 0
        self.boomerang = []

    def hit(self, dmg=None):
        """Damage the Defending Pokemon (the usual attack damage)."""
        dmg = self.damage if dmg is None else dmg
        return self.g.attack_damage(self, self.defender, dmg, apply_wr=self.apply_wr)

    def snipe(self, target, dmg):
        """Damage any opposing Pokemon; Weakness/Resistance only apply to the Active."""
        return self.g.attack_damage(self, target, dmg, apply_wr=target is self.defender)


class Game:
    def __init__(self, deck0, deck1, seed=None, log=False, max_turns=MAX_TURNS):
        self.rng = random.Random(seed)
        self.players = [Player(0, deck0), Player(1, deck1)]
        self.turn = 0
        self.current = 0
        self.stadium = None
        self.stadium_owner = None
        self.max_turns = max_turns
        self.done = False
        self.winner = None
        self.end_reason = None
        self.in_setup = True
        self.attacking = None   # AttackCtx while an attack resolves
        self._pending_triggers = []
        self.logging = log
        self.log = []
        self._uid = 0
        self.decision = None
        self._flow = self._run()
        self._advance(None)

    # ================================================================ driving
    def _advance(self, answer):
        try:
            self.decision = self._flow.send(answer)
        except StopIteration:
            self.decision = None

    def step(self, index):
        assert not self.done
        assert 0 <= index < len(self.decision.options), "option out of range"
        self._advance(index)

    def actor(self):
        return self.decision.player if self.decision else None

    def _run(self):
        try:
            yield from self._setup()
            while True:
                yield from self._turn()
        except GameOver as e:
            self.done = True
            self.winner = e.winner
            self.end_reason = e.reason
            self.say(f"Game over ({e.reason}): " + ("draw" if e.winner is None else f"P{e.winner} wins"))

    def say(self, msg):
        if self.logging:
            self.log.append(msg)

    # ============================================================== choices
    def ask(self, player, kind, options, source=None, prompt="", force=False):
        """Ask a player to pick one option. A single option is taken automatically."""
        if not options:
            return None
        if len(options) == 1 and not force:
            return options[0]
        i = yield Decision(player, kind, options, source, prompt)
        return options[i]

    def yes_no(self, player, source=None, prompt=""):
        opt = yield from self.ask(player, "yes_no", [Option("yes"), Option("no")], source, prompt, force=True)
        return opt.kind == "yes"

    def choose_cards(self, player, cards, min_n, max_n, source=None, kind="pick_card", prompt=""):
        """Pick up to max_n cards (at least min_n) out of a list. Returns the picked CardDefs."""
        pool = list(cards)
        picked = []
        while pool and len(picked) < max_n:
            opts = []
            seen = set()
            for c in pool:
                if c.id not in seen:
                    seen.add(c.id)
                    opts.append(Option("card", card=c))
            if len(picked) >= min_n:
                opts.append(Option("done"))
            if len(opts) == 1 and len(picked) < min_n and len(pool) <= min_n - len(picked):
                picked += pool  # forced: take what's left
                break
            opt = yield from self.ask(player, kind, opts, source, prompt)
            if opt.kind == "done":
                break
            pool.remove(opt.card)
            picked.append(opt.card)
        return picked

    def choose_pokemon(self, player, candidates, source=None, prompt="", optional=False):
        opts = [Option("pokemon", card=m.card, target=m) for m in candidates]
        if optional:
            opts.append(Option("done"))
        opt = yield from self.ask(player, "pick_pokemon", opts, source, prompt)
        return None if opt is None or opt.kind == "done" else opt.target

    def choose_energy(self, player, mon, n, source=None, pred=None, prompt=""):
        """Pick n energy cards attached to mon (all of them if it has n or fewer)."""
        pool = [e for e in mon.energy if pred is None or pred(e)]
        n = min(n, len(pool))
        if len({e.id for e in pool}) <= 1:
            return pool[:n]
        return (yield from self.choose_cards(player, pool, n, n, source, "pick_energy", prompt))

    def search_deck(self, player, pred, max_n, source=None, min_n=0, prompt=""):
        """Search your deck for up to max_n matching cards; they leave the deck. Shuffles."""
        p = self.players[player]
        pool = [c for c in p.deck if pred(c)]
        picked = yield from self.choose_cards(player, pool, min(min_n, len(pool)), max_n, source, "search", prompt)
        for c in picked:
            p.deck.remove(c)
        self.rng.shuffle(p.deck)
        return picked

    def choose_option(self, player, labels, source=None, prompt=""):
        opts = [Option("choice", data=i, label=l) for i, l in enumerate(labels)]
        opt = yield from self.ask(player, "pick_option", opts, source, prompt, force=True)
        return opt.data

    def place_counters(self, player, targets, n, source=None, prompt="", kind="attack"):
        """Put n damage counters on targets 'in any way you like', one counter at a time."""
        for _ in range(n):
            live = [m for m in targets if self.in_play(m) and not self.effect_immune(m, player, kind)]
            if not live:
                return
            opts = [Option("pokemon", card=m.card, target=m) for m in live]
            opt = yield from self.ask(player, "counters", opts, source, prompt)
            opt.target.damage += 10

    # ============================================================== queries
    def flip(self):
        heads = self.rng.random() < 0.5
        self.say("  coin: " + ("heads" if heads else "tails"))
        return heads

    def opponent(self, p):
        return self.players[1 - p.idx]

    def in_play(self, mon):
        p = self.players[mon.owner]
        return mon is p.active or mon in p.bench

    def is_active(self, mon):
        return self.players[mon.owner].active is mon

    def hook_sources(self, name):
        """(fn, src) for every active hook with this name."""
        out = []
        if self.stadium is not None:
            h = fx.trainer_hooks(self.stadium)
            if name in h:
                out.append((h[name], None))
        tools_on = None
        for q in self.players:
            for m in q.in_play():
                for hooks in fx.ability_hooks(m.card):
                    if name in hooks and not self.ability_blocked(m):
                        out.append((hooks[name], m))
                if m.tool is not None:
                    h = fx.trainer_hooks(m.tool)
                    if name in h:
                        if tools_on is None:
                            tools_on = not self.tools_blocked()
                        if tools_on:
                            out.append((h[name], m))
                for e in m.energy:
                    if not e.basic_energy:
                        h = fx.energy_hooks(e)
                        if name in h:
                            out.append((h[name], m))
        return out

    def fold(self, name, value, *args):
        for fn, src in self.hook_sources(name):
            value = fn(self, src, *args, value)
        return value

    def any_hook(self, name, *args):
        return any(fn(self, src, *args) for fn, src in self.hook_sources(name))

    def tools_blocked(self):
        if self.stadium is None:
            return False
        h = fx.trainer_impl(self.stadium)
        return bool(h and "tools_blocked" in h.hooks and h.hooks["tools_blocked"](self, None))

    def ability_blocked(self, mon):
        if self.stadium is not None:
            h = fx.trainer_impl(self.stadium)
            if h and "ability_blocked" in h.hooks and h.hooks["ability_blocked"](self, None, mon):
                return True
        for q in self.players:
            for m in q.in_play():
                for ab in m.card.abilities:
                    impl = fx.ABILITIES.get((m.name, ab.name))
                    if impl and impl.kind == "passive" and "ability_blocked" in impl.hooks and m is not mon:
                        if impl.hooks["ability_blocked"](self, m, mon):
                            return True
        return False

    def max_hp(self, mon):
        return self.fold("max_hp", mon.card.hp, mon)

    def hp_left(self, mon):
        return self.max_hp(mon) - mon.damage

    def bench_limit(self, p):
        return min(MAX_BENCH, self.fold("bench_limit", DEFAULT_BENCH, p))

    def card_units(self, mon, e):
        """The energy units one attached energy card provides."""
        if e.basic_energy:
            return [frozenset((e.energy_type,))] * self.fold("basic_energy_units", 1, mon, e)
        impl = fx.ENERGY.get(e.name)
        return impl.provides(self, mon) if impl else [frozenset((COLORLESS,))]

    def energy_units(self, mon):
        units = []
        for e in mon.energy:
            units += self.card_units(mon, e)
        return units

    def energy_count(self, mon, etype=None):
        units = self.energy_units(mon)
        return len(units) if etype is None else sum(etype in u for u in units)

    def attack_cost(self, mon, attack):
        return self.fold("attack_cost", list(attack.cost), mon, attack)

    def can_attack_with(self, mon, attack):
        if mon.marker("no_attack:" + attack.name, self.turn) or mon.marker("no_attack", self.turn):
            return False
        impl = fx.attack_impl(mon.card, attack)
        check = getattr(impl, "can_use", None)
        if check and not check(self, mon):
            return False
        return can_pay(self.energy_units(mon), self.attack_cost(mon, attack))

    def retreat_cost(self, mon):
        return max(0, self.fold("retreat_cost", mon.card.retreat, mon))

    def is_my_turn(self, p):
        return self.current == p.idx

    def can_evolve_now(self, mon, card):
        if mon.turn_entered < self.turn:
            return True
        return self.any_hook("evolve_same_turn", mon, card)

    def effect_immune(self, mon, by_player, kind="attack"):
        """Is mon protected from effects of by_player's attacks (kind='attack') or Abilities ('ability')?"""
        if mon.owner == by_player:
            return False
        if kind == "attack" and mon.marker("prevent_all", self.turn):
            return True
        return self.any_hook("no_effects", mon, kind)

    def apply_condition(self, mon, condition, by_player):
        if mon is None or not self.in_play(mon) or not self.is_active(mon):
            return False
        if self.effect_immune(mon, by_player) or self.any_hook("condition_immune", mon):
            return False
        if condition in ("asleep", "confused", "paralyzed"):
            mon.set_rotation(condition)
        else:
            setattr(mon, condition, True)
        return True

    def discard_stadium(self):
        if self.stadium is not None:
            self.players[self.stadium_owner].discard.append(self.stadium)
            self.stadium = self.stadium_owner = None

    def enforce_bench_limit(self, p):
        while len(p.bench) > self.bench_limit(p):
            mon = yield from self.choose_pokemon(p.idx, p.bench, prompt="discard down to the bench limit")
            self.discard_pokemon(mon)

    # ========================================================== board moves
    def new_pokemon(self, card, owner):
        self._uid += 1
        return Pokemon(card, owner, self.turn, self._uid)

    def draw(self, p, n=1):
        k = min(n, len(p.deck))
        for _ in range(k):
            p.hand.append(p.deck.pop())
        return k

    def shuffle(self, p):
        self.rng.shuffle(p.deck)

    def put_on_bench(self, p, card):
        mon = self.new_pokemon(card, p.idx)
        p.bench.append(mon)
        for fn, src in self.hook_sources("on_benched"):
            fn(self, src, mon)
        return mon

    def switch_active(self, p, bench_mon):
        """Swap p's Active with one of their Benched Pokemon."""
        old = p.active
        i = p.bench.index(bench_mon)
        old.leave_active()
        p.active, p.bench[i] = bench_mon, old
        bench_mon.moved_turn = self.turn

    def evolve(self, mon, card):
        mon.cards.append(card)
        mon.leave_active()
        mon.turn_entered = self.turn

    def discard_pokemon(self, mon):
        """Remove from play; every card on it goes to the discard pile."""
        p = self.players[mon.owner]
        p.discard += mon.cards + mon.energy + ([mon.tool] if mon.tool else [])
        if mon is p.active:
            p.active = None
        else:
            p.bench.remove(mon)

    def discard_energy(self, mon, cards):
        p = self.players[mon.owner]
        for e in cards:
            mon.energy.remove(e)
            p.discard.append(e)
            if e.name == "Boomerang Energy" and self.attacking and self.attacking.attacker is mon:
                self.attacking.boomerang.append(e)

    def heal(self, mon, n):
        mon.damage = max(0, mon.damage - n)

    def attach_energy(self, p, card, mon, from_zone):
        from_zone.remove(card)
        mon.energy.append(card)
        impl = fx.ENERGY.get(card.name)
        if impl and impl.on_attach and from_zone is p.hand:
            yield from run(impl.on_attach(self, p, mon))

    def attack_damage(self, ctx, target, dmg, apply_wr=True):
        """Deal attack damage to a Pokemon, applying every modifier. Returns the damage done."""
        if dmg <= 0 or target is None or not self.in_play(target):
            return 0
        attacker = ctx.attacker
        benched = not self.is_active(target)
        if benched and target.card.tera:
            return 0
        shielded = not (ctx.ignore_defender_effects and target is ctx.defender)
        if shielded and (target.marker("prevent_all", self.turn)
                         or (attacker.card.stage == 0 and target.marker("prevent_basic", self.turn))
                         or self.any_hook("prevent_damage", ctx, target)):
            return 0
        if target.owner != attacker.owner:
            dmg = self.fold("attack_damage", dmg, ctx, target)
            if not benched:
                dmg += _marker_total(attacker, "deal_more:", self.turn)
                owner = self.players[attacker.owner]
                if target.card.ex or target.card.mega:
                    dmg += _marker_total(owner, "bonus_vs_ex:", self.turn)
                if not attacker.card.rule_box:
                    dmg += _marker_total(owner, "bonus_nonrule:", self.turn)
                for t in attacker.card.types:
                    dmg += _marker_total(owner, f"bonus_type_{t}:", self.turn)
            dmg -= _marker_total(attacker, "deal_less:", self.turn)
        if apply_wr and not benched:
            weak = self.fold("weakness", target.card.weakness, target)
            if any(t in weak for t in attacker.card.types):
                dmg *= 2
            if not ctx.ignore_resistance and any(t in target.card.resistance for t in attacker.card.types):
                dmg -= 30
        if shielded:
            dmg = self.fold("damage_taken", dmg, ctx, target)
            dmg -= _marker_total(target, "take_less:", self.turn)
        dmg = max(0, dmg)
        if dmg:
            target.damage += dmg
            if target is ctx.defender:
                ctx.dealt += dmg
            self.say(f"  {target.name} takes {dmg} (now {target.damage}/{self.max_hp(target)})")
            for fn, src in self.hook_sources("after_damaged"):
                self._pending_triggers.append((fn, src, ctx, target, dmg))
        return dmg

    # ============================================================ knockouts
    def knockouts(self):
        """Resolve every Knocked Out Pokemon, prizes, wins and new Active Pokemon."""
        while True:
            kos = [m for q in self.players for m in q.in_play() if m.damage >= self.max_hp(m)]
            if not kos:
                break
            by_attack = self.attacking is not None
            for m in kos:
                q, taker = self.players[m.owner], self.players[1 - m.owner]
                attack_ko = by_attack and m.owner != self.current
                n = self.fold("prize_count", m.card.prizes, m, attack_ko)
                if attack_ko and m is q.active and taker.marker("briar", self.turn) \
                        and self.attacking.attacker.card.tera:
                    n += 1
                self.say(f"P{q.idx}'s {m.name} is Knocked Out")
                own = [(fn, src) for fn, src in self.hook_sources("on_knockout") if src is m]
                self.discard_pokemon(m)
                q.kod_turn = self.turn
                if m.card.owner_tag == "Team Rocket":
                    q.kod_tr_turn = self.turn
                for _ in range(min(n, len(taker.prizes))):
                    taker.hand.append(taker.prizes.pop(self.rng.randrange(len(taker.prizes))))
                for fn, src in own:
                    yield from run(fn(self, src, m, attack_ko))
            self._check_win()
        if not all(q.has_pokemon() for q in self.players):
            self._check_win()  # e.g. the only Pokemon shuffled itself into the deck
        order = [1 - self.current, self.current]
        for i in order:
            q = self.players[i]
            if q.active is None:
                mon = yield from self.choose_pokemon(i, q.bench, prompt="choose a new Active Pokemon")
                q.bench.remove(mon)
                q.active = mon
                mon.moved_turn = self.turn
                self.say(f"P{i} promotes {mon.name}")
        for q in self.players:
            yield from self.enforce_bench_limit(q)

    def _check_win(self):
        win = [not self.players[i].prizes or not self.players[1 - i].has_pokemon() for i in (0, 1)]
        if win[0] and win[1]:
            raise GameOver(None, "both players won at once")
        for i in (0, 1):
            if win[i]:
                raise GameOver(i, "took all prizes" if not self.players[i].prizes else "opponent has no Pokemon")

    # ================================================================ setup
    def _setup(self):
        for p in self.players:
            while True:
                self.rng.shuffle(p.deck)
                self.draw(p, HAND_SIZE)
                if any(c.is_basic for c in p.hand):
                    break
                p.mulligans += 1
                p.deck += p.hand
                p.hand = []
                if p.mulligans > 30:
                    raise ValueError(f"P{p.idx}'s deck has no Basic Pokemon")
        flip_winner = self.rng.randrange(2)
        opt = yield from self.ask(flip_winner, "go_first", [Option("first"), Option("second")], force=True)
        self.current = flip_winner if opt.kind == "first" else 1 - flip_winner
        self.say(f"P{self.current} goes first")
        for p in self.players:
            basics = [c for c in p.hand if c.is_basic]
            picked = yield from self.choose_cards(p.idx, basics, 1, 1, kind="setup_active")
            p.hand.remove(picked[0])
            p.active = self.new_pokemon(picked[0], p.idx)
            basics = [c for c in p.hand if c.is_basic]
            for c in (yield from self.choose_cards(p.idx, basics, 0, DEFAULT_BENCH, kind="setup_bench")):
                p.hand.remove(c)
                self.put_on_bench(p, c)
        for p in self.players:
            for _ in range(PRIZE_COUNT):
                p.prizes.append(p.deck.pop())
        for p in self.players:
            self.draw(p, self.players[1 - p.idx].mulligans)
        self.in_setup = False

    # ============================================================ turn flow
    def _turn(self):
        self.turn += 1
        if self.turn > self.max_turns:
            raise GameOver(None, "turn limit")
        p = self.players[self.current]
        p.energy_attached = p.supporter_played = p.retreated = p.stadium_played = False
        p.used = set()
        self._pending_triggers = []
        self.force_end_turn = False
        self.say(f"--- Turn {self.turn}: P{p.idx} ---")
        if not p.deck:
            raise GameOver(1 - p.idx, "opponent couldn't draw")
        self.draw(p)
        while True:
            opts = self.main_options(p)
            opt = yield from self.ask(p.idx, "main", opts, force=True)
            if opt.kind == "end":
                self.say(f"P{p.idx} ends their turn")
                break
            if opt.kind == "attack":
                yield from self._attack_turn(p, opt.data)
                break
            yield from self._do(p, opt)
            yield from self.knockouts()
            if self.force_end_turn:
                break
        for m in p.in_play():
            for e in [e for e in m.energy if e.name in fx.ENERGY and fx.ENERGY[e.name].hooks.get("discard_at_end_of_turn")]:
                self.discard_energy(m, [e])
        yield from self._checkup()

    def _checkup(self):
        p = self.players[self.current]
        p.turns_taken += 1
        for fn, src in self.hook_sources("checkup"):
            yield from run(fn(self, src))
        for q in self.players:
            a = q.active
            if a is None:
                continue
            if self.any_hook("condition_immune", a):
                a.clear_conditions()
            if a.poisoned:
                a.damage += a.poison_damage
            if a.burned:
                a.damage += 20
                if self.flip():
                    a.burned = False
            if a.asleep and self.flip():
                a.asleep = False
            if a.paralyzed and q.idx == self.current:
                a.paralyzed = False
        yield from self.knockouts()
        self.current = 1 - self.current

    def main_options(self, p):
        o = self.opponent(p)
        opts = [Option("end")]
        a = p.active
        can_act = not (a.asleep or a.paralyzed)
        if can_act and self.turn > 1:
            for atk in a.card.attacks:
                if self.can_attack_with(a, atk):
                    opts.append(Option("attack", card=a.card, target=a, data=atk.index, label=atk.name))
        if can_act and p.bench and not p.retreated and not a.marker("no_retreat", self.turn):
            if self.energy_count(a) >= self.retreat_cost(a):
                opts += [Option("retreat", card=b.card, target=b) for b in p.bench]
        for mon in p.in_play():
            for ab in mon.card.abilities:
                impl = fx.ABILITIES.get((mon.name, ab.name))
                if impl is None or impl.kind != "activated":
                    continue
                if self._ability_used(p, mon, ab, impl) or self.ability_blocked(mon):
                    continue
                if impl.can_use(self, p, mon):
                    opts.append(Option("ability", card=mon.card, target=mon, data=ab, label=ab.name))
        if self.stadium is not None:
            impl = fx.trainer_impl(self.stadium)
            if impl and impl.use and ("stadium", self.stadium.name) not in p.used and impl.can_use(self, p):
                opts.append(Option("stadium", card=self.stadium))
        bench_room = len(p.bench) < self.bench_limit(p)
        items_ok = not p.marker("no_items", self.turn)
        seen = set()
        for c in p.hand:
            if c.id in seen:
                continue
            seen.add(c.id)
            if c.is_pokemon:
                if c.stage == 0:
                    if bench_room:
                        opts.append(Option("bench", card=c))
                elif p.turns_taken > 0:
                    for m in p.in_play():
                        if m.name == c.evolves_from and self.can_evolve_now(m, c):
                            opts.append(Option("evolve", card=c, target=m))
            elif c.is_energy:
                if not p.energy_attached:
                    impl = fx.ENERGY.get(c.name)
                    ok = impl.hooks.get("can_attach") if impl else None
                    opts += [Option("attach", card=c, target=m) for m in p.in_play() if ok is None or ok(self, m)]
            else:
                impl = fx.trainer_impl(c)
                if impl is None:
                    continue
                if c.is_tool:
                    if items_ok:
                        opts += [Option("tool", card=c, target=m) for m in p.in_play()
                                 if m.tool is None and impl.can_attach(self, p, m)]
                elif c.is_supporter:
                    if not p.supporter_played and (self.turn > 1 or impl.first_turn_ok) and impl.can_play(self, p):
                        opts.append(Option("play", card=c))
                elif c.is_stadium:
                    if not p.stadium_played and (self.stadium is None or self.stadium.name != c.name):
                        opts.append(Option("play", card=c))
                elif items_ok and impl.can_play(self, p):
                    opts.append(Option("play", card=c))
        return opts

    def _ability_used(self, p, mon, ab, impl):
        if impl.once == "pokemon":
            return (mon.uid, ab.name) in p.used
        if impl.once == "name":
            return ("name", ab.name) in p.used
        return False

    def _mark_ability(self, p, mon, ab, impl):
        if impl.once == "pokemon":
            p.used.add((mon.uid, ab.name))
        elif impl.once == "name":
            p.used.add(("name", ab.name))

    def _do(self, p, opt):
        k, c, t = opt.kind, opt.card, opt.target
        if k == "bench":
            p.hand.remove(c)
            mon = self.put_on_bench(p, c)
            self.say(f"P{p.idx} benches {c.name}")
            yield from self._trigger(p, mon, "on_bench")
        elif k == "evolve":
            p.hand.remove(c)
            self.say(f"P{p.idx} evolves {t.name} into {c.name}")
            self.evolve(t, c)
            yield from self._trigger(p, t, "on_evolve")
        elif k == "attach":
            p.energy_attached = True
            self.say(f"P{p.idx} attaches {c.name} to {t.name}")
            yield from self.attach_energy(p, c, t, p.hand)
        elif k == "tool":
            p.hand.remove(c)
            t.tool = c
            self.say(f"P{p.idx} attaches {c.name} to {t.name}")
        elif k == "retreat":
            yield from self._retreat(p, t)
        elif k == "ability":
            impl = fx.ABILITIES[(t.name, opt.data.name)]
            self._mark_ability(p, t, opt.data, impl)
            self.say(f"P{p.idx} uses {opt.data.name} ({t.name})")
            yield from run(impl.use(self, p, t))
        elif k == "stadium":
            p.used.add(("stadium", c.name))
            self.say(f"P{p.idx} uses {c.name}")
            yield from run(fx.trainer_impl(c).use(self, p))
        elif k == "play":
            yield from self._play_trainer(p, c)
        yield from self._flush_triggers()

    def _trigger(self, p, mon, kind):
        for ab in mon.card.abilities:
            impl = fx.ABILITIES.get((mon.name, ab.name))
            if impl is None or impl.kind != kind or self.ability_blocked(mon):
                continue
            if not impl.can_use(self, p, mon) or self._ability_used(p, mon, ab, impl):
                continue
            if impl.optional and not (yield from self.yes_no(p.idx, mon.card, f"use {ab.name}?")):
                continue
            self._mark_ability(p, mon, ab, impl)
            self.say(f"P{p.idx} uses {ab.name}")
            yield from run(impl.use(self, p, mon))

    def _retreat(self, p, new_active):
        a = p.active
        cost = self.retreat_cost(a)
        if cost:
            paid = yield from self._choose_retreat_energy(p, a, cost)
            self.discard_energy(a, paid)
        p.retreated = True
        self.say(f"P{p.idx} retreats {a.name} for {new_active.name}")
        self.switch_active(p, new_active)

    def _choose_retreat_energy(self, p, mon, cost):
        # Pay with whole energy cards until the cost is covered
        pool, paid, covered = list(mon.energy), [], 0
        while covered < cost and pool:
            if len({e.id for e in pool}) == 1:
                e = pool[0]
            else:
                e = (yield from self.choose_cards(p.idx, pool, 1, 1, kind="discard", prompt="retreat cost"))[0]
            pool.remove(e)
            paid.append(e)
            covered += len(self.card_units(mon, e))
        return paid

    def _play_trainer(self, p, c):
        impl = fx.trainer_impl(c)
        p.hand.remove(c)
        self.say(f"P{p.idx} plays {c.name}")
        if c.is_supporter:
            p.supporter_played = True
            if c.name.startswith("Team Rocket's"):
                p.markers["tr_supporter"] = self.turn
        if c.is_stadium:
            p.stadium_played = True
            self.discard_stadium()
            self.stadium, self.stadium_owner = c, p.idx
            if impl.play:
                yield from run(impl.play(self, p))
            for q in (p, self.opponent(p)):
                yield from self.enforce_bench_limit(q)
            return
        result = yield from run(impl.play(self, p))
        p.discard.append(c)
        return result

    def _attack(self, p, index):
        a = p.active
        atk = a.card.attacks[index]
        self.say(f"P{p.idx}'s {a.name} uses {atk.name}")
        if a.confused and not self.flip():
            a.damage += 30
            self.say("  it hurt itself in confusion")
        else:
            ctx = AttackCtx(self, p, a, atk)
            self.attacking = ctx
            yield from run(fx.attack_impl(a.card, atk)(self, ctx))
            for e in ctx.boomerang:
                if e in p.discard and self.in_play(a):
                    p.discard.remove(e)
                    a.energy.append(e)
            yield from self._flush_triggers()
        yield from self.knockouts()
        self.attacking = None

    def _attack_turn(self, p, index):
        """An attack, plus a second one for Pokemon allowed to attack twice (Festival Lead)."""
        attacker = p.active
        yield from self._attack(p, index)
        if p.active is attacker and self.any_hook("attack_twice", attacker) \
                and not (attacker.asleep or attacker.paralyzed):
            opts = [Option("end")] + [Option("attack", card=attacker.card, target=attacker, data=atk.index,
                                             label=atk.name)
                                      for atk in attacker.card.attacks if self.can_attack_with(attacker, atk)]
            opt = yield from self.ask(p.idx, "main", opts, attacker.card, "attack again?")
            if opt.kind == "attack":
                yield from self._attack(p, opt.data)

    def _flush_triggers(self):
        while self._pending_triggers:
            fn, src, ctx, target, dmg = self._pending_triggers.pop(0)
            yield from run(fn(self, src, ctx, target, dmg))

    # ========================================================== for humans
    def describe(self, opt):
        k, c, t = opt.kind, opt.card, opt.target
        if k == "end":
            return "End turn"
        if k == "attack":
            atk = c.attacks[opt.data]
            return f"Attack: {atk.name} ({atk.base}{atk.mod})"
        if k == "retreat":
            return f"Retreat to {t.name}"
        if k == "ability":
            return f"Use {opt.data.name} ({t.name})"
        if k == "bench":
            return f"Bench {c.name}"
        if k == "evolve":
            return f"Evolve {t.name} into {c.name}"
        if k in ("attach", "tool"):
            return f"Attach {c.name} to {t.name}{' (Active)' if self.is_active(t) else ''}"
        if k == "play":
            return f"Play {c.name}"
        if k == "stadium":
            return f"Use Stadium {c.name}"
        if k == "card":
            return c.name
        if k == "pokemon":
            side = "your" if t.owner == self.decision.player else "opponent's"
            where = "Active" if self.is_active(t) else "Bench"
            return f"{side} {t.name} ({where}, {t.damage} dmg)"
        if k == "energy":
            return f"{c.name} on {t.name}"
        if k == "choice":
            return opt.label
        return {"done": "Done / stop", "yes": "Yes", "no": "No", "first": "Go first",
                "second": "Go second"}[k]
