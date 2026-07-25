# JD Coverage Analysis — Synergech & Lorven

**Created:** 2026-07-19
**Sources:** `C:\Users\confksq\Project\jbs\syner.txt` · `C:\Users\confksq\Project\jbs\finan.txt`
*(Both files contain the same resume; only the JD differs.)*

| File | Role | Location |
|---|---|---|
| `syner.txt` | Synergech — Lead Agentic AI Engineer / AI Architect | Atlanta, GA (5 days onsite) |
| `finan.txt` | Lorven — Technical Lead Architect, AI | Miami, FL |

**Headline: ~85% of the technologies in both JDs are covered by the library.**
Gaps cluster in three areas: AI-native frontend, infrastructure-as-code, and two bleeding-edge protocol names.

---

## ⚠️ Correction to earlier analysis

An earlier pass through this library reported that **LangGraph and AutoGen were absent**. That was
wrong — it grepped `01_Lessons/` only and overstated the result. Verified counts:

| Tech | Total hits | Real location |
|---|---|---|
| LangGraph | 244 | `L16` orchestration framework table · `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/02-crewAI-MultiAgent/03_interview_qa.md` has a direct *"crewAI vs LangGraph"* Q&A |
| AutoGen | 131 | crewAI vs AutoGen vs MAF contrast table in `02-crewAI-MultiAgent/01_concepts.md` |
| MAF (Microsoft Agent Framework) | 10 files | same contrast table |
| pgvector / Pinecone / Qdrant | 14 / 15 / — | `L09` and `L13` vector-DB decision tables |

Treat this file as the corrected baseline.

---

## Tier 1 — Well covered (lesson + portfolio depth)

Safe to discuss at any depth; most are backed by something you built.

**GenAI / RAG:** Azure AI Foundry · Azure OpenAI (GPT-4o) · Semantic Kernel · RAG architecture ·
Azure AI Search (hybrid + semantic + HNSW) · Document Intelligence · vector embeddings
(text-embedding-3) · chunking strategies · re-ranking · Prompt Engineering · Content Safety ·
Responsible AI · RAGAS evaluation · LoRA/QLoRA (PEFT)

**Frameworks / multi-cloud:** LlamaIndex · crewAI · HuggingFace Transformers · Ollama ·
Amazon Bedrock · GCP Vertex AI · GraphRAG + Neo4j · Prompt Flow · Azure ML

**Platform:** Cosmos DB · Event Grid · Service Bus · APIM · Azure Functions · Key Vault · RBAC ·
OAuth 2.0 · Managed Identity · MLOps/LLMOps · CI/CD

**Agentic:** ReAct · multi-agent orchestration · supervisor pattern · HITL · guardrails ·
fault tolerance (circuit breaker, retry, escalation) · **MCP** · **A2A** · **CAG**

> MCP, A2A, and CAG live in `08_Jobs/AscndIntr/PrepPlan/` (modules 02, 05, 07, 08) — filed as
> interview prep, but it is the only material on those protocols. Synergech names all of them.

---

## Tier 2 — Comparison-level only

You can say *"here's when I'd reach for it and why"* — an architect answer. It fails only if asked
to describe something you personally built.

| Tech | What exists | Safe framing |
|---|---|---|
| ~~**LangGraph**~~ | **See correction below — LangGraph is Tier 1, not Tier 2** | |
| **AutoGen / MAF** | Contrast table | "Conversational multi-agent; MAF is Microsoft converging SK + AutoGen" |
| **pgvector / Pinecone / Qdrant** | `L09`/`L13` decision tables | "When you're already on Postgres / when you need Azure-native / external dependency trade-off" |
| **Docker / AKS** | Architecture-context mentions | Conceptual only |
| **PostgreSQL / Databricks / MLflow** | Passing mentions | Name-level |

---

### ✅ Second correction — LangGraph is Tier 1 (2026-07-19)

`01_Lessons/Part5_AgenticProtocols/L25_AgentFramework_Comparison.md` is **258 lines with 30 LangGraph
references** — not a comparison table. It teaches:

- The state-machine-vs-conversation mental model (hospital patient-pathway analogy)
- All four core pieces: **StateGraph · State · Node · Checkpointer**
- Checkpointer resume semantics — crash at node 7 of 12 resumes at node 8, not from scratch
- A worked healthcare **prior-authorization** example with real `TypedDict` state, conditional
  edges, and a human-in-the-loop physician interrupt
- Section 6 "State Management (The Trap Question)" and Section 7 on a mixed 60% Python / 40% .NET team

**This is taught material with code — treat LangGraph as something you can discuss in depth**, not
as a name you recognize. The only caveat is that it's a prep briefing rather than a lesson in
`01_Lessons/`, and there's no evidence you've *run* a LangGraph app.

---

## Tier 3 — Genuinely absent

Every hit for these is inside the JD text itself, not in any lesson.

| Tech | Hits | Note |
|---|---|---|
| **AG-UI** | 4 — all in the JD | Whole responsibility block in Synergech |
| **CodeAct / Code Interpreter** | 2 — all in the JD | Recent enough that "read the spec, haven't shipped it" is defensible |
| **FastMCP** | 1 — in the JD | Lorven-specific, "strong plus" not required |
| **Terraform** | 3 — 2 in the JD | Required qualification at Synergech |
| **Assistants API** | 3 — mostly JD | |
| **React streaming UI · TypeScript** | 0 real | ⚠️ every "React" hit was the **ReAct** pattern — false positive |
| **Anthropic Claude API** | 0 | Constitutional AI is in `L11_4`; the API is not |
| **MongoDB · Helm · KEDA** | 0 | |
| **Snowflake · Appian · DealCloud · Backstop** | 0 | Client domain systems — not learnable prep |

### The three gap clusters

1. **AI-native frontend** — AG-UI, React streaming interactions, TypeScript, HITL UI patterns.
   Synergech's *"Full-Stack AI Application Engineering"* is an entire responsibility block and this
   is the weakest area in the library.
2. **Infrastructure-as-code** — Terraform, Helm/KEDA, pipeline-as-code, policy-as-code.
3. **Emerging protocol names** — AG-UI, CodeAct. Small surface, cheap to close, high differentiation.

---

## Backlog — lessons to prepare later

Ordered by impact per hour. None started.

| # | Proposed lesson | Closes | Effort | Why |
|---|---|---|---|---|
| 1 | **AG-UI + CodeAct primer** | Tier 3 | ~1 hr | Smallest surface, highest differentiation — few candidates can define these accurately |
| ~~2~~ | ~~LangGraph~~ — **already covered**, see correction above (now `L25`) | — | — | Removed from backlog |
| 3 | **AI Security & Governance** | pre-existing library gap | ~3 hrs | Planned as Module 21, never built; both JDs have governance sections |
| 4 | **Terraform / IaC for AI workloads** | Tier 3 | ~3 hrs | Explicit required qualification |
| 5 | **React streaming agent UI + HITL patterns** | Tier 3 | ~5 hrs | Largest gap, largest effort; genuinely new skill area |
| 6 | **Anthropic Claude API + multi-LLM routing** | Tier 3 | ~2 hrs | Synergech wants workload-based model selection across ecosystems |

---

## Cross-references

- High-level prep on memory / tokens / scaling / agents → `02_Questions/HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md`
- MCP / A2A / CAG / framework comparison → `01_Lessons/Part5_AgenticProtocols/` (L23, L25, L26, L28, L29)
- Full JD text → `08_Jobs/july20thWeek.txt`
- Portfolio evidence → `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/`
