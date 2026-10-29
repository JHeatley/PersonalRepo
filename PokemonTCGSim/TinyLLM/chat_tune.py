"""Step 3: chat tuning. The pre-trained model learns to answer: it trains on the conversations (question, answer), and only
its answers count, so it learns to reply rather than to write questions.

    python chat_tune.py                 # from out/pretrain.pt, 2 passes over the conversations, saves out/chat.pt
    python chat_tune.py --epochs 3      # more passes (more exact answers, but beware of only parroting them)

Takes minutes to an hour or two on an RTX 2060, depending on the model size and how many conversations there are.
"""
import argparse
import math
import os
import time

import numpy as np
import torch

from common import DATA, OUT, device_and_dtype, human, load_tokenizer
from model import load, save


class Conversations:
    """The chat examples (prepare_data.py): each conversation's tokens and which of them are answers."""

    def __init__(self, context, seed):
        self.tokens = np.fromfile(os.path.join(DATA, "chat_tokens.bin"), dtype=np.uint16)
        self.mask = np.fromfile(os.path.join(DATA, "chat_mask.bin"), dtype=np.uint8)
        self.index = np.fromfile(os.path.join(DATA, "chat_index.bin"), dtype=np.int64).reshape(-1, 2)
        if len(self.index) == 0:
            raise SystemExit("No conversations: build ../LlmTraining's train.jsonl, then run prepare_data.py again.")
        self.context = context
        self.rng = np.random.default_rng(seed)
        order = self.rng.permutation(len(self.index))
        self.val = order[: max(1, len(order) // 100)]   # 1 in 100 kept aside
        self.train = order[len(self.val):]

    def batch(self, ids, device):
        """Conversations padded to the longest (at most the context); a long one is cut to a window that has answers in it."""
        rows = []
        for i in ids:
            start, length = self.index[i]
            t = self.tokens[start:start + length].astype(np.int64)
            m = self.mask[start:start + length].astype(np.int64)
            if length > self.context + 1:
                first = int(np.argmax(m))  # the first answer token
                lo = max(0, min(first - self.context // 2, length - self.context - 1))
                t, m = t[lo:lo + self.context + 1], m[lo:lo + self.context + 1]
            rows.append((t, m))
        width = max(len(t) for t, _ in rows) - 1
        x = np.zeros((len(rows), width), dtype=np.int64)
        y = np.zeros((len(rows), width), dtype=np.int64)
        w = np.zeros((len(rows), width), dtype=np.int64)
        for r, (t, m) in enumerate(rows):
            n = len(t) - 1
            x[r, :n], y[r, :n], w[r, :n] = t[:-1], t[1:], m[1:]
        return [torch.from_numpy(a).to(device) for a in (x, y, w)]


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--base", default=os.path.join(OUT, "pretrain.pt"))
    ap.add_argument("--out", default=os.path.join(OUT, "chat.pt"))
    ap.add_argument("--epochs", type=float, default=2)
    ap.add_argument("--batch", type=int, default=16)
    ap.add_argument("--lr", type=float, default=3e-4)
    ap.add_argument("--cpu", action="store_true")
    ap.add_argument("--seed", type=int, default=7)
    args = ap.parse_args()

    torch.manual_seed(args.seed)
    device, dtype = device_and_dtype(args.cpu)
    sp = load_tokenizer()
    if not os.path.exists(args.base):
        raise SystemExit(f"No pre-trained model at {args.base}: run pretrain.py first.")
    model, _ = load(args.base, device)
    data = Conversations(model.cfg.context, args.seed)
    steps = max(1, int(math.ceil(len(data.train) * args.epochs / args.batch)))
    opt = torch.optim.AdamW(model.parameters(), lr=args.lr, betas=(0.9, 0.95), weight_decay=0.0)
    scaler = torch.amp.GradScaler(device, enabled=dtype == torch.float16)
    print(f"chat tuning {human(model.params())} parameters on {len(data.train):,} conversations ({args.epochs:g} passes, "
          f"{steps:,} steps) on {device}")

    def val_loss():
        model.eval()
        losses = []
        with torch.no_grad():
            for at in range(0, min(len(data.val), 256), args.batch):
                x, y, w = data.batch(data.val[at:at + args.batch], device)
                with torch.autocast(device_type=device, dtype=dtype, enabled=device == "cuda"):
                    _, loss = model(x, y, w)
                losses.append(loss.item())
        model.train()
        return sum(losses) / max(1, len(losses))

    model.train()
    clock = time.time()
    order = np.concatenate([data.rng.permutation(data.train) for _ in range(int(math.ceil(args.epochs)) + 1)])
    try:
        for step in range(steps):
            lr = args.lr * min(1.0, (step + 1) / 50) * (0.1 + 0.9 * 0.5 * (1 + math.cos(math.pi * step / steps)))
            for g in opt.param_groups:
                g["lr"] = lr
            x, y, w = data.batch(order[step * args.batch:(step + 1) * args.batch], device)
            with torch.autocast(device_type=device, dtype=dtype, enabled=device == "cuda"):
                _, loss = model(x, y, w)
            scaler.scale(loss).backward()
            scaler.unscale_(opt)
            torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
            scaler.step(opt)
            scaler.update()
            opt.zero_grad(set_to_none=True)
            if step % 50 == 0:
                rate = (step + 1) / max(1e-6, time.time() - clock)
                print(f"step {step:,}/{steps:,}  answer loss {loss.item():.3f}  {(steps - step) / max(rate, 1e-6) / 60:.0f} min left", flush=True)
            if (step + 1) % 500 == 0 or step + 1 == steps:
                print(f"  kept-aside answer loss {val_loss():.3f}", flush=True)
    except KeyboardInterrupt:
        print("stopped early: saving what it learned so far...")
    save(args.out, model)
    print(f"saved {args.out}. Try it: python chat.py")


if __name__ == "__main__":
    main()
