---
name: project-ailearn-progress
description: "AI learning library location, folder structure, and current phase — full detail lives in 00_START_HERE.md"
metadata:
  node_type: memory
  type: project
  originSessionId: 0b8fd03e-752e-4093-afb5-21fa4946920a
  modified: 2026-08-03T00:00:00.000Z
---

**Root — two active machines, both real git clones of `github.com/confksq/AIML-Learn`, kept in sync
via push/pull (no path is "dead"):**
- **Personal machine:** `C:\pers\AIML-Learn\` (WSL: `/mnt/c/pers/AIML-Learn/`) — cloned 2026-08-03
- **Office machine:** `C:\Users\confksq\Project\AIML-Learn\` (WSL: `/mnt/c/Users/confksq/Project/AIML-Learn/`)

Always `git pull` at session start and confirm which machine you're on before trusting a path.

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

**PRDs in `04_Career/` are both DELIVERED** — `00_PRD.md` (question bank, 7 tiers / 144 Qs) and
`PRD_Bala_AI_Career_Acceleration.md` (v2.1, CareerAccelerator 9/9). The latter's PATH CORRECTION
block was updated 2026-08-04 to reflect both active machines (personal `C:\pers\AIML-Learn\`,
office `C:\Users\confksq\Project\AIML-Learn\`) — no longer a single "corrected" path.

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
02_Questions/             InterviewBank · PerChapter · HighLevelPrep
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
