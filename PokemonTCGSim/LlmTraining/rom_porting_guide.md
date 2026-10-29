# GBA and DS games into Unity: questions and answers

The readable version of `extra/rom_porting.jsonl` (the same content, which `build_dataset.py` adds to the training data). The tools it refers to are in `../RomTools`; the Python and C# examples were run or compiled and checked.

# What's realistic

## Can you convert this .gba or .nds game into a Unity project?

Not by myself, and no tool does it automatically. Here's what's possible and how I can help.

**Why not directly:**
- A ROM is binary data. I read text, so I can't open a 4–512 MB binary file; and if I could, its bytes mean nothing without tools that decode them.
- The game itself is compiled ARM machine code. Turning it back into understandable source code is reverse-engineering: the community decompilation projects for Pokémon Emerald or Platinum took many people years.

**What does work, step by step:**
1. **Extract the data with tools.** `rom_info.py` shows what the ROM is. `nds_extract.py` unpacks a DS game's whole file system and converts its 2D graphics to PNG. `gba_extract.py` finds a GBA game's compressed graphics. Paste me their text output (`rom_info` output, parts of `manifest.json` or `blocks.json`) and I'll explain what each file is and what to do with it.
2. **Use a decompilation project** if one exists for the game (pret/pokeemerald, pokefirered, pokeplatinum, pokeheartgold): it has the graphics as PNGs, maps as JSON and the logic as readable C.
3. **Import the assets into Unity** as pixel-art sprites, slice the tile sheets, and build maps with Tilemaps.
4. **Rewrite the game logic in C#.** I can help port specific systems (movement, battles, menus, map scripts) from the decompilation's C, or design them fresh.

**One thing to keep in mind:** extracting assets from games you own for study, or doing this with homebrew and your own games, is fine. A Unity game you share that contains a commercial game's graphics, music, text or code infringes its copyright, which is a real risk with Nintendo's games.

## Can I give you a ROM file to read?

I can't read binary files: a ROM is millions of bytes of machine code and packed data, not text. What I can read is the text the tools produce:

```
python rom_info.py game.nds --files > info.txt
python nds_extract.py game.nds            # then open game_extracted/manifest.json
python gba_extract.py game.gba            # then open game_extracted/blocks.json
```

Paste me `info.txt`, a part of `manifest.json` or `blocks.json`, a hex dump of a few hundred bytes (`python -c "print(open('game.gba','rb').read()[0xA0:0xC0].hex(' '))"`), or a screenshot of an emulator's tile or palette viewer, and I can tell you what the files and bytes are, which tool converts them, and how to use them in Unity.

## What are the RomTools in this project and what does each do?

`PokemonTCGSim/RomTools` has four Python scripts (standard library only) and a Unity editor script:

| Tool | What it does |
|---|---|
| `rom_info.py game.nds --files` | Header (title, game code, maker, version), checks the header checksum, lists every file in a DS game with its size and kind (NARC, NCGR, NCLR, NSCR, SDAT, NSBMD...) |
| `nds_extract.py game.nds` | Unpacks a DS game: every file (`files/`), LZ-compressed files decompressed and NARC archives unpacked (`unpacked/`), NCGR tiles and NSCR screens converted to PNG with their palettes (`png/`), the ARM9/ARM7 programs, `manifest.json` |
| `gba_extract.py game.gba` | Finds LZ77-compressed blocks in a GBA game and draws them as tile sheets; `--find` locates a palette's colours, `--palette` colours the sheets, `--tiles` draws uncompressed tiles at an address |
| `test_romtools.py` | Tests, on small ROMs it builds itself |
| `Unity/RomImportSettings.cs` | In a Unity project's Editor folder: PNGs under any `RomImport` folder import as crisp pixel-art sprites |

They extract data. Code, 3D models, sound and text need other tools (Ghidra, apicula, VGMTrans, Tinke), listed in the RomTools README.

## nds_extract.py drew graphics with the wrong colours or as noise. Why?

Usually one of these:

