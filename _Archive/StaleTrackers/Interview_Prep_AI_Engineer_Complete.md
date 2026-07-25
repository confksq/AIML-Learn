# Interview Prep — Senior AI Engineer — 100 Questions
**PRD Feature 12** · Flashcard format for drilling.

Each entry: **Difficulty** · concise model answer · **Key terms** to say out loud · **Follow-up** they may ask · **Bala's example** (a real JMA/KPMG anchor to make it concrete).

> Format note: answers are deliberately tight (2–4 sentences) for rapid review. For deep dives, see the curriculum Q&A (`PartsModules/Questions/`) and the 126-question interview bank (`Questions/01–06`).

**Distribution:** RAG (15) · Azure AI Foundry/OpenAI (15) · Agents (15) · LLMOps/Eval (10) · Vector DBs/Embeddings (10) · Prompt Engineering (10) · Open-Source/HF (8) · Fine-tuning (7) · Safety/Responsible AI (5) · System Design (5).

---

## A. RAG Architecture & Design (15)

**1. What are the two independent failure points in a RAG pipeline?** · *Medium*
Retrieval (wrong/missing chunks) and generation (model ignores or misreads the context). Diagnosing which is failing is step one — no prompt fix helps a retrieval miss.
**Key terms:** retrieval vs generation, grounding, faithfulness. **Follow-up:** How do you tell which one failed? **Bala:** At JMA I inspect retrieved chunks per query before touching the prompt.

**2. Walk through an end-to-end RAG pipeline.** · *Easy*
Ingest → chunk → embed → index; then query → embed → hybrid retrieve → re-rank → assemble grounded prompt → generate → post-check groundedness/citations.
**Key terms:** ingestion vs query pipeline, hybrid search, re-ranking. **Follow-up:** Where does cost concentrate? **Bala:** JMA's Document Intelligence → AI Search → Azure OpenAI pipeline.

**3. How do you choose a chunking strategy?** · *Medium*
Recursive/structure-aware by default (512-ish tokens, 10–20% overlap); preserve tables and headings; parent-child for hierarchical docs. Tune against retrieval eval, not by guessing.
**Key terms:** recursive splitting, overlap, parent-child, semantic chunking. **Follow-up:** Fix a table split mid-row? **Bala:** JMA dealer forms use table-aware chunking.

**4. What is hybrid search and why use it?** · *Medium*
BM25 keyword + vector fused (RRF). Keyword catches exact terms (IDs/codes), vector catches paraphrases; together they beat either alone. Default for enterprise RAG.
**Key terms:** BM25, RRF, dense+sparse. **Follow-up:** When is keyword-only right? **Bala:** JMA's prod index is keyword-only; staging adds vectors.

**5. What is re-ranking and when do you add it?** · *Medium*
A cross-encoder re-scores the top-N candidates more accurately than the bi-encoder retriever. Retrieve wide (recall) cheaply, then narrow with the reranker (precision). Azure's semantic ranker is a managed one.
**Key terms:** bi-encoder vs cross-encoder, semantic ranker. **Follow-up:** Latency cost? **Bala:** Semantic ranker on JMA search.

**6. RAG answer is wrong but the doc exists in the index — diagnose.** · *Hard*
Either retrieval missed the chunk (embedding/chunking) or it was retrieved but the model ignored it. Check retrieved chunks first; if present, it's a generation/grounding problem.
**Key terms:** retrieval miss vs generation override. **Follow-up:** Right chunk retrieved, still wrong? **Bala:** Standard JMA debug order.

**7. What is "lost in the middle"?** · *Medium*
LLMs attend less reliably to info in the middle of a long context. So retrieve fewer, higher-relevance chunks and place critical content at the start or end.
**Key terms:** context positioning, attention. **Follow-up:** How does it change top-K? **Bala:** JMA prompts put key chunks first.

**8. When does GraphRAG beat vector RAG?** · *Hard*
Multi-hop relationship questions and global corpus synthesis ("which dealers share a manager handling late accounts"). Vector similarity can't traverse relationships; a knowledge graph can.
**Key terms:** knowledge graph, multi-hop, community summaries. **Follow-up:** Cost trade-off? **Bala:** Built a Neo4j GraphRAG portfolio module (L7).

**9. RAG vs fine-tuning vs prompting — decide.** · *Medium*
Prompting first; RAG for knowledge (current/private/large); fine-tuning for behavior/format/tone. "Fine-tune for behavior, RAG for knowledge."
**Key terms:** knowledge vs behavior problem. **Follow-up:** When both? **Bala:** JMA uses RAG for policy + would fine-tune only for JSON format.

**10. How do you handle multi-turn RAG (follow-up questions)?** · *Medium*
Query rewriting: rewrite "which of those are over $40k?" into a standalone query using conversation history before retrieving.
**Key terms:** query rewriting, coreference. **Follow-up:** Why not just embed the raw follow-up? **Bala:** JMA chat resolves "that dealer" via rewrite.

