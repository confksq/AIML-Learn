# AI Solutions Architect — Learning Index

**Last Updated:** 2026-07-26 | **Total Modules:** 36 across 7 Parts | **Certification:** AI-102 ✅ COMPLETED

> ⚠️ **This file was stale from 2026-06-30 to 2026-07-26** — it listed 20 modules, stopped at `L21`,
> and did not know Part 5 (Agentic Protocols), Part 6 (Applied Projects) or Part 7 (Platform
> Engineering) existed. It is the file people naturally open, because it has "AI Solutions
> Architect" in the title, so the drift did real damage.
>
> **This file is authoritative on *order*. `00_START_HERE.md` is authoritative on *progress*.**
> If they disagree, `00_START_HERE.md` wins.

---

## Complete Learning Order — All 36 Modules

Numbered in dependency order. Following the numbers is always safe.

### Part 1 — Foundations · `Part1_Foundations/`

| # | File | Topic | Depends on | Notes |
|---|---|---|---|---|
| 1 | `L01_Introduction_to_AI.md` | Introduction to AI | — start here | Foundation vocabulary |
| 2 | `L02_AzureAIServices_Overview.md` | Azure AI Services Overview | L01 | Unlocks L03–L07 |
| 3 | `L03_NLP_Fundamentals.md` | Natural Language Processing | L02 | ⚡ Feeds L11 |
| 4 | `L04_ComputerVision.md` | Computer Vision | L02 | Parallel with L03 |
| 5 | `L05_SpeechServices.md` | Speech Services | L02 | Parallel with L03, L04 |
| 6 | `L06_AzureML.md` | Azure Machine Learning | L01 | Standalone |

### Part 2 — Azure AI Services · `Part2_AzureAIServices/`

| # | File | Topic | Depends on | Notes |
|---|---|---|---|---|
| 7 | `L07_AzureAIServices_DeepDive.md` | Azure AI Services Deep Dive | L02+L03+L04+L05 | After all domain modules |
| 8 | `L08_DocumentIntelligence.md` | Document Intelligence | L07 | |
| 9 | `L09_AzureAISearch.md` | Azure AI Search | L07 | ⚡ **Must precede L13** |
| 10 | `L10_BotDevelopment.md` | Bot Development | L07+L03 | Needs CLU from L03 |

### Part 3 — GenAI & LLMs · `Part3_GenAI_LLMs/`

| # | File | Topic | Depends on | Notes |
|---|---|---|---|---|
| 11a | `L11_1_LLMs_Attention_Transformer.md` | Attention & Transformer | L01+L03 | |
| 11b | `L11_2_LLMs_Tokenization_Embeddings.md` | Tokenization & Embeddings | L11_1 | |
| 11c | `L11_3_LLMs_Pretraining_Finetuning.md` | Pretraining & Fine-Tuning | L11_1 | |
| 11d | `L11_4_LLMs_RLHF_Alignment.md` | RLHF & Alignment | L11_1 | |
| 12 | `L12_AzureOpenAI_Services.md` | Azure OpenAI Service | L02+L11 | Unlocks L13–L17 |
| 13 | `L13_RAG_DeepDive.md` | RAG Deep Dive | **L09 + L12** | Needs both |
| 14 | `L14_FineTuning.md` | Fine-Tuning LLMs | L11+L12 | ⚡ Parallel with L13 |
| 15 | `L15_PromptEngineering.md` | Prompt Engineering | L12 | ⚡ Parallel with L13, L14 |
| 16 | `L16_AIOrchestration_SK_Agents.md` | AI Orchestration — SK & Agents | L12+L13 | **After RAG.** Largest lesson (2,084 lines) |

### Part 4 — Architecture & Operations · `Part4_Architecture/`

| # | File | Topic | Depends on | Notes |
|---|---|---|---|---|
| 17 | `L17_AzureAIFoundry.md` | Azure AI Foundry | all of Part 3 | |
| 18 | `L18_AISolutionArchitecture.md` | AI Solution Architecture | everything above | |
| 19 | `L19_MLOps_LLMOps.md` | MLOps / LLMOps | L17+L18 | GitHub Actions section added 2026-07-26 |
| 20 | `L20_IntegrationPatterns.md` | Integration Patterns | everything above | |
| 21 | `L21_Python_for_AI.md` | Python for AI | — standalone | **Read-level.** Superseded by `L32` for writing-level |

