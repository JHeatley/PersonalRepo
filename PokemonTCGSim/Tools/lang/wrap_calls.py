"""Wraps the text argument of every IMGUI drawing call in Loc.T(...), so the screens can be shown in Japanese."""
import re, sys
sys.path.insert(0, sys.argv[1])
from templates import tokens

TARGETS = {('GUILayout', 'Label'): 0, ('GUILayout', 'Button'): 0, ('GUILayout', 'Box'): 0, ('GUILayout', 'Toggle'): 1,
           ('GUILayout', 'Toolbar'): 1, ('GUILayout', 'SelectionGrid'): 1, ('GUI', 'Label'): 1, ('GUI', 'Button'): 1,
           ('GUI', 'Box'): 1, ('GUI', 'Toggle'): 2, ('GUI', 'Toolbar'): 2, ('GUI', 'RepeatButton'): 1, ('GUILayout', 'RepeatButton'): 0}

def wrap(src):
    toks = tokens(src)
    edits = []
    n = len(toks)
    for i in range(n - 3):
        key = None
        if toks[i][1] in ('GUILayout', 'GUI') and toks[i + 1][1] == '.' and toks[i + 3][1] == '(' and (toks[i][1], toks[i + 2][1]) in TARGETS:
            key = TARGETS[(toks[i][1], toks[i + 2][1])]; open_at = i + 3
            if i > 1 and toks[i - 1][1] == '.': continue
        elif toks[i][1] == 'new' and toks[i + 1][1] == 'GUIContent' and toks[i + 2][1] == '(':
            key = 0; open_at = i + 2
        else: continue
        # split arguments
        depth, k, args, start = 0, open_at + 1, [], open_at + 1
        while k < n:
            t = toks[k][1]
            if t in '([{': depth += 1
            elif t in ')]}':
                if depth == 0: args.append((start, k)); break
                depth -= 1
            elif t == ',' and depth == 0: args.append((start, k)); start = k + 1
            k += 1
        if key >= len(args): continue
        a, b = args[key]
        if a >= b: continue
        if toks[a][1] == 'Loc' and a + 2 < n and toks[a + 2][1] == 'T': continue
        if b - a == 1 and toks[a][1] == 'null': continue
        s = toks[a][2]; e = toks[b - 1][2] + len(toks[b - 1][1])
        edits.append((s, e))
    out = src
    for s, e in sorted(edits, reverse=True):
        out = out[:s] + 'Loc.T(' + out[s:e] + ')' + out[e:]
    return out, len(edits)

total = 0
for f in sys.argv[2:]:
    if f.endswith('/Loc.cs'): continue
    src = open(f, encoding='utf-8').read()
    out, k = wrap(src)
    if k:
        open(f, 'w', encoding='utf-8').write(out); total += k
        print(f.split('/')[-1], k)
print('wrapped', total)