**11. How do you force citations and grounding?** · *Medium*
Tag each chunk `[Source N]`, instruct the model to cite per claim, and validate cited IDs exist post-generation (catches fabricated citations). Low temperature.
**Key terms:** citation validation, groundedness detection. **Follow-up:** Cites Source 3 but only 2 retrieved? **Bala:** JMA returns source refs with every answer.

**12. What is query rewriting / HyDE?** · *Hard*
Rewriting reformulates the query for better retrieval; HyDE embeds a hypothetical *answer* (in doc-style language) instead of the question, improving recall when query/doc language diverges.
**Key terms:** HyDE, multi-query, vocabulary mismatch. **Follow-up:** HyDE's downside? **Bala:** Portfolio RAG modules demonstrate both.

**13. How do you keep a RAG index fresh?** · *Medium*
Event-driven re-indexing on source change (Blob event → function → re-embed the changed doc); scheduled batch as fallback. Stale index = confidently wrong answers.
**Key terms:** incremental indexing, change feed, freshness SLA. **Follow-up:** Multi-region freshness? **Bala:** JMA EnterpriseSearch.Sync push pipeline.

**14. What is CAG and how does it differ from RAG?** · *Hard*
Cache-Augmented Generation precomputes the model's KV cache over a small, static knowledge base — no per-query retrieval. Use when the corpus fits context and rarely changes; RAG for large/changing corpora.
**Key terms:** KV cache, context stuffing, cache invalidation. **Follow-up:** Weekly-updating corpus? **Bala:** Covered in Ascendion prep (CAG vs RAG).

**15. How do you evaluate a RAG pipeline?** · *Medium*
Separate retrieval metrics (precision/recall, NDCG) from generation metrics (faithfulness, answer relevance) — RAGAS or Azure AI Foundry evaluators against a golden dataset.
**Key terms:** RAGAS, faithfulness, context recall/precision. **Follow-up:** End-to-end quality dropped — which stage? **Bala:** Built a RAGAS module (L3).

---

## B. Azure AI Foundry & Azure OpenAI (15)

**16. Azure OpenAI vs OpenAI direct — why enterprise picks Azure.** · *Easy*
Data residency in your region, enterprise compliance (HIPAA/ISO/FedRAMP), Azure AD/Managed Identity + Private Link, and your data is never used to train OpenAI's models.
**Key terms:** data residency, Managed Identity, Private Link. **Follow-up:** Which compliance certs? **Bala:** JMA runs Azure OpenAI for exactly these reasons.

**17. What is a deployment in Azure OpenAI?** · *Easy*
Your named instance of a model with a pinned version and a TPM quota. You call the deployment, not the model directly.
**Key terms:** deployment, TPM, model version. **Follow-up:** Why pin the version? **Bala:** JMA has separate dev/prod deployments.

**18. What is Azure AI Foundry?** · *Easy*
Microsoft's unified GenAI platform (ai.azure.com): model catalog, Prompt Flow, evaluation, fine-tuning, Content Safety, tracing, and agents — replacing the old Azure OpenAI Studio.
**Key terms:** model catalog, Prompt Flow, evaluation. **Follow-up:** Foundry vs Semantic Kernel? **Bala:** Prototype in Foundry, productionize in SK at JMA.

**19. PTU vs pay-as-you-go.** · *Medium*
PTU reserves dedicated throughput (guaranteed latency, fixed cost) for predictable high volume; PAYG bills per token on shared capacity for variable load. Often PTU baseline + PAYG overflow.
**Key terms:** provisioned throughput, TPM, break-even. **Follow-up:** Monday spike throttling? **Bala:** JMA sizes to baseline + overflow.

**20. How do you handle 429 throttling?** · *Medium*
Exponential backoff honoring Retry-After; more instances don't help (shared quota). Real fixes: quota increase, multiple deployments round-robined, or Service Bus pacing upstream.
**Key terms:** 429, Retry-After, quota. **Follow-up:** Team lead says "add instances." **Bala:** JMA queues DI calls to avoid throttling.

**21. What is "On Your Data"?** · *Medium*
Azure OpenAI's built-in managed RAG over Azure AI Search — pass a data_sources config and Azure does retrieval + prompt assembly. Fast; custom RAG for more control.
**Key terms:** managed RAG, data_sources, strictness. **Follow-up:** When custom RAG instead? **Bala:** JMA uses custom RAG for chunking control.

**22. finish_reason = length — what happened?** · *Easy*
The response hit max_tokens and was cut off. Increase max_tokens or shorten the context/prompt.
**Key terms:** max_tokens, truncation. **Follow-up:** finish_reason=content_filter? **Bala:** Monitored in JMA App Insights.

**23. Function calling — who executes the function?** · *Medium*
The model decides *what* to call and with *what* arguments; your code decides *whether* to execute (validate args, check permissions, log). Never blind-execute.
**Key terms:** tool calls, argument validation. **Follow-up:** Hallucinated argument? **Bala:** JMA validates dealer codes before executing.

