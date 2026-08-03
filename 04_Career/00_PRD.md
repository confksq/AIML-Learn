# PRD — AI Architect / GenAI Architect Interview Question Bank
**Status:** Draft — pending your review/edits
**Owner:** confksq
**Created:** 2026-07-03

---

## 1. Purpose

Build a reusable, structured interview question bank for **AI Architect / Generative AI Architect** roles, distinct from any single company's interview loop. This is a standing prep asset — not tied to a specific interview date (unlike `AscndIntr/`, which was scoped to one screener at one client).

---

## 2. Background / Candidate Context

- AI-102 certified (completed 2026-06-30)
- Production experience at JM Family: Document Intelligence, Azure AI Search (keyword-only prod index, vector-enabled staging via EnterpriseSearch.Sync Push API), RAG groundwork
- Strong C#/.NET/Azure background; Semantic Kernel is the primary orchestration path (not Python/LangChain)
- Theory coverage: 14/19 modules done (Parts 1–3 largely complete — LLMs, RAG, Azure OpenAI, Prompt Engineering, Fine-Tuning, Orchestration)
- Known gaps (from `AscndIntr` prep): A2A protocol, MCP Hub architecture, CAG vs RAG, agentic hallucination, Python-side framework comparisons (LangGraph/AutoGen)

---

## 3. Scope

### In scope
- Question bank covering GenAI Architect interview surface area: fundamentals, Azure AI platform, RAG architecture, agent orchestration, solution/system design, responsible AI & LLMOps
- Each question includes: the question, what a strong answer covers, and a likely interviewer follow-up/drill-down
- Mix of difficulty tiers: screening-level fundamentals → scenario-based → whiteboard/system-design → trade-off judgment
- Framed generically (role-based), with optional anchoring to JMA production experience where relevant

### Out of scope
- Company-specific prep (screener intel, client-specific framing) — that belongs in a dedicated folder per opportunity, following the `AscndIntr/` pattern
- Resume/assessment defense material
- Live mock interview simulation (can be a later phase, not this PRD)

---

## 4. Proposed Structure (subject to your edits)

| Tier | Focus | Target # Qs | Format |
|---|---|---|---|
| 1 | GenAI/LLM Fundamentals | 10 | Rapid-fire concept checks |
| 2 | Azure AI Platform | 12 | WHY-HOW-WHEN-SCALE-DEPLOY |
| 3 | RAG Architecture | 12 | WHY-HOW-WHEN-SCALE-DEPLOY |
| 4 | Agent Orchestration (SK, MCP, multi-agent, **memory**) | 13 | WHY-HOW-WHEN-SCALE-DEPLOY |
| 5 | Solution & Deployment Architecture (incl. **caching**, **pricing/cost optimization**) | 25 | WHY-HOW-WHEN-SCALE-DEPLOY (deployment topology + caching + pricing are the centerpiece of this tier) |
| 6 | Responsible AI, LLMOps & **Governance** | 15 | WHY-HOW-WHEN-SCALE-DEPLOY |
| **Total** | | **~87** | |

**Newly added coverage (this revision):** agent memory (short/long-term, vector-backed, summarization), caching strategy (prompt caching, semantic/CAG caching, cache invalidation across regions), pricing & cost-optimization best practices (PTU vs pay-as-you-go, token economics, model tiering/routing, reserved capacity, chargeback), and AI governance (model approval workflows, audit trails, policy-as-code, regulatory mapping, vendor/model risk management).

---

## 5. Answer Framework — WHY / HOW / WHEN / SCALE / DEPLOY

This is the core change from the original draft: every scenario-style question (all tiers except Tier 1) is answered against a fixed 5-part lens instead of a free-form outline. This mirrors how Architect interviews actually drill — they don't stop at "what is it," they chase "why this, when not, does it hold at scale, where does it physically run."

