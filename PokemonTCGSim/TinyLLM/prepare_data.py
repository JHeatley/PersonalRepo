"""Step 1: gathers the text, trains the tokenizer, and turns everything into the token files the training reads.

    python prepare_data.py                      # your data + 800 MB of general English (downloaded once)
    python prepare_data.py --general-mb 2000    # more general English (better grammar and general knowledge)
    python prepare_data.py --general-mb 0       # only your own data (no download; a tiny model will mostly memorise it)

The text comes from:
  * ../LlmTraining/train.jsonl (built by build_dataset.py: TCG rulings and cards, and everything in ../LlmTraining/extra,
    such as the Pokémon games, Thomas & Friends, Unity, C# and Python). If it isn't built, the extra/*.jsonl files.
    These conversations are used twice: as text to learn the language from (pre-training) and as the examples that
    teach it to answer (chat_tune.py).
  * my_text/*.txt and *.md next to this file: anything else you want it to read (your notes, stories, scripts).
  * General English from FineWeb-Edu (educational web pages, huggingface.co/datasets/HuggingFaceFW/fineweb-edu), so
    it learns to write sentences, not only to repeat your data. Needs `pip install datasets`; saved in data/.

Writes data/tokenizer.model, data/pretrain_train.bin, data/pretrain_val.bin, data/chat_*.bin and data/info.json.
"""
import argparse
import glob
import json
import os
import random

import numpy as np

from common import DATA, HERE, LLM_TRAINING, SPECIAL, IM_START, IM_END, human


def domain_conversations():
    """The chat examples from ../LlmTraining: train.jsonl if it's built, otherwise the files in extra/."""
    built = os.path.join(LLM_TRAINING, "train.jsonl")
    files = [built] if os.path.exists(built) else sorted(glob.glob(os.path.join(LLM_TRAINING, "extra", "*.jsonl")))
    convos = []
    for path in files:
        with open(path, encoding="utf-8") as f:
            for line in f:
                if not line.strip():
                    continue
                # The long system prompt is left out: a small model's context is short, and it has only one job.
                msgs = [m for m in json.loads(line)["messages"] if m["role"] in ("user", "assistant") and m["content"].strip()]
                if len(msgs) >= 2:
                    convos.append(msgs)
    print(f"{len(convos):,} conversations from {', '.join(os.path.basename(p) for p in files) or 'nowhere (build ../LlmTraining first)'}")
    return convos


def my_texts():
    docs = []
    for path in sorted(glob.glob(os.path.join(HERE, "my_text", "**", "*.*"), recursive=True)):
        if path.lower().endswith((".txt", ".md")):
            with open(path, encoding="utf-8", errors="replace") as f:
                docs.append(f.read())
    if docs:
        print(f"{len(docs):,} documents from my_text/")
    return docs


def general_texts(megabytes):
    """General English from FineWeb-Edu, downloaded once (streamed, so only what's used is downloaded) and kept in data/."""
    if megabytes <= 0:
        return []
    cache = os.path.join(DATA, "general.jsonl")
    have = os.path.getsize(cache) if os.path.exists(cache) else 0
    if have < megabytes * 1_000_000 * 0.98:
        try:
            from datasets import load_dataset
        except ImportError:
            raise SystemExit("General English needs the datasets package: pip install datasets  (or use --general-mb 0)")
        print(f"downloading {megabytes} MB of FineWeb-Edu (once; it's kept in data/general.jsonl)...")
        stream = load_dataset("HuggingFaceFW/fineweb-edu", name="sample-10BT", split="train", streaming=True)
        size, n = 0, 0
        with open(cache, "w", encoding="utf-8") as f:
            for row in stream:
                text = row["text"].strip()
                if len(text) < 200:
                    continue
                line = json.dumps({"text": text}, ensure_ascii=False)
                f.write(line + "\n")
                size += len(line.encode("utf-8"))
                n += 1
                if n % 20000 == 0:
                    print(f"  {size / 1e6:,.0f} MB")
                if size >= megabytes * 1_000_000:
                    break
    docs = []
    size = 0
    with open(cache, encoding="utf-8") as f:
        for line in f:
            docs.append(json.loads(line)["text"])
            size += len(line)
            if size >= megabytes * 1_000_000:
                break
    print(f"{len(docs):,} general documents ({size / 1e6:,.0f} MB)")
    return docs


def conversation_text(msgs):
    """A conversation as plain text for pre-training: the questions and answers as they are."""
    return "\n\n".join(m["content"] for m in msgs)


