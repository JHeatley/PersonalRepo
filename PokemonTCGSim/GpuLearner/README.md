# GPU learner

Makes the Neural AI's training learn on your NVIDIA graphics card. The sim still plays the training games on the
CPU, but learning from them (most of the training time) moves to the GPU, which is about 2 to 3 times as fast
overall and keeps the heaviest maths off the CPU.

## Set up (once)

1. Install Python 3.10 or newer from python.org (tick "Add python.exe to PATH").
2. Install PyTorch with CUDA (the RTX 20-, 30- and 40-series work with this one):

   ```
   pip install torch --index-url https://download.pytorch.org/whl/cu121
   pip install numpy
   ```

3. Check it sees the card:

   ```
   python -c "import torch; print(torch.cuda.is_available(), torch.cuda.get_device_name(0))"
   ```

   It should print `True` and the card's name.

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

`python learner.py --device cpu` runs it on the CPU instead (PyTorch's CPU maths is still faster than the
sim's own), and `--port` changes the port.

## What it does

The network is the sim's own (`Assets/PokemonTCG/AI/PolicyNet.cs`), rebuilt in PyTorch with the same weights;
each round the sim sends its current weights and the round's decisions, the learner runs the same PPO learning
(the same settings, dual clip, gradient clipping and Adam) and sends the learned weights back. The sim's network
and the learner's agree to within rounding (largest difference 7e-5 on 898 real decisions). It doesn't read or
change the Python bot (`PokemonTCGBot`). The message layout is at the top of `learner.py`.
