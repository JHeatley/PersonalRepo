"""Self-play data collection for PPO.

Each game puts the learner in one or both seats. In a self-play game both
seats are the learner and both sides' decisions become training data; in a
pool game the other seat is a frozen past checkpoint or the heuristic bot,
and only the learner's decisions are kept.
"""
import random

import numpy as np
import torch

from tcg.engine import Game
from .agents import NeuralAgent, HeuristicAgent


def _progress(g, seat):
    """Prizes taken by seat minus prizes taken by the opponent."""
    return len(g.players[1 - seat].prizes) - len(g.players[seat].prizes)


def collect(model, opponents, decks, n_games, concurrency=64, seed=0, gamma=0.999, lam=0.95,
            prize_reward=0.05, focus=None):
    """Play n_games and return (samples, stats).

    opponents: list of (agent or None, weight); None means self-play.
    focus: a deck name that is in every game, played by the learner's seat
           (the other seat gets a random deck from the pool).
    samples: list of dicts with enc, action, logp, value, adv, ret.
    """
    rng = random.Random(seed)
    learner = NeuralAgent(model)
    names = list(decks)
    weights = [w for _, w in opponents]
    samples = []
    stats = {"games": 0, "learner_wins": 0, "draws": 0, "decisions": 0, "turns": 0, "vs_pool": 0, "pool_wins": 0}
    started = 0
    live = []

    def start():
        nonlocal started
        opp = rng.choices(opponents, weights)[0][0]
        seat = rng.randrange(2)
        mine = focus or rng.choice(names)
        pair = (mine, rng.choice(names)) if seat == 0 else (rng.choice(names), mine)
        g = Game(decks[pair[0]], decks[pair[1]], seed=rng.randrange(1 << 30))
        live.append({"g": g, "opp": opp, "seat": seat, "traj": {0: [], 1: []}})
        started += 1

    while started < min(concurrency, n_games):
        start()
    while live:
        groups = {}
        for x in live:
            if x["g"].done:
                continue
            who = x["g"].actor()
            agent = None if (x["opp"] is None or who == x["seat"]) else x["opp"]
            groups.setdefault(id(agent), (agent, []))[1].append(x)
        for agent, batch in groups.values():
            games = [x["g"] for x in batch]
            if agent is None:
                actions, logps, values, encs = learner.act_full(games)
                for x, a, lp, v, e in zip(batch, actions, logps, values, encs):
                    seat = x["g"].actor()
                    x["traj"][seat].append([e, a, float(lp), float(v), _progress(x["g"], seat)])
            else:
                actions = agent.act(games)
            for g, a in zip(games, actions):
                g.step(a)
            stats["decisions"] += len(games)
        still = []
        for x in live:
            g = x["g"]
            if not g.done:
                still.append(x)
                continue
            stats["games"] += 1
            stats["turns"] += g.turn
            if g.winner is None:
                stats["draws"] += 1
            if x["opp"] is not None:
                stats["vs_pool"] += 1
                stats["pool_wins"] += g.winner == x["seat"]
            stats["learner_wins"] += g.winner is not None and (x["opp"] is None or g.winner == x["seat"])
            for seat, traj in x["traj"].items():
                if traj:
                    outcome = 0.0 if g.winner is None else (1.0 if g.winner == seat else -1.0)
                    samples += _finish(traj, outcome, _progress(g, seat), gamma, lam, prize_reward)
        live[:] = still
        while started < n_games and len(live) < concurrency:
            start()
    return samples, stats


def _finish(traj, outcome, final_progress, gamma, lam, prize_reward):
    """GAE over one player's decisions in one game."""
    n = len(traj)
    rewards = np.zeros(n, dtype=np.float32)
    for t in range(n):
        nxt = traj[t + 1][4] if t + 1 < n else final_progress
        rewards[t] = prize_reward * (nxt - traj[t][4])
    rewards[-1] += outcome
    values = np.array([s[3] for s in traj], dtype=np.float32)
    adv = np.zeros(n, dtype=np.float32)
    last = 0.0
    for t in reversed(range(n)):
        next_v = values[t + 1] if t + 1 < n else 0.0
        delta = rewards[t] + gamma * next_v - values[t]
        last = delta + gamma * lam * last
        adv[t] = last
    ret = adv + values
    return [{"enc": s[0], "action": s[1], "logp": s[2], "value": s[3], "adv": float(adv[t]), "ret": float(ret[t])}
            for t, s in enumerate(traj)]


def worker_collect(args):
    """Entry point for collection worker processes."""
    torch.set_num_threads(1)
    from .model import PolicyValueNet
    state, config, pool_states, heuristic_w, self_w, pool_w, decks, n_games, concurrency, seed, kw = args
    model = PolicyValueNet(**config)
    model.load_state_dict(state)
    model.eval()
    opponents = [(None, self_w), (HeuristicAgent(seed), heuristic_w)]
    for st in pool_states:
        m = PolicyValueNet(**config)
        m.load_state_dict(st)
        m.eval()
        opponents.append((NeuralAgent(m), pool_w / len(pool_states)))
    opponents = [(a, w) for a, w in opponents if w > 0]
    return collect(model, opponents, decks, n_games, concurrency, seed, **kw)
