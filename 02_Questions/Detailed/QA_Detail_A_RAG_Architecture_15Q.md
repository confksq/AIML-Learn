# Section A — RAG Architecture & Design (15 Q, detailed)

**Created:** 2026-08-08
**Format:** per `02_Questions/00_PLAN_InterviewQA_2026-08-08.md` §5, extended to
**WHAT · WHY · WHEN · HOW** + your example + the trade-off.
**Companion revision layer:** `02_Questions/InterviewBank/03_RAG_Architecture.md` (18 Q, terse)
**Companion self-test:** `02_Questions/PerChapter/QA_L13_RAG_DeepDive.md` (34 Q)

---

## ⚠️ Read this before you drill

**One source anchor in the input list contradicts the resume.** Q4's anchor says *"JMA's
prod index is keyword-only; staging adds vectors."* The resume claims **95% retrieval
accuracy via hybrid vector/keyword** in production at JM Family. Both cannot be true in the
same sentence to the same interviewer.

Pick one story and hold it everywhere:

- **Option A (matches resume):** prod is hybrid. The keyword-only phase was the *baseline
  you migrated from*, and it is the thing the 95% number is measured against.
- **Option B (matches the anchor):** prod is keyword-only for a specific index (e.g. exact
  dealer-code lookup) and hybrid on the semantic index. Then the 95% belongs to the
  semantic index only, and you must say which.

**Option A is the safer telling** and is what this document uses. If Option B is the truth,
edit Q4 and Q15 here before you drill them, because an interviewer who hears "keyword-only
in prod" will immediately ask what the 95% was measured on.

---

## Q1. What are the two independent failure points in a RAG pipeline?

**Difficulty:** Medium · **Key terms:** retrieval vs generation, grounding, faithfulness

**What they're testing:** whether you debug systematically or reach for prompt tweaks first.

**60-second spoken answer:**
> A RAG pipeline fails in two independent places, and they need completely different fixes.
> Retrieval failure means the right chunk never reached the model — wrong embedding, bad
> chunking, wrong filter. Generation failure means the right chunk *was* in the context and
> the model ignored it, misread it, or overrode it with parametric knowledge. My first
> debugging step is always to dump the retrieved chunks for the failing query before I touch
> a single word of the prompt, because no prompt engineering in the world fixes a retrieval
> miss.

### WHAT it is
Two sequentially-dependent but independently-failing stages:

| | Retrieval failure | Generation failure |
|---|---|---|
| Symptom | Answer is vague, "I don't know", or drawn from training data | Answer is confident and wrong *despite* correct context |
| Evidence | Correct chunk absent from top-K | Correct chunk present in top-K |
| Metric that catches it | context recall, recall@k | faithfulness / groundedness |
| Fix lives in | chunking, embedding, index, query | prompt, model, temperature, context ordering |

### WHY they are independent
They are separated by the context boundary. Retrieval is an **information-availability**
problem solved by search engineering; generation is an **information-use** problem solved
by prompt and model engineering. Optimising one cannot compensate for the other — a
perfectly-worded prompt over missing evidence still produces a wrong answer, and a perfect
retrieval feeding a 0.9-temperature model still drifts.

### WHEN this framing matters
Every RAG incident triage. Also whenever someone proposes "let's just improve the prompt"
as the response to a quality complaint — that proposal is only valid after you have proved
retrieval is clean.

### HOW you actually diagnose
1. Log the retrieved chunk IDs, scores, and text for the failing query.
2. Read them yourself. Is the answer physically present in that text?
3. **Not present** → retrieval bug. Go to chunking / embedding / hybrid / filters.
4. **Present** → generation bug. Go to prompt, context position, temperature, model tier.
5. Only then change something. One variable at a time.

### Your example
At JM Family I inspect the retrieved chunks per query before touching the prompt. That
ordering is the standing debug protocol on the enterprise search pipeline — chunks first,
prompt second.

### The trade-off
Per-query chunk logging costs storage and creates a PII surface — you are persisting
document content in your telemetry. At JMA that means the debug log is access-controlled
and short-retention, not open in the general observability dashboard.

**Follow-up probes:**
- *"How do you tell which one failed?"* → Dump the retrieved chunks. Answer present = generation; absent = retrieval.
- *"What if both are failing?"* → Fix retrieval first. Generation metrics are meaningless over garbage context.

**Red flag:** jumping straight to "I'd improve the prompt" without asking what was retrieved.

---

## Q2. Walk through an end-to-end RAG pipeline.

**Difficulty:** Easy · **Key terms:** ingestion vs query pipeline, hybrid search, re-ranking

**What they're testing:** can you whiteboard the system cleanly, and do you know it is *two*
pipelines, not one.

**60-second spoken answer:**
> There are two pipelines, and conflating them is the classic mistake. The **indexing
> pipeline** is offline and runs once per document: load, extract, clean, chunk, embed,
> write to the index with metadata. The **query pipeline** is online and runs per question:
> embed the question, hybrid retrieve, re-rank, assemble a grounded prompt with source tags,
> generate, then post-check groundedness and validate the citations before the answer is
> returned. At JM Family that is Document Intelligence for extraction, Azure AI Search for
> the index and the semantic ranker, and Azure OpenAI for generation.

### WHAT it is

**Indexing (offline, per document):**
```
Source → Load/OCR → Clean → Chunk → Embed → Index (vector + text + metadata)
```

**Query (online, per question):**
```
Question → [rewrite] → Embed → Hybrid retrieve (top ~50)
         → Re-rank (top ~5) → Assemble grounded prompt
         → Generate → Groundedness + citation check → Answer
```

### WHY it is split this way
Cost and latency live on opposite sides. Embedding 500K documents is an enormous one-time
batch cost you can amortise, schedule, and run on spot capacity. The query path is
latency-critical and runs thousands of times a day. Separating them lets you spend heavily
offline to make the online path cheap and fast — the entire economic argument for RAG over
context-stuffing.

### WHEN each stage is optional
| Stage | Skip it when |
|---|---|
| OCR / Document Intelligence | Source is already clean text (Confluence, DB, markdown) |
| Query rewriting | Single-turn only, no pronouns or ellipsis |
| Re-ranking | Corpus is tiny, or latency budget is under ~300 ms |
| Groundedness check | Internal low-stakes tooling; never for regulated output |

### HOW cost distributes
Concentrated in three places, in this order:
1. **Generation tokens** — usually the largest line item; driven by how many chunks you stuff.
2. **Embedding at ingest** — big one-time hit, then marginal on deltas.
3. **Index storage + query units** — AI Search SU sizing, replicas × partitions.

The cheapest lever is almost always *retrieve fewer, better chunks* — it cuts generation
tokens and improves quality simultaneously.

### Your example
JM Family's pipeline is Document Intelligence → Azure AI Search → Azure OpenAI, over 500K+
finance and insurance documents. Token-budget management and model-tier selection across
that path is where the ~30% inference cost reduction (~$150K/yr) came from.

### The trade-off
Every stage you add buys quality and costs latency, money, and a new failure mode. A
five-stage query path has five things that can break at 3 a.m. Add re-ranking, rewriting,
and groundedness checks because an eval proved you need them — not because the reference
architecture diagram had them.

**Follow-up probes:**
- *"Where does cost concentrate?"* → Generation tokens first, embedding at ingest second, index SUs third.
- *"What's the p95 latency budget?"* → Retrieval ~100 ms, rerank ~100 ms, generation dominates the rest.

**Red flag:** describing one linear flow and forgetting the indexing pipeline exists.

---

## Q3. How do you choose a chunking strategy?

**Difficulty:** Medium · **Key terms:** recursive splitting, overlap, parent-child, semantic chunking

**What they're testing:** whether you tune against evidence or cargo-cult "512 with 50 overlap".

**60-second spoken answer:**
> My default is recursive, structure-aware splitting at around 512 tokens with 10 to 20
> percent overlap, splitting on natural boundaries — headings, paragraphs, sentences — before
> falling back to character counts. Tables and headings get preserved as units, because a
> table split mid-row is destroyed information. For hierarchical documents I use parent-child:
> embed the small chunk for retrieval precision, return the parent section for generation
> context. But the honest answer is that the strategy is chosen by running retrieval eval
> across candidates, not by guessing. At JM Family the dealer forms use table-aware chunking
> specifically because the naive splitter was cutting rows in half.

### WHAT the options are

