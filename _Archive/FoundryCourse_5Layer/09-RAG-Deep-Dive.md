# RAG — Retrieval Augmented Generation
## Deep Dive

---

## The Problem RAG Solves

```
GPT-4o knows:                    GPT-4o does NOT know:
─────────────                    ────────────────────
World history                    Your Toyota inventory
General knowledge                JMA dealer policies
Common facts                     Sarah's purchase history
Public information               Your internal price lists
                                 Documents after Aug 2025
```

> RAG bridges this gap — **without retraining the model**.

---

## What RAG Stands For

```
R — Retrieval    ← FIND the right information
A — Augmented    ← ADD it to the prompt
G — Generation   ← LLM GENERATES answer using it
```

---

## RAG vs The Alternatives

```
Problem: GPT-4o doesn't know JMA inventory

Option 1 — Put everything in the prompt:
  "Here is our full inventory: [10,000 cars]..."
  ❌ Too long — exceeds context window
  ❌ Expensive — pay for all tokens every call
  ❌ Slow — huge prompt takes time

Option 2 — Fine-tune:
  Train GPT-4o on inventory data
  ❌ Inventory changes daily — retrain daily?
  ❌ Expensive to retrain
  ❌ Model memorizes, doesn't retrieve

Option 3 — RAG: ✅
  Store inventory in vector index
  Retrieve ONLY relevant chunks per question
  Inject just those chunks into prompt
  ✅ Fast, cheap, always fresh
```

---

## How RAG Works — Step by Step

### Phase 1: Indexing (Done Once / When Data Changes)

```
Your Documents (Toyota inventory, specs, manuals)
        │
        ▼
┌─── CHUNKING ───────────────────────────────┐
│  Split large docs into small pieces        │
│                                            │
│  RAV4-inventory.pdf (50 pages)             │
│   → Chunk 1: "RAV4 XLE, $42,500, Black"   │
│   → Chunk 2: "RAV4 XLE, $42,500, White"   │
│   → Chunk 3: "RAV4 Premium, $44,800..."   │
│   → ... (hundreds of chunks)              │
└────────────────────────────────────────────┘
        │
        ▼
┌─── EMBEDDING ──────────────────────────────┐
│  Convert each chunk to a vector            │
│  using text-embedding-3-large              │
│                                            │
│  "RAV4 XLE, $42,500, Black"               │
│   → [-0.023, 0.061, 0.048, ...]           │
│      (3072 numbers capturing meaning)      │
└────────────────────────────────────────────┘
        │
        ▼
┌─── STORING ────────────────────────────────┐
│  Save vectors in Azure AI Search index     │
│                                            │
│  Index: toyota-inventory-index             │
│  ┌──────────────────────────────────┐      │
│  │ Chunk 1 │ vector: [-0.023...]   │      │
│  │ Chunk 2 │ vector: [0.015...]    │      │
│  │ Chunk 3 │ vector: [-0.041...]   │      │
│  └──────────────────────────────────┘      │
└────────────────────────────────────────────┘
```

### Phase 2: Retrieval (Every Query)

```
Customer: "I need a black SUV hybrid under $45k"
        │
        ▼
┌─── EMBED QUERY ────────────────────────────┐
│  Convert question to vector                │
│  "black SUV hybrid under $45k"             │
│   → [-0.019, 0.058, 0.044, ...]           │
└────────────────────────────────────────────┘
        │
        ▼
┌─── VECTOR SEARCH ──────────────────────────┐
│  Find chunks with similar vectors          │
│  (cosine similarity)                       │
│                                            │
│  Query vector vs all chunk vectors:        │
│  Chunk 1: similarity = 0.94 ✅ TOP MATCH   │
│  Chunk 2: similarity = 0.91 ✅ GOOD        │
│  Chunk 3: similarity = 0.45 ❌ skip        │
│  Chunk 4: similarity = 0.88 ✅ GOOD        │
│                                            │
│  Return top 3 chunks                       │
└────────────────────────────────────────────┘
        │
        ▼
┌─── INJECT INTO PROMPT ─────────────────────┐
│  Build prompt with retrieved context       │
│                                            │
│  System: "You are a JMA vehicle advisor.  │
│           Answer using ONLY the context   │
│           below."                          │
│                                            │
│  Context:                                  │
│  [Chunk 1] RAV4 Hybrid XLE $42,500 Black  │
│  [Chunk 2] RAV4 Hybrid XLE $42,500 White  │
│  [Chunk 4] RAV4 Hybrid Premium $44,800    │
│                                            │
│  Question: "Black SUV hybrid under $45k?" │
└────────────────────────────────────────────┘
        │
        ▼
┌─── GENERATION ─────────────────────────────┐
│  GPT-4o reads context + question           │
│  Generates grounded answer                 │
│                                            │
│  "We have the RAV4 Hybrid XLE in          │
│   Midnight Black for $42,500 —            │
│   well within your $45k budget,           │
│   getting 41 MPG city."                   │
└────────────────────────────────────────────┘
```

---

## The Full RAG Picture

```
INDEXING PHASE (once):
Documents → Chunk → Embed → Store in AI Search

QUERY PHASE (every request):
Question → Embed → Search → Retrieve → Inject → GPT-4o → Answer
```

