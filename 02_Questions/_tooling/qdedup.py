import json, itertools, collections

P = "/tmp/claude-1000/-mnt-c-Users-sayba/6bcd62c7-f882-419b-b32c-ce4999571e73/scratchpad/"
recs = json.load(open(P + "questions.json"))
for r in recs:
    r["set"] = set(r["toks"])

def jac(a, b):
    u = len(a | b)
    return len(a & b) / u if u else 0.0

# --- 1. internal near-duplicate clusters ---
n = len(recs)
parent = list(range(n))
def find(x):
    while parent[x] != x:
        parent[x] = parent[parent[x]]
        x = parent[x]
    return x
def union(a, b):
    ra, rb = find(a), find(b)
    if ra != rb:
        parent[rb] = ra

buckets = collections.defaultdict(list)
for i, r in enumerate(recs):
    for t in r["set"]:
        buckets[t].append(i)

pairs = set()
for t, idxs in buckets.items():
    if len(idxs) > 300:
        continue
    for a, b in itertools.combinations(idxs, 2):
        pairs.add((a, b))

DUP = 0.62
for a, b in pairs:
    if jac(recs[a]["set"], recs[b]["set"]) >= DUP:
        union(a, b)

clusters = collections.defaultdict(list)
for i in range(n):
    clusters[find(i)].append(i)
multi = [c for c in clusters.values() if len(c) > 1]
cross = [c for c in multi if len({recs[i]["file"] for i in c}) > 1]

print("=" * 78)
print(f"TOTAL QUESTIONS EXTRACTED : {n}")
print(f"NEAR-DUP CLUSTERS (>=2)   : {len(multi)}")
print(f"  ...spanning >1 file     : {len(cross)}")
dupq = sum(len(c) - 1 for c in multi)
print(f"REDUNDANT QUESTIONS       : {dupq}  ({100*dupq/n:.1f}% of corpus)")
print(f"UNIQUE TOPICS             : {n - dupq}")
print("=" * 78)
print()
print("TOP 18 MOST-DUPLICATED QUESTIONS ACROSS FILES")
print("-" * 78)
for c in sorted(cross, key=len, reverse=True)[:18]:
    print(f"\n[{len(c)}x] {recs[c[0]]['q'][:110]}")
    seen = set()
    for i in c:
        f = recs[i]["file"]
        if f in seen:
            continue
        seen.add(f)
        print(f"       - {f}:{recs[i]['line']}")

# --- 2. the 14 asked questions vs corpus ---
HOT = [
 "How is memory managed in an AI agent",
 "How do you train Azure Document Intelligence custom models on documents",
 "You have 1 million documents how do you design Azure AI Search",
 "Which models do you choose and why model selection",
 "How do you manage the context window if it grows too large",
 "What type of context compression do you use",
 "Design the end to end lifecycle of a RAG system",
 "Why and when do you use AKS KEDA autoscaling for AI workloads",
 "Which chunking strategy is best and which one do you choose",
 "How do you handle PII management and redaction",
 "How can we do token saving and reduce cost",
 "Why A2A agent to agent protocol",
 "Explain the entire agent process you implemented end to end",
 "Explain each component of Azure AI Foundry",
]
STOP = set("""a an the is are was were be been being of to in on for with and or as at by from that this these those it its
how what what type which where who do does did you your i my we our can could should would will shall may might must
if then than so such into about over under between within across per vs versus explain describe walk me through
""".split())
def norm(q):
    return {t for t in q.lower().replace('-', ' ').split() if t not in STOP and len(t) > 2}

print()
print("=" * 78)
print("THE 14 ASKED QUESTIONS vs EXISTING CORPUS  (best Jaccard match)")
print("=" * 78)
for h in HOT:
    hs = norm(h)
    best = sorted(recs, key=lambda r: jac(hs, r["set"]), reverse=True)[:2]
    s = jac(hs, best[0]["set"])
    tag = "COVERED " if s >= 0.45 else ("PARTIAL " if s >= 0.28 else "**GAP** ")
    print(f"\n{tag} [{s:.2f}]  {h}")
    for b in best:
        print(f"          {jac(hs, b['set']):.2f}  {b['q'][:88]}")
        print(f"                {b['file']}:{b['line']}")
