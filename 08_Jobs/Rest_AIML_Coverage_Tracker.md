# Rest.txt (Resume) — AIML Library Coverage Audit

**Source:** `08_Jobs/Rest.txt` — AI Cloud Architect / Generative AI Engineer resume
**Audited:** 2026-08-01, via two parallel searches (grep -rniE) across `01_Lessons/`, `02_Questions/`,
`05_Assessments/`, `06_Supplementary/` for every distinct technology named in the resume.

**Overall coverage: ~76%** (75 technologies checked; 1 hard zero, everything else has at least
partial coverage — this library is largely purpose-built to mirror this resume's stack).

**Legend:** ✅ Present (dedicated module, ≥85%) · 🟡 Present (partial/light, 40–84%) · 🔴 Not Covered (0%)

---

## Certifications

| Technology | Location / path | Status | Coverage % |
|---|---|:--:|--:|
| AI-102 (Azure AI Engineer Associate) | `00_LearningIndex.md`:3, Part 2 modules (`L07`–`L10`) framed around it | ✅ | 100% |
| AI-103 (Developing AI Apps and Agents on Azure) | `00_LearningIndex.md`:222 only — real material likely in `04_Career/AI103-Material/` (outside the 4 dirs scanned) | 🟡 | 20% |
| AZ-204 (Developing Solutions for Microsoft Azure) | — no hits | 🔴 | 0% |

## Generative AI & LLMOps

| Technology | Location / path | Status | Coverage % |
|---|---|:--:|--:|
| Azure AI Foundry | `Part4_Architecture/L17_AzureAIFoundry.md` (dedicated module) | ✅ | 100% |
| Azure OpenAI (GPT-4o/4-Turbo) | `Part3_GenAI_LLMs/L12_AzureOpenAI_Services.md` (dedicated module) | ✅ | 100% |
| Semantic Kernel (.NET) | `Part3_GenAI_LLMs/L16_AIOrchestration_SK_Agents.md` (flagship, 2,084 lines) | ✅ | 100% |
| RAG Architecture | `Part3_GenAI_LLMs/L13_RAG_DeepDive.md` (flagship module) | ✅ | 100% |
| GraphRAG + Neo4j | `L13_RAG_DeepDive.md`:1320–1353 + dedicated `CareerAccelerator/07-GraphRAG-Neo4j/` | ✅ | 100% |
| Agentic Architectures & Autonomous Agents | `L18_AISolutionArchitecture.md`:70, Part 5 (`L22`–`L31`) | ✅ | 90% |
| Vector Embeddings (text-embedding-3) | `Part2_AzureAIServices/L09_AzureAISearch.md`:110–140 | ✅ | 85% |
| Azure AI Search (Hybrid + Semantic) | `L09_AzureAISearch.md` (dedicated module, 347 hits) | ✅ | 100% |
| Azure AI Document Intelligence | `Part2_AzureAIServices/L08_DocumentIntelligence.md` + `L30_OCR_Pipelines.md` | ✅ | 100% |
| Prompt Engineering | `Part3_GenAI_LLMs/L15_PromptEngineering.md` (dedicated module) | ✅ | 100% |
| Azure AI Content Safety | `L01_Introduction_to_AI.md`:389–529, `L11_4_RLHF_Alignment.md` | 🟡 | 75% |
| RAGAS 0.4 / LLM Evaluation | `CareerAccelerator/03-RAGAS-Evaluation/` (dedicated module) | ✅ | 100% |
| Responsible AI & AI Governance | `L01_Introduction_to_AI.md`:393–403 (dedicated section) | 🟡 | 85% |
| Model Context Protocol (MCP) | `Part5_AgenticProtocols/L26_MCP_ModelContextProtocol.md` (dedicated module) | ✅ | 100% |
| LoRA / QLoRA (PEFT) | `L11_1`:304, `L11_2`:529, `L14_FineTuning.md`, `PythonTrack/` PEFT code | ✅ | 85% |

## AI Frameworks & Tools

| Technology | Location / path | Status | Coverage % |
|---|---|:--:|--:|
| LangChain | `L13_RAG_DeepDive.md`:358,1190, `L16`:1 (integrated, no standalone module) | 🟡 | 60% |
| LangGraph | `Part5_AgenticProtocols/L25_AgentFramework_Comparison.md` (dedicated comparison) | ✅ | 85% |
| LlamaIndex | `CareerAccelerator/05-LlamaIndex-RAG/` (dedicated module) | ✅ | 100% |
| crewAI | `CareerAccelerator/02-crewAI-MultiAgent/` (dedicated module) | ✅ | 100% |
| Microsoft Agent Framework (MAF) | `02-crewAI-MultiAgent/01_concepts.md`:76–86 — brief comparative mention only | 🟡 | 25% |
| AutoGen | `L25_AgentFramework_Comparison.md` (dedicated comparison section) | 🟡 | 80% |
| Hugging Face Transformers | `L14_FineTuning.md`:634,683 (used within fine-tuning module) | 🟡 | 60% |
| Amazon Bedrock (boto3) | `CareerAccelerator/06-Amazon-Bedrock/` (dedicated module) | ✅ | 100% |
| GCP Vertex AI (Gemini/ADK) | `CareerAccelerator/09-Vertex-AI/` (dedicated module — confirmed via Batch 2 search) | ✅ | 90% |
| Ollama (Local LLMs) | Indexed as a CareerAccelerator tool module; `L07_AzureAIServices_DeepDive.md`:277 | 🟡 | 80% |
| OpenAI API (raw, non-Azure) | `L12_AzureOpenAI_Services.md`:35,666 (comparison table only) | 🟡 | 40% |
| Prompt Flow | `L16_AIOrchestration_SK_Agents.md`:733–761 (dedicated subsection) | 🟡 | 75% |
| Azure ML | `Part1_Foundations/L06_AzureML.md` (dedicated module) | ✅ | 100% |
| FastAPI | `L32_AdvancedPython_for_AI.md`:160,368 — flagged as a curriculum gap in `PythonTrack/AIMLcurriculum-gaps.md`:39 | 🟡 | 30% |
| Vector DBs — Pinecone/Qdrant/FAISS | `L09_AzureAISearch.md`:790–814 (comparison table) | 🟡 | 75% |
| HNSW / hybrid vector+keyword search | `L09_AzureAISearch.md`:107,393,487–489 (dedicated technical section) | ✅ | 90% |
| Function-calling / OpenAPI tools / multi-agent orchestration | `L12_AzureOpenAI_Services.md`:336; crewAI/`L16`/`L25` | ✅ | 90% |
| Prompt versioning / rollback / A-B testing | `L19_MLOps_LLMOps.md`:528–595 (dedicated section) | ✅ | 90% |
| Token budget mgmt / model tier selection | `L11_2`:568–613, `L36_LLM_Observability_FinOps.md` | ✅ | 90% |
| Prompt injection defenses / grounding / PII redaction | `L01`:533, `L07`:598 strong; PII redaction only implicit in Content Safety | 🟡 | 65% |
| Claude 3 / Titan Embeddings (Bedrock models) | `CareerAccelerator/06-Amazon-Bedrock/01_concepts.md`:30,102 | 🟡 | 85% |

## Cloud & Infrastructure

| Technology | Location / path | Status | Coverage % |
|---|---|:--:|--:|
| Azure AKS | `Part7_PlatformEngineering/L34_Kubernetes_Helm_GitOps.md`:282 + `L33`:579 | ✅ | 90% |
| Azure Functions | `L08_DocumentIntelligence.md`:10, `L20_IntegrationPatterns.md`:38 (recurring, no standalone) | 🟡 | 55% |
| Azure Event Grid | `L09_AzureAISearch.md`:910–930 (dedicated subsection) | 🟡 | 75% |
| Azure Service Bus | `L20_IntegrationPatterns.md`:34, `L12`:951–952 | 🟡 | 55% |
| Azure SQL | `L06_AzureML.md`:67, `L20`:50 — one-line mentions | 🟡 | 25% |
| Azure Cosmos DB | `L08_DocumentIntelligence.md`:20,409 (53 hits, recurring) | 🟡 | 55% |
| Azure Data Factory (ADF) | `L20_IntegrationPatterns.md`:221 (dedicated subsection + ADF-vs-Functions table) | 🟡 | 80% |
| Azure Synapse | `L20_IntegrationPatterns.md`:492–500 (dedicated subsection) | 🟡 | 80% |
| Azure Blob Storage | `L06_AzureML.md`:65,274,539 (recurring, foundational) | 🟡 | 55% |
| Azure Entra ID | `L17_AzureAIFoundry.md`:1102, `L34`:290–321 | 🟡 | 80% |
| Azure Key Vault | Recurring pattern across nearly every module | 🟡 | 65% |
| AWS S3 | `CareerAccelerator/06-Amazon-Bedrock/02_architecture.md`:31,47 | 🟡 | 75% |
| AWS Lambda | `06-Amazon-Bedrock/02_architecture.md`:12,24,49 | 🟡 | 75% |
| AWS IAM | `06-Amazon-Bedrock/01_concepts.md`:19,105 (IAM vs Managed Identity comparison) | 🟡 | 75% |
| GCP Cloud Run | `CareerAccelerator/09-Vertex-AI/01_concepts.md`:74, `02_architecture.md`:23,47 | 🟡 | 50% |
| GCP BigQuery | `09-Vertex-AI/02_architecture.md`:30,57 (recurring, no dedicated section) | 🟡 | 50% |
| ARM templates | `L02_AzureAIServices_Overview.md`:211 (shared heading, Bicep-focused content) | 🟡 | 35% |
| Bicep | `L02`:211–234 + `L33_IaC_Terraform_for_Bicep_Devs.md` (dedicated module) | ✅ | 100% |
| Terraform | `Part7_PlatformEngineering/L33_IaC_Terraform_for_Bicep_Devs.md` (dedicated, 107 hits) | ✅ | 100% |
| Docker | `L07_AzureAIServices_DeepDive.md`:239–244, `L06`:47,179 (working commands, no standalone module) | 🟡 | 65% |
| Helm | `Part7_PlatformEngineering/L34_Kubernetes_Helm_GitOps.md` (dedicated module) | ✅ | 100% |
| KEDA | `05_Assessments/VitalCare_AI_Assessment_Response.md` (assessment doc, not a lesson module) | 🟡 | 55% |

## Development & DevOps

| Technology | Location / path | Status | Coverage % |
|---|---|:--:|--:|
| Python | `L21_Python_for_AI.md`, `L32_AdvancedPython_for_AI.md`, `PythonTrack/` (dedicated, extensive) | ✅ | 100% |
| C# | Primary code-example language across nearly every module (276 hits) | ✅ | 100% |
| .NET Core / .NET 10 | .NET Core/.NET 8 covered (`DealerIntelligence-Platform/README.md`:71, `L35`:100); **.NET 10 specifically — no hits** | 🟡 | 50% |
| ASP.NET Core | `L09`:1022, `L16`:1801, `L18`:51 — architecture-diagram mentions only | 🟡 | 40% |
| Azure DevOps | `L19_MLOps_LLMOps.md`:166–203 (dedicated CI/CD subsection) | 🟡 | 85% |
| GitHub Actions | `L19_MLOps_LLMOps.md`:260–265 (dedicated subsection, added 2026-07-26) | 🟡 | 85% |
| CI/CD Pipelines (general) | `L19`:166 + 77 hits across modules | 🟡 | 85% |
| Git | One-line best-practice mentions, no dedicated tutorial | 🟡 | 30% |

## Security, Compliance & Cross-Cutting Practices

| Technology | Location / path | Status | Coverage % |
|---|---|:--:|--:|
| SOC 2 compliance | `L12`:38, `L33`:513 (comparison-table entries, no dedicated module) | 🟡 | 55% |
| OAuth2 / JWT | `L26_MCP_ModelContextProtocol.md`:124–390, `L10_BotDevelopment.md`:530, `DealerIntelligence-Platform` code | 🟡 | 80% |
| Managed Identities | `L02_AzureAIServices_Overview.md`:188–229 (dedicated best-practice pattern, 133 hits) | ✅ | 90% |
| Event-driven ADF + Synapse ingestion pattern | ADF (`L20`:221) and Synapse (`L20`:492) each dedicated, but not combined as one named pattern | 🟡 | 65% |

---

## Category subtotals

| Category | Items | Avg. coverage |
|---|--:|--:|
| Certifications | 3 | 40% |
| Generative AI & LLMOps | 15 | 95% |
| AI Frameworks & Tools | 21 | 74% |
| Cloud & Infrastructure | 22 | 68% |
| Development & DevOps | 8 | 72% |
| Security/Compliance/Practices | 4 | 73% |
| **Overall (75 items)** | **75** | **~76%** |

## What actually needs attention

- **🔴 Hard zero:** AZ-204 certification content — no lesson anywhere. Not a technology gap, just an
  exam-prep track that was never built (you may not need it if AZ-204 isn't part of your active plan).
- **🟡 Weakest real gaps:** AI-103 lesson content (a gap-plan reference exists but wasn't found in the
  scanned dirs — check `04_Career/AI103-Material/` directly), Microsoft Agent Framework (MAF, only a
  passing mention), FastAPI (flagged as a known curriculum gap), raw OpenAI API (comparison-only),
  Git (no dedicated tutorial), ARM templates (Bicep gets the real content).
- **Everything else is at minimum "present but light"** — the library is overwhelmingly built around
  this exact resume's stack, which explains the 76% overall number despite zero fully-dedicated
  modules for ~30 of the 75 items.

---

## Method notes

- Two parallel searches (`grep -rniE`) across `01_Lessons/`, `02_Questions/`, `05_Assessments/`,
  `06_Supplementary/` — case-insensitive, whole-word where substring collision was a risk (e.g. `EKS`
  vs "weeks").
- Coverage % reflects **depth**, not just presence: dedicated module ≈ 90–100%, dedicated
  section/subsection ≈ 75–90%, recurring-but-uncentralized mention ≈ 50–65%, one-line/comparison-only
  ≈ 25–40%, no hits = 0%. These are judgment calls, not a formula — re-grep any row yourself if the
  number looks off.
- GCP Vertex AI was initially flagged "index-only" by the first search but the second search
  independently found a dedicated `09-Vertex-AI/` module while checking Cloud Run/BigQuery — corrected
  here to ✅ 90%.

---

## Recommended Reading Order — Efficient Pass (added 2026-08-01)

**Goal:** revise/read the fewest files that unlock the most resume rows, in an order that respects
the library's own dependency structure (Part 1 → Part 7 was designed to build on itself — Part 4
architecture assumes Part 3 GenAI concepts, Part 7 platform engineering assumes Part 4 patterns).
Within that backbone, files are sequenced **highest-leverage first** — the file that closes the most
rows in this tracker goes earliest in its stage, so time spent pays off fastest.

Most of Stages A–D are already 🟢 in your FDE tracker — this is **revision**, not first-time study, so
times below assume a skim/refresh pace, not a first read. Stage E is genuinely new material (🟠 in the
FDE tracker) and gets real study time, not a skim estimate.

### Stage A — Foundational vocabulary (do first, everything else assumes this)

| Order | File | Lines | Rows unlocked | Est. time |
|---|---|--:|--:|--:|
| 1 | `Part1_Foundations/L01_Introduction_to_AI.md` | 637 | Content Safety, Responsible AI, prompt injection/grounding (3) | 20 min |
| 2 | `Part1_Foundations/L06_AzureML.md` | 940 | Azure ML, XAI, SQL/Blob/Docker mentions (4) | 30 min |

### Stage B — Highest-leverage GenAI core (biggest payoff per hour)

| Order | File | Lines | Rows unlocked | Est. time |
|---|---|--:|--:|--:|
| 3 | `Part2_AzureAIServices/L09_AzureAISearch.md` | 1,117 | Vector Embeddings, Azure AI Search, Vector DB comparison, HNSW, Event Grid (5) | 35 min |
| 4 | `Part3_GenAI_LLMs/L12_AzureOpenAI_Services.md` | 1,016 | Azure OpenAI, raw OpenAI API, function calling, SOC 2 mention (5) | 30 min |
| 5 | `Part3_GenAI_LLMs/L16_AIOrchestration_SK_Agents.md` | 2,084 | Semantic Kernel, LangChain, Prompt Flow, ASP.NET Core mention (4) | 50 min |
| 6 | `Part3_GenAI_LLMs/L13_RAG_DeepDive.md` | 1,527 | RAG Architecture, GraphRAG section (2, but foundational for everything downstream) | 40 min |
| 7 | `Part3_GenAI_LLMs/L15_PromptEngineering.md` | 782 | Prompt Engineering (1, foundational) | 25 min |
| 8 | `Part3_GenAI_LLMs/L11_1_LLMs_Attention_Transformer.md` + `L11_2_...Tokenization_Embeddings.md` | 359 + 643 | LoRA/QLoRA theory, token budget mgmt (2) | 30 min |
| 9 | `Part3_GenAI_LLMs/L14_FineTuning.md` | 795 | HF Transformers, LoRA/QLoRA depth (2) | 25 min |

### Stage C — Architecture & LLMOps (ties GenAI core into systems)

| Order | File | Lines | Rows unlocked | Est. time |
|---|---|--:|--:|--:|
| 10 | `Part4_Architecture/L20_IntegrationPatterns.md` | 602 | Functions, Service Bus, SQL, ADF, Synapse, event-driven pattern (**6 — highest single-file leverage in this whole list**) | 20 min |
| 11 | `Part4_Architecture/L19_MLOps_LLMOps.md` | 757 | LLMOps, prompt versioning, Azure DevOps, GitHub Actions, CI/CD (5) | 25 min |
| 12 | `Part4_Architecture/L17_AzureAIFoundry.md` | 1,102 | Azure AI Foundry, Entra ID mention (2) | 30 min |
| 13 | `Part4_Architecture/L18_AISolutionArchitecture.md` | 509 | Agentic Architectures (1–2) | 20 min |
| 14 | `Part5_AgenticProtocols/L26_MCP_ModelContextProtocol.md` | 390 | MCP, OAuth2/JWT (2) | 15 min |
| 15 | `Part5_AgenticProtocols/L25_AgentFramework_Comparison.md` | 258 | LangGraph, AutoGen (2, shortest file on the list) | 10 min |

### Stage D — Applied project modules (dedicated, self-contained, quick)

| Order | Module | Rows unlocked | Est. time |
|---|---|--:|--:|
| 16 | `CareerAccelerator/06-Amazon-Bedrock/` | Bedrock, Claude 3/Titan, S3, Lambda, IAM (5) | 25 min |
| 17 | `CareerAccelerator/09-Vertex-AI/` | Vertex AI, Cloud Run, BigQuery (3) | 20 min |
| 18 | `CareerAccelerator/02-crewAI-MultiAgent/` | crewAI, MAF mention (2) | 15 min |
| 19 | `CareerAccelerator/05-LlamaIndex-RAG/` | LlamaIndex (1) | 15 min |
| 20 | `CareerAccelerator/03-RAGAS-Evaluation/` | RAGAS (1) | 15 min |
| 21 | `CareerAccelerator/07-GraphRAG-Neo4j/` | GraphRAG + Neo4j (1) | 15 min |

**Subtotal, Stages A–D (revision pace): ~8.75 hrs.**

### Stage E — Platform Engineering (genuinely new — 🟠 in FDE tracker, real study time)

| Order | File | Lines | Rows unlocked | Est. time |
|---|---|--:|--:|--:|
| 22 | `Part7_PlatformEngineering/L33_IaC_Terraform_for_Bicep_Devs.md` | 647 | AKS, Bicep, Terraform, SOC 2 (4) | 1.5 hrs |
| 23 | `Part7_PlatformEngineering/L34_Kubernetes_Helm_GitOps.md` | 501 | AKS, Entra ID, Helm (3) | 1.75 hrs |
| 24 | `Part7_PlatformEngineering/L36_LLM_Observability_FinOps.md` | 528 | Token budget mgmt depth (1) | 2.8 hrs |
| 25 | `Part7_PlatformEngineering/L32_AdvancedPython_for_AI.md` | 762 | Python depth, FastAPI (2) | 5.0 hrs |

**Subtotal, Stage E: ~11.05 hrs** — same modules, same estimates as your FDE tracker's S2/S3/S6; this
tracker doesn't duplicate that study time, it just tells you these are the ones this *resume*
specifically needs too.

### Stage F — Light supplements (low leverage, quick, do last or skip if time-boxed)

| Order | File | Rows unlocked | Est. time |
|---|---|--:|--:|
| 26 | `Part1_Foundations/L02_AzureAIServices_Overview.md` | ARM, Bicep intro, Managed Identities (3) | 15 min |
| 27 | `Part2_AzureAIServices/L07_AzureAIServices_DeepDive.md` | Docker commands, Ollama mention (2) | 15 min |
| 28 | `Part2_AzureAIServices/L08_DocumentIntelligence.md` | Doc Intelligence, Cosmos DB, Functions mention (3) | 15 min |

*(`L21_Python_for_AI.md` deliberately skipped — your FDE tracker already flags it as superseded by
`L32` for writing-level Python; read-level only if you want the C#→Python translation reference.)*

**Subtotal, Stage F: ~45 min.**

### Not fixed by reading — verify or accept as-is, not a lesson

| Item | Action needed |
|---|---|
| AZ-204 certification | No lesson exists — skip, or treat as a separate build task if this cert becomes a real priority |
| AI-103 lesson content | Check `04_Career/AI103-Material/` directly — outside this audit's scanned folders, may already have what you need |
| .NET 10 (specifically) | Library targets .NET 8 — a version gap, not a content gap. Nothing to read; note the delta yourself |
| Git | No dedicated tutorial in the library — external resource if you need one |
| KEDA | Only in `05_Assessments/VitalCare_AI_Assessment_Response.md` (an assessment doc, not a lesson) — quick read of that section covers it |
| Ollama exact module path | Flagged uncertain during the audit — confirm the folder name under `CareerAccelerator/` before relying on it |

### Grand total

**~20.5 hrs** (8.75 revision + 11.05 new study + 0.45 light supplements) + the six non-reading items
above. This lines up closely with your FDE tracker's own "~20 hours separates current state from
full readiness" verdict — good cross-check that both audits are pointing at the same real gap
(Platform Engineering / Stage E), not two different numbers for two different problems.
