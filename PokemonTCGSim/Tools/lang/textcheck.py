"""Checks that an English card text and a Japanese one say the same thing: the same numbers (Japanese spells out 1s the English
leaves out), and the same kind of effect (more or less damage, healing, coins, Special Conditions, search, draw, discard...)."""
import re
FW = str.maketrans('０１２３４５６７８９', '0123456789')
TERMS = [
    (r'more damage|\+\d+ damage|damage for each|x \d+|\d+x', r'追加|×|x|ぶん'), (r'less damage|-\d+', r'-|－|減'),
    (r'\bheal', r'回復'), (r'flip', r'コイン'), (r'asleep', r'ねむり'), (r'poison', r'どく'), (r'burn', r'やけど'),
    (r'confus', r'こんらん'), (r'paralyz', r'マヒ'), (r'search your deck', r'山札から'), (r'\bdraw', r'引く'), (r'discard', r'トラッシュ'),
    (r'switch', r'入れ替え'), (r'bench', r'ベンチ'), (r'damage counter', r'ダメカン'), (r"can't attack|can’t attack|can't use", r'ワザが使えない|使えない'),
    (r"can't retreat|can’t retreat", r'にげられない'), (r'prevent all', r'受けない'), (r'shuffle', r'切る|もどす|戻す'),
    (r'evolve', r'進化'), (r'knocked out', r'きぜつ'), (r'prize', r'サイド'), (r'attach', r'つける'), (r'itself', r'自分にも'),
]
def numbers(s, ja=False):
    s = s.translate(FW)
    return sorted(n for n in re.findall(r'\d+', s) if n not in ('1', '0'))
def same_effect(en, ja):
    if numbers(en) != numbers(ja, True): return False
    e = en.lower(); j = ja.translate(FW)
    hits = misses = 0
    for pe, pj in TERMS:
        a, b = bool(re.search(pe, e)), bool(re.search(pj, j))
        if a and not b: misses += 1
        elif a and b: hits += 1
    return misses == 0 or (hits >= 2 * misses and misses <= 1)
