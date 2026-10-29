"""Finds and converts graphics in a Game Boy Advance ROM.

A GBA cartridge has no file system: graphics, maps, text, music and code are packed together, and only the game's
code knows where each thing is. Most games keep their graphics compressed with the GBA BIOS's LZ77 (a 0x10 byte, then
the size), so scanning for streams that decompress cleanly finds most of them. Palettes are usually stored as they
are; find one with an emulator (mGBA: Tools → View palette) and search for its colours with --find.

    python gba_extract.py game.gba                         # scan: every LZ77 block, as .bin and as a grey tile sheet
    python gba_extract.py game.gba --palette 0x3A1B24      # draw the sheets with the 16 colours at that address
    python gba_extract.py game.gba --tiles 0x3C0000 --length 0x800 --palette 0x3A1B24   # uncompressed tiles, by hand
    python gba_extract.py game.gba --find "7FFF 001F 03E0" # where these colours (15-bit values, as an emulator shows) are
    python gba_extract.py game.gba --bpp 8 --tiles-wide 32

Addresses can be ROM offsets (0x3A1B24) or GBA addresses (0x083A1B24: the ROM is mapped at 0x08000000).
Writes into game_extracted/: lz/<offset>.bin (decompressed data), png/<offset>.png and blocks.json.
"""
import argparse
import json
import os
import sys

import romlib


def address(text):
    value = int(text, 0)
    return value - 0x08000000 if value >= 0x08000000 else value


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("rom")
    ap.add_argument("--out", default="")
    ap.add_argument("--bpp", type=int, choices=(4, 8), default=4)
    ap.add_argument("--tiles-wide", type=int, default=16)
    ap.add_argument("--palette", default="", help="offset of a palette (16 colours for 4bpp, 256 for 8bpp)")
    ap.add_argument("--tiles", default="", help="offset of uncompressed tile data to draw (with --length)")
    ap.add_argument("--length", default="0x800", help="bytes of tile data for --tiles")
    ap.add_argument("--find", default="", help="colours to search for: 15-bit hex values separated by spaces")
    ap.add_argument("--min-size", type=int, default=64, help="smallest decompressed block worth keeping (bytes)")
    args = ap.parse_args()
    with open(args.rom, "rb") as f:
        rom = f.read()
    h = romlib.gba_header(rom)
    print(f"{h['title']} ({h['game_code']}), {len(rom) / 1e6:.1f} MB, header checksum {'OK' if h['checksum_ok'] else 'WRONG'}")

    if args.find:
        needle = b"".join(int(v, 16).to_bytes(2, "little") for v in args.find.split())
        at = rom.find(needle)
        hits = []
        while at >= 0 and len(hits) < 50:
            hits.append(at)
            at = rom.find(needle, at + 1)
        print("found at: " + (", ".join(f"0x{a:X} (GBA 0x{a + 0x08000000:08X})" for a in hits) or "nowhere (it may be compressed)"))
        return

    out = args.out or os.path.splitext(args.rom)[0] + "_extracted"
    os.makedirs(os.path.join(out, "png"), exist_ok=True)
    colours = romlib.grey_palette(args.bpp)
    if args.palette:
        colours = romlib.read_palette(rom, address(args.palette), 1 << args.bpp)

    if args.tiles:
        start, length = address(args.tiles), int(args.length, 0)
        w, hgt, rgba = romlib.decode_tiles(rom[start:start + length], args.bpp, colours, args.tiles_wide)
        path = os.path.join(out, "png", f"tiles_{start:06X}.png")
        romlib.write_png(path, w, hgt, rgba)
        print(f"wrote {path} ({w}x{hgt})")
        return

    os.makedirs(os.path.join(out, "lz"), exist_ok=True)
    blocks = []
    pos = 0
    while True:
        pos = rom.find(b"\x10", pos)
        if pos < 0:
            break
        if pos % 4:  # the BIOS needs compressed data aligned to 4 bytes
            pos += 1
            continue
        try:
            data, used = romlib.lz77_decompress(rom, pos, max_size=0x40000)
        except romlib.NotCompressed:
            pos += 4
            continue
        if len(data) >= args.min_size:
            kind = "palette?" if len(data) in (32, 512) else "tiles?" if len(data) % 32 == 0 else "data"
            name = f"{pos:06X}"
            with open(os.path.join(out, "lz", name + ".bin"), "wb") as f:
                f.write(data)
            entry = {"offset": f"0x{pos:X}", "gba_address": f"0x{pos + 0x08000000:08X}", "compressed": used,
                     "size": len(data), "guess": kind}
            if kind == "tiles?":
                w, hgt, rgba = romlib.decode_tiles(data, args.bpp, colours, args.tiles_wide)
                romlib.write_png(os.path.join(out, "png", name + ".png"), w, hgt, rgba)
                entry["png"] = f"png/{name}.png"
            elif kind == "palette?":
                pal = romlib.read_palette(data, 0, len(data) // 2)
                swatch = b"".join(bytes((r, g, b, 255)) for r, g, b in pal)
                romlib.write_png(os.path.join(out, "png", name + "_palette.png"), 16, len(pal) // 16, swatch)
                entry["png"] = f"png/{name}_palette.png"
            blocks.append(entry)
            pos += max(4, (used + 3) & ~3)
        else:
            pos += 4
    with open(os.path.join(out, "blocks.json"), "w") as f:
        json.dump(blocks, f, indent=1)
    tiles = sum(1 for b in blocks if b["guess"] == "tiles?")
    print(f"{len(blocks):,} LZ77 blocks ({tiles:,} look like tiles) written to {out}")
    print("Sheets are grey until you give a palette (--palette); some 'blocks' are false finds (random bytes that happen to"
          " decompress): they look like noise, ignore them.")
    if not blocks:
        print("No compressed graphics found: this game may store them uncompressed (use --tiles with an address from an"
              " emulator's tile viewer).")


if __name__ == "__main__":
    main()