**24. What are Structured Outputs?** · *Medium*
A JSON-schema mode where GPT-4o guarantees output matching your schema — replaces unreliable "respond in JSON" prompting for production extraction.
**Key terms:** json_schema, guaranteed valid JSON. **Follow-up:** vs json_object mode? **Bala:** JMA invoice extraction candidate.

**25. o1/o3 reasoning models — when and how do they differ in the API?** · *Hard*
For complex multi-step reasoning/math/code; they generate hidden chain-of-thought first. No temperature param; use max_completion_tokens. Overkill for simple classification.
**Key terms:** reasoning tokens, chain-of-thought. **Follow-up:** Don't add "think step by step" — why? **Bala:** JMA reserves o1 for complex analysis.

**26. Managed Identity vs API keys for Azure OpenAI.** · *Easy*
Managed Identity (Cognitive Services OpenAI User role via DefaultAzureCredential) — no secrets to rotate/leak, RBAC, audit trail. Keys only for local dev.
**Key terms:** DefaultAzureCredential, RBAC. **Follow-up:** Remediate a key-in-code finding? **Bala:** JMA Functions use Managed Identity.

**27. Production network posture for Azure OpenAI.** · *Medium*
Public access disabled, Private Endpoint with VNet integration, reachable only from the VNet — no public internet transit.
**Key terms:** Private Link, VNet, network isolation. **Follow-up:** DNS returns the public IP? **Bala:** Required for JMA financial docs.

**28. What's the Batch API for?** · *Easy*
Async processing of large jobs (up to ~thousands of completions) within 24h at ~50% cost — ideal for nightly classification/summarization.
**Key terms:** batch inference, 50% discount. **Follow-up:** Acceptable latency? **Bala:** JMA nightly ticket classification fit.

**29. GPT-4o vs GPT-4o-mini — how do you choose?** · *Medium*
mini is ~17x cheaper with near-equal quality on structured tasks; validate on GPT-4o, then test mini and route simple queries to it, complex to GPT-4o.
**Key terms:** model routing, cost tiering. **Follow-up:** "Use GPT-4o for everything" — counter? **Bala:** JMA routes by query complexity.

**30. How do you handle Azure OpenAI model deprecation?** · *Medium*
Deploy behind an abstraction (named deployment / routing layer), track Microsoft's 12-month deprecation notices, and verify the new version is available in every deployed region before cutover.
**Key terms:** deprecation timeline, version-agnostic routing. **Follow-up:** New version missing in one region? **Bala:** Abstraction layer in JMA services.

---

## C. AI Agents & Agentic AI (15)

**31. RAG vs function calling vs agent — distinguish.** · *Medium*
RAG = fixed retrieve-generate pipeline. Function calling = one tool decision. Agent = dynamic multi-step planner that calls tools, observes, adapts, loops.
**Key terms:** ReAct, orchestration, complexity spectrum. **Follow-up:** When is an agent overkill? **Bala:** JMA uses agents only for multi-step tasks.

**32. Explain the ReAct pattern.** · *Medium*
Reason → Act (tool call) → Observe → repeat until confident. Interleaving lets each step adapt to prior results, unlike single-shot planning.
**Key terms:** thought-action-observation loop. **Follow-up:** Stuck at 12 steps — fix? **Bala:** JMA InvoiceAgent designed this way.

**33. Semantic Kernel — Plugin vs KernelFunction.** · *Easy*
A Plugin is a class grouping related functions (the toolbox); a KernelFunction is one `[KernelFunction]`-decorated method the LLM can call (one tool).
**Key terms:** AutoInvokeKernelFunctions, plugins. **Follow-up:** How does the LLM pick a tool? **Bala:** JMA plugins: ClaimDecision, DealerEligibility, PolicyLookup.

**34. When do you need multiple agents vs one agent with tools?** · *Hard*
Multi-agent when sub-tasks need different expertise/framing, can parallelize, or need isolated context — not because it "sounds sophisticated." Earn the complexity.
**Key terms:** supervisor/specialist, coordination overhead. **Follow-up:** Team wants 5 agents for a 3-tool task. **Bala:** JMA SupervisorAgent + specialists.

**35. Design a supervisor/orchestrator multi-agent pattern.** · *Medium*
A supervisor delegates sub-tasks to specialists (sequential or parallel), collects results, and synthesizes/decides — with an explicit conflict-resolution policy.
**Key terms:** delegation, synthesis, precedence. **Follow-up:** Two specialists disagree? **Bala:** JMA ClaimValidator/FraudDetector/PolicyChecker under a supervisor.

**36. What is agentic hallucination?** · *Hard*
Not a wrong statement but a wrong *action* — a fabricated tool call or hallucinated argument. Mitigate with schema+semantic arg validation, step caps, and human gates for irreversible actions.
**Key terms:** tool-call validation, human-in-the-loop. **Follow-up:** Agent hallucinated a drug interaction — respond. **Bala:** JMA AuditFilter guards this.

