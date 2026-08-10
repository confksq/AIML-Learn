# AIML-Learn — Start Here

**Last reorganized:** 2026-07-19 · **Part 7 added:** 2026-07-26
**Status:** Curriculum complete (L01–L36, 7 Parts incl. applied projects) · AI-102 ✅ passed · Career Accelerator ✅ complete
**Current phase:** Job search — **FDE-Prep** (see `08_Jobs/FDE/FDE-Prep_Tracker.md`)

> This file is the single source of truth for what is current. If another file disagrees with this one, this one wins.

---

## Three index files — use these first

| File | Size | What it answers |
|---|---|---|
| **`00_MAP.md`** | 101 lines | *"What modules exist?"* — one line per module, fits on two screens. **Start here.** |
| **`00_INDEX.md`** | 1,021 concepts | *"Is topic X covered, how deeply, and where exactly?"* — alphabetical, depth-marked (● taught / ◐ covered / ○ mentioned), `file:line` |
| **`00_CONTENTS.md`** | 2,150 topics | *"What's inside this lesson?"* — every heading at every level, in reading order |

Between them they cover 120 teaching files. Search `00_INDEX.md` before concluding something isn't
in the library — that mistake has been made repeatedly.

---

## Where everything lives

| Folder | What's in it | Use it when |
|---|---|---|
| `01_Lessons/` | 39 lesson files in 6 Parts, **plus Part 6 applied projects** | Learning, revising, or showing work |
| `02_Questions/` | 6 interview-bank files + 19 per-chapter Q&A files | Testing yourself / interview prep |
| `04_Career/` | Roadmaps, PRDs, resume, job-demand analysis, JD tools | Applying / positioning |
| `05_Assessments/` | VitalCare enterprise architecture assessment (completed) | Reference architecture, portfolio |
| `06_Supplementary/` | Non-Azure Python track, hands-on workouts, curriculum source | Filling gaps the lessons don't cover |
| `07_ChatHistory/` | All session transcripts, consolidated from 4 former locations | Recovering past explanations |
| `08_Jobs/` | Active job postings · Ascendion prep · **`FDE/` — FDE-Prep tracker + IaC glossary** | Job search |
| `_Archive/` | Superseded material — **kept, not deleted** | Only if something's missing |

---

## Learning order

Lessons are numbered **in dependency order** — L01 → L31 is the correct sequence. The numbers already account for prerequisites, so following them in order is always safe.

### Part 1 — Foundations (`01_Lessons/Part1_Foundations/`)

| # | File | Depends on |
|---|---|---|
| 1 | `L01_Introduction_to_AI.md` | — start here |
| 2 | `L02_AzureAIServices_Overview.md` | L01 |
| 3 | `L03_NLP_Fundamentals.md` | L02 · ⚡ feeds L11 |
| 4 | `L04_ComputerVision.md` | L02 |
| 5 | `L05_SpeechServices.md` | L02 |
| 6 | `L06_AzureML.md` | L01 — standalone |

### Part 2 — Azure AI Services (`01_Lessons/Part2_AzureAIServices/`)

| # | File | Depends on |
|---|---|---|
| 7 | `L07_AzureAIServices_DeepDive.md` | L02+L03+L04+L05 |
| 8 | `L08_DocumentIntelligence.md` | L07 |
| 9 | `L09_AzureAISearch.md` | L07 · ⚡ **must precede L13** |
| 10 | `L10_BotDevelopment.md` | L07+L03 |

### Part 3 — GenAI & LLMs (`01_Lessons/Part3_GenAI_LLMs/`)

