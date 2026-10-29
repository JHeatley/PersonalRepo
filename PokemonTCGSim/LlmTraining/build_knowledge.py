"""Builds general-knowledge training files for the model, in ./extra, from public sources:

  * Pokémon video games: every Pokémon, move, ability and the type chart from PokéAPI's data (github.com/PokeAPI),
    plus the Wikipedia articles on the main series and spin-off games          -> extra/pokemon_games.jsonl
  * Thomas & Friends: characters, episodes, series and places from the Thomas & Friends wiki (ttte.fandom.com),
    plus the Wikipedia articles on the show, The Railway Series and the Awdrys -> extra/thomas.jsonl
  * Unity 6.2: the Scripting API (core classes and their members, with code examples), the User Manual's game
    development sections and the glossary (docs.unity3d.com)                    -> extra/unity.jsonl

build_dataset.py then adds every file in ./extra to train.jsonl with everything else, so it's still one model:

    python build_knowledge.py                  # all three (downloads take roughly 30-60 minutes the first time)
    python build_knowledge.py --only unity     # just one: pokemon, thomas or unity (comma-separated for several)
    python build_knowledge.py --small          # fewer pages of each, for a quicker test run
    python build_dataset.py                    # then rebuild train.jsonl as usual

Only Python's standard library is needed. Downloads are saved in ./downloads (so a second run is quick, and an
interrupted run carries on where it stopped); --refresh downloads again. The sites are asked politely: one page at a
time with a short pause between pages. Like the Compendium rulings, the downloaded text is other people's work
(Wikipedia and the Fandom wiki are CC BY-SA, the Unity docs are Unity's), so neither the downloads nor the files made
from them are kept in this project; run this yourself to make them.
"""
import argparse
import csv
import html
import io
import json
import os
import random
import re
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
from html.parser import HTMLParser

HERE = os.path.dirname(os.path.abspath(__file__))
DOWNLOADS = os.path.join(HERE, "downloads")
EXTRA = os.path.join(HERE, "extra")
USER_AGENT = ("PokemonTCGSim-LlmTraining/1.0 (personal training-data script, one request at a time; "
              "https://github.com/JHeatley/PersonalRepo)")

# build_dataset.py replaces this with its own SYSTEM when it builds train.jsonl; it's here so the files work on their own.
SYSTEM = "You are a helpful assistant."

REFRESH = False
PAUSE = 0.3
NO_WIKIPEDIA = False


# ---------------------------------------------------------------------------------------------- downloading

def fetch(url, cache_name, pause=None):
    """The page at url, from ./downloads if it was downloaded before. None if it doesn't exist (404)."""
    path = os.path.join(DOWNLOADS, cache_name)
    if os.path.exists(path) and not REFRESH:
        text = open(path, encoding="utf-8").read()
        return None if text == "\0missing" else text
    os.makedirs(os.path.dirname(path), exist_ok=True)
    for attempt in range(6):
        try:
            req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT, "Accept-Encoding": "identity"})
            text = urllib.request.urlopen(req, timeout=60).read().decode("utf-8", errors="replace")
            break
        except urllib.error.HTTPError as e:
            if e.code in (404, 410):
                text = None
                break
            if e.code in (429, 500, 502, 503, 504) and attempt < 5:  # busy: wait (as long as it asks), then try again
                wait = e.headers.get("Retry-After") if e.headers else None
                time.sleep(int(wait) if wait and wait.isdigit() and int(wait) < 600 else 5 * 2 ** attempt)
                continue
            raise
        except (urllib.error.URLError, TimeoutError, ConnectionError):
            if attempt == 5:
                raise
            time.sleep(5 * 2 ** attempt)
    with open(path, "w", encoding="utf-8") as f:
        f.write("\0missing" if text is None else text)
    time.sleep(PAUSE if pause is None else pause)
    return text


def safe_name(s):
    return re.sub(r"[^A-Za-z0-9._-]+", "_", s)[:150]


# ---------------------------------------------------------------------------------------------- HTML to blocks

VOID = {"br", "img", "hr", "input", "meta", "link", "col", "source", "wbr", "area", "base", "embed", "param", "track"}
HEADINGS = {"h1": 1, "h2": 2, "h3": 3, "h4": 4, "h5": 5, "h6": 6}
BLOCKS = {"p", "div", "li", "dd", "dt", "blockquote", "section", "ul", "ol", "dl", "table", "caption", "figcaption",
          "aside", "article", "main", "header", "footer", "center"}
ALWAYS_SKIP_TAGS = {"script", "style", "noscript", "figure", "svg", "math", "button", "textarea", "select", "form",
                    "nav", "audio", "video", "iframe", "template", "label", "input"}
ALWAYS_SKIP_CLASSES = {"reference", "mw-ref", "mw-editsection", "navbox", "infobox", "sidebar", "toc", "reflist",
                       "mw-references-wrap", "references", "hatnote", "metadata", "ambox", "thumb", "gallery",
                       "wikia-gallery", "noprint", "shortdescription", "tooltiptext", "tooltipMoreInfoLink", "suggest",
                       "scrollToFeedback", "portable-infobox", "mw-empty-elt", "navigation-not-searchable",
                       "printfooter", "catlinks", "mbox-small", "side-box", "dablink", "toccolours", "cite-bracket",
                       "mw-cite-backlink", "sistersitebox", "mw-kartographer-maplink", "switch-link", "nomobile"}


class Blocks(HTMLParser):
    """HTML to a list of blocks: ("h", level, text), ("p", text), ("li", text), ("pre", text), ("row", [cells])."""

    def __init__(self, skip_tags=(), skip_classes=()):
        super().__init__(convert_charrefs=True)
        self.skip_tags = ALWAYS_SKIP_TAGS | set(skip_tags)
        self.skip_classes = ALWAYS_SKIP_CLASSES | set(skip_classes)
        self.stack = []        # open tags: (tag, skipped)
        self.skipping = 0
        self.items = []
        self.buf = []
        self.kind = "p"
        self.pre = 0
        self.cells = None      # the cells of the table row being read
        self.cell = None
        self.heading = None

    def _flush(self):
        text = "".join(self.buf)
        self.buf = []
        if self.pre:
            return
        if self.cell is not None:
            self.cell.append(text)
            return
        text = clean_text(text)
        if text:
            self.items.append((self.kind, text))
        self.kind = "p"

    def handle_starttag(self, tag, attrs):
        if tag in VOID:
            if tag == "br" and not self.skipping:
                self.buf.append("\n")
            return
        a = dict(attrs)
        classes = set((a.get("class") or "").split())
        skip = (tag in self.skip_tags or bool(classes & self.skip_classes) or a.get("role") == "navigation"
                or a.get("aria-hidden") == "true" or "display:none" in (a.get("style") or "").replace(" ", ""))
        self.stack.append((tag, skip))
        if skip:
            self.skipping += 1
        if self.skipping:
            return
        if tag in HEADINGS:
            self._flush()
            self.heading = (HEADINGS[tag], [])
        elif tag == "pre":
            self._flush()
            self.pre += 1
        elif self.pre:
            return
        elif tag == "tr":
            self._flush()
            self.cells = []
        elif tag in ("td", "th") and self.cells is not None:
            self._flush()
            self.cell = []
        elif tag in BLOCKS:
            self._flush()
            if tag in ("li", "dd"):
                self.kind = "li"
        elif tag == "code" and not self.pre:
            self.buf.append("`")

    def handle_endtag(self, tag):
        if tag in VOID or not any(t == tag for t, _ in self.stack):
            return
        while self.stack:
            t, skip = self.stack.pop()
            if skip:
                self.skipping -= 1
            elif not self.skipping:
                self._close(t)
            if t == tag:
                break

    def _close(self, tag):
        if tag in HEADINGS and self.heading:
            level, parts = self.heading
            self.heading = None
            text = clean_text("".join(parts + self.buf))
            self.buf = []
            if text:
                self.items.append(("h", level, text))
        elif tag == "pre" and self.pre:
            self.pre -= 1
            code = html_code("".join(self.buf))
            self.buf = []
            if code.strip():
                self.items.append(("pre", code))
        elif self.pre:
            return
        elif tag in ("td", "th") and self.cell is not None:
            text = clean_text("".join(self.cell + self.buf))
            self.buf = []
            self.cells.append(text)
            self.cell = None
        elif tag == "tr" and self.cells is not None:
            self._flush()
            cells = [c for c in self.cells if c]
            self.cells = None
            if cells:
                self.items.append(("row", cells))
        elif tag in BLOCKS:
            self._flush()
        elif tag == "code":
            self.buf.append("`")

    def handle_data(self, data):
        if self.skipping:
            return
        if self.heading is not None:
            self.heading[1].append(data)
        else:
            self.buf.append(data)


