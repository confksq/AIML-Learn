# AI Engineer Roadmap — Coverage Check vs. AIML-Learn Repo

**Date:** 2026-08-03
**Repo checked:** https://github.com/confksq/AIML-Learn (HEAD, ~300 files)
**Scope searched:** `01_Lessons`, `02_Questions`, `04_Career`, `06_Supplementary`, `08_Jobs`, `09_ML`, `AI103-Material` (chat history and `_Archive` excluded from scoring)

---

## Scorecard

| # | Area | Coverage | Verdict |
|---|------|----------|---------|
| 1 | Generative AI & LLM Fundamentals | ~90% | Strong — one named gap |
| 2 | RAG & Vector Databases | ~90% | Strong — index internals thin |
| 3 | Agentic AI & Multi-Agent | ~95% | **Strongest area** |
| 4 | LLMOps & MLOps | ~95% | Strong |
| 5 | GraphRAG & Knowledge Graphs | ~85% | Good — terminology gap |
| 6 | **Microsoft Fabric & Data Eng** | **~5%** | 🔴 **MAJOR GAP** |
| 7 | Traditional ML & Python | ~75% | Adequate — classic-ML metrics thin |
| 8 | System Design & Architecture | ~90% | Strong |
| 9 | **Behavioral & Leadership** | **~15%** | 🔴 **MAJOR GAP** |

---

## 1. Generative AI & LLM Fundamentals — ~90%

| Sub-topic | Status | Where |
|---|---|---|
| Attention mechanism | ✅ Deep | `L11_1_LLMs_Attention_Transformer.md` + `QA_L11_1` |
| Positional encoding | ✅ Deep | `L11_1` §6 (dedicated section, "not good" ≠ "good not" example) |
| RoPE / ALiBi / sliding window | ✅ | `Part1-AI-LLMs.md:78`, `InterviewBank/01_Fundamentals.md:77` |
| Tokenization | ✅ Deep | `L11_2_LLMs_Tokenization_Embeddings.md` + `QA_L11_2` |
| Embedding models | ✅ Deep | `L11_2`, 54 files reference |
| System prompts | ✅ | `L15_PromptEngineering.md`, `ClinicalSystemPrompts.cs` |
| Few-shot | ✅ | `L15`, 17 files |
| Chain-of-thought | ✅ | `L15`, `L27_Agent_Workflow_EndToEnd.md` |
| Multi-turn conversations | ✅ | `L15`, `L16_AIOrchestration_SK_Agents.md` |
| Fine-tuning vs RAG | ✅ Deep | `L14_FineTuning.md`, `L13_RAG_DeepDive.md`, `08-LoRA-FineTuning/README.md`, Ascendion Day3 module |
| Model selection (GPT-4o / Claude / Llama / Mistral) | ✅ | 70+ files; `azure_vs_bedrock_comparison.md` |
| Context window management | ✅ | 25 files; `HLP01_Memory_Tokens_Scaling_Agents.md` |
| **Context engineering (as a named discipline)** | 🟡 **PARTIAL** | Only `HLP01:71` ("context rot"). No coverage of **dynamic context assembly** or **multi-source fusion** by name. |

**Action:** add a short lesson or Q-block on Context Engineering — dynamic context assembly, multi-source fusion, context rot/poisoning, budget allocation across system prompt + retrieved chunks + tool results + history.

---

## 2. RAG & Vector Databases — ~90%

| Sub-topic | Status | Where |
|---|---|---|
| Full RAG pipeline | ✅ Deep | `L13_RAG_DeepDive.md`, `05-LlamaIndex-RAG/`, `01-Ollama-LocalRAG/` |
| Chunking: fixed vs semantic | ✅ Deep | `L13`, `ClinicalGuidelineChunker.cs`, `ChunkingEmbedQandA.md` |
| Vector DBs (why) | ✅ | 36 files |
| HNSW | ✅ | 20 files, incl. `L09_AzureAISearch.md` comparison table |
| **IVF / PQ / scalar quantization** | 🟡 **THIN** | Listed in curriculum outlines (`AIMLcurriculum.md:134`) and one Azure AI Search table row (`L09:816` IVFFlat) — **never actually taught**. |
| **Recall vs latency trade-off** | 🟡 **THIN** | Single hit: `InterviewBank/03_RAG_Architecture.md`. No ef_construction / ef_search / nprobe tuning discussion. |
| Hybrid search | ✅ Deep | `HybridClinicalRetrieval.cs`, `L09`, 28 files |
| BM25 / keyword | ✅ | 16 files |
| Re-ranking / cross-encoder | ✅ | 23 files, `FormularyVectorSearch.cs` |
| Query rewriting / expansion | ✅ | `L13`, `L12_AzureOpenAI_Services.md` |
| HyDE | ✅ | `L13` + `QA_L13` |
| Multi-step retrieval | ✅ | `L13` + `QA_L13` |
| Debugging RAG failures | ✅ | `L24_Hallucination_Mitigation.md`, `L36_LLM_Observability_FinOps.md`, `03-RAGAS-Evaluation/` |

