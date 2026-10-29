# VisionTools: teach Qwen to turn pictures into Unity scenes

You give a picture (a game screenshot, a menu, a mock-up) to a local vision model. It answers with a short JSON
description of what's in it, and a Unity editor window builds that as GameObjects.

```
picture ──► Qwen3-VL (local, in Ollama) ──► scene JSON ──► Unity: Tools → Vision → Build Scene From JSON
```

It's the same model as your chat model: `../LlmTraining` trains one Qwen3-VL on everything (the TCG, Unity, C#, Git...)
and these pictures together, and installs it in Ollama as `pokemon-tcg`.

The model never writes Unity files or C# directly. A 4B model is unreliable at that, but it can learn a fixed format
well, and the Unity script turns the format into a scene the same way every time.

**What it can and can't do.**
- **What it gets right:** layout, coloured blocks, text (it reads the words), buttons, and *your* sprites by name.
  These are the things it was trained to recognise.
- **What's approximate:** positions and colours, not pixel-perfect.
- **What it can't do:**
  - recognise art it has never seen (it picks the nearest sprite, or a coloured block);
  - recreate 3D, effects or animation;
  - work out how a game behaves from a picture.

## The files

| File | What it does |
|---|---|
| `make_dataset.py` | Makes training pictures: random scenes drawn from your sprites, each saved with the exact JSON that made it |
| `ask_model.py` | Sends a picture to the model (Ollama or llama.cpp) and saves `picture.scene.json` for Unity; `--preview` draws the answer next to the picture |
| `evaluate.py` | Scores a model on the held-back pictures (before and after training) |
| `score.py` | How a scene is scored (objects found, boxes, sprite names, words, colours) |
| `scenespec.py` | The scene format, the instruction given with each picture, and the renderer |
| `Unity/Editor/SceneFromJson.cs` | The Unity window that builds the scene |
| `test_vision.py` | Tests (no model needed: a fake server answers) |

Needs Python 3.10+ and Pillow: `pip install pillow`.

## 1. Try the model as it is

```
ollama pull qwen3-vl:4b-instruct
python ask_model.py screenshot.png --model qwen3-vl:4b-instruct --preview preview.png
```

Use the `-instruct` tag: Ollama's plain `qwen3-vl:4b` is the *thinking* version, which is slower and isn't the one the
notebook trains. It's about 3.3 GB and fits your RTX 2060.

The answer is held to the scene format while the model writes it (a JSON schema), so even the untrained model gives
Unity something it can build. `preview.png` shows the picture and the model's version side by side.

## 2. Make training pictures from your sprites

```
python make_dataset.py --sprites path/to/sprites --count 4000 --out scenes_dataset
```

- **The sprites folder:** one PNG per sprite (subfolders are fine). The file name is the sprite's name:
  `oak_tree.png` → `"oak_tree"`. RomTools' output and your Unity project's sprite PNGs both work. A sprite sheet
  needs cutting into single PNGs first.
- **How many pictures:** each sprite should appear at least ~50 times. The script prints the count. With 12 sprites,
  1,000 pictures is plenty; with 200 sprites, aim for 5,000 or more.
- **What's in them:** four layouts (a scattered game scene, a menu of buttons, a game screen with a HUD bar, a dialogue
  box), five screen sizes, and random fonts, colours and button styles.
  - `--fonts folder` adds your own fonts; pixel fonts suit retro games.
  - `--demo-sprites` makes a dozen made-up sprites, to try everything before using your own.
- **Your own examples:** you can add real ones, such as screenshots with JSON you've checked, as extra lines in
  `train.jsonl` in the same format. Even 50–100 real ones help it handle real pictures.

## 3. Train it, with everything else (Colab, free)

The pictures train together with all the text, in one model, from one file:

```
cd ../LlmTraining
python build_dataset.py
```

It finds `VisionTools/scenes_dataset` by itself and writes `qwen_training.zip` (the text, the pictures, and the
held-back pictures for checking). Upload that one file and run `train_colab.ipynb`: see **2. Train it** in
`../LlmTraining/README.md`. The notebook scores a few held-back pictures before and after training, so you can see the
change.

## 4. Put it in Ollama and use it

