"""Finds the strings the UI code can show: chains of string literals joined by + with other expressions ({0}, {1}...)."""
import json, re, sys, glob, collections

TOKEN = re.compile(r'''(?P<vstr>\$?@"(?:[^"]|"")*")|(?P<str>\$?"(?:\\.|[^"\\])*")|(?P<chr>'(?:\\.|[^'\\])')|(?P<cmt>//[^\n]*|/\*.*?\*/)|(?P<id>[A-Za-z_]\w*)|(?P<num>\d[\w.]*)|(?P<op>==|!=|<=|>=|&&|\|\||\?\?|=>|\+\+|--|[-+*/%<>=!?:;,.()\[\]{}&|^~])|(?P<ws>\s+)''', re.S)

def tokens(src):
    out = []
    for m in TOKEN.finditer(src):
        k = m.lastgroup
        if k in ('ws', 'cmt'): continue
        out.append((k, m.group(0), m.start()))
    return out

def unquote(s):
    if s.startswith('$'): return None  # interpolated strings: handled as one literal below
    if s.startswith('@'): return s[2:-1].replace('""', '"')
    body = s[1:-1]
    return re.sub(r'\\(u[0-9a-fA-F]{4}|.)', lambda m: chr(int(m.group(1)[1:], 16)) if m.group(1)[0] == 'u' and len(m.group(1)) == 5 else {'n': '\n', 't': '\t', 'r': '\r', '0': '\0'}.get(m.group(1), m.group(1)), body)

def chains(toks):
    """Maximal a + b + c chains at one paren depth containing a string literal. Returns lists of operands (token lists)."""
    res = []
    i, n = 0, len(toks)
    while i < n:
        if toks[i][0] not in ('str', 'vstr'): i += 1; continue
        # walk left to the start of this chain's first operand
        start = i
        j = i - 1
        depth = 0
        # find operand start: go left while previous token is '+' joining operands
        def operand_left(k):
            d = 0
            while k >= 0:
                t = toks[k][1]
                if t in ')]': d += 1
                elif t in '([':
                    if d == 0: return k + 1
                    d -= 1
                elif d == 0 and t in (',', ';', '=', '{', '}', '?', ':', '==', '!=', '&&', '||', 'return', '=>', '??', '+'): return k + 1
                k -= 1
            return 0
        def operand_right(k):
            d = 0
            while k < n:
                t = toks[k][1]
                if t in '([': d += 1
                elif t in ')]':
                    if d == 0: return k
                    d -= 1
                elif d == 0 and t in (',', ';', '{', '}', '?', ':', '==', '!=', '&&', '||', '=>', '??', '+'): return k
                k += 1
            return n
        s = operand_left(i - 1) if i > 0 else 0
        while s > 1 and toks[s - 1][1] == '+':
            s = operand_left(s - 2)
        ops = []
        k = s
        while True:
            e = operand_right(k)
            ops.append(toks[k:e])
            if e < n and toks[e][1] == '+': k = e + 1
            else: break
        res.append((s, e, ops))
        i = max(e, i + 1)
    return res

def template(ops):
    parts, args = [], 0
    for op in ops:
        if len(op) == 1 and op[0][0] in ('str', 'vstr') and unquote(op[0][1]) is not None:
            parts.append(unquote(op[0][1]).replace('{', '{{').replace('}', '}}'))
        else:
            parts.append('{%d}' % args); args += 1
    return ''.join(parts)

if __name__ == '__main__':
    files = sys.argv[1:]
    found = collections.Counter()
    where = {}
    for f in files:
        toks = tokens(open(f, encoding='utf-8').read())
        for s, e, ops in chains(toks):
            t = template(ops)
            if not re.search(r'[A-Za-z]{2}', t.replace('{', '').replace('}', '')): continue
            found[t] += 1
            where.setdefault(t, f.split('/')[-1])
    json.dump([{'en': t, 'file': where[t]} for t in sorted(found)], open('/dev/stdout', 'w'), ensure_ascii=False, indent=0)
