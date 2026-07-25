# Azure AI Document Intelligence vs Azure AI Search
## Different Services, Different Jobs, Work Together in a Pipeline

---

## Common Confusion — They Sound Related But Are Not

```
Both deal with documents.
Both are Azure AI services.
Both appeared in JMA production (cog-jma-dev-frm-recognizer + srch-jma-dev-indexer).

But they do completely different things.
```

---

## One-Line Each

```
Azure AI Document Intelligence  =  READER
                                    "What is written in this document?"

Azure AI Search                 =  FINDER
                                    "Which documents match this question?"
```

---

## What Each Service Does

```
AZURE AI DOCUMENT INTELLIGENCE (formerly Form Recognizer)
─────────────────────────────────────────────────────────
Input:   Raw document — PDF, image, scanned paper, photo
Output:  Structured data extracted from it

What it extracts:
  ├── Full text (OCR — reads printed + handwritten text)
  ├── Key-value pairs  → contractNumber: "C-84512"
  ├── Tables           → rows + columns recognised
  ├── Signatures       → detected and located
  └── Layout           → headings, sections, bounding boxes

It does NOT store anything.
It does NOT search anything.
It reads one document and returns what is written in it.
```

```
AZURE AI SEARCH
───────────────
Input:   Chunks of text + vectors (already extracted content)
Output:  Matching results when a user searches

What it does:
  ├── Stores chunks as searchable records
  ├── Stores vectors for semantic/hybrid search
  ├── Finds matching content on keyword or vector query
  └── Returns ranked results to your application

It does NOT read raw documents.
It does NOT perform OCR.
It needs content already extracted — then it indexes and finds it.
```

---

## How They Work Together in a Full Pipeline

```
Scanned contract PDF (raw, unreadable by computer)
       │
       ▼
AZURE AI DOCUMENT INTELLIGENCE
  ← OCR reads text from scanned pages
  ← Extracts fields: contractNumber, dealerName, amount, signedDate
  ← Extracts tables (line items, payment schedule)
  ← Returns structured JSON with all extracted content
       │
       ▼
Extracted text + structured fields
"Contract #C-84512, Dealer AutoNation Duluth, Amount $42,500..."
       │
       ▼
CHUNKING
  ← split extracted text into paragraph-sized pieces (~500 tokens)
       │
       ▼
EMBEDDING (Azure OpenAI text-embedding-3-large)
  ← convert each chunk to a vector (3072 numbers)
       │
       ▼
AZURE AI SEARCH INDEX
  ← store chunks + vectors
  ← contractNumber stored as filterable field
       │
       ▼
User: "Show me contracts over $40k from AutoNation last month"
       │
       ▼
AI SEARCH finds matching chunks → GPT-4o generates answer
```

---

## Three Services — Distinct Roles

```
┌──────────────────────────────────────────────────────────────┐
│  1. Azure AI Document Intelligence                           │
│     READ — extract text + fields from raw documents          │
│     Input:  PDF / image / scan                              │
│     Output: structured text + key-value pairs + tables       │
│     Billing: per page processed                             │
├──────────────────────────────────────────────────────────────┤
│  2. Azure OpenAI (text-embedding-3-large)                    │
│     CONVERT — turn extracted text into vectors               │
│     Input:  text chunks                                     │
│     Output: vectors (arrays of numbers)                     │
│     Billing: per token embedded                             │
├──────────────────────────────────────────────────────────────┤
│  3. Azure AI Search                                          │
│     STORE + SEARCH — index vectors, find matching content    │
│     Input:  chunks + vectors                                │
│     Output: search results ranked by relevance              │
│     Billing: per search unit (tier-based)                   │
└──────────────────────────────────────────────────────────────┘
Each is a separate Azure resource. Separate billing. Separate SDK.
```

---

## Document Intelligence — Model Types

```
PREBUILT MODELS (no training needed):
  ├── Read          → full OCR on any document
  ├── Layout        → OCR + tables + headings + structure
  ├── Invoice       → extracts vendor, amount, line items, dates
  ├── Receipt       → extracts merchant, total, items
  ├── ID Document   → extracts name, DOB, ID number from IDs
  └── Contract      → extracts parties, dates, terms (new)

CUSTOM MODELS (you train on your documents):
  ├── Custom Template → fixed-layout forms (same structure every time)
  ├── Custom Neural   → varied layouts (same content, different formats)
  └── Composed        → multiple custom models combined into one

JMA USE CASE:
  cog-jma-dev-frm-recognizer likely uses a prebuilt or custom model
  to extract fields from scanned dealer forms/contracts
```

---

## Confidence Routing Pattern (Production Best Practice)

```
Document Intelligence returns a confidence score per field:

  confidence > 0.90  → auto-process, no human review needed ✅
  confidence 0.70-0.90 → send to human review queue ⚠️
  confidence < 0.70  → dead letter queue, manual processing ❌

This is the pattern used in Module 8 (Document Intelligence deep dive)
and how JMA's cog-jma-dev-frm-recognizer likely operates.
```

---

## JMA Production — What We Found

```
cog-jma-dev-frm-recognizer
  ← Azure AI Document Intelligence resource (dev)
  ← Service: Form Recognizer / Document Intelligence
  ← Manually deployed 2023-08-18, no CI/CD
  ← Owner: Matt Waterman
  ← Reads/extracts from scanned forms and documents

srch-jma-dev-indexer (documents-dev index)
  ← Azure AI Search resource (dev)
  ← Stores extracted fields: contractNumber, fileName, dates
  ← NO vectors, NO embeddings — pure keyword + filter lookup
  ← EnterpriseSearch.Sync WebJob pushes data here via Graph API

CURRENT FLOW (no AI/RAG):
  SharePoint documents → WebJob → AI Search index
  ← contractNumber used as filter to find the file
  ← fileName is the only keyword-searchable field

FUTURE OPPORTUNITY (with RAG):
  Scanned contract → Document Intelligence (extract full text)
  → chunk → embed → AI Search (vector index)
  → user asks in natural language → RAG answer with citations
```

---

## When to Use Document Intelligence

```
USE Document Intelligence when:
  ├── Documents are scanned images (PDFs from scanner, photos)
  ├── Documents have structured fields you need to extract
  ├── You need to read tables from PDFs programmatically
  ├── Source is handwritten forms or mixed-format documents
  └── You need key-value pairs extracted, not just raw text

DO NOT USE when:
  ├── Document is already machine-readable text (Word, TXT, JSON)
  ├── You just need to search through text — use AI Search directly
  └── Document is structured data — use a database
```

---

## In Azure AI Foundry

```
AI Foundry → Content Understanding
  ← this is Document Intelligence exposed through AI Foundry portal
  ← same service, same models, different UI entry point
  ← lets you test extraction on sample documents without code
  ← connects output to Knowledge / Data section for RAG ingestion
```

---

## Navigation

| | |
|---|---|
| **Previous** | [10 — Semantic Kernel](10-Semantic-Kernel.md) |
| **Next** | `12-Evaluation-and-LLMOps.md` *(coming soon)* |
