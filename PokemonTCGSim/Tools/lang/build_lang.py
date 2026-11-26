"""Writes Resources/Lang/ui_ja.txt (screen templates) and names_ja.txt (Pokémon, card, attack, Ability names) for the game."""
import csv, glob, json, os, re, sys
work, langdir = sys.argv[1], sys.argv[2]
os.makedirs(langdir, exist_ok=True)

def esc(s): return s.replace('\\', '\\\\').replace('\t', '\\t').replace('\n', '\\n').replace('\r', '')

# Screens: only with --ui (ui_ja.txt is edited by hand after the first build).
if '--ui' in sys.argv:
  d = json.load(open(os.path.join(work, 'ui_ordered.json')))
  tr = {}
  for f in sorted(glob.glob(os.path.join(work, 'tr', 'b*.json'))): tr.update({int(k): v for k, v in json.load(open(f)).items()})
  lines = ['# v176: Japanese for the game\'s screens. English template, a tab, the Japanese. {0}, {1}... are filled in by the code.']
  seen = set()
  for i, x in enumerate(d):
      if i not in tr or x['en'] in seen: continue
      seen.add(x['en'])
      lines.append(esc(x['en']) + '\t' + esc(tr[i]))
  open(os.path.join(langdir, 'ui_ja.txt'), 'w', encoding='utf-8', newline='\n').write('\n'.join(lines) + '\n')
  print('ui templates', len(lines) - 1)

# Names
dl = os.path.join(work, 'dl')
def table(name, key):
    en, ja = {}, {}
    for r in csv.DictReader(open(os.path.join(dl, name + '.csv'), encoding='utf-8')):
        if r['local_language_id'] == '9': en[int(r[key])] = r['name']
        elif r['local_language_id'] == '1': ja[int(r[key])] = r['name']
    return {en[i]: ja[i] for i in en if i in ja}
species = table('species_names', 'pokemon_species_id')
moves = table('move_names', 'move_id')
abilities_game = table('ability_names', 'ability_id')
matched = json.load(open(os.path.join(work, 'matched.json'), encoding='utf-8'))
# Bulbapedia's per-card Japanese names (tools/bulba.py) come first: one page per card, so no matching guesses.
bp = os.path.join(work, 'bulba_names.json')
bulba = json.load(open(bp, encoding='utf-8')) if os.path.exists(bp) else {}
def ok(v): return v and not re.search(r'[{}<>|=]|&nbsp;', v) and re.search(r'[\u3040-\u30ff\u4e00-\u9fff]', v)
for kind in ('cards', 'attacks', 'abilities'):
    merged = dict(matched.get(kind, {}))
    merged.update({k: v for k, v in bulba.get(kind, {}).items() if ok(v)})
    matched[kind] = merged

# Attack and Ability names used in the pool, so the file only has what the game can show.
pool_attacks, pool_abilities = set(), set()
res = os.path.join(os.path.dirname(langdir))
for f in glob.glob(os.path.join(res, 'CardData', '*_pool.json')):
    for c in json.load(open(f, encoding='utf-8'))['cards']:
        for a in c.get('attacks') or []: pool_attacks.add(a['name'])
        for a in c.get('abilities') or []: pool_abilities.add(a['name'])
lower_moves = {k.lower(): v for k, v in moves.items()}
lower_abil = {k.lower(): v for k, v in abilities_game.items()}
attacks, abil = {}, {}
src = {'card': 0, 'game': 0}
for a in sorted(pool_attacks):
    if a in matched['attacks']: attacks[a] = matched['attacks'][a]; src['card'] += 1
    elif a.lower() in lower_moves: attacks[a] = lower_moves[a.lower()]; src['game'] += 1
for a in sorted(pool_abilities):
    if a in matched['abilities']: abil[a] = matched['abilities'][a]
    elif a.lower() in lower_abil: abil[a] = lower_abil[a.lower()]
    elif a.lower() in lower_moves: abil[a] = lower_moves[a.lower()]

WORDS = [('Team Rocket\'s', 'ロケット団の'), ('Rocket\'s', 'ロケット団の'), ('Team Magma\'s', 'マグマ団の'), ('Team Aqua\'s', 'アクア団の'),
         ('Mega', 'メガ'), ('Alolan', 'アローラ'), ('Galarian', 'ガラル'), ('Hisuian', 'ヒスイ'), ('Paldean', 'パルデア'), ('Dark', 'わるい'),
         ('Light', 'やさしい'), ('Shining', 'ひかる'), ('Radiant', 'かがやく'), ('Rapid Strike', 'れんげき'), ('Single Strike', 'いちげき'),
         ('Erika\'s', 'エリカの'), ('Misty\'s', 'カスミの'), ('Brock\'s', 'タケシの'), ('Lt. Surge\'s', 'マチスの'), ('Sabrina\'s', 'ナツメの'),
         ('Koga\'s', 'キョウの'), ('Blaine\'s', 'カツラの'), ('Giovanni\'s', 'サカキの'), ('Lillie\'s', 'リーリエの'), ('N\'s', 'Nの'),
         ('Hop\'s', 'ホップの'), ('Marnie\'s', 'マリィの'), ('Iono\'s', 'ナンジャモの'), ('Ethan\'s', 'ヒビキの'), ('Cynthia\'s', 'シロナの'),
         ('Arven\'s', 'ペパーの'), ('Steven\'s', 'ダイゴの'), ('Larry\'s', 'アオキの')]
out = ['# v176: official Japanese names. kind, a tab, English, a tab, Japanese.',
       '# Sources: species, game moves and Abilities from PokeAPI (BSD-3-Clause); card, attack and Ability names from TCGdex',
       '# (MIT, data-asia) and the official Japanese card database (pokemon-card.com), matched to the English cards.']
for k, v in sorted(species.items()): out.append('species\t%s\t%s' % (k, v))
for k, v in sorted(matched['cards'].items()): out.append('card\t%s\t%s' % (k, v))
for k, v in sorted(attacks.items()): out.append('attack\t%s\t%s' % (k, v))
for k, v in sorted(abil.items()): out.append('ability\t%s\t%s' % (k, v))
tr_path = os.path.join(work, 'trainers.json')
tr_data = json.load(open(tr_path, encoding='utf-8')) if os.path.exists(tr_path) else {}
trainers = tr_data.get('names', tr_data) if isinstance(tr_data, dict) and 'names' in tr_data else tr_data
trainers = dict(trainers)
trainers.update({k: v for k, v in bulba.get('trainers', {}).items() if ok(v)})
texts = dict(matched.get('texts', {}))
texts.update(tr_data.get('texts', {}) if 'names' in tr_data else {})
for k, v in sorted(trainers.items()): out.append('trainer\t%s\t%s' % (k, v))
for k, v in WORDS: out.append('word\t%s\t%s' % (k, v))
for k, v in sorted(texts.items()): out.append('text\t%s\t%s' % (esc(k), esc(v)))
open(os.path.join(langdir, 'names_ja.txt'), 'w', encoding='utf-8', newline='\n').write('\n'.join(out) + '\n')
print('texts', len(texts), 'trainers', len(trainers), 'species', len(species), 'cards', len(matched['cards']), 'attacks', len(attacks), 'of', len(pool_attacks), src,
      'abilities', len(abil), 'of', len(pool_abilities))
