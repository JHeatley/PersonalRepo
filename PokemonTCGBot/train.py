"""Train the bot by self-play with PPO.

    python train.py --games 300000                 # long run, all decks in decks/
    python train.py --games 2000 --workers 2       # quick smoke test
    python train.py --resume checkpoints/latest.pt --games 100000
    python train.py --decks dragapult,basic_box    # only some decks

Each iteration the workers play --games-per-iter games with the current
network (mostly against itself, sometimes against past snapshots or the
heuristic bot), then the network takes a few PPO passes over the data.
Checkpoints go to --out: latest.pt every iteration, best.pt when the win
rate against the heuristic bot improves, and snapshots for the opponent pool.
"""
import argparse
import multiprocessing as mp
import os
import random
import time

import numpy as np
import torch

from tcg.deckio import load_deck, deck_names
from bot.encode import collate
from bot.model import PolicyValueNet, to_torch, save, load
from bot.agents import NeuralAgent, HeuristicAgent
from bot.arena import play_match
from bot.selfplay import worker_collect, collect


def ppo_update(model, opt, samples, epochs, minibatch, clip, vf_coef, ent_coef, max_grad):
    adv = np.array([s["adv"] for s in samples], dtype=np.float32)
    adv = (adv - adv.mean()) / (adv.std() + 1e-8)
    idx = np.arange(len(samples))
    stats = {"policy": 0.0, "value": 0.0, "entropy": 0.0, "clipfrac": 0.0, "n": 0}
    model.train()
    for _ in range(epochs):
        np.random.shuffle(idx)
        for start in range(0, len(idx), minibatch):
            mb = idx[start:start + minibatch]
            b = to_torch(collate([samples[i]["enc"] for i in mb]))
            actions = torch.tensor([samples[i]["action"] for i in mb])
            old_logp = torch.tensor([samples[i]["logp"] for i in mb])
            ret = torch.tensor([samples[i]["ret"] for i in mb])
            a = torch.from_numpy(adv[mb])
            logits, values = model(b)
            logp_all = torch.log_softmax(logits, -1)
            logp = logp_all.gather(1, actions.unsqueeze(1)).squeeze(1)
            ratio = torch.exp(logp - old_logp)
            pg = -torch.min(ratio * a, torch.clamp(ratio, 1 - clip, 1 + clip) * a).mean()
            vloss = ((values - ret.clamp(-1.5, 1.5)) ** 2).mean()
            probs = logp_all.exp() * b["opt_mask"]
            ent = -(probs * logp_all.masked_fill(~b["opt_mask"], 0)).sum(-1).mean()
            loss = pg + vf_coef * vloss - ent_coef * ent
            opt.zero_grad()
            loss.backward()
            torch.nn.utils.clip_grad_norm_(model.parameters(), max_grad)
            opt.step()
            stats["policy"] += pg.item()
            stats["value"] += vloss.item()
            stats["entropy"] += ent.item()
            stats["clipfrac"] += ((ratio - 1).abs() > clip).float().mean().item()
            stats["n"] += 1
    model.eval()
    n = max(stats.pop("n"), 1)
    return {k: v / n for k, v in stats.items()}