The notebook saves two files, the model and its image part. With both in one folder:

```
python ../LlmTraining/install_to_ollama.py pokemon-tcg.Q4_K_M.gguf
python ask_model.py screenshot.png --sprites path/to/sprites --preview preview.png
```

`ask_model.py` uses `pokemon-tcg` by default. Needs Ollama 0.30 or newer.

Then measure it: `python evaluate.py scenes_dataset`. Compare that with the untrained model's score from
`--model qwen3-vl:4b-instruct`.

**Or llama.cpp** instead of Ollama: download its Windows CUDA build from its GitHub releases page, then

```
llama-server -m pokemon-tcg.Q4_K_M.gguf --mmproj pokemon-tcg.mmproj.gguf --jinja -ngl 99 --port 8080
python ask_model.py screenshot.png --api openai --sprites path/to/sprites --preview preview.png
```

Pass the same `--sprites` folder you trained with every time. The names go into the instruction in the same order as in
training, and the schema only allows those names.

## 5. Build it in Unity

1. Copy `Unity/Editor/SceneFromJson.cs` into an `Editor` folder in your project.
2. Put the sprite PNGs somewhere under `Assets`, imported as Sprites. RomTools' `RomImportSettings.cs` does that for
   anything in a `RomImport` folder.
3. Open **Tools → Vision → Build Scene From JSON**, choose the `.scene.json` (or paste the JSON), choose the sprite
   folder, and click **Build**.

**What you get.** A new object in the Hierarchy containing:
- a camera framed on the picture;
- a Canvas with every object in drawing order: blocks and sprites as Images, text as Text, buttons as real Buttons
  (an EventSystem is added if needed);
- with **Sprites as GameObjects** on, sprites and blocks become SpriteRenderers in the world instead (a game scene),
  and text and buttons go on a Canvas drawn above them.

The build is one Undo step. A sprite name it can't find becomes a see-through magenta box, with a warning in the
Console.

## The scene format

```json
{"background": "#203040",
 "objects": [{"type": "rect",   "box": [0, 780, 1000, 1000], "colour": "#3A5F2B"},
             {"type": "sprite", "box": [150, 110, 250, 380], "name": "oak_tree", "flip": "x"},
             {"type": "text",   "box": [340, 830, 660, 900], "text": "PRESS START", "colour": "#FFFFFF"},
             {"type": "button", "box": [400, 600, 600, 680], "text": "Options", "colour": "#2050A0", "textColour": "#FFFFFF"}]}
```

- **Drawing order:** objects are drawn back to front, in list order.
- **The box:** `[left, top, right, bottom]`, from 0 to 1000 across and down the picture, whatever its size. Qwen3-VL
  points at things in images the same way, so training starts from something it already knows.
- **Width and height:** `ask_model.py` adds the picture's `width` and `height` in pixels for Unity.

## What was checked

- **Python:** `python test_vision.py` passes. It covers the format, scoring and dataset making, and a fake Ollama and
  llama-server check that the right requests are sent.
- **Unity:** `SceneFromJson.cs` compiles against Unity 6.2's own engine and UI libraries, with no errors or warnings. It
  hasn't been run inside Unity here.
- **Training format:** the training examples were run through Qwen3-VL's real processor and chat template. They come to
  at most about 1,100 tokens, against a 2,048 limit. A small randomly initialised Qwen3-VL ran a training step on them
  (forward, loss, backward) and generated text.
- **Notebook** (`../LlmTraining/train_colab.ipynb`): every cell ran here with a tiny Qwen3-VL and stand-ins for the
  GPU-only parts (Unsloth, Google Drive). The real TRL 1.15 trainer, the version Unsloth installs, trained on batches
  mixing text and pictures, saved a checkpoint and resumed from it.
- **Ollama:** checked with Ollama 0.40.13. Unsloth's own Qwen3-VL-4B-Instruct GGUF files (the same kind of export the
  notebook makes) went in with `install_to_ollama.py`, then answered text questions and, through `ask_model.py`, rebuilt
  pictures. Untrained, it already placed boxes well (about 70–87% overlap) but didn't know the sprite names, which is
  what the training teaches.
- **Not checked here:** the real GPU training run itself, which needs Colab's GPU.