| # | File | Depends on |
|---|---|---|
| 11a | `L11_1_LLMs_Attention_Transformer.md` | L01+L03 |
| 11b | `L11_2_LLMs_Tokenization_Embeddings.md` | L11_1 |
| 11c | `L11_3_LLMs_Pretraining_Finetuning.md` | L11_1 |
| 11d | `L11_4_LLMs_RLHF_Alignment.md` | L11_1 |
| 12 | `L12_AzureOpenAI_Services.md` | L02+L11 — unlocks L13–L17 |
| 13 | `L13_RAG_DeepDive.md` | **L09 + L12** |
| 14 | `L14_FineTuning.md` | L11+L12 — ⚡ parallel with L13 |
| 15 | `L15_PromptEngineering.md` | L12 — ⚡ parallel with L13, L14 |
| 16 | `L16_AIOrchestration_SK_Agents.md` | L12+L13 — **after RAG** |

### Part 4 — Architecture & Operations (`01_Lessons/Part4_Architecture/`)

| # | File | Depends on |
|---|---|---|
| 17 | `L17_AzureAIFoundry.md` | all of Part 3 |
| 18 | `L18_AISolutionArchitecture.md` | everything above |
| 19 | `L19_MLOps_LLMOps.md` | L17+L18 |
| 20 | `L20_IntegrationPatterns.md` | everything above |
| 21 | `L21_Python_for_AI.md` | none — standalone, take anytime |

### Part 5 — Agentic Protocols & Patterns (`01_Lessons/Part5_AgenticProtocols/`)

Promoted 2026-07-19 from `08_Jobs/AscndIntr/PrepPlan/`, where this material was invisible to searches
of the lesson set. Written as spoken briefings rather than reference chapters — denser and more
opinionated than Parts 1–4, with healthcare-domain examples throughout (concepts are general).

| # | File | Depends on | Note |
|---|---|---|---|
| 22 | `L22_Foundry_AgentLifecycle.md` | L17 | Platform + agent lifecycle; overlaps and updates L17 |
| 23 | `L23_CAG_vs_RAG.md` | L13 | **Only CAG material in the library** |
| 24 | `L24_Hallucination_Mitigation.md` | L13+L15 | Factual + agentic hallucination |
| 25 | `L25_AgentFramework_Comparison.md` | L16 | **The real LangGraph lesson** — StateGraph/State/Node/Checkpointer + worked code |
| 26 | `L26_MCP_ModelContextProtocol.md` | L16 | **Only MCP material** |
| 27 | `L27_Agent_Workflow_EndToEnd.md` | L16+L26 | 762 lines — the centrepiece |
| 28 | `L28_MetaAgent_Hierarchies.md` | L27 | Agents of agents |
| 29 | `L29_A2A_Protocol.md` | L27 | **Only A2A material** |
| 30 | `L30_OCR_Pipelines.md` | L08 | Document Intelligence vs John Snow Labs |
| 31 | `L31_FaultTolerance_Observability.md` | L27 | Circuit breakers, self-healing, tracing |

### Part 7 — Platform Engineering & AI-Assisted Delivery (`01_Lessons/Part7_PlatformEngineering/`)

Built 2026-07-26 for **FDE-Prep**. Closes the *engineering-hands* gaps this curriculum deliberately
scoped out — it targeted AI-900/AI-102, C#-first, Azure-managed-services-first. Each module leads
from something you already have rather than from zero.

| # | File | Depends on | Note |
|---|---|---|---|
| 32 | `L32_AdvancedPython_for_AI.md` | L21 | **Supersedes L21 for writing-level Python** — decorators, generators, dataclasses, Big-O, patterns |
| 33 | `L33_IaC_Terraform_for_Bicep_Devs.md` | Bicep knowledge | Leads with **state ownership**, not HCL syntax. Also Pulumi, CDK-in-C#, Ansible, VPC/PrivateLink, FedRAMP, Checkov |
| 34 | `L34_Kubernetes_Helm_GitOps.md` | L33 | Helm, ArgoCD, EKS/GKE, service mesh, OpenShift. Assumes your AKS familiarity |
| 35 | `L35_AI_Assisted_Engineering.md` | L15 | Cursor, Copilot as practice, computer-use, N8N. **Mostly doing, not reading** |
| 36 | `L36_LLM_Observability_FinOps.md` | L31, L19 | OTel, LangSmith, Langfuse, Arize, LiteLLM, Grafana, FinOps |

