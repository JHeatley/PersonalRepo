"""Builds training data for a Pokémon TCG language model (train.jsonl) from:

  * the Pokémon TCG Rulings Compendium (compendium.pokegym.net): every question and answer, downloaded when you run this
    (the rulings are the Compendium team's work, so they aren't kept in this project; run this yourself to get them),
  * this project's card pool (Assets/PokemonTCG/Resources/CardData): questions about what cards and their attacks do.

Each line of train.jsonl is one conversation: {"messages": [system, user, assistant]}, the format the training notebook
(train_colab.ipynb) reads. Only Python's standard library is needed:

    python build_dataset.py                     # writes train.jsonl next to this file
    python build_dataset.py --cards 6000        # more card questions (default 4000 cards; 0 for none)
    python build_dataset.py --no-download       # card questions only (or reuse the pages saved in ./compendium)

The downloaded pages are saved in ./compendium, so running it again doesn't download them again (--refresh does).
"""
import argparse
import glob
import html
import json
import os
import random
import re
import sys
import urllib.request
from html.parser import HTMLParser

HERE = os.path.dirname(os.path.abspath(__file__))
RESOURCES = os.path.join(HERE, "..", "Assets", "PokemonTCG", "Resources")

SYSTEM = ("You are an expert on the Pokémon Trading Card Game: its rules, official rulings and cards. Answer clearly and "
          "accurately. When a ruling comes from the Rulings Compendium, give its source and date.")

# The older Compendium pages (one per era) and the current site's list of every ruling.
LEGACY_PAGES = {
    "compendium.html": "Wizards of the Coast era (Base Set to Skyridge)",
    "compendium-ex.html": "EX era",
    "compendium-lvx.html": "Diamond & Pearl / Platinum / HeartGold SoulSilver era",
    "compendium-bw.html": "Black & White era",
}
MODERN_PAGE = "all-rulings-by-category/"
BASE_URL = "https://compendium.pokegym.net/"

COST_NAMES = {"G": "Grass", "R": "Fire", "W": "Water", "L": "Lightning", "P": "Psychic", "F": "Fighting",
              "D": "Darkness", "M": "Metal", "C": "Colorless", "N": "Dragon", "Y": "Fairy"}


# ---------------------------------------------------------------------------------------------- downloading

def fetch(page, folder, refresh):
    path = os.path.join(folder, page.strip("/").replace("/", "_") + ("" if page.endswith(".html") else ".html"))
    if os.path.exists(path) and not refresh:
        return open(path, encoding="utf-8", errors="replace").read()
    print(f"downloading {BASE_URL + page} ...", flush=True)
    req = urllib.request.Request(BASE_URL + page, headers={"User-Agent": "Mozilla/5.0 (PokemonTCGSim dataset builder)"})
    raw = urllib.request.urlopen(req, timeout=120).read()
    text = raw.decode("utf-8", errors="replace")
    if "�" in text[:200000]:
        text = raw.decode("cp1252", errors="replace")  # the older pages aren't UTF-8
    os.makedirs(folder, exist_ok=True)
    with open(path, "w", encoding="utf-8") as f:
        f.write(text)
    return text


# ---------------------------------------------------------------------------------------------- parsing

BLOCK_TAGS = {"p", "div", "br", "h1", "h2", "h3", "h4", "h5", "li", "blockquote", "tr", "dt", "dd", "section", "table"}


class TextLines(HTMLParser):
    """HTML to text, one line per block (paragraph, heading, line break...)."""

    def __init__(self):
        super().__init__(convert_charrefs=True)
        self.parts = []
        self.skip = 0

    def handle_starttag(self, tag, attrs):
        if tag in ("script", "style"):
            self.skip += 1
        elif tag in BLOCK_TAGS:
            self.parts.append("\n")

    def handle_endtag(self, tag):
        if tag in ("script", "style"):
            self.skip = max(0, self.skip - 1)
        elif tag in BLOCK_TAGS:
            self.parts.append("\n")

    def handle_data(self, data):
        if not self.skip:
            self.parts.append(data)

    def lines(self):
        out = []
        for line in "".join(self.parts).split("\n"):
            line = re.sub(r"\s+", " ", line).strip()
            if line:
                out.append(line)
        return out


