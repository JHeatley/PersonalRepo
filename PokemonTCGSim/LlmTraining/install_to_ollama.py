"""Turns the trained .gguf files (from train_colab.ipynb) into an Ollama model you can pick in the Ollama app.

    python install_to_ollama.py pokemon-tcg.Q4_K_M.gguf                 # makes the model "pokemon-tcg"
    python install_to_ollama.py pokemon-tcg.Q4_K_M.gguf --name my-tcg   # another name
    python install_to_ollama.py model.gguf --ctx 16384                  # a longer context (uses more memory)

The notebook makes two files: the model, and its image part (pokemon-tcg.mmproj.gguf). Keep them in the same folder: this
finds the image part by itself (or give it with --mmproj), and the model can then read pictures too. It uses Ollama's own
chat format for Qwen3-VL, which needs Ollama 0.30 or newer. It also sets the system prompt the training used.

A text-only model from the older Qwen3-4B notebook still works: it takes the chat format (TEMPLATE and stop words) from
the base model it was trained from (`ollama pull qwen3:4b`).
"""
import argparse
import glob
import os
import re
import struct
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
from build_dataset import SYSTEM  # the same instructions the training examples had

VISION_ARCHITECTURES = ("qwen3vl",)
MIN_OLLAMA = (0, 30, 0)  # the first Ollama with the qwen3-vl-instruct chat format


def gguf_architecture(path):
    """The model type written in a .gguf file's header (general.architecture), or None if it isn't there."""
    sizes = {0: 1, 1: 1, 2: 2, 3: 2, 4: 4, 5: 4, 6: 4, 7: 1, 10: 8, 11: 8, 12: 8}

    def read(f, fmt):
        return struct.unpack("<" + fmt, f.read(struct.calcsize(fmt)))[0]

    def skip(f, kind):
        if kind == 8:
            f.seek(read(f, "Q"), 1)
        elif kind == 9:
            item, count = read(f, "I"), read(f, "Q")
            if item in sizes:
                f.seek(sizes[item] * count, 1)
            else:
                for _ in range(count):
                    skip(f, item)
        else:
            f.seek(sizes[kind], 1)

    with open(path, "rb") as f:
        if f.read(4) != b"GGUF":
            return None
        read(f, "I")  # version
        read(f, "Q")  # tensor count
        for _ in range(read(f, "Q")):
            key = f.read(read(f, "Q")).decode("utf-8", "replace")
            kind = read(f, "I")
            if key == "general.architecture" and kind == 8:
                return f.read(read(f, "Q")).decode("utf-8", "replace")
            skip(f, kind)
    return None


def find_projector(model_path):
    """The image part next to the model: a .gguf in the same folder with "mmproj" in its name."""
    folder = os.path.dirname(model_path)
    found = sorted(p for p in glob.glob(os.path.join(folder, "*.gguf")) if "mmproj" in os.path.basename(p).lower())
    stem = os.path.basename(model_path).split(".")[0].lower()
    same = [p for p in found if os.path.basename(p).lower().startswith(stem)]
    return (same or found or [None])[0]


def ollama_version():
    """Ollama's version as a tuple (0, 40, 13), or None if the ollama command isn't there."""
    try:
        out = subprocess.run(["ollama", "--version"], capture_output=True, text=True, encoding="utf-8")
    except FileNotFoundError:
        return None
    m = re.search(r"(\d+)\.(\d+)\.(\d+)", out.stdout + out.stderr)
    return tuple(int(x) for x in m.groups()) if m else (0, 0, 0)


def modelfile_blocks(text):
    """The commands of a Modelfile as (command, value) pairs; values in triple quotes can span lines."""
    blocks = []
    lines = text.splitlines()
    i = 0
    while i < len(lines):
        line = lines[i]
        i += 1
        if not line.strip() or line.lstrip().startswith("#"):
            continue
        command, _, value = line.partition(" ")
        if value.startswith('"""') and not (len(value) >= 6 and value.endswith('"""')):
            parts = [value]
            while i < len(lines):
                parts.append(lines[i])
                i += 1
                if lines[i - 1].rstrip().endswith('"""'):
                    break
            value = "\n".join(parts)
        blocks.append((command.upper(), value))
    return blocks


