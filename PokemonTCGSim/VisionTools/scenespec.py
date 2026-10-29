"""The scene format a vision model answers in, and a renderer that draws it.

A scene is what a picture shows, as a list of simple objects drawn back to front:

    {"background": "#203040",
     "objects": [{"type": "rect",   "box": [0, 780, 1000, 1000], "colour": "#3A5F2B"},
                 {"type": "sprite", "box": [150, 110, 250, 380], "name": "tree", "flip": "x"},
                 {"type": "text",   "box": [340, 830, 660, 900], "text": "PRESS START", "colour": "#FFFFFF"},
                 {"type": "button", "box": [400, 600, 600, 680], "text": "Options", "colour": "#2050A0",
                  "textColour": "#FFFFFF"}]}

A box is [left, top, right, bottom] from 0 to 1000 across and down the picture, whatever its size: the same convention
Qwen3-VL uses to point at things in images, so the model starts from something it already knows. A file for Unity adds
"width" and "height" (the picture's size in pixels), which turn the boxes into pixels.

Needs Pillow for rendering (pip install pillow); the rest is standard library.
"""
import json
import os
import re

TYPES = ("rect", "sprite", "text", "button")
FLIPS = ("x", "y", "xy")
# Key order in the canonical JSON. It matches the order constrained decoding (Ollama's and llama.cpp's JSON schema
# grammars) writes them in: required keys first, then the optional ones in schema order.
KEY_ORDER = ("type", "box", "name", "text", "colour", "textColour", "flip")
COLOUR = re.compile(r"^#[0-9A-Fa-f]{6}$")


def prompt(sprite_names=()):
    """The instruction given with each picture, in training and when asking the model."""
    text = ("Rebuild this picture as a Unity scene. Reply with JSON only: {\"background\": colour, \"objects\": [...]}, "
            "the objects in drawing order from back to front. Every object has a \"type\" (rect, sprite, text or "
            "button) and a \"box\" [left, top, right, bottom] from 0 to 1000 across and down the picture. A rect has a "
            "\"colour\"; a sprite has a \"name\" and, if it is mirrored, \"flip\" (x, y or xy); a text has its "
            "\"text\" and \"colour\"; a button has \"text\", \"colour\" and \"textColour\". Colours are \"#RRGGBB\".")
    if sprite_names:
        text += " Sprite names: " + ", ".join(sprite_names) + "."
    return text


def schema(sprite_names=()):
    """A JSON schema for the answer, for constrained decoding (Ollama's "format", llama.cpp's "json_schema")."""
    obj = {"type": "object",
           "properties": {"type": {"enum": list(TYPES)},
                          "box": {"type": "array", "items": {"type": "integer", "minimum": 0, "maximum": 1000},
                                  "minItems": 4, "maxItems": 4},
                          "name": {"enum": list(sprite_names)} if sprite_names else {"type": "string"},
                          "text": {"type": "string"},
                          "colour": {"type": "string"},
                          "textColour": {"type": "string"},
                          "flip": {"enum": list(FLIPS)}},
           "required": ["type", "box"]}
    return {"type": "object",
            "properties": {"background": {"type": "string"}, "objects": {"type": "array", "items": obj}},
            "required": ["background", "objects"]}


def sprite_library(folder):
    """{name: path} for every PNG in a folder (and its subfolders): the file name without .png is the sprite's name."""
    found = {}
    if not folder:
        return found
    for root, _, files in os.walk(folder):
        for f in sorted(files):
            if f.lower().endswith(".png"):
                name = os.path.splitext(f)[0]
                if name in found:
                    raise ValueError(f"two sprites are called {name!r}: {found[name]} and {os.path.join(root, f)}")
                found[name] = os.path.join(root, f)
    return dict(sorted(found.items()))


# ---------------------------------------------------------------------------------------------------- Checking