**37. How do you stop an agent from infinite-looping?** · *Medium*
Max iteration/step cap, token budget, a FunctionInvocationFilter that counts/logs calls, clear tool descriptions (ambiguity causes re-calls), and a "can't complete" fallback.
**Key terms:** iteration cap, guardrails. **Follow-up:** It's costing thousands/day. **Bala:** SK MaxTokens + safety filter at JMA.

**38. crewAI vs Semantic Kernel vs LangGraph.** · *Medium*
SK = enterprise .NET/Azure agents; crewAI = fast Python role-based crews; LangGraph = low-level stateful/cyclic graph control. Pick per language, control, and production needs.
**Key terms:** role/goal/backstory, Process, graph nodes. **Follow-up:** JD lists SK + crewAI. **Bala:** Built a crewAI portfolio module (L2) alongside SK at JMA.

**39. What is the MCP (Model Context Protocol)?** · *Medium*
A standard for agents to discover and call tools/data sources uniformly. An MCP Hub centralizes tool registration so many agents share tools without bespoke integration; often paired with APIM for auth/rate-limits.
**Key terms:** tool registry, MCP vs APIM. **Follow-up:** Hybrid MCP+APIM design? **Bala:** JMA MCPHub + APIMGateway pattern.

**40. What is A2A (agent-to-agent) protocol?** · *Medium*
A standard for independently-built agents to discover capabilities and exchange task requests — like MCP but agent-to-agent. Value is cross-team/vendor interop.
**Key terms:** capability card, schema validation. **Follow-up:** Custom bus already works — why A2A? **Bala:** JMA AgentBus for internal, A2A for cross-boundary.

**41. Agent memory — short-term vs long-term.** · *Medium*
Short-term = session/context (ChatHistory); long-term = vector-backed persisted facts retrieved into new sessions. Conflating them bloats context or loses continuity.
**Key terms:** ChatHistory, vector memory, summarization. **Follow-up:** Session 2 forgot session 1 — why? **Bala:** SK ChatHistory + AI Search memory.

**42. How do you manage context window in a long agent conversation?** · *Hard*
Summarize old turns (ChatHistorySummarizationReducer), keep recent verbatim, store key facts in long-term memory. Otherwise turn ~47 exceeds 128k and crashes.
**Key terms:** history reducer, compaction. **Follow-up:** What do you always keep? **Bala:** SK summarizing reducer for JMA dealer support.

**43. What is Agentic RAG?** · *Medium*
An agent decides *if*, *which index*, and *how many times* to retrieve — routing across multiple knowledge sources — vs standard RAG's fixed single-index pipeline.
**Key terms:** multi-index routing, tool-based retrieval. **Follow-up:** "Why flagged AND what's the policy?" **Bala:** JMA search_invoices + search_policies tools.

**44. Design prompt-injection defense for a tool-using agent.** · *Hard*
Layered: input shields, delimit untrusted content as data, least-privilege tool permissions (an injected "delete" fails if the agent lacks the tool), arg validation, human gates, monitoring.
**Key terms:** least privilege, Prompt Shields, indirect injection. **Follow-up:** One layer only — which? **Bala:** Least privilege on JMA agent tools.

**45. Streaming in Semantic Kernel — why and how?** · *Medium*
`GetStreamingChatMessageContentsAsync` returns an IAsyncEnumerable; push each token to the UI (SignalR) for ChatGPT-style responsiveness, then persist the full response.
**Key terms:** IAsyncEnumerable, SignalR. **Follow-up:** Streaming with tool calls? **Bala:** JMA dealer chat streams tokens.

---

## D. LLMOps, Evaluation & Monitoring (10)

**46. MLOps vs LLMOps.** · *Medium*
MLOps versions model binaries, evaluates accuracy/F1, retrains on data drift. LLMOps versions prompts+config, evaluates groundedness/relevance, "retrains" via prompt update or small fine-tune; drift comes from provider updates or stale docs.
**Key terms:** prompt versioning, quality gate. **Follow-up:** Where's the model file? **Bala:** JMA versions prompts in Git.

**47. Why version prompts, and how?** · *Medium*
Prompts change behavior as much as code. Store as versioned files/registry, review + eval before promote, pin per environment, log which version produced each response.
**Key terms:** .prompty, rollback, git history. **Follow-up:** Behavior changed last Tuesday — trace it. **Bala:** JMA PromptVersioning pattern.

**48. What's a golden dataset?** · *Easy*
~100+ expert-written question + ideal-answer (+ source) pairs, stratified across cases, used to measure quality and gate deploys. Refresh as traffic drifts.
**Key terms:** quality gate, ground truth. **Follow-up:** How big, who writes it? **Bala:** JMA invoice team writes the answers.

