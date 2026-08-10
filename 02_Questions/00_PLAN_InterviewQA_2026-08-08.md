# Interview Q&A Build — Plan of Record

**Created:** 2026-08-08
**Owner:** Bala Kittappa — Lead AI Engineer / Forward Deployed Engineer
**Source resume:** `C:\pers\Resume-May2026\Bala_K_Lead_AI_Engineer_AI-103.docx`
**Source curriculum:** `C:\pers\AIML-Learn\01_Lessons\` (L01–L37, 8 Parts)
**Output location:** `C:\pers\AIML-Learn\02_Questions\` (this folder, flat)

---

## 1. Why this build exists

Three things are true at once:

1. **The resume carries ~30 quantified claims** (500K+ documents, 95% retrieval accuracy,
   30% / $150K cost reduction, 40% / $300K infra reduction, 35% GraphRAG lift, 100K tax
   filings, $1B+ transactions, 12+ hrs/week saved, 300+ business users, 200+ concurrent
   users). Every one of these is a question hook. **None of them are currently drilled
   anywhere in this repo.** An interviewer who reads the resume will attack the numbers;
   there is no prepared defence.

2. **The existing question banks are correct but not speakable.** `02_Questions/InterviewBank/`
   is 7 files / 2,183 lines in a terse `WHY / HOW / WHEN / SCALE / DEPLOY` bullet format.
   It is good revision material and bad rehearsal material — no spoken answer, no personal
   project anchor, no failure-mode framing.

3. **Real interviews have already exposed the gaps.** Fourteen questions actually asked
   across the last five interviews are catalogued in §3. Six of them have no adequate
   source anywhere in the curriculum.

---

## 2. Current state of `02_Questions/`

| Asset | Files | Lines | Verdict |
|---|---:|---:|---|
| `HighLevelPrep/HLP01_Memory_Tokens_Scaling_Agents.md` | 1 | 361 | **Strongest existing asset.** Memory · tokenization · scaling · agents, at interview altitude. Keep, cross-reference, do not duplicate. |
| `InterviewBank/01`–`07` | 7 | 1,822 | Concept-correct, bullet-thin. Becomes the *revision* layer under the new *rehearsal* layer. |
| `PerChapter/QA_L06`–`QA_L21`, `QA_L32`–`QA_L37` | 33 | — | Self-test per lesson. Fine as-is. |
| `PerChapter/` — **missing** | — | — | **No Q&A for L01–L05 and L22–L31.** L22–L31 is the entire agentic block: Foundry agent lifecycle, CAG vs RAG, hallucination, framework comparison, MCP, agent workflow, meta-agents, A2A, OCR pipelines, fault tolerance. This is the exact material the last five interviews probed. |

---

## 3. The fourteen questions actually asked — coverage matrix

Collected from the last five interviews. Ranked by how often this class of question recurs.

| # | Question as asked | Best existing source | Coverage | Action |
|---:|---|---|---|---|
| 1 | How is memory managed? | `HLP01` §1 (four layers, five strategies, episodic) | ✅ Strong | Convert to spoken answer + JM Family anchor |
| 2 | How do you train Document Intelligence documents? | `L08_DocumentIntelligence` | ⚠️ Lesson only | **Build:** custom vs prebuilt vs neural, labelling volume, train/test split, confidence thresholds, retraining trigger |
| 3 | 1 million documents — how do you design AI Search? | `L09_AzureAISearch` | ❌ **Gap** | **Build:** partition/replica maths, SU sizing, index size limits, tiering, ingestion throughput, incremental indexers, cost envelope |
| 4 | Which models do you choose and why? | `L12`, `L17` | ⚠️ Thin | **Build:** decision table — task class × latency × cost × context × compliance; the routing/tier-selection story from JM Family |
| 5 | How do you manage the context window if it grows? | `HLP01` §2, `L15` | ⚠️ Partial | **Build:** the eviction ladder, rolling window, summarisation buffer, hierarchical memory, retrieval-instead-of-stuffing |
| 6 | What type of compression do you use? | — | ❌ **Gap** | **Build:** prompt compression vs context compression vs conversation summarisation vs semantic dedup vs reranker-as-compressor; LLMLingua-class approaches; lossy/lossless framing |
| 7 | Design the lifecycle of RAG | `L13_RAG_DeepDive`, `IB03` | ✅ Strong | Convert to a whiteboard-able 8-stage lifecycle with ownership and eval gates |
| 8 | Why and when do you use AKS + KEDA autoscale for AI? | `L34`, resume bullet | ❌ **Gap** | **Build:** why HPA-on-CPU fails for LLM workloads, queue-depth scaling, scale-to-zero for GPU/embedding jobs, cold-start trade-off, KEDA scalers actually used |
| 9 | Which chunking strategy is best, and which do you choose? | `IB03` Q1–Q2, `L13` | ✅ Strong | Convert to spoken answer + "there is no best, here is the decision rule" |
| 10 | How do you manage PII? | `IB06` | ⚠️ Thin | **Build:** detect → redact → tokenise → re-hydrate flow; where in the pipeline each happens; Content Safety vs PII detection vs custom NER; audit + right-to-erasure |
| 11 | How can we save tokens? | `HLP01` §2 | ✅ Strong | Convert; add the $150K/30% JM Family story as proof |
| 12 | Why A2A? | `L29_A2A_Protocol` | ❌ **Gap** (no Q&A file) | **Build:** what problem A2A solves that MCP does not; agent cards, task delegation, discovery; when a function call is enough |
| 13 | Explain the entire agent process you implemented | `L27` (11-step centerpiece) | ⚠️ Not in his voice | **Build:** the 4–5 minute narrative anchored to JM Family production, not the generic lesson |
| 14 | Explain each component of Azure AI Foundry | `L17`, `L22` | ❌ **Gap** | **Build:** component-by-component — Hub, Project, Model Catalog, Deployments, Agent Service, Prompt Flow, Evaluations, Content Safety, Connections, Compute, Tracing — what each *is*, when you touch it, what it costs |

**Six real gaps: #3, #6, #8, #10 (partial), #12, #14.** All six cluster in exactly the area
the market is hiring for — scale design, cost control, and agent protocols.

---

## 4. Resume claim → question hook map

Every claim below becomes at least one question in File 1. Claims marked ⚠️ are the ones
most likely to draw a hostile follow-up, because they are the ones a skilled interviewer
knows are hard to achieve.

### JM Family Enterprise — Lead AI Engineer (Jun 2024 – Present)

| Resume claim | Question hooks |
|---|---|
| Production RAG, 500K+ finance/insurance docs, Foundry SDK + LangChain + GPT-4o | Architecture walkthrough; why LangChain *and* Foundry SDK; ingestion design at 500K; re-index strategy |
| ⚠️ **95% retrieval accuracy** via hybrid vector/keyword | *How did you measure it?* — this is the single most attackable number on the resume. Needs recall@k, the eval set, who labelled it, what the other 5% were |
| ⚠️ "eliminating hallucinations" | No one eliminates hallucinations. Needs restating as measured groundedness + the residual failure mode you still watch |
| 60% manual search time reduction, 300+ users | Baseline methodology; how measured; self-reported or instrumented |
| Multi-agent orchestration — Foundry + LangGraph + crewAI 1.15, dynamic function-calling | Why three frameworks; what each does; where the boundary is; why not one |
| 12+ hrs/week saved across 50+ users | Same measurement challenge |
| LLMOps with RAGAS 0.4 — faithfulness, answer relevance, context recall | What each metric actually computes; what threshold gates a release; what happens on regression |
| ⚠️ **30% inference cost cut, ~$150K/yr** — token budget mgmt, model tier selection, Azure Monitor | The full cost-engineering story. Highest-value question on the resume for a cost-conscious hiring manager |
| Responsible AI — MCP standards, prompt injection defence, Content Safety, PII redaction | MCP is a protocol, not a compliance standard — phrasing needs care; injection defence depth; PII flow |
| AKS + KEDA autoscaling; Ollama 0.6 (LLaMA 3) + LlamaIndex 0.14 air-gapped fallback; 40% ingestion latency cut | Ties directly to asked-question #8; why local fallback; what triggers failover; how you keep two model behaviours consistent |
| Mentoring junior AI engineers | Behavioural / leadership |

### KPMG — Lead AI Engineer (Sep 2021 – Jun 2024)

| Resume claim | Question hooks |
|---|---|
| 500K+ contracts/yr — Python, GPT-4, HF Transformers, Document Intelligence; 60% cycle-time cut | Ties to asked-question #2; where DI ends and the LLM begins; error handling on bad scans |
| ⚠️ **GraphRAG + Neo4j 5.x + AI Search, 35% retrieval accuracy lift**, multi-hop, 200+ concurrent | When graph beats vector; how the graph was built; entity extraction; maintenance cost; the 35% measurement |
| 20+ .NET monoliths → AKS microservices, 40% / $300K infra cut, zero-downtime | Migration strategy; strangler pattern; how zero-downtime was proven |
| Event-driven ingestion (ADF + Synapse); LoRA/QLoRA fine-tuning (PEFT) for contract classification; 35% error reduction | When fine-tune beats RAG; LoRA rank/alpha choices; dataset size; eval metrics used |

### ADP / Assurant — Cloud Architect & AI Integration Lead (Jun 2019 – Aug 2021)

| Resume claim | Question hooks |
|---|---|
| Bedrock (Claude 3, Titan) + Foundry, 100K+ tax filings, 70% manual classification cut | Multi-cloud AI rationale; Bedrock vs Azure OpenAI honest comparison; data residency |
| Terraform + Bicep, gated CI/CD, 20+ apps, SOC 2, $1B+ transactions | IaC choice; what "gated" means; compliance evidence |

### Cross-cutting

Certifications (AI-102 / AI-103 / AZ-204) · traditional ML fundamentals (the resume claims
them; expect a probe on bias–variance, ROC-AUC, cross-validation) · polyglot .NET + Python
(expect "which do you actually write?") · Microsoft Fabric / OneLake · FDE positioning
(embedded delivery, client-facing, ambiguity).

---

## 5. Deliverables

All three files land in `C:\pers\AIML-Learn\02_Questions\`.

| File | Questions | Purpose |
|---|---:|---|
| `Interview_QA_Resume_Based.md` | ~65 | Defend every claim and number on the resume, in your own voice |
| `Interview_QA_Lessons_Based.md` | ~90 | Concept depth across L01–L37 — the framework-agnostic knowledge layer |
| `Interview_QA_RealWorld_Asked.md` | 14 | Maximum depth on the questions proven to be asked, plus a 1-page cheat sheet and a drill schedule |

### Answer format — every question, no exceptions

```
### Q<n>. <the question exactly as an interviewer would ask it>

