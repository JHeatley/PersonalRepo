# Python, neural networks and Python with Unity: questions and answers

The readable version of `extra/python.jsonl` (the same content, which `build_dataset.py` adds to the training data). The code examples were run (Python 3.13, PyTorch 2.14, Gymnasium 1.4, .NET 8): the PPO script solves CartPole, and the C# network port and C# Adam match PyTorch to about 1e-7.

# Python and Windows setup

## How do I install Python on Windows and check it works?

1. Download the installer from python.org (Downloads → Windows). Pick a version your libraries support: for PyTorch and most machine-learning tools, a recent 3.11, 3.12 or 3.13 is safe; ML-Agents needs the exact version its docs name (it has been 3.10.x).
2. Run it and tick **"Add python.exe to PATH"** on the first screen. Leave "Install the py launcher" ticked.
3. Open a new PowerShell or Command Prompt and check:

```
py --version          # the launcher: picks the newest installed Python
python --version      # works if Python is on PATH
py -0                 # lists every installed version
py -3.12 --version    # run a specific version
```

If `python` opens the Microsoft Store instead, turn off the "python.exe" and "python3.exe" App execution aliases in Settings → Apps → Advanced app settings → App execution aliases, or just use `py`.

On Windows the command is `python` or `py`, not `python3` as on Linux and macOS.

## What is a virtual environment (venv) and how do I use one on Windows?

A virtual environment is a folder with its own Python and its own installed packages, so each project can have the versions it needs without breaking other projects.

```
cd E:\IndieStudio\MyProject
py -3.12 -m venv .venv            # create it (once)
.venv\Scripts\Activate.ps1        # PowerShell: activate it
.venv\Scripts\activate.bat        # Command Prompt: activate it
pip install numpy torch           # now installs go into .venv
deactivate                        # leave it
```

When it's active, your prompt shows `(.venv)` and `python` means the venv's Python.

If PowerShell says running scripts is disabled, allow scripts you created yourself once with:

```
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

You can also skip activating and call the venv's Python directly: `.venv\Scripts\python.exe train.py`. Don't commit `.venv` to git; commit a `requirements.txt` instead.

## How do pip and requirements.txt work?

`pip` installs packages from PyPI (the Python Package Index).

```
pip install numpy                 # newest version
pip install "numpy>=2.0,<3"       # a version range
pip install -r requirements.txt   # everything a project lists
pip freeze > requirements.txt     # write down exactly what's installed
pip list                          # what's installed
pip uninstall numpy
pip install --upgrade pip
```

A `requirements.txt` is just one package per line:

```
numpy>=2.0
torch
matplotlib==3.9.2
```

The safest way to run pip is `python -m pip install ...` (or `py -m pip ...`): that guarantees the package goes into the same Python you'll run, which avoids most "I installed it but it says ModuleNotFoundError" problems.

## I installed a package but Python says ModuleNotFoundError. Why?

Almost always, pip installed it into a different Python than the one running your script. Common causes on Windows:

- You have several Pythons (python.org, Microsoft Store, Anaconda, a venv) and `pip` belongs to another one.
- The venv isn't activated in this terminal (or your editor uses a different interpreter).
- The package's install name differs from its import name: `pip install opencv-python` → `import cv2`, `pip install pillow` → `import PIL`, `pip install scikit-learn` → `import sklearn`, `pip install pyyaml` → `import yaml`.

Check which Python is running and install with that exact one:

```python
import sys
print(sys.executable)   # e.g. E:\MyProject\.venv\Scripts\python.exe
```

```
E:\MyProject\.venv\Scripts\python.exe -m pip install numpy
```

In VS Code, pick the same interpreter with "Python: Select Interpreter". Also make sure your own file isn't named like the package (a file called `numpy.py` or `torch.py` hides the real one).

## How do I install PyTorch with CUDA so it uses my NVIDIA GPU on Windows?

The default `pip install torch` on Windows is the **CPU-only** build. For the GPU you need the CUDA build from PyTorch's own index:

```
pip uninstall -y torch torchvision torchaudio
pip install torch torchvision --index-url https://download.pytorch.org/whl/cu128
```

(pytorch.org's "Get Started" page shows the current command; `cu128` means CUDA 12.8. If there's no build for your Python version, use one listed there, e.g. `cu126`.) You don't need to install the CUDA Toolkit separately: the wheel brings its own CUDA libraries. You do need a recent NVIDIA driver.

Check it:

```python
import torch
print(torch.__version__)            # ends in +cu128 for the CUDA build, +cpu for the CPU one
print(torch.cuda.is_available())    # True
print(torch.cuda.get_device_name(0))
```

`nvidia-smi` in a terminal shows the driver, the GPU's memory use and which processes are using it.

## What can I train on an RTX 2060 with 6 GB of VRAM?

An RTX 2060 (Turing, 6 GB) is fine for learning and for small and medium models:

- Small networks (MLPs, small CNNs, game-playing policy networks like a card-game AI): easily, and much faster than the CPU once batches are big enough (hundreds or thousands of samples per step).
- Fine-tuning image models (ResNet-50 and similar) with moderate batch sizes.
- Running LLMs up to about 4B parameters quantised (e.g. a 4-bit 4B model in Ollama); 7–8B models spill into system RAM and get slow.
- Fine-tuning LLMs on it is tight: QLoRA on a 1–4B model with short sequences can fit; bigger needs a cloud GPU (Colab's free T4 has 15 GB).

Tips for 6 GB:

- Use mixed precision: `torch.autocast("cuda", dtype=torch.float16)` with a `GradScaler`. Turing supports float16 but **not bfloat16** efficiently.
- If you run out of memory ("CUDA out of memory"), lower the batch size, use gradient accumulation, shorten sequences, or use gradient checkpointing.
- Close other GPU users (games, browsers with hardware acceleration) and check with `nvidia-smi`.

## How do I run a Python script on Windows, and pass it arguments?

From a terminal in the script's folder:

```
python train.py
python train.py --epochs 20 --lr 0.001 --device cuda
py -3.12 train.py      # a specific Python version
```

Read the arguments with `argparse`:

```python
import argparse

ap = argparse.ArgumentParser(description="Train the model")
ap.add_argument("--epochs", type=int, default=10)
ap.add_argument("--lr", type=float, default=1e-3)
ap.add_argument("--device", default="cuda", choices=["cuda", "cpu"])
ap.add_argument("--resume", action="store_true", help="carry on from the last checkpoint")
args = ap.parse_args()
print(args.epochs, args.lr, args.device, args.resume)
```

`python train.py --help` then prints the options automatically. Double-clicking a `.py` file also runs it, but the window closes when it finishes or crashes, so you can't read errors: use a terminal.

## How do I convert a Python script written on Linux or macOS so it runs on Windows?

Most Python runs unchanged, but these are the usual problems:

1. **Paths.** Don't build paths with `"/"` string joins or hard-code `/home/...`. Use `pathlib`, which works everywhere:
   ```python
   from pathlib import Path
   data = Path(__file__).parent / "data" / "train.csv"
   ```
   In Windows path strings, use raw strings or forward slashes: `r"E:\Models\net.pt"` or `"E:/Models/net.pt"` (`"E:\new"` would contain a newline `\n`).
2. **The `if __name__ == "__main__":` guard.** Windows starts worker processes by re-importing your script (`spawn`), so `multiprocessing` and PyTorch `DataLoader(num_workers>0)` crash or loop without it. Put the top-level code in `main()` and call it under the guard.
3. **Shell commands.** `os.system("ls")`, `rm`, `cp`, `export`, `&&` chains and `.sh` scripts don't exist on Windows. Use Python instead: `shutil.copy`, `shutil.rmtree`, `Path.glob`, `os.environ["X"] = "1"`, `subprocess.run([...])` with a list of arguments.
4. **Linux-only packages and features**: `fcntl`, `signal.SIGKILL`, `os.fork`, `resource`, `uvloop`; some packages (older `bitsandbytes`, `triton`, `deepspeed`) have limited Windows support. Look for a Windows wheel, a pure-Python alternative, or use WSL2.
5. **File encodings.** Always pass `encoding="utf-8"` to `open()`; Windows' default may not be UTF-8.
6. **`python3`** → `python` or `py`; and `#!/usr/bin/env python3` lines are ignored (harmless).
7. **Line endings and case**: Windows file names are case-insensitive; files written with `"\n"` are fine, but open CSVs with `newline=""`.

If a project really depends on Linux, WSL2 (Windows Subsystem for Linux) runs it unchanged and can still use your NVIDIA GPU.

## Why do I need `if __name__ == "__main__":` in Python?

Every `.py` file can be run directly or imported as a module. `__name__` is `"__main__"` only when the file is the one being run, so code under the guard runs then and not when something imports the file:

```python
def train(epochs):
    print(f"training for {epochs} epochs")

def main():
    train(10)

if __name__ == "__main__":
    main()
```

It matters most on Windows: `multiprocessing`, `concurrent.futures.ProcessPoolExecutor` and PyTorch's `DataLoader(num_workers > 0)` start new processes that **import your script again**. Without the guard, each one would start training (and more workers) again, giving a `RuntimeError` about the "bootstrapping phase" or an endless loop.

## How do I turn a Python script into a Windows .exe?

PyInstaller bundles the Python interpreter, your code and its packages:

```
pip install pyinstaller
pyinstaller --onefile train.py              # dist\train.exe, one file
pyinstaller --onefile --noconsole app.py    # a GUI app with no console window
pyinstaller --onedir train.py               # a folder (starts faster)
```

Things to know:

