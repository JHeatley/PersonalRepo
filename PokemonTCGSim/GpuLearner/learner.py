"""GPU learner for the sim's Neural AI.

The sim (Unity, or the command-line trainer in ../Headless) plays the training games on the CPU and sends each
round's decisions here; this learns from them on the graphics card with PyTorch (the same PPO the sim does on the
CPU) and sends the new weights back. Learning is most of the training time, so this roughly doubles or triples
the games per second, and takes the heaviest maths off the CPU.

    pip uninstall -y torch                                                   (once: the CPU-only build can't use the card)
    pip install torch --index-url https://download.pytorch.org/whl/cu128     (PyTorch with CUDA; cu126 if there's no match)
    python learner.py                       # waits for the sim on 127.0.0.1:5577, learns on the GPU
    python learner.py --device cpu          # no NVIDIA card: PyTorch on the CPU (still faster than the sim's own)

Then tick "Learn on the GPU" on the sim's Train screen (or give the command-line trainer --gpu 127.0.0.1:5577).

The network is the sim's own (PolicyNet.cs): the same layers and weight names as the Python bot's model, but the
card table is the sim's, which grows as it meets new cards. Nothing here reads or changes PokemonTCGBot.

Messages (little-endian): 4 ASCII bytes of type, int64 payload length, payload. Strings: int32 byte length + UTF-8.
  TRN1 (sim -> here): int32 version, int32 epochs, int32 minibatch, int32 seed, f64 clip, f64 dual clip, f64 value
       coef, f64 entropy coef, f64 learning rate, f64 max grad norm, weights, samples
  RES1 (here -> sim): f64 policy loss, f64 value loss, f64 entropy, f64 clip fraction, int32 batches, int32 skipped,
       f64 seconds, str device, weights (learned card rows only, no static stats)
  FWD1 (sim -> here): int32 version, weights, samples      ->  OUT1: per sample its option logits then its value
  ERR1 (here -> sim): str message
weights: int32 tensor count; each: str name, int32 ndim, int32 dims..., f32 data; then int32 card rows R (row 0 = no
         card), f32[R*48] learned rows, and (sim -> here only) f32[R*64] printed stats
samples: int32 N, slots, slot features, global features, turn features, option features, bags; int32 slot cards
         [N*slots], slot tools [N*slots]; f32 slot features; per bag: int32 lengths[N], int32 ids[sum]; int32 stadium[N],
         source[N]; f32 globals, turn features; int32 option counts[N]; int32 kinds, cards, targets [sum]; f32 option
         features; int32 actions[N]; f32 old log-probabilities[N], advantages[N], returns[N]
Choosing moves here (the sim's "choose moves on the GPU too"): the learner keeps the network between messages.
  SETW (sim -> here): int32 version, weights                  ->  OKW1 (empty): the network to choose moves with
  CARD (sim -> here): int32 version, int32 first row, int32 rows, f32 learned rows, f32 printed stats  ->  OKW1: new cards
  INF1 (sim -> here): int32 version, samples                  ->  OUT1: per sample its option logits then its value
  After TRN1 the learner keeps the weights it sent back, so moves are chosen with them without another SETW.
Version 2 (training from the look-ahead: some decisions carry the look-ahead's improved choice): TRN1 adds f64
         distillation weight after max grad norm, the samples end with f32 has-target[N] and f32 targets[sum of option
         counts], and RES1 ends with f64 distillation loss. Those decisions learn to match their target (cross-entropy)
         instead of PPO. Version 1 messages work as before.
Version 3 (another network than the AI's: an exploiter, trained only to beat the AI's network): int32 3, int32 slot,
         then a whole version 1 or 2 message. The learner keeps a model and optimizer per slot, so the AI's own (slot 0,
         every message without version 3) carries on where it was. Only the latest other slot is kept.
"""
import argparse
import io
import socket
import struct
import sys
import time

import numpy as np
import torch
import torch.nn as nn
import torch.nn.functional as F

