"""Official Japanese names from Bulbapedia's MediaWiki API (about 1 request a second): for each pool set, its set page's card links,
then the card pages 50 at a time. Writes card, attack, Ability and Trainer name votes; resumable (pages are cached)."""
import collections, glob, json, os, re, subprocess, sys, time, urllib.parse
res, work = sys.argv[1], sys.argv[2]
cache_dir = os.path.join(work, 'bulba'); os.makedirs(cache_dir, exist_ok=True)
API = 'https://bulbapedia.bulbagarden.net/w/api.php?'
last = [0.0]
def api(params):
    for attempt in range(4):
        wait = 1.0 - (time.time() - last[0])
        if wait > 0: time.sleep(wait)
        last[0] = time.time()
        r = subprocess.run(['curl', '-sS', '-m', '60', '--fail', API + urllib.parse.urlencode(params)], capture_output=True)
        if r.returncode == 0:
            try: return json.loads(r.stdout)
            except ValueError: pass
        time.sleep(5 * (attempt + 1))
    return None
def cached(name, params):
    p = os.path.join(cache_dir, re.sub(r'[^\w.-]', '_', name)[:150] + '.json')
    if os.path.exists(p): return json.load(open(p, encoding='utf-8'))
    d = api(params)
    if d is not None: json.dump(d, open(p, 'w', encoding='utf-8'), ensure_ascii=False)
    return d

def norm(s): return re.sub(r'[^a-z0-9]', '', s.lower().replace('&', 'and').replace('é', 'e'))
# Bulbapedia's English expansion pages.
lst = cached('list', {'action': 'parse', 'page': 'List of Pokémon Trading Card Game expansions', 'prop': 'links', 'format': 'json'})
pages = [l['*'] for l in lst['parse']['links'] if l['*'].endswith('(TCG)')]
by_norm = {}
for p in pages:
    base = p[:-6]
    for key in {norm(base), norm(re.sub(r'^(EX|Pokémon) ', '', base))}: by_norm.setdefault(key, p)

pool = {}
for f in sorted(glob.glob(os.path.join(res, 'CardData', '*_pool.json'))):
    d = json.load(open(f, encoding='utf-8'))
    m = re.match(r'([^(]+?)\s*\(', d.get('format', ''))
    if not m or not d['cards']: continue
    pool[d['cards'][0]['set']] = (m.group(1).strip(), d['cards'])

titles_for = {}
unmatched = []
for code, (name, cards) in pool.items():
    page = by_norm.get(norm(name)) or by_norm.get(norm(name + ' Promos')) or by_norm.get(norm(re.sub(r'^Pokémon ', '', name)))
    if not page: unmatched.append((code, name)); continue
    d = cached('set_' + page, {'action': 'parse', 'page': page, 'prop': 'links', 'format': 'json', 'redirects': 1})
    if not d or 'parse' not in d: unmatched.append((code, name)); continue
    links = [l['*'] for l in d['parse']['links']]
    by_num = collections.defaultdict(list)
    for l in links:
        mm = re.match(r'^(.+) \((.+) ([A-Za-z]*\d+[a-z]?|[A-Z!?])\)$', l)
        if mm: by_num[mm.group(3).lstrip('0') or '0'].append((mm.group(1), l))
    for c in cards:
        num = str(c['number']).lstrip('0') or '0'
        cands = by_num.get(num, [])
        want = norm(c['name'])
        hit = [t for n, t in cands if norm(n) == want] or [t for n, t in cands if want in norm(n) or norm(n) in want]
        if len(hit) >= 1: titles_for[(code, c['number'])] = (c, hit[0])
print('sets', len(pool), 'unmatched sets', len(unmatched), unmatched[:20], flush=True)
print('cards with a Bulbapedia page', len(titles_for), flush=True)

BLOCK = re.compile(r'\{\{Cardtext/([^\n|]+)\n(.*?)\n\}\}', re.S)
def field(body, key):
    m = re.search(r'^\|' + key + r'=(.*)$', body, re.M)
    return m.group(1).strip() if m else ''
