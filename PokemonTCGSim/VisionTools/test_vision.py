"""Tests for the vision tools (no model or GPU needed: a fake Ollama/llama-server answers).

    python test_vision.py
"""
import base64
import http.server
import json
import os
import subprocess
import sys
import tempfile
import threading

import scenespec
import score

HERE = os.path.dirname(os.path.abspath(__file__))


class FakeServer(http.server.BaseHTTPRequestHandler):
    """Answers /api/chat (Ollama) and /v1/chat/completions (OpenAI style) with FakeServer.answer, keeping each request."""
    answer = "{}"
    requests = []

    def do_POST(self):
        body = json.loads(self.rfile.read(int(self.headers["Content-Length"])))
        FakeServer.requests.append((self.path, body))
        if self.path == "/api/chat":
            reply = {"message": {"role": "assistant", "content": FakeServer.answer}}
        else:
            reply = {"choices": [{"message": {"role": "assistant", "content": FakeServer.answer}}]}
        data = json.dumps(reply).encode()
        self.send_response(200)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(data)))
        self.end_headers()
        self.wfile.write(data)

    def log_message(self, *args):
        pass


def run(*args):
    return subprocess.run([sys.executable, *args], cwd=HERE, capture_output=True, text=True, check=True).stdout


def main():
    fails = []

    def check(cond, what):
        print(("ok    " if cond else "FAIL  ") + what)
        if not cond:
            fails.append(what)

    # ------------------------------------------------------------------ the format and scoring
    scene = {"background": "#102030", "objects": [
        {"type": "rect", "box": [0, 800, 1000, 1000], "colour": "#00ff00"},
        {"type": "sprite", "box": [100, 100, 200, 300], "name": "tree", "flip": "x"},
        {"type": "text", "box": [300, 50, 700, 120], "text": "PRESS START", "colour": "#FFFFFF"},
        {"type": "button", "box": [400, 500, 600, 580], "text": "Options", "colour": "#2050A0", "textColour": "#FFFFFF"}]}
    check(scenespec.problems(scene) == [], "a good scene has no problems")
    text = scenespec.canonical(scene)
    check(json.loads(text)["objects"][0]["colour"] == "#00FF00" and " " not in text.replace("PRESS START", ""),
          "canonical JSON is compact with upper-case colours")
    check(list(json.loads(text)["objects"][1]) == ["type", "box", "name", "flip"], "keys in the schema's order")
    check(scenespec.parse_answer("Here you go:\n```json\n" + text + "\n```") == json.loads(text),
          "an answer wrapped in prose and ``` is read")
    bad = {"background": "blue", "objects": [{"type": "sprite", "box": [5, 5, 5, 9], "name": "ghost"},
                                             {"type": "circle", "box": [0, 0, 1, 1]}]}
    found = scenespec.problems(bad, ["tree"])
    check(len(found) == 4, f"problems found in a bad scene: {found}")

    truth = json.loads(text)
    check(score.score(truth, truth)["f1"] == 1.0, "a perfect answer scores f1 100%")
    shifted = json.loads(text)
    shifted["objects"][1]["box"] = [110, 110, 210, 310]
    shifted["objects"][1]["name"] = "rock"
    s = score.score(truth, shifted)
    check(s["f1"] == 1.0 and s["names"] == 0.0, "a slightly moved sprite is still found, its wrong name counted")
    check(score.score(truth, {"background": "#102030", "objects": []})["found"] == 0.0, "an empty answer finds nothing")
    check(score.score(truth, None)["valid"] is False, "an unusable answer is marked invalid")
    half = {"background": "#FFFFFF", "objects": truth["objects"][:2]}
    s = score.score(truth, half)
    check(s["found"] == 0.5 and s["correct"] == 1.0 and s["background"] > 0.5, "half the objects: found 50%, correct 100%")

    with tempfile.TemporaryDirectory() as tmp:
        # ------------------------------------------------------------------ the dataset
        out = os.path.join(tmp, "ds")
        run("make_dataset.py", "--demo-sprites", "--count", "40", "--val", "4", "--out", out)
        with open(os.path.join(out, "train.jsonl"), encoding="utf-8") as f:
            train = [json.loads(line) for line in f]
        with open(os.path.join(out, "val.jsonl"), encoding="utf-8") as f:
            val = [json.loads(line) for line in f]
        names = json.load(open(os.path.join(out, "sprite_names.json")))
        check(len(train) == 36 and len(val) == 4 and len(names) == 12, "40 pictures: 36 to train, 4 to check, 12 sprites")
        answers = [json.loads(ex["answer"]) for ex in train + val]
        check(all(scenespec.problems(a, names) == [] for a in answers), "every answer is a valid scene")
        check(all(scenespec.canonical(a) == ex["answer"] for a, ex in zip(answers, train + val)),
              "every answer is in canonical form")
        check(all(os.path.exists(os.path.join(out, ex["image"])) for ex in train + val), "every picture was saved")
        check(all(", ".join(names) in ex["prompt"] for ex in train), "the prompt lists the sprite names")
        kinds = {o["type"] for a in answers for o in a["objects"]}
        check(kinds == set(scenespec.TYPES), f"all four kinds of object appear: {sorted(kinds)}")

        # A drawn scene puts the sprite where its box says.
        from PIL import Image
        sprites = scenespec.sprite_library(os.path.join(out, "demo_sprites"))
        one = {"background": "#000000", "objects": [{"type": "sprite", "box": [250, 250, 750, 750], "name": "coin"}]}
        img = scenespec.render(one, 200, 100, sprites)
        coin = Image.open(sprites["coin"]).convert("RGBA")
        w, h = coin.size
        centre = coin.getpixel((w // 2, h // 2))
        check(img.getpixel((100, 50)) == centre[:3], "a sprite is drawn into its box (its centre pixel at the centre)")
        check(img.getpixel((10, 10)) == (0, 0, 0), "the background fills the rest")

        # ------------------------------------------------------------------ asking a model (a fake one)
        server = http.server.HTTPServer(("127.0.0.1", 0), FakeServer)
        threading.Thread(target=server.serve_forever, daemon=True).start()
        url = f"http://127.0.0.1:{server.server_port}"
        picture = os.path.join(out, val[0]["image"])
        FakeServer.answer = val[0]["answer"]
        scene_path = os.path.join(tmp, "out.scene.json")
        preview = os.path.join(tmp, "preview.png")
        run("ask_model.py", picture, "--url", url, "--sprites", os.path.join(out, "demo_sprites"), "--out", scene_path,
            "--preview", preview)
        path, body = FakeServer.requests[-1]
        msg = body["messages"][0]
        check(path == "/api/chat" and base64.b64decode(msg["images"][0])[:4] == b"\x89PNG"
              and body["format"]["properties"]["objects"]["items"]["properties"]["name"]["enum"] == names
              and msg["content"] == val[0]["prompt"], "Ollama gets the picture, the training prompt and the schema")
        saved = json.load(open(scene_path))
        w, h = Image.open(picture).size
        check(saved["width"] == w and saved["height"] == h and saved["objects"] == json.loads(val[0]["answer"])["objects"],
              "the scene file has the picture's size and the model's objects")
        check(Image.open(preview).size == (2 * w + 8, h), "the preview shows the picture and the drawn answer")

        run("ask_model.py", picture, "--api", "openai", "--url", url, "--model", "m", "--out", scene_path,
            "--sprites", os.path.join(out, "demo_sprites"))
        path, body = FakeServer.requests[-1]
        parts = body["messages"][0]["content"]
        check(path == "/v1/chat/completions" and parts[0]["image_url"]["url"].startswith("data:image/png;base64,")
              and parts[1]["text"] == val[0]["prompt"] and body["response_format"]["type"] == "json_schema",
              "llama-server gets the picture, the prompt and the schema")

        big = os.path.join(tmp, "big.png")
        Image.new("RGB", (1920, 1080), (9, 9, 9)).save(big)
        run("ask_model.py", big, "--url", url, "--out", scene_path)
        sent = Image.open(__import__("io").BytesIO(base64.b64decode(FakeServer.requests[-1][1]["messages"][0]["images"][0])))
        check(sent.size == (640, 360) and json.load(open(scene_path))["width"] == 1920,
              "a big screenshot is shrunk to 640 for the model; the scene keeps its real size")

        report = run("evaluate.py", out, "--url", url, "--limit", "1", "--model", "fake")
        check("f1 100%" in report, "evaluate scores a perfect answer 100%")
        check(os.path.exists(os.path.join(out, "eval_fake", "000.png")), "evaluate saves previews")
        server.shutdown()

    print("all passed" if not fails else f"{len(fails)} failed")
    sys.exit(1 if fails else 0)


if __name__ == "__main__":
    main()
