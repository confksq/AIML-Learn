# Consolidation & Update Plan — AIML-Learn

**Date:** 2026-08-03
**Inputs consolidated:** `github.com/confksq/AIML-Learn` (canonical) + `C:\pers\Resume-May2026\deepseekLessons\` (8 files) + `C:\pers\AIML-Learn\` (3 stale .docx)
**Companion doc:** `Roadmap_Coverage_Check_2026-08-03.md`

---

## Part A — Deduplication Matrix

Every DeepSeek file classified against what the repo already has.

| DeepSeek file | Topic | Repo equivalent (verified) | Verdict |
|---|---|---|---|
| `..._a4e56c.md` | GapFill README / market stats | Superseded by `Roadmap_Coverage_Check_2026-08-03.md` | 🗑️ **DELETE** |
| `..._8f5547.md` | Responsible AI + MCP | `L26_MCP_ModelContextProtocol.md`, `L22_Foundry_AgentLifecycle.md`, `L18` §18.3 (threat model), `HIPAAGateway.cs`, `InterviewBank/06` (23 Qs). Prompt injection → L18/L19/L22/L23/L27. PII → L35/L36. Guardrails/content safety → L34/L36. | 🗑️ **DELETE — 100% redundant** |
| `..._2f4daa.md` | Advanced Prompt Engineering | `L15_PromptEngineering.md` + `QA_L15` (incl. prompt chaining). Token optimization → L36/L23/L18. Structured outputs/JSON mode → L22/L24. | 🗑️ **DELETE — 100% redundant** |
| `..._917f86.md` | GraphRAG + Neo4j | `07-GraphRAG-Neo4j/` — 8 files, runnable Cypher, `04c_vector_vs_graph_comparison.py` | ✂️ **HARVEST 4 lines** (local-vs-global definitions) → then DELETE |
| `..._641640.md` | Vector DBs in production | HNSW covered (L09/L13); **IVF/PQ/recall-latency absent** | ✂️ **HARVEST** → new `L13_1`. ⚠️ 2 errors to fix first |
| `..._93db8f.md` | Microsoft Fabric | **Nothing** | ✅ **KEEP as skeleton** → expand into L37 |
| `..._5143a5.txt` | Proposed 50-file tree | — | ✅ **KEEP Fabric branch as outline**, discard other 7 branches |
| `..._6f1547.md` | Interview Prep Guide | System design ⊂ `L18` + `InterviewBank/05` (36 Qs). Coding ⊂ 9 runnable `04_hands_on.py`. **Behavioral = nothing.** | ✂️ **HARVEST 5 behavioral Qs** → `InterviewBank/07` → then DELETE |

**Net: 4 delete, 3 harvest, 1 keep.** Of ~15 KB, roughly 4 KB is non-duplicate signal.

### ⚠️ Corrections — do NOT copy these claims forward

| Claim in DeepSeek file | Problem | Use instead |
|---|---|---|
| "Azure AI Search — Cons: **Limited vector capabilities**" | Outdated/wrong. It has vector, hybrid, and semantic reranking. **Your own `L09_AzureAISearch.md:816` is more accurate.** Risky to repeat given you're Azure-primary. | The L09 comparison table |
| "IVF: Faster search, **lower recall**" | Oversimplified — IVF recall is tunable via `nprobe`. It's a recall/latency *dial*, not a fixed penalty. | Frame all three as points on a recall/latency/memory triangle |
| "Fabric architect demand grew **180% YoY**", "495% YoY Responsible AI" | Unverifiable marketing figures. Quoting a fabricated stat in an interview is a real risk. | Omit entirely — argue from architecture, not stats |
| "Fabric IQ: AI-powered assistant for data exploration" | Vague/dated framing | Rewrite from current MS Learn docs when authoring L37 |

---

## Part B — Target Structure (follows existing repo conventions)

Repo currently runs **L01–L36 across Part1–Part7**, `InterviewBank/01–06`. Sub-lessons use the established `L11_1 … L11_4` pattern.

### New files to create

| # | Path | Source | Effort |
|---|---|---|---|
| 1 | `02_Questions/InterviewBank/07_Behavioral_Leadership.md` | **Write from scratch** — seed Qs from `6f1547` | 3–4 hrs |
| 2 | `01_Lessons/Part8_DataPlatform/L37_MicrosoftFabric.md` | Expand `93db8f` skeleton + `5143a5` Fabric outline | 5–7 hrs |
| 3 | `01_Lessons/Part1_Foundations/L06_1_ML_Evaluation_Metrics.md` | **Write from scratch** — DeepSeek file never generated | 2 hrs |
| 4 | `01_Lessons/Part3_GenAI_LLMs/L13_1_ANN_Index_Internals.md` | Harvest `641640` + correct the 2 errors | 1–1.5 hrs |
| 5 | `01_Lessons/Part3_GenAI_LLMs/L15_1_Context_Engineering.md` | **Write from scratch** — assemble from `HLP01` + L23 + L36 | 1.5–2 hrs |

### Matching Q&A files (repo convention: every lesson has one)

- `02_Questions/PerChapter/QA_L37_MicrosoftFabric.md`
- `02_Questions/PerChapter/QA_L06_1_ML_Evaluation_Metrics.md`
- `02_Questions/PerChapter/QA_L13_1_ANN_Index_Internals.md`
- `02_Questions/PerChapter/QA_L15_1_Context_Engineering.md`

### Existing files to update

| File | Change |
|---|---|
| `07-GraphRAG-Neo4j/03_interview_qa.md` | Add explicit **Local Search vs Global Search** naming (content exists, term missing) |
| `01_Lessons/00_LearningIndex.md` | Register Part8 + 4 new sub-lessons |
| `00_CONTENTS.md`, `00_INDEX.md`, `00_MAP.md` | Add Part8 + new entries |
| `09_ML/MLEngineer_Coverage_2026-07-26.md` | Close gap **#73** (Medallion/lakehouse) — currently 🔴 |
| `08_Jobs/FDE/FDE-Prep_Tracker.md` | Close item **#60** (Mentoring) once behavioral bank exists |

---

## Part C — Execution Plan

### Phase 0 — Consolidate the workspace (20 min) ⬅️ do first

The material is scattered across three places and **there is no local git clone** — `C:\pers\AIML-Learn\` holds only 3 stale `.docx` files, so nothing can be committed today.

1. Clone the repo properly to a working path (recommend `C:\pers\AIML-Learn\` after clearing it, or confirm another path).
2. Move `Roadmap_Coverage_Check_2026-08-03.md` + this plan into `04_Career/`.
3. Copy `93db8f` (Fabric) and `641640` (Vector DB) into a temp `_Inbox/` inside the repo.
4. **Delete** the 4 redundant DeepSeek files.
5. Verify the 3 stale `.docx` in `C:\pers\AIML-Learn\` are already represented in `_Archive/RedundantCurriculumViews/` — they appear to be. Then delete.

**Outcome:** one canonical location, everything committable.

---

### Phase 1 — Behavioral & Leadership bank (3–4 hrs) 🔴 highest ROI

**Why first:** ~15% coverage, zero study required (it's writing down work you already did), and it's typically 25–40% of a Lead loop. All 132 existing InterviewBank questions are technical.

Create `InterviewBank/07_Behavioral_Leadership.md` with **10–12 STAR stories**, matching the existing InterviewBank format (question → WHAT/WHY/HOW → follow-up probe).

Source material you already have:
- **JM Family** production AI (referenced throughout L18, L27)
- **VitalCare** prior-auth platform (`05-VitalCare-AI-Platform/` — 25 C# files)
- **Ascendion** healthcare engagement (`08_Jobs/AscndIntr/`)
- FDE tracker item #60 — "Mentoring / transformation catalyst"

Stories to cover:
1. Mentoring a junior engineer through an AI project
2. Disagreeing with a stakeholder on RAG vs fine-tuning
3. A production incident you owned end-to-end
4. Driving AI adoption against organizational resistance
5. A project with quantified business impact (tie to the ROI framing already in `InterviewBank/05:218`)
6. A failure and what you changed afterward
7. Cross-functional conflict (product vs engineering)
8. Making a cost/latency trade-off call under pressure
9. Setting engineering best practices for a team
10. Explaining an AI limitation to a non-technical executive

Add a short **STAR framework** preamble (zero hits repo-wide today).

---

### Phase 2 — Microsoft Fabric module (5–7 hrs) 🔴 largest knowledge gap

Create `Part8_DataPlatform/L37_MicrosoftFabric.md`. Use the `5143a5` 7-section outline as the skeleton, but write real content — the DeepSeek stub is definitions only, with 5 unanswered questions.

Sections, and specifically what the stub is missing:

| § | Topic | Must add beyond stub |
|---|---|---|
| 37.1 | Fabric architecture & SaaS model | Workspace model, item types, why it's not just Synapse rebranded |
| 37.2 | OneLake | Shortcuts, ADLS Gen2 foundation, **one-copy principle**, Delta-Parquet as native format |
| 37.3 | Lakehouse vs Warehouse | **Decision criteria** (stub only defines Lakehouse), **Direct Lake mode** explained properly — it's the headline feature and gets one passing clause |
| 37.4 | Medallion Bronze/Silver/Gold | Worked example — what transformation actually happens at each hop |
| 37.5 | Dataflows Gen2 vs Pipelines vs Notebooks | **Decision table** — stub covers only Dataflows |
| 37.6 | Fabric ↔ Azure AI Foundry | How OneLake grounds an agent. **Directly relevant to your FDE/Ascendion work** |
| 37.7 | Governance & cost | **Capacity Units, F-SKUs, pause/resume, bursting/smoothing** — listed as topic #6 in stub with zero content. RLS/OLS is *asked* but never explained |

Then `QA_L37_MicrosoftFabric.md` — answer the stub's 5 questions in your 4-point format, plus ~10 more.

Finally close gap **#73** in `09_ML/MLEngineer_Coverage_2026-07-26.md`.

---

### Phase 3 — ML evaluation metrics (2 hrs) 🟡

Create `Part1_Foundations/L06_1_ML_Evaluation_Metrics.md`. **Nothing to harvest — DeepSeek's `07_ML_Evaluation_Metrics/` folder was planned but never generated.**

Currently `AUC` appears only as AutoML leaderboard output (`L06:295-298`); no lesson explains the curve.

Cover: confusion matrix → precision/recall trade-off → F1 vs F-beta → **ROC curve construction** → **why AUC flatters imbalanced classifiers** (use precision-recall curve instead) → threshold selection tied to business cost → cross-validation strategies.

**Anchor to a healthcare example** (false negative on a prior-auth denial ≫ false positive) — ties directly to VitalCare and the Ascendion client.

---

### Phase 4 — ANN index internals (1–1.5 hrs) 🟡

Create `Part3_GenAI_LLMs/L13_1_ANN_Index_Internals.md`. Harvest from `641640`, **applying both corrections above.**

Cover: HNSW (`M`, `ef_construction`, `ef_search` — and what each costs), IVF (`nlist`/`nprobe`), PQ compression + memory math, and the **recall / latency / memory triangle**. Keep the 4-way DB comparison but rewrite the Azure AI Search row from your own `L09:816`.

---

### Phase 5 — Context engineering (1.5–2 hrs) 🟡

Create `Part3_GenAI_LLMs/L15_1_Context_Engineering.md`. Assemble from pieces you already own — `HLP01_Memory_Tokens_Scaling_Agents.md` (context rot), `L23_CAG_vs_RAG.md` (caching), `L36` (token budget).

Add what's genuinely missing by name: **dynamic context assembly**, **multi-source fusion**, context budget allocation across system prompt + retrieved chunks + tool results + history, context poisoning, and compaction strategies.

---

### Phase 6 — Micro-fixes (45 min) ✅

1. **GraphRAG local-vs-global naming** (15 min) — add to `07-GraphRAG-Neo4j/03_interview_qa.md`. The content is already there (community summaries, "global" questions); only the *term* "local search" is absent.
2. Update `00_LearningIndex.md`, `00_CONTENTS.md`, `00_INDEX.md`, `00_MAP.md` for Part8 + 4 sub-lessons.
3. Close FDE tracker item #60.
4. *(Optional, 30 min)* AKS vs Azure Functions vs AWS Lambda serving comparison in `L18` — only if targeting multi-cloud.

---

## Summary

| Phase | Work | Effort | Status |
|---|---|---|---|
| 0 | Consolidate workspace, delete redundant files | 20 min | ✅ **DONE** — repo at `C:\pers\AIML-Learn`; 5 of 8 DeepSeek files deleted |
| 1 | Behavioral & Leadership bank | 3–4 hrs | ✅ **DONE** — `InterviewBank/07_Behavioral_Leadership.md`, 12 STAR stories |
| 2 | Microsoft Fabric module + Q&A | 5–7 hrs | ✅ **DONE 2026-08-03** — `L37_MicrosoftFabric.md` (883 lines, 9 sections) + `QA_L37` (17 Qs) |
| 3 | ML evaluation metrics + Q&A | 2 hrs | ⬜ **NEXT** |
| 4 | ANN index internals + Q&A | 1–1.5 hrs | ⬜ pending — source `641640` still in `deepseekLessons/` |
| 5 | Context engineering + Q&A | 1.5–2 hrs | ⬜ pending |
| 6 | Micro-fixes + index updates | 45 min | ✅ **DONE** — GraphRAG Q8a, all 4 indexes, FDE #60, ML #73 |
| | **Remaining** | **~5–6 hrs** | Phases 3, 4, 5 |

### Completion log

| Date | Done |
|---|---|
| 2026-08-03 | Phase 0, Phase 1 (commit `d0e03fa`) |
| 2026-08-03 | Phase 6 quick wins: GraphRAG **Local vs Global Search** added as Q8a (file now 16 Qs); FDE tracker item **#60 → 🟢**; DeepSeek `917f86` + `6f1547` deleted after harvest |
| 2026-08-03 | Phase 2: `Part8_DataPlatform/L37_MicrosoftFabric.md` + `QA_L37_MicrosoftFabric.md`; Part 8 registered in `00_LearningIndex.md`, `00_MAP.md`, `00_CONTENTS.md`, `00_INDEX.md` (35 new concepts); ML gap **#73 → 🟢**, **#72 → 🟢** |

**Still in `C:\pers\Resume-May2026\deepseekLessons\`:** `641640` (Phase 4 source), `93db8f` +
`5143a5` (Phase 2 sources — now harvested, safe to delete).

**After completion:** all 9 roadmap areas at interview-ready depth, from a single canonical repo, with the scattered DeepSeek material either absorbed or deleted.

**Note on sequencing:** Phase 1 needs no study — only recall and writing. If time is tight before an interview, Phase 1 + Phase 6 (≈5 hrs) closes the highest-risk gap and all quick wins.
