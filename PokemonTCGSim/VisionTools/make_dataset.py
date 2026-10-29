"""Make training pictures for a vision model: random scenes drawn from your own sprites, each saved with the scene that
made it, so every picture comes with its exact answer.

    python make_dataset.py --sprites path/to/sprites --count 5000 --out scenes_dataset
    python make_dataset.py --demo-sprites --count 200 --out demo_dataset     # no sprites of your own yet: made-up ones

--sprites is a folder of PNGs (subfolders too); each file's name is the sprite's name (tree.png → "tree"), and the
Unity builder looks sprites up by the same names. Writes:

    out/images/000000.png ...    the pictures
    out/train.jsonl, val.jsonl   one example per line: {"image", "prompt", "answer"} (val is --val of them, for checking)
    out/sprite_names.json        the names the model was taught, in the order its prompt lists them
    out/tools/                   scenespec.py and score.py, for the Colab notebook

Pictures mix four layouts (a scattered game scene, a menu, a game screen with a HUD, a dialogue box), several screen
sizes and fonts (--fonts: a folder of .ttf/.otf files; pixel fonts are good for retro games), so the model learns the
format rather than one look. Add real examples too (your own screenshots, with scene JSON you've checked) by appending
lines in the same format.
"""
import argparse
import json
import os
import random
import shutil
import sys

import scenespec

SIZES = [(640, 360), (640, 480), (480, 320), (512, 384), (640, 400)]
WORDS = ["START", "Continue", "New Game", "Options", "Quit", "Load", "Save", "Back", "Play", "Settings", "Shop",
         "Items", "Bag", "Party", "Map", "Exit", "Yes", "No", "OK", "Cancel", "Level 5", "Score", "HP", "PP", "Gold",
         "Inventory", "Attack", "Defend", "Run", "Fight", "Pokédex", "Trainer", "Team", "Battle", "Victory!",
         "Game Over", "PRESS START", "Pause", "Resume", "Credits", "Volume", "Music", "Sound", "Easy", "Hard",
         "Stage 1-1", "Lives", "Time", "Coins", "Next", "Restart", "Menu", "Help", "Deck", "Cards", "Prizes"]
LINES = ["Welcome to the town!", "You found a potion.", "Do you want to save?", "The door is locked.",
         "A wild creature appeared!", "Your team is healed.", "Choose a starter.", "That's not allowed here.",
         "Thanks for playing!", "Press A to continue.", "Where are you going?", "The shop is closed today."]


def random_word(rng):
    r = rng.random()
    if r < 0.6:
        return rng.choice(WORDS)
    if r < 0.8:
        return f"{rng.choice(['HP', 'Score', 'Lv', 'x', 'Gold', 'Time'])} {rng.randint(0, 9999)}"
    # Made-up words, so the model reads text instead of remembering the word list.
    letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
    return "".join(rng.choice(letters) for _ in range(rng.randint(3, 9)))


def random_colour(rng, dark=None):
    """A random colour; dark=True or False keeps it on one side of mid-grey, for text that can be read on it."""
    while True:
        c = [rng.randint(0, 255) for _ in range(3)]
        light = 0.299 * c[0] + 0.587 * c[1] + 0.114 * c[2]
        if dark is None or (dark and light < 100) or (not dark and light > 155):
            return "#%02X%02X%02X" % tuple(c)


def lightness(colour):
    r, g, b = scenespec.rgb(colour)
    return 0.299 * r + 0.587 * g + 0.114 * b


def contrasting(rng, colour):
    return random_colour(rng, dark=lightness(colour) > 128)


def overlaps(a, b, margin=0):
    return not (a[2] + margin <= b[0] or b[2] + margin <= a[0] or a[3] + margin <= b[1] or b[3] + margin <= a[1])


