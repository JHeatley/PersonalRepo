"""Step 2: pre-training. The model learns language by guessing the next token of the text, billions of times.

    python pretrain.py                         # the "tiny" model (~17M parameters), its usual amount of text
    python pretrain.py --size small            # ~43M parameters: better, several times as long
    python pretrain.py --resume                # carry on from out/pretrain.pt (after stopping with Ctrl+C, say)
    python pretrain.py --tokens 500M           # train on this many tokens (default: 20 per parameter, the "Chinchilla" rule)

It saves out/pretrain.pt every --save-every steps and when it finishes (Ctrl+C saves too), shows the loss (lower is
better) on text it never trains on, and writes a sample so you can watch it learn to write.

On an RTX 2060 (6 GB), "tiny" takes roughly 1-3 hours. If it runs out of GPU memory, lower --batch (it then takes more steps per
update, --accum, so the result is the same).
"""
import argparse
import dataclasses
import math
import os
import time

import numpy as np
import torch

from common import DATA, OUT, device_and_dtype, human, load_tokenizer
from model import SIZES, TinyLLM, load, save


def parse_count(text):
    text = str(text).strip().upper()
    mult = {"K": 1e3, "M": 1e6, "B": 1e9}.get(text[-1:], 1)
    return int(float(text.rstrip("KMB")) * mult)


class Batches:
    """Random windows of the token file (memory-mapped: the file isn't loaded whole)."""

    def __init__(self, path, context, seed):
        self.data = np.memmap(path, dtype=np.uint16, mode="r")
        self.context = context
        self.rng = np.random.default_rng(seed)
        if len(self.data) <= context + 1:
            raise SystemExit(f"{path} is too small for this context ({len(self.data)} tokens)")

    def get(self, batch, device):
        starts = self.rng.integers(0, len(self.data) - self.context - 1, size=batch)
        x = np.stack([self.data[s:s + self.context] for s in starts]).astype(np.int64)
        y = np.stack([self.data[s + 1:s + 1 + self.context] for s in starts]).astype(np.int64)
        x, y = torch.from_numpy(x), torch.from_numpy(y)
        if device == "cuda":
            return x.pin_memory().to(device, non_blocking=True), y.pin_memory().to(device, non_blocking=True)
        return x, y


def lr_at(step, total, peak, warmup):
    """A short warm-up, then a cosine decay to a tenth of the peak."""
    if step < warmup:
        return peak * (step + 1) / warmup
    progress = min(1.0, (step - warmup) / max(1, total - warmup))
    return peak * (0.1 + 0.9 * 0.5 * (1 + math.cos(math.pi * progress)))