def clean_text(text):
    text = text.replace("\xa0", " ").replace("​", "").replace("``", "")
    lines = [re.sub(r"[ \t\r\f\v]+", " ", line).strip() for line in text.split("\n")]
    text = "\n".join(lines)
    text = re.sub(r"\n{3,}", "\n\n", text).strip()
    text = re.sub(r"\[(?:\d+|[a-z]|note \d+|citation needed|edit)\]", "", text)
    text = re.sub(r" ([,.;:!?)])", r"\1", text)
    text = re.sub(r"\( ", "(", text)
    text = re.sub(r"\s*More info\s*$", "", text)
    return text.strip(" `") if text.count("`") == 1 else text.strip()


def html_code(code):
    code = code.replace("\xa0", " ").replace("\r", "")
    lines = code.split("\n")
    while lines and not lines[0].strip():
        lines.pop(0)
    while lines and not lines[-1].strip():
        lines.pop()
    indent = min((len(l) - len(l.lstrip()) for l in lines if l.strip()), default=0)
    return "\n".join(l[indent:].rstrip() for l in lines)


def parse_blocks(markup, **kw):
    p = Blocks(**kw)
    p.feed(markup)
    p.close()
    p._flush()
    return p.items


def sections(items, max_level=3):
    """Groups blocks under their headings: [(path, blocks)], path being the headings (h1/h2/h3...) above them."""
    out = []
    path = []
    current = []
    for it in items:
        if it[0] == "h" and it[1] <= max_level:
            if current:
                out.append((tuple(path), current))
            current = []
            level = it[1]
            path = [p for p in path if p[0] < level] + [(level, it[2])]
        else:
            current.append(it)
    if current:
        out.append((tuple(path), current))
    return [(tuple(t for _, t in p), b) for p, b in out]


def render(blocks, max_chars=2600, code_lang="csharp"):
    """Blocks as a Markdown answer, cut at a block boundary near max_chars."""
    parts = []
    size = 0
    for b in blocks:
        if b[0] == "h":
            text = f"**{b[2]}**"
        elif b[0] == "li":
            text = "- " + b[1].replace("\n", " ")
        elif b[0] == "row":
            cells = b[1]
            text = "- " + (f"**{cells[0]}**: " + " | ".join(cells[1:]) if len(cells) > 1 else cells[0])
        elif b[0] == "pre":
            lang = code_lang if re.search(r"[;{}]", b[1]) else ""
            text = f"```{lang}\n{b[1]}\n```"
        else:
            text = b[1]
        if size and size + len(text) > max_chars:
            break
        if not size and len(text) > max_chars and b[0] != "pre":  # one very long paragraph: cut it at a sentence
            cut = text.rfind(". ", 0, max_chars)
            text = text[:cut + 1] if cut > 200 else text[:max_chars]
        parts.append(text)
        size += len(text) + 2
    out = []
    for t in parts:  # list items sit together; everything else is separated by a blank line
        if out and t.startswith("- ") and out[-1].startswith("- "):
            out[-1] += "\n" + t
        else:
            out.append(t)
    return "\n\n".join(out).strip()


def prose_size(blocks):
    return sum(len(b[-1]) if b[0] != "row" else sum(map(len, b[1])) for b in blocks if b[0] != "h")


def example(user, assistant):
    return {"messages": [{"role": "system", "content": SYSTEM}, {"role": "user", "content": user},
                         {"role": "assistant", "content": assistant}]}


def write(name, examples):
    os.makedirs(EXTRA, exist_ok=True)
    seen = set()
    path = os.path.join(EXTRA, name)
    with open(path, "w", encoding="utf-8") as f:
        n = 0
        for ex in examples:
            key = ex["messages"][1]["content"]
            if key in seen or not ex["messages"][2]["content"].strip():
                continue
            seen.add(key)
            f.write(json.dumps(ex, ensure_ascii=False) + "\n")
            n += 1
    print(f"wrote {n:,} examples to {path}")


# ---------------------------------------------------------------------------------------------- Wikipedia

WIKI_SKIP_SECTIONS = {"references", "external links", "notes", "see also", "further reading", "bibliography",
                      "sources", "citations", "footnotes", "explanatory notes", "works cited", "gallery",
                      "notes and references", "references and notes", "filmography", "discography", "explanatory notes"}


def wikipedia(title):
    url = "https://en.wikipedia.org/api/rest_v1/page/html/" + urllib.parse.quote(title.replace(" ", "_"), safe="")
    return fetch(url, os.path.join("wikipedia", safe_name(title) + ".html"), pause=1.0)


def wikipedia_examples(titles, subject, rng):
    """Questions about each part of each article: what it is, then its sections (gameplay, plot, reception...)."""
    out = []
    if NO_WIKIPEDIA:
        return out
    for title in titles:
        try:
            page = wikipedia(title)
        except Exception as e:
            print(f"  couldn't download Wikipedia: {title}: {e}", file=sys.stderr)
            continue
        if not page:
            print(f"  no Wikipedia article called {title}", file=sys.stderr)
            continue
        m = re.search(r"<title>(.*?)</title>", page)
        name = html.unescape(re.sub(r"<[^>]+>", "", m.group(1))) if m else title
        items = parse_blocks(page, skip_tags={"table"}, skip_classes={"wikitable"})
        for path, blocks in sections(items):
            if path and path[0].lower() in WIKI_SKIP_SECTIONS:
                continue
            if prose_size(blocks) < 250:
                continue
            answer = render(blocks)
            if not path:
                q = rng.choice([f"What is {name}?", f"Tell me about {name}.", f"Give me an overview of {name}."])
                if subject and subject.lower() not in name.lower():
                    q = q[:-1] + f" ({subject})" + q[-1]
            else:
                where = path[-1] if len(path) == 1 else f"{path[-1]} ({path[0]})"
                q = rng.choice([f"Tell me about {name}: {where}.", f"What does {name} cover under \"{where}\"?",
                                f"Explain the {where.lower() if len(path) == 1 else where} section of {name}."])
            out.append(example(q, answer))
    return out


