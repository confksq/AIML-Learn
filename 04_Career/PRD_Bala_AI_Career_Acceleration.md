# PRD: Bala AI Career Acceleration System
**Version:** 2.1  
**Owner:** Balamurugan Kittappa  
**Created:** July 4, 2026  
**Updated:** July 4, 2026 (v2.1) — Retargeted to existing repo `confksq/Learning`; all L1–L8 modules now land under `Project/AIML-Learn/PartsModules/CareerAccelerator/`  
**Status:** ✅ **DELIVERED 2026-08-03** — 9/9 modules built. ⚠️ **All paths in this document are stale — see the PATH CORRECTION block below before executing anything.**  
**Source Data:** 4-month Gmail inbox analysis (67+ real JDs, Sept 2025–July 2026)  
**Goal:** Get shortlisted for Senior AI Engineer roles within 60 days, transition to AI Architect within 18 months.

---

## HOW TO USE THIS PRD

### For Claude.ai (this chat):
Paste any Feature section and say: **"Execute this PRD feature completely."**

### For Claude Code CLI (GitHub push):
```bash
claude "Execute PRD Feature L1 from PRD_Bala_AI_Career_Acceleration.md — 
generate all files and push to confksq/Learning under 
Project/AIML-Learn/PartsModules/CareerAccelerator/01-Ollama-LocalRAG/"
```

Claude Code has write access to GitHub and will create files + push directly.

### TARGET LOCATION (v2.1 — ⚠️ SUPERSEDED, see below)
~~All L1–L8 modules land in the **existing** `confksq/Learning` repo (private) under
`Project/AIML-Learn/PartsModules/CareerAccelerator/NN-ModuleName/`.~~

