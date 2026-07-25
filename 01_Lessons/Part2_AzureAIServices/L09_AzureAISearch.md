# Module 9 — Azure AI Search
**Part 2: AI Engineering (AI-102 Level) | AI Solutions Architect Curriculum**

---

## What You Already Know (Recap)

From your JM Family work and prior modules:
- **Document Intelligence (Module 8)** — extracts structured fields from documents
- **RAG concepts (Module 11/12 sessions)** — retrieval → context → LLM → answer
- **Embeddings** — vectors that represent meaning, used for semantic/vector search
- **Azure OpenAI** — the LLM sitting at the end of the RAG pipeline
- **Cosmos DB / Blob Storage** — storage layers you already use at JM Family

This module explains the **middle layer** of your pipeline — the component that takes extracted document data, indexes it, enriches it with AI, and makes it searchable for RAG.

---

**Running example (used throughout):**
> *JM Family's RAG pipeline: Document Intelligence extracts invoice fields → Azure AI Search indexes and vectorizes → Azure OpenAI queries Search for relevant context → LLM generates answer.*

Every concept maps to a real decision in this pipeline.

---

## Topic 9.1 — Azure AI Search Fundamentals

---

### 1. What Is Azure AI Search?

Azure AI Search (formerly Azure Cognitive Search) is a **cloud search service** that lets you index, enrich, and query data at scale. It is the primary retrieval layer in Azure-based RAG architectures.

It does three things:
1. **Ingestion** — pulls data from sources (Blob, Cosmos DB, SQL, SharePoint, custom) via indexers
2. **Enrichment** — runs AI skillsets during ingestion (OCR, entity extraction, embedding generation)
3. **Querying** — supports keyword search, vector search, semantic search, and hybrid combinations

**Not just a search engine:**
- Full-text search: BM25 ranking (keyword relevance)
- Vector search: cosine similarity on embeddings (semantic meaning)
- Semantic ranking: re-ranks results using a language model for relevance
- Hybrid: combine all three in one query — best of all worlds

---

### 2. Core Components

```
┌─────────────────────────────────────────────────────────┐
│                  Azure AI Search Service                 │
│                                                         │
│  ┌──────────┐   ┌──────────┐   ┌──────────────────────┐ │
│  │  Index   │   │ Indexer  │   │   Skillset (AI)      │ │
│  │(the DB)  │   │(the pump)│   │(enrichment pipeline) │ │
│  └──────────┘   └──────────┘   └──────────────────────┘ │
│                      ↑                                  │
│               ┌──────────────┐                          │
│               │  Data Source │                          │
│               │(Blob, SQL..) │                          │
│               └──────────────┘                          │
└─────────────────────────────────────────────────────────┘
```

| Component | What it is | Analogy |
|-----------|-----------|---------|
| **Index** | The schema + stored data (like a database table) | SQL table with columns |
| **Indexer** | Pulls from data source, runs skillset, writes to index | ETL pipeline / ADF pipeline |
| **Skillset** | AI enrichment steps applied during indexing | ADF transformation activities |
| **Data Source** | Connection to where your raw data lives | ADF linked service |
| **Synonym Map** | Maps related terms for better keyword search | Optional add-on |

---

### 3. Index Schema — The Fields

An index is defined by its fields. Each field has:

```json
{
  "name": "content",
  "type": "Edm.String",
  "searchable": true,
  "filterable": false,
  "retrievable": true,
  "analyzer": "en.microsoft"
}
```

**Field attributes that matter:**

| Attribute | What it means |
|-----------|--------------|
| `searchable` | Text is tokenized and included in full-text search |
| `filterable` | Can use in `$filter=fieldName eq 'value'` |
| `sortable` | Can sort results by this field |
| `facetable` | Can aggregate/group by this field (e.g., count by category) |
| `retrievable` | Included in query results returned to caller |
| `key` | Unique identifier for each document (required, exactly one) |

**Vector field (for RAG):**
```json
{
  "name": "contentVector",
  "type": "Collection(Edm.Single)",
  "dimensions": 1536,
  "vectorSearchProfile": "my-hnsw-profile"
}
```
`dimensions` must match your embedding model output (text-embedding-3-small = 1536, text-embedding-3-large = 3072).

---

### 4. Service Tiers

| Tier | Use case | Notes |
|------|---------|-------|
| **Free** | Dev/test | 50MB storage, 3 indexes, no SLA |
| **Basic** | Small production | 2GB storage, 5 indexes |
| **Standard S1/S2/S3** | Production | Scalable replicas + partitions |
| **Storage Optimized** | Large datasets | High storage, lower query throughput |

**Replicas vs Partitions:**
- **Replicas** — copies of the index for query throughput and HA (need ≥2 for SLA, ≥3 for write HA)
- **Partitions** — shards the index for storage + indexing throughput

For JM Family RAG: **Basic** for dev, **S1** for production.

---

## Topic 9.2 — Data Ingestion

---

### 1. Three Ways to Get Data In

| Method | How | When to use |
|--------|-----|-------------|
| **Push API** | Your code calls the index REST API / SDK to upload documents | Real-time ingestion, custom pipelines (your ADF/Function pattern) |
| **Pull (Indexer)** | Azure AI Search pulls from a configured data source on a schedule | Blob Storage, SQL, Cosmos DB, SharePoint |
| **Import Data Wizard** | Portal UI wizard | Quick setup/testing only |