# ---------------------------------------------------------------------------------------------- Pokémon video games

POKEAPI = "https://raw.githubusercontent.com/PokeAPI/pokeapi/master/data/v2/csv/"
POKEAPI_FILES = ["pokemon", "pokemon_species", "pokemon_species_names", "pokemon_types", "type_names", "pokemon_stats",
                 "pokemon_abilities", "ability_names", "ability_prose", "abilities", "pokemon_species_flavor_text",
                 "version_names", "versions", "version_groups", "pokemon_evolution", "evolution_trigger_prose",
                 "item_names", "moves", "move_names", "move_effect_prose", "move_damage_class_prose", "type_efficacy",
                 "generation_names", "location_names", "pokemon_species_prose", "move_flavor_text", "pokemon_moves",
                 "pokemon_move_methods"]
ENGLISH = "9"

POKEMON_GAMES_WIKI = [
    "Pokémon (video game series)", "Gameplay of Pokémon", "List of Pokémon video games", "Pokémon",
    "Pokémon Red, Blue, and Yellow", "Pokémon Gold and Silver", "Pokémon Crystal", "Pokémon Ruby and Sapphire",
    "Pokémon Emerald",
    "Pokémon FireRed and LeafGreen", "Pokémon Diamond and Pearl", "Pokémon Platinum", "Pokémon HeartGold and SoulSilver",
    "Pokémon Black and White", "Pokémon Black 2 and White 2", "Pokémon X and Y", "Pokémon Omega Ruby and Alpha Sapphire",
    "Pokémon Sun and Moon", "Pokémon Ultra Sun and Ultra Moon", "Pokémon: Let's Go, Pikachu! and Let's Go, Eevee!",
    "Pokémon Sword and Shield", "Pokémon Brilliant Diamond and Shining Pearl", "Pokémon Legends: Arceus",
    "Pokémon Scarlet and Violet", "Pokémon Legends: Z-A", "Pokémon Stadium", "Pokémon Stadium 2", "Pokémon Snap",
    "New Pokémon Snap", "Pokémon Pinball", "Pokémon Trading Card Game (video game)", "Pokémon Puzzle League",
    "Pokémon Colosseum", "Pokémon XD: Gale of Darkness", "Pokémon Battle Revolution",
    "Pokémon Mystery Dungeon: Red Rescue Team and Blue Rescue Team",
    "Pokémon Mystery Dungeon: Explorers of Time and Explorers of Darkness", "Pokémon Ranger", "Pokémon Conquest",
    "Pokémon Rumble", "Pokkén Tournament", "Pokémon Go", "Pokémon Masters EX", "Pokémon Unite", "Pokémon Sleep",
    "Detective Pikachu (video game)", "Pokémon Trading Card Game Pocket", "Pokémon Champions", "Game Freak",
    "The Pokémon Company", "Creatures (company)", "Satoshi Tajiri", "Ken Sugimori", "Junichi Masuda", "Shigeki Morimoto",
    "Pokémon World Championships", "Pikachu", "Ash Ketchum", "Pokémon (TV series)", "Missingno.",
    "List of Pokémon", "Pokémon Trading Card Game", "Pokémon Yellow",
]


def read_csv(name):
    text = fetch(POKEAPI + name + ".csv", os.path.join("pokeapi", name + ".csv"), pause=0.1)
    return list(csv.DictReader(io.StringIO(text))) if text else []


def prose(text):
    """PokéAPI's text markup ([label]{mechanic:paralysis}) as plain text."""
    text = re.sub(r"\[([^\]]*)\]\{[^:}]+:([^}]+)\}", lambda m: m.group(1) or m.group(2).replace("-", " "), text)
    text = text.replace("­\n", "").replace("­", "").replace("\f", " ").replace("\n", " ")
    return re.sub(r"\s+", " ", text).strip()


def type_article(word):
    """'a Fire' / 'an Electric' / 'an Ice Stone': the word with its article."""
    return ("an " if word[:1] in "AEIOU" else "a ") + word


def join_words(words):
    words = list(words)
    if len(words) <= 1:
        return "".join(words)
    return ", ".join(words[:-1]) + " and " + words[-1]