### Part 5 — Agentic Protocols & Patterns · `Part5_AgenticProtocols/`

*Promoted 2026-07-19 from `08_Jobs/AscndIntr/PrepPlan/`. Spoken briefings rather than reference
chapters — denser and more opinionated than Parts 1–4.*

| # | File | Topic | Depends on | Notes |
|---|---|---|---|---|
| 22 | `L22_Foundry_AgentLifecycle.md` | Foundry & Agent Lifecycle | L17 | Overlaps and updates L17 |
| 23 | `L23_CAG_vs_RAG.md` | CAG vs RAG | L13 | **Only CAG material** |
| 24 | `L24_Hallucination_Mitigation.md` | Hallucination Mitigation | L13+L15 | Factual + agentic |
| 25 | `L25_AgentFramework_Comparison.md` | LangGraph vs AutoGen vs SK | L16 | **The real LangGraph lesson** |
| 26 | `L26_MCP_ModelContextProtocol.md` | Model Context Protocol | L16 | **Only MCP material** |
| 27 | `L27_Agent_Workflow_EndToEnd.md` | Agent Workflow End-to-End | L16+L26 | 762 lines — the centrepiece |
| 28 | `L28_MetaAgent_Hierarchies.md` | Meta-Agent Hierarchies | L27 | Agents of agents |
| 29 | `L29_A2A_Protocol.md` | A2A Protocol | L27 | **Only A2A material** |
| 30 | `L30_OCR_Pipelines.md` | OCR Pipelines | L08 | Document Intelligence vs John Snow Labs |
| 31 | `L31_FaultTolerance_Observability.md` | Fault Tolerance & Observability | L27 | Polly retry, circuit breaker, self-healing |

### Part 6 — Applied Projects · `Part6_AppliedProjects/`

*Not sequenced — take alongside the Parts they support. Carries runnable code and résumé bullets.*

| Item | Supports | Contents |
|---|---|---|
| `01-CareerAccelerator/` | L13, L14, L16 | 9 tool modules — Ollama · crewAI · RAGAS · HuggingFace · LlamaIndex · Bedrock · GraphRAG+Neo4j · LoRA · Vertex AI |
| `02-DealerIntelligence-Platform/` | L16, L18, Part 5 | C# 9-layer agentic platform + real production flow doc |
| **`03-SecurityAutomation-VulnScan/`** | L31, L33, L35 | **Added 2026-07-26.** Sanitised write-up of a shipped malware-scanning gateway + résumé bullets |
| `05-VitalCare-AI-Platform/` | L16, L18, Part 5 | Same architecture, healthcare prior-auth domain |

> ⚠️ The numbered tool modules live **inside** `01-CareerAccelerator/`, not directly under
> `Part6_AppliedProjects/`.

### Part 7 — Platform Engineering & AI-Assisted Delivery · `Part7_PlatformEngineering/`

*Built 2026-07-26 for **FDE-Prep**. Closes the engineering-hands gaps the AI-102 / architect
curriculum deliberately scoped out. Each module leads from something you already have.*

| # | File | Topic | Depends on | Notes |
|---|---|---|---|---|
| 32 | `L32_AdvancedPython_for_AI.md` | Advanced Python | L21 | **Supersedes L21** for writing-level Python |
| 33 | `L33_IaC_Terraform_for_Bicep_Devs.md` | IaC / Terraform | Bicep knowledge | Leads with **state ownership**, not HCL syntax |
| 34 | `L34_Kubernetes_Helm_GitOps.md` | Kubernetes, Helm, GitOps | L33 | Assumes AKS familiarity |
| 35 | `L35_AI_Assisted_Engineering.md` | Cursor · Copilot · computer-use | L15 | Mostly *doing*, not reading |
| 36 | `L36_LLM_Observability_FinOps.md` | Observability, tracing, FinOps | L31, L19 | Extends `L31` §4–5 |