def clean(s):
    s = re.sub(r'\{\{tt\|[^|}]*\|([^}]*)\}\}', r'\1', s)
    s = re.sub(r"<[^>]+>|''+|\[\[|\]\]", '', s)
    return s.strip()

jp_prints = {}  # English card key -> [(Japanese set name, Japanese number)] from the page's expansion boxes
out = {'cards': collections.defaultdict(collections.Counter), 'attacks': collections.defaultdict(collections.Counter),
       'abilities': collections.defaultdict(collections.Counter), 'trainers': collections.defaultdict(collections.Counter)}
items = list(titles_for.values())
for i in range(0, len(items), 50):
    batch = items[i:i + 50]
    titles = [t for _, t in batch]
    d = cached('cards_%05d' % i, {'action': 'query', 'prop': 'revisions', 'rvprop': 'content', 'rvslots': 'main', 'format': 'json',
                                  'redirects': 1, 'titles': '|'.join(titles)})
    if not d: continue
    text_of = {}
    for p in (d.get('query') or {}).get('pages', {}).values():
        if 'revisions' in p: text_of[p['title']] = p['revisions'][0]['slots']['main']['*']
    redirects = {r['from']: r['to'] for r in (d.get('query') or {}).get('redirects', [])}
    for c, t in batch:
        wt = text_of.get(redirects.get(t, t))
        if not wt: continue
        prints = []
        for box in re.findall(r'\{\{PokémoncardInfobox/Expansion([^\n]*)\}\}|\{\{TrainercardInfobox/Expansion([^\n]*)\}\}|\{\{EnergycardInfobox/Expansion([^\n]*)\}\}', wt):
            line = ''.join(box)
            no = re.search(r'\|jpcardno=([^|}]+)', line)
            st = re.search(r'\|jp(?:expansion|halfdeck|deck)=\{\{TCG\|([^}|]+)(?:\|([^}]+))?\}\}', line)
            if no and st: prints.append((st.group(2) or st.group(1), no.group(1).strip()))
        if prints: jp_prints['%s-%s' % (c['set'], c['number'])] = prints
        jn = clean(field(wt, 'jname'))
        if not jn: continue
        kind = 'cards' if c.get('kind') == 'Pokemon' else 'trainers'
        out[kind][c['name']][jn] += 1
        if c.get('kind') != 'Pokemon': continue
        blocks = [(k.strip(), body) for k, body in BLOCK.findall(wt) if k.strip() not in ('Header', 'Footer')]
        atk = [(field(b, 'name'), clean(field(b, 'jname'))) for k, b in blocks if k == 'Attack']
        abl = [(field(b, 'name'), clean(field(b, 'jname'))) for k, b in blocks if k != 'Attack']
        en_atk = [a['name'] for a in c.get('attacks') or []]
        en_abl = [a['name'] for a in c.get('abilities') or []]
        for en, (bn, jn2) in zip(en_atk, atk):
            if jn2 and norm(bn) == norm(en): out['attacks'][en][jn2] += 1
        for en, (bn, jn2) in zip(en_abl, abl):
            if jn2 and norm(bn) == norm(en): out['abilities'][en][jn2] += 1
    if (i // 50) % 20 == 0: print('pages', i + len(batch), '/', len(items), flush=True)

final = {}
for k, v in out.items():
    final[k] = {}
    for en, cnt in v.items():
        top, n = cnt.most_common(1)[0]
        if n * 3 >= 2 * sum(cnt.values()): final[k][en] = top
json.dump(final, open(os.path.join(work, 'bulba_names.json'), 'w', encoding='utf-8'), ensure_ascii=False, indent=0, sort_keys=True)
json.dump(jp_prints, open(os.path.join(work, 'bulba_jp_prints.json'), 'w', encoding='utf-8'), ensure_ascii=False, indent=0, sort_keys=True)
print('cards with their Japanese printing', len(jp_prints), flush=True)
print({k: len(v) for k, v in final.items()}, flush=True)
