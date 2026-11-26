"""Lists the old Japanese sets on pkmn.gg (one page every 2 seconds) and keeps each card's Japanese name, English name, artist, type,
number and scan URL."""
import json, os, re, subprocess, sys, time
out = sys.argv[1]
SERIES = sys.argv[2].split(',') if len(sys.argv) > 2 else ['original', 'gym', 'neo', 'e-card', 'adv', 'pcg', 'other']
def get(url):
    time.sleep(2)
    r = subprocess.run(['curl', '-sSL', '-m', '40', '--fail', url], capture_output=True)
    return r.stdout.decode('utf-8', 'replace') if r.returncode == 0 else None
def next_data(html):
    m = re.search(r'<script id="__NEXT_DATA__"[^>]*>(.*?)</script>', html or '', re.S)
    return json.loads(m.group(1)) if m else None
done = set()
if os.path.exists(out):
    for l in open(out, encoding='utf-8'): done.add(json.loads(l)['setPath'])
with open(out, 'a', encoding='utf-8') as f:
    for s in SERIES:
        html = get('https://www.pkmn.gg/jp/series/' + s)
        sets = sorted(set(re.findall(r'href="(/jp/series/%s/[^"]+)"' % re.escape(s), html or '')))
        print(s, len(sets), flush=True)
        for path in sets:
            if path in done: continue
            d = next_data(get('https://www.pkmn.gg' + path))
            cards = (((d or {}).get('props') or {}).get('pageProps') or {}).get('cardData') or []
            for c in cards:
                f.write(json.dumps({'setPath': path, 'series': s, 'set': c.get('set'), 'name': c.get('name'), 'ja': (c.get('altName') or {}).get('JP'),
                                    'artist': c.get('artist'), 'type': c.get('cardType'), 'number': c.get('number'),
                                    'release': c.get('releaseDate'), 'image': c.get('largeImageUrl')}, ensure_ascii=False) + '\n')
            if not cards: f.write(json.dumps({'setPath': path, 'empty': True}) + '\n')
            f.flush()
print('done', flush=True)
