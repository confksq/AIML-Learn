# Q&A — L13: RAG Deep Dive
**Source chapter:** `01_Lessons/Part3_GenAI_LLMs/L13_RAG_DeepDive.md` | **Format:** self-study
**Questions:** 34 | *No overlap with the interview bank (03_RAG_Architecture covers the architect-judgment versions) or the chapter's own self-test — these drill the chapter's concrete strategies and code.*

---

## Fundamentals

**Q1. What four LLM limitations does RAG address, and how does it solve each?**
Knowledge cutoff → retrieve current docs at query time. Context size → retrieve only relevant chunks, not the whole library. Hallucination → ground answers in retrieved documents. No private data → retrieve from your own indexed data. RAG = LLM's knowledge + your up-to-date private data, fused at query time.

**Q2. What do the three letters R-A-G stand for as steps?**
**Retrieve** (search the index for relevant chunks) → **Augment** (add chunks to the prompt as context) → **Generate** (LLM answers from context, not training data).

**Q3. Distinguish Naive, Advanced, and Modular RAG.**
Naive — basic retrieve→prompt→generate (fine for simple Q&A). Advanced — adds pre-retrieval and post-retrieval improvements (better chunk quality, better retrieval). Modular — RAG as a pipeline of swappable components (agents, routing, tools) for complex multi-step reasoning.

