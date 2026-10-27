"""Agents answer Decisions for a batch of games at once: act(games) -> option indices."""
import random

import numpy as np
import torch

from .encode import encode, collate
from .model import to_torch


class RandomAgent:
    name = "random"

    def __init__(self, seed=None):
        self.rng = random.Random(seed)

    def act(self, games):
        return [self.rng.randrange(len(g.decision.options)) for g in games]


class HeuristicAgent:
    """Hand-written baseline: develop the board, then attack with the biggest hit."""
    name = "heuristic"

    def __init__(self, seed=None):
        self.rng = random.Random(seed)

    def act(self, games):
        return [self._pick(g) for g in games]

    def _pick(self, g):
        d = g.decision
        scores = [self._score(g, d, o) + self.rng.random() * 0.1 for o in d.options]
        return max(range(len(scores)), key=scores.__getitem__)

    def _score(self, g, d, o):
        me = g.players[d.player]
        k = o.kind
        if d.kind == "main":
            if k == "end":
                return 0
            if k == "attack":
                atk = o.card.attacks[o.data]
                return 5 + atk.base / 100
            if k == "evolve":
                return 60
            if k == "bench":
                return 50 if len(me.bench) < 4 else 3
            if k == "attach":
                need = any(len(a.cost) > g.energy_count(o.target) for a in o.target.card.attacks)
                return (55 if g.is_active(o.target) else 45) if need else 2
            if k in ("play", "ability", "stadium"):
                return 40
            if k == "tool":
                return 30
            if k == "retreat":
                a = me.active
                stuck = not any(g.can_attack_with(a, atk) for atk in a.card.attacks)
                ready = any(g.can_attack_with(o.target, atk) for atk in o.target.card.attacks)
                return 8 if stuck and ready else -1
            return 1
        if k in ("yes", "first"):
            return 1
        if k == "done":
            return -1
        if k == "pokemon" and o.target is not None:
            mine = o.target.owner == d.player
            if d.kind in ("pick_pokemon", "counters") and not mine:
                return o.target.damage / 100 + o.target.card.prizes
            if d.prompt.startswith(("new Active", "choose a new Active")):
                return max((a.base for a in o.target.card.attacks if g.can_attack_with(o.target, a)), default=0) / 100
        if k == "card" and d.kind in ("discard",):
            return -(o.card.is_supporter or o.card.is_pokemon)
        return 0


class NeuralAgent:
    """Policy network agent. greedy=True picks the top option; otherwise it samples."""
    name = "neural"

    def __init__(self, model, greedy=False, device="cpu"):
        self.model = model
        self.greedy = greedy
        self.device = device

    @torch.no_grad()
    def evaluate(self, games):
        encs = [encode(g) for g in games]
        logits, values = self.model(to_torch(collate(encs), self.device))
        return encs, logits, values

    @torch.no_grad()
    def act(self, games):
        return self.act_full(games)[0]

    @torch.no_grad()
    def act_full(self, games):
        """Returns (actions, log-probs, values, encodings)."""
        encs, logits, values = self.evaluate(games)
        if self.greedy:
            actions = logits.argmax(-1)
        else:
            actions = torch.distributions.Categorical(logits=logits).sample()
        logp = torch.log_softmax(logits, -1).gather(1, actions.unsqueeze(1)).squeeze(1)
        return actions.tolist(), logp.numpy(), values.numpy(), encs


def make_agent(spec, device="cpu"):
    """'random', 'heuristic', or a checkpoint path (optionally ':greedy')."""
    if spec == "random":
        return RandomAgent()
    if spec == "heuristic":
        return HeuristicAgent()
    from .model import load
    path, _, mode = spec.partition(":")
    model, _ = load(path, device)
    agent = NeuralAgent(model, greedy=(mode == "greedy"), device=device)
    agent.name = spec
    return agent


def np_softmax(x):
    e = np.exp(x - x.max())
    return e / e.sum()