- **The wrong palette.** The tool colours each NCGR with the nearest NCLR in the same archive or folder, which is right most of the time but not always (some archives keep all palettes together, or one palette serves many graphics). Try `--palette-row 1`, `2`... for 4bpp graphics whose palette has several 16-colour rows, or pick the palette by hand in Tinke or NitroPaint.
- **The wrong width.** If the NCGR doesn't say how many tiles wide it is, the sheet is drawn 16 tiles across; a 64×64 sprite (8 tiles wide) then looks chopped and rearranged. Use `--tiles-wide 8` (or 4, 32...).
- **Encrypted data.** Some games encrypt graphics; Pokémon Diamond/Pearl/Platinum's Pokémon sprites, for example, come out as noise. Tools that know the game (Tinke's plugins, DSPRE) decrypt them.
- **Not graphics at all, or a game-specific format** that only looks like one. Check the file's kind in `manifest.json`.
- **8bpp or bitmap data.** 256-colour graphics or linear (non-tiled) pixels need the right settings; `read_ncgr` reads the bit depth and the linear flag from the file, but game-specific formats don't have them.

## gba_extract.py found hundreds of blocks. Which ones are real graphics?

The scan tries every 4-byte-aligned `0x10` byte as the start of an LZ77 stream and keeps what decompresses cleanly, so it finds both real data and false matches.

- Open the PNGs: real graphics show recognisable shapes (font letters, map tiles, character frames) even in grey; false finds look like random noise.
- `blocks.json` notes each block's size and a guess: `tiles?` (size a multiple of 32 bytes: whole 4bpp tiles), `palette?` (32 or 512 bytes: 16 or 256 colours), `data` (other: maps, tables, text, sound).
- Some real blocks aren't graphics: tile **maps** (2 bytes per entry: tile number, flips, palette), **metatile** definitions, text, or compressed tables. They look like noise as tiles.
- Graphics in 8bpp (256 colours) look stretched in 4bpp: retry with `--bpp 8`.
- Characters often look jumbled at the default 16 tiles across. A 32×32 sprite is 4 tiles wide, so try `--tiles-wide 4`.

To colour a sheet, find its palette: run the game in mGBA, open **Tools → View palette**, note a few 15-bit colour values of that object's palette, then `python gba_extract.py game.gba --find "7FFF 5294 0000"` and use the found address with `--palette`.

# GBA formats

## What is in a GBA ROM header?

The first 192 bytes (0x00–0xBF) of every GBA ROM:

| Offset | Size | Contents |
|---|---|---|
| 0x00 | 4 | ARM branch instruction to the start of the game's code |
| 0x04 | 156 | Nintendo logo (compressed bitmap; the BIOS checks it) |
| 0xA0 | 12 | Game title (uppercase ASCII) |
| 0xAC | 4 | Game code, e.g. `BPEE` (Pokémon Emerald, USA): first letter = type, two letters = game, last = region (E USA, P Europe, J Japan) |
| 0xB0 | 2 | Maker code (`01` = Nintendo) |
| 0xB2 | 1 | Fixed value 0x96 |
| 0xB3 | 1 | Main unit code (0) |
| 0xB4 | 1 | Device type |
| 0xBC | 1 | Software version |
| 0xBD | 1 | Header checksum |

The checksum covers 0xA0–0xBC: start with 0, subtract each byte, then subtract 0x19, keep the low 8 bits:

```python
def gba_checksum(rom):
    c = 0
    for b in rom[0xA0:0xBD]:
        c = (c - b) & 0xFF
    return (c - 0x19) & 0xFF
```

A real GBA refuses to boot if it's wrong; emulators usually just warn. Unlike the DS, there's nothing after the header that describes the contents: no file table.

## What is the GBA's memory map?

| Address | Size | What |
|---|---|---|
| 0x00000000 | 16 KB | BIOS (system calls: decompression, division, sound helpers) |
| 0x02000000 | 256 KB | EWRAM, external work RAM (slower) |
| 0x03000000 | 32 KB | IWRAM, internal work RAM (fast; hot code is copied here) |
| 0x04000000 | 1 KB | I/O registers (display, sound, DMA, timers, keys) |
| 0x05000000 | 1 KB | Palette RAM: 256 background colours + 256 sprite colours |
| 0x06000000 | 96 KB | VRAM: tiles, tile maps, bitmaps |
| 0x07000000 | 1 KB | OAM: 128 sprites' attributes |
| 0x08000000 | up to 32 MB | Game Pak ROM (also mirrored at 0x0A000000 and 0x0C000000) |
| 0x0E000000 | up to 64 KB | Game Pak SRAM/Flash (save data) |