VERSION = 1
VERSIONS = (1, 2)
VERSION_SLOT = 3
CARD_DIM, STATIC_SIZE = 48, 64
NEG_INF = -1e9


# ---------------------------------------------------------------------------------------------------- reading/writing

class Reader:
    def __init__(self, data):
        self.b = memoryview(data)
        self.p = 0

    def take(self, n):
        v = self.b[self.p:self.p + n]
        self.p += n
        return v

    def i32(self):
        return struct.unpack_from("<i", self.take(4))[0]

    def f64(self):
        return struct.unpack_from("<d", self.take(8))[0]

    def str(self):
        return bytes(self.take(self.i32())).decode("utf-8")

    def ints(self, n):
        return np.frombuffer(self.take(4 * n), dtype="<i4").astype(np.int64)

    def floats(self, n):
        return np.frombuffer(self.take(4 * n), dtype="<f4").astype(np.float32)


def w_i32(f, v):
    f.write(struct.pack("<i", v))


def w_f64(f, v):
    f.write(struct.pack("<d", float(v)))


def w_str(f, s):
    b = s.encode("utf-8")
    w_i32(f, len(b))
    f.write(b)


def w_floats(f, a):
    f.write(np.ascontiguousarray(a, dtype="<f4").tobytes())


def read_weights(r, with_static=True):
    tensors = {}
    for _ in range(r.i32()):
        name = r.str()
        shape = [r.i32() for _ in range(r.i32())]
        n = int(np.prod(shape)) if shape else 1
        tensors[name] = r.floats(n).reshape(shape)
    rows = r.i32()
    learned = r.floats(rows * CARD_DIM).reshape(rows, CARD_DIM)
    static = r.floats(rows * STATIC_SIZE).reshape(rows, STATIC_SIZE) if with_static else None
    return tensors, learned, static


def read_targets(r, s):
    """Version 2: which decisions carry the look-ahead's improved choice, and the choices (padded like the options)."""
    n, counts = s["n"], s["counts"]
    s["has_target"] = r.floats(n).astype(np.float32)
    flat = r.floats(int(counts.sum()))
    width = s["opt_mask"].shape[1]
    s["target"] = np.zeros((n, width), np.float32)
    pos = 0
    for i in range(n):
        c = counts[i]
        s["target"][i, :c] = flat[pos:pos + c]
        pos += c


def read_samples(r):
    n, slots, slot_feat, glob, turn, opt_feat, bags = (r.i32() for _ in range(7))
    s = {"n": n, "slots": slots}
    s["slot_card"] = r.ints(n * slots).reshape(n, slots)
    s["slot_tool"] = r.ints(n * slots).reshape(n, slots)
    s["slot_feat"] = r.floats(n * slots * slot_feat).reshape(n, slots, slot_feat)
    s["bags"] = []
    for _ in range(bags):
        lengths = r.ints(n)
        ids = r.ints(int(lengths.sum()))
        s["bags"].append(_pad(lengths, ids))
    s["stadium"] = r.ints(n)
    s["source"] = r.ints(n)
    s["globals"] = r.floats(n * glob).reshape(n, glob)
    s["turn"] = r.floats(n * turn).reshape(n, turn)
    counts = r.ints(n)
    k = int(counts.sum())
    kinds, cards, targets = r.ints(k), r.ints(k), r.ints(k)
    feats = r.floats(k * opt_feat).reshape(k, opt_feat)
    width = max(1, int(counts.max()) if n else 1)
    starts = np.concatenate([[0], np.cumsum(counts)[:-1]]) if n else np.zeros(0, dtype=np.int64)
    s["opt_kind"] = np.zeros((n, width), np.int64)
    s["opt_card"] = np.zeros((n, width), np.int64)
    s["opt_target"] = np.full((n, width), slots, np.int64)  # padding points at "no target"
    s["opt_feat"] = np.zeros((n, width, opt_feat), np.float32)
    s["opt_mask"] = np.zeros((n, width), bool)
    for i in range(n):
        a, c = starts[i], counts[i]
        s["opt_kind"][i, :c] = kinds[a:a + c]
        s["opt_card"][i, :c] = cards[a:a + c]
        s["opt_target"][i, :c] = targets[a:a + c]
        s["opt_feat"][i, :c] = feats[a:a + c]
        s["opt_mask"][i, :c] = True
    s["counts"] = counts
    s["action"] = r.ints(n)
    s["logp"] = r.floats(n)
    s["adv"] = r.floats(n)
    s["ret"] = r.floats(n)
    return s


