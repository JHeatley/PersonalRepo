# Pokémon TCG model for Ollama

Fine-tunes **Qwen3-4B** (Ollama's `qwen3:4b`) on Pokémon TCG rulings and cards, and installs it in Ollama as
`pokemon-tcg`. Everything here is free: the data comes from this project and the Rulings Compendium, training runs on
Google Colab's free GPU (or your own), and the model runs offline in Ollama.

## 1. Build the training data (your PC, a minute or two)

Needs Python 3 (nothing else to install):

```
cd PokemonTCGSim/LlmTraining
python build_dataset.py
```

It downloads the Rulings Compendium (compendium.pokegym.net, about 9 MB, saved in `compendium/` so it's only
downloaded once), reads the card pool in `Assets/PokemonTCG/Resources/CardData`, and writes `train.jsonl`: about
5,500 rulings and 8,000 card questions, each a short conversation (question → answer with its source).

* Every `.jsonl` file in `extra/` is added as well. `extra/csharp.jsonl` has about 100 C# and .NET questions with
  answers (types, generics, LINQ, async, threading, performance, Unity, common compiler errors). Add your own files
  there in the same format: one `{"messages": [system, user, assistant]}` per line.
* Everything goes into **one model**: every example gets the same system prompt (`SYSTEM` in `build_dataset.py`,
  which `install_to_ollama.py` also gives the installed model). When you add a new subject to `extra/`, add a few
  words about it to `SYSTEM` too.
* To add more later, don't train the trained model again: put the new files in `extra/`, rebuild `train.jsonl`, and
  train from `qwen3:4b` on everything at once. That keeps one balanced model instead of each round wearing away the last.
* `--cards 6000` writes questions about more cards (default 4,000 cards; `--cards 0` for rulings only).
* The Compendium pages and `train.jsonl` aren't kept in git (the rulings are the Compendium team's work).

## 2. Train it (Google Colab, free, about 1–2 hours)

1. Go to colab.research.google.com → *File → Upload notebook* → `train_colab.ipynb`.
2. *Runtime → Change runtime type → T4 GPU* → *Save*.
3. *Runtime → Run all*. Upload `train.jsonl` when it asks.
4. At the end it shows a few test answers and downloads `pokemon-tcg.Q4_K_M.gguf` (about 2.5 GB). If the download
   is unreliable, set `SAVE_TO_DRIVE = True` in the settings cell and it goes to your Google Drive instead.

Try `MAX_STEPS = 60` first for a quick run (a few minutes) to check that everything works.

On your own PC instead: run the notebook with Jupyter in WSL2 (Unsloth needs Linux). It fits a 6 GB RTX 2060; close
the GPU learner and games while it trains.

## 3. Put it in Ollama (your PC)

With Ollama running and `qwen3:4b` downloaded (`ollama pull qwen3:4b`):

```
python install_to_ollama.py pokemon-tcg.Q4_K_M.gguf
```

Then pick **pokemon-tcg** in the Ollama app (or `ollama run pokemon-tcg`). It uses an 8k context (`--ctx` to
change it), which fits your graphics card with room to spare. `qwen3:4b` itself is left as it was.

## What to expect

* It learns the rules logic and how rulings are worded and explained well. Exact card text it remembers only roughly:
  a 4B model can't memorise 13,000 cards, so check card details it gives you.
* About 15% of the training is general conversation, and it trains a LoRA add-on (the base model stays frozen), so it
  keeps its normal abilities.
* To improve it: add examples (more cards, your own Q&A) to `train.jsonl` and train again from `qwen3:4b`.