**For JM Family:** You'll use the **Push API** from your Azure Function after Document Intelligence extracts the fields — your Function already has the extracted JSON, so push it directly into the index.

---

### 2. Push API — C# Example

```csharp
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure.Identity;

var endpoint = new Uri("https://my-search.search.windows.net");
var credential = new DefaultAzureCredential(); // Managed Identity
var indexClient = new SearchClient(endpoint, "invoices-index", credential);

// Document to index
var doc = new
{
    id = "invoice-001",
    dealerCode = "JMF-ATL-001",
    invoiceDate = "2026-01-15",
    totalAmount = 45230.00,
    vehicleVin = "1HGBH41JXMN109186",
    content = "Full extracted text for full-text search...",
    contentVector = embeddingArray  // float[] from Azure OpenAI Embeddings API
};

var batch = IndexDocumentsBatch.Upload(new[] { doc });
await indexClient.IndexDocumentsAsync(batch);
```

**Batch uploads** — always prefer batch over single document. Max 1000 documents or 16MB per batch.

---

### 3. Pull (Indexer) — Blob Storage Example

When your documents land in Blob Storage and you want AI Search to pull them automatically:

```
Blob Storage (PDFs land here)
        ↓ Indexer polls every 5 min
Azure AI Search Indexer
        ↓ runs Skillset (OCR, entity extraction, embedding)
Index (fields populated, vectors stored)
```

**Data source definition:**
```json
{
  "name": "invoices-blob-datasource",
  "type": "azureblob",
  "credentials": { "connectionString": "ResourceId=..." },
  "container": { "name": "invoices", "query": "processed/" }
}
```

Use **Managed Identity** for the connection string (`ResourceId=/subscriptions/.../storageAccounts/...`) — no keys in config.

**Indexer schedule:**
```json
{
  "schedule": { "interval": "PT5M" }
}
```
`PT5M` = ISO 8601 for 5 minutes. Minimum interval is 5 minutes for pull indexers.

---

### 4. Change Detection

Indexers only re-process documents that changed. For Blob Storage:
- Uses `LastModified` metadata automatically
- For SQL: use a `rowVersion` or `lastModified` column
- For Cosmos DB: uses the change feed

**High water mark** — the indexer remembers the last processed timestamp so it doesn't re-index everything on every run.

---

## Topic 9.3 — AI Enrichment (Skillsets)

---

### 1. What Is a Skillset?

A skillset is an **AI enrichment pipeline** that runs during indexing. It takes raw document content and adds AI-derived fields before storing in the index.

```
Raw document (PDF text)
        ↓ Skill 1: Split into chunks
        ↓ Skill 2: Detect language
        ↓ Skill 3: Extract entities (people, orgs, locations)
        ↓ Skill 4: Generate embedding vector
Enriched document stored in index
```

**Without skillset:** Your index contains what's in the source document.
**With skillset:** Your index contains the source data + AI-extracted fields.

---

### 2. Built-in Skills (No Training Required)

| Skill | What it does | JM Family use |
|-------|-------------|--------------|
| **OCR** | Extracts text from images/scanned PDFs | Pre-process scanned invoices before DI |
| **Text Merge** | Combines OCR text with original content | Merge scanned text into main content field |
| **Split** | Chunks long text into smaller pieces | Split long contracts into 512-token chunks for RAG |
| **Language Detection** | Detects language of text | Filter or route multilingual documents |
| **Entity Recognition** | Extracts people, organizations, locations, dates | Extract named entities from document content |
| **Key Phrase Extraction** | Extracts important phrases | Auto-tagging, summarization hints |
| **Sentiment Analysis** | Positive/negative/neutral | Customer feedback analysis |
| **Image Analysis** | Describes image content | Process document with embedded images |
| **PII Detection** | Finds SSN, credit card, email, phone | Redact before storing |
| **Custom Web API** | Call your own API endpoint as a skill | Any custom logic — Azure Function |
| **Azure OpenAI Embedding** | Generate vector embeddings for chunks | **Critical for RAG** |

---

### 3. The Azure OpenAI Embedding Skill — Most Important for RAG

This skill calls Azure OpenAI Embeddings API during indexing to vectorize your content chunks:

```json
{
  "@odata.type": "#Microsoft.Skills.Text.AzureOpenAIEmbeddingSkill",
  "name": "generate-embeddings",
  "context": "/document/pages/*",
  "resourceUri": "https://my-openai.openai.azure.com",
  "deploymentId": "text-embedding-3-small",
  "inputs": [
    { "name": "text", "source": "/document/pages/*/content" }
  ],
  "outputs": [
    { "name": "embedding", "targetName": "contentVector" }
  ]
}
```

**The flow:**
1. Indexer reads document from Blob
2. Split skill breaks it into 512-token chunks → `/document/pages/*`
3. Embedding skill calls Azure OpenAI for each chunk → generates `float[]`
4. Vector stored in `contentVector` field in index

This is the ingestion half of RAG. The query half uses the same embedding model to vectorize the user's question.

---

### 4. Knowledge Store

Optionally, a skillset can write enriched output to a **Knowledge Store** (Blob or Table Storage) for:
- Debugging enrichment pipeline
- Storing enriched projections for downstream use
- Training custom models on enriched data