def pokemon_data_examples(rng, small):
    t = {name: read_csv(name) for name in POKEAPI_FILES}
    eng = lambda rows, key: {r[key]: r for r in rows if r.get("local_language_id") == ENGLISH}
    species_names = eng(t["pokemon_species_names"], "pokemon_species_id")
    type_names = {r["type_id"]: r["name"] for r in t["type_names"] if r["local_language_id"] == ENGLISH}
    gen_names = {r["generation_id"]: r["name"] for r in t["generation_names"] if r["local_language_id"] == ENGLISH}
    ability_names = {r["ability_id"]: r["name"] for r in t["ability_names"] if r["local_language_id"] == ENGLISH}
    ability_prose = eng(t["ability_prose"], "ability_id")
    version_names = {r["version_id"]: r["name"] for r in t["version_names"] if r["local_language_id"] == ENGLISH}
    item_names = {r["item_id"]: r["name"] for r in t["item_names"] if r["local_language_id"] == ENGLISH}
    location_names = {r["location_id"]: r["name"] for r in t["location_names"] if r["local_language_id"] == ENGLISH}
    move_names = {r["move_id"]: r["name"] for r in t["move_names"] if r["local_language_id"] == ENGLISH}
    trigger_names = {r["evolution_trigger_id"]: r["name"] for r in t["evolution_trigger_prose"]
                     if r["local_language_id"] == ENGLISH}
    class_names = {r["move_damage_class_id"]: r["name"] for r in t["move_damage_class_prose"]
                   if r["local_language_id"] == ENGLISH}
    effect_prose = eng(t["move_effect_prose"], "move_effect_id")
    stat_names = {"1": "HP", "2": "Attack", "3": "Defense", "4": "Sp. Atk", "5": "Sp. Def", "6": "Speed"}
    real_types = {k: v for k, v in type_names.items() if int(k) <= 18}

    # Type chart: damage_factor 0/50/100/200 for each attacking type against each defending type.
    chart = {(r["damage_type_id"], r["target_type_id"]): int(r["damage_factor"]) for r in t["type_efficacy"]}

    species = {r["id"]: r for r in t["pokemon_species"]}
    default_form = {}
    for r in t["pokemon"]:
        if r["is_default"] == "1":
            default_form[r["species_id"]] = r["id"]
    types = {}
    for r in sorted(t["pokemon_types"], key=lambda r: int(r["slot"])):
        types.setdefault(r["pokemon_id"], []).append(r["type_id"])
    stats = {}
    for r in t["pokemon_stats"]:
        stats.setdefault(r["pokemon_id"], {})[r["stat_id"]] = int(r["base_stat"])
    abilities = {}
    for r in sorted(t["pokemon_abilities"], key=lambda r: int(r["slot"])):
        abilities.setdefault(r["pokemon_id"], []).append((r["ability_id"], r["is_hidden"] == "1"))
    flavor = {}
    for r in t["pokemon_species_flavor_text"]:
        if r["language_id"] == ENGLISH:
            flavor.setdefault(r["species_id"], []).append((int(r["version_id"]), r["flavor_text"]))
    evolutions = {}
    for r in t["pokemon_evolution"]:
        evolutions.setdefault(r["evolved_species_id"], []).append(r)
    evolves_into = {}
    for sid, s in species.items():
        if s["evolves_from_species_id"]:
            evolves_into.setdefault(s["evolves_from_species_id"], []).append(sid)

    # Level-up moves in the newest games each Pokémon appears in.
    level_method = next((r["id"] for r in t["pokemon_move_methods"] if r["identifier"] == "level-up"), "1")
    group_order = {r["id"]: int(r["order"]) for r in t["version_groups"]}
    newest_group = {}
    for r in t["pokemon_moves"]:
        if r["pokemon_move_method_id"] == level_method:
            best = newest_group.get(r["pokemon_id"])
            if best is None or group_order.get(r["version_group_id"], 0) > group_order.get(best, 0):
                newest_group[r["pokemon_id"]] = r["version_group_id"]
    level_moves = {}
    for r in t["pokemon_moves"]:
        if r["pokemon_move_method_id"] == level_method and r["version_group_id"] == newest_group[r["pokemon_id"]]:
            level_moves.setdefault(r["pokemon_id"], []).append((int(r["level"] or 0), r["move_id"]))
    group_names = {}
    for v in t["versions"]:
        group_names.setdefault(v["version_group_id"], []).append(version_names.get(v["id"], v["identifier"]))

    def name_of(sid):
        return species_names.get(sid, {}).get("name") or species[sid]["identifier"].title()

    def type_words(pid):
        return "/".join(type_names[x] for x in types.get(pid, []))

    def matchups(pid):
        defs = types.get(pid, [])
        weak, resist, immune = [], [], []
        for atk, atk_name in real_types.items():
            f = 1.0
            for d in defs:
                f *= chart.get((atk, d), 100) / 100
            if f == 0:
                immune.append(atk_name)
            elif f > 1:
                weak.append(atk_name + (" (4×)" if f >= 4 else ""))
            elif f < 1:
                resist.append(atk_name + (" (¼×)" if f <= 0.25 else ""))
        return weak, resist, immune

    def one_method(e):
        """One way to evolve (one row of PokéAPI's pokemon_evolution) in words."""
        parts = []
        trig = trigger_names.get(e["evolution_trigger_id"], "")
        if trig.startswith("Trade"):
            parts.append("by trading" + (f" for {name_of(e['trade_species_id'])}" if e["trade_species_id"] else "")
                         + (" (or using a Linking Cord)" if "Linking Cord" in trig else ""))
        if e["minimum_level"]:
            parts.append(f"at level {e['minimum_level']}")
        if e["trigger_item_id"]:
            parts.append(f"using {type_article(item_names.get(e['trigger_item_id'], 'special item'))}")
        if e["held_item_id"]:
            parts.append(f"while holding {type_article(item_names.get(e['held_item_id'], 'special item'))}")
        if e["known_move_id"]:
            parts.append(f"while knowing {move_names.get(e['known_move_id'], 'a certain move')}")
        if e["known_move_type_id"]:
            parts.append(f"while knowing {type_article(type_names.get(e['known_move_type_id'], ''))}-type move")
        if e["used_move_id"]:
            parts.append(f"after using {move_names.get(e['used_move_id'], 'a certain move')}")
        if e["minimum_happiness"]:
            parts.append("with high friendship")
        if e["minimum_affection"]:
            parts.append("with high affection")
        if e["minimum_beauty"]:
            parts.append("with high Beauty")
        if e["time_of_day"]:
            parts.append(f"during the {e['time_of_day']}")
        if e["party_species_id"]:
            parts.append(f"with {name_of(e['party_species_id'])} in the party")
        if e["party_type_id"]:
            parts.append(f"with {type_article(type_names.get(e['party_type_id'], ''))}-type Pokémon in the party")
        if e["relative_physical_stats"] in ("1", "-1", "0"):
            parts.append({"1": "when its Attack is higher than its Defense", "-1": "when its Defense is higher than its "
                          "Attack", "0": "when its Attack and Defense are equal"}[e["relative_physical_stats"]])
        if e["needs_overworld_rain"] == "1":
            parts.append("while it is raining")
        if e["turn_upside_down"] == "1":
            parts.append("while the console is held upside down")
        if e["minimum_steps"]:
            parts.append(f"after walking {e['minimum_steps']} steps")
        if e["minimum_damage_taken"]:
            parts.append(f"after losing at least {e['minimum_damage_taken']} HP")
        if e["gender_id"]:
            parts.append("(female only)" if e["gender_id"] == "1" else "(male only)")
        place = ""
        if e["location_id"] or e["near_special_rock"] == "1":
            where = location_names.get(e["location_id"], "")
            place = ("by leveling up " + ("near a special rock" if e["near_special_rock"] == "1" else "at a special place")
                     + (f" (such as at {where})" if where else ""))
            parts.append(place)
        if not parts:
            parts.append("by leveling up" if trig == "Level up" else f"({trig})" if trig else "under special conditions")
        elif trig == "Level up" and not any(w in " ".join(parts) for w in ("level", "leveling")):
            parts.insert(0, "by leveling up")
        return " ".join(parts), bool(place)

    def evolution_method(sid):
        """Every way to evolve into this species: the current games' way first, older or alternative ones after."""
        rows = sorted(evolutions.get(sid, []), key=lambda r: (r["is_default"] != "1",
                                                              -group_order.get(r["version_group_id"], 0)))
        methods, had_place = [], False
        for r in rows:
            text, is_place = one_method(r)
            if is_place:
                if had_place:
                    continue
                had_place = True
            if text not in methods:
                methods.append(text)
        return " or ".join(methods[:3]) or "under special conditions"

    out = []
    ids = sorted(species, key=int)
    if small:
        ids = ids[:151]
    for sid in ids:
        s = species[sid]
        pid = default_form.get(sid)
        if not pid:
            continue
        name = name_of(sid)
        genus = species_names.get(sid, {}).get("genus", "")
        tw = type_words(pid)
        gen = gen_names.get(s["generation_id"], f"Generation {s['generation_id']}")
        st = stats.get(pid, {})
        total = sum(st.values())
        ab = abilities.get(pid, [])
        normal_ab = [ability_names.get(a, a) for a, hidden in ab if not hidden]
        hidden_ab = [ability_names.get(a, a) for a, hidden in ab if hidden]
        lines = [f"**{name}** (National Pokédex #{int(sid):04d}) is {type_article(tw)}-type Pokémon"
                 + (f", the {genus}" if genus else "") + f", introduced in {gen}."]
        tags = [w for w, flag in (("Legendary", s["is_legendary"]), ("Mythical", s["is_mythical"]),
                                  ("Baby", s["is_baby"])) if flag == "1"]
        if tags:
            lines[0] += f" It is a {tags[0]} Pokémon."
        lines.append(f"- Abilities: {join_words(normal_ab) or 'none'}"
                     + (f"; hidden ability: {join_words(hidden_ab)}" if hidden_ab else ""))
        lines.append("- Base stats: " + ", ".join(f"{stat_names[k]} {st[k]}" for k in sorted(st) if k in stat_names)
                     + f" (total {total})")
        lines.append(f"- Height {int(dict_get(t, 'pokemon', pid, 'height')) / 10:g} m, weight "
                     f"{int(dict_get(t, 'pokemon', pid, 'weight')) / 10:g} kg")
        if s["evolves_from_species_id"]:
            lines.append(f"- Evolves from {name_of(s['evolves_from_species_id'])} {evolution_method(sid)}")
        for nxt in sorted(evolves_into.get(sid, []), key=int):
            lines.append(f"- Evolves into {name_of(nxt)} {evolution_method(nxt)}")
        entries = sorted(flavor.get(sid, []))
        if entries:
            v, text = entries[-1]
            lines.append(f"\nPokédex entry ({version_names.get(str(v), 'latest games')}): \"{prose(text)}\"")
        q = rng.choice([f"Tell me about the Pokémon {name}.", f"What do you know about {name}?",
                        f"Give me {name}'s Pokédex data.", f"Describe {name} from the Pokémon games."])
        out.append(example(q, "\n".join(lines)))

        weak, resist, immune = matchups(pid)
        ans = [f"{name} is {tw}-type."]
        ans.append(f"- Weak to: {join_words(weak) or 'nothing'}")
        ans.append(f"- Resists: {join_words(resist) or 'nothing'}")
        if immune:
            ans.append(f"- Immune to: {join_words(immune)}")
        ans.append(f"So {'it takes super-effective damage from ' + join_words(w.split(' ')[0] for w in weak) + ' moves' if weak else 'nothing hits it super-effectively'}"
                   " (in the main series games; abilities like Levitate or Thick Fat can change this).")
        q = rng.choice([f"What type is {name} and what is it weak to?", f"What are {name}'s weaknesses?",
                        f"What's super effective against {name}?"])
        out.append(example(q, "\n".join(ans)))

        moves = sorted(level_moves.get(pid, []))
        if moves:
            games = join_words(group_names.get(newest_group[pid], []))
            move_list = ", ".join(f"{move_names.get(m, m)} ({'evolution' if lv == 0 else f'Lv. {lv}'})"
                                  for lv, m in moves[:40])
            out.append(example(f"What moves does {name} learn by leveling up?",
                               f"In {games}, {name} learns these moves by level-up: {move_list}."))

    # Moves.
    for m in t["moves"]:
        if int(m["id"]) >= 10000 or m["id"] not in move_names:
            continue
        name = move_names[m["id"]]
        tname = type_names.get(m["type_id"], "")
        cls = class_names.get(m["damage_class_id"], "").lower()
        eff = effect_prose.get(m["effect_id"], {})
        effect = prose((eff.get("short_effect") or "").replace("$effect_chance", m["effect_chance"] or "some"))
        long_effect = prose((eff.get("effect") or "").replace("$effect_chance", m["effect_chance"] or "some"))
        facts = [f"Power {m['power'] or '—'}", f"Accuracy {m['accuracy'] + '%' if m['accuracy'] else '— (never misses)'}",
                 f"PP {m['pp']}"]
        if m["priority"] and m["priority"] != "0":
            facts.append(f"priority {int(m['priority']):+d}")
        ans = (f"**{name}** is {type_article(tname)}-type {cls} move introduced in {gen_names.get(m['generation_id'], '')}. "
               f"{', '.join(facts)}.\n\n{long_effect if 0 < len(long_effect) < 900 else effect}")
        q = rng.choice([f"What does the move {name} do?", f"Explain the Pokémon move {name}.",
                        f"What are {name}'s power, accuracy and PP?"])
        out.append(example(q, ans))
        if small and len(out) > 900:
            break

    # Abilities.
    holders = {}
    by_pid_species = {r["id"]: r["species_id"] for r in t["pokemon"] if r["is_default"] == "1"}
    for pid, ab in abilities.items():
        if pid in by_pid_species:
            for a, hidden in ab:
                holders.setdefault(a, []).append(name_of(by_pid_species[pid]) + (" (hidden)" if hidden else ""))
    for a in t["abilities"]:
        if a["is_main_series"] != "1" or a["id"] not in ability_names:
            continue
        p = ability_prose.get(a["id"], {})
        effect = prose(p.get("effect") or p.get("short_effect") or "")
        if not effect:
            continue
        if len(effect) > 1200:
            effect = prose(p.get("short_effect") or effect[:1200])
        who = holders.get(a["id"], [])
        ans = f"**{ability_names[a['id']]}** (an ability introduced in {gen_names.get(a['generation_id'], '')}): {effect}"
        if who:
            ans += f"\n\nPokémon that can have it include {join_words(who[:25])}" + (" and others." if len(who) > 25 else ".")
        out.append(example(rng.choice([f"What does the ability {ability_names[a['id']]} do?",
                                       f"Explain the Pokémon ability {ability_names[a['id']]}."]), ans))

    # Type chart.
    for atk, atk_name in real_types.items():
        strong = [n for d, n in real_types.items() if chart.get((atk, d), 100) == 200]
        weak = [n for d, n in real_types.items() if chart.get((atk, d), 100) == 50]
        none = [n for d, n in real_types.items() if chart.get((atk, d), 100) == 0]
        ans = (f"{atk_name}-type moves are super effective (2×) against {join_words(strong) or 'no types'}; "
               f"not very effective (½×) against {join_words(weak) or 'no types'}"
               + (f"; and have no effect on {join_words(none)}." if none else "."))
        out.append(example(f"What is {atk_name} strong and weak against in Pokémon?", ans))
        hurt_by = [n for a2, n in real_types.items() if chart.get((a2, atk), 100) == 200]
        resists = [n for a2, n in real_types.items() if chart.get((a2, atk), 100) == 50]
        immune = [n for a2, n in real_types.items() if chart.get((a2, atk), 100) == 0]
        ans = (f"{type_article(atk_name)[0].upper() + type_article(atk_name)[1:]}-type Pokémon takes super-effective damage from {join_words(hurt_by) or 'no types'} moves, "
               f"resists {join_words(resists) or 'nothing'}"
               + (f", and is immune to {join_words(immune)} moves." if immune else "."))
        out.append(example(f"What are {atk_name}-type Pokémon weak to?", ans))
    return out