**Action:** one page on ANN index internals — HNSW (M, ef_construction, ef_search), IVF (nlist/nprobe), PQ compression — framed as the recall/latency/memory triangle. This is a very common interview probe.

---

## 3. Agentic AI & Multi-Agent — ~95% ✅ Strongest area

| Sub-topic | Status | Where |
|---|---|---|
| ReAct | ✅ | 32 files |
| Tool calling / function calling | ✅ Deep | `L27_Agent_Workflow_EndToEnd.md`, `L21_Python_for_AI.md` |
| LangGraph | ✅ | 21 files |
| crewAI | ✅ Deep | `02-crewAI-MultiAgent/` full project |
| AutoGen | ✅ | 10 files, `L25_AgentFramework_Comparison.md` |
| Semantic Kernel | ✅ Deep | 61 files, `L16` |
| Tool integration (APIs/DBs/code) | ✅ | `06-MCPHub/ClinicalToolRegistry.cs`, plugins under `03-PriorAuthAgent/` |
| Short/long-term memory | ✅ | `HLP01`, `L24`, `02-crewAI/03_interview_qa.md` |
| State management | ✅ | `L25`, `L31_FaultTolerance_Observability.md` |
| Agent evaluation | ✅ | `L24`, `L27`, `PASupervisorAgent.cs` |
| MCP | ✅ Deep | `L26_MCP_ModelContextProtocol.md` |
| A2A | ✅ Deep | `L29_A2A_Protocol.md`, `05-A2ACommunication/` |
| Meta-agent hierarchies | ✅ Deep | `L28_MetaAgent_Hierarchies.md`, `04-MetaAgentOrchestration/` |

**Action:** none. This area exceeds the roadmap.

---

## 4. LLMOps & MLOps — ~95%

| Sub-topic | Status | Where |
|---|---|---|
| MLOps vs LLMOps | ✅ Deep | `L19_MLOps_LLMOps.md` (side-by-side table at :52 and :657) |
| RAGAS | ✅ Deep | `03-RAGAS-Evaluation/` full project |
| Groundedness | ✅ Deep | 45 files, `ClinicalGroundednessMonitor.cs` |
| Automated eval pipelines | ✅ | `ClinicalEvalPipeline.cs`, `L36` |
| CI/CD for models | ✅ | `L34_Kubernetes_Helm_GitOps.md`, `L33_IaC_Terraform` |
| A/B testing | ✅ | `L36`, `03-RAGAS-Evaluation/README.md` |
| Versioning (model + prompt) | ✅ | `ClinicalPromptVersioning.cs`, `L34` |
| Rollback | ✅ | `L34`, `L31` |
| Data drift | ✅ | `L19`, `L06_AzureML.md`, `L36` |
| Concept drift | ✅ | `L19` + `QA_L19` |
| Cost monitoring | ✅ Deep | `L36_LLM_Observability_FinOps.md` (dedicated), `L18` §18.4 |

**Action:** none.

---

## 5. GraphRAG & Knowledge Graphs — ~85%

| Sub-topic | Status | Where |
|---|---|---|
| GraphRAG vs RAG | ✅ Deep | `07-GraphRAG-Neo4j/01_concepts.md`, `04c_vector_vs_graph_comparison.py` |
| Neo4j + vector integration | ✅ Deep | `04a_neo4j_basics.py`, `04b_graph_rag.py`, `docker-compose.yml` |
| Entity-relationship modeling | ✅ | `02_architecture.md`, `03_interview_qa.md` |
| Cypher | ✅ | 7 files |
| Multi-hop reasoning | ✅ | 15 files |
| **Local vs Global search (named)** | 🟡 **PARTIAL** | Community detection + community summaries are covered (`03_interview_qa.md:28`, `02_architecture.md:54`), and "global" questions are explained — but **"local search" is never used as a term**. Microsoft GraphRAG's local-vs-global query router isn't framed as the named dichotomy an interviewer will ask about. |