def problems(scene, sprite_names=None):
    """What is wrong with a scene (an empty list if nothing)."""
    out = []
    if not isinstance(scene, dict):
        return ["the answer is not a JSON object"]
    if not COLOUR.match(str(scene.get("background", ""))):
        out.append(f"background {scene.get('background')!r} is not a #RRGGBB colour")
    objects = scene.get("objects")
    if not isinstance(objects, list):
        return out + ["objects is not a list"]
    for i, o in enumerate(objects):
        where = f"object {i}"
        if not isinstance(o, dict):
            out.append(f"{where} is not an object")
            continue
        kind = o.get("type")
        if kind not in TYPES:
            out.append(f"{where}: unknown type {kind!r}")
            continue
        box = o.get("box")
        if (not isinstance(box, list) or len(box) != 4 or not all(isinstance(v, (int, float)) for v in box)
                or not all(0 <= v <= 1000 for v in box)):
            out.append(f"{where}: box must be 4 numbers from 0 to 1000")
        elif box[2] <= box[0] or box[3] <= box[1]:
            out.append(f"{where}: box {box} has no size (right must be more than left, bottom more than top)")
        needs = {"rect": ("colour",), "sprite": ("name",), "text": ("text", "colour"),
                 "button": ("text", "colour", "textColour")}[kind]
        for key in needs:
            if key not in o:
                out.append(f"{where} ({kind}) has no {key}")
        for key in ("colour", "textColour"):
            if key in o and not COLOUR.match(str(o[key])):
                out.append(f"{where}: {key} {o[key]!r} is not a #RRGGBB colour")
        if kind == "sprite" and sprite_names is not None and o.get("name") not in sprite_names:
            out.append(f"{where}: no sprite called {o.get('name')!r}")
        if "flip" in o and o["flip"] not in FLIPS:
            out.append(f"{where}: flip must be x, y or xy")
    return out


def canonical(scene):
    """The scene as compact JSON with its keys in a fixed order: the exact text a model is trained to write."""
    objects = []
    for o in scene["objects"]:
        o = dict(o)
        o["box"] = [int(round(v)) for v in o["box"]]
        for key in ("colour", "textColour"):
            if key in o:
                o[key] = o[key].upper()
        objects.append({k: o[k] for k in KEY_ORDER if k in o})
    return json.dumps({"background": scene["background"].upper(), "objects": objects}, ensure_ascii=False,
                      separators=(",", ":"))


def parse_answer(text):
    """The scene in a model's answer: the JSON itself, or the first {...} in it (some models wrap it in prose or ```)."""
    text = text.strip()
    try:
        return json.loads(text)
    except ValueError:
        pass
    start = text.find("{")
    end = text.rfind("}")
    if start < 0 or end <= start:
        raise ValueError("no JSON object in the answer")
    return json.loads(text[start:end + 1])


def rgb(colour):
    return tuple(int(colour[i:i + 2], 16) for i in (1, 3, 5))


def to_pixels(box, width, height):
    """A 0-1000 box as whole pixels (left, top, right, bottom), at least 1 pixel wide and tall."""
    x1, y1 = round(box[0] * width / 1000), round(box[1] * height / 1000)
    x2, y2 = round(box[2] * width / 1000), round(box[3] * height / 1000)
    return x1, y1, max(x2, x1 + 1), max(y2, y1 + 1)


def to_box(px, width, height):
    """Pixels (left, top, right, bottom) as a 0-1000 box."""
    return [round(px[0] * 1000 / width), round(px[1] * 1000 / height),
            round(px[2] * 1000 / width), round(px[3] * 1000 / height)]


# ---------------------------------------------------------------------------------------------------- Rendering

DEFAULT_FONTS = ("arial.ttf", "DejaVuSans.ttf", "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
                 "/System/Library/Fonts/Supplemental/Arial.ttf", "LiberationSans-Regular.ttf")


def load_font(path, size):
    from PIL import ImageFont
    for p in ([path] if path else []) + list(DEFAULT_FONTS):
        try:
            return ImageFont.truetype(p, size)
        except OSError:
            continue
    return ImageFont.load_default(size)


