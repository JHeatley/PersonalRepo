"""Matches English pool cards to official Japanese printings (TCGdex data-asia good sets + pokemon-card.com) and builds
English -> Japanese dictionaries for attack names, ability names and Pokémon card names. A name is kept only when the
matching Japanese printings agree on it."""
import collections, csv, glob, json, os, re, sys
dl, tcgdex, pcc, res, out = sys.argv[1:6]

def lang_table(table, key):
    en, ja = {}, {}
    for r in csv.DictReader(open(os.path.join(dl, table + '.csv'), encoding='utf-8')):
        if r['local_language_id'] == '9': en[int(r[key])] = r['name']
        elif r['local_language_id'] == '1': ja[int(r[key])] = r['name']
    return en, ja
sp_en, sp_ja = lang_table('species_names', 'pokemon_species_id')
EN_RE = re.compile(r'(?<![A-Za-z])(' + '|'.join(re.escape(n) for n in sorted(sp_en.values(), key=len, reverse=True)) + r')(?![a-z])')
EN_ID = {n: i for i, n in sp_en.items()}
JA_RE = re.compile('(' + '|'.join(re.escape(n) for n in sorted(sp_ja.values(), key=len, reverse=True)) + ')')
JA_ID = {n: i for i, n in sp_ja.items()}

LET = {'G': 'grass', 'R': 'fire', 'W': 'water', 'L': 'lightning', 'P': 'psychic', 'F': 'fighting', 'D': 'darkness', 'M': 'metal',
       'N': 'dragon', 'Y': 'fairy', 'C': 'colorless'}
WORD = {'grass': 'grass', 'fire': 'fire', 'water': 'water', 'lightning': 'lightning', 'electric': 'lightning', 'psychic': 'psychic',
        'fighting': 'fighting', 'darkness': 'darkness', 'dark': 'darkness', 'metal': 'metal', 'steel': 'metal', 'dragon': 'dragon',
        'fairy': 'fairy', 'colorless': 'colorless', 'none': 'colorless', 'void': 'colorless'}
def t(x): return WORD.get(str(x).lower(), str(x).lower())
def dmg(d):
    m = re.match(r'\d+', str(d if d is not None else ''))
    return int(m.group(0)) if m else 0
STAGE = {'basic': 'Basic', 'stage1': 'Stage1', 'stage2': 'Stage2', 'たね': 'Basic', '1 進化': 'Stage1', '2 進化': 'Stage2',
         '1\xa0進化': 'Stage1', '2\xa0進化': 'Stage2', '1進化': 'Stage1', '2進化': 'Stage2'}

ja_cards = []
good = set(json.load(open(os.path.join(os.path.dirname(tcgdex), 'good_sets.json'))))
for line in open(tcgdex, encoding='utf-8'):
    c = json.loads(line)
    if c['_serie'] + '/' + c['_set'] not in good or c.get('category') != 'Pokemon': continue
    name = (c.get('name') or {}).get('ja') or ''
    atks = c.get('attacks') or []
    if not name or any(not (a.get('name') or {}).get('ja') for a in atks): continue
    ids = sorted(set(c.get('dexId') or [JA_ID[m] for m in JA_RE.findall(name)]))
    w = c.get('weaknesses') or []
    ja_cards.append({'src': 'tcgdex', 'name': name, 'species': tuple(ids), 'hp': c.get('hp'),
                     'atk': tuple((tuple(sorted(t(x) for x in a.get('cost') or [])), dmg(a.get('damage'))) for a in atks),
                     'atk_names': tuple(a['name']['ja'] for a in atks),
                     'ab_names': tuple((a.get('name') or {}).get('ja') or '' for a in c.get('abilities') or []),
                     'stage': STAGE.get(str(c.get('stage', '')).lower()), 'weak': t(w[0]['type']) if w else None,
                     'retreat': c.get('retreat'), 'reg': c.get('regulationMark')})