---

## Chunking Strategies — Important Detail

```
How you chunk affects quality significantly:

Strategy 1 — Fixed Size:
  Split every 500 tokens regardless
  ✅ Simple    ❌ Can cut mid-sentence

Strategy 2 — Sentence/Paragraph:
  Split at natural boundaries
  ✅ Preserves meaning    ✅ Better retrieval

Strategy 3 — Semantic:
  Split when topic changes
  ✅ Best quality    ❌ More complex

Strategy 4 — Overlapping:
  Each chunk shares 10% with next chunk
  ✅ No information lost at boundaries

JMA Best Practice:
  Paragraph chunking + 10% overlap
  Chunk size: ~500 tokens
```

---

## Retrieval Types in Azure AI Search

```
┌─────────────────────────────────────────────────────┐
│  1. Vector Search (semantic meaning)                │
│     "affordable family SUV"                         │
│      → finds "budget-friendly RAV4 for families"   │
│      Matches MEANING even if words differ           │
├─────────────────────────────────────────────────────┤
│  2. Keyword Search (exact words)                    │
│     "RAV4 XLE"                                      │
│      → finds exact "RAV4 XLE" matches               │
│      Fast, precise, literal                         │
├─────────────────────────────────────────────────────┤
│  3. Hybrid Search ✅ Best Practice                   │
│     Combines Vector + Keyword                       │
│     "RAV4 hybrid under 45000"                       │
│      → catches both semantic + exact matches        │
│      Best of both worlds                            │
└─────────────────────────────────────────────────────┘
```

---

## Grounding — Why It Matters

```
WITHOUT grounding (no RAG):
  Question: "What RAV4s do you have?"
  GPT-4o:   "Toyota RAV4 comes in LE, XLE, TRD..."
             ← HALLUCINATION — making up inventory!

WITH grounding (RAG):
  Question: "What RAV4s do you have?"
  GPT-4o:   "Based on our current inventory, we have
              RAV4 Hybrid XLE in Black ($42,500) and
              RAV4 Hybrid XLE Premium in Blueprint ($44,800)"
             ← GROUNDED — from your actual data ✅
```

---

## RAG in AI Foundry — Where It Lives

```
AI Foundry Portal
 └── My assets
      └── Data + Indexes          ← upload docs, create index
           ├── Upload files       ← PDFs, Word, Excel, JSON
           ├── Connect storage    ← Azure Blob Storage
           └── Create index       ← AI Search index

 └── Build and customize
      └── Agents
           └── Knowledge          ← attach index to agent
```

---

## JMA RAG Setup

```
Documents to index:
 ├── toyota-inventory-2024.pdf      → daily update
 ├── rav4-specs-all-trims.pdf       → yearly update
 ├── warranty-guide-2024.pdf        → when policy changes
 ├── trim-comparison-matrix.xlsx    → seasonal update
 └── jma-dealer-policies.pdf        → when rules change

Index: toyota-knowledge-index
Chunking: paragraph + 10% overlap
Embedding: text-embedding-3-large
Search: Hybrid (vector + keyword)
Top K: 5 chunks per query
```

---

## RAG Quality Metrics (Evaluation Layer)

```
Groundedness   ← Is answer based on retrieved context?
                Score: 0.96 ✅ (almost entirely from docs)

Relevance      ← Did it answer the actual question?
                Score: 0.92 ✅

Context recall  ← Did retrieval find the right chunks?
                Score: 0.88 ✅

If scores drop:
 ├── Groundedness drops → model hallucinating → fix prompt
 ├── Relevance drops    → wrong chunks returned → fix chunking
 └── Context drops      → retrieval failing → fix index
```

---

## Knowledge Check

**Q: JMA uploads a new inventory PDF every morning at 6AM with updated
car prices and stock. What needs to happen in the RAG pipeline for
the 8AM queries to use fresh data?**

<details>
<summary>Answer</summary>

Between 6AM and 8AM the pipeline must:

```
6:00 AM — New inventory PDF uploaded to Azure Blob Storage
        │
        ▼
6:01 AM — Pipeline triggered (Azure DevOps / Logic App)
        │
        ▼
6:05 AM — Old index cleared / new chunking starts
          PDF split into chunks (paragraph + overlap)
        │
        ▼
6:20 AM — Embedding runs on all chunks
          text-embedding-3-large converts each chunk to vector
        │
        ▼
6:45 AM — New vectors loaded into Azure AI Search index
          toyota-inventory-index refreshed
        │
        ▼
7:00 AM — Index ready ✅
        │
        ▼
8:00 AM — Queries now return fresh inventory data ✅
```

Key point: **The indexing pipeline must complete before queries run.**
If indexing takes longer than 2 hours, 8AM queries would still
see yesterday's data — so pipeline speed matters.

</details>

---

## One-Line Summary

> RAG = **find the right pieces of your data, inject them into the prompt,
> let the model answer from them** — fresh answers from your documents
> without retraining the model.

---

## Navigation

| | |
|---|---|
| **Previous** | [08 — Multi-Agent](08-AI-Agents-MultiAgent.md) |
| **Next** | `10-Evaluation-Deep-Dive.md` *(coming soon)* |
