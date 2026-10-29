"""Reading Game Boy Advance and Nintendo DS files: headers, the DS file system and archives, the BIOS compression
formats, palettes, tiles and the DS graphics formats, and writing PNGs. Only Python's standard library is needed.

Layouts follow GBATEK (problemkaputt.de/gbatek.htm) and the community's documentation of the Nitro formats.
"""
import struct
import zlib

# ---------------------------------------------------------------------------------------------------- PNG


def write_png(path, width, height, rgba):
    """rgba: bytes of width*height*4 (red, green, blue, alpha per pixel, rows top to bottom)."""
    raw = b"".join(b"\x00" + rgba[y * width * 4:(y + 1) * width * 4] for y in range(height))

    def chunk(kind, data):
        return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", zlib.crc32(kind + data) & 0xFFFFFFFF)

    png = b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
    png += chunk(b"IDAT", zlib.compress(raw, 9)) + chunk(b"IEND", b"")
    with open(path, "wb") as f:
        f.write(png)


# ---------------------------------------------------------------------------------------------------- Colours and tiles


def bgr555(value):
    """A GBA/DS colour (15 bits: red in bits 0-4, green 5-9, blue 10-14) as (r, g, b) 0-255."""
    r, g, b = value & 31, (value >> 5) & 31, (value >> 10) & 31
    return (r << 3) | (r >> 2), (g << 3) | (g >> 2), (b << 3) | (b >> 2)


def read_palette(data, offset=0, count=16):
    """count colours (2 bytes each, little-endian BGR555) as a list of (r, g, b)."""
    return [bgr555(struct.unpack_from("<H", data, offset + 2 * i)[0]) for i in range(count) if offset + 2 * i + 2 <= len(data)]


