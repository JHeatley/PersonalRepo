"""Play many games between two agents, running them side by side so neural agents batch."""
import random
from collections import defaultdict

from tcg.engine import Game


def play_match(agent_a, agent_b, decks, n_games, concurrency=64, seed=0, deck_a=None, deck_b=None):
    """Agent A vs agent B over n_games, alternating seats. decks: {name: card list}.

    Returns a dict of results, including per-matchup records keyed by (deck_a, deck_b).
    """
    rng = random.Random(seed)
    names = list(decks)
    started = 0
    live = []  # (game, seat_of_a, deck_name_a, deck_name_b)
    wins = draws = losses = 0
    matchups = defaultdict(lambda: [0, 0, 0])
    turns = 0

    def start():
        nonlocal started
        da = deck_a or rng.choice(names)
        db = deck_b or rng.choice(names)
        seat = started % 2
        pair = (decks[da], decks[db]) if seat == 0 else (decks[db], decks[da])
        live.append((Game(*pair, seed=rng.randrange(1 << 30)), seat, da, db))
        started += 1

    while started < min(concurrency, n_games):
        start()
    while live:
        for agent, mine in ((agent_a, True), (agent_b, False)):
            batch = [x for x in live if not x[0].done and (x[0].actor() == x[1]) == mine]
            if batch:
                for (g, *_), a in zip(batch, agent.act([x[0] for x in batch])):
                    g.step(a)
        still = []
        for x in live:
            g, seat, da, db = x
            if not g.done:
                still.append(x)
                continue
            turns += g.turn
            rec = matchups[(da, db)]
            if g.winner is None:
                draws += 1
                rec[1] += 1
            elif g.winner == seat:
                wins += 1
                rec[0] += 1
            else:
                losses += 1
                rec[2] += 1
        live[:] = still
        while started < n_games and len(live) < concurrency:
            start()
    n = wins + draws + losses
    return {"games": n, "wins": wins, "draws": draws, "losses": losses,
            "win_rate": (wins + 0.5 * draws) / max(n, 1), "avg_turns": turns / max(n, 1),
            "matchups": dict(matchups)}
