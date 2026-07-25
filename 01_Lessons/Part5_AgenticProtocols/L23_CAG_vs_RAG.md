# Module 02 — CAG vs RAG
**Interview:** Ascendion AI Architect | Healthcare Client
**Your anchor:** JM Family production — RAG pipelines, Azure AI Search, document retrieval
**Schedule:** Wednesday 06/17 Block 2 (carried to Thursday 06/18)

---

## Section 1: What They Are and Why the Distinction Matters

Every LLM has a knowledge problem: its training data has a cutoff date, it cannot access your private enterprise data, and it hallucinates when asked about things it does not know well. The two main architectural patterns for solving this are **RAG** and **CAG**.

**RAG — Retrieval-Augmented Generation**
At inference time, before calling the LLM, you retrieve relevant documents from an external store and inject them into the prompt as context. The LLM answers using both its training knowledge and the retrieved documents.

```
User Query
    ↓
[Retrieval Step] → Search Azure AI Search / vector DB
    ↓
Retrieved chunks injected into prompt
    ↓
LLM generates answer grounded in retrieved context
    ↓
Response to user
```

**CAG — Cache-Augmented Generation**
You pre-load a large, static knowledge base directly into the LLM's extended context window at startup — and keep it there across all queries via KV cache. No retrieval step at inference time. The model answers directly from what is already in context.

```
Startup: Load full knowledge base into context (cached)
    ↓
User Query
    ↓
LLM generates answer from cached context — no retrieval step
    ↓
Response to user
```

The critical difference: **RAG fetches at query time. CAG pre-loads at startup.**

---

## Section 2: How RAG Works — The Architecture

RAG has three phases: indexing, retrieval, and generation.

### Phase 1 — Indexing (offline, done once or on update)
1. Chunk your documents (clinical guidelines, policy documents, FHIR records)
2. Embed each chunk using an embedding model (text-embedding-ada-002)
3. Store embeddings in a vector index (Azure AI Search with vector fields)
4. Store the original text alongside for retrieval

### Phase 2 — Retrieval (at query time)
1. Embed the user's query with the same embedding model
2. Run similarity search (cosine similarity) against the vector index
3. Retrieve top-K most relevant chunks (typically top 3-5)
4. Apply metadata filters (date range, department, document type)

### Phase 3 — Generation
1. Inject retrieved chunks into the system prompt as context
2. LLM generates answer grounded in retrieved context
3. Groundedness evaluation checks: is every claim in the answer supported by the retrieved context?

### Hybrid Search (important for healthcare)
Azure AI Search supports **hybrid search** — combines vector similarity (semantic meaning) with BM25 keyword matching (exact term matching). For clinical queries, hybrid search outperforms pure vector search because medical terminology matters. "MI" and "myocardial infarction" mean the same thing semantically but exact keyword matching catches ICD codes and drug names that embeddings can miss.

