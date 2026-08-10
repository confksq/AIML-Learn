# Session — Interview Q&A Build: Resume Defence, Real-World Bank, Drill Index, Bible Audit

**Dates:** 2026-08-08 → 2026-08-10
**Repo state at end:** commit `2a86360` on `main`
**Plan of record:** `02_Questions/00_PLAN_InterviewQA_2026-08-08.md`

---

## What was asked

Two question banks — one from the resume (`C:\pers\Resume-May2026\Bala_K_Lead_AI_Engineer_AI-103.docx`),
one from the AIML-Learn lessons — with detailed answers, plus coverage of fourteen questions
actually asked across the last five interviews:

> *"How is memory managed, how you train document intelligence documents, 1 million documents
> how you design AI search, which models you choose why, how you manage context window if it
> grows, what type of compression you use, design the life cycle of RAG, why and when you use
> AKS keda auto scale for AI, which chunking strategy is best and which one to choose, how pii
> management, how token saving we can do, why A2A, explain entire agent process you
> implemented, explain each component of azure foundry."*

Standing constraint, restated repeatedly: **no duplicates.**

---

## What changed about the plan, and why

The original plan had four phases and ~155 new questions, including a ~90-question
lessons-based file. Two things killed that.

**1. The corpus was far larger than assumed.** A scripted sweep found **1,335 question-like
entries across 131 files** — not the ~50 files initially estimated. `PerChapter/` already
covered L06–L21 and L32–L37 at 10–25 questions each, and L22–L31 carried 45 questions inline
in the lesson files. The lessons-based file would have been duplicate work and was
**cancelled**.

**2. Three findings redirected the effort:**

| Finding | Consequence |
|---|---|
| **Zero** of the 1,335 questions ask about any resume number — 500K docs, 95% retrieval, $150K, $300K, 35% GraphRAG, 300+ users | Resume-defence file confirmed as highest value |
| `InterviewBank/07_Behavioral_Leadership.md` had **44 `[FILL:]` placeholders** — the 12 STAR stories were skeletons | New phase added |
| 1,161 unique questions across 131 files in five formats is unusable as a drill set | Drill index added |

Final shape: **three phases, ~88 new questions**, not four phases and 155.

---

## Deliverables

| File | Size | What |
|---|---:|---|
| `02_Questions/Interview_QA_Resume_Based.md` | 70 Q / 4,790 lines | Every resume claim defended, 12 sections |
| `02_Questions/Interview_QA_RealWorld_Asked.md` | 18 Q / 1,847 lines | The 14 asked + 4 companion deep-dives |
| `02_Questions/00_DRILL_INDEX.md` | 181 lines | Single entry point over all 810 curated questions |
| `02_Questions/Interview_Bible_77Q_AUDIT_2026-08-10.md` | 269 lines | Findings on the parallel-session Bible |
| `02_Questions/00_PLAN_InterviewQA_2026-08-08.md` | 289 lines | Plan, coverage matrix, dedup rules |
| `02_Questions/_tooling/` | 2 scripts | `qextract.py` + `qdedup.py` |
| `InterviewBank/07_Behavioral_Leadership.md` | +136 lines | Corrected and converted to a worksheet |

### Answer format used throughout

Seven parts per question: **what they're testing · a literal 60-second spoken answer · the
four-point rule** (what it IS / why it works that way / your example with the number / the
trade-off) **· whiteboard where it earns its place · follow-up probes with answers · a red
flag** describing what a weak answer sounds like.

The four-point rule is the standing learning agreement — see `feedback_learning_approach`
in the global memory folder.

---

## The four confirmed coverage gaps

Lexical matching of the 14 asked questions against all 1,335 existing ones, verified by eye
(the matcher under-reports: chunking scored 0.25 but was genuinely covered).

| Gap | Best existing match | Now covered by |
|---|---|---|
| 1M-document AI Search design | 0.25 — a *10M-chunk latency triage* question at `L09:700` | `RealWorld` Q3 + Q15 |
| Context compression | 0.22, unrelated | `RealWorld` Q6 + Q16 |
| KEDA for AI workloads | **0.12**, unrelated. `L34` teaches Kubernetes, never connects it to AI | `RealWorld` Q8 + Q17 |
| PII management | 0.17, top match was about LlamaIndex evaluation | `RealWorld` Q10 + Q18 |

---

## Duplication control

- Both new banks scored **0 violations at ≥0.62 Jaccard** against the 1,335 pre-existing
  questions. Closest: 0.57 (`Resume` Q36 vs an `L24` prompt-injection question) and 0.50
  (`Resume` Q51 vs `InterviewBank/02`) — both the deliberate kind, where the existing question
  asks the concept and the new one asks what *you* did.