Not required for RAG — skip this for now, come back in Module 13.

---

### 5. Integrated Vectorization (Newer Pattern)

Azure AI Search now supports **integrated vectorization** — the index itself is configured to auto-call the embedding model at query time, so you don't need to generate the query embedding yourself:

```
User query (text)
    ↓ AI Search auto-calls embedding model
    ↓ vector search runs internally
Results returned
```

This simplifies RAG code but requires configuring a vectorizer on the index. We'll cover this in Module 13 RAG Deep Dive.

---

## Topic 9.4 — Querying and Search Experience

---

### 1. Three Query Types

| Type | How it works | Best for |
|------|-------------|---------|
| **Full-text (BM25)** | Tokenizes query, matches terms, ranks by TF-IDF | Exact keywords, known terms |
| **Vector** | Embeds query, finds nearest vectors by cosine similarity | Meaning/semantic match |
| **Semantic (re-ranking)** | Re-ranks top BM25 results using a language model | Improving relevance of keyword results |
| **Hybrid** | BM25 + vector combined via RRF fusion | **Best overall — recommended for RAG** |

---

### 2. Full-Text Query (BM25)

```csharp
var searchClient = new SearchClient(endpoint, "invoices-index", credential);

SearchResults<SearchDocument> results = await searchClient.SearchAsync<SearchDocument>(
    searchText: "Ford F-150 invoice January 2026",
    new SearchOptions
    {
        Filter = "dealerCode eq 'JMF-ATL-001'",
        OrderBy = { "invoiceDate desc" },
        Size = 10,
        Select = { "id", "dealerCode", "invoiceDate", "totalAmount" }
    }
);

await foreach (SearchResult<SearchDocument> result in results.GetResultsAsync())
{
    Console.WriteLine($"Score: {result.Score} | {result.Document["dealerCode"]}");
}
```

**OData filter syntax:**
```
dealerCode eq 'JMF-ATL-001'
totalAmount gt 10000 and invoiceDate ge 2026-01-01T00:00:00Z
search.in(dealerCode, 'JMF-ATL-001,JMF-DAL-002', ',')
```

---

### 3. Vector Query

```csharp
// 1. Embed the user's query
float[] queryVector = await GetEmbeddingAsync("Ford invoice Atlanta dealer January");

// 2. Vector search
var results = await searchClient.SearchAsync<SearchDocument>(
    searchText: null,  // no keyword text
    new SearchOptions
    {
        VectorSearch = new VectorSearchOptions
        {
            Queries =
            {
                new VectorizedQuery(queryVector)
                {
                    KNearestNeighborsCount = 5,
                    Fields = { "contentVector" }
                }
            }
        }
    }
);
```

**KNN vs ANN:**
- **KNN (exact)** — checks every vector, accurate, slow at scale
- **ANN (approximate)** — HNSW algorithm, fast, ~99% accurate — **use this in production**

---

### 4. Hybrid Query (Recommended for RAG)

Combines keyword + vector in one query. Results merged using **RRF (Reciprocal Rank Fusion)**:

```csharp
float[] queryVector = await GetEmbeddingAsync(userQuery);

var results = await searchClient.SearchAsync<SearchDocument>(
    searchText: userQuery,  // keyword search
    new SearchOptions
    {
        VectorSearch = new VectorSearchOptions
        {
            Queries =
            {
                new VectorizedQuery(queryVector)
                {
                    KNearestNeighborsCount = 50,  // candidates for RRF
                    Fields = { "contentVector" }
                }
            }
        },
        SemanticSearch = new SemanticSearchOptions
        {
            SemanticConfigurationName = "my-semantic-config",
            QueryCaption = new QueryCaption(QueryCaptionType.Extractive),
            QueryAnswer = new QueryAnswer(QueryAnswerType.Extractive)
        },
        QueryType = SearchQueryType.Semantic,
        Size = 5
    }
);
```

**RRF fusion:** Takes top N from BM25 and top N from vector, merges using rank positions (not scores). Neither side dominates — best results bubble up.

---

### 5. Captions and Answers

Semantic search can return:
- **Captions** — highlighted relevant snippets from matched documents
- **Answers** — direct answer extracted from top result (if question-like query)

```csharp
foreach (var result in results.GetResults())
{
    // Semantic caption
    if (result.SemanticSearch?.Captions != null)
    {
        var caption = result.SemanticSearch.Captions.FirstOrDefault();
        Console.WriteLine($"Caption: {caption?.Text}");
    }
}
```

Captions are useful as the context you pass to Azure OpenAI in RAG — instead of sending the full document chunk, send the extracted relevant excerpt.

---

### 6. Facets and Aggregations

For search UIs — count results by category:

```csharp
new SearchOptions
{
    Facets = { "dealerCode,count:10", "vehicleMake,count:5" }
}
```

Returns: `{ "dealerCode": [{"value": "JMF-ATL-001", "count": 142}, ...] }`

Use for building filter panels in search UIs.

---

## Topic 9.5 — Vector Search & Semantic Search

---

### 1. Vector Search Configuration

When creating the index, configure the vector search algorithm:

```json
{
  "vectorSearch": {
    "algorithms": [
      {
        "name": "my-hnsw",
        "kind": "hnsw",
        "hnswParameters": {
          "m": 4,
          "efConstruction": 400,
          "efSearch": 500,
          "metric": "cosine"
        }
      }
    ],
    "profiles": [
      {
        "name": "my-hnsw-profile",
        "algorithmConfigurationName": "my-hnsw"
      }
    ]
  }
}
```

**HNSW parameters:**
| Param | What it controls | Higher value = |
|-------|-----------------|----------------|
| `m` | Connections per node in graph | More accurate, more memory |
| `efConstruction` | Build-time accuracy | More accurate index, slower to build |
| `efSearch` | Query-time candidates examined | More accurate queries, slower |
| `metric` | Distance function | Use `cosine` for text embeddings |

**Rule of thumb for production:** `m=4`, `efConstruction=400`, `efSearch=500`, `metric=cosine`.

---

### 2. Semantic Ranker

Semantic ranking is a **re-ranking layer** on top of BM25 results using a Microsoft-hosted language model:

```
BM25 returns top 50 results by keyword score
        ↓
Semantic Ranker reads each result + original query
        ↓
Re-scores for semantic relevance
        ↓
Returns top 5 with semantic score + captions
```