So a pointer like `0x083A1B24` in the game's code or data means ROM offset `0x3A1B24`. That's the key to following data in a GBA ROM: tables of 4-byte little-endian pointers starting with `0x08` or `0x09` point into the ROM. The screen is 240×160 at about 59.73 frames per second; the CPU is an ARM7TDMI at 16.78 MHz with no floating-point unit.

## How do GBA graphics work: tiles, palettes, backgrounds and sprites?

- **Colours** are 15-bit BGR555 (`0bbbbbgggggrrrrr`): red in bits 0–4, green 5–9, blue 10–14. Palette RAM holds 256 background and 256 sprite colours, used as one 256-colour palette or sixteen 16-colour palettes. Colour 0 is transparent.
- **Tiles** are 8×8 pixels: **4bpp** (32 bytes, two pixels per byte, low nibble = left pixel, each index into a 16-colour palette) or **8bpp** (64 bytes, one byte per pixel into the 256-colour palette).
- **Backgrounds**: modes 0–2 use tile maps. A text-mode map entry is 16 bits: tile number (bits 0–9), horizontal flip (10), vertical flip (11), palette number (12–15). Up to 4 layers (mode 0), some can rotate and scale (affine, modes 1–2). Modes 3–5 are plain bitmaps (16-bit colour or 8-bit indexed).
- **Sprites (objects)**: 128 entries in OAM, 8 bytes each: Y, shape and colour mode (attribute 0); X, size and flips or affine index (1); first tile, priority and palette (2). Sizes run from 8×8 to 64×64. Their tiles live in the upper 32 KB of VRAM.

In Unity: a 4bpp tile sheet with its palette becomes a PNG sprite sheet; sprites are slices of it; backgrounds become Tilemaps (tile number → tile asset, flips → the tile's transform, palette number → a different coloured copy of the tile, or a palette-swap shader).

## How does GBA/DS LZ77 compression work?

The BIOS's LZ77 (type 0x10), used by most GBA games and many DS files:

- **Header** (4 bytes): `0x10`, then the decompressed size (24 bits, little-endian).
- Then groups: one **flag byte** followed by 8 items, read from the flag's highest bit to its lowest. A 0 bit means the item is **one literal byte**. A 1 bit means it's **2 bytes describing a copy**: length = (first byte >> 4) + 3 (3–18 bytes), distance = ((first byte & 0xF) << 8 | second byte) + 1 (1–4096 bytes back in the output).

```python
def lz77_decompress(data, offset=0):
    assert data[offset] == 0x10
    size = int.from_bytes(data[offset + 1:offset + 4], "little")
    out, pos = bytearray(), offset + 4
    while len(out) < size:
        flags = data[pos]; pos += 1
        for bit in range(7, -1, -1):
            if len(out) >= size:
                break
            if flags >> bit & 1:
                b0, b1 = data[pos], data[pos + 1]; pos += 2
                length, disp = (b0 >> 4) + 3, ((b0 & 0xF) << 8 | b1) + 1
                for _ in range(length):
                    out.append(out[-disp])     # byte by byte: a copy can overlap what it's writing
            else:
                out.append(data[pos]); pos += 1
    return bytes(out)
```

The DS adds **LZ11** (type 0x11), with longer copies: the first nibble of a copy chooses a 2-, 3- or 4-byte form (lengths up to 65,808). Other BIOS formats: Huffman (0x24/0x28), run-length (0x30) and diff filters (0x81/0x82). Compressed data must start on a 4-byte boundary, which is why scanners check only aligned `0x10` bytes.

## How is sound and music stored in GBA games?

Most GBA games use Nintendo's sound engine, **MusicPlayer2000** (also called M4A or "Sappy" after a popular tool). The ROM holds:

- a **song table**: entries pointing to each song's header;
- **songs**: tracks of MIDI-like events (notes, tempo, volume, instrument changes);
- **voice groups**: instrument tables pointing to **samples** (8-bit signed PCM, "Direct Sound") or settings for the four Game Boy sound channels (square waves, programmable wave, noise).

Tools: **agbplay** and **Sappy** play and export them, and **GBA Mus Ripper** converts songs to MIDI plus a SoundFont (SF2). Decompilation projects (pokeemerald) already have the songs as MIDI and the samples as `.wav`/`.aif`.

In Unity: render each song to `.ogg` (play the MIDI with its SoundFont in a DAW or with `fluidsynth -ni -F song.wav -r 44100 soundfont.sf2 song.mid`) and import as an `AudioClip`, or play MIDI live with a MIDI-player asset. Sound effects become short WAV clips on `AudioSource`s.

## How do I tell what save type a GBA game uses?

GBA games include a library string that names their save hardware; search the ROM for it:

```python
rom = open("game.gba", "rb").read()
for tag in (b"EEPROM_V", b"SRAM_V", b"SRAM_F_V", b"FLASH_V", b"FLASH512_V", b"FLASH1M_V"):
    if tag in rom:
        print(tag.decode(), "at", hex(rom.find(tag)))
```

- **SRAM** (32 KB, battery-backed) and **FLASH** (64 KB, or 128 KB for FLASH1M) are mapped at 0x0E000000.
- **EEPROM** (512 bytes or 8 KB) is accessed through DMA at the top of the ROM area.

Emulators use this to pick the save type. In a Unity remake you'd design your own save format (JSON or binary in `Application.persistentDataPath`) rather than copy the original layout; if you need to read original saves, the decompilation's save structures describe them.

## What are ARM and Thumb code in GBA and DS games, and how do I read them?

The GBA's ARM7TDMI (and the DS's ARM9 and ARM7) runs two instruction sets:

- **ARM**: 32-bit instructions, more powerful (conditional execution on almost every instruction). Used for speed-critical code, often copied to IWRAM.
- **Thumb**: 16-bit instructions, more compact. Most GBA game code is Thumb, because the ROM bus is 16 bits wide.

A function call is `BL` (branch with link: the return address goes in `LR`), returns are `BX LR` or `POP {..., PC}`, and arguments go in `R0`–`R3`. Data addresses are loaded from "literal pools" after functions (`LDR R0, =0x083A1B24`).

To read it, use **Ghidra** (free, from the NSA): create a project, import the ROM with a GBA loader (for example the community GhidraGBA loader) or, for DS, an NDS loader such as NTRGhidra, and let it analyse. Ghidra's decompiler shows C-like code for each function. Name functions and variables as you figure them out; it's slow, careful work. Emulators with debuggers (mGBA, no$gba, melonDS) let you break on reads and writes to an address, which is the fastest way to find the code that handles something you can see on screen.

# DS formats

## What is in a Nintendo DS ROM?

A `.nds` file has a header (0x000–0x1FF) that describes everything else:

| Offset | Contents |
|---|---|
| 0x000 | Game title (12 bytes), game code (4, e.g. `CPUE` = Pokémon Platinum USA), maker code (2) |
| 0x014 | Cartridge capacity (128 KB << n) |
| 0x020–0x02F | **ARM9** program: ROM offset, entry point, RAM address, size (the main CPU's code) |
| 0x030–0x03F | **ARM7** program: the same for the second CPU (sound, touch screen, Wi-Fi) |
| 0x040 / 0x048 | **FNT** (file name table) and **FAT** (file allocation table): offset and size of each |
| 0x050 / 0x058 | ARM9 and ARM7 overlay tables (code loaded on demand) |
| 0x068 | Icon and title shown on the DS menu |
| 0x15E | CRC-16 of the header (0x000–0x15D) |

Then the **NitroFS** file system: the FAT lists each file's start and end offsets by file ID, and the FNT gives the folder tree and names. So unlike the GBA, a DS game's data is in named files (`a/0/0/4`, `data/graphic/title.narc`), which tools extract exactly. `rom_info.py game.nds --files` lists them.

## How do the DS file name table (FNT) and file allocation table (FAT) work?

- **FAT**: 8 bytes per file ID: start offset and end offset in the ROM (little-endian). File IDs below the first named file are overlays (code).
- **FNT**: starts with a main table, 8 bytes per directory: offset of the directory's entries (from the FNT's start), the ID of its first file, and its parent directory ID (for the root, this field holds the total number of directories). Directory IDs are 0xF000 + index.
- Each directory's entries are a list:
  - a length byte 1–127: a **file** whose name has that many characters (then the name); files get consecutive IDs starting from the directory's first file ID;
  - a length byte 129–255: a **subdirectory**, name length = byte − 128, then the name and its 2-byte directory ID;
  - a 0 byte: end of the directory.