---

## Why some modules sit out of original numeric order

| Original # | Learns at position | Reason |
|---|---|---|
| Module 4 (NLP) | 3 — before Computer Vision | NLP feeds L11 (LLMs) — higher dependency priority |
| Module 15 (Fine-Tuning) | 14 — before Orchestration | Needs only L11+L12; Orchestration needs RAG first |
| Module 16 (Prompt Engineering) | 15 — before Orchestration | Needs only L12. Same reasoning |
| Module 32 (Advanced Python) | Part 7, not Part 4 beside L21 | It supersedes L21's ceiling; burying it in Part 4 would hide it from Part-level search |

---

## Dependency Flow (Visual)

```
L01 (Intro to AI)
    │
    ├──► L02 (Azure AI Services Overview)
    │         │
    │         ├──► L03 (NLP) ──────────────────────────────────────┐
    │         ├──► L04 (Computer Vision)                            │
    │         ├──► L05 (Speech Services)                            │
    │         │         │                                           │
    │         │         └──► L07 (Azure AI Services Deep Dive)      │
    │         │                   │                                 │
    │         │                   ├──► L08 (Document Intelligence) ─┼──► L30 (OCR)
    │         │                   ├──► L09 (AI Search) ─────────────┼──┐
    │         │                   └──► L10 (Bot Dev)                │  │
    │         │                                                     │  │
    │         └──► L12 (Azure OpenAI) ◄── L11 (LLMs) ◄──────────────┘  │
    │                   │                                               │
    └──► L06 (Azure ML) │                                               │
                        ├──► L13 (RAG) ◄────────────────────────────────┘
                        │        └──► L23 (CAG vs RAG) · L24 (Hallucination)
                        ├──► L14 (Fine-Tuning)
                        ├──► L15 (Prompt Engineering) ──────────► L35 (AI-Assisted Eng)
                        │
                        └──► L16 (Orchestration / SK / Agents) ◄── needs L13
                                   │
                                   ├──► L25 (Frameworks) · L26 (MCP)
                                   │           │
                                   │      L27 (Agent Workflow E2E)
                                   │           ├──► L28 (Meta-Agents)
                                   │           ├──► L29 (A2A)
                                   │           └──► L31 (Fault Tolerance) ──┐
                                   │                                         │
                              L17 (Azure AI Foundry) ──► L22 (Agent Lifecycle)
                                   │                                         │
                              L18 (Solution Architecture)                    │
                                   │                                         │
                              L19 (MLOps / LLMOps) ──────────────────────────┼──► L36 (LLM Obs
                                   │                                         │      + FinOps)
                              L20 (Integration Patterns)                     │
                                   │                                         │
                              L21 (Python — read level)                      │
                                   │                                         │
                              L32 (Python — write level)                     │
                                                                             │
              [Bicep / Azure DevOps you already have] ──► L33 (IaC) ──► L34 (K8s/Helm/GitOps)
```

---

## Quick Reference — What File to Open