**Key points:**
- Only available on **Standard tier and above** (not Free or Basic)
- Processes top 50 BM25 candidates (you don't control which 50)
- Adds latency (~200-500ms) — worth it for RAG quality
- Billed per 1000 semantic queries (check pricing)

**Semantic configuration:**
```json
{
  "semantic": {
    "configurations": [
      {
        "name": "my-semantic-config",
        "prioritizedFields": {
          "titleField": { "fieldName": "invoiceTitle" },
          "contentFields": [{ "fieldName": "content" }],
          "keywordsFields": [{ "fieldName": "dealerCode" }]
        }
      }
    ]
  }
}
```

`contentFields` — the fields semantic ranker reads to understand document meaning. Put your main text content here.

---

### 3. Embedding Models — Which to Use

| Model | Dimensions | Best for | Cost |
|-------|-----------|---------|------|
| `text-embedding-3-small` | 1536 | RAG, most use cases | Low |
| `text-embedding-3-large` | 3072 | Higher accuracy needs | Higher |
| `text-embedding-ada-002` | 1536 | Legacy (older deployments) | Low |

**JM Family recommendation:** `text-embedding-3-small` — good accuracy/cost balance. Use 3-large only if RAG accuracy testing shows it meaningfully better.

**Dimension truncation:** `text-embedding-3-small` and `3-large` support dimension reduction (Matryoshka embeddings). You can store 256 or 512 dimensions instead of 1536 to save space/cost — small accuracy tradeoff.

---

### 4. Full RAG Query Flow with Azure AI Search

This is the complete picture of how your JM Family RAG pipeline works:

```
User: "Show me all Ford invoices from Atlanta dealer over $40,000 in January 2026"
        ↓
1. RAG Orchestrator (Azure Function / Semantic Kernel)
        ↓
2. Embed the question → float[1536] via Azure OpenAI Embeddings
        ↓
3. Hybrid Query to Azure AI Search:
   - Keyword: "Ford invoices Atlanta dealer January 2026"
   - Vector: float[1536] against contentVector field
   - Filter: invoiceDate ge 2026-01-01T00:00:00Z and totalAmount gt 40000
   - Semantic re-rank: top 5 results
        ↓
4. Azure AI Search returns: top 5 chunks with captions
        ↓
5. Build prompt:
   System: "You are a JM Family invoice assistant. Answer from the context only."
   User: "Show me all Ford invoices..."
   Context: [top 5 chunks from search]
        ↓
6. Azure OpenAI Chat Completions → answer
        ↓
User sees: formatted answer with citations
```

---

### 5. Index Management — Operations You'll Do

**Re-indexing:** When your schema changes, you must delete and recreate the index. You cannot add a new field as `key` or change field types. You can add new optional fields to an existing index without recreation.

**Scoring profiles:** Boost certain fields or recency:
```json
{
  "scoringProfiles": [{
    "name": "boost-recent",
    "functions": [{
      "type": "freshness",
      "fieldName": "invoiceDate",
      "boost": 2,
      "freshness": { "boostingDuration": "P30D" }
    }]
  }]
}
```
`P30D` = ISO 8601 for 30 days. Documents from last 30 days get 2x score boost.

**Aliases:** Create an alias pointing to your index. Swap the alias to a new index for zero-downtime re-indexing:
```
invoices-alias → invoices-index-v1
(re-index to invoices-index-v2)
invoices-alias → invoices-index-v2  (swap, no downtime)
```

---

## Module 9 — Integration Pattern: JM Family Full Pipeline

Putting it all together — the complete architecture using what you've built across Modules 7, 8, and 9:

```
┌─────────────────────────────────────────────────────────────────┐
│                     INGESTION PATH                              │
│                                                                 │
│  Blob Storage (PDFs)                                            │
│       ↓ blob trigger                                            │
│  Azure Function (C#)                                            │
│       ↓ calls                                                   │
│  Document Intelligence ──→ Extracted JSON (fields + text)       │
│       ↓                                                         │
│  Azure OpenAI Embeddings ──→ float[1536] for content chunks     │
│       ↓                                                         │
│  Azure AI Search (Push API) ──→ Index updated                   │
│       ↓ also write metadata                                     │
│  Cosmos DB ──→ Full document record                             │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                     QUERY PATH (RAG)                            │
│                                                                 │
│  User question                                                  │
│       ↓                                                         │
│  Azure OpenAI Embeddings ──→ query vector                       │
│       ↓                                                         │
│  Azure AI Search (Hybrid + Semantic) ──→ top 5 chunks           │
│       ↓                                                         │
│  Azure OpenAI Chat Completions ──→ answer with citations        │
│       ↓                                                         │
│  User                                                           │
└─────────────────────────────────────────────────────────────────┘
```

**Authentication everywhere:** `DefaultAzureCredential` → Managed Identity. No connection strings in code.

---

## Module 9 — Self-Test Questions

**Q1.** What is the difference between a replica and a partition in Azure AI Search?

> **A:** Replicas are copies of the index for query throughput and high availability (need ≥2 for SLA). Partitions shard the index for storage capacity and indexing throughput. You scale replicas for query load; you scale partitions for data size.

---

**Q2.** In your JM Family pipeline, Document Intelligence extracts invoice fields and you want to push them to Azure AI Search. Should you use a Pull indexer or Push API? Why?

> **A:** Push API. Your Azure Function already has the extracted JSON from Document Intelligence — there is no need for AI Search to pull from Blob again. Push the pre-processed document directly from your Function after extraction. Pull indexers are best when you want AI Search to manage the ingestion schedule and run skillsets during ingestion (e.g., pulling raw PDFs from Blob and running OCR as a skill).

---

**Q3.** A user searches for "vehicles with missing VIN numbers" but the index contains documents with text like "VIN field not populated" and "no VIN recorded." Would BM25 keyword search find these? Would vector search?

> **A:** BM25 would likely miss them — the query terms ("missing VIN") don't exactly match the document terms ("not populated", "no VIN recorded"). Vector search would find them because it works on semantic meaning — "missing VIN numbers" and "VIN field not populated" mean the same thing and their embedding vectors are close in space.

---

**Q4.** What does RRF (Reciprocal Rank Fusion) do in a hybrid query?

> **A:** RRF merges the ranked result lists from keyword (BM25) and vector search. It uses each document's rank position (not its raw score) from both lists to compute a combined score. Documents that rank highly in both lists get the highest combined score. Neither search type dominates — documents relevant from both signals rise to the top.

---

**Q5.** You have an Azure AI Search index with 10 million document chunks and query latency is too high. What do you scale — replicas or partitions?

> **A:** Both, but for different reasons. If latency is high under load (many concurrent queries), scale **replicas** — each replica handles queries independently. If the index is large and vector search is slow because HNSW graph traversal is expensive, scale **partitions** — each partition holds a shard of the index and queries run in parallel across shards.

---

**Q6.** You update your index schema to add a new vector field for a second embedding model. Do you need to recreate the index?

> **A:** Adding a new optional field to an existing index does NOT require recreation — you can do it with a schema update. However, existing documents won't have the new vector field populated; you'll need to re-index (re-push) all documents to populate the new field. If you change a field type, rename a field, or change the key field — those require a full delete-and-recreate.

---

## Memory Hooks

- **"Search = Index + Indexer + Skillset + Data Source"** — the four objects you configure
- **"Replicas = queries, Partitions = data"** — scale replicas for throughput, partitions for size
- **"BM25 = keywords, Vector = meaning, Hybrid = both"** — always use hybrid for RAG
- **"RRF = rank positions, not scores"** — fusion works on list position, not raw numbers
- **"Semantic ranker re-ranks BM25 top 50"** — it improves relevance, doesn't replace retrieval
- **"Skillset runs during indexing, not querying"** — enrichment is a one-time cost per document
- **"Alias → zero downtime re-index"** — swap alias, not the index name in your app
- **"Your JM Family pipeline IS Module 9 in production"** — you're already running what this chapter teaches

---

---

## 2026 Updates

| Topic | Update |
|---|---|
| **Integrated vectorization GA** | The "Import and vectorize data" wizard in portal now GA — chunks, embeds, and indexes in one step without writing code. Supports Azure OpenAI, AI Vision, and custom embedding models |
| **Semantic ranker v3** | Improved re-ranking model — better relevance, now included in S1 tier (was add-on). Enable with `queryType: "semantic"` |
| **Vector compression** | Scalar quantization now GA — reduces vector storage by 4x with ~1% accuracy loss. Binary quantization also available (32x smaller, ~5% loss). Critical for large indexes cost control |
| **Agentic retrieval** | New `2025-05-01-preview` API — AI Search can now run a RAG pipeline internally, returning a formatted answer string (not just documents). Designed for AI Agent integration |
| **AI Foundry integration** | AI Search indexes now visible and queryable from Azure AI Foundry Agents as "Knowledge" sources. No custom code needed to wire Search to an Agent |
| **Matryoshka embeddings** | text-embedding-3-large supports variable dimensions (256, 512, 1536). Use 256-dim for storage savings, 1536-dim for accuracy. Specify dimensions in embedding API call |

---

## Interactive Learning Ideas

### Exercise 1 — Portal Wizard vs Push API Comparison (20 min)
In Azure portal → your AI Search resource:
- Try "Import and vectorize data" wizard with a sample blob container
- Note what it creates: indexer, index schema, skillset, datasource
- Compare to what JMA's EnterpriseSearch.Sync does manually
- Why does JMA use Push API instead of this wizard? (5 specific reasons from Module 9)

### Exercise 2 — Query Type Comparison (15 min)
Using the Search Explorer in Azure portal, run the same query 3 ways:
- `queryType: "simple"` (keyword BM25)
- `queryType: "semantic"` (semantic ranker)
- Vector search with an embedding
Compare ranking of results. When does semantic ranker change the order vs BM25?

### Exercise 3 — Vector Compression Test
If you have an index with vectors:
- Check current index size in portal
- Enable scalar quantization on the vector field
- Re-index a sample of documents
- Compare index size before/after
- Is the accuracy difference noticeable on your queries?

### Exercise 4 — JMA Index Audit (20 min)
Look at `srch-jma-prod-indexer` (DealerSource, forms index):
- Is it using keyword only or vector search?
- What's the index schema? (which fields, types, searchable/filterable/sortable)
- What would you add to make it RAG-capable? (which fields need vectors?)
- Estimate the cost impact of adding embeddings to all existing documents

### Exercise 5 — Hybrid Search Implementation
Write the C# code to run a hybrid search (keyword + vector) on the JMA AI Search index:
- Take a user query string
- Embed it with Azure OpenAI text-embedding-3-large
- Submit hybrid search (keyword + vector)
- Enable semantic ranker
- Return top 5 results with score, title, and excerpt

---

*Next: Module 10 — Bot Development*
*Updated: 2026-06-30*

---

## Interview Gap: Vector Database Comparison

### The Interview Question

> "Why would you choose Azure AI Search over Cosmos DB vector store or a dedicated vector DB like Qdrant or Pinecone?"

This is asked in almost every AI Solutions Architect interview. Here is the full answer.

---

### The Options

```
VECTOR DATABASE OPTIONS ON AZURE (2026):

1. Azure AI Search          ← Microsoft's primary recommendation for RAG
2. Azure Cosmos DB (vector) ← When you already use Cosmos DB for your app data
3. PostgreSQL + pgvector    ← When you're already on PostgreSQL (Azure Flexible Server)
4. Qdrant                   ← Open-source, high performance, available on Azure Marketplace
5. Pinecone                 ← SaaS vector DB, not Azure-native, external dependency
6. Weaviate                 ← Open-source, multi-modal vector search
7. Redis (vector search)    ← When low-latency cache + vector search combined needed
```

---

### Side-by-Side Comparison

| Feature | Azure AI Search | Cosmos DB Vector | PostgreSQL + pgvector | Qdrant |
|---|---|---|---|---|
| **Vector search** | ✅ HNSW, exhaustive KNN | ✅ DiskANN index | ✅ IVFFlat, HNSW | ✅ HNSW, optimized |
| **Hybrid search** | ✅ Native (vector + BM25) | ❌ Vector only | ⚠️ Partial | ⚠️ Limited |
| **Semantic reranking** | ✅ Built-in semantic ranker | ❌ Not available | ❌ Not available | ❌ Not available |
| **Full-text keyword** | ✅ BM25 + linguistic analysis | ❌ Limited | ✅ Full PostgreSQL FTS | ❌ Not available |
| **Filtering** | ✅ Rich OData filters | ✅ SQL-like queries | ✅ Full SQL WHERE | ✅ Payload filters |
| **Integrated vectorization** | ✅ Auto-embed on ingest | ❌ Manual | ❌ Manual | ❌ Manual |
| **AI Foundry integration** | ✅ Native, one click | ❌ Manual setup | ❌ Manual setup | ❌ Manual setup |
| **Metadata + structured fields** | ✅ Rich field schema | ✅ Full document model | ✅ Full relational | ⚠️ Payload only |
| **Transactions (ACID)** | ❌ Not a database | ✅ Full ACID | ✅ Full ACID | ❌ Not a database |
| **Global distribution** | ✅ Multi-region replicas | ✅ Turnkey global | ⚠️ Read replicas | ❌ Self-manage |
| **Azure RBAC / MI** | ✅ Native | ✅ Native | ✅ Native | ⚠️ Custom |
| **Private Endpoint** | ✅ Native | ✅ Native | ✅ Native | ✅ Available |
| **Pricing model** | Per Search Unit (tier) | Per RU/s + storage | Per vCore + storage | Per node |
| **Best for** | RAG, enterprise search | App DB + vectors together | Existing PostgreSQL apps | Pure vector perf |

---

### When to Use Each — Decision Guide

```
START HERE: Do you already have an existing database for your app data?

YES — App data is in Cosmos DB?
  → Use Cosmos DB vector search
  → Vectors live alongside your app documents
  → One connection, one SDK, one bill
  → Trade-off: no semantic reranker, no hybrid BM25

YES — App data is in PostgreSQL?
  → Use pgvector extension on Azure Database for PostgreSQL
  → SQL + vector search in one query
  → Trade-off: manual embedding, no semantic ranker

NO — Building a new AI-first RAG pipeline?
  → Use Azure AI Search ← default choice for Azure
  → Best hybrid search (vector + keyword)
  → Built-in semantic reranker
  → Native AI Foundry integration
  → Integrated vectorization (auto-embed on document upload)

NO — Need maximum vector query performance (millions of vectors, sub-millisecond)?
  → Consider Qdrant (Azure Marketplace)
  → Pure vector performance, flexible payload filtering
  → Trade-off: no built-in keyword search, no semantic reranker, not Azure-native

NEVER use Pinecone for Azure-primary workloads:
  → External SaaS, data leaves Azure boundary
  → No Private Endpoint to your VNet
  → Compliance and data residency risk for JMA
```

---

### JMA Recommendation

```
JMA current state:
  srch-jma-prod-indexer — Azure AI Search ✅ (correct choice)
  No Cosmos DB vector configured yet

JMA future state recommendation:
  Dealer support RAG     → Azure AI Search (hybrid + semantic reranker)
  User preference store  → Cosmos DB vector (alongside user profile data)
  Real-time session mem  → Redis vector (sub-millisecond, short TTL)
```

---

### The One-Line Interview Answer

> "Azure AI Search is the default for RAG on Azure because it's the only option with native hybrid search (vector + BM25 keyword), a built-in semantic reranker, integrated vectorization, and first-class AI Foundry integration. I'd only switch to Cosmos DB vector if the document data is already living there and the query patterns don't need keyword search or reranking."

---

## Appendix — Merged from Legacy Notes

> Consolidated 2026-07-18 during library reorganization. Source: `09b-AzureAISearch-IndexerDeepDive.md`.

### 1. Indexer Schedule Options and the Polling Limit

```
Azure Portal → AI Search → Indexer → Schedule:

  None      ← run manually only (REST API call)
  Once      ← run now, never again
  5 minutes ← minimum polling interval
  Hourly    ← most common
  Daily     ← overnight batch
```

There is **no event-driven trigger built into the indexer**. It always polls on schedule — it never "reacts" to an upload. This is the single most misunderstood property of indexers, and it sets the floor on data freshness: with an hourly schedule, a blob uploaded at 09:01 is invisible to queries until 10:00.

---

### 2. Near Real-Time Indexing — The Event Grid Pattern

When the polling floor is unacceptable, you bolt event-driven triggering on top:

```
Blob Storage
     │ (fires event on every upload, within seconds)
     ▼
Azure Event Grid
     │
     ▼
Azure Function
     │ (calls AI Search REST API)
     ▼
POST /indexers/my-indexer/run
     │
     ▼
Indexer runs immediately, picks up new blob
```

This is **extra architecture you own** — not a built-in indexer feature. The indexer still uses its high-water mark to decide what to process; Event Grid only changes *when* it wakes up. Watch for run collisions: an indexer already running will reject a concurrent `run` call.

---

### 3. Import and Vectorize Data Wizard — What It Asks and What It Creates

```
Azure Portal → your AI Search resource
 └── Overview page → "Import and vectorize data" button

  Step 1: Connect your data          ← Blob Storage / SharePoint / ADLS
  Step 2: Vectorize your text        ← EMBEDDING MODEL
           ├── Kind: Azure OpenAI
           └── Model: text-embedding-3-large
  Step 3: Vectorize images           (skip if text only)
  Step 4: Advanced settings          ← CHUNKING
           ├── Chunk size: 512 tokens (slider)
           └── Chunk overlap: 10%

OR in AI Foundry:
  ai.azure.com → My assets → Indexes → + New index
  ← same settings, easier to find
```

Chunk size and overlap live under **Advanced settings** — easy to miss, and the defaults are what most teams accidentally ship.

**What exists afterward:**

```
BEFORE import:                AFTER wizard completes:
  Blob Storage → your PDFs      AI Search
  AI Search   → (empty)         ├── INDEX created (you name it)
                                │    ├── id       → "chunk-001"
                                │    ├── content  → "RAV4 XLE $42,500 in Black..."
                                │    ├── source   → "inventory.pdf"
                                │    └── vector   → [-0.023, 0.061, 0.048, ...]
                                └── INDEXER created (auto-named)
                                     └── stays permanently, runs on schedule
```

The indexer is not a one-off import job — it is a permanent object that keeps running on its schedule after the wizard closes.

---

### 4. RAG vs File vs Multi RAG — The Blob Processing Choice

When you select Blob Storage, the wizard asks **how** to process the documents:

| Option | Behavior | Use for |
|---|---|---|
| **RAG** | Chunks text → embeds text → stores text vectors | Text-only documents: vehicle data, warranty docs, policy docs |
| **File** | Treats each file as one document, no chunking, no vectors — pure keyword | Structured files (JSON, CSV) where the whole file is the unit |
| **Multi RAG** (multi-modal) | Creates **both** text vectors and image vectors | PDFs with diagrams, charts, photos — technical manuals, brochures |

Picking **File** silently gives you no vector search. If retrieval quality is inexplicably poor on a wizard-built index, check this setting first.

---

### 5. Push vs Pull — Full Capability Comparison

```
                    AI SEARCH INDEXER (Pull)      CUSTOM CODE (Push)
────────────────────────────────────────────────────────────────────
Data source         Blob / SQL / Cosmos           Any source via code
Custom filtering    ❌ not possible               ✅ full IF/ELSE logic
Retention rules     ❌ not possible               ✅ date cutoff in code
Multi-library       ❌ one source per indexer     ✅ multiple sources
Stale doc cleanup   ⚠️ soft-delete workaround     ✅ custom diff logic
Code required       ❌ zero code                  ✅ full SDK project
Maintenance         Low — Azure manages           Higher — your code
Good for            Simple uniform data           Complex business rules
```

```
USE PULL when:                        USE PUSH when:
  ├── Source is Blob / SQL / Cosmos     ├── Source needs Graph API
  ├── No custom field filtering         │   (SharePoint w/ column filtering)
  ├── No retention rules                ├── Business rules on inclusion
  └── You want zero-code RAG fast       ├── Custom retention / date logic
                                        └── Multiple sources → one index
```

---

### 6. JMA Production — `EnterpriseSearch.Sync` and the Five Reasons for Push

This is the concrete answer to Exercise 1 above ("Why does JMA use Push API instead of the wizard?").

```
Project: <repo>/docmgmt/Azure/AppServices/app-jma-docmgmt-aisearch

Two projects:
  EnterpriseSearch.Api   ← ASP.NET Core Web API (search/read)
  EnterpriseSearch.Sync  ← .NET BackgroundService (the "WebJob")
```

| # | Reason | Evidence in code | Why the built-in indexer cannot do it |
|---|---|---|---|
| 1 | **Retention filtering** | `GraphReaderService.cs:57` — `var retentionCutoff = DateTimeOffset.UtcNow.AddMonths(-RetentionMonths);` | SharePoint indexer has no retention logic |
| 2 | **JobSource column filtering** | `GraphReaderService.cs:89` — `var allowedJobSources = _sharePointOptions.GetAllowedJobSources();` | Built-in indexer pulls everything; cannot filter by column |
| 3 | **Multi-library support** | `GraphReaderService.cs:65` — `foreach (var libraryContext in siteContext.LibraryContexts)` | One data source = one library |
| 4 | **Stale document cleanup** | `IndexWriterService.cs:38` — `await _indexWriterService.DeleteMissingDocumentsAsync(activeDocumentIds)` | No equivalent diff-and-delete mechanism |
| 5 | **Schema validation on startup** | `SearchIndexProvisioningService.cs:135` — `ValidateExistingIndex(existingResponse.Value)` | Prevents silent schema drift in shared environments |

**Sync flow (`Worker.cs`):**

```
RunSyncAsync() every run:
 │
 ├── EnsureIndexAsync()
 │    ← create index if missing, validate schema if exists
 │
 ├── GraphReaderService.ProcessDocumentsAsync()
 │    ← calls graph.microsoft.com via Microsoft Graph API
 │    ← $top=200 per page
 │    ← filters by JobSource + RetentionMonths
 │    ← skips files with no contractNumber
 │    ← batches into groups of 100
 │
 ├── IndexWriterService.UploadDocumentsAsync()
 │    ← SearchClient.UploadDocumentsAsync(batch)  ← PUSH API
 │    ← no AI Search indexer involved
 │
 └── IndexWriterService.DeleteMissingDocumentsAsync()
      ← removes stale docs no longer in SharePoint
```

**Schedule (`WorkerScheduleOptions.cs`):**

| Setting | Default |
|---|---|
| Mode | `Daily` (alternative: `Interval`) |
| Daily run time | 09:00 |
| Timezone | Eastern Standard Time |
| `IntervalMinutes` (Interval mode) | 1440 (24 hrs) |
| `RunOnStartup` | false (configurable) |

**Authentication — dual strategy, resolved at startup:**

```
SharePoint (Graph):
  → ConfidentialClientApplication if ClientId + ClientSecret configured
  → DefaultAzureCredential (Managed Identity) otherwise

AI Search:
  → AzureKeyCredential if ApiKey configured
  → ClientSecretCredential if ClientId + Secret + TenantId configured
  → DefaultAzureCredential otherwise
```

---

### 7. JMA Index Schema — Keyword-Only, No Vectors

```csharp
// SearchIndexProvisioningService.cs — BuildIndexDefinition()
// No vectors, no embeddings — pure keyword/filter lookup:

  id               → filterable only (key)
  sharePointItemId → filterable only
  sharePointDriveId→ filterable only
  sourceLibrary    → filterable only
  contractNumber   → filterable only (NOT searchable)
  jobSource        → filterable only
  fileName         → SEARCHABLE ← only keyword-searchable field
  completedDate    → filterable + sortable
  scannedDate      → filterable + sortable
```

Note the architectural consequence: `contractNumber` is **filterable but not searchable**, so it works in `$filter=contractNumber eq 'C-84512'` but not as free-text search. `fileName` is the only field a user can actually search. This index is a lookup table, not a RAG retrieval layer — adding `contentVector` and a chunked `content` field is what would make it RAG-capable.

---

### 8. Staging Environment — No Indexers (Confirmed)

```
srch-jma-stg-indexer investigation:
  Indexers:     0  ← confirmed empty (200 OK, value: [])
  Data Sources: 0  ← no SharePoint connection
  Skillsets:    0
  Indexes:      1  ← documents-stg exists

WHY: Staging doesn't connect to real SharePoint (safety).
     Test data is loaded via Push API from the deployment pipeline.
     No live SharePoint connection = no indexer needed in staging.
```

Worth remembering when auditing environments: an empty indexer list is not necessarily a misconfiguration — in a push-based architecture it is the expected state.
