# Career Accelerator — Gap-Skill Portfolio

Hands-on modules closing the open-source / multi-cloud gap skills that Senior AI Engineer & GenAI JDs screen for. Each module **bridges from existing Azure AI expertise** (Azure OpenAI, AI Search, Semantic Kernel, RAG, LLMOps) to the equivalent open-source or other-vendor tool.

**Source PRD:** `../../../04_Career/PRD_Bala_AI_Career_Acceleration.md`

---

## Modules

| # | Module | Skill | Market freq | Status |
|---|---|---|---|---|
| 01 | [Ollama + Local LLMs](01-Ollama-LocalRAG/) | Local/open-source LLM RAG | 15% | ✅ |
| 02 | [crewAI Multi-Agent](02-crewAI-MultiAgent/) | Python-native multi-agent | 20% | ✅ |
| 03 | [RAGAS Evaluation](03-RAGAS-Evaluation/) | RAG quality metrics | 30% | ✅ |
| 04 | [HuggingFace Transformers](04-HuggingFace-Transformers/) | Open-source model ecosystem | 35% | ✅ |
| 05 | [LlamaIndex RAG](05-LlamaIndex-RAG/) | RAG-specialized framework | 25% | ✅ |
| 06 | [Amazon Bedrock](06-Amazon-Bedrock/) | Multi-cloud AI (AWS) | 20% | ✅ |
| 07 | [GraphRAG + Neo4j](07-GraphRAG-Neo4j/) | Knowledge graphs | 15% | ✅ |
| 08 | [LoRA / QLoRA Fine-Tuning](08-LoRA-FineTuning/) | Hands-on fine-tuning | 20% | ✅ |
| 09 | [GCP Vertex AI + ADK](09-Vertex-AI/) | Multi-cloud AI (GCP) — completes Azure+AWS+GCP | growing | ✅ |

---

## Each module contains
- `README.md` — what/why + quick start
- `01_concepts.md` — theory bridged from Azure knowledge
- `02_architecture.md` — ASCII diagram + component breakdown
- `03_interview_qa.md` — 15–20 senior-level Q&A
- `04_hands_on.py` (or `.ipynb`) — runnable, heavily-commented code
- `05_resume_bullet.md` — ready-to-paste resume bullet
- `requirements.txt`

## The bridge philosophy
Every module answers: *"You already do X in Azure — here's the open-source / other-vendor equivalent."* You're not learning concepts from scratch; you're proving portability of skills you already have. That's the exact signal a keyword screen + hiring manager are looking for.

---
*Core 8 modules (L1–L8) complete, plus L9 (GCP Vertex AI) — a full **Azure + AWS + GCP** multi-cloud story. ✅ Ready for GitHub showcase and the job search.*

**Multi-cloud note:** Module 06 (Bedrock) + Module 09 (Vertex AI) mean you can draw the same GenAI architecture — foundation model + managed RAG + agents + safety — on all three major clouds. Azure OpenAI ↔ Bedrock ↔ Gemini · AI Search ↔ Knowledge Bases ↔ Vertex AI Search · Semantic Kernel ↔ Bedrock Agents ↔ ADK.
