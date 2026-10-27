"""Local game server for the Unity front end.

    python server.py                 # listens on http://127.0.0.1:8765
    python server.py --port 9000

Runs entirely on your machine: Unity sends your choices here, this process
runs the rules engine and the bot, and sends the board back as JSON.

Endpoints (JSON in, JSON out):
    GET  /info                          decks and bots you can pick
    POST /new     {deck, bot_deck, bot, watch, seed}
    POST /choose  {game, option}        answer the decision shown to you
    POST /advance {game}                let the bot make its next decision
    POST /state   {game}
Every reply is a full state: both boards from your side, your hand, the
log, and the pending decision (with each option's label and what it points at).
"""
import argparse
import json
import os
import random
import threading
import uuid
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

import torch

from tcg.engine import Game
from tcg.deckio import load_deck, deck_names
from tcg.carddb import SYMBOL
from bot.agents import make_agent

HUMAN = 0
_lock = threading.Lock()
_games = {}
_agents = {}


def bot_options():
    out = []
    for path in ("checkpoints/best.pt", "models/bot_126k.pt", "models/bot_53k.pt", "models/bot_venusaur.pt",
                 "checkpoints/latest.pt"):
        if os.path.exists(path):
            out.append(path + ":greedy")
    return out + ["heuristic", "random"]


def get_agent(spec):
    if spec not in _agents:
        _agents[spec] = make_agent(spec)
    return _agents[spec]


# ------------------------------------------------------------------ JSON views
def card_json(c):
    if c is None:
        return None
    if c.is_pokemon:
        lines = [f"{c.hp} HP  {'/'.join(['Grass', 'Fire', 'Water', 'Lightning', 'Psychic', 'Fighting', 'Darkness', 'Metal', 'Dragon', 'Colorless'][t] for t in c.types)}"]
        if c.evolves_from:
            lines.append(f"Evolves from {c.evolves_from}")
        for a in c.abilities:
            lines.append(f"[Ability] {a.name}: {a.text}")
        for a in c.attacks:
            cost = "".join(SYMBOL[t] for t in a.cost) or "-"
            lines.append(f"[{cost}] {a.name} {a.base or ''}{a.mod}: {a.text}".rstrip(": "))
        lines.append(f"Weakness: {','.join(SYMBOL[t] for t in c.weakness) or '-'}  Retreat: {c.retreat}")
        kind = "Pokemon"
    else:
        lines = [c.text] if c.text else []
        kind = c.trainer_kind or ("Energy" if c.is_energy else c.supertype)
    image = c.image or ""
    return {"id": c.id, "name": c.name, "kind": kind, "image": image,
            "image_large": image.replace(".png", "_hires.png") if image else "", "text": "\n".join(lines)}


def mon_json(g, m):
    units = []
    for e in m.energy:
        for u in g.card_units(m, e):
            units.append(SYMBOL[next(iter(u))] if len(u) == 1 else "*")
    return {"uid": m.uid, "card": card_json(m.card), "hp": g.max_hp(m), "damage": m.damage, "energy": units,
            "tool": [card_json(m.tool)] if m.tool else [], "conditions": m.conditions}


def side_json(g, p, show_hand):
    return {
        "prizes": len(p.prizes), "deck": len(p.deck), "discard": len(p.discard),
        "discard_top": [card_json(p.discard[-1])] if p.discard else [],
        "hand_count": len(p.hand),
        "hand": [card_json(c) for c in p.hand] if show_hand else [],
        "active": [mon_json(g, p.active)] if p.active else [],
        "bench": [mon_json(g, m) for m in p.bench],
    }


def slot_ref(g, mon, viewer):
    side = "you" if mon.owner == viewer else "opp"
    p = g.players[mon.owner]
    return f"{side}:0" if mon is p.active else f"{side}:{p.bench.index(mon) + 1}" if mon in p.bench else ""


