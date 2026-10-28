# sim-train.exe

The command-line trainer (`../../Headless`), built for Windows as one self-contained program: it doesn't need .NET
installed. The app's Train screen uses it for **Train in the background (fastest)**; you can also run it yourself
(`sim-train.exe help` lists the commands and options).

It finds the card data by looking for `Assets/PokemonTCG/Resources` in the folders above it, so keep it here
(`PokemonTCGSim/Tools/sim-train/`), or give it `--root <the PokemonTCGSim folder>`.

To rebuild it after changing the sim's code (needs the .NET 8 SDK):

```
cd PokemonTCGSim/Headless
dotnet publish Headless.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o ../Tools/sim-train
```