def clean(text):
    text = html.unescape(text).replace("Pok�mon", "Pokémon").replace("Pokemon", "Pokémon")
    return re.sub(r"\s+", " ", text).strip()


def legacy_rulings(text, era):
    """Q./A. pairs of an older Compendium page, with the heading they're under (a card or rule name) as their topic."""
    parser = TextLines()
    parser.feed(text)
    lines = parser.lines()
    pairs = []
    topic = None
    since_heading = 0
    i = 0
    while i < len(lines):
        line = lines[i]
        if re.match(r"^Q[.:]", line):
            # The question runs until the answer; the answer until the next question or heading.
            q = [re.sub(r"^Q[.:]\s*", "", line)]
            i += 1
            while i < len(lines) and not re.match(r"^A[.:]", lines[i]) and not re.match(r"^Q[.:]", lines[i]):
                q.append(lines[i])
                i += 1
            if i >= len(lines) or not re.match(r"^A[.:]", lines[i]):
                continue
            a = [re.sub(r"^A[.:]\s*", "", lines[i])]
            i += 1
            # The answer ends with its source in brackets, "(Feb 9, 2006 PUI Rules Team)"; stop there or at the next heading.
            while (i < len(lines) and not ENDS_WITH_SOURCE.search(" ".join(a)) and not re.match(r"^[QA][.:]", lines[i])
                   and not looks_like_heading(lines[i])):
                a.append(lines[i])
                i += 1
            question, answer = clean(" ".join(q)), clean(" ".join(a))
            # A heading the parser missed would leave the last topic on a long run of unrelated rulings: a topic only
            # labels the few rulings right under its heading.
            since_heading += 1
            if 10 <= len(question) <= 1500 and 2 <= len(answer) <= 2500:
                pairs.append((topic if since_heading <= 6 else None, question, answer, era))
            continue
        if looks_like_heading(line):
            t = clean(HEADING_TAIL.sub("", line)).strip("=* ")
            t = re.split(r"\s*\(", t)[0].strip()  # "SHADOW CURSE (Gengar - EX:Legend" -> "SHADOW CURSE"
            t = t.title() if t.isupper() else t
            topic = None if t.lower() in GENERIC_TOPICS or not t[:1].isalnum() else t
            since_heading = 0
        i += 1
    return pairs


ENDS_WITH_SOURCE = re.compile(r"\([^()]*(?:19|20)\d\d[^()]*\)\s*$")
HEADING_TAIL = re.compile(r"\s*\(copy link to this section\)\s*(copied!)?\s*$", re.I)
GENERIC_TOPICS = {"errata", "meta-rulings", "meta rulings", "attacks", "abilities", "pokémon powers", "poké-powers",
                  "poké-bodies", "trainers", "trainer cards", "energy", "energy cards", "general gameplay", "gameplay",
                  "team battle", "two on two gameplay", "team gameplay", "pokémon items", "pokémon attacks",
                  "pokémon powers & abilities", "introduction", "top"}


def looks_like_heading(line):
    """A card or rule name above its rulings: "== EVOLUTIONARY FAN", "SHADOW CURSE", or "Primal Winds (Aerodactyl GX -
    Unified Minds) (copy link to this section)" on the newer pages."""
    if HEADING_TAIL.search(line) or line.startswith("== "):
        return True
    if len(line) > 60 or re.match(r"^[QA][.:]", line) or line.endswith((".", "?", ":", ",", ")")):
        return False
    letters = [ch for ch in line if ch.isalpha()]
    return len(letters) >= 3 and line.isupper()