### Part 6 — Applied Projects (`01_Lessons/Part6_AppliedProjects/`)

Moved here 2026-07-19 from the former top-level `03_Portfolio/`. **Not sequenced** — take these
alongside the Parts they support, not after them. Unlike Parts 1–5 these carry runnable code and
resume bullets: they are both learning material *and* employer-facing evidence.

| Item | Supports | Contents |
|---|---|---|
| `01-CareerAccelerator/` | L13, L14, L16 | 9 tool modules — Ollama · crewAI · RAGAS · HuggingFace · LlamaIndex · Bedrock · GraphRAG+Neo4j · LoRA · Vertex AI. Each: `01_concepts` · `02_architecture` · `03_interview_qa` · `04_hands_on.py` · `05_resume_bullet` |
| `02-DealerIntelligence-Platform/` | L16, L18, Part 5 | C# 9-layer agentic platform (auto/dealer) + `JMA-DealerIntelligence-Complete-Flow.md` — **real production documentation** |
| `05-VitalCare-AI-Platform/` | L16, L18, Part 5 | Same architecture, healthcare prior-auth domain |
| `images/` | — | 3 screenshots |

> ⚠️ **Part 6 contains ~51 C# source files and Python scripts, not just readable lessons.** The
> `01_concepts.md` files are the teaching layer — for 8 of the 9 CareerAccelerator topics they are
> the **only** coverage in the entire library (see the table below). The `.cs` platform folders are
> reference implementations to read, not chapters to study.

**Three lessons sit out of original numeric order on purpose:** NLP moved ahead of Computer Vision (it feeds L11), and Fine-Tuning + Prompt Engineering moved ahead of Orchestration (they only need L12, while Orchestration needs RAG first).

---

## Known gaps — not covered by the lesson set

Verified absent. These matter because current job descriptions ask for them:

| Gap | Where partial coverage exists |
|---|---|
| **AG-UI · CodeAct** | nowhere — smallest surface, cheapest to close |
| **React streaming agent UI · TypeScript** | nowhere — largest gap |
| ~~Terraform / IaC · Helm~~ | ✅ **closed 2026-07-26 — `L33`, `L34`** |
| **KEDA** | nowhere — `L34` covers HPA only |
| **AI Security & Governance** | L18 partially — no dedicated module |
| **Anthropic Claude API** | Constitutional AI in L11_4; MCP in L26; Claude-via-Bedrock in P6/06. **The direct API is still absent** |
| **PyTorch / TensorFlow / ML math / classical ML** | ⚠️ **nowhere.** This row previously pointed at `06_Supplementary/PythonTrack/` — that was wrong. `AIMLcurriculum.md` and `-gaps.md` are **syllabi, not lessons**; PythonTrack's actual teaching files are all GenAI |
| **XGBoost · LightGBM · scikit-learn · MLflow** | ○ name-drops only — `L06` AutoML leaderboard output, `L21` notebook-reading snippet |
| **pgvector / Pinecone** | `L09` + `L13` decision tables only |
| **AWS hands-on** | `P6/06-Bedrock` + `VitalCare` are architecture-level. ⚠️ **An experience gap no lesson closes** |

> ✅ **Corrected 2026-07-19:** this table previously listed LangGraph and AutoGen as absent. They
> aren't — LangGraph is taught with working code in `L25`, and AutoGen appears in the crewAI
> contrast tables. MCP, A2A, and CAG are now `L26`, `L29`, and `L23`.

---

## ⚠️ Teaching material NOT in a numbered `L##` file

Read this before concluding a topic isn't covered. Three times in July a search of the `L##` lesson
files wrongly concluded LangGraph, MCP, A2A and CAG were absent — they were in the material below.
Since 2026-07-19 most of it lives inside `01_Lessons/`, but **not as `L##` files**, so a filename
search still misses it.

