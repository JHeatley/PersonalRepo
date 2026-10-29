"""Send a picture to a local vision model and save the scene it describes, for the Unity builder.

    python ask_model.py screenshot.png --sprites path/to/sprites                 # Ollama, model "pokemon-tcg"
    python ask_model.py screenshot.png --model qwen3-vl:4b-instruct               # the untrained model, for comparison
    python ask_model.py screenshot.png --api openai --url http://localhost:8080   # llama.cpp's llama-server
    python ask_model.py screenshot.png --preview preview.png                      # also draw the answer next to the picture

Writes screenshot.scene.json (or --out): the scene plus "width" and "height", which Unity's
Tools → Vision → Build Scene From JSON turns into GameObjects. --sprites must be the same folder the model was trained
with: its names go in the prompt (and only they are allowed in the answer), in the same order as in training.

The answer is held to the scene format while the model writes it (a JSON schema: Ollama's "format", llama-server's
"response_format"), so even an untrained model returns something the builder can read; --no-schema turns that off.
Standard library only, except --preview (Pillow).
"""
import argparse
import base64
import io
import json
import os
import sys
import urllib.error
import urllib.request

import scenespec


def image_bytes(path, max_side):
    """The picture as PNG bytes, shrunk so its longest side is at most max_side (the training pictures' size), and its
    original (width, height)."""
    try:
        from PIL import Image
    except ImportError:
        Image = None
    if Image is None:
        with open(path, "rb") as f:
            data = f.read()
        if data[:8] != b"\x89PNG\r\n\x1a\n":
            sys.exit("without Pillow (pip install pillow) only PNG pictures can be sent")
        width, height = int.from_bytes(data[16:20], "big"), int.from_bytes(data[20:24], "big")
        return data, (width, height)
    img = Image.open(path).convert("RGB")
    size = img.size
    if max_side and max(size) > max_side:
        scale = max_side / max(size)
        img = img.resize((max(1, round(size[0] * scale)), max(1, round(size[1] * scale))), Image.LANCZOS)
    out = io.BytesIO()
    img.save(out, "PNG")
    return out.getvalue(), size


def post(url, payload, timeout):
    req = urllib.request.Request(url, data=json.dumps(payload).encode("utf-8"),
                                 headers={"Content-Type": "application/json"})
    try:
        with urllib.request.urlopen(req, timeout=timeout) as r:
            return json.loads(r.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        raise RuntimeError(f"{url} answered {e.code}: {e.read().decode('utf-8', 'replace')[:500]}") from None
    except urllib.error.URLError as e:
        raise RuntimeError(f"can't reach {url} ({e.reason}). Is Ollama (or llama-server) running?") from None


def ask(png, model, sprite_names=(), api="ollama", url=None, use_schema=True, ctx=8192, timeout=600):
    """The model's answer (text) for a picture (PNG bytes)."""
    text = scenespec.prompt(sprite_names)
    b64 = base64.b64encode(png).decode("ascii")
    sch = scenespec.schema(sprite_names)
    if api == "ollama":
        payload = {"model": model, "stream": False, "options": {"temperature": 0, "num_ctx": ctx},
                   "messages": [{"role": "user", "content": text, "images": [b64]}]}
        if use_schema:
            payload["format"] = sch
        reply = post((url or "http://localhost:11434").rstrip("/") + "/api/chat", payload, timeout)
        return reply["message"]["content"]
    payload = {"model": model, "temperature": 0,
               "messages": [{"role": "user", "content": [
                   {"type": "image_url", "image_url": {"url": "data:image/png;base64," + b64}},
                   {"type": "text", "text": text}]}]}
    if use_schema:
        payload["response_format"] = {"type": "json_schema", "json_schema": {"name": "scene", "schema": sch}}
    reply = post((url or "http://localhost:8080").rstrip("/") + "/v1/chat/completions", payload, timeout)
    return reply["choices"][0]["message"]["content"]


def read_scene(answer, sprite_names=None):
    """(scene, problems): the scene in an answer and what's wrong with it (scene is None if there's no JSON at all)."""
    try:
        scene = scenespec.parse_answer(answer)
    except ValueError as e:
        return None, [f"the answer isn't JSON ({e})"]
    return scene, scenespec.problems(scene, sprite_names)


def side_by_side(original_path, scene, sprites, max_side=None):
    """The picture (left) next to the scene drawn from the model's answer (right)."""
    from PIL import Image
    img = Image.open(original_path).convert("RGB")
    if max_side and max(img.size) > max_side:
        scale = max_side / max(img.size)
        img = img.resize((round(img.size[0] * scale), round(img.size[1] * scale)), Image.LANCZOS)
    drawn = scenespec.render(scene, img.size[0], img.size[1], sprites)
    out = Image.new("RGB", (img.size[0] * 2 + 8, img.size[1]), (255, 255, 255))
    out.paste(img, (0, 0))
    out.paste(drawn, (img.size[0] + 8, 0))
    return out


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("image")
    ap.add_argument("--model", default="pokemon-tcg", help="the model's name in Ollama (or llama-server)")
    ap.add_argument("--sprites", help="the sprite folder the model was trained with")
    ap.add_argument("--api", choices=("ollama", "openai"), default="ollama",
                    help="ollama, or openai for llama.cpp's llama-server and other OpenAI-style servers")
    ap.add_argument("--url", help="the server (default http://localhost:11434 for Ollama, :8080 for llama-server)")
    ap.add_argument("--max-side", type=int, default=640, help="shrink bigger pictures to this (as in training)")
    ap.add_argument("--no-schema", action="store_true", help="don't hold the answer to the scene format")
    ap.add_argument("--ctx", type=int, default=8192, help="Ollama's context length for this request")
    ap.add_argument("--out", help="where to save the scene (default: next to the picture, .scene.json)")
    ap.add_argument("--preview", help="save a PNG of the picture next to the scene drawn from the answer")
    args = ap.parse_args()

    sprites = scenespec.sprite_library(args.sprites)
    names = list(sprites)
    png, (width, height) = image_bytes(args.image, args.max_side)
    try:
        answer = ask(png, args.model, names, args.api, args.url, not args.no_schema, args.ctx)
    except RuntimeError as e:
        sys.exit(str(e))
    scene, bad = read_scene(answer, names or None)
    if scene is None:
        print(answer)
        sys.exit("\n".join(bad))
    for p in bad:
        print("warning:", p)
    out = args.out or os.path.splitext(args.image)[0] + ".scene.json"
    with open(out, "w", encoding="utf-8") as f:
        json.dump({"width": width, "height": height, **scene}, f, indent=1, ensure_ascii=False)
    print(f"{len(scene.get('objects', []))} objects → {out}")
    if args.preview:
        if bad:
            print("(no preview: the scene has problems the renderer can't draw)")
        else:
            side_by_side(args.image, scene, sprites, args.max_side).save(args.preview)
            print(f"preview → {args.preview}")


if __name__ == "__main__":
    main()