| Strategy | Mechanism | Best for |
|---|---|---|
| Fixed-size | N tokens, hard cut | Uniform prose, baseline only |
| Recursive / structure-aware | Split on ¶ → sentence → char, respecting headings | **Default for most corpora** |
| Document-layout aware | Uses DI layout model: tables, sections, page bounds | Forms, invoices, scanned PDFs |
| Parent-child (small-to-big) | Embed child, retrieve child, return parent | Long hierarchical docs, policies |
| Semantic chunking | Split where consecutive-sentence embedding similarity drops | Unstructured narrative, no headings |

### WHY chunk size is a real trade-off
Chunk size sets a precision/context dial:

- **Too small** — high embedding precision, but the retrieved fragment lacks the surrounding
  context needed to answer. You retrieve the right sentence and still can't answer.
- **Too large** — the embedding becomes an average of several topics, so it matches
  everything weakly and nothing strongly. Retrieval precision collapses and you burn tokens.

Overlap exists to stop a fact from being severed at a boundary — the 10–20% band is enough
to carry a sentence across the seam without materially inflating index size.

### WHEN to deviate from the default
- **Tables/forms** → layout-aware, never fixed-size. A table row is atomic.
- **Code** → split on function/class boundaries, not tokens.
- **Chat transcripts / tickets** → split per turn or per thread, keep the speaker label.
- **Legal/policy with clause numbering** → parent-child, child = clause, parent = section.
- **Very short docs (FAQ)** → don't chunk. One doc = one chunk.

### HOW you decide, concretely
1. Build a golden set of ~50–100 real questions with the document/passage that answers each.
2. Index the same corpus 3–4 ways (fixed 256, recursive 512/10%, recursive 1024/15%, parent-child).
3. Measure **context recall** and **NDCG@5** per variant — retrieval metrics only, no LLM.
4. Take the winner; re-check after any embedding-model change, because the optimum moves.

### HOW to fix a table split mid-row
Stop using a text splitter for that document class. Use Document Intelligence's layout
model to extract the table as a structured object, serialise it whole (markdown or
row-per-line with the header repeated), and emit it as its own chunk with
`contentType: table` metadata. If a table genuinely exceeds the chunk budget, split by
*rows* and repeat the header row in every fragment — never split within a row.

### Your example
JM Family dealer forms use table-aware chunking. The forms are dense tabular PDFs; a naive
recursive splitter separated column headers from their values, so retrieved chunks were
numbers with no labels — technically retrieved, semantically useless.

### The trade-off
Overlap multiplies index size and cost (20% overlap ≈ 20% more vectors to store, embed, and
search). Parent-child doubles your storage and adds a lookup hop. Semantic chunking needs an
embedding pass at *ingest* just to decide the boundaries — real money at 500K documents.

**Follow-up probes:**
- *"How would you fix a table split mid-row?"* → Layout-model extraction, serialise the table whole, repeat headers if you must split by row.
- *"What's your overlap and why?"* → 10–20%; enough to carry a sentence across the seam without inflating the index materially.
- *"Does chunk size change with the embedding model?"* → Yes — re-run the eval when you change models; the optimum moves.

**Red flag:** "512 tokens with 50 overlap" delivered as a universal constant with no eval behind it.

---

## Q4. What is hybrid search and why use it?

**Difficulty:** Medium · **Key terms:** BM25, RRF, dense + sparse

**What they're testing:** do you understand that vector search has a specific, predictable
blind spot.

**60-second spoken answer:**
> Hybrid search runs a keyword search and a vector search in parallel and fuses the two
> ranked lists, usually with Reciprocal Rank Fusion. You do it because the two methods fail
> in opposite directions. Keyword search nails exact tokens — part numbers, dealer codes,
> policy IDs, error strings — which embeddings blur into their nearest neighbours. Vector
> search catches paraphrase and synonymy, which keyword search misses entirely. Fused, they
> beat either alone on almost every enterprise corpus, which is why hybrid is my default.
> At JM Family the hybrid vector-plus-keyword configuration is what got retrieval accuracy
> to 95% against our labelled eval set.

### WHAT it is
Two retrievers, one fused result:

- **Sparse (BM25 / keyword)** — lexical. Scores on term frequency, inverse document
  frequency, and length normalisation. Matches the literal token.
- **Dense (vector)** — semantic. Cosine/dot similarity in embedding space. Matches meaning.
- **Fusion (RRF)** — `score(d) = Σᵢ 1 / (k + rankᵢ(d))`, k ≈ 60. Note it uses **rank**, not
  score, so the two systems' incomparable score scales never need calibrating.

### WHY RRF specifically
BM25 returns unbounded relevance scores; cosine similarity returns roughly 0–1. There is no
principled way to add them. RRF sidesteps the problem entirely by discarding magnitudes and
using only ordinal position. The `k` constant damps the influence of the very top rank so
one retriever cannot completely dominate the fusion. It is parameter-light, robust, and
needs no training data — which is why it is the industry default rather than a learned fusion.

### WHY vector search alone fails on enterprise data
Embeddings compress meaning, and exact identifiers have no meaning to compress. `DLR-4471`
and `DLR-4417` land in nearly the same place in embedding space. Any corpus full of codes,
SKUs, account numbers, statute references, or error codes will show this failure — and
enterprise corpora are exactly that.

### WHEN keyword-only is the right answer
- The query is always an **exact identifier lookup** (dealer code → record).
- The corpus is **highly structured with controlled vocabulary** — users type the same terms the docs use.
- **Latency or cost floor** rules out the embedding call per query.
- **Explainability is mandatory** — BM25 can point at the matched term; a vector score cannot.
- No embedding budget for the ingest volume.

### WHEN vector-only is the right answer
Rarely, in enterprise. It is defensible for pure-narrative corpora with no identifiers, or
cross-lingual retrieval where lexical overlap is zero by construction.

### HOW you implement it in Azure AI Search
One query object carrying both a `search` text clause and a `vectorQueries` clause; the
service executes both and applies RRF natively. Add `queryType: semantic` to stack the
semantic ranker on top of the fused list. Tune with `k` (RRF constant) and per-retriever
weights where exposed.

### Your example
JM Family's production index is hybrid — vector plus keyword — and that configuration is
what the 95% retrieval accuracy figure is measured against. The keyword-only setup was the
baseline it replaced.

> **Consistency check:** see the ⚠️ note at the top of this file. If your true story is that
> prod is keyword-only, you must change this paragraph *and* be able to say precisely what
> corpus and configuration the 95% was measured on.

### The trade-off
Hybrid costs an embedding call per query (latency + money), roughly doubles index storage
because you keep both an inverted index and vectors, and adds a fusion parameter to tune.
On a small controlled-vocabulary corpus it can measurably *underperform* pure BM25 by
letting semantically-near-but-wrong documents into the top-K.

**Follow-up probes:**
- *"When is keyword-only right?"* → Exact-ID lookup, controlled vocabulary, hard latency floor, or mandatory explainability.
- *"Why RRF and not weighted score fusion?"* → Score scales are incomparable; RRF uses rank only, so no calibration is needed.
- *"What's `k` in RRF?"* → A damping constant, ~60; it limits how much the single top hit dominates the fused ranking.

**Red flag:** "hybrid is best practice" with no account of *what* each retriever catches.

---

## Q5. What is re-ranking and when do you add it?

**Difficulty:** Medium · **Key terms:** bi-encoder vs cross-encoder, semantic ranker

**What they're testing:** the recall-then-precision pattern, and whether you know why the
retriever can't just be more accurate in the first place.

**60-second spoken answer:**
> Re-ranking is a second, more expensive scoring pass over the candidates the retriever
> already found. The retriever is a bi-encoder — query and document are embedded separately,
> so the document vectors can be precomputed and searched in milliseconds, but the model
> never sees the pair together. A cross-encoder does: it feeds query and document through
> the model jointly and scores the pair, which is far more accurate and far too slow to run
> over a whole index. So the pattern is retrieve wide and cheap for recall — top 50 — then
> re-rank narrow and expensive for precision — top 5. In Azure, the semantic ranker is a
> managed cross-encoder that does this for you. I run it on the JM Family search path.

### WHAT the two encoder types actually do

| | Bi-encoder (retriever) | Cross-encoder (re-ranker) |
|---|---|---|
| Input | Query and doc encoded **separately** | Query and doc encoded **jointly** as one sequence |
| Precompute | Doc vectors built at index time | Nothing — every pair is a fresh forward pass |
| Cost per query | 1 embedding + ANN search | N forward passes for N candidates |
| Accuracy | Good | Substantially better |
| Scales to | Millions of docs | Tens of candidates |

