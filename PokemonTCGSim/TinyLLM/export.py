"""Step 5: put it in Ollama.

    python export.py --llama-cpp C:/path/to/llama.cpp --ollama-name tinyllm

  1. Saves the model as a Hugging Face Llama model in out/export/ (it's built in the Llama layout, so nothing changes).
  2. With --llama-cpp (a copy of github.com/ggml-org/llama.cpp), converts that to out/tinyllm.gguf with llama.cpp's own
     converter, the file Ollama reads.
  3. Writes out/Modelfile (the chat format and the word that ends a reply) and, with --ollama-name, runs `ollama create`.
     Then: ollama run tinyllm  (or pick it in the Ollama app).

Without --llama-cpp it stops after step 1 and prints the commands to finish by hand.
"""
import argparse
import json
import os
import shutil
import subprocess
import sys

import torch

from common import DATA, IM_END, IM_START, OUT, load_tokenizer
from model import load

CHAT_TEMPLATE = ("{% for m in messages %}<|im_start|>{{ m['role'] }}\n{{ m['content'] }}<|im_end|>\n{% endfor %}"
                 "{% if add_generation_prompt %}<|im_start|>assistant\n{% endif %}")

OLLAMA_TEMPLATE = """{{- range .Messages }}<|im_start|>{{ .Role }}
{{ .Content }}<|im_end|>
{{ end }}<|im_start|>assistant
"""


def export_hf(model, sp, folder):
    """The Hugging Face layout llama.cpp's converter (and transformers) reads: config, weights, tokenizer."""
    os.makedirs(folder, exist_ok=True)
    cfg = model.cfg
    im_end = sp.piece_to_id(IM_END)
    config = {
        "architectures": ["LlamaForCausalLM"], "model_type": "llama",
        "vocab_size": cfg.vocab_size, "hidden_size": cfg.dim, "intermediate_size": cfg.hidden,
        "num_hidden_layers": cfg.layers, "num_attention_heads": cfg.heads, "num_key_value_heads": cfg.heads,
        "head_dim": cfg.dim // cfg.heads, "max_position_embeddings": cfg.context, "rms_norm_eps": cfg.norm_eps,
        "rope_theta": cfg.rope_theta, "hidden_act": "silu", "tie_word_embeddings": True,
        "attention_bias": False, "mlp_bias": False, "bos_token_id": sp.bos_id(), "eos_token_id": im_end,
        "torch_dtype": "float32",
    }
    with open(os.path.join(folder, "config.json"), "w") as f:
        json.dump(config, f, indent=2)
    with open(os.path.join(folder, "generation_config.json"), "w") as f:
        json.dump({"bos_token_id": sp.bos_id(), "eos_token_id": [im_end, sp.eos_id()]}, f, indent=2)
    # The weights, without lm_head (it's the embedding, tied: tie_word_embeddings).
    weights = {k: v.detach().float().contiguous().cpu() for k, v in model.state_dict().items() if k != "lm_head.weight"}
    try:
        from safetensors.torch import save_file
        save_file(weights, os.path.join(folder, "model.safetensors"), metadata={"format": "pt"})
    except ImportError:
        torch.save(weights, os.path.join(folder, "pytorch_model.bin"))
    shutil.copy(os.path.join(DATA, "tokenizer.model"), os.path.join(folder, "tokenizer.model"))
    tok = {
        "tokenizer_class": "LlamaTokenizer", "bos_token": sp.id_to_piece(sp.bos_id()), "eos_token": IM_END,
        "unk_token": sp.id_to_piece(sp.unk_id()), "add_bos_token": True, "add_eos_token": False,
        "add_prefix_space": False, "legacy": False, "model_max_length": cfg.context,
        "additional_special_tokens": [IM_START, IM_END], "chat_template": CHAT_TEMPLATE,
        # The chat markers as special (control) tokens: llama.cpp's converter only treats them as single special tokens,
        # matched whole in a prompt and never printed, when they're listed here; otherwise they'd be read as plain text.
        "added_tokens_decoder": {
            str(sp.piece_to_id(t)): {"content": t, "special": True, "lstrip": False, "rstrip": False,
                                     "normalized": False, "single_word": False}
            for t in (IM_START, IM_END)
        },
    }
    with open(os.path.join(folder, "tokenizer_config.json"), "w") as f:
        json.dump(tok, f, indent=2)
    with open(os.path.join(folder, "special_tokens_map.json"), "w") as f:
        json.dump({k: tok[k] for k in ("bos_token", "eos_token", "unk_token", "additional_special_tokens")}, f, indent=2)
    print(f"saved the Hugging Face model in {folder}")


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--model", default=os.path.join(OUT, "chat.pt"))
    ap.add_argument("--llama-cpp", default="", help="folder of a llama.cpp copy (git clone https://github.com/ggml-org/llama.cpp)")
    ap.add_argument("--ollama-name", default="", help="install it in Ollama under this name")
    args = ap.parse_args()

    sp = load_tokenizer()
    model, _ = load(args.model, "cpu")
    folder = os.path.join(OUT, "export")
    export_hf(model, sp, folder)
    gguf = os.path.join(OUT, "tinyllm.gguf")
    modelfile = os.path.join(OUT, "Modelfile")
    with open(modelfile, "w", encoding="utf-8") as f:
        f.write(f"FROM {gguf.replace(os.sep, '/')}\n")
        f.write(f'TEMPLATE """{OLLAMA_TEMPLATE}"""\n')
        f.write(f'PARAMETER stop "{IM_END}"\nPARAMETER temperature 0.7\nPARAMETER num_ctx {model.cfg.context}\n')

    if not args.llama_cpp:
        print("\nTo finish by hand (in a separate venv for the converter, so it can't change your training PyTorch):")
        print("  git clone https://github.com/ggml-org/llama.cpp")
        print("  pip install -r llama.cpp/requirements/requirements-convert_hf_to_gguf.txt")
        print(f"  python llama.cpp/convert_hf_to_gguf.py {folder} --outfile {gguf} --outtype f16")
        print(f"  ollama create tinyllm -f {modelfile}")
        return
    script = os.path.join(args.llama_cpp, "convert_hf_to_gguf.py")
    if not os.path.exists(script):
        raise SystemExit(f"{script} isn't there: point --llama-cpp at the llama.cpp folder")
    subprocess.run([sys.executable, script, folder, "--outfile", gguf, "--outtype", "f16"], check=True)
    print(f"wrote {gguf} and {modelfile}")
    if args.ollama_name:
        subprocess.run(["ollama", "create", args.ollama_name, "-f", modelfile], check=True)
        print(f"installed: ollama run {args.ollama_name}")


if __name__ == "__main__":
    main()