**`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/*/01_concepts.md`** — ~1,000 lines of genuine teaching, and for
8 of these 9 topics it is the **only** coverage in the library:

| Topic | Lesson coverage | Read for |
|---|---|---|
| **LlamaIndex** (`05-`) | **none** | Lorven names it explicitly |
| **crewAI** (`02-`) | **none** | Synergech names it; also holds the AutoGen/MAF contrast |
| **GraphRAG / Neo4j / Cypher** (`07-`) | L13 decision table only; Cypher nowhere | Lorven wants graph DB |
| **RAGAS** (`03-`) | one line in L13 | agent/RAG evaluation |
| **Amazon Bedrock** (`06-`) + **GCP Vertex AI** (`09-`) | index mention only | the entire multi-cloud story |
| **Ollama / local LLM** (`01-`) | one mention in L07 | open-source credibility |
| **HuggingFace** (`04-`) | **none** | open-source ecosystem |
| ~~LoRA / QLoRA~~ (`08-`) | ✅ **covered in L14** | skip — the one real overlap |

*These carry runnable `.py` files and resume bullets — they are employer-facing evidence as well as
teaching material. Read `01_concepts.md` + `03_interview_qa.md`; run the `.py` files only if you want
the hands-on claim.*

**`06_Supplementary/PythonTrack/`** — framework-free Python (see below).

---

## Supplementary material — why it exists

`06_Supplementary/PythonTrack/` is **not** redundant with the lessons. L01–L21 are Azure-first and framework-first (Semantic Kernel, Azure OpenAI). The Python track teaches the same concepts framework-free:

- `1.5-AIAgents.md` — agent loop and ReAct built from scratch, no framework
- `1.4-FineTuning.md` — GPT-2 + HuggingFace PEFT locally, vs L14's Azure JSONL approach
- `Part1-AI-LLMs.md` — FAISS and raw Python RAG, vs L13's Azure AI Search approach
- `AIMLcurriculum.md` + `-gaps.md` — vendor-neutral ML-engineer track

Interviewers ask you to explain agents without naming a framework. That's what this folder is for.

---

## Complete file map

Everything you own, by folder. Lesson files are listed in the learning-order tables above.

### `02_Questions/` — interview prep

> **▶ Start at `00_DRILL_INDEX.md`.** The repo holds ~810 curated questions across dozens of
> files; the drill index is the single entry point — topic routing, route-by-interview-stage,
> and a 10-day schedule. Added 2026-08-09.

**Interview-prep layer** (built 2026-08-08 → 08-10 per `00_PLAN_InterviewQA_2026-08-08.md`):

| File | What |
|---|---|
| `00_DRILL_INDEX.md` | Entry point. What to open, for which interview, in what order |
| `Interview_QA_Resume_Based.md` | **70 Q** — defends every claim and number on the resume. Appendix A: 22 `[CONFIRM:]` items to resolve. Appendix B: 2 recommended resume edits |
| `Interview_QA_RealWorld_Asked.md` | **18 Q** — the 14 asked across the last five interviews, plus 4 companions on the confirmed gaps (1M-doc search, compression, KEDA-for-AI, PII) |
| `Interview_Bible_77Q_FDE_AI_Lead.md` | 77 Q, verbatim spoken answers. ⚠️ **Read the audit first** |
| `Interview_Bible_77Q_AUDIT_2026-08-10.md` | Findings on the Bible: 6 factual errors, 4 internal contradictions, ~20 unfalsifiable numbers |
| `Detailed/` | Rehearsal-depth expansions (RAG architecture, Foundry/OpenAI) |
| `00_PLAN_InterviewQA_2026-08-08.md` | Build plan, coverage matrix, duplication-control rules |
| `_tooling/` | `qextract.py` / `qdedup.py` — run before adding questions; ≥0.62 Jaccard = duplicate |