### WHY the cross-encoder is more accurate
The bi-encoder must compress a document into a fixed vector **before it knows what will be
asked**. That vector is a lossy summary optimised for average-case queries. The
cross-encoder sees query and document together, so attention runs *across* them — it can
weigh the specific term in the document that the specific query cares about. The
information the bi-encoder had to throw away is exactly what the cross-encoder gets to use.

### WHY you can't just use the cross-encoder for retrieval
It has no index. Scoring is O(corpus size) forward passes per query — for 500K documents
that is 500K model invocations for one question. The bi-encoder's whole value is that ANN
search over precomputed vectors is sublinear.

### WHEN to add re-ranking
Add it when:
- Eval shows **recall@50 is high but precision@5 is low** — the right chunk is being found but ranked below the noise. This is the textbook signal.
- You want to **cut generation cost**: better top-5 means you can stuff 5 chunks instead of 15.
- The corpus has many **near-duplicate or boilerplate** passages that all score similarly.

Skip it when:
- Recall@50 itself is poor — fix retrieval first; re-ranking cannot surface what was never retrieved.
- Latency budget is genuinely under ~300 ms end-to-end.
- The corpus is small enough that top-K is already precise.

### HOW to size it
Retrieve 30–50 candidates, re-rank to 3–5. Below ~20 candidates there is little for the
re-ranker to reorder; above ~100 you pay real latency for diminishing returns. Azure's
semantic ranker operates over the top 50 and returns a `@search.rerankerScore` on a 0–4
scale — that score is usefully thresholdable as a "do we have anything good at all" gate.

### Your example
Semantic ranker is enabled on the JM Family search path. It sits on top of the hybrid
result set, so the fusion produces the candidate pool and the ranker produces the final
order that reaches the prompt.

### The trade-off
Latency — typically tens to low hundreds of milliseconds on top of retrieval. Cost — Azure
bills the semantic ranker per thousand queries, on top of your search tier. And it is
another dependency: if the ranker is throttled or down, you need a defined fallback to the
fused order rather than an error.

**Follow-up probes:**
- *"What's the latency cost?"* → Tens to low hundreds of ms for a managed ranker over ~50 candidates.
- *"Retrieve 50 or 200?"* → 30–50 is the sweet spot; past ~100 you pay latency for marginal gain.
- *"What if the ranker is down?"* → Fall back to the RRF order and log it — degrade, don't fail.

**Red flag:** describing re-ranking as "a better embedding model" — it's a different architecture, not a better one of the same kind.

---

## Q6. RAG answer is wrong but the doc exists in the index — diagnose it.

**Difficulty:** Hard · **Key terms:** retrieval miss vs generation override

**What they're testing:** live debugging discipline under an open-ended prompt. This is Q1
as a scenario.

**60-second spoken answer:**
> "The doc is in the index" only rules out ingestion. It doesn't tell me whether the doc was
> *retrieved*. So step one is to dump the top-K for that exact query and look for the chunk.
> If it's absent, it's a retrieval problem and I work backwards: was it chunked so the answer
> got split across a boundary, is the query vocabulary diverging from the document
> vocabulary, is a metadata filter excluding it, is it ranked at position 30 when I'm taking
> top 5? If it *is* present in the context and the answer is still wrong, it's a generation
> problem — the model is overriding context with parametric knowledge, or the context is
> buried mid-prompt, or two retrieved chunks contradict each other and it picked the wrong
> one. Two different failure classes, two different fixes.

### WHAT the fault tree looks like

```
Doc is in the index, answer is wrong
│
├─ Is the right chunk in the top-K for this query?
│
├─ NO → RETRIEVAL FAILURE
│   ├─ Chunking      → answer split across a chunk boundary; overlap too small
│   ├─ Embedding     → query/doc vocabulary mismatch → try hybrid, HyDE, rewriting
│   ├─ Filtering     → a metadata filter silently excluded it (date, docType, ACL)
│   ├─ Ranking       → it's at rank 30, you take top 5 → add re-ranking, widen K
│   └─ Index state   → chunk exists but its vector is stale/null from a failed embed
│
└─ YES → GENERATION FAILURE
    ├─ Override      → model prefers parametric knowledge → strengthen grounding instruction
    ├─ Position      → chunk is mid-context → "lost in the middle" → reorder (see Q7)
    ├─ Conflict      → two chunks disagree → add recency/authority metadata, instruct precedence
    ├─ Comprehension → answer needs multi-chunk synthesis the model didn't perform
    └─ Sampling      → temperature too high → drop to 0–0.2
```

### WHY "it's in the index" proves almost nothing
Indexed, retrievable, retrieved, and *used* are four different states. A document can be
perfectly indexed and never surface because its embedding is a poor match for how users
phrase the question. It can surface at rank 40 and be cut. It can be filtered out by an ACL
trim the user doesn't know applies. Being in the index only eliminates the first of five
possible failures.

### WHEN each branch is most likely
- Query contains an **exact code** and it failed → embedding blur, you need keyword/hybrid.
- Failure is **specific to long documents** → chunking or position.
- Failure appeared **after a re-index** → stale or failed embeddings, filter regression.
- Failure is **intermittent across identical queries** → temperature, or a replica serving a stale index.
- The model answers with something **plausible and generic** → parametric override.

### HOW to isolate it fast
1. Re-run the query with `top=50` and no filters. If the chunk appears now → filter or ranking.
2. Search the index directly for a distinctive phrase from the chunk. No hit → the chunk isn't what you think it is (bad OCR, failed embed).
3. Paste the retrieved context and question into the playground manually. Right answer there → the bug is in your assembly, not the model.
4. Drop temperature to 0 and re-run. Fixed → sampling.

### Your example
This is the standard JM Family debug order: retrieved chunks first, prompt second, model
last. The rule exists because the team's instinct was to edit the prompt, and prompt edits
on a retrieval miss produce no improvement and a lot of churn.

### The trade-off
Doing this properly requires per-query tracing of chunk IDs, scores, and the assembled
prompt — which means storing document content and user queries in telemetry. That is a
governance cost: access control, retention limits, and PII handling on the debug store.

**Follow-up probes:**
- *"The right chunk WAS retrieved and it's still wrong — now what?"* → Generation branch: check position in context, temperature, conflicting chunks, and whether the grounding instruction is strong enough.
- *"How do you catch this class of bug before users do?"* → Golden-set regression run in CI on every index or prompt change.

**Red flag:** proposing a fix before establishing which side of the context boundary the failure is on.

---

## Q7. What is "lost in the middle"?

**Difficulty:** Medium · **Key terms:** context positioning, attention, primacy/recency

**What they're testing:** whether you know that context is not a uniform bucket.

**60-second spoken answer:**
> It's the empirical finding that LLMs don't use long contexts uniformly. Accuracy follows a
> U-shape against position — information at the very beginning or the very end of the context
> is used reliably, and information buried in the middle is measurably more likely to be
> missed, even when the model has plenty of context window left. The practical consequence is
> that stuffing more chunks can make answers *worse*, not just more expensive. So I retrieve
> fewer and better, and I place the highest-relevance chunk at the start of the context. At
> JM Family the prompts put the top-ranked chunks first for exactly this reason.

### WHAT the finding is
From Liu et al., *Lost in the Middle: How Language Models Use Long Contexts* (2023). Models
were given a set of documents with the answer placed at varying positions. Retrieval-style
accuracy was highest when the answer sat first or last and dropped noticeably in the middle
— a U-shaped curve. It reproduces across model families and persists in models with very
large advertised context windows.

### WHY it happens
Two effects compound. **Primacy** — early tokens are attended to by every subsequent token
and often sit near system-prompt content the model is trained to weight heavily. **Recency**
— tokens nearest the generation point dominate the immediate attention distribution. The
middle has neither advantage, and as sequence length grows, attention mass per token thins
out. A large context window guarantees the tokens *fit*; it guarantees nothing about how
well they are *used*.

### WHEN it bites you
- Top-K is large (10, 15, 20 chunks) — the good chunk is now sitting at position 8.
- Long conversation histories where the relevant earlier turn is now mid-prompt.
- Whole-document stuffing "because the window is 128K".
- Multi-document synthesis where evidence is spread across positions.

### HOW to design around it
1. **Retrieve fewer, better.** Re-ranking exists partly to make a small top-K sufficient.
2. **Order by relevance, don't just concatenate.** Rank 1 goes first.
3. **Bracket the critical content** — highest-relevance chunk first, and restate the actual
   question at the very end, immediately before generation. You then own both high-attention
   positions.
