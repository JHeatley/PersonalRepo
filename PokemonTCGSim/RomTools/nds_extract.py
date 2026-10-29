"""Unpacks a Nintendo DS ROM: every file of its file system, every NARC archive inside, LZ-compressed files
decompressed, and the 2D graphics (NCGR tiles with their NCLR palette, NSCR screens) converted to PNG for Unity.

    python nds_extract.py game.nds                    # into game_extracted/ next to the ROM
    python nds_extract.py game.nds --out D:/Rips/game --tiles-wide 32
    python nds_extract.py game.nds --only a/0/0/4     # just the files whose path starts with this

Writes:
  files/       the ROM's files as stored (overlays in files/overlay/)
  unpacked/    LZ-compressed files decompressed, and NARC archives unpacked (one folder each, members numbered)
  png/         the graphics converted: each NCGR drawn with the nearest NCLR in the same archive or folder, each NSCR
               (a screen: a background or a menu) drawn with its tiles and palette
  arm9.bin, arm7.bin, header.json, manifest.json (every file, its kind and what became of it)

What this doesn't do: 3D models (NSBMD/NSBTX: Tinke, NitroPaint or apicula convert them to .dae/.gltf), sound (SDAT:
VGMTrans or Tinke turn SSEQ/SWAR into MIDI/SF2/WAV), text (BMG and game-specific formats), and graphics a game
encrypts (Pokémon Diamond/Pearl/Platinum's Pokémon sprites, for one: DSPRE and Tinke know the decryption). The
program itself (arm9.bin, overlays) is machine code: Ghidra with an NDS loader, or a decompilation project, for that.
"""
import argparse
import json
import os
import sys

import romlib


