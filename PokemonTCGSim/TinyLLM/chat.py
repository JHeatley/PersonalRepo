"""Step 4: talk to it in the terminal.

    python chat.py                                  # the chat-tuned model (out/chat.pt)
    python chat.py --model out/pretrain.pt --raw    # the pre-trained one: it just continues whatever you type
    python chat.py --temperature 0.3                # steadier answers (0: always the most likely word)

Type /reset to forget the conversation, /quit to leave.
"""
import argparse
import os

import torch

from common import IM_END, IM_START, OUT, device_and_dtype, load_tokenizer
from model import load


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--model", default=os.path.join(OUT, "chat.pt"))
    ap.add_argument("--raw", action="store_true", help="no chat format: continue the text you type")
    ap.add_argument("--temperature", type=float, default=0.7)
    ap.add_argument("--top-k", type=int, default=40)
    ap.add_argument("--max-new", type=int, default=300)
    ap.add_argument("--cpu", action="store_true")
    args = ap.parse_args()

    device, _ = device_and_dtype(args.cpu)
    sp = load_tokenizer()
    model, _ = load(args.model, device)
    start, end, nl = sp.piece_to_id(IM_START), sp.piece_to_id(IM_END), sp.encode("\n")
    print(f"{os.path.basename(args.model)}: {model.params() / 1e6:.0f}M parameters. /reset, /quit")
    history = [sp.bos_id()]
    while True:
        try:
            text = input("\nyou> ").strip()
        except (EOFError, KeyboardInterrupt):
            break
        if text in ("/quit", "/exit"):
            break
        if text == "/reset":
            history = [sp.bos_id()]
            continue
        if not text:
            continue
        if args.raw:
            ids = [sp.bos_id()] + sp.encode(text)
            out = model.generate(ids, args.max_new, args.temperature, args.top_k, stop=(sp.eos_id(),))
            print(text + sp.decode(out))
            continue
        # The same layout it was tuned on: <|im_start|>user\n...<|im_end|>\n<|im_start|>assistant\n
        history += [start] + sp.encode("user\n") + sp.encode(text) + [end] + nl + [start] + sp.encode("assistant\n")
        if len(history) > model.cfg.context - args.max_new:
            history = [sp.bos_id()] + history[-(model.cfg.context - args.max_new - 1):]  # forget the oldest part
        with torch.no_grad():
            out = model.generate(history, args.max_new, args.temperature, args.top_k, stop=(end, sp.eos_id()))
        print("tiny> " + sp.decode(out).strip())
        history += out + [end] + nl


if __name__ == "__main__":
    main()
