# Module 3 — RAG Architecture
**Source plan:** `AIML-Learn/04_Career/00_PRD.md` §4–5, `01_EXECUTION_PLAN.md`
**Format:** WHY / HOW / WHEN / SCALE / DEPLOY
**Question count:** 18

---

### Q1. Fixed-size vs semantic vs recursive chunking — how do you choose?

- **WHY:** Chunking exists to fit retrievable units within embedding-model and context-window limits while preserving enough coherent meaning per chunk for both accurate embedding and useful grounding.
- **HOW:** Fixed-size = split by token/character count, simplest, risks cutting mid-thought. Semantic = split at natural boundaries (paragraphs, sections, sentence-embedding similarity shifts), preserves meaning better, costlier to compute. Recursive = try large structural boundaries first (sections → paragraphs → sentences), falling back to smaller splits only where needed — a practical middle ground.
- **WHEN:** Fixed-size for quick prototypes or highly uniform documents. Semantic when document structure varies and coherence matters (legal, clinical, technical docs). Recursive as the default production choice — good coherence without semantic chunking's compute cost.
- **SCALE:** Semantic chunking's embedding-based boundary detection adds preprocessing cost that scales with corpus size and re-indexing frequency — a real cost line at high document volume/churn.
- **DEPLOY:** Chunking strategy itself isn't region-dependent, but chunking *language-specific* documents (Q4 of Module 2's tokenization point applies here too) may need per-language tuning if deployed globally across content in multiple languages.

**Follow-up probe:** "Your fixed-size chunker keeps splitting a table mid-row and retrieval quality tanks on tabular documents — what's the fix?" (Recursive/structure-aware chunking that treats tables as atomic units, or a separate extraction path for tabular content — this connects directly to Q12 below.)

---

### Q2. How do you choose chunk size and overlap?