**Q4. Name four things RAG is NOT.**
Not fine-tuning on your docs (that bakes knowledge into weights; RAG retrieves at runtime). Not semantic search alone (search retrieves; RAG adds generation). Not stuffing the full document into the context window (no retrieval step). Not Azure AI Search alone (that's just the retrieval layer).

**Q5. What are the two separate pipelines in a RAG system?**
**Indexing pipeline** (offline) — runs once per document: chunk → embed → store. **Query pipeline** (online) — runs on every question in real time: embed question → retrieve → augment → generate.

---

## Document Processing

**Q6. Why use Document Intelligence for document *loading*, not just extraction?**
It preserves reading order (columns/headers/footers correctly sequenced), serializes tables as structured text instead of jumbling them, and preserves page boundaries for citations (chunk → page number).

**Q7. What should text cleaning preserve vs strip before chunking?**
**Preserve:** section headings (context for chunking/retrieval), table structure (serialize rows, keep column names), lists, page numbers (citations). **Strip:** repeated headers/footers, page-number patterns, watermarks, excessive whitespace.

**Q8. Why does every chunk need metadata — give the four uses.**
Filtering (`documentType eq 'invoice' and dealerCode eq '...'`), citation ("Source: FordInvoice_2026_01.pdf, Page 2"), re-ranking (boost recent, filter by date), and deduplication (re-index updated docs by `SourceDocumentId`).

---

## Chunking

**Q9. Name five chunking failure modes and their cause.**
Chunk too large → vector averages too much meaning, poor retrieval. Too small → lacks context, LLM can't answer. Split mid-sentence → broken meaning, bad embeddings. Split mid-table → table cells across chunks, unreadable. No overlap → answer spanning a boundary is missed.

**Q10. What does overlap accomplish, and how is it implemented?**
If an answer spans two chunks' boundary, overlap ensures one chunk still contains enough context. Implementation: advance the window by `chunkSize − overlap` so the last N words of each chunk repeat at the start of the next (e.g., words 463–512 repeat).

**Q11. What's the priority order in recursive character chunking, and why is it the default?**
Try separators in order: `\n\n → \n → . → , → " " → character`. It splits on the most natural boundary available before falling back to finer ones — good coherence without semantic chunking's cost. It's LangChain's `RecursiveCharacterTextSplitter` default; use it unless you have a specific reason not to.

**Q12. How does semantic chunking find boundaries, and what's its downside?**
Embed every sentence → compute cosine similarity between adjacent sentences → a **sharp similarity drop = topic boundary = chunk boundary**. Downside: expensive — requires embedding every sentence during chunking. Use for long docs with mid-page topic shifts (annual reports, research papers).

**Q13. How do you chunk tables and section-structured documents specifically?**
**Tables:** serialize the whole table as one chunk (group cells by row, join with `|`) — never split mid-table. **Sections:** use Document Intelligence paragraph roles — start a new chunk at each `SectionHeading`, carrying the section name as metadata.

**Q14. Give the recommended chunk sizes for invoices, prose contracts, and FAQs.**
Invoices/forms: 256–512 tokens, 20–30 overlap. Policies/contracts (prose): 512–1024 tokens, 50–100 overlap. FAQs: one Q&A pair per chunk, no overlap. Tables: entire table, no overlap. Default starting point: **512 tokens** (≈2048 chars ≈ ~350 words).

**Q15. Explain parent-child chunking and the precision/recall problem it solves.**
Store large parents (e.g., 1024–1500 tokens) and small children (150–256 tokens); **index only the children** (small = precise, specific embeddings = better recall), but **return the matched child's parent** to the LLM (large = full context = better answers). It resolves the trade-off: small chunks retrieve precisely, large chunks answer completely.
*Memory hook: "Retrieve small, return large."*

**Q16. What is late chunking, why does it help, and is it available in Azure OpenAI?**
Embed the **full document first** with a long-context embedding model so every token's vector is influenced by surrounding text (so "It" encodes "RAV4 Hybrid" from earlier), **then** split the token embeddings into chunks — each chunk keeps cross-document context. **Not supported by Azure OpenAI text-embedding-3** (uses early chunking); available via Jina AI / Voyage AI models in the AI Foundry model catalog.

---

## Vector Databases

**Q17. What question does a vector database answer, and how does that differ from SQL/Cosmos?**
"Which stored vectors are closest to this query vector?" — search by **mathematical distance in high-dimensional space**, versus standard databases storing and filtering structured data.

**Q18. What five capabilities make Azure AI Search a vector DB?**
Stores `Collection(Edm.Single)` float-array fields, HNSW ANN index, cosine/Euclidean/dot-product metrics, native hybrid search (keyword + vector in one query), and metadata filtering alongside vector search.

**Q19. Match distance metric to use: cosine, dot product, Euclidean.**
Cosine — text embeddings (direction matters, not magnitude) — **always use for text RAG**. Dot product — normalized embeddings (equivalent to cosine, faster). Euclidean/L2 — image embeddings / when magnitude matters.

**Q20. How does HNSW achieve fast search, and what's the trade-off?**
A layered graph (sparse top layers, dense bottom): start at the top entry point, greedily navigate toward the query, drop a layer, repeat, collect K nearest at the bottom — O(log N) instead of O(N) brute force. Trade-off: **approximate** (a few true neighbors may be missed), but recall is ~99%+ with good parameters.

---

## Retrieval Strategies

**Q21. What are the three failure modes of bad retrieval?**
The LLM answers incorrectly from wrong context, says "I don't know" (context doesn't match), or hallucinates (context absent, falls back to training data).

**Q22. Why always pre-filter before vector search when you can?**
It reduces candidates (faster), reduces noise (another dealer's invoices can't appear), and enables user/tenant-specific retrieval (multi-tenancy). Example: `Filter = "dealerCode eq 'JMF-ATL-001' and documentDate ge 2026-01-01T00:00:00Z"`.

**Q23. What is multi-query retrieval and when does it help?**
Use the LLM to generate several search-query variants from one question, retrieve for each, and deduplicate results by chunk ID. Helps when the user's business-domain language differs from the document language.

**Q24. Explain HyDE and why it improves recall.**
**Hypothetical Document Embeddings**: instead of embedding the question, ask the LLM to write a hypothetical answer in formal document language, then embed **that**. A document-style hypothetical answer matches real documents better than a conversational question — helps when question and document language diverge.
*Memory hook: "Embed the answer, not the question."*

**Q25. What does MMR balance, and what does lambda control?**
Maximal Marginal Relevance balances **relevance to the query** against **diversity from already-selected chunks** — avoids returning 5 chunks that all say the same thing. `lambda`: 1 = pure relevance, 0 = pure diversity (0.5 = balanced). Use for repetitive content (policy manuals, similar FAQs).

**Q26. Contrast bi-encoder and cross-encoder, and name Azure's managed cross-encoder.**
**Bi-encoder** — embeds query and doc separately, fast (precomputed doc vectors), good — used for first-stage retrieval. **Cross-encoder** — reads query+doc together, slow (runs per pair), better (sees full interaction) — used to re-rank the shortlist. Azure's managed cross-encoder re-ranker = the **semantic ranker** (`QueryType = Semantic`).

**Q27. What is self-querying retrieval?**
Let the LLM extract a **structured OData filter** from a natural-language query — "Ford dealers in Atlanta over $40,000 in January" → `vehicleMake eq 'Ford' and dealerCity eq 'Atlanta' and totalAmount gt 40000`. Eliminates a manual filter UI when users ask in natural language about structured data.

---

## Generation

**Q28. What temperature for RAG, and why?**
**Temperature = 0** — you want factual, deterministic answers grounded in documents, not creative generation.

**Q29. Give the standard RAG prompt structure and the two must-have instructions.**
System message (role + rules + citation instruction), then user message containing labeled context (`[Source N]: file, page` + content per chunk) followed by the question. Must-haves: "Using ONLY the sources above…" and "If the answer isn't in the sources, say 'I cannot find this information…'" plus "Cite sources using [Source N]."

**Q30. Name four context-window management strategies for when the budget is tight.**
Truncate over-long chunks to first N tokens, summarize earlier conversation turns, reduce K (fewer retrieved chunks), and reduce chunk size at indexing time.

**Q31. Name six hallucination-prevention layers.**
"Answer from sources only" instruction, "say I don't know" instruction, Azure Content Safety groundedness check, temperature=0, a **confidence gate** (if top search score < 0.7, return "no relevant documents" rather than answering), and citation requirement (ungrounded claims have no source).

**Q32. Why does multi-turn RAG need query rewriting? Give the example.**
Follow-ups reference prior context: "Which ones are over $40,000?" is meaningless as a standalone retrieval query. Rewrite it against history into "Ford invoices from Atlanta dealer JMF-ATL-001 over $40,000" before searching.

---

## On Your Data, Advanced Patterns & 2026

**Q33. Name three advanced RAG patterns and what each does.**
**Corrective RAG (CRAG)** — evaluate retrieval confidence; if low, re-query broader / different index / return "insufficient info." **Query decomposition** — split a complex multi-part question into sub-questions, answer each, synthesize. **Step-back prompting** — also retrieve a more general version of the question (e.g., general rejection policy) to interpret the specific case.

**Q34. Name the six RAG evaluation metrics and the two 2026 patterns that reduce cost/improve relationship queries.**
Metrics: **Faithfulness** (answer grounded in context), **Answer Relevance** (addresses the question), **Context Precision** (retrieved chunks relevant), **Context Recall** (found all relevant chunks — needs ground truth), Latency, Cost/query. 2026: **GraphRAG** (GA) builds a knowledge graph for multi-hop relationship queries ("dealers sharing a fleet manager who also handles late accounts"); **semantic caching** in APIM caches responses for semantically-similar queries, cutting LLM calls 20–40% on FAQ-style workloads.

---

*Curriculum Q&A Batch C — file 4 of 4 (L11_3, L11_4, L12, L13 complete). Next batch: L14, L15, L16.*