**49. Design an AI CI/CD quality gate.** · *Hard*
On prompt/model/retrieval change: run the golden set through RAGAS/Foundry evaluators; fail the build if faithfulness/relevance drop below threshold; blue-green with canary traffic.
**Key terms:** evaluation gate, canary, non-determinism. **Follow-up:** What thresholds? **Bala:** Foundry eval + PowerShell gate.

**50. What is LLM-as-judge and its risks?** · *Medium*
A model scores another's outputs against a rubric — scales evaluation. Risks: self-preference, verbosity, position, leniency bias. Calibrate against humans, pin the judge.
**Key terms:** critic model, calibration. **Follow-up:** Why pin the judge version? **Bala:** RAGAS module uses LLM-as-judge (L3).

**51. Three types of drift in production GenAI.** · *Hard*
Data/query drift (new question types), concept drift (world changes, docs stale), model drift (provider silently updates). Detect via query-embedding monitoring, groundedness drops, scheduled canary evals.
**Key terms:** data/concept/model drift. **Follow-up:** Groundedness dropped overnight, no code change. **Bala:** Weekly golden-set run at JMA.

**52. What do you monitor on a production LLM endpoint?** · *Medium*
Infra (latency p50/95/99, errors), AI-specific (tokens, 429s, Content Safety blocks), and quality (groundedness, user feedback, hallucination rate).
**Key terms:** App Insights custom metrics, token spend. **Follow-up:** Alert thresholds? **Bala:** JMA App Insights dashboards.

**53. How do you A/B test a prompt change?** · *Medium*
Route 10% traffic to v2 vs 90% v1, compare groundedness + user ratings over a window, promote if v2 wins. Feature-flag the prompt version per user.
**Key terms:** canary, feature flag. **Follow-up:** Metrics green but a ticket says worse? **Bala:** JMA prompt A/B via config.

**54. Model rollback — what must be true for it to work?** · *Hard*
Previous deployment still exists with quota; prompts versioned *with* model compatibility; no downstream depends on new-only output shape; embeddings understood as index rebuilds. Rollback unit = model+prompt+config.
**Key terms:** blue-green, compatibility. **Follow-up:** Roll back model, keep new prompt — problem? **Bala:** JMA holds prior deployment warm.

**55. What's the LLMOps maturity ladder?** · *Medium*
L0 manual → L1 versioned prompt + manual eval → L2 eval in CI/CD + dashboards → L3 A/B + automated drift detection + feedback loop. Target L2→L3.
**Key terms:** quality gate, feedback loop. **Follow-up:** Where are most teams? **Bala:** JMA targeting L2→L3.

---

## E. Vector Databases & Embeddings (10)

**56. What is an embedding and how is similarity measured?** · *Easy*
A dense vector where geometric proximity ≈ semantic proximity. Similarity via cosine (angle, magnitude-independent), not Euclidean.
**Key terms:** cosine similarity, dimensionality. **Follow-up:** Why cosine over Euclidean? **Bala:** JMA uses text-embedding-3.

**57. How does HNSW work?** · *Hard*
A layered navigable graph: greedily navigate from a sparse top layer down to dense bottom, collecting nearest neighbors — O(log n) vs O(n) brute force, ~99% recall (approximate).
**Key terms:** ANN, recall, m/efConstruction/efSearch. **Follow-up:** Flat vs HNSW? **Bala:** JMA AI Search HNSW config.

**58. Azure AI Search vs a dedicated vector DB (Qdrant/Pinecone).** · *Medium*
AI Search: native hybrid + semantic ranker + Azure integration + metadata filtering. Pick Cosmos vector if data already lives there; Qdrant for pure vector perf; never Pinecone for Azure-primary (data leaves Azure).
**Key terms:** hybrid, managed, data residency. **Follow-up:** JMA choice? **Bala:** srch-jma-prod-indexer on AI Search.

**59. Can you switch embedding models on an existing index?** · *Medium*
No — different models produce incomparable vectors and often different dimensions. Requires full re-embedding + re-index (or dual-index cutover).
**Key terms:** re-embedding, dimension mismatch. **Follow-up:** Colleague wants 3-large → 3-small. **Bala:** Hard constraint in JMA pipeline.

**60. Chunk size and overlap — how do you pick?** · *Medium*
512 tokens, 10–20% overlap as a start; smaller = precise but risks lost context, larger = context but noisier vectors. Tune against retrieval eval.
**Key terms:** overlap, precision/recall trade. **Follow-up:** Halving chunk size cost? **Bala:** JMA tunes per doc type.

**61. What's parent-child (small-to-big) retrieval?** · *Hard*
Index small child chunks (precise vectors), but return the larger parent to the LLM (full context). Resolves the precision/recall trade-off.
**Key terms:** hierarchical retrieval, node relationships. **Follow-up:** LlamaIndex support? **Bala:** LlamaIndex module (L5).

