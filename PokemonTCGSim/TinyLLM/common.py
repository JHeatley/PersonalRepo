"""Shared bits of TinyLLM: folders, the chat format, the tokenizer, and the GPU settings for your card."""
import os

HERE = os.path.dirname(os.path.abspath(__file__))
DATA = os.path.join(HERE, "data")
OUT = os.path.join(HERE, "out")
LLM_TRAINING = os.path.join(HERE, "..", "LlmTraining")

# The chat format (the same "ChatML" layout Qwen uses). Each marker is one token of the tokenizer, so the model learns
# exactly where a turn starts and ends; <|im_end|> is also what stops it talking.
IM_START, IM_END = "<|im_start|>", "<|im_end|>"
SPECIAL = [IM_START, IM_END]


def chat_text(messages, add_reply_start=False):
    """Messages ([{"role", "content"}]) in the chat format; with add_reply_start, ends ready for the assistant's reply."""
    parts = [f"{IM_START}{m['role']}\n{m['content']}{IM_END}\n" for m in messages]
    if add_reply_start:
        parts.append(f"{IM_START}assistant\n")
    return "".join(parts)


def load_tokenizer():
    import sentencepiece as spm
    path = os.path.join(DATA, "tokenizer.model")
    if not os.path.exists(path):
        raise SystemExit("No tokenizer yet: run prepare_data.py first.")
    return spm.SentencePieceProcessor(model_file=path)


def device_and_dtype(force_cpu=False):
    """The GPU if there is one, and the 16-bit format to train in: bfloat16 on RTX 30-series and newer, float16 (with a
    loss scaler) on older cards like the RTX 20-series, whose tensor cores don't do bfloat16 fast."""
    import torch
    if force_cpu or not torch.cuda.is_available():
        return "cpu", torch.float32
    major, _ = torch.cuda.get_device_capability()
    return "cuda", (torch.bfloat16 if major >= 8 else torch.float16)


def human(n):
    for unit in ("", "K", "M", "B"):
        if abs(n) < 1000:
            return f"{n:.1f}{unit}" if unit else f"{n:.0f}"
        n /= 1000
    return f"{n:.1f}T"
