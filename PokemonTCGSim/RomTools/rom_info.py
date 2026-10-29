"""What's in a .gba or .nds file: the cartridge header (title, game code, maker, version, checksum) and, for DS games,
the file system (every file's path, size and kind). Only reads; changes nothing.

    python rom_info.py game.nds
    python rom_info.py game.nds --files       # also list every file in the DS file system
    python rom_info.py game.gba
"""
import argparse
import collections
import os
import sys

import romlib


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("rom")
    ap.add_argument("--files", action="store_true", help="list every file (DS)")
    args = ap.parse_args()
    with open(args.rom, "rb") as f:
        rom = f.read()

    if args.rom.lower().endswith(".nds") or romlib.looks_like_nds(rom) and not romlib.looks_like_gba(rom):
        h = romlib.nds_header(rom)
        print(f"Nintendo DS: {h['title']!r}, game code {h['game_code']}, maker {h['maker_code']}, version {h['version']}")
        print(f"  size {h['size'] / 1e6:.1f} MB (cartridge {h['capacity'] / 1e6:.0f} MB); header checksum "
              f"{'OK' if h['header_crc_ok'] else 'WRONG (modified or damaged)'}")
        for cpu in ("arm9", "arm7"):
            c = h[cpu]
            print(f"  {cpu.upper()} program: {c['size']:,} bytes at ROM 0x{c['rom_offset']:X}, loaded to 0x{c['ram_address']:08X}, "
                  f"starts at 0x{c['entry']:08X}")
        kinds, total = collections.Counter(), 0
        files = list(romlib.nds_files(rom))
        for fid, path, data in files:
            inner = romlib.maybe_decompress(data)[0] if len(data) < 4_000_000 else data
            kind = romlib.nitro_kind(data) or (romlib.nitro_kind(inner) and romlib.nitro_kind(inner) + " (LZ)") \
                or os.path.splitext(path)[1].lstrip(".").lower() or "?"
            kinds[kind] += 1
            total += len(data)
            if args.files:
                print(f"  {fid:5d}  {len(data):10,}  {kind:12s}  {path}")
        print(f"  file system: {len(files):,} files, {total / 1e6:.1f} MB; by kind: "
              + ", ".join(f"{k} {n}" for k, n in kinds.most_common(15)))
    elif romlib.looks_like_gba(rom) or args.rom.lower().endswith(".gba"):
        h = romlib.gba_header(rom)
        print(f"Game Boy Advance: {h['title']!r}, game code {h['game_code']}, maker {h['maker_code']}, version {h['version']}")
        print(f"  size {h['size'] / 1e6:.1f} MB; header checksum {'OK' if h['checksum_ok'] else 'WRONG (modified or damaged)'}"
              f"{'' if h['fixed_96h'] else '; the fixed byte at 0xB2 is not 0x96'}")
        print("  A GBA cartridge has no file system: code, graphics, maps, text and music are packed together, found by the")
        print("  game's own code. gba_extract.py finds the compressed graphics; the rest needs a disassembler (Ghidra) or the")
        print("  game's decompilation project, if there is one.")
    else:
        sys.exit("Not a GBA or DS ROM (or a damaged one).")


if __name__ == "__main__":
    main()