| Lens | What it forces the answer to cover |
|---|---|
| **WHY** | The business/technical driver. What problem this solves, why not the obvious alternative. |
| **HOW** | The technical implementation. Components, data flow, integration points. |
| **WHEN** | Decision triggers. When to reach for this pattern vs. a competing one; maturity/scale thresholds. |
| **SCALE** | How it scales. Horizontal vs vertical, throughput ceilings, cost curve, bottleneck that breaks first. |
| **DEPLOY** | Deployment topology, explicitly walked through **local → single-region → multi-region → global**: data residency/sovereignty, latency-based routing, replication strategy, DR/failover, cost-at-each-step. |

Tier 1 (Fundamentals) stays as lighter concept checks — DEPLOY/SCALE don't meaningfully apply to "what is attention."

Tier 5 is where DEPLOY is the primary axis, not a side note — it explicitly walks the deployment scale ladder (see updated structure below) rather than treating "how do you scale this" as one question among many.

---

## 6. Deliverables

- One `.md` file per tier (e.g. `01_Fundamentals.md`, `02_Azure_AI_Platform.md`, …) inside this folder
- Each file self-contained: question → WHY/HOW/WHEN/SCALE/DEPLOY answer → follow-up probe (Tier 1: question → concept-check answer → follow-up probe)
- This PRD (`00_PRD.md`) stays as the source of truth for scope; updated if tiers/counts change

---

## 7. Open Questions / Decisions Needed From You

1. Question counts per tier — keep as proposed, or reweight further?
2. Should JMA production experience be woven into answers as a running example, or kept generic/role-neutral?
3. Any tier to exclude or add (e.g. behavioral/leadership questions for Architect-level scope)?
4. Confirm the WHY-HOW-WHEN-SCALE-DEPLOY framework is the right lens, or adjust its 5 dimensions.
5. Memory, caching, pricing, and governance are now folded into Tiers 4–6 rather than standalone tiers — confirm that's right, or split any of them out as their own module.

---

## 8. Next Steps

Once this PRD is confirmed/edited → execution plan is finalized (see `01_EXECUTION_PLAN.md`) → generate tier files starting with Tier 1.

---

## 9. Publishing / Check-in Process (post-generation)

Once the question bank modules are prepared, you'll separately request upload/check-in of these files — subject to a **cost + security review gate** before anything is checked in. This mirrors the Governance sub-topic (6e) in the plan itself.