```python
import struct

def walk(fnt, dir_index=0, path="", names=None):
    names = {} if names is None else names
    offset, file_id = struct.unpack_from("<IH", fnt, dir_index * 8)
    pos = offset
    while fnt[pos]:
        n = fnt[pos] & 0x7F
        name = fnt[pos + 1:pos + 1 + n].decode("ascii")
        pos += 1 + n
        if fnt[pos - n - 1] & 0x80:
            child = struct.unpack_from("<H", fnt, pos)[0] & 0x0FFF
            pos += 2
            walk(fnt, child, path + name + "/", names)
        else:
            names[file_id] = path + name
            file_id += 1
    return names
```

NARC archives inside the ROM use the same two tables (as their `BTNF` and `BTAF` sections), often with no names: their files are just numbered.

## What are NARC, NCGR, NCLR, NSCR, NCER and NANR files in DS games?

They're Nintendo's "Nitro" formats. Every one starts with a 16-byte header: a 4-character magic **stored reversed** (an NCGR file starts with `RGCN`), a byte-order mark 0xFEFF, a version, the file size, the header size (16) and the number of sections. Each section is a reversed magic, its size and its data.

| Format | Magic in the file | What it is |
|---|---|---|
| NARC | `NARC` | An archive of files (sections BTAF: offsets, BTNF: names, GMIF: the data) |
| NCLR | `RLCN` | **Palette** (section PLTT): BGR555 colours, 4bpp (16-colour rows) or 8bpp |
| NCGR | `RGCN` | **Tile graphics** (section CHAR): 4bpp or 8bpp 8×8 tiles (or linear pixels), with the size in tiles if known |
| NSCR | `RCSN` | **Screen** / tile map (section SCRN): 16-bit entries (tile, flips, palette row) for a background |
| NCER | `RECN` | **Cells**: how sprite pieces (OAM entries) are arranged into a whole sprite |
| NANR | `RNAN` | **Animations**: sequences of cells with frame durations |
| NSBMD / NSBTX | `BMD0` / `BTX0` | 3D models and their textures |
| NSBCA / NSBTA / NSBTP | `BCA0` / `BTA0` / `BTP0` | 3D animations: skeleton, texture coordinates, texture swaps |
| SDAT | `SDAT` | Sound archive: SSEQ sequences, SBNK instrument banks, SWAR wave archives, STRM streams |
| BMG | `MESGbmg1` | Text messages |

A 2D sprite on screen = NCGR (pixels) + NCLR (colours) + NCER (how the pieces fit) + NANR (how it animates). A background = NCGR + NCLR + NSCR. Many files are additionally LZ10/LZ11-compressed (first byte `0x10` or `0x11`).

## How do I get 3D models and sound out of DS games into Unity?

**3D models (NSBMD, textures in NSBMD/NSBTX):**
- **apicula** (command line) converts NSBMD/NSBTX and their animations to glTF (`.glb`/`.gltf`) or COLLADA (`.dae`).
- NitroPaint and Tinke can also view and export them.
- In Unity, import `.glb` with the **glTFast** package (`com.unity.cloud.gltfast`), or `.dae`/`.fbx` directly. DS textures are small and low-colour: set Filter Mode to **Point** to keep them sharp. The DS draws with vertex colours and simple lighting, so a basic unlit or vertex-colour shader looks closest.

**Sound (SDAT):**
- **VGMTrans** opens the ROM or the SDAT and exports sequences as **MIDI** and instrument banks as **SF2/DLS** SoundFonts.
- **Tinke** extracts **SWAR** samples and **STRM** streams as WAV.
- Render the MIDI with its SoundFont to WAV/OGG (`fluidsynth -ni -F song.wav -r 44100 bank.sf2 song.mid`), then import into Unity as `AudioClip`s; streams and sound effects import as-is.

**Text (BMG and game-specific formats):** Tinke or game-specific editors (DSPRE for Pokémon Gen 4) export it; in Unity keep it in your own format (JSON, CSV or ScriptableObjects) for localisation.

## What are the DS's screens, memory and hardware like, for planning a Unity remake?

