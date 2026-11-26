"""Trainer and Energy names: English -> official Japanese. Every English printing (pokemon-tcg-data: name, artist, kind, regulation
mark, text) is compared with the Japanese printings (TCGdex data-asia, good sets) by the same illustrator, of the same kind and
regulation mark. Among those, the one whose text does the same things (search, draw, discard, switch, heal, Energy types, numbers...)
wins when it's clearly the best. A name is kept when its printings agree, and each Japanese name goes to the English name with the
most evidence."""
import collections, glob, json, os, re, sys
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from textcheck import same_effect
work, res, ptcg, out = sys.argv[1:5]
good = set(json.load(open(os.path.join(work, 'good_sets.json'))))
def ill(s): return re.sub(r'[^a-z0-9]', '', (s or '').lower())
KIND = {'Item': 'Item', 'Supporter': 'Supporter', 'Stadium': 'Stadium', 'Tool': 'Tool', 'Pokémon Tool': 'Tool', 'Technical Machine': 'Tool'}
# English phrase -> Japanese phrase: what the card does.
TERMS = [
    (r'search your deck', '山札から'), (r'\bdraw\b', '引く'), (r'discard', 'トラッシュ'), (r'shuffle', '切る'),
    (r'\bheal', '回復'), (r'\bswitch', '入れ替え'), (r'basic pokémon', 'たねポケモン'), (r'evolution', '進化'), (r'\bevolve', '進化'),
    (r'benched', 'ベンチ'), (r'\bbench\b', 'ベンチ'), (r'active pokémon', 'バトルポケモン'), (r'prize card', 'サイド'), (r'flip a coin', 'コイン'),
    (r'damage counter', 'ダメカン'), (r'special condition', '特殊状態'), (r'asleep', 'ねむり'), (r'poisoned', 'どく'), (r'burned', 'やけど'),
    (r'confused', 'こんらん'), (r'paralyzed', 'マヒ'), (r'stadium', 'スタジアム'), (r'pokémon tool', 'ポケモンのどうぐ'), (r'supporter', 'サポート'),
    (r'\bitem', 'グッズ'), (r'your hand', '手札'), (r"opponent", '相手'), (r'retreat cost', 'にげる'), (r'\bretreat', 'にげる'), (r'\bhp\b', 'HP'),
    (r'lost zone', 'ロストゾーン'), (r'\babilit', '特性'), (r'\battack', 'ワザ'), (r'weakness', '弱点'), (r'resistance', '抵抗力'),
    (r'special energy', '特殊エネルギー'), (r'basic energy', '基本エネルギー'), (r'\bpokémon ex\b', 'ポケモンex'), (r'\bpokémon v\b', 'ポケモンV'),
    (r'rule box', 'ルールを持つ'), (r'top of your deck|top \d+ cards|top card', '上から'), (r'bottom of your deck', '下に'),
    (r'discard pile', 'トラッシュ'), (r'knocked out', 'きぜつ'), (r'during your next turn|next turn', '次の'), (r'reveal', '見せ'),
    (r'\bfire\b|\[r\]', '炎'), (r'\bwater\b|\[w\]', '水'), (r'\bgrass\b|\[g\]', '草'), (r'lightning|\[l\]', '雷'), (r'psychic|\[p\]', '超'),
    (r'fighting|\[f\]', '闘'), (r'darkness|\[d\]', '悪'), (r'\bmetal\b|\[m\]', '鋼'), (r'dragon', 'ドラゴン'), (r'fairy|\[y\]', 'フェアリー'),
    (r'colorless|\[c\]', '無色'), (r'tera', 'テラスタル'), (r'ancient', '古代'), (r'future', '未来'), (r'mega evolution|mega', 'メガ'),
]
FW = str.maketrans('０１２３４５６７８９', '0123456789')
def en_feats(text):
    t = (text or '').lower()
    f = {ja for en, ja in TERMS if re.search(en, t)}
    f |= {'#' + n for n in re.findall(r'\d+', t) if n != '1'}
    return f
