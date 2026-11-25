"""Writes Resources/Lang/images_ja.txt: for each card (printing) in the pool, the image of its Japanese printing. The official site
(pokemon-card.com) for the Pokémon match2.py matched; otherwise pkmn.gg's card with the same English name and illustrator, released
closest to the English set."""
import collections, datetime, glob, json, os, re, sys
work, res, ptcg = sys.argv[1:4]
def norm(s): return re.sub(r'[^a-z0-9]', '', (s or '').lower().replace('é', 'e'))
def date(s):
    for fmt in ('%Y/%m/%d', '%m/%d/%Y', '%Y-%m-%d'):
        try: return datetime.datetime.strptime(s or '', fmt)
        except ValueError: pass
    return None

pool_keys = set()
for f in glob.glob(os.path.join(res, 'CardData', '*_pool.json')):
    for c in json.load(open(f, encoding='utf-8'))['cards']: pool_keys.add('%s-%s' % (c['set'], c['number']))
pr = json.load(open(os.path.join(res, 'CardPrintings', 'printings.json'), encoding='utf-8'))
for c in pr['cards']:
    for p in c['prints']: pool_keys.add('%s-%s' % (p['set'], p['number']))

sets = {s['id']: s for s in json.load(open(os.path.join(ptcg, 'sets', 'en.json')))}
en = {}
for f in glob.glob(os.path.join(ptcg, 'cards', 'en', '*.json')):
    s = sets.get(os.path.basename(f)[:-5])
    if not s or not s.get('ptcgoCode'): continue
    for c in json.load(open(f, encoding='utf-8')):
        key = '%s-%s' % (s['ptcgoCode'], c['number'])
        if key in pool_keys: en[key] = (c['name'], c.get('artist'), date(s.get('releaseDate')))

gg = collections.defaultdict(list)
gg_exact = {}  # (Japanese set, number) -> card
for l in open(os.path.join(work, 'pkmngg', 'cards.jsonl'), encoding='utf-8'):
    c = json.loads(l)
    if c.get('empty') or not c.get('image') or not c.get('name'): continue
    gg[(norm(c['name']), norm(c.get('artist')))].append(c)
    slug = c['setPath'].rsplit('/', 1)[-1]
    num = re.sub(r'^0+', '', (c.get('number') or '').split('/')[0]) or '0'
    gg_exact.setdefault((norm(slug), num), c)
bp_path = os.path.join(work, 'bulba_jp_prints.json')
jp_prints = json.load(open(bp_path, encoding='utf-8')) if os.path.exists(bp_path) else {}
def gg_path(url):
    m = re.search(r'/images/cards/(.+)$', url)
    return 'gg:' + m.group(1) if m else url

matched = json.load(open(os.path.join(work, 'matched.json'), encoding='utf-8'))
out, src = {}, collections.Counter()
for key, path in matched.get('images', {}).items():
    if key in pool_keys and path: out[key] = 'pcc:' + path; src['pokemon-card.com'] += 1
# Bulbapedia names each card's Japanese printing (set and number): the exact pkmn.gg card.
for key, prints in jp_prints.items():
    if key in out or key not in pool_keys: continue
    for setname, number in prints:
        num = re.sub(r'^0+', '', number.split('/')[0]) or '0'
        c = gg_exact.get((norm(setname), num)) or gg_exact.get((norm(re.sub(r'\b(Promotional cards|Promos?)\b', 'promo', setname)), num))
        if c:
            out[key] = gg_path(c['image']); src['pkmn.gg (exact)'] += 1
            break
for key, (name, artist, when) in en.items():
    if key in out or not artist or key in jp_prints: continue
    cands = gg.get((norm(name), norm(artist)))
    if not cands: continue
    def gap(c):
        d = date(c.get('release'))
        return abs((d - when).days) if d and when else 10 ** 6
    # The Japanese original comes out a few months before the English set; further than that it's another printing (other art).
    near = [c for c in cands if gap(c) <= 240]
    if len({(c['setPath'], c.get('number')) for c in near}) != 1: continue  # none, or two that could be it
    best = near[0]
    out[key] = gg_path(best['image']); src['pkmn.gg (name, artist, date)'] += 1
lines = ['# v177: the image of each card\'s Japanese printing (card key, a tab, the image: pcc: = pokemon-card.com, gg: = pkmn.gg).']
lines += ['%s\t%s' % (k, v) for k, v in sorted(out.items())]
open(os.path.join(res, 'Lang', 'images_ja.txt'), 'w', encoding='utf-8', newline='\n').write('\n'.join(lines) + '\n')
print('pool printings', len(pool_keys), 'with a Japanese image', len(out), dict(src))