class SceneMaker:
    def __init__(self, sprites, fonts, rng):
        from PIL import Image, ImageDraw
        self.rng = rng
        self.sprites = sprites
        self.names = list(sprites)
        self.sizes = {n: Image.open(p).size for n, p in sprites.items()}
        self.fonts = fonts or [None]
        self.draw = ImageDraw.Draw(Image.new("RGB", (8, 8)))

    # --------------------------------------------------------------- pieces (all in pixels, turned into boxes at the end)

    def sprite(self, width, height, area=None):
        """A sprite at a whole-number scale of its own size (pixel art stays sharp), somewhere in area."""
        rng = self.rng
        name = rng.choice(self.names)
        w, h = self.sizes[name]
        ax1, ay1, ax2, ay2 = area or (0, 0, width, height)
        scales = [s for s in (1, 2, 3, 4, 6, 8) if w * s <= (ax2 - ax1) * 0.6 and h * s <= (ay2 - ay1) * 0.8
                  and w * s >= 8 and h * s >= 8]
        if not scales:
            return None
        s = rng.choice(scales)
        x = rng.randint(ax1, ax2 - w * s)
        y = rng.randint(ay1, ay2 - h * s)
        o = {"type": "sprite", "px": (x, y, x + w * s, y + h * s), "name": name}
        r = rng.random()
        if r < 0.15:
            o["flip"] = "x"
        elif r < 0.18:
            o["flip"] = "y"
        return o

    def text(self, text, x, y, size, colour, font):
        f = scenespec.load_font(font, size)
        l, t, r, b = self.draw.textbbox((x, y), text, font=f)
        return {"type": "text", "px": (l, t, r, b), "text": text, "colour": colour}

    # --------------------------------------------------------------- layouts

    def scatter(self, width, height, bg, font, objects, top=0, bottom=None):
        """A game scene: ground and bands of colour, then sprites."""
        rng = self.rng
        bottom = height if bottom is None else bottom
        for _ in range(rng.randint(0, 3)):
            y = rng.randint(top + (bottom - top) // 3, bottom - 8)
            if rng.random() < 0.7:
                objects.append({"type": "rect", "px": (0, y, width, bottom), "colour": random_colour(rng)})
            else:
                w, h = rng.randint(16, width // 2), rng.randint(8, (bottom - top) // 3)
                x, y = rng.randint(0, width - w), rng.randint(top, bottom - h)
                objects.append({"type": "rect", "px": (x, y, x + w, y + h), "colour": random_colour(rng)})
        if self.names:
            for _ in range(rng.randint(1, 12)):
                o = self.sprite(width, height, (0, top, width, bottom))
                if o:
                    objects.append(o)

    def menu(self, width, height, bg, font, objects):
        """A title and a column of buttons, sometimes on a panel, sometimes with sprites around."""
        rng = self.rng
        if self.names and rng.random() < 0.5:
            for _ in range(rng.randint(1, 4)):
                o = self.sprite(width, height)
                if o:
                    objects.append(o)
        n = rng.randint(2, 6)
        bw = rng.randint(width // 5, width // 2)
        bh = rng.randint(max(18, height // 18), max(20, height // 9))
        gap = rng.randint(4, bh)
        total = n * bh + (n - 1) * gap
        title_h = rng.randint(height // 14, height // 7)
        top = rng.randint(title_h + 16, max(title_h + 17, height - total - 8))
        x = rng.choice([(width - bw) // 2, rng.randint(8, max(9, width - bw - 8))])
        if rng.random() < 0.5:
            pad = rng.randint(6, 20)
            panel = (max(0, x - pad), max(0, top - pad), min(width, x + bw + pad), min(height, top + total + pad))
            objects.append({"type": "rect", "px": panel, "colour": random_colour(rng)})
        title_size = rng.randint(max(12, height // 16), max(14, height // 8))
        objects.append(self.text(rng.choice(["Main Menu", "Pause", "Options", "My Game", "Select a mode",
                                             random_word(rng)]), x, max(2, top - title_h - 10), title_size,
                                 contrasting(rng, bg), font))
        colour = random_colour(rng)
        text_colour = contrasting(rng, colour)
        words = rng.sample(WORDS, n) if rng.random() < 0.8 else [random_word(rng) for _ in range(n)]
        for i, word in enumerate(words):
            y = top + i * (bh + gap)
            objects.append({"type": "button", "px": (x, y, x + bw, y + bh), "text": word, "colour": colour,
                            "textColour": text_colour})

    def hud(self, width, height, bg, font, objects):
        """A game scene with a bar of numbers along the top (and sometimes buttons at the bottom)."""
        rng = self.rng
        bar_h = rng.randint(height // 14, height // 8)
        self.scatter(width, height, bg, font, objects, top=bar_h)
        bar = random_colour(rng)
        objects.append({"type": "rect", "px": (0, 0, width, bar_h), "colour": bar})
        size = max(10, int(bar_h * 0.6))
        x = 8
        for _ in range(rng.randint(1, 4)):
            o = self.text(random_word(rng), x, (bar_h - size) // 2, size, contrasting(rng, bar), font)
            if o["px"][2] > width - 4:
                break
            objects.append(o)
            x = o["px"][2] + rng.randint(16, 60)
        if rng.random() < 0.4:
            n = rng.randint(1, 4)
            bw, bh = rng.randint(width // 8, width // 5), rng.randint(height // 14, height // 9)
            colour = random_colour(rng)
            for i in range(n):
                bx = width - (n - i) * (bw + 8)
                if bx < 0:
                    continue
                objects.append({"type": "button", "px": (bx, height - bh - 8, bx + bw, height - 8),
                                "text": rng.choice(["Fight", "Bag", "Run", "Items", "Menu", "Jump", "OK", "Back"]),
                                "colour": colour, "textColour": contrasting(rng, colour)})

    def dialogue(self, width, height, bg, font, objects):
        """A game scene with a text box along the bottom."""
        rng = self.rng
        box_h = rng.randint(height // 5, height // 3)
        self.scatter(width, height, bg, font, objects, bottom=height - box_h)
        margin = rng.randint(0, 16)
        box = (margin, height - box_h - margin, width - margin, height - margin)
        colour = random_colour(rng)
        objects.append({"type": "rect", "px": box, "colour": colour})
        size = rng.randint(max(10, box_h // 6), max(12, box_h // 3))
        y = box[1] + rng.randint(4, 12)
        for line in rng.sample(LINES, rng.randint(1, 2)):
            o = self.text(line, box[0] + rng.randint(8, 20), y, size, contrasting(rng, colour), font)
            if o["px"][2] > box[2] - 4 or o["px"][3] > box[3] - 2:
                break
            objects.append(o)
            y = o["px"][3] + rng.randint(4, 10)

    def make(self, width, height):
        rng = self.rng
        bg = random_colour(rng)
        font = rng.choice(self.fonts)
        objects = []
        layout = rng.choice([self.scatter, self.menu, self.hud, self.dialogue] if self.names
                            else [self.menu, self.hud, self.dialogue])
        layout(width, height, bg, font, objects)
        if layout == self.scatter:
            for _ in range(rng.randint(0, 3)):
                size = rng.randint(max(10, height // 30), max(12, height // 10))
                objects.append(self.text(random_word(rng), rng.randint(4, width // 2), rng.randint(4, height - size - 8),
                                         size, contrasting(rng, bg), font))
        objects = self.tidy(objects, width, height)
        scene = {"background": bg, "objects": []}
        for o in objects:
            o = dict(o)
            o["box"] = scenespec.to_box(o.pop("px"), width, height)
            scene["objects"].append(o)
        style = scenespec.Style(font=font, button_radius=rng.choice([0, 0, 4, 8, 16]),
                                button_outline=rng.choice([None, None, (0, 0, 0), (255, 255, 255)]),
                                text_shadow=rng.random() < 0.2)
        return scene, style

    def tidy(self, objects, width, height):
        """Drop what the picture wouldn't show clearly: anything off the picture, too small to see, hidden under
        something drawn later, or text and buttons that overlap other text and buttons (unreadable)."""
        kept = []
        for i, o in enumerate(objects):
            x1, y1, x2, y2 = o["px"]
            if x1 < 0 or y1 < 0 or x2 > width or y2 > height or x2 - x1 < 3 or y2 - y1 < 3:
                continue
            hidden = any(later["type"] in ("rect", "button") and later["px"][0] <= x1 and later["px"][1] <= y1
                         and later["px"][2] >= x2 and later["px"][3] >= y2 for later in objects[i + 1:])
            if hidden:
                continue
            if o["type"] in ("text", "button") and any(k["type"] in ("text", "button") and overlaps(k["px"], o["px"], 2)
                                                      for k in kept):
                continue
            kept.append(o)
        return self.visible(kept, width, height)

    def visible(self, objects, width, height, keep=0.25):
        """Only the objects at least `keep` of which can be seen: paint each object's own pixels (a sprite's opaque
        ones, a whole rect or button, a text's box) with its number, back to front, and count what's left of each."""
        from PIL import Image
        ids = Image.new("I", (width, height), 0)
        own = []
        for n, o in enumerate(objects, start=1):
            x1, y1, x2, y2 = o["px"]
            if o["type"] == "sprite":
                pic = Image.open(self.sprites[o["name"]]).convert("RGBA").resize((x2 - x1, y2 - y1), Image.NEAREST)
                if "x" in o.get("flip", ""):
                    pic = pic.transpose(Image.FLIP_LEFT_RIGHT)
                if "y" in o.get("flip", ""):
                    pic = pic.transpose(Image.FLIP_TOP_BOTTOM)
                mask = pic.getchannel("A").point(lambda a: 255 if a > 0 else 0)
            else:
                mask = Image.new("L", (x2 - x1, y2 - y1), 255)
            own.append(mask.histogram()[255])
            ids.paste(n, (x1, y1, x2, y2), mask)
        seen = [0] * (len(objects) + 1)
        for count, v in ids.getcolors(width * height):
            seen[v] = count
        return [o for n, o in enumerate(objects, start=1) if own[n - 1] and seen[n] >= keep * own[n - 1]]


def demo_sprites(folder, rng):
    """A dozen made-up pixel-art sprites (simple shapes in a few colours), for trying the pipeline without your own."""
    from PIL import Image, ImageDraw
    os.makedirs(folder, exist_ok=True)
    shapes = ["tree", "rock", "coin", "heart", "star", "slime", "house", "bush", "chest", "flower", "key", "potion"]
    for name in shapes:
        w, h = rng.choice([(16, 16), (16, 24), (24, 24), (32, 32), (16, 32)])
        img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        d = ImageDraw.Draw(img)
        c = tuple(rng.randint(40, 255) for _ in range(3))
        dark = tuple(v // 2 for v in c)
        kind = shapes.index(name) % 4
        if kind == 0:
            d.ellipse((1, 1, w - 2, h - 2), fill=c, outline=dark)
        elif kind == 1:
            d.rectangle((2, h // 3, w - 3, h - 2), fill=c, outline=dark)
            d.polygon([(1, h // 3), (w // 2, 1), (w - 2, h // 3)], fill=dark)
        elif kind == 2:
            d.polygon([(w // 2, 1), (w - 2, h - 2), (1, h - 2)], fill=c, outline=dark)
        else:
            d.rectangle((w // 3, 1, w - w // 3, h - 2), fill=dark)
            d.ellipse((1, 1, w - 2, h // 2 + 2), fill=c)
        img.putpixel((w // 2, h // 2), (255, 255, 255, 255))
        img.save(os.path.join(folder, name + ".png"))
    return folder


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--sprites", help="folder of sprite PNGs (the names the model learns)")
    ap.add_argument("--demo-sprites", action="store_true", help="make a few made-up sprites (in out/demo_sprites)")
    ap.add_argument("--fonts", help="folder of .ttf/.otf fonts to vary the text (default: a system font)")
    ap.add_argument("--count", type=int, default=2000)
    ap.add_argument("--val", type=int, default=None, help="how many go in val.jsonl (default 5%%, at least 1)")
    ap.add_argument("--sizes", default=",".join(f"{w}x{h}" for w, h in SIZES), help="picture sizes, e.g. 640x360,480x320")
    ap.add_argument("--seed", type=int, default=1)
    ap.add_argument("--out", default="scenes_dataset")
    args = ap.parse_args()

    rng = random.Random(args.seed)
    folder = args.sprites
    if args.demo_sprites:
        folder = demo_sprites(os.path.join(args.out, "demo_sprites"), rng)
    sprites = scenespec.sprite_library(folder)
    if folder and not sprites:
        sys.exit(f"no PNGs in {folder}")
    fonts = []
    if args.fonts:
        fonts = [os.path.join(args.fonts, f) for f in sorted(os.listdir(args.fonts))
                 if f.lower().endswith((".ttf", ".otf"))]
        if not fonts:
            sys.exit(f"no .ttf or .otf fonts in {args.fonts}")
    sizes = [tuple(int(v) for v in s.split("x")) for s in args.sizes.split(",")]
    names = list(sprites)
    text_prompt = scenespec.prompt(names)
    maker = SceneMaker(sprites, fonts, rng)

    os.makedirs(os.path.join(args.out, "images"), exist_ok=True)
    n_val = args.val if args.val is not None else max(1, args.count // 20)
    counts = {}
    with open(os.path.join(args.out, "train.jsonl"), "w", encoding="utf-8") as train, \
            open(os.path.join(args.out, "val.jsonl"), "w", encoding="utf-8") as val:
        for i in range(args.count):
            width, height = rng.choice(sizes)
            scene, style = maker.make(width, height)
            bad = scenespec.problems(scene, names)
            assert not bad, bad
            image = os.path.join("images", f"{i:06d}.png")
            scenespec.render(scene, width, height, sprites, style).save(os.path.join(args.out, image))
            line = json.dumps({"image": image, "prompt": text_prompt, "answer": scenespec.canonical(scene)},
                              ensure_ascii=False)
            (val if i < n_val else train).write(line + "\n")
            for o in scene["objects"]:
                counts[o["type"]] = counts.get(o["type"], 0) + 1
            if (i + 1) % 500 == 0:
                print(f"{i + 1:,} pictures")
    with open(os.path.join(args.out, "sprite_names.json"), "w", encoding="utf-8") as f:
        json.dump(names, f, indent=1)
    # The format and scoring code go with the pictures, so the Colab notebook can score answers the same way.
    os.makedirs(os.path.join(args.out, "tools"), exist_ok=True)
    for name in ("scenespec.py", "score.py"):
        shutil.copy(os.path.join(os.path.dirname(os.path.abspath(__file__)), name), os.path.join(args.out, "tools", name))
    print(f"wrote {args.count:,} pictures to {args.out} ({n_val} for checking): "
          + ", ".join(f"{n:,} {k}s" for k, n in sorted(counts.items())))
    if names:
        print(f"{len(names)} sprite names; each appears about {counts.get('sprite', 0) // len(names)} times")


if __name__ == "__main__":
    main()