- **Cost review:** confirm no paid/metered service calls are wired into this content (it's static `.md` — no live API calls expected) before it's checked in anywhere billable.
- **Security review:** confirm no secrets, keys, internal endpoints, or JMA-confidential data (resource names, tenant IDs, internal architecture specifics) leak into question/answer text before pushing to any repo — especially if the target repo is not private or not JMA-internal.
- Target check-in location (repo/system) — **not yet specified**; confirm which repo/system before that step.

This is a gate to apply **at check-in time**, not before generation — generation proceeds independently.

---

## 10. Question Sourcing Methodology

Questions are produced via a **mixed approach**, three layers:

| Layer | What it is | Where it applies |
|---|---|---|
| **1. Base knowledge synthesis** | Patterns recalled from training data (real interview reports, technical docs, discussions, up to Jan 2026 cutoff) — question *patterns*, not verbatim copies; no specific source/person can be cited | All modules — foundation layer |
| **2. Original/creative synthesis** | Purpose-built questions around the WHY-HOW-WHEN-SCALE-DEPLOY framework, mapped to the actual candidate stack (Azure OpenAI, AI Search, Document Intelligence, Semantic Kernel, JMA production RAG). This combination is authored, not retrieved — won't exist verbatim anywhere online | All modules — primary layer, majority of content |
| **3. Live web validation** | Targeted `WebSearch`/`WebFetch` checks against current real interview-experience posts and current facts, used only where staleness risk is real | **Module 5 (Pricing/Cost)** — Azure pricing/PTU/reserved-capacity mechanics change often; **Module 6 (Governance)** — regulatory landscape (EU AI Act, compliance frameworks) moves fast. Not applied to Modules 1–4 — conceptually stable, no search needed |

**Integrity constraint:** no question will be presented as sourced from "a real person's actual interview" — that provenance can't be verified and won't be fabricated.

---

## 11.5. Flagged Topic — AI App / Agent / Workflow Scaling (added 2026-08-02)

A full-lesson-worthy gap surfaced in a chat session on 2026-08-02, sitting squarely in **Tier 4
(Agent Orchestration)** and **Tier 5's SCALE lens**. Not yet written as a lesson — flagged here so it
isn't lost before Tier 4/5 generation. Core content to carry forward:

- **The central point:** AI apps have independent scaling layers — app/compute, LLM quota (TPM/RPM,
  PTU), retrieval (Azure AI Search Search Units), and demand itself (semantic caching, model
  tiering) — and scaling the wrong layer doesn't help, and can actively make things worse.
- **The worked failure case (directly answers the open check question already in `08_Jobs/FDE/`):**
  scaling app replicas 5→20 while all replicas share one Azure OpenAI deployment's TPM quota
  increases 429 contention rather than fixing it — the bottleneck was the LLM quota, not compute.
  Fix: raise TPM, add a second load-balanced deployment, or move to PTU.
  This is the same "20 replicas made it worse" question already sitting unanswered in
  `08_Jobs/FDE/FDE-Prep_Tracker.md`'s "Open check questions" section — resolving it here should also
  close that entry.
- **Agent-specific wrinkle:** agents make multiple LLM calls per user request (ReAct loop), so agent
  traffic hits TPM/RPM ceilings faster than simple chat at equal user volume — cap iterations, queue
  long-running agent work instead of holding the request open.
- **State-at-scale wrinkle:** in-memory ChatHistory/session state breaks under horizontal scaling
  unless externalized (Redis/Cosmos DB) — a replica change mid-session must not lose context.

**Action when Tier 4/5 files are generated:** fold this in as one or more WHY-HOW-WHEN-SCALE-DEPLOY
questions rather than treating it as a standalone add-on.

---

## 11. Maximum Question Ceiling Per Module

Beyond the **target counts** (§4), each module has a practical **maximum** before additional questions become redundant/overlapping rather than adding new coverage. This is a ceiling for later expansion if you want more depth — not the default generation count.

| # | Module | Target (current plan) | Practical Max | Why the ceiling |
|---|---|---|---|---|
| 1 | Fundamentals | 10 | ~20 | Concept space (attention, tokenization, embeddings, pretraining, RLHF, etc.) is finite before repeating the same idea in different words |
| 2 | Azure AI Platform | 12 | ~25 | Bounded by number of distinct Azure AI services + their config surface (OpenAI, AI Search, Doc Intelligence, Foundry, Content Safety, Speech, Vision) |
| 3 | RAG Architecture | 12 | ~25 | Chunking, retrieval, hybrid search, reranking, grounding, citations, GraphRAG, CAG comparison, eval, failure modes — wide but not infinite |
| 4 | Agent Orchestration (incl. memory) | 13 | ~25 | SK, MCP, A2A, multi-agent patterns, planners, tool-calling, meta-agents, fault tolerance, memory — similar bound to Module 3 |
| 5 | Solution & Deployment Architecture (incl. caching, pricing) | 25 | ~45 | Largest surface area — full deployment ladder × caching × pricing × multi-tenant × security gives the most combinatorial scenario space |
| 6 | Responsible AI, LLMOps & Governance | 15 | ~30 | Content Safety, prompt injection, eval/drift, CI/CD, governance, compliance mapping, audit |
| **Total** | | **~87** | **~170** | |

Past the max column, further questions would mostly rephrase existing ones rather than cover new ground — better spent on deeper follow-up probes per question than on raw count.