def dict_get(t, table, row_id, column):
    index = t.setdefault("_index_" + table, {r["id"]: r for r in t[table]})
    return index[row_id][column]


def pokemon_examples(rng, small):
    print("Pokémon video games: PokéAPI data ...", flush=True)
    out = pokemon_data_examples(rng, small)
    print(f"  {len(out):,} examples from PokéAPI; Wikipedia articles ...", flush=True)
    out += wikipedia_examples(POKEMON_GAMES_WIKI[:12] if small else POKEMON_GAMES_WIKI, "Pokémon", rng)
    write("pokemon_games.jsonl", out)


# ---------------------------------------------------------------------------------------------- Thomas & Friends

FANDOM = "https://ttte.fandom.com/api.php"
THOMAS_WIKI = [
    "Thomas & Friends", "Thomas the Tank Engine", "The Railway Series", "Wilbert Awdry", "Christopher Awdry",
    "Thomas & Friends: All Engines Go", "Thomas and the Magic Railroad", "Thomas & Friends: The Adventure Begins",
    "Britt Allcroft", "Island of Sodor", "Shining Time Station", "Thomas & Friends: Big World! Big Adventures!",
    "List of Thomas & Friends characters", "Thomas Land", "Day Out with Thomas", "Talyllyn Railway", "HiT Entertainment",
    "Mattel", "Ringo Starr", "Michael Angelis", "Mike O'Donnell (composer)", "Junior Campbell", "Robert Hartshorne",
    "Thomas & Friends: Misty Island Rescue", "Thomas & Friends: Hero of the Rails", "Tale of the Brave",
    "Thomas & Friends: The Great Race", "Thomas & Friends: Sodor's Legend of the Lost Treasure",
    "Thomas & Friends: Journey Beyond Sodor", "The Railway Man (1994 TV film)",
]
FANDOM_SKIP_SECTIONS = {"appearances", "gallery", "references", "merchandise", "voice actors", "voice actor", "quotes",
                        "episodes", "songs", "music videos", "external links", "navigation", "companion volumes",
                        "home media releases", "in other languages", "other languages", "cast", "characters",
                        "locations", "rolling stock", "featured characters", "audio", "videos", "music",
                        "behind the scenes", "filming", "goofs", "songs featured", "notes", "see also", "sources",
                        "deleted and extended scenes", "dvd packs", "release dates"}


