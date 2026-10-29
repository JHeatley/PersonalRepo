"""Tests for the ROM tools, on small GBA and DS ROMs built here (no real games needed).

    python test_romtools.py
"""
import json
import os
import struct
import subprocess
import sys
import tempfile
import zlib

import romlib

HERE = os.path.dirname(os.path.abspath(__file__))


# ---------------------------------------------------------------------------------------------------- Compressors (tests only)


def lz77_compress(data, kind=0x10):
    """A simple greedy LZ77 (0x10) or LZ11 (0x11) compressor, to make test data the decompressor must read back."""
    out = bytearray([kind]) + len(data).to_bytes(3, "little")
    pos = 0
    max_len = 18 if kind == 0x10 else 0x10110
    while pos < len(data):
        flags_at = len(out)
        out.append(0)
        flags = 0
        for bit in range(7, -1, -1):
            if pos >= len(data):
                break
            best_len, best_disp = 0, 0
            for disp in range(1, min(pos, 4096) + 1):
                length = 0
                while length < max_len and pos + length < len(data) and data[pos + length - disp] == data[pos + length]:
                    length += 1
                if length > best_len:
                    best_len, best_disp = length, disp
            if best_len >= 3:
                flags |= 1 << bit
                d = best_disp - 1
                if kind == 0x10:
                    out += bytes(((best_len - 3) << 4 | d >> 8, d & 0xFF))
                elif best_len <= 0x10:
                    out += bytes(((best_len - 1) << 4 | d >> 8, d & 0xFF))
                elif best_len <= 0x110:
                    n = best_len - 0x11
                    out += bytes((n >> 4, (n & 15) << 4 | d >> 8, d & 0xFF))
                else:
                    n = best_len - 0x111
                    out += bytes((1 << 4 | n >> 12, n >> 4 & 0xFF, (n & 15) << 4 | d >> 8, d & 0xFF))
                pos += best_len
            else:
                out.append(data[pos])
                pos += 1
        out[flags_at] = flags
    while len(out) % 4:
        out.append(0)
    return bytes(out)


# ---------------------------------------------------------------------------------------------------- Test data


def tile_data(n_tiles=8):
    """4bpp tiles: tile t is filled with colour index (t % 15) + 1, with a single pixel of colour 0 in its top-left."""
    out = bytearray()
    for t in range(n_tiles):
        c = (t % 15) + 1
        tile = bytearray([c | c << 4] * 32)
        tile[0] = c << 4  # pixel (0,0) = 0 (transparent), pixel (1,0) = c
        out += tile
    return bytes(out)


PALETTE = [0x0000, 0x001F, 0x03E0, 0x7C00, 0x7FFF] + [i * 0x0421 for i in range(11)]  # black, red, green, blue, white...


def palette_bytes(values):
    return b"".join(struct.pack("<H", v) for v in values)


def nitro(magic, sections):
    body = b"".join(kind + struct.pack("<I", 8 + len(data)) + data for kind, data in sections)
    return magic + struct.pack("<HHIHH", 0xFEFF, 0x0100, 16 + len(body), 16, len(sections)) + body


def make_nclr(values):
    data = palette_bytes(values)
    return nitro(b"RLCN", [(b"TTLP", struct.pack("<HHIII", 3, 0, 0, len(data), 0x10) + data)])


