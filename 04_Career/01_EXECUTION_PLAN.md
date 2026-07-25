# Execution Plan — GenAI Architect Interview Question Bank
**Status:** Active
**Source:** `00_PRD.md`

---

## Module List (build order)

| # | File | Topic | Target # Qs |
|---|---|---|---|
| 1 | `01_Fundamentals.md` | GenAI/LLM Fundamentals — transformers, attention, tokenization, embeddings, RLHF | 15 |
| 2 | `02_Azure_AI_Platform.md` | Azure OpenAI, AI Search, Document Intelligence, AI Foundry | 18 |
| 3 | `03_RAG_Architecture.md` | Chunking, hybrid retrieval, grounding, citations, failure diagnosis | 18 |
| 4 | `04_Agent_Orchestration.md` | Semantic Kernel, MCP, multi-agent, tool-calling, planning, **agent memory** | 18 |
| 5 | `05_Solution_Architecture.md` | Solution & deployment architecture, **caching**, **pricing/cost optimization** — see breakdown below | 35 |
| 6 | `06_Responsible_AI_LLMOps.md` | Content Safety, prompt injection, evaluation, drift, CI/CD, **AI governance** | 22 |

**Total: 126 questions across 6 modules.** (Raised from 87 — increased toward, but not to, the §11 max ceiling of ~170, to add depth without diluting into redundant rephrasing.)

---

## Module 4 Breakdown — Agent Orchestration (13 Qs)

| Sub-topic | What it covers | ~# Qs |
|---|---|---|
| 4a. Tool-calling & planning | Function calling, planners, ReAct-style loops | 4 |
| 4b. Multi-agent coordination | Orchestrator patterns, A2A, meta-agent hierarchies | 4 |
| 4c. Agent memory | Short-term/session memory, long-term/vector-backed memory, conversation summarization & compaction, memory scope in multi-agent systems | 5 |

---

## Module 5 Breakdown — Solution & Deployment Architecture (25 Qs)

This module is the deployment-scale centerpiece. It walks the full ladder rather than treating "scaling" as one question among many, and now explicitly covers caching and pricing as first-class architectural decisions:

| Sub-topic | What it covers | ~# Qs |
|---|---|---|
| 5a. Local / dev deployment | Single-instance, local containerized inference, dev/test isolation, cost-zero iteration | 3 |
| 5b. Single-region production | HA within a region, availability zones, load balancing, quota/throttling design | 4 |
| 5c. Multi-region (active-passive / active-active) | Replication strategy, failover, RPO/RTO, data sync (vector index, embeddings) across regions | 5 |
| 5d. Global scale-out | Latency-based/geo routing, data residency & sovereignty (e.g. EU/healthcare constraints), CDN/edge for static assets, multi-region cost model | 4 |
| 5e. Caching strategy | Prompt caching, semantic/CAG (cache-augmented generation) caching, response caching, cache invalidation & consistency across regions | 4 |
| 5f. Pricing & cost-optimization best practices | PTU (provisioned throughput) vs pay-as-you-go, token economics, model tiering/routing (cheap model for simple tasks), reserved capacity, cost attribution/chargeback per tenant | 3 |
| 5g. Multi-tenant & cost/security trade-offs | Tenant isolation models, shared vs dedicated capacity, security boundary at each deployment tier | 2 |

---

## Module 6 Breakdown — Responsible AI, LLMOps & Governance (15 Qs)

| Sub-topic | What it covers | ~# Qs |
|---|---|---|
| 6a. Content Safety | Categories, severity levels, groundedness detection, Prompt Shields | 3 |
| 6b. Prompt injection & security | Jailbreak defense, input/output filtering, red-teaming | 3 |
| 6c. Evaluation & drift | Golden datasets, automated eval pipelines, drift detection | 3 |
| 6d. CI/CD for LLMOps | Prompt versioning, model rollback, deployment gates | 2 |
| 6e. AI Governance | Model approval workflows, audit trails, policy-as-code, regulatory mapping (EU AI Act, HIPAA/PHI where relevant), vendor/model risk management | 4 |

---

## Module Format (every file)

Each question includes:
1. **Question** — as an interviewer would ask it
2. **Answer** — structured via the **WHY / HOW / WHEN / SCALE / DEPLOY** framework (see `00_PRD.md` §5). Tier 1 uses a lighter concept-check format since DEPLOY/SCALE don't apply to pure fundamentals.
3. **Follow-up probe** — the drill-down an interviewer is likely to chase

**DEPLOY dimension, applied consistently across Tiers 2–6:** every design answer explicitly addresses how the pattern looks at each step of **local → single-region → multi-region → global**, not just "how do you scale this" as an afterthought.

---

## Batching

**2–3 modules per batch** (revised from 1-at-a-time, since Module 1's format is validated):

| Batch | Modules | Qs | Status |
|---|---|---|---|
| 1 | 1 (expanded to 15), 2, 3 | 51 | In progress |
| 2 | 4, 5 | 53 | Pending |
| 3 | 6 | 22 | Pending |

Each batch is generated, then **auto-checked in** to `https://github.com/confksq/Learning/tree/main/Project/AIML-Learn` (private repo, personal `confksq` account) as part of preparing it — not held back for a separate manual check-in request, per your latest instruction. Cost/security review gate (§9 of PRD) still applies conceptually — this content is static Q&A markdown, no secrets/JMA-confidential specifics.