n_tcgdex = len(ja_cards)
pcc_file = os.path.join(pcc, 'details.jsonl')
if os.path.exists(pcc_file):
    for line in open(pcc_file, encoding='utf-8'):
        c = json.loads(line)
        name = c.get('name') or ''
        if not name or c.get('hp') is None: continue
        ids = sorted(set(JA_ID[m] for m in JA_RE.findall(name)))
        ja_cards.append({'src': 'pcc', 'name': name, 'species': tuple(ids), 'hp': c.get('hp'),
                         'atk': tuple((tuple(sorted(t(x) for x in a['cost'])), dmg(a['damage'])) for a in c.get('attacks') or []),
                         'atk_names': tuple(a['name'] for a in c.get('attacks') or []),
                         'ab_names': tuple(x if isinstance(x, str) else x.get('name', '') for x in c.get('abilities') or []),
                         'stage': STAGE.get(c.get('stage', '')), 'weak': t(c['weakness'][0]) if c.get('weakness') else None,
                         'retreat': c.get('retreat'), 'reg': None})
print('japanese printings: tcgdex', n_tcgdex, 'official site', len(ja_cards) - n_tcgdex)

index = collections.defaultdict(list)
for j in ja_cards: index[(j['species'], j['hp'], j['atk'])].append(j)

def compatible(e, j):
    for k in ('stage', 'weak', 'retreat', 'reg'):
        if e.get(k) is not None and j.get(k) is not None and e[k] != j[k]: return False
    return len(j['ab_names']) == len(e['ab_names'])

atk_votes = collections.defaultdict(collections.Counter)
ab_votes = collections.defaultdict(collections.Counter)
name_votes = collections.defaultdict(collections.Counter)
total = matched = ambiguous = 0
for f in sorted(glob.glob(os.path.join(res, 'CardData', '*_pool.json'))):
    for c in json.load(open(f, encoding='utf-8'))['cards']:
        if c.get('kind') != 'Pokemon': continue
        total += 1
        atks = c.get('attacks') or []
        e = {'species': tuple(sorted(set(EN_ID[m] for m in EN_RE.findall(c['name'])))), 'hp': c.get('hp'),
             'atk': tuple((tuple(sorted(LET.get(x, x) for x in a.get('cost', ''))), a.get('damage', 0) or 0) for a in atks),
             'stage': STAGE.get(str(c.get('stage', '')).lower()), 'weak': t(c['weakness']) if c.get('weakness') else None,
             'retreat': c.get('retreat'), 'reg': c.get('regulationMark') or None,
             'ab_names': tuple(a['name'] for a in c.get('abilities') or [])}
        hits = [j for j in index.get((e['species'], e['hp'], e['atk']), []) if compatible(e, j)]
        if not hits: continue
        choices = collections.Counter((j['atk_names'], j['ab_names'], j['name']) for j in hits)
        (atk_ja, ab_ja, name_ja), top = choices.most_common(1)[0]
        if top * 3 < 2 * len(hits): ambiguous += 1; continue
        matched += 1
        for a, b in zip(atks, atk_ja): atk_votes[a['name']][b] += 1
        for a, b in zip(c.get('abilities') or [], ab_ja):
            if b: ab_votes[a['name']][b] += 1
        name_votes[c['name']][name_ja] += 1

def agreed(votes):
    out, dropped = {}, 0
    for k, cnt in votes.items():
        v, n = cnt.most_common(1)[0]
        if n * 5 >= 3 * sum(cnt.values()): out[k] = v
        else: dropped += 1
    return out, dropped
atk, da = agreed(atk_votes); ab, db_ = agreed(ab_votes); nm, dn = agreed(name_votes)
json.dump({'attacks': atk, 'abilities': ab, 'cards': nm}, open(out, 'w', encoding='utf-8'), ensure_ascii=False, indent=0, sort_keys=True)
print('english pokemon cards', total, 'matched', matched, 'ambiguous', ambiguous)
print('attack names', len(atk), '(disagreeing', da, ') abilities', len(ab), '(', db_, ') card names', len(nm), '(', dn, ')')