def evaluate(model, decks, n_games, opponent=None):
    agent = NeuralAgent(model, greedy=True)
    return play_match(agent, opponent or HeuristicAgent(0), decks, n_games, concurrency=64, seed=12345)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--games", type=int, default=300000, help="total self-play games")
    ap.add_argument("--games-per-iter", type=int, default=256)
    ap.add_argument("--workers", type=int, default=max(1, (os.cpu_count() or 2) - 1))
    ap.add_argument("--concurrency", type=int, default=64, help="games each worker runs at once")
    ap.add_argument("--decks", default=None, help="comma-separated deck names (default: all in decks/)")
    ap.add_argument("--lr", type=float, default=2e-4)
    ap.add_argument("--epochs", type=int, default=2)
    ap.add_argument("--minibatch", type=int, default=2048)
    ap.add_argument("--clip", type=float, default=0.2)
    ap.add_argument("--vf-coef", type=float, default=0.5)
    ap.add_argument("--ent-coef", type=float, default=0.01)
    ap.add_argument("--gamma", type=float, default=0.999)
    ap.add_argument("--lam", type=float, default=0.95)
    ap.add_argument("--prize-reward", type=float, default=0.05, help="shaping reward per prize-card swing")
    ap.add_argument("--self-weight", type=float, default=0.6, help="share of games against itself")
    ap.add_argument("--pool-weight", type=float, default=0.3, help="share against past snapshots")
    ap.add_argument("--heuristic-weight", type=float, default=0.1, help="share against the heuristic bot")
    ap.add_argument("--snapshot-every", type=int, default=10, help="iterations between pool snapshots")
    ap.add_argument("--pool-size", type=int, default=8)
    ap.add_argument("--eval-every", type=int, default=10)
    ap.add_argument("--eval-games", type=int, default=200)
    ap.add_argument("--eval-ref", default=None,
                    help="also evaluate against this checkpoint (e.g. models/bot_53k.pt); >0.5 means stronger")
    ap.add_argument("--width", type=int, default=384)
    ap.add_argument("--blocks", type=int, default=3)
    ap.add_argument("--out", default="checkpoints")
    ap.add_argument("--resume", default=None)
    ap.add_argument("--seed", type=int, default=0)
    args = ap.parse_args()

    os.makedirs(args.out, exist_ok=True)
    names = args.decks.split(",") if args.decks else deck_names()
    decks = {n: load_deck(n) for n in names}
    print(f"decks: {', '.join(names)}")
    random.seed(args.seed)
    np.random.seed(args.seed)
    torch.manual_seed(args.seed)

    games_done, iteration, best = 0, 0, -1.0
    if args.resume:
        model, ckpt = load(args.resume)
        games_done, iteration, best = ckpt.get("games", 0), ckpt.get("iteration", 0), ckpt.get("best", -1.0)
        print(f"resumed {args.resume} at {games_done} games")
    else:
        model = PolicyValueNet(width=args.width, blocks=args.blocks)
    model.eval()
    opt = torch.optim.Adam(model.parameters(), lr=args.lr)
    pool = []
    for f in sorted(os.listdir(args.out)):
        if f.startswith("pool_") and f.endswith(".pt"):
            pool.append(load(os.path.join(args.out, f))[0].state_dict())
    pool = pool[-args.pool_size:]

    ref_agent = NeuralAgent(load(args.eval_ref)[0], greedy=True) if args.eval_ref else None
    workers = mp.get_context("fork").Pool(args.workers) if args.workers > 1 else None
    torch.set_num_threads(max(1, os.cpu_count() or 1))
    kw = {"gamma": args.gamma, "lam": args.lam, "prize_reward": args.prize_reward}
    t_start = time.time()
    while games_done < args.games:
        iteration += 1
        t0 = time.time()
        state = {k: v.clone() for k, v in model.state_dict().items()}
        pool_pick = random.sample(pool, min(2, len(pool))) if pool else []
        pool_w = args.pool_weight if pool_pick else 0.0
        n_workers = max(1, args.workers)
        per = max(1, args.games_per_iter // n_workers)
        tasks = [(state, model.config, pool_pick, args.heuristic_weight, args.self_weight, pool_w, decks, per,
                  args.concurrency, args.seed * 1000003 + iteration * 101 + w, kw) for w in range(n_workers)]
        results = workers.map(worker_collect, tasks) if workers else [worker_collect(tasks[0])]
        samples = [s for r in results for s in r[0]]
        stats = {k: sum(r[1][k] for r in results) for k in results[0][1]}
        t1 = time.time()
        loss = ppo_update(model, opt, samples, args.epochs, args.minibatch, args.clip, args.vf_coef,
                          args.ent_coef, 0.5)
        games_done += stats["games"]
        t2 = time.time()
        pool_wr = stats["pool_wins"] / stats["vs_pool"] if stats["vs_pool"] else float("nan")
        print(f"it {iteration:4d} | games {games_done:7d} | {stats['decisions'] / (t1 - t0):6.0f} dec/s | "
              f"avg turns {stats['turns'] / max(stats['games'], 1):5.1f} | draws {stats['draws']:3d} | "
              f"vs pool/heur {pool_wr:.2f} | pi {loss['policy']:+.3f} v {loss['value']:.3f} "
              f"H {loss['entropy']:.2f} clip {loss['clipfrac']:.2f} | collect {t1 - t0:4.1f}s train {t2 - t1:4.1f}s",
              flush=True)

        if iteration % args.snapshot_every == 0:
            path = os.path.join(args.out, f"pool_{iteration:06d}.pt")
            save(model, path)
            pool = (pool + [{k: v.clone() for k, v in model.state_dict().items()}])[-args.pool_size:]
        if iteration % args.eval_every == 0 or games_done >= args.games:
            r = evaluate(model, decks, args.eval_games)
            print(f"   eval vs heuristic (greedy): win rate {r['win_rate']:.3f} "
                  f"({r['wins']}-{r['draws']}-{r['losses']}), avg turns {r['avg_turns']:.1f}", flush=True)
            if ref_agent is not None:
                rr = evaluate(model, decks, args.eval_games, ref_agent)
                print(f"   eval vs {args.eval_ref} (both greedy): win rate {rr['win_rate']:.3f} "
                      f"({rr['wins']}-{rr['draws']}-{rr['losses']})", flush=True)
            if r["win_rate"] > best:
                best = r["win_rate"]
                save(model, os.path.join(args.out, "best.pt"), games=games_done, iteration=iteration, best=best)
        save(model, os.path.join(args.out, "latest.pt"), games=games_done, iteration=iteration, best=best)
    if workers:
        workers.close()
    print(f"done: {games_done} games in {(time.time() - t_start) / 60:.1f} min, best eval win rate {best:.3f}")


if __name__ == "__main__":
    main()
