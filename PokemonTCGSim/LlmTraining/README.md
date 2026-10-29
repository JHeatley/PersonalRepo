# Your own model for Ollama

Fine-tunes **Qwen3-4B** (Ollama's `qwen3:4b`) into one model that knows:

* the Pokémon TCG: its rules, every Rulings Compendium ruling, and this project's cards,
* the Pokémon video games: every Pokémon, move and ability, the type chart, and the games themselves,
* Thomas & Friends and The Railway Series: characters, episodes, series and places,
* Unity 6: the Scripting API (core classes and their members, with code), the Manual and the glossary,
* C# and .NET, and Python, neural networks and training, and connecting Python to Unity games,
* Git, GitHub and GitHub Desktop (with Unity projects),

and installs it in Ollama as `pokemon-tcg` (`--name` to call it something else). Everything here is free: the data
comes from this project and public sites, training runs on Google Colab's free GPU (or your own), and the model runs
offline in Ollama.

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
  answers (types, generics, LINQ, async, threading, performance, Unity, common compiler errors). `extra/python.jsonl`
  has 86 on Python (and setting it up on Windows), NumPy and PyTorch, how neural networks and reinforcement learning
  train (PPO, self-play, MCTS, LLM fine-tuning), Python talking to Unity (sockets, ML-Agents, ONNX), and porting
  Python and trained networks to C#. `extra/git.jsonl` has 68 on Git (commits, branches, merging and conflicts,
  undoing and recovering, rebase, stash, LFS, Unity setup and Smart Merge), GitHub (pull requests, reviews, issues,
  Actions, releases, rules) and GitHub Desktop. Their code and commands were run and checked; `csharp_guide.md`,
  `python_guide.md` and `git_guide.md` are the same content to read. Add your own files there in the same format: one `{"messages": [system, user, assistant]}`
  per line.
* Everything goes into **one model**: every example gets the same system prompt (`SYSTEM` in `build_dataset.py`,
  which `install_to_ollama.py` also gives the installed model). When you add a new subject to `extra/`, add a few
  words about it to `SYSTEM` too.
* To add more later, don't train the trained model again: put the new files in `extra/`, rebuild `train.jsonl`, and
  train from `qwen3:4b` on everything at once. That keeps one balanced model instead of each round wearing away the last.
* `--cards 6000` writes questions about more cards (default 4,000 cards; `--cards 0` for rulings only).
* `--extra-limit 5000` keeps at most 5,000 examples from each file in `extra/`, for a shorter training run.
* The Compendium pages and `train.jsonl` aren't kept in git (the rulings are the Compendium team's work).

### Add the general knowledge (your PC, about 30–60 minutes the first time)

Before `build_dataset.py`, run:

```
python build_knowledge.py
python build_dataset.py
```

`build_knowledge.py` (also just Python, nothing to install) downloads and writes three more files in `extra/`:

| File | From | Roughly |
|---|---|---|
| `pokemon_games.jsonl` | PokéAPI's data (every Pokémon: types, weaknesses, stats, abilities, evolutions, Pokédex entry, level-up moves; every move and ability; the type chart) and Wikipedia's articles on the games | 5,000 examples |
| `thomas.jsonl` | The Thomas & Friends wiki (ttte.fandom.com: characters, what they're based on, episodes, series, places) and Wikipedia (the show, The Railway Series, the Awdrys, the films) | 7,000 |
| `unity.jsonl` | docs.unity3d.com for Unity 6.2: about 150 core Scripting API classes and their members, the Manual's game-development chapters, the glossary | 10,500 |

* Downloads are saved in `downloads/`, so running it again is quick and an interrupted run carries on where it
  stopped. `--only unity` (or `pokemon`, `thomas`) builds just one; `--small` does a few pages of each as a test;
  `--no-wikipedia` skips Wikipedia if it keeps refusing (it answers "429 Too Many Requests" when busy; the script
  waits and retries, so be patient). To add more pages or classes, edit the lists near the top of each section
  (`POKEMON_GAMES_WIKI`, `THOMAS_WIKI`, `UNITY_CLASSES`, `UNITY_MANUAL_SECTIONS`).
* Like the rulings, the downloaded text is other people's work, so neither `downloads/` nor these three files are
  kept in git.
* With everything, `train.jsonl` has about 37,000 examples, so training takes longer (below). That's the trade-off:
  more knowledge for more training time. `--extra-limit` trims it.

## 2. Train it (Google Colab, free, about 1–2 hours for the TCG data, 3–5 hours with everything)

1. Go to colab.research.google.com → *File → Upload notebook* → `train_colab.ipynb`.
2. *Runtime → Change runtime type → T4 GPU* → *Save*.
3. For a long run, set `SAVE_TO_DRIVE = True` in the settings cell: training saves a checkpoint to your Google Drive
   every 250 steps, and if Colab disconnects (free sessions do, especially when the tab is left alone), *Run all*
   again with the same `train.jsonl` and it carries on from the last checkpoint. The finished file goes to Drive too.
4. *Runtime → Run all*. Upload `train.jsonl` when it asks.
5. At the end it shows a few test answers (one per subject) and downloads `pokemon-tcg.Q4_K_M.gguf` (about 2.5 GB),
   or copies it to `MyDrive` with `SAVE_TO_DRIVE`.

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

* It learns the rules logic and how rulings are worded and explained well, and a good picture of each subject.
  Exact details it remembers only roughly: a 4B model can't memorise 13,000 cards, 1,000 Pokémon's stats or every
  Unity method, so check exact numbers, card text and API details it gives you. (For exact lookups, the next step is
  retrieval, RAG: the model searches the documents when you ask, instead of remembering them.)
* About 15% of the training is general conversation, and it trains a LoRA add-on (the base model stays frozen), so it
  keeps its normal abilities.
* To improve it: add examples (more cards, your own Q&A) to `train.jsonl` and train again from `qwen3:4b`.
