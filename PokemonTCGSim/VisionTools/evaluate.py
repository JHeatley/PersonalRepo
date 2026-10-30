"""Measure a vision model on the pictures held back for checking (val.jsonl), before and after training.

    python evaluate.py scenes_dataset --model qwen3-vl:4b-instruct --limit 50   # the untrained model: the starting point
    python evaluate.py scenes_dataset --model pokemon-tcg --limit 50            # after training

Prints the average scores (see score.py for what they mean) and saves, in scenes_dataset/eval_<model>/,
each answer (answers.jsonl) and, for the first --previews pictures, the picture next to the scene drawn from the answer.
The sprites are found from the dataset (demo_sprites/ if it was made with --demo-sprites) or --sprites.
"""
import argparse
import json
import os
import re
import time

import ask_model
import scenespec
import score


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("dataset")
    ap.add_argument("--model", default="pokemon-tcg")
    ap.add_argument("--sprites", help="the sprite folder (default: the dataset's demo_sprites, if any)")
    ap.add_argument("--api", choices=("ollama", "openai"), default="ollama")
    ap.add_argument("--url")
    ap.add_argument("--limit", type=int, default=50)
    ap.add_argument("--previews", type=int, default=10)
    ap.add_argument("--no-schema", action="store_true")
    args = ap.parse_args()

    folder = args.sprites or os.path.join(args.dataset, "demo_sprites")
    sprites = scenespec.sprite_library(folder if os.path.isdir(folder) else None)
    with open(os.path.join(args.dataset, "sprite_names.json"), encoding="utf-8") as f:
        names = json.load(f)
    if names != list(sprites):
        print("warning: the sprite folder's names differ from the ones the dataset was made with; "
              "previews may show missing sprites as magenta boxes")
    with open(os.path.join(args.dataset, "val.jsonl"), encoding="utf-8") as f:
        examples = [json.loads(line) for line in f][:args.limit]
    out_dir = os.path.join(args.dataset, "eval_" + re.sub(r"[^A-Za-z0-9_.-]", "_", args.model))
    os.makedirs(out_dir, exist_ok=True)

    scores = []
    start = time.time()
    with open(os.path.join(out_dir, "answers.jsonl"), "w", encoding="utf-8") as log:
        for i, ex in enumerate(examples):
            path = os.path.join(args.dataset, ex["image"])
            truth = json.loads(ex["answer"])
            png, _ = ask_model.image_bytes(path, 0)  # the dataset's pictures are already training-sized
            answer = ask_model.ask(png, args.model, names, args.api, args.url, not args.no_schema)
            guess, bad = ask_model.read_scene(answer, names)
            s = score.score(truth, None if bad else guess)
            scores.append(s)
            log.write(json.dumps({"image": ex["image"], "answer": answer, "problems": bad, "score": s},
                                 ensure_ascii=False) + "\n")
            if i < args.previews and not bad:
                ask_model.side_by_side(path, guess, sprites).save(os.path.join(out_dir, f"{i:03d}.png"))
            print(f"{i + 1}/{len(examples)}: f1 {100 * s['f1']:.0f}%" + (f"  ({bad[0]})" if bad else ""))
    print(f"\n{args.model} on {len(examples)} pictures ({time.time() - start:.0f} s):")
    print(score.describe(score.summary(scores)))
    print(f"answers and previews in {out_dir}")


if __name__ == "__main__":
    main()