def fandom_category(category, limit):
    titles = []
    cont = ""
    while len(titles) < limit:
        url = (f"{FANDOM}?action=query&list=categorymembers&cmtitle={urllib.parse.quote('Category:' + category)}"
               f"&cmnamespace=0&cmlimit=500&format=json&formatversion=2" + (f"&cmcontinue={cont}" if cont else ""))
        text = fetch(url, os.path.join("fandom", "cat_" + safe_name(category) + safe_name(cont) + ".json"))
        if not text:
            break
        d = json.loads(text)
        titles += [m["title"] for m in d.get("query", {}).get("categorymembers", [])]
        cont = urllib.parse.quote(d.get("continue", {}).get("cmcontinue", ""))
        if not cont:
            break
    return titles[:limit]


def fandom_page(title):
    url = (f"{FANDOM}?action=parse&page={urllib.parse.quote(title)}&prop=text&redirects=1&disableeditsection=1"
           f"&disabletoc=1&format=json&formatversion=2")
    text = fetch(url, os.path.join("fandom", safe_name(title) + ".json"), pause=0.5)
    if not text:
        return None
    d = json.loads(text)
    return d.get("parse", {}).get("text") if "error" not in d else None


def display_name(title):
    return re.sub(r"\s*\((?:T&F|TV series|RWS|CGI|Thomas & Friends|episode|special)\)\s*$", "", title)


def infobox(markup):
    """The facts box at the top of a Fandom page, as (label, value) pairs."""
    m = re.search(r'<aside[^>]*portable-infobox.*?</aside>', markup, re.S)
    if not m:
        return []
    pairs = []
    for label, value in re.findall(r'<h3[^>]*pi-data-label[^>]*>(.*?)</h3>\s*<div[^>]*pi-data-value[^>]*>(.*?)</div>\s*</div>',
                                   m.group(0), re.S):
        value = re.sub(r"<br\s*/?>", "; ", value)
        value = re.sub(r"<li[^>]*>", "; ", value)
        value = html.unescape(re.sub(r"<[^>]+>", "", value))
        value = re.sub(r"\s*;\s*(;\s*)*", "; ", re.sub(r"\[\d+\]", "", value)).strip("; \n")
        label = html.unescape(re.sub(r"<[^>]+>", "", label)).strip()
        if label and value and len(value) < 400:
            pairs.append((label, re.sub(r"\s+", " ", value)))
    return pairs


def fandom_examples(titles, kind, rng):
    out = []
    for title in titles:
        try:
            markup = fandom_page(title)
        except Exception as e:
            print(f"  couldn't download {title}: {e}", file=sys.stderr)
            continue
        if not markup:
            continue
        name = display_name(title)
        facts = infobox(markup)
        if facts:
            q = {"character": f"What are the key facts about {name} from Thomas & Friends?",
                 "episode": f"What are the details of the Thomas & Friends episode \"{name}\"?",
                 "series": f"What are the details of {name} of Thomas & Friends?",
                 "place": f"What are the key facts about {name} on the Island of Sodor?"}[kind]
            out.append(example(q, f"**{name}**\n" + "\n".join(f"- {k}: {v}" for k, v in facts)))
        items = parse_blocks(markup, skip_tags={"table"})
        for path, blocks in sections(items):
            if path and any(p.lower() in FANDOM_SKIP_SECTIONS or p.lower().startswith(("audio", "gallery"))
                            for p in path):
                continue
            # The quotes at the top of a page (and their "― who said it" lines) aren't part of the article.
            while blocks and blocks[0][0] != "h" and blocks[0][-1][:1] in ('"', "“", "―", "'"):
                blocks = blocks[1:]
            blocks = [b for b in blocks if b[0] == "h" or not b[-1].startswith("―")]
            if prose_size(blocks) < 120:
                continue
            answer = render(blocks, max_chars=2400)
            if not path:
                q = {"character": rng.choice([f"Who is {name} in Thomas & Friends?", f"Tell me about {name} from Thomas & Friends."]),
                     "episode": rng.choice([f"What is the Thomas & Friends episode \"{name}\" about?",
                                            f"Summarise the Thomas & Friends episode \"{name}\"."]),
                     "series": f"Tell me about {name} of Thomas & Friends.",
                     "place": f"What is {name} in Thomas & Friends?"}[kind]
            else:
                where = " - ".join(path)
                if kind == "episode" and path[0].lower() == "plot":
                    q = f"What happens in the Thomas & Friends episode \"{name}\"?"
                elif kind == "character" and path[0].lower() == "personality":
                    q = f"What is {name}'s personality like in Thomas & Friends?"
                elif kind == "character" and path[0].lower() == "technical details":
                    q = f"What kind of engine (or vehicle) is {name} in real life? What is {name} based on?"
                else:
                    q = f"Thomas & Friends, {name}: tell me about \"{where}\"."
            out.append(example(q, answer))
    return out


def thomas_examples(rng, small):
    print("Thomas & Friends: wiki pages ...", flush=True)
    lim = 25 if small else 2000
    characters = fandom_category("Thomas & Friends characters", lim)
    steam_team = fandom_category("Steam Team", 50)
    characters = steam_team + [c for c in characters if c not in steam_team]
    out = fandom_examples(characters, "character", rng)
    print(f"  {len(out):,} examples from {len(characters)} character pages", flush=True)
    out += fandom_examples(fandom_category("Series", 10 if small else 100), "series", rng)
    places = fandom_category("Landmarks", 10 if small else 300) + fandom_category("Inhabited areas", 10 if small else 200)
    out += fandom_examples(["Island of Sodor (T&F)", "North Western Railway (T&F)"] + places, "place", rng)
    episodes = fandom_category("Episodes", 25 if small else 2000)
    out += fandom_examples(episodes, "episode", rng)
    print(f"  {len(out):,} examples from the wiki; Wikipedia articles ...", flush=True)
    out += wikipedia_examples(THOMAS_WIKI[:5] if small else THOMAS_WIKI, "Thomas & Friends", rng)
    write("thomas.jsonl", out)


# ---------------------------------------------------------------------------------------------- Unity