**62. What is Matryoshka / dimension truncation?** · *Medium*
text-embedding-3 models let you request fewer dimensions (256/512/1536) from the same model — ~6x storage savings at ~3% accuracy loss. A cost lever at scale.
**Key terms:** MRL, storage cost. **Follow-up:** When truncate? **Bala:** Large JMA index candidate.

**63. What is vector quantization?** · *Medium*
Compress stored vectors: scalar quantization (4x smaller, ~1% loss), binary (32x, ~5%). Critical cost control for large indexes.
**Key terms:** scalar/binary quantization. **Follow-up:** Trade-off? **Bala:** AI Search quantization for cost.

**64. Multi-tenant vector retrieval — prevent cross-tenant leakage.** · *Hard*
Enforce tenant filter *in the search query* (not app-side after retrieval) so other tenants' docs are never candidates. Post-retrieval filtering still exposes data in memory/logs.
**Key terms:** metadata filter, row-level security. **Follow-up:** Filter's in app code — why bad? **Bala:** JMA filters by department in the query.

**65. Embeddings inside the LLM vs the Embeddings API — same thing?** · *Medium*
No. The internal embedding layer converts token IDs for the model's own processing (black box); the Embeddings API is a separate model for retrieval. Different weights, different jobs.
**Key terms:** internal vs external embeddings. **Follow-up:** Interchangeable? **Bala:** Clarified in curriculum L11_2.

---

## F. Prompt Engineering (10)

**66. Zero-shot vs few-shot vs CoT.** · *Easy*
Zero-shot = instructions only; few-shot = 2–5 examples for format; CoT = "think step by step" for complex reasoning. Escalate as the task needs.
**Key terms:** in-context learning, reasoning. **Follow-up:** 200+ examples? **Bala:** Then fine-tune, not few-shot.

**67. Why does Chain-of-Thought improve accuracy?** · *Medium*
It forces the model to process each step before concluding, making errors visible and improving the next-token quality on multi-step logic/math.
**Key terms:** step-by-step, reasoning trace. **Follow-up:** o1 models — add CoT? **Bala:** No — they reason internally.