| I want to learn... | Open this file |
|---|---|
| What is AI, ML, Deep Learning? | `L01_Introduction_to_AI.md` |
| Azure AI Services landscape map | `L02_AzureAIServices_Overview.md` |
| Sentiment, NER, PII, CLU, QA | `L03_NLP_Fundamentals.md` |
| Image analysis, OCR, Custom Vision | `L04_ComputerVision.md` |
| Speech-to-text, TTS, diarization | `L05_SpeechServices.md` |
| AutoML, model training, MLflow | `L06_AzureML.md` |
| Custom models, security, throttling | `L07_AzureAIServices_DeepDive.md` |
| Form extraction, layout, prebuilt models | `L08_DocumentIntelligence.md` |
| Vector search, hybrid search, indexing | `L09_AzureAISearch.md` |
| Teams bot, dialogs, intents | `L10_BotDevelopment.md` |
| Attention, transformers, context windows | `L11_1_LLMs_Attention_Transformer.md` |
| Tokens, BPE, embeddings, cosine similarity | `L11_2_LLMs_Tokenization_Embeddings.md` |
| Pretraining, LoRA, PEFT, fine-tuning theory | `L11_3_LLMs_Pretraining_Finetuning.md` |
| RLHF, alignment, content safety, EU AI Act | `L11_4_LLMs_RLHF_Alignment.md` |
| GPT-4o, o1/o3, Structured Outputs, Batch API | `L12_AzureOpenAI_Services.md` |
| RAG pipeline, chunking, GraphRAG, citations | `L13_RAG_DeepDive.md` |
| GPT-4o fine-tuning, JSONL, distillation | `L14_FineTuning.md` |
| Chain-of-thought, few-shot, prompt caching | `L15_PromptEngineering.md` |
| Semantic Kernel, plugins, agents, multi-agent | `L16_AIOrchestration_SK_Agents.md` |
| AI Foundry portal, agents, evaluation, tracing | `L17_AzureAIFoundry.md` |
| Architecture patterns, cost, security, latency | `L18_AISolutionArchitecture.md` |
| Golden datasets, CI/CD, drift detection | `L19_MLOps_LLMOps.md` |
| Azure Functions, Event Grid, M365 Copilot, APIM | `L20_IntegrationPatterns.md` |
| **Reading** Python written by data scientists | `L21_Python_for_AI.md` |
| CAG vs RAG | `L23_CAG_vs_RAG.md` |
| **LangGraph, AutoGen, CrewAI, framework choice** | `L25_AgentFramework_Comparison.md` |
| **MCP** | `L26_MCP_ModelContextProtocol.md` |
| End-to-end agent workflow | `L27_Agent_Workflow_EndToEnd.md` |
| Supervisor / agents-of-agents | `L28_MetaAgent_Hierarchies.md` |
| **A2A** | `L29_A2A_Protocol.md` |
| Retry, circuit breaker, self-healing agents | `L31_FaultTolerance_Observability.md` |
| **Decorators, generators, dataclasses, Big-O** | `L32_AdvancedPython_for_AI.md` |
| **Terraform, Pulumi, CDK, Ansible, PrivateLink** | `L33_IaC_Terraform_for_Bicep_Devs.md` |
| **Helm, ArgoCD, EKS, service mesh, OpenShift** | `L34_Kubernetes_Helm_GitOps.md` |
| **Cursor, Copilot practice, computer-use, N8N** | `L35_AI_Assisted_Engineering.md` |
| **OTel, LangSmith, LiteLLM, FinOps, Grafana** | `L36_LLM_Observability_FinOps.md` |
| Memory architecture (incl. episodic) | `02_Questions/HighLevelPrep/HLP01_...md` |

---

## Also in the library

| Where | What |
|---|---|
| `02_Questions/InterviewBank/` | 6 architect-judgment question sets |
| `02_Questions/PerChapter/` | Self-test per lesson — `QA_L06`–`QA_L21` and `QA_L32`–`QA_L36`. ⚠️ **None for L01–L05 or L22–L31** |
| `04_Career/` | Roadmaps, PRDs, resume, JD coverage, AI-103 gap plan |
| `05_Assessments/` | VitalCare cloud-agnostic healthcare AI architecture (1,562 lines) |
| `06_Supplementary/PythonTrack/` | Framework-free Python: raw agent loop, FAISS, PEFT. ⚠️ `AIMLcurriculum*.md` are **syllabi, not lessons** |
| `08_Jobs/FDE/` | **FDE-Prep** tracker (60 JD rows) + IaC glossary |
| `_Archive/` | Superseded material — kept, not deleted |

---

## Ground rules

1. **`00_START_HERE.md` is authoritative on progress.** This file is authoritative on *order*.
2. Search `00_INDEX.md` before concluding a topic is not covered — teaching material also lives in
   `Part6_AppliedProjects/*/01_concepts.md` and `06_Supplementary/PythonTrack/`.
3. Use `grep -w` for short terms. `EKS` matches "wEEKS"; `Arize` matches "summARIZE".

*Curriculum: AI Solutions Architect — 36 modules, 7 Parts. Renamed to sequence order 2026-06-30;
reorganised 2026-07-19; Part 7 added 2026-07-26.*