def grey_palette(bpp):
    n = 1 << bpp
    return [(i * 255 // (n - 1),) * 3 for i in range(n)]


def decode_tiles(data, bpp, palette, tiles_wide=16, transparent0=True, linear=False):
    """Tile data as an image: 8x8 tiles, 4 bits per pixel (low nibble = left pixel) or 8, laid out tiles_wide across.
    linear: the data is plain rows of pixels (not tiles), tiles_wide*8 pixels wide. Returns (width, height, rgba)."""
    tile_bytes = 32 if bpp == 4 else 64
    n_tiles = max(1, len(data) // tile_bytes)
    tw = min(tiles_wide, n_tiles)
    th = (n_tiles + tw - 1) // tw
    width, height = tw * 8, th * 8
    pixels = bytearray(width * height * 4)
    if len(palette) < (1 << bpp):
        palette = list(palette) + grey_palette(bpp)[len(palette):]

    def put(x, y, index):
        r, g, b = palette[index]
        at = (y * width + x) * 4
        pixels[at:at + 4] = bytes((r, g, b, 0 if (transparent0 and index == 0) else 255))

    for i in range(n_tiles * tile_bytes * (2 if bpp == 4 else 1)):
        at_byte = i // 2 if bpp == 4 else i
        if at_byte >= len(data):
            break
        byte = data[at_byte]
        index = ((byte >> 4) if i & 1 else (byte & 15)) if bpp == 4 else byte
        if linear:
            x, y = i % width, i // width
        else:
            tile, within = divmod(i, 64)
            x = (tile % tw) * 8 + within % 8
            y = (tile // tw) * 8 + within // 8
        if y < height:
            put(x, y, index)
    return width, height, bytes(pixels)


# ---------------------------------------------------------------------------------------------------- Compression


class NotCompressed(ValueError):
    pass


def lz77_decompress(data, offset=0, max_size=8 * 1024 * 1024):
    """GBA/DS BIOS LZ77 (type 0x10) or the DS's LZ11 (type 0x11). Returns (bytes, compressed length)."""
    if offset + 4 > len(data) or data[offset] not in (0x10, 0x11):
        raise NotCompressed("no LZ header")
    kind = data[offset]
    size = data[offset + 1] | data[offset + 2] << 8 | data[offset + 3] << 16
    pos = offset + 4
    if size == 0 and kind == 0x11:  # sizes of 16 MB and more have 4 more size bytes
        size = struct.unpack_from("<I", data, pos)[0]
        pos += 4
    if size == 0 or size > max_size:
        raise NotCompressed("implausible size")
    out = bytearray()
    while len(out) < size:
        if pos >= len(data):
            raise NotCompressed("ran out of data")
        flags = data[pos]
        pos += 1
        for bit in range(7, -1, -1):
            if len(out) >= size:
                break
            if not flags >> bit & 1:
                if pos >= len(data):
                    raise NotCompressed("ran out of data")
                out.append(data[pos])
                pos += 1
                continue
            if pos + 1 >= len(data):
                raise NotCompressed("ran out of data")
            b0, b1 = data[pos], data[pos + 1]
            if kind == 0x10:
                length, disp = (b0 >> 4) + 3, ((b0 & 15) << 8 | b1) + 1
                pos += 2
            else:
                ind = b0 >> 4
                if ind == 0:
                    b2 = data[pos + 2]
                    length, disp = ((b0 & 15) << 4 | b1 >> 4) + 0x11, ((b1 & 15) << 8 | b2) + 1
                    pos += 3
                elif ind == 1:
                    b2, b3 = data[pos + 2], data[pos + 3]
                    length, disp = ((b0 & 15) << 12 | b1 << 4 | b2 >> 4) + 0x111, ((b2 & 15) << 8 | b3) + 1
                    pos += 4
                else:
                    length, disp = ind + 1, ((b0 & 15) << 8 | b1) + 1
                    pos += 2
            if disp > len(out):
                raise NotCompressed("copies from before the start")
            for _ in range(length):
                out.append(out[-disp])
    return bytes(out[:size]), pos - offset


def maybe_decompress(data):
    """The data decompressed if it's a whole LZ77/LZ11 stream (the usual way DS files are stored), else unchanged."""
    if len(data) >= 4 and data[0] in (0x10, 0x11):
        try:
            out, used = lz77_decompress(data)
            if used >= len(data) - 3:  # the stream covers the file (allowing for padding to 4 bytes)
                return out, True
        except NotCompressed:
            pass
    return data, False


# ---------------------------------------------------------------------------------------------------- GBA


def gba_header(rom):
    """The GBA cartridge header (the first 0xC0 bytes) as a dict, with whether its checksum is right."""
    if len(rom) < 0xC0:
        raise ValueError("too small for a GBA ROM")
    title = rom[0xA0:0xAC].rstrip(b"\x00").decode("ascii", "replace")
    check = 0
    for b in rom[0xA0:0xBD]:
        check = (check - b) & 0xFF
    check = (check - 0x19) & 0xFF
    return {
        "system": "GBA", "title": title, "game_code": rom[0xAC:0xB0].decode("ascii", "replace"),
        "maker_code": rom[0xB0:0xB2].decode("ascii", "replace"), "fixed_96h": rom[0xB2] == 0x96,
        "version": rom[0xBC], "header_checksum": rom[0xBD], "checksum_ok": check == rom[0xBD],
        "size": len(rom), "entry_branch": struct.unpack_from("<I", rom, 0)[0],
    }


def looks_like_gba(rom):
    return len(rom) >= 0xC0 and rom[0xB2] == 0x96


# ---------------------------------------------------------------------------------------------------- NDS


def crc16(data, crc=0xFFFF):
    """The DS's CRC-16 (polynomial 0xA001, as used for the header checksum)."""
    for b in data:
        crc ^= b
        for _ in range(8):
            crc = (crc >> 1) ^ 0xA001 if crc & 1 else crc >> 1
    return crc


def nds_header(rom):
    if len(rom) < 0x200:
        raise ValueError("too small for a DS ROM")
    u32 = lambda off: struct.unpack_from("<I", rom, off)[0]
    h = {
        "system": "NDS", "title": rom[0:12].rstrip(b"\x00").decode("ascii", "replace"),
        "game_code": rom[12:16].decode("ascii", "replace"), "maker_code": rom[16:18].decode("ascii", "replace"),
        "unit_code": rom[0x12], "capacity": 128 * 1024 << rom[0x14], "version": rom[0x1E],
        "arm9": {"rom_offset": u32(0x20), "entry": u32(0x24), "ram_address": u32(0x28), "size": u32(0x2C)},
        "arm7": {"rom_offset": u32(0x30), "entry": u32(0x34), "ram_address": u32(0x38), "size": u32(0x3C)},
        "fnt_offset": u32(0x40), "fnt_size": u32(0x44), "fat_offset": u32(0x48), "fat_size": u32(0x4C),
        "arm9_overlays": (u32(0x50), u32(0x54)), "arm7_overlays": (u32(0x58), u32(0x5C)),
        "icon_offset": u32(0x68), "used_size": u32(0x80), "header_crc": struct.unpack_from("<H", rom, 0x15E)[0],
        "size": len(rom),
    }
    h["header_crc_ok"] = crc16(rom[:0x15E]) == h["header_crc"]
    return h


def looks_like_nds(rom):
    if len(rom) < 0x200:
        return False
    fnt, fat = struct.unpack_from("<I", rom, 0x40)[0], struct.unpack_from("<I", rom, 0x48)[0]
    return 0x200 <= fnt < len(rom) and 0x200 <= fat < len(rom)


def parse_fnt(fnt, base_path=""):
    """A Nitro file name table: {file id: path}. The main table has one 8-byte entry per directory (offset of its
    entries, first file id, parent; the root's parent field holds the number of directories); each directory's entries
    are a length byte (1-127: a file with that name length; 129-255: a subdirectory, name length minus 128, then its id)."""
    names = {}

    def walk(dir_index, path):
        sub, first_id = struct.unpack_from("<IH", fnt, dir_index * 8)
        pos, file_id = sub, first_id
        while pos < len(fnt):
            kind = fnt[pos]
            pos += 1
            if kind == 0:
                break
            length = kind & 0x7F
            name = fnt[pos:pos + length].decode("ascii", "replace")
            pos += length
            if kind & 0x80:
                child = struct.unpack_from("<H", fnt, pos)[0]
                pos += 2
                walk(child & 0x0FFF, path + name + "/")
            else:
                names[file_id] = path + name
                file_id += 1

    walk(0, base_path)
    return names


def parse_fat(fat):
    """A file allocation table: a list of (start, end) offsets, one per file id."""
    return [struct.unpack_from("<II", fat, i) for i in range(0, len(fat) - 7, 8)]


def nds_files(rom):
    """Every file in a DS ROM's file system: (file id, path, bytes). Files without a name are overlays."""
    h = nds_header(rom)
    fnt = rom[h["fnt_offset"]:h["fnt_offset"] + h["fnt_size"]]
    fat = parse_fat(rom[h["fat_offset"]:h["fat_offset"] + h["fat_size"]])
    names = parse_fnt(fnt)
    for file_id, (start, end) in enumerate(fat):
        if end <= start or end > len(rom):
            continue
        yield file_id, names.get(file_id, f"overlay/overlay_{file_id:04d}.bin"), rom[start:end]


# ---------------------------------------------------------------------------------------------------- Nitro formats


def nitro_sections(data):
    """A Nitro file's magic and its sections ({magic as stored: section bytes}). Nitro files start with a 16-byte
    header: magic (4, stored reversed, e.g. b"RGCN" for NCGR), byte order mark 0xFEFF, version, file size, header size,
    number of sections; each section is a magic (reversed), its size (including those 8 bytes), and its data."""
    if len(data) < 16:
        raise ValueError("too small")
    magic = data[0:4]
    header_size, count = struct.unpack_from("<HH", data, 12)
    sections, pos = {}, header_size
    for _ in range(count):
        if pos + 8 > len(data):
            break
        kind, size = data[pos:pos + 4], struct.unpack_from("<I", data, pos + 4)[0]
        if size < 8:
            break
        sections[kind] = data[pos:pos + size]
        pos += size
    return magic, sections


def read_narc(data):
    """A NARC archive's files: [(name or None, bytes)]. Sections: BTAF (offsets), BTNF (names, often empty), GMIF (data)."""
    magic, sec = nitro_sections(data)
    if magic != b"NARC":
        raise ValueError("not a NARC")
    btaf, btnf, gmif = sec.get(b"BTAF"), sec.get(b"BTNF"), sec.get(b"GMIF")
    count = struct.unpack_from("<H", btaf, 8)[0]
    entries = [struct.unpack_from("<II", btaf, 12 + 8 * i) for i in range(count)]
    names = {}
    if btnf is not None and len(btnf) > 16:
        try:
            names = parse_fnt(btnf[8:])
        except (struct.error, IndexError):
            names = {}
    body = gmif[8:]
    return [(names.get(i), body[s:e]) for i, (s, e) in enumerate(entries)]


def read_nclr(data):
    """An NCLR palette file: (bits per pixel, list of (r, g, b)). Section PLTT (stored "TTLP")."""
    magic, sec = nitro_sections(data)
    pltt = sec.get(b"TTLP")
    if magic != b"RLCN" or pltt is None:
        raise ValueError("not an NCLR")
    depth = struct.unpack_from("<H", pltt, 8)[0]
    size, rel = struct.unpack_from("<II", pltt, 16)
    start = 8 + rel
    colours = read_palette(pltt, start, min(size, len(pltt) - start) // 2)
    return (4 if depth == 3 else 8), colours


def read_ncgr(data):
    """An NCGR tile graphics file: dict with bpp, tiles_wide (from the file, or None), linear flag and the tile bytes.
    Section CHAR (stored "RAHC"). Some games (Pokémon Diamond/Pearl/Platinum's Pokémon sprites, for one) encrypt the
    data; this reads it as stored, so those come out as noise."""
    magic, sec = nitro_sections(data)
    char = sec.get(b"RAHC")
    if magic != b"RGCN" or char is None:
        raise ValueError("not an NCGR")
    tiles_high, tiles_wide = struct.unpack_from("<HH", char, 8)
    depth = struct.unpack_from("<I", char, 12)[0]
    linear = struct.unpack_from("<I", char, 20)[0] & 0xFF == 1
    size, rel = struct.unpack_from("<II", char, 24)
    start = 8 + rel
    return {"bpp": 4 if depth == 3 else 8, "tiles_wide": None if tiles_wide in (0xFFFF, 0) else tiles_wide,
            "tiles_high": None if tiles_high in (0xFFFF, 0) else tiles_high, "linear": linear,
            "data": char[start:start + size]}


def read_nscr(data):
    """An NSCR screen (tile map): (width in pixels, height, list of entries). Each entry: tile index (bits 0-9),
    horizontal flip (10), vertical flip (11), palette number (12-15)."""
    magic, sec = nitro_sections(data)
    scrn = sec.get(b"NRCS")
    if magic != b"RCSN" or scrn is None:
        raise ValueError("not an NSCR")
    width, height = struct.unpack_from("<HH", scrn, 8)
    size = struct.unpack_from("<I", scrn, 16)[0]
    entries = list(struct.unpack_from(f"<{size // 2}H", scrn, 20))
    return width, height, entries


def render_screen(width, height, entries, graphics, palette_colours, bpp):
    """A tile map drawn with its tiles and palette: (width, height, rgba)."""
    tile_bytes = 32 if bpp == 4 else 64
    pixels = bytearray(width * height * 4)
    cols = width // 8
    for i, e in enumerate(entries):
        tx, ty = (i % cols) * 8, (i // cols) * 8
        if ty >= height:
            break
        tile, hflip, vflip, pal = e & 0x3FF, e >> 10 & 1, e >> 11 & 1, e >> 12 & 15
        raw = graphics[tile * tile_bytes:(tile + 1) * tile_bytes]
        for p in range(64):
            if bpp == 4:
                if p // 2 >= len(raw):
                    break
                index = (raw[p // 2] >> 4) if p & 1 else (raw[p // 2] & 15)
                colour_index = pal * 16 + index
            else:
                if p >= len(raw):
                    break
                index = colour_index = raw[p]
            px, py = p % 8, p // 8
            if hflip:
                px = 7 - px
            if vflip:
                py = 7 - py
            r, g, b = palette_colours[colour_index] if colour_index < len(palette_colours) else (255, 0, 255)
            at = ((ty + py) * width + tx + px) * 4
            pixels[at:at + 4] = bytes((r, g, b, 0 if index == 0 else 255))
    return width, height, bytes(pixels)


def nitro_kind(data):
    """What a DS file is, from its magic: "NARC", "NCGR", "NCLR", "NSCR", "NCER", "NANR", "SDAT", "NSBMD", ... or None."""
    magics = {b"NARC": "NARC", b"RGCN": "NCGR", b"RLCN": "NCLR", b"RCSN": "NSCR", b"RECN": "NCER", b"RNAN": "NANR",
              b"SDAT": "SDAT", b"BMD0": "NSBMD", b"BTX0": "NSBTX", b"BCA0": "NSBCA", b"BTA0": "NSBTA", b"BTP0": "NSBTP",
              b"MESG": "BMG", b"SSEQ": "SSEQ", b"SBNK": "SBNK", b"SWAR": "SWAR", b"STRM": "STRM"}
    return magics.get(data[:4])