4. **Cap top-K empirically.** Run your eval at K = 3, 5, 10, 20 and find where quality peaks
   and turns over. It usually turns over well before the context limit.
5. For genuinely large evidence sets, **summarise or map-reduce** rather than stuffing.

### HOW it changes top-K
It converts top-K from "as much as fits" into a tuned parameter with an interior optimum.
Bigger K raises recall (the chunk is more likely present) while lowering usable precision
(it's more likely buried). Those curves cross. Find the crossing point with eval, and let
re-ranking push it leftward — better ordering means a smaller K suffices.

### Your example
JM Family prompts place the top-ranked chunks first, with the user's question restated at
the end of the prompt. Combined with the semantic ranker producing a good top-5, this keeps
the context short enough that the middle barely exists — which is the real fix.

### The trade-off
Cutting K to protect against position effects reduces recall: if the answer chunk was at
rank 7 and you now take 5, you lose it entirely. That is precisely why re-ranking and
position management are a package — you can only afford a small K if the ordering is trustworthy.

**Follow-up probes:**
- *"How does it change top-K?"* → Makes it a tuned parameter with an interior optimum, not "as much as fits".
- *"Doesn't a 128K window solve this?"* → No. The window governs what fits, not what's attended to. The U-shape persists.
- *"Where do you put the question — top or bottom?"* → Both, ideally: instruction up top, question restated at the end.

**Red flag:** treating context window size as a quality guarantee.

---

## Q8. When does GraphRAG beat vector RAG?

**Difficulty:** Hard · **Key terms:** knowledge graph, multi-hop, community summaries, entity extraction

**What they're testing:** whether you can name the specific query class vector search
structurally cannot serve — and whether you're honest about the cost.

**60-second spoken answer:**
> Vector RAG retrieves by similarity, which means it can only return passages that resemble
> the question. It has no notion of a relationship, so it cannot traverse. Two query classes
> break it. First, multi-hop relationship questions — "which dealers share a manager who
> handles late accounts" — where no single passage contains the answer; it has to be
> assembled by walking edges. Second, global corpus synthesis — "what are the recurring
> themes across these 10,000 tickets" — where the answer is a property of the whole corpus,
> not of any retrievable chunk. GraphRAG handles both by extracting entities and relations
> into a knowledge graph and pre-computing community summaries. At KPMG the GraphRAG and
> Neo4j work delivered a 35% retrieval-accuracy lift on multi-hop questions, and I've built
> a Neo4j GraphRAG portfolio module. The catch is indexing cost — it's an LLM call per chunk
> to extract entities, so ingestion gets dramatically more expensive.

### WHAT GraphRAG is
An indexing-time transformation, not a different retriever:
1. Chunk the corpus as usual.
2. **LLM-extract entities and relationships** from each chunk → nodes and typed edges.
3. Resolve duplicate entities across documents into single nodes.
4. **Detect communities** in the graph (Leiden or similar clustering).
5. **LLM-summarise each community**, producing hierarchical summaries.

At query time: **local search** traverses from matched entities across N hops; **global
search** answers corpus-wide questions by map-reducing over community summaries.

### WHY vector search structurally cannot do this
An embedding encodes *what a passage is about*, not *what it connects to*. Similarity is
symmetric and single-hop by construction. "Dealer A's manager is Smith" and "Smith handles
late accounts" are two passages, neither of which resembles the question "which dealers have
a manager handling late accounts" strongly enough to guarantee retrieval — and even if both
are retrieved, the *join* is left entirely to the LLM's reasoning over unstructured text.
The graph makes the join an explicit traversal instead of a hope.

For global questions the failure is even cleaner: "what are the main themes" has no answer
in any chunk, so top-K retrieval over chunks retrieves K arbitrary chunks. The community
summary is the only artifact that actually holds the answer.

### WHEN to use it — and when not to

| Use GraphRAG | Stick with vector RAG |
|---|---|
| Multi-hop relational questions | Single-passage factoid lookup |
| Global synthesis / thematic questions | "What does policy X say about Y" |
| Corpus with rich, extractable entity structure | Unstructured prose with few entities |
| Investigations, org/ownership, supply chain, fraud rings | FAQ, support docs, product manuals |
| Corpus is relatively stable | Corpus changes hourly |

### HOW you'd build it
1. Define the **ontology first** — entity types and relation types you care about. Extracting
   "everything" produces an unusable hairball.
2. Extract per chunk with a structured-output LLM call against that schema.
3. **Entity resolution** — this is the hard part. "J.M. Family", "JM Family Enterprises", and
   "JMFE" must become one node or the graph is worthless.
4. Load to Neo4j (or Cosmos DB Gremlin); index entity names for lookup.
5. Keep the vector index too — hybrid graph+vector is the practical production shape: vector
   finds the entry points, the graph does the traversal.

### Your example
At KPMG I built GraphRAG on Neo4j 5.x alongside Azure AI Search, which lifted retrieval
accuracy 35% on multi-hop contract questions and served 200+ concurrent users. I've also
built a standalone Neo4j GraphRAG portfolio module (Part 6, module 07) to keep the
implementation detail fresh.

### The trade-off
This is the honest answer that separates a practitioner from a reader:

- **Indexing cost explodes.** One-plus LLM call per chunk for extraction, plus summarisation
  per community. Ingestion cost can be an order of magnitude above plain embedding.
- **Maintenance is real.** A changed document may invalidate edges, entity resolutions, and
  community summaries — it is not a simple upsert.
- **Extraction errors compound.** A wrong edge is a confidently wrong traversal path.
- **You now operate a graph database** — another system, another scaling story, another skill on the team.

So: GraphRAG when the question class genuinely requires traversal. Not as a default upgrade.

**Follow-up probes:**
- *"Cost trade-off?"* → Indexing is LLM-per-chunk, roughly an order of magnitude over embedding; query cost is comparable. You buy it for the query classes vector RAG can't serve at all.
- *"How do you build the graph?"* → Ontology first, structured-output extraction per chunk, then entity resolution — resolution is where it succeeds or fails.
- *"Do you drop the vector index?"* → No. Vector finds entry-point entities; the graph traverses from them.

**Red flag:** "GraphRAG is more accurate" with no query class named and no indexing-cost acknowledgement.

---

## Q9. RAG vs fine-tuning vs prompting — how do you decide?

**Difficulty:** Medium · **Key terms:** knowledge vs behavior problem

**What they're testing:** the single most common architecture decision in applied GenAI, and
whether you reach for the expensive option first.

**60-second spoken answer:**
> My rule is: fine-tune for behavior, RAG for knowledge, and try prompting before either.
> Prompting first, always — it's the cheapest experiment and it frequently wins. If the gap
> is that the model doesn't *know* something — private data, current data, or too much data
> to fit in a prompt — that's a knowledge problem and RAG solves it, because knowledge stays
> external and updateable. If the gap is that the model knows the content but won't behave —
> wrong format, wrong tone, wrong domain conventions, won't reliably emit valid JSON — that's
> a behavior problem, and fine-tuning teaches it in a way prompting can only approximate. At
> JM Family the policy content is RAG, and the only thing I'd fine-tune for is strict output
> format.

### WHAT each one actually changes

| | Prompting | RAG | Fine-tuning |
|---|---|---|---|
| Changes | The instruction | The context | The weights |
| Solves | Task framing, reasoning style | Missing/current/private knowledge | Behavior, format, tone, domain style |
| Update latency | Instant | Minutes (re-index) | Days (retrain) |
| Cost | ~zero | Ingest + per-query retrieval | Training + hosting the adapter |
| Auditability | Full | Full — you can cite the source | Poor — knowledge is opaque in weights |
| Fails at | Genuinely unknown facts | Teaching a *style* | Keeping facts current |

### WHY fine-tuning is the wrong tool for knowledge
Three reasons, and you should be able to give all three:

1. **Update cost.** Knowledge changes. Weights are the most expensive place to store anything
   that changes — every update is a retraining run.
2. **No citations.** Once a fact is in the weights you cannot point at its source. In finance,
   insurance, and healthcare, that alone disqualifies it.
3. **It doesn't reliably work.** Fine-tuning on a document set teaches the model the *style*
   of those documents far more reliably than the *facts* in them. You get a model that
   confidently sounds like your corpus while still hallucinating its contents — arguably the
   worst possible outcome.

### WHY prompting comes first
It is the only option with a same-day feedback loop and no infrastructure. A structured
prompt with few-shot examples solves a surprising share of "we need to fine-tune" requests.
Establishing the prompting baseline is also what makes the later decision defensible — you
cannot claim RAG or fine-tuning helped without a baseline to compare against.

### WHEN you use both together
Common and correct in production: **RAG supplies the facts, fine-tuning supplies the form.**
Fine-tune a small model to always emit your exact JSON schema and house tone, then feed it
retrieved context at inference. You get current, citable knowledge *and* reliable output
shape. It's also the standard route to cost reduction — a fine-tuned small model can match a
large model's output quality on a narrow task at a fraction of the token price.

### HOW to decide, as a checklist
1. Does the model already do this with a better prompt? → **Prompting.** Stop.
2. Is the missing piece *facts* — private, current, or high-volume? → **RAG.**
3. Is the missing piece *form* — schema, tone, domain conventions, task-specific reasoning
   the prompt can't reliably enforce? → **Fine-tuning.**
4. Both? → **RAG + fine-tune**, in that order of implementation.
5. Do you have 500+ high-quality labelled examples? If not, fine-tuning is not on the table
   regardless of the answers above.

### Your example
JM Family uses RAG for policy and finance content — it changes, and every answer must carry
a citation, so weights are structurally the wrong store. The one place I'd fine-tune is
strict JSON output format for downstream system integration, where prompting gets you to
"usually valid" and the integration needs "always valid".

### The trade-off
RAG adds per-query latency and retrieval infrastructure, and its answer quality is capped by
retrieval quality. Fine-tuning adds a training pipeline, a dataset you must curate and
maintain, a hosting cost for the adapter, and a re-tuning obligation every time the base
model version moves. Prompting is free but hits a ceiling you cannot engineer past.

**Follow-up probes:**
- *"When would you use both?"* → RAG for facts, fine-tune for format. Standard production shape.
- *"Why not fine-tune on the documents?"* → Teaches style, not facts; no citations; every update is a retrain.
- *"How many examples for fine-tuning?"* → Hundreds minimum for format/tone; below that, few-shot prompting wins.

**Red flag:** "we'd fine-tune on our documents so the model knows our data." This is the single most common wrong answer in the field.

---

## Q10. How do you handle multi-turn RAG (follow-up questions)?

**Difficulty:** Medium · **Key terms:** query rewriting, coreference resolution, standalone query

**What they're testing:** whether you've noticed that retrieval is stateless and chat is not.

**60-second spoken answer:**
> The core problem is that retrieval is stateless but conversation isn't. If a user asks
> "which of those are over $40k?", embedding that string retrieves nothing useful — "those"
> carries all the meaning and it isn't in the text. So before retrieval I run a rewriting
> step: a cheap LLM call takes the conversation history plus the new turn and emits a
> standalone, self-contained query — "which dealer service contracts in the Southeast region
> exceed $40,000". That's what gets embedded. At JM Family the chat resolves references like
> "that dealer" through exactly this rewrite step before it touches the index.

### WHAT the failure mode is
Conversational turns are elliptical by nature:
- **Pronouns / coreference** — "that dealer", "those contracts", "he"
- **Ellipsis** — "what about 2023?" (what *about* it?)
- **Refinement** — "only the ones over $40k" (only which ones?)
- **Topic shift** — the new turn is unrelated and history should be *ignored*

Each of these embeds to a vector that describes the *sentence*, not the *intent*.

### WHY you can't just embed the raw follow-up
Embeddings encode the literal text. "Which of those are over $40k?" is semantically about
comparison and money — it will happily retrieve any chunk mentioning dollar thresholds,
from any topic. The referent that makes the query meaningful exists only in prior turns, and
the embedding model never sees them.

### WHY you can't just embed the whole history either
The obvious alternative — concatenate all turns and embed that — fails in two directions.
It dilutes: five turns of history average out to a vector representing the conversation's
general topic rather than this specific question. And it poisons on topic change: a genuinely
new question retrieves documents about the *previous* subject. Concatenation trades one
failure for a subtler one.

### WHEN to rewrite (and when to skip it)
Rewrite when history exists **and** the new turn is context-dependent. Skip when:
- It's the first turn — nothing to resolve.
- The turn is already self-contained — a classifier or the rewriter itself can pass it through.
- Latency budget is tight and your traffic is overwhelmingly single-turn.

A cheap gate is worth it: ask the small model "is this query self-contained? yes/no" and
only pay for a rewrite on "no".

### HOW to implement it
1. Take the last N turns (3–5 is usually plenty) plus the new question.
2. Cheap, fast model — a small tier is correct here; this is not a reasoning task.
3. Prompt: *"Given the conversation, rewrite the final question as a standalone query that
   makes sense with no prior context. Preserve all entities and constraints explicitly. If it
   is already standalone, return it unchanged."*
4. Temperature 0.
5. **Log both** the original and the rewrite — when retrieval goes wrong in a chat, the
   rewrite is the first suspect, and you can't debug what you didn't log.
6. Retrieve with the rewrite; generate with the *original* question plus full history, so the
   answer sounds natural in the conversation.

That last point matters and is often missed: **the rewrite is for the retriever, not for the
user-facing generation.**

### Your example
JM Family's chat interface resolves references like "that dealer" via the rewrite step
before retrieval. Without it, the second turn in any conversation retrieved noise, which is
the exact symptom that motivated adding it.

### The trade-off
An extra LLM call on every turn — latency and cost on the critical path — and a new failure
mode where a bad rewrite silently destroys retrieval while everything downstream looks
healthy. It also mishandles genuine topic shifts unless you explicitly instruct it to detect
them. Mitigation is the self-contained gate plus logging both versions.

**Follow-up probes:**
- *"Why not just embed the raw follow-up?"* → "Those" carries the meaning and isn't in the text; you retrieve on the wrong signal.
- *"Why not embed the whole history?"* → Dilutes the current intent and poisons retrieval on topic change.
- *"Which model does the rewrite?"* → The cheapest capable one — it's extraction, not reasoning.

**Red flag:** answering "I'd pass the chat history to the LLM" — that's generation, not retrieval. The retriever never sees it.

---

## Q11. How do you force citations and grounding?

**Difficulty:** Medium · **Key terms:** citation validation, groundedness detection, source tagging

**What they're testing:** whether you know that asking a model to cite is not the same as
the citation being real.

**60-second spoken answer:**
> Three layers, and the third is the one people forget. First, structure the context — every
> chunk goes in tagged with an explicit identifier like Source 1, Source 2, carrying its
> document name and page. Second, instruct at the claim level — cite the source after every
> factual claim, and if the context doesn't contain the answer, say so rather than filling
> the gap. Temperature at or near zero. Third, and this is the important one, validate
> after generation: parse the citation IDs out of the answer and confirm each one actually
> exists in what was retrieved. Models fabricate citations, and a fabricated citation is
> worse than none because it manufactures false confidence. At JM Family every answer comes
> back with source references attached.

### WHAT the three layers are

**1 — Structure the context**
```
[Source 1] (FordInvoice_2026_01.pdf, p.2)
<chunk text>

[Source 2] (DealerPolicy_v4.pdf, p.17)
<chunk text>
```
The model can only cite what it can name. Give it stable, unambiguous handles.

**2 — Instruct at the claim level**
- Answer *only* from the provided sources.
- Cite the source ID immediately after each factual claim.
- If the sources don't contain the answer, say "I don't have that in the available documents."
- Do not use prior knowledge to fill gaps.
- Temperature 0–0.2.

**3 — Validate post-generation**
- Regex the `[Source N]` references out of the answer.
- Assert every N is in the set actually passed to the prompt.
- On mismatch: retry, strip the bad citation, or fail closed — a product decision, but it
  must be *a* decision.
- Optionally run a groundedness scorer (Azure AI Content Safety groundedness detection, or
  an LLM-as-judge) over answer-vs-context and gate on the score.

### WHY layer 3 is non-negotiable
An LLM generating "[Source 3]" is doing token prediction, not lookup. `[Source 3]` is a
highly probable continuation after a factual sentence in a cited-answer format — whether or
not a Source 3 was ever provided. Layers 1 and 2 make correct citation *likely*; only layer 3
makes it *verified*. And the failure is insidious: a fabricated citation looks exactly like
a real one to the user and actively increases their trust in a wrong answer.

### WHY low temperature helps
Sampling temperature governs how often the model departs from its highest-probability
continuation. Grounded extraction is precisely the task where you want no creativity —
you want the token that the context supports. Temperature 0 doesn't eliminate hallucination
but it materially reduces the drift-away-from-context class of it.

### WHEN to go further than this
- **Regulated output** (finance, insurance, health) → add the groundedness scorer as a hard gate, plus human review for low scores.
- **Extractive-only requirements** → require verbatim quoted spans and verify each span is a substring of the cited chunk. Strictest form, and fully checkable.
- **High-volume low-stakes** → layers 1 and 2 plus sampled auditing may be proportionate.

### HOW to handle "cites Source 3 but only 2 were retrieved"
That is a caught hallucination — the validator did its job. Response ladder:
1. **Log it** with the query, the context, and the answer. Track the rate; it's a model-quality signal.
2. **Retry once** at temperature 0 with a strengthened instruction naming the valid source IDs explicitly.
3. **If it recurs, fail closed** — return "I can't answer that from the available documents"
   rather than serving an answer with a stripped citation. Silently deleting the bad citation
   leaves the unsupported *claim* in place, which is the actual danger.
4. **If the rate is systemic**, the fix is upstream: too many chunks, confusing source
   labelling, or a model tier that can't hold the format.

### Your example
JM Family returns source references with every answer, and the pipeline validates them
against the retrieved set before the answer is served. This sits under the "eliminating
hallucinations" line on the resume — which you should restate in interviews as **measured
groundedness with validated citations**, not elimination. No one eliminates hallucination,
and claiming it invites a skeptical follow-up you can't win.

### The trade-off
Every layer costs. Source tags consume context tokens. Strict grounding instructions raise
refusal rate — the model says "not in the documents" for questions it could have partially
answered, and users experience that as unhelpfulness. Groundedness scoring is a second model
call per answer, adding latency and cost. The refusal-rate dial is a genuine product
decision: in regulated content, over-refusal is the correct bias.

**Follow-up probes:**
- *"It cites Source 3 but only 2 were retrieved — what happens?"* → Validator catches it; log, retry once, then fail closed. Never just strip the citation and keep the claim.
- *"How do you measure groundedness at scale?"* → Groundedness detection or LLM-as-judge over a sampled stream, plus RAGAS faithfulness on the golden set in CI.
- *"Doesn't strict grounding hurt UX?"* → Yes — it raises refusals. In regulated content that's the right trade.

**Red flag:** "I tell the model to cite its sources" and stopping there. Layer 2 without layer 3.

---

## Q12. What is query rewriting / HyDE?

**Difficulty:** Hard · **Key terms:** HyDE, multi-query, vocabulary mismatch, asymmetric retrieval

**What they're testing:** understanding of the query–document asymmetry problem, and whether
you know HyDE's real downside.

**60-second spoken answer:**
> Both attack the same problem — the query and the document are written in different
> languages, so their embeddings don't land near each other. Rewriting fixes it on the query
> side: expand abbreviations, add domain synonyms, or fan one question out into several
> sub-queries and union the results. HyDE goes further and inverts the problem. Instead of
> embedding the question, you have an LLM hallucinate a *plausible answer document*, then
> embed that and search with it. It works because you're now comparing a document to
> documents instead of a question to documents — the vocabulary and structure match. The
> downside is real: it's an extra LLM call per query, and on topics the model knows nothing
> about, the hypothetical answer is confidently wrong and drags retrieval off-target.

### WHAT the underlying problem is
Retrieval is **asymmetric**. A question — "how do I file a late claim?" — is short,
interrogative, and uses user vocabulary. The document that answers it — "Claim Submission
Procedures: submissions received after the 30-day window shall be processed under §4.2
exception handling" — is long, declarative, and uses institutional vocabulary. Embedding
models trained on general text place these further apart than their actual relevance
warrants.

### WHAT each technique does

| Technique | Mechanism |
|---|---|
| **Query expansion** | Add synonyms, expand acronyms, append domain terms |
| **Query rewriting** | LLM reformulates into retrieval-friendly phrasing (also does coreference — see Q10) |
| **Multi-query** | LLM generates 3–5 phrasings; retrieve each; union and dedupe |
| **Step-back prompting** | Generate a more general question first, retrieve on both |
| **HyDE** | LLM writes a hypothetical *answer*; embed the answer; retrieve with it |

### WHY HyDE works
It converts an asymmetric comparison into a symmetric one. The hypothetical answer is
generated in document register — declarative, domain-vocabulary, similar length and
structure to real documents. Its embedding therefore lands in the region of embedding space
where the real answer documents live. Critically, the hypothetical does **not** need to be
factually correct: it only needs to be *lexically and structurally* like the right document.
It's a query in disguise, not an answer.

### WHEN to use which
- **Vocabulary mismatch between users and docs** (users say "late claim", docs say
  "post-deadline submission") → HyDE or expansion.
- **Ambiguous or broad questions** → multi-query fan-out.
- **Conversational follow-ups** → rewriting (Q10) — this is the coreference case.
- **Highly technical corpus where users type exact terms** → skip all of it; you don't have
  a mismatch problem, and hybrid search already handles identifiers.
- **Hard latency budget** → skip; every one of these is an extra LLM call.

### HOW you'd implement HyDE
1. Cheap fast model, temperature ~0.3 (you want plausible document prose, not creativity).
2. Prompt: *"Write a short passage that would plausibly answer this question, in the style of
   an internal policy document."*
3. Embed the generated passage.
4. Retrieve with that vector — **and** run the original query through keyword search in
   parallel. Fuse with RRF.

Step 4 is the production-grade detail: keeping the original query in the hybrid arm is what
protects you when the hypothetical drifts.

### HyDE's downside — say this unprompted
1. **Extra LLM call per query** — latency and cost on the hot path, before retrieval even starts.
2. **Hallucination drift.** For niche, proprietary, or post-cutoff topics the model has no
   basis for the hypothetical. It generates confident, plausible, wrong prose — and you then
   search with it. Retrieval degrades *below* the plain-query baseline, and it degrades
   exactly where your corpus is most proprietary, which in enterprise is most of it.
3. **Non-determinism** — same question, different hypothetical, different results, harder to debug.
4. **Weakens exact matching** — the hypothetical won't contain the specific ID the user typed.

Mitigations: keep the original query in a hybrid arm (above); generate 2–3 hypotheticals and
average the embeddings to reduce variance; A/B it against baseline rather than assuming it helps.

### Your example
The portfolio RAG modules (Part 6 — LlamaIndex and Ollama Local RAG) demonstrate both
rewriting and HyDE, which is where I've done the hands-on comparison against a plain-query
baseline.

### The trade-off
Every one of these techniques buys recall with latency, cost, and non-determinism. Multi-query
multiplies retrieval calls by N. HyDE can actively hurt on proprietary corpora. None of them
should ship without an A/B against the plain-query baseline on your own golden set — this is
a family of techniques with genuinely mixed real-world results.

**Follow-up probes:**
- *"HyDE's downside?"* → Extra LLM call, and on unfamiliar topics the hypothetical is confidently wrong and drags retrieval below baseline.
- *"Does the hypothetical need to be correct?"* → No — it needs to be lexically and structurally document-like. It's a query in disguise.
- *"HyDE or multi-query?"* → Multi-query for ambiguity, HyDE for vocabulary mismatch. Multi-query is safer; HyDE has more upside.

**Red flag:** presenting HyDE as a free upgrade. The interviewer is waiting for the downside.

---

## Q13. How do you keep a RAG index fresh?

**Difficulty:** Medium · **Key terms:** incremental indexing, change feed, freshness SLA, soft delete

**What they're testing:** whether you think about the system after day one.

**60-second spoken answer:**
> Event-driven as the primary path, scheduled batch as the safety net. When a source document
> changes, a Blob storage event fires, a function picks it up, re-chunks and re-embeds only
> that document, and upserts by a stable document ID so the old chunks are replaced rather
> than duplicated. A nightly or weekly indexer run catches anything the event path dropped.
> The reason this matters more than it sounds: a stale index doesn't produce a *missing*
> answer, it produces a confident answer citing a superseded policy. That's worse than no
> answer, because it comes with a citation. At JM Family that's the EnterpriseSearch.Sync
> push pipeline.

### WHAT the options are

| Approach | Mechanism | Freshness | Cost |
|---|---|---|---|
| Full re-index | Rebuild everything | Hours–days | Very high |
| Scheduled incremental | Indexer + high-water-mark on `lastModified` | Minutes–hours | Low |
| **Event-driven** | Blob/Event Grid event → function → re-embed one doc | Seconds–minutes | Low, spiky |
| Hybrid (recommended) | Event-driven primary + scheduled reconciliation | Seconds, self-healing | Low |

### WHY event-driven alone isn't enough
Events get lost. The function throws and the message dead-letters; the event grid
subscription is misconfigured after a deploy; an out-of-band bulk edit bypasses the trigger
entirely; the embedding API throttles and the retry budget expires. Each of these leaves a
document permanently stale with no signal — the pipeline looks healthy because nothing
failed *now*. The scheduled reconciliation pass is what converts a silent permanent
inconsistency into a bounded, self-healing one.

### WHY deletion is the harder half
Updates are upserts and mostly take care of themselves. Deletions are the real problem: if a
document is removed from the source and its chunks survive in the index, you will serve
content that no longer exists — and in a right-to-erasure regime, that is a compliance
incident, not a bug. Blob deletion may not fire a usable event, so you need a **soft-delete
policy** (a metadata flag the indexer reads) or a periodic reconciliation that diffs source
IDs against indexed IDs and purges orphans.

### WHEN "fresh" means what
Set an explicit **freshness SLA** per content class — this is the answer that sounds senior:

| Content | SLA | Path |
|---|---|---|
| Pricing, rates, live policy | Minutes | Event-driven, alerted |
| Standard policy docs | Hours | Event-driven + nightly reconcile |
| Reference / archival | Days | Scheduled batch only |

Different content classes get different budgets. Treating all content as equally urgent is
how you overspend on ingestion.

### HOW to implement it properly
1. **Stable document ID** — a deterministic key (source path hash) so re-ingestion upserts
   rather than duplicates. Chunk IDs derive from it: `{docId}-{chunkIndex}`.
2. **Content hash in metadata** — skip the whole embed if the extracted text is unchanged.
   This is the single biggest cost saver; most "changes" are metadata touches.
3. **Delete-then-write per document** — remove all chunks for `docId`, then write the new
   set. A document that shrinks from 20 chunks to 12 otherwise leaves 8 orphans.
4. **Dead-letter queue with alerting** on the ingest function.
5. **Reconciliation job** — diff source inventory against indexed `docId` set; re-ingest
   missing, purge orphaned.
6. **Freshness telemetry** — track and dashboard max staleness age, not just success counts.

### HOW multi-region freshness works
The problem is that a region reindexing is briefly inconsistent with one that has finished.
Options, roughly in increasing order of cost and correctness:
- **Index once, replicate** — build in a primary region, replicate the index artifact out.
  Consistent, but replication lag applies.
- **Ingest independently per region** from the same event source — simpler, but regions
  diverge transiently and can serve different answers to the same question.
- **Blue/green index aliasing** — build the new index alongside the old, verify it, then flip
  the alias atomically per region. Best correctness; doubles index storage during the build.

Whichever you pick, define the acceptable divergence window and monitor it. "Eventually
consistent" without a stated bound is not an architecture.

### Your example
JM Family's EnterpriseSearch.Sync push pipeline handles this — source change events drive
incremental re-embedding into Azure AI Search rather than periodic full rebuilds, which is
what makes freshness affordable at 500K+ documents.

### The trade-off
Event-driven ingestion is spiky: a bulk source update fires thousands of concurrent events
and will throttle your embedding endpoint. You need a queue with controlled concurrency
between the event and the embedder. And per-document reprocessing has fixed overhead, so at
very high change rates micro-batching beats true per-event processing.

**Follow-up probes:**
- *"How do you handle multi-region freshness?"* → Blue/green alias flip per region, or replicate a single built index. State the divergence window either way.
- *"What about deletions?"* → Soft-delete flag plus reconciliation diff. Orphaned chunks are a compliance problem, not a quality one.
- *"How do you avoid re-embedding unchanged content?"* → Content hash in metadata; skip when unchanged.

**Red flag:** "we re-index nightly" as the whole answer, with no deletion story and no freshness SLA.

---

## Q14. What is CAG and how does it differ from RAG?

**Difficulty:** Hard · **Key terms:** KV cache, context stuffing, cache invalidation, TTFT

**What they're testing:** currency with 2024–25 techniques, and whether you can name the
constraint that decides between them.

**60-second spoken answer:**
> Cache-Augmented Generation loads the entire knowledge base into the model's context once,
> precomputes the key-value cache for it, and then reuses that cache across every query. There
> is no retrieval step at inference — no embedding call, no vector search, no re-ranker, and
> no possibility of a retrieval miss, because everything is already in context. The constraint
> is brutal and simple: the corpus has to fit in the context window, and it has to be
> essentially static, because any change invalidates the cache and you recompute. So CAG for
> small stable corpora — a product manual, a policy handbook, an API reference — and RAG for
> anything large or changing. I covered the comparison in the Ascendion prep.

### WHAT CAG does mechanically
1. Concatenate the whole knowledge base into one long prompt prefix.
2. Run one forward pass to compute the **KV cache** — the per-layer key and value tensors for
   every token in that prefix.
3. Persist that cache.
4. Per query: load the cache, append only the user's question, generate. The prefix is never
   re-processed.

The saving is the prefill. Attention over the knowledge base has already been computed; each
query pays only for its own short suffix.

### WHY it can beat RAG when it applies
- **No retrieval failure mode.** The single largest source of RAG errors — the right chunk
  not being retrieved — is structurally impossible. Q1's first failure point disappears.
- **Full-corpus reasoning.** The model sees everything at once, so cross-document synthesis
  and comparison questions work naturally. RAG's top-K is a hard information bottleneck.
- **Lower and more predictable latency.** No embedding call, no search round-trip, no rerank.
  Time-to-first-token drops.
- **Radically simpler architecture.** No vector store, no indexer, no chunking strategy, no
  ingestion pipeline to operate.

### WHY it doesn't replace RAG
- **Context window is a hard ceiling.** 500K documents will never fit. Not a tuning problem — a physical one.
- **Cache memory.** KV cache size scales with context length × layers × heads × precision.
  A very long prefix is a large, per-model-instance memory allocation, and it constrains how
  many concurrent requests a GPU can serve.
- **Invalidation is all-or-nothing.** Change one sentence and the cache from that token
  onward is invalid. There is no incremental update.
- **Attention still dilutes.** "Lost in the middle" (Q7) applies with full force to a very
  long static prefix.
- **No citations for free.** Nothing tells you which part of the corpus the answer came from.

### WHEN to choose which

| | CAG | RAG |
|---|---|---|
| Corpus size | Fits in context (~≤100K tokens practical) | Unbounded |
| Change rate | Rare — weekly or slower | Any |
| Citations | Hard | Native |
| Infra | Model + cache | Vector store + pipeline |
| Fits | Product manual, API docs, policy handbook, onboarding FAQ | Enterprise document search |

### HOW you'd handle a weekly-updating corpus
This is the natural follow-up, and the answer is "it depends on the shape of the update":

- **Weekly is genuinely fine for CAG** if the update is a scheduled batch. Recompute the
  cache as part of the weekly release, warm it, then flip. Users never see the rebuild.
- **Hybrid is the better answer for mixed content.** Cache the stable core — the policy
  handbook that changes quarterly — and RAG the volatile tail — this week's rate updates.
  One prompt, two sources, and each mechanism does what it's good at.
- **Go RAG if the weekly update is unpredictable or mid-week hotfixes happen**, because the
  operational cost of an unscheduled full cache rebuild on a live service is what actually
  kills CAG deployments.

Also worth naming: **prompt caching** on Azure OpenAI and Anthropic's API is the commercially
available, discount-priced cousin of this idea — you get prefix-reuse economics on a static
prompt prefix without operating your own KV cache. For most teams that is the practical
on-ramp to CAG-style benefits.

### Your example
Covered in the Ascendion prep material (Module 04 — CAG vs RAG, vector stores, agent flow),
which is where I worked through the decision boundary in detail.

### The trade-off
CAG trades flexibility for simplicity and latency. You get a much simpler system with no
retrieval failures, and you pay with a hard corpus ceiling, all-or-nothing invalidation, and
per-instance memory pressure that limits concurrency. The failure mode is also different in
kind: RAG degrades gracefully as the corpus grows, while CAG works perfectly right up to the
context limit and then doesn't work at all.

**Follow-up probes:**
- *"Corpus updates weekly — CAG or RAG?"* → CAG is viable on a scheduled rebuild; hybrid (cached stable core + RAG'd volatile tail) is usually better; RAG if updates are unpredictable.
- *"Why not just stuff the context every query without caching?"* → You'd pay full prefill on every request. The KV cache is the entire point.
- *"How does it interact with prompt caching APIs?"* → Same principle, managed for you and discount-priced. Usually the pragmatic starting point.

**Red flag:** describing CAG as "just putting everything in the prompt" — that's context stuffing. The precomputed, reused KV cache is what makes it CAG.

---

## Q15. How do you evaluate a RAG pipeline?

**Difficulty:** Medium · **Key terms:** RAGAS, faithfulness, context recall/precision, NDCG, golden dataset

**What they're testing:** whether you can measure the two failure points from Q1 separately.
This question closes the loop the whole section opened with.

**60-second spoken answer:**
> You evaluate the two stages separately, because a single end-to-end score can't tell you
> which one broke. Retrieval gets information-retrieval metrics against a labelled golden
> set — context recall and context precision, plus NDCG or MRR for ranking quality.
> Generation gets faithfulness, which measures whether the claims in the answer are actually
> supported by the retrieved context, and answer relevance, which measures whether it
> addressed the question. RAGAS and the Azure AI Foundry evaluators both compute these. The
> golden dataset is the real work — a hundred or so representative questions with known
> correct sources, curated with the business, versioned, and run in CI on every prompt,
> chunking, or index change. I built a RAGAS module in the portfolio for exactly this.

### WHAT to measure, by stage

**Retrieval (no LLM needed — cheap, deterministic, fast):**
| Metric | Measures | Catches |
|---|---|---|
| Context recall | Did we retrieve everything needed? | Chunking, embedding, filter bugs |
| Context precision | Are relevant chunks ranked high? | Ranking / re-ranker quality |
| Recall@k / MRR / NDCG@k | Standard IR ranking quality | Regression on index changes |

**Generation (LLM-as-judge — slower, costs money, some variance):**
| Metric | Measures | Catches |
|---|---|---|
| Faithfulness | Are the answer's claims supported by the context? | Hallucination, parametric override |
| Answer relevance | Did it address the question asked? | Evasion, off-topic drift |
| Answer correctness | Does it match the reference answer? | End-to-end quality |

**Operational, always:** p50/p95 latency, cost per query, refusal rate, citation-validation
failure rate.

### WHY the split is the entire point
This is Q1 expressed as measurement. If you only track one end-to-end score and it drops
from 0.82 to 0.71, you know something broke and nothing else. With staged metrics the
diagnosis is immediate:

- **Context recall down, faithfulness flat** → retrieval regressed. Look at chunking,
  embeddings, index state, filters.
- **Context recall flat, faithfulness down** → generation regressed. Look at prompt, model
  version, temperature, context ordering.
- **Both down** → something upstream of both: ingestion, or a bad deploy.

That table is the answer to "end-to-end quality dropped — which stage?" and it's worth being
able to say it cleanly.

### WHY the golden dataset is the hard part
Metrics are library calls; the labelled set is judgement. It must be:
- **Representative** — sampled from real query logs, not invented by engineers. Engineers
  write questions their system can answer.
- **Labelled with source ground truth**, not just an expected answer string — otherwise you
  cannot compute retrieval metrics at all.
- **Business-validated** — an SME confirms the expected answers, or you are measuring
  agreement with your own assumptions.
- **Versioned in git**, alongside the code it gates.
- **Adversarial in part** — include questions the corpus genuinely cannot answer, and assert
  the system refuses. A RAG system that never says "I don't know" is broken in a way accuracy
  metrics won't show.

100–200 well-chosen questions beats 10,000 synthetic ones.

### WHEN you run what
| Trigger | Suite | Why |
|---|---|---|
| Every PR touching prompt/chunking/index config | Full golden set | Cheap insurance, catches regressions |
| Nightly | Full set + operational metrics | Trend detection |
| Continuously in prod | Sampled online eval — groundedness + citation validation on real traffic | Golden sets go stale; real queries drift |
| Model version change | Full set, both stages | Provider updates silently change behaviour |

Online sampled eval matters: your golden set reflects the questions you anticipated, and
production drifts away from it.

### HOW to act on the numbers
Define release gates, not dashboards. Something like: faithfulness ≥ 0.85, context recall
≥ 0.90, no metric regressed more than 3 points from the previous release. Below the gate,
the change doesn't ship. Without a gate, evaluation is decoration.

### Your example
JM Family runs LLMOps with RAGAS 0.4 on faithfulness, answer relevance, and context recall,
and the hybrid retrieval configuration measures 95% retrieval accuracy against the labelled
eval set. I also built a standalone RAGAS evaluation module in the portfolio (Part 6,
module 03) covering the metric implementations directly.

> **Be ready to defend the 95%.** Know: what metric it is (recall@k against the golden set),
> how many questions were in the set, who labelled them, and what the failing 5% looked like.
> This is flagged in the plan of record as the most attackable number on the resume, and
> "95% accuracy" without those four facts sounds unmeasured.

### The trade-off
LLM-as-judge metrics cost money per evaluation run and carry their own variance — the judge
is a model and can be wrong or drift between versions. Retrieval metrics are cheap and
stable but can't see generation quality at all. And a golden dataset is a maintained asset:
it decays as the corpus and the user base change, so it needs periodic refresh from real
query logs or it silently stops representing production.

**Follow-up probes:**
- *"End-to-end quality dropped — which stage?"* → Check retrieval metrics first. Recall down = retrieval; recall flat but faithfulness down = generation.
- *"How big is the golden set?"* → 100–200 representative, SME-validated, source-labelled questions, including unanswerable ones.
- *"What does faithfulness actually compute?"* → Decompose the answer into claims, check each against the retrieved context, score = supported claims / total claims.
- *"Who labels it?"* → SMEs from the business. If engineering labels it, you're measuring your own assumptions.

**Red flag:** "we use RAGAS" with no golden dataset, no gate, and no separation of retrieval from generation metrics.

---

## Drill sheet — the one-line version of each

| # | Question | The sentence that must appear |
|---:|---|---|
| 1 | Two failure points | Retrieval vs generation — dump the chunks before touching the prompt |
| 2 | End-to-end pipeline | Two pipelines: offline indexing, online query |
| 3 | Chunking strategy | Recursive/structure-aware default, tuned by retrieval eval, tables are atomic |
| 4 | Hybrid search | BM25 catches IDs, vectors catch paraphrase, RRF fuses by rank not score |
| 5 | Re-ranking | Bi-encoder retrieves wide for recall; cross-encoder narrows for precision |
| 6 | Wrong answer, doc indexed | Indexed ≠ retrieved. Check top-K first, then branch |
| 7 | Lost in the middle | U-shaped attention; retrieve fewer, put the best first |
| 8 | GraphRAG | Multi-hop traversal and global synthesis; indexing cost is the price |
| 9 | RAG vs FT vs prompting | Fine-tune for behavior, RAG for knowledge, prompt first |
| 10 | Multi-turn | Retrieval is stateless — rewrite to a standalone query before embedding |
| 11 | Citations | Tag, instruct, **and validate** — models fabricate source IDs |
| 12 | Rewriting / HyDE | Fixes query–document asymmetry; HyDE hurts on unfamiliar corpora |
| 13 | Index freshness | Event-driven primary + scheduled reconcile; deletions are the hard half |
| 14 | CAG | Precomputed KV cache over a static corpus; no retrieval, hard size ceiling |
| 15 | Evaluation | Measure retrieval and generation separately, gate releases on a golden set |

---

## Cross-references

| This question | Goes deeper in |
|---|---|
| Q1, Q6, Q15 | `InterviewBank/06_Responsible_AI_LLMOps.md`; `PerChapter/QA_L19_MLOps_LLMOps.md` |
| Q2, Q3, Q13 | `PerChapter/QA_L13_RAG_DeepDive.md`; `PerChapter/QA_L08_DocumentIntelligence.md` |
| Q4, Q5, Q13 | `PerChapter/QA_L09_AzureAISearch.md` |
| Q7, Q14 | `HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` §2 |
| Q8 | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/` |
| Q9 | `PerChapter/QA_L14_FineTuning.md` |
| Q11, Q15 | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/03-RAGAS-Evaluation/` |
| Q14 | `08_Jobs/AscndIntr/PrepPlan/ChatHist/Module04_RAG_CAG_VectorStores_AgentFlow_2026-06-25.md` |
