# Neural AI trainer (command line)

Trains the sim's Neural AI without Unity. It is the same network and PPO training the
Train screen uses (Neural AI), but much faster, because .NET uses the CPU's vector
instructions and Unity's runtime doesn't. It trains on every built-in deck at once,
the way the Python bot was trained.

Needs the .NET 8 SDK (https://dotnet.microsoft.com/download). From this folder:

```
dotnet run -c Release -- train --games 100000
dotnet run -c Release -- eval --games 400
```

## train

Self-play PPO on all the decks in `Assets/PokemonTCG/Resources/Decklists`. It keeps
going from `out/policy.neural` (or the AI's starting network the first time), saves
after every round and prints a report against the classic bot every `--report-every`
games. Ctrl+C stops it after the current round.

| Option | Default | |
|---|---|---|
| `--games N` | 10000 | games to play |
| `--net FILE` | `out/policy.neural` | network to start from (a `.neural` file or a Python export) |
| `--out FILE` | `out/policy.neural` | where to save |
| `--decks a,b,c` | all | only these decklists (file names without `.txt`) |
| `--deck-folder DIR` | | also the `.txt` decklists in this folder (your own decks) |
| `--focus-deck NAME` | | this deck in every game, against the field |
| `--threads N` | cores − 1 | games played at once |
| `--report-every N` | 5000 | games between reports |
| `--eval-games N` | 200 | games per report against the classic bot |
| `--sequence-penalty X` | 0.02 | reward taken off per sequencing mistake |
| `--games-per-round N` | 128 | games between learning steps |
| `--gpu 127.0.0.1:5577` | | learn on the graphics card: start `../GpuLearner/learner.py` first (see its README) |
| `--games-per-second N` | | play at most N games a second (room to spare for the CPU) |
| `--skip-cpu 2,3` | | keep off these CPU threads (a faulty core; Windows only; train and eval) |

## eval

Plays the network (top-rated moves) against the classic bot, or against another network
with `--vs FILE`, half the games on each seat, random decks.

## Using what you trained in the sim

The sim keeps the Neural AI's network next to the AI's file (the Save Folders screen
shows where): `<AI file>.neural`. Copy `out/policy.neural` over it while the sim is
closed. Training on the Train screen continues from there.

## background

What the app's **Train in the background (fastest)** button runs: a training session on the AI's own brain file, as
the Train screen would run it. The app writes a job folder (`job.txt` with the settings, `decks/` with the decklists),
then this keeps `progress.txt` up to date, stops when a file named `stop` appears there, and saves the brain (the
network, matchups, deck mastery, card results and the session's record) for the app to reload.

    sim-train background --job <folder> [--root <PokemonTCGSim folder>] [--skip-cpu 2,3]

A ready-built Windows copy is in `../Tools/sim-train/` (see its README for rebuilding it).