def modern_rulings(text):
    """The current site: an <h4> per topic, then Q&A entries (<dt>/<dd>) and single statements (errata, notes) with a source."""
    pairs = []
    for block in re.split(r"(?=<h4 )", text):
        m = re.match(r"<h4[^>]*>(?:<a[^>]*>)?(.*?)(?:</a>)?</h4>", block, re.S)
        if not m:
            continue
        topic = clean(re.sub(r"<[^>]+>", "", m.group(1)))
        for entry in re.split(r'(?=<div class="ruling-\d+">)', block)[1:]:
            source = re.search(r"Source:\s*</span>(.*?)</div>", entry, re.S)
            source = clean(re.sub(r"<[^>]+>", " ", source.group(1))) if source else ""
            qas = re.findall(r"<dt>(.*?)</dt>\s*<dd>(.*?)</dd>", entry, re.S)
            if qas:
                for q, a in qas:
                    q, a = clean(re.sub(r"<[^>]+>", " ", q)), clean(re.sub(r"<[^>]+>", " ", a))
                    if q and a:
                        pairs.append((topic, q, a + (f" (Source: {source})" if source else ""), "current Compendium"))
            else:
                for d in re.findall(r"<dd>(.*?)</dd>", entry, re.S):
                    d = clean(re.sub(r"<[^>]+>", " ", d))
                    if d:
                        q = f"Is there any errata or official ruling about {topic}?"
                        pairs.append((topic, q, d + (f" (Source: {source})" if source else ""), "current Compendium"))
    return pairs


# ---------------------------------------------------------------------------------------------- examples

def example(user, assistant):
    return {"messages": [{"role": "system", "content": SYSTEM}, {"role": "user", "content": user},
                         {"role": "assistant", "content": assistant}]}


def ruling_examples(pairs):
    seen = set()
    out = []
    for topic, q, a, era in pairs:
        key = re.sub(r"[^a-z0-9]", "", (q + "|" + a).lower())[:300]
        if key in seen:
            continue
        seen.add(key)
        if topic and topic.lower() not in q.lower() and len(topic) < 60:
            q = f"Ruling question about {topic}: {q}"
        out.append(example(q, a))
    return out


def cost_words(cost):
    if not cost:
        return "no Energy"
    counts = {}
    for ch in cost:
        counts[COST_NAMES.get(ch, ch)] = counts.get(COST_NAMES.get(ch, ch), 0) + 1
    return ", ".join(f"{n} {t}" for t, n in counts.items()) + " Energy"


def damage_words(a):
    dmg, suffix = a.get("damage", 0), a.get("damageSuffix", "")
    if not dmg:
        return "no damage by itself"
    return f"{dmg}{'×' if suffix == 'x' else suffix} damage"


def describe_card(c, set_name):
    where = f"{c['name']} ({set_name} {c['number']})"
    if c["kind"] == "Pokemon":
        types = c.get("type", "")
        if c.get("secondType"):
            types += "/" + c["secondType"]
        stage = {"Basic": "Basic", "Stage1": "Stage 1", "Stage2": "Stage 2"}.get(c.get("stage", ""), c.get("stage", ""))
        parts = [f"{where} is a {stage} {types} Pokémon with {c.get('hp', 0)} HP"
                 + (f" that evolves from {c['evolvesFrom']}" if c.get("evolvesFrom") else "") + "."]
        if c.get("text"):
            parts.append(c["text"])
        for ab in c.get("abilities", []):
            parts.append(f"{ab.get('kind') or 'Ability'} – {ab['name']}: {ab['text']}")
        for a in c.get("attacks", []):
            line = f"Attack – {a['name']} ({cost_words(a.get('cost', ''))}): {damage_words(a)}."
            if a.get("text"):
                line += " " + a["text"]
            parts.append(line)
        w = c.get("weakness")
        r = c.get("resistance")
        parts.append(f"Weakness: {w or 'none'}. Resistance: {r + ' -' + str(c.get('resistanceAmount', 30)) if r else 'none'}. "
                     f"Retreat Cost: {c.get('retreat', 0)}.")
        return " ".join(parts)
    if c["kind"] == "Trainer":
        kind = {"Item": "an Item", "Supporter": "a Supporter", "Stadium": "a Stadium", "Tool": "a Pokémon Tool"}.get(c.get("trainerType", ""), "a Trainer")
        return f"{where} is {kind} card. {c.get('text', '')}".strip()
    if c.get("basicEnergy"):
        return f"{where} is a basic {c.get('provides', '')} Energy card."
    return f"{where} is a Special Energy card. {c.get('text', '')}".strip()


