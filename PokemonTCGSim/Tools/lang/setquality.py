import csv, json, sys, collections
dl, cards = sys.argv[1], sys.argv[2]
ja = {}
for r in csv.DictReader(open(f'{dl}/species_names.csv', encoding='utf-8')):
    if r['local_language_id'] == '1': ja[int(r['pokemon_species_id'])] = r['name']
stats = collections.defaultdict(lambda: [0, 0])
for line in open(cards, encoding='utf-8'):
    c = json.loads(line)
    if c.get('category') != 'Pokemon' or not c.get('dexId'): continue
    name = (c.get('name') or {}).get('ja') or ''
    ok = any(ja.get(d, '\0') in name for d in c['dexId'])
    s = stats[(c['_serie'], c['_set'])]; s[0] += 1; s[1] += ok
bad = [(k, v) for k, v in stats.items() if v[1] < 0.9 * v[0]]
good = [k for k, v in stats.items() if v[1] >= 0.9 * v[0]]
print('sets', len(stats), 'good', len(good), 'bad', len(bad))
by = collections.Counter(k[0] for k, v in bad)
print('bad by serie', dict(by))
for k, v in sorted(bad)[:15]: print(' ', k, v)
json.dump(sorted('/'.join(k) for k in good), open('good_sets.json', 'w'))
