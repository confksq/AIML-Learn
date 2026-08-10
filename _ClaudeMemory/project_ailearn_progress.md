---
name: project-ailearn-progress
description: "AI learning library location, folder structure, and current phase — full detail lives in 00_START_HERE.md"
metadata:
  node_type: memory
  type: project
  originSessionId: 0b8fd03e-752e-4093-afb5-21fa4946920a
  modified: 2026-08-03T00:00:00.000Z
---

**Root:** `C:\pers\AIML-Learn\` (WSL: `/mnt/c/pers/AIML-Learn/`) — **moved 2026-08-03**, now a real
git clone of `github.com/confksq/AIML-Learn`. The old `C:\Users\confksq\Project\AIML-Learn\` path is
dead.

**Read `00_START_HERE.md` first — it is the authoritative index for progress, learning order, gaps,
and a full file map.** Do not rebuild that picture from memory; the file is maintained, this note is not.

**Status:** Curriculum L01–L37 across **8 Parts** · AI-102 ✅ passed · CareerAccelerator 9/9 ✅
**Current phase:** job search + closing roadmap gaps per
`04_Career/Consolidation_and_Update_Plan_2026-08-03.md` — **Phases 0, 1, 2, 6 done; Phases 3
(ML eval metrics), 4 (ANN index internals), 5 (context engineering) remain, ~5–6 hrs.**
Part 7 = Platform Engineering (L32–L36), **Part 8 = Data Platform (L37 Microsoft Fabric)**.

**Registering a new lesson takes FOUR index updates** — `01_Lessons/00_LearningIndex.md`,
`00_MAP.md`, `00_CONTENTS.md` (heading tree with line numbers), `00_INDEX.md` (A–Z concepts, insert
alphabetically + bump the header count). Missing one is how the indexes went stale in July.

**PRDs in `04_Career/` are both DELIVERED and carry stale paths** — `00_PRD.md` (question bank, 7
tiers / 144 Qs) and `PRD_Bala_AI_Career_Acceleration.md` (v2.1, CareerAccelerator 9/9, has a
PATH CORRECTION block). Do not execute their path instructions literally.

⚠️ **`_Archive/StaleTrackers/Interview_Prep_AI_Engineer_Complete.md` is stale-by-structure but NOT
dead content.** It is the delivered output of **PRD Feature 12** (100 Qs in 10 sections A–J) and is
the **source list** for `02_Questions/Detailed/`, which expands it section by section into
what/why/when/how + example + trade-off. **A (RAG, 15) and B (Azure Foundry/OpenAI, 15) are done;
C–J remain** — C (Agents, 15) is next.

⚠️ **Counting questions: `PerChapter/` uses `**Qn.**`, not `### Qn`.** A grep on a single Q-pattern
returned zero for all 33 PerChapter files, which actually hold 655 questions. Same failure family as
the `L##`-only grep noted below. `00_QA_FILE_INDEX.md` at repo root has the reconciled counts.

**Structure (as of 2026-07-19):**

```
00_START_HERE.md          ← authoritative index
01_Lessons/
  Part1_Foundations/        L01–L06
  Part2_AzureAIServices/    L07–L10
  Part3_GenAI_LLMs/         L11_1–L16
  Part4_Architecture/       L17–L21
  Part5_AgenticProtocols/   L22–L31  (MCP, A2A, CAG, LangGraph, agent workflow)
  Part6_AppliedProjects/    CareerAccelerator · Dealer · VitalCare · images (NOT sequenced)
02_Questions/             InterviewBank · PerChapter · HighLevelPrep · Detailed · _tooling
                          + 00_DRILL_INDEX.md (entry point, ~810 Qs) · Interview_Bible_77Q
                          · Interview_QA_Resume_Based · Interview_QA_RealWorld_Asked
04_Career/                roadmaps, PRDs, resume, JD + AI-103 gap analyses
05_Assessments/           VitalCare architecture response
06_Supplementary/         PythonTrack (framework-free) · workouts · curriculum source
07_ChatHistory/           all transcripts + INDEX.md
08_Jobs/                  postings + Ascendion prep (master plan, mock interview)
_Archive/                 superseded — moved, never deleted
```

Pre-2026-07-18 paths are all dead: `PartsModules/`, `OldLearn/`, `NewLearn/`, `AIFoundry/`,
`InterviewPrep_GenAI_Architect/`, `OldButRefer/`, `ImpLearning/`, and `03_Portfolio/` (now Part 6).

⚠️ **Search failure mode — this went wrong three times in July.** Grepping only `L##` files
under-reports coverage. Real teaching material also lives in:
- `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/*/01_concepts.md` — the *only* library
  coverage of LlamaIndex, crewAI, HuggingFace, Cypher, Bedrock, Vertex AI, RAGAS, Ollama
- `06_Supplementary/PythonTrack/` — framework-free Python (raw agent loop, FAISS, PEFT)

**Confirmed gaps:** AG-UI · CodeAct · React streaming UI / TypeScript · Terraform / Helm / KEDA ·
dedicated AI Security & Governance module · Anthropic Claude API. LangGraph and AutoGen are **not**
gaps — LangGraph is taught with working code in `L25`.

**Why:** User targets Azure AI Solutions Architect / AI Engineer roles while doing AI-adjacent work
at JM Family (Document Intelligence, Azure AI Search RAG).

**How to apply:** Teach interactively with Q&A, not lecture. Connect to JM Family systems (ADF,
Azure Functions, SharePoint, `srch-jma-*-indexer`, `cog-jma-dev-frm-recognizer`). Build on the chain
already covered: tokens → embeddings → attention → transformers → RAG → orchestration → agents.
See [[user-ailearning-profile]] and [[feedback-save-chat]].
