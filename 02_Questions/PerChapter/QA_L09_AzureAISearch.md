# Q&A — L09: Azure AI Search
**Source chapter:** `01_Lessons/Part2_AzureAIServices/L09_AzureAISearch.md` | **Format:** self-study
**Questions:** 32 | *No overlap with the interview bank (architect-judgment level) or the chapter's own self-test — these test the chapter's factual content directly.*

---

## Fundamentals & Index Schema

**Q1. What three things does Azure AI Search do, end to end?**
(1) **Ingestion** — pulls data from sources (Blob, Cosmos DB, SQL, SharePoint, custom) via indexers or accepts pushed documents; (2) **Enrichment** — runs AI skillsets during ingestion (OCR, entity extraction, embedding generation); (3) **Querying** — keyword (BM25), vector, semantic re-ranking, and hybrid combinations.

**Q2. Name the four core objects you configure in a search service, with the chapter's ADF analogies.**
| Object | Is | Analogy |
|---|---|---|
| **Index** | Schema + stored data | SQL table |
| **Indexer** | Pulls from source, runs skillset, writes to index | ETL/ADF pipeline |
| **Skillset** | AI enrichment steps during indexing | ADF transformation activities |
| **Data Source** | Connection to raw data | ADF linked service |
(Plus optional **Synonym Map** for keyword search.)
*Memory hook: "Search = Index + Indexer + Skillset + Data Source."*

**Q3. List the six field attributes and what each enables.**
`searchable` — tokenized into full-text search; `filterable` — usable in `$filter` expressions; `sortable` — order results by it; `facetable` — aggregate/group counts by it; `retrievable` — returned in results; `key` — the unique document identifier (required, exactly one per index).

**Q4. What two settings define a vector field, and what constraint applies?**
Type `Collection(Edm.Single)` with `dimensions` and a `vectorSearchProfile`. The **dimensions must match the embedding model's output** (text-embedding-3-small = 1536, 3-large = 3072) — and you can't mix embedding models in the same field.

**Q5. What are the service tiers, and which does the chapter recommend for JMA?**
**Free** (50MB, 3 indexes, no SLA — dev/test), **Basic** (2GB, 5 indexes — small production), **Standard S1/S2/S3** (scalable replicas+partitions — production), **Storage Optimized** (large data, lower query throughput). JMA: Basic for dev, **S1 for production**.

**Q6. How many replicas do you need for the SLA, and for write HA?**
≥2 replicas for the read SLA, **≥3 for write (read-write) HA**.

---

## Ingestion

**Q7. What are the three ways to get data into an index?**
**Push API** (your code calls the REST API/SDK — real-time, custom pipelines), **Pull/Indexer** (Search pulls from a configured source on schedule), **Import Data wizard** (portal — quick testing only).

**Q8. What are the Push API batch limits, and what's the batching rule?**
Max **1,000 documents or 16 MB per batch** — and always prefer batch upload over single-document calls.

**Q9. How do you configure a blob data source connection without keys?**
Use a Managed Identity connection string format: `ResourceId=/subscriptions/.../storageAccounts/...` in the data source's credentials — no account key in config.

**Q10. What's the minimum indexer schedule interval, and how is it written?**
**5 minutes**, in ISO 8601 duration format: `"interval": "PT5M"`.

**Q11. How does change detection work per source type?**
Blob Storage — `LastModified` metadata automatically; SQL — a `rowVersion`/`lastModified` column; Cosmos DB — the change feed. The indexer keeps a **high-water mark** (last processed timestamp) so it never re-indexes everything each run.

---

## Skillsets & Enrichment

**Q12. What's the difference between an index built with vs without a skillset?**
Without: the index contains only what's in the source documents. With: source data **plus AI-derived fields** added during indexing — chunks, language, entities, key phrases, sentiment, embeddings.

**Q13. Name eight built-in skills and a use for each.**
OCR (text from scans), Text Merge (merge OCR text into content), **Split** (chunk long text for RAG), Language Detection (route multilingual docs), Entity Recognition (people/orgs/locations/dates), Key Phrase Extraction (auto-tagging), Sentiment (feedback analysis), PII Detection (redact before storing), Image Analysis (describe embedded images), **Custom Web API** (call your own Azure Function as a skill), **Azure OpenAI Embedding** (vectorize chunks — critical for RAG).

**Q14. Walk the flow of the Azure OpenAI Embedding skill during indexing.**
Indexer reads the document from Blob → Split skill chunks it (e.g., 512-token chunks → `/document/pages/*`) → the embedding skill calls the Azure OpenAI deployment (`deploymentId: text-embedding-3-small`) per chunk → each `float[]` lands in the index's `contentVector` field. This is the **ingestion half** of RAG; query time uses the same model to vectorize the user's question.

**Q15. What is a Knowledge Store, and is it required for RAG?**
An optional output of a skillset — enriched projections written to Blob/Table Storage for debugging the enrichment pipeline, downstream use, or training custom models. **Not required for RAG.**