**Action:** 15-minute fix — add the explicit Local Search (entity-anchored, "what does X do?") vs Global Search (community-summary map-reduce, "what are the themes?") framing to `07-GraphRAG-Neo4j/03_interview_qa.md`.

---

## 6. Microsoft Fabric & Data Engineering — ~5% 🔴 MAJOR GAP

| Sub-topic | Status | Evidence |
|---|---|---|
| Microsoft Fabric | 🔴 **NONE** | 1 passing mention in `L17_AzureAIFoundry.md`; real content only in raw chat history (`claude-AIFoundryQA-imp.md`) — never turned into a lesson |
| OneLake | 🔴 **ZERO** | 0 hits in lessons/questions. Only `AscendionPrep_Day3:1863` "Fabric IQ (OneLake Catalog)" as a UI label |
| Lakehouse | 🔴 **ZERO** | Only appears in `09_ML/MLEngineer_Coverage_2026-07-26.md:105` — flagged there as **"🔴 Zero hits"** (you already identified this gap) |
| Medallion (Bronze/Silver/Gold) | 🔴 **ZERO** | Same — listed as gap #73 in your own ML coverage tracker |
| Dataflows Gen2 | 🔴 **ZERO** | 0 hits repo-wide |
| Fabric ↔ Azure AI Foundry integration | 🔴 **NONE** | — |
| Fabric capacity / CU / cost governance | 🔴 **ZERO** | 0 hits |
| Delta Lake / Parquet | 🟡 3 passing mentions | `L06_AzureML.md` |
| Synapse / Databricks | 🟡 8 mentions | `L20_IntegrationPatterns.md`, `QA_L06` |

**This is the single largest hole.** Your own `09_ML/MLEngineer_Coverage_2026-07-26.md:260` already prescribes the fix ("Data-engineering module — ADF/Databricks → Delta medallion → feature store, ~10 hrs") but it was never built.

**Action (highest priority, ~6–8 hrs):** new lesson `Part8_DataPlatform/L37_MicrosoftFabric.md` covering OneLake (one copy, shortcuts, Delta-Parquet as native format), Lakehouse vs Warehouse, Medallion Bronze/Silver/Gold, Dataflows Gen2 vs Pipelines vs Notebooks, Fabric→Foundry grounding, and capacity/CU cost governance + pause/resume.

---

## 7. Traditional ML & Python — ~75%

| Sub-topic | Status | Where |
|---|---|---|
| Supervised vs unsupervised | ✅ | `L01_Introduction_to_AI.md` |
| Classification / regression | ✅ | `L01`, `L06_AzureML.md` |
| Precision / recall | ✅ | 19 files (mostly RAGAS context) |
| **F1** | 🟡 3 hits | `L01`, `L06`, `L07` — mentioned, not worked through |
| **ROC-AUC** | 🟡 **THIN** | `AUC` appears only as AutoML leaderboard output (`L06:295-298`) and in the `L19` metrics comparison. **No lesson explains the ROC curve, threshold selection, or when AUC misleads on imbalanced data.** |
| Confusion matrix | ✅ | `L03_NLP_Fundamentals.md`, `L06` |
| LoRA | ✅ Deep | `08-LoRA-FineTuning/` full project + notebook |
| QLoRA | ✅ Deep | Same |
| PEFT | ✅ Deep | Same |
| Cross-validation | ✅ | `L06` + `QA_L06` |
| Feature engineering | ✅ | `L06` |
| scikit-learn | ✅ | `L21_Python_for_AI.md`, `L06` |
| Live-coding a RAG pipeline in Python | ✅ Deep | 9 runnable `04_hands_on.py` files across CareerAccelerator projects |

**Action (~2 hrs):** a metrics page — precision/recall trade-off, F1 vs F-beta, ROC vs precision-recall curve, why AUC flatters imbalanced classifiers, threshold selection tied to business cost. This is standard screening material and it's your thinnest classic-ML spot.

---

## 8. System Design & Architecture — ~90%

