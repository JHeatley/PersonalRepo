"""Polite, resumable reader of the official Japanese card database (pokemon-card.com): Pokémon cards only, ~1 request a second.
Phase 1 lists card IDs; phase 2 reads each card's details page into one JSON line (name, HP, stage, type, abilities, attacks)."""
import html, json, os, re, subprocess, sys, time

out = sys.argv[1]
os.makedirs(out, exist_ok=True)
BASE = 'https://www.pokemon-card.com/card-search/'
DELAY = 1.0

def get(url):
    for attempt in range(4):
        r = subprocess.run(['curl', '-sS', '-m', '40', '--fail', url], capture_output=True)
        if r.returncode == 0: return r.stdout.decode('utf-8', 'replace')
        time.sleep(5 * (attempt + 1))
    return None

listing = os.path.join(out, 'list.jsonl')
seen = {}
if os.path.exists(listing):
    for line in open(listing, encoding='utf-8'):
        c = json.loads(line); seen[c['cardID']] = c
pages_done = os.path.join(out, 'pages_done.txt')
done_pages = set(open(pages_done).read().split()) if os.path.exists(pages_done) else set()
page, max_page = 1, None
with open(listing, 'a', encoding='utf-8') as fl, open(pages_done, 'a') as fp:
    while max_page is None or page <= max_page:
        if str(page) in done_pages and max_page is not None: page += 1; continue
        text = get(BASE + 'resultAPI.php?keyword=&regulation_sidebar_form=all&pg=&illust=&sm_and_keyword=true&se_ta=pokemon&page=%d' % page)
        time.sleep(DELAY)
        if text is None: print('list page failed', page, flush=True); page += 1; continue
        try: d = json.loads(text)
        except ValueError: print('bad list page', page, flush=True); page += 1; continue
        if not d.get('cardList'): print('empty list page', page, d.get('errMsg'), flush=True); page += 1; continue
        max_page = max(max_page or 0, d.get('maxPage') or 0)
        if str(page) not in done_pages:
            for c in d.get('cardList') or []:
                if c['cardID'] not in seen:
                    seen[c['cardID']] = c; fl.write(json.dumps(c, ensure_ascii=False) + '\n')
            fp.write('%d\n' % page); fp.flush(); fl.flush()
        if page % 50 == 0: print('list', page, '/', max_page, len(seen), flush=True)
        page += 1
print('listed', len(seen), flush=True)

ICON = re.compile(r'icon-([a-z]+) icon')
def strip(s): return html.unescape(re.sub(r'<[^>]+>', '', s)).strip()

def parse(page):
    card = {}
    m = re.search(r'<h1[^>]*class="Heading1[^"]*"[^>]*>(.*?)</h1>', page, re.S) or re.search(r'<h1[^>]*>(.*?)</h1>', page, re.S)
    card['name'] = strip(m.group(1)) if m else ''
    box = page[page.find('RightBox'):]
    box = box[:box.find('class="clear"')]
    m = re.search(r'<span class="type">(.*?)</span>', box, re.S); card['stage'] = strip(m.group(1)) if m else ''
    m = re.search(r'<span class="hp-num">(\d+)</span>', box); card['hp'] = int(m.group(1)) if m else None
    m = re.search(r'<span class="hp-type">.*?</span>\s*((?:<span class="icon-[a-z]+ icon"></span>\s*)+)', box, re.S)
    card['types'] = ICON.findall(m.group(1)) if m else []
    abilities, attacks, section = [], [], None
    for tag, body in re.findall(r'<(h2|h4)[^>]*>(.*?)</\1>', box, re.S):
        if tag == 'h2': section = strip(body); continue
        if section == '特性': abilities.append(strip(body))
        elif section == 'ワザ':
            cost = ICON.findall(body)
            dm = re.search(r'<span class="f_right[^"]*">(.*?)</span>', body, re.S)
            name = strip(re.sub(r'<span class="f_right.*?</span>', '', body, flags=re.S))
            attacks.append({'name': name, 'cost': cost, 'damage': strip(dm.group(1)) if dm else ''})
        elif section in ('GXワザ', 'VSTARパワー', 'ポケパワー', 'ポケボディー', 'どうぐ', '古代能力'):
            body2 = re.sub(r'<span class="f_right.*?</span>', '', body, flags=re.S)
            (attacks if ICON.search(body) else abilities).append(
                {'name': strip(body2), 'cost': ICON.findall(body), 'damage': ''} if ICON.search(body) else strip(body2))
    card['abilities'], card['attacks'] = abilities, attacks
    m = re.search(r'<td class="escape">(.*?)</td>', box, re.S); card['retreat'] = len(ICON.findall(m.group(1))) if m else None
    m = re.search(r'<th>弱点</th>.*?<tr>\s*<td>(.*?)</td>', box, re.S); card['weakness'] = ICON.findall(m.group(1)) if m else []
    m = re.search(r'class="Link Link-arrow">(.*?)</a>', page, re.S); card['product'] = strip(m.group(1)) if m else ''
    m = re.search(r'(\d{3})\s*/\s*(\d{3})', box); card['number'] = m.group(0) if m else ''
    return card

details = os.path.join(out, 'details.jsonl')
have = set()
if os.path.exists(details):
    for line in open(details, encoding='utf-8'): have.add(json.loads(line)['cardID'])
todo = [cid for cid in seen if cid not in have]
print('details to read', len(todo), flush=True)
with open(details, 'a', encoding='utf-8') as fd:
    for n, cid in enumerate(todo, 1):
        page = get(BASE + 'details.php/card/%s/regu/all' % cid)
        time.sleep(DELAY)
        if page is None: print('details failed', cid, flush=True); continue
        try: card = parse(page)
        except Exception as e: print('parse failed', cid, e, flush=True); continue
        card['cardID'] = cid; card['image'] = seen[cid].get('cardThumbFile', '')
        fd.write(json.dumps(card, ensure_ascii=False) + '\n'); fd.flush()
        if n % 250 == 0: print('details', n, '/', len(todo), flush=True)
print('done', flush=True)
