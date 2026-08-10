# 00_DRILL_INDEX — the single entry point

**Built:** 2026-08-09 · **Phase 3** of `00_PLAN_InterviewQA_2026-08-08.md`

> **The problem this solves.** This repo contains **1,335 question-like entries across 131
> files** in five different formats. That is not a shortage of material — it is a shortage
> of *usable* material. You cannot rehearse from 131 files. This index tells you what to
> open, in what order, for which interview.

---

## 1. What actually exists, honestly tiered

Counts from a scripted sweep (`_tooling/qextract.py`) of every `.md` in the repo.

| Tier | Count | What it is | Drill it? |
|---|---:|---|---|
| **NEW — Resume defence** | **70** | `Interview_QA_Resume_Based.md` — every claim and number on your resume | ✅ **First** |
| **NEW — Actually asked** | **18** | `Interview_QA_RealWorld_Asked.md` — the 14 from your last five interviews + 4 companions | ✅ **First** |
| PerChapter | 407 | `PerChapter/QA_L*.md` — per-lesson self-test, L06–L21 and L32–L37 | ✅ Revision |
| InterviewBank | 117 | 7 thematic modules, `WHY/HOW/WHEN/SCALE/DEPLOY` format | ✅ Revision |
| Applied projects | 114 | `Part6/.../03_interview_qa.md` — 9 tool-specific sets | ✅ Targeted |
| PythonTrack | 76 | `06_Supplementary/PythonTrack/` — framework-free Python | ✅ If coding round |
| HighLevelPrep | 8 | `HLP01` — prose, not Q&A. Memory · tokens · scaling · agents | ✅ Read, don't drill |
| Lessons (inline) | 303 | Questions embedded in `L01`–`L37` teaching text | 📖 Reference only |
| Job-specific | 125 | `08_Jobs/` — Ascendion prep, FDE tracker | 📖 Reference only |
| Chat history | 171 | `07_ChatHistory/` — session transcripts | ❌ **Not drill material** |
| Archive | 8 | `_Archive/` | ❌ Stale |

**Drillable curated total: 810** (722 existing + 88 new).
Everything else is reference or noise.

> ⚠️ **Do not drill from `07_ChatHistory/`.** It contains 171 question-shaped lines, most of
> which are *me* asking *you* things during a teaching session — "Want to start Module 07
> right now?", "What do you see when you try to add a model?". They are not interview
> questions. They also contain the repo's only real duplication: three identical
> `Session_ChatHistory_2026-06-08/09/10.md` files, and a full copy of the 2026-06-21 session
> inside `08_Jobs/AscndIntr/PrepPlan/AscendionPrep_Day3_Module04-05_*.md`.

---

## 2. Route by topic

When you want to drill one subject, this is where it lives. **Primary** is the deepest
source; open it first.

