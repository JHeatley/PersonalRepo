"""Policy/value network over encoded decisions.

The state (board slots, hand, discards, globals) is encoded into a vector h.
Each option is encoded from its kind, its card and the board slot it
targets, then scored against h, so the same network handles every kind of
decision whatever the number of options. The value head predicts the final
result (+1 win / -1 loss) for the player making the decision.
"""
import numpy as np
import torch
import torch.nn as nn

from tcg.carddb import NUM_CARDS
from tcg.core import OPTION_KINDS
from .encode import (CARD_STATIC, NUM_SLOTS, SLOT_FEATURES, GLOBAL_FEATURES, OPTION_FEATURES, MEMORY_KEYS,
                     HISTORY_KEYS, TURN_FEATURES)

NEG_INF = -1e9


class CardEmbedding(nn.Module):
    """Learned per-card vector plus a projection of the card's printed stats."""

    def __init__(self, dim):
        super().__init__()
        self.ids = nn.Embedding(NUM_CARDS + 1, dim, padding_idx=0)
        self.register_buffer("static", torch.from_numpy(CARD_STATIC))
        self.proj = nn.Linear(CARD_STATIC.shape[1], dim)

    def forward(self, ids):
        return self.ids(ids) + self.proj(self.static[ids]) * (ids > 0).unsqueeze(-1)


class ResBlock(nn.Module):
    def __init__(self, width):
        super().__init__()
        self.net = nn.Sequential(nn.LayerNorm(width), nn.Linear(width, width), nn.GELU(), nn.Linear(width, width))

    def forward(self, x):
        return x + self.net(x)


def _bag(emb, ids):
    """Mean of embeddings over the non-empty entries, plus the count."""
    mask = (ids > 0).unsqueeze(-1).float()
    n = mask.sum(1)
    return torch.cat([(emb * mask).sum(1) / n.clamp(min=1), n / 20], -1)


class PolicyValueNet(nn.Module):
    def __init__(self, card_dim=48, slot_dim=96, width=384, blocks=3, option_dim=128, memory=True, history=True):
        super().__init__()
        self.config = {"card_dim": card_dim, "slot_dim": slot_dim, "width": width, "blocks": blocks,
                       "option_dim": option_dim, "memory": memory, "history": history}
        self.memory = memory
        self.history = history
        self.cards = CardEmbedding(card_dim)
        self.slot = nn.Sequential(nn.Linear(2 * card_dim + SLOT_FEATURES, slot_dim), nn.GELU(),
                                  nn.Linear(slot_dim, slot_dim))
        state_in = NUM_SLOTS * slot_dim + 3 * (card_dim + 1) + 2 * card_dim + GLOBAL_FEATURES
        self.base_inputs = state_in
        if memory:
            state_in += len(MEMORY_KEYS) * (card_dim + 1)
        if history:
            state_in += len(HISTORY_KEYS) * (card_dim + 1) + TURN_FEATURES
        self.inp = nn.Sequential(nn.Linear(state_in, width), nn.GELU())
        self.body = nn.Sequential(*[ResBlock(width) for _ in range(blocks)], nn.LayerNorm(width))
        self.kind = nn.Embedding(len(OPTION_KINDS), card_dim)
        opt_in = 2 * card_dim + slot_dim + OPTION_FEATURES
        self.option = nn.Sequential(nn.Linear(opt_in, option_dim), nn.GELU(), nn.Linear(option_dim, option_dim))
        self.query = nn.Linear(width, option_dim)
        self.score = nn.Sequential(nn.GELU(), nn.Linear(option_dim, option_dim), nn.GELU(), nn.Linear(option_dim, 1))
        self.value = nn.Sequential(nn.Linear(width, width // 2), nn.GELU(), nn.Linear(width // 2, 1))
        self.none_slot = nn.Parameter(torch.zeros(slot_dim))

    def forward(self, b):
        n = b["slot_card"].shape[0]
        slots = self.slot(torch.cat([self.cards(b["slot_card"]), self.cards(b["slot_tool"]), b["slot_feat"]], -1))
        slots = slots * (b["slot_card"] > 0).unsqueeze(-1)
        state = torch.cat([
            slots.reshape(n, -1),
            _bag(self.cards(b["hand"]), b["hand"]),
            _bag(self.cards(b["discards"][:, 0]), b["discards"][:, 0]),
            _bag(self.cards(b["discards"][:, 1]), b["discards"][:, 1]),
            self.cards(b["stadium"]), self.cards(b["source"]),
            b["globals"],
        ] + ([_bag(self.cards(b[k]), b[k]) for k in MEMORY_KEYS] if self.memory else [])
          + ([_bag(self.cards(b[k]), b[k]) for k in HISTORY_KEYS] + [b["turn_feats"]] if self.history else []), -1)
        h = self.body(self.inp(state))

        # Option encodings; target index NUM_SLOTS means "no target"
        slots_plus = torch.cat([slots, self.none_slot.expand(n, 1, -1)], 1)
        target = torch.gather(slots_plus, 1, b["opt_target"].unsqueeze(-1).expand(-1, -1, slots.shape[-1]))
        o = self.option(torch.cat([self.kind(b["opt_kind"]), self.cards(b["opt_card"]), target, b["opt_feat"]], -1))
        logits = self.score(o + self.query(h).unsqueeze(1)).squeeze(-1)
        logits = logits.masked_fill(~b["opt_mask"], NEG_INF)
        return logits, torch.tanh(self.value(h)).squeeze(-1)


def to_torch(batch, device="cpu"):
    return {k: torch.from_numpy(v if isinstance(v, np.ndarray) else np.asarray(v)).to(device)
            for k, v in batch.items()}


def save(model, path, **extra):
    torch.save({"config": model.config, "state_dict": model.state_dict(), "num_cards": NUM_CARDS, **extra}, path)


def load(path, device="cpu"):
    """Load a checkpoint. Models trained before the memory inputs existed are upgraded: the new
    inputs start with zero weights, so the model plays exactly as before until it trains on them."""
    ckpt = torch.load(path, map_location=device, weights_only=False)
    if ckpt["num_cards"] != NUM_CARDS:
        raise ValueError(f"{path} was trained on a different card database; retrain it")
    config = dict(ckpt["config"])
    state = ckpt["state_dict"]
    if not config.get("memory", False) or not config.get("history", False):
        # New inputs are appended after the old ones, so pad the first layer with zero columns
        config["memory"] = config["history"] = True
        old = state["inp.0.weight"]
        new = torch.zeros_like(PolicyValueNet(**config).inp[0].weight)
        new[:, :old.shape[1]] = old
        state = dict(state, **{"inp.0.weight": new})
    model = PolicyValueNet(**config).to(device)
    model.load_state_dict(state)
    model.eval()
    return model, ckpt