def _pad(lengths, ids):
    n = len(lengths)
    width = max(1, int(lengths.max()) if n else 1)
    out = np.zeros((n, width), np.int64)
    pos = 0
    for i, ln in enumerate(lengths):
        out[i, :ln] = ids[pos:pos + ln]
        pos += ln
    return out


def write_weights(f, model):
    named = [(k, v) for k, v in model.named_parameters() if k != "cards.ids.weight"]
    w_i32(f, len(named))
    for name, t in named:
        w_str(f, name)
        w_i32(f, t.dim())
        for d in t.shape:
            w_i32(f, d)
        w_floats(f, t.detach().float().cpu().numpy())
    rows = model.cards.ids.weight
    w_i32(f, rows.shape[0])
    w_floats(f, rows.detach().float().cpu().numpy())


# ---------------------------------------------------------------------------------------------------- the network

class Cards(nn.Module):
    """The card table: a learned row per card (row 0 = no card) plus a projection of its printed stats."""

    def __init__(self, rows):
        super().__init__()
        self.ids = nn.Embedding(rows, CARD_DIM)
        self.register_buffer("static", torch.zeros(rows, STATIC_SIZE))
        self.proj = nn.Linear(STATIC_SIZE, CARD_DIM)

    def forward(self, ids):
        return (self.ids(ids) + self.proj(self.static[ids])) * (ids > 0).unsqueeze(-1)


class ResBlock(nn.Module):
    def __init__(self, width):
        super().__init__()
        self.net = nn.Sequential(nn.LayerNorm(width), nn.Linear(width, width), nn.GELU(), nn.Linear(width, width))

    def forward(self, x):
        return x + self.net(x)


