# sim-train.exe

The command-line trainer (`../../Headless`), built for Windows as one self-contained program: it doesn't need .NET
installed. The app's Train screen uses it for **Train in the background (fastest)**; you can also run it yourself
(`sim-train.exe help` lists the commands and options).

It finds the card data by looking for `Assets/PokemonTCG/Resources` in the folders above it, so keep it here
(`PokemonTCGSim/Tools/sim-train/`), or give it `--root <the PokemonTCGSim folder>`.

**In a game build** (Windows) it comes along by itself: after the build, `Editor/TrainerBuildFiles.cs` copies this program
to `Tools/sim-train/` beside the game, its card data, decklists and starting network to `TrainerData/` (a build packs
`Resources` into Unity's own files, which it can't read), and the GPU learner to `GpuLearner/`. Give someone the whole
build folder and Train in the background works for them too; their AI is saved in their own save folder.

**When you update the project, copy this folder too**, not only the scripts under `Assets`. The app checks
(`sim-train.exe version`) and won't train in the background with a trainer older than itself: an old one can't read
the AI's file as it is now (a bigger network, for one) and would save its own starting network over it.

To rebuild it after changing the sim's code (needs the .NET 8 SDK):

```
cd PokemonTCGSim/Headless
dotnet publish Headless.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o ../Tools/sim-train
```