def safe(path):
    return path.replace(":", "_").replace("\\", "/").lstrip("/")


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("rom")
    ap.add_argument("--out", default="")
    ap.add_argument("--tiles-wide", type=int, default=16, help="tiles per row in tile sheets that don't say (default 16)")
    ap.add_argument("--only", default="", help="only files whose path starts with this")
    ap.add_argument("--palette-row", type=int, default=0, help="which 16-colour row of a palette 4bpp sheets use")
    args = ap.parse_args()
    with open(args.rom, "rb") as f:
        rom = f.read()
    if not romlib.looks_like_nds(rom):
        sys.exit("This doesn't look like a DS ROM.")
    out = args.out or os.path.splitext(args.rom)[0] + "_extracted"
    header = romlib.nds_header(rom)
    os.makedirs(out, exist_ok=True)
    with open(os.path.join(out, "header.json"), "w") as f:
        json.dump(header, f, indent=2)
    for cpu in ("arm9", "arm7"):
        c = header[cpu]
        with open(os.path.join(out, cpu + ".bin"), "wb") as f:
            f.write(rom[c["rom_offset"]:c["rom_offset"] + c["size"]])
    print(f"{header['title']} ({header['game_code']}): extracting to {out}")

    manifest = []
    # Containers: a folder of the file system, or one NARC. Graphics are paired within a container.
    containers = {}

    def add(container, index, path, data, entry):
        kind = romlib.nitro_kind(data)
        entry["kind"] = kind
        containers.setdefault(container, []).append((index, path, kind, data, entry))

    for fid, path, data in romlib.nds_files(rom):
        if args.only and not path.startswith(args.only):
            continue
        dest = os.path.join(out, "files", safe(path))
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        with open(dest, "wb") as f:
            f.write(data)
        entry = {"id": fid, "path": path, "size": len(data)}
        manifest.append(entry)
        inner, packed = romlib.maybe_decompress(data)
        if packed:
            entry["lz_decompressed_size"] = len(inner)
            dest = os.path.join(out, "unpacked", safe(path))
            os.makedirs(os.path.dirname(dest), exist_ok=True)
            with open(dest, "wb") as f:
                f.write(inner)
        if romlib.nitro_kind(inner) == "NARC":
            entry["kind"] = "NARC"
            try:
                members = romlib.read_narc(inner)
            except (ValueError, KeyError, TypeError, IndexError) as e:
                entry["error"] = f"couldn't read the NARC: {e}"
                continue
            entry["members"] = len(members)
            folder = os.path.join(out, "unpacked", safe(path) + "_narc")
            os.makedirs(folder, exist_ok=True)
            for i, (name, member) in enumerate(members):
                m_inner, m_packed = romlib.maybe_decompress(member)
                kind = romlib.nitro_kind(m_inner)
                label = name or f"{i:04d}.{(kind or 'bin').lower()}"
                with open(os.path.join(folder, safe(label)), "wb") as f:
                    f.write(m_inner)
                sub = {"path": f"{path}#{i}", "size": len(member), "narc_member": i}
                if m_packed:
                    sub["lz_decompressed_size"] = len(m_inner)
                manifest.append(sub)
                add(path, i, f"{path}#{i}", m_inner, sub)
        else:
            add(os.path.dirname(path), fid, path, inner, entry)

    converted = 0
    for container, items in containers.items():
        palettes = [(i, p, d) for i, p, k, d, e in items if k == "NCLR"]
        graphics = [(i, p, d) for i, p, k, d, e in items if k == "NCGR"]

        def nearest(pool, index):
            return min(pool, key=lambda t: (abs(t[0] - index), t[0] > index)) if pool else None

        for index, path, kind, data, entry in items:
            try:
                if kind == "NCGR":
                    g = romlib.read_ncgr(data)
                    pal = nearest(palettes, index)
                    colours, used = romlib.grey_palette(g["bpp"]), None
                    if pal:
                        _, colours = romlib.read_nclr(pal[2])
                        used = pal[1]
                        if g["bpp"] == 4:
                            colours = colours[args.palette_row * 16:args.palette_row * 16 + 16] or colours[:16]
                    wide = g["tiles_wide"] or args.tiles_wide
                    w, h, rgba = romlib.decode_tiles(g["data"], g["bpp"], colours, wide, linear=g["linear"])
                elif kind == "NSCR":
                    sw, sh, entries = romlib.read_nscr(data)
                    gr, pal = nearest(graphics, index), nearest(palettes, index)
                    if not gr:
                        continue
                    g = romlib.read_ncgr(gr[2])
                    colours = romlib.read_nclr(pal[2])[1] if pal else romlib.grey_palette(g["bpp"]) * 16
                    used = pal[1] if pal else None
                    w, h, rgba = romlib.render_screen(sw, sh, entries, g["data"], colours, g["bpp"])
                    entry["tiles"] = gr[1]
                else:
                    continue
                png = os.path.join(out, "png", safe(path).replace("#", "_") + ".png")
                os.makedirs(os.path.dirname(png), exist_ok=True)
                romlib.write_png(png, w, h, rgba)
                entry["png"] = os.path.relpath(png, out).replace("\\", "/")
                entry["palette"] = used or "none (grey)"
                converted += 1
            except (ValueError, KeyError, IndexError, TypeError, ZeroDivisionError) as e:
                entry["error"] = f"couldn't convert: {e}"

    with open(os.path.join(out, "manifest.json"), "w") as f:
        json.dump(manifest, f, indent=1)
    kinds = {}
    for e in manifest:
        kinds[e.get("kind") or "other"] = kinds.get(e.get("kind") or "other", 0) + 1
    print(f"{len(manifest):,} files and archive members; {converted:,} graphics converted to PNG; kinds: "
          + ", ".join(f"{k} {n}" for k, n in sorted(kinds.items(), key=lambda kv: -kv[1])))
    print("Graphics with the wrong colours used a palette meant for something else: try --palette-row, or pick the palette"
          " by hand in Tinke or NitroPaint.")


if __name__ == "__main__":
    main()