**⚙️ Config or Code?**
- **Portal Config only:** Create Azure AI Search index (portal), define vector field + HNSW algorithm settings (portal index schema), enable semantic ranker (portal toggle), set hybrid search profile (portal)
- **Custom Code:** Phase 1 chunking logic (Python/C# to split docs), embedding generation (call text-embedding-3-large API per chunk), push chunks + vectors to index (SDK Push API), Phase 2 query embedding + search call (SDK), Phase 3 prompt assembly with retrieved chunks (code), groundedness check integration (code)
- **Both:** Metadata filtering (define filterable fields = Config in index schema; apply filters at query time = Code)

---

## Section 3: How CAG Works — The Architecture

CAG exploits one capability of modern LLMs: very large context windows and KV (key-value) cache.

1. At startup, load your entire knowledge base into the context window (clinical guidelines, formulary, payer policies)
2. The LLM processes this once and caches the internal key-value representations
3. Every subsequent query reuses the cached KV state — no retrieval step, no embedding similarity search
4. The model answers directly from what is already in its "working memory"

**Why KV cache matters:** Modern LLMs like GPT-4o and Claude can handle 128K–200K token context windows. Processing 200K tokens once and caching the result is cheap. Serving that cache across thousands of queries is very cheap per query.

**Where CAG is used today:**
- Claude's extended context projects feature (load a full codebase into context)
- Systems where the knowledge base is small enough to fit in context and changes rarely
- Offline-capable edge deployments where you cannot run a vector DB

---

## Section 4: Head-to-Head Comparison

| Dimension | RAG | CAG |
|---|---|---|
| **Knowledge freshness** | Real-time — update the index, immediately available | Stale until restart — requires reloading context |
| **Knowledge size** | Unlimited — index can be terabytes | Bounded — must fit in context window (128K–200K tokens max) |
| **Latency** | Higher — retrieval adds a round-trip before LLM call | Lower — no retrieval step, but first-load is expensive |
| **Cost** | Search cost per query + LLM cost | High first-load cost, low per-query cost after |
| **Hallucination risk** | Lower — groundedness is measurable against retrieved chunks | Higher — harder to audit which part of context grounded the answer |
| **Auditability** | High — you know exactly which chunks were retrieved | Low — the model mixes everything in context, hard to trace |
| **Complexity** | Higher — embedding pipeline, vector DB, chunking strategy | Lower — no retrieval infrastructure needed |
| **Knowledge scope** | Targeted — retrieves only what is relevant | Full — entire knowledge base is always in context |

---

## Section 5: When to Use Which — The Decision Framework

### Use RAG when:
- Knowledge base is large (more than a few hundred pages)
- Knowledge changes frequently (drug formulary updated weekly, payer policies updated quarterly)
- You need auditability — you must be able to show which source grounded the answer
- PHI is involved — you cannot load all patient records into context, you retrieve only the relevant record
- You need metadata filtering — retrieve only cardiology guidelines, not oncology

### Use CAG when:
- Knowledge base is small and stable (fits comfortably in context)
- Latency is critical and you cannot afford a retrieval round-trip
- The use case is offline or edge-deployed (no vector DB available)
- You need the model to reason holistically across the entire knowledge base, not just top-K chunks

### The hybrid case:
In practice, most production systems combine both. Load static, rarely-changing reference data via CAG (clinical decision rules, system instructions, taxonomy), and retrieve dynamic, PHI-specific data via RAG (the specific patient record, the specific payer policy for this member).

**Healthcare example — Prior Authorization Agent:**
- CAG layer: Load the 50-page payer coverage policy into context at agent startup. It does not change more than quarterly. No need to chunk and retrieve it.
- RAG layer: At query time, retrieve the specific patient's FHIR record — you cannot pre-load all 2 million member records into context. Retrieve only this member's clinical history.

---

## Section 6: Healthcare Context

### Why RAG dominates healthcare:
1. **PHI isolation** — you never pre-load patient records. You retrieve only the record for the patient in the current session. HIPAA requires minimum necessary access — RAG enforces this architecturally.
2. **Auditability** — CMS and Joint Commission audits require you to show the source of every clinical recommendation. RAG gives you the retrieved chunks as evidence. CAG cannot show you which part of the 200K-token context grounded the answer.
3. **Knowledge freshness** — clinical guidelines, drug formularies, payer policies update frequently. RAG indexes pick up changes immediately. CAG requires a full context reload.
4. **Scale** — a health system has millions of patient records and thousands of clinical documents. No context window holds that.

### Healthcare RAG examples:

**Ambient Documentation:**
At the end of a patient encounter, the physician asks the AI to generate a SOAP note. RAG retrieves the patient's recent encounter history (last 3 visits), relevant problem list entries, and current medications from FHIR. The LLM generates the note grounded in those retrieved records. Groundedness evaluation ensures every clinical claim in the note is supported by a retrieved FHIR resource — not hallucinated.

**Prior Authorization:**
PA agent retrieves the payer's coverage policy for the requested procedure (via Azure AI Search on the payer policy document store) + the patient's clinical evidence from FHIR. Generates a PA recommendation grounded in both. Cites specific policy clauses and clinical evidence in the output for human reviewer audit.

**Clinical Decision Support:**
Physician types: "What is the recommended hypertension protocol for a diabetic patient with CKD stage 3?" RAG retrieves the relevant sections from ACC/AHA guidelines and the hospital's formulary. Generates recommendation with source citations. Every claim is traceable to a guideline chunk.

### Where CAG makes sense in healthcare:
- Loading a stable reference taxonomy (ICD-10 code descriptions, CPT code descriptions) into context for a coding agent
- Edge-deployed clinical tools in rural hospitals with limited connectivity where running a vector DB is impractical
- A chatbot grounded in a single, stable 50-page clinical protocol document

---

## Section 7: Your JM Family Anchors

**RAG at JM Family:**
> *"At JM Family I built RAG pipelines using Azure AI Search with hybrid search — combining vector similarity with BM25 keyword matching. Documents were chunked, embedded with text-embedding-ada-002, and indexed. At query time we retrieved top-5 chunks with metadata filters, then ran groundedness evaluation to ensure every answer was supported by retrieved context before surfacing to users."*

**When asked about CAG:**
> *"CAG is a complementary pattern — instead of retrieving at query time, you pre-load a stable knowledge base into the model's extended context window once and cache the KV state. At JM Family we used a hybrid approach: static reference data was pre-loaded as system context, and dynamic document retrieval was handled by RAG. The decision framework is simple: if the knowledge is large, changes frequently, or involves PHI — use RAG. If it is small, stable, and you need lower latency — CAG is viable."*

---

## Section 8: CTO Summary — Your 60-Second Verbal Answer

*"RAG and CAG solve the same problem differently — how do you give an LLM access to knowledge it was not trained on.*

*RAG retrieves at query time: embed the query, similarity-search a vector index, inject the top chunks into the prompt as context. The model answers grounded in what was retrieved. This is the dominant pattern in healthcare because it handles large knowledge bases, keeps knowledge fresh, enforces PHI minimum-necessary-access architecturally, and gives you auditable source citations.*

*CAG pre-loads a knowledge base into the LLM's extended context window at startup and caches the key-value state. No retrieval step — lower latency, simpler infrastructure, but bounded by context window size and knowledge becomes stale until you reload.*

*In practice I use both: CAG for small stable reference data like a payer coverage policy or clinical taxonomy, RAG for anything that involves PHI, changes frequently, or is too large to fit in context. At JM Family all dynamic document retrieval ran through RAG with Azure AI Search hybrid search and automated groundedness evaluation."*

---

## Section 9: Q&A Drill

**Instructions:** Answer each question in 3-4 sentences minimum. Hit all four points: what it IS, why it works that way, healthcare example, and the tradeoff or when not to use it.

---

**Q1. What is the difference between RAG and CAG? When would you use each?**

> **Expected:** RAG retrieves relevant documents from an external store at query time and injects them as context — the model answers grounded in what was just retrieved. CAG pre-loads a full knowledge base into the model's extended context window at startup and caches the KV state — no retrieval step at inference time. Use RAG when knowledge is large, changes frequently, involves PHI, or requires auditability — which is most healthcare use cases. Use CAG when the knowledge base is small, stable, fits in context, and you need lower latency — like loading a single payer coverage policy for a PA agent.

---

**Q2. Why does RAG dominate in healthcare rather than CAG?**

> **Expected:** Four reasons: PHI isolation — you cannot pre-load all patient records, you retrieve only the record for the current patient, which architecturally enforces HIPAA minimum necessary access. Auditability — clinical AI systems must show which source grounded every recommendation; RAG gives you the retrieved chunks as evidence, CAG cannot trace which part of 200K tokens grounded the answer. Knowledge freshness — drug formularies and payer policies update weekly or quarterly; RAG indexes pick up changes immediately, CAG requires a full context reload. Scale — a health system has millions of patient records, no context window holds that.

---

**Q3. A physician says the CDS tool gave a recommendation that seemed to come from nowhere — no source cited and possibly outdated. What does this tell you about the architecture and how do you fix it?**

> **Expected:** This tells you either retrieval failed (returned no relevant chunks so the model generated from parametric training knowledge) or groundedness gates are missing (the model was allowed to answer even when it did not retrieve supporting context). Fix: add a minimum retrieval confidence threshold — if similarity score falls below threshold, do not send to LLM, surface a "I cannot find supporting evidence — consult a clinical resource" response. Add groundedness evaluation to every response — flag and log any response where the answer is not supported by retrieved context. Add the failure case to the golden evaluation dataset so this class of failure is caught in offline evaluation before any future deployment.

---

**Q4. How does hybrid search in Azure AI Search improve RAG for clinical queries?**

> **Expected:** Hybrid search combines vector similarity search (semantic meaning, captures concepts) with BM25 keyword search (exact term matching). For clinical queries this matters because medical terminology is precise — "MI" and "myocardial infarction" mean the same thing semantically, but pure vector search might miss exact ICD-10 codes, drug names, or CPT codes that keyword matching catches. The two scores are combined using Reciprocal Rank Fusion — you get the semantic understanding of vector search plus the precision of keyword matching. In healthcare I would always use hybrid search over pure vector search.

---

**Q5. You are building a Prior Authorization agent. Describe how you would combine CAG and RAG in the same workflow.**

> **Expected:** Use CAG for the payer's coverage policy document — it is 50 pages, stable, updates at most quarterly, and the agent needs to reason holistically across the entire policy. Load it into context at agent startup and cache it. Use RAG for the specific patient's clinical evidence — you cannot pre-load 2 million member FHIR records, and PHI minimum-necessary-access requires you retrieve only this patient's data. At query time, retrieve the patient's diagnosis codes, procedure history, and clinical notes from FHIR via Azure AI Search. The agent reasons across both: the cached policy in context and the retrieved patient evidence injected at query time.

---

---

## Section 10 — CV SKILL: Chunking Strategies + HNSW Indexing

> **CV anchor:** "applied chunking strategies (fixed-size, semantic, paragraph-level) tuned to token budget and retrieval precision requirements" and "HNSW indexing, cosine similarity scoring"

### Chunking Strategies — the three types

**Why chunking matters:** You cannot embed an entire 100-page clinical guideline as one vector. You split it into chunks — each chunk gets its own vector. The quality of your chunks directly determines the quality of your RAG retrieval.

| Strategy | How it works | Best for | Healthcare example |
|---|---|---|---|
| **Fixed-size** | Split every N tokens regardless of content | Simple, fast, consistent | Lab result feeds where every entry is ~200 tokens |
| **Semantic** | Split at semantic boundaries — topic changes | Documents with distinct topics in one file | Clinical guidelines where each section covers a different condition |
| **Paragraph-level** | Split at paragraph breaks | Narrative documents with natural paragraph structure | Physician SOAP notes, discharge summaries |

**The token budget constraint:**
```
Context window = 128K tokens (GPT-4o)
You inject top-3 chunks into the prompt

Each chunk must be:
├── Large enough to contain a complete clinical concept
└── Small enough that 3 chunks + system prompt + query fits in context

Too small chunks → missing context → hallucination
Too large chunks → fewer chunks fit → less coverage
Sweet spot for clinical docs → 300-500 tokens per chunk
```

**Overlap — why chunks overlap:**
```
Chunk 1: tokens 1-500
Chunk 2: tokens 400-900   ← 100-token overlap with Chunk 1
Chunk 3: tokens 800-1300

Why: a key concept might span the boundary between chunks.
Without overlap → concept split → retrieval misses it.
With overlap → concept captured in at least one chunk.
JM Family: 10% overlap on dealer form chunks (50 tokens on 500-token chunks)
```

**Which strategy for which document type:**
```
Payer policy PDF (structured sections):
└── Semantic chunking → split at section headers → each chunk = one policy rule

Physician dictation (narrative):
└── Paragraph-level → each paragraph = one clinical observation

Lab result feed (structured, uniform):
└── Fixed-size → every 300 tokens → fast and consistent
```

### HNSW Indexing — what it is and why it matters

**Problem:** You have 1 million document chunks stored as vectors. A query vector arrives. Finding the closest matching vector by comparing against all 1 million is too slow for real-time use.

**HNSW = Hierarchical Navigable Small World**
```
A graph-based approximate nearest neighbor algorithm.
Instead of checking all 1M vectors:
└── Builds a multi-layer graph at index time
└── Each layer has fewer nodes but longer jumps
└── Search starts at top (coarse) layer → narrows down → fine layer
└── Finds the ~nearest neighbors in milliseconds not seconds

Speed vs exactness tradeoff:
└── HNSW finds approximate nearest neighbors (not guaranteed exact)
└── In practice: 95-99% of exact results in 10ms vs 1000ms for exact search
└── For RAG: acceptable — missing the exact best chunk by a tiny margin is fine
```

**HNSW in Azure AI Search:**
```
When you create a vector field in Azure AI Search:
└── Specify algorithm: hnsw
└── Specify parameters:
    ├── m: 4 (connections per node — higher = better recall, more memory)
    ├── efConstruction: 400 (build quality — higher = better index, slower build)
    └── efSearch: 500 (query quality — higher = better recall, slower query)
```

**⚙️ Config or Code? — HNSW + Vector Index**
- **Portal Config only:** Set HNSW parameters (m, efConstruction, efSearch) in Azure AI Search index JSON schema via portal; enable vector search profile; select embedding model for integrated vectorization
- **Custom Code:** Push vectors to index (SDK), query with vector + hybrid search (SDK), tune efSearch at query time in code
- **Both:** Index schema (define fields + HNSW config = portal JSON; push documents with vectors = SDK code)

**Cosine similarity:**
```
Two vectors are "close" if they point in the same direction
regardless of their magnitude (length).

Cos(θ) = (A · B) / (|A| × |B|)

Score: 1.0 = identical direction (same meaning)
       0.0 = perpendicular (unrelated)
      -1.0 = opposite direction (opposite meaning)

Why cosine not Euclidean distance?
└── Embedding magnitudes vary — longer text creates larger vectors
└── Cosine ignores magnitude, only compares direction
└── Captures semantic similarity independent of text length
```

---

## Section 11 — CV SKILL: Transformer Fundamentals

> **CV anchor:** "transformer internals (self-attention, multi-head attention, positional encoding), tokenization mechanics (BPE, WordPiece, token budgeting), embedding space geometry, LLM adaptation (RLHF, instruction tuning, LoRA/PEFT)"

### Self-Attention — simple explanation

```
Problem self-attention solves:
"The patient was given metformin. It lowered her blood sugar."
                                          ↑
Who does "her" refer to? → "the patient"

Before transformers: RNNs processed left-to-right and forgot context
Self-attention: every word looks at every other word simultaneously
               and learns which words are most relevant to each other
```

```
For each word, self-attention computes:
├── Query (Q): "what am I looking for?"
├── Key (K):   "what do I contain?"
└── Value (V): "what do I pass forward?"

Attention score = Q · K (dot product = how relevant is each word to me?)
Softmax → weights → weighted sum of Values → richer representation

"her" attends strongly to "patient" → learns the reference
```

**Multi-head attention:**
```
Run self-attention multiple times in parallel (8-16 heads)
Each head learns a different type of relationship:
├── Head 1: grammatical subject-verb relationships
├── Head 2: pronoun-noun references
├── Head 3: negation patterns
└── Head 4: medical term co-occurrences

Concatenate all heads → richer combined representation
```

**Positional encoding:**
```
Attention has no inherent word order (processes all words at once)
Positional encoding adds position information to each token embedding

"John prescribed Mary" vs "Mary prescribed John"
Same words, different meaning → positional encoding preserves order
```

### Tokenization — BPE and WordPiece

```
LLMs do not read words — they read TOKENS (sub-word units)

BPE (Byte-Pair Encoding) — used by GPT models:
├── Start with individual characters
├── Repeatedly merge the most frequent adjacent pair
└── "unbelievable" → ["un", "believ", "able"] → 3 tokens

WordPiece — used by BERT:
├── Similar to BPE but merges to maximize language model likelihood
└── "semaglutide" → ["s", "##ema", "##glut", "##ide"] → 4 tokens

Why it matters for token budgeting:
├── Medical terms often split into many tokens
├── "semaglutide" = 4 tokens, not 1 word
├── Your chunk of 300 "words" may actually be 400-500 tokens
└── Always calculate token count, not word count for context window planning

Token budgeting formula:
Context window (128K) - System prompt (~1K) - Tool list (~2K) - History (~5K)
= Available for RAG chunks + user query
÷ Chunk count (top-3) = Max tokens per chunk
```

### LoRA / PEFT — efficient fine-tuning

```
Problem: Fine-tuning GPT-4o means updating 200B parameters
→ Extremely expensive in compute and time

LoRA (Low-Rank Adaptation):
├── Freeze all original model weights
├── Add small trainable matrices (adapters) to attention layers
├── Only train the adapters (0.1% of parameters)
└── At inference: merge adapters back → same model, adapted behavior

PEFT (Parameter-Efficient Fine-Tuning):
└── Umbrella term for LoRA + similar techniques (Prefix Tuning, Adapter layers)

Result:
├── 100x cheaper than full fine-tuning
├── Same base model — just adapted for your domain
└── Multiple LoRA adapters can coexist on same base model
    └── Switch adapter per use case without reloading model
```

### RLHF — why models follow instructions

```
RLHF (Reinforcement Learning from Human Feedback):
How GPT-4o became helpful/harmless instead of just predicting tokens

Step 1 — Supervised Fine-Tuning (SFT):
└── Train on human-written examples of good responses

Step 2 — Reward Model:
└── Humans compare two model responses: "which is better?"
└── Train a reward model to predict human preference scores

Step 3 — RL Training:
└── Use reward model to score model outputs
└── Optimize model to produce higher-scoring outputs
└── Iteratively: model improves → better responses → higher reward

Why you care as an architect:
└── RLHF is why instruction-following works
└── When your system prompt says "never invent clinical findings" → model has been trained to follow
└── But RLHF is not perfect — system prompt instructions can be overridden by strong adversarial input (prompt injection)
    └── This is why you add Content Safety + groundedness detection as external layers
```

---

## Key Terms to Use in Interview

| Term | Use it when... |
|---|---|
| Retrieval-Augmented Generation (RAG) | Talking about grounding LLMs in enterprise knowledge |
| Cache-Augmented Generation (CAG) | Talking about pre-loading stable context |
| KV cache | Explaining why CAG is efficient at inference time |
| Hybrid search | Talking about Azure AI Search retrieval quality |
| BM25 + vector | Explaining hybrid search components |
| Groundedness evaluation | Explaining how you verify RAG answers are grounded |
| Minimum necessary access | HIPAA argument for RAG over CAG for PHI |
| Metadata filtering | Explaining how RAG retrieves the right subset |
| Top-K retrieval | Explaining the retrieval step |
| Reciprocal Rank Fusion | How hybrid search combines scores |

---

*L23 complete. Next: L24 — Hallucination (factual + agentic)*
