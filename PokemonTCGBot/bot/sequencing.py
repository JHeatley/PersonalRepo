"""Spot common turn-sequencing mistakes.

Used by tools/sequencing.py to measure a bot, and by self-play to give a
small penalty for each mistake (train.py --sequence-penalty).

  shuffled with plays left: played a hand-shuffling Supporter (Lillie's
      Determination, Judge, Unfair Stamp...) while Items, Basics to bench or
      an Energy attachment were still available. Those cards go back into
      the deck unused.
  attacked with resources left: attacked (which ends the turn) while an
      Energy attachment, an Ability or a Basic to bench was still available.
  drew then shuffled: used a draw/search Ability (Recon Directive, Run
      Errand...) and then shuffled the hand away in the same turn.
"""
HAND_SHUFFLERS = {"Lillie's Determination", "Judge", "Unfair Stamp", "Team Rocket's Archer"}
DRAW_ABILITIES = {"Recon Directive", "Run Errand", "Flip the Script", "Teal Dance", "Trade", "Lunar Cycle",
                  "Boom Boom Groove", "Attract Customers", "Run Away Draw", "Last-Ditch Catch"}
USE_FIRST = ("play", "bench", "attach", "evolve", "tool")
MISTAKES = ("shuffled with plays left", "attacked with resources left", "drew then shuffled")


class TurnTracker:
    """Follows one player's main-phase choices through a game."""

    def __init__(self):
        self.turn, self.drew = None, False

    def events(self, g, o):
        """What picking option o at g.decision counts as: actions taken and mistakes made."""
        d = g.decision
        if d.kind != "main" or d.player != g.current:
            return []
        if g.turn != self.turn:
            self.turn, self.drew = g.turn, False
        others = [x for x in d.options if x is not o]
        out = []
        if o.kind == "play" and o.card.name in HAND_SHUFFLERS:
            out.append("shuffles")
            if any(x.kind in USE_FIRST and x.card is not o.card for x in others):
                out.append("shuffled with plays left")
            if self.drew:
                out.append("drew then shuffled")
        elif o.kind == "attack":
            out.append("attacks")
            if any(x.kind in ("attach", "ability", "bench") for x in others):
                out.append("attacked with resources left")
        elif o.kind == "ability" and o.data.name in DRAW_ABILITIES:
            out.append("draw abilities")
            self.drew = True
        return out


def mistakes(events):
    return sum(e in MISTAKES for e in events)
