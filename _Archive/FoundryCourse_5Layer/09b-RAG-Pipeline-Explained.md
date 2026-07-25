# RAG Pipeline — What It Is

> Companion to: [09 — RAG Deep Dive](09-RAG-Deep-Dive.md)

---

## Simple Definition

```
Pipeline = a series of automated steps that run in sequence
           triggered by an event, completing a job end-to-end

RAG Pipeline = the automated process that keeps your
               RAG index fresh and ready to use
```

> Think of it like an **assembly line** — raw material goes in one end,
> finished product comes out the other — automatically.

---

## Two RAG Pipelines

```
RAG has TWO separate pipelines:

┌─────────────────────────────────────┐
│  1. INDEXING PIPELINE               │
│     Runs when data changes          │
│     Purpose: keep index fresh       │
│     Triggered: scheduled or event   │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  2. QUERY PIPELINE                  │
│     Runs on every user question     │
│     Purpose: find + answer          │
│     Triggered: every user request   │
└─────────────────────────────────────┘
```

---

## Pipeline 1: Indexing Pipeline

```
TRIGGER: 6AM daily (new inventory PDF arrives)
         OR file uploaded to Blob Storage
         OR manual trigger
        │
        ▼
STEP 1: INGEST
        Read PDF from Azure Blob Storage
        "toyota-inventory-2024.pdf"
        │
        ▼
STEP 2: CHUNK
        Split into ~500 token paragraphs
        with 10% overlap
        50 page PDF → 300 chunks
        │
        ▼
STEP 3: EMBED
        Call text-embedding-3-large API
        Each chunk → 3072 number vector
        300 chunks × 3072 = lots of vectors
        │
        ▼
STEP 4: INDEX
        Push all vectors into
        Azure AI Search index
        toyota-inventory-index updated
        │
        ▼
STEP 5: VALIDATE
        Run test queries
        Confirm fresh data returned
        │
        ▼
DONE: Index ready for queries ✅
```

---

## Pipeline 2: Query Pipeline

```
TRIGGER: Customer asks a question
        │
        ▼
STEP 1: RECEIVE
        "What black RAV4 hybrids under $45k?"
        │
        ▼
STEP 2: EMBED QUERY
        Convert question → vector
        using text-embedding-3-large
        │
        ▼
STEP 3: SEARCH
        Vector search in AI Search index
        Returns top 5 matching chunks
        │
        ▼
STEP 4: BUILD PROMPT
        System prompt +
        Retrieved chunks (context) +
        User question
        │
        ▼
STEP 5: GENERATE
        GPT-4o reads prompt
        Generates grounded answer
        │
        ▼
STEP 6: RETURN
        Answer + sources back to user
        Tracing captures all steps
```

---

## JMA — Both Pipelines Together

```
INDEXING PIPELINE (background, automated):

6:00 AM  New PDF in Blob Storage
    │
    │  Azure DevOps pipeline triggers
    ▼
6:05 AM  Chunking runs
6:20 AM  Embedding runs
6:45 AM  AI Search index updated
7:00 AM  Validation passes ✅
         Index is FRESH and READY


QUERY PIPELINE (foreground, real-time):

8:00 AM  Dealer asks: "Any black RAV4 hybrids?"
    │
    │  ~1.5 seconds later
    ▼
8:00 AM  Answer: "Yes, RAV4 Hybrid XLE Midnight
         Black $42,500 — updated this morning"
```

---

## What Runs the Pipelines at JMA

```
Indexing Pipeline runs on:
 ├── Azure DevOps Pipeline   ← scheduled 6AM trigger
 ├── Azure Logic App         ← event-based (file uploaded)
 └── Azure Functions         ← lightweight trigger

Query Pipeline runs on:
 ├── AI Foundry Prompt Flow  ← orchestrates the steps
 └── Semantic Kernel (C#)    ← if built in your app
```

---

## .NET / ETL Analogy

```
RAG Pipeline = ETL Pipeline you already know

ETL:                          RAG Indexing Pipeline:
────                          ──────────────────────
Extract from source     ───►  Ingest PDF from Blob
Transform data          ───►  Chunk + Embed
Load into database      ───►  Index into AI Search

Same concept — different destination!
```

---

## Knowledge Check Answer

```
Q: JMA uploads a new inventory PDF every morning at 6AM.
   What needs to happen for 8AM queries to use fresh data?

A: The INDEXING PIPELINE must complete between 6AM and 8AM:

   6:00 AM → PDF uploaded to Blob Storage
   6:05 AM → Chunking triggered automatically
   6:20 AM → Embedding runs on all chunks
   6:45 AM → AI Search index refreshed
   7:00 AM → Validation confirms fresh data
   8:00 AM → Dealer queries return today's inventory ✅

   Key risk: if indexing takes longer than 2 hours,
   8AM queries still see yesterday's data.
   Solution: monitor pipeline duration in Azure DevOps.
```

---

## One-Line Summary

> A RAG pipeline is the **automated assembly line** that takes your raw documents,
> converts them to searchable vectors (indexing pipeline), and then finds + uses
> the right ones to answer questions (query pipeline).

---

## Navigation

| | |
|---|---|
| **Previous** | [09 — RAG Deep Dive](09-RAG-Deep-Dive.md) |
| **Next** | `10-Evaluation-Deep-Dive.md` *(coming soon)* |
