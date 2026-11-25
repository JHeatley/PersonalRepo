"""Reads TCGdex data-asia card files as text (never executes them) and writes one JSON object per card."""
import json, os, re, sys

root, out = sys.argv[1], sys.argv[2]

def js_to_py(src):
    """A small parser for the object literals these files use (strings, numbers, arrays, objects, bare keys)."""
    i = 0; n = len(src)
    def ws():
        nonlocal i
        while i < n:
            if src[i] in ' \t\r\n': i += 1
            elif src.startswith('//', i): i = src.find('\n', i) if src.find('\n', i) >= 0 else n
            elif src.startswith('/*', i): i = src.find('*/', i) + 2
            else: break
    def string():
        nonlocal i
        q = src[i]; i += 1; buf = []
        while src[i] != q:
            if src[i] == '\\':
                i += 1; c = src[i]
                buf.append({'n': '\n', 't': '\t', 'r': '\r'}.get(c, c)); i += 1
            else: buf.append(src[i]); i += 1
        i += 1
        return ''.join(buf)
    def value():
        nonlocal i
        ws(); c = src[i]
        if c == '{':
            i += 1; obj = {}
            while True:
                ws()
                if src[i] == '}': i += 1; return obj
                if src[i] in '"\'': k = string()
                else:
                    m = re.match(r'[A-Za-z_$][\w$-]*', src[i:]); k = m.group(0); i += len(k)
                ws()
                if src[i] == ':':
                    i += 1; obj[k] = value()
                else: obj[k] = None  # shorthand (set,)
                ws()
                if src[i] == ',': i += 1
        if c == '[':
            i += 1; arr = []
            while True:
                ws()
                if src[i] == ']': i += 1; return arr
                arr.append(value()); ws()
                if src[i] == ',': i += 1
        if c in '"\'`': return string() if c != '`' else template()
        m = re.match(r'-?\d+(\.\d+)?', src[i:])
        if m: i += len(m.group(0)); return float(m.group(0)) if '.' in m.group(0) else int(m.group(0))
        m = re.match(r'[A-Za-z_$][\w$.]*', src[i:])
        i += len(m.group(0)); w = m.group(0)
        return {'true': True, 'false': False, 'null': None, 'undefined': None}.get(w, '@' + w)
    def template():
        nonlocal i
        i += 1; j = src.index('`', i); s = src[i:j]; i = j + 1; return s
    return value()

count = bad = 0
with open(out, 'w', encoding='utf-8') as fo:
    for dirpath, _, files in os.walk(root):
        for f in sorted(files):
            if not f.endswith('.ts'): continue
            path = os.path.join(dirpath, f)
            rel = os.path.relpath(path, root).split(os.sep)
            if len(rel) != 3: continue  # serie/set/card.ts
            text = open(path, encoding='utf-8').read()
            m = re.search(r'const\s+card\s*:\s*Card\s*=\s*', text)
            if not m: continue
            try:
                card = js_to_py(text[m.end():])
            except Exception as e:
                bad += 1; continue
            card.pop('set', None)
            card['_serie'], card['_set'], card['_id'] = rel[0], rel[1], f[:-3]
            fo.write(json.dumps(card, ensure_ascii=False) + '\n'); count += 1
print('cards', count, 'unparsed', bad)
