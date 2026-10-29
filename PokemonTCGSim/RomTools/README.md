# RomTools: reading GBA and DS games, and getting their graphics into Unity

Python scripts (standard library only) that read Game Boy Advance (`.gba`) and Nintendo DS (`.nds`) ROM files, unpack
what can be unpacked, and turn 2D graphics into PNGs that Unity imports as pixel-art sprites.

**What this is, and what it isn't.** No tool, and no AI, turns a ROM into a finished Unity project. A ROM is a compiled
program plus packed data: these tools extract the *data* (files, archives, tiles, palettes, screens); the *game*
(its rules, menus, battles, map scripts) is machine code that has to be understood and rewritten in C#. For several
Pokémon games, the community has already turned that machine code back into readable C (the "decompilation"
projects, below), which is the best reference for rewriting game logic. Your local LLM can explain the formats, read
these tools' output (`manifest.json`, `blocks.json`) and help plan and write the C#.

**Legal note.** Use these on homebrew, your own projects, and games you own for study. Putting a commercial game's
graphics, music, text or code into a game you share is copyright infringement, whatever tool extracted it.

## The tools

```
python rom_info.py game.nds --files      # header, checksum, every file in the DS file system and its kind
python rom_info.py game.gba
python nds_extract.py game.nds           # unpack everything, convert 2D graphics to PNG  → game_extracted/
python gba_extract.py game.gba           # find LZ77-compressed graphics, draw them as tile sheets
python gba_extract.py game.gba --find "7FFF 001F 03E0"          # locate a palette seen in an emulator
python gba_extract.py game.gba --palette 0x083A1B24             # draw the sheets with that palette
python gba_extract.py game.gba --tiles 0x3C0000 --length 0x800 --palette 0x3A1B24   # uncompressed tiles by hand
python test_romtools.py                  # the tests (they build small ROMs; no game needed)
```

### Nintendo DS (`nds_extract.py`)

A DS cartridge has a real file system (NitroFS), so extraction is exact:

* `files/`: every file as stored, with its path; `overlay/` holds code overlays.
* `unpacked/`: LZ77/LZ11-compressed files decompressed; NARC archives unpacked into `<name>_narc/` folders.
* `png/`: **NCGR** tile graphics drawn with the nearest **NCLR** palette in the same archive or folder, and **NSCR**
  screens (backgrounds, menus) drawn from their tile map. If the colours are wrong, the palette belongs to something
  else: try `--palette-row`, or choose the palette by hand in Tinke or NitroPaint.
* `arm9.bin`, `arm7.bin`: the two processors' programs (machine code), `header.json`, and `manifest.json` (every file, its
  kind, and what became of it).

Not converted here: 3D models (**NSBMD/NSBTX**: apicula, Tinke or NitroPaint export them to `.dae`/`.glb`), sound
(**SDAT**: VGMTrans exports sequences to MIDI and instruments to SF2; Tinke extracts WAVs), text (**BMG** and
game-specific formats), cell/animation data (**NCER/NANR**: Tinke or NitroPaint), and graphics a game encrypts
(Pokémon Diamond/Pearl/Platinum's Pokémon sprites; DSPRE and Tinke can decrypt them).

### Game Boy Advance (`gba_extract.py`)

A GBA cartridge has **no file system**. The game's code knows where everything is; a tool can only search. Most games
compress graphics with the BIOS's **LZ77** (a `0x10` byte, then the size), so the scan finds streams that decompress
cleanly and draws each as a tile sheet. Some finds are false (random bytes that happen to decompress): they look like
noise. Sheets are grey until you give a palette; palettes are usually stored uncompressed, so look one up in an emulator
(mGBA: *Tools → View palette*, which shows the 15-bit values) and find it with `--find`.

Better than searching, where it exists: a **decompilation** project lays the whole game out as source code and
files, with every graphic as a PNG and every map as JSON. For Pokémon: **pret/pokeemerald**, **pokefirered**,
**pokeruby** (GBA, complete), **pokeplatinum**, **pokeheartgold**, **pokediamond** (DS, in progress). Each project's README
says how to build it (the DS ones need your own ROM for some of their data); their graphics and data folders are what
you'd want for Unity, already organised and named.

## Into Unity

1. Copy `Unity/RomImportSettings.cs` into an `Editor` folder of your Unity project.
2. Copy the PNGs into `Assets/RomImport/` (any subfolders). They import as crisp pixel-art sprites: point filtering,
   no compression, no mipmaps, 16 pixels per unit (`RomImportSettings.PixelsPerUnit`).
3. For tile sheets: Sprite Editor → **Slice → Grid By Cell Size** (8×8 for map tiles; 16×16, 32×32 or 64×64 for
   characters), then drag the slices into a **Tile Palette** (Window → 2D → Tile Palette) and paint on a **Tilemap**.
   For characters, select the frames of a walk cycle and drag them into the Scene to make an animation.
4. Rebuild the game itself in C#: maps from the tiles, characters with `SpriteRenderer` and `Animator`, the rules and
   menus written fresh, using the decompilation (where there is one) as the reference for how the original worked.

## Tools that go further

| Need | Tools |
|---|---|
| Browse and convert DS files | Tinke, NitroPaint, DSPRE (Pokémon Gen 4), Kiwi.DS |
| 3D models from DS games | apicula (→ glTF/DAE), NitroPaint |
| Sound | VGMTrans (MIDI + SF2), Tinke (WAV) |
| Look inside a running game | mGBA, melonDS, no$gba (tile, palette, map and memory viewers; debuggers) |
| Machine code | Ghidra (with the GBA or NDS loader), IDA, the decompilation projects |
| GBA maps (Pokémon) | Porymap (with the decompilation), AdvanceMap |
