# AI Solutions Architect — Learning Index
**Last Updated:** 2026-06-30 | **Total Modules:** 20 | **Certification:** AI-102 ✅ COMPLETED

---

## Complete Learning Order — All 20 Modules

| Order | File | Topic | Part | Key Dependency | Notes |
|---|---|---|---|---|---|
| **1** | `L01_Introduction_to_AI.md` | Introduction to AI | Part 1 | None — start here | Foundation vocabulary |
| **2** | `L02_AzureAIServices_Overview.md` | Azure AI Services Overview | Part 1 | L01 | Unlocks L03–L07 |
| **3** | `L03_NLP_Fundamentals.md` | Natural Language Processing | Part 1 | L02 | ⚡ Feeds directly into L11 (LLMs) |
| **4** | `L04_ComputerVision.md` | Computer Vision | Part 1 | L02 | Parallel with L03 |
| **5** | `L05_SpeechServices.md` | Speech Services | Part 1 | L02 | Parallel with L03, L04 |
| **6** | `L06_AzureML.md` | Azure Machine Learning | Part 1 | L01 | Standalone — low dependency |
| **7** | `L07_AzureAIServices_DeepDive.md` | Azure AI Services Deep Dive | Part 2 | L02 + L03 + L04 + L05 | Must come after all domain modules |
| **8** | `L08_DocumentIntelligence.md` | Document Intelligence | Part 2 | L07 | |
| **9** | `L09_AzureAISearch.md` | Azure AI Search | Part 2 | L07 | ⚡ Must come before L13 (RAG) |
| **10** | `L10_BotDevelopment.md` | Bot Development | Part 2 | L07 + L03 | Needs CLU from L03 |
| **11** | `L11_1_LLMs_Attention_Transformer.md` | LLMs — Attention & Transformer | Part 3 | L01 + L03 | |
| **11** | `L11_2_LLMs_Tokenization_Embeddings.md` | LLMs — Tokenization & Embeddings | Part 3 | L11_1 | |
| **11** | `L11_3_LLMs_Pretraining_Finetuning.md` | LLMs — Pretraining & Fine-Tuning | Part 3 | L11_1 | |
| **11** | `L11_4_LLMs_RLHF_Alignment.md` | LLMs — RLHF & Alignment | Part 3 | L11_1 | |
| **12** | `L12_AzureOpenAI_Services.md` | Azure OpenAI Service | Part 3 | L02 + L11 | Unlocks L13–L17 |
| **13** | `L13_RAG_DeepDive.md` | RAG Deep Dive | Part 3 | L09 + L12 | Needs both AI Search AND OpenAI |
| **14** | `L14_FineTuning.md` | Fine-Tuning LLMs | Part 3 | L11 + L12 | ⚡ Parallel with L13 — no dependency between them |
| **15** | `L15_PromptEngineering.md` | Prompt Engineering | Part 3 | L12 | ⚡ Parallel with L13, L14 |
| **16** | `L16_AIOrchestration_SK_Agents.md` | AI Orchestration — SK & Agents | Part 3 | L12 + L13 | Must come AFTER RAG |
| **17** | `L17_AzureAIFoundry.md` | Azure AI Foundry | Part 4 | L12 + L13 + L14 + L15 + L16 | Connects all Part 3 modules |
| **18** | `L18_AISolutionArchitecture.md` | AI Solution Architecture | Part 4 | All above | |
| **19** | `L19_MLOps_LLMOps.md` | MLOps / LLMOps | Part 4 | L17 + L18 | |
| **20** | `L20_IntegrationPatterns.md` | Integration Patterns | Part 4 | Everything above | |
| **21** | `L21_Python_for_AI.md` | Python for AI | Part 4 | None — standalone | Bridges to 85% of job postings |

---

## Why 3 Modules Are Out of Numeric Order

| Original # | Learns at Position | Reason |
|---|---|---|
| Module 4 (NLP) | Position 3 — before Module 3 (CV) | NLP feeds into Module 11 (LLMs) — higher dependency priority |
| Module 15 (Fine-Tuning) | Position 14 — before Module 14 (Orchestration) | Fine-Tuning only needs L11+L12. Orchestration needs RAG (L13) first |
| Module 16 (Prompt Engineering) | Position 15 — before Module 14 (Orchestration) | Prompt Engineering only needs L12. Same reasoning |

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
    │         │                   ├──► L08 (Document Intelligence)  │
    │         │                   ├──► L09 (AI Search) ─────────────┼──┐
    │         │                   └──► L10 (Bot Dev)                │  │
    │         │                                                      │  │
    │         └──► L12 (Azure OpenAI) ◄── L11 (LLMs) ◄─────────────┘  │
    │                   │                                               │
    └──► L06 (Azure ML) │                                               │
                        ├──► L13 (RAG) ◄────────────────────────────────┘
                        ├──► L14 (Fine-Tuning)
                        ├──► L15 (Prompt Engineering)
                        │
                        └──► L16 (Orchestration / SK / Agents) ◄── needs L13
                                   │
                              L17 (Azure AI Foundry)
                                   │
                              L18 (Solution Architecture)
                                   │
                              L19 (MLOps / LLMOps)
                                   │
                              L20 (Integration Patterns)
```

---

## Coverage Status vs MasterCoverageTable

| Status | Count | Skills |
|---|---|---|
| ✅ **Fully covered (theory)** | 15 | Azure AI Foundry, AI Agents, GenAI/LLMs, RAG, Azure AI Services, Fine-tuning, Document Intelligence, Prompt injection, PII detection, Chunking/HNSW, Tokenization, Transformer theory, LoRA/PEFT/RLHF, Bot Dev, Integration Patterns |
| ⚠️ **Theory done — hands-on pending** | 8 | Evaluation pipelines, Content Safety Studio, LLMOps Monitoring portal, CI/CD DevOps pipeline, SK C# project, Streaming code, SK Plugins, Grounding validation code |
| 🔴 **Not done** | 5 | LangChain, Python for AI, Amazon Bedrock, Microsoft Fabric, Vertex AI |
| 🟡 **Partial** | 1 | Graph/Vector Datastores (AI Search vectors ✅, CosmosDB vector ❌) |

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

---

*Curriculum: AI Solutions Architect — 20 Modules, 4 Parts*
*Files renamed to learning sequence order: 2026-06-30*