def quoted(path):
    # Forward slashes: Windows takes them, and a backslash inside quotes could be read as an escape.
    return '"' + os.path.abspath(path).replace("\\", "/") + '"'


def vision_lines(gguf, projector):
    lines = [f"FROM {quoted(gguf)}"]
    if projector:
        lines.append(f"FROM {quoted(projector)}")
    # Ollama's own chat format for Qwen3-VL Instruct (the same as its qwen3-vl models), and Qwen's suggested sampling.
    lines += ["RENDERER qwen3-vl-instruct", "PARSER qwen3-vl-instruct",
              "PARAMETER temperature 0.7", "PARAMETER top_p 0.8", "PARAMETER top_k 20"]
    return lines


def text_lines(gguf, base):
    """A text-only Qwen3 model: the chat format and stop words come from the base model in Ollama."""
    shown = subprocess.run(["ollama", "show", base, "--modelfile"], capture_output=True, text=True, encoding="utf-8")
    if shown.returncode != 0:
        sys.exit(f"Ollama doesn't have {base}: run  ollama pull {base}  first.\n{shown.stderr.strip()}")
    lines = [f"FROM {quoted(gguf)}"]
    for command, value in modelfile_blocks(shown.stdout):
        # Keep its chat format and stop words; the file, licence, system prompt and context length are set here.
        if command == "TEMPLATE" or (command == "PARAMETER" and not value.startswith("num_ctx")):
            lines.append(f"{command} {value}")
    return lines


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("gguf", help="the trained model file (.gguf)")
    ap.add_argument("--mmproj", help="its image part (default: the .gguf with \"mmproj\" in its name next to it)")
    ap.add_argument("--name", default="pokemon-tcg", help="the model's name in Ollama")
    ap.add_argument("--base", default="qwen3:4b", help="for a text-only Qwen3 model: the Ollama model it was trained from")
    ap.add_argument("--ctx", type=int, default=8192, help="context length (8192 fits a 6 GB card with room to spare)")
    args = ap.parse_args()

    gguf = os.path.abspath(args.gguf)
    if not os.path.exists(gguf):
        sys.exit(f"Can't find {gguf}")
    version = ollama_version()
    if version is None:
        sys.exit("The ollama command isn't found: install Ollama (ollama.com) and open a new command prompt.")
    arch = gguf_architecture(gguf)
    if arch in VISION_ARCHITECTURES:
        if version < MIN_OLLAMA:
            sys.exit(f"This model needs Ollama {'.'.join(map(str, MIN_OLLAMA))} or newer (you have "
                     f"{'.'.join(map(str, version))}): update it from ollama.com.")
        projector = os.path.abspath(args.mmproj) if args.mmproj else find_projector(gguf)
        if projector and not os.path.exists(projector):
            sys.exit(f"Can't find {projector}")
        if projector:
            print(f"model: {gguf}\nimage part: {projector}")
        else:
            print("warning: no image part (a .gguf with \"mmproj\" in its name) next to the model, so it won't read "
                  "pictures. Put the notebook's pokemon-tcg.mmproj.gguf in the same folder and run this again.")
        lines = vision_lines(gguf, projector)
    else:
        lines = text_lines(gguf, args.base)
    lines.append(f"PARAMETER num_ctx {args.ctx}")
    lines.append(f'SYSTEM """{SYSTEM}"""')
    path = os.path.join(os.path.dirname(gguf), "Modelfile")
    with open(path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines) + "\n")
    print(f"wrote {path}")

    made = subprocess.run(["ollama", "create", args.name, "-f", path])
    if made.returncode != 0:
        sys.exit("ollama create failed (see above).")
    print(f"\nDone: pick '{args.name}' in the Ollama app, or run  ollama run {args.name}")
    if arch in VISION_ARCHITECTURES:
        print("For pictures: in the Ollama app, add a picture to your message; to rebuild one as a Unity scene, "
              "use VisionTools/ask_model.py (see its README).")


if __name__ == "__main__":
    main()