class SimNet(nn.Module):
    """PolicyNet.cs in PyTorch: sizes read from the weights the sim sends."""

    def __init__(self, tensors, rows):
        super().__init__()
        slot_dim, slot_in = tensors["slot.0.weight"].shape
        width, state_in = tensors["inp.0.weight"].shape
        option_dim, option_in = tensors["option.0.weight"].shape
        kinds = tensors["kind.weight"].shape[0]
        blocks = sum(1 for k in tensors if k.endswith(".net.1.weight"))
        self.cards = Cards(rows)
        self.slot = nn.Sequential(nn.Linear(slot_in, slot_dim), nn.GELU(), nn.Linear(slot_dim, slot_dim))
        self.inp = nn.Sequential(nn.Linear(state_in, width), nn.GELU())
        self.body = nn.Sequential(*[ResBlock(width) for _ in range(blocks)], nn.LayerNorm(width))
        self.kind = nn.Embedding(kinds, CARD_DIM)
        self.option = nn.Sequential(nn.Linear(option_in, option_dim), nn.GELU(), nn.Linear(option_dim, option_dim))
        self.query = nn.Linear(width, option_dim)
        self.score = nn.Sequential(nn.GELU(), nn.Linear(option_dim, option_dim), nn.GELU(), nn.Linear(option_dim, 1))
        self.value = nn.Sequential(nn.Linear(width, width // 2), nn.GELU(), nn.Linear(width // 2, 1))
        self.none_slot = nn.Parameter(torch.zeros(slot_dim))

    @staticmethod
    def _bag(vectors, ids):
        mask = (ids > 0).unsqueeze(-1).float()
        n = mask.sum(1)
        return torch.cat([(vectors * mask).sum(1) / n.clamp(min=1), n / 20], -1)

    def forward(self, b):
        n = b["slot_card"].shape[0]
        cv = self.cards
        slots = self.slot(torch.cat([cv(b["slot_card"]), cv(b["slot_tool"]), b["slot_feat"]], -1))
        slots = slots * (b["slot_card"] > 0).unsqueeze(-1)
        bags = [self._bag(cv(ids), ids) for ids in b["bags"]]
        # The sim's state layout: slots, hand, your discard, their discard, stadium, source, globals,
        # memory bags (unseen, their known hand, cards they've shown), this turn's plays, their last turn, turn features.
        state = torch.cat([slots.reshape(n, -1), bags[0], bags[1], bags[2], cv(b["stadium"]), cv(b["source"]), b["globals"],
                           bags[3], bags[4], bags[5], bags[6], bags[7], b["turn"]], -1)
        h = self.body(self.inp(state))
        slots_plus = torch.cat([slots, self.none_slot.expand(n, 1, -1)], 1)
        target = torch.gather(slots_plus, 1, b["opt_target"].unsqueeze(-1).expand(-1, -1, slots.shape[-1]))
        o = self.option(torch.cat([self.kind(b["opt_kind"]), cv(b["opt_card"]), target, b["opt_feat"]], -1))
        logits = self.score(o + self.query(h).unsqueeze(1)).squeeze(-1)
        logits = logits.masked_fill(~b["opt_mask"], NEG_INF)
        return logits, torch.tanh(self.value(h)).squeeze(-1)


def batch(s, idx, device):
    def t(a, dtype=None):
        return torch.as_tensor(a[idx], dtype=dtype, device=device)
    b = {"slot_card": t(s["slot_card"]), "slot_tool": t(s["slot_tool"]), "slot_feat": t(s["slot_feat"]),
         "bags": [t(x) for x in s["bags"]], "stadium": t(s["stadium"]), "source": t(s["source"]),
         "globals": t(s["globals"]), "turn": t(s["turn"]), "opt_kind": t(s["opt_kind"]), "opt_card": t(s["opt_card"]),
         "opt_target": t(s["opt_target"]), "opt_feat": t(s["opt_feat"]), "opt_mask": t(s["opt_mask"])}
    return b


# ---------------------------------------------------------------------------------------------------- learning

class Learner:
    def __init__(self, device):
        self.device = torch.device(device)
        self.model = None
        self.opt = None
        self.lr = None
        self.inferred = 0
        self.slot = 0
        self.slots = {}

    def select(self, slot):
        """Works on slot's model and optimizer from now on (0: the AI's own network)."""
        if slot == self.slot:
            return
        self.slots[self.slot] = (self.model, self.opt, self.lr)
        if slot != 0:
            # Only the AI's network and the latest exploiter are kept.
            for other in [k for k in self.slots if k not in (0, slot)]:
                del self.slots[other]
        self.model, self.opt, self.lr = self.slots.pop(slot, (None, None, None))
        self.slot = slot

    def _sync(self, tensors, learned, static, lr):
        """Puts the sim's current weights into the model, keeping the optimizer's state from round to round."""
        rows = learned.shape[0]
        fresh = self.model is None or self.model.cards.ids.weight.shape[0] > rows
        carried = None
        if not fresh and set(tensors) != self._names():
            # The network was made bigger in the sim (more blocks): a new model, keeping the optimizer's state for the
            # weights it already had.
            fresh = True
            if self.opt is not None:
                carried = {n: self.opt.state[p] for n, p in self.model.named_parameters() if p in self.opt.state}
        if fresh:
            self.model = SimNet(tensors, rows).to(self.device)
            self.opt = None
        elif self.model.cards.ids.weight.shape[0] < rows:
            self._grow(rows)
        with torch.no_grad():
            params = dict(self.model.named_parameters())
            for name, value in tensors.items():
                if name not in params or tuple(params[name].shape) != value.shape:
                    raise ValueError(f"the sim's weight {name} {value.shape} doesn't fit this network")
                params[name].copy_(torch.from_numpy(value))
            self.model.cards.ids.weight.copy_(torch.from_numpy(learned))
            self.model.cards.static.copy_(torch.from_numpy(static))
        if self.opt is None or self.lr != lr:
            old = self.opt
            self.opt = torch.optim.Adam(self.model.parameters(), lr=lr, betas=(0.9, 0.999), eps=1e-8)
            if old is not None:
                self.opt.load_state_dict(old.state_dict())
                for g in self.opt.param_groups:
                    g["lr"] = lr
            self.lr = lr
        if carried:
            for n, p in self.model.named_parameters():
                st = carried.get(n)
                if st and all(not torch.is_tensor(v) or v.dim() == 0 or v.shape == p.shape for v in st.values()):
                    self.opt.state[p] = st

    def _names(self):
        """The weights the sim sends for this model (all but the card table's rows)."""
        return {n for n, _ in self.model.named_parameters()} - {"cards.ids.weight"}

    def _grow(self, rows):
        """New cards: a bigger card table, carrying over the old rows' optimizer state."""
        old = self.model.cards.ids.weight
        state = self.opt.state.pop(old, None) if self.opt is not None else None
        bigger = nn.Parameter(torch.zeros(rows, CARD_DIM, device=self.device))
        self.model.cards.ids.weight = bigger
        self.model.cards.static = torch.zeros(rows, STATIC_SIZE, device=self.device)
        if self.opt is not None:
            for g in self.opt.param_groups:
                g["params"] = [bigger if p is old else p for p in g["params"]]
            if state:
                grown = {}
                for k, v in state.items():
                    if torch.is_tensor(v) and v.dim() == 2:
                        pad = torch.zeros(rows, CARD_DIM, device=self.device, dtype=v.dtype)
                        pad[:v.shape[0]] = v
                        grown[k] = pad
                    else:
                        grown[k] = v
                self.opt.state[bigger] = grown

    def train(self, r, version=1):
        start = time.time()
        epochs, minibatch, seed = r.i32(), r.i32(), r.i32()
        clip, dual, value_coef, ent_coef, lr, max_norm = (r.f64() for _ in range(6))
        distill = r.f64() if version >= 2 else 0.0
        tensors, learned, static = read_weights(r)
        s = read_samples(r)
        if version >= 2:
            read_targets(r, s)
        self._sync(tensors, learned, static, lr)
        model, dev = self.model, self.device
        n = s["n"]
        adv_all = s["adv"].astype(np.float64)
        adv_all = ((adv_all - adv_all.mean()) / (adv_all.std() + 1e-8)).astype(np.float32)
        rng = np.random.default_rng(seed)
        stats = np.zeros(4)
        distill_sum = distill_n = 0.0
        batches = skipped = 0
        model.train()
        for _ in range(epochs):
            order = rng.permutation(n)
            for a in range(0, n, minibatch):
                idx = order[a:a + minibatch]
                b = batch(s, idx, dev)
                act = torch.as_tensor(s["action"][idx], device=dev)
                old = torch.as_tensor(s["logp"][idx], device=dev)
                adv = torch.as_tensor(adv_all[idx], device=dev)
                ret = torch.as_tensor(s["ret"][idx], device=dev).clamp(-1.5, 1.5)
                logits, value = model(b)
                logp_all = F.log_softmax(logits, -1)
                p = logp_all.exp()
                entropy = -(p * logp_all).masked_fill(~b["opt_mask"], 0).sum(-1)
                logp = logp_all.gather(1, act.unsqueeze(1)).squeeze(1)
                ratio = torch.exp(logp - old)
                pg = -torch.min(ratio * adv, ratio.clamp(1 - clip, 1 + clip) * adv)
                # Dual clip, as the sim does: a decision with a negative advantage whose probability grew a lot
                # can't produce a huge step.
                pg = torch.where((adv < 0) & (ratio > dual), (-dual * adv).detach(), pg)
                v_err = value - ret
                loss = value_coef * (v_err ** 2).mean() - ent_coef * entropy.mean()
                if version >= 2:
                    # Decisions the look-ahead made learn to match its improved choice (cross-entropy), not PPO.
                    has = torch.as_tensor(s["has_target"][idx], device=dev)
                    tgt = torch.as_tensor(s["target"][idx], device=dev)
                    ce = -(tgt * logp_all.masked_fill(~b["opt_mask"], 0)).sum(-1)
                    pg = pg * (1 - has)
                    loss = loss + distill * (ce * has).mean()
                    with torch.no_grad():
                        distill_sum += (ce * has).sum().item()
                        distill_n += has.sum().item()
                loss = loss + pg.mean()
                self.opt.zero_grad(set_to_none=True)
                loss.backward()
                norm = torch.nn.utils.clip_grad_norm_(model.parameters(), max_norm)
                if not torch.isfinite(norm):
                    skipped += 1  # a step this broken would ruin every weight: skip it
                    continue
                self.opt.step()
                with torch.no_grad():
                    stats += [pg.mean().item(), (v_err ** 2).mean().item(), entropy.mean().item(),
                              ((ratio - 1).abs() > clip).float().mean().item()]
                batches += 1
        stats /= max(1, batches)
        out = io.BytesIO()
        for v in stats:
            w_f64(out, v)
        w_i32(out, batches)
        w_i32(out, skipped)
        w_f64(out, time.time() - start)
        w_str(out, str(dev) + (" (" + torch.cuda.get_device_name(dev) + ")" if dev.type == "cuda" else ""))
        write_weights(out, model)
        if version >= 2:
            w_f64(out, distill_sum / distill_n if distill_n > 0 else 0.0)
        print(f"round: {n:,} decisions, {batches} steps on {dev} in {time.time() - start:.2f}s "
              f"(policy {stats[0]:+.3f}, value {stats[1]:.3f}, entropy {stats[2]:.2f}"
              + (f", {int(distill_n / max(1, epochs)):,} look-ahead decisions, match {distill_sum / distill_n:.2f}" if distill_n > 0 else "")
              + ")", flush=True)
        return b"RES1", out.getvalue()

    def set_weights(self, r):
        tensors, learned, static = read_weights(r)
        self._sync(tensors, learned, static, self.lr or 2e-4)
        return b"OKW1", b""

    def add_cards(self, r):
        if self.model is None:
            raise ValueError("cards came before the network: send SETW first")
        first, rows = r.i32(), r.i32()
        learned = r.floats(rows * CARD_DIM).reshape(rows, CARD_DIM)
        static = r.floats(rows * STATIC_SIZE).reshape(rows, STATIC_SIZE)
        total = first + rows
        if self.model.cards.ids.weight.shape[0] < total:
            # _grow starts the table empty (training refills it); keep the rows already here
            old_ids = self.model.cards.ids.weight.detach().clone()
            old_static = self.model.cards.static.clone()
            self._grow(total)
            with torch.no_grad():
                self.model.cards.ids.weight[:old_ids.shape[0]].copy_(old_ids)
                self.model.cards.static[:old_static.shape[0]].copy_(old_static)
        with torch.no_grad():
            self.model.cards.ids.weight[first:total].copy_(torch.from_numpy(learned))
            self.model.cards.static[first:total].copy_(torch.from_numpy(static))
        return b"OKW1", b""

    def infer(self, r):
        """Moves for a batch of decisions from the sim's games, with the network this learner holds."""
        if self.model is None:
            raise ValueError("moves asked for before the network: send SETW first")
        s = read_samples(r)
        self.model.eval()
        with torch.no_grad():
            logits, value = self.model(batch(s, np.arange(s["n"]), self.device))
        logits, value = logits.float().cpu().numpy(), value.float().cpu().numpy()
        self.inferred += s["n"]
        out = io.BytesIO()
        counts = s["counts"]
        for i in range(s["n"]):
            out.write(logits[i, :counts[i]].astype("<f4").tobytes())
            out.write(value[i:i + 1].astype("<f4").tobytes())
        return b"OUT1", out.getvalue()

    def forward(self, r):
        tensors, learned, static = read_weights(r)
        s = read_samples(r)
        self._sync(tensors, learned, static, self.lr or 2e-4)
        self.model.eval()
        out = io.BytesIO()
        with torch.no_grad():
            logits, value = self.model(batch(s, np.arange(s["n"]), self.device))
        logits, value = logits.cpu().numpy(), value.cpu().numpy()
        for i in range(s["n"]):
            w_floats(out, logits[i, :s["counts"][i]])
            w_floats(out, value[i:i + 1])
        return b"OUT1", out.getvalue()


# ---------------------------------------------------------------------------------------------------- serving

def recv_exact(conn, n):
    buf = bytearray()
    while len(buf) < n:
        chunk = conn.recv(min(1 << 20, n - len(buf)))
        if not chunk:
            raise ConnectionError("the sim disconnected")
        buf += chunk
    return bytes(buf)


def serve(learner, host, port):
    srv = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    srv.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    srv.bind((host, port))
    srv.listen(4)
    print(f"GPU learner ready on {host}:{port}, learning on {learner.device}"
          + (f" ({torch.cuda.get_device_name(learner.device)})" if learner.device.type == "cuda" else ""), flush=True)
    while True:
        conn, addr = srv.accept()
        conn.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
        print(f"sim connected from {addr[0]}", flush=True)
        try:
            while True:
                head = recv_exact(conn, 12)
                kind, size = head[:4], struct.unpack("<q", head[4:])[0]
                r = Reader(recv_exact(conn, size))
                try:
                    version = r.i32()
                    slot = 0
                    if version == VERSION_SLOT and kind == b"TRN1":
                        slot = r.i32()
                        version = r.i32()
                    learner.select(slot)
                    if version not in VERSIONS:
                        raise ValueError("the sim and this learner are different versions: update both")
                    if kind == b"TRN1":
                        reply = learner.train(r, version)
                        if learner.inferred:
                            print(f"   (and chose {learner.inferred:,} moves for the sim's games since the last round)", flush=True)
                            learner.inferred = 0
                    elif kind == b"SETW":
                        reply = learner.set_weights(r)
                    elif kind == b"CARD":
                        reply = learner.add_cards(r)
                    elif kind == b"INF1":
                        reply = learner.infer(r)
                    elif kind == b"FWD1":
                        reply = learner.forward(r)
                    else:
                        raise ValueError(f"unknown message {kind!r}")
                except Exception as e:  # tell the sim; it learns that round on the CPU instead
                    print(f"error: {e}", flush=True)
                    out = io.BytesIO()
                    w_str(out, f"{type(e).__name__}: {e}")
                    reply = (b"ERR1", out.getvalue())
                conn.sendall(reply[0] + struct.pack("<q", len(reply[1])) + reply[1])
        except (ConnectionError, OSError) as e:
            print(f"sim disconnected ({e}); waiting for it again", flush=True)
        finally:
            conn.close()


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--host", default="127.0.0.1")
    ap.add_argument("--port", type=int, default=5577)
    ap.add_argument("--device", default="cuda" if torch.cuda.is_available() else "cpu",
                    help="cuda (the NVIDIA card, default when there is one) or cpu")
    ap.add_argument("--threads", type=int, default=0, help="CPU threads for --device cpu (0: PyTorch's choice)")
    args = ap.parse_args()
    if args.device.startswith("cuda") and not torch.cuda.is_available():
        sys.exit("PyTorch can't see an NVIDIA card: install the CUDA build of PyTorch (see the top of this file), "
                 "or run with --device cpu")
    if args.threads > 0:
        torch.set_num_threads(args.threads)
    serve(Learner(args.device), args.host, args.port)


if __name__ == "__main__":
    main()
