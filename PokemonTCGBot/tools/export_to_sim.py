"""Copy a trained network into the Unity sim (PokemonTCGSim) as its starting point.

    python tools/export_to_sim.py                              # models/bot_126k.pt
    python tools/export_to_sim.py snapshots/run100k_latest.pt
    python tools/export_to_sim.py models/bot_126k.pt --samples 50 --samples-out test.bin

The sim's Neural AI starts from this file and trains its own copy from there; nothing
the sim does is ever written back to this project. Run this again only when you want
the sim to start from a newer Python model.

File layout (little-endian): b"PTCGPOL1", int32 version, then
  int32 tensor count; each: str name, int32 ndim, int32 dims..., float32 data
  int32 card count N; each card 1..N: str name, str prints ("TEF-123;SVI-1"), float32[64] static
  str json config
Strings are int32 byte length + UTF-8.

--samples writes encoded decisions from random games, with this network's logits and
value for each, so the C# port can check it computes exactly the same thing.
"""
import argparse
import json
import os
import random
import struct
import sys

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
import numpy as np  # noqa: E402
import torch  # noqa: E402

from tcg.carddb import CARDS  # noqa: E402
from tcg.engine import Game  # noqa: E402
from tcg.deckio import load_deck, deck_names  # noqa: E402
from bot.encode import CARD_STATIC, FIXED_KEYS, encode, collate  # noqa: E402
from bot.model import load, to_torch  # noqa: E402

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DEFAULT_OUT = os.path.join(ROOT, "PokemonTCGSim", "Assets", "PokemonTCG", "Resources", "AI", "policy_start.bytes")
VERSION = 1


def _str(f, s):
    b = s.encode("utf-8")
    f.write(struct.pack("<i", len(b)))
    f.write(b)


def _arr(f, a):
    a = np.ascontiguousarray(a, dtype="<f4")
    f.write(a.tobytes())


def prints_of(c):
    return ";".join(f"{code}-{num}" for code, num in c.printings)


def export(model, ckpt, path):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    state = model.state_dict()
    with open(path, "wb") as f:
        f.write(b"PTCGPOL1")
        f.write(struct.pack("<i", VERSION))
        tensors = {k: v for k, v in state.items() if k != "cards.static"}  # static stats are written per card below
        f.write(struct.pack("<i", len(tensors)))
        for name, t in tensors.items():
            _str(f, name)
            f.write(struct.pack("<i", t.dim()))
            for d in t.shape:
                f.write(struct.pack("<i", d))
            _arr(f, t.detach().float().numpy())
        f.write(struct.pack("<i", len(CARDS)))
        for c in CARDS:
            _str(f, c.name)
            _str(f, prints_of(c))
            _arr(f, CARD_STATIC[c.id + 1])
        _str(f, json.dumps({**model.config, "games": ckpt.get("games", 0)}))
    return os.path.getsize(path)


def export_samples(model, path, n, seed=0):
    """Encoded decisions from games between random players, plus the network's logits and value."""
    rng = random.Random(seed)
    names = deck_names()
    samples = []
    while len(samples) < n:
        g = Game(load_deck(rng.choice(names)), load_deck(rng.choice(names)), seed=rng.randrange(1 << 30))
        while not g.done and len(samples) < n:
            if len(g.decision.options) > 1 and rng.random() < 0.15:
                samples.append(encode(g))
            g.step(rng.randrange(len(g.decision.options)))
    with torch.no_grad():
        logits, values = model(to_torch(collate(samples)))
    with open(path, "wb") as f:
        f.write(struct.pack("<i", len(samples)))
        for i, e in enumerate(samples):
            for k in FIXED_KEYS + ["opt_kind", "opt_card", "opt_target", "opt_feat"]:
                a = np.asarray(e[k])
                _str(f, k)
                f.write(struct.pack("<i", a.size))
                if a.dtype.kind in "iu":
                    f.write(np.ascontiguousarray(a.reshape(-1), dtype="<i4").tobytes())
                else:
                    _arr(f, a.reshape(-1))
            k = len(e["opt_kind"])
            _arr(f, logits[i, :k].numpy())
            _arr(f, values[i:i + 1].numpy())
    return len(samples)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("model", nargs="?", default="models/bot_126k.pt")
    ap.add_argument("--out", default=DEFAULT_OUT)
    ap.add_argument("--samples", type=int, default=0, help="also write this many test decisions")
    ap.add_argument("--samples-out", default=None)
    args = ap.parse_args()
    os.chdir(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
    model, ckpt = load(args.model)
    size = export(model, ckpt, args.out)
    print(f"wrote {args.out} ({size / 1e6:.1f} MB, {ckpt.get('games', 0)} games of training)")
    if args.samples:
        out = args.samples_out or os.path.splitext(args.out)[0] + "_samples.bin"
        print(f"wrote {export_samples(model, out, args.samples)} test decisions to {out}")


if __name__ == "__main__":
    main()
