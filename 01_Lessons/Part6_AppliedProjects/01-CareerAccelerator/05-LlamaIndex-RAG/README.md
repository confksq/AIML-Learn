# 05 — LlamaIndex RAG

**Part of:** Career Accelerator portfolio · **PRD Feature L5** · **Phase 2 (Week 2)**
**Skill:** LlamaIndex — the RAG-specialized Python framework, and how it differs from LangChain.

---

## Why this module matters for the job search

~25% of Senior AI / GenAI JDs mention **LlamaIndex**, frequently paired with Hugging Face in open-source AI roles. You already know LangChain and RAG. LlamaIndex is the **data-centric, RAG-first** alternative — same concepts you use daily (documents → chunks → index → query → cited answer), a cleaner abstraction purpose-built for retrieval. This module proves you're fluent in the RAG framework recruiters name alongside LangChain.

---

## What you'll have after this module
- A working LlamaIndex RAG pipeline (`04_hands_on.py`) with **source-node citations**
- Runs against a **local Ollama backend** — no paid API required
- 15 senior-level interview Q&A, including the LangChain vs LlamaIndex decision

---

## Prerequisites
```bash
pip install -r requirements.txt
# Local backend (free):
ollama serve && ollama pull llama3
```

---

## Quick start
```bash
pip install -r requirements.txt
python 04_hands_on.py
```
The script loads documents from the `data/` folder (auto-created with a sample on first run), builds a `VectorStoreIndex`, and answers a question with a `QueryEngine` — printing the answer **plus the source nodes** it used.

---

## Files
| File | What it is |
|---|---|
| `01_concepts.md` | LlamaIndex concepts + LangChain vs LlamaIndex, bridged from your RAG knowledge |
| `02_architecture.md` | Documents → Nodes → Index → QueryEngine flow |
| `03_interview_qa.md` | 15 senior-level interview Q&A |
| `04_hands_on.py` | Full LlamaIndex RAG with Ollama backend + source citations |
| `05_resume_bullet.md` | Ready-to-paste resume bullet |
| `requirements.txt` | Python dependencies |

---
*Bridge: LangChain (known) → LlamaIndex (RAG-specialized) · Documents/Nodes → chunks · VectorStoreIndex → Azure AI Search index · QueryEngine → your RAG orchestrator*