@torch.no_grad()
def evaluate(model, batches, batch, device, dtype, steps=20):
    model.eval()
    losses = []
    for _ in range(steps):
        x, y = batches.get(batch, device)
        with torch.autocast(device_type=device, dtype=dtype, enabled=device == "cuda"):
            _, loss = model(x, y)
        losses.append(loss.item())
    model.train()
    return sum(losses) / len(losses)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--size", choices=sorted(SIZES), default="tiny")
    ap.add_argument("--tokens", default="", help="tokens to train on (e.g. 300M); default 20 per parameter")
    ap.add_argument("--batch", type=int, default=16, help="sequences per step on the GPU (lower it if memory runs out)")
    ap.add_argument("--accum", type=int, default=4, help="steps per weight update (batch x accum sequences per update)")
    ap.add_argument("--lr", type=float, default=0, help="peak learning rate (default: by model size)")
    ap.add_argument("--warmup", type=int, default=200, help="updates of learning-rate warm-up")
    ap.add_argument("--eval-every", type=int, default=250, help="updates between checks on the kept-aside text")
    ap.add_argument("--save-every", type=int, default=500, help="updates between checkpoints")
    ap.add_argument("--resume", action="store_true", help="carry on from out/pretrain.pt")
    ap.add_argument("--cpu", action="store_true", help="train on the CPU (slow; for testing)")
    ap.add_argument("--compile", action="store_true", help="torch.compile the model (faster on Linux; often unavailable on Windows)")
    ap.add_argument("--seed", type=int, default=1337)
    args = ap.parse_args()

    torch.manual_seed(args.seed)
    device, dtype = device_and_dtype(args.cpu)
    sp = load_tokenizer()
    ckpt_path = os.path.join(OUT, "pretrain.pt")
    start_step, tokens_seen = 0, 0
    if args.resume and os.path.exists(ckpt_path):
        model, state = load(ckpt_path, device)
        start_step, tokens_seen = state.get("step", 0), state.get("tokens", 0)
        print(f"resuming at update {start_step:,} ({human(tokens_seen)} tokens seen)")
    else:
        cfg = dataclasses.replace(SIZES[args.size], vocab_size=sp.get_piece_size())
        model = TinyLLM(cfg).to(device)
        state = {}
    cfg = model.cfg
    n_params = model.params()
    total_tokens = parse_count(args.tokens) if args.tokens else state.get("total_tokens", 20 * n_params)
    per_update = args.batch * args.accum * cfg.context
    total_updates = max(1, total_tokens // per_update)
    # Smaller models take a bigger learning rate.
    peak = args.lr or state.get("peak_lr", 2e-3 if n_params < 30e6 else 1e-3 if n_params < 120e6 else 6e-4)

    # AdamW, with weight decay only on the matrices (not the norms).
    decay = [p for n, p in model.named_parameters() if p.dim() >= 2]
    no_decay = [p for n, p in model.named_parameters() if p.dim() < 2]
    opt = torch.optim.AdamW([{"params": decay, "weight_decay": 0.1}, {"params": no_decay, "weight_decay": 0.0}],
                            lr=peak, betas=(0.9, 0.95), eps=1e-8, fused=device == "cuda")
    if "optimizer" in state:
        opt.load_state_dict(state["optimizer"])
    # float16 needs its gradients scaled up so small ones don't round to zero (bfloat16 and float32 don't).
    scaler = torch.amp.GradScaler(device, enabled=dtype == torch.float16)
    train = Batches(os.path.join(DATA, "pretrain_train.bin"), cfg.context, args.seed + start_step)
    val = Batches(os.path.join(DATA, "pretrain_val.bin"), cfg.context, 99)
    run = torch.compile(model) if args.compile else model

    print(f"model: {human(n_params)} parameters ({cfg.layers} layers, width {cfg.dim}, context {cfg.context}) on {device}"
          f" ({str(dtype).replace('torch.', '')})")
    print(f"training on {human(total_tokens)} tokens: {total_updates:,} updates of {human(per_update)} tokens"
          f" (the file has {human(len(train.data))}; peak learning rate {peak:g})")

    def checkpoint(step):
        save(ckpt_path, model, {"step": step, "tokens": tokens_seen, "total_tokens": total_tokens, "peak_lr": peak,
                                "optimizer": opt.state_dict()})

    clock, last_tokens = time.time(), tokens_seen
    step = start_step
    try:
        for step in range(start_step, total_updates):
            lr = lr_at(step, total_updates, peak, args.warmup)
            for g in opt.param_groups:
                g["lr"] = lr
            for _ in range(args.accum):
                x, y = train.get(args.batch, device)
                with torch.autocast(device_type=device, dtype=dtype, enabled=device == "cuda"):
                    _, loss = run(x, y)
                scaler.scale(loss / args.accum).backward()
            scaler.unscale_(opt)
            torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
            scaler.step(opt)
            scaler.update()
            opt.zero_grad(set_to_none=True)
            tokens_seen += per_update

            if step % 25 == 0:
                now = time.time()
                speed = (tokens_seen - last_tokens) / max(1e-6, now - clock)
                eta = (total_updates - step - 1) * per_update / max(1, speed)
                print(f"update {step:,}/{total_updates:,}  loss {loss.item():.3f}  lr {lr:.2e}  {human(speed)} tokens/s  "
                      f"{eta / 3600:.1f} h left", flush=True)
                clock, last_tokens = now, tokens_seen
            if (step + 1) % args.eval_every == 0 or step + 1 == total_updates:
                v = evaluate(model, val, args.batch, device, dtype)
                sample = model.generate([sp.bos_id()] + sp.encode("The"), max_new=40, temperature=0.8)
                model.train()
                print(f"  kept-aside loss {v:.3f}  |  sample: The{sp.decode(sample)!r}", flush=True)
            if (step + 1) % args.save_every == 0:
                checkpoint(step + 1)
        step = total_updates
    except KeyboardInterrupt:
        print("stopped: saving...")
    checkpoint(step)  # the updates finished (an interrupted one doesn't count)
    print(f"saved {ckpt_path}. Next: python chat_tune.py  (or python pretrain.py --resume to carry on)")


if __name__ == "__main__":
    main()