- **Two screens**, each 256×192 pixels, the bottom one touch-sensitive (resistive, one point). In Unity: two cameras with viewport rects (top half and bottom half, or side by side), and the touch screen as mouse/touch input on the bottom camera.
- **CPUs**: ARM9 at 67 MHz (the game) and ARM7 at 33 MHz (sound, touch, wireless). **4 MB** main RAM at 0x02000000, 656 KB of VRAM in banks.
- **2D engines**: each screen has GBA-like tiled backgrounds and up to 128 sprites, with more colours (extended palettes) and larger VRAM.
- **3D engine**: about 2,048 polygons per frame at 60 fps, 512×256 maximum textures, fixed-point maths, no texture filtering by default. Models are tiny by modern standards.
- **Sound**: 16 channels of samples or PSG tones, sequenced (SDAT).

A faithful remake usually renders at 256×192 per screen with a **Pixel Perfect Camera** (URP 2D: add the Pixel Perfect Camera component, reference resolution 256×192, assets at 16 or 32 pixels per unit) and scales up by whole numbers, so pixels stay square and crisp.

# Into Unity

## How do I import extracted GBA/DS graphics into Unity so they look right?

1. Put `RomTools/Unity/RomImportSettings.cs` in an `Editor` folder, and the PNGs under `Assets/RomImport/`. Everything there imports with:
   - Texture Type **Sprite**, Sprite Mode **Multiple** (sheets);
   - Filter Mode **Point** (no blurring);
   - Compression **None** (exact colours);
   - no mipmaps;
   - **16 Pixels Per Unit**.
   Without the script, set those by hand in the Inspector.
2. **Slice** each sheet: Sprite Editor → Slice → **Grid By Cell Size** (8×8 for map tiles; 16×16, 32×32 or 64×64 for characters) → Apply. Unity names the slices `sheet_0`, `sheet_1`... in reading order, so slice index = tile index.
3. **Maps**: Window → 2D → **Tile Palette**, create a palette, drag the sliced tiles in, paint on a Tilemap (GameObject → 2D Object → Tilemap → Rectangular). Or build the Tilemap from the original map data with a script.
4. **Characters**: select a walk cycle's frames in the Project window and drag them into the Scene: Unity makes a GameObject with a `SpriteRenderer`, an `Animator` and an animation clip.
5. **Camera**: an orthographic camera with a **Pixel Perfect Camera** (reference resolution 240×160 for GBA, 256×192 per DS screen, same Pixels Per Unit as the sprites) keeps pixels square at any window size.

## How do I build a Unity Tilemap from a GBA or DS tile map?