- Build on Windows to get a Windows exe (it can't cross-compile).
- Data files must be added (`--add-data "models;models"` on Windows) and found at runtime through `sys._MEIPASS` when frozen.
- PyTorch/CUDA programs become very large (gigabytes). For a tool that talks to a Unity game, it's often simpler to ship a venv with a `requirements.txt` and a `.bat` file that runs it.
- Antivirus software sometimes flags one-file PyInstaller exes; `--onedir` is less likely to be flagged.

Alternatives: Nuitka (compiles to C, faster), or `py2exe`.

## What are the basic Python types and how does Python compare to C#?

Python is dynamically typed: variables don't have declared types, values do.

| Python | C# |
|---|---|
| `int` (unlimited size) | `int`/`long` (fixed size, can overflow) |
| `float` (64-bit) | `double` |
| `str` (immutable, Unicode) | `string` |
| `bool` (`True`/`False`) | `bool` (`true`/`false`) |
| `None` | `null` |
| `list` `[1, 2, 3]` | `List<T>` |
| `tuple` `(1, 2)` | value tuple `(1, 2)` |
| `dict` `{"a": 1}` | `Dictionary<K, V>` |
| `set` `{1, 2}` | `HashSet<T>` |

```python
hp = 120                 # int
name = "Pikachu"         # str
weak = ["Fighting"]      # list
stats = {"hp": 60, "atk": 55}
is_ex = False
nothing = None
print(type(hp), isinstance(name, str))
```

Blocks are defined by indentation (4 spaces), not braces; there are no semicolons; comments start with `#`.

## How do lists, dicts and comprehensions work in Python?

```python
cards = ["Pikachu", "Charmander", "Squirtle"]
cards.append("Bulbasaur")
first, last = cards[0], cards[-1]      # negative indexes count from the end
middle = cards[1:3]                    # slicing: items 1 and 2
for i, card in enumerate(cards):
    print(i, card)

hp = {"Pikachu": 60, "Charmander": 70}
hp["Squirtle"] = 50
print(hp.get("Mew", 0))                # 0 if missing, no KeyError
for name, value in hp.items():
    print(name, value)

# Comprehensions: build a list/dict/set in one expression
squares = [x * x for x in range(10)]
strong = [n for n, h in hp.items() if h >= 60]
by_len = {n: len(n) for n in cards}
types = {t for t in ["Fire", "Water", "Fire"]}   # a set: {"Fire", "Water"}
```

Comprehensions are the Pythonic equivalent of LINQ `Select`/`Where` and are faster than an append loop.

## How do functions, default arguments, *args and **kwargs work in Python?

```python
def damage(base, multiplier=1.0, *, weakness=False):
    # `*` makes everything after it keyword-only
    total = base * multiplier
    return total * 2 if weakness else total

damage(30)                       # 30.0
damage(30, 1.5)                  # 45.0
damage(30, weakness=True)        # 60.0

def log(*args, **kwargs):
    # args: a tuple of extra positional arguments; kwargs: a dict of extra keyword arguments
    print("args:", args, "kwargs:", kwargs)

log(1, 2, level="info")          # args: (1, 2) kwargs: {'level': 'info'}

settings = {"multiplier": 2.0, "weakness": True}
damage(10, **settings)           # unpack a dict into keyword arguments
```

**The mutable default trap**: default values are created once, when the function is defined:

```python
def add_card(card, deck=[]):     # BUG: every call shares the same list
    deck.append(card)
    return deck

def add_card(card, deck=None):   # correct
    if deck is None:
        deck = []
    deck.append(card)
    return deck
```

## How do classes and dataclasses work in Python?

```python
class Pokemon:
    def __init__(self, name, hp):
        self.name = name          # instance attributes are created by assignment
        self.hp = hp
        self.damage = 0

    @property
    def remaining(self):         # read like an attribute: mon.remaining
        return self.hp - self.damage

    def hit(self, amount):
        self.damage = min(self.hp, self.damage + amount)

    def __repr__(self):          # what print() and the debugger show
        return f"Pokemon({self.name!r}, {self.remaining}/{self.hp})"

class ExPokemon(Pokemon):         # inheritance
    prizes = 2                    # class attribute shared by all instances

    def __init__(self, name, hp):
        super().__init__(name, hp)

mon = ExPokemon("Mewtwo ex", 180)
mon.hit(50)
print(mon, mon.prizes)            # Pokemon('Mewtwo ex', 130/180) 2
```

For classes that mainly hold data, `dataclasses` writes `__init__`, `__repr__` and `__eq__` for you:

```python
from dataclasses import dataclass, field

@dataclass
class Card:
    name: str
    hp: int = 0
    attacks: list[str] = field(default_factory=list)   # never use a bare [] default

c = Card("Pikachu", 60, ["Thunder Shock"])
print(c)                          # Card(name='Pikachu', hp=60, attacks=['Thunder Shock'])
```

`self` is explicit in Python (C#'s `this` is implicit), and there is no `private`: a leading underscore (`_cache`) is the convention for "internal".

## How do exceptions work in Python?

```python
def load_deck(path):
    try:
        with open(path, encoding="utf-8") as f:
            return f.read().splitlines()
    except FileNotFoundError:
        print(f"no deck at {path}")
        return []
    except (PermissionError, IsADirectoryError) as e:
        raise RuntimeError(f"can't read {path}") from e   # wrap, keeping the cause
    finally:
        print("done trying")                                # always runs

class InvalidDeck(Exception):
    pass

def check(deck):
    if len(deck) != 60:
        raise InvalidDeck(f"a deck needs 60 cards, this has {len(deck)}")
```

Python style is "easier to ask forgiveness than permission": try the operation and catch the specific error, rather than checking everything first. Catch specific exceptions; a bare `except:` also catches Ctrl+C (`KeyboardInterrupt`) and hides bugs. Re-raise with a plain `raise` inside `except` to keep the original traceback.

## What does `with` do in Python?

`with` uses a **context manager**: an object that sets something up and is guaranteed to clean it up when the block ends, even if an exception happens. It's Python's equivalent of C#'s `using`.

```python
with open("log.txt", "w", encoding="utf-8") as f:
    f.write("hello\n")
# the file is closed here, even after an error

import torch
with torch.no_grad():            # turn off gradient tracking for this block
    pass

import threading
lock = threading.Lock()
with lock:                       # acquired here, released at the end
    pass
```

You can write your own with `contextlib`:

```python
from contextlib import contextmanager
import time

@contextmanager
def timer(label):
    start = time.perf_counter()
    try:
        yield
    finally:
        print(f"{label}: {time.perf_counter() - start:.3f}s")

with timer("training step"):
    sum(range(1_000_000))
```

## What are generators and `yield` in Python?

A function with `yield` returns a generator: it produces values one at a time, pausing between them, instead of building a whole list in memory. It's the same idea as C#'s `yield return` / `IEnumerable<T>`.

```python
def batches(items, size):
    for i in range(0, len(items), size):
        yield items[i:i + size]

for batch in batches(list(range(10)), 4):
    print(batch)          # [0, 1, 2, 3]  [4, 5, 6, 7]  [8, 9]

# Generator expression: like a list comprehension, but lazy
total = sum(x * x for x in range(1_000_000))   # no million-item list is built
```

Generators can only be iterated once. They're ideal for streaming large files (`for line in f:` is itself lazy), producing training batches, or infinite sequences.

## What are decorators in Python?

A decorator is a function that takes a function and returns a replacement, usually wrapping it with extra behaviour. `@name` above a `def` applies it.

```python
import functools
import time

def timed(fn):
    @functools.wraps(fn)                 # keeps the original name and docstring
    def wrapper(*args, **kwargs):
        start = time.perf_counter()
        result = fn(*args, **kwargs)
        print(f"{fn.__name__} took {time.perf_counter() - start:.4f}s")
        return result
    return wrapper

@timed
def train_step(n):
    return sum(i * i for i in range(n))

train_step(100_000)
```

Built-in examples: `@property`, `@staticmethod`, `@classmethod`, `@dataclass`, `@functools.lru_cache` (memoisation), `@torch.no_grad()`. They're similar in spirit to C# attributes, but a decorator actually changes the function at runtime.

## How do type hints work in Python?

Type hints document the expected types. Python doesn't enforce them at runtime, but editors (VS Code/Pylance) and checkers (`mypy`, `pyright`) use them to catch mistakes, much like C#'s compiler would.

```python
from typing import Optional, Callable

def best_card(cards: list[str], score: Callable[[str], float]) -> Optional[str]:
    return max(cards, key=score) if cards else None

hp_by_name: dict[str, int] = {"Pikachu": 60}
point: tuple[float, float] = (1.0, 2.0)
maybe: int | None = None          # Python 3.10+ union syntax
```

```
pip install mypy
mypy train.py
```

Use them on function signatures and class fields at least; they make code (especially code an AI writes for you) much easier to check and refactor.

## How do I read and write files, JSON and CSV in Python?

```python
import csv
import json
from pathlib import Path

folder = Path("data")
folder.mkdir(exist_ok=True)

# Text
(folder / "notes.txt").write_text("hello\n", encoding="utf-8")
text = (folder / "notes.txt").read_text(encoding="utf-8")

# JSON
deck = {"name": "Lightning", "cards": [{"name": "Pikachu", "count": 4}]}
with open(folder / "deck.json", "w", encoding="utf-8") as f:
    json.dump(deck, f, indent=2, ensure_ascii=False)
with open(folder / "deck.json", encoding="utf-8") as f:
    loaded = json.load(f)

# CSV (newline="" stops blank lines between rows on Windows)
with open(folder / "results.csv", "w", newline="", encoding="utf-8") as f:
    w = csv.writer(f)
    w.writerow(["episode", "reward"])
    w.writerows([[1, 0.5], [2, 0.75]])
with open(folder / "results.csv", newline="", encoding="utf-8") as f:
    rows = list(csv.DictReader(f))      # [{'episode': '1', 'reward': '0.5'}, ...] (strings!)

# Binary
(folder / "weights.bin").write_bytes(b"\x00\x01")
```

`json` maps dict↔object, list↔array, str, int/float, bool and None↔null. CSV values always come back as strings: convert with `int()`/`float()`.

## How do I read and write binary data (like weights or a network protocol) in Python?

Use `struct` for fixed layouts and NumPy for big arrays. Pick the byte order explicitly: `<` is little-endian, which is what C#'s `BinaryWriter`/`BinaryReader` use on Windows.

```python
import struct
import numpy as np

# Pack: int32, float64, then a length-prefixed UTF-8 string
name = "policy".encode("utf-8")
blob = struct.pack("<id", 3, 0.25) + struct.pack("<i", len(name)) + name

version, lr = struct.unpack_from("<id", blob, 0)
(n,) = struct.unpack_from("<i", blob, 12)
text = blob[16:16 + n].decode("utf-8")
print(version, lr, text)        # 3 0.25 policy

# Arrays: float32 is C#'s float
weights = np.random.rand(4, 3).astype(np.float32)
raw = weights.tobytes()                       # 48 bytes, row-major
back = np.frombuffer(raw, dtype="<f4").reshape(4, 3)
assert np.array_equal(weights, back)
```

Format letters: `b`/`B` 8-bit, `h` int16, `i` int32 (C# `int`), `q` int64 (`long`), `f` float32 (`float`), `d` float64 (`double`), `?` bool. C#'s `BinaryWriter.Write(string)` uses a 7-bit-encoded length prefix rather than an int32, so for cross-language protocols it's simpler to write the length as an `int` yourself, on both sides.

## How do modules, packages and imports work in Python?

A **module** is a `.py` file; a **package** is a folder of modules (usually with an `__init__.py`).

```
mygame_ai/
    train.py
    model/
        __init__.py
        network.py      # defines class PolicyNet
        utils.py
```

```python
# in train.py
from model.network import PolicyNet
from model import utils
import numpy as np                 # an installed package, with a short alias
```

Python looks for imports in the running script's folder, then the installed packages. Rules of thumb:

- Run scripts from the project root (`python train.py`) so imports resolve consistently, or run a module with `python -m model.utils`.
- Avoid circular imports (a imports b, b imports a); move shared code into a third module.
- Never name your files after packages you use (`torch.py`, `random.py`, `socket.py`): they shadow the real ones.

## What's the difference between threading, multiprocessing and asyncio in Python?

- **The GIL**: in standard CPython, only one thread runs Python bytecode at a time. So threads don't speed up pure-Python number crunching (NumPy and PyTorch release the GIL inside their heavy operations, so those do run in parallel).
- **`threading`**: good for waiting on I/O (sockets, files, web requests) while other work continues, e.g. one thread per connected game client.
- **`multiprocessing`** / `ProcessPoolExecutor`: separate processes, each with its own GIL, so CPU-heavy Python really runs on several cores. Data passed between processes is pickled (copied), which has a cost. Needs the `if __name__ == "__main__":` guard on Windows.
- **`asyncio`**: one thread juggling many I/O tasks with `async`/`await`, like C#'s async. Great for many network connections; useless for CPU work.

```python
from concurrent.futures import ThreadPoolExecutor, ProcessPoolExecutor

def simulate(seed):
    import random
    rng = random.Random(seed)
    return sum(rng.random() for _ in range(200_000))

if __name__ == "__main__":
    with ProcessPoolExecutor() as pool:          # CPU-bound: processes
        results = list(pool.map(simulate, range(8)))
    print(len(results))
```

Python 3.13+ also has an optional free-threaded build without the GIL, but most libraries still assume the normal build.

## How do I make slow Python code faster?

1. **Measure first**: `python -m cProfile -s cumtime train.py`, or time a block with `time.perf_counter()`. Optimise only what shows up.
2. **Vectorise with NumPy/PyTorch**: replace Python loops over numbers with array operations, which run in C:
   ```python
   import numpy as np
   x = np.random.rand(1_000_000)
   slow = sum(v * 2 for v in x)      # Python loop
   fast = (x * 2).sum()              # one C loop, ~100x faster
   ```
3. **Use the right data structure**: `set`/`dict` lookups are O(1), `x in list` is O(n); `collections.deque` for queues; `collections.Counter` for counting.
4. **Avoid repeated work**: cache with `functools.lru_cache`, hoist work out of loops, build strings with `"".join(parts)`.
5. **Parallelise** CPU-bound work with `multiprocessing`; batch GPU work so each call does a lot.
6. **Compile hot spots**: Numba (`@njit`) for numeric loops, Cython, or move the hot part to C#/C++.

For game AI the biggest win is usually batching: one network call on 1,000 positions is far faster than 1,000 calls on one position.

## How do I debug a Python program?

1. **Read the traceback from the bottom up.** The last line is the exception type and message; the lines above show the call chain, with your file and line number nearest the bottom.
2. **Print or log** the values involved: `print(f"{x=} {x.shape=}")` (the `=` form prints the expression and its value).
3. **Use a debugger**: put `breakpoint()` on a line and run the script; you get the `pdb` prompt:
   - `p expr` print a value, `n` next line, `s` step into, `c` continue, `l` list code, `w` where (stack), `q` quit.
   - In VS Code, set breakpoints by clicking the margin and press F5 (Python Debugger extension), which also lets you inspect variables and tensors visually.
4. **Post-mortem**: `python -m pdb train.py` stops at the crash so you can inspect the state there.
5. **Shrink the problem**: reproduce it with the smallest input (one batch, one game, a fixed random seed) so it fails in seconds.
6. **Add asserts** for things that must be true: `assert logits.shape == (batch, n_actions), logits.shape`.

Common errors: `IndentationError` (mixed tabs/spaces), `NameError` (typo or undefined variable), `TypeError` (wrong type or argument count), `KeyError` (missing dict key; use `.get`), `IndexError`, `AttributeError: 'NoneType' object has no attribute ...` (a function returned `None`).

## How do I write tests for Python code with pytest?

```
pip install pytest
```

Put tests in files named `test_*.py`, in functions named `test_*`, using plain `assert`:

```python
# test_damage.py
import pytest
from damage import apply_weakness

def test_weakness_doubles():
    assert apply_weakness(30, weak=True) == 60

def test_no_weakness():
    assert apply_weakness(30, weak=False) == 30

def test_negative_rejected():
    with pytest.raises(ValueError):
        apply_weakness(-1, weak=True)

@pytest.mark.parametrize("base,expected", [(0, 0), (10, 20), (120, 240)])
def test_many(base, expected):
    assert apply_weakness(base, weak=True) == expected
```

```
pytest            # finds and runs every test
pytest -x -q      # stop at the first failure, quiet output
pytest -k weak    # only tests with "weak" in the name
```

For machine-learning code, test the deterministic parts: shapes, that a loss decreases on a tiny dataset, that saving and loading gives identical outputs, and that the data encoding matches what the game sends.

## How do f-strings and string formatting work in Python?

```python
name, hp, win_rate = "Pikachu", 60, 0.53214
print(f"{name} has {hp} HP")                 # Pikachu has 60 HP
print(f"win rate {win_rate:.1%}")            # win rate 53.2%
print(f"loss {0.000123:.2e}")                # loss 1.23e-04
print(f"{hp:5d}|{name:<10}|{name:>10}|")     # padding and alignment
print(f"{1234567:,}")                        # 1,234,567
print(f"{hp=}")                              # hp=60 (handy for debugging)
print(f"{{literal braces}}")                 # {literal braces}

lines = ["a", "b"]
print("\n".join(lines))
print("Pikachu".lower(), " x ".strip(), "a,b".split(","), "fire" in "fire type")
```

f-strings are the Python equivalent of C#'s interpolated strings (`$"{name} has {hp} HP"`), with similar format specifiers.

## How do I use logging instead of print in Python?

```python
import logging

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(name)s: %(message)s",
    handlers=[logging.FileHandler("train.log", encoding="utf-8"), logging.StreamHandler()],
)
log = logging.getLogger("trainer")

log.debug("hidden at INFO level")
log.info("epoch %d loss %.4f", 3, 0.1234)
log.warning("learning rate is very high")
try:
    1 / 0
except ZeroDivisionError:
    log.exception("step failed")    # logs the message plus the traceback
```

Advantages over `print`: levels you can turn up or down without editing code, timestamps, writing to a file and the console at once, and tracebacks. For training metrics over time, also consider TensorBoard (`torch.utils.tensorboard.SummaryWriter`) to see loss curves.

# NumPy and PyTorch

## What is NumPy and what are the basics I need to know?

NumPy provides fast n-dimensional arrays (`ndarray`) and maths on whole arrays at once. It's the base of almost all scientific and machine-learning Python.

```python
import numpy as np

a = np.array([[1.0, 2.0, 3.0], [4.0, 5.0, 6.0]], dtype=np.float32)
print(a.shape, a.dtype, a.ndim)     # (2, 3) float32 2
print(a[0, 1], a[:, 0], a[1])       # 2.0  [1. 4.]  [4. 5. 6.]

z = np.zeros((3, 4)); o = np.ones(5); r = np.arange(0, 10, 2)  # [0 2 4 6 8]
x = np.random.default_rng(0).normal(size=(100, 8))              # seeded random numbers

b = a * 2 + 1                        # element-wise, no loop
m = a @ a.T                          # matrix multiply: (2,3) @ (3,2) -> (2,2)
print(a.sum(), a.mean(axis=0), a.max(axis=1))   # whole array, per column, per row
mask = a > 2
print(a[mask])                       # [3. 4. 5. 6.]  boolean indexing
flat = a.reshape(-1)                 # -1: work out that dimension
```

Rules of thumb: avoid Python loops over elements; mind dtypes (`float32` is what GPUs and C#'s `float` use; NumPy defaults to `float64`); and many operations return views that share memory, so use `.copy()` when you need an independent array.

## What is broadcasting in NumPy and PyTorch?

Broadcasting lets operations combine arrays of different shapes by virtually stretching dimensions of size 1 (or missing leading dimensions), without copying data.

Shapes are compared from the right; two dimensions are compatible if they're equal or one of them is 1.

```python
import numpy as np

x = np.ones((4, 3))           # 4 samples, 3 features
mean = x.mean(axis=0)         # shape (3,)
centred = x - mean            # (4,3) - (3,) -> (4,3): subtract per feature

col = np.arange(4).reshape(4, 1)   # (4,1)
row = np.arange(3)                 # (3,)
grid = col * 10 + row              # (4,1) and (3,) -> (4,3)
print(grid)

bad = np.ones((4, 3))
# bad + np.ones(4)  -> ValueError: operands could not be broadcast together with shapes (4,3) (4,)
fixed = bad + np.ones(4)[:, None]  # make it (4,1) first
```

Broadcasting bugs are silent when shapes happen to be compatible: e.g. `(N,) - (N,1)` gives an `(N,N)` matrix. When a loss looks wrong, print the shapes.

## What is a tensor in PyTorch and how do I use one?

A tensor is PyTorch's array: like a NumPy array, but it can live on the GPU and can record operations for automatic differentiation.

```python
import torch

device = "cuda" if torch.cuda.is_available() else "cpu"
x = torch.tensor([[1.0, 2.0], [3.0, 4.0]])
y = torch.randn(2, 2, device=device)          # created on the GPU if there is one
x = x.to(device)                               # move to the same device
z = x @ y + 1                                  # ops need both tensors on one device
print(z.shape, z.dtype, z.device)

print(z.sum().item())                          # .item(): a Python number from a 1-element tensor
arr = z.detach().cpu().numpy()                 # back to NumPy (must be on the CPU, without grad)
t = torch.from_numpy(arr)                      # NumPy -> tensor (shares memory)

batch = torch.zeros(32, 10)
print(batch.view(32, 2, 5).shape, batch.unsqueeze(0).shape, batch[:, :3].shape)
```

Common errors: "Expected all tensors to be on the same device" (move everything with `.to(device)`), dtype mismatches (`float32` vs `float64` or `long` for class indices), and shape mismatches.

## How does autograd (automatic differentiation) work in PyTorch?

When a tensor has `requires_grad=True`, PyTorch records every operation on it. Calling `.backward()` on a scalar result walks that record backwards (backpropagation) and stores the gradient d(result)/d(tensor) in each tensor's `.grad`.

```python
import torch

w = torch.tensor(3.0, requires_grad=True)
x = torch.tensor(2.0)
loss = (w * x - 10) ** 2        # (6 - 10)^2 = 16
loss.backward()
print(w.grad)                   # d/dw = 2*(w*x - 10)*x = 2*(-4)*2 = -16

# Gradient descent by hand: step against the gradient
with torch.no_grad():
    w -= 0.05 * w.grad          # w moves towards 5, where w*x = 10
    w.grad.zero_()              # gradients ADD UP across backward() calls, so reset them
```

Model parameters (`nn.Parameter`) have `requires_grad=True` automatically. Use `torch.no_grad()` (or `torch.inference_mode()`) when you only run the model, to save memory and time, and `.detach()` to cut a tensor out of the graph (for example targets computed from the network itself).

## How do I define a neural network in PyTorch with nn.Module?

Subclass `nn.Module`, create layers in `__init__`, and describe the computation in `forward`:

```python
import torch
import torch.nn as nn
import torch.nn.functional as F

class PolicyValueNet(nn.Module):
    def __init__(self, n_inputs, n_actions, hidden=128):
        super().__init__()
        self.body = nn.Sequential(
            nn.Linear(n_inputs, hidden), nn.ReLU(),
            nn.Linear(hidden, hidden), nn.ReLU(),
        )
        self.policy = nn.Linear(hidden, n_actions)   # one score (logit) per action
        self.value = nn.Linear(hidden, 1)            # how good the position is

    def forward(self, x):
        h = self.body(x)
        return self.policy(h), torch.tanh(self.value(h)).squeeze(-1)

net = PolicyValueNet(n_inputs=20, n_actions=6)
logits, value = net(torch.randn(8, 20))             # a batch of 8 positions
print(logits.shape, value.shape)                     # torch.Size([8, 6]) torch.Size([8])
probs = F.softmax(logits, dim=-1)
print(sum(p.numel() for p in net.parameters()), "parameters")
```

Call the module (`net(x)`), not `net.forward(x)`, so hooks run. Everything assigned as an attribute (layers, `nn.Parameter`s) is registered automatically, so `net.parameters()`, `net.to(device)` and `net.state_dict()` include it.

## What does a basic PyTorch training loop look like?

```python
import torch
import torch.nn as nn

torch.manual_seed(0)
device = "cuda" if torch.cuda.is_available() else "cpu"

# Toy data: learn y = 3x + 1 with noise
x = torch.rand(1000, 1)
y = 3 * x + 1 + 0.05 * torch.randn(1000, 1)

model = nn.Sequential(nn.Linear(1, 16), nn.ReLU(), nn.Linear(16, 1)).to(device)
opt = torch.optim.Adam(model.parameters(), lr=1e-2)
loss_fn = nn.MSELoss()

for epoch in range(200):
    perm = torch.randperm(len(x))                 # shuffle every epoch
    for i in range(0, len(x), 64):                # minibatches of 64
        idx = perm[i:i + 64]
        xb, yb = x[idx].to(device), y[idx].to(device)
        pred = model(xb)                          # 1. forward
        loss = loss_fn(pred, yb)                  # 2. how wrong
        opt.zero_grad()                           # 3. clear old gradients
        loss.backward()                           # 4. backpropagate
        opt.step()                                # 5. update the weights
    if epoch % 50 == 0:
        print(epoch, loss.item())

with torch.no_grad():
    print(model(torch.tensor([[0.5]], device=device)).item())   # about 2.5
```

The five steps (forward, loss, zero_grad, backward, step) are the same for every model, from this toy to a game-playing policy or an LLM. Real projects add a validation set, checkpoints, logging, learning-rate schedules and gradient clipping.

## What are epochs, batches and the learning rate?

- **Batch (minibatch)**: the group of examples used for one weight update. Bigger batches give smoother gradient estimates and use the GPU better, but need more memory; typical sizes are 32–4096 for small networks.
- **Step (iteration)**: one update: forward, loss, backward, optimizer step on one batch.
- **Epoch**: one pass over the whole training set. With 10,000 examples and batch 100, an epoch is 100 steps.
- **Learning rate (lr)**: how far each step moves the weights along the gradient. Too high: the loss jumps around or becomes NaN. Too low: training crawls or gets stuck. Adam's usual starting point is `1e-3` (`3e-4` is common for reinforcement learning, around `2e-4` for LoRA fine-tuning of LLMs).

Schedules change the learning rate during training, usually a short warm-up and then a decay:

```python
import torch
model = torch.nn.Linear(4, 2)
opt = torch.optim.AdamW(model.parameters(), lr=1e-3, weight_decay=0.01)
sched = torch.optim.lr_scheduler.CosineAnnealingLR(opt, T_max=1000)
for step in range(1000):
    # ... forward, loss.backward(), opt.step() ...
    opt.step()
    sched.step()
print(sched.get_last_lr())
```

If you double the batch size, you can often raise the learning rate a little; change one thing at a time and watch the loss curve.

## Which loss function should I use?

It depends on what the network predicts:

- **A number** (regression: a value estimate, a price, a score): `nn.MSELoss` (mean squared error), or `nn.SmoothL1Loss`/Huber when there are outliers.
- **One class out of several** (classification, choosing an action to imitate): `nn.CrossEntropyLoss`. It takes **raw logits** (no softmax first) and integer class indices:
  ```python
  import torch, torch.nn as nn
  logits = torch.randn(4, 3)               # 4 samples, 3 classes
  target = torch.tensor([0, 2, 1, 2])      # dtype long
  loss = nn.CrossEntropyLoss()(logits, target)
  ```
- **Yes/no or several independent yes/no labels**: `nn.BCEWithLogitsLoss` (again on raw logits; it's more stable than sigmoid + `BCELoss`).
- **Matching a probability distribution** (e.g. a look-ahead search's improved move probabilities, or a teacher network in distillation): cross-entropy against soft targets, `-(target_probs * F.log_softmax(logits, -1)).sum(-1).mean()`, or `nn.KLDivLoss`.
- **Reinforcement learning (PPO)**: a custom combination: the clipped policy loss, plus a value loss (MSE), minus an entropy bonus.

The loss must be a single number (scalar) for `.backward()`, so average or sum it over the batch.

## What's the difference between SGD and Adam?

Both use the gradient to update the weights:

- **SGD**: `w -= lr * grad`, optionally with **momentum** (keep moving in the recent average direction). Simple, uses little memory, often generalises very well, but needs careful learning-rate tuning.
- **Adam**: keeps a running average of each weight's gradient (momentum) and of its squared gradient, and scales each weight's step by them, so every parameter gets its own effective step size. Works well out of the box at `lr=1e-3`; the default choice for most projects, including reinforcement learning and transformers.
- **AdamW**: Adam with weight decay done correctly (decoupled). Prefer it over `Adam(weight_decay=...)`.

```python
import torch
model = torch.nn.Linear(10, 2)
sgd = torch.optim.SGD(model.parameters(), lr=0.01, momentum=0.9)
adam = torch.optim.AdamW(model.parameters(), lr=1e-3, weight_decay=0.01)
```

Adam stores two extra numbers per weight, so its optimizer state is twice the size of the model; save it in checkpoints (`opt.state_dict()`) so training can resume smoothly.

## What are overfitting and underfitting, and how do I fix them?

Split your data into a **training set** (to learn from) and a **validation set** (never trained on) and watch both losses:

- **Underfitting**: both losses are high. The model is too small, training is too short, the learning rate is wrong, or the inputs don't contain the information needed. Fix: bigger model, train longer, better features, tune the learning rate.
- **Overfitting**: training loss keeps falling but validation loss stops improving and rises. The model is memorising the training examples. Fix:
  - more (or more varied) data, or data augmentation,
  - **early stopping**: keep the checkpoint with the best validation loss,
  - regularisation: weight decay (AdamW), dropout (`nn.Dropout(0.1)`),
  - a smaller model or fewer epochs.

In reinforcement learning the equivalent check is evaluation games against fixed opponents not used in training. In LLM fine-tuning, overfitting shows up as the model repeating training answers word for word and getting worse at everything else, which is why fine-tunes usually run only 1–3 epochs and mix in general data.

## How do I save and load a PyTorch model and resume training?

Save the `state_dict` (a dict of parameter tensors), not the whole model object. For resuming, also save the optimizer and progress:

```python
import torch
import torch.nn as nn

model = nn.Linear(4, 2)
opt = torch.optim.Adam(model.parameters())
epoch = 5

torch.save({"model": model.state_dict(), "opt": opt.state_dict(), "epoch": epoch}, "checkpoint.pt")

# Later (same model class and sizes):
ckpt = torch.load("checkpoint.pt", map_location="cpu")   # map_location: load GPU checkpoints on any machine
model2 = nn.Linear(4, 2)
model2.load_state_dict(ckpt["model"])
opt2 = torch.optim.Adam(model2.parameters())
opt2.load_state_dict(ckpt["opt"])
start_epoch = ckpt["epoch"] + 1

model2.eval()    # for using it: turns off dropout and batch-norm updates
```

Tips: write to a temporary file and rename it, so a crash mid-save doesn't destroy your only checkpoint; keep the last few checkpoints; and save the settings (layer sizes, input format version) alongside the weights so you can rebuild the right model. `torch.load` only loads plain tensors and containers by default in recent versions (`weights_only=True`), which is also safer: never load checkpoints from untrusted sources with `weights_only=False`.

## What do model.train(), model.eval() and torch.no_grad() do?

- `model.train()` (the default) and `model.eval()` switch layers whose behaviour differs between training and use:
  - `nn.Dropout` randomly zeroes activations in train mode and does nothing in eval mode.
  - `nn.BatchNorm` uses the batch's statistics and updates its running averages in train mode, and uses the stored averages in eval mode.
  They do **not** turn off gradients.
- `torch.no_grad()` (or the slightly faster `torch.inference_mode()`) stops PyTorch recording operations for backpropagation, which saves memory and time.

So, to use a model (evaluation, playing games, serving moves to a game):

```python
import torch
model = torch.nn.Sequential(torch.nn.Linear(8, 8), torch.nn.Dropout(0.2), torch.nn.Linear(8, 2))
model.eval()
with torch.inference_mode():
    out = model(torch.randn(1, 8))
model.train()    # switch back before training again
```

Forgetting `eval()` makes outputs random (dropout) or wrong (batch norm); forgetting `no_grad()` slowly fills memory if you keep the outputs.

## How do I use the GPU efficiently and what is mixed precision?

- **Move the model and each batch to the GPU** once (`.to(device)`), and keep data there; copying between CPU and GPU every step is slow. `.item()`, `.cpu()` and printing tensors force the CPU to wait for the GPU, so do them rarely (e.g. log every 100 steps).
- **Big batches**: the GPU is only fast when each operation has a lot of work. Batch many samples (or many games' positions) into one forward pass.
- **Mixed precision**: run most maths in 16-bit floats, which is faster and uses half the memory, while keeping the weights in 32-bit:

```python
import torch
device = "cuda" if torch.cuda.is_available() else "cpu"
model = torch.nn.Linear(512, 512).to(device)
opt = torch.optim.AdamW(model.parameters(), lr=1e-3)
scaler = torch.amp.GradScaler(device, enabled=(device == "cuda"))
x = torch.randn(256, 512, device=device)

with torch.autocast(device_type=device, dtype=torch.float16, enabled=(device == "cuda")):
    loss = model(x).pow(2).mean()
opt.zero_grad()
scaler.scale(loss).backward()   # scale up so small float16 gradients don't round to zero
scaler.step(opt)
scaler.update()
```

On RTX 30/40/50 cards and newer, `dtype=torch.bfloat16` works without a scaler; on an RTX 20 series (Turing) or a T4, use float16 with the scaler.

## How do I use Dataset and DataLoader in PyTorch?

A `Dataset` says how to get one example; a `DataLoader` batches, shuffles and (optionally) loads in parallel worker processes.

```python
import torch
from torch.utils.data import Dataset, DataLoader

class GameRecords(Dataset):
    def __init__(self, features, actions):
        self.features = torch.as_tensor(features, dtype=torch.float32)
        self.actions = torch.as_tensor(actions, dtype=torch.long)

    def __len__(self):
        return len(self.actions)

    def __getitem__(self, i):
        return self.features[i], self.actions[i]

def main():
    data = GameRecords(torch.randn(1000, 20), torch.randint(0, 6, (1000,)))
    loader = DataLoader(data, batch_size=64, shuffle=True, num_workers=0, drop_last=True)
    for x, y in loader:
        print(x.shape, y.shape)    # torch.Size([64, 20]) torch.Size([64])
        break

if __name__ == "__main__":    # required on Windows when num_workers > 0
    main()
```

For data that's already in tensors in memory (like a round of self-play games), plain slicing with `torch.randperm` is simpler and faster than a DataLoader. Use `num_workers > 0` when loading is slow (images from disk, heavy preprocessing); on Windows that needs the `__main__` guard.

## Why is my loss NaN or not going down?

Work through these in order:

1. **Check the data**: NaN/inf in the inputs or targets (`torch.isfinite(x).all()`), wrong labels, unnormalised features (values in the thousands next to values near 0: scale inputs to roughly -1..1).
2. **Learning rate too high**: the most common cause of NaN. Try 10× lower.
3. **Numerically unsafe maths**: `log(0)`, division by zero, `exp` of large numbers. Use `F.log_softmax` and `F.cross_entropy`/`BCEWithLogitsLoss` instead of `log(softmax(x))`, add a small epsilon (`x / (std + 1e-8)`), clamp.
4. **Exploding gradients**: clip them: `torch.nn.utils.clip_grad_norm_(model.parameters(), 0.5)` between `backward()` and `step()`.
5. **Forgot `opt.zero_grad()`** (gradients accumulate), or **forgot `opt.step()`**.
6. **Shape or broadcasting bugs**: `(N,1)` vs `(N,)` targets in MSE silently make an `(N,N)` loss.
7. **Can it overfit one tiny batch?** Train on 10 examples for a few hundred steps: the loss should go near zero. If it can't, the bug is in the model or loss code, not the data size.
8. **Mixed precision**: use the `GradScaler` with float16.

`torch.autograd.set_detect_anomaly(True)` (slow; for debugging only) reports the operation that first produced NaN.

## How do I make a PyTorch experiment reproducible?

Seed every random number generator and keep the setup identical:

```python
import random
import numpy as np
import torch

def seed_everything(seed=42):
    random.seed(seed)
    np.random.seed(seed)
    torch.manual_seed(seed)              # also seeds CUDA
    torch.backends.cudnn.benchmark = False

seed_everything(42)
print(torch.rand(1).item())              # the same number every run
```

For bit-exact results on the GPU you may also need `torch.use_deterministic_algorithms(True)` (some operations become slower or unsupported). Results still differ between CPU and GPU, between GPU models and between library versions, because floating-point operations are done in different orders. That's why parity tests (e.g. Python versus a C# port of the same network) compare with a tolerance like `1e-5`, not exact equality.

## What is gradient clipping and why is it used?

Gradient clipping limits how big an update can be. If the total size (norm) of all gradients is larger than a maximum, they're scaled down so the norm equals that maximum; the direction is unchanged.

```python
import torch
model = torch.nn.Linear(10, 1)
opt = torch.optim.Adam(model.parameters(), lr=1e-3)
loss = model(torch.randn(32, 10)).pow(2).mean()
opt.zero_grad()
loss.backward()
total = torch.nn.utils.clip_grad_norm_(model.parameters(), max_norm=0.5)   # returns the norm before clipping
opt.step()
print(float(total))
```

It prevents one unusual batch (a huge reward, an outlier) from wrecking the network, and is standard in reinforcement learning (PPO usually uses `max_norm=0.5`), RNNs and transformers. Logging the pre-clip norm is a useful health check: if it's always far above the limit, the learning rate may be too high.

# Neural networks, reinforcement learning and LLMs

## What is a neural network, in simple terms?

A neural network is a function with millions of adjustable numbers (weights) that is tuned, by example, to turn inputs into the outputs you want.

- A **layer** takes a list of numbers, multiplies it by a weight matrix, adds a bias, and applies a simple non-linear function (an **activation**, like ReLU: `max(0, x)`). Stacking layers lets the network represent complicated relationships.
- **Training**: show it an example, measure how wrong the output is with a **loss** function, compute how each weight affected that error (**backpropagation**, which is the chain rule from calculus applied automatically), and nudge every weight slightly in the direction that reduces the error (**gradient descent**). Repeat millions of times.
- What it learns is only as good as its data and its objective: it finds patterns that reduce the loss on what it's shown.

```python
import numpy as np

# One layer by hand: 3 inputs -> 2 outputs
x = np.array([0.5, -1.0, 2.0])
W = np.array([[0.1, 0.2, 0.3], [-0.4, 0.5, 0.6]])
b = np.array([0.0, 0.1])
h = np.maximum(0, W @ x + b)    # ReLU(Wx + b)
print(h)                          # [0.45 0.6 ]
```

A game AI's network takes an encoding of the game state and outputs a score for each possible move (the **policy**) and an estimate of who's winning (the **value**).

## How does backpropagation work?

Backpropagation computes, for every weight, how much the loss would change if that weight changed slightly (the **gradient**). It applies the chain rule backwards through the network:

1. **Forward pass**: compute each layer's output from the input and keep the intermediate values.
2. Compute the loss.
3. **Backward pass**: start with d(loss)/d(output) and go layer by layer towards the input. For a layer `y = W x`, the gradient for its weights is `d(loss)/dy · xᵀ`, and the gradient passed on to the previous layer is `Wᵀ · d(loss)/dy`. Activation functions multiply by their own derivative (ReLU: 1 where the input was positive, 0 elsewhere).

```python
import numpy as np

# A 1-layer model y = w*x, loss = (y - t)^2, done by hand
x, t, w = 2.0, 10.0, 3.0
y = w * x                 # forward: 6
loss = (y - t) ** 2       # 16
dloss_dy = 2 * (y - t)    # -8
dloss_dw = dloss_dy * x   # chain rule: -16
w -= 0.05 * dloss_dw      # gradient descent step: w becomes 3.8
print(loss, dloss_dw, w)
```

Frameworks like PyTorch do this automatically (autograd). Writing it by hand, as when porting a network and its training to C#, means writing a `Backward` for each layer type and checking it against PyTorch's gradients numerically.

## What are activation functions, and which should I use?

Activation functions add non-linearity between layers; without them, any stack of linear layers collapses into one linear layer.

- **ReLU** `max(0, x)`: the default for hidden layers. Fast and works well.
- **Leaky ReLU / GELU / SiLU (Swish)**: smoother variants; GELU and SiLU are standard in transformers and modern networks.
- **tanh**: output in -1..1. Used for value heads that predict a result from -1 (loss) to +1 (win).
- **sigmoid**: output in 0..1, for a probability of a yes/no outcome (usually folded into `BCEWithLogitsLoss` instead).
- **softmax**: turns a vector of scores (logits) into probabilities that sum to 1; used for choosing one action or class out of several (usually folded into `cross_entropy`).

```python
import torch
import torch.nn.functional as F
x = torch.tensor([-2.0, 0.0, 3.0])
print(F.relu(x), torch.tanh(x), torch.sigmoid(x), F.softmax(x, dim=0))
```

For a policy over legal moves, set the logits of illegal moves to a very negative number (masking) before the softmax, so they get probability 0.

## What are embeddings?

An embedding is a learned vector of numbers that represents a discrete thing: a word, a token, a card, a Pokémon, a player. Instead of a one-hot vector with one slot per possible item, each item gets a row in a table (`nn.Embedding`), and training moves similar items to similar vectors.

```python
import torch
import torch.nn as nn

n_cards, dim = 5000, 48
card_table = nn.Embedding(n_cards, dim, padding_idx=0)   # row 0 = "no card", stays zero
hand = torch.tensor([[12, 873, 0, 0]])                    # card ids, padded with 0
vectors = card_table(hand)                                 # shape (1, 4, 48)
summary = vectors.sum(dim=1)                               # one vector for the whole hand
print(vectors.shape, summary.shape)
```

They let a network handle thousands of different items with few parameters and generalise: a newly added card can start from its printed stats and learn its own row. LLMs start by turning every token into an embedding in exactly the same way.

## What are CNNs, RNNs and transformers, briefly?

- **MLP (multi-layer perceptron)**: stacked fully connected layers. Good for fixed-size feature vectors (a game state encoded as numbers).
- **CNN (convolutional network)**: slides small learned filters over a grid, so it finds the same pattern anywhere. The standard for images and board-like grids (Go and chess engines use them).
- **RNN / LSTM / GRU**: process a sequence one step at a time with a memory state. Mostly replaced by transformers, but still used for small, streaming problems.
- **Transformer**: every element of a sequence looks at every other one through **attention** (each element asks a question, the "query", and weights the others by how well their "keys" match, then mixes their "values"). Handles long-range relationships and sets of items, and parallelises well on GPUs. All modern LLMs are transformers; they also work well for sets of things in games (cards in hand, units on a map).

```python
import torch
import torch.nn as nn
layer = nn.TransformerEncoderLayer(d_model=64, nhead=4, batch_first=True)
cards = torch.randn(2, 10, 64)          # 2 positions, 10 cards each, 64 features
print(layer(cards).shape)               # torch.Size([2, 10, 64]): each card now "knows" about the others
```

## What is reinforcement learning?

Reinforcement learning (RL) trains an **agent** to choose **actions** in an **environment** to maximise total **reward**, by trial and error rather than from labelled examples.

- **State/observation**: what the agent sees (the board, its hand, health bars, ray-cast distances).
- **Action**: what it does (play a card, move left, attack).
- **Reward**: a number from the environment: +1 for a win, -1 for a loss, small shaping rewards for progress (taking a prize, reaching a checkpoint).
- **Policy**: the agent's strategy: a network that maps a state to action probabilities.
- **Value function**: a network's prediction of the future reward from a state, used to judge whether an action turned out better or worse than expected.
- **Episode**: one game or attempt, from start to end.

The loop: play episodes with the current policy, record (state, action, reward), compute which actions led to better-than-expected results, and adjust the policy to make those more likely. Popular algorithms are PPO (policy-gradient, stable, used by ML-Agents and many game AIs), DQN (value-based, for small discrete action sets) and SAC (continuous control). Games like card games add **self-play**: the agent learns by playing against copies of itself.

## How does PPO (Proximal Policy Optimization) work?

PPO is the most widely used policy-gradient RL algorithm because it's stable and simple. Each round:

1. **Collect** experience with the current policy: many steps of (state, action, the action's log-probability, reward, value estimate).
2. **Compute advantages**: how much better each action turned out than the value network expected, usually with GAE (Generalized Advantage Estimation, below). Normalise them per batch.
3. **Update** for a few epochs over that data in minibatches, maximising the **clipped objective**:
   - `ratio = π_new(a|s) / π_old(a|s)` (computed as `exp(new_logp - old_logp)`),
   - `policy_loss = -mean(min(ratio · A, clip(ratio, 1-ε, 1+ε) · A))` with ε ≈ 0.2,
   - plus a value loss `c_v · (V(s) - return)²` and minus an entropy bonus `c_e · entropy` that keeps it exploring.

The clip is the key idea: once an action's probability has moved more than ±20% from the policy that collected the data, the gradient for it stops, so one batch can't change the policy too much.

```python
import torch

def ppo_loss(new_logp, old_logp, adv, values, returns, entropy, clip=0.2, vf=0.5, ent=0.01):
    adv = (adv - adv.mean()) / (adv.std() + 1e-8)
    ratio = torch.exp(new_logp - old_logp)
    policy = -torch.min(ratio * adv, torch.clamp(ratio, 1 - clip, 1 + clip) * adv).mean()
    value = (values - returns).pow(2).mean()
    return policy + vf * value - ent * entropy.mean()

n = 8
loss = ppo_loss(torch.randn(n, requires_grad=True), torch.randn(n), torch.randn(n),
                torch.randn(n, requires_grad=True), torch.randn(n), torch.rand(n))
loss.backward()
print(float(loss))
```

Typical settings: learning rate 3e-4, 3–10 epochs per round, minibatches of a few hundred to a few thousand, gradient clipping at 0.5, γ (discount) 0.99, λ (GAE) 0.95.

## What are returns, discounting and GAE (Generalized Advantage Estimation)?

- **Return**: the total reward from a step to the end of the episode. With a **discount** γ (e.g. 0.99), later rewards count a little less: `G_t = r_t + γ·r_{t+1} + γ²·r_{t+2} + ...`. Discounting favours sooner rewards and reduces variance.
- **Advantage**: `A_t = (what actually happened) - V(s_t)` (what the value network expected). Positive means "better than expected, do it more".
- **GAE** blends one-step estimates (low variance, biased by the value network) and full returns (unbiased, noisy) with λ (e.g. 0.95):
  `δ_t = r_t + γ·V(s_{t+1}) - V(s_t)`, then `A_t = δ_t + γλ·A_{t+1}`, computed backwards from the end of the episode.

```python
import numpy as np

def gae(rewards, values, last_value, gamma=0.99, lam=0.95):
    # rewards, values: per step of one episode; last_value: V of the state after the last step (0 if it ended)
    adv = np.zeros(len(rewards))
    next_value, running = last_value, 0.0
    for t in reversed(range(len(rewards))):
        delta = rewards[t] + gamma * next_value - values[t]
        running = delta + gamma * lam * running
        adv[t] = running
        next_value = values[t]
    returns = adv + np.asarray(values)       # targets for the value network
    return adv, returns

adv, ret = gae([0, 0, 1.0], [0.1, 0.3, 0.6], last_value=0.0)
print(adv.round(3), ret.round(3))
```

In a two-player game where only the final result is rewarded (+1/-1), every step's return is just the discounted final result from that player's point of view.

## What is self-play and how do game AIs like AlphaZero learn?

**Self-play**: the agent plays against itself (or past versions of itself), so it always has an opponent at its own level, and every game produces training data for both sides.

Common ingredients:

- **An opponent pool**: play not only the latest network but also saved older versions, so it doesn't forget how to beat old strategies or chase its own tail ("rock-paper-scissors" cycles).
- **Exploiters / a league**: train separate networks whose only goal is to beat the main one, then train the main one against them, to find and fix its weaknesses (used by AlphaStar).
- **A fixed yardstick**: evaluate regularly against fixed opponents (a scripted bot, a "champion" checkpoint) to measure real progress, because win rate against yourself always hovers around 50%.
- **Search + learning (AlphaZero)**: during play, a look-ahead search (Monte Carlo Tree Search) guided by the network finds better moves than the network alone. The network is then trained to imitate the search's move probabilities (policy target) and predict the game result (value target). Better network → better search → better targets, in a loop.
- **Hidden information** (card games, poker): the AI must not look at the opponent's hand or the deck order while searching. Approaches sample possible hidden states that fit what's known (determinisation) or use algorithms built for imperfect information (CFR, ReBeL).

The training loop alternates generating games (CPU-heavy simulation) with learning on the GPU, which is why simulator and learner are often separate programs talking over a socket.

## What is imitation learning and distillation?

- **Imitation learning (behaviour cloning)**: train the policy with supervised learning to copy an expert's actions: recorded human games, a scripted bot, or a search algorithm. The loss is cross-entropy between the network's action probabilities and the expert's choice. It's a fast way to give an RL agent a sensible starting policy before self-play.
- **Distillation**: train a network (the student) to match another source's output distribution (the teacher): a bigger network, an ensemble, or a look-ahead search. The target is the teacher's full probability distribution, not just its top choice, which carries more information ("this move is good, that one almost as good").

```python
import torch
import torch.nn.functional as F

student_logits = torch.randn(4, 5, requires_grad=True)
teacher_probs = F.softmax(torch.randn(4, 5), dim=-1)          # e.g. visit counts from a search, normalised
loss = -(teacher_probs * F.log_softmax(student_logits, dim=-1)).sum(-1).mean()
loss.backward()
print(float(loss))
```

Mixing them with RL is common: decisions where the search produced an improved target learn by distillation, and the rest learn by PPO, with a weight balancing the two.

## How do I design rewards for reinforcement learning in a game?

- **Start with the true goal**: +1 for a win, -1 for a loss (0 for a draw). It's sparse but can't be gamed.
- **Add small shaping rewards** only to speed up learning: e.g. +0.1 per prize card taken, +0.01 for moving towards the goal. Keep them much smaller than the final reward, or the agent will optimise them instead of winning (a racing agent circling to collect checkpoint rewards forever is the classic example).
- **Potential-based shaping** (reward = `γ·Φ(next state) - Φ(state)` for some measure of progress Φ) provably doesn't change the best policy.
- **Penalise what you really don't want** (illegal actions, wasting time) with small negative rewards, or make the action impossible with masking instead.
- **Time limits**: end episodes after a maximum number of steps so the agent can't stall, and tell the learner whether an episode ended by losing or by timeout.
- **Watch the agent play**, not just the reward curve. Rising reward with silly behaviour means the reward is being exploited.

## How do I know if my reinforcement learning training is working?

Track these over time (TensorBoard, CSV logs or the game's own training screen):

- **Win rate against fixed opponents** (a scripted bot, older checkpoints): the most honest measure. Self-play win rate stays near 50% and tells you little.
- **Episode reward and length**: should trend up (or length down for "win fast" tasks).
- **Policy entropy**: should decrease slowly. Collapsing to zero early means it stopped exploring; staying at the maximum means it isn't learning.
- **Value loss**: should fall then stabilise; **explained variance** of the value head near 1 is good.
- **PPO clip fraction** (share of samples whose ratio got clipped): around 0.1–0.3 is healthy. Much higher: learning rate or epochs too high.
- **Approximate KL** between old and new policy per update: small (around 0.01–0.02).
- **Gradient norm** before clipping, and any NaNs.

Also: fix a random seed and compare runs; change one setting at a time; save checkpoints often; and keep an "Elo" or rating history against a pool of past versions to catch regressions.

## How does an LLM (large language model) work, and what are tokens?

An LLM is a transformer trained to predict the next **token** given all previous tokens. Tokens are pieces of text (whole common words, word pieces, punctuation, code symbols), each mapped to an integer id; a model has a vocabulary of roughly 32,000–250,000 of them.

Generation repeats: run the model on the text so far → get a probability for every possible next token → pick one (most likely, or sampled with a **temperature**) → append it → repeat.

- **Pre-training**: next-token prediction on trillions of tokens of web text, books and code. This is where knowledge comes from.
- **Instruction tuning / chat fine-tuning**: further training on conversations so it answers questions and follows instructions.
- **Context window**: the maximum number of tokens it can look at at once (prompt plus answer). Longer contexts need more memory.
- **Parameters**: the weights (4B = 4 billion). More parameters store more knowledge and reasoning ability but need more memory: roughly 2 bytes per parameter at 16-bit, about 0.5–0.6 bytes at 4-bit quantisation (so a 4B model is about 2.5 GB in q4).

It has no database or internet inside it: answers come from patterns in its weights, which is why it can be confidently wrong ("hallucinate") about details it saw rarely.

## What are fine-tuning, LoRA and QLoRA?

**Fine-tuning** continues training an existing model on your own examples (question → answer conversations) so it learns your subject, style or format.

- **Full fine-tuning** updates every weight: needs a lot of GPU memory (weights + gradients + Adam state ≈ 16 bytes per parameter).
- **LoRA (Low-Rank Adaptation)** freezes the original weights and learns small extra matrices for selected layers: for a weight `W` (d×d), it learns `A` (d×r) and `B` (r×d) with a small rank r (8–64), and uses `W + B·A`. Only about 0.1–1% of the parameters train, so it's fast and needs little memory, and the result is a small "adapter" that can be merged into the model.
- **QLoRA** also stores the frozen base model in 4-bit, cutting memory further, so a 4B–8B model can be fine-tuned on a single 15–24 GB GPU (like Colab's free T4).

Key settings: rank r (16 is a good default), learning rate (~2e-4), epochs (1–3), sequence length, and training only on the answers ("train on responses only") rather than the questions. Afterwards it's merged and exported (e.g. GGUF for Ollama/llama.cpp).

What it's good at: teaching a domain's facts, terms and answer style, a format, or a persona. What it's bad at: precise facts it saw only once (use retrieval, RAG, for exact lookups). Too much training on narrow data causes **catastrophic forgetting** of general skills, so mix in some general conversation data.

## What is RAG (retrieval-augmented generation) and when should I use it instead of fine-tuning?

RAG gives the model relevant documents at question time: the question is used to search your own data (documents, card lists, rulings, API docs, code), and the best matches are pasted into the prompt, so the model answers from them.

1. Split your documents into chunks (a few hundred words each).
2. Turn each chunk into an **embedding** vector with an embedding model, and store them in a vector index (FAISS, Chroma, SQLite with a vector extension, or even a NumPy array).
3. For a question, embed it, find the most similar chunks (cosine similarity), and put them in the prompt with "Answer using these documents".

```python
import numpy as np

def top_k(query_vec, doc_vecs, k=3):
    q = query_vec / np.linalg.norm(query_vec)
    d = doc_vecs / np.linalg.norm(doc_vecs, axis=1, keepdims=True)
    scores = d @ q
    return np.argsort(-scores)[:k]

docs = np.random.default_rng(0).normal(size=(100, 384))
print(top_k(docs[42] + 0.01, docs))   # document 42 first
```

Use **RAG** for exact, changing or large factual data (every card's text, every ruling, a whole codebase or API reference): it's always up to date, cites sources and doesn't hallucinate details as much. Use **fine-tuning** for skills, style, formats and broad domain understanding. The best assistants often do both: a fine-tuned model that's also given retrieved documents. Ollama can provide the embedding model too (e.g. `ollama pull nomic-embed-text`).

# Complete training examples

## Show me a complete PPO training script in PyTorch.

Here's a minimal but complete PPO agent that learns CartPole (balancing a pole on a cart) with Gymnasium. It shows every part: collecting experience, GAE advantages, and clipped minibatch updates:

```python
"""PPO on CartPole: a complete, minimal reinforcement-learning training script.

    pip install torch gymnasium
    python ppo_cartpole.py
"""
import gymnasium as gym
import numpy as np
import torch
import torch.nn as nn
from torch.distributions import Categorical

STEPS_PER_ROUND, ROUNDS, EPOCHS, MINIBATCH = 2048, 30, 10, 64
GAMMA, LAM, CLIP, LR, VALUE_COEF, ENTROPY_COEF = 0.99, 0.95, 0.2, 3e-4, 0.5, 0.01


class ActorCritic(nn.Module):
    def __init__(self, n_obs, n_actions, hidden=64):
        super().__init__()
        self.policy = nn.Sequential(nn.Linear(n_obs, hidden), nn.Tanh(), nn.Linear(hidden, hidden), nn.Tanh(),
                                    nn.Linear(hidden, n_actions))
        self.value = nn.Sequential(nn.Linear(n_obs, hidden), nn.Tanh(), nn.Linear(hidden, hidden), nn.Tanh(),
                                   nn.Linear(hidden, 1))

    def act(self, obs):
        dist = Categorical(logits=self.policy(obs))
        action = dist.sample()
        return action, dist.log_prob(action), self.value(obs).squeeze(-1)


def main():
    torch.manual_seed(0)
    env = gym.make("CartPole-v1")
    model = ActorCritic(env.observation_space.shape[0], env.action_space.n)
    opt = torch.optim.Adam(model.parameters(), lr=LR)
    obs, _ = env.reset(seed=0)
    episode_return, finished_returns = 0.0, []

    for round_ in range(ROUNDS):
        # 1. Collect experience with the current policy
        buf = {k: [] for k in ("obs", "action", "logp", "reward", "done", "value")}
        for _ in range(STEPS_PER_ROUND):
            o = torch.as_tensor(obs, dtype=torch.float32)
            with torch.no_grad():
                action, logp, value = model.act(o)
            obs, reward, terminated, truncated, _ = env.step(action.item())
            for k, v in (("obs", o), ("action", action), ("logp", logp), ("reward", reward),
                         ("done", terminated), ("value", value)):
                buf[k].append(v)
            episode_return += reward
            if terminated or truncated:
                finished_returns.append(episode_return)
                episode_return = 0.0
                obs, _ = env.reset()

        # 2. Advantages with GAE, computed backwards
        with torch.no_grad():
            next_value = model.value(torch.as_tensor(obs, dtype=torch.float32)).item()
        values = torch.stack(buf["value"])
        adv = torch.zeros(STEPS_PER_ROUND)
        running = 0.0
        for t in reversed(range(STEPS_PER_ROUND)):
            not_done = 0.0 if buf["done"][t] else 1.0
            nv = next_value if t == STEPS_PER_ROUND - 1 else values[t + 1].item()
            delta = buf["reward"][t] + GAMMA * nv * not_done - values[t].item()
            running = delta + GAMMA * LAM * not_done * running
            adv[t] = running
        returns = adv + values
        obs_b, act_b, logp_b = torch.stack(buf["obs"]), torch.stack(buf["action"]), torch.stack(buf["logp"])

        # 3. Several epochs of minibatch updates with the clipped objective
        for _ in range(EPOCHS):
            for idx in torch.randperm(STEPS_PER_ROUND).split(MINIBATCH):
                dist = Categorical(logits=model.policy(obs_b[idx]))
                new_logp = dist.log_prob(act_b[idx])
                a = adv[idx]
                a = (a - a.mean()) / (a.std() + 1e-8)
                ratio = torch.exp(new_logp - logp_b[idx])
                policy_loss = -torch.min(ratio * a, torch.clamp(ratio, 1 - CLIP, 1 + CLIP) * a).mean()
                value_loss = (model.value(obs_b[idx]).squeeze(-1) - returns[idx]).pow(2).mean()
                loss = policy_loss + VALUE_COEF * value_loss - ENTROPY_COEF * dist.entropy().mean()
                opt.zero_grad()
                loss.backward()
                nn.utils.clip_grad_norm_(model.parameters(), 0.5)
                opt.step()

        recent = np.mean(finished_returns[-20:]) if finished_returns else 0.0
        print(f"round {round_ + 1:2d}: average return of the last 20 episodes {recent:6.1f}")

    torch.save(model.state_dict(), "ppo_cartpole.pt")


if __name__ == "__main__":
    main()
```

The average return climbs from about 20 (random) towards the maximum of 500 within a few dozen rounds (exact numbers vary by machine and version). To adapt it to your own game: replace the environment with one that encodes your game state as `obs` and lists actions; mask illegal actions by setting their logits to `-1e9` before building the `Categorical`; and for two-player games, collect experience for both sides with rewards from each player's point of view.

## How do I write my own reinforcement learning environment in Python with Gymnasium?

Gymnasium (the maintained successor of OpenAI Gym) defines a standard environment API, so any RL library (Stable-Baselines3, CleanRL, RLlib, your own PPO) can train on it. Implement `reset` and `step`, and declare the observation and action spaces:

```python
import gymnasium as gym
import numpy as np
from gymnasium import spaces


class CollectCoinEnv(gym.Env):
    """A 1-D track: the agent starts in the middle and must reach the coin. Actions: 0 = left, 1 = right."""

    def __init__(self, size=10, max_steps=50):
        self.size, self.max_steps = size, max_steps
        self.observation_space = spaces.Box(low=0.0, high=1.0, shape=(2,), dtype=np.float32)
        self.action_space = spaces.Discrete(2)

    def _obs(self):
        return np.array([self.pos / (self.size - 1), self.coin / (self.size - 1)], dtype=np.float32)

    def reset(self, seed=None, options=None):
        super().reset(seed=seed)                       # seeds self.np_random
        self.pos = self.size // 2
        self.coin = int(self.np_random.choice([0, self.size - 1]))
        self.steps = 0
        return self._obs(), {}

    def step(self, action):
        self.pos = int(np.clip(self.pos + (1 if action == 1 else -1), 0, self.size - 1))
        self.steps += 1
        terminated = self.pos == self.coin             # the episode ended naturally
        truncated = self.steps >= self.max_steps       # cut off by the time limit
        reward = 1.0 if terminated else -0.01          # small step penalty: be quick
        return self._obs(), reward, terminated, truncated, {}


env = CollectCoinEnv()
obs, info = env.reset(seed=1)
total, done = 0.0, False
while not done:
    action = 1 if obs[1] > obs[0] else 0              # a hand-written policy, to test the environment
    obs, reward, terminated, truncated, info = env.step(action)
    total += reward
    done = terminated or truncated
print("return:", round(total, 2))
```

- `reset(seed=...)` returns `(observation, info)`; `step(action)` returns `(observation, reward, terminated, truncated, info)`. `terminated` means the episode really ended (win/loss); `truncated` means a time limit cut it off, which matters for value estimates.
- Normalise observations to roughly -1..1 or 0..1.
- Test the environment with a hand-written policy (as above) and with random actions before training anything.
- Libraries can then train it in a few lines, e.g. Stable-Baselines3: `from stable_baselines3 import PPO; PPO("MlpPolicy", CollectCoinEnv()).learn(50_000)`.

To train a Unity game with this API, the environment's `step` sends the action to the game over a socket and returns the game's reply (or use ML-Agents' Gym wrapper).

## How does Monte Carlo Tree Search (MCTS) work? Show code.

MCTS finds good moves by building a search tree from many simulated games. Each iteration has four steps:

1. **Selection**: from the root, repeatedly pick the child with the best UCB score: `wins/visits + c·sqrt(ln(parent visits)/visits)`, which balances moves that have done well (exploitation) with moves tried rarely (exploration).
2. **Expansion**: add one untried move as a new node.
3. **Simulation**: play random moves to the end of the game (AlphaZero-style MCTS replaces this with the value network's estimate, and uses the policy network to prioritise moves).
4. **Backpropagation**: update visit and win counts along the path.

After many iterations, play the most visited move. Tic-tac-toe example:

```python
import math
import random

LINES = [(0, 1, 2), (3, 4, 5), (6, 7, 8), (0, 3, 6), (1, 4, 7), (2, 5, 8), (0, 4, 8), (2, 4, 6)]


def winner(board):
    for a, b, c in LINES:
        if board[a] != " " and board[a] == board[b] == board[c]:
            return board[a]
    return "draw" if " " not in board else None


def moves(board):
    return [i for i, v in enumerate(board) if v == " "]


def play(board, move, player):
    return board[:move] + player + board[move + 1:]


def other(player):
    return "O" if player == "X" else "X"


class Node:
    def __init__(self, board, player, parent=None, move=None):
        self.board, self.player, self.parent, self.move = board, player, parent, move  # player = who moves next
        self.children, self.untried = [], moves(board)
        self.visits, self.wins = 0, 0.0          # wins from the point of view of the player who moved INTO this node

    def ucb_child(self, c=1.4):
        return max(self.children, key=lambda ch: ch.wins / ch.visits + c * math.sqrt(math.log(self.visits) / ch.visits))


def mcts(board, player, iterations=2000, rng=random.Random(0)):
    root = Node(board, player)
    for _ in range(iterations):
        node = root
        # 1. Selection: walk down fully expanded nodes by UCB
        while not node.untried and node.children:
            node = node.ucb_child()
        # 2. Expansion: add one new child
        if node.untried and winner(node.board) is None:
            m = node.untried.pop(rng.randrange(len(node.untried)))
            child = Node(play(node.board, m, node.player), other(node.player), node, m)
            node.children.append(child)
            node = child
        # 3. Simulation: random play to the end
        b, p = node.board, node.player
        while winner(b) is None:
            b = play(b, rng.choice(moves(b)), p)
            p = other(p)
        result = winner(b)
        # 4. Backpropagation: credit each node from the view of the player who made its move
        while node is not None:
            node.visits += 1
            mover = other(node.player)
            node.wins += 1.0 if result == mover else 0.5 if result == "draw" else 0.0
            node = node.parent
    return max(root.children, key=lambda ch: ch.visits).move   # the most visited move is the most robust


board = "XX OO    "           # X to move: X can win at square 2
print("X plays", mcts(board, "X"))
board = "XX  O    "           # O to move: O must block at square 2
print("O plays", mcts(board, "O"))
```

It prints `X plays 2` (the winning move) and `O plays 2` (the necessary block). For games with hidden information (cards in hand, the deck order), don't search the true hidden state: that's cheating, and the AI will learn to rely on it. Instead, sample hidden states consistent with what the player can see (determinisation) and search each, or use information-set MCTS.

## What is DQN (Deep Q-Network) and when would I use it instead of PPO?

DQN learns a **Q-function**: for a state, the expected future reward of each action. The policy is "take the action with the highest Q" (plus random exploration, ε-greedy).

Training uses the Bellman equation: the target for `Q(s, a)` is `r + γ · max_a' Q_target(s', a')` (just `r` if the episode ended), and the loss is the (Huber) difference between them. Two tricks make it stable:

- a **replay buffer**: store past transitions and train on random minibatches, which breaks the correlation between consecutive steps and reuses data;
- a **target network**: a copy of the Q-network, updated slowly (or every N steps), used to compute the targets.

```python
import torch
import torch.nn as nn
import torch.nn.functional as F

q = nn.Sequential(nn.Linear(4, 64), nn.ReLU(), nn.Linear(64, 2))
q_target = nn.Sequential(nn.Linear(4, 64), nn.ReLU(), nn.Linear(64, 2))
q_target.load_state_dict(q.state_dict())
opt = torch.optim.Adam(q.parameters(), lr=1e-3)

# A minibatch from the replay buffer
s, a, r, s2, done = torch.randn(32, 4), torch.randint(0, 2, (32,)), torch.randn(32), torch.randn(32, 4), torch.zeros(32)
with torch.no_grad():
    target = r + 0.99 * (1 - done) * q_target(s2).max(dim=1).values
pred = q(s).gather(1, a.unsqueeze(1)).squeeze(1)
loss = F.smooth_l1_loss(pred, target)
opt.zero_grad(); loss.backward(); opt.step()
print(float(loss))
```

**DQN** suits small, fixed sets of discrete actions and environments where sampling is expensive (it reuses old data). **PPO** is usually easier to tune, handles large or variable action sets (like "which of these legal moves"), continuous actions and self-play well, and parallelises across many simulators, which is why it's the default for game AI and in ML-Agents.

## How do I plot training curves in Python?

Log numbers during training, then plot them with matplotlib:

```python
import matplotlib
matplotlib.use("Agg")                # save to a file without opening a window (remove to show one)
import matplotlib.pyplot as plt
import numpy as np

steps = np.arange(1, 201)
loss = 2.0 / np.sqrt(steps) + 0.05 * np.random.default_rng(0).random(200)
win_rate = 1 - 0.5 * np.exp(-steps / 60)

def smooth(x, k=10):
    return np.convolve(x, np.ones(k) / k, mode="valid")   # moving average: noisy curves are hard to read

fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(10, 4))
ax1.plot(steps, loss, alpha=0.3, label="loss")
ax1.plot(steps[9:], smooth(loss), label="smoothed")
ax1.set_xlabel("step"); ax1.set_ylabel("loss"); ax1.set_yscale("log"); ax1.legend()
ax2.plot(steps, win_rate)
ax2.set_xlabel("step"); ax2.set_ylabel("win rate vs fixed bot"); ax2.set_ylim(0, 1)
fig.tight_layout()
fig.savefig("training.png", dpi=120)
```

For live curves while training, TensorBoard is easier: `from torch.utils.tensorboard import SummaryWriter`, `writer = SummaryWriter("runs/exp1")`, `writer.add_scalar("loss", loss, step)` each step, then run `tensorboard --logdir runs` and open the address it prints (needs `pip install tensorboard`). ML-Agents writes TensorBoard logs automatically.

## How do I organise a machine-learning training project?

A layout that stays manageable as it grows:

```
my_ai/
    README.md            how to install, train and evaluate
    requirements.txt
    config/
        default.yaml     all hyperparameters in one place, not scattered in code
    src/
        env.py           the game interface (or the socket client to the game)
        model.py         the network
        train.py         the training loop: python -m src.train --config config/default.yaml
        evaluate.py      play against fixed opponents, report win rates
    runs/                logs and checkpoints (git-ignored), one folder per experiment
    tests/
        test_model.py    shapes, save/load round trip, a tiny overfit test
```

Habits that save time:

- **One experiment = one folder** with its config, git commit hash, logs and checkpoints, so any result can be reproduced.
- **Change one thing at a time** and compare against a baseline run.
- **Checkpoint often** (and keep the best by evaluation, not just the latest).
- **Evaluate against fixed opponents** at regular intervals.
- **Seed** runs; run important comparisons with 2–3 seeds, because RL results vary a lot between seeds.
- **Start small**: get a tiny version learning end to end first, then scale up the model, data and training time.

# Python and Unity

## What are the ways to connect Python to a Unity game?

The main options, from most to least common for AI and training:

1. **TCP sockets** (Python's `socket`, C#'s `TcpClient`): a small binary or JSON protocol between a Python process and the game. Fast, works with the Editor, builds and headless simulators, and you control everything. This is how a Python GPU learner can train a network on data a C# game or simulator generates.
2. **Unity ML-Agents**: Unity's official toolkit. C# `Agent` components in the game, and the Python `mlagents` trainer (PPO, SAC, self-play, curriculum) talking to it over gRPC. Best when you want standard RL without writing the learning code.
3. **Export the trained network and run it inside Unity**: train in Python, export to ONNX (`torch.onnx.export`), and run it with Unity's inference package (Sentis, now called Inference Engine). No Python needed when the game ships.
4. **Port the network to C#**: save the weights to a file and write the forward pass (matrix multiplies, activations) in C#. No dependencies; full control; also lets you train in C# if you write the backward pass.
5. **HTTP** (a Python FastAPI/Flask server, Unity's `UnityWebRequest`): simple for occasional requests (an LLM, a level generator), too slow for per-frame decisions.
6. **Process pipes**: Unity starts Python with `System.Diagnostics.Process` and exchanges JSON lines over stdin/stdout.

Rule of thumb: training → sockets or ML-Agents; shipping → ONNX/Inference Engine or a C# port; tools and LLM calls → HTTP.

## How do I write a Python socket server that a Unity game can talk to?

Use a small **message framing** protocol: TCP is a stream of bytes, not messages, so each message needs a header saying what it is and how long it is. Here, every message is a 4-byte ASCII type, an int64 payload length (little-endian, like C#'s `BinaryWriter`), then the payload:

```python
import socket
import struct
import threading

import numpy as np

HOST, PORT = "127.0.0.1", 5577


def recv_exact(conn, n):
    """recv() can return fewer bytes than asked for, so loop until we have all n."""
    buf = bytearray()
    while len(buf) < n:
        chunk = conn.recv(n - len(buf))
        if not chunk:
            raise ConnectionError("the game disconnected")
        buf += chunk
    return bytes(buf)


def read_message(conn):
    kind, length = struct.unpack("<4sq", recv_exact(conn, 12))   # 4-byte type, int64 length
    return kind.decode("ascii"), recv_exact(conn, length)


def send_message(conn, kind, payload):
    conn.sendall(struct.pack("<4sq", kind.encode("ascii"), len(payload)) + payload)


def choose_actions(obs):
    """Stand-in for the neural network: one row of observations per agent -> one action each."""
    return obs.argmax(axis=1).astype(np.int32)


def handle(conn, addr):
    print("game connected from", addr)
    with conn:
        try:
            while True:
                kind, payload = read_message(conn)
                if kind == "OBS1":                        # int32 agents, int32 size, float32[agents*size]
                    agents, size = struct.unpack_from("<ii", payload, 0)
                    obs = np.frombuffer(payload, dtype="<f4", count=agents * size, offset=8).reshape(agents, size)
                    actions = choose_actions(obs)
                    send_message(conn, "ACT1", struct.pack("<i", agents) + actions.astype("<i4").tobytes())
                elif kind == "BYE1":
                    break
                else:
                    send_message(conn, "ERR1", f"unknown message {kind}".encode("utf-8"))
        except ConnectionError as e:
            print(e)
    print("game disconnected")


def main():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        server.bind((HOST, PORT))
        server.listen()
        print(f"waiting for the game on {HOST}:{PORT}")
        while True:
            conn, addr = server.accept()
            conn.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)   # send small messages immediately
            threading.Thread(target=handle, args=(conn, addr), daemon=True).start()


if __name__ == "__main__":
    main()
```

Key points:

- `recv()` may return **fewer bytes than requested**, so `recv_exact` loops. Forgetting this is the classic bug that works on your PC and fails under load.
- One thread per connection (`threading.Thread`) lets several game instances connect at once; if they share a GPU model, protect it with a `threading.Lock`.
- `TCP_NODELAY` stops small replies being delayed by Nagle's algorithm (otherwise each round trip can take ~40 ms).
- NumPy reads the floats straight out of the payload (`np.frombuffer`); `"<f4"` is little-endian float32, C#'s `float`.
- Binding to `127.0.0.1` only accepts connections from the same PC, so Windows Firewall won't ask. Use `0.0.0.0` to accept from other machines (and allow it in the firewall).

## How do I connect a Unity C# script to a Python socket server?

Here's a plain C# client for the protocol (4-byte type, int64 length, payload), which works in Unity, in a .NET console app and in tests:

```csharp
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

/// <summary>Talks to the Python server: sends observations, gets actions back. Plain C#, so it also works outside Unity.</summary>
public sealed class PythonBrainClient : IDisposable
{
    TcpClient client;
    BinaryReader reader;
    BinaryWriter writer;

    public void Connect(string host = "127.0.0.1", int port = 5577, int timeoutMs = 5000)
    {
        client = new TcpClient { NoDelay = true, ReceiveTimeout = 60000, SendTimeout = 60000 };
        if (!client.ConnectAsync(host, port).Wait(timeoutMs))
            throw new IOException($"no Python server answered on {host}:{port}");
        var stream = client.GetStream();
        reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);   // little-endian, like Python's "<"
        writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
    }

    /// <summary>Sends one row of observations per agent and returns one action per agent.</summary>
    public int[] Act(float[,] observations)
    {
        int agents = observations.GetLength(0), size = observations.GetLength(1);
        var payload = new MemoryStream();
        using (var w = new BinaryWriter(payload, Encoding.UTF8, leaveOpen: true))
        {
            w.Write(agents);
            w.Write(size);
            foreach (float v in observations) w.Write(v);   // row-major, like NumPy
        }
        Send("OBS1", payload.ToArray());

        var (kind, reply) = Receive();
        if (kind == "ERR1") throw new IOException("Python: " + Encoding.UTF8.GetString(reply));
        if (kind != "ACT1") throw new IOException("unexpected reply " + kind);
        int n = BitConverter.ToInt32(reply, 0);
        var actions = new int[n];
        for (int i = 0; i < n; i++) actions[i] = BitConverter.ToInt32(reply, 4 + 4 * i);
        return actions;
    }

    void Send(string kind, byte[] payload)
    {
        writer.Write(Encoding.ASCII.GetBytes(kind));   // exactly 4 bytes, no length prefix
        writer.Write((long)payload.Length);
        writer.Write(payload);
        writer.Flush();
    }

    (string, byte[]) Receive()
    {
        string kind = Encoding.ASCII.GetString(reader.ReadBytes(4));
        long length = reader.ReadInt64();
        byte[] payload = reader.ReadBytes((int)length);   // ReadBytes loops until it has them all (or the stream ends)
        if (payload.Length != length) throw new EndOfStreamException("Python closed the connection");
        return (kind, payload);
    }

    public void Dispose()
    {
        try { if (client?.Connected == true) Send("BYE1", Array.Empty<byte>()); } catch (IOException) { }
        reader?.Dispose();
        writer?.Dispose();
        client?.Dispose();
    }
}
```

`BinaryWriter` and `BinaryReader` are little-endian, matching Python's `struct` `"<"` format, and `float` is 4 bytes like NumPy's `float32`. Write the message type as raw ASCII bytes: `writer.Write(string)` would add a 7-bit length prefix that Python doesn't expect.

Always set timeouts (`ReceiveTimeout`, a timed connect) so a dead or stuck Python process gives an exception instead of freezing your game forever, and make stopping close the socket so any thread blocked in a read wakes up.

## How do I call Python from Unity without freezing the game?

Unity's API (transforms, `GameObject`s, physics, UI) may only be used from the **main thread**, but a network round trip blocks. So: read the game state on the main thread, do the socket call on a worker thread (`Task.Run`), and apply the result back on the main thread:

```csharp
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>Asks the Python brain for actions without freezing the game: the network call runs on a worker thread,
/// and the result is applied on Unity's main thread in Update.</summary>
public class PythonBrain : MonoBehaviour
{
    public int agentCount = 4;
    public int observationSize = 8;

    PythonBrainClient brain;
    Task<int[]> pending;
    readonly ConcurrentQueue<Action> mainThread = new ConcurrentQueue<Action>();

    void Start()
    {
        brain = new PythonBrainClient();
        try { brain.Connect(); }
        catch (Exception e) { Debug.LogError($"Start the Python server first: {e.Message}"); enabled = false; }
    }

    void Update()
    {
        while (mainThread.TryDequeue(out var action)) action();   // Unity APIs only on the main thread

        if (pending == null)
        {
            float[,] obs = CollectObservations();                  // read the game state here, on the main thread
            pending = Task.Run(() => brain.Act(obs));               // the socket call runs on a worker thread
        }
        else if (pending.IsCompleted)
        {
            if (pending.IsFaulted) Debug.LogError(pending.Exception?.GetBaseException().Message);
            else ApplyActions(pending.Result);
            pending = null;
        }
    }

    float[,] CollectObservations()
    {
        var obs = new float[agentCount, observationSize];
        for (int a = 0; a < agentCount; a++)
            for (int i = 0; i < observationSize; i++)
                obs[a, i] = UnityEngine.Random.value;              // replace with real positions, health, etc.
        return obs;
    }

    void ApplyActions(int[] actions)
    {
        for (int a = 0; a < actions.Length; a++)
            Debug.Log($"agent {a} does action {actions[a]}");      // replace with moving the agents
    }

    void OnDestroy() => brain?.Dispose();
}
```

(`PythonBrainClient` is the socket client class from the previous answer.)

This keeps the frame rate smooth while the network thinks: the game continues, and actions are applied a frame or two later. For training, where every step must wait for its action, it's simpler to make the simulation itself step-locked: advance one step, send observations, wait for actions, repeat, and run the game faster than real time (`Time.timeScale`, `-batchmode -nographics`).

## How do I run a Unity game fast and headless for AI training?

- **Batch mode**: build the game (or a dedicated "Server"/Linux build) and run it without a window:
  ```
  MyGame.exe -batchmode -nographics -logFile train.log
  ```
  `-nographics` skips the GPU entirely (no rendering, cameras produce nothing, so don't use it if observations come from camera images).
- **Speed up time**: `Time.timeScale = 20f;` makes the game run 20× faster; physics stays stable if you keep `Time.fixedDeltaTime` fixed and the machine keeps up. Also set `Application.targetFrameRate = -1` and `QualitySettings.vSyncCount = 0` so frames aren't capped.
- **Run several instances** (one per CPU core), each connecting to the Python learner on its own connection, so data comes in much faster.
- **Separate simulation from presentation**: if the game logic doesn't need Unity (card games, turn-based games), put the rules in plain C# classes, so a headless .NET console program can play thousands of games per second without Unity at all, and Unity just displays them. This is usually the biggest speed-up of all.
- Use command-line arguments for settings: `System.Environment.GetCommandLineArgs()` in C#.

## How do I set up Unity ML-Agents?

ML-Agents has two parts: a Unity package (C#) and a Python trainer.

**Unity side**: Window → Package Manager → Unity Registry → install **ML Agents** (`com.unity.ml-agents`).

**Python side** (in its own venv, with the Python version the ML-Agents docs list for your release; it has been 3.10.x):

```
py -3.10 -m venv mlagents-env
mlagents-env\Scripts\activate
pip install mlagents
mlagents-learn --help
```

**Train**: write an `Agent` script (next answer), add it plus a **Behavior Parameters** component to your agent object (set the behaviour name, observation size and actions), then:

```
mlagents-learn config\my_agent.yaml --run-id=run1
```

When it says "Start training by pressing the Play button in the Unity Editor", press Play. Results and the trained `.onnx` model go to `results\run1`. Watch progress with `tensorboard --logdir results`. Resume with `--resume`, or train against a built game with `--env=Builds\MyGame.exe --num-envs=4 --no-graphics`.

A minimal config:

```yaml
behaviors:
  MyAgent:
    trainer_type: ppo
    hyperparameters:
      batch_size: 256
      buffer_size: 4096
      learning_rate: 3.0e-4
    network_settings:
      hidden_units: 128
      num_layers: 2
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
    max_steps: 1000000
```

Drag the resulting `.onnx` into the agent's Behavior Parameters → Model to run it in the game without Python.

## What does an ML-Agents Agent script look like?

```csharp
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    public Transform target;
    public float speed = 10f;
    Rigidbody body;

    public override void Initialize() => body = GetComponent<Rigidbody>();

    public override void OnEpisodeBegin()
    {
        // Reset the scene for a new attempt
        body.linearVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.5f, 0);
        target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 8 numbers: set "Space Size" = 8 in Behavior Parameters
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(body.linearVelocity.x);
        sensor.AddObservation(body.linearVelocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 2 continuous actions: set them in Behavior Parameters
        var force = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        body.AddForce(force * speed);

        if (Vector3.Distance(transform.localPosition, target.localPosition) < 1.4f)
        {
            SetReward(1f);        // reached it
            EndEpisode();
        }
        else if (transform.localPosition.y < 0)
        {
            EndEpisode();         // fell off
        }
        else
        {
            AddReward(-0.001f);   // small time penalty: hurry up
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Lets you drive it with the keyboard to test (Behavior Type = Heuristic Only)
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetAxis("Horizontal");
        c[1] = Input.GetAxis("Vertical");
    }
}
```

Add a **Decision Requester** component so the agent asks for a decision every few physics steps. For discrete choices (pick one of N moves), use `actions.DiscreteActions[0]` and mask illegal ones with `WriteDiscreteActionMask`. (`linearVelocity` is the Unity 6 name; older versions call it `velocity`.)

## How do I export a PyTorch model to ONNX?

ONNX is a standard file format for trained networks that many runtimes can load, including Unity's inference package (Sentis / Inference Engine), ONNX Runtime and OpenCV.

```python
import torch
import torch.nn as nn

class Policy(nn.Module):
    def __init__(self, n_obs=8, n_actions=4):
        super().__init__()
        self.net = nn.Sequential(nn.Linear(n_obs, 64), nn.ReLU(), nn.Linear(64, n_actions))

    def forward(self, obs):
        return torch.softmax(self.net(obs), dim=-1)

model = Policy().eval()
example = torch.zeros(1, 8)                       # an example input with the right shape
torch.onnx.export(
    model, (example,), "policy.onnx",
    input_names=["obs"], output_names=["probs"],
    dynamic_axes={"obs": {0: "batch"}, "probs": {0: "batch"}},   # allow any batch size
    opset_version=17,
)
```

(It needs `pip install onnx`; recent PyTorch versions may also want `onnxscript`, and can use the newer exporter with `dynamo=True`.) Check it with ONNX Runtime before going to Unity:

```python
import numpy as np
import onnxruntime as ort

session = ort.InferenceSession("policy.onnx")
out = session.run(None, {"obs": np.zeros((1, 8), dtype=np.float32)})[0]
with torch.no_grad():
    expected = model(torch.zeros(1, 8)).numpy()
print(np.abs(out - expected).max())               # should be about 1e-7
```

Keep the model to standard layers (Linear, Conv, ReLU, softmax, attention, etc.); custom Python logic inside `forward` (loops that depend on data, NumPy calls, dicts) can't be exported. Do pre- and post-processing (masking illegal moves, sampling) in the game's C# code instead.

## How do I run an ONNX model inside Unity?

Use Unity's neural-network inference package: **Sentis** (`com.unity.sentis`), renamed **Inference Engine** (`com.unity.ai.inference`) in 2025. The API is the same apart from the namespace: `using Unity.Sentis;` for Sentis 2.x, `using Unity.InferenceEngine;` for Inference Engine.

1. Install it from the Package Manager.
2. Drag the `.onnx` file into the Assets folder; Unity imports it as a `ModelAsset`.
3. Run it:

```csharp
using Unity.InferenceEngine;   // Sentis 2.x: using Unity.Sentis;
using UnityEngine;

public class OnnxPolicy : MonoBehaviour
{
    public ModelAsset modelAsset;     // assign policy.onnx in the Inspector
    Worker worker;

    void Start()
    {
        Model model = ModelLoader.Load(modelAsset);
        worker = new Worker(model, BackendType.GPUCompute);   // or BackendType.CPU
    }

    public float[] Probabilities(float[] observation)
    {
        using var input = new Tensor<float>(new TensorShape(1, observation.Length), observation);
        worker.Schedule(input);
        var output = worker.PeekOutput() as Tensor<float>;
        return output.DownloadToArray();                     // waits for the result
    }

    void OnDestroy() => worker?.Dispose();
}
```

Notes: dispose tensors and workers (they hold GPU memory); batch several agents into one tensor `(agents, obs)` for speed; small networks are often faster on `BackendType.CPU` than the GPU, because copying the result back costs more than the maths. Check the package documentation for your exact version, as method names have changed between major versions (Sentis 1.x used `IWorker`, `Execute` and `TensorFloat`).

# Porting Python to C#, and checking code

## How do I convert Python code to C#?

Translate the structure first, then the idioms. The common mappings:

| Python | C# |
|---|---|
| `x = 5` (no type) | `var x = 5;` / `int x = 5;` |
| `def f(a, b=2) -> int:` | `int F(int a, int b = 2)` |
| `list`, `append`, `len(xs)` | `List<T>`, `Add`, `xs.Count` |
| `dict`, `d.get(k, 0)` | `Dictionary<K,V>`, `d.TryGetValue(k, out var v) ? v : 0` / `d.GetValueOrDefault(k)` |
| `set` | `HashSet<T>` |
| `for i in range(n):` | `for (int i = 0; i < n; i++)` |
| `for i, x in enumerate(xs):` | `for (int i = 0; i < xs.Count; i++) { var x = xs[i]; ... }` |
| `[f(x) for x in xs if p(x)]` | `xs.Where(p).Select(f).ToList()` |
| `None`, `is None` | `null`, `is null` |
| `True and not False` | `true && !false` |
| `x if cond else y` | `cond ? x : y` |
| `try/except E as e/finally` | `try/catch (E e)/finally` |
| `with open(p) as f:` | `using var f = File.OpenText(p);` |
| `yield` | `yield return` (in a method returning `IEnumerable<T>`) |
| `@dataclass` | `record` or a class with properties |
| `f"{x:.2f}"` | `$"{x:F2}"` |
| `a // b` (floor division) | `Math.Floor((double)a / b)`, or `a / b` only when both are non-negative |
| `import numpy as np` | arrays and loops, `System.Numerics`, or a library such as MathNet.Numerics |

A worked example:

```python
def best_attack(attacks, energy):
    usable = [a for a in attacks if a["cost"] <= energy]
    if not usable:
        return None
    return max(usable, key=lambda a: a["damage"])["name"]
```

```csharp
public record Attack(string Name, int Cost, int Damage);

public static string BestAttack(IEnumerable<Attack> attacks, int energy)
{
    var usable = attacks.Where(a => a.Cost <= energy).ToList();
    if (usable.Count == 0) return null;
    return usable.MaxBy(a => a.Damage).Name;      // .NET 6+; in Unity: usable.OrderByDescending(a => a.Damage).First()
}
```

Replace "dict of strings" data with typed classes or records: it's more C#-like and the compiler then catches typos. Then test the port against the Python version on the same inputs.

## What are the gotchas when porting Python code to C#?

Things that silently give different answers:

- **Integer division and modulo with negatives**: Python floors, C# truncates. `-7 // 2` is `-4` in Python but `-7 / 2` is `-3` in C#; `-7 % 3` is `2` in Python, `-1` in C#. Use `Math.Floor`, or `((a % n) + n) % n` for a Python-style modulo.
- **Integer size**: Python ints never overflow; C# `int` wraps at 2,147,483,647 (use `long`, `checked`, or `BigInteger`). Hash functions and seeds are common victims.
- **Float precision**: Python `float` is 64-bit (`double`); NumPy and PyTorch usually use `float32` (`float`). Match the type, or results drift.
- **Rounding**: Python's `round(2.5)` is `2` (round half to even), and so is C#'s default `Math.Round(2.5)`; but `Math.Round(x, MidpointRounding.AwayFromZero)` and Unity's `Mathf.RoundToInt` differ in places, so pick deliberately.
- **Ranges and slices**: `range(a, b)` excludes `b`; `xs[-1]` is the last item (`xs[^1]` in C# 8, but not on `List<T>` in older Unity versions); slices copy.
- **Truthiness**: Python treats `0`, `""`, `[]`, `None` as false; C# needs explicit `x != 0`, `s.Length > 0`, `list.Count > 0`.
- **Dictionary order**: Python dicts keep insertion order; C#'s `Dictionary` order isn't guaranteed. Sort, or use a `List` of pairs, when order matters.
- **Random numbers**: `random.seed(1)` and `new System.Random(1)` produce different sequences, so ported code won't reproduce the same games. To compare, generate the random numbers once and feed both versions the same ones.
- **Mutable defaults and aliasing**: `b = a` for lists shares the list in both languages; but struct copying in C# (Unity's `Vector3`, `Color`) copies the value, which Python never does.
- **String formatting and parsing**: C# uses the computer's culture, so `float.Parse("0.5")` fails on PCs where the decimal separator is a comma. Use `CultureInfo.InvariantCulture`.

## How do I port a trained PyTorch network to C# so it runs without Python?

1. **Save the weights in a simple binary format** from Python:

```python
import struct

import numpy as np
import torch
import torch.nn as nn

class Policy(nn.Module):
    def __init__(self, n_obs=8, hidden=32, n_actions=4):
        super().__init__()
        self.l1 = nn.Linear(n_obs, hidden)
        self.l2 = nn.Linear(hidden, n_actions)

    def forward(self, x):
        return self.l2(torch.relu(self.l1(x)))

def save_weights(model, path):
    """int32 count; per tensor: int32 name length, UTF-8 name, int32 ndim, int32 dims, float32 data (little-endian)."""
    tensors = model.state_dict()
    with open(path, "wb") as f:
        f.write(struct.pack("<i", len(tensors)))
        for name, t in tensors.items():
            data = t.detach().cpu().numpy().astype("<f4")
            raw = name.encode("utf-8")
            f.write(struct.pack("<i", len(raw)) + raw)
            f.write(struct.pack("<i", data.ndim) + struct.pack(f"<{data.ndim}i", *data.shape))
            f.write(data.tobytes())   # row-major (C order), what C# expects

torch.manual_seed(0)
model = Policy().eval()
save_weights(model, "policy.bin")

# Test cases for the C# port: inputs and the outputs PyTorch gives for them
x = torch.randn(5, 8)
with torch.no_grad():
    y = model(x)
np.savetxt("test_inputs.txt", x.numpy())
np.savetxt("test_outputs.txt", y.numpy())
```

2. **Load them and write the forward pass in C#**, using the same layout PyTorch uses (an `nn.Linear` weight is `[out, in]`, row-major):

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>The Python Policy network (Linear -> ReLU -> Linear) in plain C#, loading weights saved by save_weights.</summary>
public sealed class PolicyNet
{
    readonly Dictionary<string, (int[] Shape, float[] Data)> tensors = new Dictionary<string, (int[], float[])>();

    public PolicyNet(string path)
    {
        using var r = new BinaryReader(File.OpenRead(path), Encoding.UTF8);   // little-endian, like "<" in Python
        int count = r.ReadInt32();
        for (int t = 0; t < count; t++)
        {
            string name = Encoding.UTF8.GetString(r.ReadBytes(r.ReadInt32()));
            var shape = new int[r.ReadInt32()];
            for (int d = 0; d < shape.Length; d++) shape[d] = r.ReadInt32();
            var data = new float[shape.Aggregate(1, (a, b) => a * b)];
            for (int i = 0; i < data.Length; i++) data[i] = r.ReadSingle();
            tensors[name] = (shape, data);
        }
    }

    /// <summary>nn.Linear: y = x W^T + b, with W stored as [out, in] row-major, exactly as PyTorch keeps it.</summary>
    float[] Linear(float[] x, string layer)
    {
        var (shape, w) = tensors[layer + ".weight"];
        float[] b = tensors[layer + ".bias"].Data;
        int outs = shape[0], ins = shape[1];
        if (x.Length != ins) throw new ArgumentException($"{layer} expects {ins} inputs, got {x.Length}");
        var y = new float[outs];
        for (int o = 0; o < outs; o++)
        {
            float sum = b[o];
            int row = o * ins;
            for (int i = 0; i < ins; i++) sum += w[row + i] * x[i];
            y[o] = sum;
        }
        return y;
    }

    public float[] Forward(float[] observation)
    {
        float[] h = Linear(observation, "l1");
        for (int i = 0; i < h.Length; i++) h[i] = Math.Max(0f, h[i]);   // ReLU
        return Linear(h, "l2");
    }

    public static float[] Softmax(float[] logits)
    {
        float max = logits.Max();                  // subtract the max so Exp can't overflow
        var e = logits.Select(v => (float)Math.Exp(v - max)).ToArray();
        float sum = e.Sum();
        return e.Select(v => v / sum).ToArray();
    }
}
```

3. **Test the port against PyTorch** on saved inputs and outputs (next answer).

This needs no packages, so it works in Unity, in a console app and on any platform. For bigger networks, speed it up with batching (one call for many positions), `System.Numerics.Vector<float>` (SIMD), Unity's Burst compiler and jobs, or switch to ONNX and Unity's Inference Engine. Note that the C# version must apply the same input encoding and the same activations in the same order as Python, which is where most porting bugs hide.

## How do I check that my C# port of a network gives the same results as PyTorch?

Write a **parity test**: run both on the same inputs and compare. Python saves test inputs and PyTorch's outputs (`test_inputs.txt`, `test_outputs.txt` from the export script); C# loads the weights, runs the same inputs and reports the largest difference:

```csharp
using System;
using System.Globalization;
using System.IO;
using System.Linq;

static class ParityTest
{
    static float[][] ReadRows(string path) => File.ReadAllLines(path)
        .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(v => float.Parse(v, CultureInfo.InvariantCulture)).ToArray())
        .ToArray();

    static int Main()
    {
        var net = new PolicyNet("policy.bin");
        var inputs = ReadRows("test_inputs.txt");
        var expected = ReadRows("test_outputs.txt");
        double worst = 0;
        for (int s = 0; s < inputs.Length; s++)
        {
            var got = net.Forward(inputs[s]);
            for (int i = 0; i < got.Length; i++) worst = Math.Max(worst, Math.Abs(got[i] - expected[s][i]));
        }
        Console.WriteLine($"largest difference from PyTorch: {worst:E2}");
        return worst < 1e-5 ? 0 : 1;      // float32 maths in a different order: tiny differences are normal
    }
}
```

What to expect and check:

- Differences around 1e-7 to 1e-6 are normal (float32 sums done in a different order). Differences of 1e-2 or more are bugs: usually a transposed weight matrix, a missing bias or activation, a different input order, or a softmax over the wrong axis.
- Test several inputs, including edge cases (all zeros, large values, masked options).
- If you also port **training** (the backward pass), check gradients the same way: compare your C# gradients with PyTorch's `.grad` for the same batch and loss, and/or with a numerical gradient: `(loss(w + ε) - loss(w - ε)) / 2ε` for a few weights, with ε around 1e-3 in double precision.
- Put the test in your test suite, so later changes to either side can't silently break the match.

## How do I implement the Adam optimizer in C#?

Adam keeps two running averages per weight: `m` (of the gradient) and `v` (of the squared gradient), with a bias correction for the first steps:

```csharp
using System;

public sealed class Adam
{
    readonly float lr, beta1, beta2, eps;
    readonly float[] m, v;
    int t;

    public Adam(int size, float lr = 1e-3f, float beta1 = 0.9f, float beta2 = 0.999f, float eps = 1e-8f)
    {
        this.lr = lr; this.beta1 = beta1; this.beta2 = beta2; this.eps = eps;
        m = new float[size];
        v = new float[size];
    }

    /// <summary>One update of weights in place from their gradients (same length, same order every step).</summary>
    public void Step(float[] weights, float[] grads)
    {
        t++;
        float c1 = 1 - MathF.Pow(beta1, t), c2 = 1 - MathF.Pow(beta2, t);
        for (int i = 0; i < weights.Length; i++)
        {
            float g = grads[i];
            m[i] = beta1 * m[i] + (1 - beta1) * g;
            v[i] = beta2 * v[i] + (1 - beta2) * g * g;
            float mHat = m[i] / c1, vHat = v[i] / c2;
            weights[i] -= lr * mHat / (MathF.Sqrt(vHat) + eps);
        }
    }
}
```

This matches `torch.optim.Adam` (without weight decay). For AdamW, also do `weights[i] -= lr * weightDecay * weights[i]` in the loop. Save `m`, `v` and `t` with the weights so training can resume without a jump. To verify, run one step in both PyTorch and C# from the same weights and gradients and compare the results.

## How do I start a Python script from C# or Unity and talk to it?

For low-traffic requests (a tool, an LLM call, a level generator), start Python as a child process and exchange one JSON object per line over stdin/stdout:

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

/// <summary>Starts a Python script and exchanges one JSON object per line over its stdin/stdout.</summary>
public sealed class PythonProcess : IDisposable
{
    readonly Process process;

    public PythonProcess(string pythonExe, string script)
    {
        var info = new ProcessStartInfo(pythonExe, $"-u \"{script}\"")   // -u: unbuffered, so replies arrive at once
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        process = Process.Start(info) ?? throw new IOException("couldn't start " + pythonExe);
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.Error.WriteLine("python: " + e.Data); };
        process.BeginErrorReadLine();                 // read stderr too, or a full stderr pipe can freeze Python
    }

    public T Ask<T>(object request)
    {
        process.StandardInput.WriteLine(JsonSerializer.Serialize(request));
        process.StandardInput.Flush();
        string line = process.StandardOutput.ReadLine() ?? throw new IOException("Python exited");
        return JsonSerializer.Deserialize<T>(line);
    }

    public void Dispose()
    {
        try { process.StandardInput.Close(); if (!process.WaitForExit(3000)) process.Kill(); }
        catch (InvalidOperationException) { }
        process.Dispose();
    }
}
```

```python
import json
import sys

# One JSON request per line on stdin, one JSON reply per line on stdout. Logs go to stderr, never stdout.
for line in sys.stdin:
    request = json.loads(line)
    scores = request["scores"]
    best = max(range(len(scores)), key=scores.__getitem__)
    print(json.dumps({"action": best, "count": len(scores)}), flush=True)
    print(f"answered a request with {len(scores)} scores", file=sys.stderr)
```

```csharp
record Reply(int action, int count);

using var py = new PythonProcess("python", "brain.py");
var reply = py.Ask<Reply>(new { scores = new[] { 0.1, 0.7, 0.2 } });
Console.WriteLine(reply.action);   // 1
```

Rules: Python must flush after each reply (`flush=True` or `python -u`) and must never print anything else to stdout; logs go to stderr. Always read stderr (as above), or Python can freeze when its error output fills up. In Unity, point `pythonExe` at your venv's `python.exe` (e.g. `E:\MyProject\.venv\Scripts\python.exe`), run requests off the main thread, and kill the process in `OnDestroy`/`OnApplicationQuit`. `System.Text.Json` needs a package in Unity; `JsonUtility` or Newtonsoft JSON (`com.unity.nuget.newtonsoft-json`) work there. For many requests per second, use a socket instead.

## How do I make a connection between a game and a Python learner robust?

Lessons that save hours of debugging:

- **Timeouts everywhere**: a connect timeout, and read/write timeouts on the socket. A hung peer should cause an error, never an endless wait.
- **Stopping must not hang**: if one thread is blocked reading from the socket, stopping should **close the socket** from the other thread; that makes the blocked read throw immediately, so the training loop can exit. A "Stop" button that only sets a flag can wait forever on a read that never returns.
- **A version number in every message** (or a handshake), so an old learner and a new game give a clear error instead of misreading bytes.
- **Error messages travel back**: if the learner hits an exception, send an error message with the text before closing, and show it in the game.
- **One connection per game instance**, each with its own thread and state on the Python side; share the GPU model behind a lock.
- **Reconnect with backoff** if the learner restarts, and resend whatever state it needs (e.g. the current weights).
- **Read exactly**: loop until the full message length has arrived (`recv_exact`), and check lengths before allocating.
- **Fall back**: if the learner isn't running, the game can carry on with a CPU path, and say so.
- **Log both sides** with timestamps; most "it's stuck" bugs are one side waiting for a message the other side never sent.

## How should a Unity or C# simulator and a Python GPU learner divide the work?

A common design for training a game AI on one PC:

- **The simulator (C#)** plays games on the CPU, many at once (one per core), using the current network to choose moves. It records each decision: the encoded state, the legal options, the chosen action, its probability, and later the result.
- **The learner (Python + PyTorch)** receives a round of recorded decisions, runs the training update (e.g. PPO for a few epochs) on the GPU, and sends the new weights back.
- **Move choice** can stay on the CPU (simple, low latency) or also go to the GPU: then the simulator batches the positions of all its running games into one request, which is only faster when the batches are large (hundreds of positions).

Why split it: game rules are fast in C# and awkward to port, while PyTorch on the GPU learns several times faster than hand-written C# training. The CPU and GPU also work at the same time.

Messages needed: set weights, choose moves for a batch (inference), train on a round (returns new weights and loss statistics), and an error message. Keep the network's definition identical on both sides (same layer names and shapes), and test that the C# forward pass and PyTorch's agree on the same weights.

## How do I convert a Google Colab or Jupyter notebook to run on my own PC?

1. **Download it as a script**: File → Download → Download .py (Colab), or `jupyter nbconvert --to script notebook.ipynb`. Or run the notebook locally: `pip install jupyterlab` then `jupyter lab`, or open the `.ipynb` in VS Code.
2. **Turn shell lines into setup steps**: lines starting with `!` (`!pip install ...`, `!nvidia-smi`, `!wget ...`) are shell commands. Run the `pip install`s once in your venv, and replace downloads with `urllib.request.urlretrieve` or manual downloads. `%` lines (`%cd`, `%%time`) are notebook magics: remove them or use `os.chdir`/`time`.
3. **Remove Colab-only code**: `from google.colab import drive, files`, `drive.mount(...)`, `files.upload()`, `files.download(...)`. Use local paths instead, e.g. `Path("E:/Data/train.jsonl")`.
4. **Check the GPU and versions**: install the CUDA build of PyTorch, check `torch.cuda.is_available()`, and lower batch sizes for a smaller GPU (Colab's T4 has 15 GB; an RTX 2060 has 6 GB). Some libraries (e.g. Unsloth, bitsandbytes, triton) have their own Windows install steps or need WSL2.
5. **Wrap top-level code** in `main()` with the `if __name__ == "__main__":` guard, especially if it uses multiprocessing or DataLoader workers.
6. **Run it from a terminal** so you see errors: `python notebook.py`.

## What are the most common PyTorch error messages and how do I fix them?

- **`RuntimeError: mat1 and mat2 shapes cannot be multiplied (32x10 and 20x64)`**: the input has 10 features but the `nn.Linear` expects 20. Fix the layer's `in_features`, or the input encoding.
- **`Expected all tensors to be on the same device, but found at least two devices, cuda:0 and cpu`**: move the model and every input to the same device (`.to(device)`), including tensors you create inside `forward` (`torch.zeros(..., device=x.device)`).
- **`expected scalar type Long but found Float`** (in `cross_entropy`, `nn.Embedding`, indexing): class targets and ids must be `torch.long`: `targets.long()`.
- **`Found dtype Double but expected Float`**: NumPy arrays are float64 by default; convert with `torch.as_tensor(arr, dtype=torch.float32)`.
- **`CUDA out of memory`**: smaller batch, shorter sequences, mixed precision, `torch.no_grad()` for evaluation, don't keep tensors that hold the graph (store `loss.item()`, not `loss`), and free memory with `del` + `torch.cuda.empty_cache()`.
- **`Trying to backward through the graph a second time`**: you reused an output from a previous step. Recompute the forward pass every step, or `.detach()` values carried between steps (like RNN states or targets).
- **`one of the variables needed for gradient computation has been modified by an inplace operation`**: an in-place op (`x += 1`, `relu_(...)`, `x[mask] = 0`) changed a tensor autograd needs. Use the out-of-place version (`x = x + 1`).
- **`element 0 of tensors does not require grad and does not have a grad_fn`**: the loss isn't connected to the parameters (computed under `no_grad`, from `.detach()`ed or NumPy values, or `requires_grad` is off).
- **`DataLoader worker ... exited unexpectedly`** or an endless loop on Windows: add the `if __name__ == "__main__":` guard, or use `num_workers=0`.

## How should I check code (mine or an AI's) before using it?

A routine that catches most problems before they reach your project:

1. **Run it.** Code that has never been run is a guess. Start with the smallest real input.
2. **Read the first error carefully** (the bottom of a Python traceback, the first C# compiler error), fix that one, and run again. Later errors are often caused by the first.
3. **Write or run tests** for the behaviour you care about, including edge cases: empty inputs, one item, the maximum, invalid values, negative numbers.
4. **Use the tools**: a linter and formatter (`ruff`, or the C# compiler's warnings), a type checker (`mypy`/`pyright`), and the debugger rather than guessing.
5. **Check the claims against reality**: does that API, parameter or package version actually exist? Look at the official docs or the library's source; AI models sometimes invent plausible-looking functions.
6. **Compare against a reference**: for ports and optimisations, run the old and new versions on the same inputs and compare outputs (parity tests).
7. **Keep changes small** and commit working states (git), so a bad change is easy to undo.
8. **Re-read the diff** as a reviewer: unhandled errors, resources not closed, threads touching Unity objects, hard-coded paths, secrets in code.

For game code in Unity specifically: check the Console for errors and warnings, test in Play mode and in a build (some things only fail in builds), and use the Profiler if it's slow.