| Topic | Primary | Supporting |
|---|---|---|
| **Memory & context management** | `RealWorld` Q1, Q5 | `HLP01` §1–2 · `Resume` Q18 |
| **RAG — lifecycle & architecture** | `RealWorld` Q7 · `Resume` Q7 | `InterviewBank/03` (15) · `QA_L13` (17) |
| **Chunking** | `RealWorld` Q9 | `InterviewBank/03` Q1–Q2 · `QA_L13` |
| **Embeddings & tokenization** | `QA_L11_2` (19) | `L11_2` inline (14) · `RealWorld` Q11 |
| **Hybrid search & reranking** | `Resume` Q11 | `QA_L09` (24) · `InterviewBank/02` |
| **AI Search at scale** ⚠ | `RealWorld` Q3, Q15 | `QA_L09` · `L09:700` (latency triage) |
| **Document Intelligence** | `RealWorld` Q2 · `Resume` Q50–52 | `QA_L08` (22) · `L30` OCR pipelines |
| **Agents — lifecycle** | `RealWorld` Q13 | `Resume` Q15–Q22 · `L27` · `InterviewBank/04` |
| **Agent frameworks** | `Resume` Q16 | `L25` comparison · `QA_L16` (22) |
| **MCP** | `Resume` Q35 | `L26` |
| **A2A** | `RealWorld` Q12 | `L29` |
| **Multi-agent / meta-agents** | `Resume` Q15 | `L28` · `Part6/02-crewAI` (14) |
| **Hallucination & grounding** | `Resume` Q12 | `L24` · `InterviewBank/06` |
| **Evaluation & RAGAS** | `Resume` Q23–Q28 | `Part6/03-RAGAS` (13) · `QA_L19` (16) |
| **Prompt engineering** | `QA_L15` (18) | `L15` inline · `Resume` Q25 |
| **Fine-tuning / LoRA** | `Resume` Q56 | `QA_L14` (17) · `QA_L11_3` (13) · `Part6/08-LoRA` (13) |
| **Cost & token optimisation** | `RealWorld` Q11 · `Resume` Q29–Q34 | `QA_L36` (10) · `HLP01` §2 |
| **Compression** ⚠ | `RealWorld` Q6, Q16 | *(nothing else — this was a gap)* |
| **PII & responsible AI** ⚠ | `RealWorld` Q10, Q18 · `Resume` Q37–Q40 | `InterviewBank/06` (19) |
| **Prompt injection & security** | `Resume` Q36 | `L24` · `InterviewBank/06` |
| **Azure AI Foundry** | `RealWorld` Q14 | `QA_L17` (18) · `L22` · `InterviewBank/02` |
| **Azure OpenAI service** | `QA_L12` (21) | `L12` inline · `RealWorld` Q4 |
| **Model selection** | `RealWorld` Q4 | `Resume` Q31 |
| **AKS / KEDA / scaling** ⚠ | `RealWorld` Q8, Q17 · `Resume` Q41 | `QA_L34` (11) |
| **Observability & tracing** | `Resume` Q46 | `QA_L36` (10) · `L31` |
| **Fault tolerance** | `Resume` Q19 | `L31` |
| **MLOps / LLMOps** | `Resume` Q24–Q27 | `QA_L19` (16) · `InterviewBank/06` |
| **Solution architecture** | `InterviewBank/05` (34) | `QA_L18` (17) · `Resume` Q7 |
| **IaC — Terraform / Bicep** | `Resume` Q60 | `QA_L33` (11) · `L33` |
| **Multi-cloud / Bedrock** | `Resume` Q58–Q59 | `Part6/06-Bedrock` (12) · `Part6/09-Vertex` (13) |
| **GraphRAG / Neo4j** | `Resume` Q53–Q55 | `Part6/07-GraphRAG` (10) |
| **CAG vs RAG** | `L23` | `RealWorld` Q7 follow-up |
| **Traditional ML** | `Resume` Q62–Q66 | `QA_L06` (25) · `09_ML/` |
| **Microsoft Fabric** | `QA_L37` (13) | `L37` |
| **Python / coding** | `PythonTrack` (76) | `QA_L32` (11) · `QA_L21` (18) |
| **Behavioural / leadership** | `InterviewBank/07` ⚠ *worksheet incomplete* | `Resume` Q67–Q70 |
| **FDE-specific** | `Resume` Q2, Q67–Q70 | `08_Jobs/FDE/FDE-Prep_Tracker.md` |

⚠ = was a confirmed coverage gap before Phase 2.

---

## 3. Route by interview stage

| Stage | Open these, in order |
|---|---|
| **Recruiter / screener** | `Resume` Q1–Q6 · `RealWorld` Q13, Q7 |
| **Technical deep dive** | `RealWorld` all 18 · `Resume` Q7–Q14, Q23–Q34 |
| **Architecture / system design** | `InterviewBank/05` (34) · `RealWorld` Q3, Q7, Q15, Q17 · `Resume` Q7, Q10 |
| **Agentic / AI-native round** | `RealWorld` Q12, Q13 · `Resume` Q15–Q22 · `L25`–`L29` |
| **Azure platform round** | `RealWorld` Q14 · `QA_L17`, `QA_L12`, `QA_L09`, `QA_L08` |
| **Cost / FinOps conversation** | `RealWorld` Q11 · `Resume` Q29–Q34 · `QA_L36` |
| **Coding round** | `PythonTrack` · `QA_L32` — ⚠ *reading these does not build coding muscle; write the code* |
| **Behavioural** | `InterviewBank/07` — ⚠ *finish the worksheet first* |
| **Hostile / pressure-testing** | `Resume` Q5, Q8, Q12, Q14, Q35, Q55 — every claim someone will attack |