| Sub-topic | Status | Where |
|---|---|---|
| End-to-end production RAG design | ✅ Deep | `L18_AISolutionArchitecture.md` §18.1 (3 patterns + decision table) |
| Agentic system at scale | ✅ | `L18` Pattern 2, `L27`, `L28` |
| Cost / latency / accuracy trade-offs | ✅ Deep | `L18` §18.2 (four scalability levers, latency optimisation), §18.4 (cost formula, optimisation, monitoring) |
| Security threat model | ✅ Deep | `L18` §18.3 |
| AKS | ✅ | `L34_Kubernetes_Helm_GitOps.md`, 21 files |
| Azure Functions | ✅ | `L20_IntegrationPatterns.md`, `L18` |
| **AWS Lambda** | 🟡 2 hits | Only inside `06-Amazon-Bedrock/` project |
| Scaling / throughput / PTU | ✅ | 50 files |
| Caching (semantic + prompt) | ✅ | `L23_CAG_vs_RAG.md`, `L36` |
| Fault tolerance / circuit breaker | ✅ Deep | `L31`, `PayerCircuitBreaker.cs`, `PayerAPIRetryPolicy.cs` |

**Action:** minor — if the target role is multi-cloud, add a one-paragraph AKS vs Azure Functions vs AWS Lambda vs Bedrock-hosted serving comparison. Low priority.

---

## 9. Behavioral & Leadership — ~15% 🔴 MAJOR GAP

| Sub-topic | Status | Evidence |
|---|---|---|
| **STAR method** | 🔴 **ZERO** | 0 hits repo-wide |
| **"Tell me about a time…" bank** | 🔴 **ZERO** | 0 hits repo-wide |
| **Mentoring stories** | 🔴 **NONE written** | `08_Jobs/FDE/FDE-Prep_Tracker.md:146` lists "Mentoring / transformation catalyst" as item #60 — status 🔵 "day job — write the bullet (Stage 0, zero study)". **Flagged as a to-do, never written.** |
| Cross-functional collaboration | 🟡 8 passing mentions | No prepared narrative |
| Business outcome / ROI framing | 🟡 Embedded only | `InterviewBank/05_Solution_Architecture.md:218-221` has an excellent cost-vs-outage-risk ROI framing, and several "Leadership asks…" follow-up probes exist (`05:154`, `05:214`, `06:94`, `05:388`) — but these are *technical* answers with a leadership veneer, not behavioral stories |
| Dedicated behavioral prep file | 🔴 **NONE** | No such file exists |

Your 132 InterviewBank questions are **all technical**. For a **Lead** role, behavioral is typically 25–40% of the loop and often the deciding round.

**Action (highest ROI per hour, ~3–4 hrs):** create `02_Questions/InterviewBank/07_Behavioral_Leadership.md` with 10–12 STAR stories drawn from real material you already have — JM Family production AI, the VitalCare prior-auth platform, the Ascendion healthcare work. Cover: mentoring a junior through an AI project, disagreeing with a stakeholder on RAG vs fine-tuning, a production incident you owned, driving adoption against resistance, a project whose business impact you can quantify, and a failure with what you changed after.

---

## Priority Queue

| Rank | Gap | Effort | Why now |
|---|---|---|---|
| 1 | **Behavioral & Leadership story bank** | 3–4 hrs | Zero coverage; largest share of a Lead loop; no study required, only writing |
| 2 | **Microsoft Fabric module** | 6–8 hrs | Zero coverage; explicitly required by the roadmap; already self-identified as gap #73 |
| 3 | Classic-ML metrics (ROC/AUC, F1, thresholds) | 2 hrs | Standard screening material; your thinnest classic-ML spot |
| 4 | ANN index internals (HNSW params, IVF, PQ) | 1–2 hrs | Common deep-dive probe; you have the surrounding RAG depth already |
| 5 | Context Engineering lesson | 1–2 hrs | Named roadmap item; you have adjacent pieces, needs assembly |
| 6 | GraphRAG Local vs Global naming | 15 min | Content exists, terminology missing |
| 7 | AWS Lambda / multi-cloud serving comparison | 30 min | Only if role is multi-cloud |

**Total to close every gap: ~15–19 hours.**

## Bottom Line

Your repo **over-delivers** on the areas the market currently prices highest — agentic systems, MCP/A2A protocols, RAG engineering, LLMOps, and hallucination mitigation — with runnable code, per-chapter Q&A, and two substantial applied portfolios (CareerAccelerator, VitalCare). Roughly **6 of 9 roadmap areas are at or above interview-ready depth.**

The two real holes are both **outside** your GenAI core: **Microsoft Fabric** (a data-platform topic, ~5%) and **Behavioral/Leadership** (~15%). Neither is hard — one is study, one is just writing down stories you already lived.