> ### ⚠️ PATH CORRECTION (2026-08-03) — read this before executing any feature below
>
> **Status: ✅ DELIVERED — all 9 CareerAccelerator modules built.** The per-feature sections below
> are kept as the build record, but **every path in them is dead.** Two things changed after v2.1:
>
> | v2.1 said | Actually |
> |---|---|
> | Repo `confksq/Learning` | **`confksq/AIML-Learn`** |
> | `Project/AIML-Learn/PartsModules/CareerAccelerator/` | **`01_Lessons/Part6_AppliedProjects/01-CareerAccelerator/`** |
> | Working copy `C:\Users\confksq\Project\AIML-Learn\` | **`C:\pers\AIML-Learn\`** (git clone, 2026-08-03) |
> | `Questions/` interview bank | **`02_Questions/InterviewBank/`** — now 7 tiers, see `00_PRD.md` |
>
> `PartsModules/` has not existed since the 2026-07-18 library reorganisation. Do not recreate it.
> The authoritative file map is `00_START_HERE.md`; current work is tracked in
> `Consolidation_and_Update_Plan_2026-08-03.md`.

---

## SYSTEM CONTEXT

```
Candidate:        Balamurugan Kittappa
Current Title:    Senior Azure AI Engineer & AI Foundry Architect
Employer:         JM Family Enterprise, Tampa FL (via KPMG consulting)
Experience:       17+ years total, 2+ years production AI/GenAI
Certifications:   AI-102, AI-103, AZ-204
Resume file:      Bala_Azure_AI_Engineer_AI-102_AI-103.pdf / .docx
GitHub user:      confksq
GitHub repos:     confksq/AI-RandD (C# AI projects)
                  confksq/AzureAI-RandD (C# Azure AI projects)
Learning repo:    confksq/Learning (EXISTS) → Project/AIML-Learn/PartsModules/CareerAccelerator/
LinkedIn:         linkedin.com/in/balakitt
Recruiter:        Mohan @ IDEXCEL (mmohan@idexcel.com)
Target roles:     Senior AI Engineer, GenAI Engineer, AI Agents Engineer
Target timeline:  Offer within 60 days
```

---

## EXISTING GITHUB CONVENTIONS (from confksq/AI-RandD)

Follow this exact pattern for all new modules:
```
RepoName/
└── PartsModules/
    └── NN-ModuleName/
        ├── README.md          ← Architecture + setup + usage
        ├── 01_concepts.md     ← Theory bridged from candidate's existing knowledge
        ├── 02_architecture.md ← ASCII diagram + component breakdown
        ├── 03_interview_qa.md ← 10-15 Senior-level Q&A
        ├── 04_hands_on.py     ← Working code with heavy explanatory comments
        │   (or .ipynb for ML/fine-tuning modules)
        ├── 05_resume_bullet.md ← Ready-to-copy resume bullet after completion
        └── requirements.txt
```

**Concept docs must always bridge from candidate's existing knowledge:**
- "You know Azure AI Search → FAISS is the local equivalent"
- "You know Semantic Kernel agents → crewAI is the Python-native equivalent"
- "You know Azure OpenAI endpoint → Ollama runs that endpoint locally"

---

## SKILL GAP SUMMARY (from PRD Gap Analysis)

### ✅ Already Strong (do NOT re-teach)
Azure AI Foundry, Azure OpenAI, RAG, Semantic Kernel, Azure AI Search,
Agentic AI, LLMOps, Azure AI Document Intelligence, AKS, C#/.NET, Python basics

### ❌ Gap Skills to Cover (8 modules below)
| Module | Skill | Market Frequency | PRD Phase |
|--------|-------|-----------------|-----------|
| L1 | Ollama + Local LLMs | 15% JDs | Phase 1 |
| L2 | crewAI Multi-Agent | 20% JDs | Phase 1 |
| L3 | RAGAS Evaluation | 30% JDs | Phase 1 |
| L4 | Hugging Face Transformers | 35% JDs | Phase 2 |
| L5 | LlamaIndex RAG | 25% JDs | Phase 2 |
| L6 | Amazon Bedrock | 20% JDs | Phase 2 |
| L7 | GraphRAG + Neo4j | 15% JDs, growing | Phase 2 |
| L8 | LoRA / QLoRA Fine-Tuning | 20% JDs | Phase 3 |

---

## FEATURE L1: Learning Module — Ollama + Local LLMs

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/01-Ollama-LocalRAG/`  
**PRD Phase:** 1 — Complete Week 1  
**Time to complete:** 1 day  
**Market need:** KFORCE-type roles, air-gapped/regulated environments

### CLI Execution Command
```bash
claude "Execute PRD Feature L1: Create all files for PartsModules/01-Ollama-LocalRAG/ 
in confksq/Learning repo. Create repo if it doesn't exist. Follow the file structure 
and conventions defined in the PRD System Context."
```

### Files to Create
```
PartsModules/01-Ollama-LocalRAG/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04_hands_on.py
├── 05_resume_bullet.md
└── requirements.txt
```

### Content Requirements

**README.md:**
- What this module covers and why it matters for job search
- Prerequisites (Ollama install: `curl -fsSL https://ollama.com/install.sh | sh`)
- Quick start (3 commands to get running)
- Link to hands-on file

**01_concepts.md — Bridge from candidate's existing knowledge:**
- Azure OpenAI endpoint → Ollama is the local equivalent (same API shape)
- Azure AI Search → FAISS/Chroma are local vector store equivalents
- Why companies use local LLMs: compliance, cost, air-gapped environments
- Supported models: LLaMA 3, Mistral, Phi-3, Gemma
- Ollama REST API structure (mirrors OpenAI API)
- When to choose Ollama vs Azure OpenAI (decision table)

**02_architecture.md:**
ASCII diagram showing:
```
[User Query] → [Python App] → [Ollama Server :11434] → [LLaMA3/Mistral]
                     ↓
              [FAISS Index] ← [Document Chunks] ← [Ingest Pipeline]
```
Component breakdown: Ollama server, model files, FAISS index, embedding model

**03_interview_qa.md — 15 Q&A at Senior level:**
Topics: local vs cloud tradeoffs, model serving, quantization basics,
FAISS index types, chunking strategies, when to recommend local AI

**04_hands_on.py — Single file end-to-end RAG:**
```python
# Sections (heavily commented):
# 1. Connect to Ollama (show it mirrors OpenAI API)
# 2. Load embedding model (HuggingFace sentence-transformers)
# 3. Ingest sample documents → chunk → embed → FAISS index
# 4. Query: embed question → FAISS search → Ollama LLM → answer
# 5. Print answer with source references
```
Requirements: ollama, faiss-cpu, sentence-transformers, langchain

**05_resume_bullet.md:**
```
Built local LLM RAG pipeline using Ollama (LLaMA 3) and FAISS — 
demonstrating open-source AI deployment for air-gapped/regulated environments.
```

### Acceptance Criteria
- [ ] All 7 files created and pushed to confksq/Learning
- [ ] 01_concepts.md explicitly bridges from Azure OpenAI to Ollama
- [ ] 04_hands_on.py runs end-to-end with Ollama installed
- [ ] 15 interview Q&A in 03_interview_qa.md
- [ ] No paid API key required anywhere

---

## FEATURE L2: Learning Module — crewAI Multi-Agent

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/02-crewAI-MultiAgent/`  
**PRD Phase:** 1 — Complete Week 1  
**Time to complete:** 1 day  
**Market need:** Azure AI Foundry JDs explicitly mention crewAI alongside Semantic Kernel

### CLI Execution Command
```bash
claude "Execute PRD Feature L2: Create all files for PartsModules/02-crewAI-MultiAgent/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/02-crewAI-MultiAgent/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04_hands_on.py
├── 05_resume_bullet.md
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**
- Semantic Kernel agents → crewAI is the Python-native equivalent
- crewAI concepts: Agent (role + goal + backstory), Task, Crew, Process
- Sequential vs Hierarchical process (like SK orchestration patterns)
- When to use crewAI vs Semantic Kernel vs LangGraph (decision table)
- crewAI vs AutoGen vs MAF comparison

**02_architecture.md:**
ASCII diagram showing 3-agent pipeline:
```
[User Input] → [Crew Orchestrator]
                    ↓
            [Researcher Agent] → [Task: Research topic]
                    ↓
            [Writer Agent]     → [Task: Write report]
                    ↓
            [Reviewer Agent]   → [Task: Review & finalize]
                    ↓
            [Output: report.md]
```

**03_interview_qa.md — 15 Q&A:**
Topics: agent roles/goals/backstory, task dependencies, process types,
tool integration, memory in crewAI, crewAI vs SK vs LangGraph

**04_hands_on.py — 3-agent research pipeline:**
```python
# Agent 1: Researcher — given topic, produces structured findings
# Agent 2: Writer — takes findings, produces formatted report  
# Agent 3: Reviewer — validates accuracy, produces final output
# Configurable topic via: python 04_hands_on.py --topic "Azure AI Foundry"
# Backend: OpenAI (default) or Ollama (toggle in config)
```

**05_resume_bullet.md:**
```
Built multi-agent orchestration workflows using crewAI — 
demonstrating Python-native agentic AI patterns alongside Semantic Kernel.
```

### Acceptance Criteria
- [ ] 3 distinct agents with different roles
- [ ] 01_concepts.md has crewAI vs Semantic Kernel comparison table
- [ ] CLI topic argument works: `python 04_hands_on.py --topic "..."`
- [ ] Works with both OpenAI and Ollama backends

---

## FEATURE L3: Learning Module — RAGAS Evaluation

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/03-RAGAS-Evaluation/`  
**PRD Phase:** 1 — Complete Week 1  
**Time to complete:** 1 day  
**Market need:** 30% of JDs. Azure AI Foundry JD explicitly mentions RAGAS and TruLens

### CLI Execution Command
```bash
claude "Execute PRD Feature L3: Create all files for PartsModules/03-RAGAS-Evaluation/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/03-RAGAS-Evaluation/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04_hands_on.py
├── 05_resume_bullet.md
├── sample_questions.json
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**
- Candidate already does A/B testing and groundedness checks at JMA
- RAGAS formalizes this into measurable metrics
- 4 core metrics explained in plain English:
  - **Faithfulness** — Does the answer stick to retrieved context? (hallucination check)
  - **Answer Relevance** — Is the answer relevant to the question?
  - **Context Recall** — Did retrieval find the right chunks?
  - **Context Precision** — Are retrieved chunks actually useful?
- Score interpretation: what is a good score vs bad score
- RAGAS vs TruLens vs Azure AI Evaluation comparison table

**02_architecture.md:**
```
[Questions + Ground Truth] → [RAG Pipeline] → [Answers + Contexts]
                                                        ↓
                                              [RAGAS Evaluator]
                                                        ↓
                              [Scores: Faithfulness / Relevance / Recall / Precision]
                                                        ↓
                                              [Evaluation Report]
```

**03_interview_qa.md — 15 Q&A:**
Topics: why evaluate RAG, each metric definition, score thresholds,
how to improve low scores, RAGAS vs manual evaluation, LLM-as-judge concept

**04_hands_on.py:**
```python
# Section 1: Build simple RAG pipeline (LangChain + FAISS + OpenAI)
# Section 2: Define test questions + ground truth answers
# Section 3: Run pipeline, collect answers + retrieved contexts
# Section 4: Run RAGAS evaluation → print score table
# Section 5: Interpret results, identify weakest metric
```

**sample_questions.json:** 10 realistic Q&A pairs about a sample document

**05_resume_bullet.md:**
```
Implemented RAGAS-based RAG evaluation framework — measuring faithfulness, 
answer relevance, context recall, and precision for production AI quality assurance.
```

### Acceptance Criteria
- [ ] All 4 RAGAS metrics demonstrated with real scores
- [ ] sample_questions.json has 10+ Q&A pairs
- [ ] 01_concepts.md explains each metric in plain English
- [ ] Report output is a formatted table with score + interpretation
- [ ] 15 interview Q&A covering all metrics

---

## FEATURE L4: Learning Module — Hugging Face Transformers

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/04-HuggingFace-Transformers/`  
**PRD Phase:** 2 — Complete Week 2  
**Time to complete:** 2 days  
**Market need:** 35% of JDs — highest frequency gap skill

### CLI Execution Command
```bash
claude "Execute PRD Feature L4: Create all files for PartsModules/04-HuggingFace-Transformers/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/04-HuggingFace-Transformers/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04a_text_generation.py
├── 04b_embeddings.py
├── 04c_classification.py
├── 04d_rag_with_hf.py
├── 05_resume_bullet.md
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**
- Azure OpenAI Service → Hugging Face Hub (same concept, open-source)
- Azure AI Document Intelligence → Hugging Face document models
- HuggingFace ecosystem: Hub, Transformers library, Datasets, PEFT, Inference API
- `pipeline()` API — the "one-liner" for common tasks
- Model Hub: how to find, load, and run any model
- HuggingFace vs Azure OpenAI: when to use each (decision table)
- Free tier: which models run locally vs need Inference API

**03_interview_qa.md — 15 Q&A:**
Topics: tokenizers, model loading, pipeline API, embeddings, 
model hub navigation, HuggingFace vs Azure OpenAI tradeoffs

**04a_text_generation.py:** Load GPT-2 / Mistral, generate text, explain temperature/top_p  
**04b_embeddings.py:** sentence-transformers, cosine similarity, semantic search demo  
**04c_classification.py:** Zero-shot classification with BART  
**04d_rag_with_hf.py:** Full RAG — HuggingFace embeddings + FAISS + local LLM (no paid API)

### Acceptance Criteria
- [ ] 4 demo scripts all runnable without paid API
- [ ] 01_concepts.md has HuggingFace vs Azure OpenAI comparison table
- [ ] 04d_rag_with_hf.py is complete end-to-end RAG
- [ ] 15 interview Q&A

---

## FEATURE L5: Learning Module — LlamaIndex RAG

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/05-LlamaIndex-RAG/`  
**PRD Phase:** 2 — Complete Week 2  
**Time to complete:** 1 day  
**Market need:** 25% of JDs. Frequently paired with HuggingFace in open-source AI roles

### CLI Execution Command
```bash
claude "Execute PRD Feature L5: Create all files for PartsModules/05-LlamaIndex-RAG/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/05-LlamaIndex-RAG/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04_hands_on.py
├── 05_resume_bullet.md
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**
- LangChain (known) → LlamaIndex is the alternative, data-centric approach
- LangChain vs LlamaIndex core difference: LangChain = general orchestration, LlamaIndex = RAG-specialized
- LlamaIndex key concepts: Documents, Nodes, Index, QueryEngine, Retrievers
- When to choose LlamaIndex vs LangChain (decision table with use cases)
- LlamaIndex with local models (Ollama backend)

**04_hands_on.py — Full RAG pipeline:**
- Load documents from folder
- Build VectorStoreIndex
- Query with QueryEngine
- Use Ollama as LLM backend (no API key)
- Show response with source node references

### Acceptance Criteria
- [ ] LangChain vs LlamaIndex comparison table in 01_concepts.md
- [ ] Works with Ollama (no paid API)
- [ ] Shows source references in query output
- [ ] 15 interview Q&A

---

## FEATURE L6: Learning Module — Amazon Bedrock

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/06-Amazon-Bedrock/`  
**PRD Phase:** 2 — Complete Week 3  
**Time to complete:** 2 days  
**Market need:** 20% of JDs. AI Agents roles specify Bedrock + Azure AI Foundry

### CLI Execution Command
```bash
claude "Execute PRD Feature L6: Create all files for PartsModules/06-Amazon-Bedrock/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/06-Amazon-Bedrock/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04_hands_on.py
├── 05_resume_bullet.md
├── azure_vs_bedrock_comparison.md
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**

| Azure AI Foundry | Amazon Bedrock | Notes |
|---|---|---|
| Azure OpenAI Service | Bedrock Model API | Model invocation |
| Azure AI Search | Bedrock Knowledge Bases | RAG vector store |
| Semantic Kernel / MAF | Bedrock Agents | Agent orchestration |
| Azure AI Foundry Hub | Bedrock | Platform layer |
| GPT-4o | Claude 3, Llama, Titan | Available models |

- Boto3 client setup for Bedrock
- Bedrock model IDs vs Azure deployment names
- Bedrock Knowledge Bases: create, ingest, query
- Bedrock Agents: action groups, Lambda integration

**azure_vs_bedrock_comparison.md:** 15-dimension comparison table

**04_hands_on.py:**
- Invoke Claude 3 Sonnet via Bedrock (boto3)
- Build RAG using Bedrock Knowledge Bases
- Compare same query: Bedrock vs Azure AI Foundry approach

### Acceptance Criteria
- [ ] azure_vs_bedrock_comparison.md has 15+ dimensions
- [ ] 01_concepts.md has full Azure ↔ Bedrock mapping table
- [ ] 04_hands_on.py uses boto3 (AWS free tier compatible)
- [ ] 15 interview Q&A covering multi-cloud AI tradeoffs

---

## FEATURE L7: Learning Module — GraphRAG + Neo4j

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/07-GraphRAG-Neo4j/`  
**PRD Phase:** 2 — Complete Week 4  
**Time to complete:** 2 days  
**Market need:** 15% of JDs, growing. "Lead AI Engineer – Knowledge Graphs & GenAI" role in your inbox

### CLI Execution Command
```bash
claude "Execute PRD Feature L7: Create all files for PartsModules/07-GraphRAG-Neo4j/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/07-GraphRAG-Neo4j/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04a_neo4j_basics.py
├── 04b_graph_rag.py
├── 04c_vector_vs_graph_comparison.py
├── 05_resume_bullet.md
├── docker-compose.yml
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**
- Azure AI Search (vector) → Neo4j (graph) — different retrieval paradigms
- Why graph matters: relationships between entities, multi-hop reasoning
- Knowledge Graph concepts: nodes, edges, properties, Cypher query language
- Microsoft GraphRAG: entity extraction → graph build → community summaries
- Vector RAG vs Graph RAG vs Hybrid: decision table with use cases
- When GraphRAG outperforms standard RAG (complex entity relationships)

**docker-compose.yml:** One-command Neo4j Community setup

**04a_neo4j_basics.py:** Connect to Neo4j, create nodes/edges, run Cypher queries  
**04b_graph_rag.py:** Extract entities from docs with LLM, store in Neo4j, graph-enhanced retrieval  
**04c_vector_vs_graph_comparison.py:** Same question → vector RAG answer vs graph RAG answer side by side

### Acceptance Criteria
- [ ] docker-compose.yml starts Neo4j in one command
- [ ] 01_concepts.md has Vector vs Graph vs Hybrid decision table
- [ ] 04c shows side-by-side comparison output
- [ ] 15 interview Q&A

---

## FEATURE L8: Learning Module — LoRA / QLoRA Fine-Tuning

**Target repo:** `confksq/Learning`  
**Target path:** `Project/AIML-Learn/PartsModules/CareerAccelerator/08-LoRA-FineTuning/`  
**PRD Phase:** 3 — Complete Week 5  
**Time to complete:** 3 days  
**Market need:** 20% of JDs. "AI/ML Engineer — Embedding Model Fine-Tuning" role in your inbox

### CLI Execution Command
```bash
claude "Execute PRD Feature L8: Create all files for PartsModules/08-LoRA-FineTuning/ 
in confksq/Learning repo. Follow PRD conventions."
```

### Files to Create
```
PartsModules/08-LoRA-FineTuning/
├── README.md
├── 01_concepts.md
├── 02_architecture.md
├── 03_interview_qa.md
├── 04_lora_finetune.ipynb
├── 05_resume_bullet.md
└── requirements.txt
```

### Content Requirements

**01_concepts.md — Bridge from candidate's existing knowledge:**
- Prompt engineering (known) → Fine-tuning: when prompting isn't enough
- Full fine-tuning vs LoRA vs QLoRA: memory and compute comparison table
- LoRA math intuition (no PhD required): rank decomposition in plain English
- QLoRA = LoRA + quantization: run on consumer GPU (RTX 3090 / Google Colab)
- PEFT library: the HuggingFace toolkit for LoRA/QLoRA
- Quantization formats: GGUF (Ollama), AWQ, EXL2 — what each means
- Fine-tuning vs RAG: decision framework (when to fine-tune vs RAG)
- Estimated costs: Google Colab free vs paid, AWS, Azure

**04_lora_finetune.ipynb — Google Colab compatible notebook:**
- Load base model (GPT-2 or TinyLlama — runs on free Colab)
- Apply LoRA adapters using PEFT
- Fine-tune on custom dataset (10-20 examples minimum)
- Compare: base model output vs fine-tuned output
- Save and load LoRA adapters
- Compute and display training loss curve

**03_interview_qa.md — 20 Q&A (more than other modules — high interview frequency):**
Topics: LoRA rank explained, QLoRA memory savings, GGUF vs AWQ, when to fine-tune,
PEFT library usage, adapter merging, catastrophic forgetting, fine-tuning vs RAG tradeoffs

### Acceptance Criteria
- [ ] Notebook runs on Google Colab free tier
- [ ] 01_concepts.md has fine-tuning vs RAG decision framework
- [ ] Shows base model vs fine-tuned output comparison
- [ ] 20 interview Q&A (highest count — critical topic)
- [ ] Quantization formats (GGUF/AWQ/EXL2) explained

---

## FEATURE 1: Resume Updates (Claude.ai)

**Priority:** 🔴 Critical — COMPLETED ✅  
**Status:** v10 generated — Bala_Azure_AI_Engineer_AI-102_AI-103.pdf/docx

**Changes applied in v10:**
- Python moved to first in Development & DevOps
- .NET 10 updated
- All JMA bullets Python-first
- All KPMG bullets Python-first
- Added: RAGAS, Responsible AI, MCP to Generative AI & LLMOps row
- Added: LlamaIndex, crewAI, Hugging Face, Amazon Bedrock, Ollama to AI Frameworks row
- Profile updated with open-source AI stack mention

---

## FEATURE 9: LinkedIn Outreach Templates (Claude.ai)

**Priority:** 🔴 Critical  
**Execute in:** Claude.ai chat

### Prompt to use:
```
Execute PRD Feature 9: Generate linkedin-outreach-templates.md with 10 templates:
1. Connection request → Technical Recruiter
2. Connection request → Hiring Manager (Azure AI role)
3. Connection request → Hiring Manager (GenAI role)
4. Follow-up after connection accepted
5. Follow-up after submitting application
6. Response to recruiter (interested)
7. Response to recruiter (not interested, stay warm)
8. LinkedIn post — RAG pipeline thought leadership
9. LinkedIn post — AI Agents thought leadership
10. LinkedIn post — Azure AI Foundry tip

Candidate: Senior Azure AI Engineer, AI-102 + AI-103, Tampa FL,
open to remote. Strong in Azure AI Foundry, RAG, Agentic AI.
Connection requests under 300 chars. Posts 150-300 words.
```

---

## FEATURE 10: Job Search Tracker App (Claude.ai)

**Priority:** 🟡 Medium  
**Execute in:** Claude.ai chat — generates React artifact

### Prompt to use:
```
Execute PRD Feature 10: Build a React job search tracker app with:
- Add/edit/delete job applications
- Status pipeline: Applied → Recruiter Screen → Technical → Final → Offer/Rejected
- Follow-up flag (red highlight if >48hrs no update)
- Skill gap notes per application (referencing PRD gap skills)
- Weekly stats: applications, response rate, interviews
- Navy/white color scheme matching resume
- Works entirely in browser, no backend
```

---

## FEATURE 11: JD Match Analyzer — Claude-Powered App (Claude.ai)

**Priority:** 🟡 Medium  
**Execute in:** Claude.ai chat — generates React artifact with Claude API

### Prompt to use:
```
Execute PRD Feature 11: Build a React app that uses the Claude API 
(claude-sonnet-4-6) to analyze job descriptions against Bala's profile.

Candidate profile for system prompt:
- 17+ years, Senior Azure AI Engineer, AI-102 + AI-103
- Strong: Azure AI Foundry, RAG, Azure OpenAI, Semantic Kernel, 
  Agentic AI, LLMOps, Python, C#
- Learning: Hugging Face, Ollama, LoRA, GCP Vertex AI, Bedrock, 
  crewAI, Knowledge Graphs, Databricks

Output per JD: match_score (0-100), matching_skills[], missing_skills[] 
with PRD phase that covers each, resume_tweaks[], linkedin_message, 
recommendation (Apply/Skip/Stretch), reasoning.

UI: Two-panel (JD paste left, analysis right), visual score bar,
color-coded skill lists, copy buttons.
```

---

## FEATURE 12: Interview Prep — 100 Questions (Claude.ai)

**Priority:** 🟡 Medium  
**Execute in:** Claude.ai chat

### Prompt to use:
```
Execute PRD Feature 12: Generate Interview_Prep_AI_Engineer_Complete.md
with 100 Senior AI Engineer interview questions and detailed answers.

Distribution:
- RAG Architecture & Design: 15 questions
- Azure AI Foundry & Azure OpenAI: 15 questions  
- AI Agents & Agentic AI: 15 questions
- LLMOps, Evaluation & Monitoring: 10 questions
- Vector Databases & Embeddings: 10 questions
- Prompt Engineering: 10 questions
- Open-Source LLMs & Hugging Face: 8 questions
- Fine-tuning (LoRA/QLoRA): 7 questions
- AI Safety & Responsible AI: 5 questions
- System Design — AI Systems: 5 questions

Per question format:
- Difficulty level
- Detailed answer (150-300 words)
- Key terms to mention
- Follow-up question they might ask
- Bala's real example from JMA or KPMG work
```

---

## EXECUTION SEQUENCE

```
TODAY:       Resume v10 ✅ — Submit to all active roles
             Follow up on Wipro/Berribot assessment (check spam)

WEEK 1:      CLI: Feature L1 — Ollama LocalRAG → push to Learning/CareerAccelerator
             CLI: Feature L2 — crewAI MultiAgent → push to Learning/CareerAccelerator
             CLI: Feature L3 — RAGAS Evaluation → push to Learning/CareerAccelerator
             Claude.ai: Feature 9 — LinkedIn templates

WEEK 2:      CLI: Feature L4 — HuggingFace Transformers
             CLI: Feature L5 — LlamaIndex RAG
             Claude.ai: Feature 11 — JD Match Analyzer app
             Claude.ai: Feature 10 — Job Search Tracker

WEEK 3:      CLI: Feature L6 — Amazon Bedrock
             Claude.ai: Feature 12 — 100 Interview Questions

WEEK 4:      CLI: Feature L7 — GraphRAG + Neo4j

WEEK 5:      CLI: Feature L8 — LoRA/QLoRA Fine-Tuning
```

---

## SUCCESS METRICS

| Metric | Target | Timeframe |
|--------|--------|-----------|
| Resume shortlists / week | 3-5 | Week 2 |
| Phone screens / month | 5-8 | Month 1 |
| Technical interviews / month | 2-3 | Month 2 |
| GitHub modules published | 8 | 5 weeks |
| LinkedIn response rate | 15-20% | Month 1 |
| Offer received | 1 | Month 2-3 |

---

## OPEN ITEMS — Future PRD Features

- Feature L9: GCP Vertex AI + Agent Development Kit — ✅ DONE (PartsModules/CareerAccelerator/09-Vertex-AI/)
- Feature L10: Databricks + Delta Lake pipeline
- Feature L11: PyTorch fundamentals notebook series
- Feature L12: Microsoft Copilot Studio agent demo
- Feature 13: AZ-305 study plan (Architect track)
- Feature 14: AWS AI Practitioner study plan

---

*PRD Version 2.0 | July 4, 2026*  
*Learning modules restructured for Claude Code CLI execution*  
*Resume v10 complete. Next: Run CLI Feature L1.*
