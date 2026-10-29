"""How close a model's scene is to the right one.

    python score.py truth.json guess.json

Each object in the guess is paired with the best unpaired object of the same type in the truth (by how much their
boxes overlap). A pair counts as found when the boxes overlap enough (IoU at least 0.5; 0.3 for text, whose boxes are
small). Then: did a found sprite get the right name, a text or button the right words, and how far off are colours?

The numbers:
  found       share of the truth's objects the guess found (recall)
  correct     share of the guess's objects that match one in the truth (precision)
  f1          the two together; the main number to watch
  box_iou     average overlap of the found pairs' boxes (1 = exact)
  names       found sprites with the right name
  words       found texts and buttons with exactly the right words (ignoring case and spaces at the ends)
  colour      average colour error of found pairs, as a share of the largest possible (0 = exact)
  background  the background's colour error
"""
import json
import sys

import scenespec

THRESHOLD = {"rect": 0.5, "sprite": 0.5, "button": 0.5, "text": 0.3}


def iou(a, b):
    x1, y1 = max(a[0], b[0]), max(a[1], b[1])
    x2, y2 = min(a[2], b[2]), min(a[3], b[3])
    inter = max(0, x2 - x1) * max(0, y2 - y1)
    union = (a[2] - a[0]) * (a[3] - a[1]) + (b[2] - b[0]) * (b[3] - b[1]) - inter
    return inter / union if union > 0 else 0.0


def colour_error(a, b):
    """0 for the same colour, 1 for black against white."""
    ra, rb = scenespec.rgb(a), scenespec.rgb(b)
    return (sum((x - y) ** 2 for x, y in zip(ra, rb)) / (3 * 255 ** 2)) ** 0.5


def same_words(a, b):
    return " ".join(str(a).split()).lower() == " ".join(str(b).split()).lower()


def pair(truth, guess):
    """[(truth object, guess object, iou)] for the pairs that overlap enough: the best overlaps first, each object used
    at most once, a sprite with the right name preferred over a bigger overlap with the wrong one."""
    candidates = []
    for i, t in enumerate(truth):
        for j, g in enumerate(guess):
            if t["type"] != g["type"]:
                continue
            v = iou(t["box"], g["box"])
            if v < THRESHOLD[t["type"]]:
                continue
            bonus = 1 if (t.get("name") is not None and t.get("name") == g.get("name")) or \
                         (t.get("text") is not None and same_words(t.get("text"), g.get("text", ""))) else 0
            candidates.append((bonus, v, i, j))
    candidates.sort(reverse=True)
    used_t, used_g, out = set(), set(), []
    for _, v, i, j in candidates:
        if i in used_t or j in used_g:
            continue
        used_t.add(i)
        used_g.add(j)
        out.append((truth[i], guess[j], v))
    return out


def score(truth, guess):
    """A dict of the numbers above for one picture (guess may be None when the model's answer was unusable)."""
    t_objs = truth["objects"]
    if guess is None or scenespec.problems(guess):
        return {"found": 0.0, "correct": 0.0, "f1": 0.0, "box_iou": 0.0, "names": None, "words": None,
                "colour": None, "background": None, "valid": False, "truth_objects": len(t_objs), "guess_objects": 0}
    g_objs = guess["objects"]
    pairs = pair(t_objs, g_objs)
    found = len(pairs) / len(t_objs) if t_objs else 1.0
    correct = len(pairs) / len(g_objs) if g_objs else (1.0 if not t_objs else 0.0)
    f1 = 2 * found * correct / (found + correct) if found + correct else 0.0
    sprites = [(t, g) for t, g, _ in pairs if t["type"] == "sprite"]
    worded = [(t, g) for t, g, _ in pairs if t["type"] in ("text", "button")]
    colours = []
    for t, g, _ in pairs:
        for key in ("colour", "textColour"):
            if key in t and key in g:
                colours.append(colour_error(t[key], g[key]))

    def mean(xs):
        return sum(xs) / len(xs) if xs else None

    return {"found": found, "correct": correct, "f1": f1,
            "box_iou": mean([v for _, _, v in pairs]) or 0.0,
            "names": mean([1.0 if t["name"] == g.get("name") else 0.0 for t, g in sprites]),
            "words": mean([1.0 if same_words(t["text"], g.get("text", "")) else 0.0 for t, g in worded]),
            "colour": mean(colours),
            "background": colour_error(truth["background"], guess["background"]),
            "valid": True, "truth_objects": len(t_objs), "guess_objects": len(g_objs)}


def summary(scores):
    """The average of each number over many pictures (skipping pictures where it doesn't apply)."""
    out = {}
    for key in ("f1", "found", "correct", "box_iou", "names", "words", "colour", "background"):
        values = [s[key] for s in scores if s.get(key) is not None]
        out[key] = sum(values) / len(values) if values else None
    out["valid"] = sum(1 for s in scores if s["valid"]) / len(scores) if scores else None
    out["pictures"] = len(scores)
    return out


def describe(summary_):
    def pct(v):
        return "-" if v is None else f"{100 * v:.0f}%"
    return (f"f1 {pct(summary_['f1'])} (found {pct(summary_['found'])}, correct {pct(summary_['correct'])}), "
            f"box overlap {pct(summary_['box_iou'])}, sprite names {pct(summary_['names'])}, "
            f"words {pct(summary_['words'])}, colour error {pct(summary_['colour'])}, "
            f"background error {pct(summary_['background'])}, usable answers {pct(summary_['valid'])}")


def main():
    if len(sys.argv) != 3:
        sys.exit(__doc__)
    with open(sys.argv[1], encoding="utf-8") as f:
        truth = json.load(f)
    with open(sys.argv[2], encoding="utf-8") as f:
        guess = json.load(f)
    s = score(truth, guess)
    print(describe(summary([s])))
    print(f"{s['truth_objects']} objects in the truth, {s['guess_objects']} in the guess")


if __name__ == "__main__":
    main()