**What they're testing:** one line — the real intent behind the question

**60-second spoken answer:**
> The literal words to say. Complete sentences. Deliverable from memory.

**Depth — the four-point rule:**
1. **What it IS** — the definition, precisely
2. **Why it works that way** — the mechanism, not the marketing
3. **Your example** — JM Family / KPMG / ADP with the real number attached
4. **The trade-off** — what it costs, and when you would NOT do this

**Whiteboard:** diagram, table, or formula — only where it earns its place

**Follow-up probes:**
- "<probe>" → <short answer>
- "<probe>" → <short answer>

**Red flag:** what a weak answer to this question sounds like — so you can hear
yourself drifting toward it and stop.
```

The four-point rule is non-negotiable per the standing learning agreement: every answer
must hit *what it is · why it works that way · a concrete example · the trade-off*.

---

## 6. Build order

| Phase | Work | Output |
|---|---|---|
| **0** | Coverage matrix + plan of record | **this file** ✅ |
| **1** | `Interview_QA_Resume_Based.md` — 6 sections, ~65 Q | Drillable while Phase 2 is still being written |
| **2** | `Interview_QA_Lessons_Based.md` — by Part, ~90 Q. Weighting: Part 3 (RAG/LLM internals) and Part 5 (agentic protocols) heaviest; Part 1 and Parts 6/8 lightest | Concept layer |
| **3** | `Interview_QA_RealWorld_Asked.md` — the 14 at maximum depth + cheat sheet + drill schedule | Highest-value rehearsal |
| **4** | Register all three in `00_MAP.md`, `00_CONTENTS.md`, `00_INDEX.md`, `01_Lessons/00_LearningIndex.md`; commit and push to `github.com/confksq/AIML-Learn` | Repo hygiene |

**Optional Phase 5 (recommended, not scheduled):** backfill the missing
`PerChapter/QA_L22` – `QA_L31` files. The new Phase 1–3 files will cover this material at
interview altitude, but the per-lesson self-tests would still be absent for the entire
agentic block.

---

## 7. Weighting decision for File 2

**SUPERSEDED 2026-08-08 — see §9.** The standalone lessons file was cut after a full-repo
question extraction showed the coverage it assumed did not exist. Section retained only to
record what was originally planned and why it was dropped.

---

## 9. REVISION — 2026-08-08, after full-corpus extraction

### What the extraction found

A scripted sweep of every `.md` in the repo (`qextract.py` → `qdedup.py`, both kept in
`02_Questions/_tooling/`) produced hard numbers that invalidated §5 and §7:

| Measure | Value |
|---|---:|
| Question-like lines across the repo | **1,335** |
| Files containing questions | **131** (not the ~50 previously assumed) |
| Near-duplicate clusters | 127 (120 spanning >1 file) |
| Redundant questions | 174 — **13% of corpus** |
| Unique topics already covered | **1,161** |

**The duplication is file-level, not thinking-level.** `07_ChatHistory/Session_ChatHistory_2026-06-21.md`
is copied wholesale into `08_Jobs/AscndIntr/PrepPlan/AscendionPrep_Day3_Module04-05_...md`
(identical questions at identical line numbers 1539 / 2082 / 2440 / 2534 / 2573–2577).
`Session_ChatHistory_2026-06-08 / -06-09 / -06-10.md` are three copies of one file.

**The curated banks are clean** — only two genuine cross-file collisions in ~500 curated
questions: "Prompt Flow vs Semantic Kernel" (`QA_L16:85` + `QA_L17:50`) and "Why masked
attention in decoder" (`L11_1:177` + `QA_L11_1:48`).

### Three findings that changed the plan

1. **Resume claims have zero coverage.** Grepping all 1,335 questions for `500K`, `95%`,
   `150K`, `300K`, `100K filings`, `300+ users`, `200+ concurrent` returns **no question
   anywhere**. The most-read part of the resume is entirely undrilled. → Phase 1 stands.

2. **The lessons file would have been duplicate work.** `PerChapter/` already covers
   L06–L21 and L32–L37 at 10–25 questions each; L22–L31 carry 45 questions inline in the
   lesson files themselves. → **Standalone lessons file cut.** Only the four confirmed
   gaps survive, folded into Phase 2.

3. **`InterviewBank/07_Behavioral_Leadership.md` is a skeleton** — **44 `[FILL:]`
   placeholders across 36 lines**. The 12 STAR stories are templates with the specifics
   missing. Behavioural questions are asked in every interview; this file cannot currently
   be used. → New Phase 3.

### The four confirmed gaps

Lexical matching of the 14 asked questions against all 1,335 (Jaccard, threshold 0.45
covered / 0.28 partial). Flagged results were verified by eye — chunking scored 0.25 but
is genuinely covered by `InterviewBank/03:8`; the four below are real.

| Gap | Nearest existing content | Why it does not cover it |
|---|---|---|
| 1M-document AI Search design | `L09:700` — a 10M-chunk *latency troubleshooting* question | Diagnosing a slow index ≠ sizing one from scratch |
| Context compression | — | Nothing in the repo. Best match 0.22, unrelated |
| AKS + KEDA for AI workloads | — | Score 0.12. L34 teaches Kubernetes generally, never connects to LLM/embedding workloads |
| PII management | — | Score 0.17, top match was about LlamaIndex evaluation |

### Revised deliverables

| Phase | Output | Questions | Status |
|---|---|---:|---|
| **1** | `Interview_QA_Resume_Based.md` — defend every resume claim | ~65 | Net-new, nothing exists |
| **2** | `Interview_QA_RealWorld_Asked.md` — the 14 asked + the 4 gaps | 18 | Net-new |
| **3** | Fill the 44 `[FILL:]` placeholders in `InterviewBank/07`; build `00_DRILL_INDEX.md` — a single entry point over the 1,161 existing unique questions | — | Unlocks work already done |
| *housekeeping* | Register in `00_MAP` / `00_CONTENTS` / `00_INDEX` / `00_LearningIndex`; commit and push | — | ~15 min |

**~83 new questions, not 155.** The ~90-question lessons file of §5/§7 is dropped entirely.

### Duplication gate — enforced mechanically

Before any new question is written it is scored against `questions.json` (the extracted
corpus). **Anything scoring ≥ 0.62 Jaccard against an existing question is rejected**, not
reworded. Both scripts live in `02_Questions/_tooling/` so the check is re-runnable
whenever material is added.

Deliberate, permitted overlap — one canonical answer per topic, everything else
cross-references it:

| Owner | Owns | Worked example |
|---|---|---|
| `RealWorld_Asked` | The 14, as spoken answers | Owns the token-saving *technique list* |
| `Resume_Based` | Anything carrying a number or a named project | Owns the *$150K JM Family cost story* — different question, different answer |
| Existing repo assets | Long-form teaching | `HLP01` / `L13` / `L27` stay authoritative; new files link, never restate |

Duplicate *sentences* of fact ("hybrid search combines BM25 with vector similarity") are
acceptable where two answers both need them. Duplicate *answers* are not.

### Known caveat on Phase 3

The STAR specifics cannot be authored — the actual conflict, the actual failure, the actual
stakeholder are Bala's to supply. Phase 3 pre-fills everything derivable from the resume and
the JM Family / KPMG material already in the repo, then hands over a short list of the
remaining blanks.

### Not actioned — flagged only

The duplicated chat dumps (§9 above) pollute every `grep` across the repo. **No files have
been moved or deleted.** Retiring them is a separate decision.

---

## 8. Standing conventions for this build

- Healthcare **and** JM Family examples on every concept answer, per the learning agreement.
- Numbers on the resume are treated as **claims to be defended**, never softened silently —
  where a claim is phrased in a way that invites attack (e.g. "eliminating hallucinations"),
  the file states the safer phrasing *and* explains why the original is risky.
- Cross-references point to the lesson file that carries the long-form teaching, so any
  answer can be expanded on demand.
- No answer relies on a framework version staying current; version numbers appear only
  where the resume already commits to them.