def make_ncgr(tiles, tiles_wide):
    n = len(tiles) // 32
    head = struct.pack("<HHIIIII", n // tiles_wide, tiles_wide, 3, 0, 0, len(tiles), 0x18)
    return nitro(b"RGCN", [(b"RAHC", head + tiles)])


def make_nscr(width, height, entries):
    data = b"".join(struct.pack("<H", e) for e in entries)
    return nitro(b"RCSN", [(b"NRCS", struct.pack("<HHII", width, height, 0, len(data)) + data)])


def make_narc(members):
    offsets, body = [], b""
    for m in members:
        offsets.append((len(body), len(body) + len(m)))
        body += m + b"\xff" * (-len(m) % 4)
    btaf = struct.pack("<HH", len(members), 0) + b"".join(struct.pack("<II", s, e) for s, e in offsets)
    btnf = struct.pack("<IHH", 8, 0, 1)
    return nitro(b"NARC", [(b"BTAF", btaf), (b"BTNF", btnf), (b"GMIF", body)])


def make_gba(path):
    rom = bytearray(0x10000)
    rom[0xA0:0xAC] = b"TESTGAME\x00\x00\x00\x00"
    rom[0xAC:0xB0], rom[0xB0:0xB2], rom[0xB2] = b"BTST", b"01", 0x96
    check = 0
    for b in rom[0xA0:0xBD]:
        check = (check - b) & 0xFF
    rom[0xBD] = (check - 0x19) & 0xFF
    rom[0x1000:0x1000 + 32] = palette_bytes(PALETTE)
    packed = lz77_compress(tile_data(8), 0x10)
    rom[0x2000:0x2000 + len(packed)] = packed
    with open(path, "wb") as f:
        f.write(rom)


def make_nds(path):
    pal = make_nclr(PALETTE)
    tiles = make_ncgr(tile_data(8), 4)
    screen = make_nscr(16, 16, [0, 1 | 1 << 10, 2 | 1 << 11, 3])  # 2x2 tiles, the second flipped across, the third upside down
    narc = make_narc([pal, lz77_compress(tiles, 0x11)])
    files = [("data/sprites.narc", narc), ("data/bg/title.nclr", pal), ("data/bg/title.ncgr", tiles),
             ("data/bg/title.nscr", screen)]
    # File name table: root (dir 0) holds folder "data" (dir 1), which holds sprites.narc and folder "bg" (dir 2).
    def entries(items):
        out = b""
        for name, dir_id in items:
            out += bytes([len(name) | (0x80 if dir_id is not None else 0)]) + name.encode()
            if dir_id is not None:
                out += struct.pack("<H", 0xF000 | dir_id)
        return out + b"\x00"
    sub0 = entries([("data", 1)])
    sub1 = entries([("sprites.narc", None), ("bg", 2)])
    sub2 = entries([("title.nclr", None), ("title.ncgr", None), ("title.nscr", None)])
    main = 24
    fnt = (struct.pack("<IHH", main, 0, 3) + struct.pack("<IHH", main + len(sub0), 0, 0xF000)
           + struct.pack("<IHH", main + len(sub0) + len(sub1), 1, 0xF001) + sub0 + sub1 + sub2)
    rom = bytearray(0x4000)
    rom[0:12], rom[12:16], rom[16:18] = b"TESTDS\x00\x00\x00\x00\x00\x00", b"ATST", b"01"
    pos = 0x4000
    blobs, fat = b"", b""
    for _, data in files:
        start = pos + len(blobs)
        blobs += data + b"\xff" * (-len(data) % 4)
        fat += struct.pack("<II", start, start + len(data))
    rom += blobs
    fnt_at = len(rom)
    rom += fnt + b"\xff" * (-len(fnt) % 4)
    fat_at = len(rom)
    rom += fat
    struct.pack_into("<IIII", rom, 0x40, fnt_at, len(fnt), fat_at, len(fat))
    struct.pack_into("<IIII", rom, 0x20, 0x200, 0x02000000, 0x02000000, 0x100)
    struct.pack_into("<H", rom, 0x15E, romlib.crc16(rom[:0x15E]))
    with open(path, "wb") as f:
        f.write(rom)


def read_png(path):
    """(width, height, rgba) of a PNG this toolkit wrote (8-bit RGBA, filter 0)."""
    with open(path, "rb") as f:
        png = f.read()
    width, height = struct.unpack(">II", png[16:24])
    pos, idat = 8, b""
    while pos < len(png):
        length, kind = struct.unpack(">I4s", png[pos:pos + 8])
        if kind == b"IDAT":
            idat += png[pos + 8:pos + 8 + length]
        pos += 12 + length
    raw = zlib.decompress(idat)
    rows = [raw[y * (width * 4 + 1) + 1:(y + 1) * (width * 4 + 1)] for y in range(height)]
    return width, height, b"".join(rows)


def pixel(img, x, y):
    w, _, rgba = img
    return tuple(rgba[(y * w + x) * 4:(y * w + x) * 4 + 4])


# ---------------------------------------------------------------------------------------------------- Tests


def run(*args):
    return subprocess.run([sys.executable, *args], cwd=HERE, capture_output=True, text=True, check=True).stdout


def main():
    fails = []

    def check(cond, what):
        print(("ok    " if cond else "FAIL  ") + what)
        if not cond:
            fails.append(what)

    data = os.urandom(3000) + bytes(2000) + b"abc" * 500
    for kind in (0x10, 0x11):
        check(romlib.lz77_decompress(lz77_compress(data, kind))[0] == data, f"LZ{'77' if kind == 0x10 else '11'} round trip")
    check(romlib.bgr555(0x7FFF) == (255, 255, 255) and romlib.bgr555(0x001F) == (255, 0, 0), "BGR555 colours")

    with tempfile.TemporaryDirectory() as tmp:
        gba = os.path.join(tmp, "test.gba")
        make_gba(gba)
        info = run("rom_info.py", gba)
        check("TESTGAME" in info and "checksum OK" in info, "rom_info reads the GBA header and its checksum")
        found = run("gba_extract.py", gba, "--find", "001F 03E0")
        check("0x1002" in found, "gba_extract --find locates colours")
        run("gba_extract.py", gba, "--palette", "0x08001000")
        blocks = json.load(open(os.path.join(tmp, "test_extracted", "blocks.json")))
        check(any(b["offset"] == "0x2000" and b["size"] == 256 for b in blocks), "the LZ77 tiles are found at 0x2000")
        img = read_png(os.path.join(tmp, "test_extracted", "png", "002000.png"))
        check(pixel(img, 0, 0)[3] == 0, "colour 0 is transparent")
        check(pixel(img, 1, 0) == (255, 0, 0, 255), "tile 0 uses palette colour 1 (red)")
        check(pixel(img, 9, 1) == (0, 255, 0, 255), "tile 1 uses palette colour 2 (green)")

        nds = os.path.join(tmp, "test.nds")
        make_nds(nds)
        info = run("rom_info.py", nds, "--files")
        check("TESTDS" in info and "checksum OK" in info and "data/bg/title.nscr" in info, "rom_info reads the DS header and files")
        run("nds_extract.py", nds)
        out = os.path.join(tmp, "test_extracted")
        manifest = json.load(open(os.path.join(out, "manifest.json")))
        paths = {m["path"]: m for m in manifest}
        check("data/sprites.narc#1" in paths and paths["data/sprites.narc#1"].get("lz_decompressed_size"),
              "the NARC is unpacked and its LZ11 member decompressed")
        check(os.path.exists(os.path.join(out, "files", "data", "bg", "title.ncgr")), "files keep their paths")
        sprite = read_png(os.path.join(out, paths["data/sprites.narc#1"]["png"]))
        check(sprite[0] == 32 and sprite[1] == 16, "the NCGR's 4x2 tiles become a 32x16 PNG")
        check(pixel(sprite, 1, 0) == (255, 0, 0, 255) and pixel(sprite, 17, 0) == (0, 0, 255, 255),
              "NCGR drawn with the NCLR from the same archive")
        screen = read_png(os.path.join(out, paths["data/bg/title.nscr"]["png"]))
        check(screen[0] == 16 and screen[1] == 16, "the NSCR becomes a 16x16 picture")
        check(pixel(screen, 1, 0) == (255, 0, 0, 255), "screen tile 0 as stored")
        check(pixel(screen, 15, 0)[3] == 0 and pixel(screen, 14, 0) == (0, 255, 0, 255), "screen tile 1 flipped across")
        check(pixel(screen, 0, 15)[3] == 0 and pixel(screen, 1, 15) == (0, 0, 255, 255), "screen tile 2 upside down")

    print("all passed" if not fails else f"{len(fails)} failed")
    sys.exit(1 if fails else 0)


if __name__ == "__main__":
    main()
