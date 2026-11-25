"""Reads the Japanese effect text off old card scans (pkmn.gg) for English cards that have no Japanese text yet. Each English card's
Japanese printing is the pkmn.gg card with Bulbapedia's Japanese name and the same illustrator. The scan is read with Tesseract
(Japanese); its sentences are matched to the English effects in order, and a text is kept only when it says the same thing as the
English (textcheck.same_effect) and reads cleanly. Scans are downloaded once (1 a second) and kept; results are resumable."""
import collections, glob, json, os, re, subprocess, sys, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from textcheck import same_effect
work, res, ptcg = sys.argv[1:4]
limit = int(sys.argv[4]) if len(sys.argv) > 4 else 10 ** 9
scan_dir = os.path.join(work, 'scans'); os.makedirs(scan_dir, exist_ok=True)
out_path = os.path.join(work, 'ocr_texts.jsonl')

def ill(s): return re.sub(r'[^a-z0-9]', '', (s or '').lower())
def un(s): return s.replace('\\n', '\n').replace('\\t', '\t').replace('\\\\', '\\')
have = {un(l.split('\t')[1]) for l in open(os.path.join(res, 'Lang', 'names_ja.txt'), encoding='utf-8') if l.startswith('text\t')}
bulba = json.load(open(os.path.join(work, 'bulba_names.json'), encoding='utf-8'))
jname = dict(bulba.get('cards', {})); jname.update(bulba.get('trainers', {}))
sets = {s['id']: s.get('ptcgoCode') for s in json.load(open(os.path.join(ptcg, 'sets', 'en.json')))}
artist = {}
for f in glob.glob(os.path.join(ptcg, 'cards', 'en', '*.json')):
    code = sets.get(os.path.basename(f)[:-5])
    for c in json.load(open(f, encoding='utf-8')): artist[(code, c['number'])] = c.get('artist')

jp = collections.defaultdict(list)
for l in open(os.path.join(work, 'pkmngg', 'cards.jsonl'), encoding='utf-8'):
    c = json.loads(l)
    if c.get('ja') and c.get('image'): jp[c['ja']].append(c)

done = set()
if os.path.exists(out_path):
    for l in open(out_path, encoding='utf-8'): done.add(json.loads(l)['key'])

targets = []
for f in sorted(glob.glob(os.path.join(res, 'CardData', '*_pool.json'))):
    for c in json.load(open(f, encoding='utf-8'))['cards']:
        texts = [a.get('text') for a in (c.get('abilities') or []) + (c.get('attacks') or [])] + [c.get('text')]
        missing = [t for t in texts if t and t not in have]
        if not missing: continue
        key = '%s-%s' % (c['set'], c['number'])
        if key in done or c['name'] not in jname: continue
        cands = jp.get(jname[c['name']], [])
        a = ill(artist.get((c['set'], c['number'])))
        cands = [x for x in cands if a and ill(x.get('artist')) == a] or []
        if cands: targets.append((key, c, cands[0]))
print('cards to read', len(targets), flush=True)

JA = re.compile(r'[぀-ヿ一-鿿]')
def ocr(card):
    p = os.path.join(scan_dir, re.sub(r'[^\w.-]', '_', card['image'].split('/images/cards/')[-1].split('?')[0]) + '.webp')
    if not os.path.exists(p):
        time.sleep(1)
        r = subprocess.run(['curl', '-sS', '-m', '40', '--fail', '-o', p, card['image']], capture_output=True)
        if r.returncode != 0: return None
    png = p[:-5] + '.png'
    from PIL import Image
    im = Image.open(p).convert('L')
    w, h = im.size
    im = im.crop((0, int(h * 0.50), w, int(h * 0.94))).resize((w * 2, int(h * 0.44) * 2))
    im.save(png)
    r = subprocess.run(['tesseract', png, '-', '-l', 'jpn', '--psm', '6'], capture_output=True)
    os.remove(png)
    return r.stdout.decode('utf-8', 'replace')

def sentences(raw):
    text = ''.join(l.strip() for l in raw.split('\n') if len(JA.findall(l)) >= 4)
    text = text.replace(' ', '').replace('、', '、').replace('｡', '。')
    parts = [s + '。' for s in text.split('。') if len(JA.findall(s)) >= 4]
    return parts
def clean(s):
    # Characters a card's text uses; anything else means the scan was misread.
    return re.fullmatch(r'[぀-ヿ一-鿿0-9０-９×xX、。「」『』（）()・ー－+\-%％/／]+', s) is not None

n = kept = 0
with open(out_path, 'a', encoding='utf-8') as fo:
    for key, c, card in targets[:limit]:
        raw = ocr(card)
        n += 1
        result = {}
        if raw:
            sents = sentences(raw)
            i = 0
            effects = [a.get('text') for a in (c.get('abilities') or []) + (c.get('attacks') or [])] if c.get('kind') == 'Pokemon' else [c.get('text')]
            for en in effects:
                if not en: continue
                best = None
                for k in (1, 2, 3, 4):
                    for start in range(i, min(i + 3, len(sents))):
                        cand = ''.join(sents[start:start + k])
                        if clean(cand) and same_effect(en, cand):
                            best = (start + k, cand); break
                    if best: break
                if best:
                    i, ja = best
                    if en not in have: result[en] = ja
        fo.write(json.dumps({'key': key, 'image': card['image'], 'texts': result}, ensure_ascii=False) + '\n'); fo.flush()
        kept += len(result)
        if n % 50 == 0: print('read', n, 'texts kept', kept, flush=True)
print('read', n, 'texts kept', kept, flush=True)