- Existing corpus redundancy measured at **13% (174 questions)**, and it is almost entirely
  file-level: `Session_ChatHistory_2026-06-21.md` is copied wholesale into
  `08_Jobs/AscndIntr/PrepPlan/AscendionPrep_Day3_Module04-05_*.md` at identical line numbers,
  and `Session_ChatHistory_2026-06-08/09/10.md` are three copies of one file. **The curated
  banks are clean** — only two genuine cross-file collisions in ~500 curated questions.
- Ownership rule established: **one canonical answer per topic, everything else
  cross-references it.** `RealWorld` owns the general framework; `Resume_Based` owns anything
  carrying a number or a named project; lessons and `HLP01` stay authoritative for long-form
  teaching.

---

## Three integrity findings

These mattered more than the question count.

### 1. Two resume claims invite unwinnable challenges

| Current wording | Problem |
|---|---|
| *"eliminating hallucinations"* | Not achievable. A competent interviewer asks **because** it isn't. `Resume` Q12 recommends: *"reducing ungrounded responses, measured via RAGAS faithfulness against a fixed evaluation set"* |
| *"implementing Model Context Protocol (MCP) standards… to ensure PII redaction and compliance"* | Reads as though MCP is a compliance standard. It is a tool-connection protocol. `Resume` Q35 recommends splitting the claim in two |

### 2. The behavioural file's cost story contradicted the resume

`InterviewBank/07` Q6 and Q11 quoted **"$345 → $21/month, ~$56/month total, 94% reduction"**
as achievements. Traced to `L18_AISolutionArchitecture.md:345-355` — a **worked
token-arithmetic example** demonstrating the price gap between two models. Teaching material,
not telemetry.

The resume claims **30% / ~$150K annually**. You cannot save $150,000 a year on a system that
costs $672 a year to run. Both stories could not be told. Q6 and Q11 were rewritten to the
resume framing; the mechanisms (tiering, caching, top-K reduction, embedding caching) were
correct and retained.

### 3. `Interview_Bible_77Q` needs work before it is quotable

A parallel session committed `Interview_Bible_77Q_FDE_AI_Lead.md` (77 Q, converted from
`BALA K - ULTIMATE FDEAI LEAD INTERV.txt`) mid-build. Its Section B covers the **same 14
questions**, four word-identical. Full audit written; **no changes made to the Bible**.

- **6 factual errors** — invented Azure AI Search tier limits including a nonexistent **S4
  tier**; a partition-key/routing-key mechanism the service does not expose; embedding storage
  maths off by ~10×; MCP described as a governance standard; A2A framed as the alternative to
  a monolithic agent; "confidence interval (0-100)" on LLM output.
- **4 internal contradictions** — the same 78%→95% attributed to both the cross-encoder (Q1)
  and the chunking strategy (Q3); the same $300K attributed to both KEDA (Q13) and the
  monolith migration (Q14); two different cheap models in the routing story; **two competing
  production-incident stories** (index corruption vs TPM quota exhaustion in `InterviewBank/07`).
- **~20 unfalsifiable precise numbers**, headed by *"saved exactly $152,300"*. Three that
  cannot be true as stated: *"zero hallucination"*, *"zero successful prompt injection attacks
  over 12 months"*, *"the vector index contained zero PII"*.

Overlap resolved by **declared split**, documented at the top of `RealWorld` with a full
question-mapping table: **Bible = spoken-answer layer, RealWorld = drill layer.**

---

## Outstanding — what the next session picks up

| Priority | Item | Where |
|---|---|---|
| 1 | **22 `[CONFIRM:]` items** — eval set size and *k* for the 95% claim, which cost lever produced most of the 30%, MCP built vs consumed, custom DocIntel models vs layout+LLM, multi-hop query share | `Resume_Based` Appendix A |
| 2 | **3 missing behavioural stories** — cross-functional conflict, significant failure, ambiguous requirements | `InterviewBank/07` worksheet Tier 1 |
| 3 | **15 outcome facts** — incl. ⚠ *was the TPM incident real or designed-against?* | `InterviewBank/07` worksheet Tier 2 |
| 4 | **Bible Tier 3** — resolve the four contradictions (free; choosing between stories that already exist) | Audit §Tier 3 |
| 5 | **Bible Tier 1** — six factual errors, each with a correct replacement already written | Audit §Tier 1 |
| 6 | **Bible Tier 2** — triage ~20 numbers: can you say how it was measured? | Audit §Tier 2 |
| 7 | Two resume edits | `Resume_Based` Appendix B |

**Not covered by this repo at all, and stated plainly in `00_DRILL_INDEX.md` §6:** writing
code under observation, and timed spoken rehearsal. Everything built here is silent-read
material.

---

## Provenance note

`BALA K - ULTIMATE FDEAI LEAD INTERV.txt` remains untracked at repo root. Given the audit
findings — invented tier limits, precise-to-the-dollar figures — establishing where that
export came from matters. If it was generated rather than written from notes, the whole file
needs Tier 2 triage before any of it is rehearsed.