def state_json(s):
    g, viewer = s["game"], HUMAN
    d = g.decision
    decision = None
    if d is not None:
        opts = []
        for i, o in enumerate(d.options):
            target = slot_ref(g, o.target, viewer) if o.target is not None and g.in_play(o.target) else ""
            # "hand": the option plays this card from your hand, so clicking that hand card picks it
            from_hand = o.card is not None and o.kind in ("bench", "evolve", "attach", "tool", "play")
            # Choosing cards out of your own hand (setup, Ultra Ball discards...) works the same way
            hand_pick = (o.kind == "card" and any(c is o.card for c in g.players[viewer].hand)
                         and (d.kind in ("setup_active", "setup_bench") or (d.kind == "discard" and d.source)))
            card_ref = "hand" if (from_hand or hand_pick) and d.player == viewer else ""
            opts.append({"index": i, "kind": o.kind, "label": g.describe(o), "card": [card_json(o.card)] if o.card else [],
                         "target": target, "card_ref": card_ref})
        decision = {"player": "you" if d.player == viewer else "opp", "kind": d.kind,
                    "prompt": d.prompt, "source": [card_json(d.source)] if d.source else [], "options": opts}
    you, opp = g.players[viewer], g.players[1 - viewer]
    result = ""
    if g.done:
        result = "Draw" if g.winner is None else ("You win!" if g.winner == viewer else "The bot wins")
        result += f" ({g.end_reason})"
    return {
        "game": s["id"], "turn": g.turn, "your_turn": g.current == viewer and not g.in_setup,
        "in_setup": g.in_setup, "done": g.done, "result": result,
        "you": side_json(g, you, True), "opp": side_json(g, opp, s["watch"]),
        "stadium": [card_json(g.stadium)] if g.stadium is not None else [],
        "stadium_owner": "" if g.stadium is None else ("you" if g.stadium_owner == viewer else "opp"),
        "your_deck": s["deck"], "bot_deck": s["bot_deck"], "bot": s["bot"], "watch": s["watch"],
        "waiting_on": "" if d is None else ("you" if d.player == viewer and not s["watch"] else "bot"),
        "log": g.log, "decision": [decision] if decision else [],
    }


# ------------------------------------------------------------------- actions
def new_game(req):
    decks = deck_names()
    if req.get("seed") is not None and req["seed"] < 0:
        req["seed"] = None  # Unity sends -1 for "random"
    rng = random.Random(req.get("seed"))
    deck = req.get("deck") or rng.choice(decks)
    bot_deck = req.get("bot_deck") or rng.choice(decks)
    bot = req.get("bot") or bot_options()[0]
    get_agent(bot)
    g = Game(load_deck(deck), load_deck(bot_deck), seed=req.get("seed"), log=True)
    s = {"id": uuid.uuid4().hex[:12], "game": g, "deck": deck, "bot_deck": bot_deck, "bot": bot,
         "watch": bool(req.get("watch"))}
    _games[s["id"]] = s
    return s


def choose(s, option):
    g = s["game"]
    if g.done or s["watch"] or g.actor() != HUMAN:
        raise ValueError("it's not your decision")
    if not 0 <= option < len(g.decision.options):
        raise ValueError("no such option")
    g.step(option)


def advance(s):
    """One bot decision (either seat in watch mode)."""
    g = s["game"]
    if g.done or (g.actor() == HUMAN and not s["watch"]):
        return
    g.step(get_agent(s["bot"]).act([g])[0])


class Handler(BaseHTTPRequestHandler):
    def _send(self, code, obj):
        body = json.dumps(obj).encode()
        self.send_response(code)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def log_message(self, fmt, *args):
        pass

    def do_GET(self):
        if self.path.startswith("/info"):
            self._send(200, {"decks": deck_names(), "bots": bot_options()})
        else:
            self._send(404, {"error": "unknown endpoint"})

    def do_POST(self):
        try:
            n = int(self.headers.get("Content-Length") or 0)
            req = json.loads(self.rfile.read(n) or b"{}")
            with _lock:
                if self.path == "/new":
                    s = new_game(req)
                else:
                    s = _games.get(req.get("game"))
                    if s is None:
                        return self._send(404, {"error": "unknown game"})
                    if self.path == "/choose":
                        choose(s, int(req["option"]))
                    elif self.path == "/advance":
                        advance(s)
                    elif self.path != "/state":
                        return self._send(404, {"error": "unknown endpoint"})
                self._send(200, state_json(s))
        except Exception as e:  # report problems to the client instead of dropping the connection
            self._send(400, {"error": f"{type(e).__name__}: {e}"})


def main():
    os.chdir(os.path.dirname(os.path.abspath(__file__)))  # find models/ and decks/ from anywhere
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--host", default="127.0.0.1")
    ap.add_argument("--port", type=int, default=8765)
    args = ap.parse_args()
    torch.set_num_threads(2)
    print(f"Pokemon TCG server on http://{args.host}:{args.port}  (bots: {', '.join(bot_options())})")
    print("Leave this running and press Play in Unity. Ctrl+C to stop.")
    ThreadingHTTPServer((args.host, args.port), Handler).serve_forever()


if __name__ == "__main__":
    main()
