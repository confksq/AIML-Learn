# Session — 2026-08-03 · Part 8 (Microsoft Fabric) + Phase 6 micro-fixes

**Topic:** Resume interrupted work in `C:\pers\AIML-Learn` · execute Phase 2 (Microsoft Fabric
module) and Phase 6 (micro-fixes) of `Consolidation_and_Update_Plan_2026-08-03.md`
**Commits:** `93474a9` (Phase 2 + Phase 6) · `fa3eb86` (repo memory) · `e5883db` (PRDs + this
transcript + memory refresh) — all **local, not pushed to GitHub**
**Prior session ended at:** commit `d0e03fa` — window closed mid-Phase 2

---

## 1. Opening — "where did we leave off?"

Bala's window closed unexpectedly and he asked to resume work in `C:\pers\AIML-Learn`.

**The recall was not in memory.** Global memory (`MEMORY.md`) had entries for the Ascendion
interview prep, the FDE Juno Beach role and the user profile — but **nothing pointing at
AIML-Learn**, and `project_prep_progress.md` describes a different project (the June Ascendion
5-day plan). The repo carries its own `_ClaudeMemory/` folder, whose `project_ailearn_progress.md`
still gave the dead `C:\Users\confksq\Project\AIML-Learn\` path.

**How the state was actually reconstructed:**

| Signal | What it said |
|---|---|
| `git log` | last commit `d0e03fa` — behavioral bank + roadmap coverage analysis |
| `git status` | **clean** — nothing lost |
| Directory timestamps | `01_Lessons/Part8_DataPlatform/` created **empty at 21:28**, 3 min after the commit |
| `04_Career/Consolidation_and_Update_Plan_2026-08-03.md` | the 6-phase plan of record |

→ **Conclusion: interrupted at the start of Phase 2.** Phases 0 and 1 done, Part8 folder created,
nothing written into it yet.

**Lesson recorded to memory afterwards:** an empty, freshly-created directory is a strong resume
signal — it marks the exact point work stopped.

---

## 2. Approach decision

Bala said *"you choose the best approach."*

**Chosen:** do the Phase 6 quick wins that were **genuinely unblocked** first, then Phase 2 — and
save the rest of Phase 6 for *after* L37 existed.

**Reasoning:** Phase 6's index updates register Part 8 and L37 in four index files. Registering a
lesson that doesn't exist yet is backwards, and ML gap #73 can only be closed by the lesson that
closes it. So only two Phase 6 items were actually independent:

- GraphRAG Local vs Global naming (content existed, term missing)
- FDE tracker #60 (unblocked by Phase 1, which was already done)

---

## 3. Phase 6 quick wins

### 3.1 GraphRAG Local vs Global Search

Added **Q8a** to `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/07-GraphRAG-Neo4j/03_interview_qa.md`
(file now 16 questions). Placed directly after Q8 (Microsoft GraphRAG) rather than renumbering
Q9–Q15.

The substance:

- **Local Search** — entity-anchored. Maps the question to specific entities, pulls their
  neighbourhood (connected nodes, relationships, text units that mention them), answers from that
  subgraph. The mode for *"what does X do?"* — any question with a named starting point.
- **Global Search** — community-anchored. Map-reduce over the pre-computed community summaries:
  each summary contributes a partial answer, then reduces to one. The mode for *"what are the main
  themes?"* — questions with **no** entry entity, where the answer isn't in any single chunk.
- **Trade-off:** Local is cheap and precise but blind to corpus-wide patterns. Global reads every
  community summary, costs far more tokens, and is the reason graph construction pays for itself.

> **The point that matters:** if you only describe traversal, you've described Local Search and
> missed the half of GraphRAG vector RAG genuinely cannot do.

Checked the DeepSeek source (`917f86`) before deleting — it had only four one-line bullets, all
subsumed. Deleted, along with `6f1547` (fully harvested into the behavioral bank last session).

### 3.2 FDE tracker #60

`08_Jobs/FDE/FDE-Prep_Tracker.md` row 60 (Mentoring / transformation catalyst): 🔵 day job →
🟢 pointing at `InterviewBank/07_Behavioral_Leadership.md` Q1–Q3.

---

## 4. Phase 2 — `L37_MicrosoftFabric.md`

The largest gap in the library: roadmap area #6 scored **~5%** — OneLake, Lakehouse, Medallion,
Dataflows Gen2 and capacity/CU all at **zero hits** repo-wide.

Written from scratch in the L36 house style. The DeepSeek stub (`93db8f`) contributed a topic list
and nothing else — it was definitions-only with five unanswered questions. **883 lines, 9 sections,
40 topics.**

### Section map

| § | Content |
|---|---|
| 1 | SaaS model · **why it is not Synapse rebranded** · tenant→capacity→workspace→item hierarchy · the 7 workloads |
| 2 | **OneLake** — ADLS Gen2 underneath, `Files/` vs `Tables/`, one-copy principle, **shortcuts**, Delta-Parquet + **V-Order** |
| 3 | **Lakehouse vs Warehouse** decision criteria · **Direct Lake** and its DirectQuery fallback |
| 4 | **Medallion worked through** on VitalCare prior-auth — Bronze/Silver/Gold with real transformations |
| 5 | **Dataflows Gen2 vs pipelines vs notebooks** decision table · Mirroring · **incremental processing (4 mechanisms)** · Real-Time Intelligence |
| 6 | **Fabric ↔ Foundry** — four integration patterns, grounding agents on Gold |
| 7 | **Capacity Units**, pause/resume, **smoothing/bursting/throttling**, RLS/OLS/CLS, cost checklist |
| 8 | Cross-references to L06/L09/L13/L18/L20/L24/L28/L34/L36 |
| 9 | The 60-second interview answer |

### Key content decisions

**The framing that anchors the module:**

> Fabric is a *packaging* decision, not a new class of technology. Everything in it existed before —
> Spark, Delta Lake, Power Query, T-SQL, Power BI. What changed is that they now share one storage
> layer, one billing meter and one permission model.

**§1.2 — why not just Synapse rebranded.** The answer is the **copy count**. In a classic Synapse
estate the same fact table plausibly exists three times: Parquet in the lake for Spark, loaded into
the dedicated SQL pool, imported into a Power BI model. Three copies, three refresh jobs, three
chances to disagree at 8 a.m. Fabric's goal is one copy, many engines.

Also listed **what Fabric does not replace** — Databricks for heavy ML engineering, large ADF
estates with self-hosted IRs, OLTP. Naming where a Microsoft product loses reads as evaluation
rather than enthusiasm.

**§2.3 — shortcuts are why adoption is realistic at JM Family.** The story is *not* "migrate
everything." It's: shortcut the ADLS containers that already exist, leave the data physically where
it is, build Silver on top. Two caveats stated — external shortcuts use **stored connection
credentials** (a real delegation concern in a PHI estate), and a shortcut removes the copy, not the
distance (S3 shortcut from an East US capacity still pays latency and egress).

**§3.3 — Direct Lake, with the failure mode volunteered.** VertiPaq reads V-Ordered Delta-Parquet
straight from OneLake — Import speed, DirectQuery freshness, no refresh job. **The catch is
DirectQuery fallback**, which is silent: users report *"the report got slow this week."* Discipline:
plain Delta tables not layered views, monitor in Capacity Metrics, right-size the SKU, and consider
**disabling fallback so unsupported queries fail loudly** — a silent 10× slowdown is harder to
diagnose than an error.

**§4 — Medallion worked through, not recited.** Everyone can say "raw / cleaned / aggregated."
Interviews separate on *what transformation actually happens at each hop.* Three sources: EDI 278
transactions, faxed PA forms via Document Intelligence, and reference data.

- **Bronze** — append-only, never edited, with `_ingest_ts` / `_source_file` / `_batch_id`.
  **The argument:** when Silver logic turns out to be wrong you fix the code and re-run from Bronze.
  If you'd cleaned in place, recovery means asking the payer to resend six months of EDI. *Bronze is
  cheap storage buying you the right to be wrong.*
- **Silver** — typed, deduped, conformed (`"APPR"` / `"A1"` / `"Approved"` → one enum), SCD2,
  quarantined not dropped, PHI handling, and **confidence routing** for OCR fields. Fax-sourced and
  EDI-sourced requests land in the **same** table with a `source_channel` column.
  **The boundary rule:** Silver must be reproducible from Bronze by re-running code alone.
- **Gold** — consumer-shaped, deliberately denormalized. More than one Gold table for the same
  Silver data because there's more than one consumer — including a wide, text-rich table for agent
  grounding. Always state the **grain**; most production BI bugs are grain bugs.
- **§4.4 layout is a governance choice, not a technical one.** For healthcare: **three workspaces,
  one per layer**, because the workspace is the permission boundary and Bronze holds raw PHI.

**§5.5 — incremental processing.** "How do you handle incremental refresh?" means four different
mechanisms depending on layer: Dataflow Gen2 incremental refresh (needs query folding), notebook
**watermark + MERGE**, Delta **Change Data Feed**, Structured Streaming checkpoints. Direct Lake
makes the semantic-model case moot. **Trade-off:** incremental is cheaper but carries state that
can drift; full reload is expensive but always correct. Be incremental where volume forces it,
full-reload where you can afford correctness.

**§6 — the section that matters most for architect loops.**

> Fabric is where I make the data trustworthy; Foundry is where I make it answerable. The medallion
> architecture is what stands between a demo and a system I'd put in front of a clinician.

Four patterns: Gold→AI Search→RAG · **SQL tool over the SQL analytics endpoint** · Fabric data agent
as a delegated tool (the L28 meta-agent pattern with a Microsoft-supplied specialist) · the full
unstructured chain.

**The routing rule:** *anything with a correct numeric answer goes to SQL; semantic and policy
questions go to retrieval.* "How many prior auths did Payer X deny last quarter?" is **not** a RAG
question — vector search returns plausible passages and the model produces a plausible, wrong
number. That's the L24 agentic hallucination failure.

**§6.3 — the governance question that separates candidates:**

> *"Your Gold table has RLS so a clinician only sees their own patients. Does the agent respect it?"*

**Only if you designed for it — by default, almost certainly not.** A service principal with blanket
read bypasses RLS entirely. Worse, content **copied into an AI Search index** has no idea RLS ever
existed — the copy silently discards the security model rather than failing. Mitigations in order:
identity passthrough → security trimming with a **mandatory server-side filter** → physical
separation → layered enforcement.

**The rule that must not bend:** never let the model choose the security filter. A filter the LLM
can influence is not a security control — it's a prompt-injection target.

**§7.3 — smoothing/throttling, with the failure story.** Bursting lets a job exceed baseline CU;
smoothing spreads the consumption (≈24h background, minutes interactive). **But smoothing defers
usage, it does not forgive it** — sustained over-consumption becomes **carry-forward debt** and
throttles in stages: interactive delay → interactive rejection → background rejection. User-facing
degradation comes *first*, deliberately.

> A data-science team runs a large exploratory Spark job Friday afternoon on the shared production
> capacity. It bursts, succeeds, everyone goes home. Consumption smooths across the weekend,
> Monday's scheduled refreshes stack on the carry-forward, and by 9 a.m. the executive dashboards
> are throttled. **Nobody did anything obviously wrong** — which is why capacity isolation is a
> design decision, not an afterthought.

**§7.4 — the RLS/OLS/CLS trap.** RLS on a semantic model protects **Power BI only**. It does not
protect the SQL analytics endpoint (needs its own `CREATE SECURITY POLICY`), and neither protects
direct OneLake file access. **Three doors, one copy** — that's the price of the one-copy principle.

### Corrections applied from the plan

Per the ⚠️ table in `Consolidation_and_Update_Plan_2026-08-03.md`:

- ❌ Dropped the "180% YoY Fabric demand" / "495% Responsible AI" figures — unverifiable, and
  quoting a fabricated stat in an interview is a real risk.
- ❌ Did not repeat "Azure AI Search — limited vector capabilities." Framed from `L09` instead.
- ✅ Rewrote "Fabric IQ: AI-powered assistant" — replaced with the **Fabric data agent** framing.
- ⚠️ Flagged that **V-Order defaults have shifted across releases** — verify the current default in
  MS Learn before quoting one; the *reasoning* (write cost buys read speed) is what's stable.

### `QA_L37_MicrosoftFabric.md` — 17 questions

Q1–Q5 answer the five the stub posed and never answered (Medallion · incremental refresh ·
Lakehouse vs Warehouse · Foundry integration · security model). Q6–Q17 are the follow-ups an
interviewer actually asks — Synapse comparison, OneLake/shortcuts, Direct Lake, the three-way tool
choice, capacity mechanics, cost, **the RLS/agent question**, small-file problem, Document
Intelligence placement, Git/deployment pipelines, **when NOT to adopt Fabric**, and the 60-second
version.

All in the house 4-point format: *what it IS → why it works that way → healthcare/JM Family example
→ the trade-off or when not to use it.*

---

## 5. Phase 6 — registration and gap closure

**Four indexes updated:**

| File | Change |
|---|---|
| `01_Lessons/00_LearningIndex.md` | Part 8 section · L37 row · quick-lookup row · `QA_L32`–**`QA_L37`** · footer date |
| `00_MAP.md` | Part 8 section (883 lines, 40 topics) · PerChapter range · InterviewBank/07 row |
| `00_CONTENTS.md` | Full Part 8 heading tree with line numbers |
| `00_INDEX.md` | **35 new concept entries**, alphabetically inserted via script; header 1,145 → 1,180 concepts |

Concepts added include: Bronze/Silver/Gold layers, Capacity Units, carry-forward debt, Change Data
Feed, CLS/OLS, Dataflows Gen2, Direct Lake, DirectQuery fallback, F-SKU, Fabric data agent,
**Global Search / Local Search (GraphRAG)**, grain, Lakehouse, Medallion, Mirroring, one-copy
principle, OneLake, pause/resume, security trimming, shortcuts, small-file problem, smoothing,
**STAR framework**, throttling stages, V-Order, Warehouse, watermark+MERGE, workspace.

**ML coverage gaps closed** in `09_ML/MLEngineer_Coverage_2026-07-26.md`:
- **#73 Medallion / lakehouse** 🔴 → 🟢
- **#72 Delta / Parquet** 🟡 → 🟢
- Priority-4 build row annotated: L37 closes 72–73 and advances 70–71; remaining = Airflow/Dagster
  (68), Kafka (69), feature store (74), data contracts/GE (75), DVC (76).

**Deliberately not over-claimed:** row 71 (ADF/Synapse) left at 🟡 — L37 explains the comparison but
does not build a pipeline.

---

## 6. PRD updates

**`04_Career/00_PRD.md`** (interview question bank) — Draft → ✅ **DELIVERED**:
- Added delivery-status table: 7 tiers, **144 questions** actual vs ~87 target
- Tier 7 (Behavioral & Leadership) added to the structure table
- Noted Tier 7 uses **STAR**, not WHY/HOW/WHEN/SCALE/DEPLOY — forcing the technical lens onto
  "tell me about a time you disagreed with a stakeholder" produces a technical essay, which is
  exactly the failure mode those rounds screen for
- **All 5 open questions marked resolved.** #3 ("any tier to add — e.g. behavioral?") was answered
  by this work: yes, add it

**`04_Career/PRD_Bala_AI_Career_Acceleration.md`** (v2.1, CareerAccelerator build spec) — marked
✅ DELIVERED 9/9 with a **PATH CORRECTION block**, because every path in it was dead:

| v2.1 said | Actually |
|---|---|
| repo `confksq/Learning` | `confksq/AIML-Learn` |
| `Project/AIML-Learn/PartsModules/CareerAccelerator/` | `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/` |
| `C:\Users\confksq\Project\AIML-Learn\` | `C:\pers\AIML-Learn\` |
| `Questions/` | `02_Questions/InterviewBank/` (7 tiers) |

---

## 7. Memory updates

**Repo memory** (`_ClaudeMemory/`):

- **`project_ailearn_progress.md`** — corrected root path to `C:\pers\AIML-Learn\` (now a real git
  clone); status updated to L01–L37 across 8 Parts; phase status recorded; added the **four-index
  registration rule** and a warning that both PRDs carry stale paths.
- **`feedback_save_chat.md`** — the AIML chat-history path in it was **also stale**
  (`/mnt/c/Users/confksq/Project/AIML-Learn/`). Corrected to `/mnt/c/pers/AIML-Learn/07_ChatHistory/`
  and added the "also update `INDEX.md`" step, since skipping it is what left that index stale three
  separate times.

**Global memory** (`~/.claude/.../memory/`):

- **`project_aiml_learn_library.md`** (new) + `MEMORY.md` index line — the library was **missing
  from global memory entirely**, which is precisely why resuming required state reconstruction
  instead of a lookup. Points at `00_START_HERE.md` and the repo's own `_ClaudeMemory/` as the live
  sources, and records the four-index rule.
- **`feedback_resume_from_repo_state.md`** (new) — the reconstruction sequence from §1 written down
  as a reusable method: git log → git status → directory timestamps → plan-of-record doc. Core
  principle: **the repo is the truth; memory is a pointer to where to look.**

### 7.1 Stale-path pattern worth noting

Three separate stored paths were dead this session, all pointing at the pre-2026-07-18
`C:\Users\confksq\Project\AIML-Learn\` layout:

| Stale in | Pointed at |
|---|---|
| `_ClaudeMemory/project_ailearn_progress.md` | old library root |
| `_ClaudeMemory/feedback_save_chat.md` | old `07_ChatHistory/` |
| both `04_Career/` PRDs | old repo name **and** the deleted `PartsModules/` tree |

All four corrected. The pattern: **path-carrying notes rot silently after a reorg** — anything
recording a location needs re-checking whenever the tree moves, because nothing errors until someone
executes it literally.

---

## 8. Plan status at session end

| Phase | Status |
|---|---|
| 0 · Consolidate workspace | ✅ |
| 1 · Behavioral & Leadership bank | ✅ |
| 2 · Microsoft Fabric | ✅ **this session** |
| 6 · Micro-fixes + index updates | ✅ **this session** |
| **3 · ML evaluation metrics** (ROC/AUC, F1 vs F-beta, thresholds) | ⬜ **next · 2 hrs** |
| 4 · ANN index internals (HNSW/IVF/PQ) | ⬜ 1–1.5 hrs — source `641640` still in `deepseekLessons/` |
| 5 · Context engineering | ⬜ 1.5–2 hrs |

**Both major roadmap gaps closed.** Coverage: Fabric ~5% → ~90%, Behavioral ~15% → ~90%, GraphRAG
~85% → ~95%. ~5–6 hrs remain, all 🟡 tier.

**Still in `C:\pers\Resume-May2026\deepseekLessons\`:** `641640` (Phase 4 source), `93db8f` +
`5143a5` (harvested, safe to delete).

---

## 9. Session close — "update the prd and memory, we can do this tomorrow, save the chat too"

Bala called the session at this point. Three closing actions, covered in §6, §7 and this file
itself.

**Corrections made while closing:**

- `07_ChatHistory/INDEX.md` claimed **5** sessions were unindexed; the real count is **7** (four
  from 2026-07-29, three from 2026-08-02). Header also refreshed: 47 files / ~4.4 MB → **48 files /
  ~5.1 MB**. Those 7 still have no topic summaries — a small cleanup job outstanding.
- `09_ML` row 71 (ADF / Synapse) was left at 🟡 rather than upgraded. `L37` explains the Synapse
  comparison and frames pipelines as ADF-in-Fabric, but it does not *build* a pipeline — closing
  that row would have been over-claiming.

---

## 10. Open items

1. **No Q&A drill on L37 yet.** Per the Option C approach the teaching is written and the drill
   comes after Bala has read it. `QA_L37` has 17 questions ready.
2. **Drill Tier 7 (behavioral) out loud** — STAR answers degrade badly when only read.
3. **7 unindexed transcripts** in `07_ChatHistory/` (2026-07-29 ×4, 2026-08-02 ×3).
4. **Nothing pushed to GitHub** this session — four local commits ahead of `origin`.
5. **Next up: Phase 3** — `Part1_Foundations/L06_1_ML_Evaluation_Metrics.md`, ~2 hrs, written from
   scratch (the DeepSeek source for it was planned but never generated). Confusion matrix →
   precision/recall trade-off → F1 vs F-beta → ROC construction → **why AUC flatters imbalanced
   classifiers** → threshold selection tied to business cost → cross-validation. Healthcare anchor:
   a false negative on a prior-auth denial costs far more than a false positive.