**68. What five things does a system prompt control?** · *Easy*
Persona, scope (can do), constraints (cannot do), format, and fallback (what to do when it doesn't know).
**Key terms:** persona, fallback, injection defense. **Follow-up:** 500-line system prompt problems? **Bala:** JMA keeps prompts <200 tokens.

**69. Temperature — how do you set it by task?** · *Easy*
0–0.3 for factual/extraction/RAG/classification; 0.7–1.0 for creative drafting; never >1.0 in production.
**Key terms:** determinism, sampling. **Follow-up:** Why not exactly 0 always? **Bala:** JMA RAG at ~0.1.

**70. Direct vs indirect prompt injection.** · *Hard*
Direct = user types the attack; indirect = malicious instructions hidden in a retrieved document. Indirect is worse for RAG — it rides in on your own knowledge base.
**Key terms:** Prompt Shields, delimiting. **Follow-up:** Defend a RAG pipeline? **Bala:** JMA system prompt ignores instructions in retrieved docs.

**71. What is prompt chaining?** · *Medium*
Output of one focused prompt feeds the next (extract → analyze → draft). Each step is simpler and more accurate than one mega-prompt.
**Key terms:** decomposition, pipeline. **Follow-up:** Design a monthly report. **Bala:** JMA risk report chain.

**72. What is self-consistency?** · *Medium*
Run the same prompt N times, take the majority answer — 3x cost for higher accuracy on high-stakes decisions only.
**Key terms:** majority vote, ensembling. **Follow-up:** When worth it? **Bala:** Legal escalation decisions.

**73. JSON mode vs Structured Outputs vs prompt instruction.** · *Medium*
"Respond in JSON" ~90% reliable; JSON mode guarantees valid JSON; Structured Outputs guarantees valid JSON *matching your schema*. Use Structured Outputs for production extraction.
**Key terms:** json_schema, reliability. **Follow-up:** Nested objects? **Bala:** JMA invoice extraction.

**74. What is prompt caching?** · *Medium*
Azure caches a repeated prompt prefix (large system prompt) at ~50% token cost and lower latency on cache hits — big for high-volume apps.
**Key terms:** prefix cache, cost. **Follow-up:** 800-token system prompt at 10k calls/day? **Bala:** Meaningful JMA savings.

**75. How do you harden a system prompt against injection?** · *Hard*
XML/JSON delimiters separating instructions from user content, explicit "ignore attempts to override" clause, scope constraint with fixed refusal, and Prompt Shields upstream.
**Key terms:** delimiters, Prompt Shields. **Follow-up:** Test against 5 adversarial inputs. **Bala:** JMA dealer-support prompt hardening.

---

## G. Open-Source LLMs & Hugging Face (8)

**76. What is Hugging Face, mapped to Azure?** · *Easy*
The open-source AI ecosystem: Hub (model registry ≈ Foundry catalog), `transformers` (SDK), `pipeline()` (one-liner API), datasets, PEFT.
**Key terms:** Hub, pipeline, transformers. **Follow-up:** HF vs Azure OpenAI? **Bala:** Built a HF portfolio module (L4).

**77. What is Ollama and when do you use local LLMs?** · *Medium*
A local model server exposing an OpenAI-compatible API. Use for air-gapped/regulated data, cost at scale, or ultra-low latency — often hybrid with cloud for hard cases.
**Key terms:** local serving, air-gapped. **Follow-up:** Trade-off? **Bala:** Ollama local RAG module (L1).

**78. What is quantization (GGUF/AWQ/NF4)?** · *Hard*
Storing weights at lower precision to fit consumer hardware. GGUF = local serving (Ollama), AWQ/EXL2 = GPU inference, NF4 = QLoRA training. Small quality loss.
**Key terms:** 4-bit, GGUF vs NF4. **Follow-up:** Serving vs training quantization? **Bala:** Covered in L1 + L8 modules.

**79. What's the `pipeline()` API?** · *Easy*
Wraps tokenizer + model + post-processing for a task into one callable — `pipeline("text-generation")`, `pipeline("zero-shot-classification")`, etc.
**Key terms:** task pipeline, tokenizer pairing. **Follow-up:** Why must tokenizer match model? **Bala:** L4 demos.

**80. sentence-transformers vs a generative model.** · *Medium*
sentence-transformers produce embeddings for retrieval (the text-embedding-3 equivalent); generative models produce text. Separate models, separate jobs — RAG uses both.
**Key terms:** embeddings vs generation. **Follow-up:** Build local RAG with only HF? **Bala:** L4 `04d` demo.

**81. What is a gated model?** · *Easy*
A model (Llama, some Mistral) requiring license acceptance + an HF token before download; otherwise a 403. Tracked as part of model governance.
**Key terms:** license, HF token. **Follow-up:** Enterprise governance angle? **Bala:** Noted in L4.

**82. When Hugging Face over Azure OpenAI?** · *Medium*
Open-source models you can run on-prem or fine-tune freely, specialized task models (NER/ASR/zero-shot), or sourcing a model for air-gapped serving. Azure for managed frontier quality.
**Key terms:** control vs managed. **Follow-up:** Hybrid? **Bala:** HF embeddings + Azure OpenAI generation.

**83. LangChain vs LlamaIndex.** · *Medium*
LangChain = general orchestration (chains/agents/tools); LlamaIndex = RAG-specialized (better indexing abstractions, citations with less code). Compose them; SK for enterprise .NET.
**Key terms:** RAG-specialized, VectorStoreIndex. **Follow-up:** When LlamaIndex? **Bala:** Built a LlamaIndex module (L5).

---

## H. Fine-tuning (LoRA/QLoRA) (7)

**84. When do you fine-tune vs RAG vs prompt?** · *Medium*
Fine-tune for behavior/format/tone/vocabulary; RAG for knowledge; prompt first. Only fine-tune with 100+ clean examples that prompting can't match.
**Key terms:** behavior vs knowledge. **Follow-up:** Fine-tune on changing policy? **Bala:** No — that's RAG.

**85. What does LoRA do differently from full fine-tuning?** · *Hard*
Freezes the base, trains two small matrices A×B (~0.1–1% of params) approximating the weight change. Same quality within a few %, ~95% less memory, few-MB adapter.
**Key terms:** low-rank, frozen base, adapter. **Follow-up:** Explain the math plainly. **Bala:** Built a LoRA notebook (L8).

**86. What is QLoRA?** · *Hard*
LoRA + 4-bit (NF4) quantization of the frozen base, ~3x less memory — fine-tune a 7B model on a free Colab T4. Adapters stay higher precision.
**Key terms:** NF4, bitsandbytes. **Follow-up:** Why no quality loss? **Bala:** L8 QLoRA reference.

**87. Key LoRA hyperparameters.** · *Medium*
r (rank, ~8), lora_alpha (~2×r), target_modules (q_proj/v_proj), lora_dropout (0.05).
**Key terms:** rank, alpha, target modules. **Follow-up:** Raise r when? **Bala:** L8 config.

**88. LoRA/QLoRA vs Azure OpenAI fine-tuning.** · *Medium*
PEFT for open-source models (own the adapter, control everything, run on Colab/Azure ML); Azure OpenAI managed fine-tuning for GPT-4o (hosted, pay per token).
**Key terms:** PEFT, managed. **Follow-up:** JMA JSON extraction path? **Bala:** Azure OpenAI FT for GPT-4o-mini.

**89. How do you detect overfitting in fine-tuning?** · *Medium*
Training loss falls while validation loss rises = memorizing. Fix: fewer epochs, more/varied data, lower LR.
**Key terms:** validation loss, epochs. **Follow-up:** Tiny dataset risk? **Bala:** L8 loss curve.

**90. What is catastrophic forgetting; how does LoRA help?** · *Hard*
Fine-tuning degrading general capability. LoRA mitigates by freezing the base (adding an adapter, not overwriting pretrained knowledge) — and enables per-task adapter swapping / multi-LoRA serving.
**Key terms:** frozen base, multi-LoRA. **Follow-up:** Serve many fine-tunes cheaply? **Bala:** Adapter-per-task.

---

## I. AI Safety & Responsible AI (5)

**91. Azure Content Safety vs groundedness detection.** · *Medium*
Content Safety filters harmful content (hate/violence/sexual/self-harm) by severity; groundedness detection checks whether an answer is supported by the retrieved context (hallucination). Separate concerns, both needed.
**Key terms:** severity levels, Prompt Shields, groundedness. **Follow-up:** Grounded but flagged? **Bala:** JMA runs both.

**92. Name Microsoft's six Responsible AI principles.** · *Easy*
Fairness, Reliability & Safety, Privacy & Security, Inclusiveness, Transparency, Accountability.
**Key terms:** RAI framework. **Follow-up:** Map one to a design action. **Bala:** JMA logs AI decisions (Accountability).

**93. PII handling across a GenAI pipeline.** · *Hard*
Detect/redact at ingestion, minimize PII sent to the model, and — the trap — redact PII in logs/traces (observability becomes an unmanaged PII store otherwise).
**Key terms:** PII detection, log redaction. **Follow-up:** Prompts in App Insights? **Bala:** JMA strips PII before indexing.

**94. EU AI Act — current status and architect impact.** · *Medium*
High-risk obligations deferred to Dec 2027 (Digital Omnibus); GPAI rules enforceable; chatbot transparency Aug 2026. Architect must classify use cases, add human oversight, audit logging, explainability.
**Key terms:** high-risk, conformity, human oversight. **Follow-up:** Resume screening classification? **Bala:** Annex III high-risk.

**95. Who's accountable when an AI system causes harm?** · *Hard*
A named business + technical owner per registered use case, with residual risk formally accepted at approval. "The model did it" / "the vendor changed it" explains root cause but doesn't transfer accountability.
**Key terms:** AI inventory, ownership, drift detection. **Follow-up:** Provider silently changed the model. **Bala:** Canary + rollback ownership.

---

## J. System Design — AI Systems (5)

**96. Design a production RAG system for regulated financial documents.** · *Hard*
Document Intelligence extraction → structure-aware chunking → embeddings → Azure AI Search (hybrid + semantic ranker) with tenant/access metadata → grounded prompt + citations → Content Safety + groundedness → Managed Identity, Private Endpoints, audit logging. Eval gate before deploy.
**Key terms:** hybrid RAG, Private Link, groundedness. **Follow-up:** Where could a hallucination still slip? **Bala:** JMA's exact architecture (contracts/financial docs).

**97. Design for 2,000 concurrent users.** · *Hard*
Increase/round-robin TPM across deployments (APIM gateway), Redis semantic cache for common queries, AI Search replicas, streaming + reduced top-K to cut per-call cost, App Insights alerting at 80% TPM.
**Key terms:** APIM, semantic cache, replicas. **Follow-up:** Latency budget? **Bala:** JMA scale plan.

**98. Design a cost-optimized multi-model architecture.** · *Medium*
Model tiering: cheap local/mini model triages high-volume simple tasks, escalates complex to GPT-4o; PTU for predictable baseline + PAYG overflow; semantic caching; instrument true per-interaction cost.
**Key terms:** model routing, PTU, per-interaction cost. **Follow-up:** Multi-agent cost blow-up? **Bala:** JMA complexity routing (~$16.5k/mo saving est.).

**99. Design a portable GenAI architecture across Azure and AWS.** · *Hard*
Wrap generation + retrieval behind internal interfaces (IGenerationService/IRetrievalService) with Azure OpenAI+AI Search and Bedrock+Knowledge Bases implementations; keep prompts, eval, guardrails vendor-neutral. Swap implementation per cloud.
**Key terms:** abstraction layer, multi-cloud, Bedrock. **Follow-up:** Multi-cloud risks? **Bala:** Built a Bedrock module (L6) mapping 1:1 to Azure.

**100. Design an agentic workflow with safety guardrails end to end.** · *Hard*
Orchestrator (SK) + specialist agents + tools; ReAct loop with step caps and token budget; arg validation + FunctionInvocationFilter blocking destructive calls; human-in-the-loop for irreversible actions; groundedness check on outputs; full tracing to App Insights.
**Key terms:** ReAct, guardrails, human-in-the-loop, tracing. **Follow-up:** Agent hallucinated a consequential action — immediate response? **Bala:** JMA supervisor + AuditFilter + escalation service.

---

*Feature 12 complete — 100 questions across 10 categories, each anchored to a real JMA/KPMG example. Drill the follow-ups too; that's where interviews actually go.*