def ja_feats(text):
    t = (text or '').translate(FW)
    f = {ja for en, ja in TERMS if ja in t}
    f |= {'#' + n for n in re.findall(r'\d+', t) if n != '1'}
    return f
BOILER = re.compile(r'You may play only 1 (Supporter|Stadium) card[^.]*\.|You may play any number of Item cards[^.]*\.|'
                    r'Attach a Pokémon Tool to 1 of your Pokémon[^.]*\.|This card stays in play when you play it[^.]*\.|'
                    r'Discard this card if another Stadium[^.]*\.|You may play as many Item cards[^.]*\.', re.I)
JBOIL = re.compile(r'サポートは自分の番に1枚しか使えない。|グッズは、自分の番に何枚でも使える。|ポケモンのどうぐは、自分のポケモンにつけて使う。[^。]*。|スタジアムは[^。]*。')

ja = collections.defaultdict(list)
effects = collections.defaultdict(collections.Counter)  # (Japanese name, regulation mark or None) -> official effect texts
for line in open(os.path.join(work, 'ja_cards.jsonl'), encoding='utf-8'):
    c = json.loads(line)
    if c.get('category') not in ('Trainer', 'Energy') or c['_serie'] + '/' + c['_set'] not in good: continue
    name = (c.get('name') or {}).get('ja')
    if not name or not c.get('illustrator'): continue
    kind = KIND.get(c.get('trainerType') or '', '') if c['category'] == 'Trainer' else 'Energy'
    effect = ((c.get('effect') or {}).get('ja') or '').strip()
    if effect: effects[(name, c.get('regulationMark'))][effect] += 1; effects[(name, '*')][effect] += 1
    entry = (name, ja_feats(JBOIL.sub('', effect)), c.get('regulationMark'))
    ja[(kind, ill(c['illustrator']))].append(entry)

pool_names = set()
for f in glob.glob(os.path.join(res, 'CardData', '*_pool.json')):
    for c in json.load(open(f, encoding='utf-8'))['cards']:
        if c.get('kind') in ('Trainer', 'Energy'): pool_names.add(c['name'])

def score(a, b):
    if not a and not b: return 0.5
    return len(a & b) / float(len(a | b))

votes = collections.defaultdict(collections.Counter)
prints = 0
for f in glob.glob(os.path.join(ptcg, 'cards', 'en', '*.json')):
    for c in json.load(open(f, encoding='utf-8')):
        if c.get('supertype') not in ('Trainer', 'Energy') or c['name'] not in pool_names or not c.get('artist'): continue
        subs = c.get('subtypes') or []
        kind = 'Energy' if c['supertype'] == 'Energy' else next((KIND[s] for s in subs if s in KIND), '')
        if not kind: continue
        reg = c.get('regulationMark')
        # The same era: a card with a regulation mark only matches Japanese cards with that mark, an older one only older cards.
        cands = [x for x in ja.get((kind, ill(c['artist'])), []) if (reg or None) == (x[2] or None)]
        if not cands: continue
        feats = en_feats(BOILER.sub('', ' '.join(c.get('rules') or [])))
        best = {}
        for name, jf, _ in cands: best[name] = max(best.get(name, 0), score(feats, jf))
        ranked = sorted(best.items(), key=lambda kv: -kv[1])
        top, s1 = ranked[0]
        s2 = ranked[1][1] if len(ranked) > 1 else 0
        if len(ranked) == 1 and s1 >= 0.35:
            votes[c['name']][top] += 2; prints += 1  # the only card by that illustrator, kind and era, doing the same things
        elif s1 >= 0.5 and s1 - s2 >= 0.2:
            votes[c['name']][top] += 1; prints += 1

