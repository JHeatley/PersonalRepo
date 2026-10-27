"""Build data/cards.json from the open PokemonTCG card database.

    python tools/import_cards.py            # download sets, then build
    python tools/import_cards.py --offline  # rebuild from data/raw only

The card pool is every printing with regulation mark H, I or J (Temporal
Forces through 30th Celebration) plus Basic Energy. Reprints with identical
game text collapse into one card that remembers all of its printings, so a
decklist can name any of them.
"""
import argparse
import json
import os
import urllib.request

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RAW = os.path.join(ROOT, "data", "raw")
OUT = os.path.join(ROOT, "data", "cards.json")
BASE = "https://raw.githubusercontent.com/PokemonTCG/pokemon-tcg-data/master"

# Release order matters: card ids are assigned in this order, and the network
# learns one embedding per id. Append new sets at the end.
SETS = ["sve", "sv5", "sv6", "sv6pt5", "sv7", "sv8", "sv8pt5", "sv9", "sv10", "zsv10pt5",
        "rsv10pt5", "me1", "me2", "me2pt5", "me3", "me4", "me5", "me55", "me55c"]
LEGAL_MARKS = {"H", "I", "J"}


def download():
    os.makedirs(RAW, exist_ok=True)
    for name in ["sets"] + SETS:
        url = f"{BASE}/sets/en.json" if name == "sets" else f"{BASE}/cards/en/{name}.json"
        print("fetching", url)
        with urllib.request.urlopen(url) as r, open(os.path.join(RAW, name + ".json"), "wb") as f:
            f.write(r.read())


def _legal(c):
    if c["supertype"] == "Energy" and "Basic" in c.get("subtypes", []):
        return True
    return c.get("regulationMark") in LEGAL_MARKS


REMINDER_PREFIXES = (
    "You may play any number of Item", "You may play only 1 Supporter", "You may play only 1 Stadium",
    "You may attach any number of Pokémon Tools", "Attach a Pokémon Tool to 1 of your Pokémon",
    "You can't have more than 1 ACE SPEC", "ACE SPEC: You can't have more than 1",
)


def _norm(text):
    text = text.replace("’", "'").replace("‘", "'").replace("“", '"').replace("”", '"').replace("V_atk", "V")
    return " ".join(text.split())


# Rule-box reminders carry no extra information beyond the subtypes
RULE_BOX_PREFIXES = ("Pokémon ex rule", "Mega Evolution ex Rule", "Tera:")
TERA_TEXT = "As long as this Pokémon is on your Bench, prevent all damage done to this Pokémon by attacks"


def _rules(c):
    out = []
    for r in map(_norm, c.get("rules", [])):
        if not r.startswith(REMINDER_PREFIXES + RULE_BOX_PREFIXES) and r not in out:
            out.append(r)
    return out


def _subtypes(c, rules):
    subs = [s for s in c.get("subtypes", []) if s != "Tera"]
    if any(r.startswith(TERA_TEXT) for r in rules):
        subs.append("Tera")
    return subs


def _gameplay(c):
    out = {
        "name": c["name"],
        "supertype": {"Pokémon": "Pokemon"}.get(c["supertype"], c["supertype"]),
    }
    rules = _rules(c)
    out["subtypes"] = _subtypes(c, rules)
    if c["supertype"] == "Pokémon":
        out.update({
            "hp": int(c.get("hp", 0)),
            "types": c.get("types", []),
            "evolvesFrom": c.get("evolvesFrom"),
            "abilities": [{"name": a["name"], "type": a.get("type", "Ability"), "text": _norm(a.get("text", ""))}
                          for a in c.get("abilities", [])],
            "attacks": [{"name": a["name"], "cost": [t for t in a.get("cost", []) if t != "Free"], "damage": a.get("damage", ""),
                         "text": _norm(a.get("text", ""))} for a in c.get("attacks", [])],
            "weaknesses": [w["type"] for w in c.get("weaknesses", [])],
            "resistances": [r["type"] for r in c.get("resistances", [])],
            "retreat": c.get("convertedRetreatCost", 0),
        })
    out["rules"] = rules
    return out


def build():
    with open(os.path.join(RAW, "sets.json")) as f:
        codes = {s["id"]: s.get("ptcgoCode") for s in json.load(f)}
    cards, by_key = [], {}
    for set_id in SETS:
        with open(os.path.join(RAW, set_id + ".json")) as f:
            printings = json.load(f)
        for c in printings:
            if not _legal(c):
                continue
            g = _gameplay(c)
            key = json.dumps(g, sort_keys=True)
            if key not in by_key:
                g["id"] = len(cards)
                g["regulationMark"] = c.get("regulationMark")
                g["image"] = c.get("images", {}).get("small")
                g["printings"] = []
                by_key[key] = g
                cards.append(g)
            by_key[key]["printings"].append([codes.get(set_id) or set_id, c["number"]])
    with open(OUT, "w") as f:
        json.dump(cards, f, ensure_ascii=False, separators=(",", ":"))
    kinds = {}
    for c in cards:
        kinds[c["supertype"]] = kinds.get(c["supertype"], 0) + 1
    print(f"wrote {len(cards)} cards to {OUT}: {kinds}")


if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--offline", action="store_true", help="don't download, use data/raw")
    args = ap.parse_args()
    if not args.offline:
        download()
    build()
