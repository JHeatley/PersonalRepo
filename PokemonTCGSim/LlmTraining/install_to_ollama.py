"""Turns the trained .gguf file (from train_colab.ipynb) into an Ollama model you can pick in the Ollama app.

    python install_to_ollama.py pokemon-tcg.Q4_K_M.gguf                 # makes the model "pokemon-tcg"
    python install_to_ollama.py pokemon-tcg.Q4_K_M.gguf --name my-tcg   # another name
    python install_to_ollama.py model.gguf --ctx 16384                  # a longer context (uses more memory)

It needs Ollama running and the base model it was trained from (`ollama pull qwen3:4b`): the new model takes that one's
chat format (TEMPLATE and stop words), so it answers the same way. It also sets the system prompt the training used.
"""
import argparse
import os
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
from build_dataset import SYSTEM  # the same instructions the training examples had


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


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("gguf", help="the trained model file (.gguf)")
    ap.add_argument("--name", default="pokemon-tcg", help="the model's name in Ollama")
    ap.add_argument("--base", default="qwen3:4b", help="the Ollama model it was trained from (for its chat format)")
    ap.add_argument("--ctx", type=int, default=8192, help="context length (8192 fits a 6 GB card with room to spare)")
    args = ap.parse_args()

    gguf = os.path.abspath(args.gguf)
    if not os.path.exists(gguf):
        sys.exit(f"Can't find {gguf}")
    try:
        base = subprocess.run(["ollama", "show", args.base, "--modelfile"], capture_output=True, text=True, encoding="utf-8")
    except FileNotFoundError:
        sys.exit("The ollama command isn't found: install Ollama (ollama.com) and open a new command prompt.")
    if base.returncode != 0:
        sys.exit(f"Ollama doesn't have {args.base}: run  ollama pull {args.base}  first.\n{base.stderr.strip()}")

    # Forward slashes: Windows takes them, and a backslash inside quotes could be read as an escape.
    lines = [f'FROM "{gguf.replace(chr(92), "/")}"']
    for command, value in modelfile_blocks(base.stdout):
        # Keep its chat format and stop words; the file, licence, system prompt and context length are set here.
        if command == "TEMPLATE" or (command == "PARAMETER" and not value.startswith("num_ctx")):
            lines.append(f"{command} {value}")
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


if __name__ == "__main__":
    main()