**`InterviewBank/`** — architect-judgment questions in WHY / HOW / WHEN / SCALE / DEPLOY format with follow-up probes. Built from `04_Career/00_PRD.md`.
⚠️ `07_Behavioral_Leadership.md` is **incomplete** — three stories missing entirely and fifteen outcome facts outstanding. See the worksheet at the end of that file.

`01_Fundamentals` · `02_Azure_AI_Platform` · `03_RAG_Architecture` · `04_Agent_Orchestration` · `05_Solution_Architecture` · `06_Responsible_AI_LLMOps`

**`HighLevelPrep/`** — architect-altitude prep, added 2026-07-19.
`HLP01_Memory_Tokens_Scaling_Agents.md` — Memory · Tokenization efficiency · Scaling · Agents.
Its Memory section is the **canonical treatment of memory architecture** (context window vs session vs
long-term vs state, compaction strategies, eviction order, memory poisoning) — a topic that was only
ever a subtopic inside L16.

**`PerChapter/`** — self-study Q&A, one per lesson, deliberately non-overlapping with the interview bank.

`QA_L06` → `QA_L21` (19 files).
⚠️ **`QA_L01`–`QA_L05` do not exist.** Part 1 has no per-chapter Q&A — a real gap if you want full self-test coverage.

### `01_Lessons/Part6_AppliedProjects/` — 131 files (Part 6, see learning order above)

| Item | Contents |
|---|---|
| `01-CareerAccelerator/` | 9 modules (Ollama, crewAI, RAGAS, HuggingFace, LlamaIndex, Bedrock, GraphRAG/Neo4j, LoRA, Vertex AI). Each has `README` · `01_concepts` · `02_architecture` · `03_interview_qa` · `04_hands_on.py` · `05_resume_bullet` · `requirements.txt` |
| `02-DealerIntelligence-Platform/` | C# 9-layer agentic platform (auto/dealer domain) + `WORKFLOW.md`, `FLOW_WITH_LOOPS.md`, and `JMA-DealerIntelligence-Complete-Flow.md` — **real production documentation, highest-value file here** |
| `05-VitalCare-AI-Platform/` | Same 9-layer architecture, healthcare prior-auth domain. Not a duplicate — parallel domain implementation. Lacks the two workflow docs Dealer has |
| `images/` | 3 screenshots |

### `04_Career/` — 13 files

| File | What it is |
|---|---|
| `CareerRoadmap_AIEngineer.md` | Target roles, fit rationale, positioning |
| `AI-LearningRoadmap.md` | 44-skill track-based roadmap |
| `gmailreq.md` | 66 job postings → ranked skill demand *(evidence behind the roadmap)* |
| `AIFoundry-CVCoverage-Tables.md` | CV bullet → coverage → what's missing |
| `JDCoverage_Synergech_Lorven_2026-07-19.md` | Two live JDs vs library coverage — 3 tiers + a lesson backlog. **Corrects an earlier error that said LangGraph/AutoGen were absent — they aren't** |
| `AI103_GapToCertification_2026-07-19.md` | AI-103 exam objectives mapped to the library — 25 covered / 25 partial / 13 gaps, plus a 54–70 hr study plan |
| `MasterCoverageTable_JobDemand.txt` | Skill × job demand × Foundry coverage |
| `currentresconent.txt` | Current resume bullets |
| `00_PRD.md`, `01_EXECUTION_PLAN.md` | Spec that generated `02_Questions/InterviewBank/` |
| `PRD_Bala_AI_Career_Acceleration.md` | Spec that generated `01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/` |
| `JobTools/` | `jd-match-analyzer.html`, `job-tracker.html` (working apps), JD prompt, LinkedIn templates |

### `05_Assessments/` — 3 files