def fit_text(draw, text, box_px, font_path, max_size=None):
    """The font size and position that make the text's ink fill box_px (left, top, right, bottom): as tall as the box,
    or narrower if it would be too wide, centred in it."""
    x1, y1, x2, y2 = box_px
    size = max(4, y2 - y1)
    for _ in range(4):
        font = load_font(font_path, size)
        l, t, r, b = draw.textbbox((0, 0), text, font=font)
        if r - l <= 0 or b - t <= 0:
            break
        scale = min((y2 - y1) / (b - t), (x2 - x1) / (r - l))
        new = max(4, int(size * scale))
        if max_size:
            new = min(new, max_size)
        if new == size:
            break
        size = new
    font = load_font(font_path, size)
    l, t, r, b = draw.textbbox((0, 0), text, font=font)
    cx, cy = (x1 + x2) / 2, (y1 + y2) / 2
    return font, (round(cx - (l + r) / 2), round(cy - (t + b) / 2))


class Style:
    """How things look that the scene format doesn't record, picked at random for variety in the training pictures:
    the font, rounded or outlined buttons, shadows under text."""

    def __init__(self, font=None, button_radius=0, button_outline=None, text_shadow=False):
        self.font = font
        self.button_radius = button_radius
        self.button_outline = button_outline
        self.text_shadow = text_shadow


def render(scene, width, height, sprites=None, style=None, missing="magenta"):
    """Draw a scene as a width x height RGB picture (PIL Image). sprites is {name: path}; a sprite that isn't there is
    drawn as a magenta box (missing="magenta") or skipped (missing="skip")."""
    from PIL import Image, ImageDraw
    style = style or Style()
    sprites = sprites or {}
    cache = {}
    img = Image.new("RGBA", (width, height), rgb(scene["background"]) + (255,))
    draw = ImageDraw.Draw(img)
    for o in scene["objects"]:
        x1, y1, x2, y2 = to_pixels(o["box"], width, height)
        kind = o["type"]
        if kind == "rect":
            draw.rectangle((x1, y1, x2 - 1, y2 - 1), fill=rgb(o["colour"]))
        elif kind == "sprite":
            path = sprites.get(o.get("name"))
            if path is None:
                if missing == "magenta":
                    draw.rectangle((x1, y1, x2 - 1, y2 - 1), outline=(255, 0, 255), width=2)
                continue
            if path not in cache:
                cache[path] = Image.open(path).convert("RGBA")
            pic = cache[path].resize((x2 - x1, y2 - y1), Image.NEAREST)
            flip = o.get("flip", "")
            if "x" in flip:
                pic = pic.transpose(Image.FLIP_LEFT_RIGHT)
            if "y" in flip:
                pic = pic.transpose(Image.FLIP_TOP_BOTTOM)
            img.alpha_composite(pic, (x1, y1))
            draw = ImageDraw.Draw(img)
        elif kind == "text":
            font, xy = fit_text(draw, o["text"], (x1, y1, x2, y2), style.font)
            if style.text_shadow:
                draw.text((xy[0] + 1, xy[1] + 1), o["text"], font=font, fill=(0, 0, 0))
            draw.text(xy, o["text"], font=font, fill=rgb(o["colour"]))
        elif kind == "button":
            draw.rounded_rectangle((x1, y1, x2 - 1, y2 - 1), radius=style.button_radius, fill=rgb(o["colour"]),
                                   outline=style.button_outline, width=2 if style.button_outline else 0)
            # The label: about half the button's height, with a margin at the sides.
            h = y2 - y1
            pad_x, pad_y = max(2, (x2 - x1) // 10), h // 4
            font, xy = fit_text(draw, o["text"], (x1 + pad_x, y1 + pad_y, x2 - pad_x, y2 - pad_y), style.font)
            draw.text(xy, o["text"], font=font, fill=rgb(o["textColour"]))
    return img.convert("RGB")
