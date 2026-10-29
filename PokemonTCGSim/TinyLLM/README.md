# TinyLLM: your own language model, from scratch

A small GPT you train yourself, from an empty network to something you can chat with in Ollama. Unlike
`../LlmTraining` (which fine-tunes Qwen3-4B, a model someone else pre-trained on trillions of words), everything here is
yours: the tokenizer, the network and every weight in it. It's a real LLM, built the same way as the big ones
(Llama's layout), just small enough for one graphics card.

**What to expect:** a model this size learns to write fluent-looking text in the style of what it read and picks up
facts it saw many times, but it reasons poorly and gets details wrong. That's the trade for training it yourself on
one GPU. For a genuinely useful assistant, the fine-tuned model is still the better tool; this is for learning how LLMs
work, owning every part, and small specialised models.

## Sizes and times

| `--size` | Parameters | Default training text | RTX 2060 (6 GB), roughly |
|---|---|---|---|
| `test` | 0.6M | 12M tokens | Minutes (also on the CPU): checks everything works |
| `tiny` | 17M | 345M tokens | 1–3 hours |
| `small` | 43M | 850M tokens | 6–12 hours |
| `medium` | 91M | 1.8B tokens | 1–2 days (or a few hours on a rented A100/H100) |

The default amount of text is 20 tokens per parameter (the "Chinchilla" rule); `--tokens` changes it.

## 1. Set up (once)

In this folder, on Windows (PowerShell):

```
py -3.12 -m venv .venv
.venv\Scripts\Activate.ps1
pip install torch --index-url https://download.pytorch.org/whl/cu128
pip install -r requirements.txt
python -c "import torch; print(torch.cuda.is_available())"     # should print True
```

## 2. Prepare the data (minutes; the first time also downloads)

```
cd ..\LlmTraining
python build_knowledge.py        # optional: Pokémon games, Thomas & Friends, Unity (see its README)
python build_dataset.py          # the conversations: TCG rulings and cards, plus everything in extra/
cd ..\TinyLLM
python prepare_data.py
```

It reads `../LlmTraining/train.jsonl` (about 35,000 conversations: 19M tokens of text), anything you put in a
`my_text` folder here (`.txt` and `.md` files), and **800 MB of general English** from FineWeb-Edu (educational web
pages, downloaded once into `data/`). The general English matters: your data alone is too little for a model to
learn how to write sentences. `--general-mb 2000` for more, `--general-mb 0` for none.

It trains the tokenizer (8,000 tokens, with the chat markers `<|im_start|>` and `<|im_end|>` as single tokens) and
writes the token files to `data/`.

## 3. Pre-train (the long part)

```
python pretrain.py               # the tiny model; --size small for the bigger one
```

It prints the loss as it goes (lower is better: about 9 at the start, around 3 to 4 when it writes decent text) and
every 250 updates the loss on text it never trains on, with a sample of its writing. It saves `out/pretrain.pt` every
500 updates; **Ctrl+C stops and saves**, `python pretrain.py --resume` carries on. Out of GPU memory: `--batch 8`
(it then needs more updates for the same text, which is handled for you through `--accum`).

## 4. Chat tune (minutes to an hour)

```
python chat_tune.py
```

Trains on the conversations so it answers questions instead of just continuing text; only its answers count in the
loss. Saves `out/chat.pt`.

## 5. Talk to it

```
python chat.py
python chat.py --model out/pretrain.pt --raw      # the pre-trained model: it continues whatever you type
```

## 6. Put it in Ollama

The model is saved in the Hugging Face Llama format, which llama.cpp converts to Ollama's `.gguf`. Keep llama.cpp
outside the project (here `C:\Tools`) and give its converter its own venv, so its requirements can't replace your
training PyTorch:

```
git clone https://github.com/ggml-org/llama.cpp C:\Tools\llama.cpp
py -3.12 -m venv C:\Tools\gguf-venv
C:\Tools\gguf-venv\Scripts\pip install -r C:\Tools\llama.cpp\requirements\requirements-convert_hf_to_gguf.txt
python export.py
C:\Tools\gguf-venv\Scripts\python C:\Tools\llama.cpp\convert_hf_to_gguf.py out\export --outfile out\tinyllm.gguf --outtype f16
ollama create tinyllm -f out\Modelfile
ollama run tinyllm
```

(`python export.py --llama-cpp C:\Tools\llama.cpp --ollama-name tinyllm` does the last three steps in one go, if the
venv you run it in has the converter's requirements.)

## How it works

* `model.py`: the network. A stack of transformer blocks in Llama's layout: RMSNorm, attention with rotary positions,
  a SwiGLU feed-forward part, and the output layer sharing the input embedding's weights. Its layers have the same
  names as Hugging Face's `LlamaForCausalLM`, so the exported model loads there (and in llama.cpp) unchanged.
* `prepare_data.py`: SentencePiece tokenizer (byte fallback, so any text can be written; digits split one by one; code
  whitespace kept) and the token files.
* `pretrain.py`: next-token prediction on random 512-token windows; AdamW, warm-up then cosine decay, gradient clipping,
  16-bit maths (float16 with a loss scaler on RTX 20-series cards, bfloat16 on newer ones).
* `chat_tune.py`: the same, on the conversations, with the loss only on the answers.
* `export.py`: Hugging Face config, weights (safetensors) and tokenizer files; the Ollama Modelfile.

Checked: the exported model gives the same outputs in Hugging Face transformers as here (difference 0.0), and its
tokenizer and chat template match the training format exactly.