`VitalCare_AssessmentBrief.txt` (the brief) → `Assessment_Breakdown.md` (analysis) → `VitalCare_AI_Assessment_Response.md` (**101 KB completed cloud-agnostic healthcare AI architecture — portfolio-grade**).

### `06_Supplementary/` — 10 files

| File | Why it's not redundant |
|---|---|
| `PythonTrack/1.5-AIAgents.md` | Agent loop + ReAct from scratch, no framework |
| `PythonTrack/1.4-FineTuning.md` | GPT-2 + HuggingFace PEFT locally (L14 is Azure JSONL) |
| `PythonTrack/Part1-AI-LLMs.md` | FAISS + raw Python RAG (L13 is Azure AI Search) |
| `PythonTrack/AIMLcurriculum.md` + `-gaps.md` | ⚠️ **Syllabi, not lessons** — 668 lines of topic bullets (decorators, PyTorch, MLflow, ONNX, vLLM). Useful as a **self-audit checklist**; they teach nothing |
| `AI_Curriculum_Workouts.txt` | ~45 hands-on C#/.NET exercises — the only exercise catalog |
| `CurriculumSource/…SubTopic_FnlVer.txt` | Deepest subtopic + minute-level decomposition |
| `CurriculumSource/…v2_Updated.txt` | The 23-module plan; only record of the two modules never built |
| `Azure-AI-Foundry-Complete-Hierarchy.docx` | Source doc |
| `jma-vehicle-data.txt` | Working RAG test corpus |

### `08_Jobs/` — 21 files

`july20thWeek.txt` — active postings. `AscndIntr/PrepPlan/` — master plan, defend-assessment, mock interview + transcripts. **The 10 topic modules were promoted to `01_Lessons/Part5_AgenticProtocols/` on 2026-07-19.**
**Modules 02, 05, 07, 08 are your only CAG / MCP / Meta-Agent / A2A material.** Treat as lessons.

---

## Archive manifest — `_Archive/` (45 files)

Nothing here was deleted. Each subfolder says why it was set aside.

| Subfolder | Files | Why archived |
|---|---|---|
| `SalvagedIntoLessons/` | 5 | **Content already merged into L08, L09, L17.** Kept only as provenance — the lessons now carry everything useful |
| `FoundryCourse_5Layer/` | 16 | The 5-layer Foundry course, superseded by `L17` (31 KB → now ~1100 lines). Includes the one md5-identical duplicate found in the whole tree |
| `RedundantCurriculumViews/` | 13 | The same 19-module plan rendered 6+ ways, plus expired day-by-day schedules and raw DeepSeek source |
| `SupersededLessons/` | 5 | Original `.docx` modules replaced by `L01`–`L04`. The Responsible-AI supplement is the one with possible unique content |
| `PDFRenders/` | 3 | Renders of `.md`/`.docx` originals (4.1 MB + 123 KB) + a chat transcript that's a strict subset of one in `07_ChatHistory/` |
| `StaleTrackers/` | 2 | `MasterCoverage_Latest.txt` (contradicted the lesson index) and the flat 100-question bank (superseded by `02_Questions/InterviewBank/`) |
| `Misc/` | 1 | Azure subscription screenshot |

**Two judgment calls to revisit if you disagree:**
- `SupersededLessons/Part1_Module2-Supplementary_AI_Workloads_Responsible_AI.docx` — Responsible AI is scattered across L01/L11_4/L19; this may hold material none of them have.
- `StaleTrackers/Interview_Prep_AI_Engineer_Complete.md` — 100 questions in a flatter format. Structurally weaker than the InterviewBank, but the raw question list has value.

---

## Ground rules

1. **New chat histories go to `07_ChatHistory/`** — the `/share` script defaults to the wrong folder; override it.
2. **Nothing in `_Archive/` was deleted.** If something seems missing, look there before recreating it.
3. **This file is authoritative on progress.** A previous tracker (`MasterCoverage_Latest.txt`, now archived) contradicted the lesson index by marking completed modules as pending — it was stale.
