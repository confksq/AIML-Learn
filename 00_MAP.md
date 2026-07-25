# AIML-Learn — Quick Map

**34 lessons · 6 Parts.** One line per module. For detail use `00_CONTENTS.md`; to find a topic use `00_INDEX.md`.

---


## Part 1 — Foundations

*Start here. Vocabulary, Azure AI surface, core ML concepts.*

| # | Module | Lines | Topics |
|---|---|---:|---:|
| 01 | **Introduction to AI**<br>`L01_Introduction_to_AI.md` | 637 | 40 |
| 02 | **Azure AI Services Overview**<br>`L02_AzureAIServices_Overview.md` | 501 | 28 |
| 03 | **Natural Language Processing**<br>`L03_NLP_Fundamentals.md` | 734 | 47 |
| 04 | **Computer Vision Fundamentals**<br>`L04_ComputerVision.md` | 543 | 36 |
| 05 | **Speech Services**<br>`L05_SpeechServices.md` | 601 | 32 |
| 06 | **Azure Machine Learning**<br>`L06_AzureML.md` | 940 | 45 |


## Part 2 — Azure AI Services

*The services you build on. L09 must precede L13.*

| # | Module | Lines | Topics |
|---|---|---:|---:|
| 07 | **Azure AI Services Deep Dive**<br>`L07_AzureAIServices_DeepDive.md` | 648 | 35 |
| 08 | **Document Intelligence**<br>`L08_DocumentIntelligence.md` | 813 | 43 |
| 09 | **Azure AI Search**<br>`L09_AzureAISearch.md` | 1117 | 57 |
| 10 | **Bot Development**<br>`L10_BotDevelopment.md` | 637 | 30 |


## Part 3 — GenAI & LLMs

*How LLMs work, then RAG, fine-tuning, prompting, orchestration.*

| # | Module | Lines | Topics |
|---|---|---:|---:|
| 11.1 | **How LLMs Work: Attention & Transformer Architecture**<br>`L11_1_LLMs_Attention_Transformer.md` | 359 | 26 |
| 11.2 | **Tokenization & Embeddings (Deep Dive)**<br>`L11_2_LLMs_Tokenization_Embeddings.md` | 578 | 39 |
| 11.3 | **Pre-training & Fine-tuning**<br>`L11_3_LLMs_Pretraining_Finetuning.md` | 543 | 38 |
| 11.4 | **RLHF & Alignment**<br>`L11_4_LLMs_RLHF_Alignment.md` | 511 | 38 |
| 12 | **Azure OpenAI Service**<br>`L12_AzureOpenAI_Services.md` | 1016 | 59 |
| 13 | **RAG (Retrieval-Augmented Generation) Deep Dive**<br>`L13_RAG_DeepDive.md` | 1527 | 69 |
| 14 | **Fine-tuning LLMs**<br>`L14_FineTuning.md` | 795 | 32 |
| 15 | **Prompt Engineering**<br>`L15_PromptEngineering.md` | 782 | 37 |
| 16 | **AI Orchestration: Semantic Kernel, LangChain & AI Agents**<br>`L16_AIOrchestration_SK_Agents.md` | 2084 | 81 |


## Part 4 — Architecture & Operations

*Designing, deploying and running AI in production.*

| # | Module | Lines | Topics |
|---|---|---:|---:|
| 17 | **Azure AI Foundry**<br>`L17_AzureAIFoundry.md` | 1102 | 48 |
| 18 | **AI Solution Architecture**<br>`L18_AISolutionArchitecture.md` | 509 | 29 |
| 19 | **MLOps and LLMOps**<br>`L19_MLOps_LLMOps.md` | 675 | 36 |
| 20 | **Integration Patterns**<br>`L20_IntegrationPatterns.md` | 602 | 27 |
| 21 | **Python for AI**<br>`L21_Python_for_AI.md` | 889 | 44 |


## Part 5 — Agentic Protocols & Patterns

*Agent frameworks and protocols. Written as spoken briefings.*

| # | Module | Lines | Topics |
|---|---|---:|---:|
| 22 | **Azure AI Foundry: Platform, Agent Lifecycle, and Healthcare Architecture**<br>`L22_Foundry_AgentLifecycle.md` | 323 | 16 |
| 23 | **CAG vs RAG**<br>`L23_CAG_vs_RAG.md` | 465 | 29 |
| 24 | **Hallucination: Factual + Agentic**<br>`L24_Hallucination_Mitigation.md` | 392 | 23 |
| 25 | **Framework Comparison: LangGraph vs AutoGen vs Semantic Kernel**<br>`L25_AgentFramework_Comparison.md` | 258 | 10 |
| 26 | **MCP Hub: What It Is, How It Works, and Why Healthcare Needs It**<br>`L26_MCP_ModelContextProtocol.md` | 390 | 16 |
| 27 | **Agent Workflow CENTERPIECE**<br>`L27_Agent_Workflow_EndToEnd.md` | 762 | 27 |
| 28 | **Meta-Agent Hierarchies: Agents of Agents**<br>`L28_MetaAgent_Hierarchies.md` | 164 | 9 |
| 29 | **A2A Protocol: Agent-to-Agent Communication**<br>`L29_A2A_Protocol.md` | 182 | 10 |
| 30 | **OCR Pipelines: Azure Document Intelligence vs John Snow Labs**<br>`L30_OCR_Pipelines.md` | 231 | 12 |
| 31 | **Fault Tolerance, Self-Healing Agents & Observability**<br>`L31_FaultTolerance_Observability.md` | 422 | 26 |


## Part 6 — Applied Projects

*Not sequenced — take alongside the Parts they support. Carries runnable code and resume bullets.*

| Item | Supports | What it is |
|---|---|---|
| `01-CareerAccelerator/` | L13 · L14 · L16 | 9 tool modules: Ollama · crewAI · RAGAS · HuggingFace · LlamaIndex · Bedrock · GraphRAG+Neo4j · LoRA · Vertex AI |
| `02-DealerIntelligence-Platform/` | L16 · L18 · Part 5 | C# 9-layer agentic platform + real JMA production flow doc |
| `05-VitalCare-AI-Platform/` | L16 · L18 · Part 5 | Same architecture, healthcare prior-auth domain |

---

## Also worth knowing

| Where | What |
|---|---|
| `02_Questions/HighLevelPrep/HLP01_...md` | Memory · tokens · scaling · agents at interview altitude |
| `02_Questions/InterviewBank/` | 6 architect-judgment question sets |
| `02_Questions/PerChapter/` | Self-test per lesson — **L06–L21 only, none for L01–L05** |
| `06_Supplementary/PythonTrack/` | Framework-free Python: raw agent loop, FAISS, PEFT |
| `08_Jobs/AscndIntr/PrepPlan/` | Mock interview + defend-assessment |