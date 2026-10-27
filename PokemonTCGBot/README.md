# Pokémon TCG self-play bot

A Pokémon TCG rules engine for the current Standard card pool (Temporal Forces
through 30th Celebration, regulation marks H/I/J) and a neural network bot
that teaches itself to play by grinding hundreds of thousands of games
against itself. When it's trained, you can play against it in the terminal.

## Setup

```
pip install -r requirements.txt
```

## Quick start

```
python play.py                                 # play the included trained bot (models/bot_53k.pt)
python train.py --games 300000                 # train your own by self-play (hours)
python play.py                                 # now plays your checkpoints/best.pt instead
python play.py --watch                         # watch the bot play itself
python evaluate.py checkpoints/best.pt:greedy heuristic --games 1000 --matchups
```

**Unity front end:** run `python server.py` and open `../PokemonTCGUnity` in
Unity 6000.2.8f1 to play on a proper board (see that folder's README).

`play.py` picks random decks unless you pass `--deck` / `--bot-deck`
(a name from `decks/`, or the path of any decklist file).

## How it works

**Cards.** `tools/import_cards.py` pulls every legal card from the open
[pokemon-tcg-data](https://github.com/PokemonTCG/pokemon-tcg-data) project
into `data/cards.json` (2,027 distinct cards; reprints are merged). Look a
card up with `python tools/cardinfo.py "Dragapult ex"`.

**Rules engine (`tcg/`).** The game runs as a generator: every choice a player
makes, whether picking an attack, searching their deck or placing damage
counters, is a `Decision` with a list of `Option`s. Effects are written the
same way, so a card can ask questions in the middle of resolving. Covered:
mulligans, the go-first choice, setup, prizes (1/2/3 for regular / ex /
Mega ex), one energy attachment / Supporter / Stadium / retreat per turn, the
first-turn restrictions, evolution timing, Weakness/Resistance, Tera bench
protection, all five Special Conditions with the checkup, Tools, Stadiums,
special Energy and Abilities.

**Card effects (`tcg/effects/`).**
- `templates.py` compiles common attack wording ("Flip 2 coins. This attack
  does 30 damage for each heads.", "Your opponent's Active Pokémon is now
  Paralyzed.", and so on) into code automatically.
- `trainers.py`, `pokemon.py` and `attachments.py` hold hand-written effects.

`python tools/coverage.py` shows how much of the pool works (about 56% of
all Pokémon and every attack without text, plus each card below), and
`python tools/coverage.py decks` lists anything a deck still needs.

**Decks (`decks/`).** Seventeen meta archetypes, every card fully implemented:
Dragapult (three builds), basic box, Slowking, N's Zoroark, Festival Lead,
Alakazam, Hydrapple, Hide 'n' Sneak, Mega Lopunny, Crustle, Mega Excadrill,
Mega Lucario, Mega Sharpedo, Team Rocket's Honchkrow and Mega Venusaur ex.
Mega Venusaur is a complete list; for the others the core cards come from the
Limitless TEF–30C core lists and the remaining slots are flex picks, marked
in each file's header comment. To use your exact list, paste the
TCG Live / Limitless export into a new `.txt` file in `decks/`. Any card
without an implementation shows up in `tools/coverage.py decks`.

**The bot (`bot/`).** A policy/value network (`model.py`) sees the board,
your hand, both discard piles and the decision being asked (`encode.py`).
It scores each option by combining the game state with that option's
card, kind and target, so one network handles every kind of decision.
`train.py` runs PPO: worker processes play batches of games, mostly
against the current network, sometimes against older snapshots of itself
and the heuristic bot, and the network learns from the results. The reward
is +1 for a win and −1 for a loss, plus a small bonus per prize card swing.

## Included model

`models/bot_53k.pt` has trained for 53,295 self-play games on the 16
original decks (Mega Venusaur ex was added afterwards). Over 1,000 games it
beats the earlier 40k-game model 58.5% of the time (584-2-414) and the
rule-based bot 74.7% (746-2-252), both sides always playing their top-rated
move. It's a starting point: continue training from it with
`python train.py --resume models/bot_53k.pt --games 300000`.

`models/bot_venusaur.pt` is a Mega Venusaur ex specialist: `bot_53k.pt` trained
for 40,000 more games with Venusaur in every game (`--focus-deck venusaur`).
Piloting Venusaur against `bot_53k.pt` on the field it wins 83.0% over 1,000
games (828-4-168); across all decks it's slightly weaker than `bot_53k.pt`
(46.0%), so `bot_53k.pt` stays the default. Pick it with
`python play.py --bot models/bot_venusaur.pt:greedy --bot-deck venusaur` or in
the Unity bot picker.

## Training tips

- Throughput is roughly 5–10 games/sec per CPU core. `--workers` defaults
  to every core but one, so 300k games takes several hours on a laptop.
  Let it run overnight, and use `--resume checkpoints/latest.pt` to continue.
- Watch the `eval vs heuristic` line. It's the greedy network's win rate
  against the rule-based bot on random decks, and it should climb well
  above 0.5.
- `--decks dragapult,basic_box` trains on a subset; specialising helps if
  you mostly want to practise against one matchup.
- `--sequence-penalty 0.02` takes a little reward off for each common
  sequencing mistake (shuffling your hand away with plays left, attacking
  with an attachment or Ability unused...; see `bot/sequencing.py`).
  `python tools/sequencing.py <model>` measures how often a bot makes them.

## Tests

```
python -m pytest
```