UNITY = "https://docs.unity3d.com/6000.2/Documentation/"
UNITY_CLASSES = [
    "MonoBehaviour", "GameObject", "Transform", "Component", "Object", "Behaviour", "ScriptableObject", "Vector2",
    "Vector3", "Vector2Int", "Vector3Int", "Quaternion", "Mathf", "Random", "Time", "Debug", "Input", "KeyCode",
    "Rigidbody", "Rigidbody2D", "Collider", "BoxCollider", "SphereCollider", "CapsuleCollider", "MeshCollider",
    "CharacterController", "Collider2D", "BoxCollider2D", "CircleCollider2D", "Collision", "Collision2D", "Physics",
    "Physics2D", "RaycastHit", "RaycastHit2D", "Ray", "Camera", "Light", "Renderer", "MeshRenderer",
    "SkinnedMeshRenderer", "SpriteRenderer", "Sprite", "Texture2D", "RenderTexture", "Material", "Shader", "Mesh",
    "MeshFilter", "Color", "Color32", "Gradient", "AnimationCurve", "Animator", "AnimationClip", "Animation",
    "AudioSource", "AudioClip", "AudioListener", "Resources", "Application", "PlayerPrefs", "JsonUtility", "Coroutine",
    "WaitForSeconds", "WaitForSecondsRealtime", "WaitForEndOfFrame", "WaitForFixedUpdate", "WaitUntil", "WaitWhile",
    "Awaitable", "LayerMask", "Bounds", "Rect", "RectTransform", "Canvas", "CanvasGroup", "Matrix4x4", "Screen",
    "Cursor", "Gizmos", "GUI", "GUILayout", "GUIStyle", "ParticleSystem", "LineRenderer", "TrailRenderer",
    "SceneManagement.SceneManager", "SceneManagement.Scene", "AI.NavMeshAgent", "AI.NavMesh", "Networking.UnityWebRequest",
    "SerializeField", "HeaderAttribute", "TooltipAttribute", "RangeAttribute", "RequireComponent", "ExecuteAlways",
    "CreateAssetMenuAttribute", "HideInInspector", "SerializeReference", "DisallowMultipleComponent",
    "AddComponentMenu", "ContextMenu", "Profiling.Profiler", "SystemInfo", "Graphics", "ComputeShader",
    "Texture", "Font", "TextAsset", "AssetBundle", "Terrain", "WheelCollider", "Joint", "HingeJoint", "FixedJoint",
    "ConfigurableJoint", "ConstantForce", "PhysicsMaterial", "Tilemaps.Tilemap", "Tilemaps.Tile", "Grid",
    "UIElements.VisualElement", "UIElements.UIDocument", "UIElements.Button", "UIElements.Label",
    "UnityEditor.Editor", "EditorWindow", "EditorGUILayout", "EditorGUI", "EditorUtility", "AssetDatabase",
    "PrefabUtility", "Undo", "Selection", "MenuItem", "CustomEditor", "SerializedObject", "SerializedProperty",
    "Handles", "BuildPipeline", "PlayerSettings", "EditorApplication", "InitializeOnLoadAttribute",
    "Unity.Collections.NativeArray_1", "Unity.Jobs.IJob", "Unity.Jobs.IJobParallelFor", "Unity.Jobs.JobHandle",
    "Pool.ObjectPool_1", "Events.UnityEvent", "Space", "ForceMode", "QueryTriggerInteraction", "PrimitiveType",
]
UNITY_MEMBER_HEADINGS = {"properties", "public methods", "static methods", "messages", "static properties",
                         "events", "operators", "constructors", "delegates"}
UNITY_MANUAL_SECTIONS = [  # TOC links of the Manual chapters to include (each with its sub-pages), most useful first
    ("first-time-user", 20), ("scripting", 220), ("working-with-gameobjects", 120), ("working-with-scenes", 30),
    ("PhysicsSection", 100), ("Unity2D", 270), ("Quickstart3D", 5), ("Cameras", 80), ("Input", 10),
    ("AnimationSection", 50), ("Audio", 60), ("assets-and-media", 110), ("unity-editor", 140), ("UIToolkits", 300),
    ("analysis", 150), ("building-and-publishing", 30), ("Windows", 25), ("LightingOverview", 170),
    ("materials-and-shaders", 300), ("visual-effects", 110), ("render-pipelines", 150),
    ("post-processing-and-full-screen-effects", 40), ("CreatingEnvironments", 80), ("PackagesList", 40),
    ("multiplayer", 5), ("XR", 35), ("best-practice-guides", 5), ("TroubleShooting", 5),
]


def unity_page(rel):
    try:
        return fetch(UNITY + rel, os.path.join("unity", safe_name(rel.replace("/", "__"))), pause=0.2)
    except Exception as e:  # a page that can't be reached is skipped; the rest still make the file
        print(f"  couldn't download {rel}: {e}", file=sys.stderr)
        return None


def unity_content(markup):
    """The part of a docs page between its title and the feedback box and footer at the bottom."""
    start = max(markup.find("<h1"), 0)
    ends = [markup.find(m, start) for m in ("Did you find this page useful", "Is something described here not working",
                                            '<div class="footer', "Copyright ©")]
    ends = [e for e in ends if e > 0]
    return markup[start:min(ends) if ends else len(markup)]


def unity_signatures(content):
    """A Scripting API page's C# declarations, and the page without them (so they aren't read as description)."""
    sigs = []

    def take(m):
        text = re.sub(r"<h2>Declaration</h2>", "", m.group(1))
        text = html.unescape(re.sub(r"<[^>]+>", "", text))
        text = re.sub(r"\s+", " ", text).strip()
        if text:
            sigs.append(text if text.endswith((";", "}")) else text + ";")
        return ""

    content = re.sub(r'<div class="signature-CS sig-block">(.*?)</div>', take, content, flags=re.S)
    return sigs, content


def unity_kind(markup):
    """'MonoBehaviour is a class in UnityEngine. Inherits from: Behaviour.' from the line under a class page's title."""
    m = re.search(r'<p class="cl mb0 left mr10">\s*(class|struct|enumeration|interface|attribute)\s+in\s+([^<]+)</p>',
                  markup)
    if not m:
        return None, ""
    kind, ns = m.group(1), m.group(2).strip()
    parent = re.search(r'Inherits from:\s*<a[^>]*>([^<]+)</a>', markup)
    return kind, f"It is {'an' if kind[0] in 'aeiou' else 'a'} {kind} in {ns}" + (
        f" (inherits from {parent.group(1)})." if parent else ".")


