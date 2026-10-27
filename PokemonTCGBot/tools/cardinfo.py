"""Look up cards in the pool.   python tools/cardinfo.py "Dragapult ex" ultra"""
import json
import os
import sys

SYMBOL = {"Grass": "G", "Fire": "R", "Water": "W", "Lightning": "L", "Psychic": "P", "Fighting": "F",
          "Darkness": "D", "Metal": "M", "Dragon": "N", "Fairy": "Y", "Colorless": "C"}
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))


def show(c):
    head = f"#{c['id']} {c['name']} [{', '.join(c['subtypes'])}]"
    if c["supertype"] == "Pokemon":
        head += f" {c['hp']}HP {'/'.join(c['types'])}"
        if c.get("evolvesFrom"):
            head += f" (from {c['evolvesFrom']})"
        head += f" weak:{','.join(c['weaknesses']) or '-'} resist:{','.join(c['resistances']) or '-'} retreat:{c['retreat']}"
    print(head, " ".join(f"{s} {n}" for s, n in c["printings"]))
    for a in c.get("abilities", []):
        print(f"   [{a['type']}] {a['name']}: {a['text']}")
    for a in c.get("attacks", []):
        cost = "".join(SYMBOL[t] for t in a["cost"]) or "-"
        print(f"   {cost} {a['name']} {a['damage']}: {a['text']}")
    for r in c.get("rules", []):
        print("   *", r)


if __name__ == "__main__":
    with open(os.path.join(ROOT, "data", "cards.json")) as f:
        cards = json.load(f)
    for q in sys.argv[1:]:
        for c in cards:
            if q.lower() in c["name"].lower():
                show(c)