---

## 4. The ten-day drill schedule

Assumes ~90 minutes a day. Reorder freely; **do not skip Day 1.**

| Day | Focus | Material |
|---|---|---|
| **1** | ⚠ **Resolve the facts** | `Resume` Appendix A (22 items) · `InterviewBank/07` worksheet Tier 1 + 2. **No drilling until the numbers are real** |
| **2** | The two centrepieces | `RealWorld` Q13 (agent process) + Q7 (RAG lifecycle). Out loud, timed, until 4–5 min flows without notes |
| **3** | Resume defence, part 1 | `Resume` Q1–Q14 — profile + the 500K RAG. **Q8 is the one that matters** |
| **4** | Resume defence, part 2 | `Resume` Q15–Q28 — agents + LLMOps |
| **5** | Cost + the four gaps | `Resume` Q29–Q34 · `RealWorld` Q3, Q6, Q8, Q10 |
| **6** | Platform depth | `RealWorld` Q14 · `QA_L17`, `QA_L12`, `QA_L09` |
| **7** | KPMG + multi-cloud | `Resume` Q50–Q61 |
| **8** | Behavioural | `InterviewBank/07` all 12 · `Resume` Q67–Q70 |
| **9** | ML fundamentals + companions | `Resume` Q62–Q66 · `RealWorld` Q15–Q18 |
| **10** | Full mock, cold | `08_Jobs/AscndIntr/PrepPlan/12_Mock_Interview.md` — no notes, recorded |

---

## 5. How to actually drill

Reading is not rehearsal. The failure mode on record is **headline-only answers** — knowing
the material and delivering one sentence of it.

**The protocol:**

1. **Cover the answer.** Read only the question.
2. **Answer out loud.** Not in your head — out loud, standing up if possible. The gap
   between what you know and what you can say is the entire problem.
3. **Time it.** 60 seconds for a standard question, 4–5 minutes for Q13 and Q7.
4. **Score yourself against the four-point rule.** Did you hit *what it IS · why it works
   that way · your concrete example with a number · the trade-off*? Missing the trade-off is
   the most common failure and the one that most signals seniority when you get it right.
5. **Then read the written answer** and note only what you missed.
6. **Re-drill anything you missed twice** the following day, not at the end.

**Record yourself once per session.** Listening back is unpleasant and it is the fastest
correction available — you will hear the filler, the trailing off, and the places you
stopped one sentence early.

---

## 6. Known state — read before you rely on this

Honest status as of 2026-08-09.

| Item | Status |
|---|---|
| `Interview_QA_Resume_Based.md` | ✅ Complete — 70 Q. **22 `[CONFIRM:]` items unresolved** |
| `Interview_QA_RealWorld_Asked.md` | ✅ Complete — 18 Q |
| `InterviewBank/07_Behavioral_Leadership.md` | ⚠️ **Incomplete** — 3 stories missing entirely (Q7, Q8, Q12), 15 outcome facts outstanding. Q6/Q11 figures corrected |
| `PerChapter/` L01–L05, L22–L31 | ❌ **No self-test files exist** for these lessons. Part 5 (agentic) is covered at interview altitude by the two new files, but has no per-lesson drill |
| Coding practice | ❌ **Nothing in this repo builds it.** `PythonTrack` is reading material |
| Whiteboard / timed design practice | ❌ Not covered |
| Spoken mock loop | ⚠️ `12_Mock_Interview.md` exists (15 Q) but is untimed and unrecorded |

**Two things this repo cannot give you**, and they are the difference between a good hit
rate and reliably converting: **writing code under observation**, and **speaking answers
under time pressure**. Everything here is silent-read material. Day 10 exists for that
reason and one mock is not enough.

---

## 7. Tooling

`_tooling/qextract.py` extracts every question-like line in the repo to `questions.json`.
`_tooling/qdedup.py` clusters near-duplicates and scores new questions against the corpus.

Run them before adding material. **Anything scoring ≥ 0.62 Jaccard against an existing
question is a duplicate** — rewrite the angle or cross-reference instead of adding it.

---

**Related:** `00_PLAN_InterviewQA_2026-08-08.md` — the build plan, coverage matrix, and the
duplication-control rules this index enforces.