best = {}
for k, cnt in votes.items():
    v, n = cnt.most_common(1)[0]
    if n * 3 >= 2 * sum(cnt.values()): best[k] = (v, n)
owner = {}
for k, (v, n) in best.items():
    if v not in owner or n > best[owner[v]][1]: owner[v] = k
result = {k: v for k, (v, n) in best.items() if owner[v] == k}

# The games' item names (PokeAPI) check the matches: an item the games name in kana must match it (Lum Berry is ラムのみ, not オレンのみ),
# and items the matching missed get the games' name.
import csv
en_i, ja_i = {}, {}
for r in csv.DictReader(open(os.path.join(work, 'dl', 'item_names.csv'), encoding='utf-8')):
    if r['local_language_id'] == '9': en_i[r['item_id']] = r['name']
    elif r['local_language_id'] == '1': ja_i[r['item_id']] = r['name']
items = {}
for i, n in en_i.items():
    if i in ja_i: items.setdefault(n.lower(), ja_i[i])
KANA = re.compile(r'^[\u3040-\u30ff\u30fc・ー0-9A-Za-zＡ-Ｚ.\s]+$')
fixed = added = 0
for k in sorted(pool_names):
    g = items.get(k.lower())
    if not g: continue
    if k in result:
        if result[k] != g and KANA.match(result[k]): result[k] = g; fixed += 1
    else:
        result[k] = g; added += 1
print('game item names: corrected', fixed, 'added', added)
# Checked by hand: the matching picked another card for these.
OVERRIDES = {'Dive Ball': 'ダイブボール'}
# Basic Energy (the Japanese cards are "基本草エネルギー"...).
BASIC = {'Grass': '草', 'Fire': '炎', 'Water': '水', 'Lightning': '雷', 'Psychic': '超', 'Fighting': '闘', 'Darkness': '悪', 'Metal': '鋼', 'Fairy': 'フェアリー'}
for t, j in BASIC.items(): OVERRIDES['%s Energy' % t] = '基本%sエネルギー' % j; OVERRIDES['Basic %s Energy' % t] = '基本%sエネルギー' % j
for k, v in OVERRIDES.items():
    if k in pool_names: result[k] = v
# The official Japanese text of each Trainer and Energy, keyed by the English text the game shows: from a Japanese printing of the
# same era when there is one.
texts = {}
for f in glob.glob(os.path.join(res, 'CardData', '*_pool.json')):
    for c in json.load(open(f, encoding='utf-8'))['cards']:
        if c.get('kind') not in ('Trainer', 'Energy') or not c.get('text') or c['name'] not in result: continue
        jn = result[c['name']]
        cnt = effects.get((jn, c.get('regulationMark'))) or effects.get((jn, '*'))
        if cnt:
            ok = [t for t, n in cnt.most_common() if same_effect(c['text'], t)]
            if ok: texts.setdefault(c['text'], ok[0])
print('trainer/energy texts', len(texts))
json.dump({'names': result, 'texts': texts}, open(out, 'w', encoding='utf-8'), ensure_ascii=False, indent=0, sort_keys=True)
print('pool trainer/energy names', len(pool_names), 'translated', len(result), 'from', prints, 'printings')
for k in ["Boss's Orders", 'Ultra Ball', 'Rare Candy', 'Nest Ball', "Professor's Research", 'Switch', 'Iono', 'Arven', 'Super Rod',
          'Double Colorless Energy', 'Path to the Peak', 'Electric Generator', 'Computer Search', 'Choice Belt', 'Battle VIP Pass',
          'Rescue Stretcher', 'Lysandre', 'N', 'Professor Sycamore', 'Professor Juniper', 'Energy Retrieval', 'Night Stretcher',
          'Counter Catcher', 'Buddy-Buddy Poffin', 'Earthen Vessel', 'Pokégear 3.0', 'Lillie', "Lillie's Determination", 'Jet Energy']:
    print(' ', k, result.get(k))