def card_examples(limit, rng):
    names = {}
    printings = os.path.join(RESOURCES, "CardPrintings", "printings.json")
    if os.path.exists(printings):
        for e in json.load(open(printings, encoding="utf-8"))["cards"]:
            for p in e["prints"]:
                names.setdefault(p["set"], p["setName"])
    cards = []
    for f in sorted(glob.glob(os.path.join(RESOURCES, "CardData", "*_pool.json"))):
        cards.extend(json.load(open(f, encoding="utf-8"))["cards"])
    rng.shuffle(cards)
    out = []
    for c in cards[:limit]:
        set_name = names.get(c["set"], c["set"])
        full = describe_card(c, set_name)
        label = f"{c['name']} from {set_name}"
        out.append(example(rng.choice([f"What does {label} do?", f"Tell me about the card {label}.",
                                       f"Explain {c['name']} ({set_name} {c['number']})."]), full))
        if c["kind"] == "Pokemon" and c.get("attacks"):
            a = rng.choice(c["attacks"])
            ans = f"{c['name']}'s {a['name']} costs {cost_words(a.get('cost', ''))} and does {damage_words(a)}."
            if a.get("text"):
                ans += " " + a["text"]
            out.append(example(f"What does {label}'s attack {a['name']} do?", ans))
        for ab in c.get("abilities", [])[:1]:
            out.append(example(f"What does {label}'s {ab.get('kind') or 'Ability'} {ab['name']} do?", ab["text"]))
    return out


# ---------------------------------------------------------------------------------------------- main

def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--out", default=os.path.join(HERE, "train.jsonl"))
    ap.add_argument("--cards", type=int, default=4000, help="how many cards to write questions about (0: none)")
    ap.add_argument("--no-download", action="store_true", help="don't download the Compendium (use saved pages if any)")
    ap.add_argument("--refresh", action="store_true", help="download the Compendium pages again")
    ap.add_argument("--seed", type=int, default=7)
    args = ap.parse_args()
    rng = random.Random(args.seed)
    folder = os.path.join(HERE, "compendium")

    pairs = []
    pages = list(LEGACY_PAGES.items()) + [(MODERN_PAGE, None)]
    for page, era in pages:
        saved = os.path.join(folder, page.strip("/").replace("/", "_") + ("" if page.endswith(".html") else ".html"))
        if args.no_download and not os.path.exists(saved):
            continue
        try:
            text = fetch(page, folder, args.refresh)
        except Exception as e:  # a page that can't be reached is skipped; the rest still make a dataset
            print(f"couldn't download {page}: {e}", file=sys.stderr)
            continue
        found = modern_rulings(text) if era is None else legacy_rulings(text, era)
        print(f"{page}: {len(found):,} rulings")
        pairs.extend(found)

    examples = ruling_examples(pairs)
    n_rulings = len(examples)
    examples += card_examples(args.cards, rng)
    rng.shuffle(examples)
    with open(args.out, "w", encoding="utf-8") as f:
        for ex in examples:
            f.write(json.dumps(ex, ensure_ascii=False) + "\n")
    print(f"wrote {len(examples):,} examples ({n_rulings:,} rulings, {len(examples) - n_rulings:,} card questions) to {args.out}")


if __name__ == "__main__":
    main()