def train_tokenizer(docs, vocab):
    """A SentencePiece BPE tokenizer: common words become one token, rare ones are spelled out (bytes as a last resort)."""
    import sentencepiece as spm
    corpus = os.path.join(DATA, "tokenizer_corpus.txt")
    rng = random.Random(1)
    sample = docs if len(docs) <= 200_000 else rng.sample(docs, 200_000)
    with open(corpus, "w", encoding="utf-8") as f:
        for d in sample:
            for para in d.split("\n"):
                if para.strip():
                    f.write(para[:8000] + "\n")
    spm.SentencePieceTrainer.train(
        input=corpus, model_prefix=os.path.join(DATA, "tokenizer"), vocab_size=vocab, model_type="bpe",
        byte_fallback=True,                 # any character can be written, as bytes if need be
        user_defined_symbols=SPECIAL,       # the chat markers are single tokens
        normalization_rule_name="identity",  # keep text exactly as written (code needs its symbols)
        remove_extra_whitespaces=False,     # keep code's indentation
        add_dummy_prefix=False,             # no invisible leading space (so pieces encode the same alone or together)
        split_digits=True,                  # numbers digit by digit: much better at arithmetic and damage numbers
        character_coverage=0.9999, max_sentence_length=16384, input_sentence_size=3_000_000, shuffle_input_sentence=True,
        unk_id=0, bos_id=1, eos_id=2, pad_id=-1, num_threads=os.cpu_count() or 4, minloglevel=2)
    os.remove(corpus)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--general-mb", type=int, default=800, help="megabytes of general English (0: none)")
    ap.add_argument("--vocab", type=int, default=8000, help="tokenizer size (8000 suits small models)")
    ap.add_argument("--domain-repeat", type=int, default=3,
                    help="how many times your own data is read per pass over the general text (weighs it up)")
    args = ap.parse_args()
    os.makedirs(DATA, exist_ok=True)

    convos = domain_conversations()
    mine = my_texts()
    general = general_texts(args.general_mb)
    domain_docs = [conversation_text(c) for c in convos] + mine
    if not domain_docs and not general:
        raise SystemExit("No text: build ../LlmTraining (build_dataset.py), add files to my_text/, or allow --general-mb.")

    print("training the tokenizer...")
    train_tokenizer(domain_docs + general, args.vocab)
    from common import load_tokenizer
    sp = load_tokenizer()
    bos, eos = sp.bos_id(), sp.eos_id()

    # Pre-training text: every document between BOS and EOS, 1 in 100 kept aside to check it isn't just memorising.
    # Written to the files in batches (encoded on every CPU thread), so even gigabytes of text don't fill the memory.
    rng = random.Random(7)
    docs = general + domain_docs * max(1, args.domain_repeat)
    rng.shuffle(docs)
    n_train = n_val = 0
    threads = os.cpu_count() or 4
    with open(os.path.join(DATA, "pretrain_train.bin"), "wb") as ftrain, open(os.path.join(DATA, "pretrain_val.bin"), "wb") as fval:
        for at in range(0, len(docs), 2000):
            batch = docs[at:at + 2000]
            for k, ids in enumerate(sp.encode(batch, num_threads=threads)):
                arr = np.array([bos] + ids + [eos], dtype=np.uint16)
                if (at + k) % 100 == 0:
                    arr.tofile(fval)
                    n_val += len(arr)
                else:
                    arr.tofile(ftrain)
                    n_train += len(arr)
            if at and at % 100000 == 0:
                print(f"  {at:,} of {len(docs):,} documents")
    print(f"pre-training text: {human(n_train)} tokens (+{human(n_val)} kept aside)")

    # Chat examples: tokens, and a mask of the ones the loss counts (the assistant's replies, and the end marker after
    # each, so it learns to stop). Every piece is encoded on its own, exactly as the whole would be (add_dummy_prefix off).
    start, end, nl = sp.piece_to_id(IM_START), sp.piece_to_id(IM_END), sp.encode("\n")
    tokens, mask, index = [], [], []
    for msgs in convos:
        t, m = [bos], [0]
        for msg in msgs:
            head = [start] + sp.encode(msg["role"] + "\n")
            body = sp.encode(msg["content"]) + [end]
            t += head + body + nl
            reply = msg["role"] == "assistant"
            m += [0] * len(head) + [1 if reply else 0] * len(body) + [0] * len(nl)
        index.append((len(tokens), len(t)))
        tokens += t
        mask += m
    np.array(tokens, dtype=np.uint16).tofile(os.path.join(DATA, "chat_tokens.bin"))
    np.array(mask, dtype=np.uint8).tofile(os.path.join(DATA, "chat_mask.bin"))
    np.array(index, dtype=np.int64).reshape(-1, 2).tofile(os.path.join(DATA, "chat_index.bin"))
    print(f"chat examples: {len(index):,} conversations, {human(len(tokens))} tokens")

    with open(os.path.join(DATA, "info.json"), "w", encoding="utf-8") as f:
        json.dump({"vocab_size": sp.get_piece_size(), "pretrain_tokens": n_train, "pretrain_val_tokens": n_val,
                   "chat_conversations": len(index), "chat_tokens": len(tokens), "general_mb": args.general_mb}, f, indent=2)
    print("done: next, python pretrain.py")


if __name__ == "__main__":
    main()
