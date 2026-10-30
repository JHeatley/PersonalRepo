# GPU learner

Makes the Neural AI's training learn on your NVIDIA graphics card. The sim still plays the training games on the
CPU, but learning from them (most of the training time) moves to the GPU, which is about 2 to 3 times as fast
overall and keeps the heaviest maths off the CPU.

## The easy way (Windows)

Install Python 3.10 or newer from python.org (tick "Add python.exe to PATH"), then double-click
`start_gpu_learner.bat`. The first time, it sets up its own copy of PyTorch for NVIDIA cards (about 3 GB, in a `.venv`
folder here, so nothing else on the PC changes) and checks it can see the card; every time, it starts the learner.
Keep its window open while training.

A Windows build of the game carries this folder beside the game (`GpuLearner/`), so someone you give the game to does
the same there. Without it, training still works: it learns on the CPU.

## Set up by hand (once)

1. Install Python 3.10 or newer from python.org (tick "Add python.exe to PATH").
2. Install PyTorch **with CUDA**. A plain `pip install torch` on Windows gets the CPU-only build, which can't use the
   card; if you have that one, uninstall it first (pip won't replace it otherwise):

   ```
   pip uninstall -y torch
   pip install torch --index-url https://download.pytorch.org/whl/cu128
   pip install numpy
   ```

   If pip finds no match, try `cu126` instead of `cu128` (pytorch.org's "Get Started" page lists the current ones).

3. Check it sees the card:

   ```
   python -c "import torch; print(torch.cuda.is_available(), torch.cuda.get_device_name(0))"
   ```

   It should print `True` and the card's name. `AssertionError: Torch not compiled with CUDA enabled` means the
   CPU-only build is still installed (step 2); `False` usually means the NVIDIA driver needs updating.

## Use

1. Start the learner and leave its window open:

   ```
   cd PokemonTCGSim/GpuLearner
   python learner.py
   ```

   It says `GPU learner ready on 127.0.0.1:5577, learning on cuda:0 (...)`.
2. In the sim, on the Train screen, tick **Learn on the GPU at** (the address is `127.0.0.1:5577` unless you
   started the learner with another `--port`), pick **Neural AI** and start training. The status line says
   "learning on the GPU (cuda:0 ...)" and the learner's window prints a line for each round.
   The command-line trainer takes `--gpu 127.0.0.1:5577`.

If the learner isn't running or stops, the sim learns that round on the CPU, says why, and tries the GPU again a
few rounds later; nothing is lost. Stop the learner with Ctrl+C when you're done.

**More than one sim at once** (v170): each sim that connects gets its own network on the learner, so a trainer still
running from earlier (a `sim-train.exe` left behind when Unity was closed) can't hold up a new one. When a second sim
connects, the window says `2 sims connected now`: if you didn't start another trainer, end the old `sim-train.exe` in
Task Manager. An older `learner.py` served one sim at a time, so a new trainer sat at 0 games until the old one went
away; copy this one over yours.

**Choose moves on the GPU too** (the Train screen's box under "Learn on the GPU at", `--gpu-moves` for the
command-line trainer): running the network for each move is about half of a training game's work on the CPU. With this
on, about 4 games per thread play at once, and the learner chooses all their moves together on the graphics card, so the
CPU only runs the games. The learner keeps the network between messages; the sim sends it again only when it changed.
It needs this version of `learner.py`; if the learner can't choose moves, the games choose them on the CPU and the GPU
is tried again 30 seconds later.

**Learn from look-ahead** (the Train screen's setting, `--search-share` for the command-line trainer) needs this
version of `learner.py`: those rounds also send the look-ahead's choices, which the learner trains the network to match.
An older learner.py says "different versions" and the sim learns those rounds on the CPU.

`python learner.py --device cpu` runs it on the CPU instead (PyTorch's CPU maths is still faster than the
sim's own), and `--port` changes the port.

## What it does

The network is the sim's own (`Assets/PokemonTCG/AI/PolicyNet.cs`), rebuilt in PyTorch with the same weights;
each round the sim sends its current weights and the round's decisions, the learner runs the same PPO learning
(the same settings, dual clip, gradient clipping and Adam) and sends the learned weights back. The sim's network
and the learner's agree to within rounding (largest difference 7e-5 on 898 real decisions). It doesn't read or
change the Python bot (`PokemonTCGBot`). The message layout is at the top of `learner.py`.

It also follows the sim's network when it changes shape (Make the network bigger), and keeps a second model for
the league's exploiters (a copy of the network training only to beat it), so their learning never mixes with the
network's own. Copy the new `learner.py` along with the sim's files: an older one can't train exploiters (the sim
then trains them on the CPU and says so).