**Q16. What is integrated vectorization?**
The index is configured with a **vectorizer** so AI Search auto-calls the embedding model **at query time** — you send text, it embeds and vector-searches internally, no query-embedding code on your side. (Now GA, including the portal's "Import and vectorize data" wizard for the ingestion side.)

---

## Querying

**Q17. Match the four query types to what they're best for.**
**Full-text/BM25** — exact keywords, known terms. **Vector** — semantic/meaning match. **Semantic (re-ranking)** — improving relevance of keyword results. **Hybrid (BM25 + vector via RRF)** — best overall, recommended for RAG.
*Memory hook: "BM25 = keywords, Vector = meaning, Hybrid = both."*

**Q18. Write the OData filter for: dealer JMF-ATL-001, amounts over 10000, from Jan 1 2026 on.**
`dealerCode eq 'JMF-ATL-001' and totalAmount gt 10000 and invoiceDate ge 2026-01-01T00:00:00Z`. For multi-value matching: `search.in(dealerCode, 'JMF-ATL-001,JMF-DAL-002', ',')`.

**Q19. KNN vs ANN in vector queries — what does production use?**
**KNN (exhaustive)** checks every vector — exact but slow at scale. **ANN** via HNSW — fast, ~99% accurate — the production choice.

**Q20. In a hybrid query, why set KNearestNeighborsCount to ~50 when you only want 5 results?**
The 50 are **candidates for RRF fusion** — both the keyword list and vector list contribute their top candidates, RRF merges by rank, and you take the fused top 5 (optionally after semantic re-ranking). Starving the fusion of candidates degrades the merge quality.

**Q21. What are semantic Captions and Answers, and how do captions help RAG?**
**Captions** — highlighted relevant snippets from each matched document; **Answers** — a direct answer extracted from the top result for question-like queries. In RAG, sending the caption (the relevant excerpt) instead of the full chunk shrinks prompt tokens while keeping the relevant content.

**Q22. What are facets used for, and what does a facet request look like?**
Aggregation counts for search UIs (filter panels): `Facets = { "dealerCode,count:10", "vehicleMake,count:5" }` → returns value+count buckets per field (e.g., JMF-ATL-001: 142).

---

## Vector Config & Semantic Ranker

**Q23. Name the four HNSW parameters and what raising each does.**
`m` — graph connections per node (higher = more accurate, more memory); `efConstruction` — build-time accuracy (higher = better index, slower build); `efSearch` — query-time candidates examined (higher = more accurate, slower queries); `metric` — distance function (**cosine** for text embeddings). Chapter's production defaults: m=4, efConstruction=400, efSearch=500, cosine.

**Q24. How does the semantic ranker actually operate, and what are its constraints?**
It re-ranks the **top 50 BM25 candidates** (you don't choose which 50) using a Microsoft-hosted language model, returning re-scored results with captions. Constraints: Standard tier+ (v3 is now included in S1), adds ~200–500ms latency, billed per 1,000 semantic queries.
*Memory hook: "Semantic ranker re-ranks BM25 top 50 — improves relevance, doesn't replace retrieval."*

**Q25. In a semantic configuration, what do titleField, contentFields, and keywordsFields each carry?**
`titleField` — the document's title-like field; `contentFields` — the main text the ranker reads to understand document meaning (put your primary content here); `keywordsFields` — tag-like fields (e.g., dealerCode).

**Q26. Which embedding model does the chapter recommend for JMA, and when would you upgrade?**
`text-embedding-3-small` (1536-dim) — best accuracy/cost balance. Move to `3-large` (3072) only if RAG accuracy testing shows it **meaningfully** better on your data. `ada-002` is legacy — don't use for new projects.

**Q27. What are Matryoshka embeddings, and what's the trade-off?**
The `text-embedding-3` models support requesting **reduced dimensions** (256/512/1536) from the same model — e.g., 256-dim is ~6x cheaper storage with only ~3% accuracy loss. Specify `dimensions` in the API call.

---

## Index Ops & 2026 Updates

**Q28. Which schema changes require full index recreation, and which don't?**
**No recreation:** adding a new optional field (but existing docs won't have it populated — re-push to fill it). **Full delete-and-recreate:** changing a field type, renaming a field, or changing the key field.

**Q29. What does a freshness scoring profile do, and how is 30 days expressed?**
Boosts recent documents' scores — e.g., `type: "freshness"` on `invoiceDate` with `boost: 2` and `boostingDuration: "P30D"` gives documents from the last 30 days a 2x score boost.

**Q30. What's the alias pattern for zero-downtime re-indexing?**
The app queries an **alias** (`invoices-alias → invoices-index-v1`); build `invoices-index-v2` in the background, then swap the alias to v2 — no app config change, no downtime.
*Memory hook: "Alias → zero-downtime re-index."*

**Q31. What do scalar and binary quantization buy you?**
Vector compression: **scalar** — 4x smaller storage, ~1% accuracy loss (GA); **binary** — 32x smaller, ~5% loss. Critical cost control for large vector indexes.

**Q32. What is agentic retrieval, and how does it differ from a normal query?**
A newer API capability where AI Search **runs a RAG pipeline internally and returns a formatted answer string**, not just matching documents — designed for AI-agent integration. Related: AI Foundry Agents can attach a Search index as a "Knowledge" source with no custom wiring code.

---

*Curriculum Q&A Batch B — file 1 of 4. Next: QA_L10 (Bot Development).*