- **WHY:** Chunk size trades off precision (smaller = more targeted retrieval) against context sufficiency (too small loses surrounding meaning needed to answer correctly). Overlap prevents information at a chunk boundary from being lost to either side.
- **HOW:** No universal number — typically 200-800 tokens per chunk with 10-20% overlap as a starting range, tuned against actual retrieval evaluation (Q14), not picked arbitrarily.
- **WHEN:** Smaller chunks for precise fact lookup (e.g., a specific field value). Larger chunks when answers require surrounding context/narrative (e.g., explaining a policy's rationale).
- **SCALE:** Smaller chunks = more chunks per document = more embeddings to generate/store and more vectors to search — a direct storage and query-cost multiplier at large corpus size.
- **DEPLOY:** Not deployment-topology-dependent directly, but larger effective corpora (from ingesting content across more regions/business units in a global deployment) amplify the storage/cost trade-off of the chunk-size choice.

**Follow-up probe:** "You cut chunk size in half to improve precision — what unintended cost did you just introduce, beyond storage?" (Retrieval now needs to pull more chunks to cover the same context, which can reintroduce the 'lost in the middle' problem from Module 1 Q6 when those chunks get assembled into the prompt.)

---

### Q3. How do you pick an embedding model, and what's the actual trade-off in dimensionality?

- **WHY:** Embedding model choice determines retrieval quality ceiling — a generative model can't compensate for retrieving the wrong chunks in the first place.
- **HOW:** Higher-dimensional embeddings (e.g., 3072-dim vs 1536-dim on Azure OpenAI's `text-embedding-3` family) capture more nuance but cost more to store and search; most providers now support dimension truncation to trade some quality for cost.
- **WHEN:** Default to the provider's current-generation embedding model unless benchmarking on your own domain data shows another model or dimension setting performs measurably better — don't over-optimize this without evaluation data.
- **SCALE:** Vector storage cost and search latency both scale with dimensionality × document count — at large corpus size, dimension truncation (accepting a small quality trade-off) can be a meaningful cost lever.
- **DEPLOY:** Embedding model must be consistently available in every region you deploy to, and — critically — you cannot silently switch embedding models without re-embedding the entire existing index, since vectors from different models aren't comparable. This is a hard versioning/migration constraint, not a soft preference.

**Follow-up probe:** "You want to switch to a new, better embedding model — what does the migration actually require?" (Full re-embedding and re-indexing of the entire corpus — old and new vectors aren't comparable, so this can't be a rolling/partial migration; requires either a full reindex window or dual-index cutover strategy.)

---

### Q4. Design a hybrid search + reranking pipeline end to end.

- **WHY:** Keyword (BM25) catches exact-match terms vector search misses; vector search catches semantic/paraphrased matches keyword misses; a reranker then re-scores the combined candidate set with a more expensive, more accurate model than either retrieval method alone uses.
- **HOW:** Query → parallel BM25 + vector search → fuse candidates (e.g., reciprocal rank fusion) → top-N candidates → semantic/cross-encoder reranker re-scores → top-K final results passed to the generative model as context.
- **WHEN:** This full pipeline for production RAG serving real users. A simpler vector-only or keyword-only path is acceptable only for prototypes or narrow, well-understood query patterns (Module 2 Q9).
- **SCALE:** Each added stage (fusion, reranking) adds latency — reranking specifically is the most expensive per-query step since it's a heavier model scoring each candidate; retrieve a wide candidate set (recall) cheaply, then narrow with the expensive reranker only on a small top-N (precision), not the other way around.
- **DEPLOY:** The pipeline's components (search index, reranker) both need regional presence to avoid cross-region latency on every query in a multi-region deployment — colocate retrieval infra with the region serving that traffic.

**Follow-up probe:** "Your reranker adds 400ms per query and the product team wants sub-200ms responses — what do you cut, and how do you decide?" (Reduce the candidate set size fed to the reranker rather than cutting the reranker entirely — measure the relevance/latency trade-off empirically rather than guessing; a smaller/faster reranker model is another lever before removing reranking altogether.)

---

### Q5. How do you force a generative model to cite its sources and stay grounded?

- **WHY:** Without explicit instruction and structure, a model will blend retrieved context with parametric knowledge indistinguishably — citations force traceability and give users a way to verify the answer.
- **HOW:** Structure the prompt so each retrieved chunk carries an explicit identifier (e.g., `[Source 1]`), instruct the model to cite the identifier for every factual claim, and validate post-generation that cited IDs actually exist in the provided context (catches fabricated citations, not just fabricated facts).
- **WHEN:** Any customer-facing or decision-influencing RAG answer — not optional for regulated or high-stakes domains (healthcare, financial, legal).
- **SCALE:** Citation-checking adds a lightweight post-generation validation step (string/ID matching) that's cheap even at high volume — much cheaper than a full groundedness-detection model call, and can be a first-pass filter before escalating to the more expensive check.
- **DEPLOY:** Citation formatting/instruction is deployment-topology-independent — but if UI rendering of citations differs across markets/products in a global deployment, the citation *format* contract needs to be stable so downstream consumers don't break.

**Follow-up probe:** "A response cites `[Source 3]` but only 2 sources were retrieved for that query — what does that tell you, and what do you do?" (The model fabricated a citation — this is a citation-level hallucination, worse than an uncited factual error because it actively undermines the trust mechanism; the response should be rejected/regenerated, and this pattern should be tracked as an eval metric (Q14), not just handled ad hoc.)

---

### Q6. What is groundedness detection, and how does it differ from Content Safety?

- **WHY:** Content Safety checks whether content is *harmful*; groundedness detection checks whether content is *supported by the provided context* — a factually-fabricated-but-harmless answer passes Content Safety and fails groundedness, and they must both be checked.
- **HOW:** Post-generation, the groundedness model compares the generated answer against the retrieved context and scores how well each claim is supported, flagging unsupported claims.
- **WHEN:** Any RAG pipeline where hallucination has real consequence — this should run on every production response in regulated domains, and at minimum be part of offline evaluation everywhere else.
- **SCALE:** Adds a model call per response — at high volume this is a real cost/latency addition, which is why some architectures sample (check a percentage of live traffic) rather than checking every single response, backed by full checking in the offline eval pipeline.
- **DEPLOY:** Groundedness detection availability/latency needs to match the region serving the traffic — don't route a groundedness check cross-region and add latency the user-facing path can't absorb.

**Follow-up probe:** "Groundedness detection flags 15% of production responses as ungrounded — is the fix a better groundedness model, or somewhere else in the pipeline?" (Almost always somewhere else — retrieval quality (wrong/missing chunks) or the generation prompt not instructing the model to stick to context are the more common root causes; groundedness detection is the smoke detector, not the fire's cause.)

---

### Q7. RAG answer is wrong despite grounded documents existing in the index — diagnose the two most likely causes.

- **WHY:** This is the single most common real-world RAG failure interviewers probe — it tests whether you understand the pipeline has multiple independent failure points, not one.
- **HOW:** (1) **Retrieval failure** — the relevant chunk exists in the index but wasn't retrieved for this query (embedding mismatch, chunking split the answer across chunk boundaries, or the query phrasing diverges too far from the document's phrasing). (2) **Generation failure** — the relevant chunk *was* retrieved and is in the context, but the model ignored it in favor of its own parametric "knowledge," or misread/misinterpreted the provided context.
- **WHEN:** Diagnose by inspecting the actual retrieved chunks for that query before touching the generation side — if the right chunk wasn't retrieved, no prompt engineering fixes it; if it was retrieved but ignored, that's a grounding-instruction/prompt problem.
- **SCALE:** At high query volume, systematic logging of retrieved-chunks-per-query (not just final answers) is what makes this diagnosable at all — without that visibility, you're debugging blind.
- **DEPLOY:** Not deployment-topology-specific, but multi-region deployments with regionally-partitioned indexes need to confirm the failure isn't actually a routing bug (query hit the wrong region's index, which doesn't contain the relevant document at all).

**Follow-up probe:** "You confirm the correct chunk was retrieved and is in the prompt — the model still gives the wrong answer. What's your next diagnostic step?" (Check for context-window crowding/'lost in the middle' positioning, conflicting information elsewhere in the retrieved set, or the model's own confident parametric knowledge on that topic actively contradicting and overriding the provided context — try isolating that one chunk alone in the prompt to confirm the model *can* answer correctly given clean context.)

---

### Q8. When does GraphRAG outperform standard vector RAG?

- **WHY:** Vector RAG retrieves independent chunks ranked by similarity — it struggles with questions requiring synthesis across multiple, non-adjacent pieces of information connected by relationships (multi-hop reasoning, "how does X relate to Y across these five documents").
- **HOW:** GraphRAG builds an explicit knowledge graph (entities + relationships) from the corpus, often with LLM-generated community summaries, and retrieval traverses/queries that graph structure instead of (or alongside) pure vector similarity.
- **WHEN:** Global/thematic questions across a large corpus ("summarize all policy changes affecting dealer territory codes over the last year") where the answer isn't in any single chunk — standard RAG chunk-retrieval structurally can't answer these well regardless of tuning.
- **SCALE:** Graph construction (entity/relationship extraction, community summarization) is a significant upfront and ongoing indexing cost — much higher than standard chunking/embedding — only justified when the question types actually require it.
- **DEPLOY:** Graph construction/maintenance needs a defined re-indexing cadence as source documents change (graphs go stale differently than a vector index does — a new document can invalidate existing relationship inferences, not just add new content) — this operational cost applies at every deployment tier.

**Follow-up probe:** "A client asks for GraphRAG because it's the newest technique — how do you push back or validate the ask?" (Ask what question types they actually need answered — if it's mostly single-fact lookup or narrow-context Q&A, standard hybrid RAG is cheaper and sufficient; GraphRAG's cost is only justified by genuinely multi-hop/synthesis query patterns, which needs to be demonstrated with real example queries, not assumed.)

---

### Q9. CAG (Cache-Augmented Generation) vs RAG — what's the actual trade-off?

- **WHY:** RAG retrieves relevant context dynamically per query; CAG precomputes and caches a model's internal state (KV cache) over a fixed, relatively static knowledge base, skipping retrieval latency entirely for queries answerable from that cached context.
- **HOW:** CAG loads the entire (bounded) knowledge base into the model's context once, caches the resulting key-value attention state, and reuses that cached state across queries instead of re-processing the context and re-retrieving each time.
- **WHEN:** CAG when the knowledge base is small enough to fit in context and changes infrequently (freshness isn't critical) — RAG when the corpus is large (exceeds context limits) or changes frequently enough that a static cache would serve stale answers.
- **SCALE:** CAG's viability caps hard at context-window size — it doesn't scale to large/growing corpora the way RAG's index does; RAG's retrieval step scales to arbitrarily large corpora since only relevant chunks enter the context per query.
- **DEPLOY:** CAG's cache is model/deployment-specific — in a multi-region deployment, each region's endpoint needs its own warmed cache, and any knowledge-base update requires re-caching everywhere, which is an operational cost RAG's per-query retrieval doesn't have.

**Follow-up probe:** "A healthcare client wants near-instant answers from a formulary that updates weekly — is CAG or RAG the right call?" (Formulary is likely small/bounded enough for CAG, but weekly updates mean the cache needs a defined invalidation/rebuild cadence tied to the update schedule — the freshness requirement, not just corpus size, is what should drive the RAG-vs-CAG decision here; if formulary size or update frequency grows, RAG becomes the safer default.)

---

### Q10. What is multi-hop / agentic RAG, and when is single-pass retrieval insufficient?

- **WHY:** Single-pass RAG retrieves once, generates once — insufficient when answering a question requires retrieving, reasoning about what's still missing, then retrieving again based on that intermediate reasoning.
- **HOW:** An agent loop: retrieve → assess whether the retrieved context is sufficient to answer → if not, formulate a follow-up query based on what's missing → retrieve again → repeat until sufficient or a step limit is hit → generate final answer.
- **WHEN:** Questions requiring information gathered across multiple, not-obviously-related documents where the follow-up query couldn't have been known in advance (as opposed to GraphRAG, which pre-computes relationships; agentic RAG discovers the path at query time).
- **SCALE:** Each hop is a full retrieval + reasoning round-trip — cost and latency scale roughly linearly with hop count, and without a hard step limit, a poorly-bounded agent loop can spiral into excessive cost on a single query.
- **DEPLOY:** Applies at every deployment tier identically, but the added latency/cost per query makes this a poor fit for latency-sensitive real-time endpoints — better suited to async/batch-style query patterns where multi-second-to-minute response time is acceptable.

**Follow-up probe:** "How do you prevent an agentic RAG loop from spiraling into unbounded cost on an unusually ambiguous query?" (Hard cap on hop count, a cost/token budget per query enforced at the orchestration layer, and a fallback to 'best answer with available context' rather than an unbounded retry loop.)

---

### Q11. How do you handle structured data (tables, forms) in a RAG pipeline that's otherwise built for prose?

- **WHY:** Naive text-chunking mangles tabular structure — splitting rows from headers, or losing the row/column relationship that gives a cell its meaning — producing chunks that are syntactically present but semantically useless.
- **HOW:** Extract tables as a distinct content type during ingestion (e.g., via Document Intelligence's Layout model), preserve them as structured units (markdown tables, or row-level records with explicit column context repeated per row) rather than flattening to plain text, and consider a separate retrieval path or embedding strategy for tabular vs prose content.
- **WHEN:** Any corpus with meaningful tabular content (financial statements, pricing tables, structured forms) — this is exactly the DealerFormExtractor/ChunkingStrategy territory in JMA's DealerIntelligence platform code.
- **SCALE:** Table-aware extraction adds ingestion-time cost (structure-preserving parsing is more expensive than plain text chunking) — justified by the alternative being silently wrong answers on any table-derived question, which is a worse outcome than the extra cost.
- **DEPLOY:** Not deployment-topology-specific, but table-extraction models/pipelines need the same regional-availability confirmation as any other Document Intelligence component in a multi-region design.

**Follow-up probe:** "A user asks 'what's the total for dealer code X' and the table was flattened to plain text during chunking — what happened and how do you prevent it?" (Row/column relationships were lost in flattening, so the retrieved chunk no longer reliably associates the dealer code with its corresponding total; prevent by preserving table structure explicitly during extraction/chunking, e.g., markdown table format or per-row records with repeated column headers.)

---

### Q12. How do you keep a RAG index fresh as source documents change, without over-engineering it?

- **WHY:** A stale index gives confidently wrong answers from outdated documents — RAG's grounding guarantee is only as good as the index's freshness relative to the source of truth.
- **HOW:** Event-driven re-indexing (source change triggers a targeted re-embed/re-index of just the changed document) is ideal; scheduled batch re-indexing is a simpler fallback when event triggers aren't available from the source system.
- **WHEN:** Event-driven for sources with a native change feed/webhook (Blob storage events, database change tracking). Scheduled batch when the source is static exports or lacks change notification — accept the resulting staleness window explicitly rather than pretending it's real-time.
- **SCALE:** Event-driven, targeted re-indexing scales far better than full-corpus batch re-indexing as corpus size grows — a full nightly re-index of a large, mostly-unchanged corpus wastes compute on unchanged documents.
- **DEPLOY:** In multi-region designs with regional index replicas, freshness needs a defined propagation SLA across regions, not just freshness at the primary write location — a document updated in one region shouldn't silently stay stale in another.

**Follow-up probe:** "A user complains they got an answer based on a policy that was updated yesterday — where do you look first?" (Check whether the source change actually triggered re-indexing (event delivery failure is common), then check propagation lag if multi-region, before assuming it's a retrieval or generation problem — freshness bugs are often silent pipeline failures, not model issues.)

---

### Q13. How do you evaluate RAG quality — what actually gets measured?

- **WHY:** "It looks right" isn't a metric — RAG has at least two independently-failing stages (retrieval, generation), and evaluation needs to isolate which stage is underperforming, not just score the final answer.
- **HOW:** Retrieval metrics (precision/recall of relevant chunks against a labeled golden set, NDCG/MRR for ranking quality) evaluated separately from generation metrics (faithfulness/groundedness to retrieved context, answer relevance to the question, and correctness against a reference answer) — plus end-to-end metrics (does the final answer satisfy the user).
- **WHEN:** Build a golden evaluation dataset (representative questions + known-correct answers + known-relevant source chunks) before shipping to production, and re-run it on every meaningful pipeline change (chunking strategy, embedding model, prompt) — this is what catches regressions before users do.
- **SCALE:** Golden dataset size and evaluation run frequency both need to scale with how often the pipeline changes — a large, rarely-updated golden set is fine for a stable pipeline; frequent changes need either a larger set or automated LLM-as-judge evaluation to keep pace without manual labeling bottlenecks.
- **DEPLOY:** Evaluation should be run per-region if content or query patterns differ meaningfully by region/market in a global deployment — a single global golden set can mask region-specific quality regressions.

**Follow-up probe:** "Your end-to-end answer-quality metric drops after a change — how do you determine if it's the retrieval or generation stage that regressed?" (Re-run retrieval-only metrics against the golden set in isolation — if retrieval precision/recall are unchanged but end-to-end quality dropped, the regression is in generation (prompt, model, or grounding instruction); if retrieval metrics also dropped, the regression is upstream in chunking/embedding/indexing.)

---

### Q14. What is query rewriting/expansion, and why would you add it before retrieval?

- **WHY:** User queries are often short, ambiguous, or phrased very differently from how the answer is worded in source documents (the vocabulary-mismatch problem) — rewriting bridges that gap before retrieval even runs.
- **HOW:** An LLM call (or lighter rule-based system) reformulates the raw user query into one or more retrieval-optimized queries — expanding abbreviations, adding likely synonyms, or decomposing a compound question into sub-queries retrieved separately.
- **WHEN:** When query logs show retrieval consistently missing relevant chunks for real user phrasing despite the content existing in the index — a diagnostic signal, not a default add-on for every pipeline.
- **SCALE:** Adds an LLM call (cost + latency) before retrieval even begins — for high-volume, latency-sensitive endpoints this cost needs to be justified by a measured retrieval-quality lift, not assumed.
- **DEPLOY:** Query rewriting logic/model needs to be region-appropriate if user query language/phrasing varies by market in a global deployment — a rewriter tuned on English query patterns won't necessarily help (and could hurt) in other languages.

**Follow-up probe:** "Adding query rewriting improved retrieval recall but end-to-end latency now exceeds SLA — what's the resolution path?" (Measure whether the retrieval-quality lift is large enough to justify the latency cost for this use case; if yes, look at making the rewrite step cheaper/faster (smaller model, caching common rewrites) rather than removing it outright; if the lift is marginal, cut it.)

---

### Q15. How do you handle multi-tenant access control in retrieval — preventing Tenant A's query from surfacing Tenant B's documents?

- **WHY:** A shared vector index across tenants is a real data-leakage risk if access control isn't enforced at the retrieval layer itself — application-layer filtering *after* retrieval is not sufficient if the retrieved-but-filtered content ever gets logged, cached, or leaks through a bug.
- **HOW:** Metadata-filtered retrieval — every document tagged with tenant/access-scope metadata at index time, and every query enforces that filter as part of the retrieval call itself (not a post-retrieval filter step), so cross-tenant documents are never even candidates.
- **WHEN:** Any multi-tenant RAG system, full stop — this isn't an optimization, it's a security requirement from day one of a multi-tenant design.
- **SCALE:** Metadata filtering at the index/query level scales fine with tenant count using Azure AI Search's native filter support; what doesn't scale well is retrofitting this after tenants have already been onboarded to an unfiltered shared index.
- **DEPLOY:** In multi-region deployments, tenant data-residency requirements (a tenant's data must stay in a specific region) compound with access-control filtering — you may need per-region tenant-scoped indexes, not just filtered access to one global index.

**Follow-up probe:** "A security review finds retrieval filtering happens in application code after the search call, not in the search query itself — why does that matter, and what's the fix?" (Post-retrieval filtering means unauthorized documents were still retrieved from the index and exist in application memory/logs momentarily — real risk even if never shown to the user; fix is enforcing the tenant filter as part of the Azure AI Search query itself, so unauthorized documents are never returned by the index at all.)

---

### Q16. Do you chunk a 3-page document and a 300-page document the same way?

- **WHY:** Short documents may fit entirely within a reasonable context window as a single unit (no chunking needed, or minimal chunking) — long documents structurally require chunking to be retrievable at all, and their internal structure (chapters, sections) offers natural chunk boundaries short documents don't have.
- **HOW:** For short documents, consider whole-document embedding/retrieval (or very coarse chunking) to preserve full context. For long documents, hierarchical chunking (section-level summary embeddings for coarse retrieval, paragraph-level chunks for fine-grained retrieval within a matched section) often outperforms flat uniform chunking.
- **WHEN:** Document-length-aware chunking strategy selection at ingestion time, not a single fixed chunk size applied blindly across a corpus with highly variable document lengths.
- **SCALE:** Hierarchical chunking adds ingestion-time complexity (multiple embedding levels per long document) but improves both retrieval precision and reduces the 'lost in the middle' risk from over-large single chunks — worth it for corpora dominated by long documents.
- **DEPLOY:** Not deployment-topology-specific — this is a corpus-characteristics decision made at ingestion design time, applied consistently regardless of region.

**Follow-up probe:** "Your corpus mixes 1-page memos and 200-page policy manuals under one uniform chunking config — what's likely going wrong?" (The uniform chunk size is a compromise that serves neither well — likely over-chunking short memos into meaningless fragments, or under-representing long manuals' internal structure; the fix is length-aware or hierarchical chunking, not a single 'better' fixed chunk size.)

---

### Q17. Where does RAG cost actually accumulate, and what's the highest-leverage optimization?

- **WHY:** RAG cost isn't just the final generation call — it's embedding cost (ingestion + every query embedding), storage cost (vector index size), retrieval/search cost, optional reranking cost, and the generation call's input-token cost (which scales with how much retrieved context gets stuffed into the prompt).
- **HOW:** Profile each stage's cost contribution separately rather than assuming generation dominates — for high-query-volume, low-document-churn systems, per-query embedding + search cost can rival or exceed generation cost.
- **WHEN:** Cost-optimize once you have real production volume data showing where cost actually concentrates — optimizing chunk size or embedding dimensionality before you know your actual query/ingestion volume ratio is guesswork.
- **SCALE:** The highest-leverage lever shifts with scale: at low volume, generation cost dominates (fixed context-stuffing per rare query); at high volume, the per-query embedding + search cost (which happens on every single query, even repeated ones) becomes the larger line item — caching frequent-query embeddings/results is often the highest-leverage optimization at that point.
- **DEPLOY:** Multi-region deployments multiply storage cost (index replicated per region) and can multiply embedding cost if ingestion isn't centralized — centralize embedding/ingestion once, replicate the resulting index, rather than re-embedding the same source content independently per region.

**Follow-up probe:** "Cost review flags RAG spend has tripled with only 20% more query volume growth — what do you check first?" (Check whether context-assembly is stuffing more/larger chunks per query than before — Modules 1 & 2's context-window cost point applies directly here — before assuming it's a volume-driven cost; a chunking or retrieval-count regression is a more likely culprit than proportional growth.)

---

### Q18. Walk through your end-to-end RAG architecture as if this were the "centerpiece" interview question.

- **WHY:** This is the question every GenAI Architect interview converges on — it tests whether you can hold the entire pipeline coherently, not just answer isolated sub-questions.
- **HOW:** Ingestion (Document Intelligence extraction → structure-aware chunking → embedding → indexing with tenant/access metadata) → Query time (query rewriting if needed → hybrid search → reranking → citation-structured prompt assembly → generation with low temperature → post-generation groundedness/Content Safety check) → Observability (retrieval + generation metrics logged per query, golden-set evaluation run on every pipeline change).
- **WHEN:** This is the answer to have rehearsed and ready — 4-5 minutes, structured, delivered from memory, anchored to a real production example (JMA's Document Intelligence → AI Search → EnterpriseSearch.Sync pipeline is the natural anchor).
- **SCALE:** Call out explicitly where the pipeline's cost/latency levers are (embedding, reranking, context size) and how you'd scale each independently as volume grows.
- **DEPLOY:** Close by walking the deployment ladder explicitly — local/dev, single-region production, and what multi-region/global would require (index replication strategy, data residency, DR) — this is exactly the Module 5 deployment-topology framework applied to this specific pipeline.

**Follow-up probe:** "Where in this pipeline would a hallucination most likely slip through despite every safeguard you just described?" (Honest answer: a case where retrieval genuinely returns plausible-but-wrong context — e.g., an outdated document that wasn't yet re-indexed after a source change — the model then generates a well-grounded-*looking* answer that's grounded in stale truth, which groundedness detection won't catch since it checks support against provided context, not real-world correctness. This is the answer that shows real architectural maturity, not just pipeline recitation.)

---

*Module 3 of 6 — GenAI Architect Interview Prep. Next: Module 4 — Agent Orchestration.*
