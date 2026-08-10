import os, re, json, collections

ROOT = "/mnt/c/pers/AIML-Learn"
STOP = set("""a an the is are was were be been being of to in on for with and or as at by from that this these those it its
how what why when which where who do does did you your i my we our can could should would will shall may might must
if then than so such into about over under between within across per vs versus explain describe walk me through
""".split())

qpat = [
    re.compile(r'^\s*#{2,6}\s*Q?\d*[\.\)]?\s*(.+\?)\s*$'),
    re.compile(r'^\s*\*\*Q\d*[\.\):]?\*\*\s*(.+\?)\s*$'),
    re.compile(r'^\s*\*\*(?:Q\d*[\.\):]?\s*)?(.+\?)\*\*\s*$'),
    re.compile(r'^\s*\d+[\.\)]\s+(.+\?)\s*$'),
    re.compile(r'^\s*[-*]\s+(.+\?)\s*$'),
    re.compile(r'^\s*#{2,6}\s*Q\d+[\.\)]\s*(.+?)\s*$'),
]

def norm(q):
    q = q.lower()
    q = re.sub(r'`|\*|_|"|\'', '', q)
    q = re.sub(r'[^a-z0-9\s\-]', ' ', q)
    toks = [t for t in q.split() if t not in STOP and len(t) > 2]
    return toks

records = []
for dp, dn, fn in os.walk(ROOT):
    if '.git' in dp:
        continue
    for f in fn:
        if not f.endswith('.md'):
            continue
        p = os.path.join(dp, f)
        rel = os.path.relpath(p, ROOT)
        try:
            lines = open(p, encoding='utf8', errors='ignore').read().splitlines()
        except Exception:
            continue
        for i, ln in enumerate(lines):
            if '?' not in ln or len(ln) > 400:
                continue
            for pat in qpat:
                m = pat.match(ln)
                if m:
                    q = m.group(1).strip()
                    if len(q) < 15 or len(q.split()) < 4:
                        break
                    toks = norm(q)
                    if len(toks) < 3:
                        break
                    records.append({"file": rel, "line": i + 1, "q": q, "toks": toks})
                    break

print("TOTAL QUESTION-LIKE LINES:", len(records))
byfile = collections.Counter(r["file"] for r in records)
print("FILES WITH QUESTIONS:", len(byfile))
print()
for f, c in byfile.most_common(60):
    print(f"{c:5d}  {f}")

json.dump(records, open("/tmp/claude-1000/-mnt-c-Users-sayba/6bcd62c7-f882-419b-b32c-ce4999571e73/scratchpad/questions.json", "w"), indent=0)
