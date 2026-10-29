# Your own model for Ollama

Fine-tunes **Qwen3-VL-4B-Instruct** (Ollama's `qwen3-vl:4b-instruct`, the Qwen that can also see pictures) into one
model that knows:

* the Pokémon TCG: its rules, every Rulings Compendium ruling, and this project's cards,
* the Pokémon video games: every Pokémon, move and ability, the type chart, and the games themselves,
* Thomas & Friends and The Railway Series: characters, episodes, series and places,
* Unity 6: the Scripting API (core classes and their members, with code), the Manual and the glossary,
* C# and .NET, and Python, neural networks and training, and connecting Python to Unity games,
* Git, GitHub and GitHub Desktop (with Unity projects),
* Game Boy Advance and DS games (formats, extracting their data, rebuilding games like them in Unity),
* and, if you add pictures, how to rebuild a picture as a Unity scene (with `../VisionTools`),

and installs it in Ollama as `pokemon-tcg` (`--name` to call it something else): one model for chat and pictures. Everything here is free: the data
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

It also writes **`qwen_training.zip`: everything for training in one file**, the only thing you upload to Colab
(`train.jsonl`, and the pictures if there are any, below).

* Every `.jsonl` file in `extra/` is added as well. `extra/csharp.jsonl` has about 100 C# and .NET questions with
  answers (types, generics, LINQ, async, threading, performance, Unity, common compiler errors). `extra/python.jsonl`
  has 86 on Python (and setting it up on Windows), NumPy and PyTorch, how neural networks and reinforcement learning
  train (PPO, self-play, MCTS, LLM fine-tuning), Python talking to Unity (sockets, ML-Agents, ONNX), and porting
  Python and trained networks to C#. `extra/git.jsonl` has 68 on Git (commits, branches, merging and conflicts,
  undoing and recovering, rebase, stash, LFS, Unity setup and Smart Merge), GitHub (pull requests, reviews, issues,
  Actions, releases, rules) and GitHub Desktop. `extra/rom_porting.jsonl` has 30 on Game Boy Advance and DS games:
  what can and can't be converted (the model can't read a ROM itself; it reads the output of `../RomTools`), the GBA
  and DS ROM formats, graphics, LZ77 compression, the DS file system and Nitro files, getting the graphics into Unity
  (import settings, Tilemaps, palette swaps), and porting decompiled C to C#. Their code and commands were run and
  checked; `csharp_guide.md`, `python_guide.md`, `git_guide.md` and `rom_porting_guide.md` are the same content to read. Add your own files there in the same format: one `{"messages": [system, user, assistant]}`
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

### Add pictures (optional: for rebuilding pictures as Unity scenes)

Make the training pictures from your sprites first (see `../VisionTools/README.md`), into
`VisionTools/scenes_dataset`, then build as usual:

```
cd ../VisionTools
python make_dataset.py --sprites path/to/your/sprites --count 4000 --out scenes_dataset
cd ../LlmTraining
python build_dataset.py
```

`build_dataset.py` finds `../VisionTools/scenes_dataset` by itself (`--scenes` for another folder, `--no-scenes` to leave
pictures out) and puts the pictures, the held-back pictures it scores before and after training, and the scoring code
into `qwen_training.zip`. 4,000 pictures add roughly 30–60 MB.

## 2. Train it (Google Colab, free, about 1–2 hours for the TCG data, 4–6 hours with everything)

1. Put `qwen_training.zip` at the top of your Google Drive (*My Drive*). The notebook finds it there by itself, which
   is the reliable way for a big file. (Without Drive, it asks you to upload it.)
2. Go to colab.research.google.com → *File → Upload notebook* → `train_colab.ipynb`.
3. *Runtime → Change runtime type → T4 GPU* → *Save*.
4. *Runtime → Run all*. Training saves a checkpoint to your Drive every 250 steps (`SAVE_TO_DRIVE`, on by default): if
   Colab disconnects (free sessions do, especially when the tab is left alone), *Run all* again and it carries on from
   the last checkpoint.
5. At the end it shows a few test answers (one per subject), scores the held-back pictures (if any) against how it did
   before training, and copies **two files** to your Drive: `pokemon-tcg.Q4_K_M.gguf` (the model, about 2.5 GB) and
   `pokemon-tcg.mmproj.gguf` (its image part, about 0.8 GB).

Try `MAX_STEPS = 60` first for a quick run (a few minutes) to check that everything works. With few pictures (under
2,000), `PICTURE_REPEAT = 2` uses each twice.

On your own PC instead: run the notebook with Jupyter in WSL2 (Unsloth needs Linux). It fits a 6 GB RTX 2060; close
the GPU learner and games while it trains.

## 3. Put it in Ollama (your PC)

Needs Ollama 0.30 or newer (it has the chat format for Qwen3-VL). Put both `.gguf` files in one folder, then:

```
python install_to_ollama.py pokemon-tcg.Q4_K_M.gguf
```

It finds the image part next to the model by itself. Then pick **pokemon-tcg** in the Ollama app (or
`ollama run pokemon-tcg`): chat as usual, and add a picture to a message for it to look at. To rebuild a picture as a
Unity scene, use `../VisionTools/ask_model.py` (it uses `pokemon-tcg` by default) and the Unity window in
`../VisionTools`. It uses an 8k context (`--ctx` to change it), which fits your 6 GB card with room to spare.

This route was checked here with Ollama 0.40.13: Unsloth's Qwen3-VL-4B-Instruct GGUF files (the same kind the notebook
saves) installed this way answered both text questions and pictures.

## What to expect

* It learns the rules logic and how rulings are worded and explained well, and a good picture of each subject.
  Exact details it remembers only roughly: a 4B model can't memorise 13,000 cards, 1,000 Pokémon's stats or every
  Unity method, so check exact numbers, card text and API details it gives you. (For exact lookups, the next step is
  retrieval, RAG: the model searches the documents when you ask, instead of remembering them.)
* About 15% of the training is general conversation, and it trains a LoRA add-on (the base model stays frozen), so it
  keeps its normal abilities.
* To improve it: add examples (more cards, your own Q&A, more pictures), rebuild `qwen_training.zip` and train again
  from the base model on everything at once.
* Its text skills are about the same as Qwen3-4B's; it adds sight. (A model from the older Qwen3-4B notebook still
  installs with `install_to_ollama.py`: it notices a text-only model and takes its chat format from `qwen3:4b`.)