GBA and DS tile maps (GBA backgrounds, DS NSCR files, many games' map files) are arrays of 16-bit entries: tile number (bits 0–9), horizontal flip (bit 10), vertical flip (bit 11), palette row (bits 12–15). This component turns one into a Tilemap:

```csharp
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>Builds a Tilemap from GBA/DS-style map entries: tile number (bits 0-9), flips (10, 11), palette (12-15).</summary>
public class RetroTilemapBuilder : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite[] tileSprites;      // the 8x8 slices of the tile sheet, in order (Sprite Editor: Slice → Grid By Cell Size 8x8)
    public TextAsset mapData;         // the map's raw entries: 2 bytes each, little-endian, row by row
    public int widthInTiles = 32;

    Tile[] tiles;

    [ContextMenu("Build")]
    public void Build()
    {
        tiles = new Tile[tileSprites.Length];
        for (int i = 0; i < tileSprites.Length; i++)
        {
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            tiles[i].sprite = tileSprites[i];
        }
        tilemap.ClearAllTiles();
        byte[] bytes = mapData.bytes;
        for (int i = 0; i + 1 < bytes.Length; i += 2)
        {
            int entry = bytes[i] | bytes[i + 1] << 8;
            int index = entry & 0x3FF;
            bool flipX = (entry & 0x400) != 0, flipY = (entry & 0x800) != 0;
            int n = i / 2;
            // Unity's y goes up; the console's rows go down.
            var cell = new Vector3Int(n % widthInTiles, -(n / widthInTiles), 0);
            if (index >= tiles.Length) continue;
            tilemap.SetTile(cell, tiles[index]);
            if (flipX || flipY)
            {
                tilemap.SetTileFlags(cell, TileFlags.None);   // tiles lock their transform unless told otherwise
                tilemap.SetTransformMatrix(cell, Matrix4x4.Scale(new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1)));
            }
        }
    }
}
```

Set it up: a Grid with a Tilemap child, the sheet sliced 8×8 (`tileSprites`: select all slices and drag them onto the array), the map's raw bytes as a `TextAsset` (rename the file to `.bytes` so Unity imports it as binary), and the map's width in tiles. Then right-click the component → **Build**.

Palette rows: a 4bpp map can use different palette rows per tile; with one sheet drawn in one palette, those tiles show the wrong colours. Either export the sheet once per palette row and pick the sprite by palette, or use a palette-swap shader. The `Grid`'s cell size should be 1 unit per 8 pixels: with 8×8 tiles, use 8 Pixels Per Unit for the tile sprites, or set the Grid's cell size to 0.5 at 16 Pixels Per Unit.

## How do I recreate palette swapping (like shiny Pokémon or team colours) in Unity?

Old consoles draw sprites with indexed colours, so changing the palette recolours everything at once. To do the same in Unity:

1. **Keep the colour indices in the texture**: export the sprite in **greyscale with the index as the value** (index × 16 for 4bpp, so index 3 = grey value 48), Point filtering, no compression.
2. **Put each palette in a tiny texture**: 16×1 pixels (or 16×N for N palettes), Point filtering, no compression.
3. **A shader** reads the index from the sprite and looks the colour up in the palette (Shader Graph: Sample Texture 2D the sprite → take R → multiply by 255/16 to get the index → add 0.5 and divide by 16 (the middle of that palette pixel) → use as the U coordinate to sample the palette; or HLSL):

```hlsl
float4 frag (v2f i) : SV_Target
{
    float index = tex2D(_MainTex, i.uv).r * 255.0 / 16.0;    // 0..15
    float4 colour = tex2D(_Palette, float2((index + 0.5) / 16.0, (_Row + 0.5) / _Rows));
    colour.a *= index > 0.5 ? 1 : 0;                          // index 0 is transparent
    return colour;
}
```

Changing `_Palette` or `_Row` on a material (or with a `MaterialPropertyBlock` per sprite) recolours instantly: shiny forms, team colours, damage flashes, day/night tints. The simpler alternative is to export each colour variant as its own PNG; that's fine for a few variants.

## How do I make a Unity camera and timing that feel like a GBA or DS game?

**Resolution and pixels:**
- URP 2D: add **Pixel Perfect Camera** to the camera, reference resolution 240×160 (GBA) or 256×192 (one DS screen), Assets Pixels Per Unit = your sprites' (16), and tick **Upscale Render Texture** for the true low-resolution look.
- Built-in pipeline: the **2D Pixel Perfect** package's component does the same.

**Timing:**
- The GBA runs at about 59.73 frames per second, the DS at about 59.83. Games count logic in **frames** (a walk step takes 16 frames, say).
- In Unity, run game logic in a fixed tick so behaviour matches the original: set Edit → Project Settings → Time → Fixed Timestep to `0.01674` (1/59.73) and do logic in `FixedUpdate`, or keep your own frame counter in `Update` with an accumulator.
- Smooth visuals can still run at the monitor's rate.

**Input:** map the D-pad, A, B, L, R, Start and Select (plus touch for DS) with the Input System's actions, so keyboards and gamepads both work.

# Game logic

## How do I convert a GBA or DS game's code to C# for Unity?

There's no automatic converter from machine code to working C#. The realistic routes:

1. **A decompilation project** (best, where it exists). pret's **pokeemerald**, **pokefirered**, **pokeruby** (GBA, complete) and **pokeplatinum**, **pokeheartgold**, **pokediamond** (DS, in progress) are the games' logic as readable C, with names. Port one system at a time to C#: movement, then the map scripts, the menus, the battle engine. I can help translate specific C functions and structures.
2. **Reverse-engineer it yourself** with Ghidra and an emulator debugger, function by function. This is very slow for a whole game, but fine for one mechanic you want to replicate exactly (a damage formula, an RNG, a movement rule).
3. **Observe and recreate**: play the original, measure (frame counts, speeds, formulas from community documentation like Bulbapedia's mechanics pages), and design it fresh in C#. This is how most remakes and fan games are actually built, and it gives cleaner code than a line-by-line port.

Porting notes:
- C `struct`s become C# `struct`s or classes.
- Fixed-point maths (these CPUs have no floating point: a `s16` with 8 fractional bits means value / 256) becomes `float`, unless exact results matter, in which case keep the integers.
- `u8`/`u16`/`s32` become `byte`/`ushort`/`int`, keeping their overflow behaviour (`unchecked`).
- Callbacks and the game's task system become C# state machines or coroutines.
- The game's scripting bytecode (map events) becomes your own script or event system.

## How do I port C code from a decompilation project to C#? Show an example.

Take the Gen 3 Pokémon random number generator from the decompilation (a linear congruential generator):

```c
// C (pokeemerald, simplified)
u32 gRngValue;

u16 Random(void)
{
    gRngValue = 1103515245 * gRngValue + 24691;
    return gRngValue >> 16;
}
```

In C#:

```csharp
public sealed class GbaRng
{
    uint state;
    public GbaRng(uint seed) { state = seed; }

    public ushort Next()
    {
        unchecked { state = 1103515245u * state + 24691u; }   // wraps at 32 bits, like the C u32
        return (ushort)(state >> 16);
    }
}
```

(1103515245 = 0x41C64E6D and 24691 = 0x6073, the constants of the Gen 3 and Gen 4 games.)

What carries over:
- **Integer types and overflow**: C's `u32` wraps silently; C# throws in `checked` code, so mark arithmetic `unchecked` (it's the default unless the project enables checking) and use `uint`/`ushort` for unsigned types.
- **Integer division** truncates in both languages, so it matches; but `>>` on negative signed numbers differs in subtle cases (C's is implementation-defined), so test.
- **Globals** (`gRngValue`) become fields of a class you pass around (or a singleton).
- **Arrays of structs** become `struct[]` or `List<T>`; **pointers into arrays** become indices.
- **Tables in ROM** (`const struct SpeciesInfo gSpeciesInfo[]`) become ScriptableObjects or JSON loaded at start.

Check a port by running both on the same inputs and comparing outputs: the decompilation builds a ROM you can test in an emulator.

## Could I just run the original ROM inside Unity with an emulator instead?

Technically yes: an emulator core can run inside Unity and draw to a texture. For example, a **libretro** core (mGBA for GBA, melonDS for DS) loaded as a native plugin: you feed it the ROM and button presses each frame, and it hands back the frame's pixels (copied into a `Texture2D`) and audio samples (fed to an `AudioSource` via `OnAudioFilterRead`).

But that's *playing* the game in Unity, not converting it: you can't change the game's content or rules (beyond memory hacks), and you still need the ROM at runtime. Distributing a game that includes a commercial ROM is copyright infringement, and emulator cores have their own licences (many are GPL, which affects how you can distribute your program).

It's useful for personal tools (an overlay, an AI that plays the game, a training environment for a neural network) rather than as a way to make a new game. For a game of your own, extract or recreate the assets and write the logic in C#.

## What's a sensible plan for a Unity fan game inspired by a GBA Pokémon game?

1. **Decide what you're making.** A new game in that style, with your own or properly licensed art and music, is something you can share. A copy of the original with its assets is not.
2. **Study the original's systems** with the decompilation (pokeemerald) and community documentation: movement on a 16-pixel grid, map connections, tall-grass encounters, the battle formulas, menus.
3. **Set up Unity for pixel art**: URP 2D, Pixel Perfect Camera at 240×160, 16 Pixels Per Unit, Point filtering, a fixed logic tick of about 60 frames per second.
4. **Build the core loop first**: a player on a Tilemap with grid movement and collision, a camera that follows, a text box, and map transitions.
5. **Data-driven content**: species, moves, items and trainers as ScriptableObjects or JSON, and maps as Tilemaps with trigger objects for warps, NPCs and scripted events.
6. **Then systems one at a time**: encounters, the battle state machine (turn order, damage formula, status effects), the party and bag menus, saving to `Application.persistentDataPath`.
7. **Placeholder art first**, final art later, so the game works before it looks finished.

Port exact mechanics from the decompilation where they matter (damage, catch rates, RNG); design the rest to suit your game.
