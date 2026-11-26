# Japanese language data (v176)

The game's Japanese option reads two files in `Assets/PokemonTCG/Resources/Lang`:

- `ui_ja.txt`: the screens. One line per English template, a tab, the Japanese. `{0}`, `{1}`... are the parts the code fills in
  (the Japanese may reorder them). Edit this file directly to change or add a translation.
- `names_ja.txt`: official Japanese names (`species`, `card`, `attack`, `ability`, `trainer`, `text`, `word` lines; `text` is a card effect text, the official Japanese text of a matching printing, checked by `textcheck.py`). Built by these scripts.
  `match_trainers.py` matches Trainer and Energy cards by illustrator, kind, era and what the text does (needs
  pokemon-tcg-data for the English artists), checked against the games' item names (PokeAPI `item_names.csv`).

## Rebuilding the names

0. `bulba.py` reads official Japanese card, attack, Ability and Trainer names from Bulbapedia's MediaWiki API (one page per card;
   about 1 request a second); those come first.
1. Download PokeAPI's `pokemon_species_names.csv`, `move_names.csv` and `ability_names.csv` (github.com/PokeAPI/pokeapi,
   data/v2/csv) into `<work>/dl`.
2. Clone TCGdex's cards-database (MIT) with only `data-asia`, then `python3 -I parse_tcgdex.py <repo>/data-asia <work>/ja_cards.jsonl`
   and `python3 -I setquality.py <work>/dl <work>/ja_cards.jsonl` (keeps the sets whose Pokémon have their official names; some
   older TCGdex Japanese sets are machine translations).
3. Optional, slow (about 1 request a second): `python3 -I scrape_pcc.py <work>/pcc_data` reads the official Japanese card database
   (pokemon-card.com). It resumes where it stopped.
4. `python3 -I match2.py <work>/dl <work>/ja_cards.jsonl <work>/pcc_data <Resources> <work>/matched.json` matches every English
   Pokémon card to its Japanese printings (species, HP, attack costs and damage, stage, Weakness, Retreat, regulation mark) and keeps a
   name only when the printings agree.
5. `build_lang.py` writes names_ja.txt (run from `<work>`); with `--ui` it also rebuilds ui_ja.txt from `ui_ordered.json` and `tr/`.

`templates.py` lists the strings the UI code can show; `wrap_calls.py` wrapped the UI's drawing calls in `Loc.T(...)`.

## Japanese card images (v177)

`images_ja.txt` lists each card's Japanese printing's image; the game's menu button "Download Japanese card images" downloads the
missing ones (1 a second) into a `ja` folder inside each set's folder of `StreamingAssets/CardImages` (e.g. `Base Set/ja/BS_4.jpg`),
and Japanese mode shows them. `build_images.py` makes the list: official-site images for the Pokémon `match2.py` matched, then pkmn.gg
cards by Bulbapedia's exact Japanese printing (set and number, from `bulba.py`), then pkmn.gg cards with the same English name and
illustrator released within 8 months, only when exactly one card fits. `pkmngg_sets.py` lists pkmn.gg's Japanese sets.