def unity_member_answer(title, markup):
    """A Scripting API page (a class or one member) as an answer: description, declarations, parameters, example."""
    sigs, content = unity_signatures(unity_content(markup))
    items = parse_blocks(content, skip_classes={"cl"})
    buckets = {"description": [], "declaration": sigs, "parameters": [], "returns": [], "code": []}
    members = {}
    mode = "description"
    for it in items:
        if it[0] == "h":
            h = it[2].lower()
            if h.startswith("inherited members"):
                break
            if h in UNITY_MEMBER_HEADINGS:
                mode = "members:" + it[2]
            elif h.startswith("additional resources"):
                mode = "skip"
            elif h in ("parameters", "returns", "description"):
                mode = h
            continue
        if it[0] == "pre":
            buckets["code"].append(it[1])
        elif mode.startswith("members:"):
            if it[0] == "row" and len(it[1]) >= 2 and it[1][0].lower() not in ("property", "method", "message", "event",
                                                                                  "operator", "constructor", "delegate"):
                members.setdefault(mode[8:], []).append(it[1])
        elif mode == "parameters":
            if it[0] == "row" and len(it[1]) >= 2 and it[1][0].lower() != "parameter":
                buckets["parameters"].append(f"- `{it[1][0]}`: {it[1][1]}")
        elif mode == "returns":
            buckets["returns"].append(it[-1] if it[0] != "row" else " ".join(it[1]))
        elif it[0] in ("p", "li") and mode != "skip":
            text = it[1]
            if text.startswith(("Switch to Manual", "Leave feedback", "Suggest a change", "Additional resources")):
                continue
            if it[0] == "li":
                text = "- " + text
            if text not in buckets["description"]:
                buckets["description"].append(text)
    parts = []
    kind, kind_line = unity_kind(markup)
    desc = "\n\n".join(buckets["description"])
    if kind_line:
        desc = (desc + "\n\n" if desc else "") + f"`{title}`: " + kind_line
    if len(desc) > 2200:
        desc = desc[:desc.rfind("\n\n", 0, 2200)] if desc.rfind("\n\n", 0, 2200) > 300 else desc[:2200]
    if desc:
        parts.append(desc)
    decls = list(dict.fromkeys(buckets["declaration"]))
    if decls:
        parts.append("```csharp\n" + "\n".join(decls[:6]) + "\n```")
    if buckets["parameters"]:
        parts.append("Parameters:\n" + "\n".join(dict.fromkeys(buckets["parameters"])))
    if buckets["returns"]:
        parts.append("Returns: " + " ".join(buckets["returns"]))
    for name, rows in members.items():
        listed = rows[:30]
        parts.append(f"{name}:\n" + "\n".join(f"- `{r[0]}`: {r[1]}" for r in listed)
                     + (f"\n- ... and {len(rows) - 30} more" if len(rows) > 30 else ""))
    size = sum(map(len, parts))
    for code in buckets["code"][:2]:
        if size + len(code) < 5200:
            parts.append("Example:\n```csharp\n" + code + "\n```")
            size += len(code)
    return "\n\n".join(parts).strip(), kind


def unity_member_links(markup):
    """The member pages a class page links to, in its own (not inherited) member tables."""
    body = unity_content(markup)
    cut = body.find("Inherited Members")
    body = body[:cut if cut > 0 else len(body)]
    return list(dict.fromkeys(re.findall(r'<td class="lbl"><a href="([^"#]+\.html)"', body)))


def unity_examples(rng, small):
    print("Unity: Scripting API ...", flush=True)
    out = []
    classes = UNITY_CLASSES[:6] if small else UNITY_CLASSES
    member_cap = 6 if small else 60
    for cls in classes:
        markup = unity_page(f"ScriptReference/{cls}.html")
        if not markup:
            print(f"  no Scripting API page for {cls}", file=sys.stderr)
            continue
        title = cls.replace("_1", "<T>")
        answer, kind = unity_member_answer(title, markup)
        kind = kind or "class"
        if answer:
            short = title.split(".")[-1]
            out.append(example(rng.choice([f"What is {short} in Unity?", f"Explain Unity's {short} {kind}.",
                                           f"What does the {title} {kind} do in Unity, and what are its main members?"]),
                               answer))
        for link in unity_member_links(markup)[:member_cap]:
            page = unity_page("ScriptReference/" + link)
            if not page:
                continue
            member = link[:-5].replace("-", ".").replace("_1", "<T>")
            answer, _ = unity_member_answer(member, page)
            if len(answer) < 60:
                continue
            q = rng.choice([f"How do I use {member} in Unity?", f"What does {member} do in Unity?",
                            f"Explain {member} in the Unity Scripting API.", f"What is {member} in Unity and how is it used?"])
            out.append(example(q, answer))
    print(f"  {len(out):,} examples from the Scripting API; Manual ...", flush=True)

    toc_text = unity_page("Manual/docdata/toc.json")
    pages = []
    if toc_text:
        toc = json.loads(toc_text)

        def find(node, link):
            if node.get("link") == link:
                return node
            for c in node.get("children") or []:
                f = find(c, link)
                if f:
                    return f
            return None

        def walk(node, acc):
            if node.get("link") and node["link"] != "null":
                acc.append((node["link"], node.get("title", "").strip()))
            for c in node.get("children") or []:
                walk(c, acc)
            return acc

        for link, cap in UNITY_MANUAL_SECTIONS:
            node = find(toc, link)
            if node:
                pages += walk(node, [])[:(5 if small else cap)]
        pages = list(dict.fromkeys(pages))
        if small:
            pages = pages[:40]
    for link, toc_title in pages:
        markup = unity_page(f"Manual/{link}.html")
        if not markup:
            continue
        items = parse_blocks(unity_content(markup))
        title = next((it[2] for it in items if it[0] == "h" and it[1] == 1), toc_title) or toc_title
        in_unity = "" if "unity" in title.lower() else " in Unity"
        secs = sections(items, max_level=3)
        total = sum(prose_size(b) for _, b in secs)
        if total < 200:
            continue
        if total < 3000:  # a short page: one answer
            blocks = []
            for path, b in secs:
                if len(path) > 1:
                    blocks.append(("h", 3, path[-1]))
                blocks += b
            q = rng.choice([f"Explain {title}{in_unity}.", f"What should I know about {title}{in_unity}?",
                            f"Unity: tell me about {title}."])
            out.append(example(q, render(blocks, max_chars=3600)))
            continue
        for path, b in secs:
            if prose_size(b) < 200 or (path and path[-1].lower().startswith("additional resources")):
                continue
            if len(path) <= 1:
                q = rng.choice([f"Explain {title}{in_unity}.", f"What should I know about {title}{in_unity}?"])
            else:
                q = rng.choice([f"Unity, {title}: what about \"{path[-1]}\"?", f"Unity {title}: explain \"{path[-1]}\"."])
            out.append(example(q, render(b, max_chars=3000)))

    glossary = unity_page("Manual/Glossary.html")
    if glossary:
        items = parse_blocks(unity_content(glossary))
        term = None
        for it in items:
            if it[0] == "h" and it[1] == 4:
                term = it[2].rstrip(":").strip()
            elif term and it[0] == "p" and len(it[1]) > 20:
                out.append(example(rng.choice([f"What does \"{term}\" mean in Unity?", f"In Unity, what is {term}?"]),
                                   it[1]))
                term = None
    out += wikipedia_examples(["Unity (game engine)"], "", rng)
    write("unity.jsonl", out)


# ---------------------------------------------------------------------------------------------- main

def main():
    global REFRESH, PAUSE, NO_WIKIPEDIA
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--only", default="pokemon,thomas,unity", help="which to build: pokemon, thomas, unity")
    ap.add_argument("--small", action="store_true", help="a few pages of each, for a quick test")
    ap.add_argument("--refresh", action="store_true", help="download everything again")
    ap.add_argument("--pause", type=float, default=0.3, help="seconds to wait between downloads (default 0.3)")
    ap.add_argument("--no-wikipedia", action="store_true", help="skip the Wikipedia articles")
    ap.add_argument("--seed", type=int, default=11)
    args = ap.parse_args()
    NO_WIKIPEDIA = args.no_wikipedia
    REFRESH = args.refresh
    PAUSE = args.pause
    rng = random.Random(args.seed)
    builders = {"pokemon": pokemon_examples, "thomas": thomas_examples, "unity": unity_examples}
    for name in [s.strip() for s in args.only.split(",") if s.strip()]:
        if name not in builders:
            sys.exit(f"--only: unknown '{name}' (use pokemon, thomas, unity)")
        builders[name](random.Random(rng.random()), args.small)
    print("done; now run build_dataset.py to rebuild train.jsonl with these included")


if __name__ == "__main__":
    main()
